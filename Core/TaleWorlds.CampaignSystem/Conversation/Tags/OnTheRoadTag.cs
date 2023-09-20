using System;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x02000223 RID: 547
	public class OnTheRoadTag : ConversationTag
	{
		// Token: 0x170007A7 RID: 1959
		// (get) Token: 0x06001E6F RID: 7791 RVA: 0x00087705 File Offset: 0x00085905
		public override string StringId
		{
			get
			{
				return "OnTheRoadTag";
			}
		}

		// Token: 0x06001E70 RID: 7792 RVA: 0x0008770C File Offset: 0x0008590C
		public override bool IsApplicableTo(CharacterObject character)
		{
			return Settlement.CurrentSettlement == null;
		}

		// Token: 0x040009A6 RID: 2470
		public const string Id = "OnTheRoadTag";
	}
}
