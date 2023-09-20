using System;
using StoryMode.StoryModeObjects;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation.Tags;

namespace StoryMode
{
	public class IsStoryModeMentorTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "IsStoryModeMentorTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return StoryModeHeroes.AntiImperialMentor.CharacterObject == character || StoryModeHeroes.ImperialMentor.CharacterObject == character;
		}

		public const string Id = "IsStoryModeMentorTag";
	}
}
