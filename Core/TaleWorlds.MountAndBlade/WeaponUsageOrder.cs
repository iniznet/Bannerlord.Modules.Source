using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000147 RID: 327
	public struct WeaponUsageOrder
	{
		// Token: 0x060010C6 RID: 4294 RVA: 0x00036F6E File Offset: 0x0003516E
		private WeaponUsageOrder(WeaponUsageOrder.WeaponUsageOrderEnum orderEnum)
		{
			this.OrderEnum = orderEnum;
		}

		// Token: 0x17000395 RID: 917
		// (get) Token: 0x060010C7 RID: 4295 RVA: 0x00036F77 File Offset: 0x00035177
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

		// Token: 0x060010C8 RID: 4296 RVA: 0x00036F88 File Offset: 0x00035188
		public override bool Equals(object obj)
		{
			if (obj is WeaponUsageOrder)
			{
				WeaponUsageOrder weaponUsageOrder = (WeaponUsageOrder)obj;
				return weaponUsageOrder == this;
			}
			return false;
		}

		// Token: 0x060010C9 RID: 4297 RVA: 0x00036FB4 File Offset: 0x000351B4
		public override int GetHashCode()
		{
			return (int)this.OrderEnum;
		}

		// Token: 0x060010CA RID: 4298 RVA: 0x00036FBC File Offset: 0x000351BC
		public static bool operator !=(WeaponUsageOrder wuo1, WeaponUsageOrder wuo2)
		{
			return wuo1.OrderEnum != wuo2.OrderEnum;
		}

		// Token: 0x060010CB RID: 4299 RVA: 0x00036FCF File Offset: 0x000351CF
		public static bool operator ==(WeaponUsageOrder wuo1, WeaponUsageOrder wuo2)
		{
			return wuo1.OrderEnum == wuo2.OrderEnum;
		}

		// Token: 0x0400043F RID: 1087
		public readonly WeaponUsageOrder.WeaponUsageOrderEnum OrderEnum;

		// Token: 0x04000440 RID: 1088
		public static readonly WeaponUsageOrder WeaponUsageOrderUseAny = new WeaponUsageOrder(WeaponUsageOrder.WeaponUsageOrderEnum.UseAnyWeapon);

		// Token: 0x04000441 RID: 1089
		public static readonly WeaponUsageOrder WeaponUsageOrderUseOnlyBlunt = new WeaponUsageOrder(WeaponUsageOrder.WeaponUsageOrderEnum.UseBluntWeaponsOnly);

		// Token: 0x02000494 RID: 1172
		public enum WeaponUsageOrderEnum
		{
			// Token: 0x040019AB RID: 6571
			UseAnyWeapon,
			// Token: 0x040019AC RID: 6572
			UseBluntWeaponsOnly
		}
	}
}
