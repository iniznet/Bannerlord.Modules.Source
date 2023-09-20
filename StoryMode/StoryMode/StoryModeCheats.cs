using System;
using System.Collections.Generic;
using System.Linq;
using StoryMode.Quests.SecondPhase;
using StoryMode.StoryModeObjects;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace StoryMode
{
	// Token: 0x02000010 RID: 16
	public static class StoryModeCheats
	{
		// Token: 0x0600006F RID: 111 RVA: 0x00003F19 File Offset: 0x00002119
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

		// Token: 0x06000070 RID: 112 RVA: 0x00003F34 File Offset: 0x00002134
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
			if (Clan.PlayerClan.Kingdom != null)
			{
				foreach (QuestBase questBase in Campaign.Current.QuestManager.Quests.Where((QuestBase t) => t is WeakenEmpireQuestBehavior.WeakenEmpireQuest || t is AssembleEmpireQuestBehavior.AssembleEmpireQuest).ToList<QuestBase>())
				{
					questBase.CompleteQuestWithCancel(null);
				}
				StoryModeManager.Current.MainStoryLine.CompleteSecondPhase();
				return "success";
			}
			return "Player is not in a kingdom";
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00004014 File Offset: 0x00002214
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

		// Token: 0x06000072 RID: 114 RVA: 0x000040A8 File Offset: 0x000022A8
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
			if (CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"storymode.weaken_kingdom [KingdomName]\".";
			}
			string text2 = CampaignCheats.ConcatenateString(strings);
			Kingdom kingdom = null;
			foreach (Kingdom kingdom2 in Kingdom.All)
			{
				if (kingdom2.Name.ToString().Equals(text2, StringComparison.OrdinalIgnoreCase))
				{
					kingdom = kingdom2;
					break;
				}
				if (text2.Length >= 2 && kingdom2.Name.ToString().ToLower().Substring(0, 2)
					.Equals(text2.ToLower().Substring(0, 2)))
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
				return "success";
			}
			return "Cant find kingdom";
		}

		// Token: 0x06000073 RID: 115 RVA: 0x000042E4 File Offset: 0x000024E4
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
			if (CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"storymode.reinforce_kingdom [KingdomName]\".";
			}
			string text2 = CampaignCheats.ConcatenateString(strings);
			Kingdom kingdom = null;
			foreach (Kingdom kingdom2 in Kingdom.All)
			{
				if (kingdom2.Name.ToString().Equals(text2, StringComparison.OrdinalIgnoreCase))
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
				return "success";
			}
			return "Cant find kingdom";
		}
	}
}
