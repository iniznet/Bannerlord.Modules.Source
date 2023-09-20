using System;
using StoryMode.Quests.PlayerClanQuests;
using StoryMode.StoryModeObjects;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;

namespace StoryMode.GameComponents.CampaignBehaviors
{
	// Token: 0x0200004D RID: 77
	public class MainStorylineCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x06000431 RID: 1073 RVA: 0x000195E4 File Offset: 0x000177E4
		public override void RegisterEvents()
		{
			CampaignEvents.CanHeroDieEvent.AddNonSerializedListener(this, new ReferenceAction<Hero, KillCharacterAction.KillCharacterActionDetail, bool>(this.CanHeroDie));
			CampaignEvents.ClanChangedKingdom.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, ChangeKingdomAction.ChangeKingdomActionDetail, bool>(this.OnClanChangedKingdom));
			CampaignEvents.OnGameLoadFinishedEvent.AddNonSerializedListener(this, new Action(this.OnGameLoadFinished));
		}

		// Token: 0x06000432 RID: 1074 RVA: 0x00019638 File Offset: 0x00017838
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

		// Token: 0x06000433 RID: 1075 RVA: 0x000196F3 File Offset: 0x000178F3
		private void OnClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification = true)
		{
			if (clan == Clan.PlayerClan && newKingdom != null && (detail == 7 || detail == 1))
			{
				Clan.PlayerClan.IsNoble = true;
			}
		}

		// Token: 0x06000434 RID: 1076 RVA: 0x00019715 File Offset: 0x00017915
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x06000435 RID: 1077 RVA: 0x00019718 File Offset: 0x00017918
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
