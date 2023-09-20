using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class ChivalrousTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "ChivalrousTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.GetTraitLevel(DefaultTraits.Honor) + character.GetTraitLevel(DefaultTraits.Valor) > 0;
		}

		public const string Id = "ChivalrousTag";
	}
}
