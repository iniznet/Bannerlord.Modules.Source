using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class ArcherPosition
	{
		public GameEntity Entity { get; }

		public TacticalPosition TacticalArcherPosition { get; }

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

		public Formation GetLastAssignedFormation(int teamIndex)
		{
			if (teamIndex >= 0)
			{
				return this._lastAssignedFormations[teamIndex];
			}
			return null;
		}

		public ArcherPosition(GameEntity _entity, SiegeQuerySystem siegeQuerySystem, BattleSideEnum battleSide)
		{
			this.Entity = _entity;
			this.TacticalArcherPosition = this.Entity.GetFirstScriptOfType<TacticalPosition>();
			this._siegeQuerySystem = siegeQuerySystem;
			this.DetermineArcherPositionSide(battleSide);
			this._lastAssignedFormations = new Formation[Mission.Current.Teams.Count];
		}

		private static int ConvertToBinaryPow(int pow)
		{
			return 1 << pow;
		}

		public bool IsArcherPositionRelatedToSide(FormationAI.BehaviorSide side)
		{
			return (ArcherPosition.ConvertToBinaryPow((int)side) & this.ConnectedSides) != 0;
		}

		public FormationAI.BehaviorSide GetArcherPositionClosestSide()
		{
			return this._closestSide;
		}

		public void OnDeploymentFinished(SiegeQuerySystem siegeQuerySystem, BattleSideEnum battleSide)
		{
			this._siegeQuerySystem = siegeQuerySystem;
			this.DetermineArcherPositionSide(battleSide);
		}

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

		public void SetLastAssignedFormation(int teamIndex, Formation formation)
		{
			if (teamIndex >= 0)
			{
				this._lastAssignedFormations[teamIndex] = formation;
			}
		}

		private FormationAI.BehaviorSide _closestSide;

		private int _connectedSides;

		private SiegeQuerySystem _siegeQuerySystem;

		private readonly Formation[] _lastAssignedFormations;
	}
}
