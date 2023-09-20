using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.Inventory;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection
{
	public static class PropertyBasedTooltipVMExtensions
	{
		public static void UpdateTooltip(this PropertyBasedTooltipVM propertyBasedTooltipVM, Track track)
		{
			propertyBasedTooltipVM.Mode = 1;
			MapTrackModel mapTrackModel = Campaign.Current.Models.MapTrackModel;
			if (mapTrackModel != null)
			{
				TextObject textObject = mapTrackModel.TrackTitle(track);
				propertyBasedTooltipVM.AddProperty("", textObject.ToString(), 0, TooltipProperty.TooltipPropertyFlags.Title);
				foreach (ValueTuple<TextObject, string> valueTuple in mapTrackModel.GetTrackDescription(track))
				{
					TextObject item = valueTuple.Item1;
					propertyBasedTooltipVM.AddProperty((item != null) ? item.ToString() : null, valueTuple.Item2, 0, TooltipProperty.TooltipPropertyFlags.None);
				}
			}
		}

		public static void UpdateTooltip(this PropertyBasedTooltipVM propertyBasedTooltipVM, Hero hero, bool isNear)
		{
			StringHelpers.SetCharacterProperties("NPC", hero.CharacterObject, null, false);
			TextObject textObject;
			bool flag = CampaignUIHelper.IsHeroInformationHidden(hero, out textObject);
			if (hero.IsEnemy(Hero.MainHero))
			{
				propertyBasedTooltipVM.Mode = 3;
			}
			else if (hero == Hero.MainHero || hero.IsFriend(Hero.MainHero))
			{
				propertyBasedTooltipVM.Mode = 2;
			}
			else
			{
				propertyBasedTooltipVM.Mode = 1;
			}
			propertyBasedTooltipVM.AddProperty("", hero.Name.ToString(), 0, TooltipProperty.TooltipPropertyFlags.Title);
			if (!hero.IsNotable && !hero.IsWanderer)
			{
				Clan clan = hero.Clan;
				if (((clan != null) ? clan.Kingdom : null) != null)
				{
					propertyBasedTooltipVM.AddProperty("", CampaignUIHelper.GetHeroKingdomRank(hero), 0, TooltipProperty.TooltipPropertyFlags.None);
				}
				if (Game.Current.IsDevelopmentMode)
				{
					Clan clan2 = hero.Clan;
					if (((clan2 != null) ? clan2.Leader : null) == hero)
					{
						propertyBasedTooltipVM.AddProperty("DEBUG Clan Leader", "", 0, TooltipProperty.TooltipPropertyFlags.None);
					}
				}
				if (Game.Current.IsDevelopmentMode)
				{
					Clan clan3 = hero.Clan;
					if (((clan3 != null) ? clan3.Kingdom : null) != null)
					{
						IFaction mapFaction = hero.MapFaction;
						if (hero == ((mapFaction != null) ? mapFaction.Leader : null))
						{
							Clan clan4 = hero.Clan;
							if (((clan4 != null) ? clan4.Kingdom : null) != null)
							{
								propertyBasedTooltipVM.AddProperty("DEBUG Kingdom Gold", hero.Clan.Kingdom.KingdomBudgetWallet.ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
							}
						}
						propertyBasedTooltipVM.AddProperty("DEBUG Gold", hero.Gold.ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
						if (Game.Current.IsDevelopmentMode && hero.Clan != null && hero.Clan.IsUnderMercenaryService)
						{
							propertyBasedTooltipVM.AddProperty("DEBUG Mercenary Award", hero.Clan.MercenaryAwardMultiplier.ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
						}
						if (Game.Current.IsDevelopmentMode)
						{
							Clan clan5 = hero.Clan;
							if (((clan5 != null) ? clan5.Leader : null) == hero)
							{
								propertyBasedTooltipVM.AddProperty("DEBUG Debt To Kingdom", hero.Clan.DebtToKingdom.ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
							}
						}
					}
				}
				if (Game.Current.IsDevelopmentMode && hero.PartyBelongedTo != null && !hero.IsSpecial)
				{
					propertyBasedTooltipVM.AddProperty("DEBUG Party Size", hero.PartyBelongedTo.MemberRoster.TotalManCount.ToString() + "/" + hero.PartyBelongedTo.Party.PartySizeLimit.ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
					propertyBasedTooltipVM.AddProperty("DEBUG Party Position", ((int)hero.PartyBelongedTo.Position2D.X).ToString() + "," + ((int)hero.PartyBelongedTo.Position2D.Y).ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
					propertyBasedTooltipVM.AddProperty("DEBUG Party Wage", hero.PartyBelongedTo.TotalWage.ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
				}
				if (Game.Current.IsDevelopmentMode && hero.PartyBelongedTo != null)
				{
					propertyBasedTooltipVM.AddProperty("DEBUG Party Morale", hero.PartyBelongedTo.Morale.ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
				}
				if (Game.Current.IsDevelopmentMode && hero.PartyBelongedTo != null)
				{
					propertyBasedTooltipVM.AddProperty("DEBUG Starving", hero.PartyBelongedTo.Party.IsStarving.ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
				}
				if (Game.Current.IsDevelopmentMode)
				{
					IFaction mapFaction2 = hero.MapFaction;
					if (((mapFaction2 != null) ? mapFaction2.Leader : null) != null && hero != hero.MapFaction.Leader)
					{
						propertyBasedTooltipVM.AddProperty("DEBUG King Relation", hero.GetRelation(hero.MapFaction.Leader).ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
					}
				}
				if (Game.Current.IsDevelopmentMode && hero.PartyBelongedToAsPrisoner != null)
				{
					propertyBasedTooltipVM.AddProperty("DEBUG Prisoner at", hero.PartyBelongedToAsPrisoner.Name.ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
				}
			}
			if (hero.Clan != null)
			{
				propertyBasedTooltipVM.AddProperty("", hero.Clan.Name.ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
			}
			propertyBasedTooltipVM.AddProperty("", "", -1, TooltipProperty.TooltipPropertyFlags.None);
			if (!flag)
			{
				int num = 0;
				foreach (Settlement settlement in Settlement.All)
				{
					if (settlement.IsTown)
					{
						Town town = settlement.Town;
						for (int i = 0; i < town.Workshops.Length; i++)
						{
							if (town.Workshops[i].Owner == hero && !town.Workshops[i].WorkshopType.IsHidden)
							{
								if (num == 0)
								{
									MBTextManager.SetTextVariable("STR1", new TextObject("{=VZjxs5Dt}Owner of ", null), false);
									MBTextManager.SetTextVariable("STR2", town.Workshops[i].WorkshopType.Name, false);
									string text = GameTexts.FindText("str_STR1_STR2", null).ToString();
									MBTextManager.SetTextVariable("LEFT", text, false);
									MBTextManager.SetTextVariable("PROPERTIES", text, false);
								}
								else
								{
									MBTextManager.SetTextVariable("RIGHT", town.Workshops[i].WorkshopType.Name, false);
									string text2 = GameTexts.FindText("str_LEFT_comma_RIGHT", null).ToString();
									MBTextManager.SetTextVariable("LEFT", text2, false);
									MBTextManager.SetTextVariable("PROPERTIES", text2, false);
								}
								num++;
							}
						}
					}
					if (settlement.IsTown || settlement.IsVillage)
					{
						foreach (Alley alley in settlement.Alleys)
						{
							if (alley.Owner == hero)
							{
								if (num == 0)
								{
									MBTextManager.SetTextVariable("STR1", new TextObject("{=VZjxs5Dt}Owner of ", null), false);
									MBTextManager.SetTextVariable("STR2", alley.Name, false);
									string text3 = GameTexts.FindText("str_STR1_STR2", null).ToString();
									MBTextManager.SetTextVariable("STR1", text3, false);
									MBTextManager.SetTextVariable("NUMBER_OF_MEN", Campaign.Current.Models.AlleyModel.GetTroopsOfAIOwnedAlley(alley).TotalManCount);
									MBTextManager.SetTextVariable("STR2", GameTexts.FindText("str_men_count_in_paranthesis_wo_wounded", null), false);
									text3 = GameTexts.FindText("str_STR1_STR2", null).ToString();
									MBTextManager.SetTextVariable("LEFT", text3, false);
									MBTextManager.SetTextVariable("PROPERTIES", text3, false);
								}
								else
								{
									MBTextManager.SetTextVariable("STR1", alley.Name, false);
									MBTextManager.SetTextVariable("NUMBER_OF_MEN", Campaign.Current.Models.AlleyModel.GetTroopsOfAIOwnedAlley(alley).TotalManCount);
									MBTextManager.SetTextVariable("STR2", GameTexts.FindText("str_men_count_in_paranthesis_wo_wounded", null), false);
									MBTextManager.SetTextVariable("RIGHT", GameTexts.FindText("str_STR1_STR2", null).ToString(), false);
									string text4 = GameTexts.FindText("str_LEFT_comma_RIGHT", null).ToString();
									MBTextManager.SetTextVariable("LEFT", text4, false);
									MBTextManager.SetTextVariable("PROPERTIES", text4, false);
								}
								num++;
							}
						}
					}
				}
				string text5 = new TextObject("{=j8uZBakZ}{PROPERTIES}", null).ToString();
				if (num > 0)
				{
					propertyBasedTooltipVM.AddProperty("", text5, 0, TooltipProperty.TooltipPropertyFlags.MultiLine);
				}
				int num2 = 0;
				TextObject textObject2 = new TextObject("{=C2qpwFq5}Owner of {SETTLEMENTS}", null);
				foreach (Settlement settlement2 in Settlement.All)
				{
					if (settlement2.IsFortification && settlement2.OwnerClan != null && settlement2.OwnerClan.Leader == hero)
					{
						if (num2 == 0)
						{
							MBTextManager.SetTextVariable("SETTLEMENTS", settlement2.Name, false);
						}
						else
						{
							MBTextManager.SetTextVariable("RIGHT", settlement2.Name.ToString(), false);
							MBTextManager.SetTextVariable("LEFT", new TextObject("{=!}{SETTLEMENTS}", null).ToString(), false);
							MBTextManager.SetTextVariable("SETTLEMENTS", GameTexts.FindText("str_LEFT_comma_RIGHT", null).ToString(), false);
						}
						num2++;
					}
				}
				if (num2 > 0)
				{
					propertyBasedTooltipVM.AddProperty("", textObject2.ToString(), 0, TooltipProperty.TooltipPropertyFlags.MultiLine);
				}
				if (hero.OwnedCaravans.Count > 0)
				{
					TextObject textObject3 = new TextObject("{=TEkWkzbH}Owned Caravans: {CARAVAN_COUNT}", null);
					textObject3.SetTextVariable("CARAVAN_COUNT", hero.OwnedCaravans.Count);
					propertyBasedTooltipVM.AddProperty("", textObject3.ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
				}
				if (hero.GovernorOf != null)
				{
					MBTextManager.SetTextVariable("STR1", new TextObject("{=jQdBl4hf}Governor of ", null), false);
					MBTextManager.SetTextVariable("STR2", hero.GovernorOf.Name, false);
					TextObject textObject4 = GameTexts.FindText("str_STR1_STR2", null);
					propertyBasedTooltipVM.AddProperty("", new Func<string>(textObject4.ToString), 0, TooltipProperty.TooltipPropertyFlags.None);
				}
			}
			if (hero != Hero.MainHero)
			{
				MBTextManager.SetTextVariable("LEFT", GameTexts.FindText("str_tooltip_label_relation", null), false);
				string text6 = GameTexts.FindText("str_LEFT_ONLY", null).ToString();
				propertyBasedTooltipVM.AddProperty(text6, ((int)hero.GetRelationWithPlayer()).ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
			}
			if (hero.HomeSettlement != null)
			{
				MBTextManager.SetTextVariable("LEFT", GameTexts.FindText("str_tooltip_label_home", null), false);
				string text7 = GameTexts.FindText("str_LEFT_ONLY", null).ToString();
				propertyBasedTooltipVM.AddProperty(text7, hero.HomeSettlement.Name.ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
			}
			if (hero.IsNotable)
			{
				MBTextManager.SetTextVariable("LEFT", GameTexts.FindText("str_notable_power", null), false);
				string text8 = GameTexts.FindText("str_LEFT_colon", null).ToString();
				MBTextManager.SetTextVariable("RANK", Campaign.Current.Models.NotablePowerModel.GetPowerRankName(hero).ToString(), false);
				MBTextManager.SetTextVariable("NUMBER", ((int)hero.Power).ToString(), false);
				string text9 = GameTexts.FindText("str_RANK_with_NUM_between_parenthesis", null).ToString();
				propertyBasedTooltipVM.AddProperty(text8, text9, 0, TooltipProperty.TooltipPropertyFlags.None);
			}
			MBTextManager.SetTextVariable("LEFT", GameTexts.FindText("str_tooltip_label_type", null), false);
			string text10 = GameTexts.FindText("str_LEFT_ONLY", null).ToString();
			propertyBasedTooltipVM.AddProperty(text10, HeroHelper.GetCharacterTypeName(hero).ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
			if (hero.CurrentSettlement != null && LocationComplex.Current != null && hero.CurrentSettlement == Hero.MainHero.CurrentSettlement && LocationComplex.Current.GetLocationOfCharacter(hero) != null)
			{
				MBTextManager.SetTextVariable("LEFT", GameTexts.FindText("str_tooltip_label_location", null), false);
				string text11 = GameTexts.FindText("str_LEFT_ONLY", null).ToString();
				propertyBasedTooltipVM.AddProperty(text11, LocationComplex.Current.GetLocationOfCharacter(hero).DoorName.ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
			}
			if (hero.CurrentSettlement != null && hero.IsNotable && hero.SupporterOf != null)
			{
				MBTextManager.SetTextVariable("LEFT", GameTexts.FindText("str_tooltip_label_supporter_of", null), false);
				string text12 = GameTexts.FindText("str_LEFT_ONLY", null).ToString();
				propertyBasedTooltipVM.AddProperty(text12, hero.SupporterOf.Name.ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
			}
			if (isNear)
			{
				List<ValueTuple<CampaignUIHelper.IssueQuestFlags, TextObject, TextObject>> questStateOfHero = CampaignUIHelper.GetQuestStateOfHero(hero);
				for (int j = 0; j < questStateOfHero.Count; j++)
				{
					string questExplanationOfHero = CampaignUIHelper.GetQuestExplanationOfHero(questStateOfHero[j].Item1);
					if (!string.IsNullOrEmpty(questExplanationOfHero))
					{
						propertyBasedTooltipVM.AddProperty("", "", -1, TooltipProperty.TooltipPropertyFlags.None);
						propertyBasedTooltipVM.AddProperty(questExplanationOfHero, questStateOfHero[j].Item2.ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
					}
				}
			}
			if (!hero.IsAlive)
			{
				propertyBasedTooltipVM.AddProperty("", GameTexts.FindText("str_dead", null).ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
			}
		}

		public static void UpdateTooltip(this PropertyBasedTooltipVM propertyBasedTooltipVM, InventoryLogic inventory)
		{
			propertyBasedTooltipVM.Mode = 0;
			List<ValueTuple<ItemRosterElement, int>> soldItems = inventory.GetSoldItems();
			List<ValueTuple<ItemRosterElement, int>> boughtItems = inventory.GetBoughtItems();
			TextObject textObject = new TextObject("{=bPFjmYCI}{SHOP_NAME} x {SHOP_DIFFERENCE_COUNT}", null);
			TextObject textObject2 = new TextObject("{=lxwGbRwu}x {SHOP_DIFFERENCE_COUNT}", null);
			TextObject textObject3 = (inventory.IsTrading ? textObject : textObject2);
			int num = 0;
			int num2 = 40;
			foreach (ValueTuple<ItemRosterElement, int> valueTuple in soldItems)
			{
				if (num == num2)
				{
					break;
				}
				TextObject textObject4 = textObject3;
				string text = "SHOP_NAME";
				ItemRosterElement itemRosterElement = valueTuple.Item1;
				textObject4.SetTextVariable(text, itemRosterElement.EquipmentElement.GetModifiedItemName());
				TextObject textObject5 = textObject3;
				string text2 = "SHOP_DIFFERENCE_COUNT";
				itemRosterElement = valueTuple.Item1;
				textObject5.SetTextVariable(text2, itemRosterElement.Amount);
				if (inventory.IsTrading)
				{
					string text3 = textObject3.ToString();
					string text4 = "+";
					int item = valueTuple.Item2;
					propertyBasedTooltipVM.AddColoredProperty(text3, text4 + item.ToString(), UIColors.PositiveIndicator, 0, TooltipProperty.TooltipPropertyFlags.None);
				}
				else
				{
					itemRosterElement = valueTuple.Item1;
					propertyBasedTooltipVM.AddColoredProperty(itemRosterElement.EquipmentElement.GetModifiedItemName().ToString(), textObject3.ToString(), UIColors.NegativeIndicator, 0, TooltipProperty.TooltipPropertyFlags.None);
				}
				num++;
			}
			foreach (ValueTuple<ItemRosterElement, int> valueTuple2 in boughtItems)
			{
				if (num == num2)
				{
					break;
				}
				TextObject textObject6 = textObject3;
				string text5 = "SHOP_NAME";
				ItemRosterElement itemRosterElement = valueTuple2.Item1;
				textObject6.SetTextVariable(text5, itemRosterElement.EquipmentElement.GetModifiedItemName());
				TextObject textObject7 = textObject3;
				string text6 = "SHOP_DIFFERENCE_COUNT";
				itemRosterElement = valueTuple2.Item1;
				textObject7.SetTextVariable(text6, itemRosterElement.Amount);
				if (inventory.IsTrading)
				{
					propertyBasedTooltipVM.AddColoredProperty(textObject3.ToString(), (-valueTuple2.Item2).ToString(), UIColors.NegativeIndicator, 0, TooltipProperty.TooltipPropertyFlags.None);
				}
				else
				{
					itemRosterElement = valueTuple2.Item1;
					propertyBasedTooltipVM.AddColoredProperty(itemRosterElement.EquipmentElement.GetModifiedItemName().ToString(), textObject3.ToString(), UIColors.PositiveIndicator, 0, TooltipProperty.TooltipPropertyFlags.None);
				}
				num++;
			}
			if (num == num2)
			{
				int num3 = soldItems.Count + boughtItems.Count - num;
				if (num3 > 0)
				{
					TextObject textObject8 = new TextObject("{=OpsiBFCu}... and {COUNT} more items.", null);
					textObject8.SetTextVariable("COUNT", num3);
					propertyBasedTooltipVM.AddProperty("", textObject8.ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
				}
			}
		}

		private static void UpdateTooltip(this PropertyBasedTooltipVM propertyBasedTooltipVM, WeaponDesignElement craftingPiece)
		{
			propertyBasedTooltipVM.Mode = 0;
			propertyBasedTooltipVM.AddProperty("", craftingPiece.CraftingPiece.Name.ToString(), 0, TooltipProperty.TooltipPropertyFlags.Title);
			TextObject textObject = GameTexts.FindText("str_crafting_piece_type", craftingPiece.CraftingPiece.PieceType.ToString());
			propertyBasedTooltipVM.AddProperty("", textObject.ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
			propertyBasedTooltipVM.AddProperty(new TextObject("{=Oo3fkeab}Difficulty: ", null).ToString(), Campaign.Current.Models.SmithingModel.GetCraftingPartDifficulty(craftingPiece.CraftingPiece).ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
			propertyBasedTooltipVM.AddProperty(new TextObject("{=XUtiwiYP}Length: ", null).ToString(), MathF.Round(craftingPiece.CraftingPiece.Length * 100f, 2).ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
			propertyBasedTooltipVM.AddProperty(GameTexts.FindText("str_weight_text", null).ToString(), MathF.Round(craftingPiece.CraftingPiece.Weight, 2).ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
			if (craftingPiece.CraftingPiece.PieceType == CraftingPiece.PieceTypes.Blade)
			{
				if (craftingPiece.CraftingPiece.BladeData.SwingDamageType != DamageTypes.Invalid)
				{
					DamageTypes swingDamageType = craftingPiece.CraftingPiece.BladeData.SwingDamageType;
					MBTextManager.SetTextVariable("SWING_DAMAGE_FACTOR", MathF.Round(craftingPiece.CraftingPiece.BladeData.SwingDamageFactor, 2) + " " + GameTexts.FindText("str_damage_types", swingDamageType.ToString()).ToString()[0].ToString(), false);
					propertyBasedTooltipVM.AddProperty(new TextObject("{=nYYUQQm0}Swing Damage Factor ", null).ToString(), new TextObject("{=aTdrjrEh}{SWING_DAMAGE_FACTOR}", null).ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
				}
				if (craftingPiece.CraftingPiece.BladeData.ThrustDamageType != DamageTypes.Invalid)
				{
					DamageTypes thrustDamageType = craftingPiece.CraftingPiece.BladeData.ThrustDamageType;
					MBTextManager.SetTextVariable("THRUST_DAMAGE_FACTOR", MathF.Round(craftingPiece.CraftingPiece.BladeData.ThrustDamageFactor, 2) + " " + GameTexts.FindText("str_damage_types", thrustDamageType.ToString()).ToString()[0].ToString(), false);
					propertyBasedTooltipVM.AddProperty(new TextObject("{=KTKBKmvp}Thrust Damage Factor ", null).ToString(), new TextObject("{=DNq9bdvV}{THRUST_DAMAGE_FACTOR}", null).ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
				}
			}
			if (craftingPiece.CraftingPiece.ArmorBonus != 0)
			{
				propertyBasedTooltipVM.AddModifierProperty(new TextObject("{=7Xynf4IA}Hand Armor", null).ToString(), craftingPiece.CraftingPiece.ArmorBonus, 0, TooltipProperty.TooltipPropertyFlags.None);
			}
			if (craftingPiece.CraftingPiece.SwingDamageBonus != 0)
			{
				propertyBasedTooltipVM.AddModifierProperty(new TextObject("{=QeToaiLt}Swing Damage", null).ToString(), craftingPiece.CraftingPiece.SwingDamageBonus, 0, TooltipProperty.TooltipPropertyFlags.None);
			}
			if (craftingPiece.CraftingPiece.SwingSpeedBonus != 0)
			{
				propertyBasedTooltipVM.AddModifierProperty(new TextObject("{=sVZaIPoQ}Swing Speed", null).ToString(), craftingPiece.CraftingPiece.SwingSpeedBonus, 0, TooltipProperty.TooltipPropertyFlags.None);
			}
			if (craftingPiece.CraftingPiece.ThrustDamageBonus != 0)
			{
				propertyBasedTooltipVM.AddModifierProperty(new TextObject("{=dO95yR9b}Thrust Damage", null).ToString(), craftingPiece.CraftingPiece.ThrustDamageBonus, 0, TooltipProperty.TooltipPropertyFlags.None);
			}
			if (craftingPiece.CraftingPiece.ThrustSpeedBonus != 0)
			{
				propertyBasedTooltipVM.AddModifierProperty(new TextObject("{=4uMWNDoi}Thrust Speed", null).ToString(), craftingPiece.CraftingPiece.ThrustSpeedBonus, 0, TooltipProperty.TooltipPropertyFlags.None);
			}
			if (craftingPiece.CraftingPiece.HandlingBonus != 0)
			{
				propertyBasedTooltipVM.AddModifierProperty(new TextObject("{=oibdTnXP}Handling", null).ToString(), craftingPiece.CraftingPiece.HandlingBonus, 0, TooltipProperty.TooltipPropertyFlags.None);
			}
			if (craftingPiece.CraftingPiece.AccuracyBonus != 0)
			{
				propertyBasedTooltipVM.AddModifierProperty(new TextObject("{=TAnabTdy}Accuracy", null).ToString(), craftingPiece.CraftingPiece.AccuracyBonus, 0, TooltipProperty.TooltipPropertyFlags.None);
			}
			propertyBasedTooltipVM.AddProperty(string.Empty, string.Empty, -1, TooltipProperty.TooltipPropertyFlags.None);
			propertyBasedTooltipVM.AddProperty(new TextObject("{=hr4MuPnt}Required Materials", null).ToString(), " ", 0, TooltipProperty.TooltipPropertyFlags.None);
			propertyBasedTooltipVM.AddProperty("", string.Empty, -1, TooltipProperty.TooltipPropertyFlags.RundownSeperator);
			foreach (ValueTuple<CraftingMaterials, int> valueTuple in craftingPiece.CraftingPiece.MaterialsUsed)
			{
				ItemObject craftingMaterialItem = Campaign.Current.Models.SmithingModel.GetCraftingMaterialItem(valueTuple.Item1);
				if (craftingMaterialItem != null)
				{
					string text = craftingMaterialItem.Name.ToString();
					int item = valueTuple.Item2;
					propertyBasedTooltipVM.AddProperty(text, item.ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
				}
			}
		}

		public static void UpdateTooltip(this PropertyBasedTooltipVM propertyBasedTooltipVM, CharacterObject character)
		{
			propertyBasedTooltipVM.Mode = 1;
			propertyBasedTooltipVM.AddProperty("", character.Name.ToString(), 0, TooltipProperty.TooltipPropertyFlags.Title);
			TextObject textObject = GameTexts.FindText("str_party_troop_tier", null);
			textObject.SetTextVariable("TIER_LEVEL", character.Tier);
			propertyBasedTooltipVM.AddProperty("", textObject.ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
			if (character.UpgradeTargets.Length != 0)
			{
				GameTexts.SetVariable("XP_AMOUNT", character.GetUpgradeXpCost(PartyBase.MainParty, 0));
				propertyBasedTooltipVM.AddProperty("", GameTexts.FindText("str_required_xp_to_upgrade", null).ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
			}
			if (character.TroopWage > 0)
			{
				GameTexts.SetVariable("LEFT", GameTexts.FindText("str_wage", null));
				GameTexts.SetVariable("STR1", character.TroopWage);
				GameTexts.SetVariable("STR2", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">");
				GameTexts.SetVariable("RIGHT", GameTexts.FindText("str_STR1_space_STR2", null));
				propertyBasedTooltipVM.AddProperty("", GameTexts.FindText("str_LEFT_colon_RIGHT_wSpaceAfterColon", null).ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
			}
			propertyBasedTooltipVM.AddProperty("", "", 0, TooltipProperty.TooltipPropertyFlags.None);
			propertyBasedTooltipVM.AddProperty("", GameTexts.FindText("str_skills", null).ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
			propertyBasedTooltipVM.AddProperty("", "", 0, TooltipProperty.TooltipPropertyFlags.RundownSeperator);
			foreach (SkillObject skillObject in Skills.All)
			{
				if (character.GetSkillValue(skillObject) > 0)
				{
					propertyBasedTooltipVM.AddProperty(skillObject.Name.ToString(), character.GetSkillValue(skillObject).ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
				}
			}
		}

		public static void UpdateTooltip(this PropertyBasedTooltipVM propertyBasedTooltipVM, EquipmentElement? equipmentElement)
		{
			ItemObject item = equipmentElement.Value.Item;
			propertyBasedTooltipVM.Mode = 1;
			propertyBasedTooltipVM.AddProperty("", item.Name.ToString(), 0, TooltipProperty.TooltipPropertyFlags.Title);
			propertyBasedTooltipVM.AddProperty(new TextObject("{=zMMqgxb1}Type", null).ToString(), GameTexts.FindText("str_inventory_type_" + (int)item.Type, null).ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
			propertyBasedTooltipVM.AddProperty(" ", " ", 0, TooltipProperty.TooltipPropertyFlags.None);
			if (Game.Current.IsDevelopmentMode)
			{
				if (item.Culture != null)
				{
					propertyBasedTooltipVM.AddProperty("Culture: ", item.Culture.StringId, 0, TooltipProperty.TooltipPropertyFlags.None);
				}
				else
				{
					propertyBasedTooltipVM.AddProperty("Culture: ", "No Culture", 0, TooltipProperty.TooltipPropertyFlags.None);
				}
				propertyBasedTooltipVM.AddProperty("ID: ", item.StringId, 0, TooltipProperty.TooltipPropertyFlags.None);
			}
			if (item.RelevantSkill != null && item.Difficulty > 0)
			{
				propertyBasedTooltipVM.AddProperty(new TextObject("{=dWYm9GsC}Requires", null).ToString(), " ", 0, TooltipProperty.TooltipPropertyFlags.None);
				propertyBasedTooltipVM.AddProperty(" ", " ", 0, TooltipProperty.TooltipPropertyFlags.RundownSeperator);
				propertyBasedTooltipVM.AddProperty(item.RelevantSkill.Name.ToString(), item.Difficulty.ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
				propertyBasedTooltipVM.AddProperty(" ", " ", 0, TooltipProperty.TooltipPropertyFlags.None);
			}
			propertyBasedTooltipVM.AddProperty(new TextObject("{=4Dd2xgPm}Weight", null).ToString(), item.Weight.ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
			string text = "";
			if (item.IsUniqueItem)
			{
				text = text + GameTexts.FindText("str_inventory_flag_unique", null).ToString() + " ";
			}
			if (item.ItemFlags.HasAnyFlag(ItemFlags.NotUsableByFemale))
			{
				text = text + GameTexts.FindText("str_inventory_flag_male_only", null).ToString() + " ";
			}
			if (item.ItemFlags.HasAnyFlag(ItemFlags.NotUsableByMale))
			{
				text = text + GameTexts.FindText("str_inventory_flag_female_only", null).ToString() + " ";
			}
			if (!string.IsNullOrEmpty(text))
			{
				propertyBasedTooltipVM.AddProperty(new TextObject("{=eHVq6yDa}Item Properties", null).ToString(), text, 0, TooltipProperty.TooltipPropertyFlags.None);
			}
			if (item.HasArmorComponent)
			{
				if (Campaign.Current != null)
				{
					propertyBasedTooltipVM.AddProperty(new TextObject("{=US7UmBbt}Armor Tier", null).ToString(), ((int)(item.Tier + 1)).ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
				}
				if (item.ArmorComponent.HeadArmor != 0)
				{
					propertyBasedTooltipVM.AddProperty(new TextObject("{=O3dhjtOS}Head Armor", null).ToString(), equipmentElement.Value.GetModifiedHeadArmor().ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
				}
				if (item.ArmorComponent.BodyArmor != 0)
				{
					if (item.Type == ItemObject.ItemTypeEnum.HorseHarness)
					{
						propertyBasedTooltipVM.AddProperty(new TextObject("{=kftE5nvv}Horse Armor", null).ToString(), equipmentElement.Value.GetModifiedMountBodyArmor().ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
					}
					else
					{
						propertyBasedTooltipVM.AddProperty(new TextObject("{=HkfY3Ds5}Body Armor", null).ToString(), equipmentElement.Value.GetModifiedBodyArmor().ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
					}
				}
				if (item.ArmorComponent.ArmArmor != 0)
				{
					propertyBasedTooltipVM.AddProperty(new TextObject("{=kx7q8ybD}Arm Armor", null).ToString(), equipmentElement.Value.GetModifiedArmArmor().ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
				}
				if (item.ArmorComponent.LegArmor != 0)
				{
					propertyBasedTooltipVM.AddProperty(new TextObject("{=eIws123Z}Leg Armor", null).ToString(), equipmentElement.Value.GetModifiedLegArmor().ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
				}
			}
			else if (item.WeaponComponent != null && item.Weapons.Count > 0)
			{
				int num = ((item.Weapons.Count > 1 && propertyBasedTooltipVM.IsExtended) ? 1 : 0);
				WeaponComponentData weaponComponentData = item.Weapons[num];
				propertyBasedTooltipVM.AddProperty(new TextObject("{=sqdzHOPe}Class", null).ToString(), GameTexts.FindText("str_inventory_weapon", ((int)weaponComponentData.WeaponClass).ToString()).ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
				if (Campaign.Current != null)
				{
					propertyBasedTooltipVM.AddProperty(new TextObject("{=hn9TPqhK}Weapon Tier", null).ToString(), ((int)(item.Tier + 1)).ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
				}
				ItemObject.ItemTypeEnum itemTypeFromWeaponClass = WeaponComponentData.GetItemTypeFromWeaponClass(weaponComponentData.WeaponClass);
				if (itemTypeFromWeaponClass == ItemObject.ItemTypeEnum.OneHandedWeapon || itemTypeFromWeaponClass == ItemObject.ItemTypeEnum.TwoHandedWeapon || itemTypeFromWeaponClass == ItemObject.ItemTypeEnum.Polearm)
				{
					if (weaponComponentData.SwingDamageType != DamageTypes.Invalid)
					{
						propertyBasedTooltipVM.AddProperty(new TextObject("{=sVZaIPoQ}Swing Speed", null).ToString(), equipmentElement.Value.GetModifiedSwingSpeedForUsage(num).ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
						propertyBasedTooltipVM.AddProperty(new TextObject("{=QeToaiLt}Swing Damage", null).ToString(), equipmentElement.Value.GetModifiedSwingDamageForUsage(num).ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
					}
					if (weaponComponentData.ThrustDamageType != DamageTypes.Invalid)
					{
						propertyBasedTooltipVM.AddProperty(new TextObject("{=4uMWNDoi}Thrust Speed", null).ToString(), equipmentElement.Value.GetModifiedThrustSpeedForUsage(num).ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
						propertyBasedTooltipVM.AddProperty(new TextObject("{=dO95yR9b}Thrust Damage", null).ToString(), equipmentElement.Value.GetModifiedThrustDamageForUsage(num).ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
					}
					propertyBasedTooltipVM.AddProperty(new TextObject("{=ZcybPatO}Weapon Length", null).ToString(), weaponComponentData.WeaponLength.ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
					propertyBasedTooltipVM.AddProperty(new TextObject("{=oibdTnXP}Handling", null).ToString(), weaponComponentData.Handling.ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
				}
				if (itemTypeFromWeaponClass == ItemObject.ItemTypeEnum.Thrown)
				{
					propertyBasedTooltipVM.AddProperty(new TextObject("{=ZcybPatO}Weapon Length", null).ToString(), weaponComponentData.WeaponLength.ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
					propertyBasedTooltipVM.AddProperty(new TextObject("{=s31DnnAf}Damage", null).ToString(), ItemHelper.GetMissileDamageText(weaponComponentData, equipmentElement.Value.ItemModifier).ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
					propertyBasedTooltipVM.AddProperty(new TextObject("{=bAqDnkaT}Missile Speed", null).ToString(), equipmentElement.Value.GetModifiedMissileSpeedForUsage(num).ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
					propertyBasedTooltipVM.AddProperty(new TextObject("{=TAnabTdy}Accuracy", null).ToString(), weaponComponentData.Accuracy.ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
					propertyBasedTooltipVM.AddProperty(new TextObject("{=twtbH1zv}Stack Amount", null).ToString(), equipmentElement.Value.GetModifiedStackCountForUsage(num).ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
				}
				if (itemTypeFromWeaponClass == ItemObject.ItemTypeEnum.Shield)
				{
					propertyBasedTooltipVM.AddProperty(new TextObject("{=6GSXsdeX}Speed", null).ToString(), equipmentElement.Value.GetModifiedSwingSpeedForUsage(num).ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
					propertyBasedTooltipVM.AddProperty(new TextObject("{=oBbiVeKE}Hit Points", null).ToString(), equipmentElement.Value.GetModifiedMaximumHitPointsForUsage(num).ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
				}
				if (itemTypeFromWeaponClass == ItemObject.ItemTypeEnum.Bow || itemTypeFromWeaponClass == ItemObject.ItemTypeEnum.Crossbow)
				{
					propertyBasedTooltipVM.AddProperty(new TextObject("{=6GSXsdeX}Speed", null).ToString(), equipmentElement.Value.GetModifiedSwingSpeedForUsage(num).ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
					propertyBasedTooltipVM.AddProperty(new TextObject("{=s31DnnAf}Damage", null).ToString(), ItemHelper.GetThrustDamageText(weaponComponentData, equipmentElement.Value.ItemModifier).ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
					propertyBasedTooltipVM.AddProperty(new TextObject("{=TAnabTdy}Accuracy", null).ToString(), weaponComponentData.Accuracy.ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
					propertyBasedTooltipVM.AddProperty(new TextObject("{=bAqDnkaT}Missile Speed", null).ToString(), equipmentElement.Value.GetModifiedMissileSpeedForUsage(num).ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
					if (itemTypeFromWeaponClass == ItemObject.ItemTypeEnum.Crossbow)
					{
						propertyBasedTooltipVM.AddProperty(new TextObject("{=cnmRwV4s}Ammo Limit", null).ToString(), weaponComponentData.MaxDataValue.ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
					}
				}
				if (item != null && item.HasBannerComponent)
				{
					bool flag;
					if (item == null)
					{
						flag = null != null;
					}
					else
					{
						BannerComponent bannerComponent = item.BannerComponent;
						flag = ((bannerComponent != null) ? bannerComponent.BannerEffect : null) != null;
					}
					TextObject textObject;
					if (flag)
					{
						GameTexts.SetVariable("RANK", item.BannerComponent.BannerEffect.Name);
						string text2 = string.Empty;
						if (item.BannerComponent.BannerEffect.IncrementType == BannerEffect.EffectIncrementType.AddFactor)
						{
							GameTexts.FindText("str_NUMBER_percent", null).SetTextVariable("NUMBER", ((int)Math.Abs(item.BannerComponent.GetBannerEffectBonus() * 100f)).ToString());
							object obj;
							text2 = obj.ToString();
						}
						else if (item.BannerComponent.BannerEffect.IncrementType == BannerEffect.EffectIncrementType.Add)
						{
							text2 = item.BannerComponent.GetBannerEffectBonus().ToString();
						}
						GameTexts.SetVariable("NUMBER", text2);
						textObject = GameTexts.FindText("str_RANK_with_NUM_between_parenthesis", null);
					}
					else
					{
						textObject = new TextObject("{=koX9okuG}None", null);
					}
					propertyBasedTooltipVM.AddProperty(new TextObject("{=DbXZjPdf}Banner Effect: ", null).ToString(), textObject.ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
				}
				if (weaponComponentData.IsAmmo)
				{
					propertyBasedTooltipVM.AddProperty(new TextObject("{=TAnabTdy}Accuracy", null).ToString(), weaponComponentData.Accuracy.ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
					propertyBasedTooltipVM.AddProperty(new TextObject("{=s31DnnAf}Damage", null).ToString(), ItemHelper.GetThrustDamageText(weaponComponentData, equipmentElement.Value.ItemModifier).ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
					propertyBasedTooltipVM.AddProperty(new TextObject("{=twtbH1zv}Stack Amount", null).ToString(), equipmentElement.Value.GetModifiedStackCountForUsage(num).ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
				}
				if (item.Weapons.Any(delegate(WeaponComponentData x)
				{
					string weaponDescriptionId = x.WeaponDescriptionId;
					return weaponDescriptionId != null && weaponDescriptionId.IndexOf("couch", StringComparison.OrdinalIgnoreCase) >= 0;
				}))
				{
					propertyBasedTooltipVM.AddProperty("", GameTexts.FindText("str_inventory_flag_couchable", null).ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
				}
				if (item.Weapons.Any(delegate(WeaponComponentData x)
				{
					string weaponDescriptionId2 = x.WeaponDescriptionId;
					return weaponDescriptionId2 != null && weaponDescriptionId2.IndexOf("bracing", StringComparison.OrdinalIgnoreCase) >= 0;
				}))
				{
					propertyBasedTooltipVM.AddProperty("", GameTexts.FindText("str_inventory_flag_braceable", null).ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
				}
			}
			else if (item.HasHorseComponent)
			{
				if (item.HorseComponent.IsMount)
				{
					propertyBasedTooltipVM.AddProperty(new TextObject("{=8BlMRMiR}Horse Tier", null).ToString(), ((int)(item.Tier + 1)).ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
					propertyBasedTooltipVM.AddProperty(new TextObject("{=Mfbc4rQR}Charge Damage", null).ToString(), equipmentElement.Value.GetModifiedMountCharge(EquipmentElement.Invalid).ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
					propertyBasedTooltipVM.AddProperty(new TextObject("{=6GSXsdeX}Speed", null).ToString(), equipmentElement.Value.GetModifiedMountSpeed(EquipmentElement.Invalid).ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
					propertyBasedTooltipVM.AddProperty(new TextObject("{=rg7OuWS2}Maneuver", null).ToString(), equipmentElement.Value.GetModifiedMountManeuver(EquipmentElement.Invalid).ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
					propertyBasedTooltipVM.AddProperty(new TextObject("{=oBbiVeKE}Hit Points", null).ToString(), equipmentElement.Value.GetModifiedMountHitPoints().ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
					propertyBasedTooltipVM.AddProperty(new TextObject("{=ZUgoQ1Ws}Horse Type", null).ToString(), item.ItemCategory.GetName().ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
				}
			}
			else if (item.HasFoodComponent)
			{
				if (item.FoodComponent.MoraleBonus > 0)
				{
					propertyBasedTooltipVM.AddProperty(new TextObject("{=myMbtwXi}Morale Bonus", null).ToString(), item.FoodComponent.MoraleBonus.ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
				}
				if (item.IsFood)
				{
					propertyBasedTooltipVM.AddProperty(new TextObject("{=qSi4DlT4}Food", null).ToString(), " ", 0, TooltipProperty.TooltipPropertyFlags.None);
				}
			}
			if (item.HasBannerComponent)
			{
				BannerComponent bannerComponent2 = item.BannerComponent;
				if (((bannerComponent2 != null) ? bannerComponent2.BannerEffect : null) != null)
				{
					GameTexts.SetVariable("RANK", item.BannerComponent.BannerEffect.Name);
					string text3 = string.Empty;
					if (item.BannerComponent.BannerEffect.IncrementType == BannerEffect.EffectIncrementType.AddFactor)
					{
						GameTexts.FindText("str_NUMBER_percent", null).SetTextVariable("NUMBER", ((int)Math.Abs(item.BannerComponent.GetBannerEffectBonus() * 100f)).ToString());
						object obj2;
						text3 = obj2.ToString();
					}
					else if (item.BannerComponent.BannerEffect.IncrementType == BannerEffect.EffectIncrementType.Add)
					{
						text3 = item.BannerComponent.GetBannerEffectBonus().ToString();
					}
					GameTexts.SetVariable("NUMBER", text3);
					propertyBasedTooltipVM.AddProperty(new TextObject("{=DbXZjPdf}Banner Effect: ", null).ToString(), GameTexts.FindText("str_RANK_with_NUM_between_parenthesis", null).ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
				}
			}
		}

		public static void UpdateTooltip(this PropertyBasedTooltipVM propertyBasedTooltipVM, Building building)
		{
			propertyBasedTooltipVM.Mode = 1;
			propertyBasedTooltipVM.AddProperty("", building.Name.ToString(), 0, TooltipProperty.TooltipPropertyFlags.Title);
			if (building.BuildingType.IsDefaultProject)
			{
				propertyBasedTooltipVM.AddProperty("", new TextObject("{=bd7oAQq6}Daily", null).ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
			}
			else
			{
				propertyBasedTooltipVM.AddProperty(new TextObject("{=IJdjwXvn}Current Level: ", null).ToString(), building.CurrentLevel.ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
			}
			propertyBasedTooltipVM.AddProperty("", building.Explanation.ToString(), 0, TooltipProperty.TooltipPropertyFlags.MultiLine);
			propertyBasedTooltipVM.AddProperty("", building.GetBonusExplanation().ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
		}

		public static void UpdateTooltip(this PropertyBasedTooltipVM propertyBasedTooltipVM, Workshop workshop)
		{
			propertyBasedTooltipVM.Mode = 1;
			propertyBasedTooltipVM.AddProperty("", workshop.WorkshopType.Name.ToString(), 0, TooltipProperty.TooltipPropertyFlags.Title);
			propertyBasedTooltipVM.AddProperty(new TextObject("{=qRqnrtdX}Owner", null).ToString(), workshop.Owner.Name.ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
			propertyBasedTooltipVM.AddProperty(string.Empty, string.Empty, 0, TooltipProperty.TooltipPropertyFlags.None);
			propertyBasedTooltipVM.AddProperty(new TextObject("{=xtt9Oxer}Productions", null).ToString(), " ", 0, TooltipProperty.TooltipPropertyFlags.None);
			propertyBasedTooltipVM.AddProperty(string.Empty, string.Empty, -1, TooltipProperty.TooltipPropertyFlags.None);
			IEnumerable<ValueTuple<ItemCategory, int>> enumerable = workshop.WorkshopType.Productions.SelectMany((WorkshopType.Production p) => p.Inputs).Distinct(PropertyBasedTooltipVMExtensions.itemCategoryDistinctComparer);
			IEnumerable<ValueTuple<ItemCategory, int>> enumerable2 = workshop.WorkshopType.Productions.SelectMany((WorkshopType.Production p) => p.Outputs).Distinct(PropertyBasedTooltipVMExtensions.itemCategoryDistinctComparer);
			if (enumerable.Any<ValueTuple<ItemCategory, int>>())
			{
				propertyBasedTooltipVM.AddProperty(new TextObject("{=XCz81XYm}Inputs", null).ToString(), " ", 0, TooltipProperty.TooltipPropertyFlags.None);
				propertyBasedTooltipVM.AddProperty("", "", 0, TooltipProperty.TooltipPropertyFlags.DefaultSeperator);
				foreach (ValueTuple<ItemCategory, int> valueTuple in enumerable)
				{
					propertyBasedTooltipVM.AddProperty(" ", valueTuple.Item1.GetName().ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
				}
				propertyBasedTooltipVM.AddProperty(string.Empty, string.Empty, -1, TooltipProperty.TooltipPropertyFlags.None);
			}
			if (enumerable2.Any<ValueTuple<ItemCategory, int>>())
			{
				propertyBasedTooltipVM.AddProperty(new TextObject("{=ErnykQEH}Outputs", null).ToString(), " ", 0, TooltipProperty.TooltipPropertyFlags.None);
				propertyBasedTooltipVM.AddProperty("", "", 0, TooltipProperty.TooltipPropertyFlags.DefaultSeperator);
				foreach (ValueTuple<ItemCategory, int> valueTuple2 in enumerable2)
				{
					propertyBasedTooltipVM.AddProperty(" ", valueTuple2.Item1.GetName().ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
				}
			}
		}

		private static void AddEncounterParties(this PropertyBasedTooltipVM propertyBasedTooltipVM, MBReadOnlyList<MapEventParty> parties1, MBReadOnlyList<MapEventParty> parties2, bool isExtended)
		{
			propertyBasedTooltipVM.AddProperty("", "", 0, TooltipProperty.TooltipPropertyFlags.BattleMode);
			int num = 0;
			while (num < parties1.Count || num < parties2.Count)
			{
				MBTextManager.SetTextVariable("PARTY_1S_MEMBERS", "", false);
				MBTextManager.SetTextVariable("PARTY_2S_MEMBERS", "", false);
				if (num < parties1.Count)
				{
					MBTextManager.SetTextVariable("PARTY_1S_MEMBERS", parties1[num].Party.Name, false);
				}
				if (num < parties2.Count)
				{
					MBTextManager.SetTextVariable("PARTY_2S_MEMBERS", parties2[num].Party.Name, false);
				}
				propertyBasedTooltipVM.AddProperty(new TextObject("{=CExQ40Ux}{PARTY_1S_MEMBERS}   ", null).ToString(), new TextObject("{=OTaPfaJl}{PARTY_2S_MEMBERS}   ", null).ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
				num++;
			}
			if (parties1.Count > 0 && parties2.Count > 0)
			{
				PartyBase party = parties1[0].Party;
				if (((party != null) ? party.MapFaction : null) != null)
				{
					PartyBase party2 = parties2[0].Party;
					if (((party2 != null) ? party2.MapFaction : null) != null)
					{
						propertyBasedTooltipVM.AddProperty("", "", 0, TooltipProperty.TooltipPropertyFlags.DefaultSeperator);
						MBTextManager.SetTextVariable("PARTY_1S_MEMBERS", parties1[0].Party.MapFaction.Name, false);
						MBTextManager.SetTextVariable("PARTY_2S_MEMBERS", parties2[0].Party.MapFaction.Name, false);
						propertyBasedTooltipVM.AddProperty(new TextObject("{=CExQ40Ux}{PARTY_1S_MEMBERS}   ", null).ToString(), new TextObject("{=OTaPfaJl}{PARTY_2S_MEMBERS}   ", null).ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
					}
				}
			}
			int lastHeroIndex = 0;
			TroopRoster troopRoster = TroopRoster.CreateDummyTroopRoster();
			foreach (MapEventParty mapEventParty in parties1)
			{
				for (int i = 0; i < mapEventParty.Party.MemberRoster.Count; i++)
				{
					TroopRosterElement elementCopyAtIndex = mapEventParty.Party.MemberRoster.GetElementCopyAtIndex(i);
					if (elementCopyAtIndex.Character.IsHero)
					{
						troopRoster.AddToCounts(elementCopyAtIndex.Character, elementCopyAtIndex.Number, false, elementCopyAtIndex.WoundedNumber, 0, true, lastHeroIndex);
						int num2 = lastHeroIndex;
						lastHeroIndex = num2 + 1;
					}
					else
					{
						troopRoster.AddToCounts(elementCopyAtIndex.Character, elementCopyAtIndex.Number, false, elementCopyAtIndex.WoundedNumber, 0, true, -1);
					}
				}
			}
			lastHeroIndex = 0;
			TroopRoster troopRoster2 = TroopRoster.CreateDummyTroopRoster();
			foreach (MapEventParty mapEventParty2 in parties2)
			{
				for (int j = 0; j < mapEventParty2.Party.MemberRoster.Count; j++)
				{
					TroopRosterElement elementCopyAtIndex2 = mapEventParty2.Party.MemberRoster.GetElementCopyAtIndex(j);
					if (elementCopyAtIndex2.Character.IsHero)
					{
						troopRoster2.AddToCounts(elementCopyAtIndex2.Character, elementCopyAtIndex2.Number, false, elementCopyAtIndex2.WoundedNumber, 0, true, lastHeroIndex);
						int num2 = lastHeroIndex;
						lastHeroIndex = num2 + 1;
					}
					else
					{
						troopRoster2.AddToCounts(elementCopyAtIndex2.Character, elementCopyAtIndex2.Number, false, elementCopyAtIndex2.WoundedNumber, 0, true, -1);
					}
				}
			}
			Func<string> func = () => "";
			Func<string> func2 = () => "";
			if (troopRoster.Count > 0)
			{
				func = delegate
				{
					TroopRoster troopRoster3 = TroopRoster.CreateDummyTroopRoster();
					lastHeroIndex = 0;
					foreach (MapEventParty mapEventParty3 in parties1)
					{
						for (int k = 0; k < mapEventParty3.Party.MemberRoster.Count; k++)
						{
							TroopRosterElement elementCopyAtIndex3 = mapEventParty3.Party.MemberRoster.GetElementCopyAtIndex(k);
							if (elementCopyAtIndex3.Character.IsHero)
							{
								troopRoster3.AddToCounts(elementCopyAtIndex3.Character, elementCopyAtIndex3.Number, false, elementCopyAtIndex3.WoundedNumber, 0, true, lastHeroIndex);
								int lastHeroIndex3 = lastHeroIndex;
								lastHeroIndex = lastHeroIndex3 + 1;
							}
							else
							{
								troopRoster3.AddToCounts(elementCopyAtIndex3.Character, elementCopyAtIndex3.Number, false, elementCopyAtIndex3.WoundedNumber, 0, true, -1);
							}
						}
					}
					TextObject textObject = new TextObject("{=QlbkxoSp} {TOOLTIP_TROOPS} ({PARTY_SIZE})", null);
					textObject.SetTextVariable("TOOLTIP_TROOPS", GameTexts.FindText("str_map_tooltip_troops", null));
					textObject.SetTextVariable("PARTY_SIZE", PartyBaseHelper.GetPartySizeText(troopRoster3.TotalManCount - troopRoster3.TotalWounded, troopRoster3.TotalWounded, true));
					return textObject.ToString();
				};
			}
			if (troopRoster2.Count > 0)
			{
				func2 = delegate
				{
					TroopRoster troopRoster4 = TroopRoster.CreateDummyTroopRoster();
					lastHeroIndex = 0;
					foreach (MapEventParty mapEventParty4 in parties2)
					{
						for (int l = 0; l < mapEventParty4.Party.MemberRoster.Count; l++)
						{
							TroopRosterElement elementCopyAtIndex4 = mapEventParty4.Party.MemberRoster.GetElementCopyAtIndex(l);
							if (elementCopyAtIndex4.Character.IsHero)
							{
								troopRoster4.AddToCounts(elementCopyAtIndex4.Character, elementCopyAtIndex4.Number, false, elementCopyAtIndex4.WoundedNumber, 0, true, lastHeroIndex);
								int lastHeroIndex2 = lastHeroIndex;
								lastHeroIndex = lastHeroIndex2 + 1;
							}
							else
							{
								troopRoster4.AddToCounts(elementCopyAtIndex4.Character, elementCopyAtIndex4.Number, false, elementCopyAtIndex4.WoundedNumber, 0, true, -1);
							}
						}
					}
					TextObject textObject2 = new TextObject("{=QlbkxoSp} {TOOLTIP_TROOPS} ({PARTY_SIZE})", null);
					textObject2.SetTextVariable("TOOLTIP_TROOPS", GameTexts.FindText("str_map_tooltip_troops", null));
					textObject2.SetTextVariable("PARTY_SIZE", PartyBaseHelper.GetPartySizeText(troopRoster4.TotalManCount - troopRoster4.TotalWounded, troopRoster4.TotalWounded, true));
					return textObject2.ToString();
				};
			}
			if (func().Length != 0 && func2().Length != 0)
			{
				propertyBasedTooltipVM.AddProperty("", "", -1, TooltipProperty.TooltipPropertyFlags.None);
				propertyBasedTooltipVM.AddProperty(func, func2, 0, TooltipProperty.TooltipPropertyFlags.None);
			}
			if (isExtended)
			{
				propertyBasedTooltipVM.AddProperty("", "", 0, TooltipProperty.TooltipPropertyFlags.DefaultSeperator);
				int num3 = 0;
				while (num3 < troopRoster.Count || num3 < troopRoster2.Count)
				{
					string blankString = new TextObject("{=!} ", null).ToString();
					Func<string> func3 = () => blankString;
					Func<string> func4 = () => blankString;
					if (num3 < troopRoster.Count)
					{
						CharacterObject character2 = troopRoster.GetElementCopyAtIndex(num3).Character;
						func3 = delegate
						{
							lastHeroIndex = 0;
							foreach (MapEventParty mapEventParty5 in parties1)
							{
								for (int m = 0; m < mapEventParty5.Party.MemberRoster.Count; m++)
								{
									TroopRosterElement elementCopyAtIndex5 = mapEventParty5.Party.MemberRoster.GetElementCopyAtIndex(m);
									if (elementCopyAtIndex5.Character == character2)
									{
										TextObject textObject3;
										if (elementCopyAtIndex5.Character.IsHero)
										{
											textObject3 = new TextObject("{=W1tsTWZv} {PARTY_MEMBER.LINK} ({MEMBER_HEALTH}%)", null);
											textObject3.SetTextVariable("MEMBER_HEALTH", elementCopyAtIndex5.Character.HeroObject.HitPoints * 100 / elementCopyAtIndex5.Character.MaxHitPoints());
										}
										else
										{
											textObject3 = new TextObject("{=vLaBJFGy} {PARTY_MEMBER.LINK} ({PARTY_SIZE})", null);
											textObject3.SetTextVariable("PARTY_SIZE", PartyBaseHelper.GetPartySizeText(elementCopyAtIndex5.Number - elementCopyAtIndex5.WoundedNumber, elementCopyAtIndex5.WoundedNumber, true));
										}
										StringHelpers.SetCharacterProperties("PARTY_MEMBER", elementCopyAtIndex5.Character, textObject3, false);
										return textObject3.ToString();
									}
								}
							}
							return blankString;
						};
					}
					if (num3 < troopRoster2.Count)
					{
						CharacterObject character = troopRoster2.GetElementCopyAtIndex(num3).Character;
						func4 = delegate
						{
							lastHeroIndex = 0;
							foreach (MapEventParty mapEventParty6 in parties2)
							{
								for (int n = 0; n < mapEventParty6.Party.MemberRoster.Count; n++)
								{
									TroopRosterElement elementCopyAtIndex6 = mapEventParty6.Party.MemberRoster.GetElementCopyAtIndex(n);
									if (character == elementCopyAtIndex6.Character)
									{
										TextObject textObject4;
										if (character.IsHero)
										{
											textObject4 = new TextObject("{=PS02CqPu} {PARTY_MEMBER.LINK} (Health: {MEMBER_HEALTH}%)", null);
											textObject4.SetTextVariable("MEMBER_HEALTH", elementCopyAtIndex6.Character.HeroObject.HitPoints * 100 / elementCopyAtIndex6.Character.MaxHitPoints());
										}
										else
										{
											textObject4 = new TextObject("{=vLaBJFGy} {PARTY_MEMBER.LINK} ({PARTY_SIZE})", null);
											textObject4.SetTextVariable("PARTY_SIZE", PartyBaseHelper.GetPartySizeText(elementCopyAtIndex6.Number - elementCopyAtIndex6.WoundedNumber, elementCopyAtIndex6.WoundedNumber, true));
										}
										StringHelpers.SetCharacterProperties("PARTY_MEMBER", elementCopyAtIndex6.Character, textObject4, false);
										return textObject4.ToString();
									}
								}
							}
							return blankString;
						};
					}
					propertyBasedTooltipVM.AddProperty(func3, func4, 0, TooltipProperty.TooltipPropertyFlags.None);
					num3++;
				}
			}
			propertyBasedTooltipVM.AddProperty("", "", 0, TooltipProperty.TooltipPropertyFlags.BattleModeOver);
		}

		public static void UpdateEncounterTooltip(this PropertyBasedTooltipVM propertyBasedTooltipVM, int side)
		{
			List<MobileParty> list = new List<MobileParty> { MobileParty.MainParty };
			List<MobileParty> list2 = new List<MobileParty> { Campaign.Current.ConversationManager.ConversationParty };
			PlayerEncounter.Current.FindAllNpcPartiesWhoWillJoinEvent(ref list, ref list2);
			List<MobileParty> parties = null;
			if (side == 0)
			{
				parties = list;
				propertyBasedTooltipVM.Mode = 2;
			}
			else
			{
				parties = list2;
				propertyBasedTooltipVM.Mode = 3;
			}
			TroopRoster troopRoster = TroopRoster.CreateDummyTroopRoster();
			TroopRoster troopRoster2 = TroopRoster.CreateDummyTroopRoster();
			foreach (MobileParty mobileParty in parties)
			{
				for (int i = 0; i < mobileParty.MemberRoster.Count; i++)
				{
					TroopRosterElement elementCopyAtIndex = mobileParty.MemberRoster.GetElementCopyAtIndex(i);
					troopRoster.AddToCounts(elementCopyAtIndex.Character, elementCopyAtIndex.Number, false, elementCopyAtIndex.WoundedNumber, 0, true, -1);
				}
				for (int j = 0; j < mobileParty.PrisonRoster.Count; j++)
				{
					TroopRosterElement elementCopyAtIndex2 = mobileParty.PrisonRoster.GetElementCopyAtIndex(j);
					troopRoster2.AddToCounts(elementCopyAtIndex2.Character, elementCopyAtIndex2.Number, false, elementCopyAtIndex2.WoundedNumber, 0, true, -1);
				}
			}
			Func<TroopRoster> func = delegate
			{
				TroopRoster troopRoster3 = TroopRoster.CreateDummyTroopRoster();
				foreach (MobileParty mobileParty3 in parties)
				{
					for (int k = 0; k < mobileParty3.MemberRoster.Count; k++)
					{
						TroopRosterElement elementCopyAtIndex3 = mobileParty3.MemberRoster.GetElementCopyAtIndex(k);
						troopRoster3.AddToCounts(elementCopyAtIndex3.Character, elementCopyAtIndex3.Number, false, elementCopyAtIndex3.WoundedNumber, 0, true, -1);
					}
				}
				return troopRoster3;
			};
			Func<TroopRoster> func2 = delegate
			{
				TroopRoster troopRoster4 = TroopRoster.CreateDummyTroopRoster();
				foreach (MobileParty mobileParty4 in parties)
				{
					for (int l = 0; l < mobileParty4.PrisonRoster.Count; l++)
					{
						TroopRosterElement elementCopyAtIndex4 = mobileParty4.PrisonRoster.GetElementCopyAtIndex(l);
						troopRoster4.AddToCounts(elementCopyAtIndex4.Character, elementCopyAtIndex4.Number, false, elementCopyAtIndex4.WoundedNumber, 0, true, -1);
					}
				}
				return troopRoster4;
			};
			bool flag = false;
			foreach (MobileParty mobileParty2 in parties)
			{
				flag = flag || mobileParty2.IsInspected;
				propertyBasedTooltipVM.AddProperty("", mobileParty2.Name.ToString(), 1, TooltipProperty.TooltipPropertyFlags.None);
				string text = mobileParty2.Name.ToString();
				IFaction mapFaction = mobileParty2.MapFaction;
				if (text != ((mapFaction != null) ? mapFaction.Name.ToString() : null))
				{
					string text2 = "";
					IFaction mapFaction2 = mobileParty2.MapFaction;
					propertyBasedTooltipVM.AddProperty(text2, ((mapFaction2 != null) ? mapFaction2.Name.ToString() : null) ?? "", 0, TooltipProperty.TooltipPropertyFlags.None);
				}
			}
			if (troopRoster.Count > 0)
			{
				propertyBasedTooltipVM.AddPartyTroopProperties(troopRoster, GameTexts.FindText("str_map_tooltip_troops", null), flag, func);
			}
			if (troopRoster2.Count > 0)
			{
				propertyBasedTooltipVM.AddPartyTroopProperties(troopRoster2, GameTexts.FindText("str_map_tooltip_prisoners", null), flag, func2);
			}
			if (!Campaign.Current.IsMapTooltipLongForm && !propertyBasedTooltipVM.IsExtended)
			{
				propertyBasedTooltipVM.AddProperty("", "", -1, TooltipProperty.TooltipPropertyFlags.None);
				GameTexts.SetVariable("EXTEND_KEY", propertyBasedTooltipVM.GetKeyText(PropertyBasedTooltipVMExtensions.ExtendKeyId));
				propertyBasedTooltipVM.AddProperty("", GameTexts.FindText("str_map_tooltip_info", null).ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
			}
		}

		public static void UpdateTooltip(this PropertyBasedTooltipVM propertyBasedTooltipVM, MapEvent mapEvent)
		{
			propertyBasedTooltipVM.Mode = 4;
			TooltipProperty.TooltipPropertyFlags tooltipPropertyFlags;
			if (FactionManager.IsAtWarAgainstFaction(mapEvent.AttackerSide.LeaderParty.MapFaction, PartyBase.MainParty.MapFaction))
			{
				tooltipPropertyFlags = TooltipProperty.TooltipPropertyFlags.WarFirstEnemy;
			}
			else if (mapEvent.AttackerSide.LeaderParty.MapFaction == PartyBase.MainParty.MapFaction || FactionManager.IsAlliedWithFaction(mapEvent.AttackerSide.LeaderParty.MapFaction, PartyBase.MainParty.MapFaction))
			{
				tooltipPropertyFlags = TooltipProperty.TooltipPropertyFlags.WarFirstAlly;
			}
			else
			{
				tooltipPropertyFlags = TooltipProperty.TooltipPropertyFlags.WarFirstNeutral;
			}
			TooltipProperty.TooltipPropertyFlags tooltipPropertyFlags2;
			if (FactionManager.IsAtWarAgainstFaction(mapEvent.DefenderSide.LeaderParty.MapFaction, PartyBase.MainParty.MapFaction))
			{
				tooltipPropertyFlags2 = TooltipProperty.TooltipPropertyFlags.WarSecondEnemy;
			}
			else if (mapEvent.DefenderSide.LeaderParty.MapFaction == PartyBase.MainParty.MapFaction || FactionManager.IsAlliedWithFaction(mapEvent.DefenderSide.LeaderParty.MapFaction, PartyBase.MainParty.MapFaction))
			{
				tooltipPropertyFlags2 = TooltipProperty.TooltipPropertyFlags.WarSecondAlly;
			}
			else
			{
				tooltipPropertyFlags2 = TooltipProperty.TooltipPropertyFlags.WarSecondNeutral;
			}
			propertyBasedTooltipVM.AddProperty("", "", 1, tooltipPropertyFlags | tooltipPropertyFlags2);
			if (mapEvent.IsSiegeAssault)
			{
				TextObject textObject = new TextObject("{=43HYUImy}{SETTLEMENT}'s Siege", null);
				textObject.SetTextVariable("SETTLEMENT", mapEvent.MapEventSettlement.Name);
				propertyBasedTooltipVM.AddProperty("", textObject.ToString(), 0, TooltipProperty.TooltipPropertyFlags.Title);
			}
			else if (mapEvent.IsRaid)
			{
				TextObject textObject2 = new TextObject("{=T9bndUYP}{SETTLEMENT}'s Raid", null);
				textObject2.SetTextVariable("SETTLEMENT", mapEvent.MapEventSettlement.Name);
				propertyBasedTooltipVM.AddProperty("", textObject2.ToString(), 0, TooltipProperty.TooltipPropertyFlags.Title);
			}
			else
			{
				TextObject textObject3 = new TextObject("{=CnsIzaWo}Field Battle", null);
				propertyBasedTooltipVM.AddProperty("", textObject3.ToString(), 0, TooltipProperty.TooltipPropertyFlags.Title);
			}
			propertyBasedTooltipVM.AddProperty("", "", -1, TooltipProperty.TooltipPropertyFlags.None);
			propertyBasedTooltipVM.AddEncounterParties(mapEvent.AttackerSide.Parties, mapEvent.DefenderSide.Parties, propertyBasedTooltipVM.IsExtended);
			if (!propertyBasedTooltipVM.IsExtended)
			{
				propertyBasedTooltipVM.AddProperty("", "", -1, TooltipProperty.TooltipPropertyFlags.None);
				GameTexts.SetVariable("EXTEND_KEY", propertyBasedTooltipVM.GetKeyText(PropertyBasedTooltipVMExtensions.ExtendKeyId));
				propertyBasedTooltipVM.AddProperty("", GameTexts.FindText("str_map_tooltip_info", null).ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
			}
		}

		public static void UpdateTooltip(this PropertyBasedTooltipVM propertyBasedTooltipVM, Settlement settlement, bool openedFromMenuLayout)
		{
			PropertyBasedTooltipVMExtensions.<>c__DisplayClass15_0 CS$<>8__locals1 = new PropertyBasedTooltipVMExtensions.<>c__DisplayClass15_0();
			CS$<>8__locals1.settlement = settlement;
			CS$<>8__locals1.settlementAsParty = CS$<>8__locals1.settlement.Party;
			if (CS$<>8__locals1.settlementAsParty != null)
			{
				if (FactionManager.IsAtWarAgainstFaction(CS$<>8__locals1.settlementAsParty.MapFaction, PartyBase.MainParty.MapFaction))
				{
					propertyBasedTooltipVM.Mode = 3;
				}
				else if (CS$<>8__locals1.settlementAsParty.MapFaction == PartyBase.MainParty.MapFaction || FactionManager.IsAlliedWithFaction(CS$<>8__locals1.settlementAsParty.MapFaction, PartyBase.MainParty.MapFaction))
				{
					propertyBasedTooltipVM.Mode = 2;
				}
				else
				{
					propertyBasedTooltipVM.Mode = 1;
				}
				if (Game.Current.IsDevelopmentMode && CS$<>8__locals1.settlement.IsHideout)
				{
					propertyBasedTooltipVM.AddProperty("", string.Concat(new object[]
					{
						CS$<>8__locals1.settlement.Name,
						" (",
						CS$<>8__locals1.settlementAsParty.Id,
						")"
					}), 1, TooltipProperty.TooltipPropertyFlags.None);
				}
				else
				{
					propertyBasedTooltipVM.AddProperty("", CS$<>8__locals1.settlement.Name.ToString(), 0, TooltipProperty.TooltipPropertyFlags.Title);
				}
				TextObject textObject;
				bool flag = !CampaignUIHelper.IsSettlementInformationHidden(CS$<>8__locals1.settlement, out textObject);
				propertyBasedTooltipVM.AddProperty(string.Empty, string.Empty, -1, TooltipProperty.TooltipPropertyFlags.None);
				propertyBasedTooltipVM.AddProperty(GameTexts.FindText("str_owner", null).ToString(), " ", 0, TooltipProperty.TooltipPropertyFlags.None);
				propertyBasedTooltipVM.AddProperty("", "", 0, TooltipProperty.TooltipPropertyFlags.RundownSeperator);
				TextObject textObject2 = new TextObject("{=HAaElX8X}{PARTY_OWNERS_FACTION}", null);
				TextObject textObject3 = ((CS$<>8__locals1.settlement.OwnerClan == null) ? new TextObject("{=3PzgpFGq}Neutral", null) : CS$<>8__locals1.settlement.OwnerClan.Name);
				textObject2.SetTextVariable("PARTY_OWNERS_FACTION", textObject3);
				propertyBasedTooltipVM.AddProperty(GameTexts.FindText("str_clan", null).ToString(), textObject2.ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
				if (CS$<>8__locals1.settlementAsParty.MapFaction != null)
				{
					TextObject textObject4 = new TextObject("{=s6koeapc}{MAP_FACTION}", null);
					TextObject textObject5 = textObject4;
					string text = "MAP_FACTION";
					IFaction mapFaction = CS$<>8__locals1.settlementAsParty.MapFaction;
					textObject5.SetTextVariable(text, ((mapFaction != null) ? mapFaction.Name : null) ?? new TextObject("{=!}ERROR", null));
					propertyBasedTooltipVM.AddProperty(GameTexts.FindText("str_faction", null).ToString(), textObject4.ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
				}
				if (CS$<>8__locals1.settlement.Culture != null && !TextObject.IsNullOrEmpty(CS$<>8__locals1.settlement.Culture.Name))
				{
					TextObject textObject6 = new TextObject("{=!}{CULTURE}", null);
					textObject6.SetTextVariable("CULTURE", CS$<>8__locals1.settlement.Culture.Name);
					propertyBasedTooltipVM.AddProperty(GameTexts.FindText("str_culture", null).ToString(), textObject6.ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
				}
				if (flag)
				{
					if (CS$<>8__locals1.settlementAsParty.IsSettlement && (CS$<>8__locals1.settlementAsParty.Settlement.IsVillage || CS$<>8__locals1.settlementAsParty.Settlement.IsTown || CS$<>8__locals1.settlementAsParty.Settlement.IsCastle))
					{
						propertyBasedTooltipVM.AddProperty(string.Empty, string.Empty, -1, TooltipProperty.TooltipPropertyFlags.None);
						propertyBasedTooltipVM.AddProperty(GameTexts.FindText("str_information", null).ToString(), " ", 0, TooltipProperty.TooltipPropertyFlags.None);
						propertyBasedTooltipVM.AddProperty("", "", 0, TooltipProperty.TooltipPropertyFlags.RundownSeperator);
					}
					if (CS$<>8__locals1.settlement.IsVillage || CS$<>8__locals1.settlement.IsFortification)
					{
						propertyBasedTooltipVM.AddProperty(CS$<>8__locals1.settlementAsParty.Settlement.IsFortification ? GameTexts.FindText("str_map_tooltip_prosperity", null).ToString() : GameTexts.FindText("str_map_tooltip_hearths", null).ToString(), new Func<string>(CS$<>8__locals1.<UpdateTooltip>g__getProsperity|0), 0, TooltipProperty.TooltipPropertyFlags.None);
					}
					if (CS$<>8__locals1.settlement.IsFortification)
					{
						propertyBasedTooltipVM.AddProperty(GameTexts.FindText("str_loyalty", null).ToString(), new Func<string>(CS$<>8__locals1.<UpdateTooltip>g__getLoyalty|1), 0, TooltipProperty.TooltipPropertyFlags.None);
						propertyBasedTooltipVM.AddProperty(GameTexts.FindText("str_security", null).ToString(), CS$<>8__locals1.<UpdateTooltip>g__getSecurity|2(), 0, TooltipProperty.TooltipPropertyFlags.None);
					}
					if (CS$<>8__locals1.settlement.IsVillage || CS$<>8__locals1.settlement.IsFortification)
					{
						propertyBasedTooltipVM.AddProperty(GameTexts.FindText("str_map_tooltip_militia", null).ToString(), new Func<string>(CS$<>8__locals1.<UpdateTooltip>g__getMilitia|3), 0, TooltipProperty.TooltipPropertyFlags.None);
					}
					if (CS$<>8__locals1.settlement.IsFortification)
					{
						propertyBasedTooltipVM.AddProperty(GameTexts.FindText("str_map_tooltip_garrison", null).ToString(), new Func<string>(CS$<>8__locals1.<UpdateTooltip>g__getGarrison|6), 0, TooltipProperty.TooltipPropertyFlags.None);
						propertyBasedTooltipVM.AddProperty(GameTexts.FindText("str_map_tooltip_food_stocks", null).ToString(), new Func<string>(CS$<>8__locals1.<UpdateTooltip>g__getFood|7), 0, TooltipProperty.TooltipPropertyFlags.None);
						int wallLevel = CS$<>8__locals1.settlementAsParty.Settlement.Town.GetWallLevel();
						propertyBasedTooltipVM.AddProperty(GameTexts.FindText("str_map_tooltip_wall_level", null).ToString(), wallLevel.ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
					}
				}
				if (CS$<>8__locals1.settlement.IsVillage)
				{
					string text2 = GameTexts.FindText("str_bound_settlement", null).ToString();
					string text3 = CS$<>8__locals1.settlementAsParty.Settlement.Village.Bound.Name.ToString();
					propertyBasedTooltipVM.AddProperty(text2, text3, 0, TooltipProperty.TooltipPropertyFlags.None);
					if (CS$<>8__locals1.settlementAsParty.Settlement.Village.TradeBound != null)
					{
						string text4 = GameTexts.FindText("str_trade_bound_settlement", null).ToString();
						string text5 = CS$<>8__locals1.settlementAsParty.Settlement.Village.TradeBound.Name.ToString();
						propertyBasedTooltipVM.AddProperty(text4, text5, 0, TooltipProperty.TooltipPropertyFlags.None);
					}
					ItemObject primaryProduction = CS$<>8__locals1.settlementAsParty.Settlement.Village.VillageType.PrimaryProduction;
					propertyBasedTooltipVM.AddProperty(GameTexts.FindText("str_primary_production", null).ToString(), primaryProduction.Name.ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
				}
				if (CS$<>8__locals1.settlement.BoundVillages.Count > 0)
				{
					string text6 = "";
					string text7 = GameTexts.FindText("str_bound_village", null).ToString();
					if (CS$<>8__locals1.settlementAsParty.Settlement.BoundVillages.Count == 1)
					{
						text6 = CS$<>8__locals1.settlementAsParty.Settlement.BoundVillages[0].Name.ToString();
					}
					else
					{
						for (int i = 0; i < CS$<>8__locals1.settlementAsParty.Settlement.BoundVillages.Count; i++)
						{
							if (i + 1 != CS$<>8__locals1.settlementAsParty.Settlement.BoundVillages.Count)
							{
								text6 = text6 + CS$<>8__locals1.settlementAsParty.Settlement.BoundVillages[i].Name.ToString() + ",\n";
							}
							else
							{
								text6 += CS$<>8__locals1.settlementAsParty.Settlement.BoundVillages[i].Name.ToString();
							}
						}
					}
					propertyBasedTooltipVM.AddProperty(text7, text6, 0, TooltipProperty.TooltipPropertyFlags.None);
					if (propertyBasedTooltipVM.IsExtended && CS$<>8__locals1.settlement.IsTown && CS$<>8__locals1.settlement.Town.TradeBoundVillages.Count > 0)
					{
						string text8 = "";
						string text9 = GameTexts.FindText("str_trade_bound_village", null).ToString();
						for (int j = 0; j < CS$<>8__locals1.settlement.Town.TradeBoundVillages.Count; j++)
						{
							if (j + 1 != CS$<>8__locals1.settlement.Town.TradeBoundVillages.Count)
							{
								text8 = text8 + CS$<>8__locals1.settlement.Town.TradeBoundVillages[j].Name.ToString() + ",\n";
							}
							else
							{
								text8 += CS$<>8__locals1.settlement.Town.TradeBoundVillages[j].Name.ToString();
							}
						}
						propertyBasedTooltipVM.AddProperty(text9, text8, 0, TooltipProperty.TooltipPropertyFlags.None);
					}
				}
				if (Game.Current.IsDevelopmentMode && CS$<>8__locals1.settlement.IsTown)
				{
					propertyBasedTooltipVM.AddProperty(string.Empty, string.Empty, -1, TooltipProperty.TooltipPropertyFlags.None);
					propertyBasedTooltipVM.AddProperty("[DEV] " + GameTexts.FindText("str_shops", null).ToString(), " ", 0, TooltipProperty.TooltipPropertyFlags.None);
					propertyBasedTooltipVM.AddProperty("", "", 0, TooltipProperty.TooltipPropertyFlags.RundownSeperator);
					int num = 1;
					foreach (Workshop workshop in CS$<>8__locals1.settlementAsParty.Settlement.Town.Workshops)
					{
						if (workshop.WorkshopType != null)
						{
							propertyBasedTooltipVM.AddProperty("[DEV] Shop " + num.ToString(), workshop.WorkshopType.Name.ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
							num++;
						}
					}
				}
				TroopRoster troopRoster = TroopRoster.CreateDummyTroopRoster();
				TroopRoster troopRoster2 = TroopRoster.CreateDummyTroopRoster();
				TroopRoster.CreateDummyTroopRoster();
				Func<TroopRoster> func = delegate
				{
					TroopRoster troopRoster3 = TroopRoster.CreateDummyTroopRoster();
					foreach (MobileParty mobileParty3 in CS$<>8__locals1.settlement.Parties)
					{
						if ((mobileParty3.Aggressiveness >= 0.01f || mobileParty3.IsGarrison || mobileParty3.IsMilitia) && !mobileParty3.IsMainParty)
						{
							for (int l = 0; l < mobileParty3.MemberRoster.Count; l++)
							{
								TroopRosterElement elementCopyAtIndex = mobileParty3.MemberRoster.GetElementCopyAtIndex(l);
								troopRoster3.AddToCounts(elementCopyAtIndex.Character, elementCopyAtIndex.Number, false, elementCopyAtIndex.WoundedNumber, 0, true, -1);
							}
						}
					}
					return troopRoster3;
				};
				Func<TroopRoster> func2 = delegate
				{
					TroopRoster troopRoster4 = TroopRoster.CreateDummyTroopRoster();
					foreach (MobileParty mobileParty4 in CS$<>8__locals1.settlement.Parties)
					{
						if (!mobileParty4.IsMainParty && !FactionManager.IsAtWarAgainstFaction(mobileParty4.MapFaction, CS$<>8__locals1.settlementAsParty.MapFaction))
						{
							for (int m = 0; m < mobileParty4.PrisonRoster.Count; m++)
							{
								TroopRosterElement elementCopyAtIndex2 = mobileParty4.PrisonRoster.GetElementCopyAtIndex(m);
								troopRoster4.AddToCounts(elementCopyAtIndex2.Character, elementCopyAtIndex2.Number, false, elementCopyAtIndex2.WoundedNumber, 0, true, -1);
							}
						}
					}
					for (int n = 0; n < CS$<>8__locals1.settlementAsParty.PrisonRoster.Count; n++)
					{
						TroopRosterElement elementCopyAtIndex3 = CS$<>8__locals1.settlementAsParty.PrisonRoster.GetElementCopyAtIndex(n);
						troopRoster4.AddToCounts(elementCopyAtIndex3.Character, elementCopyAtIndex3.Number, false, elementCopyAtIndex3.WoundedNumber, 0, true, -1);
					}
					return troopRoster4;
				};
				troopRoster = func();
				troopRoster2 = func2();
				if (propertyBasedTooltipVM.IsExtended)
				{
					if (troopRoster.Count > 0)
					{
						propertyBasedTooltipVM.AddPartyTroopProperties(troopRoster, GameTexts.FindText("str_map_tooltip_troops", null), openedFromMenuLayout || flag, func);
					}
				}
				else
				{
					propertyBasedTooltipVM.AddProperty(string.Empty, string.Empty, -1, TooltipProperty.TooltipPropertyFlags.None);
					if (!CS$<>8__locals1.settlement.IsHideout && (flag || openedFromMenuLayout))
					{
						List<MobileParty> list = CS$<>8__locals1.settlement.Parties.Where(delegate(MobileParty p)
						{
							if (!p.IsLordParty && (!p.IsMilitia || (CS$<>8__locals1.settlement.IsTown && CS$<>8__locals1.settlement.Town.InRebelliousState)) && !p.IsGarrison)
							{
								return false;
							}
							if (p.IsMainParty)
							{
								IFaction mapFaction2 = CS$<>8__locals1.settlementAsParty.MapFaction;
								return mapFaction2 == null || !mapFaction2.IsAtWarWith(Hero.MainHero.MapFaction);
							}
							return true;
						}).ToList<MobileParty>();
						list.Sort(CampaignUIHelper.MobilePartyPrecedenceComparerInstance);
						List<MobileParty> list2 = CS$<>8__locals1.settlement.Parties.Where((MobileParty p) => !p.IsLordParty && !p.IsMilitia && !p.IsGarrison).ToList<MobileParty>();
						list2.Sort(CampaignUIHelper.MobilePartyPrecedenceComparerInstance);
						if (list.Count > 0)
						{
							int num2 = list.Sum((MobileParty p) => p.Party.NumberOfHealthyMembers);
							int num3 = list.Sum((MobileParty p) => p.Party.NumberOfWoundedTotalMembers);
							string text10 = num2 + ((num3 > 0) ? ("+" + num3 + GameTexts.FindText("str_party_nameplate_wounded_abbr", null).ToString()) : "");
							propertyBasedTooltipVM.AddProperty(GameTexts.FindText("str_map_tooltip_defenders", null).ToString(), text10, 0, TooltipProperty.TooltipPropertyFlags.None);
							propertyBasedTooltipVM.AddProperty("", "", 0, TooltipProperty.TooltipPropertyFlags.RundownSeperator);
							foreach (MobileParty mobileParty in list)
							{
								propertyBasedTooltipVM.AddProperty(mobileParty.Name.ToString(), CampaignUIHelper.GetPartyNameplateText(mobileParty), 0, TooltipProperty.TooltipPropertyFlags.None);
							}
							propertyBasedTooltipVM.AddProperty(string.Empty, string.Empty, -1, TooltipProperty.TooltipPropertyFlags.None);
						}
						if (list2.Count <= 0)
						{
							goto IL_B3D;
						}
						propertyBasedTooltipVM.AddProperty("", "", 0, TooltipProperty.TooltipPropertyFlags.DefaultSeperator);
						using (List<MobileParty>.Enumerator enumerator = list2.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								MobileParty mobileParty2 = enumerator.Current;
								propertyBasedTooltipVM.AddProperty(mobileParty2.Name.ToString(), CampaignUIHelper.GetPartyNameplateText(mobileParty2), 0, TooltipProperty.TooltipPropertyFlags.None);
							}
							goto IL_B3D;
						}
					}
					string text11 = GameTexts.FindText("str_missing_info_indicator", null).ToString();
					propertyBasedTooltipVM.AddProperty(GameTexts.FindText("str_map_tooltip_parties", null).ToString(), text11, 0, TooltipProperty.TooltipPropertyFlags.None);
				}
				IL_B3D:
				if (!CS$<>8__locals1.settlement.IsHideout && troopRoster2.Count > 0 && (flag || openedFromMenuLayout))
				{
					propertyBasedTooltipVM.AddPartyTroopProperties(troopRoster2, GameTexts.FindText("str_map_tooltip_prisoners", null), flag, func2);
				}
				if (CS$<>8__locals1.settlement.IsFortification && CS$<>8__locals1.settlement.Town.InRebelliousState)
				{
					propertyBasedTooltipVM.AddProperty(string.Empty, GameTexts.FindText("str_settlement_rebellious_state", null).ToString(), -1, TooltipProperty.TooltipPropertyFlags.None);
				}
				propertyBasedTooltipVM.AddProperty(string.Empty, string.Empty, -1, TooltipProperty.TooltipPropertyFlags.None);
				if (!CS$<>8__locals1.settlement.IsHideout && !propertyBasedTooltipVM.IsExtended && (flag || openedFromMenuLayout))
				{
					GameTexts.SetVariable("EXTEND_KEY", propertyBasedTooltipVM.GetKeyText(PropertyBasedTooltipVMExtensions.ExtendKeyId));
					propertyBasedTooltipVM.AddProperty(string.Empty, GameTexts.FindText("str_map_tooltip_info", null).ToString(), -1, TooltipProperty.TooltipPropertyFlags.None);
				}
			}
		}

		public static void UpdateTooltip(this PropertyBasedTooltipVM propertyBasedTooltipVM, MobileParty mobileParty, bool openedFromMenuLayout, bool checkForMapVisibility)
		{
			if (mobileParty != null)
			{
				if (FactionManager.IsAtWarAgainstFaction(mobileParty.MapFaction, PartyBase.MainParty.MapFaction))
				{
					propertyBasedTooltipVM.Mode = 3;
				}
				else if (mobileParty.MapFaction == PartyBase.MainParty.MapFaction || FactionManager.IsAlliedWithFaction(mobileParty.MapFaction, PartyBase.MainParty.MapFaction))
				{
					propertyBasedTooltipVM.Mode = 2;
				}
				else
				{
					propertyBasedTooltipVM.Mode = 1;
				}
				if (Game.Current.IsDevelopmentMode)
				{
					propertyBasedTooltipVM.AddProperty("", string.Concat(new object[] { mobileParty.Name, " (", mobileParty.Id, ")" }), 1, TooltipProperty.TooltipPropertyFlags.Title);
				}
				else
				{
					propertyBasedTooltipVM.AddProperty("", mobileParty.Name.ToString(), 0, TooltipProperty.TooltipPropertyFlags.Title);
				}
				bool isInspected = mobileParty.IsInspected;
				if (mobileParty.IsDisorganized)
				{
					TextObject hoursAndDaysTextFromHourValue = CampaignUIHelper.GetHoursAndDaysTextFromHourValue(MathF.Ceiling(mobileParty.DisorganizedUntilTime.RemainingHoursFromNow));
					TextObject textObject = new TextObject("{=BbLTwhsA}Disorganized for {REMAINING_TIME}", null);
					textObject.SetTextVariable("REMAINING_TIME", hoursAndDaysTextFromHourValue.ToString());
					propertyBasedTooltipVM.AddProperty("", textObject.ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
					propertyBasedTooltipVM.AddProperty("", "", -1, TooltipProperty.TooltipPropertyFlags.None);
				}
				if (isInspected)
				{
					propertyBasedTooltipVM.AddProperty("", CampaignUIHelper.GetMobilePartyBehaviorText(mobileParty), 0, TooltipProperty.TooltipPropertyFlags.None);
				}
				propertyBasedTooltipVM.AddProperty(string.Empty, string.Empty, -1, TooltipProperty.TooltipPropertyFlags.None);
				propertyBasedTooltipVM.AddProperty(GameTexts.FindText("str_owner", null).ToString(), " ", 0, TooltipProperty.TooltipPropertyFlags.None);
				propertyBasedTooltipVM.AddProperty("", "", 0, TooltipProperty.TooltipPropertyFlags.RundownSeperator);
				if (mobileParty.LeaderHero != null && mobileParty.LeaderHero.Clan != mobileParty.MapFaction)
				{
					TextObject textObject2 = new TextObject("{=oUhd9YhP}{PARTY_LEADERS_FACTION}", null);
					textObject2.SetTextVariable("PARTY_LEADERS_FACTION", mobileParty.LeaderHero.Clan.Name);
					propertyBasedTooltipVM.AddProperty(GameTexts.FindText("str_clan", null).ToString(), textObject2.ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
				}
				if (mobileParty.MapFaction != null)
				{
					TextObject textObject3 = new TextObject("{=s6koeapc}{MAP_FACTION}", null);
					TextObject textObject4 = textObject3;
					string text = "MAP_FACTION";
					IFaction mapFaction = mobileParty.MapFaction;
					textObject4.SetTextVariable(text, ((mapFaction != null) ? mapFaction.Name : null) ?? new TextObject("{=!}ERROR", null));
					propertyBasedTooltipVM.AddProperty(GameTexts.FindText("str_faction", null).ToString(), textObject3.ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
				}
				if (isInspected)
				{
					propertyBasedTooltipVM.AddProperty(string.Empty, string.Empty, -1, TooltipProperty.TooltipPropertyFlags.None);
					propertyBasedTooltipVM.AddProperty(GameTexts.FindText("str_information", null).ToString(), " ", 0, TooltipProperty.TooltipPropertyFlags.None);
					propertyBasedTooltipVM.AddProperty("", "", 0, TooltipProperty.TooltipPropertyFlags.RundownSeperator);
					propertyBasedTooltipVM.AddProperty(GameTexts.FindText("str_map_tooltip_speed", null).ToString(), CampaignUIHelper.FloatToString(mobileParty.Speed), 0, TooltipProperty.TooltipPropertyFlags.None);
					if (propertyBasedTooltipVM.IsExtended)
					{
						TerrainType faceTerrainType = Campaign.Current.MapSceneWrapper.GetFaceTerrainType(mobileParty.CurrentNavigationFace);
						string text2 = GameTexts.FindText("str_terrain", null).ToString();
						string text3 = "str_terrain_types";
						int num = (int)faceTerrainType;
						propertyBasedTooltipVM.AddProperty(text2, GameTexts.FindText(text3, num.ToString()).ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
					}
				}
				TroopRoster troopRoster = TroopRoster.CreateDummyTroopRoster();
				TroopRoster troopRoster2 = TroopRoster.CreateDummyTroopRoster();
				TroopRoster.CreateDummyTroopRoster();
				Func<TroopRoster> func = delegate
				{
					TroopRoster troopRoster3 = TroopRoster.CreateDummyTroopRoster();
					for (int i = 0; i < mobileParty.MemberRoster.Count; i++)
					{
						TroopRosterElement elementCopyAtIndex = mobileParty.MemberRoster.GetElementCopyAtIndex(i);
						troopRoster3.AddToCounts(elementCopyAtIndex.Character, elementCopyAtIndex.Number, false, elementCopyAtIndex.WoundedNumber, 0, true, -1);
					}
					return troopRoster3;
				};
				Func<TroopRoster> func2 = delegate
				{
					TroopRoster troopRoster4 = TroopRoster.CreateDummyTroopRoster();
					for (int j = 0; j < mobileParty.PrisonRoster.Count; j++)
					{
						TroopRosterElement elementCopyAtIndex2 = mobileParty.PrisonRoster.GetElementCopyAtIndex(j);
						troopRoster4.AddToCounts(elementCopyAtIndex2.Character, elementCopyAtIndex2.Number, false, elementCopyAtIndex2.WoundedNumber, 0, true, -1);
					}
					return troopRoster4;
				};
				troopRoster = func();
				troopRoster2 = func2();
				if (troopRoster.Count > 0)
				{
					propertyBasedTooltipVM.AddPartyTroopProperties(troopRoster, GameTexts.FindText("str_map_tooltip_troops", null), openedFromMenuLayout || isInspected || !checkForMapVisibility, func);
				}
				if (troopRoster2.Count > 0 && (isInspected || openedFromMenuLayout))
				{
					propertyBasedTooltipVM.AddPartyTroopProperties(troopRoster2, GameTexts.FindText("str_map_tooltip_prisoners", null), isInspected || !checkForMapVisibility, func2);
				}
				propertyBasedTooltipVM.AddProperty(string.Empty, string.Empty, -1, TooltipProperty.TooltipPropertyFlags.None);
				if (!propertyBasedTooltipVM.IsExtended && (isInspected || openedFromMenuLayout))
				{
					GameTexts.SetVariable("EXTEND_KEY", propertyBasedTooltipVM.GetKeyText(PropertyBasedTooltipVMExtensions.ExtendKeyId));
					propertyBasedTooltipVM.AddProperty(string.Empty, GameTexts.FindText("str_map_tooltip_info", null).ToString(), -1, TooltipProperty.TooltipPropertyFlags.None);
				}
				if (mobileParty != MobileParty.MainParty && !openedFromMenuLayout)
				{
					GameTexts.SetVariable("MODIFIER_KEY", propertyBasedTooltipVM.GetKeyText(PropertyBasedTooltipVMExtensions.FollowModifierKeyId));
					GameTexts.SetVariable("CLICK_KEY", propertyBasedTooltipVM.GetKeyText(PropertyBasedTooltipVMExtensions.MapClickKeyId));
					propertyBasedTooltipVM.AddProperty(string.Empty, GameTexts.FindText("str_map_follow_party_info", null).ToString(), -1, TooltipProperty.TooltipPropertyFlags.None);
				}
			}
		}

		public static void UpdateTooltip(this PropertyBasedTooltipVM propertyBasedTooltipVM, Army army, bool openedFromMenuLayout, bool checkForMapVisibility)
		{
			PropertyBasedTooltipVMExtensions.<>c__DisplayClass17_0 CS$<>8__locals1 = new PropertyBasedTooltipVMExtensions.<>c__DisplayClass17_0();
			CS$<>8__locals1.army = army;
			MobileParty leaderParty = CS$<>8__locals1.army.LeaderParty;
			if (leaderParty != null)
			{
				if (FactionManager.IsAtWarAgainstFaction(leaderParty.MapFaction, PartyBase.MainParty.MapFaction))
				{
					propertyBasedTooltipVM.Mode = 3;
				}
				else if (CS$<>8__locals1.army.MapFaction == PartyBase.MainParty.MapFaction || FactionManager.IsAlliedWithFaction(leaderParty.MapFaction, PartyBase.MainParty.MapFaction))
				{
					propertyBasedTooltipVM.Mode = 2;
				}
				else
				{
					propertyBasedTooltipVM.Mode = 1;
				}
				propertyBasedTooltipVM.AddProperty("", CS$<>8__locals1.army.Name.ToString(), 0, TooltipProperty.TooltipPropertyFlags.Title);
				if (leaderParty.IsInspected || !checkForMapVisibility)
				{
					propertyBasedTooltipVM.AddProperty("", CampaignUIHelper.GetMobilePartyBehaviorText(leaderParty), 0, TooltipProperty.TooltipPropertyFlags.None);
				}
				propertyBasedTooltipVM.AddProperty(string.Empty, string.Empty, -1, TooltipProperty.TooltipPropertyFlags.None);
				propertyBasedTooltipVM.AddProperty(GameTexts.FindText("str_owner", null).ToString(), " ", 0, TooltipProperty.TooltipPropertyFlags.None);
				propertyBasedTooltipVM.AddProperty("", "", 0, TooltipProperty.TooltipPropertyFlags.RundownSeperator);
				if (CS$<>8__locals1.army.MapFaction != null)
				{
					TextObject textObject = new TextObject("{=s6koeapc}{MAP_FACTION}", null);
					TextObject textObject2 = textObject;
					string text = "MAP_FACTION";
					Kingdom mapFaction = CS$<>8__locals1.army.MapFaction;
					textObject2.SetTextVariable(text, ((mapFaction != null) ? mapFaction.Name : null) ?? new TextObject("{=!}ERROR", null));
					propertyBasedTooltipVM.AddProperty(GameTexts.FindText("str_faction", null).ToString(), textObject.ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
				}
				if (leaderParty.IsInspected || !checkForMapVisibility)
				{
					propertyBasedTooltipVM.AddProperty(string.Empty, string.Empty, -1, TooltipProperty.TooltipPropertyFlags.None);
					propertyBasedTooltipVM.AddProperty(GameTexts.FindText("str_information", null).ToString(), " ", 0, TooltipProperty.TooltipPropertyFlags.None);
					propertyBasedTooltipVM.AddProperty("", "", 0, TooltipProperty.TooltipPropertyFlags.RundownSeperator);
					propertyBasedTooltipVM.AddProperty(GameTexts.FindText("str_map_tooltip_speed", null).ToString(), CampaignUIHelper.FloatToString(leaderParty.Speed), 0, TooltipProperty.TooltipPropertyFlags.None);
					if (propertyBasedTooltipVM.IsExtended)
					{
						TerrainType faceTerrainType = Campaign.Current.MapSceneWrapper.GetFaceTerrainType(leaderParty.CurrentNavigationFace);
						string text2 = GameTexts.FindText("str_terrain", null).ToString();
						string text3 = "str_terrain_types";
						int num = (int)faceTerrainType;
						propertyBasedTooltipVM.AddProperty(text2, GameTexts.FindText(text3, num.ToString()).ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
					}
				}
				TroopRoster troopRoster = CS$<>8__locals1.<UpdateTooltip>g__GettingTempRoster|0();
				TroopRoster troopRoster2 = CS$<>8__locals1.<UpdateTooltip>g__GettingTempPrisonerRoster|1();
				if (troopRoster.Count > 0)
				{
					propertyBasedTooltipVM.AddPartyTroopProperties(troopRoster, GameTexts.FindText("str_map_tooltip_troops", null), openedFromMenuLayout || leaderParty.IsInspected, new Func<TroopRoster>(CS$<>8__locals1.<UpdateTooltip>g__GettingTempRoster|0));
				}
				if (troopRoster2.Count > 0 && (leaderParty.IsInspected || openedFromMenuLayout || !checkForMapVisibility))
				{
					propertyBasedTooltipVM.AddPartyTroopProperties(troopRoster2, GameTexts.FindText("str_map_tooltip_prisoners", null), leaderParty.IsInspected || !checkForMapVisibility, new Func<TroopRoster>(CS$<>8__locals1.<UpdateTooltip>g__GettingTempPrisonerRoster|1));
				}
			}
		}

		public static void AddPartyTroopProperties(this PropertyBasedTooltipVM propertyBasedTooltipVM, TroopRoster troopRoster, TextObject title, bool isInspected, Func<TroopRoster> funcToDoBeforeLambda = null)
		{
			propertyBasedTooltipVM.AddProperty(string.Empty, string.Empty, -1, TooltipProperty.TooltipPropertyFlags.None);
			propertyBasedTooltipVM.AddProperty(title.ToString(), delegate
			{
				TroopRoster troopRoster2 = ((funcToDoBeforeLambda != null) ? funcToDoBeforeLambda() : troopRoster);
				int num2 = 0;
				int num3 = 0;
				for (int l = 0; l < troopRoster2.Count; l++)
				{
					TroopRosterElement elementCopyAtIndex3 = troopRoster2.GetElementCopyAtIndex(l);
					num2 += elementCopyAtIndex3.Number - elementCopyAtIndex3.WoundedNumber;
					num3 += elementCopyAtIndex3.WoundedNumber;
				}
				TextObject textObject3 = new TextObject("{=iXXTONWb} ({PARTY_SIZE})", null);
				textObject3.SetTextVariable("PARTY_SIZE", PartyBaseHelper.GetPartySizeText(num2, num3, isInspected));
				return textObject3.ToString();
			}, 0, TooltipProperty.TooltipPropertyFlags.None);
			if (isInspected)
			{
				propertyBasedTooltipVM.AddProperty("", "", 0, TooltipProperty.TooltipPropertyFlags.RundownSeperator);
			}
			if (isInspected)
			{
				Dictionary<FormationClass, Tuple<int, int>> dictionary = new Dictionary<FormationClass, Tuple<int, int>>();
				for (int i = 0; i < troopRoster.Count; i++)
				{
					TroopRosterElement elementCopyAtIndex = troopRoster.GetElementCopyAtIndex(i);
					if (dictionary.ContainsKey(elementCopyAtIndex.Character.DefaultFormationClass))
					{
						Tuple<int, int> tuple = dictionary[elementCopyAtIndex.Character.DefaultFormationClass];
						dictionary[elementCopyAtIndex.Character.DefaultFormationClass] = new Tuple<int, int>(tuple.Item1 + elementCopyAtIndex.Number - elementCopyAtIndex.WoundedNumber, tuple.Item2 + elementCopyAtIndex.WoundedNumber);
					}
					else
					{
						dictionary.Add(elementCopyAtIndex.Character.DefaultFormationClass, new Tuple<int, int>(elementCopyAtIndex.Number - elementCopyAtIndex.WoundedNumber, elementCopyAtIndex.WoundedNumber));
					}
				}
				foreach (KeyValuePair<FormationClass, Tuple<int, int>> keyValuePair in dictionary.OrderBy((KeyValuePair<FormationClass, Tuple<int, int>> x) => x.Key))
				{
					TextObject textObject = new TextObject("{=Dqydb21E} {PARTY_SIZE}", null);
					textObject.SetTextVariable("PARTY_SIZE", PartyBaseHelper.GetPartySizeText(keyValuePair.Value.Item1, keyValuePair.Value.Item2, true));
					TextObject textObject2 = GameTexts.FindText("str_troop_type_name", keyValuePair.Key.GetName());
					propertyBasedTooltipVM.AddProperty(textObject2.ToString(), textObject.ToString(), 0, TooltipProperty.TooltipPropertyFlags.None);
				}
			}
			if (propertyBasedTooltipVM.IsExtended & isInspected)
			{
				propertyBasedTooltipVM.AddProperty(string.Empty, string.Empty, -1, TooltipProperty.TooltipPropertyFlags.None);
				propertyBasedTooltipVM.AddProperty(GameTexts.FindText("str_troop_types", null).ToString(), " ", 0, TooltipProperty.TooltipPropertyFlags.None);
				propertyBasedTooltipVM.AddProperty("", "", 0, TooltipProperty.TooltipPropertyFlags.DefaultSeperator);
				for (int j = 0; j < troopRoster.Count; j++)
				{
					TroopRosterElement elementCopyAtIndex2 = troopRoster.GetElementCopyAtIndex(j);
					if (elementCopyAtIndex2.Character.IsHero)
					{
						CharacterObject hero = elementCopyAtIndex2.Character;
						propertyBasedTooltipVM.AddProperty(elementCopyAtIndex2.Character.Name.ToString(), delegate
						{
							TroopRoster troopRoster3 = ((funcToDoBeforeLambda != null) ? funcToDoBeforeLambda() : troopRoster);
							int num4 = troopRoster3.FindIndexOfTroop(hero);
							if (num4 == -1)
							{
								return string.Empty;
							}
							TroopRosterElement elementCopyAtIndex4 = troopRoster3.GetElementCopyAtIndex(num4);
							TextObject textObject4 = new TextObject("{=aE4ZRbB6} {HEALTH}%", null);
							textObject4.SetTextVariable("HEALTH", elementCopyAtIndex4.Character.HeroObject.HitPoints * 100 / elementCopyAtIndex4.Character.MaxHitPoints());
							return textObject4.ToString();
						}, 0, TooltipProperty.TooltipPropertyFlags.None);
					}
				}
				for (int k = 0; k < troopRoster.Count; k++)
				{
					int num = k;
					CharacterObject character = troopRoster.GetElementCopyAtIndex(num).Character;
					if (!character.IsHero)
					{
						propertyBasedTooltipVM.AddProperty(character.Name.ToString(), delegate
						{
							TroopRoster troopRoster4 = ((funcToDoBeforeLambda != null) ? funcToDoBeforeLambda() : troopRoster);
							int num5 = troopRoster4.FindIndexOfTroop(character);
							if (num5 != -1)
							{
								if (num5 > troopRoster4.Count)
								{
									return string.Empty;
								}
								TroopRosterElement elementCopyAtIndex5 = troopRoster4.GetElementCopyAtIndex(num5);
								if (elementCopyAtIndex5.Character == null)
								{
									return string.Empty;
								}
								CharacterObject character2 = elementCopyAtIndex5.Character;
								if (character2 != null && !character2.IsHero)
								{
									TextObject textObject5 = new TextObject("{=QyVbwGLp}{PARTY_SIZE}", null);
									textObject5.SetTextVariable("PARTY_SIZE", PartyBaseHelper.GetPartySizeText(elementCopyAtIndex5.Number - elementCopyAtIndex5.WoundedNumber, elementCopyAtIndex5.WoundedNumber, true));
									return textObject5.ToString();
								}
							}
							return string.Empty;
						}, 0, TooltipProperty.TooltipPropertyFlags.None);
					}
				}
			}
		}

		public static void FillCampaignTooltipTypes()
		{
			PropertyBasedTooltipVM.AddTooltipType(typeof(List<MobileParty>), new Action<PropertyBasedTooltipVM, object[]>(PropertyBasedTooltipVMExtensions.EncounterAction));
			PropertyBasedTooltipVM.AddTooltipType(typeof(Track), new Action<PropertyBasedTooltipVM, object[]>(PropertyBasedTooltipVMExtensions.TrackBaseAction));
			PropertyBasedTooltipVM.AddTooltipType(typeof(MapEvent), new Action<PropertyBasedTooltipVM, object[]>(PropertyBasedTooltipVMExtensions.MapEventAction));
			PropertyBasedTooltipVM.AddTooltipType(typeof(Army), new Action<PropertyBasedTooltipVM, object[]>(PropertyBasedTooltipVMExtensions.ArmyAction));
			PropertyBasedTooltipVM.AddTooltipType(typeof(MobileParty), new Action<PropertyBasedTooltipVM, object[]>(PropertyBasedTooltipVMExtensions.MobilePartyAction));
			PropertyBasedTooltipVM.AddTooltipType(typeof(Hero), new Action<PropertyBasedTooltipVM, object[]>(PropertyBasedTooltipVMExtensions.HeroAction));
			PropertyBasedTooltipVM.AddTooltipType(typeof(Settlement), new Action<PropertyBasedTooltipVM, object[]>(PropertyBasedTooltipVMExtensions.SettlementAction));
			PropertyBasedTooltipVM.AddTooltipType(typeof(CharacterObject), new Action<PropertyBasedTooltipVM, object[]>(PropertyBasedTooltipVMExtensions.CharacterAction));
			PropertyBasedTooltipVM.AddTooltipType(typeof(WeaponDesignElement), new Action<PropertyBasedTooltipVM, object[]>(PropertyBasedTooltipVMExtensions.CraftingPartAction));
			PropertyBasedTooltipVM.AddTooltipType(typeof(InventoryLogic), new Action<PropertyBasedTooltipVM, object[]>(PropertyBasedTooltipVMExtensions.InventoryTooltipAction));
			PropertyBasedTooltipVM.AddTooltipType(typeof(ItemObject), new Action<PropertyBasedTooltipVM, object[]>(PropertyBasedTooltipVMExtensions.ItemTooltipAction));
			PropertyBasedTooltipVM.AddTooltipType(typeof(Building), new Action<PropertyBasedTooltipVM, object[]>(PropertyBasedTooltipVMExtensions.BuildingTooltipAction));
			PropertyBasedTooltipVM.AddTooltipType(typeof(Workshop), new Action<PropertyBasedTooltipVM, object[]>(PropertyBasedTooltipVMExtensions.WorkshopTypeTooltipAction));
		}

		private static void EncounterAction(PropertyBasedTooltipVM propertyBasedTooltipVM, object[] args)
		{
			propertyBasedTooltipVM.UpdateEncounterTooltip((int)args[0]);
		}

		private static void TrackBaseAction(PropertyBasedTooltipVM propertyBasedTooltipVM, object[] args)
		{
			propertyBasedTooltipVM.UpdateTooltip(args[0] as Track);
		}

		private static void MobilePartyAction(PropertyBasedTooltipVM propertyBasedTooltipVM, object[] args)
		{
			propertyBasedTooltipVM.UpdateTooltip((MobileParty)args[0], (bool)args[1], (bool)args[2]);
		}

		private static void SettlementAction(PropertyBasedTooltipVM propertyBasedTooltipVM, object[] args)
		{
			propertyBasedTooltipVM.UpdateTooltip((Settlement)args[0], (bool)args[1]);
		}

		private static void MapEventAction(PropertyBasedTooltipVM propertyBasedTooltipVM, object[] args)
		{
			propertyBasedTooltipVM.UpdateTooltip((MapEvent)args[0]);
		}

		private static void ArmyAction(PropertyBasedTooltipVM propertyBasedTooltipVM, object[] args)
		{
			propertyBasedTooltipVM.UpdateTooltip((Army)args[0], (bool)args[1], (bool)args[2]);
		}

		private static void HeroAction(PropertyBasedTooltipVM propertyBasedTooltipVM, object[] args)
		{
			propertyBasedTooltipVM.UpdateTooltip(args[0] as Hero, (bool)args[1]);
		}

		private static void CharacterAction(PropertyBasedTooltipVM propertyBasedTooltipVM, object[] args)
		{
			propertyBasedTooltipVM.UpdateTooltip(args[0] as CharacterObject);
		}

		private static void CraftingPartAction(PropertyBasedTooltipVM propertyBasedTooltipVM, object[] args)
		{
			propertyBasedTooltipVM.UpdateTooltip(args[0] as WeaponDesignElement);
		}

		private static void InventoryTooltipAction(PropertyBasedTooltipVM propertyBasedTooltipVM, object[] args)
		{
			propertyBasedTooltipVM.UpdateTooltip(args[0] as InventoryLogic);
		}

		public static void BuildingTooltipAction(this PropertyBasedTooltipVM propertyBasedTooltipVM, object[] args)
		{
			propertyBasedTooltipVM.UpdateTooltip(args[0] as Building);
		}

		public static void WorkshopTypeTooltipAction(this PropertyBasedTooltipVM propertyBasedTooltipVM, object[] args)
		{
			propertyBasedTooltipVM.UpdateTooltip(args[0] as Workshop);
		}

		private static void ItemTooltipAction(PropertyBasedTooltipVM propertyBasedTooltipVM, object[] args)
		{
			propertyBasedTooltipVM.UpdateTooltip(args[0] as EquipmentElement?);
		}

		private static readonly IEqualityComparer<ValueTuple<ItemCategory, int>> itemCategoryDistinctComparer = new CampaignUIHelper.ProductInputOutputEqualityComparer();

		private static string ExtendKeyId = "ExtendModifier";

		private static string FollowModifierKeyId = "FollowModifier";

		private static string MapClickKeyId = "MapClick";
	}
}
