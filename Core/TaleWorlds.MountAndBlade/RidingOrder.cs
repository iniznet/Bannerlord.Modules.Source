using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000146 RID: 326
	public struct RidingOrder
	{
		// Token: 0x060010BF RID: 4287 RVA: 0x00036ECE File Offset: 0x000350CE
		private RidingOrder(RidingOrder.RidingOrderEnum orderEnum)
		{
			this.OrderEnum = orderEnum;
		}

		// Token: 0x17000394 RID: 916
		// (get) Token: 0x060010C0 RID: 4288 RVA: 0x00036ED7 File Offset: 0x000350D7
		public OrderType OrderType
		{
			get
			{
				if (this.OrderEnum == RidingOrder.RidingOrderEnum.Free)
				{
					return OrderType.RideFree;
				}
				if (this.OrderEnum != RidingOrder.RidingOrderEnum.Mount)
				{
					return OrderType.Dismount;
				}
				return OrderType.Mount;
			}
		}

		// Token: 0x060010C1 RID: 4289 RVA: 0x00036EF4 File Offset: 0x000350F4
		public override bool Equals(object obj)
		{
			if (obj is RidingOrder)
			{
				RidingOrder ridingOrder = (RidingOrder)obj;
				return ridingOrder == this;
			}
			return false;
		}

		// Token: 0x060010C2 RID: 4290 RVA: 0x00036F20 File Offset: 0x00035120
		public override int GetHashCode()
		{
			return (int)this.OrderEnum;
		}

		// Token: 0x060010C3 RID: 4291 RVA: 0x00036F28 File Offset: 0x00035128
		public static bool operator !=(RidingOrder r1, RidingOrder r2)
		{
			return r1.OrderEnum != r2.OrderEnum;
		}

		// Token: 0x060010C4 RID: 4292 RVA: 0x00036F3B File Offset: 0x0003513B
		public static bool operator ==(RidingOrder r1, RidingOrder r2)
		{
			return r1.OrderEnum == r2.OrderEnum;
		}

		// Token: 0x0400043B RID: 1083
		public readonly RidingOrder.RidingOrderEnum OrderEnum;

		// Token: 0x0400043C RID: 1084
		public static readonly RidingOrder RidingOrderFree = new RidingOrder(RidingOrder.RidingOrderEnum.Free);

		// Token: 0x0400043D RID: 1085
		public static readonly RidingOrder RidingOrderMount = new RidingOrder(RidingOrder.RidingOrderEnum.Mount);

		// Token: 0x0400043E RID: 1086
		public static readonly RidingOrder RidingOrderDismount = new RidingOrder(RidingOrder.RidingOrderEnum.Dismount);

		// Token: 0x02000493 RID: 1171
		public enum RidingOrderEnum
		{
			// Token: 0x040019A7 RID: 6567
			Free,
			// Token: 0x040019A8 RID: 6568
			Mount,
			// Token: 0x040019A9 RID: 6569
			Dismount
		}
	}
}
