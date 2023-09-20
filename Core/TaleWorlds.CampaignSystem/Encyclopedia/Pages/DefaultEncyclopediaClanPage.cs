using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem.Encyclopedia.Pages
{
	// Token: 0x02000165 RID: 357
	[EncyclopediaModel(new Type[] { typeof(Clan) })]
	public class DefaultEncyclopediaClanPage : EncyclopediaPage
	{
		// Token: 0x06001887 RID: 6279 RVA: 0x0007BBAF File Offset: 0x00079DAF
		public DefaultEncyclopediaClanPage()
		{
			base.HomePageOrderIndex = 500;
		}

		// Token: 0x06001888 RID: 6280 RVA: 0x0007BBC2 File Offset: 0x00079DC2
		protected override IEnumerable<EncyclopediaListItem> InitializeListItems()
		{
			foreach (Clan clan in Clan.NonBanditFactions)
			{
				if (this.IsValidEncyclopediaItem(clan))
				{
					yield return new EncyclopediaListItem(clan, clan.Name.ToString(), "", clan.StringId, base.GetIdentifier(typeof(Clan)), true, null);
				}
			}
			IEnumerator<Clan> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x06001889 RID: 6281 RVA: 0x0007BBD4 File Offset: 0x00079DD4
		protected override IEnumerable<EncyclopediaFilterGroup> InitializeFilterItems()
		{
			List<EncyclopediaFilterGroup> list = new List<EncyclopediaFilterGroup>();
			List<EncyclopediaFilterItem> list2 = new List<EncyclopediaFilterItem>();
			list2.Add(new EncyclopediaFilterItem(new TextObject("{=QwpHoMJu}Minor", null), (object f) => ((IFaction)f).IsMinorFaction));
			list.Add(new EncyclopediaFilterGroup(list2, new TextObject("{=zMMqgxb1}Type", null)));
			List<EncyclopediaFilterItem> list3 = new List<EncyclopediaFilterItem>();
			list3.Add(new EncyclopediaFilterItem(new TextObject("{=lEHjxPTs}Ally", null), (object f) => FactionManager.IsAlliedWithFaction((IFaction)f, Hero.MainHero.MapFaction)));
			list3.Add(new EncyclopediaFilterItem(new TextObject("{=sPmQz21k}Enemy", null), (object f) => FactionManager.IsAtWarAgainstFaction((IFaction)f, Hero.MainHero.MapFaction) && !((IFaction)f).IsBanditFaction));
			list3.Add(new EncyclopediaFilterItem(new TextObject("{=3PzgpFGq}Neutral", null), (object f) => FactionManager.IsNeutralWithFaction((IFaction)f, Hero.MainHero.MapFaction)));
			list.Add(new EncyclopediaFilterGroup(list3, new TextObject("{=L7wn49Uz}Diplomacy", null)));
			List<EncyclopediaFilterItem> list4 = new List<EncyclopediaFilterItem>();
			list4.Add(new EncyclopediaFilterItem(new TextObject("{=SlubkZ1A}Eliminated", null), (object f) => ((IFaction)f).IsEliminated));
			list4.Add(new EncyclopediaFilterItem(new TextObject("{=YRbSBxqT}Active", null), (object f) => !((IFaction)f).IsEliminated));
			list.Add(new EncyclopediaFilterGroup(list4, new TextObject("{=DXczLzml}Status", null)));
			List<EncyclopediaFilterItem> list5 = new List<EncyclopediaFilterItem>();
			using (List<CultureObject>.Enumerator enumerator = (from x in Game.Current.ObjectManager.GetObjectTypeList<CultureObject>()
				orderby !x.IsMainCulture descending
				select x).ThenBy((CultureObject f) => f.Name.ToString()).ToList<CultureObject>().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					CultureObject culture = enumerator.Current;
					if (culture.StringId != "neutral_culture" && !culture.IsBandit)
					{
						list5.Add(new EncyclopediaFilterItem(culture.Name, (object c) => ((IFaction)c).Culture == culture));
					}
				}
			}
			list.Add(new EncyclopediaFilterGroup(list5, GameTexts.FindText("str_culture", null)));
			return list;
		}

		// Token: 0x0600188A RID: 6282 RVA: 0x0007BE8C File Offset: 0x0007A08C
		protected override IEnumerable<EncyclopediaSortController> InitializeSortControllers()
		{
			return new List<EncyclopediaSortController>
			{
				new EncyclopediaSortController(new TextObject("{=qtII2HbK}Wealth", null), new DefaultEncyclopediaClanPage.EncyclopediaListClanWealthComparer()),
				new EncyclopediaSortController(new TextObject("{=cc1d7mkq}Tier", null), new DefaultEncyclopediaClanPage.EncyclopediaListClanTierComparer()),
				new EncyclopediaSortController(GameTexts.FindText("str_strength", null), new DefaultEncyclopediaClanPage.EncyclopediaListClanStrengthComparer()),
				new EncyclopediaSortController(GameTexts.FindText("str_fiefs", null), new DefaultEncyclopediaClanPage.EncyclopediaListClanFiefComparer()),
				new EncyclopediaSortController(GameTexts.FindText("str_members", null), new DefaultEncyclopediaClanPage.EncyclopediaListClanMemberComparer())
			};
		}

		// Token: 0x0600188B RID: 6283 RVA: 0x0007BF25 File Offset: 0x0007A125
		public override string GetViewFullyQualifiedName()
		{
			return "EncyclopediaClanPage";
		}

		// Token: 0x0600188C RID: 6284 RVA: 0x0007BF2C File Offset: 0x0007A12C
		public override TextObject GetName()
		{
			return GameTexts.FindText("str_clans", null);
		}

		// Token: 0x0600188D RID: 6285 RVA: 0x0007BF39 File Offset: 0x0007A139
		public override TextObject GetDescriptionText()
		{
			return GameTexts.FindText("str_clan_description", null);
		}

		// Token: 0x0600188E RID: 6286 RVA: 0x0007BF46 File Offset: 0x0007A146
		public override string GetStringID()
		{
			return "EncyclopediaClan";
		}

		// Token: 0x0600188F RID: 6287 RVA: 0x0007BF4D File Offset: 0x0007A14D
		public override MBObjectBase GetObject(string typeName, string stringID)
		{
			return Campaign.Current.CampaignObjectManager.Find<Clan>(stringID);
		}

		// Token: 0x06001890 RID: 6288 RVA: 0x0007BF60 File Offset: 0x0007A160
		public override bool IsValidEncyclopediaItem(object o)
		{
			IFaction faction = o as IFaction;
			return faction != null && (!faction.IsClan || (faction.IsClan && !((Clan)faction).IsNeutralClan));
		}

		// Token: 0x02000527 RID: 1319
		private class EncyclopediaListClanWealthComparer : DefaultEncyclopediaClanPage.EncyclopediaListClanComparer
		{
			// Token: 0x060042A8 RID: 17064 RVA: 0x00135A54 File Offset: 0x00133C54
			private string GetClanWealthStatusText(Clan _clan)
			{
				string text = string.Empty;
				if (_clan.Leader.Gold < 15000)
				{
					text = new TextObject("{=SixPXaNh}Very Poor", null).ToString();
				}
				else if (_clan.Leader.Gold < 45000)
				{
					text = new TextObject("{=poorWealthStatus}Poor", null).ToString();
				}
				else if (_clan.Leader.Gold < 135000)
				{
					text = new TextObject("{=averageWealthStatus}Average", null).ToString();
				}
				else if (_clan.Leader.Gold < 405000)
				{
					text = new TextObject("{=UbRqC0Yz}Rich", null).ToString();
				}
				else
				{
					text = new TextObject("{=oJmRg2ms}Very Rich", null).ToString();
				}
				return text;
			}

			// Token: 0x060042A9 RID: 17065 RVA: 0x00135B10 File Offset: 0x00133D10
			public override int Compare(EncyclopediaListItem x, EncyclopediaListItem y)
			{
				return base.CompareClans(x, y, DefaultEncyclopediaClanPage.EncyclopediaListClanWealthComparer._comparison);
			}

			// Token: 0x060042AA RID: 17066 RVA: 0x00135B20 File Offset: 0x00133D20
			public override string GetComparedValueText(EncyclopediaListItem item)
			{
				Clan clan;
				if ((clan = item.Object as Clan) != null)
				{
					return this.GetClanWealthStatusText(clan);
				}
				Debug.FailedAssert("Unable to get the gold of a non-clan object.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Encyclopedia\\Pages\\DefaultEncyclopediaClanPage.cs", "GetComparedValueText", 158);
				return "";
			}

			// Token: 0x040015FC RID: 5628
			private static Func<Clan, Clan, int> _comparison = (Clan c1, Clan c2) => c1.Gold.CompareTo(c2.Gold);
		}

		// Token: 0x02000528 RID: 1320
		private class EncyclopediaListClanTierComparer : DefaultEncyclopediaClanPage.EncyclopediaListClanComparer
		{
			// Token: 0x060042AD RID: 17069 RVA: 0x00135B81 File Offset: 0x00133D81
			public override int Compare(EncyclopediaListItem x, EncyclopediaListItem y)
			{
				return base.CompareClans(x, y, DefaultEncyclopediaClanPage.EncyclopediaListClanTierComparer._comparison);
			}

			// Token: 0x060042AE RID: 17070 RVA: 0x00135B90 File Offset: 0x00133D90
			public override string GetComparedValueText(EncyclopediaListItem item)
			{
				Clan clan;
				if ((clan = item.Object as Clan) != null)
				{
					return clan.Tier.ToString();
				}
				Debug.FailedAssert("Unable to get the tier of a non-clan object.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Encyclopedia\\Pages\\DefaultEncyclopediaClanPage.cs", "GetComparedValueText", 179);
				return "";
			}

			// Token: 0x040015FD RID: 5629
			private static Func<Clan, Clan, int> _comparison = (Clan c1, Clan c2) => c1.Tier.CompareTo(c2.Tier);
		}

		// Token: 0x02000529 RID: 1321
		private class EncyclopediaListClanStrengthComparer : DefaultEncyclopediaClanPage.EncyclopediaListClanComparer
		{
			// Token: 0x060042B1 RID: 17073 RVA: 0x00135BF8 File Offset: 0x00133DF8
			public override int Compare(EncyclopediaListItem x, EncyclopediaListItem y)
			{
				return base.CompareClans(x, y, DefaultEncyclopediaClanPage.EncyclopediaListClanStrengthComparer._comparison);
			}

			// Token: 0x060042B2 RID: 17074 RVA: 0x00135C08 File Offset: 0x00133E08
			public override string GetComparedValueText(EncyclopediaListItem item)
			{
				Clan clan;
				if ((clan = item.Object as Clan) != null)
				{
					return ((int)clan.TotalStrength).ToString();
				}
				Debug.FailedAssert("Unable to get the strength of a non-clan object.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Encyclopedia\\Pages\\DefaultEncyclopediaClanPage.cs", "GetComparedValueText", 200);
				return "";
			}

			// Token: 0x040015FE RID: 5630
			private static Func<Clan, Clan, int> _comparison = (Clan c1, Clan c2) => c1.TotalStrength.CompareTo(c2.TotalStrength);
		}

		// Token: 0x0200052A RID: 1322
		private class EncyclopediaListClanFiefComparer : DefaultEncyclopediaClanPage.EncyclopediaListClanComparer
		{
			// Token: 0x060042B5 RID: 17077 RVA: 0x00135C71 File Offset: 0x00133E71
			public override int Compare(EncyclopediaListItem x, EncyclopediaListItem y)
			{
				return base.CompareClans(x, y, DefaultEncyclopediaClanPage.EncyclopediaListClanFiefComparer._comparison);
			}

			// Token: 0x060042B6 RID: 17078 RVA: 0x00135C80 File Offset: 0x00133E80
			public override string GetComparedValueText(EncyclopediaListItem item)
			{
				Clan clan;
				if ((clan = item.Object as Clan) != null)
				{
					return clan.Fiefs.Count.ToString();
				}
				Debug.FailedAssert("Unable to get the fief count of a non-clan object.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Encyclopedia\\Pages\\DefaultEncyclopediaClanPage.cs", "GetComparedValueText", 221);
				return "";
			}

			// Token: 0x040015FF RID: 5631
			private static Func<Clan, Clan, int> _comparison = (Clan c1, Clan c2) => c1.Fiefs.Count.CompareTo(c2.Fiefs.Count);
		}

		// Token: 0x0200052B RID: 1323
		private class EncyclopediaListClanMemberComparer : DefaultEncyclopediaClanPage.EncyclopediaListClanComparer
		{
			// Token: 0x060042B9 RID: 17081 RVA: 0x00135CED File Offset: 0x00133EED
			public override int Compare(EncyclopediaListItem x, EncyclopediaListItem y)
			{
				return base.CompareClans(x, y, DefaultEncyclopediaClanPage.EncyclopediaListClanMemberComparer._comparison);
			}

			// Token: 0x060042BA RID: 17082 RVA: 0x00135CFC File Offset: 0x00133EFC
			public override string GetComparedValueText(EncyclopediaListItem item)
			{
				Clan clan;
				if ((clan = item.Object as Clan) != null)
				{
					return clan.Heroes.Count.ToString();
				}
				Debug.FailedAssert("Unable to get members of a non-clan object.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Encyclopedia\\Pages\\DefaultEncyclopediaClanPage.cs", "GetComparedValueText", 242);
				return "";
			}

			// Token: 0x04001600 RID: 5632
			private static Func<Clan, Clan, int> _comparison = (Clan c1, Clan c2) => c1.Heroes.Count.CompareTo(c2.Heroes.Count);
		}

		// Token: 0x0200052C RID: 1324
		public abstract class EncyclopediaListClanComparer : EncyclopediaListItemComparerBase
		{
			// Token: 0x060042BD RID: 17085 RVA: 0x00135D6C File Offset: 0x00133F6C
			public int CompareClans(EncyclopediaListItem x, EncyclopediaListItem y, Func<Clan, Clan, int> comparison)
			{
				Clan clan;
				Clan clan2;
				if ((clan = x.Object as Clan) == null || (clan2 = y.Object as Clan) == null)
				{
					Debug.FailedAssert("Both objects should be clans.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Encyclopedia\\Pages\\DefaultEncyclopediaClanPage.cs", "CompareClans", 257);
					return 0;
				}
				int num = comparison(clan, clan2) * (base.IsAscending ? 1 : (-1));
				if (num == 0)
				{
					return base.ResolveEquality(x, y);
				}
				return num;
			}
		}
	}
}
