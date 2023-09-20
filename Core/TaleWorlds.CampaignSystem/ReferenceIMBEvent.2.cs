﻿using System;

namespace TaleWorlds.CampaignSystem
{
	public interface ReferenceIMBEvent<T1, T2, T3> : IMbEventBase
	{
		void AddNonSerializedListener(object owner, ReferenceAction<T1, T2, T3> action);
	}
}
