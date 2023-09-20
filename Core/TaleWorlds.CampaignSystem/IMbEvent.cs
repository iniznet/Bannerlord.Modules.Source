using System;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x02000031 RID: 49
	public interface IMbEvent
	{
		// Token: 0x06000350 RID: 848
		void AddNonSerializedListener(object owner, Action action);

		// Token: 0x06000351 RID: 849
		void ClearListeners(object o);
	}
}
