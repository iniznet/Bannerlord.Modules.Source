using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x0200021D RID: 541
	public class MetBeforeTag : ConversationTag
	{
		// Token: 0x170007A1 RID: 1953
		// (get) Token: 0x06001E5D RID: 7773 RVA: 0x00087594 File Offset: 0x00085794
		public override string StringId
		{
			get
			{
				return "MetBeforeTag";
			}
		}

		// Token: 0x06001E5E RID: 7774 RVA: 0x0008759B File Offset: 0x0008579B
		public override bool IsApplicableTo(CharacterObject character)
		{
			return !Campaign.Current.ConversationManager.CurrentConversationIsFirst;
		}

		// Token: 0x0400099F RID: 2463
		public const string Id = "MetBeforeTag";
	}
}
