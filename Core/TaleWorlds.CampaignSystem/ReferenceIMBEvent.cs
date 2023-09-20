using System;

namespace TaleWorlds.CampaignSystem
{
	public interface ReferenceIMBEvent<T1, T2> : IMbEventBase
	{
		void AddNonSerializedListener(object owner, ReferenceAction<T1, T2> action);
	}
}
