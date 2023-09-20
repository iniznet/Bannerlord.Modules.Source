using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace StoryMode
{
	public static class StoryModeData
	{
		public static void OnGameEnd()
		{
			StoryModeData._northernEmpireKingdom = null;
			StoryModeData._westernEmpireKingdom = null;
			StoryModeData._southernEmpireKingdom = null;
			StoryModeData._sturgiaKingdom = null;
			StoryModeData._aseraiKingdom = null;
			StoryModeData._vlandiaKingdom = null;
			StoryModeData._battaniaKingdom = null;
			StoryModeData._khuzaitKingdom = null;
		}

		public static bool IsKingdomImperial(Kingdom kingdomToCheck)
		{
			return kingdomToCheck != null && kingdomToCheck.Culture == StoryModeData.ImperialCulture;
		}

		public static CultureObject ImperialCulture
		{
			get
			{
				return StoryModeData.NorthernEmpireKingdom.Culture;
			}
		}

		public static Kingdom NorthernEmpireKingdom
		{
			get
			{
				if (StoryModeData._northernEmpireKingdom != null)
				{
					return StoryModeData._northernEmpireKingdom;
				}
				foreach (Kingdom kingdom in Kingdom.All)
				{
					if (kingdom.StringId == "empire")
					{
						StoryModeData._northernEmpireKingdom = kingdom;
						return kingdom;
					}
				}
				Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\StoryMode\\StoryModeData.cs", "NorthernEmpireKingdom", 49);
				return null;
			}
		}

		public static Kingdom WesternEmpireKingdom
		{
			get
			{
				if (StoryModeData._westernEmpireKingdom != null)
				{
					return StoryModeData._westernEmpireKingdom;
				}
				foreach (Kingdom kingdom in Kingdom.All)
				{
					if (kingdom.StringId == "empire_w")
					{
						StoryModeData._westernEmpireKingdom = kingdom;
						return kingdom;
					}
				}
				Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\StoryMode\\StoryModeData.cs", "WesternEmpireKingdom", 74);
				return null;
			}
		}

		public static Kingdom SouthernEmpireKingdom
		{
			get
			{
				if (StoryModeData._southernEmpireKingdom != null)
				{
					return StoryModeData._southernEmpireKingdom;
				}
				foreach (Kingdom kingdom in Kingdom.All)
				{
					if (kingdom.StringId == "empire_s")
					{
						StoryModeData._southernEmpireKingdom = kingdom;
						return kingdom;
					}
				}
				Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\StoryMode\\StoryModeData.cs", "SouthernEmpireKingdom", 99);
				return null;
			}
		}

		public static Kingdom SturgiaKingdom
		{
			get
			{
				if (StoryModeData._sturgiaKingdom != null)
				{
					return StoryModeData._sturgiaKingdom;
				}
				foreach (Kingdom kingdom in Kingdom.All)
				{
					if (kingdom.StringId == "sturgia")
					{
						StoryModeData._sturgiaKingdom = kingdom;
						return kingdom;
					}
				}
				Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\StoryMode\\StoryModeData.cs", "SturgiaKingdom", 124);
				return null;
			}
		}

		public static Kingdom AseraiKingdom
		{
			get
			{
				if (StoryModeData._aseraiKingdom != null)
				{
					return StoryModeData._aseraiKingdom;
				}
				foreach (Kingdom kingdom in Kingdom.All)
				{
					if (kingdom.StringId == "aserai")
					{
						StoryModeData._aseraiKingdom = kingdom;
						return kingdom;
					}
				}
				Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\StoryMode\\StoryModeData.cs", "AseraiKingdom", 149);
				return null;
			}
		}

		public static Kingdom VlandiaKingdom
		{
			get
			{
				if (StoryModeData._vlandiaKingdom != null)
				{
					return StoryModeData._vlandiaKingdom;
				}
				foreach (Kingdom kingdom in Kingdom.All)
				{
					if (kingdom.StringId == "vlandia")
					{
						StoryModeData._vlandiaKingdom = kingdom;
						return kingdom;
					}
				}
				Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\StoryMode\\StoryModeData.cs", "VlandiaKingdom", 175);
				return null;
			}
		}

		public static Kingdom BattaniaKingdom
		{
			get
			{
				if (StoryModeData._battaniaKingdom != null)
				{
					return StoryModeData._battaniaKingdom;
				}
				foreach (Kingdom kingdom in Kingdom.All)
				{
					if (kingdom.StringId == "battania")
					{
						StoryModeData._battaniaKingdom = kingdom;
						return kingdom;
					}
				}
				Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\StoryMode\\StoryModeData.cs", "BattaniaKingdom", 202);
				return null;
			}
		}

		public static Kingdom KhuzaitKingdom
		{
			get
			{
				if (StoryModeData._khuzaitKingdom != null)
				{
					return StoryModeData._khuzaitKingdom;
				}
				foreach (Kingdom kingdom in Kingdom.All)
				{
					if (kingdom.StringId == "khuzait")
					{
						StoryModeData._khuzaitKingdom = kingdom;
						return kingdom;
					}
				}
				Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\StoryMode\\StoryModeData.cs", "KhuzaitKingdom", 228);
				return null;
			}
		}

		private static Kingdom _northernEmpireKingdom;

		private static Kingdom _westernEmpireKingdom;

		private static Kingdom _southernEmpireKingdom;

		private static Kingdom _sturgiaKingdom;

		private static Kingdom _aseraiKingdom;

		private static Kingdom _vlandiaKingdom;

		private static Kingdom _battaniaKingdom;

		private static Kingdom _khuzaitKingdom;
	}
}
