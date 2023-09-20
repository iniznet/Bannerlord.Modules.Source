using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class GangLeaderNotableTypeTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "GangLeaderNotableTypeTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.Occupation == Occupation.GangLeader;
		}

		public const string Id = "GangLeaderNotableTypeTag";
	}
}
