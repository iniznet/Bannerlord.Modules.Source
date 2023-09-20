using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x020003BA RID: 954
	public class PartyUpgraderCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x060038BD RID: 14525 RVA: 0x00102C10 File Offset: 0x00100E10
		public override void RegisterEvents()
		{
			CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.MapEventEnded));
			CampaignEvents.DailyTickPartyEvent.AddNonSerializedListener(this, new Action<MobileParty>(this.DailyTickParty));
		}

		// Token: 0x060038BE RID: 14526 RVA: 0x00102C40 File Offset: 0x00100E40
		private void MapEventEnded(MapEvent mapEvent)
		{
			foreach (PartyBase partyBase in mapEvent.InvolvedParties)
			{
				this.UpgradeReadyTroops(partyBase);
			}
		}

		// Token: 0x060038BF RID: 14527 RVA: 0x00102C90 File Offset: 0x00100E90
		public void DailyTickParty(MobileParty party)
		{
			if (party.MapEvent == null)
			{
				this.UpgradeReadyTroops(party.Party);
			}
		}

		// Token: 0x060038C0 RID: 14528 RVA: 0x00102CA6 File Offset: 0x00100EA6
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x060038C1 RID: 14529 RVA: 0x00102CA8 File Offset: 0x00100EA8
		private PartyUpgraderCampaignBehavior.TroopUpgradeArgs SelectPossibleUpgrade(List<PartyUpgraderCampaignBehavior.TroopUpgradeArgs> possibleUpgrades)
		{
			PartyUpgraderCampaignBehavior.TroopUpgradeArgs troopUpgradeArgs = possibleUpgrades[0];
			if (possibleUpgrades.Count > 1)
			{
				float num = 0f;
				foreach (PartyUpgraderCampaignBehavior.TroopUpgradeArgs troopUpgradeArgs2 in possibleUpgrades)
				{
					num += troopUpgradeArgs2.UpgradeChance;
				}
				float num2 = num * MBRandom.RandomFloat;
				foreach (PartyUpgraderCampaignBehavior.TroopUpgradeArgs troopUpgradeArgs3 in possibleUpgrades)
				{
					num2 -= troopUpgradeArgs3.UpgradeChance;
					if (num2 <= 0f)
					{
						troopUpgradeArgs = troopUpgradeArgs3;
						break;
					}
				}
			}
			return troopUpgradeArgs;
		}

		// Token: 0x060038C2 RID: 14530 RVA: 0x00102D6C File Offset: 0x00100F6C
		private List<PartyUpgraderCampaignBehavior.TroopUpgradeArgs> GetPossibleUpgradeTargets(PartyBase party, TroopRosterElement element)
		{
			PartyWageModel partyWageModel = Campaign.Current.Models.PartyWageModel;
			List<PartyUpgraderCampaignBehavior.TroopUpgradeArgs> list = new List<PartyUpgraderCampaignBehavior.TroopUpgradeArgs>();
			CharacterObject character = element.Character;
			int num = element.Number - element.WoundedNumber;
			if (num > 0)
			{
				PartyTroopUpgradeModel partyTroopUpgradeModel = Campaign.Current.Models.PartyTroopUpgradeModel;
				int i = 0;
				while (i < character.UpgradeTargets.Length)
				{
					CharacterObject characterObject = character.UpgradeTargets[i];
					int upgradeXpCost = character.GetUpgradeXpCost(party, i);
					if (upgradeXpCost <= 0)
					{
						goto IL_7F;
					}
					num = MathF.Min(num, element.Xp / upgradeXpCost);
					if (num != 0)
					{
						goto IL_7F;
					}
					IL_256:
					i++;
					continue;
					IL_7F:
					if (characterObject.Tier > character.Tier && party.MobileParty.HasLimitedWage() && party.MobileParty.TotalWage + num * (partyWageModel.GetCharacterWage(characterObject) - partyWageModel.GetCharacterWage(character)) > party.MobileParty.PaymentLimit)
					{
						num = MathF.Max(0, MathF.Min(num, (party.MobileParty.PaymentLimit - party.MobileParty.TotalWage) / (partyWageModel.GetCharacterWage(characterObject) - partyWageModel.GetCharacterWage(character))));
						if (num == 0)
						{
							goto IL_256;
						}
					}
					int upgradeGoldCost = character.GetUpgradeGoldCost(party, i);
					if (party.LeaderHero != null && upgradeGoldCost != 0 && num * upgradeGoldCost > party.LeaderHero.Gold)
					{
						num = party.LeaderHero.Gold / upgradeGoldCost;
						if (num == 0)
						{
							goto IL_256;
						}
					}
					if (party.Owner != null && party.Owner.Clan == Clan.PlayerClan && characterObject.UpgradeRequiresItemFromCategory != null)
					{
						bool flag = false;
						int num2 = 0;
						foreach (ItemRosterElement itemRosterElement in party.ItemRoster)
						{
							if (itemRosterElement.EquipmentElement.Item.ItemCategory == characterObject.UpgradeRequiresItemFromCategory && itemRosterElement.EquipmentElement.ItemModifier == null)
							{
								num2 += itemRosterElement.Amount;
								flag = true;
								if (num2 >= num)
								{
									break;
								}
							}
						}
						if (!flag)
						{
							goto IL_256;
						}
						num = MathF.Min(num2, num);
						if (num == 0)
						{
							goto IL_256;
						}
					}
					if ((!party.Culture.IsBandit || characterObject.Culture.IsBandit) && (character.Occupation != Occupation.Bandit || partyTroopUpgradeModel.CanPartyUpgradeTroopToTarget(party, character, characterObject)))
					{
						float upgradeChanceForTroopUpgrade = Campaign.Current.Models.PartyTroopUpgradeModel.GetUpgradeChanceForTroopUpgrade(party, character, i);
						list.Add(new PartyUpgraderCampaignBehavior.TroopUpgradeArgs(character, characterObject, num, upgradeGoldCost, upgradeXpCost, upgradeChanceForTroopUpgrade));
						goto IL_256;
					}
					goto IL_256;
				}
			}
			return list;
		}

		// Token: 0x060038C3 RID: 14531 RVA: 0x00102FF8 File Offset: 0x001011F8
		private void ApplyEffects(PartyBase party, PartyUpgraderCampaignBehavior.TroopUpgradeArgs upgradeArgs)
		{
			if (party.Owner != null && party.Owner.IsAlive)
			{
				SkillLevelingManager.OnUpgradeTroops(party, upgradeArgs.Target, upgradeArgs.UpgradeTarget, upgradeArgs.PossibleUpgradeCount);
				GiveGoldAction.ApplyBetweenCharacters(party.Owner, null, upgradeArgs.UpgradeGoldCost * upgradeArgs.PossibleUpgradeCount, true);
				return;
			}
			if (party.LeaderHero != null && party.LeaderHero.IsAlive)
			{
				SkillLevelingManager.OnUpgradeTroops(party, upgradeArgs.Target, upgradeArgs.UpgradeTarget, upgradeArgs.PossibleUpgradeCount);
				GiveGoldAction.ApplyBetweenCharacters(party.LeaderHero, null, upgradeArgs.UpgradeGoldCost * upgradeArgs.PossibleUpgradeCount, true);
			}
		}

		// Token: 0x060038C4 RID: 14532 RVA: 0x00103094 File Offset: 0x00101294
		private void UpgradeTroop(PartyBase party, int rosterIndex, PartyUpgraderCampaignBehavior.TroopUpgradeArgs upgradeArgs)
		{
			TroopRoster memberRoster = party.MemberRoster;
			CharacterObject upgradeTarget = upgradeArgs.UpgradeTarget;
			int possibleUpgradeCount = upgradeArgs.PossibleUpgradeCount;
			int num = upgradeArgs.UpgradeXpCost * possibleUpgradeCount;
			memberRoster.SetElementXp(rosterIndex, memberRoster.GetElementXp(rosterIndex) - num);
			memberRoster.AddToCounts(upgradeArgs.Target, -possibleUpgradeCount, false, 0, 0, true, -1);
			memberRoster.AddToCounts(upgradeTarget, possibleUpgradeCount, false, 0, 0, true, -1);
			if (party.Owner != null && party.Owner.Clan == Clan.PlayerClan && upgradeTarget.UpgradeRequiresItemFromCategory != null)
			{
				int num2 = possibleUpgradeCount;
				foreach (ItemRosterElement itemRosterElement in party.ItemRoster)
				{
					if (itemRosterElement.EquipmentElement.Item.ItemCategory == upgradeTarget.UpgradeRequiresItemFromCategory && itemRosterElement.EquipmentElement.ItemModifier == null)
					{
						int num3 = MathF.Min(num2, itemRosterElement.Amount);
						party.ItemRoster.AddToCounts(itemRosterElement.EquipmentElement.Item, -num3);
						num2 -= num3;
						if (num2 == 0)
						{
							break;
						}
					}
				}
			}
			if (possibleUpgradeCount > 0)
			{
				this.ApplyEffects(party, upgradeArgs);
			}
		}

		// Token: 0x060038C5 RID: 14533 RVA: 0x001031D8 File Offset: 0x001013D8
		public void UpgradeReadyTroops(PartyBase party)
		{
			if (party != PartyBase.MainParty && party.IsActive)
			{
				TroopRoster memberRoster = party.MemberRoster;
				PartyTroopUpgradeModel partyTroopUpgradeModel = Campaign.Current.Models.PartyTroopUpgradeModel;
				for (int i = 0; i < memberRoster.Count; i++)
				{
					TroopRosterElement elementCopyAtIndex = memberRoster.GetElementCopyAtIndex(i);
					if (partyTroopUpgradeModel.IsTroopUpgradeable(party, elementCopyAtIndex.Character))
					{
						List<PartyUpgraderCampaignBehavior.TroopUpgradeArgs> possibleUpgradeTargets = this.GetPossibleUpgradeTargets(party, elementCopyAtIndex);
						if (possibleUpgradeTargets.Count > 0)
						{
							PartyUpgraderCampaignBehavior.TroopUpgradeArgs troopUpgradeArgs = this.SelectPossibleUpgrade(possibleUpgradeTargets);
							this.UpgradeTroop(party, i, troopUpgradeArgs);
						}
					}
				}
			}
		}

		// Token: 0x0200070A RID: 1802
		private readonly struct TroopUpgradeArgs
		{
			// Token: 0x0600557E RID: 21886 RVA: 0x0016D42E File Offset: 0x0016B62E
			public TroopUpgradeArgs(CharacterObject target, CharacterObject upgradeTarget, int possibleUpgradeCount, int upgradeGoldCost, int upgradeXpCost, float upgradeChance)
			{
				this.Target = target;
				this.UpgradeTarget = upgradeTarget;
				this.PossibleUpgradeCount = possibleUpgradeCount;
				this.UpgradeGoldCost = upgradeGoldCost;
				this.UpgradeXpCost = upgradeXpCost;
				this.UpgradeChance = upgradeChance;
			}

			// Token: 0x04001CEA RID: 7402
			public readonly CharacterObject Target;

			// Token: 0x04001CEB RID: 7403
			public readonly CharacterObject UpgradeTarget;

			// Token: 0x04001CEC RID: 7404
			public readonly int PossibleUpgradeCount;

			// Token: 0x04001CED RID: 7405
			public readonly int UpgradeGoldCost;

			// Token: 0x04001CEE RID: 7406
			public readonly int UpgradeXpCost;

			// Token: 0x04001CEF RID: 7407
			public readonly float UpgradeChance;
		}
	}
}
