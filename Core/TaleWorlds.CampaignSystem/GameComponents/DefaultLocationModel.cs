using System;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultLocationModel : LocationModel
	{
		public override int GetSettlementUpgradeLevel(LocationEncounter locationEncounter)
		{
			return locationEncounter.Settlement.Town.GetWallLevel();
		}

		public override string GetCivilianSceneLevel(Settlement settlement)
		{
			string text = "civilian";
			if (settlement.IsFortification)
			{
				string upgradeLevelTag = this.GetUpgradeLevelTag(settlement.Town.GetWallLevel());
				if (!upgradeLevelTag.IsEmpty<char>())
				{
					text = text + " " + upgradeLevelTag;
				}
			}
			return text;
		}

		public override string GetCivilianUpgradeLevelTag(int upgradeLevel)
		{
			if (upgradeLevel == 0)
			{
				return "";
			}
			string text = "civilian";
			string upgradeLevelTag = this.GetUpgradeLevelTag(upgradeLevel);
			if (!upgradeLevelTag.IsEmpty<char>())
			{
				text = text + " " + upgradeLevelTag;
			}
			return text;
		}

		public override string GetUpgradeLevelTag(int upgradeLevel)
		{
			switch (upgradeLevel)
			{
			case 1:
				return "level_1";
			case 2:
				return "level_2";
			case 3:
				return "level_3";
			default:
				return "";
			}
		}
	}
}
