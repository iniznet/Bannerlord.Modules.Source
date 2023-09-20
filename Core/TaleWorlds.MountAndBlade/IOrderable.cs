using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000379 RID: 889
	public interface IOrderable
	{
		// Token: 0x0600305E RID: 12382
		OrderType GetOrder(BattleSideEnum side);
	}
}
