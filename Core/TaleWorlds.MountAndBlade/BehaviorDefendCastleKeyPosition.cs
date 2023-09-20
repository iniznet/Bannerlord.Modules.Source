using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000100 RID: 256
	public class BehaviorDefendCastleKeyPosition : BehaviorComponent
	{
		// Token: 0x17000312 RID: 786
		// (get) Token: 0x06000CA3 RID: 3235 RVA: 0x0001B35E File Offset: 0x0001955E
		public override float NavmeshlessTargetPositionPenalty
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x06000CA4 RID: 3236 RVA: 0x0001B365 File Offset: 0x00019565
		public BehaviorDefendCastleKeyPosition(Formation formation)
			: base(formation)
		{
			this._teamAISiegeDefender = formation.Team.TeamAI as TeamAISiegeComponent;
			this._behaviorState = BehaviorDefendCastleKeyPosition.BehaviorState.UnSet;
			this._laddersOnThisSide = new List<SiegeLadder>();
			this.ResetOrderPositions();
			this._hasFormedShieldWall = true;
		}

		// Token: 0x06000CA5 RID: 3237 RVA: 0x0001B3A4 File Offset: 0x000195A4
		protected override void CalculateCurrentOrder()
		{
			base.CalculateCurrentOrder();
			base.CurrentOrder = ((this._behaviorState == BehaviorDefendCastleKeyPosition.BehaviorState.Ready) ? this._readyOrder : this._waitOrder);
			this.CurrentFacingOrder = ((base.Formation.QuerySystem.ClosestEnemyFormation != null && TeamAISiegeComponent.IsFormationInsideCastle(base.Formation.QuerySystem.ClosestEnemyFormation.Formation, true, 0.4f)) ? FacingOrder.FacingOrderLookAtEnemy : ((this._behaviorState == BehaviorDefendCastleKeyPosition.BehaviorState.Ready) ? this._readyFacingOrder : this._waitFacingOrder));
		}

		// Token: 0x06000CA6 RID: 3238 RVA: 0x0001B42C File Offset: 0x0001962C
		public override TextObject GetBehaviorString()
		{
			TextObject behaviorString = base.GetBehaviorString();
			TextObject textObject = GameTexts.FindText("str_formation_ai_side_strings", base.Formation.AI.Side.ToString());
			behaviorString.SetTextVariable("SIDE_STRING", textObject);
			behaviorString.SetTextVariable("IS_GENERAL_SIDE", "0");
			return behaviorString;
		}

		// Token: 0x06000CA7 RID: 3239 RVA: 0x0001B488 File Offset: 0x00019688
		private void ResetOrderPositions()
		{
			this._behaviorSide = base.Formation.AI.Side;
			this._innerGate = null;
			this._outerGate = null;
			this._laddersOnThisSide.Clear();
			WorldFrame worldFrame;
			WorldFrame worldFrame2;
			if (this._teamAISiegeDefender.OuterGate.DefenseSide == this._behaviorSide)
			{
				CastleGate outerGate = this._teamAISiegeDefender.OuterGate;
				this._innerGate = this._teamAISiegeDefender.InnerGate;
				this._outerGate = this._teamAISiegeDefender.OuterGate;
				worldFrame = outerGate.MiddleFrame;
				worldFrame2 = outerGate.DefenseWaitFrame;
				this._tacticalMiddlePos = outerGate.MiddlePosition;
				this._tacticalWaitPos = outerGate.WaitPosition;
			}
			else
			{
				WallSegment wallSegment = this._teamAISiegeDefender.WallSegments.Where((WallSegment ws) => ws.DefenseSide == this._behaviorSide && ws.IsBreachedWall).FirstOrDefault<WallSegment>();
				if (wallSegment != null)
				{
					worldFrame = wallSegment.MiddleFrame;
					worldFrame2 = wallSegment.DefenseWaitFrame;
					this._tacticalMiddlePos = wallSegment.MiddlePosition;
					this._tacticalWaitPos = wallSegment.WaitPosition;
				}
				else
				{
					IEnumerable<IPrimarySiegeWeapon> enumerable = this._teamAISiegeDefender.PrimarySiegeWeapons.Where(delegate(IPrimarySiegeWeapon sw)
					{
						SiegeWeapon siegeWeapon;
						return sw.WeaponSide == this._behaviorSide && (((siegeWeapon = sw as SiegeWeapon) != null && !siegeWeapon.IsDestroyed && !siegeWeapon.IsDeactivated) || sw.HasCompletedAction());
					});
					if (!enumerable.Any<IPrimarySiegeWeapon>())
					{
						worldFrame = WorldFrame.Invalid;
						worldFrame2 = WorldFrame.Invalid;
						this._tacticalMiddlePos = null;
						this._tacticalWaitPos = null;
					}
					else
					{
						this._laddersOnThisSide = enumerable.OfType<SiegeLadder>().ToList<SiegeLadder>();
						ICastleKeyPosition castleKeyPosition = enumerable.FirstOrDefault<IPrimarySiegeWeapon>().TargetCastlePosition as ICastleKeyPosition;
						worldFrame = castleKeyPosition.MiddleFrame;
						worldFrame2 = castleKeyPosition.DefenseWaitFrame;
						this._tacticalMiddlePos = castleKeyPosition.MiddlePosition;
						this._tacticalWaitPos = castleKeyPosition.WaitPosition;
					}
				}
			}
			if (this._tacticalMiddlePos != null)
			{
				this._readyOrderPosition = this._tacticalMiddlePos.Position;
				this._readyOrder = MovementOrder.MovementOrderMove(this._readyOrderPosition);
				this._readyFacingOrder = FacingOrder.FacingOrderLookAtDirection(this._tacticalMiddlePos.Direction);
			}
			else if (worldFrame.Origin.IsValid)
			{
				worldFrame.Rotation.f.Normalize();
				this._readyOrderPosition = worldFrame.Origin;
				this._readyOrder = MovementOrder.MovementOrderMove(this._readyOrderPosition);
				this._readyFacingOrder = FacingOrder.FacingOrderLookAtDirection(worldFrame.Rotation.f.AsVec2);
			}
			else
			{
				this._readyOrderPosition = WorldPosition.Invalid;
				this._readyOrder = MovementOrder.MovementOrderStop;
				this._readyFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
			}
			if (this._tacticalWaitPos != null)
			{
				this._waitOrder = MovementOrder.MovementOrderMove(this._tacticalWaitPos.Position);
				this._waitFacingOrder = FacingOrder.FacingOrderLookAtDirection(this._tacticalWaitPos.Direction);
			}
			else if (worldFrame2.Origin.IsValid)
			{
				worldFrame2.Rotation.f.Normalize();
				this._waitOrder = MovementOrder.MovementOrderMove(worldFrame2.Origin);
				this._waitFacingOrder = FacingOrder.FacingOrderLookAtDirection(worldFrame2.Rotation.f.AsVec2);
			}
			else
			{
				this._waitOrder = MovementOrder.MovementOrderStop;
				this._waitFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
			}
			base.CurrentOrder = ((this._behaviorState == BehaviorDefendCastleKeyPosition.BehaviorState.Ready) ? this._readyOrder : this._waitOrder);
			this.CurrentFacingOrder = ((base.Formation.QuerySystem.ClosestEnemyFormation != null && TeamAISiegeComponent.IsFormationInsideCastle(base.Formation.QuerySystem.ClosestEnemyFormation.Formation, true, 0.4f)) ? FacingOrder.FacingOrderLookAtEnemy : ((this._behaviorState == BehaviorDefendCastleKeyPosition.BehaviorState.Ready) ? this._readyFacingOrder : this._waitFacingOrder));
		}

		// Token: 0x06000CA8 RID: 3240 RVA: 0x0001B7E7 File Offset: 0x000199E7
		public override void OnValidBehaviorSideChanged()
		{
			base.OnValidBehaviorSideChanged();
			this.ResetOrderPositions();
		}

		// Token: 0x06000CA9 RID: 3241 RVA: 0x0001B7F8 File Offset: 0x000199F8
		public override void TickOccasionally()
		{
			base.TickOccasionally();
			bool flag = false;
			if (this._teamAISiegeDefender != null && !base.Formation.IsDeployment)
			{
				for (int i = 0; i < TeamAISiegeComponent.SiegeLanes.Count; i++)
				{
					SiegeLane siegeLane = TeamAISiegeComponent.SiegeLanes[i];
					if (siegeLane.LaneSide == this._behaviorSide)
					{
						if (siegeLane.IsOpen)
						{
							flag = true;
						}
						else
						{
							for (int j = 0; j < siegeLane.PrimarySiegeWeapons.Count; j++)
							{
								IPrimarySiegeWeapon primarySiegeWeapon = siegeLane.PrimarySiegeWeapons[j];
								SiegeLadder siegeLadder;
								if ((siegeLadder = primarySiegeWeapon as SiegeLadder) != null)
								{
									if (siegeLadder.IsUsed)
									{
										flag = true;
										break;
									}
								}
								else if ((primarySiegeWeapon as SiegeWeapon).GetComponent<SiegeWeaponMovementComponent>().HasApproachedTarget)
								{
									flag = true;
									break;
								}
							}
						}
					}
				}
			}
			BehaviorDefendCastleKeyPosition.BehaviorState behaviorState = (flag ? BehaviorDefendCastleKeyPosition.BehaviorState.Ready : BehaviorDefendCastleKeyPosition.BehaviorState.Waiting);
			bool flag2 = false;
			if (behaviorState != this._behaviorState)
			{
				this._behaviorState = behaviorState;
				base.CurrentOrder = ((this._behaviorState == BehaviorDefendCastleKeyPosition.BehaviorState.Ready) ? this._readyOrder : this._waitOrder);
				this.CurrentFacingOrder = ((this._behaviorState == BehaviorDefendCastleKeyPosition.BehaviorState.Ready) ? this._readyFacingOrder : this._waitFacingOrder);
				flag2 = true;
			}
			if (Mission.Current.MissionTeamAIType == Mission.MissionTeamAITypeEnum.Siege)
			{
				if (this._outerGate != null && this._outerGate.State == CastleGate.GateState.Open && !this._outerGate.IsDestroyed)
				{
					if (!this._outerGate.IsUsedByFormation(base.Formation))
					{
						base.Formation.StartUsingMachine(this._outerGate, false);
					}
				}
				else if (this._innerGate != null && this._innerGate.State == CastleGate.GateState.Open && !this._innerGate.IsDestroyed && !this._innerGate.IsUsedByFormation(base.Formation))
				{
					base.Formation.StartUsingMachine(this._innerGate, false);
				}
				foreach (SiegeLadder siegeLadder2 in this._laddersOnThisSide)
				{
					if (!siegeLadder2.IsDisabledForBattleSide(BattleSideEnum.Defender) && !siegeLadder2.IsUsedByFormation(base.Formation))
					{
						base.Formation.StartUsingMachine(siegeLadder2, false);
					}
				}
			}
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.FacingOrder = this.CurrentFacingOrder;
			if (this._behaviorState == BehaviorDefendCastleKeyPosition.BehaviorState.Ready && this._tacticalMiddlePos != null)
			{
				base.Formation.FormOrder = FormOrder.FormOrderCustom(this._tacticalMiddlePos.Width);
			}
			else if (this._behaviorState == BehaviorDefendCastleKeyPosition.BehaviorState.Waiting && this._tacticalWaitPos != null)
			{
				base.Formation.FormOrder = FormOrder.FormOrderCustom(this._tacticalWaitPos.Width);
			}
			if (flag2 || !this._hasFormedShieldWall)
			{
				bool flag3;
				if (this._behaviorState == BehaviorDefendCastleKeyPosition.BehaviorState.Ready && this._readyOrderPosition.IsValid)
				{
					Vec3 navMeshVec = base.Formation.QuerySystem.MedianPosition.GetNavMeshVec3();
					flag3 = this._readyOrderPosition.DistanceSquaredWithLimit(navMeshVec, MathF.Min(base.Formation.Depth, base.Formation.Width) * 1.2f) <= (this._hasFormedShieldWall ? (MathF.Min(base.Formation.Depth, base.Formation.Width) * MathF.Min(base.Formation.Depth, base.Formation.Width)) : (MathF.Min(base.Formation.Depth, base.Formation.Width) * MathF.Min(base.Formation.Depth, base.Formation.Width) * 0.25f));
				}
				else
				{
					flag3 = true;
				}
				bool flag4 = flag3;
				if (flag4 != this._hasFormedShieldWall)
				{
					this._hasFormedShieldWall = flag4;
					base.Formation.ArrangementOrder = (this._hasFormedShieldWall ? ArrangementOrder.ArrangementOrderShieldWall : ArrangementOrder.ArrangementOrderLine);
				}
			}
		}

		// Token: 0x06000CAA RID: 3242 RVA: 0x0001BBD4 File Offset: 0x00019DD4
		protected override void OnBehaviorActivatedAux()
		{
			this.ResetOrderPositions();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.FacingOrder = this.CurrentFacingOrder;
			base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
			this._hasFormedShieldWall = true;
			base.Formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
			base.Formation.FormOrder = FormOrder.FormOrderWide;
			base.Formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
		}

		// Token: 0x06000CAB RID: 3243 RVA: 0x0001BC50 File Offset: 0x00019E50
		protected override float GetAiWeight()
		{
			return 1f;
		}

		// Token: 0x040002FB RID: 763
		private TeamAISiegeComponent _teamAISiegeDefender;

		// Token: 0x040002FC RID: 764
		private CastleGate _innerGate;

		// Token: 0x040002FD RID: 765
		private CastleGate _outerGate;

		// Token: 0x040002FE RID: 766
		private List<SiegeLadder> _laddersOnThisSide;

		// Token: 0x040002FF RID: 767
		private BehaviorDefendCastleKeyPosition.BehaviorState _behaviorState;

		// Token: 0x04000300 RID: 768
		private MovementOrder _waitOrder;

		// Token: 0x04000301 RID: 769
		private MovementOrder _readyOrder;

		// Token: 0x04000302 RID: 770
		private FacingOrder _waitFacingOrder;

		// Token: 0x04000303 RID: 771
		private FacingOrder _readyFacingOrder;

		// Token: 0x04000304 RID: 772
		private TacticalPosition _tacticalMiddlePos;

		// Token: 0x04000305 RID: 773
		private TacticalPosition _tacticalWaitPos;

		// Token: 0x04000306 RID: 774
		private bool _hasFormedShieldWall;

		// Token: 0x04000307 RID: 775
		private WorldPosition _readyOrderPosition;

		// Token: 0x0200043F RID: 1087
		private enum BehaviorState
		{
			// Token: 0x0400184A RID: 6218
			UnSet,
			// Token: 0x0400184B RID: 6219
			Waiting,
			// Token: 0x0400184C RID: 6220
			Ready
		}
	}
}
