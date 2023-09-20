using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem.Actions
{
	public static class DeclareWarAction
	{
		private static void ApplyInternal(IFaction faction1, IFaction faction2, DeclareWarAction.DeclareWarDetail declareWarDetail)
		{
			FactionManager.DeclareWar(faction1, faction2, false);
			if (faction1.IsKingdomFaction && (float)faction2.Fiefs.Count > 1f + (float)faction1.Fiefs.Count * 0.2f)
			{
				Kingdom kingdom = (Kingdom)faction1;
				kingdom.PoliticalStagnation = (int)((float)kingdom.PoliticalStagnation * 0.85f - 3f);
				if (kingdom.PoliticalStagnation < 0)
				{
					kingdom.PoliticalStagnation = 0;
				}
			}
			if (faction2.IsKingdomFaction && (float)faction1.Fiefs.Count > 1f + (float)faction2.Fiefs.Count * 0.2f)
			{
				Kingdom kingdom2 = (Kingdom)faction2;
				kingdom2.PoliticalStagnation = (int)((float)kingdom2.PoliticalStagnation * 0.85f - 3f);
				if (kingdom2.PoliticalStagnation < 0)
				{
					kingdom2.PoliticalStagnation = 0;
				}
			}
			if (faction1 == Hero.MainHero.MapFaction || faction2 == Hero.MainHero.MapFaction)
			{
				IFaction dirtySide = ((faction1 == Hero.MainHero.MapFaction) ? faction2 : faction1);
				IEnumerable<Settlement> all = Settlement.All;
				Func<Settlement, bool> func;
				Func<Settlement, bool> <>9__0;
				if ((func = <>9__0) == null)
				{
					func = (<>9__0 = (Settlement party) => party.IsVisible && party.MapFaction == dirtySide);
				}
				foreach (Settlement settlement in all.Where(func))
				{
					settlement.Party.Visuals.SetMapIconAsDirty();
				}
				IEnumerable<MobileParty> all2 = MobileParty.All;
				Func<MobileParty, bool> func2;
				Func<MobileParty, bool> <>9__1;
				if ((func2 = <>9__1) == null)
				{
					func2 = (<>9__1 = (MobileParty party) => party.IsVisible && party.MapFaction == dirtySide);
				}
				foreach (MobileParty mobileParty in all2.Where(func2))
				{
					mobileParty.Party.Visuals.SetMapIconAsDirty();
				}
			}
			CampaignEventDispatcher.Instance.OnWarDeclared(faction1, faction2, declareWarDetail);
		}

		public static void ApplyByKingdomDecision(IFaction faction1, IFaction faction2)
		{
			DeclareWarAction.ApplyInternal(faction1, faction2, DeclareWarAction.DeclareWarDetail.CausedByKingdomDecision);
		}

		public static void ApplyByDefault(IFaction faction1, IFaction faction2)
		{
			DeclareWarAction.ApplyInternal(faction1, faction2, DeclareWarAction.DeclareWarDetail.Default);
		}

		public static void ApplyByPlayerHostility(IFaction faction1, IFaction faction2)
		{
			DeclareWarAction.ApplyInternal(faction1, faction2, DeclareWarAction.DeclareWarDetail.CausedByPlayerHostility);
		}

		public static void ApplyByRebellion(IFaction faction1, IFaction faction2)
		{
			DeclareWarAction.ApplyInternal(faction1, faction2, DeclareWarAction.DeclareWarDetail.CausedByRebellion);
		}

		public static void ApplyByCrimeRatingChange(IFaction faction1, IFaction faction2)
		{
			DeclareWarAction.ApplyInternal(faction1, faction2, DeclareWarAction.DeclareWarDetail.CausedByCrimeRatingChange);
		}

		public static void ApplyByKingdomCreation(IFaction faction1, IFaction faction2)
		{
			DeclareWarAction.ApplyInternal(faction1, faction2, DeclareWarAction.DeclareWarDetail.CausedByKingdomCreation);
		}

		public enum DeclareWarDetail
		{
			Default,
			CausedByPlayerHostility,
			CausedByKingdomDecision,
			CausedByRebellion,
			CausedByCrimeRatingChange,
			CausedByKingdomCreation
		}
	}
}
