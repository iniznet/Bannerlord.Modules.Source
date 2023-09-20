using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem.Encyclopedia.Pages
{
	// Token: 0x02000168 RID: 360
	[EncyclopediaModel(new Type[] { typeof(Hero) })]
	public class DefaultEncyclopediaHeroPage : EncyclopediaPage
	{
		// Token: 0x060018A4 RID: 6308 RVA: 0x0007C3AB File Offset: 0x0007A5AB
		public DefaultEncyclopediaHeroPage()
		{
			base.HomePageOrderIndex = 200;
		}

		// Token: 0x060018A5 RID: 6309 RVA: 0x0007C3BE File Offset: 0x0007A5BE
		protected override IEnumerable<EncyclopediaListItem> InitializeListItems()
		{
			int comingOfAge = Campaign.Current.Models.AgeModel.HeroComesOfAge;
			TextObject heroName = new TextObject("{=TauRjAud}{NAME} of the {FACTION}", null);
			string text = string.Empty;
			using (List<Hero>.Enumerator enumerator = Hero.AllAliveHeroes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Hero hero2 = enumerator.Current;
					if (this.IsValidEncyclopediaItem(hero2) && !hero2.IsNotable && hero2.Age >= (float)comingOfAge)
					{
						Clan clan = hero2.Clan;
						if (clan != null && !clan.IsNeutralClan)
						{
							heroName.SetTextVariable("NAME", hero2.FirstName ?? hero2.Name);
							TextObject textObject = heroName;
							string text2 = "FACTION";
							Clan clan2 = hero2.Clan;
							textObject.SetTextVariable(text2, ((clan2 != null) ? clan2.Name : null) ?? TextObject.Empty);
							text = heroName.ToString();
						}
						else
						{
							text = hero2.Name.ToString();
						}
						yield return new EncyclopediaListItem(hero2, text, "", hero2.StringId, base.GetIdentifier(typeof(Hero)), DefaultEncyclopediaHeroPage.CanPlayerSeeValuesOf(hero2), delegate
						{
							InformationManager.ShowTooltip(typeof(Hero), new object[] { hero2, false });
						});
					}
				}
			}
			List<Hero>.Enumerator enumerator = default(List<Hero>.Enumerator);
			using (List<Hero>.Enumerator enumerator = Hero.DeadOrDisabledHeroes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Hero hero = enumerator.Current;
					if (this.IsValidEncyclopediaItem(hero) && !hero.IsNotable && hero.Age >= (float)comingOfAge)
					{
						Clan clan3 = hero.Clan;
						if (clan3 != null && !clan3.IsNeutralClan)
						{
							heroName.SetTextVariable("NAME", hero.FirstName ?? hero.Name);
							TextObject textObject2 = heroName;
							string text3 = "FACTION";
							Clan clan4 = hero.Clan;
							textObject2.SetTextVariable(text3, ((clan4 != null) ? clan4.Name : null) ?? TextObject.Empty);
							yield return new EncyclopediaListItem(hero, heroName.ToString(), "", hero.StringId, base.GetIdentifier(typeof(Hero)), DefaultEncyclopediaHeroPage.CanPlayerSeeValuesOf(hero), delegate
							{
								InformationManager.ShowTooltip(typeof(Hero), new object[] { hero, false });
							});
						}
						else
						{
							yield return new EncyclopediaListItem(hero, hero.Name.ToString(), "", hero.StringId, base.GetIdentifier(typeof(Hero)), DefaultEncyclopediaHeroPage.CanPlayerSeeValuesOf(hero), delegate
							{
								InformationManager.ShowTooltip(typeof(Hero), new object[] { hero, false });
							});
						}
					}
				}
			}
			enumerator = default(List<Hero>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x060018A6 RID: 6310 RVA: 0x0007C3D0 File Offset: 0x0007A5D0
		protected override IEnumerable<EncyclopediaFilterGroup> InitializeFilterItems()
		{
			List<EncyclopediaFilterGroup> list = new List<EncyclopediaFilterGroup>();
			List<EncyclopediaFilterItem> list2 = new List<EncyclopediaFilterItem>();
			list2.Add(new EncyclopediaFilterItem(new TextObject("{=5xi0t1dD}Met Before", null), (object h) => ((Hero)h).HasMet));
			list.Add(new EncyclopediaFilterGroup(list2, new TextObject("{=BlidMNGT}Relation", null)));
			List<EncyclopediaFilterItem> list3 = new List<EncyclopediaFilterItem>();
			list3.Add(new EncyclopediaFilterItem(new TextObject("{=oAb4NqO5}Male", null), (object h) => !((Hero)h).IsFemale));
			list3.Add(new EncyclopediaFilterItem(new TextObject("{=2YUUGQvG}Female", null), (object h) => ((Hero)h).IsFemale));
			list.Add(new EncyclopediaFilterGroup(list3, new TextObject("{=fGFMqlGz}Gender", null)));
			List<EncyclopediaFilterItem> list4 = new List<EncyclopediaFilterItem>();
			list4.Add(new EncyclopediaFilterItem(new TextObject("{=uvjOVy5P}Dead", null), (object h) => !((Hero)h).IsAlive));
			list4.Add(new EncyclopediaFilterItem(new TextObject("{=3TmLIou4}Alive", null), (object h) => ((Hero)h).IsAlive));
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
						list5.Add(new EncyclopediaFilterItem(culture.Name, (object c) => ((Hero)c).Culture == culture));
					}
				}
			}
			list.Add(new EncyclopediaFilterGroup(list5, GameTexts.FindText("str_culture", null)));
			List<EncyclopediaFilterItem> list6 = new List<EncyclopediaFilterItem>();
			list6.Add(new EncyclopediaFilterItem(new TextObject("{=b9ty57rJ}Faction Leader", null), (object h) => ((Hero)h).IsFactionLeader));
			list6.Add(new EncyclopediaFilterItem(new TextObject("{=4vleNtxb}Lord/Lady", null), (object h) => ((Hero)h).IsLord));
			list6.Add(new EncyclopediaFilterItem(new TextObject("{=vmMqs3Ck}Noble", null), delegate(object h)
			{
				Clan clan = ((Hero)h).Clan;
				return clan != null && clan.IsNoble;
			}));
			list6.Add(new EncyclopediaFilterItem(new TextObject("{=FLa5OuyK}Wanderer", null), (object h) => ((Hero)h).IsWanderer));
			list.Add(new EncyclopediaFilterGroup(list6, new TextObject("{=GZxFIeiJ}Occupation", null)));
			List<EncyclopediaFilterItem> list7 = new List<EncyclopediaFilterItem>();
			list7.Add(new EncyclopediaFilterItem(new TextObject("{=qIAgh9VL}Not Married", null), (object h) => ((Hero)h).Spouse == null));
			list7.Add(new EncyclopediaFilterItem(new TextObject("{=xeawD38S}Married", null), (object h) => ((Hero)h).Spouse != null));
			list.Add(new EncyclopediaFilterGroup(list7, new TextObject("{=PMio7set}Marital Status", null)));
			return list;
		}

		// Token: 0x060018A7 RID: 6311 RVA: 0x0007C7D4 File Offset: 0x0007A9D4
		protected override IEnumerable<EncyclopediaSortController> InitializeSortControllers()
		{
			return new List<EncyclopediaSortController>
			{
				new EncyclopediaSortController(new TextObject("{=jaaQijQs}Age", null), new DefaultEncyclopediaHeroPage.EncyclopediaListHeroAgeComparer()),
				new EncyclopediaSortController(new TextObject("{=BlidMNGT}Relation", null), new DefaultEncyclopediaHeroPage.EncyclopediaListHeroRelationComparer())
			};
		}

		// Token: 0x060018A8 RID: 6312 RVA: 0x0007C811 File Offset: 0x0007AA11
		public override string GetViewFullyQualifiedName()
		{
			return "EncyclopediaHeroPage";
		}

		// Token: 0x060018A9 RID: 6313 RVA: 0x0007C818 File Offset: 0x0007AA18
		public override string GetStringID()
		{
			return "EncyclopediaHero";
		}

		// Token: 0x060018AA RID: 6314 RVA: 0x0007C81F File Offset: 0x0007AA1F
		public override TextObject GetName()
		{
			return GameTexts.FindText("str_encyclopedia_heroes", null);
		}

		// Token: 0x060018AB RID: 6315 RVA: 0x0007C82C File Offset: 0x0007AA2C
		public override TextObject GetDescriptionText()
		{
			return GameTexts.FindText("str_hero_description", null);
		}

		// Token: 0x060018AC RID: 6316 RVA: 0x0007C839 File Offset: 0x0007AA39
		public override MBObjectBase GetObject(string typeName, string stringID)
		{
			return Campaign.Current.CampaignObjectManager.Find<Hero>(stringID);
		}

		// Token: 0x060018AD RID: 6317 RVA: 0x0007C84C File Offset: 0x0007AA4C
		public override bool IsValidEncyclopediaItem(object o)
		{
			Hero hero = o as Hero;
			if (hero != null && !hero.IsTemplate && hero.IsReady)
			{
				IFaction mapFaction = hero.MapFaction;
				if (mapFaction == null || !mapFaction.IsBanditFaction)
				{
					return !hero.CharacterObject.HiddenInEncylopedia;
				}
			}
			return false;
		}

		// Token: 0x060018AE RID: 6318 RVA: 0x0007C89A File Offset: 0x0007AA9A
		private static bool CanPlayerSeeValuesOf(Hero hero)
		{
			return Campaign.Current.Models.InformationRestrictionModel.DoesPlayerKnowDetailsOf(hero);
		}

		// Token: 0x02000538 RID: 1336
		private class EncyclopediaListHeroAgeComparer : DefaultEncyclopediaHeroPage.EncyclopediaListHeroComparer
		{
			// Token: 0x06004303 RID: 17155 RVA: 0x001366BE File Offset: 0x001348BE
			public override int Compare(EncyclopediaListItem x, EncyclopediaListItem y)
			{
				return base.CompareHeroes(x, y, DefaultEncyclopediaHeroPage.EncyclopediaListHeroAgeComparer._comparison);
			}

			// Token: 0x06004304 RID: 17156 RVA: 0x001366D0 File Offset: 0x001348D0
			public override string GetComparedValueText(EncyclopediaListItem item)
			{
				Hero hero;
				if ((hero = item.Object as Hero) == null)
				{
					Debug.FailedAssert("Unable to get the age of a non-hero object.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Encyclopedia\\Pages\\DefaultEncyclopediaHeroPage.cs", "GetComparedValueText", 179);
					return "";
				}
				if (!DefaultEncyclopediaHeroPage.CanPlayerSeeValuesOf(hero))
				{
					return this._missingValue.ToString();
				}
				return ((int)hero.Age).ToString();
			}

			// Token: 0x0400162A RID: 5674
			private static Func<Hero, Hero, int> _comparison = (Hero h1, Hero h2) => h1.Age.CompareTo(h2.Age);
		}

		// Token: 0x02000539 RID: 1337
		private class EncyclopediaListHeroRelationComparer : DefaultEncyclopediaHeroPage.EncyclopediaListHeroComparer
		{
			// Token: 0x06004307 RID: 17159 RVA: 0x0013674D File Offset: 0x0013494D
			public override int Compare(EncyclopediaListItem x, EncyclopediaListItem y)
			{
				return base.CompareHeroes(x, y, DefaultEncyclopediaHeroPage.EncyclopediaListHeroRelationComparer._comparison);
			}

			// Token: 0x06004308 RID: 17160 RVA: 0x0013675C File Offset: 0x0013495C
			public override string GetComparedValueText(EncyclopediaListItem item)
			{
				Hero hero;
				if ((hero = item.Object as Hero) == null)
				{
					Debug.FailedAssert("Unable to get the relation between a non-hero object and the player.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Encyclopedia\\Pages\\DefaultEncyclopediaHeroPage.cs", "GetComparedValueText", 209);
					return "";
				}
				if (!DefaultEncyclopediaHeroPage.CanPlayerSeeValuesOf(hero))
				{
					return this._missingValue.ToString();
				}
				int num = (int)hero.GetRelationWithPlayer();
				MBTextManager.SetTextVariable("NUMBER", num);
				if (num <= 0)
				{
					return num.ToString();
				}
				return GameTexts.FindText("str_plus_with_number", null).ToString();
			}

			// Token: 0x0400162B RID: 5675
			private static Func<Hero, Hero, int> _comparison = (Hero h1, Hero h2) => h1.GetRelationWithPlayer().CompareTo(h2.GetRelationWithPlayer());
		}

		// Token: 0x0200053A RID: 1338
		public abstract class EncyclopediaListHeroComparer : EncyclopediaListItemComparerBase
		{
			// Token: 0x0600430B RID: 17163 RVA: 0x001367FC File Offset: 0x001349FC
			protected bool CompareVisibility(Hero h1, Hero h2, out int comparisonResult)
			{
				bool flag = DefaultEncyclopediaHeroPage.CanPlayerSeeValuesOf(h1);
				bool flag2 = DefaultEncyclopediaHeroPage.CanPlayerSeeValuesOf(h2);
				if (!flag && !flag2)
				{
					comparisonResult = 0;
					return true;
				}
				if (!flag)
				{
					comparisonResult = (base.IsAscending ? 1 : (-1));
					return true;
				}
				if (!flag2)
				{
					comparisonResult = (base.IsAscending ? (-1) : 1);
					return true;
				}
				comparisonResult = 0;
				return false;
			}

			// Token: 0x0600430C RID: 17164 RVA: 0x0013684C File Offset: 0x00134A4C
			protected int CompareHeroes(EncyclopediaListItem x, EncyclopediaListItem y, Func<Hero, Hero, int> comparison)
			{
				Hero hero;
				Hero hero2;
				if ((hero = x.Object as Hero) == null || (hero2 = y.Object as Hero) == null)
				{
					Debug.FailedAssert("Both objects should be heroes.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Encyclopedia\\Pages\\DefaultEncyclopediaHeroPage.cs", "CompareHeroes", 258);
					return 0;
				}
				int num;
				if (this.CompareVisibility(hero, hero2, out num))
				{
					if (num == 0)
					{
						return base.ResolveEquality(x, y);
					}
					return num * (base.IsAscending ? 1 : (-1));
				}
				else
				{
					int num2 = comparison(hero, hero2) * (base.IsAscending ? 1 : (-1));
					if (num2 == 0)
					{
						return base.ResolveEquality(x, y);
					}
					return num2;
				}
			}

			// Token: 0x02000783 RID: 1923
			// (Invoke) Token: 0x060056DD RID: 22237
			protected delegate bool HeroVisibilityComparerDelegate(Hero h1, Hero h2, out int comparisonResult);
		}
	}
}
