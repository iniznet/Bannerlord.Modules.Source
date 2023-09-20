using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x02000233 RID: 563
	public class KhuzaitTag : ConversationTag
	{
		// Token: 0x170007B7 RID: 1975
		// (get) Token: 0x06001E9F RID: 7839 RVA: 0x00087A2D File Offset: 0x00085C2D
		public override string StringId
		{
			get
			{
				return "KhuzaitTag";
			}
		}

		// Token: 0x06001EA0 RID: 7840 RVA: 0x00087A34 File Offset: 0x00085C34
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.Culture.StringId == "khuzait";
		}

		// Token: 0x040009B6 RID: 2486
		public const string Id = "KhuzaitTag";
	}
}
