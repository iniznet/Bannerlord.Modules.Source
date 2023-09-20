using System;
using System.Collections.Generic;
using System.Linq;
using NetworkMessages.FromClient;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200028F RID: 655
	public abstract class MissionLobbyComponent : MissionNetwork
	{
		// Token: 0x14000039 RID: 57
		// (add) Token: 0x0600229D RID: 8861 RVA: 0x0007DD40 File Offset: 0x0007BF40
		// (remove) Token: 0x0600229E RID: 8862 RVA: 0x0007DD78 File Offset: 0x0007BF78
		public event Action OnPostMatchEnded;

		// Token: 0x1400003A RID: 58
		// (add) Token: 0x0600229F RID: 8863 RVA: 0x0007DDB0 File Offset: 0x0007BFB0
		// (remove) Token: 0x060022A0 RID: 8864 RVA: 0x0007DDE8 File Offset: 0x0007BFE8
		public event Action OnCultureSelectionRequested;

		// Token: 0x17000693 RID: 1683
		// (get) Token: 0x060022A1 RID: 8865 RVA: 0x0007DE1D File Offset: 0x0007C01D
		public bool IsInWarmup
		{
			get
			{
				return this._warmupComponent != null && this._warmupComponent.IsInWarmup;
			}
		}

		// Token: 0x060022A2 RID: 8866 RVA: 0x0007DE34 File Offset: 0x0007C034
		static MissionLobbyComponent()
		{
			MissionLobbyComponent.AddLobbyComponentType(typeof(MissionBattleSchedulerClientComponent), LobbyMissionType.Matchmaker, false);
			MissionLobbyComponent.AddLobbyComponentType(typeof(MissionCustomGameClientComponent), LobbyMissionType.Custom, false);
		}

		// Token: 0x060022A3 RID: 8867 RVA: 0x0007DE81 File Offset: 0x0007C081
		public static void AddLobbyComponentType(Type type, LobbyMissionType missionType, bool isSeverComponent)
		{
			MissionLobbyComponent._lobbyComponentTypes.Add(new Tuple<LobbyMissionType, bool>(missionType, isSeverComponent), type);
		}

		// Token: 0x060022A4 RID: 8868 RVA: 0x0007DE98 File Offset: 0x0007C098
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this.CurrentMultiplayerState = MissionLobbyComponent.MultiplayerGameState.WaitingFirstPlayers;
			if (GameNetwork.IsServerOrRecorder)
			{
				MissionMultiplayerGameModeBase missionBehavior = Mission.Current.GetMissionBehavior<MissionMultiplayerGameModeBase>();
				if (missionBehavior != null && !missionBehavior.AllowCustomPlayerBanners())
				{
					this._usingFixedBanners = true;
					return;
				}
			}
			else
			{
				this._inactivityTimer = new Timer(base.Mission.CurrentTime, MissionLobbyComponent.InactivityThreshold, true);
			}
		}

		// Token: 0x060022A5 RID: 8869 RVA: 0x0007DEF4 File Offset: 0x0007C0F4
		protected override void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
		{
			if (GameNetwork.IsClient)
			{
				registerer.Register<KillDeathCountChange>(new GameNetworkMessage.ServerMessageHandlerDelegate<KillDeathCountChange>(this.HandleServerEventKillDeathCountChangeEvent));
				registerer.Register<MissionStateChange>(new GameNetworkMessage.ServerMessageHandlerDelegate<MissionStateChange>(this.HandleServerEventMissionStateChange));
				registerer.Register<NetworkMessages.FromServer.CreateBanner>(new GameNetworkMessage.ServerMessageHandlerDelegate<NetworkMessages.FromServer.CreateBanner>(this.HandleServerEventCreateBannerForPeer));
				registerer.Register<ChangeCulture>(new GameNetworkMessage.ServerMessageHandlerDelegate<ChangeCulture>(this.HandleServerEventChangeCulture));
				return;
			}
			if (GameNetwork.IsClientOrReplay)
			{
				registerer.Register<ChangeCulture>(new GameNetworkMessage.ServerMessageHandlerDelegate<ChangeCulture>(this.HandleServerEventChangeCulture));
				return;
			}
			if (GameNetwork.IsServer)
			{
				registerer.Register<NetworkMessages.FromClient.CreateBanner>(new GameNetworkMessage.ClientMessageHandlerDelegate<NetworkMessages.FromClient.CreateBanner>(this.HandleClientEventCreateBannerForPeer));
				registerer.Register<RequestCultureChange>(new GameNetworkMessage.ClientMessageHandlerDelegate<RequestCultureChange>(this.HandleClientEventRequestCultureChange));
				registerer.Register<RequestChangeCharacterMessage>(new GameNetworkMessage.ClientMessageHandlerDelegate<RequestChangeCharacterMessage>(this.HandleClientEventRequestChangeCharacterMessage));
			}
		}

		// Token: 0x060022A6 RID: 8870 RVA: 0x0007DFA8 File Offset: 0x0007C1A8
		protected override void OnUdpNetworkHandlerClose()
		{
			if (GameNetwork.IsServerOrRecorder || this._usingFixedBanners)
			{
				this._usingFixedBanners = false;
			}
		}

		// Token: 0x060022A7 RID: 8871 RVA: 0x0007DFC0 File Offset: 0x0007C1C0
		public static MissionLobbyComponent CreateBehavior()
		{
			return (MissionLobbyComponent)Activator.CreateInstance(MissionLobbyComponent._lobbyComponentTypes[new Tuple<LobbyMissionType, bool>(BannerlordNetwork.LobbyMissionType, GameNetwork.IsDedicatedServer)]);
		}

		// Token: 0x060022A8 RID: 8872 RVA: 0x0007DFE5 File Offset: 0x0007C1E5
		public virtual void QuitMission()
		{
		}

		// Token: 0x060022A9 RID: 8873 RVA: 0x0007DFE8 File Offset: 0x0007C1E8
		public override void AfterStart()
		{
			base.Mission.MakeDefaultDeploymentPlans();
			this._missionScoreboardComponent = base.Mission.GetMissionBehavior<MissionScoreboardComponent>();
			this._gameMode = base.Mission.GetMissionBehavior<MissionMultiplayerGameModeBase>();
			this._timerComponent = base.Mission.GetMissionBehavior<MultiplayerTimerComponent>();
			this._roundComponent = base.Mission.GetMissionBehavior<IRoundComponent>();
			this._warmupComponent = base.Mission.GetMissionBehavior<MultiplayerWarmupComponent>();
			if (GameNetwork.IsClient)
			{
				base.Mission.GetMissionBehavior<MissionNetworkComponent>().OnMyClientSynchronized += this.OnMyClientSynchronized;
			}
		}

		// Token: 0x060022AA RID: 8874 RVA: 0x0007E078 File Offset: 0x0007C278
		private void OnMyClientSynchronized()
		{
			base.Mission.GetMissionBehavior<MissionNetworkComponent>().OnMyClientSynchronized -= this.OnMyClientSynchronized;
			MissionPeer component = GameNetwork.MyPeer.GetComponent<MissionPeer>();
			if (component != null && component.Culture == null)
			{
				this.RequestCultureSelection();
			}
		}

		// Token: 0x060022AB RID: 8875 RVA: 0x0007E0C0 File Offset: 0x0007C2C0
		public override void EarlyStart()
		{
			if (GameNetwork.IsServer)
			{
				base.Mission.SpectatorTeam = base.Mission.Teams.Add(BattleSideEnum.None, uint.MaxValue, uint.MaxValue, null, true, false, true);
			}
		}

		// Token: 0x060022AC RID: 8876 RVA: 0x0007E0F8 File Offset: 0x0007C2F8
		public override void OnMissionTick(float dt)
		{
			if (GameNetwork.IsClient && this._inactivityTimer.Check(base.Mission.CurrentTime))
			{
				NetworkMain.GameClient.IsInCriticalState = MBAPI.IMBNetwork.ElapsedTimeSinceLastUdpPacketArrived() > (double)MissionLobbyComponent.InactivityThreshold;
			}
			if (this.CurrentMultiplayerState == MissionLobbyComponent.MultiplayerGameState.WaitingFirstPlayers)
			{
				if (GameNetwork.IsServer && (this._warmupComponent == null || (!this._warmupComponent.IsInWarmup && this._timerComponent.CheckIfTimerPassed())))
				{
					int num = GameNetwork.NetworkPeers.Count((NetworkCommunicator x) => x.IsSynchronized);
					int num2 = MultiplayerOptions.OptionType.NumberOfBotsTeam1.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) + MultiplayerOptions.OptionType.NumberOfBotsTeam2.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
					int intValue = MultiplayerOptions.OptionType.MinNumberOfPlayersForMatchStart.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
					if (num + num2 >= intValue || MBCommon.CurrentGameType == MBCommon.GameType.MultiClientServer)
					{
						this.SetStatePlayingAsServer();
						return;
					}
				}
			}
			else if (this.CurrentMultiplayerState == MissionLobbyComponent.MultiplayerGameState.Playing)
			{
				bool flag = this._timerComponent.CheckIfTimerPassed();
				if (GameNetwork.IsServerOrRecorder && this._gameMode.RoundController == null && (flag || this._gameMode.CheckForMatchEnd()))
				{
					this._gameMode.GetWinnerTeam();
					this._gameMode.SpawnComponent.SpawningBehavior.RequestStopSpawnSession();
					this._gameMode.SpawnComponent.SpawningBehavior.SetRemainingAgentsInvulnerable();
					this.SetStateEndingAsServer();
				}
			}
		}

		// Token: 0x060022AD RID: 8877 RVA: 0x0007E24A File Offset: 0x0007C44A
		protected override void OnUdpNetworkHandlerTick()
		{
			if (this.CurrentMultiplayerState == MissionLobbyComponent.MultiplayerGameState.Ending && this._timerComponent.CheckIfTimerPassed() && GameNetwork.IsServer)
			{
				this.EndGameAsServer();
			}
		}

		// Token: 0x060022AE RID: 8878 RVA: 0x0007E26F File Offset: 0x0007C46F
		public override void OnRemoveBehavior()
		{
			NetworkCommunicator myPeer = GameNetwork.MyPeer;
			this.QuitMission();
			base.OnRemoveBehavior();
		}

		// Token: 0x060022AF RID: 8879 RVA: 0x0007E284 File Offset: 0x0007C484
		private void HandleServerEventMissionStateChange(MissionStateChange message)
		{
			this.CurrentMultiplayerState = message.CurrentState;
			if (this.CurrentMultiplayerState != MissionLobbyComponent.MultiplayerGameState.WaitingFirstPlayers)
			{
				if (this.CurrentMultiplayerState == MissionLobbyComponent.MultiplayerGameState.Playing && this._warmupComponent != null)
				{
					base.Mission.RemoveMissionBehavior(this._warmupComponent);
					this._warmupComponent = null;
				}
				float num = ((this.CurrentMultiplayerState == MissionLobbyComponent.MultiplayerGameState.Playing) ? ((float)(MultiplayerOptions.OptionType.MapTimeLimit.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) * 60)) : MissionLobbyComponent.PostMatchWaitDuration);
				this._timerComponent.StartTimerAsClient(message.StateStartTimeInSeconds, num);
			}
			if (this.CurrentMultiplayerState == MissionLobbyComponent.MultiplayerGameState.Ending)
			{
				this.SetStateEndingAsClient();
			}
		}

		// Token: 0x060022B0 RID: 8880 RVA: 0x0007E30C File Offset: 0x0007C50C
		private void HandleServerEventKillDeathCountChangeEvent(KillDeathCountChange message)
		{
			if (message.VictimPeer != null)
			{
				MissionPeer component = message.VictimPeer.GetComponent<MissionPeer>();
				NetworkCommunicator attackerPeer = message.AttackerPeer;
				MissionPeer missionPeer = ((attackerPeer != null) ? attackerPeer.GetComponent<MissionPeer>() : null);
				if (component != null)
				{
					component.KillCount = message.KillCount;
					component.AssistCount = message.AssistCount;
					component.DeathCount = message.DeathCount;
					component.Score = message.Score;
					if (missionPeer != null)
					{
						missionPeer.OnKillAnotherPeer(component);
					}
					if (message.KillCount == 0 && message.AssistCount == 0 && message.DeathCount == 0 && message.Score == 0)
					{
						component.ResetKillRegistry();
					}
				}
				if (this._missionScoreboardComponent != null)
				{
					this._missionScoreboardComponent.PlayerPropertiesChanged(message.VictimPeer);
				}
			}
		}

		// Token: 0x060022B1 RID: 8881 RVA: 0x0007E3C0 File Offset: 0x0007C5C0
		private void HandleServerEventCreateBannerForPeer(NetworkMessages.FromServer.CreateBanner message)
		{
			MissionPeer component = message.Peer.GetComponent<MissionPeer>();
			if (component != null)
			{
				component.Peer.BannerCode = message.BannerCode;
			}
		}

		// Token: 0x060022B2 RID: 8882 RVA: 0x0007E3F0 File Offset: 0x0007C5F0
		private void HandleServerEventChangeCulture(ChangeCulture message)
		{
			MissionPeer component = message.Peer.GetComponent<MissionPeer>();
			if (component != null)
			{
				component.Culture = message.Culture;
			}
		}

		// Token: 0x060022B3 RID: 8883 RVA: 0x0007E418 File Offset: 0x0007C618
		private bool HandleClientEventRequestCultureChange(NetworkCommunicator peer, RequestCultureChange message)
		{
			MissionPeer component = peer.GetComponent<MissionPeer>();
			if (component != null && this._gameMode.CheckIfPlayerCanDespawn(component))
			{
				component.Culture = message.Culture;
				this.DespawnPlayer(component);
			}
			return true;
		}

		// Token: 0x060022B4 RID: 8884 RVA: 0x0007E454 File Offset: 0x0007C654
		private bool HandleClientEventCreateBannerForPeer(NetworkCommunicator peer, NetworkMessages.FromClient.CreateBanner message)
		{
			MissionMultiplayerGameModeBase missionBehavior = Mission.Current.GetMissionBehavior<MissionMultiplayerGameModeBase>();
			if (missionBehavior == null || !missionBehavior.AllowCustomPlayerBanners())
			{
				return false;
			}
			MissionPeer component = peer.GetComponent<MissionPeer>();
			if (component == null)
			{
				return false;
			}
			component.Peer.BannerCode = message.BannerCode;
			MissionLobbyComponent.SyncBannersToAllClients(message.BannerCode, component.GetNetworkPeer());
			return true;
		}

		// Token: 0x060022B5 RID: 8885 RVA: 0x0007E4A8 File Offset: 0x0007C6A8
		private bool HandleClientEventRequestChangeCharacterMessage(NetworkCommunicator peer, RequestChangeCharacterMessage message)
		{
			MissionPeer component = message.NetworkPeer.GetComponent<MissionPeer>();
			if (component != null && this._gameMode.CheckIfPlayerCanDespawn(component))
			{
				this.DespawnPlayer(component);
			}
			return true;
		}

		// Token: 0x060022B6 RID: 8886 RVA: 0x0007E4DA File Offset: 0x0007C6DA
		private static void SyncBannersToAllClients(string bannerCode, NetworkCommunicator ownerPeer)
		{
			GameNetwork.BeginBroadcastModuleEvent();
			GameNetwork.WriteMessage(new NetworkMessages.FromServer.CreateBanner(ownerPeer, bannerCode));
			GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.ExcludeTargetPlayer, ownerPeer);
		}

		// Token: 0x060022B7 RID: 8887 RVA: 0x0007E4F4 File Offset: 0x0007C6F4
		protected override void HandleNewClientConnect(PlayerConnectionInfo clientConnectionInfo)
		{
			base.HandleNewClientConnect(clientConnectionInfo);
		}

		// Token: 0x060022B8 RID: 8888 RVA: 0x0007E4FD File Offset: 0x0007C6FD
		protected override void HandleLateNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
		{
			if (!networkPeer.IsServerPeer)
			{
				this.SendExistingObjectsToPeer(networkPeer);
			}
		}

		// Token: 0x060022B9 RID: 8889 RVA: 0x0007E510 File Offset: 0x0007C710
		private void SendExistingObjectsToPeer(NetworkCommunicator peer)
		{
			long num = 0L;
			if (this.CurrentMultiplayerState != MissionLobbyComponent.MultiplayerGameState.WaitingFirstPlayers)
			{
				num = this._timerComponent.GetCurrentTimerStartTime().NumberOfTicks;
			}
			GameNetwork.BeginModuleEventAsServer(peer);
			GameNetwork.WriteMessage(new MissionStateChange(this.CurrentMultiplayerState, num));
			GameNetwork.EndModuleEventAsServer();
			this.SendPeerInformationsToPeer(peer);
		}

		// Token: 0x060022BA RID: 8890 RVA: 0x0007E560 File Offset: 0x0007C760
		private void SendPeerInformationsToPeer(NetworkCommunicator peer)
		{
			foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeersIncludingDisconnectedPeers)
			{
				bool flag = networkCommunicator.VirtualPlayer != MBNetwork.VirtualPlayers[networkCommunicator.VirtualPlayer.Index];
				if (flag || networkCommunicator.IsSynchronized || networkCommunicator.JustReconnecting)
				{
					MissionPeer component = networkCommunicator.GetComponent<MissionPeer>();
					GameNetwork.BeginModuleEventAsServer(peer);
					GameNetwork.WriteMessage(new KillDeathCountChange(component.GetNetworkPeer(), null, component.KillCount, component.AssistCount, component.DeathCount, component.Score));
					GameNetwork.EndModuleEventAsServer();
					if (component.BotsUnderControlAlive != 0 || component.BotsUnderControlTotal != 0)
					{
						GameNetwork.BeginModuleEventAsServer(peer);
						GameNetwork.WriteMessage(new BotsControlledChange(component.GetNetworkPeer(), component.BotsUnderControlAlive, component.BotsUnderControlTotal));
						GameNetwork.EndModuleEventAsServer();
					}
				}
				else
				{
					Debug.Print(string.Concat(new string[] { ">#< Can't send the info of ", networkCommunicator.UserName, " to ", peer.UserName, "." }), 0, Debug.DebugColor.BrightWhite, 17179869184UL);
					Debug.Print(string.Format("isDisconnectedPeer: {0}", flag), 0, Debug.DebugColor.BrightWhite, 17179869184UL);
					Debug.Print(string.Format("networkPeer.IsSynchronized: {0}", networkCommunicator.IsSynchronized), 0, Debug.DebugColor.BrightWhite, 17179869184UL);
					Debug.Print(string.Format("peer == networkPeer: {0}", peer == networkCommunicator), 0, Debug.DebugColor.BrightWhite, 17179869184UL);
					Debug.Print(string.Format("networkPeer.JustReconnecting: {0}", networkCommunicator.JustReconnecting), 0, Debug.DebugColor.BrightWhite, 17179869184UL);
				}
			}
		}

		// Token: 0x060022BB RID: 8891 RVA: 0x0007E738 File Offset: 0x0007C938
		public void DespawnPlayer(MissionPeer missionPeer)
		{
			if (missionPeer.ControlledAgent != null && missionPeer.ControlledAgent.IsActive())
			{
				Agent controlledAgent = missionPeer.ControlledAgent;
				if (controlledAgent == null)
				{
					return;
				}
				controlledAgent.FadeOut(true, true);
			}
		}

		// Token: 0x060022BC RID: 8892 RVA: 0x0007E761 File Offset: 0x0007C961
		public override void OnScoreHit(Agent affectedAgent, Agent affectorAgent, WeaponComponentData attackerWeapon, bool isBlocked, bool isSiegeEngineHit, in Blow blow, in AttackCollisionData collisionData, float damagedHp, float hitDistance, float shotDifficulty)
		{
			if (GameNetwork.IsServer && !isBlocked && affectorAgent != affectedAgent && affectorAgent.MissionPeer != null && damagedHp > 0f)
			{
				affectedAgent.AddHitter(affectorAgent.MissionPeer, damagedHp, affectorAgent.IsFriendOf(affectedAgent));
			}
		}

		// Token: 0x060022BD RID: 8893 RVA: 0x0007E798 File Offset: 0x0007C998
		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
		{
			base.OnAgentRemoved(affectedAgent, affectorAgent, agentState, killingBlow);
			if (GameNetwork.IsServer)
			{
				if (this.CurrentMultiplayerState == MissionLobbyComponent.MultiplayerGameState.Ending)
				{
					return;
				}
				if ((agentState == AgentState.Killed || agentState == AgentState.Unconscious || agentState == AgentState.Routed) && affectedAgent != null && affectedAgent.IsHuman)
				{
					MissionPeer missionPeer = ((affectorAgent != null) ? affectorAgent.MissionPeer : null) ?? ((affectorAgent != null) ? affectorAgent.OwningAgentMissionPeer : null);
					MissionPeer missionPeer2 = this.RemoveHittersAndGetAssistorPeer((affectorAgent != null) ? affectorAgent.MissionPeer : null, affectedAgent);
					if (affectedAgent.MissionPeer != null)
					{
						this.OnPlayerDies(affectedAgent.MissionPeer, missionPeer, missionPeer2);
					}
					else
					{
						this.OnBotDies(affectedAgent, missionPeer, missionPeer2);
					}
					if (affectorAgent != null && affectorAgent.IsHuman)
					{
						if (affectorAgent != affectedAgent)
						{
							if (affectorAgent.MissionPeer != null)
							{
								this.OnPlayerKills(affectorAgent.MissionPeer, affectedAgent, missionPeer2);
								return;
							}
							this.OnBotKills(affectorAgent, affectedAgent);
							return;
						}
						else if (affectorAgent.MissionPeer != null)
						{
							affectorAgent.MissionPeer.Score -= (int)((float)this._gameMode.GetScoreForKill(affectedAgent) * 1.5f);
							this._missionScoreboardComponent.PlayerPropertiesChanged(affectorAgent.MissionPeer.GetNetworkPeer());
							GameNetwork.BeginBroadcastModuleEvent();
							GameNetwork.WriteMessage(new KillDeathCountChange(affectorAgent.MissionPeer.GetNetworkPeer(), affectedAgent.MissionPeer.GetNetworkPeer(), affectorAgent.MissionPeer.KillCount, affectorAgent.MissionPeer.AssistCount, affectorAgent.MissionPeer.DeathCount, affectorAgent.MissionPeer.Score));
							GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
						}
					}
				}
			}
		}

		// Token: 0x060022BE RID: 8894 RVA: 0x0007E90C File Offset: 0x0007CB0C
		public override void OnAgentBuild(Agent agent, Banner banner)
		{
			if (GameNetwork.IsServer)
			{
				if (agent.IsMount)
				{
					return;
				}
				if (agent.MissionPeer == null)
				{
					Formation formation = agent.Formation;
					if (((formation != null) ? formation.PlayerOwner : null) != null)
					{
						MissionPeer missionPeer = agent.Formation.PlayerOwner.MissionPeer;
						if (missionPeer != null)
						{
							MissionPeer missionPeer2 = missionPeer;
							int num = missionPeer2.BotsUnderControlAlive;
							missionPeer2.BotsUnderControlAlive = num + 1;
							MissionPeer missionPeer3 = missionPeer;
							num = missionPeer3.BotsUnderControlTotal;
							missionPeer3.BotsUnderControlTotal = num + 1;
							return;
						}
					}
					else
					{
						this._missionScoreboardComponent.Sides[(int)agent.Team.Side].BotScores.AliveCount++;
					}
				}
			}
		}

		// Token: 0x060022BF RID: 8895 RVA: 0x0007E9A8 File Offset: 0x0007CBA8
		protected virtual void OnPlayerKills(MissionPeer killerPeer, Agent killedAgent, MissionPeer assistorPeer)
		{
			if (killedAgent.MissionPeer == null)
			{
				NetworkCommunicator networkCommunicator = GameNetwork.NetworkPeers.SingleOrDefault((NetworkCommunicator x) => x.GetComponent<MissionPeer>() != null && x.GetComponent<MissionPeer>().ControlledFormation != null && x.GetComponent<MissionPeer>().ControlledFormation == killedAgent.Formation);
				if (networkCommunicator != null)
				{
					MissionPeer component = networkCommunicator.GetComponent<MissionPeer>();
					killerPeer.OnKillAnotherPeer(component);
				}
			}
			else
			{
				killerPeer.OnKillAnotherPeer(killedAgent.MissionPeer);
			}
			if (killerPeer.Team.IsEnemyOf(killedAgent.Team))
			{
				killerPeer.Score += this._gameMode.GetScoreForKill(killedAgent);
				int num = killerPeer.KillCount;
				killerPeer.KillCount = num + 1;
			}
			else
			{
				killerPeer.Score -= (int)((float)this._gameMode.GetScoreForKill(killedAgent) * 1.5f);
				int num = killerPeer.KillCount;
				killerPeer.KillCount = num - 1;
			}
			this._missionScoreboardComponent.PlayerPropertiesChanged(killerPeer.GetNetworkPeer());
			GameNetwork.BeginBroadcastModuleEvent();
			GameNetwork.WriteMessage(new KillDeathCountChange(killerPeer.GetNetworkPeer(), null, killerPeer.KillCount, killerPeer.AssistCount, killerPeer.DeathCount, killerPeer.Score));
			GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
		}

		// Token: 0x060022C0 RID: 8896 RVA: 0x0007EAD0 File Offset: 0x0007CCD0
		protected virtual void OnPlayerDies(MissionPeer peer, MissionPeer affectorPeer, MissionPeer assistorPeer)
		{
			if (assistorPeer != null)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new KillDeathCountChange(assistorPeer.GetNetworkPeer(), null, assistorPeer.KillCount, assistorPeer.AssistCount, assistorPeer.DeathCount, assistorPeer.Score));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
			}
			int deathCount = peer.DeathCount;
			peer.DeathCount = deathCount + 1;
			peer.SpawnTimer.Reset(Mission.Current.CurrentTime, (float)MissionLobbyComponent.GetSpawnPeriodDurationForPeer(peer));
			peer.WantsToSpawnAsBot = false;
			peer.HasSpawnTimerExpired = false;
			this._missionScoreboardComponent.PlayerPropertiesChanged(peer.GetNetworkPeer());
			GameNetwork.BeginBroadcastModuleEvent();
			GameNetwork.WriteMessage(new KillDeathCountChange(peer.GetNetworkPeer(), (affectorPeer != null) ? affectorPeer.GetNetworkPeer() : null, peer.KillCount, peer.AssistCount, peer.DeathCount, peer.Score));
			GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
		}

		// Token: 0x060022C1 RID: 8897 RVA: 0x0007EBA0 File Offset: 0x0007CDA0
		protected virtual void OnBotKills(Agent botAgent, Agent killedAgent)
		{
			Agent botAgent2 = botAgent;
			if (((botAgent2 != null) ? botAgent2.Team : null) != null)
			{
				Formation formation = botAgent.Formation;
				if (((formation != null) ? formation.PlayerOwner : null) != null)
				{
					NetworkCommunicator networkCommunicator = GameNetwork.NetworkPeers.SingleOrDefault((NetworkCommunicator x) => x.GetComponent<MissionPeer>() != null && x.GetComponent<MissionPeer>().ControlledFormation == botAgent.Formation);
					if (networkCommunicator != null)
					{
						MissionPeer component = networkCommunicator.GetComponent<MissionPeer>();
						MissionPeer missionPeer = killedAgent.MissionPeer;
						NetworkCommunicator networkCommunicator2 = ((missionPeer != null) ? missionPeer.GetNetworkPeer() : null);
						if (killedAgent.MissionPeer == null)
						{
							NetworkCommunicator networkCommunicator3 = GameNetwork.NetworkPeers.SingleOrDefault((NetworkCommunicator x) => x.GetComponent<MissionPeer>() != null && x.GetComponent<MissionPeer>().ControlledFormation == killedAgent.Formation);
							if (networkCommunicator3 != null)
							{
								NetworkCommunicator networkCommunicator4 = networkCommunicator3;
								component.OnKillAnotherPeer(networkCommunicator4.GetComponent<MissionPeer>());
							}
						}
						else
						{
							component.OnKillAnotherPeer(killedAgent.MissionPeer);
						}
						if (botAgent.Team.IsEnemyOf(killedAgent.Team))
						{
							MissionPeer missionPeer2 = component;
							int num = missionPeer2.KillCount;
							missionPeer2.KillCount = num + 1;
							component.Score += this._gameMode.GetScoreForKill(killedAgent);
						}
						else
						{
							MissionPeer missionPeer3 = component;
							int num = missionPeer3.KillCount;
							missionPeer3.KillCount = num - 1;
							component.Score -= (int)((float)this._gameMode.GetScoreForKill(killedAgent) * 1.5f);
						}
						this._missionScoreboardComponent.PlayerPropertiesChanged(networkCommunicator);
						GameNetwork.BeginBroadcastModuleEvent();
						GameNetwork.WriteMessage(new KillDeathCountChange(networkCommunicator, null, component.KillCount, component.AssistCount, component.DeathCount, component.Score));
						GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
					}
				}
				else
				{
					MissionScoreboardComponent.MissionScoreboardSide sideSafe = this._missionScoreboardComponent.GetSideSafe(botAgent.Team.Side);
					BotData botScores = sideSafe.BotScores;
					if (botAgent.Team.IsEnemyOf(killedAgent.Team))
					{
						botScores.KillCount++;
					}
					else
					{
						botScores.KillCount--;
					}
					this._missionScoreboardComponent.BotPropertiesChanged(sideSafe.Side);
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new BotData(sideSafe.Side, botScores.KillCount, botScores.AssistCount, botScores.DeathCount, botScores.AliveCount));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
				}
				this._missionScoreboardComponent.BotPropertiesChanged(botAgent.Team.Side);
			}
		}

		// Token: 0x060022C2 RID: 8898 RVA: 0x0007EE0C File Offset: 0x0007D00C
		protected virtual void OnBotDies(Agent botAgent, MissionPeer affectorPeer, MissionPeer assistorPeer)
		{
			if (assistorPeer != null)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new KillDeathCountChange(assistorPeer.GetNetworkPeer(), (affectorPeer != null) ? affectorPeer.GetNetworkPeer() : null, assistorPeer.KillCount, assistorPeer.AssistCount, assistorPeer.DeathCount, assistorPeer.Score));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
			}
			if (botAgent != null)
			{
				Formation formation = botAgent.Formation;
				if (((formation != null) ? formation.PlayerOwner : null) != null)
				{
					NetworkCommunicator networkCommunicator = GameNetwork.NetworkPeers.SingleOrDefault((NetworkCommunicator x) => x.GetComponent<MissionPeer>() != null && x.GetComponent<MissionPeer>().ControlledFormation == botAgent.Formation);
					if (networkCommunicator != null)
					{
						MissionPeer component = networkCommunicator.GetComponent<MissionPeer>();
						MissionPeer missionPeer = component;
						int num = missionPeer.DeathCount;
						missionPeer.DeathCount = num + 1;
						MissionPeer missionPeer2 = component;
						num = missionPeer2.BotsUnderControlAlive;
						missionPeer2.BotsUnderControlAlive = num - 1;
						this._missionScoreboardComponent.PlayerPropertiesChanged(networkCommunicator);
						GameNetwork.BeginBroadcastModuleEvent();
						GameNetwork.WriteMessage(new KillDeathCountChange(networkCommunicator, (affectorPeer != null) ? affectorPeer.GetNetworkPeer() : null, component.KillCount, component.AssistCount, component.DeathCount, component.Score));
						GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
						GameNetwork.BeginBroadcastModuleEvent();
						GameNetwork.WriteMessage(new BotsControlledChange(networkCommunicator, component.BotsUnderControlAlive, component.BotsUnderControlTotal));
						GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
					}
				}
				else
				{
					MissionScoreboardComponent.MissionScoreboardSide sideSafe = this._missionScoreboardComponent.GetSideSafe(botAgent.Team.Side);
					BotData botScores = sideSafe.BotScores;
					botScores.DeathCount++;
					botScores.AliveCount--;
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new BotData(sideSafe.Side, botScores.KillCount, botScores.AssistCount, botScores.DeathCount, botScores.AliveCount));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
				}
				this._missionScoreboardComponent.BotPropertiesChanged(botAgent.Team.Side);
			}
		}

		// Token: 0x060022C3 RID: 8899 RVA: 0x0007EFD4 File Offset: 0x0007D1D4
		public override void OnClearScene()
		{
			foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeersIncludingDisconnectedPeers)
			{
				MissionPeer component = networkCommunicator.GetComponent<MissionPeer>();
				if (component != null)
				{
					component.BotsUnderControlAlive = 0;
					component.BotsUnderControlTotal = 0;
					component.ControlledFormation = null;
				}
			}
		}

		// Token: 0x060022C4 RID: 8900 RVA: 0x0007F038 File Offset: 0x0007D238
		public static int GetSpawnPeriodDurationForPeer(MissionPeer peer)
		{
			return Mission.Current.GetMissionBehavior<SpawnComponent>().GetMaximumReSpawnPeriodForPeer(peer);
		}

		// Token: 0x060022C5 RID: 8901 RVA: 0x0007F04C File Offset: 0x0007D24C
		public virtual void SetStateEndingAsServer()
		{
			this.CurrentMultiplayerState = MissionLobbyComponent.MultiplayerGameState.Ending;
			MBDebug.Print("Multiplayer game mission ending", 0, Debug.DebugColor.White, 17592186044416UL);
			this._timerComponent.StartTimerAsServer(MissionLobbyComponent.PostMatchWaitDuration);
			GameNetwork.BeginBroadcastModuleEvent();
			GameNetwork.WriteMessage(new MissionStateChange(this.CurrentMultiplayerState, this._timerComponent.GetCurrentTimerStartTime().NumberOfTicks));
			GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
			Action onPostMatchEnded = this.OnPostMatchEnded;
			if (onPostMatchEnded == null)
			{
				return;
			}
			onPostMatchEnded();
		}

		// Token: 0x060022C6 RID: 8902 RVA: 0x0007F0C8 File Offset: 0x0007D2C8
		private void SetStatePlayingAsServer()
		{
			this._warmupComponent = null;
			this.CurrentMultiplayerState = MissionLobbyComponent.MultiplayerGameState.Playing;
			this._timerComponent.StartTimerAsServer((float)(MultiplayerOptions.OptionType.MapTimeLimit.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) * 60));
			GameNetwork.BeginBroadcastModuleEvent();
			GameNetwork.WriteMessage(new MissionStateChange(this.CurrentMultiplayerState, this._timerComponent.GetCurrentTimerStartTime().NumberOfTicks));
			GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
		}

		// Token: 0x060022C7 RID: 8903 RVA: 0x0007F129 File Offset: 0x0007D329
		protected virtual void EndGameAsServer()
		{
		}

		// Token: 0x060022C8 RID: 8904 RVA: 0x0007F12C File Offset: 0x0007D32C
		private MissionPeer RemoveHittersAndGetAssistorPeer(MissionPeer killerPeer, Agent killedAgent)
		{
			Agent.Hitter assistingHitter = killedAgent.GetAssistingHitter(killerPeer);
			if (((assistingHitter != null) ? assistingHitter.HitterPeer : null) != null)
			{
				if (!assistingHitter.IsFriendlyHit)
				{
					MissionPeer hitterPeer = assistingHitter.HitterPeer;
					int num = hitterPeer.AssistCount;
					hitterPeer.AssistCount = num + 1;
				}
				else
				{
					MissionPeer hitterPeer2 = assistingHitter.HitterPeer;
					int num = hitterPeer2.AssistCount;
					hitterPeer2.AssistCount = num - 1;
				}
			}
			if (assistingHitter == null)
			{
				return null;
			}
			return assistingHitter.HitterPeer;
		}

		// Token: 0x060022C9 RID: 8905 RVA: 0x0007F18E File Offset: 0x0007D38E
		private void SetStateEndingAsClient()
		{
			Action onPostMatchEnded = this.OnPostMatchEnded;
			if (onPostMatchEnded == null)
			{
				return;
			}
			onPostMatchEnded();
		}

		// Token: 0x060022CA RID: 8906 RVA: 0x0007F1A0 File Offset: 0x0007D3A0
		public void RequestCultureSelection()
		{
			Action onCultureSelectionRequested = this.OnCultureSelectionRequested;
			if (onCultureSelectionRequested == null)
			{
				return;
			}
			onCultureSelectionRequested();
		}

		// Token: 0x060022CB RID: 8907 RVA: 0x0007F1B2 File Offset: 0x0007D3B2
		public void RequestTroopSelection()
		{
			if (GameNetwork.IsClient)
			{
				GameNetwork.BeginModuleEventAsClient();
				GameNetwork.WriteMessage(new RequestChangeCharacterMessage(GameNetwork.MyPeer));
				GameNetwork.EndModuleEventAsClient();
			}
		}

		// Token: 0x060022CC RID: 8908 RVA: 0x0007F1D4 File Offset: 0x0007D3D4
		public void OnCultureSelected(BasicCultureObject culture)
		{
			if (GameNetwork.IsClient)
			{
				GameNetwork.BeginModuleEventAsClient();
				GameNetwork.WriteMessage(new RequestCultureChange(culture));
				GameNetwork.EndModuleEventAsClient();
			}
		}

		// Token: 0x17000694 RID: 1684
		// (get) Token: 0x060022CD RID: 8909 RVA: 0x0007F1F2 File Offset: 0x0007D3F2
		// (set) Token: 0x060022CE RID: 8910 RVA: 0x0007F1FA File Offset: 0x0007D3FA
		public MissionLobbyComponent.MultiplayerGameType MissionType { get; set; }

		// Token: 0x17000695 RID: 1685
		// (get) Token: 0x060022CF RID: 8911 RVA: 0x0007F203 File Offset: 0x0007D403
		// (set) Token: 0x060022D0 RID: 8912 RVA: 0x0007F20B File Offset: 0x0007D40B
		public MissionLobbyComponent.MultiplayerGameState CurrentMultiplayerState
		{
			get
			{
				return this._currentMultiplayerState;
			}
			private set
			{
				if (this._currentMultiplayerState != value)
				{
					this._currentMultiplayerState = value;
					Action<MissionLobbyComponent.MultiplayerGameState> currentMultiplayerStateChanged = this.CurrentMultiplayerStateChanged;
					if (currentMultiplayerStateChanged == null)
					{
						return;
					}
					currentMultiplayerStateChanged(value);
				}
			}
		}

		// Token: 0x1400003B RID: 59
		// (add) Token: 0x060022D1 RID: 8913 RVA: 0x0007F230 File Offset: 0x0007D430
		// (remove) Token: 0x060022D2 RID: 8914 RVA: 0x0007F268 File Offset: 0x0007D468
		public event Action<MissionLobbyComponent.MultiplayerGameState> CurrentMultiplayerStateChanged;

		// Token: 0x060022D3 RID: 8915 RVA: 0x0007F29D File Offset: 0x0007D49D
		public int GetRandomFaceSeedForCharacter(BasicCharacterObject character, int addition = 0)
		{
			IRoundComponent roundComponent = this._roundComponent;
			return character.GetDefaultFaceSeed(addition + ((roundComponent != null) ? roundComponent.RoundCount : 0)) % 2000;
		}

		// Token: 0x060022D4 RID: 8916 RVA: 0x0007F2C0 File Offset: 0x0007D4C0
		[CommandLineFunctionality.CommandLineArgumentFunction("kill_player", "mp_host")]
		public static string MPHostChangeParam(List<string> strings)
		{
			if (Mission.Current == null)
			{
				return "kill_player can only be called within a mission.";
			}
			if (!GameNetwork.IsServer)
			{
				return "kill_player can only be called by the server.";
			}
			if (strings == null || strings.Count == 0)
			{
				return "usage: kill_player {UserName}.";
			}
			foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
			{
				if (networkCommunicator.UserName == strings[0] && networkCommunicator.ControlledAgent != null)
				{
					Mission.Current.KillAgentCheat(networkCommunicator.ControlledAgent);
					return "Success.";
				}
			}
			return "Could not find the player " + strings[0] + " or the agent.";
		}

		// Token: 0x04000CEA RID: 3306
		private static readonly float InactivityThreshold = 2f;

		// Token: 0x04000CEB RID: 3307
		public static readonly float PostMatchWaitDuration = 15f;

		// Token: 0x04000CEE RID: 3310
		private MissionScoreboardComponent _missionScoreboardComponent;

		// Token: 0x04000CEF RID: 3311
		private MissionMultiplayerGameModeBase _gameMode;

		// Token: 0x04000CF0 RID: 3312
		private MultiplayerTimerComponent _timerComponent;

		// Token: 0x04000CF1 RID: 3313
		private IRoundComponent _roundComponent;

		// Token: 0x04000CF2 RID: 3314
		private Timer _inactivityTimer;

		// Token: 0x04000CF3 RID: 3315
		private MultiplayerWarmupComponent _warmupComponent;

		// Token: 0x04000CF4 RID: 3316
		private static readonly Dictionary<Tuple<LobbyMissionType, bool>, Type> _lobbyComponentTypes = new Dictionary<Tuple<LobbyMissionType, bool>, Type>();

		// Token: 0x04000CF5 RID: 3317
		private bool _usingFixedBanners;

		// Token: 0x04000CF7 RID: 3319
		private MissionLobbyComponent.MultiplayerGameState _currentMultiplayerState;

		// Token: 0x02000595 RID: 1429
		public enum MultiplayerGameState
		{
			// Token: 0x04001DAD RID: 7597
			WaitingFirstPlayers,
			// Token: 0x04001DAE RID: 7598
			Playing,
			// Token: 0x04001DAF RID: 7599
			Ending
		}

		// Token: 0x02000596 RID: 1430
		public enum MultiplayerGameType
		{
			// Token: 0x04001DB1 RID: 7601
			FreeForAll,
			// Token: 0x04001DB2 RID: 7602
			TeamDeathmatch,
			// Token: 0x04001DB3 RID: 7603
			Duel,
			// Token: 0x04001DB4 RID: 7604
			Siege,
			// Token: 0x04001DB5 RID: 7605
			Battle,
			// Token: 0x04001DB6 RID: 7606
			Captain,
			// Token: 0x04001DB7 RID: 7607
			Skirmish
		}
	}
}
