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
	public class SecondPhaseCampaignBehavior : CampaignBehaviorBase
	{
		public SecondPhaseCampaignBehavior()
		{
			this._conspiracyQuestTriggerDayCounter = 0;
			this._isConspiracySetUpStarted = false;
		}

		public override void RegisterEvents()
		{
			CampaignEvents.WeeklyTickEvent.AddNonSerializedListener(this, new Action(this.WeeklyTick));
			CampaignEvents.OnQuestStartedEvent.AddNonSerializedListener(this, new Action<QuestBase>(this.OnQuestStarted));
			CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.DailyTick));
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
			CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnGameLoaded));
			StoryModeEvents.OnConspiracyActivatedEvent.AddNonSerializedListener(this, new Action(this.OnConspiracyActivated));
		}

		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<int>("_conspiracyQuestTriggerDayCounter", ref this._conspiracyQuestTriggerDayCounter);
			dataStore.SyncData<bool>("_isConspiracySetUpStarted", ref this._isConspiracySetUpStarted);
		}

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

		private void OnQuestStarted(QuestBase quest)
		{
			if (quest is AssembleEmpireQuestBehavior.AssembleEmpireQuest || quest is WeakenEmpireQuestBehavior.WeakenEmpireQuest)
			{
				StoryModeManager.Current.MainStoryLine.CompleteFirstPhase();
				this._isConspiracySetUpStarted = true;
			}
		}

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

		private void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
		{
			SecondPhase instance = SecondPhase.Instance;
			if (instance == null)
			{
				return;
			}
			instance.OnSessionLaunched();
		}

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

		private void OnConspiracyActivated()
		{
			CampaignEventDispatcher.Instance.RemoveListeners(this);
		}

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

		private int _conspiracyQuestTriggerDayCounter;

		private bool _isConspiracySetUpStarted;
	}
}
