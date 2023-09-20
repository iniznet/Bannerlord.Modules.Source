using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x020001F5 RID: 501
	public class DefaultTag : ConversationTag
	{
		// Token: 0x17000779 RID: 1913
		// (get) Token: 0x06001DE5 RID: 7653 RVA: 0x00086B74 File Offset: 0x00084D74
		public override string StringId
		{
			get
			{
				return "DefaultTag";
			}
		}

		// Token: 0x06001DE6 RID: 7654 RVA: 0x00086B7B File Offset: 0x00084D7B
		public override bool IsApplicableTo(CharacterObject character)
		{
			return true;
		}

		// Token: 0x04000977 RID: 2423
		public const string Id = "DefaultTag";
	}
}
