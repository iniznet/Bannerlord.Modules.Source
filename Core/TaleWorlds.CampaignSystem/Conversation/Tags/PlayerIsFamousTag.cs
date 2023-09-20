using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x0200020A RID: 522
	public class PlayerIsFamousTag : ConversationTag
	{
		// Token: 0x1700078E RID: 1934
		// (get) Token: 0x06001E24 RID: 7716 RVA: 0x0008705A File Offset: 0x0008525A
		public override string StringId
		{
			get
			{
				return "PlayerIsFamousTag";
			}
		}

		// Token: 0x06001E25 RID: 7717 RVA: 0x00087061 File Offset: 0x00085261
		public override bool IsApplicableTo(CharacterObject character)
		{
			return Clan.PlayerClan.Renown >= 50f;
		}

		// Token: 0x0400098C RID: 2444
		public const string Id = "PlayerIsFamousTag";
	}
}
