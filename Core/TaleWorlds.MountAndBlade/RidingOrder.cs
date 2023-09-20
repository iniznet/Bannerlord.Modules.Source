using System;

namespace TaleWorlds.MountAndBlade
{
	public struct RidingOrder
	{
		private RidingOrder(RidingOrder.RidingOrderEnum orderEnum)
		{
			this.OrderEnum = orderEnum;
		}

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

		public override bool Equals(object obj)
		{
			if (obj is RidingOrder)
			{
				RidingOrder ridingOrder = (RidingOrder)obj;
				return ridingOrder == this;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (int)this.OrderEnum;
		}

		public static bool operator !=(RidingOrder r1, RidingOrder r2)
		{
			return r1.OrderEnum != r2.OrderEnum;
		}

		public static bool operator ==(RidingOrder r1, RidingOrder r2)
		{
			return r1.OrderEnum == r2.OrderEnum;
		}

		public readonly RidingOrder.RidingOrderEnum OrderEnum;

		public static readonly RidingOrder RidingOrderFree = new RidingOrder(RidingOrder.RidingOrderEnum.Free);

		public static readonly RidingOrder RidingOrderMount = new RidingOrder(RidingOrder.RidingOrderEnum.Mount);

		public static readonly RidingOrder RidingOrderDismount = new RidingOrder(RidingOrder.RidingOrderEnum.Dismount);

		public enum RidingOrderEnum
		{
			Free,
			Mount,
			Dismount
		}
	}
}
