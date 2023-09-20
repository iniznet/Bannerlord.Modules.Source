using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x0200037C RID: 892
	public class CampaignBattleRecoveryBehavior : CampaignBehaviorBase
	{
		// Token: 0x06003419 RID: 13337 RVA: 0x000DA081 File Offset: 0x000D8281
		public override void RegisterEvents()
		{
			CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.OnMapEventEnded));
			CampaignEvents.DailyTickPartyEvent.AddNonSerializedListener(this, new Action<MobileParty>(this.DailyTickParty));
		}

		// Token: 0x0600341A RID: 13338 RVA: 0x000DA0B4 File Offset: 0x000D82B4
		private void DailyTickParty(MobileParty party)
		{
			if (party.HasPerk(DefaultPerks.Medicine.Veterinarian, false) && MBRandom.RandomFloat < DefaultPerks.Medicine.Veterinarian.PrimaryBonus)
			{
				ItemModifier @object = MBObjectManager.Instance.GetObject<ItemModifier>("lame_horse");
				int num = MBRandom.RandomInt(party.ItemRoster.Count);
				for (int i = num; i < party.ItemRoster.Count + num; i++)
				{
					int num2 = i % party.ItemRoster.Count;
					ItemObject itemAtIndex = party.ItemRoster.GetItemAtIndex(num2);
					ItemRosterElement elementCopyAtIndex = party.ItemRoster.GetElementCopyAtIndex(num2);
					if (elementCopyAtIndex.EquipmentElement.ItemModifier == @object)
					{
						party.ItemRoster.AddToCounts(elementCopyAtIndex.EquipmentElement, -1);
						party.ItemRoster.Add(new ItemRosterElement(itemAtIndex, 1, null));
						return;
					}
				}
			}
		}

		// Token: 0x0600341B RID: 13339 RVA: 0x000DA185 File Offset: 0x000D8385
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x0600341C RID: 13340 RVA: 0x000DA187 File Offset: 0x000D8387
		private void OnMapEventEnded(MapEvent mapEvent)
		{
			this.CheckRecoveryForMapEventSide(mapEvent.AttackerSide);
			this.CheckRecoveryForMapEventSide(mapEvent.DefenderSide);
		}

		// Token: 0x0600341D RID: 13341 RVA: 0x000DA1A4 File Offset: 0x000D83A4
		private void CheckRecoveryForMapEventSide(MapEventSide mapEventSide)
		{
			if (mapEventSide.MapEvent.EventType == MapEvent.BattleTypes.FieldBattle || mapEventSide.MapEvent.EventType == MapEvent.BattleTypes.Siege || mapEventSide.MapEvent.EventType == MapEvent.BattleTypes.SiegeOutside)
			{
				foreach (MapEventParty mapEventParty in mapEventSide.Parties)
				{
					PartyBase party = mapEventParty.Party;
					if (party.IsMobile)
					{
						MobileParty mobileParty = party.MobileParty;
						foreach (TroopRosterElement troopRosterElement in mapEventParty.WoundedInBattle.GetTroopRoster())
						{
							int num = mapEventParty.WoundedInBattle.FindIndexOfTroop(troopRosterElement.Character);
							int elementNumber = mapEventParty.WoundedInBattle.GetElementNumber(num);
							if (mobileParty.HasPerk(DefaultPerks.Medicine.BattleHardened, false))
							{
								this.GiveTroopXp(troopRosterElement, elementNumber, party, (int)DefaultPerks.Medicine.BattleHardened.PrimaryBonus);
							}
						}
						foreach (TroopRosterElement troopRosterElement2 in mapEventParty.DiedInBattle.GetTroopRoster())
						{
							int num2 = mapEventParty.DiedInBattle.FindIndexOfTroop(troopRosterElement2.Character);
							int elementNumber2 = mapEventParty.DiedInBattle.GetElementNumber(num2);
							if (mobileParty.HasPerk(DefaultPerks.Medicine.Veterinarian, false) && troopRosterElement2.Character.IsMounted)
							{
								this.RecoverMountWithChance(troopRosterElement2, elementNumber2, party);
							}
						}
					}
				}
			}
		}

		// Token: 0x0600341E RID: 13342 RVA: 0x000DA37C File Offset: 0x000D857C
		private void RecoverMountWithChance(TroopRosterElement troopRosterElement, int count, PartyBase party)
		{
			EquipmentElement equipmentElement = troopRosterElement.Character.Equipment[10];
			if (equipmentElement.Item != null)
			{
				for (int i = 0; i < count; i++)
				{
					if (MBRandom.RandomFloat < DefaultPerks.Medicine.Veterinarian.SecondaryBonus)
					{
						party.ItemRoster.AddToCounts(equipmentElement.Item, 1);
					}
				}
			}
		}

		// Token: 0x0600341F RID: 13343 RVA: 0x000DA3D6 File Offset: 0x000D85D6
		private void GiveTroopXp(TroopRosterElement troopRosterElement, int count, PartyBase partyBase, int xp)
		{
			partyBase.MemberRoster.AddXpToTroop(xp * count, troopRosterElement.Character);
		}
	}
}
