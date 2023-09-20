using System;
using System.Linq;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class PlayerIsNobleTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "PlayerIsNobleTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return Settlement.All.Any((Settlement x) => x.OwnerClan == Hero.MainHero.Clan);
		}

		public const string Id = "PlayerIsNobleTag";
	}
}
