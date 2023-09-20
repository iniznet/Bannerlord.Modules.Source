using System;
using System.Collections.Generic;
using System.Linq;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.MissionRepresentatives;
using TaleWorlds.MountAndBlade.Objects;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002A2 RID: 674
	public class MissionMultiplayerSiege : MissionMultiplayerGameModeBase, IAnalyticsFlagInfo, IMissionBehavior
	{
		// Token: 0x170006DF RID: 1759
		// (get) Token: 0x0600251A RID: 9498 RVA: 0x0008ACCC File Offset: 0x00088ECC
		public override bool IsGameModeHidingAllAgentVisuals
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170006E0 RID: 1760
		// (get) Token: 0x0600251B RID: 9499 RVA: 0x0008ACCF File Offset: 0x00088ECF
		public override bool IsGameModeUsingOpposingTeams
		{
			get
			{
				return true;
			}
		}

		// Token: 0x14000055 RID: 85
		// (add) Token: 0x0600251C RID: 9500 RVA: 0x0008ACD4 File Offset: 0x00088ED4
		// (remove) Token: 0x0600251D RID: 9501 RVA: 0x0008AD0C File Offset: 0x00088F0C
		public event MissionMultiplayerSiege.OnDestructableComponentDestroyedDelegate OnDestructableComponentDestroyed;

		// Token: 0x14000056 RID: 86
		// (add) Token: 0x0600251E RID: 9502 RVA: 0x0008AD44 File Offset: 0x00088F44
		// (remove) Token: 0x0600251F RID: 9503 RVA: 0x0008AD7C File Offset: 0x00088F7C
		public event MissionMultiplayerSiege.OnObjectiveGoldGainedDelegate OnObjectiveGoldGained;

		// Token: 0x170006E1 RID: 1761
		// (get) Token: 0x06002520 RID: 9504 RVA: 0x0008ADB1 File Offset: 0x00088FB1
		// (set) Token: 0x06002521 RID: 9505 RVA: 0x0008ADB9 File Offset: 0x00088FB9
		public MBReadOnlyList<FlagCapturePoint> AllCapturePoints { get; private set; }

		// Token: 0x06002522 RID: 9506 RVA: 0x0008ADC4 File Offset: 0x00088FC4
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._objectiveSystem = new MissionMultiplayerSiege.ObjectiveSystem();
			this._childDestructableComponents = new Dictionary<GameEntity, List<DestructableComponent>>();
			this._gameModeSiegeClient = Mission.Current.GetMissionBehavior<MissionMultiplayerSiegeClient>();
			this._warmupComponent = Mission.Current.GetMissionBehavior<MultiplayerWarmupComponent>();
			this._capturePointOwners = new Team[7];
			this._capturePointRemainingMoraleGains = new int[7];
			this._morales = new int[2];
			this._morales[1] = 360;
			this._morales[0] = 360;
			this.AllCapturePoints = Mission.Current.MissionObjects.FindAllWithType<FlagCapturePoint>().ToMBList<FlagCapturePoint>();
			foreach (FlagCapturePoint flagCapturePoint in this.AllCapturePoints)
			{
				flagCapturePoint.SetTeamColorsSynched(4284111450U, uint.MaxValue);
				this._capturePointOwners[flagCapturePoint.FlagIndex] = null;
				this._capturePointRemainingMoraleGains[flagCapturePoint.FlagIndex] = 90;
				if (flagCapturePoint.GameEntity.HasTag("keep_capture_point"))
				{
					this._masterFlag = flagCapturePoint;
				}
			}
			foreach (DestructableComponent destructableComponent in Mission.Current.MissionObjects.FindAllWithType<DestructableComponent>())
			{
				if (destructableComponent.BattleSide != BattleSideEnum.None)
				{
					GameEntity root = destructableComponent.GameEntity.Root;
					if (this._objectiveSystem.RegisterObjective(root))
					{
						this._childDestructableComponents.Add(root, new List<DestructableComponent>());
						MissionMultiplayerSiege.GetDestructableCompoenentClosestToTheRoot(root).OnDestroyed += new DestructableComponent.OnHitTakenAndDestroyedDelegate(this.DestructableComponentOnDestroyed);
					}
					this._childDestructableComponents[root].Add(destructableComponent);
					destructableComponent.OnHitTaken += new DestructableComponent.OnHitTakenAndDestroyedDelegate(this.DestructableComponentOnHitTaken);
				}
			}
			List<RangedSiegeWeapon> list = new List<RangedSiegeWeapon>();
			List<IMoveableSiegeWeapon> list2 = new List<IMoveableSiegeWeapon>();
			foreach (UsableMachine usableMachine in Mission.Current.MissionObjects.FindAllWithType<UsableMachine>())
			{
				RangedSiegeWeapon rangedSiegeWeapon;
				IMoveableSiegeWeapon moveableSiegeWeapon;
				if ((rangedSiegeWeapon = usableMachine as RangedSiegeWeapon) != null)
				{
					list.Add(rangedSiegeWeapon);
					rangedSiegeWeapon.OnAgentLoadsMachine += this.RangedSiegeMachineOnAgentLoadsMachine;
				}
				else if ((moveableSiegeWeapon = usableMachine as IMoveableSiegeWeapon) != null)
				{
					list2.Add(moveableSiegeWeapon);
					this._objectiveSystem.RegisterObjective(usableMachine.GameEntity.Root);
				}
			}
			this._lastReloadingAgentPerRangedSiegeMachine = new ValueTuple<RangedSiegeWeapon, Agent>[list.Count];
			for (int i = 0; i < this._lastReloadingAgentPerRangedSiegeMachine.Length; i++)
			{
				this._lastReloadingAgentPerRangedSiegeMachine[i] = ValueTuple.Create<RangedSiegeWeapon, Agent>(list[i], null);
			}
			this._movingObjectives = new ValueTuple<IMoveableSiegeWeapon, Vec3>[list2.Count];
			for (int j = 0; j < this._movingObjectives.Length; j++)
			{
				SiegeWeapon siegeWeapon = list2[j] as SiegeWeapon;
				this._movingObjectives[j] = ValueTuple.Create<IMoveableSiegeWeapon, Vec3>(list2[j], siegeWeapon.GameEntity.GlobalPosition);
			}
		}

		// Token: 0x06002523 RID: 9507 RVA: 0x0008B0F0 File Offset: 0x000892F0
		private static DestructableComponent GetDestructableCompoenentClosestToTheRoot(GameEntity entity)
		{
			DestructableComponent destructableComponent = entity.GetFirstScriptOfType<DestructableComponent>();
			while (destructableComponent == null && entity.ChildCount != 0)
			{
				for (int i = 0; i < entity.ChildCount; i++)
				{
					destructableComponent = MissionMultiplayerSiege.GetDestructableCompoenentClosestToTheRoot(entity.GetChild(i));
					if (destructableComponent != null)
					{
						break;
					}
				}
			}
			return destructableComponent;
		}

		// Token: 0x06002524 RID: 9508 RVA: 0x0008B134 File Offset: 0x00089334
		private void RangedSiegeMachineOnAgentLoadsMachine(RangedSiegeWeapon siegeWeapon, Agent reloadingAgent)
		{
			for (int i = 0; i < this._lastReloadingAgentPerRangedSiegeMachine.Length; i++)
			{
				if (this._lastReloadingAgentPerRangedSiegeMachine[i].Item1 == siegeWeapon)
				{
					this._lastReloadingAgentPerRangedSiegeMachine[i].Item2 = reloadingAgent;
				}
			}
		}

		// Token: 0x06002525 RID: 9509 RVA: 0x0008B17C File Offset: 0x0008937C
		private void DestructableComponentOnHitTaken(DestructableComponent destructableComponent, Agent attackerAgent, in MissionWeapon weapon, ScriptComponentBehavior attackerScriptComponentBehavior, int inflictedDamage)
		{
			if (!this.WarmupComponent.IsInWarmup)
			{
				GameEntity root = destructableComponent.GameEntity.Root;
				BatteringRam batteringRam;
				if ((batteringRam = attackerScriptComponentBehavior as BatteringRam) != null)
				{
					int userCountNotInStruckAction = batteringRam.UserCountNotInStruckAction;
					if (userCountNotInStruckAction <= 0)
					{
						goto IL_227;
					}
					float num = (float)inflictedDamage / (float)userCountNotInStruckAction;
					using (List<StandingPoint>.Enumerator enumerator = batteringRam.StandingPoints.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							StandingPoint standingPoint = enumerator.Current;
							Agent userAgent = standingPoint.UserAgent;
							if (((userAgent != null) ? userAgent.MissionPeer : null) != null && !userAgent.IsInBeingStruckAction && userAgent.MissionPeer.Team.Side == destructableComponent.BattleSide.GetOppositeSide())
							{
								this._objectiveSystem.AddContributionForObjective(root, userAgent.MissionPeer, num);
							}
						}
						goto IL_227;
					}
				}
				bool flag;
				if (attackerAgent == null)
				{
					flag = null != null;
				}
				else
				{
					MissionPeer missionPeer = attackerAgent.MissionPeer;
					flag = ((missionPeer != null) ? missionPeer.Team : null) != null;
				}
				if (flag && attackerAgent.MissionPeer.Team.Side == destructableComponent.BattleSide.GetOppositeSide())
				{
					StandingPoint standingPoint2;
					if (attackerAgent.CurrentlyUsedGameObject != null && (standingPoint2 = attackerAgent.CurrentlyUsedGameObject as StandingPoint) != null)
					{
						RangedSiegeWeapon firstScriptOfTypeInFamily = standingPoint2.GameEntity.GetFirstScriptOfTypeInFamily<RangedSiegeWeapon>();
						if (firstScriptOfTypeInFamily != null)
						{
							for (int i = 0; i < this._lastReloadingAgentPerRangedSiegeMachine.Length; i++)
							{
								if (this._lastReloadingAgentPerRangedSiegeMachine[i].Item1 == firstScriptOfTypeInFamily)
								{
									Agent item = this._lastReloadingAgentPerRangedSiegeMachine[i].Item2;
									if (((item != null) ? item.MissionPeer : null) != null)
									{
										Agent item2 = this._lastReloadingAgentPerRangedSiegeMachine[i].Item2;
										BattleSideEnum? battleSideEnum = ((item2 != null) ? new BattleSideEnum?(item2.MissionPeer.Team.Side) : null);
										BattleSideEnum oppositeSide = destructableComponent.BattleSide.GetOppositeSide();
										if ((battleSideEnum.GetValueOrDefault() == oppositeSide) & (battleSideEnum != null))
										{
											this._objectiveSystem.AddContributionForObjective(root, this._lastReloadingAgentPerRangedSiegeMachine[i].Item2.MissionPeer, (float)inflictedDamage * 0.33f);
										}
									}
								}
							}
						}
					}
					this._objectiveSystem.AddContributionForObjective(root, attackerAgent.MissionPeer, (float)inflictedDamage);
				}
				IL_227:
				if (destructableComponent.IsDestroyed)
				{
					destructableComponent.OnHitTaken -= new DestructableComponent.OnHitTakenAndDestroyedDelegate(this.DestructableComponentOnHitTaken);
					this._childDestructableComponents[root].Remove(destructableComponent);
				}
			}
		}

		// Token: 0x06002526 RID: 9510 RVA: 0x0008B3F0 File Offset: 0x000895F0
		private void DestructableComponentOnDestroyed(DestructableComponent destructableComponent, Agent attackerAgent, in MissionWeapon weapon, ScriptComponentBehavior attackerScriptComponentBehavior, int inflictedDamage)
		{
			GameEntity root = destructableComponent.GameEntity.Root;
			List<KeyValuePair<MissionPeer, float>> allContributorsForSideAndClear = this._objectiveSystem.GetAllContributorsForSideAndClear(root, destructableComponent.BattleSide.GetOppositeSide());
			float num = allContributorsForSideAndClear.Sum((KeyValuePair<MissionPeer, float> ac) => ac.Value);
			List<MissionPeer> list = new List<MissionPeer>();
			foreach (KeyValuePair<MissionPeer, float> keyValuePair in allContributorsForSideAndClear)
			{
				int goldGainsFromObjectiveAssist = (keyValuePair.Key.Representative as SiegeMissionRepresentative).GetGoldGainsFromObjectiveAssist(root, keyValuePair.Value / num, false);
				if (goldGainsFromObjectiveAssist > 0)
				{
					base.ChangeCurrentGoldForPeer(keyValuePair.Key, keyValuePair.Key.Representative.Gold + goldGainsFromObjectiveAssist);
					list.Add(keyValuePair.Key);
					MissionMultiplayerSiege.OnObjectiveGoldGainedDelegate onObjectiveGoldGained = this.OnObjectiveGoldGained;
					if (onObjectiveGoldGained != null)
					{
						onObjectiveGoldGained(keyValuePair.Key, goldGainsFromObjectiveAssist);
					}
				}
			}
			destructableComponent.OnDestroyed -= new DestructableComponent.OnHitTakenAndDestroyedDelegate(this.DestructableComponentOnDestroyed);
			foreach (DestructableComponent destructableComponent2 in this._childDestructableComponents[root])
			{
				destructableComponent2.OnHitTaken -= new DestructableComponent.OnHitTakenAndDestroyedDelegate(this.DestructableComponentOnHitTaken);
			}
			this._childDestructableComponents.Remove(root);
			MissionMultiplayerSiege.OnDestructableComponentDestroyedDelegate onDestructableComponentDestroyed = this.OnDestructableComponentDestroyed;
			if (onDestructableComponentDestroyed == null)
			{
				return;
			}
			onDestructableComponentDestroyed(destructableComponent, attackerScriptComponentBehavior, list.ToArray());
		}

		// Token: 0x06002527 RID: 9511 RVA: 0x0008B584 File Offset: 0x00089784
		public override MissionLobbyComponent.MultiplayerGameType GetMissionType()
		{
			return MissionLobbyComponent.MultiplayerGameType.Siege;
		}

		// Token: 0x06002528 RID: 9512 RVA: 0x0008B587 File Offset: 0x00089787
		public override bool UseRoundController()
		{
			return false;
		}

		// Token: 0x06002529 RID: 9513 RVA: 0x0008B58C File Offset: 0x0008978C
		public override void AfterStart()
		{
			BasicCultureObject @object = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam1.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
			BasicCultureObject object2 = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam2.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
			Banner banner = new Banner(@object.BannerKey, @object.BackgroundColor1, @object.ForegroundColor1);
			Banner banner2 = new Banner(object2.BannerKey, object2.BackgroundColor2, object2.ForegroundColor2);
			base.Mission.Teams.Add(BattleSideEnum.Attacker, @object.BackgroundColor1, @object.ForegroundColor1, banner, true, false, true);
			base.Mission.Teams.Add(BattleSideEnum.Defender, object2.BackgroundColor2, object2.ForegroundColor2, banner2, true, false, true);
			foreach (FlagCapturePoint flagCapturePoint in this.AllCapturePoints)
			{
				this._capturePointOwners[flagCapturePoint.FlagIndex] = base.Mission.Teams.Defender;
				flagCapturePoint.SetTeamColors(base.Mission.Teams.Defender.Color, base.Mission.Teams.Defender.Color2);
				MissionMultiplayerSiegeClient gameModeSiegeClient = this._gameModeSiegeClient;
				if (gameModeSiegeClient != null)
				{
					gameModeSiegeClient.OnCapturePointOwnerChanged(flagCapturePoint, base.Mission.Teams.Defender);
				}
			}
			this._warmupComponent.OnWarmupEnding += this.OnWarmupEnding;
		}

		// Token: 0x0600252A RID: 9514 RVA: 0x0008B700 File Offset: 0x00089900
		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			if (!this._firstTickDone)
			{
				foreach (CastleGate castleGate in Mission.Current.MissionObjects.FindAllWithType<CastleGate>())
				{
					castleGate.OpenDoor();
					foreach (StandingPoint standingPoint in castleGate.StandingPoints)
					{
						standingPoint.SetIsDeactivatedSynched(true);
					}
				}
				this._firstTickDone = true;
			}
			if (this.MissionLobbyComponent.CurrentMultiplayerState == MissionLobbyComponent.MultiplayerGameState.Playing && !this.WarmupComponent.IsInWarmup)
			{
				this.CheckMorales(dt);
				if (this.CheckObjectives(dt))
				{
					this.TickFlags(dt);
					this.TickObjectives(dt);
				}
			}
		}

		// Token: 0x0600252B RID: 9515 RVA: 0x0008B7E4 File Offset: 0x000899E4
		private void CheckMorales(float dt)
		{
			this._dtSumCheckMorales += dt;
			if (this._dtSumCheckMorales >= 1f)
			{
				this._dtSumCheckMorales -= 1f;
				int num = MathF.Max(this._morales[1] + this.GetMoraleGain(BattleSideEnum.Attacker), 0);
				int num2 = MBMath.ClampInt(this._morales[0] + this.GetMoraleGain(BattleSideEnum.Defender), 0, 360);
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new SiegeMoraleChangeMessage(num, num2, this._capturePointRemainingMoraleGains));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
				MissionMultiplayerSiegeClient gameModeSiegeClient = this._gameModeSiegeClient;
				if (gameModeSiegeClient != null)
				{
					gameModeSiegeClient.OnMoraleChanged(num, num2, this._capturePointRemainingMoraleGains);
				}
				this._morales[1] = num;
				this._morales[0] = num2;
			}
		}

		// Token: 0x0600252C RID: 9516 RVA: 0x0008B89D File Offset: 0x00089A9D
		public override bool CheckForMatchEnd()
		{
			return this._morales.Any((int morale) => morale == 0);
		}

		// Token: 0x0600252D RID: 9517 RVA: 0x0008B8CC File Offset: 0x00089ACC
		public override Team GetWinnerTeam()
		{
			Team team = null;
			if (this._morales[1] <= 0 && this._morales[0] > 0)
			{
				team = base.Mission.Teams.Defender;
			}
			if (this._morales[0] <= 0 && this._morales[1] > 0)
			{
				team = base.Mission.Teams.Attacker;
			}
			team = team ?? base.Mission.Teams.Defender;
			base.Mission.GetMissionBehavior<MissionScoreboardComponent>().ChangeTeamScore(team, 1);
			return team;
		}

		// Token: 0x0600252E RID: 9518 RVA: 0x0008B954 File Offset: 0x00089B54
		private int GetMoraleGain(BattleSideEnum side)
		{
			int num = 0;
			bool flag2 = this._masterFlagBestAgent != null && this._masterFlagBestAgent.Team.Side == side;
			if (side == BattleSideEnum.Attacker)
			{
				if (!flag2)
				{
					num += -1;
				}
				using (IEnumerator<FlagCapturePoint> enumerator = this.AllCapturePoints.Where((FlagCapturePoint flag) => flag != this._masterFlag && !flag.IsDeactivated && flag.IsFullyRaised && this.GetFlagOwnerTeam(flag).Side == BattleSideEnum.Attacker).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						FlagCapturePoint flagCapturePoint = enumerator.Current;
						this._capturePointRemainingMoraleGains[flagCapturePoint.FlagIndex]--;
						num++;
						if (this._capturePointRemainingMoraleGains[flagCapturePoint.FlagIndex] == 0)
						{
							num += 90;
							foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
							{
								MissionPeer component = networkCommunicator.GetComponent<MissionPeer>();
								if (component != null)
								{
									Team team = component.Team;
									BattleSideEnum? battleSideEnum = ((team != null) ? new BattleSideEnum?(team.Side) : null);
									if ((battleSideEnum.GetValueOrDefault() == side) & (battleSideEnum != null))
									{
										base.ChangeCurrentGoldForPeer(component, base.GetCurrentGoldForPeer(component) + 35);
									}
								}
							}
							flagCapturePoint.RemovePointAsServer();
							(base.SpawnComponent.SpawnFrameBehavior as SiegeSpawnFrameBehavior).OnFlagDeactivated(flagCapturePoint);
							this._gameModeSiegeClient.OnNumberOfFlagsChanged();
							GameNetwork.BeginBroadcastModuleEvent();
							GameNetwork.WriteMessage(new FlagDominationFlagsRemovedMessage());
							GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
							this.NotificationsComponent.FlagsXRemoved(flagCapturePoint);
						}
					}
					return num;
				}
			}
			if (this._masterFlag.IsFullyRaised)
			{
				if (this.GetFlagOwnerTeam(this._masterFlag).Side == BattleSideEnum.Attacker)
				{
					if (!flag2)
					{
						int num2 = 0;
						for (int i = 0; i < this.AllCapturePoints.Count; i++)
						{
							if (this.AllCapturePoints[i] != this._masterFlag && !this.AllCapturePoints[i].IsDeactivated)
							{
								num2++;
							}
						}
						num += -6 + num2;
					}
				}
				else
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x0600252F RID: 9519 RVA: 0x0008BB88 File Offset: 0x00089D88
		public Team GetFlagOwnerTeam(FlagCapturePoint flag)
		{
			return this._capturePointOwners[flag.FlagIndex];
		}

		// Token: 0x06002530 RID: 9520 RVA: 0x0008BB97 File Offset: 0x00089D97
		private bool CheckObjectives(float dt)
		{
			this._dtSumObjectiveCheck += dt;
			if (this._dtSumObjectiveCheck >= 0.25f)
			{
				this._dtSumObjectiveCheck -= 0.25f;
				return true;
			}
			return false;
		}

		// Token: 0x06002531 RID: 9521 RVA: 0x0008BBCC File Offset: 0x00089DCC
		private void TickFlags(float dt)
		{
			foreach (FlagCapturePoint flagCapturePoint in this.AllCapturePoints)
			{
				if (!flagCapturePoint.IsDeactivated)
				{
					Team flagOwnerTeam = this.GetFlagOwnerTeam(flagCapturePoint);
					Agent agent = null;
					float num = float.MaxValue;
					AgentProximityMap.ProximityMapSearchStruct proximityMapSearchStruct = AgentProximityMap.BeginSearch(Mission.Current, flagCapturePoint.Position.AsVec2, 4f, false);
					while (proximityMapSearchStruct.LastFoundAgent != null)
					{
						Agent lastFoundAgent = proximityMapSearchStruct.LastFoundAgent;
						if (!lastFoundAgent.IsMount && lastFoundAgent.IsActive())
						{
							float num2 = lastFoundAgent.Position.DistanceSquared(flagCapturePoint.Position);
							if (num2 <= 16f && num2 < num)
							{
								agent = lastFoundAgent;
								num = num2;
							}
						}
						AgentProximityMap.FindNext(Mission.Current, ref proximityMapSearchStruct);
					}
					if (flagCapturePoint == this._masterFlag)
					{
						this._masterFlagBestAgent = agent;
					}
					CaptureTheFlagFlagDirection captureTheFlagFlagDirection = CaptureTheFlagFlagDirection.None;
					bool isContested = flagCapturePoint.IsContested;
					if (flagOwnerTeam == null)
					{
						if (!isContested && agent != null)
						{
							captureTheFlagFlagDirection = CaptureTheFlagFlagDirection.Down;
						}
						else if (agent == null && isContested)
						{
							captureTheFlagFlagDirection = CaptureTheFlagFlagDirection.Up;
						}
					}
					else if (agent != null)
					{
						if (agent.Team != flagOwnerTeam && !isContested)
						{
							captureTheFlagFlagDirection = CaptureTheFlagFlagDirection.Down;
						}
						else if (agent.Team == flagOwnerTeam && isContested)
						{
							captureTheFlagFlagDirection = CaptureTheFlagFlagDirection.Up;
						}
					}
					else if (isContested)
					{
						captureTheFlagFlagDirection = CaptureTheFlagFlagDirection.Up;
					}
					if (captureTheFlagFlagDirection != CaptureTheFlagFlagDirection.None)
					{
						flagCapturePoint.SetMoveFlag(captureTheFlagFlagDirection, 1f);
					}
					bool flag;
					flagCapturePoint.OnAfterTick(agent != null, out flag);
					if (flag)
					{
						Team team = agent.Team;
						uint num3 = ((team != null) ? team.Color : 4284111450U);
						uint num4 = ((team != null) ? team.Color2 : uint.MaxValue);
						flagCapturePoint.SetTeamColorsSynched(num3, num4);
						this._capturePointOwners[flagCapturePoint.FlagIndex] = team;
						GameNetwork.BeginBroadcastModuleEvent();
						GameNetwork.WriteMessage(new FlagDominationCapturePointMessage(flagCapturePoint.FlagIndex, team));
						GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
						MissionMultiplayerSiegeClient gameModeSiegeClient = this._gameModeSiegeClient;
						if (gameModeSiegeClient != null)
						{
							gameModeSiegeClient.OnCapturePointOwnerChanged(flagCapturePoint, team);
						}
						this.NotificationsComponent.FlagXCapturedByTeamX(flagCapturePoint, agent.Team);
					}
				}
			}
		}

		// Token: 0x06002532 RID: 9522 RVA: 0x0008BDDC File Offset: 0x00089FDC
		private void TickObjectives(float dt)
		{
			for (int i = this._movingObjectives.Length - 1; i >= 0; i--)
			{
				IMoveableSiegeWeapon item = this._movingObjectives[i].Item1;
				if (item != null)
				{
					SiegeWeapon siegeWeapon = item as SiegeWeapon;
					if (siegeWeapon.IsDeactivated || siegeWeapon.IsDestroyed || siegeWeapon.IsDisabled)
					{
						this._movingObjectives[i].Item1 = null;
					}
					else
					{
						if (item.MovementComponent.HasArrivedAtTarget)
						{
							this._movingObjectives[i].Item1 = null;
							GameEntity root = siegeWeapon.GameEntity.Root;
							List<KeyValuePair<MissionPeer, float>> allContributorsForSideAndClear = this._objectiveSystem.GetAllContributorsForSideAndClear(root, BattleSideEnum.Attacker);
							float num = allContributorsForSideAndClear.Sum((KeyValuePair<MissionPeer, float> ac) => ac.Value);
							using (List<KeyValuePair<MissionPeer, float>>.Enumerator enumerator = allContributorsForSideAndClear.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									KeyValuePair<MissionPeer, float> keyValuePair = enumerator.Current;
									int goldGainsFromObjectiveAssist = (keyValuePair.Key.Representative as SiegeMissionRepresentative).GetGoldGainsFromObjectiveAssist(root, keyValuePair.Value / num, true);
									if (goldGainsFromObjectiveAssist > 0)
									{
										base.ChangeCurrentGoldForPeer(keyValuePair.Key, keyValuePair.Key.Representative.Gold + goldGainsFromObjectiveAssist);
										MissionMultiplayerSiege.OnObjectiveGoldGainedDelegate onObjectiveGoldGained = this.OnObjectiveGoldGained;
										if (onObjectiveGoldGained != null)
										{
											onObjectiveGoldGained(keyValuePair.Key, goldGainsFromObjectiveAssist);
										}
									}
								}
								goto IL_223;
							}
						}
						GameEntity gameEntity = siegeWeapon.GameEntity;
						Vec3 item2 = this._movingObjectives[i].Item2;
						Vec3 globalPosition = gameEntity.GlobalPosition;
						float lengthSquared = (globalPosition - item2).LengthSquared;
						if (lengthSquared > 1f)
						{
							this._movingObjectives[i].Item2 = globalPosition;
							foreach (StandingPoint standingPoint in siegeWeapon.StandingPoints)
							{
								Agent userAgent = standingPoint.UserAgent;
								if (((userAgent != null) ? userAgent.MissionPeer : null) != null && userAgent.MissionPeer.Team.Side == siegeWeapon.Side)
								{
									this._objectiveSystem.AddContributionForObjective(gameEntity.Root, userAgent.MissionPeer, lengthSquared);
								}
							}
						}
					}
				}
				IL_223:;
			}
		}

		// Token: 0x06002533 RID: 9523 RVA: 0x0008C034 File Offset: 0x0008A234
		private void OnWarmupEnding()
		{
			this.NotificationsComponent.WarmupEnding();
		}

		// Token: 0x06002534 RID: 9524 RVA: 0x0008C044 File Offset: 0x0008A244
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

		// Token: 0x06002535 RID: 9525 RVA: 0x0008C0E4 File Offset: 0x0008A2E4
		protected override void HandleEarlyNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
		{
			networkPeer.AddComponent<SiegeMissionRepresentative>();
		}

		// Token: 0x06002536 RID: 9526 RVA: 0x0008C0F0 File Offset: 0x0008A2F0
		protected override void HandleNewClientAfterSynchronized(NetworkCommunicator networkPeer)
		{
			int num = 120;
			if (this._warmupComponent != null && this._warmupComponent.IsInWarmup)
			{
				num = 160;
			}
			base.ChangeCurrentGoldForPeer(networkPeer.GetComponent<MissionPeer>(), num);
			MissionMultiplayerSiegeClient gameModeSiegeClient = this._gameModeSiegeClient;
			if (gameModeSiegeClient != null)
			{
				gameModeSiegeClient.OnGoldAmountChangedForRepresentative(networkPeer.GetComponent<SiegeMissionRepresentative>(), num);
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

		// Token: 0x06002537 RID: 9527 RVA: 0x0008C1D8 File Offset: 0x0008A3D8
		public override void OnPeerChangedTeam(NetworkCommunicator peer, Team oldTeam, Team newTeam)
		{
			if (this.MissionLobbyComponent.CurrentMultiplayerState == MissionLobbyComponent.MultiplayerGameState.Playing && oldTeam != null && oldTeam != newTeam)
			{
				base.ChangeCurrentGoldForPeer(peer.GetComponent<MissionPeer>(), 100);
			}
		}

		// Token: 0x06002538 RID: 9528 RVA: 0x0008C200 File Offset: 0x0008A400
		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
		{
			if (this.MissionLobbyComponent.CurrentMultiplayerState == MissionLobbyComponent.MultiplayerGameState.Playing && blow.DamageType != DamageTypes.Invalid && (agentState == AgentState.Unconscious || agentState == AgentState.Killed) && affectedAgent.IsHuman)
			{
				MissionPeer missionPeer = affectedAgent.MissionPeer;
				if (missionPeer != null)
				{
					int num = 100;
					if (affectorAgent != affectedAgent)
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
						int num2 = array[1].Count - array[0].Count;
						BattleSideEnum battleSideEnum = ((num2 == 0) ? BattleSideEnum.None : ((num2 < 0) ? BattleSideEnum.Attacker : BattleSideEnum.Defender));
						if (battleSideEnum != BattleSideEnum.None && battleSideEnum == missionPeer.Team.Side)
						{
							num2 = MathF.Abs(num2);
							int count = array[(int)battleSideEnum].Count;
							if (count > 0)
							{
								int num3 = num * num2 / 10 / count * 10;
								num += num3;
							}
						}
					}
					base.ChangeCurrentGoldForPeer(missionPeer, missionPeer.Representative.Gold + num);
				}
				bool flag = ((affectorAgent != null) ? affectorAgent.Team : null) != null && affectedAgent.Team != null && affectorAgent.Team.Side == affectedAgent.Team.Side;
				MultiplayerClassDivisions.MPHeroClass mpheroClassForCharacter = MultiplayerClassDivisions.GetMPHeroClassForCharacter(affectedAgent.Character);
				Agent.Hitter assistingHitter = affectedAgent.GetAssistingHitter((affectorAgent != null) ? affectorAgent.MissionPeer : null);
				if (((affectorAgent != null) ? affectorAgent.MissionPeer : null) != null && affectorAgent != affectedAgent && affectedAgent.Team != affectorAgent.Team)
				{
					SiegeMissionRepresentative siegeMissionRepresentative = affectorAgent.MissionPeer.Representative as SiegeMissionRepresentative;
					int goldGainsFromKillDataAndUpdateFlags = siegeMissionRepresentative.GetGoldGainsFromKillDataAndUpdateFlags(MPPerkObject.GetPerkHandler(affectorAgent.MissionPeer), MPPerkObject.GetPerkHandler((assistingHitter != null) ? assistingHitter.HitterPeer : null), mpheroClassForCharacter, false, blow.IsMissile, flag);
					base.ChangeCurrentGoldForPeer(affectorAgent.MissionPeer, siegeMissionRepresentative.Gold + goldGainsFromKillDataAndUpdateFlags);
				}
				if (((assistingHitter != null) ? assistingHitter.HitterPeer : null) != null && !assistingHitter.IsFriendlyHit)
				{
					SiegeMissionRepresentative siegeMissionRepresentative2 = assistingHitter.HitterPeer.Representative as SiegeMissionRepresentative;
					int goldGainsFromKillDataAndUpdateFlags2 = siegeMissionRepresentative2.GetGoldGainsFromKillDataAndUpdateFlags(MPPerkObject.GetPerkHandler((affectorAgent != null) ? affectorAgent.MissionPeer : null), MPPerkObject.GetPerkHandler(assistingHitter.HitterPeer), mpheroClassForCharacter, true, blow.IsMissile, flag);
					base.ChangeCurrentGoldForPeer(assistingHitter.HitterPeer, siegeMissionRepresentative2.Gold + goldGainsFromKillDataAndUpdateFlags2);
				}
				if (((missionPeer != null) ? missionPeer.Team : null) != null)
				{
					MPPerkObject.MPPerkHandler perkHandler = MPPerkObject.GetPerkHandler(missionPeer);
					IEnumerable<ValueTuple<MissionPeer, int>> enumerable = ((perkHandler != null) ? perkHandler.GetTeamGoldRewardsOnDeath() : null);
					if (enumerable != null)
					{
						foreach (ValueTuple<MissionPeer, int> valueTuple in enumerable)
						{
							MissionPeer item = valueTuple.Item1;
							int item2 = valueTuple.Item2;
							SiegeMissionRepresentative siegeMissionRepresentative3;
							if (item2 > 0 && (siegeMissionRepresentative3 = ((item != null) ? item.Representative : null) as SiegeMissionRepresentative) != null)
							{
								int goldGainsFromAllyDeathReward = siegeMissionRepresentative3.GetGoldGainsFromAllyDeathReward(item2);
								if (goldGainsFromAllyDeathReward > 0)
								{
									base.ChangeCurrentGoldForPeer(item, siegeMissionRepresentative3.Gold + goldGainsFromAllyDeathReward);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06002539 RID: 9529 RVA: 0x0008C558 File Offset: 0x0008A758
		protected override void HandleNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
		{
			GameNetwork.BeginBroadcastModuleEvent();
			GameNetwork.WriteMessage(new SiegeMoraleChangeMessage(this._morales[1], this._morales[0], this._capturePointRemainingMoraleGains));
			GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
		}

		// Token: 0x0600253A RID: 9530 RVA: 0x0008C586 File Offset: 0x0008A786
		public override void OnRemoveBehavior()
		{
			base.OnRemoveBehavior();
			this._warmupComponent.OnWarmupEnding -= this.OnWarmupEnding;
		}

		// Token: 0x0600253B RID: 9531 RVA: 0x0008C5A8 File Offset: 0x0008A7A8
		public override void OnClearScene()
		{
			base.OnClearScene();
			base.ClearPeerCounts();
			foreach (CastleGate castleGate in Mission.Current.MissionObjects.FindAllWithType<CastleGate>())
			{
				foreach (StandingPoint standingPoint in castleGate.StandingPoints)
				{
					standingPoint.SetIsDeactivatedSynched(false);
				}
			}
		}

		// Token: 0x04000DA1 RID: 3489
		public const int NumberOfFlagsInGame = 7;

		// Token: 0x04000DA2 RID: 3490
		public const int NumberOfFlagsAffectingMoraleInGame = 6;

		// Token: 0x04000DA3 RID: 3491
		public const int MaxMorale = 1440;

		// Token: 0x04000DA4 RID: 3492
		public const int StartingMorale = 360;

		// Token: 0x04000DA5 RID: 3493
		private const int FirstSpawnGold = 120;

		// Token: 0x04000DA6 RID: 3494
		private const int FirstSpawnGoldForEarlyJoin = 160;

		// Token: 0x04000DA7 RID: 3495
		private const int RespawnGold = 100;

		// Token: 0x04000DA8 RID: 3496
		private const float ObjectiveCheckPeriod = 0.25f;

		// Token: 0x04000DA9 RID: 3497
		private const float MoraleTickTimeInSeconds = 1f;

		// Token: 0x04000DAA RID: 3498
		public const int MaxMoraleGainPerFlag = 90;

		// Token: 0x04000DAB RID: 3499
		private const int MoraleBoostOnFlagRemoval = 90;

		// Token: 0x04000DAC RID: 3500
		private const int MoraleDecayInTick = -1;

		// Token: 0x04000DAD RID: 3501
		private const int MoraleDecayOnDefenderInTick = -6;

		// Token: 0x04000DAE RID: 3502
		public const int MoraleGainPerFlag = 1;

		// Token: 0x04000DAF RID: 3503
		public const int GoldBonusOnFlagRemoval = 35;

		// Token: 0x04000DB0 RID: 3504
		public const string MasterFlagTag = "keep_capture_point";

		// Token: 0x04000DB3 RID: 3507
		private int[] _morales;

		// Token: 0x04000DB4 RID: 3508
		private Agent _masterFlagBestAgent;

		// Token: 0x04000DB5 RID: 3509
		private FlagCapturePoint _masterFlag;

		// Token: 0x04000DB6 RID: 3510
		private Team[] _capturePointOwners;

		// Token: 0x04000DB7 RID: 3511
		private int[] _capturePointRemainingMoraleGains;

		// Token: 0x04000DB9 RID: 3513
		private float _dtSumCheckMorales;

		// Token: 0x04000DBA RID: 3514
		private float _dtSumObjectiveCheck;

		// Token: 0x04000DBB RID: 3515
		private MissionMultiplayerSiege.ObjectiveSystem _objectiveSystem;

		// Token: 0x04000DBC RID: 3516
		private ValueTuple<IMoveableSiegeWeapon, Vec3>[] _movingObjectives;

		// Token: 0x04000DBD RID: 3517
		private ValueTuple<RangedSiegeWeapon, Agent>[] _lastReloadingAgentPerRangedSiegeMachine;

		// Token: 0x04000DBE RID: 3518
		private MissionMultiplayerSiegeClient _gameModeSiegeClient;

		// Token: 0x04000DBF RID: 3519
		private MultiplayerWarmupComponent _warmupComponent;

		// Token: 0x04000DC0 RID: 3520
		private Dictionary<GameEntity, List<DestructableComponent>> _childDestructableComponents;

		// Token: 0x04000DC1 RID: 3521
		private bool _firstTickDone;

		// Token: 0x020005C1 RID: 1473
		private class ObjectiveSystem
		{
			// Token: 0x06003BC8 RID: 15304 RVA: 0x000F0080 File Offset: 0x000EE280
			public ObjectiveSystem()
			{
				this._objectiveContributorMap = new Dictionary<GameEntity, List<MissionMultiplayerSiege.ObjectiveSystem.ObjectiveContributor>[]>();
			}

			// Token: 0x06003BC9 RID: 15305 RVA: 0x000F0094 File Offset: 0x000EE294
			public bool RegisterObjective(GameEntity entity)
			{
				if (!this._objectiveContributorMap.ContainsKey(entity))
				{
					this._objectiveContributorMap.Add(entity, new List<MissionMultiplayerSiege.ObjectiveSystem.ObjectiveContributor>[2]);
					for (int i = 0; i < 2; i++)
					{
						this._objectiveContributorMap[entity][i] = new List<MissionMultiplayerSiege.ObjectiveSystem.ObjectiveContributor>();
					}
					return true;
				}
				return false;
			}

			// Token: 0x06003BCA RID: 15306 RVA: 0x000F00E4 File Offset: 0x000EE2E4
			public void AddContributionForObjective(GameEntity objectiveEntity, MissionPeer contributorPeer, float contribution)
			{
				string text = objectiveEntity.Tags.FirstOrDefault((string x) => x.StartsWith("mp_siege_objective_")) ?? "";
				bool flag = false;
				for (int i = 0; i < 2; i++)
				{
					foreach (MissionMultiplayerSiege.ObjectiveSystem.ObjectiveContributor objectiveContributor in this._objectiveContributorMap[objectiveEntity][i])
					{
						if (objectiveContributor.Peer == contributorPeer)
						{
							Debug.Print(string.Format("[CONT > {0}] Increased contribution for {1}({2}) by {3}.", new object[]
							{
								text,
								contributorPeer.Name,
								contributorPeer.Team.Side.ToString(),
								contribution
							}), 0, Debug.DebugColor.White, 17179869184UL);
							objectiveContributor.IncreaseAmount(contribution);
							flag = true;
							break;
						}
					}
					if (flag)
					{
						break;
					}
				}
				if (!flag)
				{
					Debug.Print(string.Format("[CONT > {0}] Adding {1} contribution for {2}({3}).", new object[]
					{
						text,
						contribution,
						contributorPeer.Name,
						contributorPeer.Team.Side.ToString()
					}), 0, Debug.DebugColor.White, 17179869184UL);
					this._objectiveContributorMap[objectiveEntity][(int)contributorPeer.Team.Side].Add(new MissionMultiplayerSiege.ObjectiveSystem.ObjectiveContributor(contributorPeer, contribution));
				}
			}

			// Token: 0x06003BCB RID: 15307 RVA: 0x000F026C File Offset: 0x000EE46C
			public List<KeyValuePair<MissionPeer, float>> GetAllContributorsForSideAndClear(GameEntity objectiveEntity, BattleSideEnum side)
			{
				List<KeyValuePair<MissionPeer, float>> list = new List<KeyValuePair<MissionPeer, float>>();
				string text = objectiveEntity.Tags.FirstOrDefault((string x) => x.StartsWith("mp_siege_objective_")) ?? "";
				foreach (MissionMultiplayerSiege.ObjectiveSystem.ObjectiveContributor objectiveContributor in this._objectiveContributorMap[objectiveEntity][(int)side])
				{
					Debug.Print(string.Format("[CONT > {0}] Rewarding {1} contribution for {2}({3}).", new object[]
					{
						text,
						objectiveContributor.Contribution,
						objectiveContributor.Peer.Name,
						side.ToString()
					}), 0, Debug.DebugColor.White, 17179869184UL);
					list.Add(new KeyValuePair<MissionPeer, float>(objectiveContributor.Peer, objectiveContributor.Contribution));
				}
				this._objectiveContributorMap[objectiveEntity][(int)side].Clear();
				return list;
			}

			// Token: 0x04001E09 RID: 7689
			private readonly Dictionary<GameEntity, List<MissionMultiplayerSiege.ObjectiveSystem.ObjectiveContributor>[]> _objectiveContributorMap;

			// Token: 0x020006FF RID: 1791
			private class ObjectiveContributor
			{
				// Token: 0x17000A25 RID: 2597
				// (get) Token: 0x0600407C RID: 16508 RVA: 0x000FA09F File Offset: 0x000F829F
				// (set) Token: 0x0600407D RID: 16509 RVA: 0x000FA0A7 File Offset: 0x000F82A7
				public float Contribution { get; private set; }

				// Token: 0x0600407E RID: 16510 RVA: 0x000FA0B0 File Offset: 0x000F82B0
				public ObjectiveContributor(MissionPeer peer, float initialContribution)
				{
					this.Peer = peer;
					this.Contribution = initialContribution;
				}

				// Token: 0x0600407F RID: 16511 RVA: 0x000FA0C6 File Offset: 0x000F82C6
				public void IncreaseAmount(float deltaContribution)
				{
					this.Contribution += deltaContribution;
				}

				// Token: 0x04002348 RID: 9032
				public readonly MissionPeer Peer;
			}
		}

		// Token: 0x020005C2 RID: 1474
		// (Invoke) Token: 0x06003BCD RID: 15309
		public delegate void OnDestructableComponentDestroyedDelegate(DestructableComponent destructableComponent, ScriptComponentBehavior attackerScriptComponentBehaviour, MissionPeer[] contributors);

		// Token: 0x020005C3 RID: 1475
		// (Invoke) Token: 0x06003BD1 RID: 15313
		public delegate void OnObjectiveGoldGainedDelegate(MissionPeer peer, int goldGain);
	}
}
