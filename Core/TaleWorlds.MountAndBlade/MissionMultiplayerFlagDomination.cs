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
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002A0 RID: 672
	public class MissionMultiplayerFlagDomination : MissionMultiplayerGameModeBase, IAnalyticsFlagInfo, IMissionBehavior
	{
		// Token: 0x170006D5 RID: 1749
		// (get) Token: 0x060024BF RID: 9407 RVA: 0x000883F9 File Offset: 0x000865F9
		public override bool IsGameModeHidingAllAgentVisuals
		{
			get
			{
				return this._gameType == MissionLobbyComponent.MultiplayerGameType.Captain || this._gameType == MissionLobbyComponent.MultiplayerGameType.Battle;
			}
		}

		// Token: 0x170006D6 RID: 1750
		// (get) Token: 0x060024C0 RID: 9408 RVA: 0x0008840F File Offset: 0x0008660F
		public override bool IsGameModeUsingOpposingTeams
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170006D7 RID: 1751
		// (get) Token: 0x060024C1 RID: 9409 RVA: 0x00088412 File Offset: 0x00086612
		// (set) Token: 0x060024C2 RID: 9410 RVA: 0x0008841A File Offset: 0x0008661A
		public MBReadOnlyList<FlagCapturePoint> AllCapturePoints { get; private set; }

		// Token: 0x170006D8 RID: 1752
		// (get) Token: 0x060024C3 RID: 9411 RVA: 0x00088423 File Offset: 0x00086623
		public float MoraleRounded
		{
			get
			{
				return (float)((int)(this._morale / 0.01f)) * 0.01f;
			}
		}

		// Token: 0x170006D9 RID: 1753
		// (get) Token: 0x060024C4 RID: 9412 RVA: 0x00088439 File Offset: 0x00086639
		public bool GameModeUsesSingleSpawning
		{
			get
			{
				return this.GetMissionType() == MissionLobbyComponent.MultiplayerGameType.Captain || this.GetMissionType() == MissionLobbyComponent.MultiplayerGameType.Battle;
			}
		}

		// Token: 0x060024C5 RID: 9413 RVA: 0x0008844F File Offset: 0x0008664F
		public bool UseGold()
		{
			return this._gameModeFlagDominationClient.IsGameModeUsingGold;
		}

		// Token: 0x060024C6 RID: 9414 RVA: 0x0008845C File Offset: 0x0008665C
		public override bool AllowCustomPlayerBanners()
		{
			return false;
		}

		// Token: 0x060024C7 RID: 9415 RVA: 0x0008845F File Offset: 0x0008665F
		public override bool UseRoundController()
		{
			return true;
		}

		// Token: 0x060024C8 RID: 9416 RVA: 0x00088464 File Offset: 0x00086664
		public MissionMultiplayerFlagDomination(MissionLobbyComponent.MultiplayerGameType gameType)
		{
			this._gameType = gameType;
			switch (this._gameType)
			{
			case MissionLobbyComponent.MultiplayerGameType.Battle:
				this._moraleMultiplierForEachFlag = 0.75f;
				this._pointRemovalTimeInSeconds = 210f;
				this._moraleMultiplierOnLastFlag = 3.5f;
				MissionMultiplayerFlagDomination._defaultGoldAmountForTroopSelection = 120;
				MissionMultiplayerFlagDomination._maxGoldAmountToCarryOver = 110;
				MissionMultiplayerFlagDomination._maxGoldAmountToCarryOverForSurvival = 20;
				return;
			case MissionLobbyComponent.MultiplayerGameType.Captain:
				this._moraleMultiplierForEachFlag = 1f;
				this._pointRemovalTimeInSeconds = 180f;
				this._moraleMultiplierOnLastFlag = 2f;
				return;
			case MissionLobbyComponent.MultiplayerGameType.Skirmish:
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

		// Token: 0x060024C9 RID: 9417 RVA: 0x00088570 File Offset: 0x00086770
		public override MissionLobbyComponent.MultiplayerGameType GetMissionType()
		{
			return this._gameType;
		}

		// Token: 0x060024CA RID: 9418 RVA: 0x00088578 File Offset: 0x00086778
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

		// Token: 0x060024CB RID: 9419 RVA: 0x00088628 File Offset: 0x00086828
		public override void AfterStart()
		{
			base.AfterStart();
			this.RoundController.OnRoundStarted += this.OnPreparationStart;
			MissionPeer.OnPreTeamChanged += this.OnPreTeamChanged;
			this.RoundController.OnPreparationEnded += this.OnPreparationEnded;
			this.WarmupComponent.OnWarmupEnding += this.OnWarmupEnding;
			this.RoundController.OnPreRoundEnding += this.OnRoundEnd;
			this.RoundController.OnPostRoundEnded += this.OnPostRoundEnd;
			BasicCultureObject @object = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam1.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
			BasicCultureObject object2 = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam2.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
			Banner banner = new Banner(@object.BannerKey, @object.BackgroundColor1, @object.ForegroundColor1);
			Banner banner2 = new Banner(object2.BannerKey, object2.BackgroundColor2, object2.ForegroundColor2);
			base.Mission.Teams.Add(BattleSideEnum.Attacker, @object.BackgroundColor1, @object.ForegroundColor1, banner, false, true, true);
			base.Mission.Teams.Add(BattleSideEnum.Defender, object2.BackgroundColor2, object2.ForegroundColor2, banner2, false, true, true);
		}

		// Token: 0x060024CC RID: 9420 RVA: 0x00088759 File Offset: 0x00086959
		protected override void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
		{
			registerer.Register<RequestForfeitSpawn>(new GameNetworkMessage.ClientMessageHandlerDelegate<RequestForfeitSpawn>(this.HandleClientEventRequestForfeitSpawn));
		}

		// Token: 0x060024CD RID: 9421 RVA: 0x00088770 File Offset: 0x00086970
		public override void OnRemoveBehavior()
		{
			this.RoundController.OnRoundStarted -= this.OnPreparationStart;
			MissionPeer.OnPreTeamChanged -= this.OnPreTeamChanged;
			this.RoundController.OnPreparationEnded -= this.OnPreparationEnded;
			this.WarmupComponent.OnWarmupEnding -= this.OnWarmupEnding;
			this.RoundController.OnPreRoundEnding -= this.OnRoundEnd;
			this.RoundController.OnPostRoundEnded -= this.OnPostRoundEnd;
			base.OnRemoveBehavior();
		}

		// Token: 0x060024CE RID: 9422 RVA: 0x00088807 File Offset: 0x00086A07
		public override void OnPeerChangedTeam(NetworkCommunicator peer, Team oldTeam, Team newTeam)
		{
			if (oldTeam != null && oldTeam != newTeam && this.UseGold() && (this.WarmupComponent == null || !this.WarmupComponent.IsInWarmup))
			{
				base.ChangeCurrentGoldForPeer(peer.GetComponent<MissionPeer>(), 0);
			}
		}

		// Token: 0x060024CF RID: 9423 RVA: 0x0008883A File Offset: 0x00086A3A
		private void OnPreparationStart()
		{
			this.NotificationsComponent.PreparationStarted();
		}

		// Token: 0x060024D0 RID: 9424 RVA: 0x00088848 File Offset: 0x00086A48
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

		// Token: 0x060024D1 RID: 9425 RVA: 0x000888B0 File Offset: 0x00086AB0
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

		// Token: 0x060024D2 RID: 9426 RVA: 0x00088A70 File Offset: 0x00086C70
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

		// Token: 0x060024D3 RID: 9427 RVA: 0x00088B3C File Offset: 0x00086D3C
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

		// Token: 0x060024D4 RID: 9428 RVA: 0x00088BD0 File Offset: 0x00086DD0
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

		// Token: 0x060024D5 RID: 9429 RVA: 0x00088C4C File Offset: 0x00086E4C
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

		// Token: 0x060024D6 RID: 9430 RVA: 0x00089168 File Offset: 0x00087368
		private int RemoveCapturePoint(FlagCapturePoint capToRemove)
		{
			int flagIndex = capToRemove.FlagIndex;
			capToRemove.RemovePointAsServer();
			GameNetwork.BeginBroadcastModuleEvent();
			GameNetwork.WriteMessage(new FlagDominationCapturePointMessage(flagIndex, null));
			GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
			return flagIndex;
		}

		// Token: 0x060024D7 RID: 9431 RVA: 0x00089190 File Offset: 0x00087390
		public override void OnClearScene()
		{
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

		// Token: 0x060024D8 RID: 9432 RVA: 0x00089230 File Offset: 0x00087430
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

		// Token: 0x060024D9 RID: 9433 RVA: 0x000892A8 File Offset: 0x000874A8
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

		// Token: 0x060024DA RID: 9434 RVA: 0x00089348 File Offset: 0x00087548
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

		// Token: 0x060024DB RID: 9435 RVA: 0x00089520 File Offset: 0x00087720
		public override bool UseCultureSelection()
		{
			return false;
		}

		// Token: 0x060024DC RID: 9436 RVA: 0x00089523 File Offset: 0x00087723
		private void OnWarmupEnding()
		{
			this.NotificationsComponent.WarmupEnding();
		}

		// Token: 0x060024DD RID: 9437 RVA: 0x00089530 File Offset: 0x00087730
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

		// Token: 0x060024DE RID: 9438 RVA: 0x000897C8 File Offset: 0x000879C8
		public override void OnAgentBuild(Agent agent, Banner banner)
		{
			agent.UpdateSyncHealthToAllClients(true);
			if (agent.IsPlayerControlled)
			{
				agent.MissionPeer.GetComponent<FlagDominationMissionRepresentative>().UpdateSelectedClassServer(agent);
			}
		}

		// Token: 0x060024DF RID: 9439 RVA: 0x000897EC File Offset: 0x000879EC
		private void HandleRoundEnd(CaptureTheFlagCaptureResultEnum roundResult)
		{
			AgentVictoryLogic missionBehavior = base.Mission.GetMissionBehavior<AgentVictoryLogic>();
			if (missionBehavior == null)
			{
				Debug.FailedAssert("Agent victory logic should not be null after someone just won/lost!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Multiplayer\\MissionNetworkLogics\\MultiplayerGameModeLogics\\ServerGameModeLogics\\MissionMultiplayerFlagDomination.cs", "HandleRoundEnd", 761);
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

		// Token: 0x060024E0 RID: 9440 RVA: 0x0008983C File Offset: 0x00087A3C
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

		// Token: 0x060024E1 RID: 9441 RVA: 0x00089958 File Offset: 0x00087B58
		protected override void HandleEarlyPlayerDisconnect(NetworkCommunicator networkPeer)
		{
			if (this.RoundController.IsRoundInProgress && MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) > 0)
			{
				this.MakePlayerFormationCharge(networkPeer);
			}
		}

		// Token: 0x060024E2 RID: 9442 RVA: 0x00089979 File Offset: 0x00087B79
		private void OnPreTeamChanged(NetworkCommunicator peer, Team currentTeam, Team newTeam)
		{
			if (peer.IsSynchronized && peer.GetComponent<MissionPeer>().ControlledAgent != null)
			{
				this.MakePlayerFormationCharge(peer);
			}
		}

		// Token: 0x060024E3 RID: 9443 RVA: 0x00089998 File Offset: 0x00087B98
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

		// Token: 0x060024E4 RID: 9444 RVA: 0x00089ADC File Offset: 0x00087CDC
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

		// Token: 0x060024E5 RID: 9445 RVA: 0x00089B4C File Offset: 0x00087D4C
		private int PlayerDistanceToFormation(MissionPeer missionPeer)
		{
			float num = 0f;
			if (missionPeer != null && missionPeer.ControlledAgent != null && missionPeer.ControlledFormation != null)
			{
				float num2 = missionPeer.ControlledFormation.GetAveragePositionOfUnits(true, true).Distance(missionPeer.ControlledAgent.Position.AsVec2);
				float num3 = missionPeer.ControlledFormation.OrderPosition.Distance(missionPeer.ControlledAgent.Position.AsVec2);
				num += num2 + num3;
				if (missionPeer.ControlledFormation.IsCavalry())
				{
					num *= 0.8f;
				}
			}
			return (int)num;
		}

		// Token: 0x060024E6 RID: 9446 RVA: 0x00089BE4 File Offset: 0x00087DE4
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

		// Token: 0x060024E7 RID: 9447 RVA: 0x00089C2C File Offset: 0x00087E2C
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

		// Token: 0x060024E8 RID: 9448 RVA: 0x00089C60 File Offset: 0x00087E60
		protected override void HandleEarlyNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
		{
			networkPeer.AddComponent<FlagDominationMissionRepresentative>();
		}

		// Token: 0x060024E9 RID: 9449 RVA: 0x00089C6C File Offset: 0x00087E6C
		protected override void HandleNewClientAfterSynchronized(NetworkCommunicator networkPeer)
		{
			if (this.UseGold())
			{
				int num = ((this._gameType == MissionLobbyComponent.MultiplayerGameType.Battle) ? 200 : MissionMultiplayerFlagDomination._defaultGoldAmountForTroopSelection);
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
					GameNetwork.WriteMessage(new FlagDominationCapturePointMessage(flagCapturePoint.FlagIndex, this._capturePointOwners[flagCapturePoint.FlagIndex]));
					GameNetwork.EndModuleEventAsServer();
				}
			}
		}

		// Token: 0x060024EA RID: 9450 RVA: 0x00089D64 File Offset: 0x00087F64
		private bool HandleClientEventRequestForfeitSpawn(NetworkCommunicator peer, RequestForfeitSpawn message)
		{
			this.ForfeitSpawning(peer);
			return true;
		}

		// Token: 0x060024EB RID: 9451 RVA: 0x00089D70 File Offset: 0x00087F70
		public void ForfeitSpawning(NetworkCommunicator peer)
		{
			MissionPeer component = peer.GetComponent<MissionPeer>();
			if (component != null && component.HasSpawnedAgentVisuals && this.UseGold() && this.RoundController.IsRoundInProgress)
			{
				Mission.Current.GetMissionBehavior<MultiplayerMissionAgentVisualSpawnComponent>().RemoveAgentVisuals(component, true);
				base.ChangeCurrentGoldForPeer(component, -1);
			}
		}

		// Token: 0x060024EC RID: 9452 RVA: 0x00089DC0 File Offset: 0x00087FC0
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

		// Token: 0x060024ED RID: 9453 RVA: 0x00089E7C File Offset: 0x0008807C
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
						GameNetwork.WriteMessage(new FlagDominationCapturePointMessage(flagCapturePoint.FlagIndex, team2));
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

		// Token: 0x060024EE RID: 9454 RVA: 0x0008A1E8 File Offset: 0x000883E8
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

		// Token: 0x060024EF RID: 9455 RVA: 0x0008A28A File Offset: 0x0008848A
		public Team GetFlagOwnerTeam(FlagCapturePoint flag)
		{
			if (flag == null)
			{
				return null;
			}
			return this._capturePointOwners[flag.FlagIndex];
		}

		// Token: 0x060024F0 RID: 9456 RVA: 0x0008A2A0 File Offset: 0x000884A0
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
			if (this._gameType == MissionLobbyComponent.MultiplayerGameType.Battle && affectedAgent.IsHuman && this.RoundController.IsRoundInProgress && blow.DamageType != DamageTypes.Invalid && (agentState == AgentState.Unconscious || agentState == AgentState.Killed))
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

		// Token: 0x060024F1 RID: 9457 RVA: 0x0008A6A0 File Offset: 0x000888A0
		public override float GetTroopNumberMultiplierForMissingPlayer(MissionPeer spawningPeer)
		{
			if (this._gameType == MissionLobbyComponent.MultiplayerGameType.Captain)
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

		// Token: 0x060024F2 RID: 9458 RVA: 0x0008A7CC File Offset: 0x000889CC
		protected override void HandleNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
		{
			if (!networkPeer.IsServerPeer)
			{
				GameNetwork.BeginModuleEventAsServer(networkPeer);
				GameNetwork.WriteMessage(new FlagDominationMoraleChangeMessage(this.MoraleRounded));
				GameNetwork.EndModuleEventAsServer();
			}
		}

		// Token: 0x04000D6E RID: 3438
		public const int NumberOfFlagsInGame = 3;

		// Token: 0x04000D6F RID: 3439
		public const float MoraleRoundPrecision = 0.01f;

		// Token: 0x04000D70 RID: 3440
		public const int DefaultGoldAmountForTroopSelectionForSkirmish = 300;

		// Token: 0x04000D71 RID: 3441
		public const int MaxGoldAmountToCarryOverForSkirmish = 80;

		// Token: 0x04000D72 RID: 3442
		private const int MaxGoldAmountToCarryOverForSurvivalForSkirmish = 30;

		// Token: 0x04000D73 RID: 3443
		public const int InitialGoldAmountForTroopSelectionForBattle = 200;

		// Token: 0x04000D74 RID: 3444
		public const int DefaultGoldAmountForTroopSelectionForBattle = 120;

		// Token: 0x04000D75 RID: 3445
		public const int MaxGoldAmountToCarryOverForBattle = 110;

		// Token: 0x04000D76 RID: 3446
		private const int MaxGoldAmountToCarryOverForSurvivalForBattle = 20;

		// Token: 0x04000D77 RID: 3447
		private const float MoraleGainOnTick = 0.000625f;

		// Token: 0x04000D78 RID: 3448
		private const float MoralePenaltyPercentageIfNoPointsCaptured = 0.1f;

		// Token: 0x04000D79 RID: 3449
		private const float MoraleTickTimeInSeconds = 0.25f;

		// Token: 0x04000D7A RID: 3450
		public const float TimeTillFlagRemovalForPriorInfoInSeconds = 30f;

		// Token: 0x04000D7B RID: 3451
		public const float PointRemovalTimeInSecondsForBattle = 210f;

		// Token: 0x04000D7C RID: 3452
		public const float PointRemovalTimeInSecondsForCaptain = 180f;

		// Token: 0x04000D7D RID: 3453
		public const float PointRemovalTimeInSecondsForSkirmish = 120f;

		// Token: 0x04000D7E RID: 3454
		public const float MoraleMultiplierForEachFlagForBattle = 0.75f;

		// Token: 0x04000D7F RID: 3455
		public const float MoraleMultiplierForEachFlagForCaptain = 1f;

		// Token: 0x04000D80 RID: 3456
		private const float MoraleMultiplierOnLastFlagForBattle = 3.5f;

		// Token: 0x04000D81 RID: 3457
		private static int _defaultGoldAmountForTroopSelection = -1;

		// Token: 0x04000D82 RID: 3458
		private static int _maxGoldAmountToCarryOver = -1;

		// Token: 0x04000D83 RID: 3459
		private static int _maxGoldAmountToCarryOverForSurvival = -1;

		// Token: 0x04000D84 RID: 3460
		private const float MoraleMultiplierOnLastFlagForCaptainSkirmish = 2f;

		// Token: 0x04000D85 RID: 3461
		public const float MoraleMultiplierForEachFlagForSkirmish = 2f;

		// Token: 0x04000D86 RID: 3462
		private readonly float _pointRemovalTimeInSeconds = -1f;

		// Token: 0x04000D87 RID: 3463
		private readonly float _moraleMultiplierForEachFlag = -1f;

		// Token: 0x04000D88 RID: 3464
		private readonly float _moraleMultiplierOnLastFlag = -1f;

		// Token: 0x04000D89 RID: 3465
		private Team[] _capturePointOwners;

		// Token: 0x04000D8B RID: 3467
		private bool _flagRemovalOccured;

		// Token: 0x04000D8C RID: 3468
		private float _nextTimeToCheckForPointRemoval = float.MinValue;

		// Token: 0x04000D8D RID: 3469
		private MissionMultiplayerGameModeFlagDominationClient _gameModeFlagDominationClient;

		// Token: 0x04000D8E RID: 3470
		private float _morale;

		// Token: 0x04000D8F RID: 3471
		private readonly MissionLobbyComponent.MultiplayerGameType _gameType;

		// Token: 0x04000D90 RID: 3472
		private int[] _agentCountsOnSide = new int[2];

		// Token: 0x04000D91 RID: 3473
		private ValueTuple<int, int>[] _defenderAttackerCountsInFlagArea = new ValueTuple<int, int>[3];
	}
}
