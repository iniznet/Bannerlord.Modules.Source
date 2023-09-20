using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using StoryMode.StoryModeObjects;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace StoryMode.Quests.FirstPhase
{
	// Token: 0x02000034 RID: 52
	public class MeetWithArzagosQuest : StoryModeQuestBase
	{
		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x06000300 RID: 768 RVA: 0x00010804 File Offset: 0x0000EA04
		private TextObject _startQuestLog
		{
			get
			{
				TextObject textObject = new TextObject("{=5K4wvz3w}Find and meet {HERO.LINK} to learn more about Neretzes' Banner. He is currently in {SETTLEMENT}.", null);
				StringHelpers.SetCharacterProperties("HERO", StoryModeHeroes.AntiImperialMentor.CharacterObject, textObject, false);
				textObject.SetTextVariable("SETTLEMENT", StoryModeHeroes.AntiImperialMentor.CurrentSettlement.EncyclopediaLinkWithName);
				return textObject;
			}
		}

		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x06000301 RID: 769 RVA: 0x00010850 File Offset: 0x0000EA50
		private TextObject _endQuestLog
		{
			get
			{
				TextObject textObject = new TextObject("{=qMUfOtyk}You talked with {HERO.LINK}.", null);
				StringHelpers.SetCharacterProperties("HERO", StoryModeHeroes.AntiImperialMentor.CharacterObject, textObject, false);
				return textObject;
			}
		}

		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x06000302 RID: 770 RVA: 0x00010884 File Offset: 0x0000EA84
		public override TextObject Title
		{
			get
			{
				TextObject textObject = new TextObject("{=Y6SqyQwn}Meet with {HERO.NAME}", null);
				StringHelpers.SetCharacterProperties("HERO", StoryModeHeroes.AntiImperialMentor.CharacterObject, textObject, false);
				return textObject;
			}
		}

		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x06000303 RID: 771 RVA: 0x000108B5 File Offset: 0x0000EAB5
		public override bool IsRemainingTimeHidden
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06000304 RID: 772 RVA: 0x000108B8 File Offset: 0x0000EAB8
		public MeetWithArzagosQuest(Settlement settlement)
			: base("meet_with_arzagos_story_mode_quest", null, StoryModeManager.Current.MainStoryLine.FirstPhase.FirstPhaseEndTime)
		{
			this._metAntiImperialMentor = false;
			this.SetDialogs();
			HeroHelper.SpawnHeroForTheFirstTime(StoryModeHeroes.AntiImperialMentor, settlement);
			base.AddTrackedObject(settlement);
			base.AddTrackedObject(StoryModeHeroes.AntiImperialMentor);
			base.AddLog(this._startQuestLog, false);
		}

		// Token: 0x06000305 RID: 773 RVA: 0x0001091D File Offset: 0x0000EB1D
		protected override void InitializeQuestOnGameLoad()
		{
			this.SetDialogs();
		}

		// Token: 0x06000306 RID: 774 RVA: 0x00010928 File Offset: 0x0000EB28
		protected override void SetDialogs()
		{
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("lord_start", 110).NpcLine(new TextObject("{=unOLk4PY}So. Who are you, and what brings you to me?", null), null, null).Condition(() => Hero.OneToOneConversationHero != null && Hero.OneToOneConversationHero == StoryModeHeroes.AntiImperialMentor && !this._metAntiImperialMentor)
				.PlayerLine(new TextObject("{=5QiuMoe6}I believe I have a piece of the Dragon Banner of Calradios.", null), null)
				.NpcLine(new TextObject("{=uvbCyLiR}Is that true? Well, that is interesting.", null), null, null)
				.NpcLine(new TextObject("{=pOuGX9j0}You may have one piece of the banner, but it's of little use in itself. You'll have to find the other parts. But once you can bring together the pieces, you'll have something of tremendous value.", null), null, null)
				.PlayerLine(new TextObject("{=t71lPdyb}How so?", null), null)
				.NpcLine(new TextObject("{=SmVwMrUM}The banner of Calradios is part of a legend. It was said to be carried by Calradios the Great, who first led his people to this land, to conquer and despoil. The legend says that no army led by a true son of Calradios shall be defeated in battle.", null), null, null)
				.NpcLine(new TextObject("{=cNwejsNl}Convenient legend, eh? Of course the Calradians have been defeated many times, but I guess those were not 'true sons.' Still, you could say it represents the strength and endurance of this empire.", null), null, null)
				.PlayerLine(new TextObject("{=FBp2ranI}So, can you help me find a buyer for it?", null), null)
				.NpcLine(new TextObject("{=3G64Ej64}A buyer? I can help you do far more than that.", null), null, null)
				.PlayerLine(new TextObject("{=MnmblprY}So, where can I find the other pieces?", null), null)
				.NpcLine(new TextObject("{=Fgta5mF6}Before I answer, you and I need to know more about each other. I don't know what you know about me.  I was a citizen of the Empire. I was a commander in the imperial armies. But I am not imperial.", null), null, null)
				.NpcLine(new TextObject("{=R5kLv5kg}I am what they call Palaic. Palaic is a language that is no longer spoken, except by a few old people. Even the word 'Palaic' is imperial. We are a people who have forgotten who we are.", null), null, null)
				.NpcLine(new TextObject("{=cfTiiEEM}The Empire has a genius for destruction - the destruction of languages, traditions, gods. It takes our fortresses, slaughters our men, and turns our children into its own children.", null), null, null)
				.NpcLine(new TextObject("{=qoA4UPly}Nothing can bring the Palaic people back. They are now imperial. But it is an insult to our name, to our gods, to our memory, that the state which destroyed our shrines and fortresses should last and thrive.", null), null, null)
				.NpcLine(new TextObject("{=rMem50oz}I have vowed that this Empire shall not survive this civil war, if I can do anything to stop it. And believe me, if I had that banner, there is very much something I could do.", null), null, null)
				.Consequence(new ConversationSentence.OnConsequenceDelegate(this.ActivateAssembleTheBannerQuest))
				.BeginPlayerOptions()
				.PlayerOption(new TextObject("{=tkXKef0Z}I too would see the empire destroyed.", null), null)
				.NpcLine(new TextObject("{=1CLNFbzR}Good. Then I will tell you what I know. I heard about one other piece.", null), null, null)
				.NpcLine(new TextObject("{=4WZ9zJbF}I do not know where the other pieces are, you may need to keep searching for them.", null), null, null)
				.NpcLine(new TextObject("{=TcRH7abK}When you have recovered all pieces, return to me and I'll help you put them to use.", null), null, null)
				.Consequence(delegate
				{
					Campaign.Current.ConversationManager.ConversationEndOneShot += base.CompleteQuestWithSuccess;
				})
				.CloseDialog()
				.PlayerOption(new TextObject("{=gdgbaMOP}I am not sure I share your views.", null), null)
				.Consequence(delegate
				{
					this._metAntiImperialMentor = true;
				})
				.NpcLine(new TextObject("{=7ULaG8aT}Then you can come back when you made your mind up.", null), null, null)
				.CloseDialog()
				.EndPlayerOptions()
				.CloseDialog(), this);
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("lord_start", 110).NpcLine(new TextObject("{=oaSTbNwz}So have you made up your mind now?", null), null, null).Condition(() => Hero.OneToOneConversationHero != null && Hero.OneToOneConversationHero == StoryModeHeroes.AntiImperialMentor && this._metAntiImperialMentor)
				.BeginPlayerOptions()
				.PlayerOption(new TextObject("{=upyNhwZ9}Yes, I intend to use the banner to help destroy the empire.", null), null)
				.NpcLine(new TextObject("{=1CLNFbzR}Good. Then I will tell you what I know. I heard about one other piece.", null), null, null)
				.NpcLine(new TextObject("{=4WZ9zJbF}I do not know where the other pieces are, you may need to keep searching for them.", null), null, null)
				.NpcLine(new TextObject("{=TcRH7abK}When you have recovered all pieces, return to me and I'll help you put them to use.", null), null, null)
				.Consequence(delegate
				{
					Campaign.Current.ConversationManager.ConversationEndOneShot += base.CompleteQuestWithSuccess;
				})
				.CloseDialog()
				.PlayerOption(new TextObject("{=ibm9EEPa}No, I need more time to decide.", null), null)
				.NpcLine(new TextObject("{=7ULaG8aT}Then you can come back when you made your mind up.", null), null, null)
				.CloseDialog()
				.EndPlayerOptions()
				.CloseDialog(), this);
		}

		// Token: 0x06000307 RID: 775 RVA: 0x00010BF8 File Offset: 0x0000EDF8
		private void ActivateAssembleTheBannerQuest()
		{
			if (!Campaign.Current.QuestManager.Quests.Any((QuestBase q) => q is AssembleTheBannerQuest))
			{
				new AssembleTheBannerQuest().StartQuest();
			}
		}

		// Token: 0x06000308 RID: 776 RVA: 0x00010C44 File Offset: 0x0000EE44
		protected override void OnCompleteWithSuccess()
		{
			base.OnCompleteWithSuccess();
			base.AddLog(this._endQuestLog, false);
		}

		// Token: 0x06000309 RID: 777 RVA: 0x00010C5A File Offset: 0x0000EE5A
		internal static void AutoGeneratedStaticCollectObjectsMeetWithArzagosQuest(object o, List<object> collectedObjects)
		{
			((MeetWithArzagosQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x0600030A RID: 778 RVA: 0x00010C68 File Offset: 0x0000EE68
		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x0600030B RID: 779 RVA: 0x00010C71 File Offset: 0x0000EE71
		internal static object AutoGeneratedGetMemberValue_metAntiImperialMentor(object o)
		{
			return ((MeetWithArzagosQuest)o)._metAntiImperialMentor;
		}

		// Token: 0x040000F9 RID: 249
		[SaveableField(1)]
		private bool _metAntiImperialMentor;
	}
}
