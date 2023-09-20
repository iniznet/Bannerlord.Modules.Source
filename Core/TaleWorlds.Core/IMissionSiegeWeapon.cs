using System;

namespace TaleWorlds.Core
{
	// Token: 0x0200008B RID: 139
	public interface IMissionSiegeWeapon
	{
		// Token: 0x17000294 RID: 660
		// (get) Token: 0x060007C4 RID: 1988
		int Index { get; }

		// Token: 0x17000295 RID: 661
		// (get) Token: 0x060007C5 RID: 1989
		SiegeEngineType Type { get; }

		// Token: 0x17000296 RID: 662
		// (get) Token: 0x060007C6 RID: 1990
		float Health { get; }

		// Token: 0x17000297 RID: 663
		// (get) Token: 0x060007C7 RID: 1991
		float InitialHealth { get; }

		// Token: 0x17000298 RID: 664
		// (get) Token: 0x060007C8 RID: 1992
		float MaxHealth { get; }
	}
}
