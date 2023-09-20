using System;
using System.Collections.Generic;
using System.Linq;
using StoryMode.Quests.SecondPhase;
using StoryMode.Quests.SecondPhase.ConspiracyQuests;
using StoryMode.StoryModeObjects;
using StoryMode.StoryModePhases;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace StoryMode
{
	public static class StoryModeCheats
	{
		public static bool CheckGameMode(out string message)
		{
			message = string.Empty;
			if (StoryModeManager.Current != null)
			{
				return false;
			}
			message = "Game mode is not correct!";
			return true;
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("activate_conspiracy_quest", "storymode")]
		public static string ActivateConspiracyQuest(List<string> strings)
		{
			string text;
			if (StoryModeCheats.CheckGameMode(out text))
			{
				return text;
			}
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (!CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"storymode.activate_conspiracy_quest\".";
			}
			if (StoryModeManager.Current.MainStoryLine.PlayerSupportedKingdom == null)
			{
				return " Player supported kingdom doesn't exist.";
			}
			foreach (QuestBase questBase in Campaign.Current.QuestManager.Quests.Where((QuestBase t) => t is WeakenEmpireQuestBehavior.WeakenEmpireQuest || t is AssembleEmpireQuestBehavior.AssembleEmpireQuest).ToList<QuestBase>())
			{
				questBase.CompleteQuestWithCancel(null);
			}
			StoryModeManager.Current.MainStoryLine.CompleteSecondPhase();
			return "Success";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("add_family_members", "storymode")]
		public static string AddFamilyMembers(List<string> strings)
		{
			string text;
			if (StoryModeCheats.CheckGameMode(out text))
			{
				return text;
			}
			foreach (Hero hero in new List<Hero>
			{
				StoryModeHeroes.LittleBrother,
				StoryModeHeroes.ElderBrother,
				StoryModeHeroes.LittleSister
			})
			{
				AddHeroToPartyAction.Apply(hero, MobileParty.MainParty, true);
				hero.Clan = Clan.PlayerClan;
			}
			return "Success";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("weaken_kingdom", "storymode")]
		public static string WeakenKingdom(List<string> strings)
		{
			string text;
			if (StoryModeCheats.CheckGameMode(out text))
			{
				return text;
			}
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			string text2 = "Format is \"storymode.weaken_kingdom [KingdomName]\".";
			if (CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckHelp(strings))
			{
				return text2;
			}
			string text3 = CampaignCheats.ConcatenateString(strings);
			Kingdom kingdom = null;
			foreach (Kingdom kingdom2 in Kingdom.All)
			{
				if (kingdom2.Name.ToString().Replace(" ", "").Equals(text3.Replace(" ", ""), StringComparison.OrdinalIgnoreCase))
				{
					kingdom = kingdom2;
					break;
				}
				if (text3.Length >= 2 && kingdom2.Name.ToString().ToLower().Substring(0, 2)
					.Equals(text3.ToLower().Substring(0, 2)))
				{
					kingdom = kingdom2;
					break;
				}
			}
			if (kingdom != null)
			{
				foreach (Settlement settlement in kingdom.Settlements.Where((Settlement t) => t.IsTown || t.IsCastle).Take(3).ToList<Settlement>())
				{
					ChangeOwnerOfSettlementAction.ApplyByDefault(Hero.MainHero, settlement);
				}
				foreach (MobileParty mobileParty in kingdom.AllParties.Where((MobileParty t) => t.MapEvent == null))
				{
					foreach (TroopRosterElement troopRosterElement in mobileParty.MemberRoster.GetTroopRoster())
					{
						mobileParty.MemberRoster.RemoveTroop(troopRosterElement.Character, mobileParty.MemberRoster.GetTroopCount(troopRosterElement.Character) / 2, default(UniqueTroopDescriptor), 0);
					}
				}
				return "Success";
			}
			return "Kingdom is not found\n" + text2;
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("reinforce_kingdom", "storymode")]
		public static string ReinforceKingdom(List<string> strings)
		{
			string text;
			if (StoryModeCheats.CheckGameMode(out text))
			{
				return text;
			}
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			string text2 = "Format is \"storymode.reinforce_kingdom [KingdomName]\".";
			if (CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckHelp(strings))
			{
				return text2;
			}
			string text3 = CampaignCheats.ConcatenateString(strings);
			Kingdom kingdom = null;
			foreach (Kingdom kingdom2 in Kingdom.All)
			{
				if (kingdom2.Name.ToString().Replace(" ", "").Equals(text3.Replace(" ", ""), StringComparison.OrdinalIgnoreCase))
				{
					kingdom = kingdom2;
					break;
				}
			}
			if (kingdom != null)
			{
				IEnumerable<Settlement> all = Settlement.All;
				Func<Settlement, bool> <>9__0;
				Func<Settlement, bool> func;
				if ((func = <>9__0) == null)
				{
					func = (<>9__0 = (Settlement t) => (t.IsTown || t.IsCastle) && t.MapFaction != kingdom);
				}
				foreach (Settlement settlement in all.Where(func).Take(3).ToList<Settlement>())
				{
					ChangeOwnerOfSettlementAction.ApplyByDefault(kingdom.Leader, settlement);
				}
				foreach (MobileParty mobileParty in kingdom.AllParties.Where((MobileParty t) => t.MapEvent == null))
				{
					foreach (TroopRosterElement troopRosterElement in mobileParty.MemberRoster.GetTroopRoster())
					{
						mobileParty.MemberRoster.AddToCounts(troopRosterElement.Character, 200, false, 0, 0, true, -1);
					}
				}
				return "Success";
			}
			return "Kingdom is not found\n" + text2;
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("start_conspiracy_quest_destroy_raiders", "storymode")]
		public static string StartDestroyRaidersConspiracyQuest(List<string> strings)
		{
			string text = "";
			if (!CampaignCheats.CheckCheatUsage(ref text))
			{
				return text;
			}
			new DestroyRaidersConspiracyQuest("cheat_quest", StoryModeHeroes.ImperialMentor).StartQuest();
			return "Success";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("start_next_second_phase_quest", "storymode")]
		public static string SecondPhaseStartNextQuest(List<string> strings)
		{
			string text = "";
			if (!CampaignCheats.CheckCheatUsage(ref text))
			{
				return text;
			}
			if (SecondPhase.Instance != null)
			{
				SecondPhase.Instance.CreateNextConspiracyQuest();
				return "Success";
			}
			return "Second phase not found.";
		}
	}
}
