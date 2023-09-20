using System;
using Helpers;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x020000F3 RID: 243
	public class DefaultBattleRewardModel : BattleRewardModel
	{
		// Token: 0x170005C7 RID: 1479
		// (get) Token: 0x06001483 RID: 5251 RVA: 0x0005B5EC File Offset: 0x000597EC
		public override float DestroyHideoutBannerLootChance
		{
			get
			{
				return 0.1f;
			}
		}

		// Token: 0x170005C8 RID: 1480
		// (get) Token: 0x06001484 RID: 5252 RVA: 0x0005B5F3 File Offset: 0x000597F3
		public override float CaptureSettlementBannerLootChance
		{
			get
			{
				return 0.5f;
			}
		}

		// Token: 0x170005C9 RID: 1481
		// (get) Token: 0x06001485 RID: 5253 RVA: 0x0005B5FA File Offset: 0x000597FA
		public override float DefeatRegularHeroBannerLootChance
		{
			get
			{
				return 0.5f;
			}
		}

		// Token: 0x170005CA RID: 1482
		// (get) Token: 0x06001486 RID: 5254 RVA: 0x0005B601 File Offset: 0x00059801
		public override float DefeatClanLeaderBannerLootChance
		{
			get
			{
				return 0.25f;
			}
		}

		// Token: 0x170005CB RID: 1483
		// (get) Token: 0x06001487 RID: 5255 RVA: 0x0005B608 File Offset: 0x00059808
		public override float DefeatKingdomRulerBannerLootChance
		{
			get
			{
				return 0.1f;
			}
		}

		// Token: 0x06001488 RID: 5256 RVA: 0x0005B610 File Offset: 0x00059810
		public override int GetPlayerGainedRelationAmount(MapEvent mapEvent, Hero hero)
		{
			MapEventSide mapEventSide = (mapEvent.AttackerSide.IsMainPartyAmongParties() ? mapEvent.AttackerSide : mapEvent.DefenderSide);
			float playerPartyContributionRate = mapEventSide.GetPlayerPartyContributionRate();
			float num = (mapEvent.StrengthOfSide[(int)PartyBase.MainParty.Side] - PlayerEncounter.Current.PlayerPartyInitialStrength) / (mapEvent.StrengthOfSide[(int)PartyBase.MainParty.OpponentSide] + 1f);
			float num2 = ((num < 1f) ? (1f + (1f - num)) : ((num < 3f) ? (0.5f * (3f - num)) : 0f));
			float renownValue = mapEvent.GetRenownValue((mapEventSide == mapEvent.AttackerSide) ? BattleSideEnum.Attacker : BattleSideEnum.Defender);
			ExplainedNumber explainedNumber = new ExplainedNumber(0.75f + MathF.Pow(playerPartyContributionRate * 1.3f * (num2 + renownValue), 0.67f), false, null);
			if (Hero.MainHero.GetPerkValue(DefaultPerks.Charm.Camaraderie))
			{
				explainedNumber.AddFactor(DefaultPerks.Charm.Camaraderie.PrimaryBonus, DefaultPerks.Charm.Camaraderie.Name);
			}
			return (int)explainedNumber.ResultNumber;
		}

		// Token: 0x06001489 RID: 5257 RVA: 0x0005B718 File Offset: 0x00059918
		public override ExplainedNumber CalculateRenownGain(PartyBase party, float renownValueOfBattle, float contributionShare)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(renownValueOfBattle * contributionShare, true, null);
			if (party.IsMobile)
			{
				if (party.MobileParty.HasPerk(DefaultPerks.Throwing.LongReach, true))
				{
					PerkHelper.AddPerkBonusForParty(DefaultPerks.Throwing.LongReach, party.MobileParty, false, ref explainedNumber);
				}
				if (party.MobileParty.HasPerk(DefaultPerks.Charm.PublicSpeaker, false))
				{
					explainedNumber.AddFactor(DefaultPerks.Charm.PublicSpeaker.PrimaryBonus, DefaultPerks.Charm.PublicSpeaker.Name);
				}
				if (party.LeaderHero != null)
				{
					PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Leadership.FamousCommander, party.LeaderHero.CharacterObject, true, ref explainedNumber);
				}
				if (PartyBaseHelper.HasFeat(party, DefaultCulturalFeats.VlandianRenownMercenaryFeat))
				{
					explainedNumber.AddFactor(DefaultCulturalFeats.VlandianRenownMercenaryFeat.EffectBonus, GameTexts.FindText("str_culture", null));
				}
			}
			return explainedNumber;
		}

		// Token: 0x0600148A RID: 5258 RVA: 0x0005B7DC File Offset: 0x000599DC
		public override ExplainedNumber CalculateInfluenceGain(PartyBase party, float influenceValueOfBattle, float contributionShare)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(party.MapFaction.IsKingdomFaction ? (influenceValueOfBattle * contributionShare) : 0f, true, null);
			Hero leaderHero = party.LeaderHero;
			if (leaderHero != null && leaderHero.GetPerkValue(DefaultPerks.Charm.Warlord))
			{
				explainedNumber.AddFactor(DefaultPerks.Charm.Warlord.PrimaryBonus, DefaultPerks.Charm.Warlord.Name);
			}
			return explainedNumber;
		}

		// Token: 0x0600148B RID: 5259 RVA: 0x0005B840 File Offset: 0x00059A40
		public override ExplainedNumber CalculateMoraleGainVictory(PartyBase party, float renownValueOfBattle, float contributionShare)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(0.5f + renownValueOfBattle * contributionShare * 0.5f, true, null);
			if (party.IsMobile && party.MobileParty.HasPerk(DefaultPerks.Throwing.LongReach, true))
			{
				PerkHelper.AddPerkBonusForParty(DefaultPerks.Throwing.LongReach, party.MobileParty, false, ref explainedNumber);
			}
			if (party.IsMobile && party.MobileParty.HasPerk(DefaultPerks.Leadership.CitizenMilitia, true))
			{
				PerkHelper.AddPerkBonusForParty(DefaultPerks.Leadership.CitizenMilitia, party.MobileParty, false, ref explainedNumber);
			}
			return explainedNumber;
		}

		// Token: 0x0600148C RID: 5260 RVA: 0x0005B8C4 File Offset: 0x00059AC4
		public override int CalculateGoldLossAfterDefeat(Hero partyLeaderHero)
		{
			float num = (float)partyLeaderHero.Gold * 0.05f;
			if (num > 10000f)
			{
				num = 10000f;
			}
			return (int)num;
		}

		// Token: 0x0600148D RID: 5261 RVA: 0x0005B8F0 File Offset: 0x00059AF0
		public override EquipmentElement GetLootedItemFromTroop(CharacterObject character, float targetValue)
		{
			Equipment randomElement = character.AllEquipments.GetRandomElement<Equipment>();
			return this.GetRandomItem(randomElement, targetValue);
		}

		// Token: 0x0600148E RID: 5262 RVA: 0x0005B914 File Offset: 0x00059B14
		private EquipmentElement GetRandomItem(Equipment equipment, float targetValue = 0f)
		{
			int num = 0;
			for (int i = 0; i < 12; i++)
			{
				if (equipment[i].Item != null && !equipment[i].Item.NotMerchandise)
				{
					DefaultBattleRewardModel._indices[num] = i;
					num++;
				}
			}
			for (int j = 0; j < num - 1; j++)
			{
				int num2 = j;
				int num3 = equipment[DefaultBattleRewardModel._indices[j]].Item.Value;
				for (int k = j + 1; k < num; k++)
				{
					if (equipment[DefaultBattleRewardModel._indices[k]].Item.Value > num3)
					{
						num2 = k;
						num3 = equipment[DefaultBattleRewardModel._indices[k]].Item.Value;
					}
				}
				int num4 = DefaultBattleRewardModel._indices[j];
				DefaultBattleRewardModel._indices[j] = DefaultBattleRewardModel._indices[num2];
				DefaultBattleRewardModel._indices[num2] = num4;
			}
			if (num > 0)
			{
				for (int l = 0; l < num; l++)
				{
					int num5 = DefaultBattleRewardModel._indices[l];
					EquipmentElement equipmentElement = equipment[num5];
					if (equipmentElement.Item != null && !equipment[num5].Item.NotMerchandise)
					{
						float num6 = (float)equipmentElement.Item.Value + 0.1f;
						float num7 = 0.6f * (targetValue / (MathF.Max(targetValue, num6) * (float)(num - l)));
						if (MBRandom.RandomFloat < num7)
						{
							ItemComponent itemComponent = equipmentElement.Item.ItemComponent;
							ItemModifier itemModifier;
							if (itemComponent == null)
							{
								itemModifier = null;
							}
							else
							{
								ItemModifierGroup itemModifierGroup = itemComponent.ItemModifierGroup;
								itemModifier = ((itemModifierGroup != null) ? itemModifierGroup.GetRandomItemModifierLootScoreBased() : null);
							}
							ItemModifier itemModifier2 = itemModifier;
							if (itemModifier2 != null)
							{
								equipmentElement = new EquipmentElement(equipmentElement.Item, itemModifier2, null, false);
							}
							return equipmentElement;
						}
					}
				}
			}
			return default(EquipmentElement);
		}

		// Token: 0x0600148F RID: 5263 RVA: 0x0005BADC File Offset: 0x00059CDC
		public override float GetPartySavePrisonerAsMemberShareProbability(PartyBase winnerParty, float lootAmount)
		{
			float num = lootAmount;
			if (winnerParty.IsMobile && winnerParty.MobileParty.IsBandit && winnerParty.MobileParty.CurrentSettlement != null && winnerParty.MobileParty.CurrentSettlement.IsHideout)
			{
				num = 0f;
			}
			return num;
		}

		// Token: 0x06001490 RID: 5264 RVA: 0x0005BB26 File Offset: 0x00059D26
		public override float GetExpectedLootedItemValue(CharacterObject character)
		{
			return 6f * (float)(character.Level * character.Level);
		}

		// Token: 0x06001491 RID: 5265 RVA: 0x0005BB3C File Offset: 0x00059D3C
		public override float GetAITradePenalty()
		{
			return 0.018181818f;
		}

		// Token: 0x04000732 RID: 1842
		private static int[] _indices = new int[12];
	}
}
