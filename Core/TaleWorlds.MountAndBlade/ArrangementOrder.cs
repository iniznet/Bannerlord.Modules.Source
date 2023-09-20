using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public struct ArrangementOrder
	{
		public static int GetUnitSpacingOf(ArrangementOrder.ArrangementOrderEnum a)
		{
			if (a == ArrangementOrder.ArrangementOrderEnum.Loose)
			{
				return 6;
			}
			if (a != ArrangementOrder.ArrangementOrderEnum.ShieldWall)
			{
				return 2;
			}
			return 0;
		}

		public static bool GetUnitLooseness(ArrangementOrder.ArrangementOrderEnum a)
		{
			return a != ArrangementOrder.ArrangementOrderEnum.ShieldWall;
		}

		public ArrangementOrder(ArrangementOrder.ArrangementOrderEnum orderEnum)
		{
			this.OrderEnum = orderEnum;
			this._walkRestriction = null;
			switch (this.OrderEnum)
			{
			case ArrangementOrder.ArrangementOrderEnum.Circle:
				this._runRestriction = new float?(0.5f);
				goto IL_9A;
			case ArrangementOrder.ArrangementOrderEnum.Line:
				this._runRestriction = new float?(0.8f);
				goto IL_9A;
			case ArrangementOrder.ArrangementOrderEnum.Loose:
			case ArrangementOrder.ArrangementOrderEnum.Scatter:
			case ArrangementOrder.ArrangementOrderEnum.Skein:
				this._runRestriction = new float?(0.9f);
				goto IL_9A;
			case ArrangementOrder.ArrangementOrderEnum.ShieldWall:
			case ArrangementOrder.ArrangementOrderEnum.Square:
				this._runRestriction = new float?(0.3f);
				goto IL_9A;
			}
			this._runRestriction = new float?(1f);
			IL_9A:
			this._unitSpacing = ArrangementOrder.GetUnitSpacingOf(this.OrderEnum);
		}

		public void GetMovementSpeedRestriction(out float? runRestriction, out float? walkRestriction)
		{
			runRestriction = this._runRestriction;
			walkRestriction = this._walkRestriction;
		}

		public IFormationArrangement GetArrangement(Formation formation)
		{
			ArrangementOrder.ArrangementOrderEnum orderEnum = this.OrderEnum;
			if (orderEnum <= ArrangementOrder.ArrangementOrderEnum.Column)
			{
				if (orderEnum == ArrangementOrder.ArrangementOrderEnum.Circle)
				{
					return new CircularFormation(formation);
				}
				if (orderEnum == ArrangementOrder.ArrangementOrderEnum.Column)
				{
					return new ColumnFormation(formation, null, 1);
				}
			}
			else
			{
				if (orderEnum == ArrangementOrder.ArrangementOrderEnum.Skein)
				{
					return new SkeinFormation(formation);
				}
				if (orderEnum == ArrangementOrder.ArrangementOrderEnum.Square)
				{
					return new RectilinearSchiltronFormation(formation);
				}
			}
			return new LineFormation(formation, true);
		}

		public void OnApply(Formation formation)
		{
			formation.SetPositioning(null, null, new int?(this.GetUnitSpacing()));
			this.Rearrange(formation);
			if (this.OrderEnum == ArrangementOrder.ArrangementOrderEnum.Scatter)
			{
				this.TickOccasionally(formation);
				formation.ResetArrangementOrderTickTimer();
			}
			ArrangementOrder.ArrangementOrderEnum orderEnum = this.OrderEnum;
			formation.ApplyActionOnEachUnit(delegate(Agent agent)
			{
				if (agent.IsAIControlled)
				{
					Agent.UsageDirection shieldDirectionOfUnit = ArrangementOrder.GetShieldDirectionOfUnit(formation, agent, orderEnum);
					agent.EnforceShieldUsage(shieldDirectionOfUnit);
				}
				agent.UpdateAgentProperties();
			}, null);
			if (formation.GetReadonlyMovementOrderReference().OrderEnum != MovementOrder.MovementOrderEnum.Charge && formation.GetReadonlyMovementOrderReference().OrderEnum != MovementOrder.MovementOrderEnum.ChargeToTarget)
			{
				if (this.OrderEnum != ArrangementOrder.ArrangementOrderEnum.Circle && this.OrderEnum != ArrangementOrder.ArrangementOrderEnum.ShieldWall && this.OrderEnum != ArrangementOrder.ArrangementOrderEnum.Square && this.OrderEnum != ArrangementOrder.ArrangementOrderEnum.Column)
				{
					formation.ApplyActionOnEachUnit(new Action<Agent>(MovementOrder.SetDefaultMoveBehaviorValues), null);
					return;
				}
				if (this.OrderEnum != ArrangementOrder.ArrangementOrderEnum.Column)
				{
					formation.ApplyActionOnEachUnit(new Action<Agent>(MovementOrder.SetDefensiveArrangementMoveBehaviorValues), null);
					return;
				}
				formation.ApplyActionOnEachUnit(new Action<Agent>(MovementOrder.SetFollowBehaviorValues), null);
			}
		}

		public void SoftUpdate(Formation formation)
		{
			if (this.OrderEnum == ArrangementOrder.ArrangementOrderEnum.Scatter)
			{
				this.TickOccasionally(formation);
				formation.ResetArrangementOrderTickTimer();
			}
		}

		public static Agent.UsageDirection GetShieldDirectionOfUnit(Formation formation, Agent unit, ArrangementOrder.ArrangementOrderEnum orderEnum)
		{
			Agent.UsageDirection usageDirection;
			if (unit.IsDetachedFromFormation)
			{
				usageDirection = Agent.UsageDirection.None;
			}
			else if (orderEnum == ArrangementOrder.ArrangementOrderEnum.ShieldWall)
			{
				if (((IFormationUnit)unit).FormationRankIndex == 0)
				{
					usageDirection = Agent.UsageDirection.DefendDown;
				}
				else if (formation.Arrangement.GetNeighborUnitOfLeftSide(unit) == null)
				{
					usageDirection = Agent.UsageDirection.DefendLeft;
				}
				else if (formation.Arrangement.GetNeighborUnitOfRightSide(unit) == null)
				{
					usageDirection = Agent.UsageDirection.DefendRight;
				}
				else
				{
					usageDirection = Agent.UsageDirection.AttackEnd;
				}
			}
			else if (orderEnum == ArrangementOrder.ArrangementOrderEnum.Circle || orderEnum == ArrangementOrder.ArrangementOrderEnum.Square)
			{
				if (((IFormationUnit)unit).IsShieldUsageEncouraged)
				{
					if (((IFormationUnit)unit).FormationRankIndex == 0)
					{
						usageDirection = Agent.UsageDirection.DefendDown;
					}
					else
					{
						usageDirection = Agent.UsageDirection.AttackEnd;
					}
				}
				else
				{
					usageDirection = Agent.UsageDirection.None;
				}
			}
			else
			{
				usageDirection = Agent.UsageDirection.None;
			}
			return usageDirection;
		}

		public int GetUnitSpacing()
		{
			return this._unitSpacing;
		}

		public void Rearrange(Formation formation)
		{
			if (this.OrderEnum == ArrangementOrder.ArrangementOrderEnum.Column)
			{
				this.RearrangeAux(formation, false);
				return;
			}
			formation.Rearrange(this.GetArrangement(formation));
		}

		public void RearrangeAux(Formation formation, bool isDirectly)
		{
			float num = MathF.Max(1f, MathF.Max(formation.Depth, formation.Width) * 0.8f);
			float lengthSquared = (formation.CurrentPosition - formation.OrderPosition).LengthSquared;
			if (!isDirectly && lengthSquared < num * num)
			{
				ArrangementOrder.TransposeLineFormation(formation);
				formation.OnTick += formation.TickForColumnArrangementInitialPositioning;
				return;
			}
			formation.OnTick -= formation.TickForColumnArrangementInitialPositioning;
			formation.ReferencePosition = null;
			formation.Rearrange(this.GetArrangement(formation));
		}

		private unsafe static void TransposeLineFormation(Formation formation)
		{
			formation.Rearrange(new TransposedLineFormation(formation));
			MovementOrder movementOrder = *formation.GetReadonlyMovementOrderReference();
			formation.SetPositioning(new WorldPosition?(movementOrder.CreateNewOrderWorldPosition(formation, WorldPosition.WorldPositionEnforcedCache.None)), null, null);
			formation.ReferencePosition = new Vec2?(formation.OrderPosition);
		}

		public void OnCancel(Formation formation)
		{
			if (this.OrderEnum == ArrangementOrder.ArrangementOrderEnum.Scatter)
			{
				Team team = formation.Team;
				if (((team != null) ? team.TeamAI : null) != null)
				{
					MBReadOnlyList<StrategicArea> strategicAreas = formation.Team.TeamAI.StrategicAreas;
					for (int i = formation.Detachments.Count - 1; i >= 0; i--)
					{
						IDetachment detachment = formation.Detachments[i];
						foreach (StrategicArea strategicArea in strategicAreas)
						{
							if (detachment == strategicArea)
							{
								formation.LeaveDetachment(detachment);
								break;
							}
						}
					}
				}
			}
			if (this.OrderEnum == ArrangementOrder.ArrangementOrderEnum.ShieldWall || this.OrderEnum == ArrangementOrder.ArrangementOrderEnum.Circle || this.OrderEnum == ArrangementOrder.ArrangementOrderEnum.Square)
			{
				formation.ApplyActionOnEachUnit(delegate(Agent agent)
				{
					if (agent.IsAIControlled)
					{
						agent.EnforceShieldUsage(Agent.UsageDirection.None);
					}
				}, null);
			}
			if (this.OrderEnum == ArrangementOrder.ArrangementOrderEnum.Column)
			{
				formation.OnTick -= formation.TickForColumnArrangementInitialPositioning;
			}
		}

		private static StrategicArea CreateStrategicArea(Scene scene, WorldPosition position, Vec2 direction, float width, int capacity, BattleSideEnum side)
		{
			WorldFrame worldFrame = new WorldFrame(new Mat3
			{
				f = direction.ToVec3(0f),
				u = Vec3.Up
			}, position);
			GameEntity gameEntity = GameEntity.Instantiate(scene, "strategic_area_autogen", worldFrame.ToNavMeshMatrixFrame());
			gameEntity.SetMobility(GameEntity.Mobility.dynamic);
			StrategicArea firstScriptOfType = gameEntity.GetFirstScriptOfType<StrategicArea>();
			firstScriptOfType.InitializeAutogenerated(width, capacity, side);
			return firstScriptOfType;
		}

		private static IEnumerable<StrategicArea> CreateStrategicAreas(Mission mission, int count, WorldPosition center, float distance, WorldPosition target, float width, int capacity, BattleSideEnum side)
		{
			Scene scene = mission.Scene;
			float distanceMultiplied = distance * 0.7f;
			Func<WorldPosition> func = delegate
			{
				WorldPosition center2 = center;
				float num2 = MBRandom.RandomFloat * 3.1415927f * 2f;
				center2.SetVec2(center.AsVec2 + Vec2.FromRotation(num2) * distanceMultiplied);
				return center2;
			};
			WorldPosition[] array = delegate
			{
				float num3 = MBRandom.RandomFloat * 3.1415927f * 2f;
				switch (count)
				{
				case 2:
				{
					WorldPosition center3 = center;
					center3.SetVec2(center.AsVec2 + Vec2.FromRotation(num3) * distanceMultiplied);
					WorldPosition center4 = center;
					center4.SetVec2(center.AsVec2 + Vec2.FromRotation(num3 + 3.1415927f) * distanceMultiplied);
					return new WorldPosition[] { center3, center4 };
				}
				case 3:
				{
					WorldPosition center5 = center;
					center5.SetVec2(center.AsVec2 + Vec2.FromRotation(num3 + 0f) * distanceMultiplied);
					WorldPosition center6 = center;
					center6.SetVec2(center.AsVec2 + Vec2.FromRotation(num3 + 2.0943952f) * distanceMultiplied);
					WorldPosition center7 = center;
					center7.SetVec2(center.AsVec2 + Vec2.FromRotation(num3 + 4.1887903f) * distanceMultiplied);
					return new WorldPosition[] { center5, center6, center7 };
				}
				case 4:
				{
					WorldPosition center8 = center;
					center8.SetVec2(center.AsVec2 + Vec2.FromRotation(num3 + 0f) * distanceMultiplied);
					WorldPosition center9 = center;
					center9.SetVec2(center.AsVec2 + Vec2.FromRotation(num3 + 1.5707964f) * distanceMultiplied);
					WorldPosition center10 = center;
					center10.SetVec2(center.AsVec2 + Vec2.FromRotation(num3 + 3.1415927f) * distanceMultiplied);
					WorldPosition center11 = center;
					center11.SetVec2(center.AsVec2 + Vec2.FromRotation(num3 + 4.712389f) * distanceMultiplied);
					return new WorldPosition[] { center8, center9, center10, center11 };
				}
				default:
					Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\Orders\\ArrangementOrder.cs", "CreateStrategicAreas", 379);
					return new WorldPosition[0];
				}
			}();
			List<WorldPosition> positions = new List<WorldPosition>();
			WorldPosition[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				WorldPosition worldPosition = array2[i];
				WorldPosition worldPosition2 = worldPosition;
				WorldPosition position = mission.FindPositionWithBiggestSlopeTowardsDirectionInSquare(ref worldPosition2, distance * 0.25f, ref target);
				Func<WorldPosition, bool> func2 = delegate(WorldPosition p)
				{
					float num4;
					if (!positions.Any((WorldPosition wp) => wp.AsVec2.DistanceSquared(p.AsVec2) < 1f) && (scene.GetPathDistanceBetweenPositions(ref center, ref p, 0f, out num4) && num4 < center.AsVec2.Distance(p.AsVec2) * 2f))
					{
						positions.Add(position);
						return true;
					}
					return false;
				};
				if (!func2(position) && !func2(worldPosition))
				{
					int num = 0;
					while (num++ < 10 && !func2(func()))
					{
					}
					if (num >= 10)
					{
						positions.Add(center);
					}
				}
			}
			Vec2 direction = (target.AsVec2 - center.AsVec2).Normalized();
			foreach (WorldPosition worldPosition3 in positions)
			{
				yield return ArrangementOrder.CreateStrategicArea(scene, worldPosition3, direction, width, capacity, side);
			}
			List<WorldPosition>.Enumerator enumerator = default(List<WorldPosition>.Enumerator);
			yield break;
			yield break;
		}

		private bool IsStrategicAreaClose(StrategicArea strategicArea, Formation formation)
		{
			if (!strategicArea.IsUsableBy(formation.Team.Side))
			{
				return false;
			}
			if (strategicArea.IgnoreHeight)
			{
				return MathF.Abs(strategicArea.GameEntity.GlobalPosition.x - formation.OrderPosition.X) <= strategicArea.DistanceToCheck && MathF.Abs(strategicArea.GameEntity.GlobalPosition.y - formation.OrderPosition.Y) <= strategicArea.DistanceToCheck;
			}
			WorldPosition worldPosition = formation.CreateNewOrderWorldPosition(WorldPosition.WorldPositionEnforcedCache.None);
			Vec3 globalPosition = strategicArea.GameEntity.GlobalPosition;
			return worldPosition.DistanceSquaredWithLimit(globalPosition, strategicArea.DistanceToCheck * strategicArea.DistanceToCheck + 1E-05f) < strategicArea.DistanceToCheck * strategicArea.DistanceToCheck;
		}

		public void TickOccasionally(Formation formation)
		{
			if (this.OrderEnum == ArrangementOrder.ArrangementOrderEnum.Scatter)
			{
				Team team = formation.Team;
				if (((team != null) ? team.TeamAI : null) != null)
				{
					MBReadOnlyList<StrategicArea> strategicAreas = formation.Team.TeamAI.StrategicAreas;
					foreach (StrategicArea strategicArea in strategicAreas)
					{
						if (this.IsStrategicAreaClose(strategicArea, formation))
						{
							bool flag = false;
							foreach (IDetachment detachment in formation.Detachments)
							{
								if (strategicArea == detachment)
								{
									flag = true;
									break;
								}
							}
							if (!flag)
							{
								formation.JoinDetachment(strategicArea);
							}
						}
					}
					for (int i = formation.Detachments.Count - 1; i >= 0; i--)
					{
						IDetachment detachment2 = formation.Detachments[i];
						foreach (StrategicArea strategicArea2 in strategicAreas)
						{
							if (detachment2 == strategicArea2 && !this.IsStrategicAreaClose(strategicArea2, formation))
							{
								formation.LeaveDetachment(detachment2);
								break;
							}
						}
					}
				}
			}
		}

		public OrderType OrderType
		{
			get
			{
				switch (this.OrderEnum)
				{
				case ArrangementOrder.ArrangementOrderEnum.Circle:
					return OrderType.ArrangementCircular;
				case ArrangementOrder.ArrangementOrderEnum.Column:
					return OrderType.ArrangementColumn;
				case ArrangementOrder.ArrangementOrderEnum.Line:
					return OrderType.ArrangementLine;
				case ArrangementOrder.ArrangementOrderEnum.Loose:
					return OrderType.ArrangementLoose;
				case ArrangementOrder.ArrangementOrderEnum.Scatter:
					return OrderType.ArrangementScatter;
				case ArrangementOrder.ArrangementOrderEnum.ShieldWall:
					return OrderType.ArrangementCloseOrder;
				case ArrangementOrder.ArrangementOrderEnum.Skein:
					return OrderType.ArrangementVee;
				case ArrangementOrder.ArrangementOrderEnum.Square:
					return OrderType.ArrangementSchiltron;
				default:
					return OrderType.ArrangementLine;
				}
			}
		}

		public ArrangementOrder.ArrangementOrderEnum GetNativeEnum()
		{
			return this.OrderEnum;
		}

		public override bool Equals(object obj)
		{
			return obj is ArrangementOrder && (ArrangementOrder)obj == this;
		}

		public override int GetHashCode()
		{
			return (int)this.OrderEnum;
		}

		public static bool operator !=(ArrangementOrder a1, ArrangementOrder a2)
		{
			return a1.OrderEnum != a2.OrderEnum;
		}

		public static bool operator ==(ArrangementOrder a1, ArrangementOrder a2)
		{
			return a1.OrderEnum == a2.OrderEnum;
		}

		public void OnOrderPositionChanged(Formation formation, Vec2 previousOrderPosition)
		{
			if (this.OrderEnum == ArrangementOrder.ArrangementOrderEnum.Column && formation.Arrangement is TransposedLineFormation)
			{
				Vec2 direction = formation.Direction;
				Vec2 vec = (formation.OrderPosition - previousOrderPosition).Normalized();
				float num = direction.AngleBetween(vec);
				if ((num > 1.5707964f || num < -1.5707964f) && formation.QuerySystem.AveragePosition.DistanceSquared(formation.OrderPosition) < formation.Depth * formation.Depth / 10f)
				{
					formation.ReferencePosition = new Vec2?(formation.OrderPosition);
				}
			}
		}

		public static int GetArrangementOrderDefensiveness(ArrangementOrder.ArrangementOrderEnum orderEnum)
		{
			if (orderEnum == ArrangementOrder.ArrangementOrderEnum.Circle || orderEnum == ArrangementOrder.ArrangementOrderEnum.ShieldWall || orderEnum == ArrangementOrder.ArrangementOrderEnum.Square)
			{
				return 1;
			}
			return 0;
		}

		public static int GetArrangementOrderDefensivenessChange(ArrangementOrder.ArrangementOrderEnum previousOrderEnum, ArrangementOrder.ArrangementOrderEnum nextOrderEnum)
		{
			if (previousOrderEnum == ArrangementOrder.ArrangementOrderEnum.Circle || previousOrderEnum == ArrangementOrder.ArrangementOrderEnum.ShieldWall || previousOrderEnum == ArrangementOrder.ArrangementOrderEnum.Square)
			{
				if (nextOrderEnum != ArrangementOrder.ArrangementOrderEnum.Circle && nextOrderEnum != ArrangementOrder.ArrangementOrderEnum.ShieldWall && nextOrderEnum != ArrangementOrder.ArrangementOrderEnum.Square)
				{
					return -1;
				}
				return 0;
			}
			else
			{
				if (nextOrderEnum == ArrangementOrder.ArrangementOrderEnum.Circle || nextOrderEnum == ArrangementOrder.ArrangementOrderEnum.ShieldWall || nextOrderEnum == ArrangementOrder.ArrangementOrderEnum.Square)
				{
					return 1;
				}
				return 0;
			}
		}

		public float CalculateFormationDirectionEnforcingFactorForRank(int formationRankIndex, int rankCount)
		{
			if (this.OrderEnum == ArrangementOrder.ArrangementOrderEnum.Circle || this.OrderEnum == ArrangementOrder.ArrangementOrderEnum.ShieldWall || this.OrderEnum == ArrangementOrder.ArrangementOrderEnum.Square)
			{
				return 1f - MBMath.ClampFloat(((float)formationRankIndex + 1f) / ((float)rankCount * 2f), 0f, 1f);
			}
			if (this.OrderEnum == ArrangementOrder.ArrangementOrderEnum.Column)
			{
				return 0f;
			}
			return 1f - MBMath.ClampFloat(((float)formationRankIndex + 1f) / ((float)rankCount * 0.5f), 0f, 1f);
		}

		private float? _walkRestriction;

		private float? _runRestriction;

		private int _unitSpacing;

		public readonly ArrangementOrder.ArrangementOrderEnum OrderEnum;

		public static readonly ArrangementOrder ArrangementOrderCircle = new ArrangementOrder(ArrangementOrder.ArrangementOrderEnum.Circle);

		public static readonly ArrangementOrder ArrangementOrderColumn = new ArrangementOrder(ArrangementOrder.ArrangementOrderEnum.Column);

		public static readonly ArrangementOrder ArrangementOrderLine = new ArrangementOrder(ArrangementOrder.ArrangementOrderEnum.Line);

		public static readonly ArrangementOrder ArrangementOrderLoose = new ArrangementOrder(ArrangementOrder.ArrangementOrderEnum.Loose);

		public static readonly ArrangementOrder ArrangementOrderScatter = new ArrangementOrder(ArrangementOrder.ArrangementOrderEnum.Scatter);

		public static readonly ArrangementOrder ArrangementOrderShieldWall = new ArrangementOrder(ArrangementOrder.ArrangementOrderEnum.ShieldWall);

		public static readonly ArrangementOrder ArrangementOrderSkein = new ArrangementOrder(ArrangementOrder.ArrangementOrderEnum.Skein);

		public static readonly ArrangementOrder ArrangementOrderSquare = new ArrangementOrder(ArrangementOrder.ArrangementOrderEnum.Square);

		public enum ArrangementOrderEnum
		{
			Circle,
			Column,
			Line,
			Loose,
			Scatter,
			ShieldWall,
			Skein,
			Square
		}
	}
}
