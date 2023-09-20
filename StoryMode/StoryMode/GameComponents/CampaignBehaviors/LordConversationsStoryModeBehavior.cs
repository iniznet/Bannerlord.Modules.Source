using System;
using Helpers;
using StoryMode.StoryModeObjects;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;

namespace StoryMode.GameComponents.CampaignBehaviors
{
	// Token: 0x0200004C RID: 76
	public class LordConversationsStoryModeBehavior : CampaignBehaviorBase
	{
		// Token: 0x06000429 RID: 1065 RVA: 0x00019487 File Offset: 0x00017687
		public override void RegisterEvents()
		{
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
		}

		// Token: 0x0600042A RID: 1066 RVA: 0x000194A0 File Offset: 0x000176A0
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x0600042B RID: 1067 RVA: 0x000194A2 File Offset: 0x000176A2
		private void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
		{
			this.AddDialogs(campaignGameStarter);
		}

		// Token: 0x0600042C RID: 1068 RVA: 0x000194AC File Offset: 0x000176AC
		private void AddDialogs(CampaignGameStarter starter)
		{
			starter.AddDialogLine("anti_imperial_mentor_introduction", "lord_introduction", "lord_start", "{=TB20aFsf}You probably are aware that I am {CONVERSATION_HERO.FIRSTNAME}. I am not sure why you have sought me out, but know that my old life, as imperial lap-dog, is over.", new ConversationSentence.OnConditionDelegate(this.conversation_anti_imperial_mentor_introduction_on_condition), null, 150, null);
			starter.AddDialogLine("imperial_mentor_introduction", "lord_introduction", "lord_start", "{=6aDiS9eP}I am {CONVERSATION_HERO.FIRSTNAME}. You probably already know that, though. Once I wielded great power, but now... Anyway, I am most curious what you might want with me.", new ConversationSentence.OnConditionDelegate(this.conversation_imperial_mentor_introduction_on_condition), null, 150, null);
			starter.AddDialogLine("start_default_for_mentors", "start", "lord_start", "{=!}{PLAYER.NAME}...", new ConversationSentence.OnConditionDelegate(this.start_default_for_mentors_on_condition), null, 150, null);
		}

		// Token: 0x0600042D RID: 1069 RVA: 0x00019543 File Offset: 0x00017743
		private bool conversation_imperial_mentor_introduction_on_condition()
		{
			if (Campaign.Current.ConversationManager.CurrentConversationIsFirst && Hero.OneToOneConversationHero == StoryModeHeroes.ImperialMentor)
			{
				StringHelpers.SetCharacterProperties("CONVERSATION_HERO", CharacterObject.OneToOneConversationCharacter, null, false);
				return true;
			}
			return false;
		}

		// Token: 0x0600042E RID: 1070 RVA: 0x00019577 File Offset: 0x00017777
		private bool conversation_anti_imperial_mentor_introduction_on_condition()
		{
			if (Campaign.Current.ConversationManager.CurrentConversationIsFirst && Hero.OneToOneConversationHero == StoryModeHeroes.AntiImperialMentor)
			{
				StringHelpers.SetCharacterProperties("CONVERSATION_HERO", CharacterObject.OneToOneConversationCharacter, null, false);
				return true;
			}
			return false;
		}

		// Token: 0x0600042F RID: 1071 RVA: 0x000195AB File Offset: 0x000177AB
		private bool start_default_for_mentors_on_condition()
		{
			return Hero.OneToOneConversationHero != null && Hero.OneToOneConversationHero.HasMet && (Hero.OneToOneConversationHero == StoryModeHeroes.AntiImperialMentor || Hero.OneToOneConversationHero == StoryModeHeroes.ImperialMentor);
		}
	}
}
