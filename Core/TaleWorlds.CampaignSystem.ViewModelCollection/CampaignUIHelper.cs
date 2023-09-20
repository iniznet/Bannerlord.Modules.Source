using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Helpers;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.CraftingSystem;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Encyclopedia;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.Issues;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem.ViewModelCollection
{
	public static class CampaignUIHelper
	{
		private static void TooltipAddPropertyTitleWithValue(List<TooltipProperty> properties, string propertyName, float currentValue)
		{
			string text = currentValue.ToString("0.##");
			properties.Add(new TooltipProperty(propertyName, text, 0, false, TooltipProperty.TooltipPropertyFlags.Title));
		}

		private static void TooltipAddPropertyTitleWithValue(List<TooltipProperty> properties, string propertyName, string currentValue)
		{
			properties.Add(new TooltipProperty(propertyName, currentValue, 0, false, TooltipProperty.TooltipPropertyFlags.Title));
		}

		private static void TooltipAddExplanation(List<TooltipProperty> properties, ref ExplainedNumber explainedNumber)
		{
			List<ValueTuple<string, float>> lines = explainedNumber.GetLines();
			if (lines.Count > 0)
			{
				for (int i = 0; i < lines.Count; i++)
				{
					ValueTuple<string, float> valueTuple = lines[i];
					string item = valueTuple.Item1;
					string changeValueString = CampaignUIHelper.GetChangeValueString(valueTuple.Item2);
					properties.Add(new TooltipProperty(item, changeValueString, 0, false, TooltipProperty.TooltipPropertyFlags.None));
				}
			}
		}

		private static void TooltipAddPropertyTitle(List<TooltipProperty> properties, string propertyName)
		{
			properties.Add(new TooltipProperty(propertyName, string.Empty, 0, false, TooltipProperty.TooltipPropertyFlags.Title));
		}

		private static void TooltipAddExplainedResultChange(List<TooltipProperty> properties, float changeValue)
		{
			string changeValueString = CampaignUIHelper.GetChangeValueString(changeValue);
			properties.Add(new TooltipProperty(CampaignUIHelper._changeStr.ToString(), changeValueString, 0, false, TooltipProperty.TooltipPropertyFlags.RundownResult));
		}

		private static void TooltipAddExplanedChange(List<TooltipProperty> properties, ref ExplainedNumber explainedNumber)
		{
			CampaignUIHelper.TooltipAddExplanation(properties, ref explainedNumber);
			CampaignUIHelper.TooltipAddDoubleSeperator(properties, false);
			CampaignUIHelper.TooltipAddExplainedResultChange(properties, explainedNumber.ResultNumber);
		}

		private static void TooltipAddExplainedResultTotal(List<TooltipProperty> properties, float changeValue)
		{
			string changeValueString = CampaignUIHelper.GetChangeValueString(changeValue);
			properties.Add(new TooltipProperty(CampaignUIHelper._totalStr.ToString(), changeValueString, 0, false, TooltipProperty.TooltipPropertyFlags.RundownResult));
		}

		public static List<TooltipProperty> GetTooltipForAccumulatingProperty(string propertyName, float currentValue, ExplainedNumber explainedNumber)
		{
			List<TooltipProperty> list = new List<TooltipProperty>();
			CampaignUIHelper.TooltipAddPropertyTitleWithValue(list, propertyName, currentValue);
			CampaignUIHelper.TooltipAddExplanedChange(list, ref explainedNumber);
			return list;
		}

		public static List<TooltipProperty> GetTooltipForAccumulatingPropertyWithResult(string propertyName, float currentValue, ref ExplainedNumber explainedNumber)
		{
			List<TooltipProperty> list = new List<TooltipProperty>();
			CampaignUIHelper.TooltipAddPropertyTitle(list, propertyName);
			CampaignUIHelper.TooltipAddExplanation(list, ref explainedNumber);
			CampaignUIHelper.TooltipAddDoubleSeperator(list, false);
			CampaignUIHelper.TooltipAddExplainedResultTotal(list, currentValue);
			return list;
		}

		public static List<TooltipProperty> GetTooltipForgProperty(string propertyName, float currentValue, ExplainedNumber explainedNumber)
		{
			List<TooltipProperty> list = new List<TooltipProperty>();
			CampaignUIHelper.TooltipAddPropertyTitleWithValue(list, propertyName, currentValue);
			CampaignUIHelper.TooltipAddDoubleSeperator(list, false);
			CampaignUIHelper.TooltipAddExplanation(list, ref explainedNumber);
			return list;
		}

		private static void TooltipAddSeperator(List<TooltipProperty> properties, bool onlyShowOnExtend = false)
		{
			properties.Add(new TooltipProperty("", string.Empty, 0, onlyShowOnExtend, TooltipProperty.TooltipPropertyFlags.DefaultSeperator));
		}

		private static void TooltipAddDoubleSeperator(List<TooltipProperty> properties, bool onlyShowOnExtend = false)
		{
			properties.Add(new TooltipProperty("", string.Empty, 0, onlyShowOnExtend, TooltipProperty.TooltipPropertyFlags.RundownSeperator));
		}

		private static void TooltipAddExtendInfo(List<TooltipProperty> properties, bool onlyShowOnExtend = false)
		{
			TooltipProperty tooltipProperty = new TooltipProperty("", "", -1, false, TooltipProperty.TooltipPropertyFlags.None)
			{
				OnlyShowWhenNotExtended = true
			};
			properties.Add(tooltipProperty);
			GameTexts.SetVariable("EXTEND_KEY", Game.Current.GameTextManager.FindText("str_game_key_text", "anyalt").ToString());
			TooltipProperty tooltipProperty2 = new TooltipProperty("", GameTexts.FindText("str_map_tooltip_info", null).ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None)
			{
				OnlyShowWhenNotExtended = true
			};
			properties.Add(tooltipProperty2);
		}

		private static void TooltipAddEmptyLine(List<TooltipProperty> properties, bool onlyShowOnExtend = false)
		{
			properties.Add(new TooltipProperty(string.Empty, string.Empty, -1, onlyShowOnExtend, TooltipProperty.TooltipPropertyFlags.None));
		}

		public static string GetTownWallsTooltip(Town town)
		{
			TextObject textObject;
			bool flag = CampaignUIHelper.IsSettlementInformationHidden(town.Settlement, out textObject);
			GameTexts.SetVariable("newline", "\n");
			if (flag)
			{
				GameTexts.SetVariable("LEVEL", GameTexts.FindText("str_missing_info_indicator", null).ToString());
			}
			else
			{
				GameTexts.SetVariable("LEVEL", town.GetWallLevel());
			}
			return GameTexts.FindText("str_walls_with_value", null).ToString();
		}

		public static List<TooltipProperty> GetVillageMilitiaTooltip(Village village)
		{
			return CampaignUIHelper.GetSettlementPropertyTooltip(village.Settlement, CampaignUIHelper._militiaStr.ToString(), village.Militia, village.MilitiaChangeExplanation);
		}

		public static List<TooltipProperty> GetTownMilitiaTooltip(Town town)
		{
			return CampaignUIHelper.GetSettlementPropertyTooltip(town.Settlement, CampaignUIHelper._militiaStr.ToString(), town.Militia, town.MilitiaChangeExplanation);
		}

		public static List<TooltipProperty> GetTownFoodTooltip(Town town)
		{
			return CampaignUIHelper.GetSettlementPropertyTooltip(town.Settlement, CampaignUIHelper._foodStr.ToString(), town.FoodStocks, town.FoodChangeExplanation);
		}

		public static List<TooltipProperty> GetTownLoyaltyTooltip(Town town)
		{
			TextObject textObject;
			bool flag = CampaignUIHelper.IsSettlementInformationHidden(town.Settlement, out textObject);
			ExplainedNumber loyaltyChangeExplanation = town.LoyaltyChangeExplanation;
			List<TooltipProperty> settlementPropertyTooltip = CampaignUIHelper.GetSettlementPropertyTooltip(town.Settlement, CampaignUIHelper._loyaltyStr.ToString(), town.Loyalty, loyaltyChangeExplanation);
			if (!flag)
			{
				if (!town.OwnerClan.IsRebelClan)
				{
					if (town.Loyalty < (float)Campaign.Current.Models.SettlementLoyaltyModel.RebellionStartLoyaltyThreshold)
					{
						CampaignUIHelper.TooltipAddSeperator(settlementPropertyTooltip, false);
						settlementPropertyTooltip.Add(new TooltipProperty(" ", new TextObject("{=NxEy5Nbt}High risk of rebellion", null).ToString(), 1, UIColors.NegativeIndicator, false, TooltipProperty.TooltipPropertyFlags.None));
					}
					else if (town.Loyalty < (float)Campaign.Current.Models.SettlementLoyaltyModel.RebelliousStateStartLoyaltyThreshold && loyaltyChangeExplanation.ResultNumber < 0f)
					{
						CampaignUIHelper.TooltipAddSeperator(settlementPropertyTooltip, false);
						settlementPropertyTooltip.Add(new TooltipProperty(" ", new TextObject("{=F0a7hyp0}Risk of rebellion", null).ToString(), 1, UIColors.NegativeIndicator, false, TooltipProperty.TooltipPropertyFlags.None));
					}
				}
				else
				{
					CampaignUIHelper.TooltipAddSeperator(settlementPropertyTooltip, false);
					settlementPropertyTooltip.Add(new TooltipProperty(" ", new TextObject("{=hOVPiG3z}Recently rebelled", null).ToString(), 1, UIColors.NegativeIndicator, false, TooltipProperty.TooltipPropertyFlags.None));
				}
			}
			return settlementPropertyTooltip;
		}

		public static List<TooltipProperty> GetTownProsperityTooltip(Town town)
		{
			return CampaignUIHelper.GetSettlementPropertyTooltip(town.Settlement, CampaignUIHelper._prosperityStr.ToString(), town.Prosperity, town.ProsperityChangeExplanation);
		}

		public static List<TooltipProperty> GetTownDailyProductionTooltip(Town town)
		{
			ExplainedNumber constructionExplanation = town.ConstructionExplanation;
			return CampaignUIHelper.GetSettlementPropertyTooltipWithResult(town.Settlement, CampaignUIHelper._dailyProductionStr.ToString(), constructionExplanation.ResultNumber, ref constructionExplanation);
		}

		public static List<TooltipProperty> GetTownSecurityTooltip(Town town)
		{
			ExplainedNumber securityChangeExplanation = town.SecurityChangeExplanation;
			return CampaignUIHelper.GetSettlementPropertyTooltip(town.Settlement, CampaignUIHelper._securityStr.ToString(), town.Security, securityChangeExplanation);
		}

		public static List<TooltipProperty> GetVillageProsperityTooltip(Village village)
		{
			return CampaignUIHelper.GetSettlementPropertyTooltip(village.Settlement, CampaignUIHelper._hearthStr.ToString(), village.Hearth, village.HearthChangeExplanation);
		}

		public static List<TooltipProperty> GetTownGarrisonTooltip(Town town)
		{
			Settlement settlement = town.Settlement;
			string text = CampaignUIHelper._garrisonStr.ToString();
			MobileParty garrisonParty = town.GarrisonParty;
			return CampaignUIHelper.GetSettlementPropertyTooltip(settlement, text, (float)((garrisonParty != null) ? garrisonParty.MemberRoster.TotalManCount : 0), town.GarrisonChangeExplanation);
		}

		public static List<TooltipProperty> GetPartyTroopSizeLimitTooltip(PartyBase party)
		{
			ExplainedNumber partySizeLimitExplainer = party.PartySizeLimitExplainer;
			return CampaignUIHelper.GetTooltipForAccumulatingPropertyWithResult(CampaignUIHelper._partyTroopSizeLimitStr.ToString(), partySizeLimitExplainer.ResultNumber, ref partySizeLimitExplainer);
		}

		public static List<TooltipProperty> GetPartyPrisonerSizeLimitTooltip(PartyBase party)
		{
			ExplainedNumber prisonerSizeLimitExplainer = party.PrisonerSizeLimitExplainer;
			return CampaignUIHelper.GetTooltipForAccumulatingPropertyWithResult(CampaignUIHelper._partyPrisonerSizeLimitStr.ToString(), prisonerSizeLimitExplainer.ResultNumber, ref prisonerSizeLimitExplainer);
		}

		public static List<TooltipProperty> GetUsedHorsesTooltip(List<Tuple<EquipmentElement, int>> usedUpgradeHorsesHistory)
		{
			List<TooltipProperty> list = new List<TooltipProperty>();
			if (usedUpgradeHorsesHistory.Count > 0)
			{
				foreach (IGrouping<ItemCategory, Tuple<EquipmentElement, int>> grouping in from h in usedUpgradeHorsesHistory
					group h by h.Item1.Item.ItemCategory)
				{
					int num = grouping.Sum((Tuple<EquipmentElement, int> c) => c.Item2);
					list.Add(new TooltipProperty(grouping.Key.GetName().ToString(), num.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
				}
			}
			return list;
		}

		public static List<TooltipProperty> GetArmyCohesionTooltip(Army army)
		{
			return CampaignUIHelper.GetTooltipForAccumulatingProperty(CampaignUIHelper._armyCohesionStr.ToString(), army.Cohesion, army.DailyCohesionChangeExplanation);
		}

		public static List<TooltipProperty> GetArmyManCountTooltip(Army army)
		{
			List<TooltipProperty> list = new List<TooltipProperty>();
			if (army.LeaderParty != null)
			{
				list.Add(new TooltipProperty("", CampaignUIHelper._numTotalTroopsInTheArmyStr.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.Title));
				Dictionary<FormationClass, int> dictionary = new Dictionary<FormationClass, int>();
				Dictionary<FormationClass, int> dictionary2 = new Dictionary<FormationClass, int>();
				for (int i = 0; i < army.LeaderParty.MemberRoster.Count; i++)
				{
					TroopRosterElement elementCopyAtIndex = army.LeaderParty.MemberRoster.GetElementCopyAtIndex(i);
					int num;
					dictionary.TryGetValue(elementCopyAtIndex.Character.DefaultFormationClass, out num);
					dictionary[elementCopyAtIndex.Character.DefaultFormationClass] = num + elementCopyAtIndex.WoundedNumber;
					int num2;
					dictionary2.TryGetValue(elementCopyAtIndex.Character.DefaultFormationClass, out num2);
					dictionary2[elementCopyAtIndex.Character.DefaultFormationClass] = num2 + elementCopyAtIndex.Number - elementCopyAtIndex.WoundedNumber;
				}
				int num3 = army.LeaderParty.MemberRoster.TotalManCount;
				foreach (MobileParty mobileParty in army.LeaderParty.AttachedParties)
				{
					for (int j = 0; j < mobileParty.MemberRoster.Count; j++)
					{
						TroopRosterElement elementCopyAtIndex2 = mobileParty.MemberRoster.GetElementCopyAtIndex(j);
						int num4;
						dictionary.TryGetValue(elementCopyAtIndex2.Character.DefaultFormationClass, out num4);
						dictionary[elementCopyAtIndex2.Character.DefaultFormationClass] = num4 + elementCopyAtIndex2.WoundedNumber;
						int num5;
						dictionary2.TryGetValue(elementCopyAtIndex2.Character.DefaultFormationClass, out num5);
						dictionary2[elementCopyAtIndex2.Character.DefaultFormationClass] = num5 + elementCopyAtIndex2.Number - elementCopyAtIndex2.WoundedNumber;
					}
					num3 += mobileParty.MemberRoster.TotalManCount;
				}
				foreach (FormationClass formationClass in FormationClassExtensions.FormationClassValues)
				{
					int num6;
					dictionary.TryGetValue(formationClass, out num6);
					int num7;
					dictionary2.TryGetValue(formationClass, out num7);
					if (num6 + num7 > 0)
					{
						TextObject textObject = new TextObject("{=Dqydb21E} {PARTY_SIZE}", null);
						textObject.SetTextVariable("PARTY_SIZE", PartyBaseHelper.GetPartySizeText(num7, num6, true));
						TextObject textObject2 = GameTexts.FindText("str_troop_type_name", formationClass.GetName());
						list.Add(new TooltipProperty(textObject2.ToString(), textObject.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
					}
				}
				list.Add(new TooltipProperty("", "", 0, false, TooltipProperty.TooltipPropertyFlags.RundownSeperator));
				list.Add(new TooltipProperty(CampaignUIHelper._totalStr.ToString(), num3.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.RundownResult));
			}
			return list;
		}

		public static string GetDaysUntilNoFood(float totalFood, float foodChange)
		{
			if (totalFood <= 1E-45f)
			{
				totalFood = 0f;
			}
			if (foodChange >= -1E-45f)
			{
				return GameTexts.FindText("str_days_until_no_food_never", null).ToString();
			}
			return MathF.Ceiling(MathF.Abs(totalFood / foodChange)).ToString();
		}

		public static List<TooltipProperty> GetSettlementPropertyTooltip(Settlement settlement, string valueName, float value, ExplainedNumber explainedNumber)
		{
			TextObject textObject;
			if (CampaignUIHelper.IsSettlementInformationHidden(settlement, out textObject))
			{
				List<TooltipProperty> list = new List<TooltipProperty>();
				string text = GameTexts.FindText("str_missing_info_indicator", null).ToString();
				CampaignUIHelper.TooltipAddPropertyTitleWithValue(list, valueName, text);
				CampaignUIHelper.TooltipAddEmptyLine(list, false);
				list.Add(new TooltipProperty(string.Empty, textObject.ToString(), -1, false, TooltipProperty.TooltipPropertyFlags.None));
				return list;
			}
			return CampaignUIHelper.GetTooltipForAccumulatingProperty(valueName, value, explainedNumber);
		}

		public static List<TooltipProperty> GetSettlementPropertyTooltipWithResult(Settlement settlement, string valueName, float value, ref ExplainedNumber explainedNumber)
		{
			TextObject textObject;
			if (CampaignUIHelper.IsSettlementInformationHidden(settlement, out textObject))
			{
				List<TooltipProperty> list = new List<TooltipProperty>();
				string text = GameTexts.FindText("str_missing_info_indicator", null).ToString();
				CampaignUIHelper.TooltipAddPropertyTitleWithValue(list, valueName, text);
				CampaignUIHelper.TooltipAddEmptyLine(list, false);
				list.Add(new TooltipProperty(string.Empty, textObject.ToString(), -1, false, TooltipProperty.TooltipPropertyFlags.None));
				return list;
			}
			return CampaignUIHelper.GetTooltipForAccumulatingPropertyWithResult(valueName, value, ref explainedNumber);
		}

		public static List<TooltipProperty> GetArmyFoodTooltip(Army army)
		{
			List<TooltipProperty> list = new List<TooltipProperty>();
			float num = (float)army.LeaderParty.ItemRoster.TotalFood;
			foreach (MobileParty mobileParty in army.LeaderParty.AttachedParties)
			{
				num += (float)mobileParty.ItemRoster.TotalFood;
			}
			list.Add(new TooltipProperty(GameTexts.FindText("str_total_army_food", null).ToString(), CampaignUIHelper.FloatToString(num), 0, false, TooltipProperty.TooltipPropertyFlags.None));
			float num2 = army.LeaderParty.FoodChange;
			foreach (MobileParty mobileParty2 in army.LeaderParty.AttachedParties)
			{
				num2 += mobileParty2.FoodChange;
			}
			list.Add(new TooltipProperty(GameTexts.FindText("str_total_army_food_consumption", null).ToString(), CampaignUIHelper.FloatToString(num2), 0, false, TooltipProperty.TooltipPropertyFlags.None));
			list.Add(new TooltipProperty(GameTexts.FindText("str_total_days_until_no_food", null).ToString(), CampaignUIHelper.GetDaysUntilNoFood(num, num2), 0, false, TooltipProperty.TooltipPropertyFlags.None));
			return list;
		}

		public static string GetClanWealthStatusText(Clan clan)
		{
			string text = string.Empty;
			if (clan.Leader.Gold < 15000)
			{
				text = new TextObject("{=SixPXaNh}Very Poor", null).ToString();
			}
			else if (clan.Leader.Gold < 45000)
			{
				text = new TextObject("{=poorWealthStatus}Poor", null).ToString();
			}
			else if (clan.Leader.Gold < 135000)
			{
				text = new TextObject("{=averageWealthStatus}Average", null).ToString();
			}
			else if (clan.Leader.Gold < 405000)
			{
				text = new TextObject("{=UbRqC0Yz}Rich", null).ToString();
			}
			else
			{
				text = new TextObject("{=oJmRg2ms}Very Rich", null).ToString();
			}
			return text;
		}

		public static List<TooltipProperty> GetClanProsperityTooltip(Clan clan)
		{
			List<TooltipProperty> list = new List<TooltipProperty>
			{
				new TooltipProperty("", GameTexts.FindText("str_prosperity", null).ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.Title)
			};
			int num = 0;
			EncyclopediaPage pageOf = Campaign.Current.EncyclopediaManager.GetPageOf(typeof(Hero));
			for (int i = 0; i < clan.Heroes.Count; i++)
			{
				Hero hero = clan.Heroes[i];
				if (hero.Gold != 0 && hero.IsAlive && hero.Age >= 18f && pageOf.IsValidEncyclopediaItem(hero))
				{
					int num2 = hero.Gold;
					list.Add(new TooltipProperty(hero.Name.ToString(), num2.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
					num += num2;
				}
			}
			for (int j = 0; j < clan.Companions.Count; j++)
			{
				Hero hero2 = clan.Companions[j];
				if (hero2.Gold != 0 && hero2.IsAlive && hero2.Age >= 18f && pageOf.IsValidEncyclopediaItem(hero2))
				{
					int num2 = hero2.Gold;
					list.Add(new TooltipProperty(hero2.Name.ToString(), hero2.Gold.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
					num += num2;
				}
			}
			CampaignUIHelper.TooltipAddDoubleSeperator(list, false);
			list.Add(new TooltipProperty(GameTexts.FindText("str_total_gold", null).ToString(), num.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.RundownResult));
			return list;
		}

		private static List<TooltipProperty> GetDiplomacySettlementStatComparisonTooltip(List<Settlement> settlements, string title, string emptyExplanation = "")
		{
			List<TooltipProperty> list = new List<TooltipProperty>();
			list.Add(new TooltipProperty("", title, 0, false, TooltipProperty.TooltipPropertyFlags.Title));
			if (settlements.Count == 0)
			{
				list.Add(new TooltipProperty(emptyExplanation, "", 0, false, TooltipProperty.TooltipPropertyFlags.None));
				list.Add(new TooltipProperty("", "", -1, false, TooltipProperty.TooltipPropertyFlags.None));
				return list;
			}
			for (int i = 0; i < settlements.Count; i++)
			{
				Settlement settlement = settlements[i];
				list.Add(new TooltipProperty(settlement.Name.ToString(), "", 0, false, TooltipProperty.TooltipPropertyFlags.None));
			}
			list.Add(new TooltipProperty("", "", -1, false, TooltipProperty.TooltipPropertyFlags.None));
			return list;
		}

		public static List<TooltipProperty> GetTruceOwnedSettlementsTooltip(List<Settlement> settlements, TextObject factionName, bool isTown)
		{
			TextObject textObject = (isTown ? new TextObject("{=o79dIa3L}Towns owned by {FACTION}", null) : new TextObject("{=z3Xg0IaG}Castles owned by {FACTION}", null));
			TextObject textObject2 = (isTown ? new TextObject("{=cedvCZ73}There is no town owned by {FACTION}", null) : new TextObject("{=ZZmlYrgL}There is no castle owned by {FACTION}", null));
			textObject.SetTextVariable("FACTION", factionName);
			textObject2.SetTextVariable("FACTION", factionName);
			return CampaignUIHelper.GetDiplomacySettlementStatComparisonTooltip(settlements, textObject.ToString(), textObject2.ToString());
		}

		public static List<TooltipProperty> GetWarSuccessfulRaidsTooltip(List<Settlement> settlements, TextObject factionName)
		{
			TextObject textObject = new TextObject("{=1qm74K2t}Successful raids by {FACTION}", null);
			TextObject textObject2 = new TextObject("{=huqEEfGD}There is no successful raid for {FACTION}", null);
			textObject.SetTextVariable("FACTION", factionName);
			textObject2.SetTextVariable("FACTION", factionName);
			return CampaignUIHelper.GetDiplomacySettlementStatComparisonTooltip(settlements, textObject.ToString(), textObject2.ToString());
		}

		public static List<TooltipProperty> GetWarSuccessfulSiegesTooltip(List<Settlement> settlements, TextObject factionName, bool isTown)
		{
			TextObject textObject = (isTown ? new TextObject("{=mSPyh91Q}Towns conquered by {FACTION}", null) : new TextObject("{=eTxcYvRr}Castles conquered by {FACTION}", null));
			TextObject textObject2 = (isTown ? new TextObject("{=Zemk86FK}There is no town conquered by {FACTION}", null) : new TextObject("{=nKQmaSDO}There is no castle conquered by {FACTION}", null));
			textObject.SetTextVariable("FACTION", factionName);
			textObject2.SetTextVariable("FACTION", factionName);
			return CampaignUIHelper.GetDiplomacySettlementStatComparisonTooltip(settlements, textObject.ToString(), textObject2.ToString());
		}

		public static List<TooltipProperty> GetWarPrisonersTooltip(List<Hero> capturedPrisoners, TextObject factionName)
		{
			List<TooltipProperty> list = new List<TooltipProperty>();
			TextObject textObject = new TextObject("{=8BJDQe6o}Prisoners captured by {FACTION}", null);
			textObject.SetTextVariable("FACTION", factionName);
			list.Add(new TooltipProperty("", textObject.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.Title));
			if (capturedPrisoners.Count == 0)
			{
				TextObject textObject2 = new TextObject("{=CK68QXen}There is no prisoner captured by {FACTION}", null);
				textObject2.SetTextVariable("FACTION", factionName);
				list.Add(new TooltipProperty(textObject2.ToString(), "", 0, false, TooltipProperty.TooltipPropertyFlags.None));
				list.Add(new TooltipProperty("", "", -1, false, TooltipProperty.TooltipPropertyFlags.None));
				return list;
			}
			string text = new TextObject("{=MT4b8H9h}Unknown", null).ToString();
			TextObject textObject3 = new TextObject("{=btoiLePb}{HERO} ({PLACE})", null);
			for (int i = 0; i < capturedPrisoners.Count; i++)
			{
				Hero hero = capturedPrisoners[i];
				PartyBase partyBelongedToAsPrisoner = hero.PartyBelongedToAsPrisoner;
				string text2 = ((partyBelongedToAsPrisoner != null) ? partyBelongedToAsPrisoner.Name.ToString() : null) ?? text;
				textObject3.SetTextVariable("HERO", hero.Name.ToString());
				textObject3.SetTextVariable("PLACE", text2);
				list.Add(new TooltipProperty(textObject3.ToString(), "", 0, false, TooltipProperty.TooltipPropertyFlags.None));
			}
			list.Add(new TooltipProperty("", "", -1, false, TooltipProperty.TooltipPropertyFlags.None));
			return list;
		}

		public static List<TooltipProperty> GetClanStrengthTooltip(Clan clan)
		{
			List<TooltipProperty> list = new List<TooltipProperty>
			{
				new TooltipProperty("", GameTexts.FindText("str_strength", null).ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.Title)
			};
			float num = 0f;
			for (int i = 0; i < MobileParty.AllLordParties.Count; i++)
			{
				MobileParty mobileParty = MobileParty.AllLordParties[i];
				if (mobileParty.ActualClan == clan && !mobileParty.IsDisbanding)
				{
					float totalStrength = mobileParty.Party.TotalStrength;
					list.Add(new TooltipProperty(mobileParty.Name.ToString(), totalStrength.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
					num += totalStrength;
				}
			}
			CampaignUIHelper.TooltipAddDoubleSeperator(list, false);
			list.Add(new TooltipProperty(GameTexts.FindText("str_total_strength", null).ToString(), num.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.RundownResult));
			return list;
		}

		public static List<TooltipProperty> GetCrimeTooltip(Settlement settlement)
		{
			if (settlement.MapFaction == null)
			{
				ExplainedNumber explainedNumber = new ExplainedNumber(0f, true, null);
				return CampaignUIHelper.GetTooltipForAccumulatingProperty(CampaignUIHelper._criminalRatingStr.ToString(), 0f, explainedNumber);
			}
			return CampaignUIHelper.GetTooltipForAccumulatingProperty(CampaignUIHelper._criminalRatingStr.ToString(), settlement.MapFaction.MainHeroCrimeRating, settlement.MapFaction.DailyCrimeRatingChangeExplained);
		}

		public static List<TooltipProperty> GetInfluenceTooltip(Clan clan)
		{
			List<TooltipProperty> tooltipForAccumulatingProperty = CampaignUIHelper.GetTooltipForAccumulatingProperty(CampaignUIHelper._influenceStr.ToString(), clan.Influence, clan.InfluenceChangeExplained);
			if (tooltipForAccumulatingProperty != null && clan.IsUnderMercenaryService)
			{
				tooltipForAccumulatingProperty.Add(new TooltipProperty("", CampaignUIHelper._mercenaryClanInfluenceStr.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
			}
			return tooltipForAccumulatingProperty;
		}

		public static List<TooltipProperty> GetClanRenownTooltip(Clan clan)
		{
			List<TooltipProperty> list = new List<TooltipProperty>();
			TextObject textObject;
			ValueTuple<ExplainedNumber, bool> valueTuple = Campaign.Current.Models.ClanTierModel.HasUpcomingTier(clan, out textObject, true);
			ExplainedNumber item = valueTuple.Item1;
			bool item2 = valueTuple.Item2;
			list.Add(new TooltipProperty(GameTexts.FindText("str_enc_sf_renown", null).ToString(), ((int)clan.Renown).ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
			if (item2)
			{
				CampaignUIHelper.TooltipAddEmptyLine(list, false);
				list.Add(new TooltipProperty(GameTexts.FindText("str_clan_next_tier", null).ToString(), clan.RenownRequirementForNextTier.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
				CampaignUIHelper.TooltipAddEmptyLine(list, false);
				GameTexts.SetVariable("LEFT", GameTexts.FindText("str_clan_tier_bonus", null).ToString());
				TextObject textObject2 = GameTexts.FindText("str_string_newline_string", null);
				textObject2.SetTextVariable("STR1", item.GetExplanations());
				textObject2.SetTextVariable("STR2", textObject);
				list.Add(new TooltipProperty(GameTexts.FindText("str_LEFT_colon_wSpace", null).ToString(), textObject2.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
			}
			return list;
		}

		public static List<TooltipProperty> GetGoldTooltip(Clan clan)
		{
			return CampaignUIHelper.GetTooltipForAccumulatingProperty(CampaignUIHelper._goldStr.ToString(), (float)clan.Gold, Campaign.Current.Models.ClanFinanceModel.CalculateClanGoldChange(clan, true, false, true));
		}

		public static List<TooltipProperty> GetPartyMoraleTooltip(MobileParty mainParty)
		{
			return CampaignUIHelper.GetTooltipForgProperty(CampaignUIHelper._partyMoraleStr.ToString(), mainParty.Morale, mainParty.MoraleExplained);
		}

		public static List<TooltipProperty> GetPartyHealthTooltip(PartyBase party)
		{
			List<TooltipProperty> list = new List<TooltipProperty>();
			CampaignUIHelper.TooltipAddPropertyTitleWithValue(list, CampaignUIHelper._battleReadyTroopsStr.ToString(), (float)party.NumberOfHealthyMembers);
			int num = party.NumberOfAllMembers - party.NumberOfHealthyMembers;
			CampaignUIHelper.TooltipAddPropertyTitleWithValue(list, CampaignUIHelper._woundedTroopsStr.ToString(), (float)num);
			if (num > 0)
			{
				ExplainedNumber healingRateForRegularsExplained = MobileParty.MainParty.HealingRateForRegularsExplained;
				CampaignUIHelper.TooltipAddDoubleSeperator(list, false);
				CampaignUIHelper.TooltipAddPropertyTitleWithValue(list, CampaignUIHelper._regularsHealingRateStr.ToString(), healingRateForRegularsExplained.ResultNumber);
				CampaignUIHelper.TooltipAddSeperator(list, false);
				CampaignUIHelper.TooltipAddExplanation(list, ref healingRateForRegularsExplained);
			}
			return list;
		}

		public static List<TooltipProperty> GetPlayerHitpointsTooltip()
		{
			List<TooltipProperty> list = new List<TooltipProperty>();
			ExplainedNumber maxHitPointsExplanation = Hero.MainHero.CharacterObject.MaxHitPointsExplanation;
			CampaignUIHelper.TooltipAddPropertyTitleWithValue(list, CampaignUIHelper._hitPointsStr.ToString(), (float)Hero.MainHero.HitPoints);
			CampaignUIHelper.TooltipAddSeperator(list, false);
			CampaignUIHelper.TooltipAddPropertyTitleWithValue(list, CampaignUIHelper._maxhitPointsStr.ToString(), maxHitPointsExplanation.ResultNumber);
			CampaignUIHelper.TooltipAddDoubleSeperator(list, false);
			CampaignUIHelper.TooltipAddExplanation(list, ref maxHitPointsExplanation);
			if (Hero.MainHero.HitPoints < Hero.MainHero.MaxHitPoints)
			{
				ExplainedNumber healingRateForHeroesExplained = MobileParty.MainParty.HealingRateForHeroesExplained;
				CampaignUIHelper.TooltipAddDoubleSeperator(list, false);
				CampaignUIHelper.TooltipAddPropertyTitleWithValue(list, CampaignUIHelper._heroesHealingRateStr.ToString(), healingRateForHeroesExplained.ResultNumber);
				CampaignUIHelper.TooltipAddSeperator(list, false);
				CampaignUIHelper.TooltipAddExplanation(list, ref healingRateForHeroesExplained);
			}
			return list;
		}

		public static List<TooltipProperty> GetPartyFoodTooltip(MobileParty mainParty)
		{
			List<TooltipProperty> list = new List<TooltipProperty>();
			float num = ((mainParty.Food > 0f) ? mainParty.Food : 0f);
			CampaignUIHelper.TooltipAddPropertyTitleWithValue(list, CampaignUIHelper._foodStr.ToString(), num);
			ExplainedNumber foodChangeExplained = mainParty.FoodChangeExplained;
			CampaignUIHelper.TooltipAddExplanedChange(list, ref foodChangeExplained);
			CampaignUIHelper.TooltipAddEmptyLine(list, false);
			List<TooltipProperty> list2 = new List<TooltipProperty>();
			int num2 = 0;
			List<TooltipProperty> list3 = new List<TooltipProperty>();
			int num3 = 0;
			for (int i = 0; i < mainParty.ItemRoster.Count; i++)
			{
				ItemRosterElement itemRosterElement = mainParty.ItemRoster[i];
				if (!itemRosterElement.IsEmpty)
				{
					ItemObject item = itemRosterElement.EquipmentElement.Item;
					if (item != null && item.IsFood)
					{
						list2.Add(new TooltipProperty(itemRosterElement.EquipmentElement.GetModifiedItemName().ToString(), itemRosterElement.Amount.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
						num2 += itemRosterElement.Amount;
					}
					else
					{
						ItemObject item2 = itemRosterElement.EquipmentElement.Item;
						bool flag;
						if (item2 == null)
						{
							flag = false;
						}
						else
						{
							HorseComponent horseComponent = item2.HorseComponent;
							bool? flag2 = ((horseComponent != null) ? new bool?(horseComponent.IsLiveStock) : null);
							bool flag3 = true;
							flag = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
						}
						if (flag)
						{
							GameTexts.SetVariable("RANK", itemRosterElement.EquipmentElement.Item.HorseComponent.MeatCount);
							GameTexts.SetVariable("NUMBER", GameTexts.FindText("str_meat", null));
							GameTexts.SetVariable("NUM2", GameTexts.FindText("str_RANK_with_NUM_between_parenthesis", null).ToString());
							GameTexts.SetVariable("NUM1", itemRosterElement.Amount);
							list3.Add(new TooltipProperty(itemRosterElement.EquipmentElement.GetModifiedItemName().ToString(), GameTexts.FindText("str_NUM_times_NUM_with_space", null).ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
							num3 += itemRosterElement.Amount * itemRosterElement.EquipmentElement.Item.HorseComponent.MeatCount;
						}
					}
				}
			}
			if (num2 > 0)
			{
				list.Add(new TooltipProperty(CampaignUIHelper._foodItemsStr.ToString(), num2.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
				CampaignUIHelper.TooltipAddDoubleSeperator(list, false);
				list.AddRange(list2);
				CampaignUIHelper.TooltipAddEmptyLine(list, false);
			}
			if (num3 > 0)
			{
				list.Add(new TooltipProperty(CampaignUIHelper._livestockStr.ToString(), num3.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
				CampaignUIHelper.TooltipAddDoubleSeperator(list, false);
				list.AddRange(list3);
				CampaignUIHelper.TooltipAddEmptyLine(list, false);
			}
			list.Add(new TooltipProperty(GameTexts.FindText("str_total_days_until_no_food", null).ToString(), CampaignUIHelper.GetDaysUntilNoFood(num, foodChangeExplained.ResultNumber), 0, false, TooltipProperty.TooltipPropertyFlags.None));
			return list;
		}

		public static List<TooltipProperty> GetPartySpeedTooltip()
		{
			Game.Current.EventManager.TriggerEvent<PlayerInspectedPartySpeedEvent>(new PlayerInspectedPartySpeedEvent());
			List<TooltipProperty> list = new List<TooltipProperty>();
			if (Hero.MainHero.IsPrisoner)
			{
				list.Add(new TooltipProperty(string.Empty, GameTexts.FindText("str_main_hero_is_imprisoned", null).ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
			}
			else
			{
				ExplainedNumber speedExplained = MobileParty.MainParty.SpeedExplained;
				float resultNumber = speedExplained.ResultNumber;
				list = CampaignUIHelper.GetTooltipForAccumulatingPropertyWithResult(CampaignUIHelper._partySpeedStr.ToString(), resultNumber, ref speedExplained);
			}
			return list;
		}

		public static List<TooltipProperty> GetPartyWageTooltip()
		{
			ExplainedNumber totalWageExplained = MobileParty.MainParty.TotalWageExplained;
			return CampaignUIHelper.GetTooltipForAccumulatingPropertyWithResult(GameTexts.FindText("str_party_wage", null).ToString(), totalWageExplained.ResultNumber, ref totalWageExplained);
		}

		public static List<TooltipProperty> GetPartyWageTooltip(MobileParty mobileParty)
		{
			ExplainedNumber totalWageExplained = mobileParty.TotalWageExplained;
			return CampaignUIHelper.GetTooltipForAccumulatingPropertyWithResult(GameTexts.FindText("str_party_wage", null).ToString(), totalWageExplained.ResultNumber, ref totalWageExplained);
		}

		public static List<TooltipProperty> GetViewDistanceTooltip()
		{
			ExplainedNumber seeingRangeExplanation = MobileParty.MainParty.SeeingRangeExplanation;
			return CampaignUIHelper.GetTooltipForAccumulatingPropertyWithResult(CampaignUIHelper._viewDistanceFoodStr.ToString(), seeingRangeExplanation.ResultNumber, ref seeingRangeExplanation);
		}

		public static List<TooltipProperty> GetMainPartyHealthTooltip()
		{
			PartyBase party = MobileParty.MainParty.Party;
			List<TooltipProperty> list = new List<TooltipProperty>();
			list.Add(new TooltipProperty(CampaignUIHelper._battleReadyTroopsStr.ToString(), party.NumberOfHealthyMembers.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
			CampaignUIHelper.TooltipAddEmptyLine(list, false);
			int num = party.NumberOfAllMembers - party.NumberOfHealthyMembers;
			list.Add(new TooltipProperty(CampaignUIHelper._woundedTroopsStr.ToString(), num.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
			if (num > 0)
			{
				CampaignUIHelper.TooltipAddDoubleSeperator(list, false);
				list.Add(new TooltipProperty(CampaignUIHelper._regularsHealingRateStr.ToString(), MobileParty.MainParty.HealingRateForRegulars.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
				CampaignUIHelper.TooltipAddSeperator(list, false);
				ExplainedNumber healingRateForRegularsExplained = MobileParty.MainParty.HealingRateForRegularsExplained;
				CampaignUIHelper.TooltipAddExplanation(list, ref healingRateForRegularsExplained);
				CampaignUIHelper.TooltipAddEmptyLine(list, false);
			}
			int totalManCount = party.PrisonRoster.TotalManCount;
			if (totalManCount > 0)
			{
				list.Add(new TooltipProperty(CampaignUIHelper._prisonersStr.ToString(), totalManCount.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
			}
			return list;
		}

		public static List<TooltipProperty> GetPartyInventoryCapacityTooltip(MobileParty party)
		{
			List<TooltipProperty> list = new List<TooltipProperty>();
			CampaignUIHelper.TooltipAddPropertyTitleWithValue(list, CampaignUIHelper._partyInventoryCapacityStr.ToString(), (float)party.InventoryCapacity);
			CampaignUIHelper.TooltipAddSeperator(list, false);
			ExplainedNumber inventoryCapacityExplainedNumber = party.InventoryCapacityExplainedNumber;
			CampaignUIHelper.TooltipAddExplanation(list, ref inventoryCapacityExplainedNumber);
			return list;
		}

		public static List<TooltipProperty> GetPerkEffectText(PerkObject perk, bool isActive)
		{
			List<TooltipProperty> list = new List<TooltipProperty>
			{
				new TooltipProperty("", perk.Name.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.Title)
			};
			TextObject perkRoleText = CampaignUIHelper.GetPerkRoleText(perk, false);
			if (perkRoleText != null)
			{
				list.Add(new TooltipProperty("", perkRoleText.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
				list.Add(new TooltipProperty("", perk.PrimaryDescription.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
				list.Add(new TooltipProperty("", "", 0, false, TooltipProperty.TooltipPropertyFlags.None));
			}
			TextObject perkRoleText2 = CampaignUIHelper.GetPerkRoleText(perk, true);
			if (perkRoleText2 != null)
			{
				list.Add(new TooltipProperty("", perkRoleText2.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
				list.Add(new TooltipProperty("", perk.SecondaryDescription.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
				list.Add(new TooltipProperty("", "", 0, false, TooltipProperty.TooltipPropertyFlags.None));
			}
			if (isActive)
			{
				list.Add(new TooltipProperty("", GameTexts.FindText("str_perk_active", null).ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
			}
			list.Add(new TooltipProperty(GameTexts.FindText("str_required_level_perk", null).ToString(), ((int)perk.RequiredSkillValue).ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
			return list;
		}

		public static TextObject GetPerkRoleText(PerkObject perk, bool getSecondary)
		{
			TextObject textObject = null;
			if (!getSecondary && perk.PrimaryRole != SkillEffect.PerkRole.None)
			{
				textObject = GameTexts.FindText("str_perk_one_role", null);
				textObject.SetTextVariable("PRIMARY_ROLE", GameTexts.FindText("role", perk.PrimaryRole.ToString()));
			}
			else if (getSecondary && perk.SecondaryRole != SkillEffect.PerkRole.None)
			{
				textObject = GameTexts.FindText("str_perk_one_role", null);
				textObject.SetTextVariable("PRIMARY_ROLE", GameTexts.FindText("role", perk.SecondaryRole.ToString()));
			}
			return textObject;
		}

		public static TextObject GetCombinedPerkRoleText(PerkObject perk)
		{
			TextObject textObject = null;
			if (perk.PrimaryRole != SkillEffect.PerkRole.None && perk.SecondaryRole != SkillEffect.PerkRole.None)
			{
				textObject = GameTexts.FindText("str_perk_two_roles", null);
				textObject.SetTextVariable("PRIMARY_ROLE", GameTexts.FindText("role", perk.PrimaryRole.ToString()));
				textObject.SetTextVariable("SECONDARY_ROLE", GameTexts.FindText("role", perk.SecondaryRole.ToString()));
			}
			else if (perk.PrimaryRole != SkillEffect.PerkRole.None)
			{
				textObject = GameTexts.FindText("str_perk_one_role", null);
				textObject.SetTextVariable("PRIMARY_ROLE", GameTexts.FindText("role", perk.PrimaryRole.ToString()));
			}
			return textObject;
		}

		public static List<TooltipProperty> GetSiegeMachineTooltip(SiegeEngineType engineType, bool showDescription = true, int hoursUntilCompletion = 0)
		{
			List<TooltipProperty> list = new List<TooltipProperty>();
			if (showDescription)
			{
				GameTexts.SetVariable("NEWLINE", "\n");
			}
			string text = (showDescription ? GameTexts.FindText("str_siege_weapon_tooltip_text", engineType.StringId).ToString() : engineType.Name.ToString());
			list.Add(new TooltipProperty(" ", text, 0, false, TooltipProperty.TooltipPropertyFlags.None));
			if (hoursUntilCompletion > 0)
			{
				TooltipProperty siegeMachineProgressLine = CampaignUIHelper.GetSiegeMachineProgressLine(hoursUntilCompletion);
				if (siegeMachineProgressLine != null)
				{
					list.Add(siegeMachineProgressLine);
				}
			}
			return list;
		}

		public static string GetSiegeMachineName(SiegeEngineType engineType)
		{
			if (engineType != null)
			{
				return engineType.Name.ToString();
			}
			return "";
		}

		public static string GetSiegeMachineNameWithDesctiption(SiegeEngineType engineType)
		{
			if (engineType != null)
			{
				return GameTexts.FindText("str_siege_weapon_tooltip_text", engineType.StringId).ToString();
			}
			return "";
		}

		public static List<TooltipProperty> GetTroopConformityTooltip(TroopRosterElement troop)
		{
			List<TooltipProperty> list = new List<TooltipProperty>();
			if (troop.Character != null)
			{
				string text = PartyBase.MainParty.PrisonRoster.GetElementXp(troop.Character).ToString();
				int conformityNeededToRecruitPrisoner = troop.Character.ConformityNeededToRecruitPrisoner;
				list.Add(new TooltipProperty(GameTexts.FindText("str_party_troop_current_conformity", null).ToString(), text, 0, false, TooltipProperty.TooltipPropertyFlags.None));
				string text2 = "CONFORMITY_AMOUNT";
				int num = conformityNeededToRecruitPrisoner;
				GameTexts.SetVariable(text2, num.ToString());
				list.Add(new TooltipProperty("", GameTexts.FindText("str_party_troop_conformity_explanation", null).ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.MultiLine));
			}
			return list;
		}

		public static List<TooltipProperty> GetLearningRateTooltip(int attributeValue, int focusValue, int skillValue, int characterLevel, TextObject attributeName)
		{
			ExplainedNumber explainedNumber = Campaign.Current.Models.CharacterDevelopmentModel.CalculateLearningRate(attributeValue, focusValue, skillValue, characterLevel, attributeName, true);
			return CampaignUIHelper.GetTooltipForAccumulatingPropertyWithResult(CampaignUIHelper._learningRateStr.ToString(), explainedNumber.ResultNumber, ref explainedNumber);
		}

		public static List<TooltipProperty> GetTroopXPTooltip(TroopRosterElement troop)
		{
			List<TooltipProperty> list = new List<TooltipProperty>();
			if (troop.Character != null && troop.Character.UpgradeTargets.Length != 0)
			{
				int upgradeXpCost = troop.Character.GetUpgradeXpCost(PartyBase.MainParty, 0);
				list.Add(new TooltipProperty(GameTexts.FindText("str_party_troop_current_xp", null).ToString(), (troop.Xp % upgradeXpCost).ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
				list.Add(new TooltipProperty(GameTexts.FindText("str_party_troop_upgrade_xp_cost", null).ToString(), upgradeXpCost.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
				int num = upgradeXpCost - troop.Xp % upgradeXpCost;
				GameTexts.SetVariable("XP_AMOUNT", num);
				list.Add(new TooltipProperty("", GameTexts.FindText("str_party_troop_xp_explanation", null).ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.MultiLine));
			}
			return list;
		}

		public static List<TooltipProperty> GetLearningLimitTooltip(int attributeValue, int focusValue, TextObject attributeName)
		{
			ExplainedNumber explainedNumber = Campaign.Current.Models.CharacterDevelopmentModel.CalculateLearningLimit(attributeValue, focusValue, attributeName, true);
			return CampaignUIHelper.GetTooltipForAccumulatingPropertyWithResult(CampaignUIHelper._learningLimitStr.ToString(), explainedNumber.ResultNumber, ref explainedNumber);
		}

		public static List<TooltipProperty> GetSettlementConsumptionTooltip(Settlement settlement)
		{
			List<TooltipProperty> list = new List<TooltipProperty>();
			list.Add(new TooltipProperty("", GameTexts.FindText("str_consumption", null).ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.Title));
			if (settlement.IsTown)
			{
				using (IEnumerator<Town.SellLog> enumerator = settlement.Town.SoldItems.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Town.SellLog sellLog = enumerator.Current;
						list.Add(new TooltipProperty(sellLog.Category.GetName().ToString(), sellLog.Number.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
					}
					return list;
				}
			}
			Debug.FailedAssert("Only towns' consumptions are tracked", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem.ViewModelCollection\\CampaignUIHelper.cs", "GetSettlementConsumptionTooltip", 1116);
			return list;
		}

		public static StringItemWithHintVM GetCharacterTierData(CharacterObject character, bool isBig = false)
		{
			int tier = character.Tier;
			if (tier <= 0 || tier > 7)
			{
				return new StringItemWithHintVM("", TextObject.Empty);
			}
			string text = (isBig ? (tier.ToString() + "_big") : tier.ToString());
			string text2 = "General\\TroopTierIcons\\icon_tier_" + text;
			GameTexts.SetVariable("TIER_LEVEL", tier);
			TextObject textObject = new TextObject("{=!}" + GameTexts.FindText("str_party_troop_tier", null).ToString(), null);
			return new StringItemWithHintVM(text2, textObject);
		}

		public static List<TooltipProperty> GetSettlementProductionTooltip(Settlement settlement)
		{
			List<TooltipProperty> list = new List<TooltipProperty>();
			list.Add(new TooltipProperty("", GameTexts.FindText("str_production", null).ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.Title));
			if (settlement.IsFortification)
			{
				list.Add(new TooltipProperty(GameTexts.FindText("str_villages", null).ToString(), " ", 0, false, TooltipProperty.TooltipPropertyFlags.None));
				CampaignUIHelper.TooltipAddDoubleSeperator(list, false);
				for (int i = 0; i < settlement.BoundVillages.Count; i++)
				{
					Village village = settlement.BoundVillages[i];
					list.Add(new TooltipProperty(village.Name.ToString(), village.VillageType.PrimaryProduction.Name.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
				}
				list.Add(new TooltipProperty(GameTexts.FindText("str_shops_in_town", null).ToString(), " ", 0, false, TooltipProperty.TooltipPropertyFlags.None));
				CampaignUIHelper.TooltipAddDoubleSeperator(list, false);
				using (IEnumerator<Workshop> enumerator = settlement.Town.Workshops.Where((Workshop w) => w.WorkshopType != null && !w.WorkshopType.IsHidden).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Workshop workshop = enumerator.Current;
						list.Add(new TooltipProperty(" ", workshop.WorkshopType.Name.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
					}
					return list;
				}
			}
			if (settlement.IsVillage)
			{
				list.Add(new TooltipProperty(GameTexts.FindText("str_production_in_village", null).ToString(), " ", 0, false, TooltipProperty.TooltipPropertyFlags.None));
				CampaignUIHelper.TooltipAddDoubleSeperator(list, false);
				for (int j = 0; j < settlement.Village.VillageType.Productions.Count; j++)
				{
					ValueTuple<ItemObject, float> valueTuple = settlement.Village.VillageType.Productions[j];
					list.Add(new TooltipProperty(" ", valueTuple.Item1.Name.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
				}
			}
			return list;
		}

		public static string GetHintTextFromReasons(List<TextObject> reasons)
		{
			TextObject textObject = TextObject.Empty;
			for (int i = 0; i < reasons.Count; i++)
			{
				if (i >= 1)
				{
					GameTexts.SetVariable("STR1", textObject.ToString());
					GameTexts.SetVariable("STR2", reasons[i]);
					textObject = GameTexts.FindText("str_string_newline_string", null);
				}
				else
				{
					textObject = reasons[i];
				}
			}
			return textObject.ToString();
		}

		public static TextObject GetHoursAndDaysTextFromHourValue(int hours)
		{
			TextObject textObject = TextObject.Empty;
			if (hours > 0)
			{
				int num = hours / 24;
				int num2 = hours % 24;
				textObject = ((num > 0) ? ((num2 > 0) ? GameTexts.FindText("str_days_hours", null) : GameTexts.FindText("str_days", null)) : GameTexts.FindText("str_hours", null));
				textObject.SetTextVariable("DAY", num);
				textObject.SetTextVariable("PLURAL_DAYS", (num > 1) ? 1 : 0);
				textObject.SetTextVariable("HOUR", num2);
				textObject.SetTextVariable("PLURAL_HOURS", (num2 > 1) ? 1 : 0);
			}
			return textObject;
		}

		public static TextObject GetTeleportationDelayText(Hero hero, PartyBase target)
		{
			TextObject textObject = TextObject.Empty;
			if (hero != null && target != null)
			{
				float resultNumber = Campaign.Current.Models.DelayedTeleportationModel.GetTeleportationDelayAsHours(hero, target).ResultNumber;
				if (hero.IsTraveling)
				{
					textObject = CampaignUIHelper._travelingText.CopyTextObject();
				}
				else if (resultNumber > 0f)
				{
					TextObject textObject2 = new TextObject("{=P0To9aRW}Travel time: {TRAVEL_TIME}", null);
					textObject2.SetTextVariable("TRAVEL_TIME", CampaignUIHelper.GetHoursAndDaysTextFromHourValue((int)Math.Ceiling((double)resultNumber)));
					textObject = textObject2;
				}
				else
				{
					textObject = CampaignUIHelper._noDelayText.CopyTextObject();
				}
			}
			return textObject;
		}

		public static List<TooltipProperty> GetTimeOfDayAndResetCameraTooltip()
		{
			List<TooltipProperty> list = new List<TooltipProperty>();
			int getHourOfDay = CampaignTime.Now.GetHourOfDay;
			TextObject textObject = TextObject.Empty;
			if (getHourOfDay >= 6 && getHourOfDay < 12)
			{
				textObject = new TextObject("{=X3gcUz7C}Morning", null);
			}
			else if (getHourOfDay >= 12 && getHourOfDay < 15)
			{
				textObject = new TextObject("{=CTtjSwRb}Noon", null);
			}
			else if (getHourOfDay >= 15 && getHourOfDay < 18)
			{
				textObject = new TextObject("{=J2gvnexb}Afternoon", null);
			}
			else if (getHourOfDay >= 18 && getHourOfDay < 22)
			{
				textObject = new TextObject("{=gENb9SSW}Evening", null);
			}
			else
			{
				textObject = new TextObject("{=fAxjyMt5}Night", null);
			}
			list.Add(new TooltipProperty(textObject.ToString(), "", 0, false, TooltipProperty.TooltipPropertyFlags.None));
			list.Add(new TooltipProperty("", new TextObject("{=sFiU3Ss2}Click to Reset Camera", null).ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
			return list;
		}

		public static List<TooltipProperty> GetTournamentChampionRewardsTooltip(Hero hero, Town town)
		{
			List<TooltipProperty> list = new List<TooltipProperty>();
			CampaignUIHelper.TooltipAddPropertyTitle(list, new TextObject("{=CGVK6l8I}Champion Benefits", null).ToString());
			TextObject textObject = new TextObject("{=4vZLpzPi}+1 Renown / Day", null);
			list.Add(new TooltipProperty(textObject.ToString(), "", 0, false, TooltipProperty.TooltipPropertyFlags.None));
			return list;
		}

		public static StringItemWithHintVM GetCharacterTypeData(CharacterObject character, bool isBig = false)
		{
			if (character.IsHero)
			{
				return new StringItemWithHintVM("", TextObject.Empty);
			}
			TextObject textObject = TextObject.Empty;
			string text;
			if (character.IsRanged && character.IsMounted)
			{
				text = (isBig ? "horse_archer_big" : "horse_archer");
				textObject = GameTexts.FindText("str_troop_type_name", "HorseArcher");
			}
			else if (character.IsRanged)
			{
				text = (isBig ? "bow_big" : "bow");
				textObject = GameTexts.FindText("str_troop_type_name", "Ranged");
			}
			else if (character.IsMounted)
			{
				text = (isBig ? "cavalry_big" : "cavalry");
				textObject = GameTexts.FindText("str_troop_type_name", "Cavalry");
			}
			else
			{
				if (!character.IsInfantry)
				{
					return new StringItemWithHintVM("", TextObject.Empty);
				}
				text = (isBig ? "infantry_big" : "infantry");
				textObject = GameTexts.FindText("str_troop_type_name", "Infantry");
			}
			return new StringItemWithHintVM("General\\TroopTypeIcons\\icon_troop_type_" + text, new TextObject("{=!}" + textObject.ToString(), null));
		}

		public static List<TooltipProperty> GetHeroHealthTooltip(Hero hero)
		{
			List<TooltipProperty> list = new List<TooltipProperty>();
			GameTexts.SetVariable("LEFT", hero.HitPoints.ToString("0.##"));
			GameTexts.SetVariable("RIGHT", hero.MaxHitPoints.ToString("0.##"));
			list.Add(new TooltipProperty(CampaignUIHelper._hitPointsStr.ToString(), GameTexts.FindText("str_LEFT_over_RIGHT", null).ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.Title));
			CampaignUIHelper.TooltipAddSeperator(list, false);
			CampaignUIHelper.TooltipAddPropertyTitleWithValue(list, CampaignUIHelper._maxhitPointsStr.ToString(), (float)hero.MaxHitPoints);
			CampaignUIHelper.TooltipAddDoubleSeperator(list, false);
			ExplainedNumber maxHitPointsExplanation = hero.CharacterObject.MaxHitPointsExplanation;
			CampaignUIHelper.TooltipAddExplanation(list, ref maxHitPointsExplanation);
			return list;
		}

		public static List<TooltipProperty> GetSiegeWallTooltip(int wallLevel, int wallHitpoints)
		{
			return new List<TooltipProperty>
			{
				new TooltipProperty(GameTexts.FindText("str_map_tooltip_wall_level", null).ToString(), wallLevel.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None),
				new TooltipProperty(GameTexts.FindText("str_map_tooltip_wall_hitpoints", null).ToString(), wallHitpoints.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None)
			};
		}

		public static List<TooltipProperty> GetGovernorPerksTooltipForHero(Hero hero)
		{
			List<TooltipProperty> list = new List<TooltipProperty>();
			list.Add(new TooltipProperty(GameTexts.FindText("str_clan_governor_perks", null).ToString(), " ", 0, false, TooltipProperty.TooltipPropertyFlags.None));
			CampaignUIHelper.TooltipAddSeperator(list, false);
			List<PerkObject> governorPerksForHero = PerkHelper.GetGovernorPerksForHero(hero);
			for (int i = 0; i < governorPerksForHero.Count; i++)
			{
				if (governorPerksForHero[i].PrimaryRole == SkillEffect.PerkRole.Governor)
				{
					list.Add(new TooltipProperty(governorPerksForHero[i].Name.ToString(), governorPerksForHero[i].PrimaryDescription.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
				}
				else if (governorPerksForHero[i].SecondaryRole == SkillEffect.PerkRole.Governor)
				{
					list.Add(new TooltipProperty(governorPerksForHero[i].Name.ToString(), governorPerksForHero[i].SecondaryDescription.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
				}
			}
			return list;
		}

		[return: TupleElementNames(new string[] { "titleText", "bodyText" })]
		public static ValueTuple<TextObject, TextObject> GetGovernorSelectionConfirmationPopupTexts(Hero currentGovernor, Hero newGovernor, Settlement settlement)
		{
			if (settlement != null)
			{
				bool flag = newGovernor == null;
				DelayedTeleportationModel delayedTeleportationModel = Campaign.Current.Models.DelayedTeleportationModel;
				int num = ((!flag) ? ((int)Math.Ceiling((double)delayedTeleportationModel.GetTeleportationDelayAsHours(newGovernor, settlement.Party).ResultNumber)) : 0);
				MBTextManager.SetTextVariable("TRAVEL_DURATION", CampaignUIHelper.GetHoursAndDaysTextFromHourValue(num).ToString(), false);
				CharacterObject characterObject = (flag ? ((currentGovernor != null) ? currentGovernor.CharacterObject : null) : ((newGovernor != null) ? newGovernor.CharacterObject : null));
				if (characterObject != null)
				{
					StringHelpers.SetCharacterProperties("GOVERNOR", characterObject, null, false);
				}
				string text = "SETTLEMENT_NAME";
				TextObject name = settlement.Name;
				MBTextManager.SetTextVariable(text, ((name != null) ? name.ToString() : null) ?? string.Empty, false);
				TextObject textObject = GameTexts.FindText(flag ? "str_clan_remove_governor" : "str_clan_assign_governor", null);
				TextObject textObject2 = GameTexts.FindText(flag ? "str_remove_governor_inquiry" : ((num == 0) ? "str_change_governor_instantly_inquiry" : "str_change_governor_inquiry"), null);
				return new ValueTuple<TextObject, TextObject>(textObject, textObject2);
			}
			return new ValueTuple<TextObject, TextObject>(TextObject.Empty, TextObject.Empty);
		}

		public static List<TooltipProperty> GetHeroGovernorEffectsTooltip(Hero hero, Settlement settlement)
		{
			List<TooltipProperty> list = new List<TooltipProperty>
			{
				new TooltipProperty("", hero.Name.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.Title)
			};
			list.Add(new TooltipProperty(string.Empty, CampaignUIHelper.GetTeleportationDelayText(hero, settlement.Party).ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
			MBTextManager.SetTextVariable("LEFT", GameTexts.FindText("str_tooltip_label_relation", null), false);
			string text = GameTexts.FindText("str_LEFT_ONLY", null).ToString();
			list.Add(new TooltipProperty(text, ((int)hero.GetRelationWithPlayer()).ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
			MBTextManager.SetTextVariable("LEFT", GameTexts.FindText("str_tooltip_label_type", null), false);
			string text2 = GameTexts.FindText("str_LEFT_ONLY", null).ToString();
			list.Add(new TooltipProperty(text2, HeroHelper.GetCharacterTypeName(hero).ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
			MobileParty partyBelongedTo = hero.PartyBelongedTo;
			SkillEffect.PerkRole? perkRole = ((partyBelongedTo != null) ? new SkillEffect.PerkRole?(partyBelongedTo.GetHeroPerkRole(hero)) : null);
			if (perkRole != null)
			{
				SkillEffect.PerkRole? perkRole2 = perkRole;
				SkillEffect.PerkRole perkRole3 = SkillEffect.PerkRole.None;
				if (!((perkRole2.GetValueOrDefault() == perkRole3) & (perkRole2 != null)))
				{
					TextObject textObject = GameTexts.FindText("role", perkRole.Value.ToString());
					list.Add(new TooltipProperty(new TextObject("{=9FJi2SaE}Party Role", null).ToString(), textObject.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
				}
			}
			CampaignUIHelper.TooltipAddEmptyLine(list, false);
			list.Add(new TooltipProperty(new TextObject("{=J8ddrAOf}Governor Effects", null).ToString(), " ", 0, false, TooltipProperty.TooltipPropertyFlags.None));
			CampaignUIHelper.TooltipAddSeperator(list, false);
			ValueTuple<TextObject, TextObject> governorEngineeringSkillEffectForHero = PerkHelper.GetGovernorEngineeringSkillEffectForHero(hero);
			list.Add(new TooltipProperty(governorEngineeringSkillEffectForHero.Item1.ToString(), governorEngineeringSkillEffectForHero.Item2.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
			CampaignUIHelper.TooltipAddEmptyLine(list, false);
			List<TooltipProperty> governorPerksTooltipForHero = CampaignUIHelper.GetGovernorPerksTooltipForHero(hero);
			list.AddRange(governorPerksTooltipForHero);
			return list;
		}

		public static List<TooltipProperty> GetEncounterPartyMoraleTooltip(List<MobileParty> parties)
		{
			return new List<TooltipProperty>();
		}

		public static TextObject GetCraftingTemplatePieceUnlockProgressHint(float progress)
		{
			TextObject textObject = GameTexts.FindText("str_LEFT_over_RIGHT_in_paranthesis", null);
			textObject.SetTextVariable("LEFT", progress.ToString("F0"));
			textObject.SetTextVariable("RIGHT", "100");
			TextObject textObject2 = new TextObject("{=opU0Nr2G}Progress for unlocking a new piece.", null);
			TextObject textObject3 = GameTexts.FindText("str_STR1_space_STR2", null);
			textObject3.SetTextVariable("STR1", textObject2);
			textObject3.SetTextVariable("STR2", textObject);
			return textObject3;
		}

		public static List<ValueTuple<string, TextObject>> GetWeaponFlagDetails(WeaponFlags weaponFlags, CharacterObject character = null)
		{
			List<ValueTuple<string, TextObject>> list = new List<ValueTuple<string, TextObject>>();
			if (weaponFlags.HasAnyFlag(WeaponFlags.BonusAgainstShield))
			{
				string text = "WeaponFlagIcons\\bonus_against_shield";
				TextObject textObject = GameTexts.FindText("str_inventory_flag_bonus_against_shield", null);
				list.Add(new ValueTuple<string, TextObject>(text, textObject));
			}
			if (weaponFlags.HasAnyFlag(WeaponFlags.CanKnockDown))
			{
				string text = "WeaponFlagIcons\\can_knock_down";
				TextObject textObject = GameTexts.FindText("str_inventory_flag_can_knockdown", null);
				list.Add(new ValueTuple<string, TextObject>(text, textObject));
			}
			if (weaponFlags.HasAnyFlag(WeaponFlags.CanDismount))
			{
				string text = "WeaponFlagIcons\\can_dismount";
				TextObject textObject = GameTexts.FindText("str_inventory_flag_can_dismount", null);
				list.Add(new ValueTuple<string, TextObject>(text, textObject));
			}
			if (weaponFlags.HasAnyFlag(WeaponFlags.CanHook))
			{
				string text = "WeaponFlagIcons\\can_dismount";
				TextObject textObject = GameTexts.FindText("str_inventory_flag_can_hook", null);
				list.Add(new ValueTuple<string, TextObject>(text, textObject));
			}
			if (weaponFlags.HasAnyFlag(WeaponFlags.CanCrushThrough))
			{
				string text = "WeaponFlagIcons\\can_crush_through";
				TextObject textObject = GameTexts.FindText("str_inventory_flag_can_crush_through", null);
				list.Add(new ValueTuple<string, TextObject>(text, textObject));
			}
			if (weaponFlags.HasAnyFlag(WeaponFlags.NotUsableWithTwoHand))
			{
				string text = "WeaponFlagIcons\\not_usable_with_two_hand";
				TextObject textObject = GameTexts.FindText("str_inventory_flag_not_usable_two_hand", null);
				list.Add(new ValueTuple<string, TextObject>(text, textObject));
			}
			if (weaponFlags.HasAnyFlag(WeaponFlags.NotUsableWithOneHand))
			{
				string text = "WeaponFlagIcons\\not_usable_with_one_hand";
				TextObject textObject = GameTexts.FindText("str_inventory_flag_not_usable_one_hand", null);
				list.Add(new ValueTuple<string, TextObject>(text, textObject));
			}
			if (weaponFlags.HasAnyFlag(WeaponFlags.CantReloadOnHorseback) && (character == null || !character.GetPerkValue(DefaultPerks.Crossbow.MountedCrossbowman)))
			{
				string text = "WeaponFlagIcons\\cant_reload_on_horseback";
				TextObject textObject = GameTexts.FindText("str_inventory_flag_cant_reload_on_horseback", null);
				list.Add(new ValueTuple<string, TextObject>(text, textObject));
			}
			return list;
		}

		public static List<Tuple<string, TextObject>> GetItemFlagDetails(ItemFlags itemFlags)
		{
			List<Tuple<string, TextObject>> list = new List<Tuple<string, TextObject>>();
			if (itemFlags.HasAnyFlag(ItemFlags.Civilian))
			{
				string text = "GeneralFlagIcons\\civillian";
				TextObject textObject = GameTexts.FindText("str_inventory_flag_civillian", null);
				list.Add(new Tuple<string, TextObject>(text, textObject));
			}
			return list;
		}

		public static List<ValueTuple<string, TextObject>> GetItemUsageSetFlagDetails(ItemObject.ItemUsageSetFlags flags, CharacterObject character = null)
		{
			List<ValueTuple<string, TextObject>> list = new List<ValueTuple<string, TextObject>>();
			if (flags.HasAnyFlag(ItemObject.ItemUsageSetFlags.RequiresNoMount) && (character == null || !character.GetPerkValue(DefaultPerks.Bow.MountedArchery)))
			{
				list.Add(new ValueTuple<string, TextObject>("WeaponFlagIcons\\cant_use_on_horseback", GameTexts.FindText("str_inventory_flag_cant_use_with_mounts", null)));
			}
			if (flags.HasAnyFlag(ItemObject.ItemUsageSetFlags.RequiresNoShield))
			{
				list.Add(new ValueTuple<string, TextObject>("WeaponFlagIcons\\cant_use_with_shields", GameTexts.FindText("str_inventory_flag_cant_use_with_shields", null)));
			}
			return list;
		}

		public static List<ValueTuple<string, TextObject>> GetFlagDetailsForWeapon(WeaponComponentData weapon, ItemObject.ItemUsageSetFlags itemUsageFlags, CharacterObject character = null)
		{
			List<ValueTuple<string, TextObject>> list = new List<ValueTuple<string, TextObject>>();
			if (weapon == null)
			{
				return list;
			}
			if (weapon.RelevantSkill == DefaultSkills.Bow)
			{
				list.Add(new ValueTuple<string, TextObject>("WeaponFlagIcons\\bow", GameTexts.FindText("str_inventory_flag_bow", null)));
			}
			if (weapon.RelevantSkill == DefaultSkills.Crossbow)
			{
				list.Add(new ValueTuple<string, TextObject>("WeaponFlagIcons\\crossbow", GameTexts.FindText("str_inventory_flag_crossbow", null)));
			}
			if (weapon.RelevantSkill == DefaultSkills.Polearm)
			{
				list.Add(new ValueTuple<string, TextObject>("WeaponFlagIcons\\polearm", GameTexts.FindText("str_inventory_flag_polearm", null)));
			}
			if (weapon.RelevantSkill == DefaultSkills.OneHanded)
			{
				list.Add(new ValueTuple<string, TextObject>("WeaponFlagIcons\\one_handed", GameTexts.FindText("str_inventory_flag_one_handed", null)));
			}
			if (weapon.RelevantSkill == DefaultSkills.TwoHanded)
			{
				list.Add(new ValueTuple<string, TextObject>("WeaponFlagIcons\\two_handed", GameTexts.FindText("str_inventory_flag_two_handed", null)));
			}
			if (weapon.RelevantSkill == DefaultSkills.Throwing)
			{
				list.Add(new ValueTuple<string, TextObject>("WeaponFlagIcons\\throwing", GameTexts.FindText("str_inventory_flag_throwing", null)));
			}
			List<ValueTuple<string, TextObject>> weaponFlagDetails = CampaignUIHelper.GetWeaponFlagDetails(weapon.WeaponFlags, character);
			list.AddRange(weaponFlagDetails);
			List<ValueTuple<string, TextObject>> itemUsageSetFlagDetails = CampaignUIHelper.GetItemUsageSetFlagDetails(itemUsageFlags, character);
			list.AddRange(itemUsageSetFlagDetails);
			string weaponDescriptionId = weapon.WeaponDescriptionId;
			if (weaponDescriptionId != null && weaponDescriptionId.IndexOf("couch", StringComparison.OrdinalIgnoreCase) >= 0)
			{
				list.Add(new ValueTuple<string, TextObject>("WeaponFlagIcons\\can_couchable", GameTexts.FindText("str_inventory_flag_couchable", null)));
			}
			string weaponDescriptionId2 = weapon.WeaponDescriptionId;
			if (weaponDescriptionId2 != null && weaponDescriptionId2.IndexOf("bracing", StringComparison.OrdinalIgnoreCase) >= 0)
			{
				list.Add(new ValueTuple<string, TextObject>("WeaponFlagIcons\\braceable", GameTexts.FindText("str_inventory_flag_braceable", null)));
			}
			return list;
		}

		public static List<TooltipProperty> GetCraftingHeroTooltip(Hero hero, CraftingOrder order)
		{
			object obj = order != null && !order.IsOrderAvailableForHero(hero);
			ICraftingCampaignBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<ICraftingCampaignBehavior>();
			List<TooltipProperty> list = new List<TooltipProperty>();
			object obj2 = obj;
			string text = ((obj2 != null) ? GameTexts.FindText("str_crafting_hero_can_not_craft_item", null).ToString() : hero.Name.ToString());
			CampaignUIHelper.TooltipAddPropertyTitle(list, text);
			if (obj2 != null)
			{
				List<Hero> list2 = (from h in CraftingHelper.GetAvailableHeroesForCrafting()
					where order.IsOrderAvailableForHero(h)
					select h).ToList<Hero>();
				if (list2.Count > 0)
				{
					list.Add(new TooltipProperty(GameTexts.FindText("str_crafting_hero_not_enough_skills", null).ToString(), "", 0, false, TooltipProperty.TooltipPropertyFlags.None));
					CampaignUIHelper.TooltipAddEmptyLine(list, false);
					list.Add(new TooltipProperty(GameTexts.FindText("str_crafting_following_can_craft_order", null).ToString(), "", 0, false, TooltipProperty.TooltipPropertyFlags.None));
					for (int i = 0; i < list2.Count; i++)
					{
						Hero hero2 = list2[i];
						GameTexts.SetVariable("HERO_NAME", hero2.Name);
						list.Add(new TooltipProperty(GameTexts.FindText("str_crafting_hero_able_to_craft", null).ToString(), "", 0, false, TooltipProperty.TooltipPropertyFlags.None));
					}
				}
				else
				{
					list.Add(new TooltipProperty(GameTexts.FindText("str_crafting_no_one_can_craft_order", null).ToString(), "", 0, false, TooltipProperty.TooltipPropertyFlags.None));
				}
			}
			else
			{
				list.Add(new TooltipProperty(new TextObject("{=cUUI8u2G}Smithy Stamina", null).ToString(), campaignBehavior.GetHeroCraftingStamina(hero).ToString() + " / " + campaignBehavior.GetMaxHeroCraftingStamina(hero).ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
				list.Add(new TooltipProperty(new TextObject("{=lVuGCYPC}Smithing Skill", null).ToString(), hero.GetSkillValue(DefaultSkills.Crafting).ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
			}
			return list;
		}

		public static List<TooltipProperty> GetOrderCannotBeCompletedReasonTooltip(CraftingOrder order, ItemObject item)
		{
			CampaignUIHelper.<>c__DisplayClass143_0 CS$<>8__locals1;
			CS$<>8__locals1.order = order;
			CS$<>8__locals1.properties = new List<TooltipProperty>();
			CampaignUIHelper.TooltipAddPropertyTitle(CS$<>8__locals1.properties, new TextObject("{=Syha8biz}Order Can Not Be Completed", null).ToString());
			CS$<>8__locals1.properties.Add(new TooltipProperty(new TextObject("{=gTbE6t9I}Following requirements are not met:", null).ToString(), "", 0, false, TooltipProperty.TooltipPropertyFlags.None));
			if (CS$<>8__locals1.order.PreCraftedWeaponDesignItem.PrimaryWeapon.SwingDamageType != item.PrimaryWeapon.SwingDamageType)
			{
				DamageTypes swingDamageType = CS$<>8__locals1.order.PreCraftedWeaponDesignItem.PrimaryWeapon.SwingDamageType;
				int thrustDamageType = (int)item.PrimaryWeapon.ThrustDamageType;
				TextObject textObject = TextObject.Empty;
				if (thrustDamageType == -1)
				{
					textObject = TextObject.Empty;
				}
				else
				{
					textObject = new TextObject("{=MT5A04X8} - Swing Damage Type does not match. Should be: {TYPE}", null);
					TextObject textObject2 = textObject;
					string text = "TYPE";
					string text2 = "str_inventory_dmg_type";
					int i = (int)swingDamageType;
					textObject2.SetTextVariable(text, GameTexts.FindText(text2, i.ToString()));
				}
				CS$<>8__locals1.properties.Add(new TooltipProperty(textObject.ToString(), "", 0, false, TooltipProperty.TooltipPropertyFlags.None));
			}
			if (CS$<>8__locals1.order.PreCraftedWeaponDesignItem.PrimaryWeapon.ThrustDamageType != item.PrimaryWeapon.ThrustDamageType)
			{
				DamageTypes thrustDamageType2 = CS$<>8__locals1.order.PreCraftedWeaponDesignItem.PrimaryWeapon.ThrustDamageType;
				int thrustDamageType3 = (int)item.PrimaryWeapon.ThrustDamageType;
				TextObject textObject3 = TextObject.Empty;
				if (thrustDamageType3 == -1)
				{
					textObject3 = TextObject.Empty;
				}
				else
				{
					textObject3 = new TextObject("{=Tx9Mynbt} - Thrust Damage Type does not match. Should be: {TYPE}", null);
					TextObject textObject4 = textObject3;
					string text3 = "TYPE";
					string text4 = "str_inventory_dmg_type";
					int i = (int)thrustDamageType2;
					textObject4.SetTextVariable(text3, GameTexts.FindText(text4, i.ToString()).ToString());
				}
				CS$<>8__locals1.properties.Add(new TooltipProperty(textObject3.ToString(), "", 0, false, TooltipProperty.TooltipPropertyFlags.None));
			}
			float num = (float)CS$<>8__locals1.order.PreCraftedWeaponDesignItem.PrimaryWeapon.ThrustSpeed;
			float num2 = (float)item.PrimaryWeapon.ThrustSpeed;
			if (num > num2)
			{
				CampaignUIHelper.<GetOrderCannotBeCompletedReasonTooltip>g__AddProperty|143_0(CraftingTemplate.CraftingStatTypes.ThrustSpeed, num, ref CS$<>8__locals1);
			}
			num = (float)CS$<>8__locals1.order.PreCraftedWeaponDesignItem.PrimaryWeapon.SwingSpeed;
			num2 = (float)item.PrimaryWeapon.SwingSpeed;
			if (num > num2)
			{
				CampaignUIHelper.<GetOrderCannotBeCompletedReasonTooltip>g__AddProperty|143_0(CraftingTemplate.CraftingStatTypes.SwingSpeed, num, ref CS$<>8__locals1);
			}
			num = (float)CS$<>8__locals1.order.PreCraftedWeaponDesignItem.PrimaryWeapon.MissileSpeed;
			num2 = (float)item.PrimaryWeapon.MissileSpeed;
			if (num > num2)
			{
				CampaignUIHelper.<GetOrderCannotBeCompletedReasonTooltip>g__AddProperty|143_0(CraftingTemplate.CraftingStatTypes.MissileSpeed, num, ref CS$<>8__locals1);
			}
			num = (float)CS$<>8__locals1.order.PreCraftedWeaponDesignItem.PrimaryWeapon.ThrustDamage;
			num2 = (float)item.PrimaryWeapon.ThrustDamage;
			if (num > num2)
			{
				CampaignUIHelper.<GetOrderCannotBeCompletedReasonTooltip>g__AddProperty|143_0(CraftingTemplate.CraftingStatTypes.ThrustDamage, num, ref CS$<>8__locals1);
			}
			num = (float)CS$<>8__locals1.order.PreCraftedWeaponDesignItem.PrimaryWeapon.SwingDamage;
			num2 = (float)item.PrimaryWeapon.SwingDamage;
			if (num > num2)
			{
				CampaignUIHelper.<GetOrderCannotBeCompletedReasonTooltip>g__AddProperty|143_0(CraftingTemplate.CraftingStatTypes.SwingDamage, num, ref CS$<>8__locals1);
			}
			num = (float)CS$<>8__locals1.order.PreCraftedWeaponDesignItem.PrimaryWeapon.Accuracy;
			num2 = (float)item.PrimaryWeapon.Accuracy;
			if (num > num2)
			{
				CampaignUIHelper.<GetOrderCannotBeCompletedReasonTooltip>g__AddProperty|143_0(CraftingTemplate.CraftingStatTypes.Accuracy, num, ref CS$<>8__locals1);
			}
			num = (float)CS$<>8__locals1.order.PreCraftedWeaponDesignItem.PrimaryWeapon.Handling;
			num2 = (float)item.PrimaryWeapon.Handling;
			if (num > num2)
			{
				CampaignUIHelper.<GetOrderCannotBeCompletedReasonTooltip>g__AddProperty|143_0(CraftingTemplate.CraftingStatTypes.Handling, num, ref CS$<>8__locals1);
			}
			bool flag = true;
			WeaponDescription[] weaponDescriptions = CS$<>8__locals1.order.PreCraftedWeaponDesignItem.WeaponDesign.Template.WeaponDescriptions;
			for (int i = 0; i < weaponDescriptions.Length; i++)
			{
				WeaponDescription weaponDescription = weaponDescriptions[i];
				if (item.WeaponDesign.Template.WeaponDescriptions.All((WeaponDescription d) => d.WeaponClass != weaponDescription.WeaponClass))
				{
					flag = false;
					break;
				}
			}
			if (!flag)
			{
				CS$<>8__locals1.properties.Add(new TooltipProperty(new TextObject("{=Q1KwpZYu}Weapon usage does not match requirements", null).ToString(), "", 0, false, TooltipProperty.TooltipPropertyFlags.None));
			}
			return CS$<>8__locals1.properties;
		}

		public static List<TooltipProperty> GetCraftingOrderDisabledReasonTooltip(Hero heroToCheck, CraftingOrder order)
		{
			List<TooltipProperty> list = new List<TooltipProperty>();
			if (order.IsOrderAvailableForHero(heroToCheck))
			{
				return list;
			}
			GameTexts.SetVariable("SKILL", GameTexts.FindText("str_crafting", null).ToString());
			CampaignUIHelper.TooltipAddPropertyTitle(list, GameTexts.FindText("str_crafting_cannot_be_crafted", null).ToString());
			if (!order.IsOrderAvailableForHero(heroToCheck))
			{
				List<Hero> list2 = (from h in CraftingHelper.GetAvailableHeroesForCrafting()
					where order.IsOrderAvailableForHero(h)
					select h).ToList<Hero>();
				if (list2.Count > 0)
				{
					MathF.Ceiling(order.OrderDifficulty);
					heroToCheck.GetSkillValue(DefaultSkills.Crafting);
					GameTexts.SetVariable("HERO", heroToCheck.Name.ToString());
					list.Add(new TooltipProperty(GameTexts.FindText("str_crafting_player_not_enough_skills", null).ToString(), "", 0, false, TooltipProperty.TooltipPropertyFlags.None));
					CampaignUIHelper.TooltipAddEmptyLine(list, false);
					list.Add(new TooltipProperty(GameTexts.FindText("str_crafting_following_can_craft_order", null).ToString(), "", 0, false, TooltipProperty.TooltipPropertyFlags.None));
					for (int i = 0; i < list2.Count; i++)
					{
						Hero hero = list2[i];
						GameTexts.SetVariable("HERO_NAME", hero.Name);
						list.Add(new TooltipProperty(GameTexts.FindText("str_crafting_hero_able_to_craft", null).ToString(), "", 0, false, TooltipProperty.TooltipPropertyFlags.None));
					}
				}
				else
				{
					float num = order.OrderDifficulty / 1.1f - (float)heroToCheck.GetSkillValue(DefaultSkills.Crafting);
					GameTexts.SetVariable("AMOUNT", MathF.Round(num));
					list.Add(new TooltipProperty(GameTexts.FindText("str_crafting_no_one_can_craft_order", null).ToString(), "", 0, false, TooltipProperty.TooltipPropertyFlags.None));
				}
			}
			return list;
		}

		public static string GetCraftingOrderMissingPropertyWarningText(CraftingOrder order, ItemObject craftedItem)
		{
			if (order == null)
			{
				return string.Empty;
			}
			bool flag = true;
			bool flag2 = true;
			WeaponComponentData statWeapon = order.GetStatWeapon();
			WeaponComponentData weaponComponentData = null;
			for (int i = 0; i < craftedItem.Weapons.Count; i++)
			{
				if (craftedItem.Weapons[i].WeaponDescriptionId == statWeapon.WeaponDescriptionId)
				{
					weaponComponentData = craftedItem.Weapons[i];
					break;
				}
			}
			if (weaponComponentData == null)
			{
				weaponComponentData = craftedItem.PrimaryWeapon;
			}
			string text = string.Empty;
			if (statWeapon.SwingDamageType != DamageTypes.Invalid && statWeapon.SwingDamageType != weaponComponentData.SwingDamageType)
			{
				flag = false;
				text = GameTexts.FindText("str_damage_types", statWeapon.SwingDamageType.ToString()).ToString();
			}
			if (statWeapon.ThrustDamageType != DamageTypes.Invalid && statWeapon.ThrustDamageType != weaponComponentData.ThrustDamageType)
			{
				flag2 = false;
				text = GameTexts.FindText("str_damage_types", statWeapon.ThrustDamageType.ToString()).ToString();
			}
			if (!flag)
			{
				return GameTexts.FindText("str_crafting_should_have_swing_damage", null).SetTextVariable("SWING_DAMAGE_TYPE", text).ToString();
			}
			if (!flag2)
			{
				return GameTexts.FindText("str_crafting_should_have_thrust_damage", null).SetTextVariable("THRUST_DAMAGE_TYPE", text).ToString();
			}
			return string.Empty;
		}

		public static List<TooltipProperty> GetInventoryCharacterTooltip(Hero hero)
		{
			List<TooltipProperty> list = new List<TooltipProperty>();
			CampaignUIHelper._inventorySkillTooltipTitle.SetTextVariable("HERO_NAME", hero.Name);
			CampaignUIHelper.TooltipAddPropertyTitle(list, CampaignUIHelper._inventorySkillTooltipTitle.ToString());
			CampaignUIHelper.TooltipAddDoubleSeperator(list, false);
			for (int i = 0; i < Skills.All.Count; i++)
			{
				SkillObject skillObject = Skills.All[i];
				list.Add(new TooltipProperty(skillObject.Name.ToString(), hero.GetSkillValue(skillObject).ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
			}
			return list;
		}

		public static string GetHeroOccupationName(Hero hero)
		{
			string text;
			if (hero.IsWanderer)
			{
				text = "str_wanderer";
			}
			else if (hero.IsGangLeader)
			{
				text = "str_gang_leader";
			}
			else if (hero.IsPreacher)
			{
				text = "str_preacher";
			}
			else if (hero.IsMerchant)
			{
				text = "str_merchant";
			}
			else
			{
				Clan clan = hero.Clan;
				if (clan != null && clan.IsClanTypeMercenary)
				{
					text = "str_mercenary";
				}
				else if (hero.IsArtisan)
				{
					text = "str_artisan";
				}
				else if (hero.IsRuralNotable)
				{
					text = "str_charactertype_ruralnotable";
				}
				else if (hero.IsHeadman)
				{
					text = "str_charactertype_headman";
				}
				else if (hero.IsMinorFactionHero)
				{
					text = "str_charactertype_minorfaction";
				}
				else
				{
					if (!hero.IsLord)
					{
						return "";
					}
					if (hero.IsFemale)
					{
						text = "str_charactertype_lady";
					}
					else
					{
						text = "str_charactertype_lord";
					}
				}
			}
			return GameTexts.FindText(text, null).ToString();
		}

		private static TooltipProperty GetSiegeMachineProgressLine(int hoursRemaining)
		{
			if (hoursRemaining > 0)
			{
				string text = CampaignUIHelper.GetHoursAndDaysTextFromHourValue(hoursRemaining).ToString();
				MBTextManager.SetTextVariable("PREPARATION_TIME", text, false);
				string text2 = GameTexts.FindText("str_preparations_complete_in_hours", null).ToString();
				return new TooltipProperty(" ", text2, 0, false, TooltipProperty.TooltipPropertyFlags.None);
			}
			return null;
		}

		public static TextObject GetCommaSeparatedText(TextObject label, IEnumerable<TextObject> texts)
		{
			TextObject textObject = new TextObject("{=!}{RESULT}", null);
			int num = 0;
			foreach (TextObject textObject2 in texts)
			{
				if (num == 0)
				{
					MBTextManager.SetTextVariable("STR1", label ?? TextObject.Empty, false);
					MBTextManager.SetTextVariable("STR2", textObject2, false);
					string text = GameTexts.FindText("str_STR1_STR2", null).ToString();
					MBTextManager.SetTextVariable("LEFT", text, false);
					textObject.SetTextVariable("RESULT", text);
				}
				else
				{
					MBTextManager.SetTextVariable("RIGHT", textObject2, false);
					string text2 = GameTexts.FindText("str_LEFT_comma_RIGHT", null).ToString();
					MBTextManager.SetTextVariable("LEFT", text2, false);
					textObject.SetTextVariable("RESULT", text2);
				}
				num++;
			}
			return textObject;
		}

		public static string GetHeroKingdomRank(Hero hero)
		{
			if (hero.Clan.Kingdom != null)
			{
				bool isUnderMercenaryService = hero.Clan.IsUnderMercenaryService;
				bool flag = hero == hero.Clan.Kingdom.Leader;
				bool flag2 = hero.Clan.Leader == hero;
				bool flag3 = !flag && !flag2;
				bool flag4 = hero.PartyBelongedTo != null && hero.PartyBelongedTo.LeaderHero == hero;
				TextObject textObject = TextObject.Empty;
				GameTexts.SetVariable("FACTION", hero.Clan.Kingdom.Name);
				if (flag)
				{
					textObject = GameTexts.FindText("str_hero_rank_of_faction", 1.ToString());
				}
				else if (isUnderMercenaryService)
				{
					textObject = GameTexts.FindText("str_hero_rank_of_faction_mercenary", null);
				}
				else if (flag2 || flag4)
				{
					textObject = GameTexts.FindText("str_hero_rank_of_faction", 0.ToString());
				}
				else if (flag3)
				{
					textObject = GameTexts.FindText("str_hero_rank_of_faction_nobleman", null);
				}
				textObject.SetCharacterProperties("HERO", hero.CharacterObject, false);
				return textObject.ToString();
			}
			return string.Empty;
		}

		public static string GetHeroRank(Hero hero)
		{
			if (hero.Clan != null)
			{
				bool isUnderMercenaryService = hero.Clan.IsUnderMercenaryService;
				Kingdom kingdom = hero.Clan.Kingdom;
				bool flag = hero == ((kingdom != null) ? kingdom.Leader : null);
				bool flag2 = hero.Clan.Leader == hero && hero.Clan.Kingdom != null;
				bool flag3 = !flag && !flag2 && hero.Clan.Kingdom != null;
				if (flag)
				{
					return GameTexts.FindText("str_hero_rank", 1.ToString()).ToString();
				}
				if (isUnderMercenaryService)
				{
					return GameTexts.FindText("str_hero_rank_mercenary", null).ToString();
				}
				if (flag2)
				{
					return GameTexts.FindText("str_hero_rank", 0.ToString()).ToString();
				}
				if (flag3)
				{
					return GameTexts.FindText("str_hero_rank_nobleman", null).ToString();
				}
			}
			return string.Empty;
		}

		public static bool IsSettlementInformationHidden(Settlement settlement, out TextObject disableReason)
		{
			bool flag = !Campaign.Current.Models.InformationRestrictionModel.DoesPlayerKnowDetailsOf(settlement);
			disableReason = (flag ? new TextObject("{=cDkHJOkl}You need to be in the viewing range, control this settlement with your kingdom or have a clan member in the settlement to see its details.", null) : TextObject.Empty);
			return flag;
		}

		public static bool IsHeroInformationHidden(Hero hero, out TextObject disableReason)
		{
			bool flag = !Campaign.Current.Models.InformationRestrictionModel.DoesPlayerKnowDetailsOf(hero);
			disableReason = (flag ? new TextObject("{=akHsjtPh}You haven't met this hero yet.", null) : TextObject.Empty);
			return flag;
		}

		public static string GetPartyNameplateText(MobileParty party)
		{
			int num = party.MemberRoster.TotalHealthyCount;
			int num2 = party.MemberRoster.TotalWounded;
			if (party.Army != null && party.Army.LeaderParty == party)
			{
				for (int i = 0; i < party.Army.LeaderParty.AttachedParties.Count; i++)
				{
					MobileParty mobileParty = party.Army.LeaderParty.AttachedParties[i];
					num += mobileParty.MemberRoster.TotalHealthyCount;
					num2 += mobileParty.MemberRoster.TotalWounded;
				}
			}
			string abbreviatedValueTextFromValue = CampaignUIHelper.GetAbbreviatedValueTextFromValue(num);
			string abbreviatedValueTextFromValue2 = CampaignUIHelper.GetAbbreviatedValueTextFromValue(num2);
			return abbreviatedValueTextFromValue + ((num2 > 0) ? (" + " + abbreviatedValueTextFromValue2 + GameTexts.FindText("str_party_nameplate_wounded_abbr", null).ToString()) : "");
		}

		public static string GetPartyNameplateText(PartyBase party)
		{
			int totalHealthyCount = party.MemberRoster.TotalHealthyCount;
			int totalWounded = party.MemberRoster.TotalWounded;
			string abbreviatedValueTextFromValue = CampaignUIHelper.GetAbbreviatedValueTextFromValue(totalHealthyCount);
			string abbreviatedValueTextFromValue2 = CampaignUIHelper.GetAbbreviatedValueTextFromValue(totalWounded);
			return abbreviatedValueTextFromValue + ((totalWounded > 0) ? (" + " + abbreviatedValueTextFromValue2 + GameTexts.FindText("str_party_nameplate_wounded_abbr", null).ToString()) : "");
		}

		public static string GetUpgradeHint(int index, int numOfItems, int availableUpgrades, int upgradeCoinCost, bool hasRequiredPerk, PerkObject requiredPerk, CharacterObject character, TroopRosterElement troop, int partyGoldChangeAmount, string entireStackShortcutKeyText, string fiveStackShortcutKeyText)
		{
			string text = null;
			CharacterObject characterObject = character.UpgradeTargets[index];
			int level = characterObject.Level;
			if (character.Culture.IsBandit ? (level >= character.Level) : (level > character.Level))
			{
				int upgradeXpCost = character.GetUpgradeXpCost(PartyBase.MainParty, index);
				GameTexts.SetVariable("newline", "\n");
				TextObject textObject = new TextObject("{=f4nc7FfE}Upgrade to {UPGRADE_NAME}", null);
				textObject.SetTextVariable("UPGRADE_NAME", characterObject.Name);
				text = textObject.ToString();
				if (troop.Xp < upgradeXpCost)
				{
					TextObject textObject2 = new TextObject("{=Voa0sinH}Required: {NEEDED_EXP_AMOUNT}xp (You have {CURRENT_EXP_AMOUNT})", null);
					textObject2.SetTextVariable("NEEDED_EXP_AMOUNT", upgradeXpCost);
					textObject2.SetTextVariable("CURRENT_EXP_AMOUNT", troop.Xp);
					GameTexts.SetVariable("STR1", text);
					GameTexts.SetVariable("STR2", textObject2);
					text = GameTexts.FindText("str_string_newline_string", null).ToString();
				}
				if (characterObject.UpgradeRequiresItemFromCategory != null)
				{
					TextObject textObject3 = new TextObject((numOfItems > 0) ? "{=Raa4j4rF}Required: {UPGRADE_ITEM}" : "{=rThSy9ed}Required: {UPGRADE_ITEM} (You have none)", null);
					textObject3.SetTextVariable("UPGRADE_ITEM", characterObject.UpgradeRequiresItemFromCategory.GetName().ToString());
					GameTexts.SetVariable("STR1", text);
					GameTexts.SetVariable("STR2", textObject3.ToString());
					text = GameTexts.FindText("str_string_newline_string", null).ToString();
				}
				TextObject textObject4 = new TextObject((Hero.MainHero.Gold + partyGoldChangeAmount < upgradeCoinCost) ? "{=63Ic1Ahe}Cost: {UPGRADE_COST} (You don't have)" : "{=McJjNM50}Cost: {UPGRADE_COST}", null);
				textObject4.SetTextVariable("UPGRADE_COST", upgradeCoinCost);
				GameTexts.SetVariable("STR1", textObject4);
				GameTexts.SetVariable("STR2", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">");
				string text2 = GameTexts.FindText("str_STR1_STR2", null).ToString();
				GameTexts.SetVariable("STR1", text);
				GameTexts.SetVariable("STR2", text2);
				text = GameTexts.FindText("str_string_newline_string", null).ToString();
				if (!hasRequiredPerk)
				{
					GameTexts.SetVariable("STR1", text);
					TextObject textObject5 = new TextObject("{=68IlDbA2}You need to have {PERK_NAME} perk to upgrade a bandit troop to a normal troop.", null);
					textObject5.SetTextVariable("PERK_NAME", requiredPerk.Name);
					GameTexts.SetVariable("STR2", textObject5);
					text = GameTexts.FindText("str_string_newline_string", null).ToString();
				}
				GameTexts.SetVariable("STR2", "");
				if (availableUpgrades > 0 && !string.IsNullOrEmpty(entireStackShortcutKeyText))
				{
					GameTexts.SetVariable("KEY_NAME", entireStackShortcutKeyText);
					string text3 = GameTexts.FindText("str_entire_stack_shortcut_upgrade_units", null).ToString();
					GameTexts.SetVariable("STR1", text3);
					GameTexts.SetVariable("STR2", "");
					if (availableUpgrades >= 5 && !string.IsNullOrEmpty(fiveStackShortcutKeyText))
					{
						GameTexts.SetVariable("KEY_NAME", fiveStackShortcutKeyText);
						string text4 = GameTexts.FindText("str_five_stack_shortcut_upgrade_units", null).ToString();
						GameTexts.SetVariable("STR2", text4);
					}
					string text5 = GameTexts.FindText("str_string_newline_string", null).ToString();
					GameTexts.SetVariable("STR2", text5);
				}
				GameTexts.SetVariable("STR1", text);
				text = GameTexts.FindText("str_string_newline_string", null).ToString();
			}
			return text;
		}

		public static string ConvertToHexColor(uint color)
		{
			uint num = color % 4278190080U;
			return "#" + Convert.ToString((long)((ulong)num), 16).PadLeft(6, '0').ToUpper() + "FF";
		}

		public static bool GetMapScreenActionIsEnabledWithReason(out TextObject disabledReason)
		{
			if (Hero.MainHero.IsPrisoner)
			{
				disabledReason = GameTexts.FindText("str_action_disabled_reason_prisoner", null);
				return false;
			}
			GameStateManager gameStateManager = GameStateManager.Current;
			bool flag;
			if (gameStateManager == null)
			{
				flag = false;
			}
			else
			{
				flag = gameStateManager.GameStates.Any((GameState x) => x.IsMission);
			}
			if (flag)
			{
				disabledReason = new TextObject("{=FdzsOvDq}This action is disabled while in a mission", null);
				return false;
			}
			if (PlayerEncounter.Current != null)
			{
				if (PlayerEncounter.EncounterSettlement == null)
				{
					disabledReason = GameTexts.FindText("str_action_disabled_reason_encounter", null);
					return false;
				}
				Village village = PlayerEncounter.EncounterSettlement.Village;
				if (village != null && village.VillageState == Village.VillageStates.BeingRaided)
				{
					MapEvent mapEvent = MobileParty.MainParty.MapEvent;
					if (mapEvent != null && mapEvent.IsRaid)
					{
						disabledReason = GameTexts.FindText("str_action_disabled_reason_raid", null);
						return false;
					}
				}
				if (PlayerEncounter.EncounterSettlement.IsUnderSiege)
				{
					disabledReason = GameTexts.FindText("str_action_disabled_reason_siege", null);
					return false;
				}
			}
			if (PlayerSiege.PlayerSiegeEvent != null)
			{
				disabledReason = GameTexts.FindText("str_action_disabled_reason_siege", null);
				return false;
			}
			if (MobileParty.MainParty.MapEvent != null)
			{
				disabledReason = new TextObject("{=MIylzRc5}You can't perform this action while you are in a map event.", null);
				return false;
			}
			disabledReason = TextObject.Empty;
			return true;
		}

		public static string GetClanSupportDisableReasonString(bool hasEnoughInfluence, bool isTargetMainClan, bool isMainClanMercenary)
		{
			if (Hero.MainHero.IsPrisoner)
			{
				return GameTexts.FindText("str_action_disabled_reason_prisoner", null).ToString();
			}
			if (isTargetMainClan)
			{
				return GameTexts.FindText("str_cannot_support_your_clan", null).ToString();
			}
			if (isMainClanMercenary)
			{
				return GameTexts.FindText("str_mercenaries_cannot_support_clans", null).ToString();
			}
			if (!hasEnoughInfluence)
			{
				return GameTexts.FindText("str_warning_you_dont_have_enough_influence", null).ToString();
			}
			return null;
		}

		public static string GetClanExpelDisableReasonString(bool hasEnoughInfluence, bool isTargetMainClan, bool isTargetRulingClan, bool isMainClanMercenary)
		{
			if (Hero.MainHero.IsPrisoner)
			{
				return GameTexts.FindText("str_action_disabled_reason_prisoner", null).ToString();
			}
			if (isMainClanMercenary)
			{
				return GameTexts.FindText("str_mercenaries_cannot_expel_clans", null).ToString();
			}
			if (isTargetMainClan)
			{
				return GameTexts.FindText("str_cannot_expel_your_clan", null).ToString();
			}
			if (isTargetRulingClan)
			{
				return GameTexts.FindText("str_cannot_expel_ruling_clan", null).ToString();
			}
			if (!hasEnoughInfluence)
			{
				return GameTexts.FindText("str_warning_you_dont_have_enough_influence", null).ToString();
			}
			return null;
		}

		public static string GetArmyDisbandDisableReasonString(bool hasEnoughInfluence, bool isArmyInAnyEvent, bool isPlayerClanMercenary, bool isPlayerInThisArmy)
		{
			if (Hero.MainHero.IsPrisoner)
			{
				return GameTexts.FindText("str_action_disabled_reason_prisoner", null).ToString();
			}
			if (isPlayerClanMercenary)
			{
				return GameTexts.FindText("str_cannot_disband_army_while_mercenary", null).ToString();
			}
			if (isArmyInAnyEvent)
			{
				return GameTexts.FindText("str_cannot_disband_army_while_in_event", null).ToString();
			}
			if (isPlayerInThisArmy)
			{
				return GameTexts.FindText("str_cannot_disband_army_while_in_that_army", null).ToString();
			}
			if (!hasEnoughInfluence)
			{
				return GameTexts.FindText("str_warning_you_dont_have_enough_influence", null).ToString();
			}
			return null;
		}

		public static TextObject GetCreateNewPartyReasonString(bool haveEmptyPartySlots, bool haveAvailableHero)
		{
			if (Hero.MainHero.IsPrisoner)
			{
				return GameTexts.FindText("str_action_disabled_reason_prisoner", null);
			}
			if (!haveEmptyPartySlots)
			{
				return GameTexts.FindText("str_clan_doesnt_have_empty_party_slots", null);
			}
			if (!haveAvailableHero)
			{
				return GameTexts.FindText("str_clan_doesnt_have_available_heroes", null);
			}
			return TextObject.Empty;
		}

		public static string GetCraftingDisableReasonString(bool playerHasEnoughMaterials)
		{
			if (!playerHasEnoughMaterials)
			{
				return GameTexts.FindText("str_warning_crafing_materials", null).ToString();
			}
			return string.Empty;
		}

		public static string GetAddFocusHintString(bool playerHasEnoughPoints, bool isMaxedSkill, int currentFocusAmount, int currentAttributeAmount, int currentSkillValue, IHeroDeveloper developer, SkillObject skill)
		{
			GameTexts.SetVariable("newline", "\n");
			string text = GameTexts.FindText("str_focus_points", null).ToString();
			TextObject textObject = new TextObject("{=j3iwQmoA}Current focus amount: {CURRENT_AMOUNT}", null);
			textObject.SetTextVariable("CURRENT_AMOUNT", currentFocusAmount);
			GameTexts.SetVariable("STR1", text);
			GameTexts.SetVariable("STR2", textObject);
			text = GameTexts.FindText("str_string_newline_string", null).ToString();
			if (!playerHasEnoughPoints)
			{
				GameTexts.SetVariable("STR1", text);
				GameTexts.SetVariable("STR2", GameTexts.FindText("str_player_doesnt_have_enough_points", null));
				return GameTexts.FindText("str_string_newline_string", null).ToString();
			}
			if (isMaxedSkill)
			{
				GameTexts.SetVariable("STR1", text);
				GameTexts.SetVariable("STR2", GameTexts.FindText("str_player_cannot_give_more_points", null));
				return GameTexts.FindText("str_string_newline_string", null).ToString();
			}
			GameTexts.SetVariable("COST", 1);
			GameTexts.SetVariable("STR1", text);
			GameTexts.SetVariable("STR2", GameTexts.FindText("str_cost_COUNT", null));
			return GameTexts.FindText("str_string_newline_string", null).ToString();
		}

		public static string GetSkillEffectText(SkillEffect effect, int skillLevel)
		{
			MBTextManager.SetTextVariable("a0", effect.GetPrimaryValue(skillLevel).ToString("0.0"), false);
			MBTextManager.SetTextVariable("a1", effect.GetSecondaryValue(skillLevel).ToString("0.0"), false);
			string text = effect.Description.ToString();
			if (effect.PrimaryRole == SkillEffect.PerkRole.None || effect.PrimaryBonus == 0f)
			{
				return text;
			}
			TextObject textObject = GameTexts.FindText("role", effect.PrimaryRole.ToString());
			if (effect.SecondaryRole != SkillEffect.PerkRole.None && effect.SecondaryBonus != 0f)
			{
				TextObject textObject2 = GameTexts.FindText("role", effect.SecondaryRole.ToString());
				return string.Format("({0} / {1}){2} ", textObject.ToString(), textObject2.ToString(), text);
			}
			return string.Format("({0}) {1} ", textObject.ToString(), text);
		}

		public static string GetMobilePartyBehaviorText(MobileParty party)
		{
			TextObject textObject;
			if (party.Army != null && party.Army.LeaderParty == party && !party.Ai.IsFleeing())
			{
				textObject = party.Army.GetBehaviorText(false);
			}
			else if (party.DefaultBehavior == AiBehavior.Hold || (party.IsMainParty && Campaign.Current.IsMainPartyWaiting))
			{
				textObject = new TextObject("{=RClxLG6N}Holding", null);
			}
			else if (party.ShortTermBehavior == AiBehavior.EngageParty && party.ShortTermTargetParty != null)
			{
				textObject = new TextObject("{=5bzk75Ql}Engaging {TARGET_PARTY}.", null);
				textObject.SetTextVariable("TARGET_PARTY", party.ShortTermTargetParty.Name);
			}
			else if (party.DefaultBehavior == AiBehavior.GoAroundParty && party.ShortTermBehavior == AiBehavior.GoToPoint)
			{
				textObject = new TextObject("{=XYAVu2f0}Chasing {TARGET_PARTY}.", null);
				textObject.SetTextVariable("TARGET_PARTY", party.TargetParty.Name);
			}
			else if (party.ShortTermBehavior == AiBehavior.FleeToParty && party.ShortTermTargetParty != null)
			{
				textObject = new TextObject("{=R8vuwKaf}Running from {TARGET_PARTY} to ally party.", null);
				textObject.SetTextVariable("TARGET_PARTY", party.ShortTermTargetParty.Name);
			}
			else if (party.ShortTermBehavior == AiBehavior.FleeToPoint && party.ShortTermTargetParty != null)
			{
				textObject = new TextObject("{=AcMayd1p}Running from {TARGET_PARTY}", null);
				textObject.SetTextVariable("TARGET_PARTY", party.ShortTermTargetParty.Name);
			}
			else if (party.ShortTermBehavior == AiBehavior.FleeToGate && party.ShortTermTargetParty != null)
			{
				textObject = new TextObject("{=J5u0uOKc}Running from {TARGET_PARTY} to settlement.", null);
				textObject.SetTextVariable("TARGET_PARTY", party.ShortTermTargetParty.Name);
			}
			else if (party.DefaultBehavior == AiBehavior.DefendSettlement)
			{
				if (party.ShortTermBehavior == AiBehavior.EscortParty)
				{
					textObject = new TextObject("{=yD7rL5Nc}Helping ally party to defend {TARGET_SETTLEMENT}.", null);
				}
				else
				{
					textObject = new TextObject("{=rGy8vjOv}Defending {TARGET_SETTLEMENT}.", null);
				}
				textObject.SetTextVariable("TARGET_SETTLEMENT", party.TargetSettlement.Name);
			}
			else if (party.DefaultBehavior == AiBehavior.RaidSettlement)
			{
				textObject = new TextObject("{=VtWa9Pmh}Raiding {TARGET_SETTLEMENT}.", null);
				textObject.SetTextVariable("TARGET_SETTLEMENT", party.TargetSettlement.Name);
			}
			else if (party.DefaultBehavior == AiBehavior.BesiegeSettlement)
			{
				textObject = new TextObject("{=JTxI3sW2}Besieging {TARGET_SETTLEMENT}", null);
				textObject.SetTextVariable("TARGET_SETTLEMENT", party.TargetSettlement.Name);
			}
			else if (party.ShortTermBehavior == AiBehavior.GoToPoint)
			{
				if (party.ShortTermTargetParty != null)
				{
					textObject = new TextObject("{=AcMayd1p}Running from {TARGET_PARTY}", null);
					textObject.SetTextVariable("TARGET_PARTY", party.ShortTermTargetParty.Name);
				}
				else if (party.TargetSettlement != null)
				{
					if (party.DefaultBehavior == AiBehavior.PatrolAroundPoint)
					{
						if (party.TargetSettlement.GatePosition.Distance(party.Position2D) > Campaign.AverageDistanceBetweenTwoFortifications)
						{
							textObject = new TextObject("{=rba7kgwS}Travelling to {TARGET_SETTLEMENT}.", null);
						}
						else
						{
							textObject = new TextObject("{=yUVv3z5V}Patrolling around {TARGET_SETTLEMENT}.", null);
						}
						textObject.SetTextVariable("TARGET_SETTLEMENT", (party.TargetSettlement != null) ? party.TargetSettlement.Name : party.HomeSettlement.Name);
					}
					else
					{
						textObject = new TextObject("{=TaK6ydAx}Travelling.", null);
					}
				}
				else if (party.DefaultBehavior == AiBehavior.PatrolAroundPoint)
				{
					textObject = new TextObject("{=BifGz0h4}Patrolling", null);
				}
				else
				{
					textObject = new TextObject("{=XAL3t1bs}Going to a point", null);
				}
			}
			else if (party.ShortTermBehavior == AiBehavior.GoToSettlement || party.DefaultBehavior == AiBehavior.GoToSettlement)
			{
				if (party.ShortTermBehavior == AiBehavior.GoToSettlement && party.ShortTermTargetSettlement != null && party.ShortTermTargetSettlement != party.TargetSettlement)
				{
					textObject = new TextObject("{=NRpbagbZ}Running to {TARGET_PARTY}", null);
					textObject.SetTextVariable("TARGET_PARTY", party.ShortTermTargetSettlement.Name);
				}
				else if (party.DefaultBehavior == AiBehavior.GoToSettlement && party.TargetSettlement != null)
				{
					if (party.CurrentSettlement == party.TargetSettlement)
					{
						textObject = new TextObject("{=Y65gdbrx}Waiting in {TARGET_PARTY}", null);
					}
					else
					{
						textObject = new TextObject("{=EQHq3bHM}Travelling to {TARGET_PARTY}", null);
					}
					textObject.SetTextVariable("TARGET_PARTY", party.TargetSettlement.Name);
				}
				else if (party.ShortTermTargetParty != null)
				{
					textObject = new TextObject("{=AcMayd1p}Running from {TARGET_PARTY}", null);
					textObject.SetTextVariable("TARGET_PARTY", party.ShortTermTargetParty.Name);
				}
				else
				{
					textObject = new TextObject("{=QGyoSLeY}Traveling to a settlement", null);
				}
			}
			else if (party.ShortTermBehavior == AiBehavior.AssaultSettlement)
			{
				textObject = new TextObject("{=exnL6SS7}Attacking {TARGET_SETTLEMENT}", null);
				textObject.SetTextVariable("TARGET_SETTLEMENT", party.ShortTermTargetSettlement.Name);
			}
			else if (party.DefaultBehavior == AiBehavior.EscortParty || party.ShortTermBehavior == AiBehavior.EscortParty)
			{
				textObject = new TextObject("{=OpzzCPiP}Following {TARGET_PARTY}", null);
				textObject.SetTextVariable("TARGET_PARTY", (party.ShortTermTargetParty != null) ? party.ShortTermTargetParty.Name : party.TargetParty.Name);
			}
			else
			{
				textObject = new TextObject("{=QXBf26Rv}Unknown Behavior", null);
			}
			return textObject.ToString();
		}

		public static string GetHeroBehaviorText(Hero hero, ITeleportationCampaignBehavior teleportationBehavior = null)
		{
			if (hero.CurrentSettlement != null)
			{
				GameTexts.SetVariable("SETTLEMENT_NAME", hero.CurrentSettlement.Name);
			}
			if (hero.IsPrisoner)
			{
				if (hero.CurrentSettlement != null)
				{
					return GameTexts.FindText("str_prisoner_at_settlement", null).ToString();
				}
				if (hero.PartyBelongedToAsPrisoner != null)
				{
					CampaignUIHelper._prisonerOfText.SetTextVariable("PARTY_NAME", hero.PartyBelongedToAsPrisoner.Name);
					return CampaignUIHelper._prisonerOfText.ToString();
				}
				return new TextObject("{=tYz4D8Or}Prisoner", null).ToString();
			}
			else
			{
				if (hero.IsTraveling)
				{
					IMapPoint mapPoint = null;
					bool flag = false;
					bool flag2 = false;
					if (teleportationBehavior == null || !teleportationBehavior.GetTargetOfTeleportingHero(hero, out flag, out flag2, out mapPoint))
					{
						return CampaignUIHelper._travelingText.ToString();
					}
					Settlement settlement;
					if (flag && (settlement = mapPoint as Settlement) != null)
					{
						TextObject textObject = new TextObject("{=gUUnZNGk}Moving to {SETTLEMENT_NAME} to be the new governor", null);
						textObject.SetTextVariable("SETTLEMENT_NAME", settlement.Name.ToString());
						return textObject.ToString();
					}
					if (flag2 && mapPoint is MobileParty)
					{
						return new TextObject("{=g08mptth}Moving to a party to be the new leader", null).ToString();
					}
					MobileParty mobileParty;
					if ((mobileParty = mapPoint as MobileParty) != null)
					{
						TextObject textObject2 = new TextObject("{=qaQqAYGc}Moving to {LEADER.NAME}{.o} Party", null);
						bool flag3;
						if (mobileParty == null)
						{
							flag3 = null != null;
						}
						else
						{
							Hero leaderHero = mobileParty.LeaderHero;
							flag3 = ((leaderHero != null) ? leaderHero.CharacterObject : null) != null;
						}
						if (flag3)
						{
							StringHelpers.SetCharacterProperties("LEADER", mobileParty.LeaderHero.CharacterObject, textObject2, false);
						}
						return textObject2.ToString();
					}
					Settlement settlement2;
					if ((settlement2 = mapPoint as Settlement) != null)
					{
						TextObject textObject3 = new TextObject("{=UUaW0dba}Moving to {SETTLEMENT_NAME}", null);
						string text = "SETTLEMENT_NAME";
						string text2;
						if (settlement2 == null)
						{
							text2 = null;
						}
						else
						{
							TextObject name = settlement2.Name;
							text2 = ((name != null) ? name.ToString() : null);
						}
						textObject3.SetTextVariable(text, text2 ?? string.Empty);
						return textObject3.ToString();
					}
				}
				if (hero.PartyBelongedTo != null)
				{
					if (hero.PartyBelongedTo.LeaderHero == hero && hero.PartyBelongedTo.Army != null)
					{
						CampaignUIHelper._attachedToText.SetTextVariable("PARTY_NAME", hero.PartyBelongedTo.Army.Name);
						return CampaignUIHelper._attachedToText.ToString();
					}
					if (hero.PartyBelongedTo == MobileParty.MainParty)
					{
						return CampaignUIHelper._inYourPartyText.ToString();
					}
					Settlement closestSettlementForNavigationMesh = Campaign.Current.Models.MapDistanceModel.GetClosestSettlementForNavigationMesh(hero.PartyBelongedTo.CurrentNavigationFace);
					CampaignUIHelper._nearSettlementText.SetTextVariable("SETTLEMENT_NAME", closestSettlementForNavigationMesh.Name);
					return CampaignUIHelper._nearSettlementText.ToString();
				}
				else if (hero.CurrentSettlement != null)
				{
					if (hero.CurrentSettlement.Town != null && hero.GovernorOf == hero.CurrentSettlement.Town)
					{
						return GameTexts.FindText("str_governing_at_settlement", null).ToString();
					}
					if (Campaign.Current.GetCampaignBehavior<IAlleyCampaignBehavior>().IsHeroAlleyLeaderOfAnyPlayerAlley(hero))
					{
						return GameTexts.FindText("str_alley_leader_at_settlement", null).ToString();
					}
					return GameTexts.FindText("str_staying_at_settlement", null).ToString();
				}
				else
				{
					if (Campaign.Current.IssueManager.IssueSolvingCompanionList.Contains(hero))
					{
						return GameTexts.FindText("str_solving_issue", null).ToString();
					}
					if (hero.IsFugitive)
					{
						return CampaignUIHelper._regroupingText.ToString();
					}
					if (hero.IsReleased)
					{
						GameTexts.SetVariable("LEFT", CampaignUIHelper._recoveringText);
						GameTexts.SetVariable("RIGHT", CampaignUIHelper._recentlyReleasedText);
						return GameTexts.FindText("str_LEFT_comma_RIGHT", null).ToString();
					}
					return new TextObject("{=RClxLG6N}Holding", null).ToString();
				}
			}
		}

		public static Hero GetTeleportingLeaderHero(MobileParty party, ITeleportationCampaignBehavior teleportationBehavior)
		{
			if (party != null && teleportationBehavior != null)
			{
				foreach (Hero hero in Hero.MainHero.Clan.Heroes.Where((Hero x) => x.IsAlive && x.IsTraveling))
				{
					bool flag;
					bool flag2;
					IMapPoint mapPoint;
					MobileParty mobileParty;
					if (teleportationBehavior.GetTargetOfTeleportingHero(hero, out flag, out flag2, out mapPoint) && flag2 && (mobileParty = mapPoint as MobileParty) != null && mobileParty == party)
					{
						return hero;
					}
				}
			}
			return null;
		}

		public static Hero GetTeleportingGovernor(Settlement settlement, ITeleportationCampaignBehavior teleportationBehavior)
		{
			if (settlement != null && teleportationBehavior != null)
			{
				foreach (Hero hero in Hero.MainHero.Clan.Heroes.Where((Hero x) => x.IsAlive && x.IsTraveling))
				{
					bool flag;
					bool flag2;
					IMapPoint mapPoint;
					Settlement settlement2;
					if (teleportationBehavior.GetTargetOfTeleportingHero(hero, out flag, out flag2, out mapPoint) && flag && (settlement2 = mapPoint as Settlement) != null && settlement2 == settlement)
					{
						return hero;
					}
				}
			}
			return null;
		}

		public static TextObject GetHeroRelationToHeroText(Hero queriedHero, Hero baseHero, bool uppercaseFirst)
		{
			GameTexts.SetVariable("RELATION_TEXT", ConversationHelper.GetHeroRelationToHeroTextShort(queriedHero, baseHero, uppercaseFirst));
			StringHelpers.SetCharacterProperties("BASE_HERO", baseHero.CharacterObject, null, false);
			return GameTexts.FindText("str_hero_family_relation", null);
		}

		public static string GetAbbreviatedValueTextFromValue(int valueAmount)
		{
			string text = "";
			decimal num = valueAmount;
			if (valueAmount < 10000)
			{
				return valueAmount.ToString();
			}
			if (valueAmount >= 10000 && valueAmount < 1000000)
			{
				text = new TextObject("{=thousandabbr}k", null).ToString();
				num /= 1000m;
			}
			else if (valueAmount >= 1000000 && valueAmount < 1000000000)
			{
				text = new TextObject("{=millionabbr}m", null).ToString();
				num /= 1000000m;
			}
			else if (valueAmount >= 1000000000 && valueAmount <= 2147483647)
			{
				text = new TextObject("{=billionabbr}b", null).ToString();
				num /= 1000000000m;
			}
			int num2 = (int)num;
			string text2 = num2.ToString();
			if (text2.Length < 3)
			{
				text2 += ".";
				string text3 = num.ToString("F3").Split(new char[] { '.' }).ElementAtOrDefault(1);
				if (text3 != null)
				{
					for (int i = 0; i < 3 - num2.ToString().Length; i++)
					{
						if (text3.ElementAtOrDefault(i) != '\0')
						{
							text2 += text3.ElementAtOrDefault(i).ToString();
						}
					}
				}
			}
			CampaignUIHelper._denarValueInfoText.SetTextVariable("DENAR_AMOUNT", text2);
			CampaignUIHelper._denarValueInfoText.SetTextVariable("VALUE_ABBREVIATION", text);
			return CampaignUIHelper._denarValueInfoText.ToString();
		}

		public static string GetPartyDistanceByTimeText(float distance, float speed)
		{
			int num = MathF.Ceiling(distance / speed);
			int num2 = num / 24;
			num %= 24;
			GameTexts.SetVariable("IS_UNDER_A_DAY", (num2 <= 0) ? 1 : 0);
			GameTexts.SetVariable("IS_MORE_THAN_ONE_DAY", (num2 > 1) ? 1 : 0);
			GameTexts.SetVariable("DAY_VALUE", num2);
			GameTexts.SetVariable("IS_UNDER_ONE_HOUR", (num <= 0) ? 1 : 0);
			GameTexts.SetVariable("IS_MORE_THAN_AN_HOUR", (num > 1) ? 1 : 0);
			GameTexts.SetVariable("HOUR_VALUE", num);
			return GameTexts.FindText("str_distance_by_time", null).ToString();
		}

		public static CharacterCode GetCharacterCode(CharacterObject character, bool useCivilian = false)
		{
			TextObject textObject;
			if (character.IsHero && CampaignUIHelper.IsHeroInformationHidden(character.HeroObject, out textObject))
			{
				return CharacterCode.CreateEmpty();
			}
			Hero heroObject = character.HeroObject;
			uint? num;
			if (heroObject == null)
			{
				num = null;
			}
			else
			{
				IFaction mapFaction = heroObject.MapFaction;
				num = ((mapFaction != null) ? new uint?(mapFaction.Color) : null);
			}
			uint num2 = num ?? ((character.Culture != null) ? character.Culture.Color : Color.White.ToUnsignedInteger());
			Hero heroObject2 = character.HeroObject;
			uint? num3;
			if (heroObject2 == null)
			{
				num3 = null;
			}
			else
			{
				IFaction mapFaction2 = heroObject2.MapFaction;
				num3 = ((mapFaction2 != null) ? new uint?(mapFaction2.Color2) : null);
			}
			uint num4 = num3 ?? ((character.Culture != null) ? character.Culture.Color2 : Color.White.ToUnsignedInteger());
			string text = string.Empty;
			BodyProperties bodyProperties = character.GetBodyProperties(character.Equipment, -1);
			bool flag;
			if (!useCivilian)
			{
				Hero heroObject3 = character.HeroObject;
				flag = heroObject3 != null && heroObject3.IsNoncombatant;
			}
			else
			{
				flag = true;
			}
			useCivilian = flag;
			if (character.IsHero && character.HeroObject.IsLord)
			{
				Equipment equipment = ((useCivilian && character.FirstCivilianEquipment != null) ? character.FirstCivilianEquipment.Clone(false) : character.Equipment.Clone(false));
				equipment[EquipmentIndex.NumAllWeaponSlots] = new EquipmentElement(null, null, null, false);
				for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; equipmentIndex++)
				{
					ItemObject item = equipment[equipmentIndex].Item;
					bool flag2;
					if (item == null)
					{
						flag2 = false;
					}
					else
					{
						WeaponComponent weaponComponent = item.WeaponComponent;
						bool? flag3;
						if (weaponComponent == null)
						{
							flag3 = null;
						}
						else
						{
							WeaponComponentData primaryWeapon = weaponComponent.PrimaryWeapon;
							flag3 = ((primaryWeapon != null) ? new bool?(primaryWeapon.IsShield) : null);
						}
						bool? flag4 = flag3;
						bool flag5 = true;
						flag2 = (flag4.GetValueOrDefault() == flag5) & (flag4 != null);
					}
					if (flag2)
					{
						equipment.AddEquipmentToSlotWithoutAgent(equipmentIndex, default(EquipmentElement));
					}
				}
				text = equipment.CalculateEquipmentCode();
			}
			else
			{
				text = ((useCivilian && character.FirstCivilianEquipment != null) ? character.FirstCivilianEquipment.Clone(false) : character.FirstBattleEquipment.Clone(false)).CalculateEquipmentCode();
			}
			return CharacterCode.CreateFrom(text, bodyProperties, character.IsFemale, character.IsHero, num2, num4, character.DefaultFormationClass, character.Race);
		}

		public static string GetTraitNameText(TraitObject traitObject, Hero hero)
		{
			if (traitObject != DefaultTraits.Mercy && traitObject != DefaultTraits.Valor && traitObject != DefaultTraits.Honor && traitObject != DefaultTraits.Generosity && traitObject != DefaultTraits.Calculating)
			{
				Debug.FailedAssert("Cannot show this trait as text.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem.ViewModelCollection\\CampaignUIHelper.cs", "GetTraitNameText", 2950);
				return "";
			}
			int traitLevel = hero.GetTraitLevel(traitObject);
			if (traitLevel != 0)
			{
				return GameTexts.FindText("str_trait_name_" + traitObject.StringId.ToLower(), (traitLevel + MathF.Abs(traitObject.MinValue)).ToString()).ToString();
			}
			return "";
		}

		public static string GetTraitTooltipText(TraitObject traitObject, int traitValue)
		{
			if (traitObject != DefaultTraits.Mercy && traitObject != DefaultTraits.Valor && traitObject != DefaultTraits.Honor && traitObject != DefaultTraits.Generosity && traitObject != DefaultTraits.Calculating)
			{
				Debug.FailedAssert("Cannot show this trait's tooltip.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem.ViewModelCollection\\CampaignUIHelper.cs", "GetTraitTooltipText", 2975);
				return null;
			}
			GameTexts.SetVariable("NEWLINE", "\n");
			if (traitValue != 0)
			{
				TextObject textObject = GameTexts.FindText("str_trait_name_" + traitObject.StringId.ToLower(), (traitValue + MathF.Abs(traitObject.MinValue)).ToString());
				GameTexts.SetVariable("TRAIT_VALUE", traitValue);
				GameTexts.SetVariable("TRAIT_NAME", textObject);
				TextObject textObject2 = GameTexts.FindText("str_trait", traitObject.StringId.ToLower());
				GameTexts.SetVariable("TRAIT", textObject2);
				GameTexts.SetVariable("TRAIT_DESCRIPTION", traitObject.Description);
				return GameTexts.FindText("str_trait_tooltip", null).ToString();
			}
			TextObject textObject3 = GameTexts.FindText("str_trait", traitObject.StringId.ToLower());
			GameTexts.SetVariable("TRAIT", textObject3);
			GameTexts.SetVariable("TRAIT_DESCRIPTION", traitObject.Description);
			return GameTexts.FindText("str_trait_description_tooltip", null).ToString();
		}

		public static string GetTextForRole(SkillEffect.PerkRole role)
		{
			switch (role)
			{
			case SkillEffect.PerkRole.None:
				return new TextObject("{=koX9okuG}None", null).ToString();
			case SkillEffect.PerkRole.Ruler:
				return new TextObject("{=IcgVKFxZ}Ruler", null).ToString();
			case SkillEffect.PerkRole.ClanLeader:
				return new TextObject("{=pqfz386V}Clan Leader", null).ToString();
			case SkillEffect.PerkRole.Governor:
				return new TextObject("{=Fa2nKXxI}Governor", null).ToString();
			case SkillEffect.PerkRole.ArmyCommander:
				return new TextObject("{=g9VIbA9s}Sergeant", null).ToString();
			case SkillEffect.PerkRole.PartyLeader:
				return new TextObject("{=ggpRTQQl}Party Leader", null).ToString();
			case SkillEffect.PerkRole.PartyOwner:
				return new TextObject("{=YifFZaG7}Party Owner", null).ToString();
			case SkillEffect.PerkRole.Surgeon:
				return new TextObject("{=QBPrRdQJ}Surgeon", null).ToString();
			case SkillEffect.PerkRole.Engineer:
				return new TextObject("{=7h6cXdW7}Engineer", null).ToString();
			case SkillEffect.PerkRole.Scout:
				return new TextObject("{=92M0Pb5T}Scout", null).ToString();
			case SkillEffect.PerkRole.Quartermaster:
				return new TextObject("{=redwEIlW}Quartermaster", null).ToString();
			case SkillEffect.PerkRole.PartyMember:
				return new TextObject("{=HcAV8x7p}Party Member", null).ToString();
			case SkillEffect.PerkRole.Personal:
				return new TextObject("{=UxAl9iyi}Personal", null).ToString();
			default:
				return "";
			}
		}

		public static int GetHeroCompareSortIndex(Hero x, Hero y)
		{
			int num;
			if (x.Clan == null && y.Clan == null)
			{
				num = 0;
			}
			else if (x.Clan == null)
			{
				num = 1;
			}
			else if (y.Clan == null)
			{
				num = -1;
			}
			else if (x.IsLord && !y.IsLord)
			{
				num = -1;
			}
			else if (!x.IsLord && y.IsLord)
			{
				num = 1;
			}
			else
			{
				num = -x.Clan.Renown.CompareTo(y.Clan.Renown);
			}
			if (num != 0)
			{
				return num;
			}
			int num2 = x.IsGangLeader.CompareTo(y.IsGangLeader);
			if (num2 != 0)
			{
				return num2;
			}
			num2 = y.Power.CompareTo(x.Power);
			if (num2 == 0)
			{
				return x.Name.ToString().CompareTo(y.Name.ToString());
			}
			return num2;
		}

		public static string GetHeroClanRoleText(Hero hero, Clan clan)
		{
			return GameTexts.FindText("role", MobileParty.MainParty.GetHeroPerkRole(hero).ToString()).ToString();
		}

		public static int GetItemObjectTypeSortIndex(ItemObject item)
		{
			if (item == null)
			{
				return -1;
			}
			int num = CampaignUIHelper._itemObjectTypeSortIndices.IndexOf(item.Type) * 100;
			switch (item.Type)
			{
			case ItemObject.ItemTypeEnum.Invalid:
			case ItemObject.ItemTypeEnum.HeadArmor:
			case ItemObject.ItemTypeEnum.BodyArmor:
			case ItemObject.ItemTypeEnum.LegArmor:
			case ItemObject.ItemTypeEnum.HandArmor:
			case ItemObject.ItemTypeEnum.Animal:
			case ItemObject.ItemTypeEnum.Book:
			case ItemObject.ItemTypeEnum.ChestArmor:
			case ItemObject.ItemTypeEnum.Cape:
			case ItemObject.ItemTypeEnum.HorseHarness:
			case ItemObject.ItemTypeEnum.Banner:
				return num;
			case ItemObject.ItemTypeEnum.Horse:
				if (!item.HorseComponent.IsRideable)
				{
					return num;
				}
				return num + 1;
			case ItemObject.ItemTypeEnum.OneHandedWeapon:
			case ItemObject.ItemTypeEnum.TwoHandedWeapon:
			case ItemObject.ItemTypeEnum.Polearm:
			case ItemObject.ItemTypeEnum.Arrows:
			case ItemObject.ItemTypeEnum.Bolts:
			case ItemObject.ItemTypeEnum.Shield:
			case ItemObject.ItemTypeEnum.Bow:
			case ItemObject.ItemTypeEnum.Crossbow:
			case ItemObject.ItemTypeEnum.Thrown:
			case ItemObject.ItemTypeEnum.Pistol:
			case ItemObject.ItemTypeEnum.Musket:
			case ItemObject.ItemTypeEnum.Bullets:
				return (int)(num + item.PrimaryWeapon.WeaponClass);
			case ItemObject.ItemTypeEnum.Goods:
				if (!item.IsFood)
				{
					return num + 1;
				}
				return num;
			default:
				return 1;
			}
		}

		public static string GetItemLockStringID(EquipmentElement equipmentElement)
		{
			return equipmentElement.Item.StringId + ((equipmentElement.ItemModifier != null) ? equipmentElement.ItemModifier.StringId : "");
		}

		public static string GetTroopLockStringID(TroopRosterElement rosterElement)
		{
			return rosterElement.Character.StringId;
		}

		public static List<ValueTuple<CampaignUIHelper.IssueQuestFlags, TextObject, TextObject>> GetQuestStateOfHero(Hero queriedHero)
		{
			List<ValueTuple<CampaignUIHelper.IssueQuestFlags, TextObject, TextObject>> list = new List<ValueTuple<CampaignUIHelper.IssueQuestFlags, TextObject, TextObject>>();
			if (Campaign.Current != null)
			{
				IssueBase relatedIssue;
				Campaign.Current.IssueManager.Issues.TryGetValue(queriedHero, out relatedIssue);
				if (relatedIssue == null)
				{
					relatedIssue = queriedHero.Issue;
				}
				List<QuestBase> questsRelatedToHero = CampaignUIHelper.GetQuestsRelatedToHero(queriedHero);
				if (questsRelatedToHero.Count > 0)
				{
					for (int i = 0; i < questsRelatedToHero.Count; i++)
					{
						if (questsRelatedToHero[i].QuestGiver == queriedHero)
						{
							list.Add(new ValueTuple<CampaignUIHelper.IssueQuestFlags, TextObject, TextObject>(questsRelatedToHero[i].IsSpecialQuest ? CampaignUIHelper.IssueQuestFlags.ActiveStoryQuest : CampaignUIHelper.IssueQuestFlags.ActiveIssue, questsRelatedToHero[i].Title, (questsRelatedToHero[i].JournalEntries.Count > 0) ? questsRelatedToHero[i].JournalEntries[0].LogText : TextObject.Empty));
						}
						else
						{
							list.Add(new ValueTuple<CampaignUIHelper.IssueQuestFlags, TextObject, TextObject>(questsRelatedToHero[i].IsSpecialQuest ? CampaignUIHelper.IssueQuestFlags.TrackedStoryQuest : CampaignUIHelper.IssueQuestFlags.TrackedIssue, questsRelatedToHero[i].Title, (questsRelatedToHero[i].JournalEntries.Count > 0) ? questsRelatedToHero[i].JournalEntries[0].LogText : TextObject.Empty));
						}
					}
				}
				bool flag;
				if (questsRelatedToHero != null)
				{
					IssueBase relatedIssue2 = relatedIssue;
					if (((relatedIssue2 != null) ? relatedIssue2.IssueQuest : null) != null)
					{
						flag = questsRelatedToHero.Any((QuestBase q) => q == relatedIssue.IssueQuest);
						goto IL_171;
					}
				}
				flag = false;
				IL_171:
				bool flag2 = flag;
				if (relatedIssue != null && !flag2)
				{
					ValueTuple<CampaignUIHelper.IssueQuestFlags, TextObject, TextObject> valueTuple = new ValueTuple<CampaignUIHelper.IssueQuestFlags, TextObject, TextObject>(CampaignUIHelper.GetIssueType(relatedIssue), relatedIssue.Title, relatedIssue.Description);
					list.Add(valueTuple);
				}
			}
			return list;
		}

		public static string GetQuestExplanationOfHero(CampaignUIHelper.IssueQuestFlags questType)
		{
			bool flag = (questType & CampaignUIHelper.IssueQuestFlags.ActiveIssue) != CampaignUIHelper.IssueQuestFlags.None || (questType & CampaignUIHelper.IssueQuestFlags.AvailableIssue) > CampaignUIHelper.IssueQuestFlags.None;
			bool flag2 = (questType & CampaignUIHelper.IssueQuestFlags.ActiveIssue) > CampaignUIHelper.IssueQuestFlags.None;
			string text = null;
			if (questType != CampaignUIHelper.IssueQuestFlags.None)
			{
				if (flag)
				{
					text = GameTexts.FindText("str_hero_has_" + (flag2 ? "active" : "available") + "_issue", null).ToString();
				}
				else
				{
					text = GameTexts.FindText("str_hero_has_active_quest", null).ToString();
				}
			}
			return text;
		}

		public static List<QuestBase> GetQuestsRelatedToHero(Hero hero)
		{
			List<QuestBase> list = new List<QuestBase>();
			List<QuestBase> list2;
			Campaign.Current.QuestManager.TrackedObjects.TryGetValue(hero, out list2);
			if (list2 != null)
			{
				for (int i = 0; i < list2.Count; i++)
				{
					if (list2[i].IsTrackEnabled)
					{
						list.Add(list2[i]);
					}
				}
			}
			IssueBase issue = hero.Issue;
			if (((issue != null) ? issue.IssueQuest : null) != null && hero.Issue.IssueQuest.IsTrackEnabled && !hero.Issue.IssueQuest.IsTracked(hero))
			{
				list.Add(hero.Issue.IssueQuest);
			}
			return list;
		}

		public static List<QuestBase> GetQuestsRelatedToParty(MobileParty party)
		{
			List<QuestBase> list = new List<QuestBase>();
			List<QuestBase> list2;
			Campaign.Current.QuestManager.TrackedObjects.TryGetValue(party, out list2);
			if (list2 != null)
			{
				for (int i = 0; i < list2.Count; i++)
				{
					if (list2[i].IsTrackEnabled)
					{
						list.Add(list2[i]);
					}
				}
			}
			if (party.MemberRoster.TotalHeroes > 0)
			{
				if (party.LeaderHero != null && party.MemberRoster.TotalHeroes == 1)
				{
					List<QuestBase> questsRelatedToHero = CampaignUIHelper.GetQuestsRelatedToHero(party.LeaderHero);
					if (questsRelatedToHero != null && questsRelatedToHero.Count > 0)
					{
						list.AddRange(questsRelatedToHero);
					}
				}
				else
				{
					for (int j = 0; j < party.MemberRoster.Count; j++)
					{
						CharacterObject characterAtIndex = party.MemberRoster.GetCharacterAtIndex(j);
						Hero hero = ((characterAtIndex != null) ? characterAtIndex.HeroObject : null);
						if (hero != null)
						{
							List<QuestBase> questsRelatedToHero2 = CampaignUIHelper.GetQuestsRelatedToHero(hero);
							if (questsRelatedToHero2 != null && questsRelatedToHero2.Count > 0)
							{
								list.AddRange(questsRelatedToHero2);
							}
						}
					}
				}
			}
			return list;
		}

		public static List<QuestBase> GetQuestsRelatedToSettlement(Settlement settlement)
		{
			List<QuestBase> list = new List<QuestBase>();
			foreach (KeyValuePair<ITrackableCampaignObject, List<QuestBase>> keyValuePair in Campaign.Current.QuestManager.TrackedObjects)
			{
				Hero hero;
				MobileParty mobileParty;
				if (((hero = keyValuePair.Key as Hero) != null && hero.CurrentSettlement == settlement) || ((mobileParty = keyValuePair.Key as MobileParty) != null && mobileParty.CurrentSettlement == settlement))
				{
					for (int i = 0; i < keyValuePair.Value.Count; i++)
					{
						if (!list.Contains(keyValuePair.Value[i]) && keyValuePair.Value[i].IsTrackEnabled)
						{
							list.Add(keyValuePair.Value[i]);
						}
					}
				}
			}
			return list;
		}

		public static CampaignUIHelper.IssueQuestFlags GetIssueType(IssueBase issue)
		{
			if (issue.IsSolvingWithAlternative || issue.IsSolvingWithLordSolution || issue.IsSolvingWithQuest)
			{
				return CampaignUIHelper.IssueQuestFlags.ActiveIssue;
			}
			return CampaignUIHelper.IssueQuestFlags.AvailableIssue;
		}

		public static CampaignUIHelper.IssueQuestFlags GetQuestType(QuestBase quest, Hero queriedQuestGiver)
		{
			if (quest.QuestGiver != null && quest.QuestGiver == queriedQuestGiver)
			{
				if (!quest.IsSpecialQuest)
				{
					return CampaignUIHelper.IssueQuestFlags.ActiveIssue;
				}
				return CampaignUIHelper.IssueQuestFlags.ActiveStoryQuest;
			}
			else
			{
				if (!quest.IsSpecialQuest)
				{
					return CampaignUIHelper.IssueQuestFlags.TrackedIssue;
				}
				return CampaignUIHelper.IssueQuestFlags.TrackedStoryQuest;
			}
		}

		public static IEnumerable<TraitObject> GetHeroTraits()
		{
			yield return DefaultTraits.Generosity;
			yield return DefaultTraits.Honor;
			yield return DefaultTraits.Valor;
			yield return DefaultTraits.Mercy;
			yield return DefaultTraits.Calculating;
			yield break;
		}

		public static bool IsItemUsageApplicable(WeaponComponentData weapon)
		{
			WeaponDescription weaponDescription = ((weapon != null && weapon.WeaponDescriptionId != null) ? MBObjectManager.Instance.GetObject<WeaponDescription>(weapon.WeaponDescriptionId) : null);
			return weaponDescription != null && !weaponDescription.IsHiddenFromUI;
		}

		public static string FloatToString(float x)
		{
			return x.ToString("F1");
		}

		private static Tuple<bool, TextObject> GetIsStringApplicableForHeroName(string name)
		{
			bool flag;
			if (name.Length < 3)
			{
				if (!name.Any((char c) => Common.IsCharAsian(c)))
				{
					flag = false;
					goto IL_40;
				}
			}
			flag = name.Length <= 50;
			IL_40:
			if (!flag)
			{
				TextObject textObject = new TextObject("{=TUST9cUX}Character name should be at least {MIN} characters", null);
				textObject.SetTextVariable("MIN", 3);
				return new Tuple<bool, TextObject>(false, textObject);
			}
			if (string.IsNullOrEmpty(name))
			{
				return new Tuple<bool, TextObject>(false, new TextObject("{=C9tKA0ul}Character name cannot be empty", null));
			}
			if (!name.All((char x) => char.IsLetterOrDigit(x) || char.IsWhiteSpace(x) || char.IsPunctuation(x)))
			{
				return new Tuple<bool, TextObject>(false, new TextObject("{=P1hk0m4o}Character name cannot contain special characters", null));
			}
			return new Tuple<bool, TextObject>(true, TextObject.Empty);
		}

		public static Tuple<bool, string> IsStringApplicableForHeroName(string name)
		{
			Tuple<bool, TextObject> isStringApplicableForHeroName = CampaignUIHelper.GetIsStringApplicableForHeroName(name);
			return new Tuple<bool, string>(isStringApplicableForHeroName.Item1, isStringApplicableForHeroName.Item2.ToString());
		}

		public static CharacterObject GetVisualPartyLeader(PartyBase party)
		{
			return PartyBaseHelper.GetVisualPartyLeader(party);
		}

		private static string GetChangeValueString(float value)
		{
			string text = value.ToString("0.##");
			if (value > 0.001f)
			{
				MBTextManager.SetTextVariable("NUMBER", text, false);
				return GameTexts.FindText("str_plus_with_number", null).ToString();
			}
			return text;
		}

		[CompilerGenerated]
		internal static void <GetOrderCannotBeCompletedReasonTooltip>g__AddProperty|143_0(CraftingTemplate.CraftingStatTypes type, float reqValue, ref CampaignUIHelper.<>c__DisplayClass143_0 A_2)
		{
			TextObject textObject = GameTexts.FindText("str_crafting_stat", type.ToString());
			TextObject textObject2 = GameTexts.FindText("str_inventory_dmg_type", ((int)A_2.order.PreCraftedWeaponDesignItem.PrimaryWeapon.ThrustDamageType).ToString());
			textObject.SetTextVariable("THRUST_DAMAGE_TYPE", textObject2);
			TextObject textObject3 = GameTexts.FindText("str_inventory_dmg_type", ((int)A_2.order.PreCraftedWeaponDesignItem.PrimaryWeapon.SwingDamageType).ToString());
			textObject.SetTextVariable("SWING_DAMAGE_TYPE", textObject3);
			CampaignUIHelper._orderRequirementText.SetTextVariable("STAT", textObject);
			CampaignUIHelper._orderRequirementText.SetTextVariable("REQUIREMENT", reqValue);
			A_2.properties.Add(new TooltipProperty(CampaignUIHelper._orderRequirementText.ToString(), "", 0, false, TooltipProperty.TooltipPropertyFlags.None));
		}

		private static readonly TextObject _changeStr = new TextObject("{=R2AaCaPJ}Expected Change", null);

		private static readonly TextObject _totalStr = new TextObject("{=kWVbHPtT}Total", null);

		private static readonly TextObject _noChangesStr = new TextObject("{=XIioBPi0}No changes", null);

		private static readonly TextObject _hitPointsStr = new TextObject("{=oBbiVeKE}Hit Points", null);

		private static readonly TextObject _maxhitPointsStr = new TextObject("{=mDFhzEMC}Max. Hit Points", null);

		private static readonly TextObject _prosperityStr = new TextObject("{=IagYTD5O}Prosperity", null);

		private static readonly TextObject _hearthStr = new TextObject("{=2GWR9Cba}Hearth", null);

		private static readonly TextObject _dailyProductionStr = new TextObject("{=94aHU6nD}Construction", null);

		private static readonly TextObject _securityStr = new TextObject("{=MqCH7R4A}Security", null);

		private static readonly TextObject _criminalRatingStr = new TextObject("{=it8oPzb1}Criminal Rating", null);

		private static readonly TextObject _militiaStr = new TextObject("{=gsVtO9A7}Militia", null);

		private static readonly TextObject _foodStr = new TextObject("{=qSi4DlT4}Food", null);

		private static readonly TextObject _foodItemsStr = new TextObject("{=IQY9yykn}Food Items", null);

		private static readonly TextObject _livestockStr = new TextObject("{=UI0q8rWw}Livestock", null);

		private static readonly TextObject _armyCohesionStr = new TextObject("{=iZ3w6opW}Cohesion", null);

		private static readonly TextObject _loyaltyStr = new TextObject("{=YO0x7ZAo}Loyalty", null);

		private static readonly TextObject _wallsStr = new TextObject("{=LsZEdD2z}Walls", null);

		private static readonly TextObject _plusStr = new TextObject("{=eTw2aNV5}+", null);

		private static readonly TextObject _heroesHealingRateStr = new TextObject("{=HHTQVp52}Heroes Healing Rate", null);

		private static readonly TextObject _numTotalTroopsInTheArmyStr = new TextObject("{=DRJOxrRF}Troops in Army", null);

		private static readonly TextObject _garrisonStr = new TextObject("{=jlgjLDo7}Garrison", null);

		private static readonly TextObject _hitPoints = new TextObject("{=UbZL2BJQ}Hitpoints", null);

		private static readonly TextObject _maxhitPoints = new TextObject("{=KTTyBbsp}Max HP", null);

		private static readonly TextObject _goldStr = new TextObject("{=Hxf6bzmR}Current Denars", null);

		private static readonly TextObject _resultGold = new TextObject("{=NC9bbrt5}End-of-day denars", null);

		private static readonly TextObject _influenceStr = new TextObject("{=RVPidk5a}Influence", null);

		private static readonly TextObject _partyMoraleStr = GameTexts.FindText("str_party_morale", null);

		private static readonly TextObject _partyFoodStr = new TextObject("{=mg7id9om}Number of Consumable Items", null);

		private static readonly TextObject _partySpeedStr = new TextObject("{=zWaVxD6T}Party Speed", null);

		private static readonly TextObject _partySizeLimitStr = new TextObject("{=mp68RYnD}Party Size Limit", null);

		private static readonly TextObject _viewDistanceFoodStr = new TextObject("{=hTzTMLsf}View Distance", null);

		private static readonly TextObject _battleReadyTroopsStr = new TextObject("{=LVmkE2Ow}Battle Ready Troops", null);

		private static readonly TextObject _woundedTroopsStr = new TextObject("{=TzLtVzdg}Wounded Troops", null);

		private static readonly TextObject _prisonersStr = new TextObject("{=N6QTvjMf}Prisoners", null);

		private static readonly TextObject _regularsHealingRateStr = new TextObject("{=tf7301NC}Healing Rate", null);

		private static readonly TextObject _learningRateStr = new TextObject("{=q1J4a8rr}Learning Rate", null);

		private static readonly TextObject _learningLimitStr = new TextObject("{=YT9giTet}Learning Limit", null);

		private static readonly TextObject _partyInventoryCapacityStr = new TextObject("{=fI7a7RoE}Inventory Capacity", null);

		private static readonly TextObject _partyTroopSizeLimitStr = new TextObject("{=2Cq3tViJ}Party Troop Size Limit", null);

		private static readonly TextObject _partyPrisonerSizeLimitStr = new TextObject("{=UHLcmf9A}Party Prisoner Size Limit", null);

		private static readonly TextObject _inventorySkillTooltipTitle = new TextObject("{=Y7qbwrWE}{HERO_NAME}'s Skills", null);

		private static readonly TextObject _mercenaryClanInfluenceStr = new TextObject("{=GP3jpU0X}Influence is periodically converted to denars for mercenary clans.", null);

		private static readonly TextObject _orderRequirementText = new TextObject("{=dVqowrRz} - {STAT} {REQUIREMENT}", null);

		private static readonly TextObject _denarValueInfoText = new TextObject("{=mapbardenarvalue}{DENAR_AMOUNT}{VALUE_ABBREVIATION}", null);

		private static readonly TextObject _prisonerOfText = new TextObject("{=a8nRxITn}Prisoner of {PARTY_NAME}", null);

		private static readonly TextObject _attachedToText = new TextObject("{=8Jy9DnKk}Attached to {PARTY_NAME}", null);

		private static readonly TextObject _inYourPartyText = new TextObject("{=CRi905Ao}In your party", null);

		private static readonly TextObject _travelingText = new TextObject("{=vdKiLwaf}Traveling", null);

		private static readonly TextObject _recoveringText = new TextObject("{=heroRecovering}Recovering", null);

		private static readonly TextObject _recentlyReleasedText = new TextObject("{=NLFeyz7m}Recently Released From Captivity", null);

		private static readonly TextObject _recentlyEscapedText = new TextObject("{=84oSzquz}Recently Escaped Captivity", null);

		private static readonly TextObject _nearSettlementText = new TextObject("{=XjT8S4ng}Near {SETTLEMENT_NAME}", null);

		private static readonly TextObject _noDelayText = new TextObject("{=bDwTWrru}No delay", null);

		private static readonly TextObject _regroupingText = new TextObject("{=KxLoeSEO}Regrouping", null);

		public static readonly CampaignUIHelper.MobilePartyPrecedenceComparer MobilePartyPrecedenceComparerInstance = new CampaignUIHelper.MobilePartyPrecedenceComparer();

		private static readonly List<ItemObject.ItemTypeEnum> _itemObjectTypeSortIndices = new List<ItemObject.ItemTypeEnum>
		{
			ItemObject.ItemTypeEnum.Horse,
			ItemObject.ItemTypeEnum.OneHandedWeapon,
			ItemObject.ItemTypeEnum.TwoHandedWeapon,
			ItemObject.ItemTypeEnum.Polearm,
			ItemObject.ItemTypeEnum.Shield,
			ItemObject.ItemTypeEnum.Bow,
			ItemObject.ItemTypeEnum.Arrows,
			ItemObject.ItemTypeEnum.Crossbow,
			ItemObject.ItemTypeEnum.Bolts,
			ItemObject.ItemTypeEnum.Thrown,
			ItemObject.ItemTypeEnum.Pistol,
			ItemObject.ItemTypeEnum.Musket,
			ItemObject.ItemTypeEnum.Bullets,
			ItemObject.ItemTypeEnum.Goods,
			ItemObject.ItemTypeEnum.HeadArmor,
			ItemObject.ItemTypeEnum.Cape,
			ItemObject.ItemTypeEnum.BodyArmor,
			ItemObject.ItemTypeEnum.ChestArmor,
			ItemObject.ItemTypeEnum.HandArmor,
			ItemObject.ItemTypeEnum.LegArmor,
			ItemObject.ItemTypeEnum.Invalid,
			ItemObject.ItemTypeEnum.Animal,
			ItemObject.ItemTypeEnum.Book,
			ItemObject.ItemTypeEnum.HorseHarness,
			ItemObject.ItemTypeEnum.Banner
		};

		[Flags]
		public enum IssueQuestFlags
		{
			None = 0,
			AvailableIssue = 1,
			ActiveIssue = 2,
			ActiveStoryQuest = 4,
			TrackedIssue = 8,
			TrackedStoryQuest = 16
		}

		public class MobilePartyPrecedenceComparer : IComparer<MobileParty>
		{
			public int Compare(MobileParty x, MobileParty y)
			{
				if (x.IsGarrison && !y.IsGarrison)
				{
					return -1;
				}
				if (x.IsGarrison && y.IsGarrison)
				{
					return -x.Party.TotalStrength.CompareTo(y.Party.TotalStrength);
				}
				if (x.IsMilitia && y.IsGarrison)
				{
					return 1;
				}
				if (x.IsMilitia && !y.IsGarrison && !y.IsMilitia)
				{
					return -1;
				}
				if (x.IsMilitia && y.IsMilitia)
				{
					return -x.Party.TotalStrength.CompareTo(y.Party.TotalStrength);
				}
				if (x.LeaderHero != null && (y.IsGarrison || y.IsMilitia))
				{
					return 1;
				}
				if (x.LeaderHero != null && y.LeaderHero == null)
				{
					return -1;
				}
				if (x.LeaderHero != null && y.LeaderHero != null)
				{
					return -x.Party.TotalStrength.CompareTo(y.Party.TotalStrength);
				}
				if (x.LeaderHero == null && (y.IsGarrison || y.IsMilitia || y.LeaderHero != null))
				{
					return 1;
				}
				if (x.LeaderHero == null)
				{
					Hero leaderHero = y.LeaderHero;
					return -x.Party.TotalStrength.CompareTo(y.Party.TotalStrength);
				}
				return -x.Party.TotalStrength.CompareTo(y.Party.TotalStrength);
			}
		}

		public class ProductInputOutputEqualityComparer : IEqualityComparer<ValueTuple<ItemCategory, int>>
		{
			public bool Equals(ValueTuple<ItemCategory, int> x, ValueTuple<ItemCategory, int> y)
			{
				return x.Item1 == y.Item1;
			}

			public int GetHashCode(ValueTuple<ItemCategory, int> obj)
			{
				return obj.Item1.GetHashCode();
			}
		}
	}
}
