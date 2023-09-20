using System;

namespace TaleWorlds.CampaignSystem.Conversation
{
	// Token: 0x020001F0 RID: 496
	public interface IConversationStateHandler
	{
		// Token: 0x06001DB3 RID: 7603
		void OnConversationInstall();

		// Token: 0x06001DB4 RID: 7604
		void OnConversationUninstall();

		// Token: 0x06001DB5 RID: 7605
		void OnConversationActivate();

		// Token: 0x06001DB6 RID: 7606
		void OnConversationDeactivate();

		// Token: 0x06001DB7 RID: 7607
		void OnConversationContinue();

		// Token: 0x06001DB8 RID: 7608
		void ExecuteConversationContinue();
	}
}
