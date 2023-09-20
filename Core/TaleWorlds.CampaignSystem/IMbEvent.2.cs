using System;

namespace TaleWorlds.CampaignSystem
{
	public interface IMbEvent<out T> : IMbEventBase
	{
		void AddNonSerializedListener(object owner, Action<T> action);
	}
}
