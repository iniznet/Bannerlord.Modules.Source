using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem.Encyclopedia.Pages
{
	[EncyclopediaModel(new Type[] { typeof(Clan) })]
	public class DefaultEncyclopediaClanPage : EncyclopediaPage
	{
		public DefaultEncyclopediaClanPage()
		{
			base.HomePageOrderIndex = 500;
		}

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

		public override string GetViewFullyQualifiedName()
		{
			return "EncyclopediaClanPage";
		}

		public override TextObject GetName()
		{
			return GameTexts.FindText("str_clans", null);
		}

		public override TextObject GetDescriptionText()
		{
			return GameTexts.FindText("str_clan_description", null);
		}

		public override string GetStringID()
		{
			return "EncyclopediaClan";
		}

		public override MBObjectBase GetObject(string typeName, string stringID)
		{
			return Campaign.Current.CampaignObjectManager.Find<Clan>(stringID);
		}

		public override bool IsValidEncyclopediaItem(object o)
		{
			return o is IFaction;
		}

		private class EncyclopediaListClanWealthComparer : DefaultEncyclopediaClanPage.EncyclopediaListClanComparer
		{
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

			public override int Compare(EncyclopediaListItem x, EncyclopediaListItem y)
			{
				return base.CompareClans(x, y, DefaultEncyclopediaClanPage.EncyclopediaListClanWealthComparer._comparison);
			}

			public override string GetComparedValueText(EncyclopediaListItem item)
			{
				Clan clan;
				if ((clan = item.Object as Clan) != null)
				{
					return this.GetClanWealthStatusText(clan);
				}
				Debug.FailedAssert("Unable to get the gold of a non-clan object.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Encyclopedia\\Pages\\DefaultEncyclopediaClanPage.cs", "GetComparedValueText", 157);
				return "";
			}

			private static Func<Clan, Clan, int> _comparison = (Clan c1, Clan c2) => c1.Gold.CompareTo(c2.Gold);
		}

		private class EncyclopediaListClanTierComparer : DefaultEncyclopediaClanPage.EncyclopediaListClanComparer
		{
			public override int Compare(EncyclopediaListItem x, EncyclopediaListItem y)
			{
				return base.CompareClans(x, y, DefaultEncyclopediaClanPage.EncyclopediaListClanTierComparer._comparison);
			}

			public override string GetComparedValueText(EncyclopediaListItem item)
			{
				Clan clan;
				if ((clan = item.Object as Clan) != null)
				{
					return clan.Tier.ToString();
				}
				Debug.FailedAssert("Unable to get the tier of a non-clan object.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Encyclopedia\\Pages\\DefaultEncyclopediaClanPage.cs", "GetComparedValueText", 178);
				return "";
			}

			private static Func<Clan, Clan, int> _comparison = (Clan c1, Clan c2) => c1.Tier.CompareTo(c2.Tier);
		}

		private class EncyclopediaListClanStrengthComparer : DefaultEncyclopediaClanPage.EncyclopediaListClanComparer
		{
			public override int Compare(EncyclopediaListItem x, EncyclopediaListItem y)
			{
				return base.CompareClans(x, y, DefaultEncyclopediaClanPage.EncyclopediaListClanStrengthComparer._comparison);
			}

			public override string GetComparedValueText(EncyclopediaListItem item)
			{
				Clan clan;
				if ((clan = item.Object as Clan) != null)
				{
					return ((int)clan.TotalStrength).ToString();
				}
				Debug.FailedAssert("Unable to get the strength of a non-clan object.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Encyclopedia\\Pages\\DefaultEncyclopediaClanPage.cs", "GetComparedValueText", 199);
				return "";
			}

			private static Func<Clan, Clan, int> _comparison = (Clan c1, Clan c2) => c1.TotalStrength.CompareTo(c2.TotalStrength);
		}

		private class EncyclopediaListClanFiefComparer : DefaultEncyclopediaClanPage.EncyclopediaListClanComparer
		{
			public override int Compare(EncyclopediaListItem x, EncyclopediaListItem y)
			{
				return base.CompareClans(x, y, DefaultEncyclopediaClanPage.EncyclopediaListClanFiefComparer._comparison);
			}

			public override string GetComparedValueText(EncyclopediaListItem item)
			{
				Clan clan;
				if ((clan = item.Object as Clan) != null)
				{
					return clan.Fiefs.Count.ToString();
				}
				Debug.FailedAssert("Unable to get the fief count of a non-clan object.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Encyclopedia\\Pages\\DefaultEncyclopediaClanPage.cs", "GetComparedValueText", 220);
				return "";
			}

			private static Func<Clan, Clan, int> _comparison = (Clan c1, Clan c2) => c1.Fiefs.Count.CompareTo(c2.Fiefs.Count);
		}

		private class EncyclopediaListClanMemberComparer : DefaultEncyclopediaClanPage.EncyclopediaListClanComparer
		{
			public override int Compare(EncyclopediaListItem x, EncyclopediaListItem y)
			{
				return base.CompareClans(x, y, DefaultEncyclopediaClanPage.EncyclopediaListClanMemberComparer._comparison);
			}

			public override string GetComparedValueText(EncyclopediaListItem item)
			{
				Clan clan;
				if ((clan = item.Object as Clan) != null)
				{
					return clan.Heroes.Count.ToString();
				}
				Debug.FailedAssert("Unable to get members of a non-clan object.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Encyclopedia\\Pages\\DefaultEncyclopediaClanPage.cs", "GetComparedValueText", 241);
				return "";
			}

			private static Func<Clan, Clan, int> _comparison = (Clan c1, Clan c2) => c1.Heroes.Count.CompareTo(c2.Heroes.Count);
		}

		public abstract class EncyclopediaListClanComparer : EncyclopediaListItemComparerBase
		{
			public int CompareClans(EncyclopediaListItem x, EncyclopediaListItem y, Func<Clan, Clan, int> comparison)
			{
				Clan clan;
				Clan clan2;
				if ((clan = x.Object as Clan) == null || (clan2 = y.Object as Clan) == null)
				{
					Debug.FailedAssert("Both objects should be clans.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Encyclopedia\\Pages\\DefaultEncyclopediaClanPage.cs", "CompareClans", 256);
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
