using System;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.CharacterDevelopment
{
	public class TraitLevelingHelper
	{
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

		public static void OnTroopsSacrificed()
		{
			TraitLevelingHelper.AddPlayerTraitXPAndLogEntry(DefaultTraits.Valor, -30, ActionNotes.SacrificedTroops, null);
		}

		public static void OnLordExecuted()
		{
			TraitLevelingHelper.AddPlayerTraitXPAndLogEntry(DefaultTraits.Honor, -1000, ActionNotes.SacrificedTroops, null);
		}

		public static void OnVillageRaided()
		{
			TraitLevelingHelper.AddPlayerTraitXPAndLogEntry(DefaultTraits.Mercy, -30, ActionNotes.VillageRaid, null);
		}

		public static void OnHostileAction(int amount)
		{
			TraitLevelingHelper.AddPlayerTraitXPAndLogEntry(DefaultTraits.Honor, amount, ActionNotes.HostileAction, null);
			TraitLevelingHelper.AddPlayerTraitXPAndLogEntry(DefaultTraits.Mercy, amount, ActionNotes.HostileAction, null);
		}

		public static void OnPartyTreatedWell()
		{
			TraitLevelingHelper.AddPlayerTraitXPAndLogEntry(DefaultTraits.Generosity, 20, ActionNotes.PartyTakenCareOf, null);
		}

		public static void OnPartyStarved()
		{
			TraitLevelingHelper.AddPlayerTraitXPAndLogEntry(DefaultTraits.Generosity, -20, ActionNotes.PartyHungry, null);
		}

		public static void OnIssueFailed(Hero targetHero, Tuple<TraitObject, int>[] effectedTraits)
		{
			foreach (Tuple<TraitObject, int> tuple in effectedTraits)
			{
				TraitLevelingHelper.AddPlayerTraitXPAndLogEntry(tuple.Item1, tuple.Item2, ActionNotes.QuestFailed, targetHero);
			}
		}

		public static void OnIssueSolvedThroughQuest(Hero targetHero, Tuple<TraitObject, int>[] effectedTraits)
		{
			foreach (Tuple<TraitObject, int> tuple in effectedTraits)
			{
				TraitLevelingHelper.AddPlayerTraitXPAndLogEntry(tuple.Item1, tuple.Item2, ActionNotes.QuestSuccess, targetHero);
			}
		}

		public static void OnIssueSolvedThroughAlternativeSolution(Hero targetHero, Tuple<TraitObject, int>[] effectedTraits)
		{
			foreach (Tuple<TraitObject, int> tuple in effectedTraits)
			{
				TraitLevelingHelper.AddPlayerTraitXPAndLogEntry(tuple.Item1, tuple.Item2, ActionNotes.QuestSuccess, targetHero);
			}
		}

		public static void OnIssueSolvedThroughBetrayal(Hero targetHero, Tuple<TraitObject, int>[] effectedTraits)
		{
			foreach (Tuple<TraitObject, int> tuple in effectedTraits)
			{
				TraitLevelingHelper.AddPlayerTraitXPAndLogEntry(tuple.Item1, tuple.Item2, ActionNotes.QuestBetrayal, targetHero);
			}
		}

		public static void OnLordFreed(Hero targetHero)
		{
			TraitLevelingHelper.AddPlayerTraitXPAndLogEntry(DefaultTraits.Calculating, 20, ActionNotes.NPCFreed, targetHero);
		}

		public static void OnPersuasionDefection(Hero targetHero)
		{
			TraitLevelingHelper.AddPlayerTraitXPAndLogEntry(DefaultTraits.Calculating, 20, ActionNotes.PersuadedToDefect, targetHero);
		}

		public static void OnSiegeAftermathApplied(Settlement settlement, SiegeAftermathAction.SiegeAftermath aftermathType, TraitObject[] effectedTraits)
		{
			foreach (TraitObject traitObject in effectedTraits)
			{
				TraitLevelingHelper.AddPlayerTraitXPAndLogEntry(traitObject, Campaign.Current.Models.SiegeAftermathModel.GetSiegeAftermathTraitXpChangeForPlayer(traitObject, settlement, aftermathType), ActionNotes.SiegeAftermath, null);
			}
		}

		private const int LordExecutedHonorPenalty = -1000;

		private const int TroopsSacrificedValorPenalty = -30;

		private const int VillageRaidedMercyPenalty = -30;

		private const int PartyStarvingGenerosityPenalty = -20;

		private const int PartyTreatedWellGenerosityBonus = 20;

		private const int LordFreedCalculatingBonus = 20;

		private const int PersuasionDefectionCalculatingBonus = 20;
	}
}
