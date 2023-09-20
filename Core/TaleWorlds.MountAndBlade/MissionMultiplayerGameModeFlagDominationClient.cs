using System;
using System.Collections.Generic;
using System.Linq;
using NetworkMessages.FromClient;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.MissionRepresentatives;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.MountAndBlade.Objects;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200029B RID: 667
	public class MissionMultiplayerGameModeFlagDominationClient : MissionMultiplayerGameModeBaseClient, ICommanderInfo, IMissionBehavior
	{
		// Token: 0x170006C0 RID: 1728
		// (get) Token: 0x06002425 RID: 9253 RVA: 0x0008540E File Offset: 0x0008360E
		public override bool IsGameModeUsingGold
		{
			get
			{
				return this.GameType != MissionLobbyComponent.MultiplayerGameType.Captain;
			}
		}

		// Token: 0x170006C1 RID: 1729
		// (get) Token: 0x06002426 RID: 9254 RVA: 0x0008541C File Offset: 0x0008361C
		public override bool IsGameModeTactical
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170006C2 RID: 1730
		// (get) Token: 0x06002427 RID: 9255 RVA: 0x0008541F File Offset: 0x0008361F
		public override bool IsGameModeUsingRoundCountdown
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170006C3 RID: 1731
		// (get) Token: 0x06002428 RID: 9256 RVA: 0x00085422 File Offset: 0x00083622
		public override MissionLobbyComponent.MultiplayerGameType GameType
		{
			get
			{
				return this._currentGameType;
			}
		}

		// Token: 0x170006C4 RID: 1732
		// (get) Token: 0x06002429 RID: 9257 RVA: 0x0008542A File Offset: 0x0008362A
		public override bool IsGameModeUsingCasualGold
		{
			get
			{
				return false;
			}
		}

		// Token: 0x14000048 RID: 72
		// (add) Token: 0x0600242A RID: 9258 RVA: 0x00085430 File Offset: 0x00083630
		// (remove) Token: 0x0600242B RID: 9259 RVA: 0x00085468 File Offset: 0x00083668
		public event Action<NetworkCommunicator> OnBotsControlledChangedEvent;

		// Token: 0x14000049 RID: 73
		// (add) Token: 0x0600242C RID: 9260 RVA: 0x000854A0 File Offset: 0x000836A0
		// (remove) Token: 0x0600242D RID: 9261 RVA: 0x000854D8 File Offset: 0x000836D8
		public event Action<BattleSideEnum, float> OnTeamPowerChangedEvent;

		// Token: 0x1400004A RID: 74
		// (add) Token: 0x0600242E RID: 9262 RVA: 0x00085510 File Offset: 0x00083710
		// (remove) Token: 0x0600242F RID: 9263 RVA: 0x00085548 File Offset: 0x00083748
		public event Action<BattleSideEnum, float> OnMoraleChangedEvent;

		// Token: 0x1400004B RID: 75
		// (add) Token: 0x06002430 RID: 9264 RVA: 0x00085580 File Offset: 0x00083780
		// (remove) Token: 0x06002431 RID: 9265 RVA: 0x000855B8 File Offset: 0x000837B8
		public event Action OnFlagNumberChangedEvent;

		// Token: 0x1400004C RID: 76
		// (add) Token: 0x06002432 RID: 9266 RVA: 0x000855F0 File Offset: 0x000837F0
		// (remove) Token: 0x06002433 RID: 9267 RVA: 0x00085628 File Offset: 0x00083828
		public event Action<FlagCapturePoint, Team> OnCapturePointOwnerChangedEvent;

		// Token: 0x1400004D RID: 77
		// (add) Token: 0x06002434 RID: 9268 RVA: 0x00085660 File Offset: 0x00083860
		// (remove) Token: 0x06002435 RID: 9269 RVA: 0x00085698 File Offset: 0x00083898
		public event Action<GoldGain> OnGoldGainEvent;

		// Token: 0x170006C5 RID: 1733
		// (get) Token: 0x06002436 RID: 9270 RVA: 0x000856CD File Offset: 0x000838CD
		// (set) Token: 0x06002437 RID: 9271 RVA: 0x000856D5 File Offset: 0x000838D5
		public IEnumerable<FlagCapturePoint> AllCapturePoints { get; private set; }

		// Token: 0x170006C6 RID: 1734
		// (get) Token: 0x06002438 RID: 9272 RVA: 0x000856DE File Offset: 0x000838DE
		public bool AreMoralesIndependent
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06002439 RID: 9273 RVA: 0x000856E4 File Offset: 0x000838E4
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._scoreboardComponent = Mission.Current.GetMissionBehavior<MissionScoreboardComponent>();
			if (MultiplayerOptions.OptionType.SingleSpawn.GetBoolValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions))
			{
				this._currentGameType = ((MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) > 0) ? MissionLobbyComponent.MultiplayerGameType.Captain : MissionLobbyComponent.MultiplayerGameType.Battle);
			}
			else
			{
				this._currentGameType = MissionLobbyComponent.MultiplayerGameType.Skirmish;
			}
			this.ResetTeamPowers(1f);
			this._capturePointOwners = new Team[3];
			this.AllCapturePoints = Mission.Current.MissionObjects.FindAllWithType<FlagCapturePoint>();
			base.RoundComponent.OnPreparationEnded += this.OnPreparationEnded;
			base.MissionNetworkComponent.OnMyClientSynchronized += this.OnMyClientSynchronized;
		}

		// Token: 0x0600243A RID: 9274 RVA: 0x00085789 File Offset: 0x00083989
		public override void OnRemoveBehavior()
		{
			base.OnRemoveBehavior();
			base.RoundComponent.OnPreparationEnded -= this.OnPreparationEnded;
			base.MissionNetworkComponent.OnMyClientSynchronized -= this.OnMyClientSynchronized;
		}

		// Token: 0x0600243B RID: 9275 RVA: 0x000857BF File Offset: 0x000839BF
		private void OnMyClientSynchronized()
		{
			this._myRepresentative = GameNetwork.MyPeer.GetComponent<FlagDominationMissionRepresentative>();
		}

		// Token: 0x0600243C RID: 9276 RVA: 0x000857D1 File Offset: 0x000839D1
		public override void AfterStart()
		{
			Mission.Current.SetMissionMode(MissionMode.Battle, true);
		}

		// Token: 0x0600243D RID: 9277 RVA: 0x000857E0 File Offset: 0x000839E0
		protected override void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
		{
			if (GameNetwork.IsClient)
			{
				registerer.Register<BotsControlledChange>(new GameNetworkMessage.ServerMessageHandlerDelegate<BotsControlledChange>(this.HandleServerEventBotsControlledChangeEvent));
				registerer.Register<FlagDominationMoraleChangeMessage>(new GameNetworkMessage.ServerMessageHandlerDelegate<FlagDominationMoraleChangeMessage>(this.HandleMoraleChangedMessage));
				registerer.Register<SyncGoldsForSkirmish>(new GameNetworkMessage.ServerMessageHandlerDelegate<SyncGoldsForSkirmish>(this.HandleServerEventUpdateGold));
				registerer.Register<FlagDominationFlagsRemovedMessage>(new GameNetworkMessage.ServerMessageHandlerDelegate<FlagDominationFlagsRemovedMessage>(this.HandleFlagsRemovedMessage));
				registerer.Register<FlagDominationCapturePointMessage>(new GameNetworkMessage.ServerMessageHandlerDelegate<FlagDominationCapturePointMessage>(this.HandleServerEventPointCapturedMessage));
				registerer.Register<FormationWipedMessage>(new GameNetworkMessage.ServerMessageHandlerDelegate<FormationWipedMessage>(this.HandleServerEventFormationWipedMessage));
				registerer.Register<GoldGain>(new GameNetworkMessage.ServerMessageHandlerDelegate<GoldGain>(this.HandleServerEventPersonalGoldGain));
			}
		}

		// Token: 0x0600243E RID: 9278 RVA: 0x00085874 File Offset: 0x00083A74
		public void OnPreparationEnded()
		{
			this.AllCapturePoints = Mission.Current.MissionObjects.FindAllWithType<FlagCapturePoint>();
			Action onFlagNumberChangedEvent = this.OnFlagNumberChangedEvent;
			if (onFlagNumberChangedEvent != null)
			{
				onFlagNumberChangedEvent();
			}
			foreach (FlagCapturePoint flagCapturePoint in this.AllCapturePoints)
			{
				Action<FlagCapturePoint, Team> onCapturePointOwnerChangedEvent = this.OnCapturePointOwnerChangedEvent;
				if (onCapturePointOwnerChangedEvent != null)
				{
					onCapturePointOwnerChangedEvent(flagCapturePoint, null);
				}
			}
		}

		// Token: 0x0600243F RID: 9279 RVA: 0x000858F4 File Offset: 0x00083AF4
		public override SpectatorCameraTypes GetMissionCameraLockMode(bool lockedToMainPlayer)
		{
			SpectatorCameraTypes spectatorCameraTypes = SpectatorCameraTypes.Invalid;
			MissionPeer missionPeer = (GameNetwork.IsMyPeerReady ? GameNetwork.MyPeer.GetComponent<MissionPeer>() : null);
			if (!lockedToMainPlayer && missionPeer != null)
			{
				if (missionPeer.Team != base.Mission.SpectatorTeam)
				{
					if (this.GameType == MissionLobbyComponent.MultiplayerGameType.Captain && base.IsRoundInProgress)
					{
						Formation controlledFormation = missionPeer.ControlledFormation;
						if (controlledFormation != null)
						{
							if (controlledFormation.HasUnitsWithCondition((Agent agent) => !agent.IsPlayerControlled && agent.IsActive()))
							{
								spectatorCameraTypes = SpectatorCameraTypes.LockToPlayerFormation;
							}
						}
					}
				}
				else
				{
					spectatorCameraTypes = SpectatorCameraTypes.Free;
				}
			}
			return spectatorCameraTypes;
		}

		// Token: 0x06002440 RID: 9280 RVA: 0x0008597C File Offset: 0x00083B7C
		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
		{
			if (base.IsRoundInProgress && !affectedAgent.IsMount)
			{
				Team team = affectedAgent.Team;
				if (this.IsGameModeUsingGold)
				{
					this.UpdateTeamPowerBasedOnGold(team);
					return;
				}
				this.UpdateTeamPowerBasedOnTroopCount(team);
			}
		}

		// Token: 0x06002441 RID: 9281 RVA: 0x000859B8 File Offset: 0x00083BB8
		public override void OnClearScene()
		{
			this._informedAboutFlagRemoval = false;
			this.AllCapturePoints = Mission.Current.MissionObjects.FindAllWithType<FlagCapturePoint>();
			foreach (FlagCapturePoint flagCapturePoint in this.AllCapturePoints)
			{
				this._capturePointOwners[flagCapturePoint.FlagIndex] = null;
			}
			this.ResetTeamPowers(1f);
			if (this._bellSoundEvent != null)
			{
				this._remainingTimeForBellSoundToStop = float.MinValue;
				this._bellSoundEvent.Stop();
				this._bellSoundEvent = null;
			}
		}

		// Token: 0x06002442 RID: 9282 RVA: 0x00085A58 File Offset: 0x00083C58
		protected override int GetWarningTimer()
		{
			int num = 0;
			if (base.IsRoundInProgress)
			{
				float num2 = -1f;
				switch (this.GameType)
				{
				case MissionLobbyComponent.MultiplayerGameType.Battle:
					num2 = 210f;
					break;
				case MissionLobbyComponent.MultiplayerGameType.Captain:
					num2 = 180f;
					break;
				case MissionLobbyComponent.MultiplayerGameType.Skirmish:
					num2 = 120f;
					break;
				default:
					Debug.FailedAssert("A flag domination mode cannot be " + this.GameType + ".", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Multiplayer\\MissionNetworkLogics\\MultiplayerGameModeLogics\\ClientGameModeLogics\\MissionMultiplayerGameModeFlagDominationClient.cs", "GetWarningTimer", 207);
					break;
				}
				float num3 = (float)MultiplayerOptions.OptionType.RoundTimeLimit.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) - num2;
				float num4 = num3 + 30f;
				if (base.RoundComponent.RemainingRoundTime <= num4 && base.RoundComponent.RemainingRoundTime > num3)
				{
					num = MathF.Ceiling(30f - (num4 - base.RoundComponent.RemainingRoundTime));
					if (!this._informedAboutFlagRemoval)
					{
						this._informedAboutFlagRemoval = true;
						base.NotificationsComponent.FlagsWillBeRemovedInXSeconds(30);
					}
				}
			}
			return num;
		}

		// Token: 0x06002443 RID: 9283 RVA: 0x00085B43 File Offset: 0x00083D43
		public Team GetFlagOwner(FlagCapturePoint flag)
		{
			return this._capturePointOwners[flag.FlagIndex];
		}

		// Token: 0x06002444 RID: 9284 RVA: 0x00085B54 File Offset: 0x00083D54
		private void HandleServerEventBotsControlledChangeEvent(BotsControlledChange message)
		{
			MissionPeer component = message.Peer.GetComponent<MissionPeer>();
			this.OnBotsControlledChanged(component, message.AliveCount, message.TotalCount);
		}

		// Token: 0x06002445 RID: 9285 RVA: 0x00085B80 File Offset: 0x00083D80
		private void HandleMoraleChangedMessage(FlagDominationMoraleChangeMessage message)
		{
			this.OnMoraleChanged(message.Morale);
		}

		// Token: 0x06002446 RID: 9286 RVA: 0x00085B90 File Offset: 0x00083D90
		private void HandleServerEventUpdateGold(SyncGoldsForSkirmish message)
		{
			FlagDominationMissionRepresentative component = message.VirtualPlayer.GetComponent<FlagDominationMissionRepresentative>();
			this.OnGoldAmountChangedForRepresentative(component, message.GoldAmount);
		}

		// Token: 0x06002447 RID: 9287 RVA: 0x00085BB6 File Offset: 0x00083DB6
		private void HandleFlagsRemovedMessage(FlagDominationFlagsRemovedMessage message)
		{
			this.OnNumberOfFlagsChanged();
		}

		// Token: 0x06002448 RID: 9288 RVA: 0x00085BC0 File Offset: 0x00083DC0
		private void HandleServerEventPointCapturedMessage(FlagDominationCapturePointMessage message)
		{
			foreach (FlagCapturePoint flagCapturePoint in this.AllCapturePoints)
			{
				if (flagCapturePoint.FlagIndex == message.FlagIndex)
				{
					this.OnCapturePointOwnerChanged(flagCapturePoint, message.OwnerTeam);
					break;
				}
			}
		}

		// Token: 0x06002449 RID: 9289 RVA: 0x00085C24 File Offset: 0x00083E24
		private void HandleServerEventFormationWipedMessage(FormationWipedMessage message)
		{
			MatrixFrame cameraFrame = Mission.Current.GetCameraFrame();
			Vec3 vec = cameraFrame.origin + cameraFrame.rotation.u;
			MBSoundEvent.PlaySound(SoundEvent.GetEventIdFromString("event:/alerts/report/squad_wiped"), vec);
		}

		// Token: 0x0600244A RID: 9290 RVA: 0x00085C64 File Offset: 0x00083E64
		private void HandleServerEventPersonalGoldGain(GoldGain message)
		{
			Action<GoldGain> onGoldGainEvent = this.OnGoldGainEvent;
			if (onGoldGainEvent == null)
			{
				return;
			}
			onGoldGainEvent(message);
		}

		// Token: 0x0600244B RID: 9291 RVA: 0x00085C77 File Offset: 0x00083E77
		public void OnTeamPowerChanged(BattleSideEnum teamSide, float power)
		{
			Action<BattleSideEnum, float> onTeamPowerChangedEvent = this.OnTeamPowerChangedEvent;
			if (onTeamPowerChangedEvent == null)
			{
				return;
			}
			onTeamPowerChangedEvent(teamSide, power);
		}

		// Token: 0x0600244C RID: 9292 RVA: 0x00085C8C File Offset: 0x00083E8C
		public void OnMoraleChanged(float morale)
		{
			for (int i = 0; i < 2; i++)
			{
				float num = (morale + 1f) / 2f;
				if (i == 0)
				{
					Action<BattleSideEnum, float> onMoraleChangedEvent = this.OnMoraleChangedEvent;
					if (onMoraleChangedEvent != null)
					{
						onMoraleChangedEvent(BattleSideEnum.Defender, 1f - num);
					}
				}
				else if (i == 1)
				{
					Action<BattleSideEnum, float> onMoraleChangedEvent2 = this.OnMoraleChangedEvent;
					if (onMoraleChangedEvent2 != null)
					{
						onMoraleChangedEvent2(BattleSideEnum.Attacker, num);
					}
				}
			}
			FlagDominationMissionRepresentative myRepresentative = this._myRepresentative;
			if (((myRepresentative != null) ? myRepresentative.MissionPeer.Team : null) != null && this._myRepresentative.MissionPeer.Team.Side != BattleSideEnum.None)
			{
				float num2 = MathF.Abs(morale);
				if (this._remainingTimeForBellSoundToStop < 0f)
				{
					if (num2 >= 0.6f && num2 < 1f)
					{
						this._remainingTimeForBellSoundToStop = float.MaxValue;
					}
					else
					{
						this._remainingTimeForBellSoundToStop = float.MinValue;
					}
					if (this._remainingTimeForBellSoundToStop > 0f)
					{
						BattleSideEnum side = this._myRepresentative.MissionPeer.Team.Side;
						if ((side == BattleSideEnum.Defender && morale >= 0.6f) || (side == BattleSideEnum.Attacker && morale <= -0.6f))
						{
							this._bellSoundEvent = SoundEvent.CreateEventFromString("event:/multiplayer/warning_bells_defender", base.Mission.Scene);
						}
						else
						{
							this._bellSoundEvent = SoundEvent.CreateEventFromString("event:/multiplayer/warning_bells_attacker", base.Mission.Scene);
						}
						MatrixFrame globalFrame = this.AllCapturePoints.Where((FlagCapturePoint cp) => !cp.IsDeactivated).GetRandomElementInefficiently<FlagCapturePoint>().GameEntity.GetGlobalFrame();
						this._bellSoundEvent.PlayInPosition(globalFrame.origin + globalFrame.rotation.u * 3f);
						return;
					}
				}
				else if (num2 >= 1f || num2 < 0.6f)
				{
					this._remainingTimeForBellSoundToStop = float.MinValue;
				}
			}
		}

		// Token: 0x0600244D RID: 9293 RVA: 0x00085E58 File Offset: 0x00084058
		public override void OnGoldAmountChangedForRepresentative(MissionRepresentativeBase representative, int goldAmount)
		{
			if (representative != null)
			{
				MissionPeer component = representative.GetComponent<MissionPeer>();
				if (component != null)
				{
					representative.UpdateGold(goldAmount);
					this._scoreboardComponent.PlayerPropertiesChanged(component);
					if (this.IsGameModeUsingGold && base.IsRoundInProgress && component.Team != null && component.Team.Side != BattleSideEnum.None)
					{
						this.UpdateTeamPowerBasedOnGold(component.Team);
					}
				}
			}
		}

		// Token: 0x0600244E RID: 9294 RVA: 0x00085EB7 File Offset: 0x000840B7
		public void OnNumberOfFlagsChanged()
		{
			Action onFlagNumberChangedEvent = this.OnFlagNumberChangedEvent;
			if (onFlagNumberChangedEvent == null)
			{
				return;
			}
			onFlagNumberChangedEvent();
		}

		// Token: 0x0600244F RID: 9295 RVA: 0x00085EC9 File Offset: 0x000840C9
		public void OnBotsControlledChanged(MissionPeer missionPeer, int botAliveCount, int botTotalCount)
		{
			missionPeer.BotsUnderControlAlive = botAliveCount;
			missionPeer.BotsUnderControlTotal = botTotalCount;
			Action<NetworkCommunicator> onBotsControlledChangedEvent = this.OnBotsControlledChangedEvent;
			if (onBotsControlledChangedEvent == null)
			{
				return;
			}
			onBotsControlledChangedEvent(missionPeer.GetNetworkPeer());
		}

		// Token: 0x06002450 RID: 9296 RVA: 0x00085EF0 File Offset: 0x000840F0
		public void OnCapturePointOwnerChanged(FlagCapturePoint flagCapturePoint, Team ownerTeam)
		{
			this._capturePointOwners[flagCapturePoint.FlagIndex] = ownerTeam;
			Action<FlagCapturePoint, Team> onCapturePointOwnerChangedEvent = this.OnCapturePointOwnerChangedEvent;
			if (onCapturePointOwnerChangedEvent != null)
			{
				onCapturePointOwnerChangedEvent(flagCapturePoint, ownerTeam);
			}
			if (this._myRepresentative != null && this._myRepresentative.MissionPeer.Team != null)
			{
				MatrixFrame cameraFrame = Mission.Current.GetCameraFrame();
				Vec3 vec = cameraFrame.origin + cameraFrame.rotation.u;
				if (this._myRepresentative.MissionPeer.Team == ownerTeam)
				{
					MBSoundEvent.PlaySound(SoundEvent.GetEventIdFromString("event:/alerts/report/flag_captured"), vec);
					return;
				}
				MBSoundEvent.PlaySound(SoundEvent.GetEventIdFromString("event:/alerts/report/flag_lost"), vec);
			}
		}

		// Token: 0x06002451 RID: 9297 RVA: 0x00085F90 File Offset: 0x00084190
		public void OnRequestForfeitSpawn()
		{
			if (GameNetwork.IsClient)
			{
				GameNetwork.BeginModuleEventAsClient();
				GameNetwork.WriteMessage(new RequestForfeitSpawn());
				GameNetwork.EndModuleEventAsClient();
				return;
			}
			Mission.Current.GetMissionBehavior<MissionMultiplayerFlagDomination>().ForfeitSpawning(GameNetwork.MyPeer);
		}

		// Token: 0x06002452 RID: 9298 RVA: 0x00085FC2 File Offset: 0x000841C2
		private void ResetTeamPowers(float value = 1f)
		{
			Action<BattleSideEnum, float> onTeamPowerChangedEvent = this.OnTeamPowerChangedEvent;
			if (onTeamPowerChangedEvent != null)
			{
				onTeamPowerChangedEvent(BattleSideEnum.Attacker, value);
			}
			Action<BattleSideEnum, float> onTeamPowerChangedEvent2 = this.OnTeamPowerChangedEvent;
			if (onTeamPowerChangedEvent2 == null)
			{
				return;
			}
			onTeamPowerChangedEvent2(BattleSideEnum.Defender, value);
		}

		// Token: 0x06002453 RID: 9299 RVA: 0x00085FEC File Offset: 0x000841EC
		private void UpdateTeamPowerBasedOnGold(Team team)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
			{
				MissionPeer component = networkCommunicator.GetComponent<MissionPeer>();
				if (((component != null) ? component.Team : null) != null && component.Team.Side == team.Side)
				{
					int gold = component.GetComponent<FlagDominationMissionRepresentative>().Gold;
					if (gold >= 100)
					{
						num2 += gold;
					}
					if (component.ControlledAgent != null && component.ControlledAgent.IsActive())
					{
						MultiplayerClassDivisions.MPHeroClass mpheroClassForCharacter = MultiplayerClassDivisions.GetMPHeroClassForCharacter(component.ControlledAgent.Character);
						num2 += ((this._currentGameType == MissionLobbyComponent.MultiplayerGameType.Battle) ? mpheroClassForCharacter.TroopBattleCost : mpheroClassForCharacter.TroopCost);
					}
					num++;
				}
			}
			if (this._currentGameType == MissionLobbyComponent.MultiplayerGameType.Battle)
			{
				num3 = 120;
			}
			else
			{
				num3 = 300;
			}
			num += ((team.Side == BattleSideEnum.Attacker) ? MultiplayerOptions.OptionType.NumberOfBotsTeam1.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) : MultiplayerOptions.OptionType.NumberOfBotsTeam2.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
			using (List<Agent>.Enumerator enumerator2 = team.ActiveAgents.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (enumerator2.Current.MissionPeer == null)
					{
						num2 += num3;
					}
				}
			}
			int num4 = num * num3;
			float num5 = ((num4 == 0) ? 0f : ((float)num2 / (float)num4));
			num5 = MathF.Min(1f, num5);
			Action<BattleSideEnum, float> onTeamPowerChangedEvent = this.OnTeamPowerChangedEvent;
			if (onTeamPowerChangedEvent == null)
			{
				return;
			}
			onTeamPowerChangedEvent(team.Side, num5);
		}

		// Token: 0x06002454 RID: 9300 RVA: 0x00086184 File Offset: 0x00084384
		private void UpdateTeamPowerBasedOnTroopCount(Team team)
		{
			int count = team.ActiveAgents.Count;
			int num = count + team.QuerySystem.DeathCount;
			float num2 = (float)count / (float)num;
			Action<BattleSideEnum, float> onTeamPowerChangedEvent = this.OnTeamPowerChangedEvent;
			if (onTeamPowerChangedEvent == null)
			{
				return;
			}
			onTeamPowerChangedEvent(team.Side, num2);
		}

		// Token: 0x06002455 RID: 9301 RVA: 0x000861C8 File Offset: 0x000843C8
		public override List<CompassItemUpdateParams> GetCompassTargets()
		{
			List<CompassItemUpdateParams> list = new List<CompassItemUpdateParams>();
			if (!GameNetwork.IsMyPeerReady || !base.IsRoundInProgress)
			{
				return list;
			}
			MissionPeer component = GameNetwork.MyPeer.GetComponent<MissionPeer>();
			if (component == null || component.Team == null || component.Team.Side == BattleSideEnum.None)
			{
				return list;
			}
			foreach (FlagCapturePoint flagCapturePoint in this.AllCapturePoints.Where((FlagCapturePoint cp) => !cp.IsDeactivated))
			{
				int num = 17 + flagCapturePoint.FlagIndex;
				list.Add(new CompassItemUpdateParams(flagCapturePoint, (TargetIconType)num, flagCapturePoint.Position, flagCapturePoint.GetFlagColor(), flagCapturePoint.GetFlagColor2()));
			}
			bool flag = true;
			foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
			{
				MissionPeer component2 = networkCommunicator.GetComponent<MissionPeer>();
				if (((component2 != null) ? component2.Team : null) != null && component2.Team.Side != BattleSideEnum.None)
				{
					bool flag2 = component2.ControlledFormation != null;
					if (!flag2)
					{
						flag = false;
					}
					if (flag || component2.Team == component.Team)
					{
						MultiplayerClassDivisions.MPHeroClass mpheroClassForPeer = MultiplayerClassDivisions.GetMPHeroClassForPeer(component2, false);
						if (flag2)
						{
							Formation controlledFormation = component2.ControlledFormation;
							if (controlledFormation.CountOfUnits != 0)
							{
								WorldPosition medianPosition = controlledFormation.QuerySystem.MedianPosition;
								Vec2 vec = controlledFormation.SmoothedAverageUnitPosition;
								if (!vec.IsValid)
								{
									vec = controlledFormation.QuerySystem.AveragePosition;
								}
								medianPosition.SetVec2(vec);
								BannerCode bannerCode = null;
								bool flag3 = false;
								bool flag4 = false;
								if (controlledFormation.Team != null)
								{
									if (controlledFormation.Banner == null)
									{
										controlledFormation.Banner = new Banner(controlledFormation.BannerCode, controlledFormation.Team.Color, controlledFormation.Team.Color2);
									}
									flag3 = controlledFormation.Team.IsAttacker;
									flag4 = controlledFormation.Team.IsPlayerAlly;
									bannerCode = BannerCode.CreateFrom(controlledFormation.Banner);
								}
								TargetIconType targetIconType = ((mpheroClassForPeer != null) ? mpheroClassForPeer.IconType : TargetIconType.None);
								list.Add(new CompassItemUpdateParams(controlledFormation, targetIconType, medianPosition.GetNavMeshVec3(), bannerCode, flag3, flag4));
							}
						}
						else
						{
							Agent controlledAgent = component2.ControlledAgent;
							if (controlledAgent != null && controlledAgent.IsActive() && controlledAgent.Controller != Agent.ControllerType.Player)
							{
								BannerCode bannerCode2 = BannerCode.CreateFrom(new Banner(component2.Peer.BannerCode, component2.Team.Color, component2.Team.Color2));
								list.Add(new CompassItemUpdateParams(controlledAgent, mpheroClassForPeer.IconType, controlledAgent.Position, bannerCode2, component2.Team.IsAttacker, component2.Team.IsPlayerAlly));
							}
						}
					}
				}
			}
			return list;
		}

		// Token: 0x06002456 RID: 9302 RVA: 0x000864D0 File Offset: 0x000846D0
		public override int GetGoldAmount()
		{
			if (this._myRepresentative != null)
			{
				return this._myRepresentative.Gold;
			}
			return 0;
		}

		// Token: 0x06002457 RID: 9303 RVA: 0x000864E8 File Offset: 0x000846E8
		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			if (this._remainingTimeForBellSoundToStop > 0f)
			{
				this._remainingTimeForBellSoundToStop -= dt;
			}
			if (this._bellSoundEvent != null && (this._remainingTimeForBellSoundToStop <= 0f || base.MissionLobbyComponent.CurrentMultiplayerState != MissionLobbyComponent.MultiplayerGameState.Playing))
			{
				this._remainingTimeForBellSoundToStop = float.MinValue;
				this._bellSoundEvent.Stop();
				this._bellSoundEvent = null;
			}
		}

		// Token: 0x04000D31 RID: 3377
		private const float MySideMoraleDropThreshold = 0.4f;

		// Token: 0x04000D32 RID: 3378
		private float _remainingTimeForBellSoundToStop = float.MinValue;

		// Token: 0x04000D33 RID: 3379
		private SoundEvent _bellSoundEvent;

		// Token: 0x04000D34 RID: 3380
		private FlagDominationMissionRepresentative _myRepresentative;

		// Token: 0x04000D35 RID: 3381
		private MissionScoreboardComponent _scoreboardComponent;

		// Token: 0x04000D36 RID: 3382
		private MissionLobbyComponent.MultiplayerGameType _currentGameType;

		// Token: 0x04000D37 RID: 3383
		private Team[] _capturePointOwners;

		// Token: 0x04000D39 RID: 3385
		private bool _informedAboutFlagRemoval;
	}
}
