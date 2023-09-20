using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001BE RID: 446
	public interface IMusicHandler
	{
		// Token: 0x17000520 RID: 1312
		// (get) Token: 0x060019AE RID: 6574
		bool IsPausable { get; }

		// Token: 0x060019AF RID: 6575
		void OnUpdated(float dt);
	}
}
