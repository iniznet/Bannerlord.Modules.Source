using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Conversation.Tags;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Issues;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem
{
	public static class CampaignCheats
	{
		public static bool CheckCheatUsage(ref string ErrorType)
		{
			if (Campaign.Current == null)
			{
				ErrorType = "Campaign was not started.";
				return false;
			}
			if (!Game.Current.CheatMode)
			{
				ErrorType = "Cheat mode is disabled!";
				return false;
			}
			ErrorType = "";
			return true;
		}

		public static bool CheckParameters(List<string> strings, int ParameterCount)
		{
			if (strings.Count == 0)
			{
				return ParameterCount == 0;
			}
			return strings.Count == ParameterCount;
		}

		public static bool CheckHelp(List<string> strings)
		{
			return strings.Count != 0 && strings[0].ToLower() == "help";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("set_hero_crafting_stamina", "campaign")]
		public static string SetCraftingStamina(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckHelp(strings) || CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckParameters(strings, 1))
			{
				return "Format is \"campaign.set_hero_crafting_stamina [Stamina] [HeroName]\".";
			}
			ICraftingCampaignBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<ICraftingCampaignBehavior>();
			int num = 0;
			if (!int.TryParse(strings[0], out num) || num < 0 || num > 100)
			{
				return "Please enter a valid number between 0-100";
			}
			Hero hero = CampaignCheats.GetHero(CampaignCheats.ConcatenateString(strings.GetRange(1, strings.Count - 1)));
			if (hero != null)
			{
				int num2 = (int)((float)(campaignBehavior.GetMaxHeroCraftingStamina(hero) * num) / 100f);
				campaignBehavior.SetHeroCraftingStamina(hero, num2);
				return "Success";
			}
			return "Hero is not found";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("set_hero_culture", "campaign")]
		public static string SetHeroCulture(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckHelp(strings) || CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckParameters(strings, 1))
			{
				return "Format is \"campaign.set_hero_culture [HeroName] [CultureId]\".";
			}
			string text = strings[strings.Count - 1];
			CultureObject @object = Campaign.Current.ObjectManager.GetObject<CultureObject>(text);
			if (@object == null)
			{
				return "Culture couldn't be found!";
			}
			strings.RemoveAt(strings.Count - 1);
			Hero hero = CampaignCheats.GetHero(CampaignCheats.ConcatenateString(strings));
			if (hero == null)
			{
				return "Hero is not found";
			}
			if (hero.Culture == @object)
			{
				return string.Format("Hero culture is already {0}", @object.Name);
			}
			hero.Culture = @object;
			return "Success";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("make_hero_wounded", "campaign")]
		public static string MakeHeroWounded(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.make_hero_wounded [HeroName]\".";
			}
			Hero hero = CampaignCheats.GetHero(CampaignCheats.ConcatenateString(strings));
			if (hero != null)
			{
				hero.MakeWounded(null, KillCharacterAction.KillCharacterActionDetail.None);
				return "Success";
			}
			return "Hero is not found";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("reset_player_skills_level_and_perks", "campaign")]
		public static string ResetPlayerSkillsLevelAndPerk(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckParameters(strings, 0) && !CampaignCheats.CheckHelp(strings))
			{
				Hero.MainHero.CharacterObject.Level = 0;
				Hero.MainHero.HeroDeveloper.ClearHero();
				return "Success";
			}
			return "Format is \"campaign.reset_player_skills_level_and_perks\".";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("set_skills_of_hero", "campaign")]
		public static string SetSkillsOfGivenHero(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckHelp(strings) || CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckParameters(strings, 1))
			{
				return "Format is \"campaign.set_skills_of_hero [Level] [HeroName]\".";
			}
			int num = -1;
			Hero hero = null;
			if (!int.TryParse(strings[0], out num))
			{
				return "Level must be a number";
			}
			string text = CampaignCheats.ConcatenateString(strings.GetRange(1, strings.Count - 1));
			hero = CampaignCheats.GetHero(text);
			if (hero == null)
			{
				return "Hero: " + text + " not found.";
			}
			if (num > 0 && num <= 300)
			{
				hero.CharacterObject.Level = 0;
				hero.HeroDeveloper.ClearHero();
				Campaign.Current.Models.CharacterDevelopmentModel.GetXpRequiredForSkillLevel(num);
				int num2 = MathF.Min(num / 25 + 1, 10);
				foreach (SkillObject skillObject in Skills.All)
				{
					hero.HeroDeveloper.AddFocus(skillObject, num2, false);
					hero.HeroDeveloper.SetInitialSkillLevel(skillObject, num);
				}
				hero.HeroDeveloper.UnspentFocusPoints = 0;
				return string.Format("{0}'s skills are set to level {1}.", text, num);
			}
			return string.Format("Level must be between 0 - {0}.", 300);
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("set_parties_visible", "campaign")]
		public static string SetAllPartiesVisible(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			int num;
			if (CampaignCheats.CheckParameters(strings, 1) && !CampaignCheats.CheckHelp(strings) && int.TryParse(strings[0], out num) && (num == 1 || num == 0))
			{
				foreach (MobileParty mobileParty in MobileParty.All)
				{
					mobileParty.IsVisible = num == 1;
				}
				return "Success";
			}
			return "Format is \"campaign.set_parties_visible [1/0]\".";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("set_settlements_visible", "campaign")]
		public static string SetAllSettlementsVisible(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			int num;
			if (CampaignCheats.CheckParameters(strings, 1) && !CampaignCheats.CheckHelp(strings) && int.TryParse(strings[0], out num) && (num == 2 || num == 1 || num == 0))
			{
				foreach (Settlement settlement in Settlement.All)
				{
					bool flag = num != 0 && (!settlement.IsHideout || num == 1);
					settlement.IsVisible = flag;
					settlement.IsInspected = flag;
				}
				return "Success";
			}
			return "Format is \"campaign.set_settlements_visible [2(no hideouts)/1(all)/0(none)]\".";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("set_skill_main_hero", "campaign")]
		public static string SetSkillMainHero(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (!CampaignCheats.CheckParameters(strings, 1) && !CampaignCheats.CheckParameters(strings, 0) && !CampaignCheats.CheckHelp(strings))
			{
				foreach (SkillObject skillObject in Skills.All)
				{
					if (string.Equals(skillObject.Name.ToString(), CampaignCheats.ConcatenateString(strings.GetRange(1, strings.Count - 1)), StringComparison.OrdinalIgnoreCase))
					{
						int num = 1;
						if (!int.TryParse(strings[0], out num))
						{
							return "Please enter a number";
						}
						if (num <= 0 || num > 300)
						{
							return string.Format("Level must be between 0 - {0}.", 300);
						}
						Hero.MainHero.HeroDeveloper.SetInitialSkillLevel(skillObject, num);
						Hero.MainHero.HeroDeveloper.InitializeSkillXp(skillObject);
						return "Success";
					}
				}
				return "Skill is not found.";
			}
			return "Format is \"campaign.set_skill_main_hero [LevelValue] [SkillName]\".";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("set_all_skills_main_hero", "campaign")]
		public static string SetAllSkillsMainHero(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			int num = 1;
			if (strings.IsEmpty<string>() || !int.TryParse(strings[0], out num))
			{
				return "Please enter a number";
			}
			if (num <= 0 || num > 300)
			{
				return string.Format("Level must be between 0 - {0}.", 300);
			}
			if (CampaignCheats.CheckParameters(strings, 1) && !CampaignCheats.CheckHelp(strings))
			{
				foreach (SkillObject skillObject in Skills.All)
				{
					Hero.MainHero.HeroDeveloper.SetInitialSkillLevel(skillObject, num);
					Hero.MainHero.HeroDeveloper.InitializeSkillXp(skillObject);
				}
				return "Success";
			}
			return "Format is \"campaign.set_all_skills_main_hero [LevelValue]\".";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("set_skill_companion", "campaign")]
		public static string SetSkillCompanion(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (!CampaignCheats.CheckParameters(strings, 1) && !CampaignCheats.CheckParameters(strings, 0) && !CampaignCheats.CheckHelp(strings))
			{
				foreach (SkillObject skillObject in Skills.All)
				{
					if (string.Equals(skillObject.Name.ToString(), CampaignCheats.ConcatenateString(strings.GetRange(1, strings.Count - 1)), StringComparison.OrdinalIgnoreCase))
					{
						int num = 1;
						if (!int.TryParse(strings[0], out num))
						{
							return "Please enter a number";
						}
						if (num <= 0 || num > 300)
						{
							return string.Format("Level must be between 0 - {0}.", 300);
						}
						foreach (Hero hero in Clan.PlayerClan.Companions)
						{
							hero.HeroDeveloper.SetInitialSkillLevel(skillObject, num);
							hero.HeroDeveloper.InitializeSkillXp(skillObject);
						}
						return "Success";
					}
				}
				return "Update failed.";
			}
			return "Format is \"campaign.set_skill_companion [LevelValue] [SkillName]\".";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("set_all_companion_skills", "campaign")]
		public static string FullCompanion(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			foreach (SkillObject skillObject in Skills.All)
			{
				int num = 1;
				if (strings.Count == 0 || !int.TryParse(strings[0], out num))
				{
					return "Please enter a number";
				}
				if (num <= 0 || num > 300)
				{
					return string.Format("Level must be between 0 - {0}.", 300);
				}
				foreach (Hero hero in Clan.PlayerClan.Companions)
				{
					hero.HeroDeveloper.SetInitialSkillLevel(skillObject, num);
					hero.HeroDeveloper.InitializeSkillXp(skillObject);
				}
			}
			return "Success";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("set_all_heroes_skills", "campaign")]
		public static string SetAllHeroSkills(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			int num;
			if (strings.Count == 0 || !int.TryParse(strings[0], out num))
			{
				return "Please enter a number";
			}
			foreach (Hero hero in Hero.AllAliveHeroes.Where((Hero x) => x.IsActive && x.PartyBelongedTo != null).ToList<Hero>())
			{
				foreach (SkillObject skillObject in Skills.All)
				{
					if (num <= 0 || num > 300)
					{
						return string.Format("Level must be between 0 - {0}.", 300);
					}
					hero.HeroDeveloper.SetInitialSkillLevel(skillObject, num);
					hero.HeroDeveloper.InitializeSkillXp(skillObject);
				}
			}
			return "Success";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("find_tournament", "campaign")]
		public static string FindTournament(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.find_tournament\".";
			}
			Settlement settlement = Settlement.FindFirst((Settlement x) => x.IsTown && Campaign.Current.TournamentManager.GetTournamentGame(x.Town) != null);
			if (settlement == null)
			{
				return "There isn't any tournament right now.";
			}
			settlement.Party.SetAsCameraFollowParty();
			return "Success";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("get_hostile_army", "campaign")]
		public static string GetHostileArmy(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.get_hostile_army\".";
			}
			Army army = null;
			foreach (Kingdom kingdom in Kingdom.All)
			{
				if (kingdom != Clan.PlayerClan.MapFaction && !kingdom.Armies.IsEmpty<Army>() && kingdom.IsAtWarWith(Clan.PlayerClan.MapFaction))
				{
					army = kingdom.Armies.GetRandomElement<Army>();
				}
				if (army != null)
				{
					break;
				}
			}
			if (army == null)
			{
				return "There isn't any hostile army right now.";
			}
			army.LeaderParty.Party.SetAsCameraFollowParty();
			return "Success";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("set_loyalty_of_settlement", "campaign")]
		public static string SetLoyaltyOfSettlement(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckParameters(strings, 1) || CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.set_loyalty_of_settlement [SettlementName] [loyalty]\".";
			}
			int num = 0;
			if (!int.TryParse(strings[strings.Count - 1], out num))
			{
				return "Please enter a number";
			}
			if (num > 100 || num < 0)
			{
				return "Loyalty has to be in the range of 0 to 100";
			}
			strings.RemoveAt(strings.Count - 1);
			string settlementName = string.Join(" ", strings);
			Settlement settlement = Settlement.FindFirst((Settlement x) => string.Compare(x.Name.ToString(), settlementName, StringComparison.OrdinalIgnoreCase) == 0);
			if (settlement == null)
			{
				return "Settlement is not found: " + settlementName;
			}
			if (settlement.IsVillage)
			{
				return "Settlement must be castle or town";
			}
			settlement.Town.Loyalty = (float)num;
			return "Success";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("get_main_party_position", "campaign")]
		public static string GetMainPartyPosition(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (!CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.get_main_party_position\".";
			}
			return MobileParty.MainParty.Position2D.x + " " + MobileParty.MainParty.Position2D.y;
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("start_world_war", "campaign")]
		public static string StartWorldWar(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (!CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.start_world_war\".";
			}
			foreach (Kingdom kingdom in Kingdom.All)
			{
				foreach (Kingdom kingdom2 in Kingdom.All)
				{
					if (kingdom != kingdom2 && !FactionManager.IsAtWarAgainstFaction(kingdom, kingdom2))
					{
						DeclareWarAction.ApplyByDefault(kingdom, kingdom2);
					}
				}
			}
			return "All kingdoms are at war with each other!";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("start_world_peace", "campaign")]
		public static string StartWorldPeace(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (!CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.start_world_peace\".";
			}
			foreach (Kingdom kingdom in Kingdom.All)
			{
				foreach (Kingdom kingdom2 in Kingdom.All)
				{
					if (kingdom != kingdom2 && FactionManager.IsAtWarAgainstFaction(kingdom, kingdom2))
					{
						MakePeaceAction.Apply(kingdom, kingdom2, 0);
					}
				}
			}
			return "All kingdoms are at peace with each other!";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("find_mobile_party", "campaign")]
		public static string FindMobileParty(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.find_mobile_party [PartyName]\".";
			}
			string text = CampaignCheats.ConcatenateString(strings);
			foreach (MobileParty mobileParty in MobileParty.All)
			{
				if (string.Equals(text, mobileParty.Name.ToString(), StringComparison.OrdinalIgnoreCase))
				{
					mobileParty.Party.SetAsCameraFollowParty();
					return "Success";
				}
			}
			return "Party is not found: " + text;
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("add_modified_item", "campaign")]
		public static string AddModifiedItem(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (!CampaignCheats.CheckParameters(strings, 2) || CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.add_modified_item [ItemId] [ModifierId]\".";
			}
			string text = strings[0];
			string text2 = strings[1];
			ItemObject @object = Campaign.Current.ObjectManager.GetObject<ItemObject>(text);
			if (@object == null)
			{
				return "Cant find the item";
			}
			ItemModifier object2 = Campaign.Current.ObjectManager.GetObject<ItemModifier>(text2);
			if (object2 != null)
			{
				EquipmentElement equipmentElement = new EquipmentElement(@object, object2, null, false);
				MobileParty.MainParty.ItemRoster.AddToCounts(equipmentElement, 5);
				return "Success";
			}
			return "Cant find the modifier";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("change_player_name", "campaign")]
		public static string ChangePlayerName(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.find_hero [HeroName]\".";
			}
			string text = CampaignCheats.ConcatenateString(strings);
			TextObject textObject = GameTexts.FindText("str_generic_character_firstname", null);
			textObject.SetTextVariable("CHARACTER_FIRSTNAME", new TextObject(text, null));
			TextObject textObject2 = GameTexts.FindText("str_generic_character_name", null);
			textObject2.SetTextVariable("CHARACTER_NAME", new TextObject(text, null));
			Hero.MainHero.SetName(textObject2, textObject);
			return "Success";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("find_hero", "campaign")]
		public static string FindHero(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.find_hero [HeroName]\".";
			}
			string text = CampaignCheats.ConcatenateString(strings);
			Hero hero = CampaignCheats.GetHero(text);
			if (hero == null)
			{
				return "Hero is not found: " + text;
			}
			if (hero.CurrentSettlement != null)
			{
				hero.CurrentSettlement.Party.SetAsCameraFollowParty();
				return "Success";
			}
			if (hero.PartyBelongedTo != null)
			{
				hero.PartyBelongedTo.Party.SetAsCameraFollowParty();
				return "Success";
			}
			if (hero.PartyBelongedToAsPrisoner != null)
			{
				hero.PartyBelongedToAsPrisoner.SetAsCameraFollowParty();
				return "Success";
			}
			return "Party is not found: " + text;
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("add_crafting_materials", "campaign")]
		public static string AddCraftingMaterials(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (!CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.add_crafting_materials\".";
			}
			for (int i = 0; i < 9; i++)
			{
				PartyBase.MainParty.ItemRoster.AddToCounts(Campaign.Current.Models.SmithingModel.GetCraftingMaterialItem((CraftingMaterials)i), 100);
			}
			return "100 pieces for each crafting material is added to the player inventory.";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("change_hero_relation", "campaign")]
		public static string ChangeHeroRelation(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckHelp(strings) || CampaignCheats.CheckParameters(strings, 1))
			{
				return "Format is \"campaign.change_relation [Value] [HeroName]\". \n \"campaign.change_relation [Value] [All] to change relation with everyone.";
			}
			int num;
			if (!int.TryParse(strings[0], out num))
			{
				return "Please enter a number";
			}
			strings.RemoveAt(0);
			string text = CampaignCheats.ConcatenateString(strings);
			Hero hero = CampaignCheats.GetHero(text);
			if (hero != null)
			{
				ChangeRelationAction.ApplyPlayerRelation(hero, num, true, true);
				return "Success";
			}
			if (string.Equals(text, "all", StringComparison.OrdinalIgnoreCase))
			{
				foreach (Hero hero2 in Hero.AllAliveHeroes)
				{
					if (!hero2.IsHumanPlayerCharacter)
					{
						ChangeRelationAction.ApplyPlayerRelation(hero2, num, true, true);
					}
				}
				return "Success";
			}
			return "Hero is not found";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("heal_main_party", "campaign")]
		public static string HealMainParty(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (!CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.heal_main_party\".";
			}
			if (MobileParty.MainParty.MapEvent == null)
			{
				for (int i = 0; i < PartyBase.MainParty.MemberRoster.Count; i++)
				{
					TroopRosterElement elementCopyAtIndex = PartyBase.MainParty.MemberRoster.GetElementCopyAtIndex(i);
					if (elementCopyAtIndex.Character.IsHero)
					{
						elementCopyAtIndex.Character.HeroObject.Heal(elementCopyAtIndex.Character.HeroObject.MaxHitPoints, false);
					}
					else
					{
						MobileParty.MainParty.Party.AddToMemberRosterElementAtIndex(i, 0, -PartyBase.MainParty.MemberRoster.GetElementWoundedNumber(i));
					}
				}
				return "Success";
			}
			return "Main party shouldn't be in a map event.";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("declare_war", "campaign")]
		public static string DeclareWar(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (!CampaignCheats.CheckParameters(strings, 2) || CampaignCheats.CheckHelp(strings))
			{
				return "Format is faction names without space \"campaign.declare_war [Faction1] [Faction2]\".";
			}
			string text = strings[0].ToLower();
			string text2 = strings[1].ToLower();
			IFaction faction = null;
			IFaction faction2 = null;
			foreach (IFaction faction3 in Campaign.Current.Factions)
			{
				string text3 = faction3.Name.ToString().ToLower().Replace(" ", "");
				if (text3 == text)
				{
					faction = faction3;
				}
				else if (text3 == text2)
				{
					faction2 = faction3;
				}
			}
			if (faction != null && faction2 != null)
			{
				DeclareWarAction.ApplyByDefault(faction, faction2);
				return string.Concat(new object[] { "War declared between ", faction.Name, " and ", faction2.Name });
			}
			if (faction == null)
			{
				return "Faction is not found: " + text;
			}
			return "Faction is not found: " + text2;
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("declare_peace", "campaign")]
		public static string DeclarePeace(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (!CampaignCheats.CheckParameters(strings, 2) || CampaignCheats.CheckHelp(strings))
			{
				return "Format is faction names without space  \"campaign.declare_peace [Faction1] [Faction2]";
			}
			string text = strings[0].ToLower();
			string text2 = strings[1].ToLower();
			IFaction faction = null;
			IFaction faction2 = null;
			foreach (IFaction faction3 in Campaign.Current.Factions)
			{
				string text3 = faction3.Name.ToString().ToLower().Replace(" ", "");
				if (text3 == text)
				{
					faction = faction3;
				}
				else if (text3 == text2)
				{
					faction2 = faction3;
				}
			}
			if (faction != null && faction2 != null)
			{
				if (faction.GetStanceWith(faction2).IsAtConstantWar)
				{
					return "Can't declare peace between factions that are at constant war";
				}
				MakePeaceAction.Apply(faction, faction2, 0);
				return string.Concat(new object[] { "Peace declared between ", faction.Name, " and ", faction2.Name });
			}
			else
			{
				if (faction == null)
				{
					return "Faction is not found: " + faction;
				}
				return "Faction is not found: " + faction2;
			}
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("give_item_to_main_party", "campaign")]
		public static string GiveItemToMainParty(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckHelp(strings) || strings.Count > 2)
			{
				return "Format is \"campaign.give_item_to_main_party [ItemObject] [Amount]\"\n If amount is not entered only 1 item will be given.";
			}
			ItemObject @object = MBObjectManager.Instance.GetObject<ItemObject>(strings[0]);
			if (@object == null)
			{
				return "Item is not found";
			}
			int num = 1;
			if (strings.Count == 2 && (!int.TryParse(strings[1], out num) || num < 1))
			{
				return "Please enter a positive number";
			}
			PartyBase.MainParty.ItemRoster.AddToCounts(@object, num);
			return @object.Name + " has been given to the main party.";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("give_all_crafting_materials_to_main_party", "campaign")]
		public static string GiveCraftingMaterialItemsToMainParty(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (strings.Count > 1 || CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.give_all_crafting_materials_to_main_party [Amount]\n If amount is not entered only 1 item per material will be given.\".";
			}
			int num = 1;
			if (strings.Count == 1 && (!int.TryParse(strings[0], out num) || num < 1))
			{
				return "Please enter a positive number";
			}
			for (CraftingMaterials craftingMaterials = CraftingMaterials.IronOre; craftingMaterials < CraftingMaterials.NumCraftingMats; craftingMaterials++)
			{
				ItemObject craftingMaterialItem = Campaign.Current.Models.SmithingModel.GetCraftingMaterialItem(craftingMaterials);
				PartyBase.MainParty.ItemRoster.AddToCounts(craftingMaterialItem, num);
			}
			return "Crafting materials have been given to the main party.";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("kill_capturer_party", "campaign")]
		public static string KillCapturerParty(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (!CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.kill_capturer_party\".";
			}
			if (!PlayerCaptivity.IsCaptive)
			{
				return "Player is not captive";
			}
			if (PlayerCaptivity.CaptorParty.IsSettlement)
			{
				return "Can't destroy settlement";
			}
			GameMenu.SwitchToMenu("menu_captivity_end_by_party_removed");
			DestroyPartyAction.Apply(null, PlayerCaptivity.CaptorParty.MobileParty);
			return "Success";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("add_influence", "campaign")]
		public static string AddInfluence(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.add_influence [Number]\". If Number is not entered, 100 influence will be added.";
			}
			int num = 100;
			bool flag = false;
			if (!CampaignCheats.CheckParameters(strings, 0))
			{
				flag = int.TryParse(strings[0], out num);
			}
			if (flag || CampaignCheats.CheckParameters(strings, 0))
			{
				float num2 = MBMath.ClampFloat(Hero.MainHero.Clan.Influence + (float)num, 0f, float.MaxValue);
				float num3 = num2 - Hero.MainHero.Clan.Influence;
				ChangeClanInfluenceAction.Apply(Clan.PlayerClan, num2);
				return string.Format("The influence of player is changed by {0}.", num3);
			}
			return "Please enter a positive number\nFormat is \"campaign.add_influence [Number]\".";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("add_renown_to_clan", "campaign")]
		public static string AddRenown(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.add_renown [PositiveNumber] [ClanName]\". \n If number is not specified, 100 will be added. \n If clan name is not specified, player clan will get the renown.";
			}
			int num = 100;
			string text = "";
			Hero hero = Hero.MainHero;
			bool flag = false;
			if (CampaignCheats.CheckParameters(strings, 1))
			{
				if (!int.TryParse(strings[0], out num))
				{
					num = 100;
					text = strings[0];
					hero = CampaignCheats.GetClanLeader(text);
					flag = true;
				}
			}
			else if (!CampaignCheats.CheckParameters(strings, 0))
			{
				if (!int.TryParse(strings[0], out num))
				{
					num = 100;
					text = CampaignCheats.ConcatenateString(strings.GetRange(0, strings.Count));
					hero = CampaignCheats.GetClanLeader(text);
				}
				else
				{
					text = CampaignCheats.ConcatenateString(strings.GetRange(1, strings.Count - 1));
					hero = CampaignCheats.GetClanLeader(text);
				}
				flag = true;
			}
			if (hero != null)
			{
				if (num > 0)
				{
					GainRenownAction.Apply(hero, (float)num, false);
					return string.Format("Added {0} renown to ", num) + hero.Clan.Name;
				}
				return "Please enter a positive number\nFormat is \"campaign.add_renown [PositiveNumber] [ClanName]\".";
			}
			else
			{
				if (flag)
				{
					return "Clan: " + text + " not found.\nFormat is \"campaign.add_renown [PositiveNumber] [ClanName]\".";
				}
				return "Wrong Input.\nFormat is \"campaign.add_renown [PositiveNumber] [ClanName]\".";
			}
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("add_gold_to_hero", "campaign")]
		public static string AddGoldToHero(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.add_gold_to_hero [PositiveNumber] [HeroName]\".\n If number is not specified, 100 will be added. \n If hero name is not specified, player will get the gold.";
			}
			int num = 1000;
			Hero hero = Hero.MainHero;
			string text = "";
			bool flag = false;
			if (CampaignCheats.CheckParameters(strings, 1))
			{
				if (!int.TryParse(strings[0], out num))
				{
					num = 1000;
					text = strings[0];
					hero = CampaignCheats.GetHero(text);
					flag = true;
				}
			}
			else if (!CampaignCheats.CheckParameters(strings, 0))
			{
				if (!int.TryParse(strings[0], out num))
				{
					num = 1000;
					text = CampaignCheats.ConcatenateString(strings.GetRange(0, strings.Count));
					hero = CampaignCheats.GetHero(text);
				}
				else
				{
					text = CampaignCheats.ConcatenateString(strings.GetRange(1, strings.Count - 1));
					hero = CampaignCheats.GetHero(text);
				}
				flag = true;
			}
			if (hero != null)
			{
				if (num < 1)
				{
					return "Please enter a positive number";
				}
				GiveGoldAction.ApplyBetweenCharacters(null, hero, num, true);
				return string.Format("{0}'s denars changed by {1}.", hero.Name, num);
			}
			else
			{
				if (flag)
				{
					return "Hero: " + text + " not found.\nFormat is \"campaign.add_gold_to_hero [Number] [HeroName]\".";
				}
				return "Wrong input.\nFormat is \"campaign.add_gold_to_hero [Number] [HeroName]\".";
			}
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("add_gold_to_all_heroes", "campaign")]
		public static string AddGoldToAllHeroes(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.add_gold_to_all_heroes [PositiveNumber]\".\n If number is not specified, 100 will be added.";
			}
			int num = 1000;
			bool flag = false;
			if (!CampaignCheats.CheckParameters(strings, 0))
			{
				flag = int.TryParse(strings[0], out num);
			}
			if (!flag && !CampaignCheats.CheckParameters(strings, 0))
			{
				return "Wrong input.\nFormat is \"campaign.add_gold_to_all_heroes [Number]\".";
			}
			if (num < 1)
			{
				return "Please enter a positive number";
			}
			foreach (Hero hero in Hero.AllAliveHeroes)
			{
				if (hero != null)
				{
					GiveGoldAction.ApplyBetweenCharacters(null, hero, num, true);
				}
			}
			return string.Format("All party's denars changed by {0}.", num);
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("activate_all_policies_for_player_kingdom", "campaign")]
		public static string ActivateAllPolicies(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (!CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.activate_all_policies_for_player_kingdom";
			}
			if (Clan.PlayerClan.Kingdom != null)
			{
				Kingdom kingdom = Clan.PlayerClan.Kingdom;
				foreach (PolicyObject policyObject in PolicyObject.All)
				{
					if (!kingdom.ActivePolicies.Contains(policyObject))
					{
						kingdom.AddPolicy(policyObject);
					}
				}
				return "All policies are now active for player kingdom";
			}
			return "Player is not in a kingdom";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("add_building_level", "campaign")]
		public static string AddDevelopment(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			string text = "Format is \"campaign.add_building_level [SettlementName] |  [Building]\".";
			if (CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckParameters(strings, 1) || CampaignCheats.CheckHelp(strings))
			{
				return text;
			}
			List<string> separatedNames = CampaignCheats.GetSeparatedNames(strings, "|");
			if (separatedNames.Count != 2)
			{
				return text;
			}
			Settlement settlement = CampaignCheats.GetSettlement(separatedNames[0]);
			if (settlement != null && settlement.IsFortification)
			{
				BuildingType buildingType = null;
				foreach (BuildingType buildingType2 in BuildingType.All)
				{
					if (buildingType2.Name.ToString().ToLower().Trim()
						.Equals(separatedNames[1].ToString().ToLower().Trim()))
					{
						if (buildingType2.BuildingLocation == BuildingLocation.Castle && settlement.IsCastle)
						{
							buildingType = buildingType2;
							break;
						}
						if (settlement.IsTown && (buildingType2.BuildingLocation == BuildingLocation.Settlement || buildingType2.BuildingLocation == BuildingLocation.Daily))
						{
							buildingType = buildingType2;
							break;
						}
					}
				}
				if (buildingType == null)
				{
					return "Development could not be found";
				}
				foreach (Building building in settlement.Town.Buildings)
				{
					if (building.BuildingType == buildingType)
					{
						if (building.CurrentLevel < 3)
						{
							Building building2 = building;
							int currentLevel = building2.CurrentLevel;
							building2.CurrentLevel = currentLevel + 1;
							return string.Concat(new object[] { buildingType.Name, " level increased to ", building.CurrentLevel, " at ", settlement.Name });
						}
						return buildingType.Name + " is already at top level!";
					}
				}
			}
			return "Settlement is not found";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("add_progress_to_current_building", "campaign")]
		public static string AddDevelopmentProgress(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckParameters(strings, 1) || CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.add_progress_to_current_building [SettlementName] [Progress (0-100)]\".";
			}
			int num;
			if (!int.TryParse(strings[1], out num))
			{
				return "Please enter a number";
			}
			if (num > 100 || num < 0)
			{
				return "Progress must be between 0 and 100.";
			}
			Settlement settlement = CampaignCheats.GetSettlement(strings[0]);
			if (settlement != null && settlement.IsFortification)
			{
				Building currentBuilding = settlement.Town.CurrentBuilding;
				if (currentBuilding != null)
				{
					if (!currentBuilding.BuildingType.IsDefaultProject)
					{
						settlement.Town.BuildingsInProgress.Peek().BuildingProgress += ((float)currentBuilding.GetConstructionCost() - currentBuilding.BuildingProgress) * (float)num / 100f;
						return string.Concat(new object[]
						{
							"Development progress increased to ",
							(int)(settlement.Town.BuildingsInProgress.Peek().BuildingProgress * 100f),
							" at ",
							settlement.Name
						});
					}
					return "Currently there are no buildings in queue.";
				}
			}
			return "Settlement is not found";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("change_current_building", "campaign")]
		public static string ChangeCurrentDevelopment(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			string text = "Format is \"campaign.change_current_building [SettlementName] | [BuildingTypeName]\".";
			if (CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckParameters(strings, 1) || CampaignCheats.CheckHelp(strings))
			{
				return text;
			}
			List<string> separatedNames = CampaignCheats.GetSeparatedNames(strings, "|");
			if (separatedNames.Count != 2)
			{
				return text;
			}
			Settlement settlement = CampaignCheats.GetSettlement(separatedNames[0]);
			if (settlement != null && settlement.IsFortification)
			{
				BuildingType buildingType = null;
				bool flag = true;
				foreach (BuildingType buildingType2 in BuildingType.All)
				{
					if (separatedNames[1].Equals(buildingType2.Name.ToString(), StringComparison.OrdinalIgnoreCase))
					{
						buildingType = buildingType2;
						flag = false;
						break;
					}
				}
				if (!flag)
				{
					foreach (Building building in settlement.Town.Buildings)
					{
						if (building.BuildingType == buildingType && building.CurrentLevel < 3)
						{
							BuildingHelper.ChangeCurrentBuilding(buildingType, settlement.Town);
							return string.Concat(new object[]
							{
								"Current building changed to ",
								building.BuildingType.Name,
								" at ",
								settlement.Name
							});
						}
					}
				}
				return "Building type is not found";
			}
			return "Settlement is not found";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("add_skill_xp_to_hero", "campaign")]
		public static string AddSkillXpToHero(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			bool flag = false;
			Hero hero = Hero.MainHero;
			int num = 100;
			if (CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.add_skill_xp_to_hero [SkillName] [PositiveNumber] [HeroName]\".";
			}
			if (!CampaignCheats.CheckParameters(strings, 0))
			{
				if (!CampaignCheats.CheckParameters(strings, 1))
				{
					if (!CampaignCheats.CheckParameters(strings, 2))
					{
						hero = CampaignCheats.GetHero(CampaignCheats.ConcatenateString(strings.GetRange(2, strings.Count - 2)));
					}
					flag = int.TryParse(strings[1], out num);
				}
				if (hero != null)
				{
					if (flag)
					{
						if (num <= 0)
						{
							return "Please enter a positive number\nFormat is \"campaign.add_skill_xp_to_hero [SkillName] [PositiveNumber] [HeroName]\".";
						}
						string text = strings[0];
						foreach (SkillObject skillObject in Skills.All)
						{
							if (skillObject.Name.ToString().Equals(text, StringComparison.InvariantCultureIgnoreCase) || skillObject.StringId == text)
							{
								if (hero.GetSkillValue(skillObject) < 300)
								{
									hero.HeroDeveloper.AddSkillXp(skillObject, (float)num, true, true);
									int num2 = (int)(hero.HeroDeveloper.GetFocusFactor(skillObject) * (float)num);
									return string.Format("Input {0} xp is modified to {1} xp due to focus point factor \nand added to the {2}'s {3} skill. ", new object[] { num, num2, hero.Name, skillObject.Name });
								}
								return string.Format("{0} value for {1} is already at max.. ", skillObject, hero);
							}
						}
						return "Skill not found.\nFormat is \"campaign.add_skill_xp_to_hero [SkillName] [PositiveNumber] [HeroName]\".";
					}
					else
					{
						if (!CampaignCheats.CheckParameters(strings, 1))
						{
							goto IL_31E;
						}
						if (num <= 0)
						{
							return "Please enter a positive number\nFormat is \"campaign.add_skill_xp_to_hero [SkillName] [PositiveNumber] [HeroName]\".";
						}
						string text2 = strings[0];
						foreach (SkillObject skillObject2 in Skills.All)
						{
							if (skillObject2.Name.ToString().Equals(text2, StringComparison.InvariantCultureIgnoreCase) || skillObject2.StringId == text2)
							{
								if (hero.GetSkillValue(skillObject2) < 300)
								{
									hero.HeroDeveloper.AddSkillXp(skillObject2, (float)num, true, true);
									int num3 = (int)(hero.HeroDeveloper.GetFocusFactor(skillObject2) * (float)num);
									return string.Format("Input {0} xp is modified to {1} xp due to focus point factor \nand added to the {2}'s {3} skill. ", new object[] { num, num3, hero.Name, skillObject2.Name });
								}
								return string.Format("{0} value for {1} is already at max.. ", skillObject2, hero);
							}
						}
						return "Skill not found.\nFormat is \"campaign.add_skill_xp_to_hero [SkillName] [PositiveNumber] [HeroName]\".";
					}
					string text3;
					return text3;
				}
				IL_31E:
				return "Hero is not found.\nFormat is \"campaign.add_skill_xp_to_hero [SkillName] [PositiveNumber] [HeroName]\".";
			}
			if (hero != null)
			{
				string text4 = "";
				foreach (SkillObject skillObject3 in Skills.All)
				{
					hero.HeroDeveloper.AddSkillXp(skillObject3, (float)num, true, true);
					int num4 = (int)(hero.HeroDeveloper.GetFocusFactor(skillObject3) * (float)num);
					text4 += string.Format("{0} xp is modified to {1} xp due to focus point factor \nand added to the {2}'s {3} skill.\n", new object[] { num, num4, hero.Name, skillObject3.Name });
				}
				return text4;
			}
			return "Wrong Input.\nFormat is \"campaign.add_skill_xp_to_hero [SkillName] [PositiveNumber] [HeroName]\".";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("show_prisoners", "campaign")]
		public static string ShowPrisoners(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (!CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.show_prisoners\".";
			}
			string text = "";
			foreach (Hero hero in Hero.AllAliveHeroes)
			{
				if (hero.IsPrisoner)
				{
					text = string.Concat(new object[]
					{
						text,
						hero.Name,
						"    (captor: ",
						hero.PartyBelongedToAsPrisoner.Name,
						")\n"
					});
				}
			}
			return text + "\nSuccess";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("add_companions", "campaign")]
		public static string AddCompanions(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (!CampaignCheats.CheckParameters(strings, 1) || CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.add_companions [number]\".";
			}
			int num;
			if (!int.TryParse(strings[0], out num))
			{
				return "Invalid number";
			}
			for (int i = 0; i < num; i++)
			{
				CampaignCheats.AddCompanion(new List<string>());
			}
			return "Success";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("add_companion", "campaign")]
		public static string AddCompanion(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (!CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.add_companion\".";
			}
			CharacterObject wanderer = CharacterObject.PlayerCharacter.Culture.NotableAndWandererTemplates.GetRandomElementWithPredicate((CharacterObject x) => x.Occupation == Occupation.Wanderer);
			Settlement randomElementWithPredicate = Settlement.All.GetRandomElementWithPredicate((Settlement settlement) => settlement.Culture == wanderer.Culture && settlement.IsTown);
			Hero hero = HeroCreator.CreateSpecialHero(wanderer, randomElementWithPredicate, null, null, -1);
			GiveGoldAction.ApplyBetweenCharacters(null, hero, 20000, true);
			hero.SetHasMet();
			hero.ChangeState(Hero.CharacterStates.Active);
			hero.HeroDeveloper.ClearHero();
			Campaign.Current.Models.CharacterDevelopmentModel.GetXpRequiredForSkillLevel(hero.Level);
			int num = MathF.Min(hero.Level / 25 + 1, 10);
			foreach (SkillObject skillObject in Skills.All)
			{
				hero.HeroDeveloper.AddFocus(skillObject, num, false);
				hero.HeroDeveloper.SetInitialSkillLevel(skillObject, hero.Level);
			}
			hero.HeroDeveloper.UnspentFocusPoints = 0;
			AddCompanionAction.Apply(Clan.PlayerClan, hero);
			AddHeroToPartyAction.Apply(hero, MobileParty.MainParty, true);
			return "companion has been added.";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("set_player_reputation_trait", "campaign")]
		public static string SetPlayerReputationTrait(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckParameters(strings, 1) || CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.set_player_reputation_trait [Trait] [Number]\".";
			}
			int num;
			if (int.TryParse(strings[1], out num))
			{
				string text = strings[0];
				foreach (TraitObject traitObject in TraitObject.All)
				{
					if (traitObject.Name.ToString().Equals(text, StringComparison.InvariantCultureIgnoreCase) || traitObject.StringId == text)
					{
						if (num >= traitObject.MinValue && num <= traitObject.MaxValue)
						{
							Hero.MainHero.SetTraitLevel(traitObject, num);
							Campaign.Current.PlayerTraitDeveloper.UpdateTraitXPAccordingToTraitLevels();
							return string.Format("Set {0} to {1}.", traitObject.Name, num);
						}
						return string.Format("Number must be between {0} and {1}.", traitObject.MinValue, traitObject.MaxValue);
					}
				}
				return "Trait not found";
			}
			return "Please enter a number";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("list_player_traits", "campaign")]
		public static string ListPlayerTrait(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.list_player_traits\".";
			}
			string text = "";
			foreach (TraitObject traitObject in TraitObject.All)
			{
				text = string.Concat(new object[]
				{
					text,
					traitObject.Name.ToString(),
					" Trait Level:  ",
					Hero.MainHero.GetTraitLevel(traitObject),
					" Trait Xp: ",
					Campaign.Current.PlayerTraitDeveloper.GetPropertyValue(traitObject),
					"\n"
				});
			}
			return text;
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("add_horse", "campaign")]
		public static string AddHorse(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.add_horse [Number]\".";
			}
			int num = 1;
			if (!int.TryParse(strings[0], out num))
			{
				return "Please enter a number";
			}
			if (num > 0)
			{
				ItemObject itemObject = Items.All.FirstOrDefault((ItemObject x) => x.IsMountable);
				PartyBase.MainParty.ItemRoster.AddToCounts(itemObject, num);
				return string.Format("Added {0} {1} to player inventory.", num, itemObject.Name);
			}
			return "Nothing added.";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("give_settlement_to_player", "campaign")]
		public static string GiveSettlementToPlayer(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckParameters(strings, 0))
			{
				return "Format is \"campaign.give_settlement_to_player [SettlementName/SettlementId]\nWrite \"campaign.give_settlement_to_player help\" to list available settlements.";
			}
			string text = CampaignCheats.ConcatenateString(strings);
			if (text.ToLower() == "help")
			{
				string text2 = "";
				text2 += "\n";
				text2 += "Available settlements";
				text2 += "\n";
				text2 += "==============================";
				text2 += "\n";
				foreach (Settlement settlement in MBObjectManager.Instance.GetObjectTypeList<Settlement>())
				{
					text2 = string.Concat(new object[] { text2, "Id: ", settlement.StringId, " Name: ", settlement.Name, "\n" });
				}
				return text2;
			}
			string text3 = text;
			MBReadOnlyList<Settlement> objectTypeList = MBObjectManager.Instance.GetObjectTypeList<Settlement>();
			if (text.ToLower() == "calradia")
			{
				foreach (Settlement settlement2 in objectTypeList)
				{
					if (settlement2.IsCastle || settlement2.IsTown)
					{
						ChangeOwnerOfSettlementAction.ApplyByDefault(Hero.MainHero, settlement2);
					}
				}
				return "You own all of Calradia now!";
			}
			Settlement settlement3 = MBObjectManager.Instance.GetObject<Settlement>(text3);
			if (settlement3 == null)
			{
				foreach (Settlement settlement4 in objectTypeList)
				{
					if (settlement4.Name.ToString().Equals(text3, StringComparison.InvariantCultureIgnoreCase))
					{
						settlement3 = settlement4;
						break;
					}
				}
			}
			if (settlement3 == null)
			{
				return "Given settlement name or id could not be found.";
			}
			if (settlement3.IsVillage)
			{
				return "Settlement must be castle or town";
			}
			ChangeOwnerOfSettlementAction.ApplyByDefault(Hero.MainHero, settlement3);
			return settlement3.Name + " has been given to the player.";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("give_settlement_to_kingdom", "campaign")]
		public static string GiveSettlementToKingdom(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckParameters(strings, 1))
			{
				return "Format is \"campaign.give_settlement_to_kingdom [SettlementName] | [KingdomName]";
			}
			string[] array = CampaignCheats.ConcatenateString(strings).Split(new char[] { '|' });
			if (array.Length != 2)
			{
				return "Format is \"campaign.give_settlement_to_kingdom [SettlementName] | [KingdomName]";
			}
			Settlement settlement = CampaignCheats.GetSettlement(array[0].Trim());
			if (settlement == null)
			{
				return "Given settlement name could not be found.";
			}
			if (settlement.IsVillage)
			{
				settlement = settlement.Village.Bound;
			}
			Kingdom kingdom = CampaignCheats.GetKingdom(array[1].Trim());
			if (kingdom == null)
			{
				return "Given kingdom could not be found.";
			}
			if (settlement.MapFaction == kingdom)
			{
				return "Kingdom already owns the settlement.";
			}
			if (settlement.IsVillage)
			{
				return "Settlement must be castle or town";
			}
			ChangeOwnerOfSettlementAction.ApplyByDefault(kingdom.Leader, settlement);
			return settlement.Name + string.Format(" has been given to {0}.", kingdom.Leader.Name);
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("add_power_to_notable", "campaign")]
		public static string AddPowerToNotable(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckParameters(strings, 1))
			{
				return "Format is \"campaign.add_power_to_notable [HeroName] [Number]";
			}
			int num;
			if (!int.TryParse(strings[strings.Count - 1], out num))
			{
				return "Please enter a valid number";
			}
			strings.RemoveAt(strings.Count - 1);
			Hero hero = CampaignCheats.GetHero(CampaignCheats.ConcatenateString(strings));
			if (hero == null)
			{
				return "Hero is not found";
			}
			if (!hero.IsNotable)
			{
				return "Hero is not a notable";
			}
			hero.AddPower((float)num);
			return string.Format("{0} power is {1}", hero.Name, hero.Power);
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("kill_hero", "campaign")]
		public static string KillHero(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.kill_hero [HeroName]\".";
			}
			string text = CampaignCheats.ConcatenateString(strings);
			Hero hero = CampaignCheats.GetHero(text);
			if (hero == null)
			{
				return "Hero is not found: " + text.ToLower();
			}
			if (!hero.IsAlive)
			{
				return "Hero " + text + " is already dead.";
			}
			if (!hero.CanDie(KillCharacterAction.KillCharacterActionDetail.Murdered))
			{
				return "Hero cant die!";
			}
			if (hero == Hero.MainHero)
			{
				return "Hero " + text + " is main hero. Use [campaingn.make_main_hero_ill] to kill main hero.";
			}
			KillCharacterAction.ApplyByMurder(hero, null, true);
			return "Hero " + text.ToLower() + " is killed.";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("show_character_feats", "campaign")]
		public static string ShowCharacterFeats(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.show_character_feats [HeroName]\".";
			}
			string text = CampaignCheats.ConcatenateString(strings);
			Hero hero = CampaignCheats.GetHero(text);
			string text2 = "";
			if (hero != null)
			{
				foreach (FeatObject featObject in FeatObject.All)
				{
					text2 = string.Concat(new object[]
					{
						text2,
						"\n",
						featObject.Name,
						" :",
						hero.Culture.HasFeat(featObject).ToString()
					});
				}
				return text2;
			}
			return "Hero is not found: " + text;
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("make_hero_fugitive", "campaign")]
		public static string MakeHeroFugitive(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.make_hero_fugitive [HeroName]";
			}
			string text = CampaignCheats.ConcatenateString(strings);
			Hero hero = CampaignCheats.GetHero(text);
			if (hero == null)
			{
				return "Hero is not found: " + text.ToLower();
			}
			if (!hero.IsAlive)
			{
				return "Hero " + text + " is dead.";
			}
			if (hero.PartyBelongedTo != null)
			{
				if (hero.PartyBelongedTo == MobileParty.MainParty)
				{
					return "You cannot be fugitive when you are in your main party";
				}
				DestroyPartyAction.Apply(null, hero.PartyBelongedTo);
			}
			MakeHeroFugitiveAction.Apply(hero);
			return "Hero " + text.ToLower() + " is now fugitive.";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("leave_faction", "campaign")]
		public static string LeaveFaction(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.leave_faction\".";
			}
			if (Hero.MainHero.MapFaction == Clan.PlayerClan)
			{
				return "Function execution failed.";
			}
			if (Hero.MainHero.MapFaction.Leader == Hero.MainHero)
			{
				string text;
				if (Hero.MainHero.MapFaction.Name.ToString().ToLower() == "empire")
				{
					text = "lord_1_1";
				}
				else if (Hero.MainHero.MapFaction.Name.ToString().ToLower() == "sturgia")
				{
					text = "lord_2_1";
				}
				else if (Hero.MainHero.MapFaction.Name.ToString().ToLower() == "aserai")
				{
					text = "lord_3_1";
				}
				else if (Hero.MainHero.MapFaction.Name.ToString().ToLower() == "vlandia")
				{
					text = "lord_4_1";
				}
				else if (Hero.MainHero.MapFaction.Name.ToString().ToLower() == "battania")
				{
					text = "lord_5_1";
				}
				else if (Hero.MainHero.MapFaction.Name.ToString().ToLower() == "khuzait")
				{
					text = "lord_6_1";
				}
				else
				{
					text = "lord_1_1";
				}
				Hero heroObject = Game.Current.ObjectManager.GetObject<CharacterObject>(text).HeroObject;
				if (!Hero.MainHero.MapFaction.IsKingdomFaction)
				{
					(Hero.MainHero.MapFaction as Clan).SetLeader(heroObject);
				}
				else
				{
					ChangeRulingClanAction.Apply(Hero.MainHero.MapFaction as Kingdom, heroObject.Clan);
				}
			}
			ChangeKingdomAction.ApplyByLeaveKingdom(Hero.MainHero.Clan, true);
			return "Success";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("lead_your_faction", "campaign")]
		public static string LeadYourFaction(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.lead_your_faction\".";
			}
			if (Hero.MainHero.MapFaction.Leader != Hero.MainHero)
			{
				if (Hero.MainHero.MapFaction.IsKingdomFaction)
				{
					ChangeRulingClanAction.Apply(Hero.MainHero.MapFaction as Kingdom, Clan.PlayerClan);
				}
				else
				{
					(Hero.MainHero.MapFaction as Clan).SetLeader(Hero.MainHero);
				}
				return "Success";
			}
			return "Function execution failed.";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("list_heroes_suitable_for_marriage", "campaign")]
		public static string ListHeroesSuitableForMarriage(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"list_heroes_suitable_for_marriage\".";
			}
			List<Hero> list = new List<Hero>();
			List<Hero> list2 = new List<Hero>();
			foreach (Kingdom kingdom in Kingdom.All)
			{
				foreach (Hero hero in kingdom.Lords)
				{
					if (hero.CanMarry())
					{
						if (hero.IsFemale)
						{
							list.Add(hero);
						}
						else
						{
							list2.Add(hero);
						}
					}
				}
			}
			string text = "Maidens:\n";
			string text2 = "Suitors:\n";
			foreach (Hero hero2 in list)
			{
				TextObject textObject = ((hero2.PartyBelongedTo == null) ? TextObject.Empty : hero2.PartyBelongedTo.Name);
				text = string.Concat(new object[] { text, "Name: ", hero2.Name, " --- Clan: ", hero2.Clan, " --- Party:", textObject, "\n" });
			}
			foreach (Hero hero3 in list2)
			{
				TextObject textObject2 = ((hero3.PartyBelongedTo == null) ? TextObject.Empty : hero3.PartyBelongedTo.Name);
				text2 = string.Concat(new object[] { text2, "Name: ", hero3.Name, " --- Clan: ", hero3.Clan, " --- Party:", textObject2, "\n" });
			}
			return text + "\n" + text2;
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("marry_player_with_hero", "campaign")]
		public static string MarryPlayerWithHero(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.marry_player_with_hero [HeroName]\".";
			}
			string text = CampaignCheats.ConcatenateString(strings);
			Hero hero = CampaignCheats.GetHero(text);
			if (hero == null)
			{
				return "Hero is not found: " + text.ToLower();
			}
			if (Campaign.Current.Models.MarriageModel.IsCoupleSuitableForMarriage(Hero.MainHero, hero))
			{
				MarriageAction.Apply(Hero.MainHero, hero, true);
				return "Success";
			}
			return string.Concat(new object[]
			{
				"Marriage is not suitable between ",
				Hero.MainHero.Name,
				" and ",
				hero.Name,
				"\n"
			});
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("is_hero_suitable_for_marriage_with_player", "campaign")]
		public static string IsHeroSuitableForMarriageWithPlayer(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.is_hero_suitable_for_marriage_with_player [HeroName]\".";
			}
			string text = CampaignCheats.ConcatenateString(strings);
			Hero hero = CampaignCheats.GetHero(text);
			if (hero == null)
			{
				return "Hero is not found: " + text.ToLower();
			}
			if (Campaign.Current.Models.MarriageModel.IsCoupleSuitableForMarriage(Hero.MainHero, hero))
			{
				return string.Concat(new object[]
				{
					"Marriage is suitable between ",
					Hero.MainHero.Name,
					" and ",
					hero.Name,
					"\n"
				});
			}
			return string.Concat(new object[]
			{
				"Marriage is not suitable between ",
				Hero.MainHero.Name,
				" and ",
				hero.Name,
				"\n"
			});
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("start_player_vs_world_war", "campaign")]
		public static string StartPlayerVsWorldWar(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.start_player_vs_world_war\".";
			}
			foreach (IFaction faction in Campaign.Current.Factions)
			{
				if ((faction != Clan.PlayerClan || faction != Hero.MainHero.MapFaction) && faction != CampaignData.NeutralFaction && !faction.IsEliminated && (faction.IsKingdomFaction || faction.IsMinorFaction))
				{
					DeclareWarAction.ApplyByDefault(faction, Clan.PlayerClan);
				}
			}
			return "Success";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("start_player_vs_world_truce", "campaign")]
		public static string StartPlayerVsWorldTruce(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.start_player_vs_world_truce\".";
			}
			foreach (IFaction faction in Campaign.Current.Factions)
			{
				if (faction != Clan.PlayerClan || faction != Hero.MainHero.MapFaction)
				{
					MakePeaceAction.Apply(faction, Clan.PlayerClan, 0);
				}
			}
			return "Success";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("create_player_kingdom", "campaign")]
		public static string CreatePlayerKingdom(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckHelp(strings) || !CampaignCheats.CheckParameters(strings, 0))
			{
				return "Format is \"campaign.create_player_kingdom\".";
			}
			Campaign.Current.KingdomManager.CreateKingdom(Clan.PlayerClan.Name, Clan.PlayerClan.InformalName, Clan.PlayerClan.Culture, Clan.PlayerClan, null, null, null, null);
			return "success";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("create_random_clan", "campaign")]
		public static string CreateRandomClan(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckHelp(strings) || CampaignCheats.CheckParameters(strings, 2))
			{
				return "Format is \"campaign.create_random_clan [KingdomName]\".";
			}
			Kingdom kingdom;
			if (strings.Count > 0)
			{
				kingdom = CampaignCheats.GetKingdom(CampaignCheats.ConcatenateString(strings));
			}
			else
			{
				kingdom = Kingdom.All.GetRandomElement<Kingdom>();
			}
			if (kingdom == null)
			{
				return "Kingdom is not valid!";
			}
			CultureObject culture = kingdom.Culture;
			Settlement settlement = kingdom.Settlements.FirstOrDefault((Settlement x) => x.IsTown) ?? kingdom.Settlements.GetRandomElement<Settlement>();
			TextObject textObject = NameGenerator.Current.GenerateClanName(culture, settlement);
			Clan clan = Clan.CreateClan("test_clan_" + Clan.All.Count);
			clan.InitializeClan(textObject, new TextObject("{=!}informal", null), Kingdom.All.GetRandomElement<Kingdom>().Culture, Banner.CreateRandomClanBanner(-1), default(Vec2), false);
			CharacterObject characterObject = culture.LordTemplates.FirstOrDefault((CharacterObject x) => x.Occupation == Occupation.Lord);
			if (characterObject == null)
			{
				return "Can't find a proper lord template.";
			}
			Settlement randomElement = kingdom.Settlements.GetRandomElement<Settlement>();
			Hero hero = HeroCreator.CreateSpecialHero(characterObject, randomElement, clan, null, MBRandom.RandomInt(18, 36));
			hero.HeroDeveloper.DeriveSkillsFromTraits(false, null);
			hero.ChangeState(Hero.CharacterStates.Active);
			clan.SetLeader(hero);
			ChangeKingdomAction.ApplyByJoinToKingdom(clan, kingdom, false);
			EnterSettlementAction.ApplyForCharacterOnly(hero, settlement);
			GiveGoldAction.ApplyBetweenCharacters(null, hero, 15000, false);
			return string.Format("{0} is added to {1}. Its leader is: {2}", clan.Name, kingdom.Name, hero.Name);
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("join_kingdom", "campaign")]
		public static string JoinKingdom(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckParameters(strings, 0))
			{
				return "Format is \"campaign.join_kingdom[KingdomName / FirstTwoCharactersOfKingdomName]\".\nWrite \"campaign.join_kingdom help\" to list available Kingdoms.";
			}
			string text = CampaignCheats.ConcatenateString(strings);
			if (text.ToLower() == "help")
			{
				string text2 = "";
				text2 += "\n";
				text2 += "Format is \"campaign.join_kingdom [KingdomName/FirstTwoCharacterOfKingdomName]\".";
				text2 += "\n";
				text2 += "Available Kingdoms";
				text2 += "\n";
				foreach (Kingdom kingdom in Kingdom.All)
				{
					text2 = text2 + "Kingdom name " + kingdom.Name.ToString() + "\n";
				}
				return text2;
			}
			Kingdom kingdom2 = null;
			foreach (Kingdom kingdom3 in Kingdom.All)
			{
				if (kingdom3.Name.ToString().Equals(text, StringComparison.OrdinalIgnoreCase))
				{
					kingdom2 = kingdom3;
					break;
				}
				if (text.Length >= 2 && kingdom3.Name.ToString().ToLower().Substring(0, 2)
					.Equals(text.ToLower().Substring(0, 2)))
				{
					kingdom2 = kingdom3;
					break;
				}
			}
			if (kingdom2 == null)
			{
				return "Kingdom is not found: " + text;
			}
			ChangeKingdomAction.ApplyByJoinToKingdom(Hero.MainHero.Clan, kingdom2, true);
			return "Success";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("join_kingdom_as_mercenary", "campaign")]
		public static string JoinKingdomAsMercenary(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckParameters(strings, 0))
			{
				return "Format is \"campaign.join_kingdom_as_mercenary[KingdomName / FirstTwoCharactersOfKingdomName]\".\nWrite \"campaign.join_kingdom_as_mercenary help\" to list available Kingdoms.";
			}
			string text = CampaignCheats.ConcatenateString(strings);
			if (text.ToLower() == "help")
			{
				string text2 = "";
				text2 += "\n";
				text2 += "Format is \"campaign.join_kingdom_as_mercenary [KingdomName/FirstTwoCharacterOfKingdomName]\".";
				text2 += "\n";
				text2 += "Available Kingdoms";
				text2 += "\n";
				foreach (Kingdom kingdom in Kingdom.All)
				{
					text2 = text2 + "Kingdom name " + kingdom.Name.ToString() + "\n";
				}
				return text2;
			}
			Kingdom kingdom2 = null;
			foreach (Kingdom kingdom3 in Kingdom.All)
			{
				if (kingdom3.Name.ToString().Equals(text, StringComparison.OrdinalIgnoreCase))
				{
					kingdom2 = kingdom3;
					break;
				}
				if (text.Length >= 2 && kingdom3.Name.ToString().ToLower().Substring(0, 2)
					.Equals(text.ToLower().Substring(0, 2)))
				{
					kingdom2 = kingdom3;
					break;
				}
			}
			if (kingdom2 == null)
			{
				return "Kingdom is not found: " + text;
			}
			ChangeKingdomAction.ApplyByJoinFactionAsMercenary(Hero.MainHero.Clan, kingdom2, 50, true);
			return "Success";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("set_criminal_rating", "campaign")]
		public static string SetCriminalRating(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			int num = 0;
			if (CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckParameters(strings, 1) || !int.TryParse(strings[strings.Count - 1], out num))
			{
				return "Format is \"campaign.set_criminal_rating[FactionName][Value]\".\nWrite \"campaign.set_criminal_rating help\" to list available Factions.";
			}
			strings.RemoveAt(strings.Count - 1);
			string text = CampaignCheats.ConcatenateString(strings);
			if (text.ToLower() == "help")
			{
				string text2 = "";
				text2 += "\n";
				text2 += "Format is \"campaign.set_faction_criminal_rating [Faction] [Gold]\".";
				text2 += "\n";
				text2 += "Available Factions";
				text2 += "\n";
				foreach (Kingdom kingdom in Kingdom.All)
				{
					text2 = text2 + "Faction name " + kingdom.Name.ToString() + "\n";
				}
				foreach (Clan clan in Clan.NonBanditFactions)
				{
					text2 = text2 + "Faction name " + clan.Name.ToString() + "\n";
				}
				return text2;
			}
			foreach (Clan clan2 in Clan.NonBanditFactions)
			{
				if (clan2.Name.ToString().Equals(text, StringComparison.OrdinalIgnoreCase))
				{
					ChangeCrimeRatingAction.Apply(clan2, (float)num - clan2.MainHeroCrimeRating, true);
					return "Success";
				}
			}
			foreach (Kingdom kingdom2 in Kingdom.All)
			{
				if (kingdom2.Name.ToString().Equals(text, StringComparison.OrdinalIgnoreCase))
				{
					ChangeCrimeRatingAction.Apply(kingdom2, (float)num - kingdom2.MainHeroCrimeRating, true);
					return "Success";
				}
			}
			return "Faction is not found: " + text;
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("get_criminal_ratings", "campaign")]
		public static string GetCriminalRatings(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (!CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.get_criminal_ratings";
			}
			string text = "";
			bool flag = true;
			foreach (Kingdom kingdom in Kingdom.All)
			{
				if (kingdom.MainHeroCrimeRating > 0f)
				{
					text = string.Concat(new object[] { text, kingdom.Name, "   criminal rating: ", kingdom.MainHeroCrimeRating, "\n" });
					flag = false;
				}
			}
			text += "-----------\n";
			foreach (Clan clan in Clan.NonBanditFactions)
			{
				if (clan.MainHeroCrimeRating > 0f)
				{
					text = string.Concat(new object[] { text, clan.Name, "   criminal rating: ", clan.MainHeroCrimeRating, "\n" });
					flag = false;
				}
			}
			if (flag)
			{
				return "You don't have any criminal rating.";
			}
			return text;
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("set_main_hero_age", "campaign")]
		public static string SetMainHeroAge(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.set_main_hero_age [Age]\".";
			}
			int num = 1;
			if (!int.TryParse(strings[0], out num))
			{
				return "Please enter a number";
			}
			if (num < Campaign.Current.Models.AgeModel.HeroComesOfAge || num > Campaign.Current.Models.AgeModel.MaxAge)
			{
				return string.Format("Age must be between {0} - {1}", Campaign.Current.Models.AgeModel.HeroComesOfAge, Campaign.Current.Models.AgeModel.MaxAge);
			}
			Hero.MainHero.SetBirthDay(HeroHelper.GetRandomBirthDayForAge((float)num));
			return "Success";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("get_infested_hideout", "campaign")]
		public static string GetInfestedHideout(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckHelp(strings) || CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.get_infested_hideout [Optional: Number of troops]\".";
			}
			MBList<Settlement> mblist = Settlement.All.Where((Settlement t) => t.IsHideout && t.Parties.Count > 0).ToMBList<Settlement>();
			if (mblist.IsEmpty<Settlement>())
			{
				return "All hideouts are empty!";
			}
			Settlement settlement;
			if (strings.Count > 0)
			{
				int troopCount = -1;
				int.TryParse(strings[0], out troopCount);
				if (troopCount == -1)
				{
					return "Incorrect input.";
				}
				MBList<Settlement> mblist2 = mblist.Where((Settlement t) => t.Parties.Sum((MobileParty p) => p.MemberRoster.TotalManCount) >= troopCount).ToMBList<Settlement>();
				if (mblist2.IsEmpty<Settlement>())
				{
					return "Can't find suitable hideout.";
				}
				settlement = mblist2.GetRandomElement<Settlement>();
			}
			else
			{
				settlement = mblist.GetRandomElement<Settlement>();
			}
			if (settlement != null)
			{
				settlement.Party.SetAsCameraFollowParty();
				return "Success";
			}
			return "Unable to find such a hideout.";
		}

		public static bool MainPartyIsAttackable
		{
			get
			{
				return CampaignCheats._mainPartyIsAttackable;
			}
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("set_main_party_attackable", "campaign")]
		public static string SetMainPartyAttackable(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckHelp(strings) || !CampaignCheats.CheckParameters(strings, 1))
			{
				return "Format is campaign.set_main_party_attackable [1/0]\".";
			}
			if (strings[0] == "0" || strings[0] == "1")
			{
				bool flag = strings[0] == "1";
				CampaignCheats._mainPartyIsAttackable = flag;
				return "Main party is" + (flag ? " " : " NOT ") + "attackable.";
			}
			return "Wrong input";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("add_morale_to_party", "campaign")]
		public static string AddMoraleToParty(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.add_morale_to_party [Number] [HeroName]\".";
			}
			int num = 10;
			Hero hero = Hero.MainHero;
			string text = "";
			bool flag = false;
			if (CampaignCheats.CheckParameters(strings, 1))
			{
				if (!int.TryParse(strings[0], out num))
				{
					num = 10;
					text = strings[0];
					hero = CampaignCheats.GetHero(text);
					flag = true;
				}
			}
			else if (!CampaignCheats.CheckParameters(strings, 0))
			{
				if (!int.TryParse(strings[0], out num))
				{
					num = 100;
					text = CampaignCheats.ConcatenateString(strings.GetRange(0, strings.Count));
					hero = CampaignCheats.GetHero(text);
				}
				else
				{
					text = CampaignCheats.ConcatenateString(strings.GetRange(1, strings.Count - 1));
					hero = CampaignCheats.GetHero(text);
				}
				flag = true;
			}
			if (hero != null)
			{
				MobileParty partyBelongedTo = hero.PartyBelongedTo;
				if (partyBelongedTo != null)
				{
					float num2 = MBMath.ClampFloat(partyBelongedTo.RecentEventsMorale + (float)num, 0f, float.MaxValue);
					float num3 = num2 - partyBelongedTo.RecentEventsMorale;
					partyBelongedTo.RecentEventsMorale = num2;
					return string.Format("The base morale of {0}'s party changed by {1}.", hero.Name, num3);
				}
				return "Hero: " + text + " does not belonged to any party.\nFormat is \"campaign.add_morale_to_party [Number] [HeroName]\"";
			}
			else
			{
				if (flag)
				{
					return "Hero: " + text + " not found.\nFormat is \"campaign.add_morale_to_party [Number] [HeroName]\"";
				}
				return "Wrong input.\nFormat is \"campaign.add_morale_to_party [Number] [HeroName]\"";
			}
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("boost_cohesion_of_army", "campaign")]
		public static string BoostCohesionOfArmy(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"boost_cohesion_of_army [ArmyLeaderName]\".";
			}
			Hero hero = Hero.MainHero;
			Army army = hero.PartyBelongedTo.Army;
			if (!CampaignCheats.CheckParameters(strings, 0))
			{
				string text = CampaignCheats.ConcatenateString(strings.GetRange(0, strings.Count));
				hero = CampaignCheats.GetHero(text);
				if (hero == null)
				{
					return "Hero: " + text + " not found.";
				}
				if (hero.PartyBelongedTo == null)
				{
					return "Hero: " + text + " does not belong to any army.";
				}
				army = hero.PartyBelongedTo.Army;
				if (army == null)
				{
					return "Hero: " + text + " does not belong to any army.";
				}
			}
			if (army != null)
			{
				army.Cohesion = 100f;
				return string.Format("{0}'s army cohesion is boosted.", hero.Name);
			}
			return "Wrong input.\nFormat is \"boost_cohesion_of_army [ArmyLeaderName]\".";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("boost_cohesion_of_all_armies", "campaign")]
		public static string BoostCohersionOfAllArmies(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"boost_cohersion_of_all_armies\".";
			}
			if (CampaignCheats.CheckParameters(strings, 0))
			{
				foreach (MobileParty mobileParty in MobileParty.All)
				{
					if (mobileParty.Army != null)
					{
						mobileParty.Army.Cohesion = 100f;
					}
				}
				return "All armies cohesion are boosted.";
			}
			return "Wrong input.\nFormat is \"boost_cohesion_of_all_armies [ArmyLeaderName]\".";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("add_focus_points_to_hero", "campaign")]
		public static string AddFocusPointCheat(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.add_focus_points_to_hero [PositiveNumber] [HeroName]\".";
			}
			bool flag = false;
			int num = 1;
			Hero hero;
			if (CampaignCheats.CheckParameters(strings, 0))
			{
				Hero.MainHero.HeroDeveloper.UnspentFocusPoints = MBMath.ClampInt(Hero.MainHero.HeroDeveloper.UnspentFocusPoints + 1, 0, int.MaxValue);
				hero = Hero.MainHero;
				return string.Format("{0} focus points added to the {1}. ", num, hero.Name);
			}
			int num2 = 0;
			bool flag2 = int.TryParse(strings[0], out num2);
			if (num2 <= 0 && flag2)
			{
				return "Enter a Positive number.\nFormat is \"campaign.add_focus_points_to_hero [PositiveNumber] [HeroName]\".";
			}
			if (CampaignCheats.CheckParameters(strings, 1) && flag2)
			{
				Hero.MainHero.HeroDeveloper.UnspentFocusPoints = MBMath.ClampInt(Hero.MainHero.HeroDeveloper.UnspentFocusPoints + num2, 0, int.MaxValue);
				hero = Hero.MainHero;
				flag = true;
				num = num2;
			}
			else if (flag2)
			{
				hero = CampaignCheats.GetHero(CampaignCheats.ConcatenateString(strings.GetRange(1, strings.Count - 1)));
				if (hero != null)
				{
					hero.HeroDeveloper.UnspentFocusPoints = MBMath.ClampInt(hero.HeroDeveloper.UnspentFocusPoints + num2, 0, int.MaxValue);
					flag = true;
					num = num2;
				}
			}
			else
			{
				hero = CampaignCheats.GetHero(CampaignCheats.ConcatenateString(strings.GetRange(0, strings.Count)));
				if (hero != null)
				{
					hero.HeroDeveloper.UnspentFocusPoints = MBMath.ClampInt(hero.HeroDeveloper.UnspentFocusPoints + 1, 0, int.MaxValue);
					flag = true;
				}
			}
			if (flag)
			{
				return string.Format("{0} focus points added to the {1}. ", num, hero.Name);
			}
			return "Hero is not Found.\nFormat is \"campaign.add_focus_points_to_hero [PositiveNumber] [HeroName]\".";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("add_attribute_points_to_hero", "campaign")]
		public static string AddAttributePointsCheat(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.add_attribute_points_to_hero [PositiveNumber] [HeroName]\".";
			}
			bool flag = false;
			int num = 1;
			Hero hero;
			if (CampaignCheats.CheckParameters(strings, 0))
			{
				Hero.MainHero.HeroDeveloper.UnspentAttributePoints = MBMath.ClampInt(Hero.MainHero.HeroDeveloper.UnspentAttributePoints + 1, 0, int.MaxValue);
				hero = Hero.MainHero;
				return string.Format("{0} attribute points added to the {1}. ", num, hero.Name);
			}
			int num2;
			bool flag2 = int.TryParse(strings[0], out num2);
			if (num2 <= 0 && flag2)
			{
				return "Enter a Positive number.\nFormat is \"campaign.add_attribute_points_to_hero [PositiveNumber] [HeroName]\".";
			}
			if (CampaignCheats.CheckParameters(strings, 1) && flag2)
			{
				Hero.MainHero.HeroDeveloper.UnspentAttributePoints = MBMath.ClampInt(Hero.MainHero.HeroDeveloper.UnspentAttributePoints + num2, 0, int.MaxValue);
				hero = Hero.MainHero;
				flag = true;
				num = num2;
			}
			else if (flag2)
			{
				hero = CampaignCheats.GetHero(CampaignCheats.ConcatenateString(strings.GetRange(1, strings.Count - 1)));
				if (hero != null)
				{
					hero.HeroDeveloper.UnspentAttributePoints = MBMath.ClampInt(hero.HeroDeveloper.UnspentAttributePoints + num2, 0, int.MaxValue);
					flag = true;
					num = num2;
				}
			}
			else
			{
				hero = CampaignCheats.GetHero(CampaignCheats.ConcatenateString(strings.GetRange(0, strings.Count)));
				if (hero != null)
				{
					hero.HeroDeveloper.UnspentAttributePoints = MBMath.ClampInt(hero.HeroDeveloper.UnspentAttributePoints + 1, 0, int.MaxValue);
					flag = true;
				}
			}
			if (flag)
			{
				return string.Format("{0} attribute points added to the {1}. ", num, hero.Name);
			}
			return "Hero is not Found.\nFormat is \"campaign.UnspentAttributePoints [PositiveNumber] [HeroName]\".";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("list_tournaments", "campaign")]
		public static string GetSettlementsWithTournament(List<string> strings)
		{
			string text = "";
			if (!CampaignCheats.CheckCheatUsage(ref text))
			{
				return text;
			}
			if (!Campaign.Current.IsDay)
			{
				return "Cant list tournaments. Wait day light.";
			}
			string text2 = "";
			foreach (Town town in Town.AllTowns)
			{
				if (Campaign.Current.TournamentManager.GetTournamentGame(town) != null)
				{
					text2 = text2 + town.Name + "\n";
				}
			}
			return text2;
		}

		public static string ConvertListToMultiLine(List<string> strings)
		{
			string text = "";
			foreach (string text2 in strings)
			{
				text = text + text2 + "\n";
			}
			return text;
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("print_all_issues", "campaign")]
		public static string PrintAllIssues(List<string> strings)
		{
			string text = "";
			if (!CampaignCheats.CheckCheatUsage(ref text))
			{
				return text;
			}
			string text2 = "Total issue count : " + Campaign.Current.IssueManager.Issues.Count + "\n";
			int num = 0;
			foreach (KeyValuePair<Hero, IssueBase> keyValuePair in Campaign.Current.IssueManager.Issues)
			{
				text2 = string.Concat(new object[]
				{
					text2,
					++num,
					") ",
					keyValuePair.Value.Title,
					", ",
					keyValuePair.Key,
					": ",
					keyValuePair.Value.IssueSettlement,
					"\n"
				});
			}
			return text2;
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("print_issues", "campaign")]
		public static string PrintIssues(List<string> strings)
		{
			string text = "";
			if (!CampaignCheats.CheckCheatUsage(ref text))
			{
				return text;
			}
			string text2 = "";
			Dictionary<Type, string> dictionary = new Dictionary<Type, string>();
			foreach (KeyValuePair<Hero, IssueBase> keyValuePair in Campaign.Current.IssueManager.Issues)
			{
				if (!dictionary.ContainsKey(keyValuePair.Value.GetType()))
				{
					dictionary.Add(keyValuePair.Value.GetType(), string.Concat(new object[]
					{
						keyValuePair.Value.Title,
						", ",
						keyValuePair.Key,
						": ",
						keyValuePair.Value.IssueSettlement,
						"\n"
					}));
				}
			}
			foreach (KeyValuePair<Type, string> keyValuePair2 in dictionary)
			{
				text2 += keyValuePair2.Value;
			}
			return text2;
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("give_workshop_to_player", "campaign")]
		public static string GiveWorkshopToPlayer(List<string> strings)
		{
			string text = "Wrong input";
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				text = CampaignCheats.ErrorType;
			}
			else if (CampaignCheats.CheckHelp(strings) || !CampaignCheats.CheckParameters(strings, 2))
			{
				if (Settlement.CurrentSettlement != null)
				{
					if (CampaignCheats.CheckParameters(strings, 1))
					{
						int num;
						if (int.TryParse(strings[0], out num) && num > 0 && num < Settlement.CurrentSettlement.Town.Workshops.Length)
						{
							Workshop workshop = Settlement.CurrentSettlement.Town.Workshops[num];
							ChangeOwnerOfWorkshopAction.ApplyByTrade(workshop, Hero.MainHero, workshop.WorkshopType, Campaign.Current.Models.WorkshopModel.GetInitialCapital(1), true, 0, null);
							text = string.Format("Gave {0} to {1}", workshop.WorkshopType.Name, Hero.MainHero.Name);
						}
					}
					else
					{
						text = "Format is \"campaign.give_workshop_to_player [settlement] [workshop_no]\".";
						for (int i = 0; i < Settlement.CurrentSettlement.Town.Workshops.Length; i++)
						{
							Workshop workshop2 = Settlement.CurrentSettlement.Town.Workshops[i];
							text = string.Concat(new object[]
							{
								text,
								"\n",
								i,
								" : ",
								workshop2.Name,
								" - owner : ",
								(workshop2.Owner != null) ? workshop2.Owner.Name.ToString() : ""
							});
							if (workshop2.WorkshopType.IsHidden)
							{
								text += "(hidden)";
							}
						}
					}
				}
				else
				{
					text = "You need to be in a settlement to see the workshops available.";
				}
			}
			else
			{
				Settlement settlement = CampaignCheats.GetSettlement(strings[0]);
				if (settlement == null || !settlement.IsTown)
				{
					return "Settlement should be a town";
				}
				int num2;
				if (int.TryParse(strings[1], out num2) && num2 >= 0 && num2 < settlement.Town.Workshops.Length)
				{
					Workshop workshop3 = settlement.Town.Workshops[num2];
					if (workshop3.WorkshopType.IsHidden)
					{
						text = "Workshop is hidden";
					}
					else
					{
						ChangeOwnerOfWorkshopAction.ApplyByTrade(workshop3, Hero.MainHero, workshop3.WorkshopType, Campaign.Current.Models.WorkshopModel.GetInitialCapital(1), true, 0, null);
						text = "Success";
					}
				}
			}
			return text;
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("conceive_child", "campaign")]
		public static string MakePregnant(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (Hero.MainHero.Spouse == null)
			{
				Hero hero = Hero.AllAliveHeroes.FirstOrDefault((Hero t) => t != Hero.MainHero && Campaign.Current.Models.MarriageModel.IsCoupleSuitableForMarriage(Hero.MainHero, t));
				if (hero == null)
				{
					return "error";
				}
				MarriageAction.Apply(Hero.MainHero, hero, true);
				if (Hero.MainHero.IsFemale ? (!Hero.MainHero.IsPregnant) : (!Hero.MainHero.Spouse.IsPregnant))
				{
					MakePregnantAction.Apply(Hero.MainHero.IsFemale ? Hero.MainHero : Hero.MainHero.Spouse);
					return "success";
				}
				return "You are expecting a child already.";
			}
			else
			{
				if (Hero.MainHero.IsFemale ? (!Hero.MainHero.IsPregnant) : (!Hero.MainHero.Spouse.IsPregnant))
				{
					MakePregnantAction.Apply(Hero.MainHero.IsFemale ? Hero.MainHero : Hero.MainHero.Spouse);
					return "success";
				}
				return "You are expecting a child already.";
			}
		}

		public static Hero GenerateChild(Hero hero, bool isFemale, CultureObject culture)
		{
			if (hero.Spouse == null)
			{
				List<Hero> list = Hero.AllAliveHeroes.ToList<Hero>();
				list.Shuffle<Hero>();
				Hero hero2 = list.FirstOrDefault((Hero t) => t != hero && Campaign.Current.Models.MarriageModel.IsCoupleSuitableForMarriage(hero, t));
				if (hero2 != null)
				{
					MarriageAction.Apply(hero, hero2, true);
					if (hero.IsFemale ? (!hero.IsPregnant) : (!hero.Spouse.IsPregnant))
					{
						MakePregnantAction.Apply(hero.IsFemale ? hero : hero.Spouse);
					}
				}
			}
			Hero hero3 = (hero.IsFemale ? hero : hero.Spouse);
			Hero spouse = hero3.Spouse;
			Hero hero4 = HeroCreator.DeliverOffSpring(hero3, spouse, isFemale);
			hero3.IsPregnant = false;
			return hero4;
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("add_prisoner_to_party", "campaign")]
		public static string AddPrisonerToParty(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckHelp(strings) || CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckParameters(strings, 1))
			{
				return "Format is \"campaign.add_prisoner_to_party [prisonerName] | [capturername]\".";
			}
			string[] array = CampaignCheats.ConcatenateString(strings).Split(new char[] { '|' });
			if (array.Length != 2)
			{
				return "Format is \"campaign.add_prisoner_to_party [prisonerName] | [capturername]\".";
			}
			string text = array[0].Trim();
			string text2 = array[1].Trim();
			Hero hero = CampaignCheats.GetHero(text);
			Hero hero2 = CampaignCheats.GetHero(text2);
			if (hero == null || hero2 == null)
			{
				return "Can't find one of the heroes";
			}
			if (!hero2.IsActive || hero2.PartyBelongedTo == null)
			{
				return "Capturer hero is not active!";
			}
			if (!hero.IsActive || hero.IsHumanPlayerCharacter || (hero.Occupation != Occupation.Lord && hero.Occupation != Occupation.Wanderer))
			{
				return "Hero can't be taken as a prisoner!";
			}
			if (!FactionManager.IsAtWarAgainstFaction(hero.MapFaction, hero2.MapFaction))
			{
				return "Factions are not at war!";
			}
			if (hero.PartyBelongedTo != null)
			{
				if (hero.PartyBelongedTo.MapEvent != null)
				{
					return "prisoners party shouldn't be in a map event.";
				}
				if (hero.PartyBelongedTo.LeaderHero == hero)
				{
					DestroyPartyAction.Apply(null, hero.PartyBelongedTo);
				}
				else
				{
					hero.PartyBelongedTo.MemberRoster.RemoveTroop(hero.CharacterObject, 1, default(UniqueTroopDescriptor), 0);
				}
			}
			if (hero.IsPrisoner)
			{
				EndCaptivityAction.ApplyByEscape(hero, null);
			}
			if (hero.CurrentSettlement != null)
			{
				LeaveSettlementAction.ApplyForCharacterOnly(hero);
			}
			if (hero2.IsHumanPlayerCharacter)
			{
				hero.SetHasMet();
			}
			TakePrisonerAction.Apply(hero2.PartyBelongedTo.Party, hero);
			return "Success";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("add_random_prisoner_hero", "campaign")]
		public static string AddRandomPrisonerHeroCheat(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.add_random_prisoner_hero\".";
			}
			if (!Hero.MainHero.IsPartyLeader)
			{
				return string.Format("{0} is not a party leader.", Hero.MainHero.Name);
			}
			Hero randomElementWithPredicate = Hero.AllAliveHeroes.GetRandomElementWithPredicate((Hero x) => !x.CharacterObject.IsPlayerCharacter && x.IsActive && x.PartyBelongedTo == null && !x.IsPrisoner && x.CharacterObject.Occupation == Occupation.Lord);
			if (randomElementWithPredicate == null)
			{
				return "There is not any available heroes right now.";
			}
			if (randomElementWithPredicate.CurrentSettlement != null)
			{
				LeaveSettlementAction.ApplyForCharacterOnly(randomElementWithPredicate);
			}
			MobileParty partyBelongedTo = randomElementWithPredicate.PartyBelongedTo;
			bool flag = ((partyBelongedTo != null) ? partyBelongedTo.LeaderHero : null) == randomElementWithPredicate;
			MobileParty partyBelongedTo2 = randomElementWithPredicate.PartyBelongedTo;
			TakePrisonerAction.Apply(PartyBase.MainParty, randomElementWithPredicate);
			randomElementWithPredicate.SetHasMet();
			if (flag)
			{
				DisbandPartyAction.StartDisband(partyBelongedTo2);
				partyBelongedTo2.IsDisbanding = true;
			}
			return "Success";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("control_party_ai_by_cheats", "campaign")]
		public static string ControlPartyAIByCheats(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckHelp(strings) || CampaignCheats.CheckParameters(strings, 1) || (strings[0] != "0" && strings[0] != "1"))
			{
				return "Format is \"campaign.control_party_ai_by_cheats [0|1] [HeroName]\".";
			}
			Hero hero = CampaignCheats.GetHero(CampaignCheats.ConcatenateString(strings.GetRange(1, strings.Count - 1)));
			bool flag = strings[0] == "1";
			string text;
			CampaignCheats.ControlPartyAIByCheatsInternal(hero, flag, out text);
			return text;
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("ai_siege_settlement", "campaign")]
		public static string AISiegeSettlement(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			string text = "Format is \"campaign.ai_siege_settlement [HeroName] | [SettlementName]\".";
			if (CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckHelp(strings) || CampaignCheats.CheckParameters(strings, 1))
			{
				return text;
			}
			List<string> separatedNames = CampaignCheats.GetSeparatedNames(strings, "|");
			if (separatedNames.Count != 2)
			{
				return text;
			}
			Hero hero = CampaignCheats.GetHero(separatedNames[0]);
			Settlement settlement = CampaignCheats.GetSettlement(separatedNames[1]);
			if (hero == null)
			{
				return "Hero is not found";
			}
			if (settlement == null)
			{
				return "Settlement is not found";
			}
			if (!settlement.IsFortification)
			{
				return "Settlement is not a fortification (Town or Castle)";
			}
			if (hero.MapFaction == settlement.MapFaction)
			{
				return string.Format("Hero Faction: {0} and Settlement Faction: {1} are the same", hero.MapFaction.Name, settlement.MapFaction.Name);
			}
			if (!FactionManager.IsAtWarAgainstFaction(hero.MapFaction, settlement.MapFaction))
			{
				return string.Format("Hero Faction: {0} and Settlement Faction: {1} are not at war, you can use \"campaign.declare_war\" cheat", hero.MapFaction.Name, settlement.MapFaction.Name);
			}
			if (settlement.IsUnderSiege)
			{
				return "Settlement is already under siege";
			}
			string text2;
			if (CampaignCheats.ControlPartyAIByCheatsInternal(hero, true, out text2))
			{
				SetPartyAiAction.GetActionForBesiegingSettlement(hero.PartyBelongedTo, settlement);
				return text2 + "\nParty AI can be enabled again by using \"campaign.control_party_ai_by_cheats\"\nSuccess";
			}
			return text2;
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("ai_raid_village", "campaign")]
		public static string AIRaidVillage(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			string text = "Format is \"campaign.ai_raid_village [HeroName] | [VillageName]\".";
			if (CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckHelp(strings) || CampaignCheats.CheckParameters(strings, 1))
			{
				return text;
			}
			List<string> separatedNames = CampaignCheats.GetSeparatedNames(strings, "|");
			if (separatedNames.Count != 2)
			{
				return text;
			}
			Hero hero = CampaignCheats.GetHero(separatedNames[0]);
			Settlement settlement = CampaignCheats.GetSettlement(separatedNames[1]);
			if (hero == null)
			{
				return "Hero is not found";
			}
			if (settlement == null)
			{
				return "Settlement is not found";
			}
			if (!settlement.IsVillage)
			{
				return "Settlement is not a village";
			}
			if (hero.MapFaction == settlement.MapFaction)
			{
				return string.Format("Hero Faction: {0} and Village Faction: {1} are the same", hero.MapFaction.Name, settlement.MapFaction.Name);
			}
			if (!FactionManager.IsAtWarAgainstFaction(hero.MapFaction, settlement.MapFaction))
			{
				return string.Format("Hero Faction: {0} and Village Faction: {1} are not at war, you can use \"campaign.declare_war\" cheat", hero.MapFaction.Name, settlement.MapFaction.Name);
			}
			if (settlement.IsUnderRaid)
			{
				return "Village is already under raid";
			}
			string text2;
			if (CampaignCheats.ControlPartyAIByCheatsInternal(hero, true, out text2))
			{
				SetPartyAiAction.GetActionForRaidingSettlement(hero.PartyBelongedTo, settlement);
				return text2 + "\nParty AI can be enabled again by using \"campaign.control_party_ai_by_cheats\"\nSuccess";
			}
			return text2;
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("ai_attack_party", "campaign")]
		public static string AIAttackParty(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			string text = "Format is \"campaign.ai_attack_party [AttackerHeroName] | [HeroName]\".";
			if (CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckHelp(strings) || CampaignCheats.CheckParameters(strings, 1))
			{
				return text;
			}
			List<string> separatedNames = CampaignCheats.GetSeparatedNames(strings, "|");
			if (separatedNames.Count != 2)
			{
				return text;
			}
			Hero hero = CampaignCheats.GetHero(separatedNames[0]);
			Hero hero2 = CampaignCheats.GetHero(separatedNames[1]);
			if (hero == null || hero2 == null)
			{
				return "Hero is not found";
			}
			if (hero2.PartyBelongedTo == null)
			{
				return "Second hero is not in a party";
			}
			if (hero.MapFaction == hero2.MapFaction)
			{
				return string.Format("Attacker Hero Faction: {0} and Other Hero Faction: {1} are the same", hero.MapFaction.Name, hero2.MapFaction.Name);
			}
			if (!FactionManager.IsAtWarAgainstFaction(hero.MapFaction, hero2.MapFaction))
			{
				return string.Format("Attacker Hero Faction: {0} and Other Hero Faction: {1} are not at war, you can use \"campaign.declare_war\" cheat", hero.MapFaction.Name, hero2.MapFaction.Name);
			}
			string text2;
			if (CampaignCheats.ControlPartyAIByCheatsInternal(hero, true, out text2))
			{
				SetPartyAiAction.GetActionForEngagingParty(hero.PartyBelongedTo, hero2.PartyBelongedTo);
				return text2 + "\nParty AI can be enabled again by using \"campaign.control_party_ai_by_cheats\"\nSuccess";
			}
			return text2;
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("ai_defend_settlement", "campaign")]
		public static string AIDefendSettlement(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			string text = "Format is \"campaign.ai_defend_settlement [HeroName] | [SettlementName]\".";
			if (CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckHelp(strings) || CampaignCheats.CheckParameters(strings, 1))
			{
				return text;
			}
			List<string> separatedNames = CampaignCheats.GetSeparatedNames(strings, "|");
			if (separatedNames.Count != 2)
			{
				return text;
			}
			Hero hero = CampaignCheats.GetHero(separatedNames[0]);
			Settlement settlement = CampaignCheats.GetSettlement(separatedNames[1]);
			if (hero == null)
			{
				return "Hero is not found";
			}
			if (settlement == null)
			{
				return "Settlement is not found";
			}
			if (FactionManager.IsAtWarAgainstFaction(hero.MapFaction, settlement.MapFaction))
			{
				return string.Format("Hero Faction: {0} and Settlement Faction: {1} are at war.", hero.MapFaction.Name, settlement.MapFaction.Name);
			}
			if (!settlement.IsUnderRaid && !settlement.IsUnderSiege)
			{
				return "Settlement is not under siege nor raid";
			}
			string text2;
			if (CampaignCheats.ControlPartyAIByCheatsInternal(hero, true, out text2))
			{
				SetPartyAiAction.GetActionForDefendingSettlement(hero.PartyBelongedTo, settlement);
				return text2 + "\nParty AI can be enabled again by using \"campaign.control_party_ai_by_cheats\"\nSuccess";
			}
			return text2;
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("ai_goto_settlement", "campaign")]
		public static string AIGotoSettlement(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			string text = "Format is \"campaign.ai_goto_settlement [HeroName] | [SettlementName]\".";
			if (CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckHelp(strings) || CampaignCheats.CheckParameters(strings, 1))
			{
				return text;
			}
			List<string> separatedNames = CampaignCheats.GetSeparatedNames(strings, "|");
			if (separatedNames.Count != 2)
			{
				return text;
			}
			Hero hero = CampaignCheats.GetHero(separatedNames[0]);
			Settlement settlement = CampaignCheats.GetSettlement(separatedNames[1]);
			if (hero == null)
			{
				return "Hero is not found";
			}
			if (settlement == null)
			{
				return "Settlement is not found";
			}
			if (FactionManager.IsAtWarAgainstFaction(hero.MapFaction, settlement.MapFaction))
			{
				return string.Format("Hero Faction: {0} and Settlement Faction: {1} are at war", hero.MapFaction, settlement.MapFaction.Name);
			}
			string text2;
			if (CampaignCheats.ControlPartyAIByCheatsInternal(hero, true, out text2))
			{
				SetPartyAiAction.GetActionForVisitingSettlement(hero.PartyBelongedTo, settlement);
				return text2 + "\nParty AI can be enabled again by using \"campaign.control_party_ai_by_cheats\"\nSuccess";
			}
			return text2;
		}

		public static List<string> GetSeparatedNames(List<string> strings, string separator)
		{
			List<string> list = new List<string>();
			List<int> list2 = new List<int>(strings.Count);
			for (int i = 0; i < strings.Count; i++)
			{
				if (strings[i] == separator)
				{
					list2.Add(i);
				}
			}
			list2.Add(strings.Count);
			int num = 0;
			for (int j = 0; j < list2.Count; j++)
			{
				int num2 = list2[j];
				string text = CampaignCheats.ConcatenateString(strings.GetRange(num, num2 - num));
				num = num2 + 1;
				list.Add(text);
			}
			return list;
		}

		private static bool ControlPartyAIByCheatsInternal(Hero hero, bool enable, out string resultDescription)
		{
			if (hero == null)
			{
				resultDescription = "Hero is not found";
				return false;
			}
			if (hero == Hero.MainHero)
			{
				resultDescription = "Hero cannot be MainHero";
				return false;
			}
			MobileParty partyBelongedTo = hero.PartyBelongedTo;
			if (partyBelongedTo == null)
			{
				resultDescription = "Hero is not part of a party";
				return false;
			}
			if (partyBelongedTo.Army != null && partyBelongedTo.Army.LeaderParty != partyBelongedTo)
			{
				resultDescription = "Party AI cannot be changed while party is part of an army and not the leader of the army";
				return false;
			}
			partyBelongedTo.Ai.SetDoNotMakeNewDecisions(enable);
			resultDescription = (enable ? string.Format("Party AI of {0} is controlled by cheats", hero) : string.Format("Party AI of {0} isn't controlled by cheats", hero));
			return true;
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("clear_settlement_defense", "campaign")]
		public static string ClearSettlementDefense(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.clear_settlement_defense [SettlementName]\".";
			}
			Settlement settlement = CampaignCheats.GetSettlement(CampaignCheats.ConcatenateString(strings.GetRange(0, strings.Count)));
			if (settlement == null)
			{
				return "Settlement is not found";
			}
			settlement.Militia = 0f;
			MobileParty mobileParty = (settlement.IsFortification ? settlement.Town.GarrisonParty : null);
			if (mobileParty != null)
			{
				DestroyPartyAction.Apply(null, mobileParty);
			}
			return "Success";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("print_party_prisoners", "campaign")]
		public static string PrintPartyPrisoners(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.print_party_prisoners [PartyName]\".";
			}
			string text = CampaignCheats.ConcatenateString(strings);
			foreach (MobileParty mobileParty in MobileParty.All)
			{
				if (string.Equals(text, mobileParty.Name.ToString(), StringComparison.OrdinalIgnoreCase))
				{
					string text2 = "";
					foreach (TroopRosterElement troopRosterElement in mobileParty.PrisonRoster.GetTroopRoster())
					{
						text2 = string.Concat(new object[]
						{
							text2,
							troopRosterElement.Character.Name,
							" count: ",
							mobileParty.PrisonRoster.GetTroopCount(troopRosterElement.Character),
							"\n"
						});
					}
					if (string.IsNullOrEmpty(text2))
					{
						return "There is not any prisoners in the party right now.";
					}
					return text2;
				}
			}
			return "Party is not found: " + text;
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("give_prisoners_xp", "campaign")]
		public static string GivePrisonersXp(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (!CampaignCheats.CheckParameters(strings, 1) || CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.give_prisoners_xp [Amount]\".";
			}
			int num = 1;
			if (!int.TryParse(strings[0], out num) && num > 0)
			{
				return "Please enter a positive number";
			}
			for (int i = 0; i < MobileParty.MainParty.PrisonRoster.Count; i++)
			{
				MobileParty.MainParty.PrisonRoster.SetElementXp(i, MobileParty.MainParty.PrisonRoster.GetElementXp(i) + num);
				InformationManager.DisplayMessage(new InformationMessage(string.Concat(new object[]
				{
					"[DEBUG] ",
					num,
					" xp given to ",
					MobileParty.MainParty.PrisonRoster.GetElementCopyAtIndex(i).Character.Name
				})));
			}
			return "Success";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("set_settlement_variable", "campaign")]
		public static string SetSettlementVariable(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckParameters(strings, 1) || CampaignCheats.CheckParameters(strings, 2) || CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.set_settlement_variable [SettlementName/SettlementID] [VariableName] [Value]\". Available variables:\nProsperity\nSecurity\nFood\nLoyalty\nMilitia\nHearth";
			}
			string text = strings[strings.Count - 2].ToLower();
			string text2 = strings[strings.Count - 1].ToLower();
			strings.RemoveAt(strings.Count - 1);
			strings.RemoveAt(strings.Count - 1);
			string settlementName = CampaignCheats.ConcatenateString(strings).ToLower();
			Settlement settlement = Settlement.FindFirst((Settlement x) => string.Compare(x.Name.ToString(), settlementName, StringComparison.OrdinalIgnoreCase) == 0);
			if (settlement == null)
			{
				return "Settlement is not found: " + settlementName;
			}
			if ((!settlement.IsVillage || (!text.Equals("hearth") && !text.Equals("militia") && !text.Equals("prosperity"))) && (settlement.IsVillage || text.Equals("hearth")))
			{
				return "Settlement don't have variable: " + text;
			}
			float num = -333f;
			if (!float.TryParse(text2, out num))
			{
				return "Value must be numerical!";
			}
			if (!(text == "prosperity"))
			{
				if (!(text == "militia"))
				{
					if (!(text == "hearth"))
					{
						if (!(text == "security"))
						{
							if (!(text == "loyalty"))
							{
								if (!(text == "food"))
								{
									return "Invalid variable: " + text;
								}
								settlement.Town.FoodStocks = num;
							}
							else
							{
								settlement.Town.Loyalty = num;
							}
						}
						else
						{
							settlement.Town.Security = num;
						}
					}
					else
					{
						settlement.Village.Hearth = num;
					}
				}
				else
				{
					settlement.Militia = num;
				}
			}
			else
			{
				settlement.Prosperity = num;
			}
			return "Success";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("set_hero_trait", "campaign")]
		public static string SetHeroTrait(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.set_hero_trait [HeroName] | [Trait] | [Value]\".";
			}
			strings = CampaignCheats.GetSeparatedNames(strings, "|");
			if (strings.Count < 3)
			{
				return "Format is \"campaign.set_hero_trait [HeroName] | [Trait] | [Value]\".";
			}
			int num;
			if (!int.TryParse(strings[2], out num))
			{
				return "Level must be a number";
			}
			Hero hero = CampaignCheats.GetHero(strings[0]);
			if (hero != null)
			{
				int num2;
				if (int.TryParse(strings[2], out num2))
				{
					string text = strings[1];
					foreach (TraitObject traitObject in TraitObject.All)
					{
						if (traitObject.Name.ToString().Equals(text, StringComparison.InvariantCultureIgnoreCase) || traitObject.StringId == text)
						{
							int traitLevel = hero.GetTraitLevel(traitObject);
							if (num2 >= traitObject.MinValue && num2 <= traitObject.MaxValue)
							{
								hero.SetTraitLevel(traitObject, num2);
								if (hero == Hero.MainHero)
								{
									Campaign.Current.PlayerTraitDeveloper.UpdateTraitXPAccordingToTraitLevels();
									CampaignEventDispatcher.Instance.OnPlayerTraitChanged(traitObject, traitLevel);
								}
								return string.Format("{0} 's {1} trait has been set to {2}.", strings[0], traitObject.Name, num2);
							}
							return string.Format("Number must be between {0} and {1}.", traitObject.MinValue, traitObject.MaxValue);
						}
					}
				}
				return "Trait not found";
			}
			return "Hero: " + strings[0] + " not found.";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("list_hero_traits", "campaign")]
		public static string ListHeroTraits(List<string> heroName)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			Hero hero = CampaignCheats.GetHero(CampaignCheats.ConcatenateString(heroName));
			if (hero != null)
			{
				string text = null;
				foreach (TraitObject traitObject in TraitObject.All)
				{
					text = string.Concat(new object[]
					{
						text,
						traitObject.Name,
						hero.GetTraitLevel(traitObject).ToString(),
						"\n"
					});
				}
				return text;
			}
			return string.Format("Hero: {0} not found.", heroName);
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("remove_militas_from_settlement", "campaign")]
		public static string RemoveMilitiasFromSettlement(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.remove_militas_from_settlement [SettlementName]\".";
			}
			string concatenated = CampaignCheats.ConcatenateString(strings);
			Settlement settlement = Settlement.FindFirst((Settlement x) => string.Compare(x.Name.ToString(), concatenated, StringComparison.OrdinalIgnoreCase) == 0);
			if (settlement == null)
			{
				return "Settlement is not found: " + concatenated;
			}
			if (settlement.Party.MapEvent != null)
			{
				return "Settlement, " + concatenated + " is in a MapEvent, try later to remove them";
			}
			List<MobileParty> list = new List<MobileParty>();
			IEnumerable<MobileParty> all = MobileParty.All;
			Func<MobileParty, bool> <>9__1;
			Func<MobileParty, bool> func;
			if ((func = <>9__1) == null)
			{
				func = (<>9__1 = (MobileParty x) => x.IsMilitia && x.CurrentSettlement == settlement);
			}
			foreach (MobileParty mobileParty in all.Where(func))
			{
				if (mobileParty.MapEvent != null)
				{
					return "Milita in " + concatenated + " are in a MapEvent, try later to remove them";
				}
				list.Add(mobileParty);
			}
			foreach (MobileParty mobileParty2 in list)
			{
				mobileParty2.RemoveParty();
			}
			return "Success";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("cancel_quest", "campaign")]
		public static string CancelQuestCheat(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckHelp(strings))
			{
				return "Format is campaign.complete_quest [quest name]";
			}
			string text = "";
			for (int i = 0; i < strings.Count; i++)
			{
				text += strings[i];
				if (i + 1 != strings.Count)
				{
					text += " ";
				}
			}
			if (text.IsEmpty<char>())
			{
				return "Format is campaign.complete_quest [quest name]";
			}
			QuestBase questBase = null;
			int num = 0;
			foreach (QuestBase questBase2 in Campaign.Current.QuestManager.Quests)
			{
				if (text.Equals(questBase2.Title.ToString(), StringComparison.OrdinalIgnoreCase))
				{
					num++;
					if (num == 1)
					{
						questBase = questBase2;
					}
				}
			}
			if (questBase == null)
			{
				return "Quest not found.";
			}
			if (num > 1)
			{
				return "There are more than one quest with the name: " + text;
			}
			questBase.CompleteQuestWithCancel(new TextObject("{=!}Quest is canceled by cheat.", null));
			return "Success";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("kick_companion", "campaign")]
		public static string KickAllCompanionsFromParty(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.kick_companion [CompanionName] or [all](kicks all companions) or [noargument](kicks first companion if any) \".";
			}
			IEnumerable<TroopRosterElement> enumerable = from h in MobileParty.MainParty.MemberRoster.GetTroopRoster()
				where h.Character != null && h.Character.IsHero && h.Character.HeroObject.IsWanderer
				select h;
			if (enumerable.IsEmpty<TroopRosterElement>())
			{
				return "There are no companions in your party.";
			}
			if (strings.IsEmpty<string>())
			{
				RemoveCompanionAction.ApplyByFire(Clan.PlayerClan, enumerable.First<TroopRosterElement>().Character.HeroObject);
				return "Success";
			}
			if (strings[0].ToLower() == "all")
			{
				foreach (TroopRosterElement troopRosterElement in enumerable)
				{
					RemoveCompanionAction.ApplyByFire(Clan.PlayerClan, troopRosterElement.Character.HeroObject);
				}
				return "Success";
			}
			foreach (TroopRosterElement troopRosterElement2 in enumerable)
			{
				if (troopRosterElement2.Character.Name.ToString().ToLower().Contains(strings[0].ToLower()))
				{
					RemoveCompanionAction.ApplyByFire(Clan.PlayerClan, troopRosterElement2.Character.HeroObject);
					return "Success";
				}
			}
			return "No companion named: " + strings[0] + " is found";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("give_money_to_main_party", "campaign")]
		public static string GiveMoneyToMainParty(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (!CampaignCheats.CheckParameters(strings, 1) || CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.give_money_to_main_party [Amount]\".";
			}
			int num;
			if (int.TryParse(strings[0], out num) && num > 0)
			{
				GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, num, false);
				return "Main hero gained " + num + " gold.";
			}
			return "Please enter a positive number";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("give_troops_xp", "campaign")]
		public static string GiveTroopsXp(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (!CampaignCheats.CheckParameters(strings, 1) || CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.give_troops_xp [Amount]\".";
			}
			int num = 1;
			if (!int.TryParse(strings[0], out num) && num > 0)
			{
				return "Please enter a positive number";
			}
			for (int i = 0; i < MobileParty.MainParty.MemberRoster.Count; i++)
			{
				MobileParty.MainParty.MemberRoster.SetElementXp(i, MobileParty.MainParty.MemberRoster.GetElementXp(i) + num);
				InformationManager.DisplayMessage(new InformationMessage(string.Concat(new object[]
				{
					"[DEBUG] ",
					num,
					" xp given to ",
					MobileParty.MainParty.MemberRoster.GetElementCopyAtIndex(i).Character.Name
				})));
			}
			return "Success";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("find_issue", "campaign")]
		public static string FindIssues(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.find_issue [IssueName]\".";
			}
			string text = CampaignCheats.ConcatenateString(strings);
			IssueBase issueBase = null;
			foreach (KeyValuePair<Hero, IssueBase> keyValuePair in Campaign.Current.IssueManager.Issues)
			{
				if (keyValuePair.Value.Title.ToString().ToLower().Contains(text))
				{
					if (keyValuePair.Value.IssueSettlement != null)
					{
						issueBase = keyValuePair.Value;
						keyValuePair.Value.IssueSettlement.Party.SetAsCameraFollowParty();
					}
					else if (keyValuePair.Value.IssueOwner.PartyBelongedTo != null)
					{
						issueBase = keyValuePair.Value;
						MobileParty partyBelongedTo = keyValuePair.Value.IssueOwner.PartyBelongedTo;
						if (partyBelongedTo != null)
						{
							partyBelongedTo.Party.SetAsCameraFollowParty();
						}
					}
					else if (keyValuePair.Value.IssueOwner.CurrentSettlement != null)
					{
						issueBase = keyValuePair.Value;
						keyValuePair.Value.IssueOwner.CurrentSettlement.Party.SetAsCameraFollowParty();
					}
					if (issueBase != null)
					{
						return "Found issue: " + issueBase.Title.ToString() + ". Issue Owner: " + issueBase.IssueOwner.Name.ToString();
					}
				}
			}
			return "Issue Not Found";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("toggle_information_restrictions", "campaign")]
		public static string ToggleInformationRestrictions(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.toggle_information_restrictions\".";
			}
			DefaultInformationRestrictionModel defaultInformationRestrictionModel = Campaign.Current.Models.InformationRestrictionModel as DefaultInformationRestrictionModel;
			if (defaultInformationRestrictionModel == null)
			{
				return "DefaultInformationRestrictionModel is missing.";
			}
			defaultInformationRestrictionModel.IsDisabledByCheat = !defaultInformationRestrictionModel.IsDisabledByCheat;
			return "Information restrictions are " + (defaultInformationRestrictionModel.IsDisabledByCheat ? "disabled" : "enabled") + ".";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("get_random_hero_with_parameters", "campaign")]
		public static string GetRandomHeroWithFeatures(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			string text = "Format is \"campaign.get_random_hero_with_parameters [accent_class] [persona] [gender(optional)]\".\n accent_class: t for tribal, l for imperial low, h for imperial high\n persona: s for softspoken, c for curt, i for ironic, e for earnest\n gender: f for female, m for male";
			if (CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckHelp(strings))
			{
				return text;
			}
			if (strings.Count != 2 && strings.Count != 3)
			{
				return text;
			}
			Func<CharacterObject, bool> func;
			if (strings[0].Equals("t"))
			{
				func = new Func<CharacterObject, bool>(ConversationTagHelper.TribalVoiceGroup);
			}
			else if (strings[0].Equals("l"))
			{
				func = new Func<CharacterObject, bool>(ConversationTagHelper.UsesLowRegister);
			}
			else
			{
				if (!strings[0].Equals("i"))
				{
					return text;
				}
				func = new Func<CharacterObject, bool>(ConversationTagHelper.UsesHighRegister);
			}
			TraitObject traitObject;
			if (strings[1].Equals("s"))
			{
				traitObject = DefaultTraits.PersonaSoftspoken;
			}
			else if (strings[1].Equals("c"))
			{
				traitObject = DefaultTraits.PersonaCurt;
			}
			else if (strings[1].Equals("i"))
			{
				traitObject = DefaultTraits.PersonaIronic;
			}
			else
			{
				if (!strings[1].Equals("e"))
				{
					return text;
				}
				traitObject = DefaultTraits.PersonaEarnest;
			}
			bool flag = strings.Count == 3 && (strings[2].Equals("f") || strings[2].Equals("m"));
			bool flag2 = false;
			if (flag)
			{
				flag2 = strings[2].Equals("f");
			}
			List<Hero> list = new List<Hero>();
			foreach (Hero hero in Hero.AllAliveHeroes)
			{
				if ((hero.IsActive || hero.IsPrisoner) && func(hero.CharacterObject) && hero.CharacterObject.GetPersona() == traitObject)
				{
					if (flag)
					{
						if (hero.IsFemale == flag2)
						{
							list.Add(hero);
						}
					}
					else
					{
						list.Add(hero);
					}
				}
			}
			if (list.IsEmpty<Hero>())
			{
				return "Could not find a proper hero.";
			}
			Hero randomElement = list.GetRandomElement<Hero>();
			CampaignCheats.FindHero(new List<string> { randomElement.Name.ToString() });
			return string.Concat(new object[]
			{
				"Name: ",
				randomElement.Name,
				"\nCulture: ",
				randomElement.Culture.Name,
				"\nPersona: ",
				randomElement.CharacterObject.GetPersona().Name
			});
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("set_campaign_speed_multiplier", "campaign")]
		public static string SetCampaignSpeed(List<string> strings)
		{
			string text = "Format is \"campaign.set_campaign_speed_multiplier  [positive speedUp multiplier]\".";
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (!CampaignCheats.CheckParameters(strings, 1) || CampaignCheats.CheckHelp(strings))
			{
				return text;
			}
			float num;
			if (!float.TryParse(strings[0], out num) || num <= 0f)
			{
				return text;
			}
			if (num <= 15f)
			{
				Campaign.Current.SpeedUpMultiplier = num;
				return "Success";
			}
			Campaign.Current.SpeedUpMultiplier = 15f;
			return "Campaign speed is set to " + 15f + ". which is the maximum value for speed up multiplier!";
		}

		public static Hero GetHero(string heroName)
		{
			foreach (Hero hero in Hero.AllAliveHeroes)
			{
				if (string.Equals(hero.Name.ToString(), heroName, StringComparison.OrdinalIgnoreCase))
				{
					return hero;
				}
			}
			foreach (Hero hero2 in Hero.DeadOrDisabledHeroes)
			{
				if (string.Equals(hero2.Name.ToString(), heroName, StringComparison.OrdinalIgnoreCase))
				{
					return hero2;
				}
			}
			return null;
		}

		public static Clan GetClan(string clanName)
		{
			foreach (Clan clan in Clan.NonBanditFactions)
			{
				if (string.Equals(clan.Name.ToString(), clanName, StringComparison.OrdinalIgnoreCase))
				{
					return clan;
				}
			}
			return null;
		}

		public static Hero GetClanLeader(string clanName)
		{
			foreach (Clan clan in Clan.NonBanditFactions)
			{
				if (string.Equals(clan.Name.ToString(), clanName, StringComparison.OrdinalIgnoreCase))
				{
					return clan.Leader;
				}
			}
			return null;
		}

		public static Kingdom GetKingdom(string kingdomName)
		{
			foreach (Kingdom kingdom in Kingdom.All)
			{
				if (string.Equals(kingdom.Name.ToString(), kingdomName, StringComparison.OrdinalIgnoreCase))
				{
					return kingdom;
				}
			}
			return null;
		}

		public static Settlement GetSettlement(string settlementName)
		{
			foreach (Settlement settlement in Settlement.All)
			{
				if (string.Equals(settlement.Name.ToString(), settlementName, StringComparison.OrdinalIgnoreCase) || string.Equals(settlement.Name.ToString().Replace(" ", ""), settlementName, StringComparison.OrdinalIgnoreCase))
				{
					return settlement;
				}
			}
			return null;
		}

		public static Settlement GetDefaultSettlement()
		{
			Settlement settlement2 = Hero.MainHero.HomeSettlement;
			if (settlement2 == null)
			{
				settlement2 = Kingdom.All.GetRandomElement<Kingdom>().Settlements.GetRandomElementWithPredicate((Settlement settlement) => settlement.IsTown);
			}
			return settlement2;
		}

		public static string ConcatenateString(List<string> strings)
		{
			if (strings == null || strings.IsEmpty<string>())
			{
				return string.Empty;
			}
			string text = strings[0];
			if (strings.Count > 1)
			{
				for (int i = 1; i < strings.Count; i++)
				{
					text = text + " " + strings[i];
				}
			}
			return text;
		}

		public const string CampaignNotStarted = "Campaign was not started.";

		public const string CheatmodeDisabled = "Cheat mode is disabled!";

		public const string AchievementsAreDisabled = "Achievements are disabled!";

		public const string Help = "help";

		public const string EnterNumber = "Please enter a number";

		public const string EnterPositiveNumber = "Please enter a positive number";

		public const string SettlementNotFound = "Settlement is not found";

		public const string HeroNotFound = "Hero is not found";

		public const string KingdomNotFound = "Kingdom is not found";

		public const string VillageNotFound = "Village is not found";

		public const string FactionNotFound = "Faction is not found";

		public const string PartyNotFound = "Party is not found";

		public const string OK = "Success";

		public const string CheatNameSeparator = "|";

		public const string AiDisabledHelper = "Party AI can be enabled again by using \"campaign.control_party_ai_by_cheats\"";

		public static string ErrorType = "";

		public const int MaxSkillValue = 300;

		private static bool _mainPartyIsAttackable = true;
	}
}
