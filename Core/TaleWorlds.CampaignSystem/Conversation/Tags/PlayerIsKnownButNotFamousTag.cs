using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class PlayerIsKnownButNotFamousTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "PlayerIsKnownButNotFamousTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			int num = Campaign.Current.Models.DiplomacyModel.GetBaseRelation(Hero.MainHero, Hero.OneToOneConversationHero);
			if (Hero.OneToOneConversationHero.Clan != null && num == 0)
			{
				num = Campaign.Current.Models.DiplomacyModel.GetBaseRelation(Hero.MainHero, Hero.OneToOneConversationHero.Clan.Leader);
			}
			return num != 0 && Clan.PlayerClan.Renown < 50f && Campaign.Current.ConversationManager.CurrentConversationIsFirst;
		}

		public const string Id = "PlayerIsKnownButNotFamousTag";
	}
}
