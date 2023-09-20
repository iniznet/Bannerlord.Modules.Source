using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.Extensions
{
	public static class Attributes
	{
		public static MBReadOnlyList<CharacterAttribute> All
		{
			get
			{
				return Campaign.Current.AllCharacterAttributes;
			}
		}
	}
}
