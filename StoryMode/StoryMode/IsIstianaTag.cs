using System;
using StoryMode.StoryModeObjects;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation.Tags;

namespace StoryMode
{
	public class IsIstianaTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "IsIstianaTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return StoryModeHeroes.ImperialMentor.CharacterObject == character;
		}

		public const string Id = "IsIstianaTag";
	}
}
