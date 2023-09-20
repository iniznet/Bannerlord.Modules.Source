using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.Encyclopedia.Pages
{
	// Token: 0x02000169 RID: 361
	[EncyclopediaModel(new Type[] { typeof(Settlement) })]
	public class DefaultEncyclopediaSettlementPage : EncyclopediaPage
	{
		// Token: 0x060018AF RID: 6319 RVA: 0x0007C8B1 File Offset: 0x0007AAB1
		public DefaultEncyclopediaSettlementPage()
		{
			base.HomePageOrderIndex = 100;
		}

		// Token: 0x060018B0 RID: 6320 RVA: 0x0007C8C1 File Offset: 0x0007AAC1
		protected override IEnumerable<EncyclopediaListItem> InitializeListItems()
		{
			using (List<Settlement>.Enumerator enumerator = Settlement.All.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Settlement settlement = enumerator.Current;
					if (this.IsValidEncyclopediaItem(settlement))
					{
						yield return new EncyclopediaListItem(settlement, settlement.Name.ToString(), "", settlement.StringId, base.GetIdentifier(typeof(Settlement)), DefaultEncyclopediaSettlementPage.CanPlayerSeeValuesOf(settlement), delegate
						{
							InformationManager.ShowTooltip(typeof(Settlement), new object[] { settlement, false });
						});
					}
				}
			}
			List<Settlement>.Enumerator enumerator = default(List<Settlement>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x060018B1 RID: 6321 RVA: 0x0007C8D4 File Offset: 0x0007AAD4
		protected override IEnumerable<EncyclopediaFilterGroup> InitializeFilterItems()
		{
			List<EncyclopediaFilterGroup> list = new List<EncyclopediaFilterGroup>();
			List<EncyclopediaFilterItem> list2 = new List<EncyclopediaFilterItem>();
			list2.Add(new EncyclopediaFilterItem(new TextObject("{=bOTQ7Pta}Town", null), (object s) => ((Settlement)s).IsTown));
			list2.Add(new EncyclopediaFilterItem(new TextObject("{=sVXa3zFx}Castle", null), (object s) => ((Settlement)s).IsCastle));
			list2.Add(new EncyclopediaFilterItem(new TextObject("{=Ua6CNLBZ}Village", null), (object s) => ((Settlement)s).IsVillage));
			List<EncyclopediaFilterItem> list3 = list2;
			list.Add(new EncyclopediaFilterGroup(list3, new TextObject("{=zMMqgxb1}Type", null)));
			List<EncyclopediaFilterItem> list4 = new List<EncyclopediaFilterItem>();
			using (List<CultureObject>.Enumerator enumerator = (from x in Game.Current.ObjectManager.GetObjectTypeList<CultureObject>()
				orderby !x.IsMainCulture descending
				select x).ThenBy((CultureObject f) => f.Name.ToString()).ToList<CultureObject>().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					CultureObject culture = enumerator.Current;
					if (culture.StringId != "neutral_culture" && culture.CanHaveSettlement)
					{
						list4.Add(new EncyclopediaFilterItem(culture.Name, (object c) => ((Settlement)c).Culture == culture));
					}
				}
			}
			list.Add(new EncyclopediaFilterGroup(list4, GameTexts.FindText("str_culture", null)));
			return list;
		}

		// Token: 0x060018B2 RID: 6322 RVA: 0x0007CAB0 File Offset: 0x0007ACB0
		protected override IEnumerable<EncyclopediaSortController> InitializeSortControllers()
		{
			return new List<EncyclopediaSortController>
			{
				new EncyclopediaSortController(GameTexts.FindText("str_garrison", null), new DefaultEncyclopediaSettlementPage.EncyclopediaListSettlementGarrisonComparer()),
				new EncyclopediaSortController(GameTexts.FindText("str_food", null), new DefaultEncyclopediaSettlementPage.EncyclopediaListSettlementFoodComparer()),
				new EncyclopediaSortController(GameTexts.FindText("str_security", null), new DefaultEncyclopediaSettlementPage.EncyclopediaListSettlementSecurityComparer()),
				new EncyclopediaSortController(GameTexts.FindText("str_loyalty", null), new DefaultEncyclopediaSettlementPage.EncyclopediaListSettlementLoyaltyComparer()),
				new EncyclopediaSortController(GameTexts.FindText("str_militia", null), new DefaultEncyclopediaSettlementPage.EncyclopediaListSettlementMilitiaComparer()),
				new EncyclopediaSortController(GameTexts.FindText("str_prosperity", null), new DefaultEncyclopediaSettlementPage.EncyclopediaListSettlementProsperityComparer())
			};
		}

		// Token: 0x060018B3 RID: 6323 RVA: 0x0007CB64 File Offset: 0x0007AD64
		public override string GetViewFullyQualifiedName()
		{
			return "EncyclopediaSettlementPage";
		}

		// Token: 0x060018B4 RID: 6324 RVA: 0x0007CB6B File Offset: 0x0007AD6B
		public override TextObject GetName()
		{
			return GameTexts.FindText("str_settlements", null);
		}

		// Token: 0x060018B5 RID: 6325 RVA: 0x0007CB78 File Offset: 0x0007AD78
		public override TextObject GetDescriptionText()
		{
			return GameTexts.FindText("str_settlement_description", null);
		}

		// Token: 0x060018B6 RID: 6326 RVA: 0x0007CB85 File Offset: 0x0007AD85
		public override string GetStringID()
		{
			return "EncyclopediaSettlement";
		}

		// Token: 0x060018B7 RID: 6327 RVA: 0x0007CB8C File Offset: 0x0007AD8C
		public override bool IsValidEncyclopediaItem(object o)
		{
			Settlement settlement = o as Settlement;
			return settlement != null && (settlement.IsFortification || settlement.IsVillage);
		}

		// Token: 0x060018B8 RID: 6328 RVA: 0x0007CBB5 File Offset: 0x0007ADB5
		private static bool CanPlayerSeeValuesOf(Settlement settlement)
		{
			return Campaign.Current.Models.InformationRestrictionModel.DoesPlayerKnowDetailsOf(settlement);
		}

		// Token: 0x02000540 RID: 1344
		private class EncyclopediaListSettlementGarrisonComparer : DefaultEncyclopediaSettlementPage.EncyclopediaListSettlementComparer
		{
			// Token: 0x0600432E RID: 17198 RVA: 0x00136FA4 File Offset: 0x001351A4
			private static int GarrisonComparison(Town t1, Town t2)
			{
				return t1.GarrisonParty.MemberRoster.TotalManCount.CompareTo(t2.GarrisonParty.MemberRoster.TotalManCount);
			}

			// Token: 0x0600432F RID: 17199 RVA: 0x00136FDC File Offset: 0x001351DC
			protected override bool CompareVisibility(Settlement s1, Settlement s2, out int comparisonResult)
			{
				if (s1.IsTown && s2.IsTown)
				{
					if (s1.Town.GarrisonParty == null && s2.Town.GarrisonParty == null)
					{
						comparisonResult = 0;
						return true;
					}
					if (s1.Town.GarrisonParty == null)
					{
						comparisonResult = (base.IsAscending ? 2 : (-2));
						return true;
					}
					if (s2.Town.GarrisonParty == null)
					{
						comparisonResult = (base.IsAscending ? (-2) : 2);
						return true;
					}
				}
				return base.CompareVisibility(s1, s2, out comparisonResult);
			}

			// Token: 0x06004330 RID: 17200 RVA: 0x0013705D File Offset: 0x0013525D
			public override int Compare(EncyclopediaListItem x, EncyclopediaListItem y)
			{
				return base.CompareFiefs(x, y, new DefaultEncyclopediaSettlementPage.EncyclopediaListSettlementComparer.SettlementVisibilityComparerDelegate(this.CompareVisibility), new Func<Town, Town, int>(DefaultEncyclopediaSettlementPage.EncyclopediaListSettlementGarrisonComparer.GarrisonComparison));
			}

			// Token: 0x06004331 RID: 17201 RVA: 0x00137080 File Offset: 0x00135280
			public override string GetComparedValueText(EncyclopediaListItem item)
			{
				Settlement settlement;
				if ((settlement = item.Object as Settlement) == null)
				{
					Debug.FailedAssert("Unable to get the garrison of a non-settlement object.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Encyclopedia\\Pages\\DefaultEncyclopediaSettlementPage.cs", "GetComparedValueText", 159);
					return "";
				}
				if (settlement.IsVillage)
				{
					return this._emptyValue.ToString();
				}
				if (!DefaultEncyclopediaSettlementPage.CanPlayerSeeValuesOf(settlement))
				{
					return this._missingValue.ToString();
				}
				MobileParty garrisonParty = settlement.Town.GarrisonParty;
				return ((garrisonParty != null) ? garrisonParty.MemberRoster.TotalManCount.ToString() : null) ?? 0.ToString();
			}
		}

		// Token: 0x02000541 RID: 1345
		private class EncyclopediaListSettlementFoodComparer : DefaultEncyclopediaSettlementPage.EncyclopediaListSettlementComparer
		{
			// Token: 0x06004333 RID: 17203 RVA: 0x0013711C File Offset: 0x0013531C
			public override int Compare(EncyclopediaListItem x, EncyclopediaListItem y)
			{
				return base.CompareFiefs(x, y, new DefaultEncyclopediaSettlementPage.EncyclopediaListSettlementComparer.SettlementVisibilityComparerDelegate(this.CompareVisibility), new Func<Town, Town, int>(DefaultEncyclopediaSettlementPage.EncyclopediaListSettlementFoodComparer.FoodComparison));
			}

			// Token: 0x06004334 RID: 17204 RVA: 0x00137140 File Offset: 0x00135340
			private static int FoodComparison(Town t1, Town t2)
			{
				return t1.FoodStocks.CompareTo(t2.FoodStocks);
			}

			// Token: 0x06004335 RID: 17205 RVA: 0x00137164 File Offset: 0x00135364
			public override string GetComparedValueText(EncyclopediaListItem item)
			{
				Settlement settlement;
				if ((settlement = item.Object as Settlement) == null)
				{
					Debug.FailedAssert("Unable to get the food stocks of a non-settlement object.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Encyclopedia\\Pages\\DefaultEncyclopediaSettlementPage.cs", "GetComparedValueText", 194);
					return "";
				}
				if (settlement.IsVillage)
				{
					return this._emptyValue.ToString();
				}
				if (!DefaultEncyclopediaSettlementPage.CanPlayerSeeValuesOf(settlement))
				{
					return this._missingValue.ToString();
				}
				return ((int)settlement.Town.FoodStocks).ToString();
			}
		}

		// Token: 0x02000542 RID: 1346
		private class EncyclopediaListSettlementSecurityComparer : DefaultEncyclopediaSettlementPage.EncyclopediaListSettlementComparer
		{
			// Token: 0x06004337 RID: 17207 RVA: 0x001371E3 File Offset: 0x001353E3
			public override int Compare(EncyclopediaListItem x, EncyclopediaListItem y)
			{
				return base.CompareFiefs(x, y, new DefaultEncyclopediaSettlementPage.EncyclopediaListSettlementComparer.SettlementVisibilityComparerDelegate(this.CompareVisibility), new Func<Town, Town, int>(DefaultEncyclopediaSettlementPage.EncyclopediaListSettlementSecurityComparer.SecurityComparison));
			}

			// Token: 0x06004338 RID: 17208 RVA: 0x00137208 File Offset: 0x00135408
			private static int SecurityComparison(Town t1, Town t2)
			{
				return t1.Security.CompareTo(t2.Security);
			}

			// Token: 0x06004339 RID: 17209 RVA: 0x0013722C File Offset: 0x0013542C
			public override string GetComparedValueText(EncyclopediaListItem item)
			{
				Settlement settlement;
				if ((settlement = item.Object as Settlement) == null)
				{
					Debug.FailedAssert("Unable to get the security of a non-settlement object.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Encyclopedia\\Pages\\DefaultEncyclopediaSettlementPage.cs", "GetComparedValueText", 229);
					return "";
				}
				if (settlement.IsVillage)
				{
					return this._emptyValue.ToString();
				}
				if (!DefaultEncyclopediaSettlementPage.CanPlayerSeeValuesOf(settlement))
				{
					return this._missingValue.ToString();
				}
				return ((int)settlement.Town.Security).ToString();
			}
		}

		// Token: 0x02000543 RID: 1347
		private class EncyclopediaListSettlementLoyaltyComparer : DefaultEncyclopediaSettlementPage.EncyclopediaListSettlementComparer
		{
			// Token: 0x0600433B RID: 17211 RVA: 0x001372AB File Offset: 0x001354AB
			public override int Compare(EncyclopediaListItem x, EncyclopediaListItem y)
			{
				return base.CompareFiefs(x, y, new DefaultEncyclopediaSettlementPage.EncyclopediaListSettlementComparer.SettlementVisibilityComparerDelegate(this.CompareVisibility), new Func<Town, Town, int>(DefaultEncyclopediaSettlementPage.EncyclopediaListSettlementLoyaltyComparer.LoyaltyComparison));
			}

			// Token: 0x0600433C RID: 17212 RVA: 0x001372D0 File Offset: 0x001354D0
			private static int LoyaltyComparison(Town t1, Town t2)
			{
				return t1.Loyalty.CompareTo(t2.Loyalty);
			}

			// Token: 0x0600433D RID: 17213 RVA: 0x001372F4 File Offset: 0x001354F4
			public override string GetComparedValueText(EncyclopediaListItem item)
			{
				Settlement settlement;
				if ((settlement = item.Object as Settlement) == null)
				{
					Debug.FailedAssert("Unable to get the loyalty of a non-settlement object.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Encyclopedia\\Pages\\DefaultEncyclopediaSettlementPage.cs", "GetComparedValueText", 264);
					return "";
				}
				if (settlement.IsVillage)
				{
					return this._emptyValue.ToString();
				}
				if (!DefaultEncyclopediaSettlementPage.CanPlayerSeeValuesOf(settlement))
				{
					return this._missingValue.ToString();
				}
				return ((int)settlement.Town.Loyalty).ToString();
			}
		}

		// Token: 0x02000544 RID: 1348
		private class EncyclopediaListSettlementMilitiaComparer : DefaultEncyclopediaSettlementPage.EncyclopediaListSettlementComparer
		{
			// Token: 0x0600433F RID: 17215 RVA: 0x00137373 File Offset: 0x00135573
			public override int Compare(EncyclopediaListItem x, EncyclopediaListItem y)
			{
				return base.CompareSettlements(x, y, new DefaultEncyclopediaSettlementPage.EncyclopediaListSettlementComparer.SettlementVisibilityComparerDelegate(this.CompareVisibility), new Func<Settlement, Settlement, int>(DefaultEncyclopediaSettlementPage.EncyclopediaListSettlementMilitiaComparer.MilitiaComparison));
			}

			// Token: 0x06004340 RID: 17216 RVA: 0x00137398 File Offset: 0x00135598
			private static int MilitiaComparison(Settlement t1, Settlement t2)
			{
				return t1.Militia.CompareTo(t2.Militia);
			}

			// Token: 0x06004341 RID: 17217 RVA: 0x001373BC File Offset: 0x001355BC
			public override string GetComparedValueText(EncyclopediaListItem item)
			{
				Settlement settlement;
				if ((settlement = item.Object as Settlement) == null)
				{
					Debug.FailedAssert("Unable to get the militia of a non-settlement object.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Encyclopedia\\Pages\\DefaultEncyclopediaSettlementPage.cs", "GetComparedValueText", 295);
					return "";
				}
				if (!DefaultEncyclopediaSettlementPage.CanPlayerSeeValuesOf(settlement))
				{
					return this._missingValue.ToString();
				}
				return ((int)settlement.Militia).ToString();
			}
		}

		// Token: 0x02000545 RID: 1349
		private class EncyclopediaListSettlementProsperityComparer : DefaultEncyclopediaSettlementPage.EncyclopediaListSettlementComparer
		{
			// Token: 0x06004343 RID: 17219 RVA: 0x00137422 File Offset: 0x00135622
			public override int Compare(EncyclopediaListItem x, EncyclopediaListItem y)
			{
				return base.CompareSettlements(x, y, new DefaultEncyclopediaSettlementPage.EncyclopediaListSettlementComparer.SettlementVisibilityComparerDelegate(this.CompareVisibility), new Func<Settlement, Settlement, int>(DefaultEncyclopediaSettlementPage.EncyclopediaListSettlementProsperityComparer.ProsperityComparison));
			}

			// Token: 0x06004344 RID: 17220 RVA: 0x00137448 File Offset: 0x00135648
			private static int ProsperityComparison(Settlement t1, Settlement t2)
			{
				return t1.Prosperity.CompareTo(t2.Prosperity);
			}

			// Token: 0x06004345 RID: 17221 RVA: 0x0013746C File Offset: 0x0013566C
			public override string GetComparedValueText(EncyclopediaListItem item)
			{
				Settlement settlement;
				if ((settlement = item.Object as Settlement) == null)
				{
					Debug.FailedAssert("Unable to get the prosperity of a non-settlement object.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Encyclopedia\\Pages\\DefaultEncyclopediaSettlementPage.cs", "GetComparedValueText", 326);
					return "";
				}
				if (!DefaultEncyclopediaSettlementPage.CanPlayerSeeValuesOf(settlement))
				{
					return this._missingValue.ToString();
				}
				return ((int)settlement.Prosperity).ToString();
			}
		}

		// Token: 0x02000546 RID: 1350
		public abstract class EncyclopediaListSettlementComparer : EncyclopediaListItemComparerBase
		{
			// Token: 0x06004347 RID: 17223 RVA: 0x001374D4 File Offset: 0x001356D4
			protected virtual bool CompareVisibility(Settlement s1, Settlement s2, out int comparisonResult)
			{
				bool flag = DefaultEncyclopediaSettlementPage.CanPlayerSeeValuesOf(s1);
				bool flag2 = DefaultEncyclopediaSettlementPage.CanPlayerSeeValuesOf(s2);
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

			// Token: 0x06004348 RID: 17224 RVA: 0x00137524 File Offset: 0x00135724
			protected int CompareSettlements(EncyclopediaListItem x, EncyclopediaListItem y, DefaultEncyclopediaSettlementPage.EncyclopediaListSettlementComparer.SettlementVisibilityComparerDelegate visibilityComparison, Func<Settlement, Settlement, int> comparison)
			{
				Settlement settlement;
				Settlement settlement2;
				if ((settlement = x.Object as Settlement) == null || (settlement2 = y.Object as Settlement) == null)
				{
					Debug.FailedAssert("Both objects should be settlements.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Encyclopedia\\Pages\\DefaultEncyclopediaSettlementPage.cs", "CompareSettlements", 375);
					return 0;
				}
				int num;
				if (visibilityComparison(settlement, settlement2, out num))
				{
					if (num == 0)
					{
						return base.ResolveEquality(x, y);
					}
					return num * (base.IsAscending ? 1 : (-1));
				}
				else
				{
					int num2 = comparison(settlement, settlement2) * (base.IsAscending ? 1 : (-1));
					if (num2 == 0)
					{
						return base.ResolveEquality(x, y);
					}
					return num2;
				}
			}

			// Token: 0x06004349 RID: 17225 RVA: 0x001375B8 File Offset: 0x001357B8
			protected int CompareFiefs(EncyclopediaListItem x, EncyclopediaListItem y, DefaultEncyclopediaSettlementPage.EncyclopediaListSettlementComparer.SettlementVisibilityComparerDelegate visibilityComparison, Func<Town, Town, int> comparison)
			{
				Settlement settlement;
				Settlement settlement2;
				if ((settlement = x.Object as Settlement) == null || (settlement2 = y.Object as Settlement) == null)
				{
					Debug.FailedAssert("Unable to compare loyalty of non-fief (castle or town) objects.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Encyclopedia\\Pages\\DefaultEncyclopediaSettlementPage.cs", "CompareFiefs", 399);
					return 0;
				}
				int num = settlement.IsVillage.CompareTo(settlement2.IsVillage);
				if (num != 0)
				{
					return num;
				}
				if (settlement.IsVillage && settlement2.IsVillage)
				{
					return base.ResolveEquality(x, y);
				}
				int num2;
				if (visibilityComparison(settlement, settlement2, out num2))
				{
					if (num2 == 0)
					{
						return base.ResolveEquality(x, y);
					}
					return num2 * (base.IsAscending ? 1 : (-1));
				}
				else
				{
					num = comparison(settlement.Town, settlement2.Town) * (base.IsAscending ? 1 : (-1));
					if (num == 0)
					{
						return base.ResolveEquality(x, y);
					}
					return num;
				}
			}

			// Token: 0x02000784 RID: 1924
			// (Invoke) Token: 0x060056E1 RID: 22241
			protected delegate bool SettlementVisibilityComparerDelegate(Settlement s1, Settlement s2, out int comparisonResult);
		}
	}
}
