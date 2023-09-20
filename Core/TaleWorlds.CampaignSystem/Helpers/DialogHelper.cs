using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;

namespace Helpers
{
	// Token: 0x02000014 RID: 20
	public static class DialogHelper
	{
		// Token: 0x060000CC RID: 204 RVA: 0x0000A993 File Offset: 0x00008B93
		public static void SetDialogString(string stringVariable, string gameTextId)
		{
			MBTextManager.SetTextVariable(stringVariable, Campaign.Current.ConversationManager.FindMatchingTextOrNull(gameTextId, CharacterObject.OneToOneConversationCharacter), false);
		}
	}
}
