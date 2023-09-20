using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x0200021C RID: 540
	public class CurrentConversationIsFirst : ConversationTag
	{
		// Token: 0x170007A0 RID: 1952
		// (get) Token: 0x06001E5A RID: 7770 RVA: 0x00087574 File Offset: 0x00085774
		public override string StringId
		{
			get
			{
				return "CurrentConversationIsFirst";
			}
		}

		// Token: 0x06001E5B RID: 7771 RVA: 0x0008757B File Offset: 0x0008577B
		public override bool IsApplicableTo(CharacterObject character)
		{
			return Campaign.Current.ConversationManager.CurrentConversationIsFirst;
		}

		// Token: 0x0400099E RID: 2462
		public const string Id = "CurrentConversationIsFirst";
	}
}
