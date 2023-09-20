using System;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x02000049 RID: 73
	public interface IMbEvent<out T1, out T2, out T3, out T4, out T5, out T6> : IMbEventBase
	{
		// Token: 0x06000755 RID: 1877
		void AddNonSerializedListener(object owner, Action<T1, T2, T3, T4, T5, T6> action);
	}
}
