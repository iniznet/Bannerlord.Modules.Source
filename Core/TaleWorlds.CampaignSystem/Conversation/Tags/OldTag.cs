using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x02000221 RID: 545
	public class OldTag : ConversationTag
	{
		// Token: 0x170007A5 RID: 1957
		// (get) Token: 0x06001E69 RID: 7785 RVA: 0x0008769F File Offset: 0x0008589F
		public override string StringId
		{
			get
			{
				return "OldTag";
			}
		}

		// Token: 0x06001E6A RID: 7786 RVA: 0x000876A6 File Offset: 0x000858A6
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.Age > 49f;
		}

		// Token: 0x040009A4 RID: 2468
		public const string Id = "OldTag";
	}
}
