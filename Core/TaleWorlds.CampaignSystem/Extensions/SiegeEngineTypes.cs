using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.Extensions
{
	// Token: 0x02000155 RID: 341
	public static class SiegeEngineTypes
	{
		// Token: 0x17000668 RID: 1640
		// (get) Token: 0x06001839 RID: 6201 RVA: 0x0007B01F File Offset: 0x0007921F
		public static MBReadOnlyList<SiegeEngineType> All
		{
			get
			{
				return Campaign.Current.AllSiegeEngineTypes;
			}
		}
	}
}
