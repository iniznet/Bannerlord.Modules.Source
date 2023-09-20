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
	public class SupportKingdomQuest : StoryModeQuestBase
	{
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

		public override TextObject Title
		{
			get
			{
				TextObject textObject = new TextObject("{=XtC0hXhr}Support {?IS_IMPERIAL}an imperial faction{?}a non-imperial kingdom{\\?}", null);
				textObject.SetTextVariable("IS_IMPERIAL", this._isImperial ? 1 : 0);
				return textObject;
			}
		}

		private TextObject _onQuestStartedImperialLogText
		{
			get
			{
				TextObject textObject = new TextObject("{=TZZX9kWf}{MENTOR.LINK} suggested that you should support an imperial faction by offering them the Dragon Banner.", null);
				StringHelpers.SetCharacterProperties("MENTOR", StoryModeHeroes.ImperialMentor.CharacterObject, textObject, false);
				return textObject;
			}
		}

		private TextObject _onQuestStartedAntiImperialLogText
		{
			get
			{
				TextObject textObject = new TextObject("{=4d5SP6B6}{MENTOR.LINK} suggested that you should support an anti-imperial kingdom by offering them the Dragon Banner.", null);
				StringHelpers.SetCharacterProperties("MENTOR", StoryModeHeroes.AntiImperialMentor.CharacterObject, textObject, false);
				return textObject;
			}
		}

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

		private TextObject _onPlayerRuledKingdomSupportedLogText
		{
			get
			{
				return new TextObject("{=kqj1Wp0f}You have decided to keep the Dragon Banner within the kingdom you are ruling.", null);
			}
		}

		private TextObject _questFailedLogText
		{
			get
			{
				return new TextObject("{=tVlZTOst}You have chosen a different path.", null);
			}
		}

		public override bool IsRemainingTimeHidden
		{
			get
			{
				return false;
			}
		}

		protected override void SetDialogs()
		{
			this.DiscussDialogFlow = DialogFlow.CreateDialogFlow("quest_discuss", 100).NpcLine(new TextObject("{=9tpTkKdY}Tell me which path you choose when you've made progress.", null), null, null).Condition(() => Hero.OneToOneConversationHero == base.QuestGiver)
				.CloseDialog();
		}

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

		private bool IsPlayerTheRulerOfAKingdom()
		{
			bool flag = Clan.PlayerClan.Kingdom != null && Clan.PlayerClan.Kingdom.Leader == Hero.MainHero && StoryModeData.IsKingdomImperial(Clan.PlayerClan.Kingdom) == this._isImperial;
			if (flag)
			{
				MBTextManager.SetTextVariable("FACTION", Clan.PlayerClan.Kingdom.Name, false);
			}
			return flag;
		}

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

		private bool CheckConditionToSupportKingdom(out TextObject explanation)
		{
			explanation = new TextObject("{=qNR8WKcX}You should join a kingdom before supporting it with the Dragon Banner.", null);
			return Clan.PlayerClan.Kingdom != null && Clan.PlayerClan.Kingdom == Hero.OneToOneConversationHero.Clan.Kingdom;
		}

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

		private void MainStoryLineChosen(MainStoryLineSide chosenSide)
		{
			if ((this._isImperial && chosenSide != MainStoryLineSide.SupportImperialKingdom && chosenSide != MainStoryLineSide.CreateImperialKingdom) || (!this._isImperial && chosenSide != MainStoryLineSide.SupportAntiImperialKingdom && chosenSide != MainStoryLineSide.CreateAntiImperialKingdom))
			{
				base.AddLog(this._questFailedLogText, false);
				base.CompleteQuestWithFail(null);
			}
		}

		protected override void RegisterEvents()
		{
			StoryModeEvents.OnMainStoryLineSideChosenEvent.AddNonSerializedListener(this, new Action<MainStoryLineSide>(this.MainStoryLineChosen));
			CampaignEvents.RulingClanChanged.AddNonSerializedListener(this, new Action<Kingdom, Clan>(this.OnRulingClanChanged));
		}

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

		internal static void AutoGeneratedStaticCollectObjectsSupportKingdomQuest(object o, List<object> collectedObjects)
		{
			((SupportKingdomQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this._playerRuledKingdom);
		}

		internal static object AutoGeneratedGetMemberValue_isImperial(object o)
		{
			return ((SupportKingdomQuest)o)._isImperial;
		}

		internal static object AutoGeneratedGetMemberValue_playerRuledKingdom(object o)
		{
			return ((SupportKingdomQuest)o)._playerRuledKingdom;
		}

		[SaveableField(1)]
		private bool _isImperial;

		[SaveableField(2)]
		private Kingdom _playerRuledKingdom;
	}
}
