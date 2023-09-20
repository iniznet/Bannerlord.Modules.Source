using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem.Encyclopedia.Pages
{
	[EncyclopediaModel(new Type[] { typeof(Kingdom) })]
	public class DefaultEncyclopediaFactionPage : EncyclopediaPage
	{
		public DefaultEncyclopediaFactionPage()
		{
			base.HomePageOrderIndex = 400;
		}

		public override string GetViewFullyQualifiedName()
		{
			return "EncyclopediaFactionPage";
		}

		public override TextObject GetName()
		{
			return GameTexts.FindText("str_kingdoms_group", null);
		}

		public override TextObject GetDescriptionText()
		{
			return GameTexts.FindText("str_faction_description", null);
		}

		public override string GetStringID()
		{
			return "EncyclopediaKingdom";
		}

		public override MBObjectBase GetObject(string typeName, string stringID)
		{
			return Campaign.Current.CampaignObjectManager.Find<Kingdom>(stringID);
		}

		public override bool IsValidEncyclopediaItem(object o)
		{
			IFaction faction = o as IFaction;
			return faction != null && !faction.IsBanditFaction;
		}

		protected override IEnumerable<EncyclopediaListItem> InitializeListItems()
		{
			foreach (Kingdom kingdom in Kingdom.All)
			{
				if (this.IsValidEncyclopediaItem(kingdom))
				{
					yield return new EncyclopediaListItem(kingdom, kingdom.Name.ToString(), "", kingdom.StringId, base.GetIdentifier(typeof(Kingdom)), true, null);
				}
			}
			List<Kingdom>.Enumerator enumerator = default(List<Kingdom>.Enumerator);
			yield break;
			yield break;
		}

		protected override IEnumerable<EncyclopediaFilterGroup> InitializeFilterItems()
		{
			List<EncyclopediaFilterGroup> list = new List<EncyclopediaFilterGroup>();
			List<EncyclopediaFilterItem> list2 = new List<EncyclopediaFilterItem>();
			list2 = new List<EncyclopediaFilterItem>();
			list2.Add(new EncyclopediaFilterItem(new TextObject("{=lEHjxPTs}Ally", null), (object f) => FactionManager.IsAlliedWithFaction((IFaction)f, Hero.MainHero.MapFaction)));
			list2.Add(new EncyclopediaFilterItem(new TextObject("{=sPmQz21k}Enemy", null), (object f) => FactionManager.IsAtWarAgainstFaction((IFaction)f, Hero.MainHero.MapFaction) && !((IFaction)f).IsBanditFaction));
			list2.Add(new EncyclopediaFilterItem(new TextObject("{=3PzgpFGq}Neutral", null), (object f) => FactionManager.IsNeutralWithFaction((IFaction)f, Hero.MainHero.MapFaction)));
			list.Add(new EncyclopediaFilterGroup(list2, new TextObject("{=L7wn49Uz}Diplomacy", null)));
			return list;
		}

		protected override IEnumerable<EncyclopediaSortController> InitializeSortControllers()
		{
			return new List<EncyclopediaSortController>
			{
				new EncyclopediaSortController(GameTexts.FindText("str_total_strength", null), new DefaultEncyclopediaFactionPage.EncyclopediaListKingdomTotalStrengthComparer()),
				new EncyclopediaSortController(GameTexts.FindText("str_fiefs", null), new DefaultEncyclopediaFactionPage.EncyclopediaListKingdomFiefsComparer()),
				new EncyclopediaSortController(GameTexts.FindText("str_clans", null), new DefaultEncyclopediaFactionPage.EncyclopediaListKingdomClanComparer())
			};
		}

		private class EncyclopediaListKingdomTotalStrengthComparer : DefaultEncyclopediaFactionPage.EncyclopediaListKingdomComparer
		{
			public override int Compare(EncyclopediaListItem x, EncyclopediaListItem y)
			{
				return base.CompareKingdoms(x, y, DefaultEncyclopediaFactionPage.EncyclopediaListKingdomTotalStrengthComparer._comparison);
			}

			public override string GetComparedValueText(EncyclopediaListItem item)
			{
				Kingdom kingdom;
				if ((kingdom = item.Object as Kingdom) != null)
				{
					return ((int)kingdom.TotalStrength).ToString();
				}
				Debug.FailedAssert("Unable to get the total strength of a non-kingdom object.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Encyclopedia\\Pages\\DefaultEncyclopediaFactionPage.cs", "GetComparedValueText", 107);
				return "";
			}

			private static Func<Kingdom, Kingdom, int> _comparison = (Kingdom k1, Kingdom k2) => k1.TotalStrength.CompareTo(k2.TotalStrength);
		}

		private class EncyclopediaListKingdomFiefsComparer : DefaultEncyclopediaFactionPage.EncyclopediaListKingdomComparer
		{
			public override int Compare(EncyclopediaListItem x, EncyclopediaListItem y)
			{
				return base.CompareKingdoms(x, y, DefaultEncyclopediaFactionPage.EncyclopediaListKingdomFiefsComparer._comparison);
			}

			public override string GetComparedValueText(EncyclopediaListItem item)
			{
				Kingdom kingdom;
				if ((kingdom = item.Object as Kingdom) != null)
				{
					return kingdom.Fiefs.Count.ToString();
				}
				Debug.FailedAssert("Unable to get the fief count from a non-kingdom object.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Encyclopedia\\Pages\\DefaultEncyclopediaFactionPage.cs", "GetComparedValueText", 128);
				return "";
			}

			private static Func<Kingdom, Kingdom, int> _comparison = (Kingdom k1, Kingdom k2) => k1.Fiefs.Count.CompareTo(k2.Fiefs.Count);
		}

		private class EncyclopediaListKingdomClanComparer : DefaultEncyclopediaFactionPage.EncyclopediaListKingdomComparer
		{
			public override int Compare(EncyclopediaListItem x, EncyclopediaListItem y)
			{
				return base.CompareKingdoms(x, y, DefaultEncyclopediaFactionPage.EncyclopediaListKingdomClanComparer._comparison);
			}

			public override string GetComparedValueText(EncyclopediaListItem item)
			{
				Kingdom kingdom;
				if ((kingdom = item.Object as Kingdom) != null)
				{
					return kingdom.Clans.Count.ToString();
				}
				Debug.FailedAssert("Unable to get the clan count from a non-kingdom object.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Encyclopedia\\Pages\\DefaultEncyclopediaFactionPage.cs", "GetComparedValueText", 149);
				return "";
			}

			private static Func<Kingdom, Kingdom, int> _comparison = (Kingdom k1, Kingdom k2) => k1.Clans.Count.CompareTo(k2.Clans.Count);
		}

		public abstract class EncyclopediaListKingdomComparer : EncyclopediaListItemComparerBase
		{
			public int CompareKingdoms(EncyclopediaListItem x, EncyclopediaListItem y, Func<Kingdom, Kingdom, int> comparison)
			{
				Kingdom kingdom;
				Kingdom kingdom2;
				if ((kingdom = x.Object as Kingdom) == null || (kingdom2 = y.Object as Kingdom) == null)
				{
					Debug.FailedAssert("Both objects should be kingdoms.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Encyclopedia\\Pages\\DefaultEncyclopediaFactionPage.cs", "CompareKingdoms", 164);
					return 0;
				}
				int num = comparison(kingdom, kingdom2) * (base.IsAscending ? 1 : (-1));
				if (num == 0)
				{
					return base.ResolveEquality(x, y);
				}
				return num;
			}
		}
	}
}
