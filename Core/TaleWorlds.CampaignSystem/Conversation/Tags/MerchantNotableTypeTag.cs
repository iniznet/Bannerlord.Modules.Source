using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class MerchantNotableTypeTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "MerchantNotableTypeTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.Occupation == Occupation.Merchant;
		}

		public const string Id = "MerchantNotableTypeTag";
	}
}
