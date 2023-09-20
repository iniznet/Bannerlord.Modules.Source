using System;
using System.Collections.Generic;
using System.Linq;
using StoryMode.StoryModeObjects;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace StoryMode.Quests.SecondPhase
{
	// Token: 0x02000026 RID: 38
	public abstract class ConspiracyQuestBase : QuestBase
	{
		// Token: 0x1700005D RID: 93
		// (get) Token: 0x060001B0 RID: 432
		public abstract TextObject SideNotificationText { get; }

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x060001B1 RID: 433
		public abstract TextObject StartMessageLogFromMentor { get; }

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x060001B2 RID: 434
		public abstract TextObject StartLog { get; }

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x060001B3 RID: 435
		public abstract float ConspiracyStrengthDecreaseAmount { get; }

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x060001B4 RID: 436 RVA: 0x0000A146 File Offset: 0x00008346
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

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x060001B5 RID: 437 RVA: 0x0000A164 File Offset: 0x00008364
		public override bool IsRemainingTimeHidden
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x060001B6 RID: 438 RVA: 0x0000A167 File Offset: 0x00008367
		public override bool IsSpecialQuest
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060001B7 RID: 439 RVA: 0x0000A16A File Offset: 0x0000836A
		protected ConspiracyQuestBase(string questId, Hero questGiver)
			: base(questId, questGiver, CampaignTime.WeeksFromNow(3f), 0)
		{
			base.ChangeQuestDueTime(CampaignTime.WeeksFromNow(3f));
		}

		// Token: 0x060001B8 RID: 440 RVA: 0x0000A18F File Offset: 0x0000838F
		protected override void RegisterEvents()
		{
			StoryModeEvents.OnConspiracyActivatedEvent.AddNonSerializedListener(this, new Action(this.OnConspiracyActivated));
		}

		// Token: 0x060001B9 RID: 441 RVA: 0x0000A1A8 File Offset: 0x000083A8
		private void OnConspiracyActivated()
		{
			base.CompleteQuestWithFail(null);
		}

		// Token: 0x060001BA RID: 442 RVA: 0x0000A1B1 File Offset: 0x000083B1
		protected override void OnStartQuest()
		{
			base.OnStartQuest();
			Campaign.Current.CampaignInformationManager.NewMapNoticeAdded(new ConspiracyQuestMapNotification(this, this.SideNotificationText));
			base.AddLog(this.StartMessageLogFromMentor, false);
			base.AddLog(this.StartLog, false);
		}

		// Token: 0x060001BB RID: 443 RVA: 0x0000A1F0 File Offset: 0x000083F0
		protected override void OnCompleteWithSuccess()
		{
			base.OnCompleteWithSuccess();
			StoryModeManager.Current.MainStoryLine.SecondPhase.DecreaseConspiracyStrength(this.ConspiracyStrengthDecreaseAmount);
		}

		// Token: 0x060001BC RID: 444 RVA: 0x0000A214 File Offset: 0x00008414
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

		// Token: 0x060001BD RID: 445 RVA: 0x0000A4AC File Offset: 0x000086AC
		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
		}
	}
}
