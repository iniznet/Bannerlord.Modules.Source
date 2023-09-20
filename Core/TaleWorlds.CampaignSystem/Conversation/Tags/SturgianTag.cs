using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x02000235 RID: 565
	public class SturgianTag : ConversationTag
	{
		// Token: 0x170007B9 RID: 1977
		// (get) Token: 0x06001EA5 RID: 7845 RVA: 0x00087A79 File Offset: 0x00085C79
		public override string StringId
		{
			get
			{
				return "SturgianTag";
			}
		}

		// Token: 0x06001EA6 RID: 7846 RVA: 0x00087A80 File Offset: 0x00085C80
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.Culture.StringId == "sturgia";
		}

		// Token: 0x040009B8 RID: 2488
		public const string Id = "SturgianTag";
	}
}
