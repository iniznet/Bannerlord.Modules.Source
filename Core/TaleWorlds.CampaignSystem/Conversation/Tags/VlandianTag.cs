using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x02000232 RID: 562
	public class VlandianTag : ConversationTag
	{
		// Token: 0x170007B6 RID: 1974
		// (get) Token: 0x06001E9C RID: 7836 RVA: 0x00087A07 File Offset: 0x00085C07
		public override string StringId
		{
			get
			{
				return "VlandianTag";
			}
		}

		// Token: 0x06001E9D RID: 7837 RVA: 0x00087A0E File Offset: 0x00085C0E
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.Culture.StringId == "vlandia";
		}

		// Token: 0x040009B5 RID: 2485
		public const string Id = "VlandianTag";
	}
}
