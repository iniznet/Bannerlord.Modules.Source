using System;

namespace TaleWorlds.CampaignSystem
{
	public interface IMbEvent
	{
		void AddNonSerializedListener(object owner, Action action);

		void ClearListeners(object o);
	}
}
