using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200016C RID: 364
	public class ArcherPosition
	{
		// Token: 0x17000417 RID: 1047
		// (get) Token: 0x060012B7 RID: 4791 RVA: 0x00048AD0 File Offset: 0x00046CD0
		public GameEntity Entity { get; }

		// Token: 0x17000418 RID: 1048
		// (get) Token: 0x060012B8 RID: 4792 RVA: 0x00048AD8 File Offset: 0x00046CD8
		public TacticalPosition TacticalArcherPosition { get; }

		// Token: 0x17000419 RID: 1049
		// (get) Token: 0x060012B9 RID: 4793 RVA: 0x00048AE0 File Offset: 0x00046CE0
		// (set) Token: 0x060012BA RID: 4794 RVA: 0x00048AE8 File Offset: 0x00046CE8
		public int ConnectedSides
		{
			get
			{
				return this._connectedSides;
			}
			private set
			{
				this._connectedSides = value;
			}
		}

		// Token: 0x060012BB RID: 4795 RVA: 0x00048AF1 File Offset: 0x00046CF1
		public Formation GetLastAssignedFormation(int teamIndex)
		{
			if (teamIndex >= 0)
			{
				return this._lastAssignedFormations[teamIndex];
			}
			return null;
		}

		// Token: 0x060012BC RID: 4796 RVA: 0x00048B04 File Offset: 0x00046D04
		public ArcherPosition(GameEntity _entity, SiegeQuerySystem siegeQuerySystem, BattleSideEnum battleSide)
		{
			this.Entity = _entity;
			this.TacticalArcherPosition = this.Entity.GetFirstScriptOfType<TacticalPosition>();
			this._siegeQuerySystem = siegeQuerySystem;
			this.DetermineArcherPositionSide(battleSide);
			this._lastAssignedFormations = new Formation[Mission.Current.Teams.Count];
		}

		// Token: 0x060012BD RID: 4797 RVA: 0x00048B57 File Offset: 0x00046D57
		private static int ConvertToBinaryPow(int pow)
		{
			return 1 << pow;
		}

		// Token: 0x060012BE RID: 4798 RVA: 0x00048B5F File Offset: 0x00046D5F
		public bool IsArcherPositionRelatedToSide(FormationAI.BehaviorSide side)
		{
			return (ArcherPosition.ConvertToBinaryPow((int)side) & this.ConnectedSides) != 0;
		}

		// Token: 0x060012BF RID: 4799 RVA: 0x00048B71 File Offset: 0x00046D71
		public FormationAI.BehaviorSide GetArcherPositionClosestSide()
		{
			return this._closestSide;
		}

		// Token: 0x060012C0 RID: 4800 RVA: 0x00048B79 File Offset: 0x00046D79
		public void OnDeploymentFinished(SiegeQuerySystem siegeQuerySystem, BattleSideEnum battleSide)
		{
			this._siegeQuerySystem = siegeQuerySystem;
			this.DetermineArcherPositionSide(battleSide);
		}

		// Token: 0x060012C1 RID: 4801 RVA: 0x00048B8C File Offset: 0x00046D8C
		private void DetermineArcherPositionSide(BattleSideEnum battleSide)
		{
			this.ConnectedSides = 0;
			if (this.TacticalArcherPosition != null)
			{
				int tacticalPositionSide = (int)this.TacticalArcherPosition.TacticalPositionSide;
				if (tacticalPositionSide < 3)
				{
					this._closestSide = this.TacticalArcherPosition.TacticalPositionSide;
					this.ConnectedSides = ArcherPosition.ConvertToBinaryPow(tacticalPositionSide);
				}
			}
			if (this.ConnectedSides == 0)
			{
				if (battleSide == BattleSideEnum.Defender)
				{
					ArcherPosition.CalculateArcherPositionSideUsingDefenderLanes(this._siegeQuerySystem, this.Entity.GlobalPosition, out this._closestSide, out this._connectedSides);
					return;
				}
				ArcherPosition.CalculateArcherPositionSideUsingAttackerRegions(this._siegeQuerySystem, this.Entity.GlobalPosition, out this._closestSide, out this._connectedSides);
			}
		}

		// Token: 0x060012C2 RID: 4802 RVA: 0x00048C28 File Offset: 0x00046E28
		private static void CalculateArcherPositionSideUsingAttackerRegions(SiegeQuerySystem siegeQuerySystem, Vec3 position, out FormationAI.BehaviorSide _closestSide, out int ConnectedSides)
		{
			float num = position.DistanceSquared(siegeQuerySystem.LeftAttackerOrigin);
			float num2 = position.DistanceSquared(siegeQuerySystem.MiddleAttackerOrigin);
			float num3 = position.DistanceSquared(siegeQuerySystem.RightAttackerOrigin);
			FormationAI.BehaviorSide behaviorSide;
			if (num < num2 && num < num3)
			{
				behaviorSide = FormationAI.BehaviorSide.Left;
			}
			else if (num3 < num2)
			{
				behaviorSide = FormationAI.BehaviorSide.Right;
			}
			else
			{
				behaviorSide = FormationAI.BehaviorSide.Middle;
			}
			_closestSide = behaviorSide;
			ConnectedSides = ArcherPosition.ConvertToBinaryPow((int)behaviorSide);
			Vec2 vec = position.AsVec2 - siegeQuerySystem.LeftDefenderOrigin.AsVec2;
			if (vec.DotProduct(siegeQuerySystem.LeftToMidDir) >= 0f && vec.DotProduct(siegeQuerySystem.LeftToMidDir.RightVec()) >= 0f)
			{
				ConnectedSides |= ArcherPosition.ConvertToBinaryPow(0);
			}
			else
			{
				vec = position.AsVec2 - siegeQuerySystem.MidDefenderOrigin.AsVec2;
				if (vec.DotProduct(siegeQuerySystem.MidToLeftDir) >= 0f && vec.DotProduct(siegeQuerySystem.MidToLeftDir.RightVec()) >= 0f)
				{
					ConnectedSides |= ArcherPosition.ConvertToBinaryPow(0);
				}
			}
			vec = position.AsVec2 - siegeQuerySystem.MidDefenderOrigin.AsVec2;
			if (vec.DotProduct(siegeQuerySystem.LeftToMidDir) >= 0f && vec.DotProduct(siegeQuerySystem.LeftToMidDir.LeftVec()) >= 0f)
			{
				ConnectedSides |= ArcherPosition.ConvertToBinaryPow(1);
			}
			else
			{
				vec = position.AsVec2 - siegeQuerySystem.RightDefenderOrigin.AsVec2;
				if (vec.DotProduct(siegeQuerySystem.RightToMidDir) >= 0f && vec.DotProduct(siegeQuerySystem.RightToMidDir.RightVec()) >= 0f)
				{
					ConnectedSides |= ArcherPosition.ConvertToBinaryPow(1);
				}
			}
			vec = position.AsVec2 - siegeQuerySystem.RightDefenderOrigin.AsVec2;
			if (vec.DotProduct(siegeQuerySystem.MidToRightDir) >= 0f && vec.DotProduct(siegeQuerySystem.MidToRightDir.LeftVec()) >= 0f)
			{
				ConnectedSides |= ArcherPosition.ConvertToBinaryPow(2);
				return;
			}
			vec = position.AsVec2 - siegeQuerySystem.RightDefenderOrigin.AsVec2;
			if (vec.DotProduct(siegeQuerySystem.RightToMidDir) >= 0f && vec.DotProduct(siegeQuerySystem.RightToMidDir.LeftVec()) >= 0f)
			{
				ConnectedSides |= ArcherPosition.ConvertToBinaryPow(2);
			}
		}

		// Token: 0x060012C3 RID: 4803 RVA: 0x00048EA0 File Offset: 0x000470A0
		private static void CalculateArcherPositionSideUsingDefenderLanes(SiegeQuerySystem siegeQuerySystem, Vec3 position, out FormationAI.BehaviorSide _closestSide, out int ConnectedSides)
		{
			float num = position.DistanceSquared(siegeQuerySystem.LeftDefenderOrigin);
			float num2 = position.DistanceSquared(siegeQuerySystem.MidDefenderOrigin);
			float num3 = position.DistanceSquared(siegeQuerySystem.RightDefenderOrigin);
			FormationAI.BehaviorSide behaviorSide;
			if (num < num2 && num < num3)
			{
				behaviorSide = FormationAI.BehaviorSide.Left;
			}
			else if (num3 < num2)
			{
				behaviorSide = FormationAI.BehaviorSide.Right;
			}
			else
			{
				behaviorSide = FormationAI.BehaviorSide.Middle;
			}
			FormationAI.BehaviorSide behaviorSide2 = FormationAI.BehaviorSide.BehaviorSideNotSet;
			switch (behaviorSide)
			{
			case FormationAI.BehaviorSide.Left:
				if ((position.AsVec2 - siegeQuerySystem.LeftDefenderOrigin.AsVec2).Normalized().DotProduct(siegeQuerySystem.DefenderLeftToDefenderMidDir) > 0f)
				{
					behaviorSide2 = FormationAI.BehaviorSide.Middle;
				}
				break;
			case FormationAI.BehaviorSide.Middle:
				if ((position.AsVec2 - siegeQuerySystem.MidDefenderOrigin.AsVec2).Normalized().DotProduct(siegeQuerySystem.DefenderMidToDefenderRightDir) > 0f)
				{
					behaviorSide2 = FormationAI.BehaviorSide.Right;
				}
				else
				{
					behaviorSide2 = FormationAI.BehaviorSide.Left;
				}
				break;
			case FormationAI.BehaviorSide.Right:
				if ((position.AsVec2 - siegeQuerySystem.RightDefenderOrigin.AsVec2).Normalized().DotProduct(siegeQuerySystem.DefenderMidToDefenderRightDir) < 0f)
				{
					behaviorSide2 = FormationAI.BehaviorSide.Middle;
				}
				break;
			}
			_closestSide = behaviorSide;
			ConnectedSides = ArcherPosition.ConvertToBinaryPow((int)behaviorSide);
			if (behaviorSide2 != FormationAI.BehaviorSide.BehaviorSideNotSet)
			{
				ConnectedSides |= ArcherPosition.ConvertToBinaryPow((int)behaviorSide2);
			}
		}

		// Token: 0x060012C4 RID: 4804 RVA: 0x00048FEB File Offset: 0x000471EB
		public void SetLastAssignedFormation(int teamIndex, Formation formation)
		{
			if (teamIndex >= 0)
			{
				this._lastAssignedFormations[teamIndex] = formation;
			}
		}

		// Token: 0x0400053C RID: 1340
		private FormationAI.BehaviorSide _closestSide;

		// Token: 0x0400053D RID: 1341
		private int _connectedSides;

		// Token: 0x0400053E RID: 1342
		private SiegeQuerySystem _siegeQuerySystem;

		// Token: 0x0400053F RID: 1343
		private readonly Formation[] _lastAssignedFormations;
	}
}
