using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000141 RID: 321
	public struct FiringOrder
	{
		// Token: 0x0600105F RID: 4191 RVA: 0x000348C4 File Offset: 0x00032AC4
		private FiringOrder(FiringOrder.RangedWeaponUsageOrderEnum orderEnum)
		{
			this.OrderEnum = orderEnum;
		}

		// Token: 0x17000386 RID: 902
		// (get) Token: 0x06001060 RID: 4192 RVA: 0x000348CD File Offset: 0x00032ACD
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

		// Token: 0x06001061 RID: 4193 RVA: 0x000348DC File Offset: 0x00032ADC
		public override bool Equals(object obj)
		{
			if (obj is FiringOrder)
			{
				FiringOrder firingOrder = (FiringOrder)obj;
				return firingOrder == this;
			}
			return false;
		}

		// Token: 0x06001062 RID: 4194 RVA: 0x00034908 File Offset: 0x00032B08
		public override int GetHashCode()
		{
			return (int)this.OrderEnum;
		}

		// Token: 0x06001063 RID: 4195 RVA: 0x00034910 File Offset: 0x00032B10
		public static bool operator !=(FiringOrder f1, FiringOrder f2)
		{
			return f1.OrderEnum != f2.OrderEnum;
		}

		// Token: 0x06001064 RID: 4196 RVA: 0x00034923 File Offset: 0x00032B23
		public static bool operator ==(FiringOrder f1, FiringOrder f2)
		{
			return f1.OrderEnum == f2.OrderEnum;
		}

		// Token: 0x0400041B RID: 1051
		public readonly FiringOrder.RangedWeaponUsageOrderEnum OrderEnum;

		// Token: 0x0400041C RID: 1052
		public static readonly FiringOrder FiringOrderFireAtWill = new FiringOrder(FiringOrder.RangedWeaponUsageOrderEnum.FireAtWill);

		// Token: 0x0400041D RID: 1053
		public static readonly FiringOrder FiringOrderHoldYourFire = new FiringOrder(FiringOrder.RangedWeaponUsageOrderEnum.HoldYourFire);

		// Token: 0x02000483 RID: 1155
		public enum RangedWeaponUsageOrderEnum
		{
			// Token: 0x04001973 RID: 6515
			FireAtWill,
			// Token: 0x04001974 RID: 6516
			HoldYourFire
		}
	}
}
