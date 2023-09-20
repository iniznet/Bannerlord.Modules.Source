using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.Objects;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000251 RID: 593
	public interface ICommanderInfo : IMissionBehavior
	{
		// Token: 0x14000028 RID: 40
		// (add) Token: 0x06001FF9 RID: 8185
		// (remove) Token: 0x06001FFA RID: 8186
		event Action<BattleSideEnum, float> OnMoraleChangedEvent;

		// Token: 0x14000029 RID: 41
		// (add) Token: 0x06001FFB RID: 8187
		// (remove) Token: 0x06001FFC RID: 8188
		event Action OnFlagNumberChangedEvent;

		// Token: 0x1400002A RID: 42
		// (add) Token: 0x06001FFD RID: 8189
		// (remove) Token: 0x06001FFE RID: 8190
		event Action<FlagCapturePoint, Team> OnCapturePointOwnerChangedEvent;

		// Token: 0x17000640 RID: 1600
		// (get) Token: 0x06001FFF RID: 8191
		IEnumerable<FlagCapturePoint> AllCapturePoints { get; }

		// Token: 0x06002000 RID: 8192
		Team GetFlagOwner(FlagCapturePoint flag);

		// Token: 0x17000641 RID: 1601
		// (get) Token: 0x06002001 RID: 8193
		bool AreMoralesIndependent { get; }
	}
}
