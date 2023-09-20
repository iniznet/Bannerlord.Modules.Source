using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace StoryMode
{
	// Token: 0x02000011 RID: 17
	public static class StoryModeData
	{
		// Token: 0x06000074 RID: 116 RVA: 0x000044F4 File Offset: 0x000026F4
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

		// Token: 0x06000075 RID: 117 RVA: 0x00004526 File Offset: 0x00002726
		public static bool IsKingdomImperial(Kingdom kingdomToCheck)
		{
			return kingdomToCheck != null && kingdomToCheck.Culture == StoryModeData.ImperialCulture;
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000076 RID: 118 RVA: 0x0000453A File Offset: 0x0000273A
		public static CultureObject ImperialCulture
		{
			get
			{
				return StoryModeData.NorthernEmpireKingdom.Culture;
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000077 RID: 119 RVA: 0x00004548 File Offset: 0x00002748
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

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000078 RID: 120 RVA: 0x000045D8 File Offset: 0x000027D8
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

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000079 RID: 121 RVA: 0x00004668 File Offset: 0x00002868
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

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600007A RID: 122 RVA: 0x000046F8 File Offset: 0x000028F8
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

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x0600007B RID: 123 RVA: 0x00004788 File Offset: 0x00002988
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

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x0600007C RID: 124 RVA: 0x00004818 File Offset: 0x00002A18
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

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x0600007D RID: 125 RVA: 0x000048A8 File Offset: 0x00002AA8
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

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x0600007E RID: 126 RVA: 0x00004938 File Offset: 0x00002B38
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

		// Token: 0x04000021 RID: 33
		private static Kingdom _northernEmpireKingdom;

		// Token: 0x04000022 RID: 34
		private static Kingdom _westernEmpireKingdom;

		// Token: 0x04000023 RID: 35
		private static Kingdom _southernEmpireKingdom;

		// Token: 0x04000024 RID: 36
		private static Kingdom _sturgiaKingdom;

		// Token: 0x04000025 RID: 37
		private static Kingdom _aseraiKingdom;

		// Token: 0x04000026 RID: 38
		private static Kingdom _vlandiaKingdom;

		// Token: 0x04000027 RID: 39
		private static Kingdom _battaniaKingdom;

		// Token: 0x04000028 RID: 40
		private static Kingdom _khuzaitKingdom;
	}
}
