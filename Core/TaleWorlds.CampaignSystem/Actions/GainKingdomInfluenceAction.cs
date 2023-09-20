using System;
using Helpers;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.Actions
{
	public static class GainKingdomInfluenceAction
	{
		private static void ApplyInternal(Hero hero, MobileParty party, float gainedInfluence, GainKingdomInfluenceAction.InfluenceGainingReason detail)
		{
			Clan clan = ((hero != null) ? (hero.CompanionOf ?? hero.Clan) : party.ActualClan);
			if (clan.Kingdom == null)
			{
				return;
			}
			MobileParty mobileParty = party ?? hero.PartyBelongedTo;
			if (detail != GainKingdomInfluenceAction.InfluenceGainingReason.BeingAtArmy && detail == GainKingdomInfluenceAction.InfluenceGainingReason.ClanSupport)
			{
				gainedInfluence = 0.5f;
			}
			if (detail != GainKingdomInfluenceAction.InfluenceGainingReason.Default && detail != GainKingdomInfluenceAction.InfluenceGainingReason.GivingFood && detail != GainKingdomInfluenceAction.InfluenceGainingReason.JoinFaction && detail != GainKingdomInfluenceAction.InfluenceGainingReason.ClanSupport && ((Kingdom)clan.MapFaction).ActivePolicies.Contains(DefaultPolicies.MilitaryCoronae))
			{
				gainedInfluence *= 1.2f;
			}
			ExplainedNumber explainedNumber = new ExplainedNumber(gainedInfluence, false, null);
			if (detail == GainKingdomInfluenceAction.InfluenceGainingReason.Battle && gainedInfluence > 0f)
			{
				PerkHelper.AddPerkBonusForParty(DefaultPerks.Tactics.PreBattleManeuvers, mobileParty, true, ref explainedNumber);
			}
			if (detail == GainKingdomInfluenceAction.InfluenceGainingReason.CaptureSettlement && (hero != null || mobileParty.LeaderHero != null))
			{
				Hero hero2 = hero ?? mobileParty.LeaderHero;
				PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Tactics.Besieged, hero2.CharacterObject, false, ref explainedNumber);
			}
			ChangeClanInfluenceAction.Apply(clan, explainedNumber.ResultNumber);
			if (detail == GainKingdomInfluenceAction.InfluenceGainingReason.Battle && hero == Hero.MainHero)
			{
				MBTextManager.SetTextVariable("INFLUENCE", (int)gainedInfluence);
				MBTextManager.SetTextVariable("NEW_INFLUENCE", (int)clan.Influence);
				InformationManager.DisplayMessage(new InformationMessage(GameTexts.FindText("str_influence_gain_message", null).ToString()));
			}
			if (detail == GainKingdomInfluenceAction.InfluenceGainingReason.SiegeSafePassage && hero == Hero.MainHero)
			{
				MBTextManager.SetTextVariable("INFLUENCE", -(int)gainedInfluence);
				InformationManager.DisplayMessage(new InformationMessage(GameTexts.FindText("str_leave_siege_lose_influence_message", null).ToString()));
			}
		}

		public static void ApplyForBattle(Hero hero, float value)
		{
			GainKingdomInfluenceAction.ApplyInternal(hero, null, value, GainKingdomInfluenceAction.InfluenceGainingReason.Battle);
		}

		public static void ApplyForGivingFood(Hero hero1, Hero hero2, float value)
		{
			GainKingdomInfluenceAction.ApplyInternal(hero1, null, value, GainKingdomInfluenceAction.InfluenceGainingReason.GivingFood);
			GainKingdomInfluenceAction.ApplyInternal(hero2, null, -value, GainKingdomInfluenceAction.InfluenceGainingReason.GivingFood);
		}

		public static void ApplyForDefault(Hero hero, float value)
		{
			GainKingdomInfluenceAction.ApplyInternal(hero, null, value, GainKingdomInfluenceAction.InfluenceGainingReason.Default);
		}

		public static void ApplyForJoiningFaction(Hero hero, float value)
		{
			GainKingdomInfluenceAction.ApplyInternal(hero, null, value, GainKingdomInfluenceAction.InfluenceGainingReason.JoinFaction);
		}

		public static void ApplyForDonatePrisoners(MobileParty donatingParty, float value)
		{
			GainKingdomInfluenceAction.ApplyInternal(null, donatingParty, value, GainKingdomInfluenceAction.InfluenceGainingReason.DonatePrisoners);
		}

		public static void ApplyForRaidingEnemyVillage(MobileParty side1Party, float value)
		{
			GainKingdomInfluenceAction.ApplyInternal(null, side1Party, value, GainKingdomInfluenceAction.InfluenceGainingReason.Raiding);
		}

		public static void ApplyForBesiegingEnemySettlement(MobileParty side1Party, float value)
		{
			GainKingdomInfluenceAction.ApplyInternal(null, side1Party, value, GainKingdomInfluenceAction.InfluenceGainingReason.Besieging);
		}

		public static void ApplyForSiegeSafePassageBarter(MobileParty side1Party, float value)
		{
			GainKingdomInfluenceAction.ApplyInternal(null, side1Party, value, GainKingdomInfluenceAction.InfluenceGainingReason.SiegeSafePassage);
		}

		public static void ApplyForCapturingEnemySettlement(MobileParty side1Party, float value)
		{
			GainKingdomInfluenceAction.ApplyInternal(null, side1Party, value, GainKingdomInfluenceAction.InfluenceGainingReason.CaptureSettlement);
		}

		public static void ApplyForLeavingTroopToGarrison(Hero hero, float value)
		{
			GainKingdomInfluenceAction.ApplyInternal(hero, null, value, GainKingdomInfluenceAction.InfluenceGainingReason.LeaveGarrison);
		}

		public static void ApplyForBoardGameWon(Hero hero, float value)
		{
			GainKingdomInfluenceAction.ApplyInternal(hero, null, value, GainKingdomInfluenceAction.InfluenceGainingReason.BoardGameWon);
		}

		private enum InfluenceGainingReason
		{
			Default,
			BeingAtArmy,
			Battle,
			Raiding,
			Besieging,
			CaptureSettlement,
			JoinFaction,
			GivingFood,
			LeaveGarrison,
			BoardGameWon,
			ClanSupport,
			DonatePrisoners,
			SiegeSafePassage
		}
	}
}
