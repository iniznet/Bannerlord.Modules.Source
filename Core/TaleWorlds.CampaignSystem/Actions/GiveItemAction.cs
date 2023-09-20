using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.Actions
{
	public static class GiveItemAction
	{
		private static void ApplyInternal(Hero giver, Hero receiver, PartyBase giverParty, PartyBase receiverParty, ItemObject item, int count)
		{
			bool flag = false;
			if (giver == null && receiver == null)
			{
				giverParty.ItemRoster.AddToCounts(item, -count);
				receiverParty.ItemRoster.AddToCounts(item, count);
				flag = true;
			}
			else if (giver.PartyBelongedTo != null && receiver.PartyBelongedTo != null)
			{
				giver.PartyBelongedTo.Party.ItemRoster.AddToCounts(item, -count);
				receiver.PartyBelongedTo.Party.ItemRoster.AddToCounts(item, count);
				flag = true;
			}
			if (flag)
			{
				CampaignEventDispatcher.Instance.OnHeroOrPartyGaveItem(new ValueTuple<Hero, PartyBase>(giver, giverParty), new ValueTuple<Hero, PartyBase>(receiver, receiverParty), item, count, true);
			}
		}

		public static void ApplyForHeroes(Hero giver, Hero receiver, ItemRosterElement item, int count = 1)
		{
			GiveItemAction.ApplyInternal(giver, receiver, null, null, item.EquipmentElement.Item, count);
		}

		public static void ApplyForParties(PartyBase giverParty, PartyBase receiverParty, ItemRosterElement item, int count = 1)
		{
			GiveItemAction.ApplyInternal(null, null, giverParty, receiverParty, item.EquipmentElement.Item, count);
		}

		public static void ApplyForHeroes(Hero giver, Hero receiver, ItemObject item, int count = 1)
		{
			GiveItemAction.ApplyInternal(giver, receiver, null, null, item, count);
		}

		public static void ApplyForParties(PartyBase giverParty, PartyBase receiverParty, ItemObject item, int count = 1)
		{
			GiveItemAction.ApplyInternal(null, null, giverParty, receiverParty, item, count);
		}
	}
}
