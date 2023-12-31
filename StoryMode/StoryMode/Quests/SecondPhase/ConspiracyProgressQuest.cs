﻿using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using StoryMode.StoryModeObjects;
using StoryMode.StoryModePhases;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace StoryMode.Quests.SecondPhase
{
	public class ConspiracyProgressQuest : StoryModeQuestBase
	{
		private bool _isImperialSide
		{
			get
			{
				return StoryModeManager.Current.MainStoryLine.IsOnImperialQuestLine;
			}
		}

		private TextObject _startQuestLogText
		{
			get
			{
				TextObject textObject = new TextObject("{=oX2aoilb}{MENTOR.NAME} knows of the rise of your {KINGDOM_NAME}. Rumors say {MENTOR.NAME} is planning to undo your progress. Be ready!", null);
				StringHelpers.SetCharacterProperties("MENTOR", this._isImperialSide ? StoryModeHeroes.AntiImperialMentor.CharacterObject : StoryModeHeroes.ImperialMentor.CharacterObject, textObject, false);
				textObject.SetTextVariable("KINGDOM_NAME", (Clan.PlayerClan.Kingdom != null) ? Clan.PlayerClan.Kingdom.Name : Clan.PlayerClan.Name);
				return textObject;
			}
		}

		private TextObject _questCanceledLogText
		{
			get
			{
				return new TextObject("{=tVlZTOst}You have chosen a different path.", null);
			}
		}

		public override TextObject Title
		{
			get
			{
				TextObject textObject;
				if (this._isImperialSide)
				{
					textObject = new TextObject("{=PJ5C3Dim}{ANTIIMPERIAL_MENTOR.NAME}'s Conspiracy", null);
					StringHelpers.SetCharacterProperties("ANTIIMPERIAL_MENTOR", StoryModeHeroes.AntiImperialMentor.CharacterObject, textObject, false);
				}
				else
				{
					textObject = new TextObject("{=i3SSc0I4}{IMPERIAL_MENTOR.NAME}'s Plan", null);
					StringHelpers.SetCharacterProperties("IMPERIAL_MENTOR", StoryModeHeroes.ImperialMentor.CharacterObject, textObject, false);
				}
				return textObject;
			}
		}

		public ConspiracyProgressQuest()
			: base("conspiracy_quest_campaign_behavior", null, CampaignTime.Never)
		{
			SecondPhase.Instance.TriggerConspiracy();
		}

		protected override void InitializeQuestOnGameLoad()
		{
			this.SetDialogs();
		}

		protected override void HourlyTick()
		{
		}

		protected override void RegisterEvents()
		{
			CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.DailyTick));
			CampaignEvents.OnQuestCompletedEvent.AddNonSerializedListener(this, new Action<QuestBase, QuestBase.QuestCompleteDetails>(this.OnQuestCompleted));
			StoryModeEvents.OnConspiracyActivatedEvent.AddNonSerializedListener(this, new Action(this.OnConspiracyActivated));
			CampaignEvents.OnClanChangedKingdomEvent.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, ChangeKingdomAction.ChangeKingdomActionDetail, bool>(this.OnClanChangedKingdom));
		}

		private void OnClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification = true)
		{
			if (clan == Clan.PlayerClan && oldKingdom == StoryModeManager.Current.MainStoryLine.PlayerSupportedKingdom)
			{
				base.CompleteQuestWithCancel(this._questCanceledLogText);
				StoryModeManager.Current.MainStoryLine.CancelSecondAndThirdPhase();
			}
		}

		protected override void OnStartQuest()
		{
			this._startQuestLog = base.AddDiscreteLog(this._startQuestLogText, new TextObject("{=1LrHV647}Conspiracy Strength", null), (int)SecondPhase.Instance.ConspiracyStrength, 2000, null, false);
		}

		protected override void SetDialogs()
		{
		}

		protected override void OnFinalize()
		{
			base.OnFinalize();
			foreach (QuestBase questBase in Campaign.Current.QuestManager.Quests.ToList<QuestBase>())
			{
				if (typeof(ConspiracyQuestBase) == questBase.GetType().BaseType && questBase.IsOngoing)
				{
					questBase.CompleteQuestWithCancel(new TextObject("{=YJxCbbpd}Conspiracy is activated!", null));
				}
			}
		}

		private void DailyTick()
		{
			StoryModeManager.Current.MainStoryLine.SecondPhase.IncreaseConspiracyStrength();
			this._startQuestLog.UpdateCurrentProgress((int)StoryModeManager.Current.MainStoryLine.SecondPhase.ConspiracyStrength);
		}

		private void OnQuestCompleted(QuestBase quest, QuestBase.QuestCompleteDetails detail)
		{
			if (detail == 1 && typeof(ConspiracyQuestBase) == quest.GetType().BaseType)
			{
				this._startQuestLog.UpdateCurrentProgress((int)StoryModeManager.Current.MainStoryLine.SecondPhase.ConspiracyStrength);
			}
		}

		private void OnConspiracyActivated()
		{
			base.CompleteQuestWithTimeOut(null);
		}

		internal static void AutoGeneratedStaticCollectObjectsConspiracyProgressQuest(object o, List<object> collectedObjects)
		{
			((ConspiracyProgressQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this._startQuestLog);
		}

		internal static object AutoGeneratedGetMemberValue_startQuestLog(object o)
		{
			return ((ConspiracyProgressQuest)o)._startQuestLog;
		}

		[SaveableField(2)]
		private JournalLog _startQuestLog;
	}
}
