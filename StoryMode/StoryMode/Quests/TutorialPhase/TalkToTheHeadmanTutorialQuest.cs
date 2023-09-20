using System;
using System.Collections.Generic;
using Helpers;
using StoryMode.StoryModeObjects;
using StoryMode.StoryModePhases;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace StoryMode.Quests.TutorialPhase
{
	public class TalkToTheHeadmanTutorialQuest : StoryModeQuestBase
	{
		private TextObject _startQuestLog
		{
			get
			{
				return new TextObject("{=rinefpgo}You have arrived at the village. You can buy some food and hire some men to help hunt for the raiders. First go into the village and talk to the headman.", null);
			}
		}

		private TextObject _readyToGoLog
		{
			get
			{
				return new TextObject("{=KhL2ctsi}You're ready to leave now. Talk to the headman again. He had said he has a task for you.", null);
			}
		}

		private TextObject _goBackToVillageMenuLog
		{
			get
			{
				return new TextObject("{=awgBkdXx}You should go back to the village menu and make your preparations to go after the raiders, then find out about the task that the headman has for you.", null);
			}
		}

		public override TextObject Title
		{
			get
			{
				TextObject textObject = new TextObject("{=HqlXdzcv}Talk with Headman {HEADMAN.FIRSTNAME}", null);
				StringHelpers.SetCharacterProperties("HEADMAN", this._headman.CharacterObject, textObject, false);
				return textObject;
			}
		}

		public TalkToTheHeadmanTutorialQuest(Hero headman)
			: base("talk_to_the_headman_tutorial_quest", null, CampaignTime.Never)
		{
			this._headman = headman;
			base.AddTrackedObject(this._headman);
			this.SetDialogs();
			base.InitializeQuestOnCreation();
			base.AddLog(this._startQuestLog, false);
			TutorialPhase.Instance.SetTutorialFocusSettlement(Settlement.CurrentSettlement);
		}

		protected override void InitializeQuestOnGameLoad()
		{
			this.SetDialogs();
		}

		protected override void RegisterEvents()
		{
			CampaignEvents.OnQuestCompletedEvent.AddNonSerializedListener(this, new Action<QuestBase, QuestBase.QuestCompleteDetails>(this.OnQuestCompleted));
			CampaignEvents.BeforeMissionOpenedEvent.AddNonSerializedListener(this, new Action(this.OnBeforeMissionOpened));
		}

		protected override void SetDialogs()
		{
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 1000010).NpcLine(new TextObject("{=YEeb0B1V}I am {HEADMAN.FIRSTNAME}, headman of this village. What brings you here?", null), null, null).Condition(new ConversationSentence.OnConditionDelegate(this.headman_quest_conversation_start_on_condition))
				.PlayerLine(new TextObject("{=StLYbEQZ}We need help. Some raiders have taken our younger brother and sister captive. We think they may have passed this way.", null), null)
				.NpcLine(new TextObject("{=uNgu02FH}They got your people too? Sorry to hear that. Those bastards have done a bit of killing and looting in these parts as well.[if:convo_grave]", null), null, null)
				.NpcLine(new TextObject("{=bNcGO33Q}We think they've gone north. I reckon there are a few folk around here who'll join you in going after them if you'll pay for their gear.", null), null, null)
				.NpcLine(new TextObject("{=5Mw4trfs}Once you've made your preparations, come and talk to me again. I may have a task for you if you are going after the raiders.", null), null, null)
				.Consequence(new ConversationSentence.OnConsequenceDelegate(this.headman_quest_conversation_end_on_consequence))
				.CloseDialog(), this);
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 1000009).NpcLine(new TextObject("{=uhYXopnJ}Have you finished your preparations?", null), null, null).Condition(() => Hero.OneToOneConversationHero == this._headman && (!this._recruitTroopsQuest.IsFinalized || !this._purchaseGrainQuest.IsFinalized))
				.PlayerLine(new TextObject("{=elJCacQO}I am working on it.", null), null)
				.CloseDialog(), this);
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 1000010).NpcLine(new TextObject("{=TIaVyqMx}Glad to see you found what you needed. Now, about that matter I mentioned earlier...[if:convo_grave]", null), null, null).Condition(new ConversationSentence.OnConditionDelegate(this.headman_quest_end_conversation_start_on_condition))
				.NpcLine(new TextObject("{=lnAhXvbo}There's this wandering doctor who comes through here from time to time. Name of Tacteos. Treats people for free... We're fond of him.", null), null, null)
				.NpcLine(new TextObject("{=xGdoz9Pn}Well, we last saw him a few days ago. He was carrying some sort of chest, which he was very mysterious about. He was on some sort of 'quest', he said, though wouldn't tell us more.", null), null, null)
				.NpcLine(new TextObject("{=WDylM3dx}He set off on the road just a few hours before the raiders came through here. Well, he's not really a worldly type, just the kind of fellow who'd stumble into a trap and let himself be captured. We're worried about him.", null), null, null)
				.NpcLine(new TextObject("{=MREvo37b}If you can keep an eye out for him, this Tacteos, we'd be very grateful. Maybe, if he's alive and well, he'll tell you a little more about his 'quest.'", null), null, null)
				.Consequence(new ConversationSentence.OnConsequenceDelegate(this.headman_quest_end_conversation_start_on_consequence))
				.CloseDialog(), this);
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 1000009).NpcLine(new TextObject("{=gX0RzZoT}Let's just go speak to the headman.", null), null, null).Condition(new ConversationSentence.OnConditionDelegate(this.headman_quest_conversation_talk_with_brother_on_condition))
				.CloseDialog(), this);
		}

		private bool headman_quest_conversation_start_on_condition()
		{
			StringHelpers.SetCharacterProperties("HEADMAN", this._headman.CharacterObject, null, false);
			return Hero.OneToOneConversationHero == this._headman && !this._headman.HasMet;
		}

		private bool headman_quest_conversation_talk_with_brother_on_condition()
		{
			StringHelpers.SetCharacterProperties("BROTHER", StoryModeHeroes.ElderBrother.CharacterObject, null, false);
			return Hero.OneToOneConversationHero == StoryModeHeroes.ElderBrother && !this._headman.HasMet;
		}

		private void headman_quest_conversation_end_on_consequence()
		{
			this._headman.SetHasMet();
			this._headman.SetPersonalRelation(Hero.MainHero, 100);
			this._recruitTroopsQuest = new RecruitTroopsTutorialQuest(this._headman);
			this._recruitTroopsQuest.StartQuest();
			this._purchaseGrainQuest = new PurchaseGrainTutorialQuest(this._headman);
			this._purchaseGrainQuest.StartQuest();
			TutorialPhase.Instance.SetTutorialQuestPhase(TutorialQuestPhase.RecruitAndPurchaseStarted);
			base.AddLog(this._goBackToVillageMenuLog, false);
		}

		private bool headman_quest_end_conversation_start_on_condition()
		{
			return Hero.OneToOneConversationHero == this._headman && this._recruitTroopsQuest.IsFinalized && this._purchaseGrainQuest.IsFinalized;
		}

		private void headman_quest_end_conversation_start_on_consequence()
		{
			TutorialPhase.Instance.SetLockTutorialVillageEnter(false);
			base.CompleteQuestWithSuccess();
		}

		protected override void OnCompleteWithSuccess()
		{
			TutorialPhase.Instance.RemoveTutorialFocusSettlement();
		}

		private void OnQuestCompleted(QuestBase quest, QuestBase.QuestCompleteDetails detail)
		{
			if (this._recruitTroopsQuest.IsFinalized && this._purchaseGrainQuest.IsFinalized)
			{
				TutorialPhase.Instance.SetLockTutorialVillageEnter(true);
				TextObject textObject = new TextObject("{=3YHL3wpM}{BROTHER.NAME}:", null);
				TextObjectExtensions.SetCharacterProperties(textObject, "BROTHER", StoryModeHeroes.ElderBrother.CharacterObject, false);
				InformationManager.ShowInquiry(new InquiryData(textObject.ToString(), new TextObject("{=1xqmoDvS}We have finished our preparations. Let's talk to the headman again. He had said he may have a task for us. We could use his friendship.", null).ToString(), true, false, new TextObject("{=lmG7uRK2}Okay", null).ToString(), null, null, null, "", 0f, null, null, null), false, false);
				base.AddLog(this._readyToGoLog, false);
			}
		}

		private void OnBeforeMissionOpened()
		{
			if (Settlement.CurrentSettlement != null && Settlement.CurrentSettlement.StringId == "village_ES3_2")
			{
				int hitPoints = StoryModeHeroes.ElderBrother.HitPoints;
				int num = 50;
				if (hitPoints < num)
				{
					int num2 = num - hitPoints;
					StoryModeHeroes.ElderBrother.Heal(num2, false);
				}
				LocationCharacter locationCharacterOfHero = LocationComplex.Current.GetLocationCharacterOfHero(StoryModeHeroes.ElderBrother);
				locationCharacterOfHero.CharacterRelation = 0;
				PlayerEncounter.LocationEncounter.AddAccompanyingCharacter(locationCharacterOfHero, true);
			}
		}

		internal static void AutoGeneratedStaticCollectObjectsTalkToTheHeadmanTutorialQuest(object o, List<object> collectedObjects)
		{
			((TalkToTheHeadmanTutorialQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this._headman);
			collectedObjects.Add(this._recruitTroopsQuest);
			collectedObjects.Add(this._purchaseGrainQuest);
		}

		internal static object AutoGeneratedGetMemberValue_headman(object o)
		{
			return ((TalkToTheHeadmanTutorialQuest)o)._headman;
		}

		internal static object AutoGeneratedGetMemberValue_recruitTroopsQuest(object o)
		{
			return ((TalkToTheHeadmanTutorialQuest)o)._recruitTroopsQuest;
		}

		internal static object AutoGeneratedGetMemberValue_purchaseGrainQuest(object o)
		{
			return ((TalkToTheHeadmanTutorialQuest)o)._purchaseGrainQuest;
		}

		[SaveableField(1)]
		private readonly Hero _headman;

		[SaveableField(2)]
		private RecruitTroopsTutorialQuest _recruitTroopsQuest;

		[SaveableField(3)]
		private PurchaseGrainTutorialQuest _purchaseGrainQuest;
	}
}
