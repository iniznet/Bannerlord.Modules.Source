using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x0200037A RID: 890
	public class BattleCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x0600340B RID: 13323 RVA: 0x000D982E File Offset: 0x000D7A2E
		public override void RegisterEvents()
		{
			CampaignEvents.OnHeroCombatHitEvent.AddNonSerializedListener(this, new Action<CharacterObject, CharacterObject, PartyBase, WeaponComponentData, bool, int>(this.OnHeroCombatHit));
			CampaignEvents.CollectLootsEvent.AddNonSerializedListener(this, new Action<MapEvent, PartyBase, Dictionary<PartyBase, ItemRoster>, ItemRoster, MBList<TroopRosterElement>, float>(this.CollectLoots));
		}

		// Token: 0x0600340C RID: 13324 RVA: 0x000D985E File Offset: 0x000D7A5E
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x0600340D RID: 13325 RVA: 0x000D9860 File Offset: 0x000D7A60
		private void CollectLoots(MapEvent mapEvent, PartyBase winnerParty, Dictionary<PartyBase, ItemRoster> lootedItems, ItemRoster lootedItemsForParty, ICollection<TroopRosterElement> shareFromCasualties, float lootAmount)
		{
			foreach (KeyValuePair<PartyBase, ItemRoster> keyValuePair in lootedItems)
			{
				ItemRoster value = keyValuePair.Value;
				new ItemRoster();
				for (int i = value.Count - 1; i >= 0; i--)
				{
					ItemRosterElement elementCopyAtIndex = value.GetElementCopyAtIndex(i);
					ItemModifier itemModifier = elementCopyAtIndex.EquipmentElement.ItemModifier;
					bool flag = itemModifier != null && winnerParty.IsMobile && winnerParty.MobileParty.HasPerk(DefaultPerks.Engineering.Metallurgy, false) && itemModifier.PriceMultiplier < 1f;
					for (int j = 0; j < elementCopyAtIndex.Amount; j++)
					{
						if (MBRandom.RandomFloat < lootAmount)
						{
							if (flag && MBRandom.RandomFloat < DefaultPerks.Engineering.Metallurgy.PrimaryBonus)
							{
								ItemRosterElement itemRosterElement = new ItemRosterElement(elementCopyAtIndex.EquipmentElement.Item, 1, null);
								lootedItemsForParty.Add(itemRosterElement);
							}
							else
							{
								lootedItemsForParty.AddToCounts(elementCopyAtIndex.EquipmentElement, 1);
							}
						}
					}
				}
			}
		}

		// Token: 0x0600340E RID: 13326 RVA: 0x000D9990 File Offset: 0x000D7B90
		private void OnHeroCombatHit(CharacterObject attacker, CharacterObject attacked, PartyBase party, WeaponComponentData attackerWeapon, bool isFatal, int xpGained)
		{
			if (attacker.HeroObject.GetPerkValue(DefaultPerks.TwoHanded.BaptisedInBlood) && attackerWeapon != null && isFatal && party.MemberRoster.TotalRegulars > 0 && (attackerWeapon.WeaponClass == WeaponClass.TwoHandedSword || attackerWeapon.WeaponClass == WeaponClass.TwoHandedPolearm || attackerWeapon.WeaponClass == WeaponClass.TwoHandedAxe || attackerWeapon.WeaponClass == WeaponClass.TwoHandedMace))
			{
				for (int i = 0; i < party.MemberRoster.Count; i++)
				{
					TroopRosterElement elementCopyAtIndex = party.MemberRoster.GetElementCopyAtIndex(i);
					if (!elementCopyAtIndex.Character.IsHero && elementCopyAtIndex.Character.IsInfantry)
					{
						party.MemberRoster.AddXpToTroopAtIndex((int)DefaultPerks.TwoHanded.BaptisedInBlood.PrimaryBonus * elementCopyAtIndex.Number, i);
					}
				}
			}
		}
	}
}
