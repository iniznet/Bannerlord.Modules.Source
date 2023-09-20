using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x02000226 RID: 550
	public class FirstMeetingTag : ConversationTag
	{
		// Token: 0x170007AA RID: 1962
		// (get) Token: 0x06001E78 RID: 7800 RVA: 0x000877F2 File Offset: 0x000859F2
		public override string StringId
		{
			get
			{
				return "FirstMeetingTag";
			}
		}

		// Token: 0x06001E79 RID: 7801 RVA: 0x000877F9 File Offset: 0x000859F9
		public override bool IsApplicableTo(CharacterObject character)
		{
			return Campaign.Current.ConversationManager.CurrentConversationIsFirst;
		}

		// Token: 0x040009A9 RID: 2473
		public const string Id = "FirstMeetingTag";
	}
}
