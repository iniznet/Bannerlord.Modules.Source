using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem.Actions
{
	public static class MakePeaceAction
	{
		private static void ApplyInternal(IFaction faction1, IFaction faction2, int dailyTributeFrom1To2, MakePeaceAction.MakePeaceDetail detail = MakePeaceAction.MakePeaceDetail.Default)
		{
			StanceLink stanceWith = faction1.GetStanceWith(faction2);
			stanceWith.StanceType = StanceType.Neutral;
			stanceWith.SetDailyTributePaid(faction1, dailyTributeFrom1To2);
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
			CampaignEventDispatcher.Instance.OnMakePeace(faction1, faction2, detail);
		}

		public static void ApplyPardonPlayer(IFaction faction)
		{
			MakePeaceAction.ApplyInternal(faction, Hero.MainHero.MapFaction, 0, MakePeaceAction.MakePeaceDetail.Default);
		}

		public static void Apply(IFaction faction1, IFaction faction2, int dailyTributeFrom1To2 = 0)
		{
			MakePeaceAction.ApplyInternal(faction1, faction2, dailyTributeFrom1To2, MakePeaceAction.MakePeaceDetail.Default);
		}

		public static void ApplyByKingdomDecision(IFaction faction1, IFaction faction2, int dailyTributeFrom1To2 = 0)
		{
			MakePeaceAction.ApplyInternal(faction1, faction2, dailyTributeFrom1To2, MakePeaceAction.MakePeaceDetail.ByKingdomDecision);
		}

		private const float DefaultValueForBeingLimitedAfterPeace = 100000f;

		public enum MakePeaceDetail
		{
			Default,
			ByKingdomDecision
		}
	}
}
