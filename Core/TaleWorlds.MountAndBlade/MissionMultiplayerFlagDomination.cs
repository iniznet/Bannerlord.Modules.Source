﻿using System;
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
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
	public class MissionMultiplayerFlagDomination : MissionMultiplayerGameModeBase, IAnalyticsFlagInfo, IMissionBehavior
	{
		public override bool IsGameModeHidingAllAgentVisuals
		{
			get
			{
				return this._gameType == MultiplayerGameType.Captain || this._gameType == MultiplayerGameType.Battle;
			}
		}

		public override bool IsGameModeUsingOpposingTeams
		{
			get
			{
				return true;
			}
		}

		public MBReadOnlyList<FlagCapturePoint> AllCapturePoints { get; private set; }

		public float MoraleRounded
		{
			get
			{
				return (float)((int)(this._morale / 0.01f)) * 0.01f;
			}
		}

		public bool GameModeUsesSingleSpawning
		{
			get
			{
				return this.GetMissionType() == MultiplayerGameType.Captain || this.GetMissionType() == MultiplayerGameType.Battle;
			}
		}

		public bool UseGold()
		{
			return this._gameModeFlagDominationClient.IsGameModeUsingGold;
		}

		public override bool AllowCustomPlayerBanners()
		{
			return false;
		}

		public override bool UseRoundController()
		{
			return true;
		}

		public MissionMultiplayerFlagDomination(MultiplayerGameType gameType)
		{
			this._gameType = gameType;
			switch (this._gameType)
			{
			case MultiplayerGameType.Battle:
				this._moraleMultiplierForEachFlag = 0.75f;
				this._pointRemovalTimeInSeconds = 210f;
				this._moraleMultiplierOnLastFlag = 3.5f;
				MissionMultiplayerFlagDomination._defaultGoldAmountForTroopSelection = 120;
				MissionMultiplayerFlagDomination._maxGoldAmountToCarryOver = 110;
				MissionMultiplayerFlagDomination._maxGoldAmountToCarryOverForSurvival = 20;
				return;
			case MultiplayerGameType.Captain:
				this._moraleMultiplierForEachFlag = 1f;
				this._pointRemovalTimeInSeconds = 180f;
				this._moraleMultiplierOnLastFlag = 2f;
				return;
			case MultiplayerGameType.Skirmish:
				this._moraleMultiplierForEachFlag = 2f;
				this._pointRemovalTimeInSeconds = 120f;
				this._moraleMultiplierOnLastFlag = 2f;
				MissionMultiplayerFlagDomination._defaultGoldAmountForTroopSelection = 300;
				MissionMultiplayerFlagDomination._maxGoldAmountToCarryOver = 80;
				MissionMultiplayerFlagDomination._maxGoldAmountToCarryOverForSurvival = 30;
				return;
			default:
				return;
			}
		}

		public override MultiplayerGameType GetMissionType()
		{
			return this._gameType;
		}

		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._gameModeFlagDominationClient = Mission.Current.GetMissionBehavior<MissionMultiplayerGameModeFlagDominationClient>();
			this._morale = 0f;
			this._capturePointOwners = new Team[3];
			this.AllCapturePoints = Mission.Current.MissionObjects.FindAllWithType<FlagCapturePoint>().ToMBList<FlagCapturePoint>();
			foreach (FlagCapturePoint flagCapturePoint in this.AllCapturePoints)
			{
				flagCapturePoint.SetTeamColorsWithAllSynched(4284111450U, uint.MaxValue);
				this._capturePointOwners[flagCapturePoint.FlagIndex] = null;
			}
		}

		public override void AfterStart()
		{
			base.AfterStart();
			this.RoundController.OnRoundStarted += this.OnPreparationStart;
			MissionPeer.OnPreTeamChanged += this.OnPreTeamChanged;
			this.RoundController.OnPreparationEnded += this.OnPreparationEnded;
			if (this.WarmupComponent != null)
			{
				this.WarmupComponent.OnWarmupEnding += this.OnWarmupEnding;
			}
			this.RoundController.OnPreRoundEnding += this.OnRoundEnd;
			this.RoundController.OnPostRoundEnded += this.OnPostRoundEnd;
			BasicCultureObject @object = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam1.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
			BasicCultureObject object2 = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam2.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
			Banner banner = new Banner(@object.BannerKey, @object.BackgroundColor1, @object.ForegroundColor1);
			Banner banner2 = new Banner(object2.BannerKey, object2.BackgroundColor2, object2.ForegroundColor2);
			base.Mission.Teams.Add(BattleSideEnum.Attacker, @object.BackgroundColor1, @object.ForegroundColor1, banner, false, true, true);
			base.Mission.Teams.Add(BattleSideEnum.Defender, object2.BackgroundColor2, object2.ForegroundColor2, banner2, false, true, true);
		}

		protected override void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
		{
			registerer.RegisterBaseHandler<RequestForfeitSpawn>(new GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage>(this.HandleClientEventRequestForfeitSpawn));
		}

		public override void OnRemoveBehavior()
		{
			this.RoundController.OnRoundStarted -= this.OnPreparationStart;
			MissionPeer.OnPreTeamChanged -= this.OnPreTeamChanged;
			this.RoundController.OnPreparationEnded -= this.OnPreparationEnded;
			if (this.WarmupComponent != null)
			{
				this.WarmupComponent.OnWarmupEnding -= this.OnWarmupEnding;
			}
			this.RoundController.OnPreRoundEnding -= this.OnRoundEnd;
			this.RoundController.OnPostRoundEnded -= this.OnPostRoundEnd;
			base.OnRemoveBehavior();
		}

		public override void OnPeerChangedTeam(NetworkCommunicator peer, Team oldTeam, Team newTeam)
		{
			if (oldTeam != null && oldTeam != newTeam && this.UseGold() && (this.WarmupComponent == null || !this.WarmupComponent.IsInWarmup))
			{
				base.ChangeCurrentGoldForPeer(peer.GetComponent<MissionPeer>(), 0);
			}
		}

		private void OnPreparationStart()
		{
			this.NotificationsComponent.PreparationStarted();
		}

		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			if (this.MissionLobbyComponent.CurrentMultiplayerState == MissionLobbyComponent.MultiplayerGameState.Playing)
			{
				if (MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) > 0)
				{
					this.CheckForPlayersSpawningAsBots();
					this.CheckPlayerBeingDetached();
				}
				if (this.RoundController.IsRoundInProgress && base.CanGameModeSystemsTickThisFrame)
				{
					if (!this._flagRemovalOccured)
					{
						this.CheckRemovingOfPoints();
					}
					this.CheckMorales();
					this.TickFlags();
				}
			}
		}

		private void CheckForPlayersSpawningAsBots()
		{
			foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
			{
				if (networkCommunicator.IsSynchronized)
				{
					MissionPeer component = networkCommunicator.GetComponent<MissionPeer>();
					if (component != null && component.ControlledAgent == null && component.Team != null && component.ControlledFormation != null && component.SpawnCountThisRound > 0)
					{
						if (!component.HasSpawnTimerExpired && component.SpawnTimer.Check(base.Mission.CurrentTime))
						{
							component.HasSpawnTimerExpired = true;
						}
						if (component.HasSpawnTimerExpired && component.WantsToSpawnAsBot)
						{
							if (component.ControlledFormation.HasUnitsWithCondition((Agent agent) => agent.IsActive() && agent.IsAIControlled))
							{
								Agent newAgent = null;
								Agent followingAgent = component.FollowedAgent;
								if (followingAgent != null && followingAgent.IsActive() && followingAgent.IsAIControlled && component.ControlledFormation.HasUnitsWithCondition((Agent agent) => agent == followingAgent))
								{
									newAgent = followingAgent;
								}
								else
								{
									float maxHealth = 0f;
									component.ControlledFormation.ApplyActionOnEachUnit(delegate(Agent agent)
									{
										if (agent.Health > maxHealth)
										{
											maxHealth = agent.Health;
											newAgent = agent;
										}
									}, null);
								}
								Mission.Current.ReplaceBotWithPlayer(newAgent, component);
								component.WantsToSpawnAsBot = false;
								component.HasSpawnTimerExpired = false;
							}
						}
					}
				}
			}
		}

		private bool GetMoraleGain(out float moraleGain)
		{
			List<FlagCapturePoint> list = this.AllCapturePoints.Where((FlagCapturePoint flag) => !flag.IsDeactivated && this.GetFlagOwnerTeam(flag) != null && flag.IsFullyRaised).ToList<FlagCapturePoint>();
			int num = list.Count((FlagCapturePoint flag) => this.GetFlagOwnerTeam(flag).Side == BattleSideEnum.Attacker) - list.Count((FlagCapturePoint flag) => this.GetFlagOwnerTeam(flag).Side == BattleSideEnum.Defender);
			int num2 = MathF.Sign(num);
			moraleGain = 0f;
			if (num2 != 0)
			{
				float num3 = 0.000625f * this._moraleMultiplierForEachFlag * (float)MathF.Abs(num);
				if (num2 > 0)
				{
					moraleGain = MBMath.ClampFloat((float)num2 - this._morale, 1f, 2f) * num3;
				}
				else
				{
					moraleGain = MBMath.ClampFloat((float)num2 - this._morale, -2f, -1f) * num3;
				}
				if (this._flagRemovalOccured)
				{
					moraleGain *= this._moraleMultiplierOnLastFlag;
				}
				return true;
			}
			return false;
		}

		public float GetTimeUntilBattleSideVictory(BattleSideEnum side)
		{
			float num = float.MaxValue;
			if ((side == BattleSideEnum.Attacker && this._morale > 0f) || (side == BattleSideEnum.Defender && this._morale < 0f))
			{
				num = this.RoundController.RemainingRoundTime;
			}
			float num2 = float.MaxValue;
			float num3;
			this.GetMoraleGain(out num3);
			if (side == BattleSideEnum.Attacker && num3 > 0f)
			{
				num2 = (1f - this._morale) / num3;
			}
			else if (side == BattleSideEnum.Defender && num3 < 0f)
			{
				num2 = (-1f - this._morale) / (num3 / 0.25f);
			}
			return MathF.Min(num, num2);
		}

		private void CheckMorales()
		{
			float num;
			if (this.GetMoraleGain(out num))
			{
				this._morale += num;
				this._morale = MBMath.ClampFloat(this._morale, -1f, 1f);
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new FlagDominationMoraleChangeMessage(this.MoraleRounded));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
				MissionMultiplayerGameModeFlagDominationClient gameModeFlagDominationClient = this._gameModeFlagDominationClient;
				if (gameModeFlagDominationClient != null)
				{
					gameModeFlagDominationClient.OnMoraleChanged(this.MoraleRounded);
				}
				MPPerkObject.RaiseEventForAllPeers(MPPerkCondition.PerkEventFlags.MoraleChange);
			}
		}

		private void CheckRemovingOfPoints()
		{
			if (this._nextTimeToCheckForPointRemoval < 0f)
			{
				this._nextTimeToCheckForPointRemoval = base.Mission.CurrentTime + this._pointRemovalTimeInSeconds;
			}
			if (base.Mission.CurrentTime >= this._nextTimeToCheckForPointRemoval)
			{
				this._nextTimeToCheckForPointRemoval += this._pointRemovalTimeInSeconds;
				List<BattleSideEnum> list = new List<BattleSideEnum>();
				using (List<Team>.Enumerator enumerator = base.Mission.Teams.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Team team = enumerator.Current;
						if (team.Side != BattleSideEnum.None)
						{
							int num = team.Side * BattleSideEnum.NumSides - BattleSideEnum.Attacker;
							if (this.AllCapturePoints.All((FlagCapturePoint cp) => this.GetFlagOwnerTeam(cp) != team))
							{
								if (this.AllCapturePoints.FirstOrDefault((FlagCapturePoint cp) => this.GetFlagOwnerTeam(cp) == null) != null)
								{
									this._morale -= 0.1f * (float)num;
									list.Add(BattleSideEnum.None);
								}
								else
								{
									this._morale -= 0.1f * (float)num * 2f;
									list.Add(team.Side.GetOppositeSide());
								}
								this._morale = MBMath.ClampFloat(this._morale, -1f, 1f);
							}
							else
							{
								list.Add(team.Side);
							}
						}
					}
				}
				List<int> removedCapIndexList = new List<int>();
				MBList<FlagCapturePoint> mblist = this.AllCapturePoints.ToMBList<FlagCapturePoint>();
				using (List<BattleSideEnum>.Enumerator enumerator2 = list.GetEnumerator())
				{
					Func<FlagCapturePoint, bool> <>9__4;
					while (enumerator2.MoveNext())
					{
						BattleSideEnum side = enumerator2.Current;
						if (side == BattleSideEnum.None)
						{
							removedCapIndexList.Add(this.RemoveCapturePoint(mblist.GetRandomElementWithPredicate((FlagCapturePoint cp) => this.GetFlagOwnerTeam(cp) == null)));
						}
						else
						{
							List<FlagCapturePoint> list2 = mblist.Where((FlagCapturePoint cp) => this.GetFlagOwnerTeam(cp) != null && this.GetFlagOwnerTeam(cp).Side == side).ToList<FlagCapturePoint>();
							MBList<FlagCapturePoint> mblist2 = list2.Where((FlagCapturePoint cp) => this.GetNumberOfAttackersAroundFlag(cp) == 0).ToMBList<FlagCapturePoint>();
							if (mblist2.Count > 0)
							{
								removedCapIndexList.Add(this.RemoveCapturePoint(mblist2.GetRandomElement<FlagCapturePoint>()));
							}
							else
							{
								MBList<KeyValuePair<FlagCapturePoint, int>> mblist3 = new MBList<KeyValuePair<FlagCapturePoint, int>>();
								foreach (FlagCapturePoint flagCapturePoint in list2)
								{
									if (mblist3.Count == 0)
									{
										mblist3.Add(new KeyValuePair<FlagCapturePoint, int>(flagCapturePoint, this.GetNumberOfAttackersAroundFlag(flagCapturePoint)));
									}
									else
									{
										int count = this.GetNumberOfAttackersAroundFlag(flagCapturePoint);
										if (mblist3.Any((KeyValuePair<FlagCapturePoint, int> cc) => cc.Value > count))
										{
											mblist3.Clear();
											mblist3.Add(new KeyValuePair<FlagCapturePoint, int>(flagCapturePoint, count));
										}
										else if (mblist3.Any((KeyValuePair<FlagCapturePoint, int> cc) => cc.Value == count))
										{
											mblist3.Add(new KeyValuePair<FlagCapturePoint, int>(flagCapturePoint, count));
										}
									}
								}
								removedCapIndexList.Add(this.RemoveCapturePoint(mblist3.GetRandomElement<KeyValuePair<FlagCapturePoint, int>>().Key));
							}
						}
						IEnumerable<FlagCapturePoint> enumerable = mblist;
						Func<FlagCapturePoint, bool> func;
						if ((func = <>9__4) == null)
						{
							func = (<>9__4 = (FlagCapturePoint fl) => fl.FlagIndex == removedCapIndexList[removedCapIndexList.Count - 1]);
						}
						FlagCapturePoint flagCapturePoint2 = enumerable.First(func);
						mblist.Remove(flagCapturePoint2);
					}
				}
				removedCapIndexList.Sort();
				int first = removedCapIndexList[0];
				int second = removedCapIndexList[1];
				FlagCapturePoint flagCapturePoint3 = this.AllCapturePoints.First((FlagCapturePoint cp) => cp.FlagIndex != first && cp.FlagIndex != second);
				this.NotificationsComponent.FlagXRemaining(flagCapturePoint3);
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new FlagDominationMoraleChangeMessage(this.MoraleRounded));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new FlagDominationFlagsRemovedMessage());
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
				this._flagRemovalOccured = true;
				MissionMultiplayerGameModeFlagDominationClient gameModeFlagDominationClient = this._gameModeFlagDominationClient;
				if (gameModeFlagDominationClient != null)
				{
					gameModeFlagDominationClient.OnNumberOfFlagsChanged();
				}
				foreach (MissionBehavior missionBehavior in base.Mission.MissionBehaviors)
				{
					IFlagRemoved flagRemoved = missionBehavior as IFlagRemoved;
					if (flagRemoved != null)
					{
						flagRemoved.OnFlagsRemoved(flagCapturePoint3.FlagIndex);
					}
				}
				MPPerkObject.RaiseEventForAllPeers(MPPerkCondition.PerkEventFlags.FlagRemoval);
			}
		}

		private int RemoveCapturePoint(FlagCapturePoint capToRemove)
		{
			int flagIndex = capToRemove.FlagIndex;
			capToRemove.RemovePointAsServer();
			GameNetwork.BeginBroadcastModuleEvent();
			GameNetwork.WriteMessage(new FlagDominationCapturePointMessage(flagIndex, -1));
			GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
			return flagIndex;
		}

		public override void OnClearScene()
		{
			base.OnClearScene();
			this.AllCapturePoints = Mission.Current.MissionObjects.FindAllWithType<FlagCapturePoint>().ToMBList<FlagCapturePoint>();
			foreach (FlagCapturePoint flagCapturePoint in this.AllCapturePoints)
			{
				flagCapturePoint.ResetPointAsServer(4284111450U, uint.MaxValue);
				this._capturePointOwners[flagCapturePoint.FlagIndex] = null;
			}
			this._morale = 0f;
			this._nextTimeToCheckForPointRemoval = float.MinValue;
			this._flagRemovalOccured = false;
		}

		public override bool CheckIfOvertime()
		{
			if (!this._flagRemovalOccured)
			{
				return false;
			}
			FlagCapturePoint flagCapturePoint = this.AllCapturePoints.FirstOrDefault((FlagCapturePoint flag) => !flag.IsDeactivated);
			Team flagOwnerTeam = this.GetFlagOwnerTeam(flagCapturePoint);
			return flagOwnerTeam != null && ((float)(flagOwnerTeam.Side * BattleSideEnum.NumSides - BattleSideEnum.Attacker) * this._morale < 0f || this.GetNumberOfAttackersAroundFlag(flagCapturePoint) > 0);
		}

		public override bool CheckForWarmupEnd()
		{
			int[] array = new int[2];
			foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
			{
				MissionPeer component = networkCommunicator.GetComponent<MissionPeer>();
				if (networkCommunicator.IsSynchronized && ((component != null) ? component.Team : null) != null && component.Team.Side != BattleSideEnum.None)
				{
					array[(int)component.Team.Side]++;
				}
			}
			return array.Sum() >= MultiplayerOptions.OptionType.MaxNumberOfPlayers.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
		}

		public override bool CheckForRoundEnd()
		{
			if (base.CanGameModeSystemsTickThisFrame)
			{
				if (MathF.Abs(this._morale) >= 1f)
				{
					if (!this._flagRemovalOccured)
					{
						return true;
					}
					FlagCapturePoint flagCapturePoint = this.AllCapturePoints.FirstOrDefault((FlagCapturePoint flag) => !flag.IsDeactivated);
					Team flagOwnerTeam = this.GetFlagOwnerTeam(flagCapturePoint);
					if (flagOwnerTeam == null)
					{
						return true;
					}
					BattleSideEnum battleSideEnum = ((this._morale > 0f) ? BattleSideEnum.Attacker : BattleSideEnum.Defender);
					return flagOwnerTeam.Side == battleSideEnum && flagCapturePoint.IsFullyRaised && this.GetNumberOfAttackersAroundFlag(flagCapturePoint) == 0;
				}
				else
				{
					bool flag3 = base.Mission.AttackerTeam.ActiveAgents.Count > 0;
					bool flag2 = base.Mission.DefenderTeam.ActiveAgents.Count > 0;
					if (flag3 && flag2)
					{
						return false;
					}
					if (!base.SpawnComponent.AreAgentsSpawning())
					{
						return true;
					}
					bool[] array = new bool[2];
					if (this.UseGold())
					{
						foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
						{
							MissionPeer component = networkCommunicator.GetComponent<MissionPeer>();
							if (((component != null) ? component.Team : null) != null && component.Team.Side != BattleSideEnum.None && !array[(int)component.Team.Side])
							{
								string text = MultiplayerOptions.OptionType.CultureTeam1.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
								if (component.Team.Side != BattleSideEnum.Attacker)
								{
									text = MultiplayerOptions.OptionType.CultureTeam2.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
								}
								if (base.GetCurrentGoldForPeer(component) >= MultiplayerClassDivisions.GetMinimumTroopCost(MBObjectManager.Instance.GetObject<BasicCultureObject>(text)))
								{
									array[(int)component.Team.Side] = true;
								}
							}
						}
					}
					if ((!flag3 && !array[1]) || (!flag2 && !array[0]))
					{
						return true;
					}
				}
			}
			return false;
		}

		public override bool UseCultureSelection()
		{
			return false;
		}

		private void OnWarmupEnding()
		{
			this.NotificationsComponent.WarmupEnding();
		}

		private void OnRoundEnd()
		{
			foreach (FlagCapturePoint flagCapturePoint in this.AllCapturePoints)
			{
				if (!flagCapturePoint.IsDeactivated)
				{
					flagCapturePoint.SetMoveNone();
				}
			}
			RoundEndReason roundEndReason = RoundEndReason.Invalid;
			bool flag = this.RoundController.RemainingRoundTime <= 0f && !this.CheckIfOvertime();
			int num = -1;
			for (int i = 0; i < 2; i++)
			{
				int num2 = i * 2 - 1;
				if ((flag && (float)num2 * this._morale > 0f) || (!flag && (float)num2 * this._morale >= 1f))
				{
					num = i;
					break;
				}
			}
			CaptureTheFlagCaptureResultEnum captureTheFlagCaptureResultEnum = CaptureTheFlagCaptureResultEnum.NotCaptured;
			if (num >= 0)
			{
				captureTheFlagCaptureResultEnum = ((num == 0) ? CaptureTheFlagCaptureResultEnum.DefendersWin : CaptureTheFlagCaptureResultEnum.AttackersWin);
				this.RoundController.RoundWinner = ((num == 0) ? BattleSideEnum.Defender : BattleSideEnum.Attacker);
				roundEndReason = (flag ? RoundEndReason.RoundTimeEnded : RoundEndReason.GameModeSpecificEnded);
			}
			else
			{
				bool flag2 = base.Mission.AttackerTeam.ActiveAgents.Count > 0;
				bool flag3 = base.Mission.DefenderTeam.ActiveAgents.Count > 0;
				if (flag2 && flag3)
				{
					if (this._morale > 0f)
					{
						captureTheFlagCaptureResultEnum = CaptureTheFlagCaptureResultEnum.AttackersWin;
						this.RoundController.RoundWinner = BattleSideEnum.Attacker;
					}
					else if (this._morale < 0f)
					{
						captureTheFlagCaptureResultEnum = CaptureTheFlagCaptureResultEnum.DefendersWin;
						this.RoundController.RoundWinner = BattleSideEnum.Defender;
					}
					else
					{
						captureTheFlagCaptureResultEnum = CaptureTheFlagCaptureResultEnum.Draw;
						this.RoundController.RoundWinner = BattleSideEnum.None;
					}
					roundEndReason = RoundEndReason.RoundTimeEnded;
				}
				else if (flag2)
				{
					captureTheFlagCaptureResultEnum = CaptureTheFlagCaptureResultEnum.AttackersWin;
					this.RoundController.RoundWinner = BattleSideEnum.Attacker;
					roundEndReason = RoundEndReason.SideDepleted;
				}
				else if (flag3)
				{
					captureTheFlagCaptureResultEnum = CaptureTheFlagCaptureResultEnum.DefendersWin;
					this.RoundController.RoundWinner = BattleSideEnum.Defender;
					roundEndReason = RoundEndReason.SideDepleted;
				}
				else
				{
					foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
					{
						MissionPeer component = networkCommunicator.GetComponent<MissionPeer>();
						if (((component != null) ? component.Team : null) != null && component.Team.Side != BattleSideEnum.None)
						{
							string text = MultiplayerOptions.OptionType.CultureTeam1.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
							if (component.Team.Side != BattleSideEnum.Attacker)
							{
								text = MultiplayerOptions.OptionType.CultureTeam2.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
							}
							if (base.GetCurrentGoldForPeer(component) >= MultiplayerClassDivisions.GetMinimumTroopCost(MBObjectManager.Instance.GetObject<BasicCultureObject>(text)))
							{
								this.RoundController.RoundWinner = component.Team.Side;
								roundEndReason = RoundEndReason.SideDepleted;
								captureTheFlagCaptureResultEnum = ((component.Team.Side == BattleSideEnum.Attacker) ? CaptureTheFlagCaptureResultEnum.AttackersWin : CaptureTheFlagCaptureResultEnum.DefendersWin);
								break;
							}
						}
					}
				}
			}
			if (captureTheFlagCaptureResultEnum != CaptureTheFlagCaptureResultEnum.NotCaptured)
			{
				this.RoundController.RoundEndReason = roundEndReason;
				this.HandleRoundEnd(captureTheFlagCaptureResultEnum);
			}
		}

		public override void OnAgentBuild(Agent agent, Banner banner)
		{
			agent.UpdateSyncHealthToAllClients(true);
			if (agent.IsPlayerControlled)
			{
				agent.MissionPeer.GetComponent<FlagDominationMissionRepresentative>().UpdateSelectedClassServer(agent);
			}
		}

		private void HandleRoundEnd(CaptureTheFlagCaptureResultEnum roundResult)
		{
			AgentVictoryLogic missionBehavior = base.Mission.GetMissionBehavior<AgentVictoryLogic>();
			if (missionBehavior == null)
			{
				Debug.FailedAssert("Agent victory logic should not be null after someone just won/lost!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Multiplayer\\MissionNetworkLogics\\MultiplayerGameModeLogics\\ServerGameModeLogics\\MissionMultiplayerFlagDomination.cs", "HandleRoundEnd", 771);
				return;
			}
			if (roundResult == CaptureTheFlagCaptureResultEnum.AttackersWin)
			{
				missionBehavior.SetTimersOfVictoryReactionsOnBattleEnd(BattleSideEnum.Attacker);
				return;
			}
			if (roundResult != CaptureTheFlagCaptureResultEnum.DefendersWin)
			{
				return;
			}
			missionBehavior.SetTimersOfVictoryReactionsOnBattleEnd(BattleSideEnum.Defender);
		}

		private void OnPostRoundEnd()
		{
			if (this.UseGold() && !this.RoundController.IsMatchEnding)
			{
				foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
				{
					MissionPeer component = networkCommunicator.GetComponent<MissionPeer>();
					if (component != null && this.RoundController.RoundCount > 0)
					{
						int num = MissionMultiplayerFlagDomination._defaultGoldAmountForTroopSelection;
						int num2 = base.GetCurrentGoldForPeer(component);
						if (num2 < 0)
						{
							num2 = MissionMultiplayerFlagDomination._maxGoldAmountToCarryOver;
						}
						else if (component.Team != null && component.Team.Side != BattleSideEnum.None && this.RoundController.RoundWinner == component.Team.Side && component.GetComponent<FlagDominationMissionRepresentative>().CheckIfSurvivedLastRoundAndReset())
						{
							num2 += MissionMultiplayerFlagDomination._maxGoldAmountToCarryOverForSurvival;
						}
						num += MBMath.ClampInt(num2, 0, MissionMultiplayerFlagDomination._maxGoldAmountToCarryOver);
						if (num > MissionMultiplayerFlagDomination._defaultGoldAmountForTroopSelection)
						{
							int num3 = num - MissionMultiplayerFlagDomination._defaultGoldAmountForTroopSelection;
							this.NotificationsComponent.GoldCarriedFromPreviousRound(num3, component.GetNetworkPeer());
						}
						base.ChangeCurrentGoldForPeer(component, num);
					}
				}
			}
		}

		protected override void HandleEarlyPlayerDisconnect(NetworkCommunicator networkPeer)
		{
			if (this.RoundController.IsRoundInProgress && MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) > 0)
			{
				this.MakePlayerFormationCharge(networkPeer);
			}
		}

		private void OnPreTeamChanged(NetworkCommunicator peer, Team currentTeam, Team newTeam)
		{
			if (peer.IsSynchronized && peer.GetComponent<MissionPeer>().ControlledAgent != null)
			{
				this.MakePlayerFormationCharge(peer);
			}
		}

		private void OnPreparationEnded()
		{
			if (this.UseGold())
			{
				List<MissionPeer>[] array = new List<MissionPeer>[2];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = new List<MissionPeer>();
				}
				foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
				{
					MissionPeer component = networkCommunicator.GetComponent<MissionPeer>();
					if (component != null && component.Team != null && component.Team.Side != BattleSideEnum.None)
					{
						array[(int)component.Team.Side].Add(component);
					}
				}
				int num = array[1].Count - array[0].Count;
				BattleSideEnum battleSideEnum = ((num == 0) ? BattleSideEnum.None : ((num < 0) ? BattleSideEnum.Attacker : BattleSideEnum.Defender));
				if (battleSideEnum != BattleSideEnum.None)
				{
					num = MathF.Abs(num);
					int count = array[(int)battleSideEnum].Count;
					if (count > 0)
					{
						int num2 = MissionMultiplayerFlagDomination._defaultGoldAmountForTroopSelection * num / 10 / count * 10;
						foreach (MissionPeer missionPeer in array[(int)battleSideEnum])
						{
							base.ChangeCurrentGoldForPeer(missionPeer, base.GetCurrentGoldForPeer(missionPeer) + num2);
						}
					}
				}
			}
		}

		private void CheckPlayerBeingDetached()
		{
			foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
			{
				if (networkCommunicator.IsSynchronized)
				{
					MissionPeer component = networkCommunicator.GetComponent<MissionPeer>();
					if (this.PlayerDistanceToFormation(component) >= component.CaptainBeingDetachedThreshold)
					{
						this.MakePlayerFormationFollowPlayer(component.GetNetworkPeer());
					}
				}
			}
		}

		private int PlayerDistanceToFormation(MissionPeer missionPeer)
		{
			float num = 0f;
			if (missionPeer != null && missionPeer.ControlledAgent != null && missionPeer.ControlledFormation != null)
			{
				float num2 = missionPeer.ControlledFormation.GetAveragePositionOfUnits(true, true).Distance(missionPeer.ControlledAgent.Position.AsVec2);
				float num3 = missionPeer.ControlledFormation.OrderPosition.Distance(missionPeer.ControlledAgent.Position.AsVec2);
				num += num2 + num3;
				if (missionPeer.ControlledFormation.PhysicalClass.IsMounted())
				{
					num *= 0.8f;
				}
			}
			return (int)num;
		}

		private void MakePlayerFormationFollowPlayer(NetworkCommunicator peer)
		{
			if (peer.IsSynchronized)
			{
				MissionPeer component = peer.GetComponent<MissionPeer>();
				if (component.ControlledFormation != null)
				{
					component.ControlledFormation.SetMovementOrder(MovementOrder.MovementOrderFollow(component.ControlledAgent));
					this.NotificationsComponent.FormationAutoFollowEnforced(peer);
				}
			}
		}

		private void MakePlayerFormationCharge(NetworkCommunicator peer)
		{
			if (peer.IsSynchronized)
			{
				MissionPeer component = peer.GetComponent<MissionPeer>();
				if (component.ControlledFormation != null)
				{
					component.ControlledFormation.SetMovementOrder(MovementOrder.MovementOrderCharge);
				}
			}
		}

		protected override void HandleEarlyNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
		{
			networkPeer.AddComponent<FlagDominationMissionRepresentative>();
		}

		protected override void HandleNewClientAfterSynchronized(NetworkCommunicator networkPeer)
		{
			if (this.UseGold())
			{
				int num = ((this._gameType == MultiplayerGameType.Battle) ? 200 : MissionMultiplayerFlagDomination._defaultGoldAmountForTroopSelection);
				int num2 = ((!this.RoundController.IsRoundInProgress) ? num : 0);
				base.ChangeCurrentGoldForPeer(networkPeer.GetComponent<MissionPeer>(), num2);
				MissionMultiplayerGameModeFlagDominationClient gameModeFlagDominationClient = this._gameModeFlagDominationClient;
				if (gameModeFlagDominationClient != null)
				{
					gameModeFlagDominationClient.OnGoldAmountChangedForRepresentative(networkPeer.GetComponent<FlagDominationMissionRepresentative>(), num2);
				}
			}
			if (this.AllCapturePoints != null && !networkPeer.IsServerPeer)
			{
				foreach (FlagCapturePoint flagCapturePoint in this.AllCapturePoints.Where((FlagCapturePoint cp) => !cp.IsDeactivated))
				{
					GameNetwork.BeginModuleEventAsServer(networkPeer);
					int flagIndex = flagCapturePoint.FlagIndex;
					Team team = this._capturePointOwners[flagCapturePoint.FlagIndex];
					GameNetwork.WriteMessage(new FlagDominationCapturePointMessage(flagIndex, (team != null) ? team.TeamIndex : (-1)));
					GameNetwork.EndModuleEventAsServer();
				}
			}
		}

		private bool HandleClientEventRequestForfeitSpawn(NetworkCommunicator peer, GameNetworkMessage baseMessage)
		{
			this.ForfeitSpawning(peer);
			return true;
		}

		public void ForfeitSpawning(NetworkCommunicator peer)
		{
			MissionPeer component = peer.GetComponent<MissionPeer>();
			if (component != null && component.HasSpawnedAgentVisuals && this.UseGold() && this.RoundController.IsRoundInProgress)
			{
				Mission.Current.GetMissionBehavior<MultiplayerMissionAgentVisualSpawnComponent>().RemoveAgentVisuals(component, true);
				if (GameNetwork.IsServerOrRecorder)
				{
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new RemoveAgentVisualsForPeer(component.GetNetworkPeer()));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
				}
				component.HasSpawnedAgentVisuals = false;
				base.ChangeCurrentGoldForPeer(component, -1);
			}
		}

		public static void SetWinnerTeam(int winnerTeamNo)
		{
			Mission mission = Mission.Current;
			MissionMultiplayerFlagDomination missionBehavior = mission.GetMissionBehavior<MissionMultiplayerFlagDomination>();
			if (missionBehavior != null)
			{
				foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
				{
					MissionPeer component = networkCommunicator.GetComponent<MissionPeer>();
					missionBehavior.ChangeCurrentGoldForPeer(component, 0);
				}
				for (int i = mission.Agents.Count - 1; i >= 0; i--)
				{
					Agent agent = mission.Agents[i];
					if (agent.IsHuman && agent.Team.MBTeam.Index != winnerTeamNo + 1)
					{
						Mission.Current.KillAgentCheat(agent);
					}
				}
			}
		}

		private void TickFlags()
		{
			foreach (FlagCapturePoint flagCapturePoint in this.AllCapturePoints)
			{
				if (!flagCapturePoint.IsDeactivated)
				{
					for (int i = 0; i < 2; i++)
					{
						this._agentCountsOnSide[i] = 0;
					}
					Team team = this._capturePointOwners[flagCapturePoint.FlagIndex];
					Agent agent = null;
					float num = 16f;
					AgentProximityMap.ProximityMapSearchStruct proximityMapSearchStruct = AgentProximityMap.BeginSearch(Mission.Current, flagCapturePoint.Position.AsVec2, 6f, false);
					while (proximityMapSearchStruct.LastFoundAgent != null)
					{
						Agent lastFoundAgent = proximityMapSearchStruct.LastFoundAgent;
						if (lastFoundAgent.IsHuman && lastFoundAgent.IsActive())
						{
							this._agentCountsOnSide[(int)lastFoundAgent.Team.Side]++;
							float num2 = lastFoundAgent.Position.DistanceSquared(flagCapturePoint.Position);
							if (num2 <= num)
							{
								agent = lastFoundAgent;
								num = num2;
							}
						}
						AgentProximityMap.FindNext(Mission.Current, ref proximityMapSearchStruct);
					}
					ValueTuple<int, int> valueTuple = ValueTuple.Create<int, int>(this._agentCountsOnSide[0], this._agentCountsOnSide[1]);
					bool flag = valueTuple.Item1 != this._defenderAttackerCountsInFlagArea[flagCapturePoint.FlagIndex].Item1 || valueTuple.Item2 != this._defenderAttackerCountsInFlagArea[flagCapturePoint.FlagIndex].Item2;
					this._defenderAttackerCountsInFlagArea[flagCapturePoint.FlagIndex] = valueTuple;
					bool isContested = flagCapturePoint.IsContested;
					float num3 = 1f;
					if (agent != null)
					{
						BattleSideEnum side = agent.Team.Side;
						BattleSideEnum oppositeSide = side.GetOppositeSide();
						if (this._agentCountsOnSide[(int)oppositeSide] != 0)
						{
							float num4 = (float)Math.Min(this._agentCountsOnSide[(int)side], 200);
							int num5 = Math.Min(this._agentCountsOnSide[(int)oppositeSide], 200);
							float num6 = (MathF.Log10(num4) + 1f) / (2f * (MathF.Log10((float)num5) + 1f)) - 0.09f;
							num3 = Math.Min(1f, num6);
						}
					}
					if (team == null)
					{
						if (!isContested && agent != null)
						{
							flagCapturePoint.SetMoveFlag(CaptureTheFlagFlagDirection.Down, num3);
						}
						else if (agent == null && isContested)
						{
							flagCapturePoint.SetMoveFlag(CaptureTheFlagFlagDirection.Up, num3);
						}
						else if (flag)
						{
							flagCapturePoint.ChangeMovementSpeed(num3);
						}
					}
					else if (agent != null)
					{
						if (agent.Team != team && !isContested)
						{
							flagCapturePoint.SetMoveFlag(CaptureTheFlagFlagDirection.Down, num3);
						}
						else if (agent.Team == team && isContested)
						{
							flagCapturePoint.SetMoveFlag(CaptureTheFlagFlagDirection.Up, num3);
						}
						else if (flag)
						{
							flagCapturePoint.ChangeMovementSpeed(num3);
						}
					}
					else if (isContested)
					{
						flagCapturePoint.SetMoveFlag(CaptureTheFlagFlagDirection.Up, num3);
					}
					else if (flag)
					{
						flagCapturePoint.ChangeMovementSpeed(num3);
					}
					bool flag2;
					flagCapturePoint.OnAfterTick(agent != null, out flag2);
					if (flag2)
					{
						Team team2 = agent.Team;
						uint num7 = ((team2 != null) ? team2.Color : 4284111450U);
						uint num8 = ((team2 != null) ? team2.Color2 : uint.MaxValue);
						flagCapturePoint.SetTeamColorsWithAllSynched(num7, num8);
						this._capturePointOwners[flagCapturePoint.FlagIndex] = team2;
						GameNetwork.BeginBroadcastModuleEvent();
						GameNetwork.WriteMessage(new FlagDominationCapturePointMessage(flagCapturePoint.FlagIndex, (team2 != null) ? team2.TeamIndex : (-1)));
						GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
						MissionMultiplayerGameModeFlagDominationClient gameModeFlagDominationClient = this._gameModeFlagDominationClient;
						if (gameModeFlagDominationClient != null)
						{
							gameModeFlagDominationClient.OnCapturePointOwnerChanged(flagCapturePoint, team2);
						}
						this.NotificationsComponent.FlagXCapturedByTeamX(flagCapturePoint, agent.Team);
						MPPerkObject.RaiseEventForAllPeers(MPPerkCondition.PerkEventFlags.FlagCapture);
					}
				}
			}
		}

		public int GetNumberOfAttackersAroundFlag(FlagCapturePoint capturePoint)
		{
			Team flagOwnerTeam = this.GetFlagOwnerTeam(capturePoint);
			if (flagOwnerTeam == null)
			{
				return 0;
			}
			int num = 0;
			AgentProximityMap.ProximityMapSearchStruct proximityMapSearchStruct = AgentProximityMap.BeginSearch(Mission.Current, capturePoint.Position.AsVec2, 6f, false);
			while (proximityMapSearchStruct.LastFoundAgent != null)
			{
				Agent lastFoundAgent = proximityMapSearchStruct.LastFoundAgent;
				if (lastFoundAgent.IsHuman && lastFoundAgent.IsActive() && lastFoundAgent.Position.DistanceSquared(capturePoint.Position) <= 36f && lastFoundAgent.Team.Side != flagOwnerTeam.Side)
				{
					num++;
				}
				AgentProximityMap.FindNext(Mission.Current, ref proximityMapSearchStruct);
			}
			return num;
		}

		public Team GetFlagOwnerTeam(FlagCapturePoint flag)
		{
			if (flag == null)
			{
				return null;
			}
			return this._capturePointOwners[flag.FlagIndex];
		}

		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
		{
			base.OnAgentRemoved(affectedAgent, affectorAgent, agentState, blow);
			if (this.UseGold() && affectorAgent != null && affectedAgent != null && affectedAgent.IsHuman && blow.DamageType != DamageTypes.Invalid && (agentState == AgentState.Unconscious || agentState == AgentState.Killed))
			{
				bool flag = affectorAgent.Team != null && affectedAgent.Team != null && affectorAgent.Team.Side == affectedAgent.Team.Side;
				Agent.Hitter assistingHitter = affectedAgent.GetAssistingHitter(affectorAgent.MissionPeer);
				MultiplayerClassDivisions.MPHeroClass mpheroClassForCharacter = MultiplayerClassDivisions.GetMPHeroClassForCharacter(affectedAgent.Character);
				FlagDominationMissionRepresentative flagDominationMissionRepresentative;
				if (affectorAgent.MissionPeer != null && (flagDominationMissionRepresentative = affectorAgent.MissionPeer.Representative as FlagDominationMissionRepresentative) != null)
				{
					int goldGainsFromKillData = flagDominationMissionRepresentative.GetGoldGainsFromKillData(MPPerkObject.GetPerkHandler(affectorAgent.MissionPeer), MPPerkObject.GetPerkHandler((assistingHitter != null) ? assistingHitter.HitterPeer : null), mpheroClassForCharacter, false, flag);
					if (goldGainsFromKillData > 0)
					{
						base.ChangeCurrentGoldForPeer(affectorAgent.MissionPeer, flagDominationMissionRepresentative.Gold + goldGainsFromKillData);
					}
				}
				FlagDominationMissionRepresentative flagDominationMissionRepresentative2;
				if (((assistingHitter != null) ? assistingHitter.HitterPeer : null) != null && assistingHitter.HitterPeer.Peer.Communicator.IsConnectionActive && !assistingHitter.IsFriendlyHit && (flagDominationMissionRepresentative2 = assistingHitter.HitterPeer.Representative as FlagDominationMissionRepresentative) != null)
				{
					int goldGainsFromKillData2 = flagDominationMissionRepresentative2.GetGoldGainsFromKillData(MPPerkObject.GetPerkHandler(affectorAgent.MissionPeer), MPPerkObject.GetPerkHandler(assistingHitter.HitterPeer), mpheroClassForCharacter, true, flag);
					if (goldGainsFromKillData2 > 0)
					{
						base.ChangeCurrentGoldForPeer(assistingHitter.HitterPeer, flagDominationMissionRepresentative2.Gold + goldGainsFromKillData2);
					}
				}
				MissionPeer missionPeer = affectedAgent.MissionPeer;
				if (((missionPeer != null) ? missionPeer.Team : null) != null && !flag)
				{
					MPPerkObject.MPPerkHandler perkHandler = MPPerkObject.GetPerkHandler(affectedAgent.MissionPeer);
					IEnumerable<ValueTuple<MissionPeer, int>> enumerable = ((perkHandler != null) ? perkHandler.GetTeamGoldRewardsOnDeath() : null);
					if (enumerable != null)
					{
						foreach (ValueTuple<MissionPeer, int> valueTuple in enumerable)
						{
							MissionPeer item = valueTuple.Item1;
							int item2 = valueTuple.Item2;
							FlagDominationMissionRepresentative flagDominationMissionRepresentative3;
							if (item2 > 0 && (flagDominationMissionRepresentative3 = ((item != null) ? item.Representative : null) as FlagDominationMissionRepresentative) != null)
							{
								int goldGainsFromAllyDeathReward = flagDominationMissionRepresentative3.GetGoldGainsFromAllyDeathReward(item2);
								if (goldGainsFromAllyDeathReward > 0)
								{
									base.ChangeCurrentGoldForPeer(item, flagDominationMissionRepresentative3.Gold + goldGainsFromAllyDeathReward);
								}
							}
						}
					}
				}
			}
			if (affectedAgent.IsPlayerControlled)
			{
				affectedAgent.MissionPeer.GetComponent<FlagDominationMissionRepresentative>().UpdateSelectedClassServer(null);
			}
			else if (MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) > 0 && (this.WarmupComponent == null || !this.WarmupComponent.IsInWarmup) && !affectedAgent.IsMount && affectedAgent.OwningAgentMissionPeer != null && affectedAgent.Formation != null && affectedAgent.Formation.CountOfUnits == 1)
			{
				if (!GameNetwork.IsDedicatedServer)
				{
					MatrixFrame cameraFrame = Mission.Current.GetCameraFrame();
					Vec3 vec = cameraFrame.origin + cameraFrame.rotation.u;
					MBSoundEvent.PlaySound(SoundEvent.GetEventIdFromString("event:/alerts/report/squad_wiped"), vec);
				}
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new FormationWipedMessage());
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.ExcludeOtherTeamPlayers, affectedAgent.OwningAgentMissionPeer.GetNetworkPeer());
			}
			if (this._gameType == MultiplayerGameType.Battle && affectedAgent.IsHuman && this.RoundController.IsRoundInProgress && blow.DamageType != DamageTypes.Invalid && (agentState == AgentState.Unconscious || agentState == AgentState.Killed))
			{
				MultiplayerClassDivisions.MPHeroClass mpheroClassForCharacter2 = MultiplayerClassDivisions.GetMPHeroClassForCharacter(affectedAgent.Character);
				if (((affectorAgent != null) ? affectorAgent.MissionPeer : null) != null && affectorAgent.Team != affectedAgent.Team)
				{
					FlagDominationMissionRepresentative flagDominationMissionRepresentative4 = affectorAgent.MissionPeer.Representative as FlagDominationMissionRepresentative;
					int goldGainFromKillDataAndUpdateFlags = flagDominationMissionRepresentative4.GetGoldGainFromKillDataAndUpdateFlags(mpheroClassForCharacter2, false);
					base.ChangeCurrentGoldForPeer(affectorAgent.MissionPeer, flagDominationMissionRepresentative4.Gold + goldGainFromKillDataAndUpdateFlags);
				}
				Agent.Hitter assistingHitter2 = affectedAgent.GetAssistingHitter((affectorAgent != null) ? affectorAgent.MissionPeer : null);
				if (((assistingHitter2 != null) ? assistingHitter2.HitterPeer : null) != null && !assistingHitter2.IsFriendlyHit)
				{
					FlagDominationMissionRepresentative flagDominationMissionRepresentative5 = assistingHitter2.HitterPeer.Representative as FlagDominationMissionRepresentative;
					int goldGainFromKillDataAndUpdateFlags2 = flagDominationMissionRepresentative5.GetGoldGainFromKillDataAndUpdateFlags(mpheroClassForCharacter2, true);
					base.ChangeCurrentGoldForPeer(assistingHitter2.HitterPeer, flagDominationMissionRepresentative5.Gold + goldGainFromKillDataAndUpdateFlags2);
				}
			}
		}

		public override float GetTroopNumberMultiplierForMissingPlayer(MissionPeer spawningPeer)
		{
			if (this._gameType == MultiplayerGameType.Captain)
			{
				List<MissionPeer>[] array = new List<MissionPeer>[2];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = new List<MissionPeer>();
				}
				foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
				{
					MissionPeer component = networkCommunicator.GetComponent<MissionPeer>();
					if (component != null && component.Team != null && component.Team.Side != BattleSideEnum.None)
					{
						array[(int)component.Team.Side].Add(component);
					}
				}
				int[] array2 = new int[]
				{
					0,
					MultiplayerOptions.OptionType.NumberOfBotsTeam1.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions)
				};
				array2[0] = MultiplayerOptions.OptionType.NumberOfBotsTeam2.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
				int num = array[1].Count + array2[1] - (array[0].Count + array2[0]);
				BattleSideEnum battleSideEnum = ((num == 0) ? BattleSideEnum.None : ((num < 0) ? BattleSideEnum.Attacker : BattleSideEnum.Defender));
				if (battleSideEnum == spawningPeer.Team.Side)
				{
					num = MathF.Abs(num);
					int num2 = array[(int)battleSideEnum].Count + array2[(int)battleSideEnum];
					return 1f + (float)num / (float)num2;
				}
			}
			return 1f;
		}

		protected override void HandleNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
		{
			if (!networkPeer.IsServerPeer)
			{
				GameNetwork.BeginModuleEventAsServer(networkPeer);
				GameNetwork.WriteMessage(new FlagDominationMoraleChangeMessage(this.MoraleRounded));
				GameNetwork.EndModuleEventAsServer();
			}
		}

		public const int NumberOfFlagsInGame = 3;

		public const float MoraleRoundPrecision = 0.01f;

		public const int DefaultGoldAmountForTroopSelectionForSkirmish = 300;

		public const int MaxGoldAmountToCarryOverForSkirmish = 80;

		private const int MaxGoldAmountToCarryOverForSurvivalForSkirmish = 30;

		public const int InitialGoldAmountForTroopSelectionForBattle = 200;

		public const int DefaultGoldAmountForTroopSelectionForBattle = 120;

		public const int MaxGoldAmountToCarryOverForBattle = 110;

		private const int MaxGoldAmountToCarryOverForSurvivalForBattle = 20;

		private const float MoraleGainOnTick = 0.000625f;

		private const float MoralePenaltyPercentageIfNoPointsCaptured = 0.1f;

		private const float MoraleTickTimeInSeconds = 0.25f;

		public const float TimeTillFlagRemovalForPriorInfoInSeconds = 30f;

		public const float PointRemovalTimeInSecondsForBattle = 210f;

		public const float PointRemovalTimeInSecondsForCaptain = 180f;

		public const float PointRemovalTimeInSecondsForSkirmish = 120f;

		public const float MoraleMultiplierForEachFlagForBattle = 0.75f;

		public const float MoraleMultiplierForEachFlagForCaptain = 1f;

		private const float MoraleMultiplierOnLastFlagForBattle = 3.5f;

		private static int _defaultGoldAmountForTroopSelection = -1;

		private static int _maxGoldAmountToCarryOver = -1;

		private static int _maxGoldAmountToCarryOverForSurvival = -1;

		private const float MoraleMultiplierOnLastFlagForCaptainSkirmish = 2f;

		public const float MoraleMultiplierForEachFlagForSkirmish = 2f;

		private readonly float _pointRemovalTimeInSeconds = -1f;

		private readonly float _moraleMultiplierForEachFlag = -1f;

		private readonly float _moraleMultiplierOnLastFlag = -1f;

		private Team[] _capturePointOwners;

		private bool _flagRemovalOccured;

		private float _nextTimeToCheckForPointRemoval = float.MinValue;

		private MissionMultiplayerGameModeFlagDominationClient _gameModeFlagDominationClient;

		private float _morale;

		private readonly MultiplayerGameType _gameType;

		private int[] _agentCountsOnSide = new int[2];

		private ValueTuple<int, int>[] _defenderAttackerCountsInFlagArea = new ValueTuple<int, int>[3];
	}
}
