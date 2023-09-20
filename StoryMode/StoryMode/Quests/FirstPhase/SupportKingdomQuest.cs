using System;
using System.Collections.Generic;
using Helpers;
using StoryMode.StoryModeObjects;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.SceneInformationPopupTypes;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace StoryMode.Quests.FirstPhase
{
	// Token: 0x02000036 RID: 54
	public class SupportKingdomQuest : StoryModeQuestBase
	{
		// Token: 0x06000322 RID: 802 RVA: 0x00011204 File Offset: 0x0000F404
		public SupportKingdomQuest(Hero questGiver)
			: base("main_storyline_support_kingdom_quest_" + ((StoryModeHeroes.ImperialMentor == questGiver) ? "1" : "0"), questGiver, StoryModeManager.Current.MainStoryLine.FirstPhase.FirstPhaseEndTime)
		{
			this._isImperial = StoryModeHeroes.ImperialMentor == questGiver;
			this.SetDialogs();
			if (this._isImperial)
			{
				base.AddLog(this._onQuestStartedImperialLogText, false);
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetImperialKingDialogueFlow(), this);
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetImperialMentorDialogueFlow(), this);
			}
			else
			{
				base.AddLog(this._onQuestStartedAntiImperialLogText, false);
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetAntiImperialKingDialogueFlow(), this);
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetAntiImperialMentorDialogueFlow(), this);
			}
			base.InitializeQuestOnCreation();
		}

		// Token: 0x170000BA RID: 186
		// (get) Token: 0x06000323 RID: 803 RVA: 0x000112E2 File Offset: 0x0000F4E2
		public override TextObject Title
		{
			get
			{
				TextObject textObject = new TextObject("{=XtC0hXhr}Support {?IS_IMPERIAL}an imperial faction{?}a non-imperial kingdom{\\?}", null);
				textObject.SetTextVariable("IS_IMPERIAL", this._isImperial ? 1 : 0);
				return textObject;
			}
		}

		// Token: 0x170000BB RID: 187
		// (get) Token: 0x06000324 RID: 804 RVA: 0x00011308 File Offset: 0x0000F508
		private TextObject _onQuestStartedImperialLogText
		{
			get
			{
				TextObject textObject = new TextObject("{=TZZX9kWf}{MENTOR.LINK} suggested that you should support an imperial faction by offering them the Dragon Banner.", null);
				StringHelpers.SetCharacterProperties("MENTOR", StoryModeHeroes.ImperialMentor.CharacterObject, textObject, false);
				return textObject;
			}
		}

		// Token: 0x170000BC RID: 188
		// (get) Token: 0x06000325 RID: 805 RVA: 0x0001133C File Offset: 0x0000F53C
		private TextObject _onQuestStartedAntiImperialLogText
		{
			get
			{
				TextObject textObject = new TextObject("{=4d5SP6B6}{MENTOR.LINK} suggested that you should support an anti-imperial kingdom by offering them the Dragon Banner.", null);
				StringHelpers.SetCharacterProperties("MENTOR", StoryModeHeroes.AntiImperialMentor.CharacterObject, textObject, false);
				return textObject;
			}
		}

		// Token: 0x170000BD RID: 189
		// (get) Token: 0x06000326 RID: 806 RVA: 0x00011370 File Offset: 0x0000F570
		private TextObject _onImperialKingdomSupportedLogText
		{
			get
			{
				TextObject textObject = new TextObject("{=atUTLABh}You have chosen to support the {KINGDOM} by presenting them the Dragon Banner, taking the advice of {MENTOR.LINK}.", null);
				StringHelpers.SetCharacterProperties("MENTOR", StoryModeHeroes.ImperialMentor.CharacterObject, textObject, false);
				textObject.SetTextVariable("KINGDOM", Clan.PlayerClan.Kingdom.EncyclopediaLinkWithName);
				return textObject;
			}
		}

		// Token: 0x170000BE RID: 190
		// (get) Token: 0x06000327 RID: 807 RVA: 0x000113BC File Offset: 0x0000F5BC
		private TextObject _onAntiImperialKingdomSupportedLogText
		{
			get
			{
				TextObject textObject = new TextObject("{=atUTLABh}You have chosen to support the {KINGDOM} by presenting them the Dragon Banner, taking the advice of {MENTOR.LINK}.", null);
				StringHelpers.SetCharacterProperties("MENTOR", StoryModeHeroes.AntiImperialMentor.CharacterObject, textObject, false);
				textObject.SetTextVariable("KINGDOM", Clan.PlayerClan.Kingdom.EncyclopediaLinkWithName);
				return textObject;
			}
		}

		// Token: 0x170000BF RID: 191
		// (get) Token: 0x06000328 RID: 808 RVA: 0x00011408 File Offset: 0x0000F608
		private TextObject _onPlayerRuledKingdomSupportedLogText
		{
			get
			{
				return new TextObject("{=kqj1Wp0f}You have decided to keep the Dragon Banner within the kingdom you are ruling.", null);
			}
		}

		// Token: 0x170000C0 RID: 192
		// (get) Token: 0x06000329 RID: 809 RVA: 0x00011415 File Offset: 0x0000F615
		private TextObject _questFailedLogText
		{
			get
			{
				return new TextObject("{=tVlZTOst}You have chosen a different path.", null);
			}
		}

		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x0600032A RID: 810 RVA: 0x00011422 File Offset: 0x0000F622
		public override bool IsRemainingTimeHidden
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600032B RID: 811 RVA: 0x00011425 File Offset: 0x0000F625
		protected override void SetDialogs()
		{
			this.DiscussDialogFlow = DialogFlow.CreateDialogFlow("quest_discuss", 100).NpcLine(new TextObject("{=9tpTkKdY}Tell me which path you choose when you've made progress.", null), null, null).Condition(() => Hero.OneToOneConversationHero == base.QuestGiver)
				.CloseDialog();
		}

		// Token: 0x0600032C RID: 812 RVA: 0x00011464 File Offset: 0x0000F664
		private DialogFlow GetImperialKingDialogueFlow()
		{
			return DialogFlow.CreateDialogFlow("hero_main_options", 300).BeginPlayerOptions().PlayerSpecialOption(new TextObject("{=Ke7f4XSC}I present you with the Dragon Banner of Calradios.", null), null)
				.ClickableCondition(new ConversationSentence.OnClickableConditionDelegate(this.CheckConditionToSupportKingdom))
				.Condition(() => Hero.OneToOneConversationHero.Clan != null && Hero.OneToOneConversationHero.Clan.Kingdom != null && Hero.OneToOneConversationHero.Clan.Kingdom.Leader == Hero.OneToOneConversationHero && StoryModeData.IsKingdomImperial(Hero.OneToOneConversationHero.Clan.Kingdom))
				.NpcLine("{=PQgzfHLk}Well now. I had heard rumors that you had obtained this great artifact.", null, null)
				.NpcLine("{=ULn7iWlz}It will be a powerful tool in our hands. People will believe that the Heavens intend us to restore the Empire of Calradia.", null, null)
				.NpcLine("{=S1yCTPrL}This is one of the most valuable services anyone has ever done for me. I am very grateful.", null, null)
				.Consequence(delegate
				{
					this.OnKingdomSupported(Hero.OneToOneConversationHero.Clan.Kingdom, true);
					if (PlayerEncounter.Current != null)
					{
						PlayerEncounter.LeaveEncounter = true;
					}
					TextObject textObject = new TextObject("{=IL4FcHXv}You've pledged your allegiance to the {KINGDOM_NAME}!", null);
					textObject.SetTextVariable("KINGDOM_NAME", Hero.OneToOneConversationHero.Clan.Kingdom.Name);
					MBInformationManager.AddQuickInformation(textObject, 0, null, "");
				})
				.CloseDialog()
				.EndPlayerOptions()
				.CloseDialog();
		}

		// Token: 0x0600032D RID: 813 RVA: 0x00011510 File Offset: 0x0000F710
		private DialogFlow GetAntiImperialKingDialogueFlow()
		{
			return DialogFlow.CreateDialogFlow("hero_main_options", 300).BeginPlayerOptions().PlayerSpecialOption(new TextObject("{=Ke7f4XSC}I present you with the Dragon Banner of Calradios.", null), null)
				.ClickableCondition(new ConversationSentence.OnClickableConditionDelegate(this.CheckConditionToSupportKingdom))
				.Condition(() => Hero.OneToOneConversationHero.Clan != null && Hero.OneToOneConversationHero.Clan.Kingdom != null && Hero.OneToOneConversationHero.Clan.Kingdom.Leader == Hero.OneToOneConversationHero && !StoryModeData.IsKingdomImperial(Hero.OneToOneConversationHero.Clan.Kingdom))
				.NpcLine("{=PQgzfHLk}Well now. I had heard rumors that you had obtained this great artifact.", null, null)
				.NpcLine("{=4olAbDTq}It will be a powerful tool in our hands. People will believe that the Heavens have transferred dominion over Calradia from the Empire to us.", null, null)
				.NpcLine("{=S1yCTPrL}This is one of the most valuable services anyone has ever done for me. I am very grateful.", null, null)
				.Consequence(delegate
				{
					this.OnKingdomSupported(Hero.OneToOneConversationHero.Clan.Kingdom, false);
					if (PlayerEncounter.Current != null)
					{
						PlayerEncounter.LeaveEncounter = true;
					}
					TextObject textObject = new TextObject("{=IL4FcHXv}You've pledged your allegiance to the {KINGDOM_NAME}!", null);
					textObject.SetTextVariable("KINGDOM_NAME", Hero.OneToOneConversationHero.Clan.Kingdom.Name);
					MBInformationManager.AddQuickInformation(textObject, 0, null, "");
				})
				.CloseDialog()
				.EndPlayerOptions()
				.CloseDialog();
		}

		// Token: 0x0600032E RID: 814 RVA: 0x000115BC File Offset: 0x0000F7BC
		private DialogFlow GetImperialMentorDialogueFlow()
		{
			return DialogFlow.CreateDialogFlow("hero_main_options", 300).BeginPlayerOptions().PlayerSpecialOption(new TextObject("{=O2BAcMNO}As the legitimate {?PLAYER.GENDER}Empress{?}Emperor{\\?} of Calradia, I am ready to declare my ownership of the Dragon Banner.", null), null)
				.Condition(() => base.IsOngoing && Hero.OneToOneConversationHero == StoryModeHeroes.ImperialMentor)
				.NpcLine("{=ATduKfHu}This will make a great impression. It will attract allies, but also probably make you new enemies. Are you sure you're ready?", null, null)
				.BeginPlayerOptions()
				.PlayerOption("{=n8pmVHNn}Yes, I am ready.", null)
				.ClickableCondition(new ConversationSentence.OnClickableConditionDelegate(this.CheckPlayerCanDeclareBannerOwnershipClickableCondition))
				.NpcLine("{=gL241Hoz}Very nice. Superstitious twaddle, of course, but people will believe you. Very well, oh heir to Calradios, go forth!", null, null)
				.Consequence(delegate
				{
					this.OnKingdomSupported(Clan.PlayerClan.Kingdom, true);
				})
				.CloseDialog()
				.PlayerOption("{=fRMIoPUK}Give me more time.", null)
				.NpcLine("{=KH07mJ5k}Very well, come back when you are ready.", null, null)
				.EndPlayerOptions()
				.CloseDialog()
				.PlayerOption("{=eYXLYgsC}I still am not sure what I will do with it.", null)
				.Condition(() => base.IsOngoing && Hero.OneToOneConversationHero == StoryModeHeroes.ImperialMentor)
				.NpcLine("{=UCoOMWaj}As I said before, there's a case for all of the claimants. When this war began, I thought Rhagaea understood best how to rule, Garios was the strongest warrior, and Lucon had the firmest grasp of our traditions.", null, null)
				.NpcLine("{=uFsMzAuR}Speak to whichever one you choose, or come back to me if you wish to claim the banner for yourself.", null, null)
				.CloseDialog()
				.EndPlayerOptions()
				.NpcLine("{=Z54ZrDG9}Until next time, then.", null, null)
				.CloseDialog();
		}

		// Token: 0x0600032F RID: 815 RVA: 0x000116C0 File Offset: 0x0000F8C0
		private DialogFlow GetAntiImperialMentorDialogueFlow()
		{
			return DialogFlow.CreateDialogFlow("hero_main_options", 300).BeginPlayerOptions().PlayerSpecialOption(new TextObject("{=N5jJtZyr}As the Empire's nemesis, I am ready to declare my ownership of the Dragon Banner.", null), null)
				.Condition(() => base.IsOngoing && Hero.OneToOneConversationHero == StoryModeHeroes.AntiImperialMentor)
				.NpcLine("{=ATduKfHu}This will make a great impression. It will attract allies, but also probably make you new enemies. Are you sure you're ready?", null, null)
				.BeginPlayerOptions()
				.PlayerOption("{=ALWqXMiP}Yes, I am sure.", null)
				.ClickableCondition(new ConversationSentence.OnClickableConditionDelegate(this.CheckPlayerCanDeclareBannerOwnershipClickableCondition))
				.NpcLine("{=exoZygYL}Very well. The Dragon Banner in your hands proclaims you the avenger of the Empire's crimes and its successor. Now go forth and claim your destiny!", null, null)
				.Consequence(delegate
				{
					this.OnKingdomSupported(Clan.PlayerClan.Kingdom, false);
				})
				.CloseDialog()
				.PlayerOption("{=fRMIoPUK}Give me more time.", null)
				.NpcLine("{=KH07mJ5k}Very well, come back when you are ready.", null, null)
				.EndPlayerOptions()
				.CloseDialog()
				.PlayerOption("{=tzsZTcWd}I wonder which kingdom should I support..", null)
				.Condition(() => base.IsOngoing && Hero.OneToOneConversationHero == StoryModeHeroes.AntiImperialMentor)
				.NpcLine("{=1v6aYpDx}You must choose, but choose wisely. Or you can claim it yourself. I have no preference.", null, null)
				.GotoDialogState("hero_main_options")
				.EndPlayerOptions()
				.NpcLine("{=Z54ZrDG9}Until next time, then.", null, null)
				.CloseDialog();
		}

		// Token: 0x06000330 RID: 816 RVA: 0x000117BC File Offset: 0x0000F9BC
		private bool IsPlayerTheRulerOfAKingdom()
		{
			bool flag = Clan.PlayerClan.Kingdom != null && Clan.PlayerClan.Kingdom.Leader == Hero.MainHero && StoryModeData.IsKingdomImperial(Clan.PlayerClan.Kingdom) == this._isImperial;
			if (flag)
			{
				MBTextManager.SetTextVariable("FACTION", Clan.PlayerClan.Kingdom.Name, false);
			}
			return flag;
		}

		// Token: 0x06000331 RID: 817 RVA: 0x00011822 File Offset: 0x0000FA22
		private bool CheckPlayerCanDeclareBannerOwnershipClickableCondition(out TextObject explanation)
		{
			if (this.IsPlayerTheRulerOfAKingdom())
			{
				explanation = TextObject.Empty;
				return true;
			}
			explanation = (this._isImperial ? new TextObject("{=mziMNKm2}You should be ruling a kingdom of the imperial culture.", null) : new TextObject("{=HCA9xOOo}You should be ruling a kingdom of non-imperial culture.", null));
			return false;
		}

		// Token: 0x06000332 RID: 818 RVA: 0x00011858 File Offset: 0x0000FA58
		private bool CheckConditionToSupportKingdom(out TextObject explanation)
		{
			explanation = new TextObject("{=qNR8WKcX}You should join a kingdom before supporting it with the Dragon Banner.", null);
			return Clan.PlayerClan.Kingdom != null && Clan.PlayerClan.Kingdom == Hero.OneToOneConversationHero.Clan.Kingdom;
		}

		// Token: 0x06000333 RID: 819 RVA: 0x00011890 File Offset: 0x0000FA90
		public void OnRulingClanChanged(Kingdom kingdom, Clan newRulingClan)
		{
			if (newRulingClan == Clan.PlayerClan)
			{
				this._playerRuledKingdom = kingdom;
				return;
			}
			if (this._playerRuledKingdom == kingdom)
			{
				this._playerRuledKingdom = null;
			}
		}

		// Token: 0x06000334 RID: 820 RVA: 0x000118B4 File Offset: 0x0000FAB4
		private void OnKingdomSupported(Kingdom kingdom, bool isImperial)
		{
			if (isImperial)
			{
				if (this._playerRuledKingdom != null && this._playerRuledKingdom == kingdom)
				{
					base.AddLog(this._onPlayerRuledKingdomSupportedLogText, false);
					StoryModeManager.Current.MainStoryLine.SetStoryLineSide(MainStoryLineSide.CreateImperialKingdom);
					MBInformationManager.ShowSceneNotification(new DeclareDragonBannerSceneNotificationItem(true));
				}
				else
				{
					base.AddLog(this._onImperialKingdomSupportedLogText, false);
					StoryModeManager.Current.MainStoryLine.SetStoryLineSide(MainStoryLineSide.SupportImperialKingdom);
					MBInformationManager.ShowSceneNotification(new PledgeAllegianceSceneNotificationItem(Hero.MainHero, true));
				}
			}
			else if (this._playerRuledKingdom != null && this._playerRuledKingdom == kingdom)
			{
				base.AddLog(this._onPlayerRuledKingdomSupportedLogText, false);
				StoryModeManager.Current.MainStoryLine.SetStoryLineSide(MainStoryLineSide.CreateAntiImperialKingdom);
				MBInformationManager.ShowSceneNotification(new DeclareDragonBannerSceneNotificationItem(false));
			}
			else
			{
				base.AddLog(this._onAntiImperialKingdomSupportedLogText, false);
				StoryModeManager.Current.MainStoryLine.SetStoryLineSide(MainStoryLineSide.SupportAntiImperialKingdom);
				MBInformationManager.ShowSceneNotification(new PledgeAllegianceSceneNotificationItem(Hero.MainHero, false));
			}
			base.CompleteQuestWithSuccess();
		}

		// Token: 0x06000335 RID: 821 RVA: 0x000119A3 File Offset: 0x0000FBA3
		private void MainStoryLineChosen(MainStoryLineSide chosenSide)
		{
			if ((this._isImperial && chosenSide != MainStoryLineSide.SupportImperialKingdom && chosenSide != MainStoryLineSide.CreateImperialKingdom) || (!this._isImperial && chosenSide != MainStoryLineSide.SupportAntiImperialKingdom && chosenSide != MainStoryLineSide.CreateAntiImperialKingdom))
			{
				base.AddLog(this._questFailedLogText, false);
				base.CompleteQuestWithFail(null);
			}
		}

		// Token: 0x06000336 RID: 822 RVA: 0x000119DA File Offset: 0x0000FBDA
		protected override void RegisterEvents()
		{
			StoryModeEvents.OnMainStoryLineSideChosenEvent.AddNonSerializedListener(this, new Action<MainStoryLineSide>(this.MainStoryLineChosen));
			CampaignEvents.RulingClanChanged.AddNonSerializedListener(this, new Action<Kingdom, Clan>(this.OnRulingClanChanged));
		}

		// Token: 0x06000337 RID: 823 RVA: 0x00011A0C File Offset: 0x0000FC0C
		protected override void InitializeQuestOnGameLoad()
		{
			this.SetDialogs();
			if (this._isImperial)
			{
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetImperialKingDialogueFlow(), this);
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetImperialMentorDialogueFlow(), this);
				return;
			}
			Campaign.Current.ConversationManager.AddDialogFlow(this.GetAntiImperialKingDialogueFlow(), this);
			Campaign.Current.ConversationManager.AddDialogFlow(this.GetAntiImperialMentorDialogueFlow(), this);
		}

		// Token: 0x06000338 RID: 824 RVA: 0x00011A80 File Offset: 0x0000FC80
		internal static void AutoGeneratedStaticCollectObjectsSupportKingdomQuest(object o, List<object> collectedObjects)
		{
			((SupportKingdomQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x06000339 RID: 825 RVA: 0x00011A8E File Offset: 0x0000FC8E
		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this._playerRuledKingdom);
		}

		// Token: 0x0600033A RID: 826 RVA: 0x00011AA3 File Offset: 0x0000FCA3
		internal static object AutoGeneratedGetMemberValue_isImperial(object o)
		{
			return ((SupportKingdomQuest)o)._isImperial;
		}

		// Token: 0x0600033B RID: 827 RVA: 0x00011AB5 File Offset: 0x0000FCB5
		internal static object AutoGeneratedGetMemberValue_playerRuledKingdom(object o)
		{
			return ((SupportKingdomQuest)o)._playerRuledKingdom;
		}

		// Token: 0x040000FB RID: 251
		[SaveableField(1)]
		private bool _isImperial;

		// Token: 0x040000FC RID: 252
		[SaveableField(2)]
		private Kingdom _playerRuledKingdom;
	}
}
