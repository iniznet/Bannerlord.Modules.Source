﻿using System;

namespace TaleWorlds.CampaignSystem
{
	public delegate void ReferenceAction<T1, T2, T3>(T1 t1, T2 t2, ref T3 t3);
}
