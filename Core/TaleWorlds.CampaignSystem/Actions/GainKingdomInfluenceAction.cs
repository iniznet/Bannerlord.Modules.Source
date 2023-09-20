using System;
using Helpers;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.Actions
{
	// Token: 0x02000441 RID: 1089
	public static class GainKingdomInfluenceAction
	{
		// Token: 0x06003EFC RID: 16124 RVA: 0x0012CE5C File Offset: 0x0012B05C
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

		// Token: 0x06003EFD RID: 16125 RVA: 0x0012CFB5 File Offset: 0x0012B1B5
		public static void ApplyForBattle(Hero hero, float value)
		{
			GainKingdomInfluenceAction.ApplyInternal(hero, null, value, GainKingdomInfluenceAction.InfluenceGainingReason.Battle);
		}

		// Token: 0x06003EFE RID: 16126 RVA: 0x0012CFC0 File Offset: 0x0012B1C0
		public static void ApplyForGivingFood(Hero hero1, Hero hero2, float value)
		{
			GainKingdomInfluenceAction.ApplyInternal(hero1, null, value, GainKingdomInfluenceAction.InfluenceGainingReason.GivingFood);
			GainKingdomInfluenceAction.ApplyInternal(hero2, null, -value, GainKingdomInfluenceAction.InfluenceGainingReason.GivingFood);
		}

		// Token: 0x06003EFF RID: 16127 RVA: 0x0012CFD5 File Offset: 0x0012B1D5
		public static void ApplyForDefault(Hero hero, float value)
		{
			GainKingdomInfluenceAction.ApplyInternal(hero, null, value, GainKingdomInfluenceAction.InfluenceGainingReason.Default);
		}

		// Token: 0x06003F00 RID: 16128 RVA: 0x0012CFE0 File Offset: 0x0012B1E0
		public static void ApplyForJoiningFaction(Hero hero, float value)
		{
			GainKingdomInfluenceAction.ApplyInternal(hero, null, value, GainKingdomInfluenceAction.InfluenceGainingReason.JoinFaction);
		}

		// Token: 0x06003F01 RID: 16129 RVA: 0x0012CFEB File Offset: 0x0012B1EB
		public static void ApplyForDonatePrisoners(MobileParty donatingParty, float value)
		{
			GainKingdomInfluenceAction.ApplyInternal(null, donatingParty, value, GainKingdomInfluenceAction.InfluenceGainingReason.DonatePrisoners);
		}

		// Token: 0x06003F02 RID: 16130 RVA: 0x0012CFF7 File Offset: 0x0012B1F7
		public static void ApplyForRaidingEnemyVillage(MobileParty side1Party, float value)
		{
			GainKingdomInfluenceAction.ApplyInternal(null, side1Party, value, GainKingdomInfluenceAction.InfluenceGainingReason.Raiding);
		}

		// Token: 0x06003F03 RID: 16131 RVA: 0x0012D002 File Offset: 0x0012B202
		public static void ApplyForBesiegingEnemySettlement(MobileParty side1Party, float value)
		{
			GainKingdomInfluenceAction.ApplyInternal(null, side1Party, value, GainKingdomInfluenceAction.InfluenceGainingReason.Besieging);
		}

		// Token: 0x06003F04 RID: 16132 RVA: 0x0012D00D File Offset: 0x0012B20D
		public static void ApplyForSiegeSafePassageBarter(MobileParty side1Party, float value)
		{
			GainKingdomInfluenceAction.ApplyInternal(null, side1Party, value, GainKingdomInfluenceAction.InfluenceGainingReason.SiegeSafePassage);
		}

		// Token: 0x06003F05 RID: 16133 RVA: 0x0012D019 File Offset: 0x0012B219
		public static void ApplyForCapturingEnemySettlement(MobileParty side1Party, float value)
		{
			GainKingdomInfluenceAction.ApplyInternal(null, side1Party, value, GainKingdomInfluenceAction.InfluenceGainingReason.CaptureSettlement);
		}

		// Token: 0x06003F06 RID: 16134 RVA: 0x0012D024 File Offset: 0x0012B224
		public static void ApplyForLeavingTroopToGarrison(Hero hero, float value)
		{
			GainKingdomInfluenceAction.ApplyInternal(hero, null, value, GainKingdomInfluenceAction.InfluenceGainingReason.LeaveGarrison);
		}

		// Token: 0x06003F07 RID: 16135 RVA: 0x0012D02F File Offset: 0x0012B22F
		public static void ApplyForBoardGameWon(Hero hero, float value)
		{
			GainKingdomInfluenceAction.ApplyInternal(hero, null, value, GainKingdomInfluenceAction.InfluenceGainingReason.BoardGameWon);
		}

		// Token: 0x0200075F RID: 1887
		private enum InfluenceGainingReason
		{
			// Token: 0x04001E48 RID: 7752
			Default,
			// Token: 0x04001E49 RID: 7753
			BeingAtArmy,
			// Token: 0x04001E4A RID: 7754
			Battle,
			// Token: 0x04001E4B RID: 7755
			Raiding,
			// Token: 0x04001E4C RID: 7756
			Besieging,
			// Token: 0x04001E4D RID: 7757
			CaptureSettlement,
			// Token: 0x04001E4E RID: 7758
			JoinFaction,
			// Token: 0x04001E4F RID: 7759
			GivingFood,
			// Token: 0x04001E50 RID: 7760
			LeaveGarrison,
			// Token: 0x04001E51 RID: 7761
			BoardGameWon,
			// Token: 0x04001E52 RID: 7762
			ClanSupport,
			// Token: 0x04001E53 RID: 7763
			DonatePrisoners,
			// Token: 0x04001E54 RID: 7764
			SiegeSafePassage
		}
	}
}
