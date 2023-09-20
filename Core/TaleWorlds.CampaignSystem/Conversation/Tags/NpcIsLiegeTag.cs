using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class NpcIsLiegeTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "NpcIsLiegeTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.HeroObject.IsFactionLeader;
		}

		public const string Id = "NpcIsLiegeTag";
	}
}
