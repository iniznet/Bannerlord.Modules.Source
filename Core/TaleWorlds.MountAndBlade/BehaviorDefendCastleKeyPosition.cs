using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
	public class BehaviorDefendCastleKeyPosition : BehaviorComponent
	{
		public override float NavmeshlessTargetPositionPenalty
		{
			get
			{
				return 1f;
			}
		}

		public BehaviorDefendCastleKeyPosition(Formation formation)
			: base(formation)
		{
			this._teamAISiegeDefender = formation.Team.TeamAI as TeamAISiegeComponent;
			this._behaviorState = BehaviorDefendCastleKeyPosition.BehaviorState.UnSet;
			this._laddersOnThisSide = new List<SiegeLadder>();
			this.ResetOrderPositions();
			this._hasFormedShieldWall = true;
		}

		protected override void CalculateCurrentOrder()
		{
			base.CalculateCurrentOrder();
			base.CurrentOrder = ((this._behaviorState == BehaviorDefendCastleKeyPosition.BehaviorState.Ready) ? this._readyOrder : this._waitOrder);
			this.CurrentFacingOrder = ((base.Formation.QuerySystem.ClosestEnemyFormation != null && TeamAISiegeComponent.IsFormationInsideCastle(base.Formation.QuerySystem.ClosestEnemyFormation.Formation, true, 0.4f)) ? FacingOrder.FacingOrderLookAtEnemy : ((this._behaviorState == BehaviorDefendCastleKeyPosition.BehaviorState.Ready) ? this._readyFacingOrder : this._waitFacingOrder));
		}

		public override TextObject GetBehaviorString()
		{
			TextObject behaviorString = base.GetBehaviorString();
			TextObject textObject = GameTexts.FindText("str_formation_ai_side_strings", base.Formation.AI.Side.ToString());
			behaviorString.SetTextVariable("SIDE_STRING", textObject);
			behaviorString.SetTextVariable("IS_GENERAL_SIDE", "0");
			return behaviorString;
		}

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

		public override void OnValidBehaviorSideChanged()
		{
			base.OnValidBehaviorSideChanged();
			this.ResetOrderPositions();
		}

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

		protected override float GetAiWeight()
		{
			return 1f;
		}

		private TeamAISiegeComponent _teamAISiegeDefender;

		private CastleGate _innerGate;

		private CastleGate _outerGate;

		private List<SiegeLadder> _laddersOnThisSide;

		private BehaviorDefendCastleKeyPosition.BehaviorState _behaviorState;

		private MovementOrder _waitOrder;

		private MovementOrder _readyOrder;

		private FacingOrder _waitFacingOrder;

		private FacingOrder _readyFacingOrder;

		private TacticalPosition _tacticalMiddlePos;

		private TacticalPosition _tacticalWaitPos;

		private bool _hasFormedShieldWall;

		private WorldPosition _readyOrderPosition;

		private enum BehaviorState
		{
			UnSet,
			Waiting,
			Ready
		}
	}
}
