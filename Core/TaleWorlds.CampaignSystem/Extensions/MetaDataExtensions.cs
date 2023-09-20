using System;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.Extensions
{
	public static class MetaDataExtensions
	{
		public static string GetUniqueGameId(this MetaData metaData)
		{
			string text;
			if (metaData == null || !metaData.TryGetValue("UniqueGameId", out text))
			{
				return "";
			}
			return text;
		}

		public static int GetMainHeroLevel(this MetaData metaData)
		{
			string text;
			if (metaData == null || !metaData.TryGetValue("MainHeroLevel", out text))
			{
				return 0;
			}
			return int.Parse(text);
		}

		public static float GetMainPartyFood(this MetaData metaData)
		{
			string text;
			if (metaData == null || !metaData.TryGetValue("MainPartyFood", out text))
			{
				return 0f;
			}
			return float.Parse(text);
		}

		public static int GetMainHeroGold(this MetaData metaData)
		{
			string text;
			if (metaData == null || !metaData.TryGetValue("MainHeroGold", out text))
			{
				return 0;
			}
			return int.Parse(text);
		}

		public static float GetClanInfluence(this MetaData metaData)
		{
			string text;
			if (metaData == null || !metaData.TryGetValue("ClanInfluence", out text))
			{
				return 0f;
			}
			return float.Parse(text);
		}

		public static int GetClanFiefs(this MetaData metaData)
		{
			string text;
			if (metaData == null || !metaData.TryGetValue("ClanFiefs", out text))
			{
				return 0;
			}
			return int.Parse(text);
		}

		public static int GetMainPartyHealthyMemberCount(this MetaData metaData)
		{
			string text;
			if (metaData == null || !metaData.TryGetValue("MainPartyHealthyMemberCount", out text))
			{
				return 0;
			}
			return int.Parse(text);
		}

		public static int GetMainPartyPrisonerMemberCount(this MetaData metaData)
		{
			string text;
			if (metaData == null || !metaData.TryGetValue("MainPartyPrisonerMemberCount", out text))
			{
				return 0;
			}
			return int.Parse(text);
		}

		public static int GetMainPartyWoundedMemberCount(this MetaData metaData)
		{
			string text;
			if (metaData == null || !metaData.TryGetValue("MainPartyWoundedMemberCount", out text))
			{
				return 0;
			}
			return int.Parse(text);
		}

		public static string GetClanBannerCode(this MetaData metaData)
		{
			string text;
			if (metaData == null || !metaData.TryGetValue("ClanBannerCode", out text))
			{
				return "";
			}
			return text;
		}

		public static string GetCharacterName(this MetaData metaData)
		{
			string text;
			if (metaData == null || !metaData.TryGetValue("CharacterName", out text))
			{
				return "";
			}
			return text;
		}

		public static string GetCharacterVisualCode(this MetaData metaData)
		{
			string text;
			if (metaData == null || !metaData.TryGetValue("MainHeroVisual", out text))
			{
				return "";
			}
			return text;
		}

		public static double GetDayLong(this MetaData metaData)
		{
			string text;
			if (metaData == null || !metaData.TryGetValue("DayLong", out text))
			{
				return 0.0;
			}
			return double.Parse(text);
		}

		public static bool GetIronmanMode(this MetaData metaData)
		{
			string text;
			int num;
			return metaData != null && metaData.TryGetValue("IronmanMode", out text) && int.TryParse(text, out num) && num == 1;
		}

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
