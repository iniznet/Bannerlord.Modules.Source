using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x020001F4 RID: 500
	public abstract class ConversationTag
	{
		// Token: 0x17000778 RID: 1912
		// (get) Token: 0x06001DE1 RID: 7649
		public abstract string StringId { get; }

		// Token: 0x06001DE2 RID: 7650
		public abstract bool IsApplicableTo(CharacterObject character);

		// Token: 0x06001DE3 RID: 7651 RVA: 0x00086B64 File Offset: 0x00084D64
		public override string ToString()
		{
			return this.StringId;
		}
	}
}
