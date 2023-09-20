using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x0200020E RID: 526
	public class NpcIsFemaleTag : ConversationTag
	{
		// Token: 0x17000792 RID: 1938
		// (get) Token: 0x06001E30 RID: 7728 RVA: 0x0008717A File Offset: 0x0008537A
		public override string StringId
		{
			get
			{
				return "NpcIsFemaleTag";
			}
		}

		// Token: 0x06001E31 RID: 7729 RVA: 0x00087181 File Offset: 0x00085381
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsFemale;
		}

		// Token: 0x04000990 RID: 2448
		public const string Id = "NpcIsFemaleTag";
	}
}
