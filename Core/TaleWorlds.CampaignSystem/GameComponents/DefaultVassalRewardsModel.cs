using System;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultVassalRewardsModel : VassalRewardsModel
	{
		public override int RelationRewardWithLeader
		{
			get
			{
				return 10;
			}
		}

		public override float InfluenceReward
		{
			get
			{
				return 10f;
			}
		}

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

		private ItemObject GetRandomBannerAtLevel(int bannerLevel, CultureObject culture = null)
		{
			MBList<ItemObject> mblist = Campaign.Current.Models.BannerItemModel.GetPossibleRewardBannerItems().ToMBList<ItemObject>();
			if (culture == null)
			{
				return mblist.GetRandomElementWithPredicate((ItemObject i) => (i.ItemComponent as BannerComponent).BannerLevel == bannerLevel);
			}
			return mblist.GetRandomElementWithPredicate((ItemObject i) => (i.ItemComponent as BannerComponent).BannerLevel == bannerLevel && i.Culture == culture);
		}

		public override TroopRoster GetTroopRewardsForJoiningKingdom(Kingdom kingdom)
		{
			TroopRoster troopRoster = TroopRoster.CreateDummyTroopRoster();
			foreach (PartyTemplateStack partyTemplateStack in kingdom.Culture.VassalRewardTroopsPartyTemplate.Stacks)
			{
				troopRoster.AddToCounts(partyTemplateStack.Character, partyTemplateStack.MaxValue, false, 0, 0, true, -1);
			}
			return troopRoster;
		}

		private const int VassalRewardBannerLevel = 2;
	}
}
