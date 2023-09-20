using System;

namespace TaleWorlds.CampaignSystem.Conversation
{
	// Token: 0x020001E8 RID: 488
	public static class CampaignMapConversation
	{
		// Token: 0x06001D2D RID: 7469 RVA: 0x000837E0 File Offset: 0x000819E0
		public static void OpenConversation(ConversationCharacterData playerCharacterData, ConversationCharacterData conversationPartnerData)
		{
			Campaign.Current.ConversationManager.OpenMapConversation(playerCharacterData, conversationPartnerData);
		}
	}
}
