using System;
using System.Collections.Generic;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.Conversation
{
	// Token: 0x020001EA RID: 490
	public class ConversationAnimData
	{
		// Token: 0x06001D38 RID: 7480 RVA: 0x0008382B File Offset: 0x00081A2B
		public ConversationAnimData()
		{
			this.Reactions = new Dictionary<string, string>();
		}

		// Token: 0x04000910 RID: 2320
		[SaveableField(0)]
		public string IdleAnimStart;

		// Token: 0x04000911 RID: 2321
		[SaveableField(1)]
		public string IdleAnimLoop;

		// Token: 0x04000912 RID: 2322
		[SaveableField(2)]
		public int FamilyType;

		// Token: 0x04000913 RID: 2323
		[SaveableField(3)]
		public int MountFamilyType;

		// Token: 0x04000914 RID: 2324
		[SaveableField(4)]
		public Dictionary<string, string> Reactions;
	}
}
