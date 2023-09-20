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
	public class MissionMultiplayerGameModeFlagDominationClient : MissionMultiplayerGameModeBaseClient, ICommanderInfo, IMissionBehavior
	{
		public override bool IsGameModeUsingGold
		{
			get
			{
				return this.GameType != MissionLobbyComponent.MultiplayerGameType.Captain;
			}
		}

		public override bool IsGameModeTactical
		{
			get
			{
				return true;
			}
		}

		public override bool IsGameModeUsingRoundCountdown
		{
			get
			{
				return true;
			}
		}

		public override MissionLobbyComponent.MultiplayerGameType GameType
		{
			get
			{
				return this._currentGameType;
			}
		}

		public override bool IsGameModeUsingCasualGold
		{
			get
			{
				return false;
			}
		}

		public event Action<NetworkCommunicator> OnBotsControlledChangedEvent;

		public event Action<BattleSideEnum, float> OnTeamPowerChangedEvent;

		public event Action<BattleSideEnum, float> OnMoraleChangedEvent;

		public event Action OnFlagNumberChangedEvent;

		public event Action<FlagCapturePoint, Team> OnCapturePointOwnerChangedEvent;

		public event Action<GoldGain> OnGoldGainEvent;

		public IEnumerable<FlagCapturePoint> AllCapturePoints { get; private set; }

		public bool AreMoralesIndependent
		{
			get
			{
				return false;
			}
		}

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

		public override void OnRemoveBehavior()
		{
			base.OnRemoveBehavior();
			base.RoundComponent.OnPreparationEnded -= this.OnPreparationEnded;
			base.MissionNetworkComponent.OnMyClientSynchronized -= this.OnMyClientSynchronized;
		}

		private void OnMyClientSynchronized()
		{
			this._myRepresentative = GameNetwork.MyPeer.GetComponent<FlagDominationMissionRepresentative>();
		}

		public override void AfterStart()
		{
			Mission.Current.SetMissionMode(MissionMode.Battle, true);
		}

		protected override void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
		{
			if (GameNetwork.IsClient)
			{
				registerer.RegisterBaseHandler<BotsControlledChange>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventBotsControlledChangeEvent));
				registerer.RegisterBaseHandler<FlagDominationMoraleChangeMessage>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleMoraleChangedMessage));
				registerer.RegisterBaseHandler<SyncGoldsForSkirmish>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventUpdateGold));
				registerer.RegisterBaseHandler<FlagDominationFlagsRemovedMessage>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleFlagsRemovedMessage));
				registerer.RegisterBaseHandler<FlagDominationCapturePointMessage>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventPointCapturedMessage));
				registerer.RegisterBaseHandler<FormationWipedMessage>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventFormationWipedMessage));
				registerer.RegisterBaseHandler<GoldGain>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventPersonalGoldGain));
			}
		}

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

		public Team GetFlagOwner(FlagCapturePoint flag)
		{
			return this._capturePointOwners[flag.FlagIndex];
		}

		private void HandleServerEventBotsControlledChangeEvent(GameNetworkMessage baseMessage)
		{
			BotsControlledChange botsControlledChange = (BotsControlledChange)baseMessage;
			MissionPeer component = botsControlledChange.Peer.GetComponent<MissionPeer>();
			this.OnBotsControlledChanged(component, botsControlledChange.AliveCount, botsControlledChange.TotalCount);
		}

		private void HandleMoraleChangedMessage(GameNetworkMessage baseMessage)
		{
			FlagDominationMoraleChangeMessage flagDominationMoraleChangeMessage = (FlagDominationMoraleChangeMessage)baseMessage;
			this.OnMoraleChanged(flagDominationMoraleChangeMessage.Morale);
		}

		private void HandleServerEventUpdateGold(GameNetworkMessage baseMessage)
		{
			SyncGoldsForSkirmish syncGoldsForSkirmish = (SyncGoldsForSkirmish)baseMessage;
			FlagDominationMissionRepresentative component = syncGoldsForSkirmish.VirtualPlayer.GetComponent<FlagDominationMissionRepresentative>();
			this.OnGoldAmountChangedForRepresentative(component, syncGoldsForSkirmish.GoldAmount);
		}

		private void HandleFlagsRemovedMessage(GameNetworkMessage baseMessage)
		{
			this.OnNumberOfFlagsChanged();
		}

		private void HandleServerEventPointCapturedMessage(GameNetworkMessage baseMessage)
		{
			FlagDominationCapturePointMessage flagDominationCapturePointMessage = (FlagDominationCapturePointMessage)baseMessage;
			foreach (FlagCapturePoint flagCapturePoint in this.AllCapturePoints)
			{
				if (flagCapturePoint.FlagIndex == flagDominationCapturePointMessage.FlagIndex)
				{
					this.OnCapturePointOwnerChanged(flagCapturePoint, flagDominationCapturePointMessage.OwnerTeam);
					break;
				}
			}
		}

		private void HandleServerEventFormationWipedMessage(GameNetworkMessage baseMessage)
		{
			MatrixFrame cameraFrame = Mission.Current.GetCameraFrame();
			Vec3 vec = cameraFrame.origin + cameraFrame.rotation.u;
			MBSoundEvent.PlaySound(SoundEvent.GetEventIdFromString("event:/alerts/report/squad_wiped"), vec);
		}

		private void HandleServerEventPersonalGoldGain(GameNetworkMessage baseMessage)
		{
			GoldGain goldGain = (GoldGain)baseMessage;
			Action<GoldGain> onGoldGainEvent = this.OnGoldGainEvent;
			if (onGoldGainEvent == null)
			{
				return;
			}
			onGoldGainEvent(goldGain);
		}

		public void OnTeamPowerChanged(BattleSideEnum teamSide, float power)
		{
			Action<BattleSideEnum, float> onTeamPowerChangedEvent = this.OnTeamPowerChangedEvent;
			if (onTeamPowerChangedEvent == null)
			{
				return;
			}
			onTeamPowerChangedEvent(teamSide, power);
		}

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

		public void OnNumberOfFlagsChanged()
		{
			Action onFlagNumberChangedEvent = this.OnFlagNumberChangedEvent;
			if (onFlagNumberChangedEvent == null)
			{
				return;
			}
			onFlagNumberChangedEvent();
		}

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

		public override int GetGoldAmount()
		{
			if (this._myRepresentative != null)
			{
				return this._myRepresentative.Gold;
			}
			return 0;
		}

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

		private const float MySideMoraleDropThreshold = 0.4f;

		private float _remainingTimeForBellSoundToStop = float.MinValue;

		private SoundEvent _bellSoundEvent;

		private FlagDominationMissionRepresentative _myRepresentative;

		private MissionScoreboardComponent _scoreboardComponent;

		private MissionLobbyComponent.MultiplayerGameType _currentGameType;

		private Team[] _capturePointOwners;

		private bool _informedAboutFlagRemoval;
	}
}
