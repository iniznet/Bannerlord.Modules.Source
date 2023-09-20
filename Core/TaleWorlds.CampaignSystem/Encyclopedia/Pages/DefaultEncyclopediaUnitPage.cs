using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.Encyclopedia.Pages
{
	[EncyclopediaModel(new Type[] { typeof(CharacterObject) })]
	public class DefaultEncyclopediaUnitPage : EncyclopediaPage
	{
		public DefaultEncyclopediaUnitPage()
		{
			base.HomePageOrderIndex = 300;
		}

		protected override IEnumerable<EncyclopediaListItem> InitializeListItems()
		{
			using (List<CharacterObject>.Enumerator enumerator = CharacterObject.All.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					CharacterObject character = enumerator.Current;
					if (this.IsValidEncyclopediaItem(character))
					{
						yield return new EncyclopediaListItem(character, character.Name.ToString(), "", character.StringId, base.GetIdentifier(typeof(CharacterObject)), true, delegate
						{
							InformationManager.ShowTooltip(typeof(CharacterObject), new object[] { character });
						});
					}
				}
			}
			List<CharacterObject>.Enumerator enumerator = default(List<CharacterObject>.Enumerator);
			yield break;
			yield break;
		}

		protected override IEnumerable<EncyclopediaFilterGroup> InitializeFilterItems()
		{
			List<EncyclopediaFilterGroup> list = new List<EncyclopediaFilterGroup>();
			List<EncyclopediaFilterItem> list2 = new List<EncyclopediaFilterItem>();
			list2.Add(new EncyclopediaFilterItem(new TextObject("{=1Bm1Wk1v}Infantry", null), (object s) => ((CharacterObject)s).IsInfantry));
			list2.Add(new EncyclopediaFilterItem(new TextObject("{=bIiBytSB}Archers", null), (object s) => ((CharacterObject)s).IsRanged && !((CharacterObject)s).IsMounted));
			list2.Add(new EncyclopediaFilterItem(new TextObject("{=YVGtcLHF}Cavalry", null), (object s) => ((CharacterObject)s).IsMounted && !((CharacterObject)s).IsRanged));
			list2.Add(new EncyclopediaFilterItem(new TextObject("{=I1CMeL9R}Mounted Archers", null), (object s) => ((CharacterObject)s).IsRanged && ((CharacterObject)s).IsMounted));
			List<EncyclopediaFilterItem> list3 = list2;
			list.Add(new EncyclopediaFilterGroup(list3, new TextObject("{=zMMqgxb1}Type", null)));
			List<EncyclopediaFilterItem> list4 = new List<EncyclopediaFilterItem>();
			list4.Add(new EncyclopediaFilterItem(GameTexts.FindText("str_occupation", "Soldier"), (object s) => ((CharacterObject)s).Occupation == Occupation.Soldier));
			list4.Add(new EncyclopediaFilterItem(GameTexts.FindText("str_occupation", "Mercenary"), (object s) => ((CharacterObject)s).Occupation == Occupation.Mercenary));
			list4.Add(new EncyclopediaFilterItem(GameTexts.FindText("str_occupation", "Bandit"), (object s) => ((CharacterObject)s).Occupation == Occupation.Bandit));
			List<EncyclopediaFilterItem> list5 = list4;
			list.Add(new EncyclopediaFilterGroup(list5, new TextObject("{=GZxFIeiJ}Occupation", null)));
			List<EncyclopediaFilterItem> list6 = new List<EncyclopediaFilterItem>();
			using (List<CultureObject>.Enumerator enumerator = (from x in Game.Current.ObjectManager.GetObjectTypeList<CultureObject>()
				orderby !x.IsMainCulture descending
				select x).ThenBy((CultureObject f) => f.Name.ToString()).ToList<CultureObject>().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					CultureObject culture = enumerator.Current;
					if (culture.StringId != "neutral_culture")
					{
						list6.Add(new EncyclopediaFilterItem(culture.Name, (object c) => ((CharacterObject)c).Culture == culture));
					}
				}
			}
			list.Add(new EncyclopediaFilterGroup(list6, GameTexts.FindText("str_culture", null)));
			return list;
		}

		protected override IEnumerable<EncyclopediaSortController> InitializeSortControllers()
		{
			return new List<EncyclopediaSortController>
			{
				new EncyclopediaSortController(new TextObject("{=cc1d7mkq}Tier", null), new DefaultEncyclopediaUnitPage.EncyclopediaListUnitTierComparer()),
				new EncyclopediaSortController(GameTexts.FindText("str_level_tag", null), new DefaultEncyclopediaUnitPage.EncyclopediaListUnitLevelComparer())
			};
		}

		public override string GetViewFullyQualifiedName()
		{
			return "EncyclopediaUnitPage";
		}

		public override TextObject GetName()
		{
			return GameTexts.FindText("str_encyclopedia_troops", null);
		}

		public override TextObject GetDescriptionText()
		{
			return GameTexts.FindText("str_unit_description", null);
		}

		public override string GetStringID()
		{
			return "EncyclopediaUnit";
		}

		public override bool IsValidEncyclopediaItem(object o)
		{
			CharacterObject characterObject = o as CharacterObject;
			return characterObject != null && !characterObject.IsTemplate && characterObject != null && !characterObject.HiddenInEncylopedia && (characterObject.Occupation == Occupation.Soldier || characterObject.Occupation == Occupation.Mercenary || characterObject.Occupation == Occupation.Bandit || characterObject.Occupation == Occupation.Gangster || characterObject.Occupation == Occupation.CaravanGuard || (characterObject.Occupation == Occupation.Villager && characterObject.UpgradeTargets.Length != 0));
		}

		private class EncyclopediaListUnitTierComparer : DefaultEncyclopediaUnitPage.EncyclopediaListUnitComparer
		{
			public override int Compare(EncyclopediaListItem x, EncyclopediaListItem y)
			{
				return base.CompareUnits(x, y, DefaultEncyclopediaUnitPage.EncyclopediaListUnitTierComparer._comparison);
			}

			public override string GetComparedValueText(EncyclopediaListItem item)
			{
				CharacterObject characterObject;
				if ((characterObject = item.Object as CharacterObject) != null)
				{
					return characterObject.Tier.ToString();
				}
				Debug.FailedAssert("Unable to get the tier of a non-character object.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Encyclopedia\\Pages\\DefaultEncyclopediaUnitPage.cs", "GetComparedValueText", 138);
				return "";
			}

			private static Func<CharacterObject, CharacterObject, int> _comparison = (CharacterObject c1, CharacterObject c2) => c1.Tier.CompareTo(c2.Tier);
		}

		private class EncyclopediaListUnitLevelComparer : DefaultEncyclopediaUnitPage.EncyclopediaListUnitComparer
		{
			public override int Compare(EncyclopediaListItem x, EncyclopediaListItem y)
			{
				return base.CompareUnits(x, y, DefaultEncyclopediaUnitPage.EncyclopediaListUnitLevelComparer._comparison);
			}

			public override string GetComparedValueText(EncyclopediaListItem item)
			{
				CharacterObject characterObject;
				if ((characterObject = item.Object as CharacterObject) != null)
				{
					return characterObject.Level.ToString();
				}
				Debug.FailedAssert("Unable to get the level of a non-character object.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Encyclopedia\\Pages\\DefaultEncyclopediaUnitPage.cs", "GetComparedValueText", 159);
				return "";
			}

			private static Func<CharacterObject, CharacterObject, int> _comparison = (CharacterObject c1, CharacterObject c2) => c1.Level.CompareTo(c2.Level);
		}

		public abstract class EncyclopediaListUnitComparer : EncyclopediaListItemComparerBase
		{
			public int CompareUnits(EncyclopediaListItem x, EncyclopediaListItem y, Func<CharacterObject, CharacterObject, int> comparison)
			{
				CharacterObject characterObject;
				CharacterObject characterObject2;
				if ((characterObject = x.Object as CharacterObject) == null || (characterObject2 = y.Object as CharacterObject) == null)
				{
					Debug.FailedAssert("Both objects should be character objects.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Encyclopedia\\Pages\\DefaultEncyclopediaUnitPage.cs", "CompareUnits", 174);
					return 0;
				}
				int num = comparison(characterObject, characterObject2) * (base.IsAscending ? 1 : (-1));
				if (num == 0)
				{
					return base.ResolveEquality(x, y);
				}
				return num;
			}
		}
	}
}
