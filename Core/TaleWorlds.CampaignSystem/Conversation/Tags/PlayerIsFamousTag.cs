using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class PlayerIsFamousTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "PlayerIsFamousTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return Clan.PlayerClan.Renown >= 50f;
		}

		public const string Id = "PlayerIsFamousTag";
	}
}
