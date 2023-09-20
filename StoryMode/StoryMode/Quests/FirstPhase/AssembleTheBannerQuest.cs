using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using StoryMode.StoryModeObjects;
using StoryMode.StoryModePhases;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace StoryMode.Quests.FirstPhase
{
	public class AssembleTheBannerQuest : StoryModeQuestBase
	{
		private TextObject _startQuestLog
		{
			get
			{
				return new TextObject("{=OS8YjyE5}You should collect all of the pieces of the Dragon Banner before deciding your path.", null);
			}
		}

		private TextObject _allPiecesCollectedQuestLog
		{
			get
			{
				TextObject textObject = new TextObject("{=eV8R0SKp}Now you can decide what to do with the {DRAGON_BANNER}.", null);
				textObject.SetTextVariable("DRAGON_BANNER", StoryModeManager.Current.MainStoryLine.DragonBanner.Name);
				StringHelpers.SetCharacterProperties("IMPERIAL_MENTOR", StoryModeHeroes.ImperialMentor.CharacterObject, textObject, false);
				StringHelpers.SetCharacterProperties("ANTI_IMPERIAL_MENTOR", StoryModeHeroes.AntiImperialMentor.CharacterObject, textObject, false);
				return textObject;
			}
		}

		private TextObject _talkedWithImperialMentorButNotWithAntiImperialMentorQuestLog
		{
			get
			{
				TextObject textObject = new TextObject("{=yNcBDr9j}You talked with {IMPERIAL_MENTOR.LINK}. Now, you may want to talk with {ANTI_IMPERIAL_MENTOR.LINK} and take {?ANTI_IMPERIAL_MENTOR.GENDER}her{?}his{\\?} opinions too. {?ANTI_IMPERIAL_MENTOR.GENDER}She{?}He{\\?} is currently in {SETTLEMENT_LINK}.", null);
				StringHelpers.SetCharacterProperties("IMPERIAL_MENTOR", StoryModeHeroes.ImperialMentor.CharacterObject, textObject, false);
				StringHelpers.SetCharacterProperties("ANTI_IMPERIAL_MENTOR", StoryModeHeroes.AntiImperialMentor.CharacterObject, textObject, false);
				textObject.SetTextVariable("SETTLEMENT_LINK", StoryModeHeroes.AntiImperialMentor.CurrentSettlement.EncyclopediaLinkWithName);
				return textObject;
			}
		}

		private TextObject _talkedWithImperialMentorQuestLog
		{
			get
			{
				TextObject textObject = new TextObject("{=RwlDeE9t}You talked with {IMPERIAL_MENTOR.LINK} too. Now you should make a decision.", null);
				StringHelpers.SetCharacterProperties("IMPERIAL_MENTOR", StoryModeHeroes.ImperialMentor.CharacterObject, textObject, false);
				return textObject;
			}
		}

		private TextObject _talkedWithAntiImperialMentorButNotWithImperialMentorQuestLog
		{
			get
			{
				TextObject textObject = new TextObject("{=yub8ZSFP}You talked with {ANTI_IMPERIAL_MENTOR.LINK}. Now, you may want to talk with {IMPERIAL_MENTOR.LINK} and take {?IMPERIAL_MENTOR.GENDER}her{?}his{\\?} opinions too. {?IMPERIAL_MENTOR.GENDER}She{?}He{\\?} is currently in {SETTLEMENT_LINK}.", null);
				StringHelpers.SetCharacterProperties("ANTI_IMPERIAL_MENTOR", StoryModeHeroes.AntiImperialMentor.CharacterObject, textObject, false);
				StringHelpers.SetCharacterProperties("IMPERIAL_MENTOR", StoryModeHeroes.ImperialMentor.CharacterObject, textObject, false);
				textObject.SetTextVariable("SETTLEMENT_LINK", StoryModeHeroes.ImperialMentor.CurrentSettlement.EncyclopediaLinkWithName);
				return textObject;
			}
		}

		private TextObject _talkedWithAntiImperialMentorQuestLog
		{
			get
			{
				TextObject textObject = new TextObject("{=rfkKxdxp}You talked with {ANTI_IMPERIAL_MENTOR.LINK} too. Now you should make a decision.", null);
				StringHelpers.SetCharacterProperties("ANTI_IMPERIAL_MENTOR", StoryModeHeroes.AntiImperialMentor.CharacterObject, textObject, false);
				return textObject;
			}
		}

		private TextObject _endQuestLog
		{
			get
			{
				return new TextObject("{=eNJBjYG8}You successfully assembled the Dragon Banner of Calradios.", null);
			}
		}

		public override TextObject Title
		{
			get
			{
				return new TextObject("{=y84UnOQX}Assemble the Dragon Banner", null);
			}
		}

		public override bool IsRemainingTimeHidden
		{
			get
			{
				return false;
			}
		}

		public AssembleTheBannerQuest()
			: base("assemble_the_banner_story_mode_quest", null, StoryModeManager.Current.MainStoryLine.FirstPhase.FirstPhaseEndTime)
		{
			this._talkedWithImperialMentor = false;
			this._talkedWithAntiImperialMentor = false;
		}

		protected override void InitializeQuestOnGameLoad()
		{
			this.SetDialogs();
		}

		protected override void RegisterEvents()
		{
			StoryModeEvents.OnBannerPieceCollectedEvent.AddNonSerializedListener(this, new Action(this.OnBannerPieceCollected));
			CampaignEvents.OnQuestCompletedEvent.AddNonSerializedListener(this, new Action<QuestBase, QuestBase.QuestCompleteDetails>(this.OnQuestCompleted));
		}

		protected override void OnStartQuest()
		{
			this.SetDialogs();
			this._startLog = base.AddDiscreteLog(this._startQuestLog, new TextObject("{=xL3WGYsw}Collected Pieces", null), FirstPhase.Instance.CollectedBannerPieceCount, 3, null, false);
		}

		protected override void OnCompleteWithSuccess()
		{
			base.AddLog(this._endQuestLog, false);
		}

		private void OnBannerPieceCollected()
		{
			this._startLog.UpdateCurrentProgress(FirstPhase.Instance.CollectedBannerPieceCount);
			if (FirstPhase.Instance.AllPiecesCollected)
			{
				base.AddLog(this._allPiecesCollectedQuestLog, false);
				base.AddTrackedObject(StoryModeHeroes.ImperialMentor.CurrentSettlement);
				base.AddTrackedObject(StoryModeHeroes.AntiImperialMentor.CurrentSettlement);
				base.AddTrackedObject(StoryModeHeroes.ImperialMentor);
				base.AddTrackedObject(StoryModeHeroes.AntiImperialMentor);
				FirstPhase firstPhase = StoryModeManager.Current.MainStoryLine.FirstPhase;
				if (firstPhase == null)
				{
					return;
				}
				firstPhase.MergeDragonBanner();
			}
		}

		private void OnQuestCompleted(QuestBase quest, QuestBase.QuestCompleteDetails detail)
		{
			if (quest is CreateKingdomQuest || quest is SupportKingdomQuest)
			{
				if (base.IsTracked(StoryModeHeroes.AntiImperialMentor.CurrentSettlement))
				{
					base.RemoveTrackedObject(StoryModeHeroes.AntiImperialMentor.CurrentSettlement);
				}
				if (base.IsTracked(StoryModeHeroes.ImperialMentor.CurrentSettlement))
				{
					base.RemoveTrackedObject(StoryModeHeroes.ImperialMentor.CurrentSettlement);
				}
				if (base.IsTracked(StoryModeHeroes.AntiImperialMentor))
				{
					base.RemoveTrackedObject(StoryModeHeroes.AntiImperialMentor);
				}
				if (base.IsTracked(StoryModeHeroes.ImperialMentor))
				{
					base.RemoveTrackedObject(StoryModeHeroes.ImperialMentor);
				}
				base.CompleteQuestWithSuccess();
			}
		}

		public override void OnFailed()
		{
			base.OnFailed();
			this.RemoveRemainingBannerPieces();
		}

		public override void OnCanceled()
		{
			base.OnCanceled();
			this.RemoveRemainingBannerPieces();
		}

		private void RemoveRemainingBannerPieces()
		{
			ItemObject @object = Campaign.Current.ObjectManager.GetObject<ItemObject>("dragon_banner_center");
			ItemObject object2 = Campaign.Current.ObjectManager.GetObject<ItemObject>("dragon_banner_dragonhead");
			ItemObject object3 = Campaign.Current.ObjectManager.GetObject<ItemObject>("dragon_banner_handle");
			foreach (ItemRosterElement itemRosterElement in MobileParty.MainParty.ItemRoster)
			{
				if (itemRosterElement.EquipmentElement.Item == @object || itemRosterElement.EquipmentElement.Item == object2 || itemRosterElement.EquipmentElement.Item == object3)
				{
					MobileParty.MainParty.ItemRoster.Remove(itemRosterElement);
				}
			}
		}

		protected override void SetDialogs()
		{
			Campaign.Current.ConversationManager.AddDialogFlow(this.GetImperialMentorEndQuestDialog(), this);
			Campaign.Current.ConversationManager.AddDialogFlow(this.GetAntiImperialMentorEndQuestDialog(), this);
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("lord_start", 150).NpcLine(new TextObject("{=AHDQffXv}Have you assembled the banner?", null), null, null).Condition(new ConversationSentence.OnConditionDelegate(this.AssembleBannerConditionDialogCondition))
				.PlayerLine(new TextObject("{=2h7IlBmv}Not yet, I'm working on it...", null), null)
				.Consequence(delegate
				{
					if (PlayerEncounter.Current != null)
					{
						PlayerEncounter.LeaveEncounter = true;
					}
				})
				.CloseDialog(), this);
		}

		private bool AssembleBannerConditionDialogCondition()
		{
			if ((Hero.OneToOneConversationHero == StoryModeHeroes.ImperialMentor || Hero.OneToOneConversationHero == StoryModeHeroes.AntiImperialMentor) && !FirstPhase.Instance.AllPiecesCollected)
			{
				if (Hero.OneToOneConversationHero == StoryModeHeroes.ImperialMentor)
				{
					if (Campaign.Current.QuestManager.Quests.Any((QuestBase q) => !q.IsFinalized && q is MeetWithIstianaQuest))
					{
						return false;
					}
				}
				if (Hero.OneToOneConversationHero == StoryModeHeroes.AntiImperialMentor)
				{
					if (Campaign.Current.QuestManager.Quests.Any((QuestBase q) => !q.IsFinalized && q is MeetWithArzagosQuest))
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		private DialogFlow GetAntiImperialMentorEndQuestDialog()
		{
			string text;
			return DialogFlow.CreateDialogFlow("hero_main_options", 150).BeginPlayerOptions().PlayerSpecialOption(new TextObject("{=r8ZLabb0}I have gathered all pieces of the Dragon Banner. What now?", null), null)
				.Condition(() => Hero.OneToOneConversationHero == StoryModeHeroes.AntiImperialMentor && FirstPhase.Instance.AllPiecesCollected && !this._talkedWithAntiImperialMentor)
				.NpcLine(new TextObject("{=5j6qvGAF}Excellent work! When you unfurl this banner, and men see what they thought was lost, it will make a powerful impression.", null), null, null)
				.Consequence(new ConversationSentence.OnConsequenceDelegate(this.GetAntiImperialQuests))
				.NpcLine(new TextObject("{=MOVWOyeh}Clearly you have been chosen by Heaven for a great purpose. I see the makings of a new legend here... Allow me to call you 'Bannerlord.'", null), null, null)
				.NpcLine(new TextObject("{=o791xRtb}Right then, to the business of bringing down this cursed Empire. As I see it, you have two options...", null), null, null)
				.GetOutputToken(ref text)
				.NpcLine(new TextObject("{=c6pDNXbb}You can create your own kingdom or support an existing one...", null), null, null)
				.BeginPlayerOptions()
				.PlayerOption(new TextObject("{=0pilmavQ}How can I create my own kingdom?", null), null)
				.NpcLine(new TextObject("{=frk7T3ue}It will not be easy, but I can explain in detail...", null), null, null)
				.NpcLine(new TextObject("{=rmyXSgy7}Firstly, your clan must be independent. You cannot be pledged to an existing realm.", null), null, null)
				.NpcLine(new TextObject("{=tJQ5oajd}Next, your clan must have won for itself considerable renown, or no one will follow you.", null), null, null)
				.NpcLine(new TextObject("{=MJd5agS2}I would recommend that you gather a fairly large army, as you may soon be at war with more powerful and established realms.", null), null, null)
				.NpcLine(new TextObject("{=6YhGGJ7a}Finally, you need a capital for your realm. It can be any settlement you own, so long as they do not speak the imperial tongue. I will not help you create another Empire.", null), null, null)
				.NpcLine(new TextObject("{=fprOWs1E}Now, when you are ready to declare your new kingdom, instruct the governor of your capital to have a proclamation read out throughout your lands.", null), null, null)
				.NpcLine(new TextObject("{=Q2obAF4E}So! You have much to do. I will await news of your success. Return to me when you wish to declare your ownership of the banner to the world.", null), null, null)
				.GotoDialogState(text)
				.PlayerOption(new TextObject("{=mtiaY2Pa}How can I support an existing kingdom?", null), null)
				.NpcLine(new TextObject("{=oKknZdXn}You should join the kingdom that you wish to support by talking to the leader. None will bring back the Palaic people, but the final victory of any one of those would be suitable vengeance.", null), null, null)
				.NpcLine(new TextObject("{=dPb2Vph3}My informants will tell me once you pledged your support...", null), null, null)
				.GotoDialogState(text)
				.PlayerOption(new TextObject("{=6LQUuQhV}Thank you for your precious help.", null), null)
				.CloseDialog()
				.EndPlayerOptions()
				.CloseDialog();
		}

		private void GetAntiImperialQuests()
		{
			this._talkedWithAntiImperialMentor = true;
			if (!this._talkedWithImperialMentor)
			{
				base.AddLog(this._talkedWithAntiImperialMentorButNotWithImperialMentorQuestLog, false);
			}
			else
			{
				base.AddLog(this._talkedWithAntiImperialMentorQuestLog, false);
			}
			if (base.IsTracked(StoryModeHeroes.AntiImperialMentor.CurrentSettlement))
			{
				base.RemoveTrackedObject(StoryModeHeroes.AntiImperialMentor.CurrentSettlement);
			}
			new CreateKingdomQuest(StoryModeHeroes.AntiImperialMentor).StartQuest();
			new SupportKingdomQuest(StoryModeHeroes.AntiImperialMentor).StartQuest();
		}

		private DialogFlow GetImperialMentorEndQuestDialog()
		{
			string text;
			return DialogFlow.CreateDialogFlow("hero_main_options", 150).BeginPlayerOptions().PlayerSpecialOption(new TextObject("{=r8ZLabb0}I have gathered all pieces of the Dragon Banner. What now?", null), null)
				.Condition(() => Hero.OneToOneConversationHero == StoryModeHeroes.ImperialMentor && FirstPhase.Instance.AllPiecesCollected && !this._talkedWithImperialMentor)
				.NpcLine(new TextObject("{=UjyZ7GFk}Impressive, most impressive. Well, things will get interesting now.", null), null, null)
				.Consequence(new ConversationSentence.OnConsequenceDelegate(this.GetImperialQuests))
				.NpcLine(new TextObject("{=9E6faNBg}I will need to embroider a proper legend about you. Divine omens at your birth, that kind of thing. For now, we can call you 'Bannerlord,' who brings down the wrath of Heaven on the impudent barbarians.", null), null, null)
				.NpcLine(new TextObject("{=CnXA7oyE}Now, there are two paths that lie ahead of you, my child!", null), null, null)
				.GetOutputToken(ref text)
				.NpcLine(new TextObject("{=1GgTNRNl}You can make your own claim to the rulership of the Empire and try to win the civil war, or support an existing claimant...", null), null, null)
				.BeginPlayerOptions()
				.PlayerOption(new TextObject("{=Dgdopl1b}How can I create my own imperial kingdom?", null), null)
				.NpcLine(new TextObject("{=NdkqUnXb}To have a chance as an imperial contender, you must fullfil some conditions.", null), null, null)
				.NpcLine(new TextObject("{=yCzcfKNM}Firstly, your clan must be independent. You cannot be pledged to an existing realm.", null), null, null)
				.NpcLine(new TextObject("{=LLJ0oB8i}Next, your clan's renown must have spread far and wide, or no one will take you seriously.", null), null, null)
				.NpcLine(new TextObject("{=3XbTo6O7}Also, of course, I recommend that you have as large an army as you can gather.", null), null, null)
				.NpcLine(new TextObject("{=Cl4xi6Be}Finally, you need a capital. Any settlement will do, so long as the inhabitants speak the imperial language.", null), null, null)
				.NpcLine(new TextObject("{=fprOWs1E}Now, when you are ready to declare your new kingdom, instruct the governor of your capital to have a proclamation read out throughout your lands.", null), null, null)
				.NpcLine(new TextObject("{=tkJD40hE}Well, that should keep you busy for a while. Come back when you are ready.", null), null, null)
				.GotoDialogState(text)
				.PlayerOption(new TextObject("{=tRzjuX0E}How can I support an existing imperial claimant?", null), null)
				.NpcLine(new TextObject("{=oL9BdThD}Choose one and pledge allegiance. When this civil war began, I was a bit torn... Rhagaea was the cleverest ruler, Garios probably the best fighter, and Lucon seemed to have the best grasp of our laws and traditions. But you can make up your own mind.", null), null, null)
				.NpcLine(new TextObject("{=eaxOH9mb}My little birds will tell me once you pledge your support...", null), null, null)
				.GotoDialogState(text)
				.PlayerOption(new TextObject("{=6LQUuQhV}Thank you for your precious help.", null), null)
				.CloseDialog()
				.EndPlayerOptions()
				.CloseDialog();
		}

		private void GetImperialQuests()
		{
			this._talkedWithImperialMentor = true;
			if (!this._talkedWithAntiImperialMentor)
			{
				base.AddLog(this._talkedWithImperialMentorButNotWithAntiImperialMentorQuestLog, false);
			}
			else
			{
				base.AddLog(this._talkedWithImperialMentorQuestLog, false);
			}
			if (base.IsTracked(StoryModeHeroes.ImperialMentor.CurrentSettlement))
			{
				base.RemoveTrackedObject(StoryModeHeroes.ImperialMentor.CurrentSettlement);
			}
			new CreateKingdomQuest(StoryModeHeroes.ImperialMentor).StartQuest();
			new SupportKingdomQuest(StoryModeHeroes.ImperialMentor).StartQuest();
		}

		internal static void AutoGeneratedStaticCollectObjectsAssembleTheBannerQuest(object o, List<object> collectedObjects)
		{
			((AssembleTheBannerQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this._startLog);
		}

		internal static object AutoGeneratedGetMemberValue_startLog(object o)
		{
			return ((AssembleTheBannerQuest)o)._startLog;
		}

		internal static object AutoGeneratedGetMemberValue_talkedWithImperialMentor(object o)
		{
			return ((AssembleTheBannerQuest)o)._talkedWithImperialMentor;
		}

		internal static object AutoGeneratedGetMemberValue_talkedWithAntiImperialMentor(object o)
		{
			return ((AssembleTheBannerQuest)o)._talkedWithAntiImperialMentor;
		}

		[SaveableField(1)]
		private JournalLog _startLog;

		[SaveableField(2)]
		private bool _talkedWithImperialMentor;

		[SaveableField(3)]
		private bool _talkedWithAntiImperialMentor;
	}
}
