using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x02000234 RID: 564
	public class AseraiTag : ConversationTag
	{
		// Token: 0x170007B8 RID: 1976
		// (get) Token: 0x06001EA2 RID: 7842 RVA: 0x00087A53 File Offset: 0x00085C53
		public override string StringId
		{
			get
			{
				return "AseraiTag";
			}
		}

		// Token: 0x06001EA3 RID: 7843 RVA: 0x00087A5A File Offset: 0x00085C5A
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.Culture.StringId == "aserai";
		}

		// Token: 0x040009B7 RID: 2487
		public const string Id = "AseraiTag";
	}
}
