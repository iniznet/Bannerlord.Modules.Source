using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x02000201 RID: 513
	public class PlayerIsAffiliatedTag : ConversationTag
	{
		// Token: 0x17000785 RID: 1925
		// (get) Token: 0x06001E09 RID: 7689 RVA: 0x00086E2A File Offset: 0x0008502A
		public override string StringId
		{
			get
			{
				return "PlayerIsAffiliatedTag";
			}
		}

		// Token: 0x06001E0A RID: 7690 RVA: 0x00086E31 File Offset: 0x00085031
		public override bool IsApplicableTo(CharacterObject character)
		{
			return Hero.MainHero.MapFaction.IsKingdomFaction;
		}

		// Token: 0x04000983 RID: 2435
		public const string Id = "PlayerIsAffiliatedTag";
	}
}
