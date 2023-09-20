using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200013F RID: 319
	public struct ArrangementOrder
	{
		// Token: 0x06001038 RID: 4152 RVA: 0x00033C69 File Offset: 0x00031E69
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

		// Token: 0x06001039 RID: 4153 RVA: 0x00033C7A File Offset: 0x00031E7A
		public static bool GetUnitLooseness(ArrangementOrder.ArrangementOrderEnum a)
		{
			return a != ArrangementOrder.ArrangementOrderEnum.ShieldWall;
		}

		// Token: 0x0600103A RID: 4154 RVA: 0x00033C84 File Offset: 0x00031E84
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

		// Token: 0x0600103B RID: 4155 RVA: 0x00033D3C File Offset: 0x00031F3C
		public void GetMovementSpeedRestriction(out float? runRestriction, out float? walkRestriction)
		{
			runRestriction = this._runRestriction;
			walkRestriction = this._walkRestriction;
		}

		// Token: 0x0600103C RID: 4156 RVA: 0x00033D58 File Offset: 0x00031F58
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

		// Token: 0x0600103D RID: 4157 RVA: 0x00033DA8 File Offset: 0x00031FA8
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

		// Token: 0x0600103E RID: 4158 RVA: 0x00033ED9 File Offset: 0x000320D9
		public void SoftUpdate(Formation formation)
		{
			if (this.OrderEnum == ArrangementOrder.ArrangementOrderEnum.Scatter)
			{
				this.TickOccasionally(formation);
				formation.ResetArrangementOrderTickTimer();
			}
		}

		// Token: 0x0600103F RID: 4159 RVA: 0x00033EF4 File Offset: 0x000320F4
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

		// Token: 0x06001040 RID: 4160 RVA: 0x00033F6F File Offset: 0x0003216F
		public int GetUnitSpacing()
		{
			return this._unitSpacing;
		}

		// Token: 0x06001041 RID: 4161 RVA: 0x00033F77 File Offset: 0x00032177
		public void Rearrange(Formation formation)
		{
			if (this.OrderEnum == ArrangementOrder.ArrangementOrderEnum.Column)
			{
				this.RearrangeAux(formation, false);
				return;
			}
			formation.Rearrange(this.GetArrangement(formation));
		}

		// Token: 0x06001042 RID: 4162 RVA: 0x00033F98 File Offset: 0x00032198
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

		// Token: 0x06001043 RID: 4163 RVA: 0x00034030 File Offset: 0x00032230
		private unsafe static void TransposeLineFormation(Formation formation)
		{
			formation.Rearrange(new TransposedLineFormation(formation));
			MovementOrder movementOrder = *formation.GetReadonlyMovementOrderReference();
			formation.SetPositioning(new WorldPosition?(movementOrder.CreateNewOrderWorldPosition(formation, WorldPosition.WorldPositionEnforcedCache.None)), null, null);
			formation.ReferencePosition = new Vec2?(formation.OrderPosition);
		}

		// Token: 0x06001044 RID: 4164 RVA: 0x0003408C File Offset: 0x0003228C
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

		// Token: 0x06001045 RID: 4165 RVA: 0x00034194 File Offset: 0x00032394
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

		// Token: 0x06001046 RID: 4166 RVA: 0x000341FC File Offset: 0x000323FC
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

		// Token: 0x06001047 RID: 4167 RVA: 0x0003424C File Offset: 0x0003244C
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

		// Token: 0x06001048 RID: 4168 RVA: 0x00034318 File Offset: 0x00032518
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

		// Token: 0x17000384 RID: 900
		// (get) Token: 0x06001049 RID: 4169 RVA: 0x0003446C File Offset: 0x0003266C
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

		// Token: 0x0600104A RID: 4170 RVA: 0x000344C2 File Offset: 0x000326C2
		public ArrangementOrder.ArrangementOrderEnum GetNativeEnum()
		{
			return this.OrderEnum;
		}

		// Token: 0x0600104B RID: 4171 RVA: 0x000344CA File Offset: 0x000326CA
		public override bool Equals(object obj)
		{
			return obj is ArrangementOrder && (ArrangementOrder)obj == this;
		}

		// Token: 0x0600104C RID: 4172 RVA: 0x000344E7 File Offset: 0x000326E7
		public override int GetHashCode()
		{
			return (int)this.OrderEnum;
		}

		// Token: 0x0600104D RID: 4173 RVA: 0x000344EF File Offset: 0x000326EF
		public static bool operator !=(ArrangementOrder a1, ArrangementOrder a2)
		{
			return a1.OrderEnum != a2.OrderEnum;
		}

		// Token: 0x0600104E RID: 4174 RVA: 0x00034502 File Offset: 0x00032702
		public static bool operator ==(ArrangementOrder a1, ArrangementOrder a2)
		{
			return a1.OrderEnum == a2.OrderEnum;
		}

		// Token: 0x0600104F RID: 4175 RVA: 0x00034514 File Offset: 0x00032714
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

		// Token: 0x06001050 RID: 4176 RVA: 0x000345AE File Offset: 0x000327AE
		public static int GetArrangementOrderDefensiveness(ArrangementOrder.ArrangementOrderEnum orderEnum)
		{
			if (orderEnum == ArrangementOrder.ArrangementOrderEnum.Circle || orderEnum == ArrangementOrder.ArrangementOrderEnum.ShieldWall || orderEnum == ArrangementOrder.ArrangementOrderEnum.Square)
			{
				return 1;
			}
			return 0;
		}

		// Token: 0x06001051 RID: 4177 RVA: 0x000345BE File Offset: 0x000327BE
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

		// Token: 0x06001052 RID: 4178 RVA: 0x000345E8 File Offset: 0x000327E8
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

		// Token: 0x0400040C RID: 1036
		private float? _walkRestriction;

		// Token: 0x0400040D RID: 1037
		private float? _runRestriction;

		// Token: 0x0400040E RID: 1038
		private int _unitSpacing;

		// Token: 0x0400040F RID: 1039
		public readonly ArrangementOrder.ArrangementOrderEnum OrderEnum;

		// Token: 0x04000410 RID: 1040
		public static readonly ArrangementOrder ArrangementOrderCircle = new ArrangementOrder(ArrangementOrder.ArrangementOrderEnum.Circle);

		// Token: 0x04000411 RID: 1041
		public static readonly ArrangementOrder ArrangementOrderColumn = new ArrangementOrder(ArrangementOrder.ArrangementOrderEnum.Column);

		// Token: 0x04000412 RID: 1042
		public static readonly ArrangementOrder ArrangementOrderLine = new ArrangementOrder(ArrangementOrder.ArrangementOrderEnum.Line);

		// Token: 0x04000413 RID: 1043
		public static readonly ArrangementOrder ArrangementOrderLoose = new ArrangementOrder(ArrangementOrder.ArrangementOrderEnum.Loose);

		// Token: 0x04000414 RID: 1044
		public static readonly ArrangementOrder ArrangementOrderScatter = new ArrangementOrder(ArrangementOrder.ArrangementOrderEnum.Scatter);

		// Token: 0x04000415 RID: 1045
		public static readonly ArrangementOrder ArrangementOrderShieldWall = new ArrangementOrder(ArrangementOrder.ArrangementOrderEnum.ShieldWall);

		// Token: 0x04000416 RID: 1046
		public static readonly ArrangementOrder ArrangementOrderSkein = new ArrangementOrder(ArrangementOrder.ArrangementOrderEnum.Skein);

		// Token: 0x04000417 RID: 1047
		public static readonly ArrangementOrder ArrangementOrderSquare = new ArrangementOrder(ArrangementOrder.ArrangementOrderEnum.Square);

		// Token: 0x0200047B RID: 1147
		public enum ArrangementOrderEnum
		{
			// Token: 0x04001945 RID: 6469
			Circle,
			// Token: 0x04001946 RID: 6470
			Column,
			// Token: 0x04001947 RID: 6471
			Line,
			// Token: 0x04001948 RID: 6472
			Loose,
			// Token: 0x04001949 RID: 6473
			Scatter,
			// Token: 0x0400194A RID: 6474
			ShieldWall,
			// Token: 0x0400194B RID: 6475
			Skein,
			// Token: 0x0400194C RID: 6476
			Square
		}
	}
}
