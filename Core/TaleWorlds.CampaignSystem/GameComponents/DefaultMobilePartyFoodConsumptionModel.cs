using System;
using Helpers;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x0200010C RID: 268
	public class DefaultMobilePartyFoodConsumptionModel : MobilePartyFoodConsumptionModel
	{
		// Token: 0x170005F9 RID: 1529
		// (get) Token: 0x060015A9 RID: 5545 RVA: 0x00066597 File Offset: 0x00064797
		public override int NumberOfMenOnMapToEatOneFood
		{
			get
			{
				return 20;
			}
		}

		// Token: 0x060015AA RID: 5546 RVA: 0x0006659C File Offset: 0x0006479C
		public override ExplainedNumber CalculateDailyBaseFoodConsumptionf(MobileParty party, bool includeDescription = false)
		{
			int num = party.Party.NumberOfAllMembers + party.Party.NumberOfPrisoners / 2;
			num = ((num < 1) ? 1 : num);
			return new ExplainedNumber(-(float)num / (float)this.NumberOfMenOnMapToEatOneFood, includeDescription, null);
		}

		// Token: 0x060015AB RID: 5547 RVA: 0x000665DE File Offset: 0x000647DE
		public override ExplainedNumber CalculateDailyFoodConsumptionf(MobileParty party, ExplainedNumber baseConsumption)
		{
			this.CalculatePerkEffects(party, ref baseConsumption);
			baseConsumption.LimitMax(0f);
			return baseConsumption;
		}

		// Token: 0x060015AC RID: 5548 RVA: 0x000665F8 File Offset: 0x000647F8
		private void CalculatePerkEffects(MobileParty party, ref ExplainedNumber result)
		{
			int num = 0;
			for (int i = 0; i < party.MemberRoster.Count; i++)
			{
				if (party.MemberRoster.GetCharacterAtIndex(i).Culture.IsBandit)
				{
					num += party.MemberRoster.GetElementNumber(i);
				}
			}
			for (int j = 0; j < party.PrisonRoster.Count; j++)
			{
				if (party.PrisonRoster.GetCharacterAtIndex(j).Culture.IsBandit)
				{
					num += party.PrisonRoster.GetElementNumber(j);
				}
			}
			if (party.LeaderHero != null && party.LeaderHero.GetPerkValue(DefaultPerks.Roguery.Promises) && num > 0)
			{
				float num2 = (float)num / (float)this.NumberOfMenOnMapToEatOneFood * DefaultPerks.Roguery.Promises.PrimaryBonus;
				result.Add(num2, DefaultPerks.Roguery.Promises.Name, null);
			}
			PerkHelper.AddPerkBonusForParty(DefaultPerks.Athletics.Spartan, party, false, ref result);
			PerkHelper.AddPerkBonusForParty(DefaultPerks.Steward.WarriorsDiet, party, true, ref result);
			if (party.EffectiveQuartermaster != null)
			{
				PerkHelper.AddEpicPerkBonusForCharacter(DefaultPerks.Steward.PriceOfLoyalty, party.EffectiveQuartermaster.CharacterObject, DefaultSkills.Steward, true, ref result, 250);
			}
			TerrainType faceTerrainType = Campaign.Current.MapSceneWrapper.GetFaceTerrainType(party.CurrentNavigationFace);
			if (faceTerrainType == TerrainType.Forest || faceTerrainType == TerrainType.Steppe)
			{
				PerkHelper.AddPerkBonusForParty(DefaultPerks.Scouting.Foragers, party, true, ref result);
			}
			if (party.IsGarrison && party.CurrentSettlement != null && party.CurrentSettlement.Town.IsUnderSiege)
			{
				PerkHelper.AddPerkBonusForTown(DefaultPerks.Athletics.StrongLegs, party.CurrentSettlement.Town, ref result);
			}
			if (party.Army != null)
			{
				PerkHelper.AddPerkBonusForParty(DefaultPerks.Steward.StiffUpperLip, party, true, ref result);
			}
			SiegeEvent siegeEvent = party.SiegeEvent;
			if (((siegeEvent != null) ? siegeEvent.BesiegerCamp : null) != null)
			{
				if (party.SiegeEvent.BesiegerCamp.HasInvolvedPartyForEventType(party.Party, MapEvent.BattleTypes.Siege) && party.HasPerk(DefaultPerks.Steward.SoundReserves, true))
				{
					PerkHelper.AddPerkBonusForParty(DefaultPerks.Steward.SoundReserves, party, false, ref result);
				}
				if (party.HasPerk(DefaultPerks.Steward.MasterOfPlanning, false))
				{
					result.AddFactor(DefaultPerks.Steward.MasterOfPlanning.PrimaryBonus, DefaultPerks.Steward.MasterOfPlanning.Name);
				}
			}
		}

		// Token: 0x060015AD RID: 5549 RVA: 0x000667F8 File Offset: 0x000649F8
		public override bool DoesPartyConsumeFood(MobileParty mobileParty)
		{
			return mobileParty.IsActive && (mobileParty.LeaderHero == null || mobileParty.LeaderHero.IsLord || mobileParty.LeaderHero.Clan == Clan.PlayerClan || mobileParty.LeaderHero.IsMinorFactionHero) && !mobileParty.IsGarrison && !mobileParty.IsCaravan && !mobileParty.IsBandit && !mobileParty.IsMilitia && !mobileParty.IsVillager;
		}

		// Token: 0x04000794 RID: 1940
		private static readonly TextObject _partyConsumption = new TextObject("{=UrFzdy4z}Daily Consumption", null);
	}
}
