using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000140 RID: 320
	public struct FacingOrder
	{
		// Token: 0x06001054 RID: 4180 RVA: 0x000346D1 File Offset: 0x000328D1
		public static FacingOrder FacingOrderLookAtDirection(Vec2 direction)
		{
			return new FacingOrder(FacingOrder.FacingOrderEnum.LookAtDirection, direction);
		}

		// Token: 0x06001055 RID: 4181 RVA: 0x000346DA File Offset: 0x000328DA
		private FacingOrder(FacingOrder.FacingOrderEnum orderEnum, Vec2 direction)
		{
			this.OrderEnum = orderEnum;
			this._lookAtDirection = direction;
		}

		// Token: 0x06001056 RID: 4182 RVA: 0x000346EA File Offset: 0x000328EA
		private FacingOrder(FacingOrder.FacingOrderEnum orderEnum)
		{
			this.OrderEnum = orderEnum;
			this._lookAtDirection = Vec2.Invalid;
		}

		// Token: 0x06001057 RID: 4183 RVA: 0x00034700 File Offset: 0x00032900
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

		// Token: 0x17000385 RID: 901
		// (get) Token: 0x06001058 RID: 4184 RVA: 0x00034846 File Offset: 0x00032A46
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

		// Token: 0x06001059 RID: 4185 RVA: 0x00034855 File Offset: 0x00032A55
		public Vec2 GetDirection(Formation f, Agent targetAgent = null)
		{
			return this.GetDirectionAux(f, targetAgent);
		}

		// Token: 0x0600105A RID: 4186 RVA: 0x00034860 File Offset: 0x00032A60
		public override bool Equals(object obj)
		{
			if (obj is FacingOrder)
			{
				FacingOrder facingOrder = (FacingOrder)obj;
				return facingOrder == this;
			}
			return false;
		}

		// Token: 0x0600105B RID: 4187 RVA: 0x0003488C File Offset: 0x00032A8C
		public override int GetHashCode()
		{
			return (int)this.OrderEnum;
		}

		// Token: 0x0600105C RID: 4188 RVA: 0x00034894 File Offset: 0x00032A94
		public static bool operator !=(FacingOrder f1, FacingOrder f2)
		{
			return f1.OrderEnum != f2.OrderEnum;
		}

		// Token: 0x0600105D RID: 4189 RVA: 0x000348A7 File Offset: 0x00032AA7
		public static bool operator ==(FacingOrder f1, FacingOrder f2)
		{
			return f1.OrderEnum == f2.OrderEnum;
		}

		// Token: 0x04000418 RID: 1048
		public readonly FacingOrder.FacingOrderEnum OrderEnum;

		// Token: 0x04000419 RID: 1049
		private readonly Vec2 _lookAtDirection;

		// Token: 0x0400041A RID: 1050
		public static readonly FacingOrder FacingOrderLookAtEnemy = new FacingOrder(FacingOrder.FacingOrderEnum.LookAtEnemy);

		// Token: 0x02000482 RID: 1154
		public enum FacingOrderEnum
		{
			// Token: 0x04001970 RID: 6512
			LookAtDirection,
			// Token: 0x04001971 RID: 6513
			LookAtEnemy
		}
	}
}
