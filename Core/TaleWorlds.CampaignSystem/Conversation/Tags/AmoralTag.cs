using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class AmoralTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "AmoralTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.GetTraitLevel(DefaultTraits.Honor) + character.GetTraitLevel(DefaultTraits.Mercy) < 0;
		}

		public const string Id = "AmoralTag";
	}
}
