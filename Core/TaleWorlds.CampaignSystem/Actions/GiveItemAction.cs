using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.Actions
{
	// Token: 0x02000445 RID: 1093
	public static class GiveItemAction
	{
		// Token: 0x06003F16 RID: 16150 RVA: 0x0012D334 File Offset: 0x0012B534
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

		// Token: 0x06003F17 RID: 16151 RVA: 0x0012D3D8 File Offset: 0x0012B5D8
		public static void ApplyForHeroes(Hero giver, Hero receiver, ItemRosterElement item, int count = 1)
		{
			GiveItemAction.ApplyInternal(giver, receiver, null, null, item.EquipmentElement.Item, count);
		}

		// Token: 0x06003F18 RID: 16152 RVA: 0x0012D400 File Offset: 0x0012B600
		public static void ApplyForParties(PartyBase giverParty, PartyBase receiverParty, ItemRosterElement item, int count = 1)
		{
			GiveItemAction.ApplyInternal(null, null, giverParty, receiverParty, item.EquipmentElement.Item, count);
		}

		// Token: 0x06003F19 RID: 16153 RVA: 0x0012D426 File Offset: 0x0012B626
		public static void ApplyForHeroes(Hero giver, Hero receiver, ItemObject item, int count = 1)
		{
			GiveItemAction.ApplyInternal(giver, receiver, null, null, item, count);
		}

		// Token: 0x06003F1A RID: 16154 RVA: 0x0012D433 File Offset: 0x0012B633
		public static void ApplyForParties(PartyBase giverParty, PartyBase receiverParty, ItemObject item, int count = 1)
		{
			GiveItemAction.ApplyInternal(null, null, giverParty, receiverParty, item, count);
		}
	}
}
