using System;
using StoryMode.StoryModeObjects;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation.Tags;

namespace StoryMode
{
	public class IsArzagosTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "IsArzagosTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return StoryModeHeroes.AntiImperialMentor.CharacterObject == character;
		}

		public const string Id = "IsArzagosTag";
	}
}
