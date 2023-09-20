using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class HighRegisterTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "HighRegisterTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && ConversationTagHelper.UsesHighRegister(character);
		}

		public const string Id = "HighRegisterTag";
	}
}
