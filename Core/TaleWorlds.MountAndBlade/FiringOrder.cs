using System;

namespace TaleWorlds.MountAndBlade
{
	public struct FiringOrder
	{
		private FiringOrder(FiringOrder.RangedWeaponUsageOrderEnum orderEnum)
		{
			this.OrderEnum = orderEnum;
		}

		public OrderType OrderType
		{
			get
			{
				if (this.OrderEnum != FiringOrder.RangedWeaponUsageOrderEnum.FireAtWill)
				{
					return OrderType.HoldFire;
				}
				return OrderType.FireAtWill;
			}
		}

		public override bool Equals(object obj)
		{
			if (obj is FiringOrder)
			{
				FiringOrder firingOrder = (FiringOrder)obj;
				return firingOrder == this;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (int)this.OrderEnum;
		}

		public static bool operator !=(FiringOrder f1, FiringOrder f2)
		{
			return f1.OrderEnum != f2.OrderEnum;
		}

		public static bool operator ==(FiringOrder f1, FiringOrder f2)
		{
			return f1.OrderEnum == f2.OrderEnum;
		}

		public readonly FiringOrder.RangedWeaponUsageOrderEnum OrderEnum;

		public static readonly FiringOrder FiringOrderFireAtWill = new FiringOrder(FiringOrder.RangedWeaponUsageOrderEnum.FireAtWill);

		public static readonly FiringOrder FiringOrderHoldYourFire = new FiringOrder(FiringOrder.RangedWeaponUsageOrderEnum.HoldYourFire);

		public enum RangedWeaponUsageOrderEnum
		{
			FireAtWill,
			HoldYourFire
		}
	}
}
