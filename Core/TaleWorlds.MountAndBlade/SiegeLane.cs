using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	public class SiegeLane
	{
		public SiegeLane.LaneStateEnum LaneState { get; private set; }

		public FormationAI.BehaviorSide LaneSide { get; }

		public List<IPrimarySiegeWeapon> PrimarySiegeWeapons { get; private set; }

		public bool IsOpen { get; private set; }

		public bool IsBreach { get; private set; }

		public bool HasGate { get; private set; }

		public List<ICastleKeyPosition> DefensePoints { get; private set; }

		public WorldPosition DefenderOrigin { get; private set; }

		public WorldPosition AttackerOrigin { get; private set; }

		public SiegeLane(FormationAI.BehaviorSide laneSide, SiegeQuerySystem siegeQuerySystem)
		{
			this.LaneSide = laneSide;
			this.IsOpen = false;
			this.PrimarySiegeWeapons = new List<IPrimarySiegeWeapon>();
			this.DefensePoints = new List<ICastleKeyPosition>();
			this.IsBreach = false;
			this._siegeQuerySystem = siegeQuerySystem;
			this._lastAssignedFormations = new Formation[Mission.Current.Teams.Count];
			this.HasGate = false;
			this.LaneState = SiegeLane.LaneStateEnum.Active;
		}

		public bool CalculateIsLaneUnusable()
		{
			if (this.IsOpen)
			{
				return false;
			}
			if (this.HasGate)
			{
				for (int i = 0; i < this.DefensePoints.Count; i++)
				{
					CastleGate castleGate;
					if ((castleGate = this.DefensePoints[i] as CastleGate) != null && castleGate.IsGateOpen && castleGate.GameEntity.HasTag("outer_gate"))
					{
						return false;
					}
				}
			}
			for (int j = 0; j < this.PrimarySiegeWeapons.Count; j++)
			{
				IPrimarySiegeWeapon primarySiegeWeapon = this.PrimarySiegeWeapons[j];
				UsableMachine usableMachine;
				SiegeTower siegeTower;
				BatteringRam batteringRam;
				if (((usableMachine = primarySiegeWeapon as UsableMachine) == null || usableMachine.GameEntity != null) && ((siegeTower = primarySiegeWeapon as SiegeTower) == null || !siegeTower.IsDestroyed) && (primarySiegeWeapon.HasCompletedAction() || (batteringRam = primarySiegeWeapon as BatteringRam) == null || !batteringRam.IsDestroyed))
				{
					return false;
				}
			}
			return true;
		}

		public Formation GetLastAssignedFormation(int teamIndex)
		{
			if (teamIndex >= 0)
			{
				return this._lastAssignedFormations[teamIndex];
			}
			return null;
		}

		public void SetLaneState(SiegeLane.LaneStateEnum newLaneState)
		{
			this.LaneState = newLaneState;
		}

		public void SetLastAssignedFormation(int teamIndex, Formation formation)
		{
			if (teamIndex >= 0)
			{
				this._lastAssignedFormations[teamIndex] = formation;
			}
		}

		public void SetSiegeQuerySystem(SiegeQuerySystem siegeQuerySystem)
		{
			this._siegeQuerySystem = siegeQuerySystem;
		}

		public float CalculateLaneCapacity()
		{
			bool flag = false;
			for (int i = 0; i < this.DefensePoints.Count; i++)
			{
				WallSegment wallSegment;
				if ((wallSegment = this.DefensePoints[i] as WallSegment) != null && wallSegment.IsBreachedWall)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				return 60f;
			}
			if (this.HasGate)
			{
				bool flag2 = true;
				for (int j = 0; j < this.DefensePoints.Count; j++)
				{
					CastleGate castleGate;
					if ((castleGate = this.DefensePoints[j] as CastleGate) != null && !castleGate.IsGateOpen)
					{
						flag2 = false;
						break;
					}
				}
				if (flag2)
				{
					return 60f;
				}
			}
			float num = 0f;
			for (int k = 0; k < this.PrimarySiegeWeapons.Count; k++)
			{
				SiegeWeapon siegeWeapon = this.PrimarySiegeWeapons[k] as SiegeWeapon;
				if (!siegeWeapon.IsDisabled && !siegeWeapon.IsDestroyed)
				{
					num += this.PrimarySiegeWeapons[k].SiegeWeaponPriority;
				}
			}
			return num;
		}

		public SiegeLane.LaneDefenseStates GetDefenseState()
		{
			switch (this.LaneState)
			{
			case SiegeLane.LaneStateEnum.Safe:
			case SiegeLane.LaneStateEnum.Unused:
				return SiegeLane.LaneDefenseStates.Empty;
			case SiegeLane.LaneStateEnum.Used:
			case SiegeLane.LaneStateEnum.Abandoned:
				return SiegeLane.LaneDefenseStates.Token;
			case SiegeLane.LaneStateEnum.Active:
			case SiegeLane.LaneStateEnum.Contested:
			case SiegeLane.LaneStateEnum.Conceited:
				return SiegeLane.LaneDefenseStates.Full;
			default:
				return SiegeLane.LaneDefenseStates.Full;
			}
		}

		private bool IsPowerBehindLane()
		{
			switch (this.LaneSide)
			{
			case FormationAI.BehaviorSide.Left:
				return this._siegeQuerySystem.LeftRegionMemberCount >= 30;
			case FormationAI.BehaviorSide.Middle:
				return this._siegeQuerySystem.MiddleRegionMemberCount >= 30;
			case FormationAI.BehaviorSide.Right:
				return this._siegeQuerySystem.RightRegionMemberCount >= 30;
			default:
				MBDebug.ShowWarning("Lane without side");
				return false;
			}
		}

		public bool IsUnderAttack()
		{
			switch (this.LaneSide)
			{
			case FormationAI.BehaviorSide.Left:
				return this._siegeQuerySystem.LeftCloseAttackerCount >= 15;
			case FormationAI.BehaviorSide.Middle:
				return this._siegeQuerySystem.MiddleCloseAttackerCount >= 15;
			case FormationAI.BehaviorSide.Right:
				return this._siegeQuerySystem.RightCloseAttackerCount >= 15;
			default:
				MBDebug.ShowWarning("Lane without side");
				return false;
			}
		}

		public bool IsDefended()
		{
			switch (this.LaneSide)
			{
			case FormationAI.BehaviorSide.Left:
				return this._siegeQuerySystem.LeftDefenderCount >= 15;
			case FormationAI.BehaviorSide.Middle:
				return this._siegeQuerySystem.MiddleDefenderCount >= 15;
			case FormationAI.BehaviorSide.Right:
				return this._siegeQuerySystem.RightDefenderCount >= 15;
			default:
				MBDebug.ShowWarning("Lane without side");
				return false;
			}
		}

		public void DetermineLaneState()
		{
			if (this.LaneState != SiegeLane.LaneStateEnum.Conceited || this.IsDefended())
			{
				if (this.CalculateIsLaneUnusable())
				{
					this.LaneState = SiegeLane.LaneStateEnum.Safe;
				}
				else if (Mission.Current.IsTeleportingAgents)
				{
					this.LaneState = SiegeLane.LaneStateEnum.Active;
				}
				else if (!this.IsOpen)
				{
					bool flag = true;
					foreach (IPrimarySiegeWeapon primarySiegeWeapon in this.PrimarySiegeWeapons)
					{
						if (!(primarySiegeWeapon is IMoveableSiegeWeapon) || primarySiegeWeapon.HasCompletedAction() || ((SiegeWeapon)primarySiegeWeapon).IsUsed)
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						this.LaneState = SiegeLane.LaneStateEnum.Unused;
					}
					else
					{
						this.LaneState = ((!this.IsPowerBehindLane()) ? SiegeLane.LaneStateEnum.Used : SiegeLane.LaneStateEnum.Active);
					}
				}
				else if (!this.IsPowerBehindLane())
				{
					this.LaneState = SiegeLane.LaneStateEnum.Abandoned;
				}
				else
				{
					this.LaneState = ((!this.IsUnderAttack() || this.IsDefended()) ? SiegeLane.LaneStateEnum.Contested : SiegeLane.LaneStateEnum.Conceited);
				}
				if (this.HasGate && this.LaneState < SiegeLane.LaneStateEnum.Active && TeamAISiegeComponent.QuerySystem.InsideAttackerCount >= 15)
				{
					this.LaneState = SiegeLane.LaneStateEnum.Active;
				}
			}
		}

		public WorldPosition GetCurrentAttackerPosition()
		{
			if (this.IsBreach)
			{
				return this.DefenderOrigin;
			}
			if (this._attackerMovableWeapon != null)
			{
				return this._attackerMovableWeapon.WaitFrame.origin.ToWorldPosition();
			}
			return this.AttackerOrigin;
		}

		public void DetermineOrigins()
		{
			this._attackerMovableWeapon = null;
			if (this.IsBreach)
			{
				WallSegment wallSegment = this.DefensePoints.FirstOrDefault((ICastleKeyPosition dp) => dp is WallSegment && (dp as WallSegment).IsBreachedWall) as WallSegment;
				this.DefenderOrigin = wallSegment.MiddleFrame.Origin;
				this.AttackerOrigin = wallSegment.AttackerWaitFrame.Origin;
				return;
			}
			this.HasGate = this.DefensePoints.Any((ICastleKeyPosition dp) => dp is CastleGate);
			IEnumerable<IPrimarySiegeWeapon> enumerable;
			if (this.PrimarySiegeWeapons.Count != 0)
			{
				IEnumerable<IPrimarySiegeWeapon> primarySiegeWeapons = this.PrimarySiegeWeapons;
				enumerable = primarySiegeWeapons;
			}
			else
			{
				enumerable = from sw in Mission.Current.MissionObjects.FindAllWithType<SiegeWeapon>().Where(delegate(SiegeWeapon sw)
					{
						IPrimarySiegeWeapon primarySiegeWeapon;
						return (primarySiegeWeapon = sw as IPrimarySiegeWeapon) != null && primarySiegeWeapon.WeaponSide == this.LaneSide;
					})
					select sw as IPrimarySiegeWeapon;
			}
			IEnumerable<IPrimarySiegeWeapon> enumerable2 = enumerable;
			IMoveableSiegeWeapon moveableSiegeWeapon;
			if ((moveableSiegeWeapon = enumerable2.FirstOrDefault((IPrimarySiegeWeapon psw) => psw is IMoveableSiegeWeapon) as IMoveableSiegeWeapon) != null)
			{
				this._attackerMovableWeapon = moveableSiegeWeapon as SiegeWeapon;
				this.DefenderOrigin = ((moveableSiegeWeapon as IPrimarySiegeWeapon).TargetCastlePosition as ICastleKeyPosition).MiddleFrame.Origin;
				this.AttackerOrigin = moveableSiegeWeapon.GetInitialFrame().origin.ToWorldPosition();
				return;
			}
			SiegeLadder siegeLadder = enumerable2.FirstOrDefault((IPrimarySiegeWeapon psw) => psw is SiegeLadder) as SiegeLadder;
			this.DefenderOrigin = (siegeLadder.TargetCastlePosition as ICastleKeyPosition).MiddleFrame.Origin;
			this.AttackerOrigin = siegeLadder.InitialWaitPosition.GetGlobalFrame().origin.ToWorldPosition();
		}

		public void RefreshLane()
		{
			for (int i = this.PrimarySiegeWeapons.Count - 1; i >= 0; i--)
			{
				SiegeWeapon siegeWeapon;
				if ((siegeWeapon = this.PrimarySiegeWeapons[i] as SiegeWeapon) != null && siegeWeapon.IsDisabled)
				{
					this.PrimarySiegeWeapons.RemoveAt(i);
				}
			}
			bool flag = false;
			for (int j = 0; j < this.DefensePoints.Count; j++)
			{
				WallSegment wallSegment;
				if ((wallSegment = this.DefensePoints[j] as WallSegment) != null && wallSegment.IsBreachedWall)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				this.IsOpen = true;
				this.IsBreach = true;
				return;
			}
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = true;
			for (int k = 0; k < this.DefensePoints.Count; k++)
			{
				ICastleKeyPosition castleKeyPosition = this.DefensePoints[k];
				CastleGate castleGate;
				if (flag4 && (castleGate = castleKeyPosition as CastleGate) != null)
				{
					flag2 = true;
					flag3 = true;
					if (!castleGate.IsDestroyed && castleGate.State != CastleGate.GateState.Open)
					{
						flag4 = false;
						break;
					}
				}
				else if (!flag3 && !(castleKeyPosition is WallSegment))
				{
					flag3 = true;
				}
			}
			bool flag5 = false;
			if (!flag3)
			{
				for (int l = 0; l < this.PrimarySiegeWeapons.Count; l++)
				{
					IPrimarySiegeWeapon primarySiegeWeapon = this.PrimarySiegeWeapons[l];
					if (primarySiegeWeapon.HasCompletedAction() && !(primarySiegeWeapon as UsableMachine).IsDestroyed)
					{
						flag5 = true;
						break;
					}
				}
			}
			this.IsOpen = (flag2 && flag4) || flag5;
		}

		public void SetPrimarySiegeWeapons(List<IPrimarySiegeWeapon> primarySiegeWeapons)
		{
			this.PrimarySiegeWeapons = primarySiegeWeapons;
		}

		public void SetDefensePoints(List<ICastleKeyPosition> defensePoints)
		{
			this.DefensePoints = defensePoints;
			foreach (ICastleKeyPosition castleKeyPosition in this.PrimarySiegeWeapons.Select((IPrimarySiegeWeapon psw) => psw.TargetCastlePosition as ICastleKeyPosition))
			{
				if (castleKeyPosition != null && !this.DefensePoints.Contains(castleKeyPosition))
				{
					this.DefensePoints.Add(castleKeyPosition);
				}
			}
		}

		private readonly Formation[] _lastAssignedFormations;

		private SiegeQuerySystem _siegeQuerySystem;

		private SiegeWeapon _attackerMovableWeapon;

		public enum LaneStateEnum
		{
			Safe,
			Unused,
			Used,
			Active,
			Abandoned,
			Contested,
			Conceited
		}

		public enum LaneDefenseStates
		{
			Empty,
			Token,
			Full
		}
	}
}
