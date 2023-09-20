using System;

namespace TaleWorlds.CampaignSystem
{
	public interface IMbEvent<out T1, out T2, out T3, out T4> : IMbEventBase
	{
		void AddNonSerializedListener(object owner, Action<T1, T2, T3, T4> action);
	}
}
