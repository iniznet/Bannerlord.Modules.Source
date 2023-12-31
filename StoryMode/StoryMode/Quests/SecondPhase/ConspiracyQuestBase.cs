﻿using System;
using System.Collections.Generic;
using System.Linq;
using StoryMode.StoryModeObjects;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace StoryMode.Quests.SecondPhase
{
	public abstract class ConspiracyQuestBase : QuestBase
	{
		public abstract TextObject SideNotificationText { get; }

		public abstract TextObject StartMessageLogFromMentor { get; }

		public abstract TextObject StartLog { get; }

		public abstract float ConspiracyStrengthDecreaseAmount { get; }

		public Hero Mentor
		{
			get
			{
				if (!StoryModeManager.Current.MainStoryLine.IsOnImperialQuestLine)
				{
					return StoryModeHeroes.AntiImperialMentor;
				}
				return StoryModeHeroes.ImperialMentor;
			}
		}

		public override bool IsRemainingTimeHidden
		{
			get
			{
				return false;
			}
		}

		public override bool IsSpecialQuest
		{
			get
			{
				return true;
			}
		}

		protected ConspiracyQuestBase(string questId, Hero questGiver)
			: base(questId, questGiver, CampaignTime.WeeksFromNow(3f), 0)
		{
			base.ChangeQuestDueTime(CampaignTime.WeeksFromNow(3f));
		}

		protected override void RegisterEvents()
		{
			StoryModeEvents.OnConspiracyActivatedEvent.AddNonSerializedListener(this, new Action(this.OnConspiracyActivated));
		}

		private void OnConspiracyActivated()
		{
			base.CompleteQuestWithFail(null);
		}

		protected override void OnStartQuest()
		{
			base.OnStartQuest();
			Campaign.Current.CampaignInformationManager.NewMapNoticeAdded(new ConspiracyQuestMapNotification(this, this.SideNotificationText));
			base.AddLog(this.StartMessageLogFromMentor, false);
			base.AddLog(this.StartLog, false);
		}

		protected override void OnCompleteWithSuccess()
		{
			base.OnCompleteWithSuccess();
			StoryModeManager.Current.MainStoryLine.SecondPhase.DecreaseConspiracyStrength(this.ConspiracyStrengthDecreaseAmount);
		}

		protected void DistributeConspiracyRaiderTroopsByLevel(PartyTemplateObject raiderTemplate, PartyBase partyToFill, int troopCountLimit)
		{
			List<KeyValuePair<int, List<CharacterObject>>> list = new List<KeyValuePair<int, List<CharacterObject>>>();
			foreach (PartyTemplateStack partyTemplateStack in raiderTemplate.Stacks.OrderBy((PartyTemplateStack t) => t.Character.Level))
			{
				int key = partyTemplateStack.Character.Level;
				if (!list.Exists((KeyValuePair<int, List<CharacterObject>> t) => t.Key == key))
				{
					list.Add(new KeyValuePair<int, List<CharacterObject>>(key, new List<CharacterObject>()));
				}
				CharacterObject character = partyTemplateStack.Character;
				KeyValuePair<int, List<CharacterObject>> keyValuePair = list.Find((KeyValuePair<int, List<CharacterObject>> t) => t.Key == key);
				if (keyValuePair.Value != null && !keyValuePair.Value.Contains(character))
				{
					keyValuePair.Value.Add(character);
				}
			}
			int num = list.Sum((KeyValuePair<int, List<CharacterObject>> t) => t.Key);
			List<KeyValuePair<int, int>> list2 = new List<KeyValuePair<int, int>>();
			foreach (KeyValuePair<int, List<CharacterObject>> keyValuePair2 in list)
			{
				list2.Add(new KeyValuePair<int, int>(keyValuePair2.Key, MathF.Floor((float)keyValuePair2.Key / (float)num * (float)troopCountLimit)));
			}
			foreach (PartyTemplateStack partyTemplateStack2 in raiderTemplate.Stacks)
			{
				int level = partyTemplateStack2.Character.Level;
				int num2 = list2.FindIndex((KeyValuePair<int, int> t) => t.Key == level);
				int num3 = list2.Count - 1 - num2;
				int num4 = MathF.Floor((float)list2[num3].Value / (float)list[num2].Value.Count);
				partyToFill.MemberRoster.AddToCounts(partyTemplateStack2.Character, num4, false, 0, 0, true, -1);
			}
			if (partyToFill.MemberRoster.TotalManCount < troopCountLimit)
			{
				partyToFill.MemberRoster.AddToCounts(list[0].Value[0], troopCountLimit - partyToFill.MemberRoster.TotalManCount, false, 0, 0, true, -1);
			}
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
		}
	}
}
