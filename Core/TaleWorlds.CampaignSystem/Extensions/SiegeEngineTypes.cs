using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.Extensions
{
	public static class SiegeEngineTypes
	{
		public static MBReadOnlyList<SiegeEngineType> All
		{
			get
			{
				return Campaign.Current.AllSiegeEngineTypes;
			}
		}
	}
}
