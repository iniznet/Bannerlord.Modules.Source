using System;
using StoryMode.Quests.PlayerClanQuests;
using StoryMode.StoryModeObjects;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;

namespace StoryMode.GameComponents.CampaignBehaviors
{
	public class MainStorylineCampaignBehavior : CampaignBehaviorBase
	{
		public override void RegisterEvents()
		{
			CampaignEvents.CanHeroDieEvent.AddNonSerializedListener(this, new ReferenceAction<Hero, KillCharacterAction.KillCharacterActionDetail, bool>(this.CanHeroDie));
			CampaignEvents.ClanChangedKingdom.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, ChangeKingdomAction.ChangeKingdomActionDetail, bool>(this.OnClanChangedKingdom));
			CampaignEvents.OnGameLoadFinishedEvent.AddNonSerializedListener(this, new Action(this.OnGameLoadFinished));
		}

		private void OnGameLoadFinished()
		{
			if (Clan.PlayerClan.Kingdom != null && !Clan.PlayerClan.IsUnderMercenaryService)
			{
				Clan.PlayerClan.IsNoble = true;
			}
			int heroComesOfAge = Campaign.Current.Models.AgeModel.HeroComesOfAge;
			if (StoryModeHeroes.LittleSister.Age < (float)heroComesOfAge && !StoryModeHeroes.LittleSister.IsDisabled && !StoryModeHeroes.LittleSister.IsNotSpawned)
			{
				DisableHeroAction.Apply(StoryModeHeroes.LittleSister);
				StoryModeHeroes.LittleSister.ChangeState(0);
			}
			if (StoryModeHeroes.LittleBrother.Age < (float)heroComesOfAge && !StoryModeHeroes.LittleBrother.IsDisabled && !StoryModeHeroes.LittleBrother.IsNotSpawned)
			{
				DisableHeroAction.Apply(StoryModeHeroes.LittleBrother);
				StoryModeHeroes.LittleBrother.ChangeState(0);
			}
		}

		private void OnClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification = true)
		{
			if (clan == Clan.PlayerClan && newKingdom != null && (detail == 7 || detail == 1))
			{
				Clan.PlayerClan.IsNoble = true;
			}
		}

		public override void SyncData(IDataStore dataStore)
		{
		}

		private void CanHeroDie(Hero hero, KillCharacterAction.KillCharacterActionDetail causeOfDeath, ref bool result)
		{
			if (hero == StoryModeHeroes.Radagos && StoryModeManager.Current.MainStoryLine.TutorialPhase.IsCompleted && !Campaign.Current.QuestManager.IsThereActiveQuestWithType(typeof(RescueFamilyQuestBehavior.RescueFamilyQuest)) && !Campaign.Current.QuestManager.IsThereActiveQuestWithType(typeof(RebuildPlayerClanQuest)) && causeOfDeath == 6)
			{
				result = true;
				return;
			}
			if (hero.IsSpecial && hero != StoryModeHeroes.RadagosHencman && !StoryModeManager.Current.MainStoryLine.IsCompleted)
			{
				result = false;
			}
		}
	}
}
