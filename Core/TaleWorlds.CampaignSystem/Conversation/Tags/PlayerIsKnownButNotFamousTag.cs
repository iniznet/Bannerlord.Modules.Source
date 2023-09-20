using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x0200020B RID: 523
	public class PlayerIsKnownButNotFamousTag : ConversationTag
	{
		// Token: 0x1700078F RID: 1935
		// (get) Token: 0x06001E27 RID: 7719 RVA: 0x0008707F File Offset: 0x0008527F
		public override string StringId
		{
			get
			{
				return "PlayerIsKnownButNotFamousTag";
			}
		}

		// Token: 0x06001E28 RID: 7720 RVA: 0x00087088 File Offset: 0x00085288
		public override bool IsApplicableTo(CharacterObject character)
		{
			int num = Campaign.Current.Models.DiplomacyModel.GetBaseRelation(Hero.MainHero, Hero.OneToOneConversationHero);
			if (Hero.OneToOneConversationHero.Clan != null && num == 0)
			{
				num = Campaign.Current.Models.DiplomacyModel.GetBaseRelation(Hero.MainHero, Hero.OneToOneConversationHero.Clan.Leader);
			}
			return num != 0 && Clan.PlayerClan.Renown < 50f && Campaign.Current.ConversationManager.CurrentConversationIsFirst;
		}

		// Token: 0x0400098D RID: 2445
		public const string Id = "PlayerIsKnownButNotFamousTag";
	}
}
