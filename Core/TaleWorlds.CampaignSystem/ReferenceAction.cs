using System;

namespace TaleWorlds.CampaignSystem
{
	public delegate void ReferenceAction<T1, T2>(T1 t1, ref T2 t2);
}
