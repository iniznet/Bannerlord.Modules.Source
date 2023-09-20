using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x02000231 RID: 561
	public class BattanianTag : ConversationTag
	{
		// Token: 0x170007B5 RID: 1973
		// (get) Token: 0x06001E99 RID: 7833 RVA: 0x000879E1 File Offset: 0x00085BE1
		public override string StringId
		{
			get
			{
				return "BattanianTag";
			}
		}

		// Token: 0x06001E9A RID: 7834 RVA: 0x000879E8 File Offset: 0x00085BE8
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.Culture.StringId == "battania";
		}

		// Token: 0x040009B4 RID: 2484
		public const string Id = "BattanianTag";
	}
}
