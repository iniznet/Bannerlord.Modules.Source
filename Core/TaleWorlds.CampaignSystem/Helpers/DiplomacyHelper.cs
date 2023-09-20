using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Helpers
{
	// Token: 0x02000005 RID: 5
	public static class DiplomacyHelper
	{
		// Token: 0x06000010 RID: 16 RVA: 0x00002E24 File Offset: 0x00001024
		public static bool IsWarCausedByPlayer(IFaction faction1, IFaction faction2, DeclareWarAction.DeclareWarDetail declareWarDetail)
		{
			switch (declareWarDetail)
			{
			case DeclareWarAction.DeclareWarDetail.CausedByPlayerHostility:
				return true;
			case DeclareWarAction.DeclareWarDetail.CausedByKingdomDecision:
				return faction1 == Hero.MainHero.MapFaction && Hero.MainHero.MapFaction.Leader == Hero.MainHero;
			case DeclareWarAction.DeclareWarDetail.CausedByCrimeRatingChange:
				return faction2 == Hero.MainHero.MapFaction && faction2.MainHeroCrimeRating > (float)Campaign.Current.Models.CrimeModel.DeclareWarCrimeRatingThreshold;
			case DeclareWarAction.DeclareWarDetail.CausedByKingdomCreation:
				return faction1 == Hero.MainHero.MapFaction;
			}
			return false;
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002EB8 File Offset: 0x000010B8
		private static bool IsLogInTimeRange(LogEntry entry, CampaignTime time)
		{
			return entry.GameTime.NumTicks >= time.NumTicks;
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00002EE0 File Offset: 0x000010E0
		public static List<ValueTuple<LogEntry, IFaction, IFaction>> GetLogsForWar(StanceLink stance)
		{
			CampaignTime warStartDate = stance.WarStartDate;
			List<ValueTuple<LogEntry, IFaction, IFaction>> list = new List<ValueTuple<LogEntry, IFaction, IFaction>>();
			for (int i = Campaign.Current.LogEntryHistory.GameActionLogs.Count - 1; i >= 0; i--)
			{
				LogEntry logEntry = Campaign.Current.LogEntryHistory.GameActionLogs[i];
				IWarLog warLog;
				IFaction faction;
				IFaction faction2;
				if (DiplomacyHelper.IsLogInTimeRange(logEntry, warStartDate) && (warLog = logEntry as IWarLog) != null && warLog.IsRelatedToWar(stance, out faction, out faction2))
				{
					list.Add(new ValueTuple<LogEntry, IFaction, IFaction>(logEntry, faction, faction2));
				}
			}
			return list;
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002F64 File Offset: 0x00001164
		public static List<Settlement> GetSuccessfullSiegesInWarForFaction(IFaction capturerFaction, StanceLink stance, Func<Settlement, bool> condition = null)
		{
			CampaignTime warStartDate = stance.WarStartDate;
			List<Settlement> list = new List<Settlement>();
			for (int i = Campaign.Current.LogEntryHistory.GameActionLogs.Count - 1; i >= 0; i--)
			{
				LogEntry logEntry = Campaign.Current.LogEntryHistory.GameActionLogs[i];
				ChangeSettlementOwnerLogEntry changeSettlementOwnerLogEntry;
				IFaction faction;
				IFaction faction2;
				if (DiplomacyHelper.IsLogInTimeRange(logEntry, warStartDate) && (changeSettlementOwnerLogEntry = logEntry as ChangeSettlementOwnerLogEntry) != null && (condition == null || condition(changeSettlementOwnerLogEntry.Settlement)) && !list.Contains(changeSettlementOwnerLogEntry.Settlement) && changeSettlementOwnerLogEntry.IsRelatedToWar(stance, out faction, out faction2) && faction == capturerFaction)
				{
					list.Add(changeSettlementOwnerLogEntry.Settlement);
				}
			}
			return list;
		}

		// Token: 0x06000014 RID: 20 RVA: 0x0000300C File Offset: 0x0000120C
		public static List<Settlement> GetRaidsInWar(IFaction faction, StanceLink stance, Func<Settlement, bool> condition = null)
		{
			CampaignTime warStartDate = stance.WarStartDate;
			List<Settlement> list = new List<Settlement>();
			for (int i = Campaign.Current.LogEntryHistory.GameActionLogs.Count - 1; i >= 0; i--)
			{
				LogEntry logEntry = Campaign.Current.LogEntryHistory.GameActionLogs[i];
				VillageStateChangedLogEntry villageStateChangedLogEntry;
				IFaction faction2;
				IFaction faction3;
				if (DiplomacyHelper.IsLogInTimeRange(logEntry, warStartDate) && (villageStateChangedLogEntry = logEntry as VillageStateChangedLogEntry) != null && (condition == null || condition(villageStateChangedLogEntry.Village.Settlement)) && villageStateChangedLogEntry.IsRelatedToWar(stance, out faction2, out faction3) && faction2 == faction && !list.Contains(villageStateChangedLogEntry.Village.Settlement))
				{
					list.Add(villageStateChangedLogEntry.Village.Settlement);
				}
			}
			return list;
		}

		// Token: 0x06000015 RID: 21 RVA: 0x000030C8 File Offset: 0x000012C8
		public static List<Hero> GetPrisonersOfWarTakenByFaction(IFaction capturerFaction, IFaction prisonerFaction)
		{
			List<Hero> list = new List<Hero>();
			foreach (Hero hero in prisonerFaction.Lords)
			{
				if (hero.IsPrisoner)
				{
					PartyBase partyBelongedToAsPrisoner = hero.PartyBelongedToAsPrisoner;
					if (((partyBelongedToAsPrisoner != null) ? partyBelongedToAsPrisoner.MapFaction : null) == capturerFaction)
					{
						list.Add(hero);
					}
				}
			}
			return list;
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00003140 File Offset: 0x00001340
		public static bool DidMainHeroSwornNotToAttackFaction(IFaction faction, out TextObject explanation)
		{
			explanation = TextObject.Empty;
			if (faction.NotAttackableByPlayerUntilTime.IsFuture)
			{
				explanation = GameTexts.FindText("str_enemy_not_attackable_tooltip", null);
				return true;
			}
			return false;
		}
	}
}
