using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x0200020D RID: 525
	public class PlayerIsRulerTag : ConversationTag
	{
		// Token: 0x17000791 RID: 1937
		// (get) Token: 0x06001E2D RID: 7725 RVA: 0x00087153 File Offset: 0x00085353
		public override string StringId
		{
			get
			{
				return "PlayerIsRulerTag";
			}
		}

		// Token: 0x06001E2E RID: 7726 RVA: 0x0008715A File Offset: 0x0008535A
		public override bool IsApplicableTo(CharacterObject character)
		{
			return Hero.MainHero.Clan.Leader == Hero.MainHero;
		}

		// Token: 0x0400098F RID: 2447
		public const string Id = "PlayerIsRulerTag";
	}
}
