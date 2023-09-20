using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.Encyclopedia.Pages
{
	// Token: 0x0200016A RID: 362
	[EncyclopediaModel(new Type[] { typeof(CharacterObject) })]
	public class DefaultEncyclopediaUnitPage : EncyclopediaPage
	{
		// Token: 0x060018B9 RID: 6329 RVA: 0x0007CBCC File Offset: 0x0007ADCC
		public DefaultEncyclopediaUnitPage()
		{
			base.HomePageOrderIndex = 300;
		}

		// Token: 0x060018BA RID: 6330 RVA: 0x0007CBDF File Offset: 0x0007ADDF
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

		// Token: 0x060018BB RID: 6331 RVA: 0x0007CBF0 File Offset: 0x0007ADF0
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

		// Token: 0x060018BC RID: 6332 RVA: 0x0007CEBC File Offset: 0x0007B0BC
		protected override IEnumerable<EncyclopediaSortController> InitializeSortControllers()
		{
			return new List<EncyclopediaSortController>
			{
				new EncyclopediaSortController(new TextObject("{=cc1d7mkq}Tier", null), new DefaultEncyclopediaUnitPage.EncyclopediaListUnitTierComparer()),
				new EncyclopediaSortController(GameTexts.FindText("str_level_tag", null), new DefaultEncyclopediaUnitPage.EncyclopediaListUnitLevelComparer())
			};
		}

		// Token: 0x060018BD RID: 6333 RVA: 0x0007CEF9 File Offset: 0x0007B0F9
		public override string GetViewFullyQualifiedName()
		{
			return "EncyclopediaUnitPage";
		}

		// Token: 0x060018BE RID: 6334 RVA: 0x0007CF00 File Offset: 0x0007B100
		public override TextObject GetName()
		{
			return GameTexts.FindText("str_encyclopedia_troops", null);
		}

		// Token: 0x060018BF RID: 6335 RVA: 0x0007CF0D File Offset: 0x0007B10D
		public override TextObject GetDescriptionText()
		{
			return GameTexts.FindText("str_unit_description", null);
		}

		// Token: 0x060018C0 RID: 6336 RVA: 0x0007CF1A File Offset: 0x0007B11A
		public override string GetStringID()
		{
			return "EncyclopediaUnit";
		}

		// Token: 0x060018C1 RID: 6337 RVA: 0x0007CF24 File Offset: 0x0007B124
		public override bool IsValidEncyclopediaItem(object o)
		{
			CharacterObject characterObject = o as CharacterObject;
			return characterObject != null && !characterObject.IsTemplate && characterObject != null && !characterObject.HiddenInEncylopedia && (characterObject.Occupation == Occupation.Soldier || characterObject.Occupation == Occupation.Mercenary || characterObject.Occupation == Occupation.Bandit || characterObject.Occupation == Occupation.Gangster || characterObject.Occupation == Occupation.CaravanGuard || (characterObject.Occupation == Occupation.Villager && characterObject.UpgradeTargets.Length != 0));
		}

		// Token: 0x0200054B RID: 1355
		private class EncyclopediaListUnitTierComparer : DefaultEncyclopediaUnitPage.EncyclopediaListUnitComparer
		{
			// Token: 0x0600435F RID: 17247 RVA: 0x0013792B File Offset: 0x00135B2B
			public override int Compare(EncyclopediaListItem x, EncyclopediaListItem y)
			{
				return base.CompareUnits(x, y, DefaultEncyclopediaUnitPage.EncyclopediaListUnitTierComparer._comparison);
			}

			// Token: 0x06004360 RID: 17248 RVA: 0x0013793C File Offset: 0x00135B3C
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

			// Token: 0x04001651 RID: 5713
			private static Func<CharacterObject, CharacterObject, int> _comparison = (CharacterObject c1, CharacterObject c2) => c1.Tier.CompareTo(c2.Tier);
		}

		// Token: 0x0200054C RID: 1356
		private class EncyclopediaListUnitLevelComparer : DefaultEncyclopediaUnitPage.EncyclopediaListUnitComparer
		{
			// Token: 0x06004363 RID: 17251 RVA: 0x001379A4 File Offset: 0x00135BA4
			public override int Compare(EncyclopediaListItem x, EncyclopediaListItem y)
			{
				return base.CompareUnits(x, y, DefaultEncyclopediaUnitPage.EncyclopediaListUnitLevelComparer._comparison);
			}

			// Token: 0x06004364 RID: 17252 RVA: 0x001379B4 File Offset: 0x00135BB4
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

			// Token: 0x04001652 RID: 5714
			private static Func<CharacterObject, CharacterObject, int> _comparison = (CharacterObject c1, CharacterObject c2) => c1.Level.CompareTo(c2.Level);
		}

		// Token: 0x0200054D RID: 1357
		public abstract class EncyclopediaListUnitComparer : EncyclopediaListItemComparerBase
		{
			// Token: 0x06004367 RID: 17255 RVA: 0x00137A1C File Offset: 0x00135C1C
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
