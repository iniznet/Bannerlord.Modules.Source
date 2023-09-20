using System;

namespace TaleWorlds.MountAndBlade
{
	public struct WeaponUsageOrder
	{
		private WeaponUsageOrder(WeaponUsageOrder.WeaponUsageOrderEnum orderEnum)
		{
			this.OrderEnum = orderEnum;
		}

		public OrderType OrderType
		{
			get
			{
				if (this.OrderEnum != WeaponUsageOrder.WeaponUsageOrderEnum.UseAnyWeapon)
				{
					return OrderType.UseBluntWeaponsOnly;
				}
				return OrderType.UseAnyWeapon;
			}
		}

		public override bool Equals(object obj)
		{
			if (obj is WeaponUsageOrder)
			{
				WeaponUsageOrder weaponUsageOrder = (WeaponUsageOrder)obj;
				return weaponUsageOrder == this;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (int)this.OrderEnum;
		}

		public static bool operator !=(WeaponUsageOrder wuo1, WeaponUsageOrder wuo2)
		{
			return wuo1.OrderEnum != wuo2.OrderEnum;
		}

		public static bool operator ==(WeaponUsageOrder wuo1, WeaponUsageOrder wuo2)
		{
			return wuo1.OrderEnum == wuo2.OrderEnum;
		}

		public readonly WeaponUsageOrder.WeaponUsageOrderEnum OrderEnum;

		public static readonly WeaponUsageOrder WeaponUsageOrderUseAny = new WeaponUsageOrder(WeaponUsageOrder.WeaponUsageOrderEnum.UseAnyWeapon);

		public static readonly WeaponUsageOrder WeaponUsageOrderUseOnlyBlunt = new WeaponUsageOrder(WeaponUsageOrder.WeaponUsageOrderEnum.UseBluntWeaponsOnly);

		public enum WeaponUsageOrderEnum
		{
			UseAnyWeapon,
			UseBluntWeaponsOnly
		}
	}
}
