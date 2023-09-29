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
	public abstract class MissionLobbyComponent : MissionNetwork
	{
		public event Action OnPostMatchEnded;

		public event Action OnCultureSelectionRequested;

		public bool IsInWarmup
		{
			get
			{
				return this._warmupComponent != null && this._warmupComponent.IsInWarmup;
			}
		}

		static MissionLobbyComponent()
		{
			MissionLobbyComponent.AddLobbyComponentType(typeof(MissionBattleSchedulerClientComponent), LobbyMissionType.Matchmaker, false);
			MissionLobbyComponent.AddLobbyComponentType(typeof(MissionCustomGameClientComponent), LobbyMissionType.Custom, false);
			MissionLobbyComponent.AddLobbyComponentType(typeof(MissionCommunityClientComponent), LobbyMissionType.Community, false);
		}

		public static void AddLobbyComponentType(Type type, LobbyMissionType missionType, bool isSeverComponent)
		{
			MissionLobbyComponent._lobbyComponentTypes.Add(new Tuple<LobbyMissionType, bool>(missionType, isSeverComponent), type);
		}

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

		protected override void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
		{
			if (GameNetwork.IsClient)
			{
				registerer.RegisterBaseHandler<KillDeathCountChange>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventKillDeathCountChangeEvent));
				registerer.RegisterBaseHandler<MissionStateChange>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventMissionStateChange));
				registerer.RegisterBaseHandler<NetworkMessages.FromServer.CreateBanner>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventCreateBannerForPeer));
				registerer.RegisterBaseHandler<ChangeCulture>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventChangeCulture));
				return;
			}
			if (GameNetwork.IsClientOrReplay)
			{
				registerer.RegisterBaseHandler<ChangeCulture>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventChangeCulture));
				return;
			}
			if (GameNetwork.IsServer)
			{
				registerer.RegisterBaseHandler<NetworkMessages.FromClient.CreateBanner>(new GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage>(this.HandleClientEventCreateBannerForPeer));
				registerer.RegisterBaseHandler<RequestCultureChange>(new GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage>(this.HandleClientEventRequestCultureChange));
				registerer.RegisterBaseHandler<RequestChangeCharacterMessage>(new GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage>(this.HandleClientEventRequestChangeCharacterMessage));
			}
		}

		protected override void OnUdpNetworkHandlerClose()
		{
			if (GameNetwork.IsServerOrRecorder || this._usingFixedBanners)
			{
				this._usingFixedBanners = false;
			}
		}

		public static MissionLobbyComponent CreateBehavior()
		{
			return (MissionLobbyComponent)Activator.CreateInstance(MissionLobbyComponent._lobbyComponentTypes[new Tuple<LobbyMissionType, bool>(BannerlordNetwork.LobbyMissionType, GameNetwork.IsDedicatedServer)]);
		}

		public virtual void QuitMission()
		{
		}

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

		private void OnMyClientSynchronized()
		{
			base.Mission.GetMissionBehavior<MissionNetworkComponent>().OnMyClientSynchronized -= this.OnMyClientSynchronized;
			MissionPeer component = GameNetwork.MyPeer.GetComponent<MissionPeer>();
			if (component != null && component.Culture == null)
			{
				this.RequestCultureSelection();
			}
		}

		public override void EarlyStart()
		{
			if (GameNetwork.IsServer)
			{
				base.Mission.SpectatorTeam = base.Mission.Teams.Add(BattleSideEnum.None, uint.MaxValue, uint.MaxValue, null, true, false, true);
			}
		}

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

		protected override void OnUdpNetworkHandlerTick()
		{
			if (this.CurrentMultiplayerState == MissionLobbyComponent.MultiplayerGameState.Ending && this._timerComponent.CheckIfTimerPassed() && GameNetwork.IsServer)
			{
				this.EndGameAsServer();
			}
		}

		public override void OnRemoveBehavior()
		{
			NetworkCommunicator myPeer = GameNetwork.MyPeer;
			this.QuitMission();
			base.OnRemoveBehavior();
		}

		private void HandleServerEventMissionStateChange(GameNetworkMessage baseMessage)
		{
			MissionStateChange missionStateChange = (MissionStateChange)baseMessage;
			this.CurrentMultiplayerState = missionStateChange.CurrentState;
			if (this.CurrentMultiplayerState != MissionLobbyComponent.MultiplayerGameState.WaitingFirstPlayers)
			{
				if (this.CurrentMultiplayerState == MissionLobbyComponent.MultiplayerGameState.Playing && this._warmupComponent != null)
				{
					base.Mission.RemoveMissionBehavior(this._warmupComponent);
					this._warmupComponent = null;
				}
				float num = ((this.CurrentMultiplayerState == MissionLobbyComponent.MultiplayerGameState.Playing) ? ((float)(MultiplayerOptions.OptionType.MapTimeLimit.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) * 60)) : MissionLobbyComponent.PostMatchWaitDuration);
				this._timerComponent.StartTimerAsClient(missionStateChange.StateStartTimeInSeconds, num);
			}
			if (this.CurrentMultiplayerState == MissionLobbyComponent.MultiplayerGameState.Ending)
			{
				this.SetStateEndingAsClient();
			}
		}

		private void HandleServerEventKillDeathCountChangeEvent(GameNetworkMessage baseMessage)
		{
			KillDeathCountChange killDeathCountChange = (KillDeathCountChange)baseMessage;
			if (killDeathCountChange.VictimPeer != null)
			{
				MissionPeer component = killDeathCountChange.VictimPeer.GetComponent<MissionPeer>();
				NetworkCommunicator attackerPeer = killDeathCountChange.AttackerPeer;
				MissionPeer missionPeer = ((attackerPeer != null) ? attackerPeer.GetComponent<MissionPeer>() : null);
				if (component != null)
				{
					component.KillCount = killDeathCountChange.KillCount;
					component.AssistCount = killDeathCountChange.AssistCount;
					component.DeathCount = killDeathCountChange.DeathCount;
					component.Score = killDeathCountChange.Score;
					if (missionPeer != null)
					{
						missionPeer.OnKillAnotherPeer(component);
					}
					if (killDeathCountChange.KillCount == 0 && killDeathCountChange.AssistCount == 0 && killDeathCountChange.DeathCount == 0 && killDeathCountChange.Score == 0)
					{
						component.ResetKillRegistry();
					}
				}
				if (this._missionScoreboardComponent != null)
				{
					this._missionScoreboardComponent.PlayerPropertiesChanged(killDeathCountChange.VictimPeer);
				}
			}
		}

		private void HandleServerEventCreateBannerForPeer(GameNetworkMessage baseMessage)
		{
			NetworkMessages.FromServer.CreateBanner createBanner = (NetworkMessages.FromServer.CreateBanner)baseMessage;
			MissionPeer component = createBanner.Peer.GetComponent<MissionPeer>();
			if (component != null)
			{
				component.Peer.BannerCode = createBanner.BannerCode;
			}
		}

		private void HandleServerEventChangeCulture(GameNetworkMessage baseMessage)
		{
			ChangeCulture changeCulture = (ChangeCulture)baseMessage;
			MissionPeer component = changeCulture.Peer.GetComponent<MissionPeer>();
			if (component != null)
			{
				component.Culture = changeCulture.Culture;
			}
		}

		private bool HandleClientEventRequestCultureChange(NetworkCommunicator peer, GameNetworkMessage baseMessage)
		{
			RequestCultureChange requestCultureChange = (RequestCultureChange)baseMessage;
			MissionPeer component = peer.GetComponent<MissionPeer>();
			if (component != null && this._gameMode.CheckIfPlayerCanDespawn(component))
			{
				component.Culture = requestCultureChange.Culture;
				this.DespawnPlayer(component);
			}
			return true;
		}

		private bool HandleClientEventCreateBannerForPeer(NetworkCommunicator peer, GameNetworkMessage baseMessage)
		{
			NetworkMessages.FromClient.CreateBanner createBanner = (NetworkMessages.FromClient.CreateBanner)baseMessage;
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
			component.Peer.BannerCode = createBanner.BannerCode;
			MissionLobbyComponent.SyncBannersToAllClients(createBanner.BannerCode, component.GetNetworkPeer());
			return true;
		}

		private bool HandleClientEventRequestChangeCharacterMessage(NetworkCommunicator peer, GameNetworkMessage baseMessage)
		{
			MissionPeer component = ((RequestChangeCharacterMessage)baseMessage).NetworkPeer.GetComponent<MissionPeer>();
			if (component != null && this._gameMode.CheckIfPlayerCanDespawn(component))
			{
				this.DespawnPlayer(component);
			}
			return true;
		}

		private static void SyncBannersToAllClients(string bannerCode, NetworkCommunicator ownerPeer)
		{
			GameNetwork.BeginBroadcastModuleEvent();
			GameNetwork.WriteMessage(new NetworkMessages.FromServer.CreateBanner(ownerPeer, bannerCode));
			GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.ExcludeTargetPlayer, ownerPeer);
		}

		protected override void HandleNewClientConnect(PlayerConnectionInfo clientConnectionInfo)
		{
			base.HandleNewClientConnect(clientConnectionInfo);
		}

		protected override void HandleLateNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
		{
			if (!networkPeer.IsServerPeer)
			{
				this.SendExistingObjectsToPeer(networkPeer);
			}
		}

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

		private void SendPeerInformationsToPeer(NetworkCommunicator peer)
		{
			foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeersIncludingDisconnectedPeers)
			{
				bool flag = networkCommunicator.VirtualPlayer != GameNetwork.VirtualPlayers[networkCommunicator.VirtualPlayer.Index];
				if (flag || networkCommunicator.IsSynchronized || networkCommunicator.JustReconnecting)
				{
					MissionPeer component = networkCommunicator.GetComponent<MissionPeer>();
					if (component != null)
					{
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
						Debug.Print(">#< SendPeerInformationsToPeer MissionPeer is null.", 0, Debug.DebugColor.BrightWhite, 17179869184UL);
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

		public override void OnScoreHit(Agent affectedAgent, Agent affectorAgent, WeaponComponentData attackerWeapon, bool isBlocked, bool isSiegeEngineHit, in Blow blow, in AttackCollisionData collisionData, float damagedHp, float hitDistance, float shotDifficulty)
		{
			if (affectorAgent != null && GameNetwork.IsServer && !isBlocked && affectorAgent != affectedAgent && affectorAgent.MissionPeer != null && damagedHp > 0f)
			{
				affectedAgent.AddHitter(affectorAgent.MissionPeer, damagedHp, affectorAgent.IsFriendOf(affectedAgent));
			}
		}

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
					if (agent.OwningAgentMissionPeer != null)
					{
						MissionPeer owningAgentMissionPeer = agent.OwningAgentMissionPeer;
						int num = owningAgentMissionPeer.BotsUnderControlAlive;
						owningAgentMissionPeer.BotsUnderControlAlive = num + 1;
						MissionPeer owningAgentMissionPeer2 = agent.OwningAgentMissionPeer;
						num = owningAgentMissionPeer2.BotsUnderControlTotal;
						owningAgentMissionPeer2.BotsUnderControlTotal = num + 1;
						return;
					}
					this._missionScoreboardComponent.Sides[(int)agent.Team.Side].BotScores.AliveCount++;
				}
			}
		}

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

		public static int GetSpawnPeriodDurationForPeer(MissionPeer peer)
		{
			return Mission.Current.GetMissionBehavior<SpawnComponent>().GetMaximumReSpawnPeriodForPeer(peer);
		}

		public virtual void SetStateEndingAsServer()
		{
			this.CurrentMultiplayerState = MissionLobbyComponent.MultiplayerGameState.Ending;
			MBDebug.Print("Multiplayer game mission ending", 0, Debug.DebugColor.White, 17592186044416UL);
			this._timerComponent.StartTimerAsServer(MissionLobbyComponent.PostMatchWaitDuration);
			GameNetwork.BeginBroadcastModuleEvent();
			GameNetwork.WriteMessage(new MissionStateChange(this.CurrentMultiplayerState, this._timerComponent.GetCurrentTimerStartTime().NumberOfTicks));
			GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
			Debug.Print(string.Format("Current multiplayer state sent to clients: {0}", this.CurrentMultiplayerState), 0, Debug.DebugColor.White, 17592186044416UL);
			Action onPostMatchEnded = this.OnPostMatchEnded;
			if (onPostMatchEnded == null)
			{
				return;
			}
			onPostMatchEnded();
		}

		private void SetStatePlayingAsServer()
		{
			this._warmupComponent = null;
			this.CurrentMultiplayerState = MissionLobbyComponent.MultiplayerGameState.Playing;
			this._timerComponent.StartTimerAsServer((float)(MultiplayerOptions.OptionType.MapTimeLimit.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) * 60));
			GameNetwork.BeginBroadcastModuleEvent();
			GameNetwork.WriteMessage(new MissionStateChange(this.CurrentMultiplayerState, this._timerComponent.GetCurrentTimerStartTime().NumberOfTicks));
			GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
		}

		protected virtual void EndGameAsServer()
		{
		}

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

		private void SetStateEndingAsClient()
		{
			Action onPostMatchEnded = this.OnPostMatchEnded;
			if (onPostMatchEnded == null)
			{
				return;
			}
			onPostMatchEnded();
		}

		public void RequestCultureSelection()
		{
			Action onCultureSelectionRequested = this.OnCultureSelectionRequested;
			if (onCultureSelectionRequested == null)
			{
				return;
			}
			onCultureSelectionRequested();
		}

		public void RequestTroopSelection()
		{
			if (GameNetwork.IsClient)
			{
				GameNetwork.BeginModuleEventAsClient();
				GameNetwork.WriteMessage(new RequestChangeCharacterMessage(GameNetwork.MyPeer));
				GameNetwork.EndModuleEventAsClient();
				return;
			}
			if (GameNetwork.IsServer)
			{
				MissionPeer component = GameNetwork.MyPeer.GetComponent<MissionPeer>();
				if (component != null && this._gameMode.CheckIfPlayerCanDespawn(component))
				{
					this.DespawnPlayer(component);
				}
			}
		}

		public void OnCultureSelected(BasicCultureObject culture)
		{
			if (GameNetwork.IsClient)
			{
				GameNetwork.BeginModuleEventAsClient();
				GameNetwork.WriteMessage(new RequestCultureChange(culture));
				GameNetwork.EndModuleEventAsClient();
				return;
			}
			if (GameNetwork.IsServer)
			{
				MissionPeer component = GameNetwork.MyPeer.GetComponent<MissionPeer>();
				if (component != null && this._gameMode.CheckIfPlayerCanDespawn(component))
				{
					component.Culture = culture;
					this.DespawnPlayer(component);
				}
			}
		}

		public MultiplayerGameType MissionType { get; set; }

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

		public event Action<MissionLobbyComponent.MultiplayerGameState> CurrentMultiplayerStateChanged;

		public int GetRandomFaceSeedForCharacter(BasicCharacterObject character, int addition = 0)
		{
			IRoundComponent roundComponent = this._roundComponent;
			return character.GetDefaultFaceSeed(addition + ((roundComponent != null) ? roundComponent.RoundCount : 0)) % 2000;
		}

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

		private static readonly float InactivityThreshold = 2f;

		public static readonly float PostMatchWaitDuration = 15f;

		private MissionScoreboardComponent _missionScoreboardComponent;

		private MissionMultiplayerGameModeBase _gameMode;

		private MultiplayerTimerComponent _timerComponent;

		private IRoundComponent _roundComponent;

		private Timer _inactivityTimer;

		private MultiplayerWarmupComponent _warmupComponent;

		private static readonly Dictionary<Tuple<LobbyMissionType, bool>, Type> _lobbyComponentTypes = new Dictionary<Tuple<LobbyMissionType, bool>, Type>();

		private bool _usingFixedBanners;

		private MissionLobbyComponent.MultiplayerGameState _currentMultiplayerState;

		public enum MultiplayerGameState
		{
			WaitingFirstPlayers,
			Playing,
			Ending
		}
	}
}
