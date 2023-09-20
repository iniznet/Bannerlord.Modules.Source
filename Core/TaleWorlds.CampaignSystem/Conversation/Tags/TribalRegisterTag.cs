using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class TribalRegisterTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "TribalRegisterTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return !ConversationTagHelper.UsesHighRegister(character) && !ConversationTagHelper.UsesLowRegister(character);
		}

		public const string Id = "TribalRegisterTag";
	}
}
