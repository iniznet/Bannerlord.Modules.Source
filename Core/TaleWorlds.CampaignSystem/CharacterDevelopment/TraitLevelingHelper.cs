using System;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.CharacterDevelopment
{
	// Token: 0x02000356 RID: 854
	public class TraitLevelingHelper
	{
		// Token: 0x06003049 RID: 12361 RVA: 0x000CB650 File Offset: 0x000C9850
		private static void AddPlayerTraitXPAndLogEntry(TraitObject trait, int xpValue, ActionNotes context, Hero referenceHero)
		{
			int traitLevel = Hero.MainHero.GetTraitLevel(trait);
			Campaign.Current.PlayerTraitDeveloper.AddTraitXp(trait, xpValue);
			if (traitLevel != Hero.MainHero.GetTraitLevel(trait))
			{
				CampaignEventDispatcher.Instance.OnPlayerTraitChanged(trait, traitLevel);
			}
			if (MathF.Abs(xpValue) >= 10)
			{
				LogEntry.AddLogEntry(new PlayerReputationChangesLogEntry(trait, referenceHero, context));
			}
		}

		// Token: 0x0600304A RID: 12362 RVA: 0x000CB6AC File Offset: 0x000C98AC
		public static void OnBattleWon(MapEvent mapEvent, float contribution)
		{
			float num = 0f;
			float strengthRatio = mapEvent.GetMapEventSide(PlayerEncounter.Current.PlayerSide).StrengthRatio;
			if (strengthRatio > 0.9f)
			{
				num = MathF.Min(20f, MathF.Sqrt(mapEvent.StrengthOfSide[(int)mapEvent.GetOtherSide(PlayerEncounter.Current.PlayerSide)]) * strengthRatio);
			}
			int num2 = (int)(num * contribution);
			TraitLevelingHelper.AddPlayerTraitXPAndLogEntry(DefaultTraits.Valor, num2, ActionNotes.BattleValor, null);
		}

		// Token: 0x0600304B RID: 12363 RVA: 0x000CB719 File Offset: 0x000C9919
		public static void OnTroopsSacrificed()
		{
			TraitLevelingHelper.AddPlayerTraitXPAndLogEntry(DefaultTraits.Valor, -30, ActionNotes.SacrificedTroops, null);
		}

		// Token: 0x0600304C RID: 12364 RVA: 0x000CB72A File Offset: 0x000C992A
		public static void OnLordExecuted()
		{
			TraitLevelingHelper.AddPlayerTraitXPAndLogEntry(DefaultTraits.Honor, -1000, ActionNotes.SacrificedTroops, null);
		}

		// Token: 0x0600304D RID: 12365 RVA: 0x000CB73E File Offset: 0x000C993E
		public static void OnVillageRaided()
		{
			TraitLevelingHelper.AddPlayerTraitXPAndLogEntry(DefaultTraits.Mercy, -30, ActionNotes.VillageRaid, null);
		}

		// Token: 0x0600304E RID: 12366 RVA: 0x000CB74F File Offset: 0x000C994F
		public static void OnHostileAction(int amount)
		{
			TraitLevelingHelper.AddPlayerTraitXPAndLogEntry(DefaultTraits.Honor, amount, ActionNotes.HostileAction, null);
			TraitLevelingHelper.AddPlayerTraitXPAndLogEntry(DefaultTraits.Mercy, amount, ActionNotes.HostileAction, null);
		}

		// Token: 0x0600304F RID: 12367 RVA: 0x000CB76D File Offset: 0x000C996D
		public static void OnPartyTreatedWell()
		{
			TraitLevelingHelper.AddPlayerTraitXPAndLogEntry(DefaultTraits.Generosity, 20, ActionNotes.PartyTakenCareOf, null);
		}

		// Token: 0x06003050 RID: 12368 RVA: 0x000CB77E File Offset: 0x000C997E
		public static void OnPartyStarved()
		{
			TraitLevelingHelper.AddPlayerTraitXPAndLogEntry(DefaultTraits.Generosity, -20, ActionNotes.PartyHungry, null);
		}

		// Token: 0x06003051 RID: 12369 RVA: 0x000CB790 File Offset: 0x000C9990
		public static void OnIssueFailed(Hero targetHero, Tuple<TraitObject, int>[] effectedTraits)
		{
			foreach (Tuple<TraitObject, int> tuple in effectedTraits)
			{
				TraitLevelingHelper.AddPlayerTraitXPAndLogEntry(tuple.Item1, tuple.Item2, ActionNotes.QuestFailed, targetHero);
			}
		}

		// Token: 0x06003052 RID: 12370 RVA: 0x000CB7C8 File Offset: 0x000C99C8
		public static void OnIssueSolvedThroughQuest(Hero targetHero, Tuple<TraitObject, int>[] effectedTraits)
		{
			foreach (Tuple<TraitObject, int> tuple in effectedTraits)
			{
				TraitLevelingHelper.AddPlayerTraitXPAndLogEntry(tuple.Item1, tuple.Item2, ActionNotes.QuestSuccess, targetHero);
			}
		}

		// Token: 0x06003053 RID: 12371 RVA: 0x000CB800 File Offset: 0x000C9A00
		public static void OnIssueSolvedThroughAlternativeSolution(Hero targetHero, Tuple<TraitObject, int>[] effectedTraits)
		{
			foreach (Tuple<TraitObject, int> tuple in effectedTraits)
			{
				TraitLevelingHelper.AddPlayerTraitXPAndLogEntry(tuple.Item1, tuple.Item2, ActionNotes.QuestSuccess, targetHero);
			}
		}

		// Token: 0x06003054 RID: 12372 RVA: 0x000CB838 File Offset: 0x000C9A38
		public static void OnIssueSolvedThroughBetrayal(Hero targetHero, Tuple<TraitObject, int>[] effectedTraits)
		{
			foreach (Tuple<TraitObject, int> tuple in effectedTraits)
			{
				TraitLevelingHelper.AddPlayerTraitXPAndLogEntry(tuple.Item1, tuple.Item2, ActionNotes.QuestBetrayal, targetHero);
			}
		}

		// Token: 0x06003055 RID: 12373 RVA: 0x000CB86D File Offset: 0x000C9A6D
		public static void OnLordFreed(Hero targetHero)
		{
			TraitLevelingHelper.AddPlayerTraitXPAndLogEntry(DefaultTraits.Calculating, 20, ActionNotes.NPCFreed, targetHero);
		}

		// Token: 0x06003056 RID: 12374 RVA: 0x000CB87E File Offset: 0x000C9A7E
		public static void OnPersuasionDefection(Hero targetHero)
		{
			TraitLevelingHelper.AddPlayerTraitXPAndLogEntry(DefaultTraits.Calculating, 20, ActionNotes.PersuadedToDefect, targetHero);
		}

		// Token: 0x06003057 RID: 12375 RVA: 0x000CB890 File Offset: 0x000C9A90
		public static void OnSiegeAftermathApplied(Settlement settlement, SiegeAftermathAction.SiegeAftermath aftermathType, TraitObject[] effectedTraits)
		{
			foreach (TraitObject traitObject in effectedTraits)
			{
				TraitLevelingHelper.AddPlayerTraitXPAndLogEntry(traitObject, Campaign.Current.Models.SiegeAftermathModel.GetSiegeAftermathTraitXpChangeForPlayer(traitObject, settlement, aftermathType), ActionNotes.SiegeAftermath, null);
			}
		}

		// Token: 0x04000FE0 RID: 4064
		private const int LordExecutedHonorPenalty = -1000;

		// Token: 0x04000FE1 RID: 4065
		private const int TroopsSacrificedValorPenalty = -30;

		// Token: 0x04000FE2 RID: 4066
		private const int VillageRaidedMercyPenalty = -30;

		// Token: 0x04000FE3 RID: 4067
		private const int PartyStarvingGenerosityPenalty = -20;

		// Token: 0x04000FE4 RID: 4068
		private const int PartyTreatedWellGenerosityBonus = 20;

		// Token: 0x04000FE5 RID: 4069
		private const int LordFreedCalculatingBonus = 20;

		// Token: 0x04000FE6 RID: 4070
		private const int PersuasionDefectionCalculatingBonus = 20;
	}
}
