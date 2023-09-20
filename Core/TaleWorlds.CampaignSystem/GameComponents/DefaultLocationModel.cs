using System;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x02000116 RID: 278
	public class DefaultLocationModel : LocationModel
	{
		// Token: 0x060015E1 RID: 5601 RVA: 0x00067787 File Offset: 0x00065987
		public override int GetSettlementUpgradeLevel(LocationEncounter locationEncounter)
		{
			return locationEncounter.Settlement.Town.GetWallLevel();
		}

		// Token: 0x060015E2 RID: 5602 RVA: 0x0006779C File Offset: 0x0006599C
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

		// Token: 0x060015E3 RID: 5603 RVA: 0x000677E0 File Offset: 0x000659E0
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

		// Token: 0x060015E4 RID: 5604 RVA: 0x0006781A File Offset: 0x00065A1A
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
