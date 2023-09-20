using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem.Actions
{
	// Token: 0x0200044C RID: 1100
	public static class MakePeaceAction
	{
		// Token: 0x06003F33 RID: 16179 RVA: 0x0012E618 File Offset: 0x0012C818
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

		// Token: 0x06003F34 RID: 16180 RVA: 0x0012E760 File Offset: 0x0012C960
		public static void ApplyPardonPlayer(IFaction faction)
		{
			MakePeaceAction.ApplyInternal(faction, Hero.MainHero.MapFaction, 0, MakePeaceAction.MakePeaceDetail.Default);
		}

		// Token: 0x06003F35 RID: 16181 RVA: 0x0012E774 File Offset: 0x0012C974
		public static void Apply(IFaction faction1, IFaction faction2, int dailyTributeFrom1To2 = 0)
		{
			MakePeaceAction.ApplyInternal(faction1, faction2, dailyTributeFrom1To2, MakePeaceAction.MakePeaceDetail.Default);
		}

		// Token: 0x06003F36 RID: 16182 RVA: 0x0012E77F File Offset: 0x0012C97F
		public static void ApplyByKingdomDecision(IFaction faction1, IFaction faction2, int dailyTributeFrom1To2 = 0)
		{
			MakePeaceAction.ApplyInternal(faction1, faction2, dailyTributeFrom1To2, MakePeaceAction.MakePeaceDetail.ByKingdomDecision);
		}

		// Token: 0x040012CA RID: 4810
		private const float DefaultValueForBeingLimitedAfterPeace = 100000f;

		// Token: 0x02000765 RID: 1893
		public enum MakePeaceDetail
		{
			// Token: 0x04001E65 RID: 7781
			Default,
			// Token: 0x04001E66 RID: 7782
			ByKingdomDecision
		}
	}
}
