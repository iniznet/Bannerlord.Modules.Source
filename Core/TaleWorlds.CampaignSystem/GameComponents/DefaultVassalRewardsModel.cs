using System;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x0200014A RID: 330
	public class DefaultVassalRewardsModel : VassalRewardsModel
	{
		// Token: 0x1700065E RID: 1630
		// (get) Token: 0x06001806 RID: 6150 RVA: 0x00079885 File Offset: 0x00077A85
		public override int RelationRewardWithLeader
		{
			get
			{
				return 10;
			}
		}

		// Token: 0x1700065F RID: 1631
		// (get) Token: 0x06001807 RID: 6151 RVA: 0x00079889 File Offset: 0x00077A89
		public override float InfluenceReward
		{
			get
			{
				return 10f;
			}
		}

		// Token: 0x06001808 RID: 6152 RVA: 0x00079890 File Offset: 0x00077A90
		public override ItemRoster GetEquipmentRewardsForJoiningKingdom(Kingdom kingdom)
		{
			ItemRoster itemRoster = new ItemRoster();
			foreach (ItemObject itemObject in kingdom.Culture.VassalRewardItems)
			{
				itemRoster.AddToCounts(itemObject, 1);
			}
			ItemObject randomBannerAtLevel = this.GetRandomBannerAtLevel(2, kingdom.Culture);
			if (randomBannerAtLevel != null)
			{
				itemRoster.AddToCounts(randomBannerAtLevel, 1);
			}
			return itemRoster;
		}

		// Token: 0x06001809 RID: 6153 RVA: 0x0007990C File Offset: 0x00077B0C
		private ItemObject GetRandomBannerAtLevel(int bannerLevel, CultureObject culture = null)
		{
			MBList<ItemObject> mblist = Campaign.Current.Models.BannerItemModel.GetPossibleRewardBannerItems().ToMBList<ItemObject>();
			if (culture == null)
			{
				return mblist.GetRandomElementWithPredicate((ItemObject i) => (i.ItemComponent as BannerComponent).BannerLevel == bannerLevel);
			}
			return mblist.GetRandomElementWithPredicate((ItemObject i) => (i.ItemComponent as BannerComponent).BannerLevel == bannerLevel && i.Culture == culture);
		}

		// Token: 0x0600180A RID: 6154 RVA: 0x00079974 File Offset: 0x00077B74
		public override TroopRoster GetTroopRewardsForJoiningKingdom(Kingdom kingdom)
		{
			TroopRoster troopRoster = TroopRoster.CreateDummyTroopRoster();
			foreach (PartyTemplateStack partyTemplateStack in kingdom.Culture.VassalRewardTroopsPartyTemplate.Stacks)
			{
				troopRoster.AddToCounts(partyTemplateStack.Character, partyTemplateStack.MaxValue, false, 0, 0, true, -1);
			}
			return troopRoster;
		}

		// Token: 0x04000880 RID: 2176
		private const int VassalRewardBannerLevel = 2;
	}
}
