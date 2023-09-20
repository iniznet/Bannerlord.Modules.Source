using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200016E RID: 366
	public class SiegeLane
	{
		// Token: 0x1700041D RID: 1053
		// (get) Token: 0x060012EB RID: 4843 RVA: 0x000496A4 File Offset: 0x000478A4
		// (set) Token: 0x060012EC RID: 4844 RVA: 0x000496AC File Offset: 0x000478AC
		public SiegeLane.LaneStateEnum LaneState { get; private set; }

		// Token: 0x1700041E RID: 1054
		// (get) Token: 0x060012ED RID: 4845 RVA: 0x000496B5 File Offset: 0x000478B5
		public FormationAI.BehaviorSide LaneSide { get; }

		// Token: 0x1700041F RID: 1055
		// (get) Token: 0x060012EE RID: 4846 RVA: 0x000496BD File Offset: 0x000478BD
		// (set) Token: 0x060012EF RID: 4847 RVA: 0x000496C5 File Offset: 0x000478C5
		public List<IPrimarySiegeWeapon> PrimarySiegeWeapons { get; private set; }

		// Token: 0x17000420 RID: 1056
		// (get) Token: 0x060012F0 RID: 4848 RVA: 0x000496CE File Offset: 0x000478CE
		// (set) Token: 0x060012F1 RID: 4849 RVA: 0x000496D6 File Offset: 0x000478D6
		public bool IsOpen { get; private set; }

		// Token: 0x17000421 RID: 1057
		// (get) Token: 0x060012F2 RID: 4850 RVA: 0x000496DF File Offset: 0x000478DF
		// (set) Token: 0x060012F3 RID: 4851 RVA: 0x000496E7 File Offset: 0x000478E7
		public bool IsBreach { get; private set; }

		// Token: 0x17000422 RID: 1058
		// (get) Token: 0x060012F4 RID: 4852 RVA: 0x000496F0 File Offset: 0x000478F0
		// (set) Token: 0x060012F5 RID: 4853 RVA: 0x000496F8 File Offset: 0x000478F8
		public bool HasGate { get; private set; }

		// Token: 0x17000423 RID: 1059
		// (get) Token: 0x060012F6 RID: 4854 RVA: 0x00049701 File Offset: 0x00047901
		// (set) Token: 0x060012F7 RID: 4855 RVA: 0x00049709 File Offset: 0x00047909
		public List<ICastleKeyPosition> DefensePoints { get; private set; }

		// Token: 0x17000424 RID: 1060
		// (get) Token: 0x060012F8 RID: 4856 RVA: 0x00049712 File Offset: 0x00047912
		// (set) Token: 0x060012F9 RID: 4857 RVA: 0x0004971A File Offset: 0x0004791A
		public WorldPosition DefenderOrigin { get; private set; }

		// Token: 0x17000425 RID: 1061
		// (get) Token: 0x060012FA RID: 4858 RVA: 0x00049723 File Offset: 0x00047923
		// (set) Token: 0x060012FB RID: 4859 RVA: 0x0004972B File Offset: 0x0004792B
		public WorldPosition AttackerOrigin { get; private set; }

		// Token: 0x060012FC RID: 4860 RVA: 0x00049734 File Offset: 0x00047934
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

		// Token: 0x060012FD RID: 4861 RVA: 0x000497A4 File Offset: 0x000479A4
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

		// Token: 0x060012FE RID: 4862 RVA: 0x0004987A File Offset: 0x00047A7A
		public Formation GetLastAssignedFormation(int teamIndex)
		{
			if (teamIndex >= 0)
			{
				return this._lastAssignedFormations[teamIndex];
			}
			return null;
		}

		// Token: 0x060012FF RID: 4863 RVA: 0x0004988A File Offset: 0x00047A8A
		public void SetLaneState(SiegeLane.LaneStateEnum newLaneState)
		{
			this.LaneState = newLaneState;
		}

		// Token: 0x06001300 RID: 4864 RVA: 0x00049893 File Offset: 0x00047A93
		public void SetLastAssignedFormation(int teamIndex, Formation formation)
		{
			if (teamIndex >= 0)
			{
				this._lastAssignedFormations[teamIndex] = formation;
			}
		}

		// Token: 0x06001301 RID: 4865 RVA: 0x000498A2 File Offset: 0x00047AA2
		public void SetSiegeQuerySystem(SiegeQuerySystem siegeQuerySystem)
		{
			this._siegeQuerySystem = siegeQuerySystem;
		}

		// Token: 0x06001302 RID: 4866 RVA: 0x000498AC File Offset: 0x00047AAC
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

		// Token: 0x06001303 RID: 4867 RVA: 0x000499AC File Offset: 0x00047BAC
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

		// Token: 0x06001304 RID: 4868 RVA: 0x000499EC File Offset: 0x00047BEC
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

		// Token: 0x06001305 RID: 4869 RVA: 0x00049A58 File Offset: 0x00047C58
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

		// Token: 0x06001306 RID: 4870 RVA: 0x00049AC4 File Offset: 0x00047CC4
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

		// Token: 0x06001307 RID: 4871 RVA: 0x00049B30 File Offset: 0x00047D30
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

		// Token: 0x06001308 RID: 4872 RVA: 0x00049C58 File Offset: 0x00047E58
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

		// Token: 0x06001309 RID: 4873 RVA: 0x00049C90 File Offset: 0x00047E90
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

		// Token: 0x0600130A RID: 4874 RVA: 0x00049E60 File Offset: 0x00048060
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

		// Token: 0x0600130B RID: 4875 RVA: 0x00049FC4 File Offset: 0x000481C4
		public void SetPrimarySiegeWeapons(List<IPrimarySiegeWeapon> primarySiegeWeapons)
		{
			this.PrimarySiegeWeapons = primarySiegeWeapons;
		}

		// Token: 0x0600130C RID: 4876 RVA: 0x00049FD0 File Offset: 0x000481D0
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

		// Token: 0x04000549 RID: 1353
		private readonly Formation[] _lastAssignedFormations;

		// Token: 0x0400054A RID: 1354
		private SiegeQuerySystem _siegeQuerySystem;

		// Token: 0x0400054B RID: 1355
		private SiegeWeapon _attackerMovableWeapon;

		// Token: 0x020004E7 RID: 1255
		public enum LaneStateEnum
		{
			// Token: 0x04001B19 RID: 6937
			Safe,
			// Token: 0x04001B1A RID: 6938
			Unused,
			// Token: 0x04001B1B RID: 6939
			Used,
			// Token: 0x04001B1C RID: 6940
			Active,
			// Token: 0x04001B1D RID: 6941
			Abandoned,
			// Token: 0x04001B1E RID: 6942
			Contested,
			// Token: 0x04001B1F RID: 6943
			Conceited
		}

		// Token: 0x020004E8 RID: 1256
		public enum LaneDefenseStates
		{
			// Token: 0x04001B21 RID: 6945
			Empty,
			// Token: 0x04001B22 RID: 6946
			Token,
			// Token: 0x04001B23 RID: 6947
			Full
		}
	}
}
