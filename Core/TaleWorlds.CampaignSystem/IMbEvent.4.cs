using System;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x02000043 RID: 67
	public interface IMbEvent<out T1, out T2, out T3> : IMbEventBase
	{
		// Token: 0x06000740 RID: 1856
		void AddNonSerializedListener(object owner, Action<T1, T2, T3> action);
	}
}
