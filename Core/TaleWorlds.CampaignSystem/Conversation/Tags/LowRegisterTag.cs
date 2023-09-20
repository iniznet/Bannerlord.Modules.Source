using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class LowRegisterTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "LowRegisterTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && !ConversationTagHelper.UsesHighRegister(character) && ConversationTagHelper.UsesLowRegister(character);
		}

		public const string Id = "LowRegisterTag";
	}
}
