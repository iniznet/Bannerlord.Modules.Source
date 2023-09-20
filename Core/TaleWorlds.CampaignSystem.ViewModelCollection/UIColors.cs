using System;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection
{
	public static class UIColors
	{
		public static Color PositiveIndicator
		{
			get
			{
				return UIColors._positiveIndicator;
			}
		}

		public static Color NegativeIndicator
		{
			get
			{
				return UIColors._negativeIndicator;
			}
		}

		public static Color Gold
		{
			get
			{
				return UIColors._gold;
			}
		}

		private static Color _positiveIndicator = Color.FromUint(4285250886U);

		private static Color _negativeIndicator = Color.FromUint(4290070086U);

		private static Color _gold = Color.FromUint(4294957447U);
	}
}
