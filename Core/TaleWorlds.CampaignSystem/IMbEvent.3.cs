using System;

namespace TaleWorlds.CampaignSystem
{
	public interface IMbEvent<out T1, out T2> : IMbEventBase
	{
		void AddNonSerializedListener(object owner, Action<T1, T2> action);
	}
}
