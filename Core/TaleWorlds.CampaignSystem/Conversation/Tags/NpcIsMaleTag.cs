using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x0200020F RID: 527
	public class NpcIsMaleTag : ConversationTag
	{
		// Token: 0x17000793 RID: 1939
		// (get) Token: 0x06001E33 RID: 7731 RVA: 0x00087191 File Offset: 0x00085391
		public override string StringId
		{
			get
			{
				return "NpcIsMaleTag";
			}
		}

		// Token: 0x06001E34 RID: 7732 RVA: 0x00087198 File Offset: 0x00085398
		public override bool IsApplicableTo(CharacterObject character)
		{
			return !character.IsFemale;
		}

		// Token: 0x04000991 RID: 2449
		public const string Id = "NpcIsMaleTag";
	}
}
