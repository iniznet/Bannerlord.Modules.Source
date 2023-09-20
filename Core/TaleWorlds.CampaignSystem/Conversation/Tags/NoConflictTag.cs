using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class NoConflictTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "NoConflictTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			bool flag = new HostileRelationshipTag().IsApplicableTo(character);
			bool flag2 = new PlayerIsEnemyTag().IsApplicableTo(character);
			return !flag && !flag2;
		}

		public const string Id = "NoConflictTag";
	}
}
