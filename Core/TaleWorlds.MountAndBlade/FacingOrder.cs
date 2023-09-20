using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public struct FacingOrder
	{
		public static FacingOrder FacingOrderLookAtDirection(Vec2 direction)
		{
			return new FacingOrder(FacingOrder.FacingOrderEnum.LookAtDirection, direction);
		}

		private FacingOrder(FacingOrder.FacingOrderEnum orderEnum, Vec2 direction)
		{
			this.OrderEnum = orderEnum;
			this._lookAtDirection = direction;
		}

		private FacingOrder(FacingOrder.FacingOrderEnum orderEnum)
		{
			this.OrderEnum = orderEnum;
			this._lookAtDirection = Vec2.Invalid;
		}

		private Vec2 GetDirectionAux(Formation f, Agent targetAgent)
		{
			if (f.IsMounted() && targetAgent != null && targetAgent.Velocity.LengthSquared > targetAgent.RunSpeedCached * targetAgent.RunSpeedCached * 0.09f)
			{
				return targetAgent.Velocity.AsVec2.Normalized();
			}
			if (this.OrderEnum == FacingOrder.FacingOrderEnum.LookAtDirection)
			{
				return this._lookAtDirection;
			}
			Vec2 currentPosition = f.CurrentPosition;
			Vec2 weightedAverageEnemyPosition = f.QuerySystem.WeightedAverageEnemyPosition;
			if (!weightedAverageEnemyPosition.IsValid)
			{
				return f.Direction;
			}
			Vec2 vec = (weightedAverageEnemyPosition - currentPosition).Normalized();
			float length = (weightedAverageEnemyPosition - currentPosition).Length;
			int enemyUnitCount = f.QuerySystem.Team.EnemyUnitCount;
			int countOfUnits = f.CountOfUnits;
			Vec2 vec2 = f.Direction;
			bool flag = length >= (float)countOfUnits * 0.2f;
			if (enemyUnitCount == 0 || countOfUnits == 0)
			{
				flag = false;
			}
			float num = ((!flag) ? 1f : (MBMath.ClampFloat((float)countOfUnits * 1f / (float)enemyUnitCount, 0.33333334f, 3f) * MBMath.ClampFloat(length / (float)countOfUnits, 0.33333334f, 3f)));
			if (flag && MathF.Abs(vec.AngleBetween(vec2)) > 0.17453292f * num)
			{
				vec2 = vec;
			}
			return vec2;
		}

		public OrderType OrderType
		{
			get
			{
				if (this.OrderEnum != FacingOrder.FacingOrderEnum.LookAtDirection)
				{
					return OrderType.LookAtEnemy;
				}
				return OrderType.LookAtDirection;
			}
		}

		public Vec2 GetDirection(Formation f, Agent targetAgent = null)
		{
			return this.GetDirectionAux(f, targetAgent);
		}

		public override bool Equals(object obj)
		{
			if (obj is FacingOrder)
			{
				FacingOrder facingOrder = (FacingOrder)obj;
				return facingOrder == this;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (int)this.OrderEnum;
		}

		public static bool operator !=(FacingOrder f1, FacingOrder f2)
		{
			return f1.OrderEnum != f2.OrderEnum;
		}

		public static bool operator ==(FacingOrder f1, FacingOrder f2)
		{
			return f1.OrderEnum == f2.OrderEnum;
		}

		public readonly FacingOrder.FacingOrderEnum OrderEnum;

		private readonly Vec2 _lookAtDirection;

		public static readonly FacingOrder FacingOrderLookAtEnemy = new FacingOrder(FacingOrder.FacingOrderEnum.LookAtEnemy);

		public enum FacingOrderEnum
		{
			LookAtDirection,
			LookAtEnemy
		}
	}
}
