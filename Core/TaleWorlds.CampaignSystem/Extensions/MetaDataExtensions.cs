using System;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.Extensions
{
	// Token: 0x02000159 RID: 345
	public static class MetaDataExtensions
	{
		// Token: 0x06001840 RID: 6208 RVA: 0x0007B0D0 File Offset: 0x000792D0
		public static string GetUniqueGameId(this MetaData metaData)
		{
			string text;
			if (metaData == null || !metaData.TryGetValue("UniqueGameId", out text))
			{
				return "";
			}
			return text;
		}

		// Token: 0x06001841 RID: 6209 RVA: 0x0007B0F8 File Offset: 0x000792F8
		public static int GetMainHeroLevel(this MetaData metaData)
		{
			string text;
			if (metaData == null || !metaData.TryGetValue("MainHeroLevel", out text))
			{
				return 0;
			}
			return int.Parse(text);
		}

		// Token: 0x06001842 RID: 6210 RVA: 0x0007B120 File Offset: 0x00079320
		public static float GetMainPartyFood(this MetaData metaData)
		{
			string text;
			if (metaData == null || !metaData.TryGetValue("MainPartyFood", out text))
			{
				return 0f;
			}
			return float.Parse(text);
		}

		// Token: 0x06001843 RID: 6211 RVA: 0x0007B14C File Offset: 0x0007934C
		public static int GetMainHeroGold(this MetaData metaData)
		{
			string text;
			if (metaData == null || !metaData.TryGetValue("MainHeroGold", out text))
			{
				return 0;
			}
			return int.Parse(text);
		}

		// Token: 0x06001844 RID: 6212 RVA: 0x0007B174 File Offset: 0x00079374
		public static float GetClanInfluence(this MetaData metaData)
		{
			string text;
			if (metaData == null || !metaData.TryGetValue("ClanInfluence", out text))
			{
				return 0f;
			}
			return float.Parse(text);
		}

		// Token: 0x06001845 RID: 6213 RVA: 0x0007B1A0 File Offset: 0x000793A0
		public static int GetClanFiefs(this MetaData metaData)
		{
			string text;
			if (metaData == null || !metaData.TryGetValue("ClanFiefs", out text))
			{
				return 0;
			}
			return int.Parse(text);
		}

		// Token: 0x06001846 RID: 6214 RVA: 0x0007B1C8 File Offset: 0x000793C8
		public static int GetMainPartyHealthyMemberCount(this MetaData metaData)
		{
			string text;
			if (metaData == null || !metaData.TryGetValue("MainPartyHealthyMemberCount", out text))
			{
				return 0;
			}
			return int.Parse(text);
		}

		// Token: 0x06001847 RID: 6215 RVA: 0x0007B1F0 File Offset: 0x000793F0
		public static int GetMainPartyPrisonerMemberCount(this MetaData metaData)
		{
			string text;
			if (metaData == null || !metaData.TryGetValue("MainPartyPrisonerMemberCount", out text))
			{
				return 0;
			}
			return int.Parse(text);
		}

		// Token: 0x06001848 RID: 6216 RVA: 0x0007B218 File Offset: 0x00079418
		public static int GetMainPartyWoundedMemberCount(this MetaData metaData)
		{
			string text;
			if (metaData == null || !metaData.TryGetValue("MainPartyWoundedMemberCount", out text))
			{
				return 0;
			}
			return int.Parse(text);
		}

		// Token: 0x06001849 RID: 6217 RVA: 0x0007B240 File Offset: 0x00079440
		public static string GetClanBannerCode(this MetaData metaData)
		{
			string text;
			if (metaData == null || !metaData.TryGetValue("ClanBannerCode", out text))
			{
				return "";
			}
			return text;
		}

		// Token: 0x0600184A RID: 6218 RVA: 0x0007B268 File Offset: 0x00079468
		public static string GetCharacterName(this MetaData metaData)
		{
			string text;
			if (metaData == null || !metaData.TryGetValue("CharacterName", out text))
			{
				return "";
			}
			return text;
		}

		// Token: 0x0600184B RID: 6219 RVA: 0x0007B290 File Offset: 0x00079490
		public static string GetCharacterVisualCode(this MetaData metaData)
		{
			string text;
			if (metaData == null || !metaData.TryGetValue("MainHeroVisual", out text))
			{
				return "";
			}
			return text;
		}

		// Token: 0x0600184C RID: 6220 RVA: 0x0007B2B8 File Offset: 0x000794B8
		public static double GetDayLong(this MetaData metaData)
		{
			string text;
			if (metaData == null || !metaData.TryGetValue("DayLong", out text))
			{
				return 0.0;
			}
			return double.Parse(text);
		}

		// Token: 0x0600184D RID: 6221 RVA: 0x0007B2E8 File Offset: 0x000794E8
		public static bool GetIronmanMode(this MetaData metaData)
		{
			string text;
			int num;
			return metaData != null && metaData.TryGetValue("IronmanMode", out text) && int.TryParse(text, out num) && num == 1;
		}

		// Token: 0x0600184E RID: 6222 RVA: 0x0007B318 File Offset: 0x00079518
		public static int GetPlayerHealthPercentage(this MetaData metaData)
		{
			string text;
			int num;
			if (metaData == null || !metaData.TryGetValue("HealthPercentage", out text) || !int.TryParse(text, out num))
			{
				return 100;
			}
			return num;
		}
	}
}
