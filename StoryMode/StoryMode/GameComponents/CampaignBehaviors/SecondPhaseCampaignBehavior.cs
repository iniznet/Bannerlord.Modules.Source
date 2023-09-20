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
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace StoryMode.GameComponents.CampaignBehaviors
{
	// Token: 0x0200004E RID: 78
	public class SecondPhaseCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x06000437 RID: 1079 RVA: 0x000197AD File Offset: 0x000179AD
		public SecondPhaseCampaignBehavior()
		{
			this._conspiracyQuestTriggerDayCounter = 0;
			this._isConspiracySetUpStarted = false;
		}

		// Token: 0x06000438 RID: 1080 RVA: 0x000197C4 File Offset: 0x000179C4
		public override void RegisterEvents()
		{
			CampaignEvents.WeeklyTickEvent.AddNonSerializedListener(this, new Action(this.WeeklyTick));
			CampaignEvents.OnQuestStartedEvent.AddNonSerializedListener(this, new Action<QuestBase>(this.OnQuestStarted));
			CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.DailyTick));
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
			CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnGameLoaded));
			StoryModeEvents.OnConspiracyActivatedEvent.AddNonSerializedListener(this, new Action(this.OnConspiracyActivated));
		}

		// Token: 0x06000439 RID: 1081 RVA: 0x0001985B File Offset: 0x00017A5B
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<int>("_conspiracyQuestTriggerDayCounter", ref this._conspiracyQuestTriggerDayCounter);
			dataStore.SyncData<bool>("_isConspiracySetUpStarted", ref this._isConspiracySetUpStarted);
		}

		// Token: 0x0600043A RID: 1082 RVA: 0x00019884 File Offset: 0x00017A84
		private void WeeklyTick()
		{
			int num = 14;
			SecondPhase instance = SecondPhase.Instance;
			int num2 = num + MBRandom.RandomIntWithSeed((uint)((instance != null) ? instance.LastConspiracyQuestCreationTime.ToMilliseconds : 53.0), 2000U) % 8;
			if (this._isConspiracySetUpStarted && StoryModeManager.Current.MainStoryLine.ThirdPhase == null && SecondPhase.Instance.ConspiracyStrength < 2000f && SecondPhase.Instance.LastConspiracyQuestCreationTime.ElapsedDaysUntilNow >= (float)num2 && !this.IsThereActiveConspiracyQuest())
			{
				SecondPhase.Instance.CreateNextConspiracyQuest();
			}
		}

		// Token: 0x0600043B RID: 1083 RVA: 0x00019915 File Offset: 0x00017B15
		private void OnQuestStarted(QuestBase quest)
		{
			if (quest is AssembleEmpireQuestBehavior.AssembleEmpireQuest || quest is WeakenEmpireQuestBehavior.WeakenEmpireQuest)
			{
				StoryModeManager.Current.MainStoryLine.CompleteFirstPhase();
				this._isConspiracySetUpStarted = true;
			}
		}

		// Token: 0x0600043C RID: 1084 RVA: 0x0001993D File Offset: 0x00017B3D
		private void DailyTick()
		{
			if (this._isConspiracySetUpStarted && this._conspiracyQuestTriggerDayCounter < 10)
			{
				this._conspiracyQuestTriggerDayCounter++;
				if (this._conspiracyQuestTriggerDayCounter >= 10)
				{
					new ConspiracyProgressQuest().StartQuest();
				}
			}
		}

		// Token: 0x0600043D RID: 1085 RVA: 0x00019973 File Offset: 0x00017B73
		private void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
		{
			SecondPhase instance = SecondPhase.Instance;
			if (instance == null)
			{
				return;
			}
			instance.OnSessionLaunched();
		}

		// Token: 0x0600043E RID: 1086 RVA: 0x00019984 File Offset: 0x00017B84
		private void OnGameLoaded(CampaignGameStarter campaignGameStarter)
		{
			foreach (MobileParty mobileParty in Campaign.Current.CustomParties.ToList<MobileParty>())
			{
				if (mobileParty.Name.HasSameValue(new TextObject("{=eVzg5Mtl}Conspiracy Caravan", null)))
				{
					bool flag = true;
					foreach (QuestBase questBase in Campaign.Current.QuestManager.Quests)
					{
						if (questBase.GetType() == typeof(DisruptSupplyLinesConspiracyQuest) && ((DisruptSupplyLinesConspiracyQuest)questBase).ConspiracyCaravan == mobileParty)
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						DestroyPartyAction.Apply(null, mobileParty);
					}
				}
			}
		}

		// Token: 0x0600043F RID: 1087 RVA: 0x00019A74 File Offset: 0x00017C74
		private void OnConspiracyActivated()
		{
			CampaignEventDispatcher.Instance.RemoveListeners(this);
		}

		// Token: 0x06000440 RID: 1088 RVA: 0x00019A84 File Offset: 0x00017C84
		private bool IsThereActiveConspiracyQuest()
		{
			foreach (QuestBase questBase in Campaign.Current.QuestManager.Quests)
			{
				if (questBase.IsOngoing && typeof(ConspiracyQuestBase) == questBase.GetType().BaseType)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000441 RID: 1089 RVA: 0x00019B04 File Offset: 0x00017D04
		[CommandLineFunctionality.CommandLineArgumentFunction("start_conspiracy_quest_destroy_raiders", "storymode")]
		public static string StartDestroyRaidersConspiracyQuest(List<string> strings)
		{
			string text;
			if (StoryModeCheats.CheckGameMode(out text))
			{
				return text;
			}
			string text2 = "";
			if (!CampaignCheats.CheckCheatUsage(ref text2))
			{
				return text2;
			}
			new DestroyRaidersConspiracyQuest("cheat_quest", StoryModeHeroes.ImperialMentor).StartQuest();
			return "success";
		}

		// Token: 0x06000442 RID: 1090 RVA: 0x00019B48 File Offset: 0x00017D48
		[CommandLineFunctionality.CommandLineArgumentFunction("start_next_second_phase_quest", "storymode")]
		public static string SecondPhaseStartNextQuest(List<string> strings)
		{
			string text;
			if (StoryModeCheats.CheckGameMode(out text))
			{
				return text;
			}
			string text2 = "";
			if (!CampaignCheats.CheckCheatUsage(ref text2))
			{
				return text2;
			}
			if (SecondPhase.Instance != null)
			{
				SecondPhase.Instance.CreateNextConspiracyQuest();
				return "success";
			}
			return "Second phase not found";
		}

		// Token: 0x040001BE RID: 446
		private int _conspiracyQuestTriggerDayCounter;

		// Token: 0x040001BF RID: 447
		private bool _isConspiracySetUpStarted;
	}
}
