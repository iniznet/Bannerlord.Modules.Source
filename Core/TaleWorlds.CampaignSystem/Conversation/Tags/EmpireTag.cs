using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x02000230 RID: 560
	public class EmpireTag : ConversationTag
	{
		// Token: 0x170007B4 RID: 1972
		// (get) Token: 0x06001E96 RID: 7830 RVA: 0x000879BB File Offset: 0x00085BBB
		public override string StringId
		{
			get
			{
				return "EmpireTag";
			}
		}

		// Token: 0x06001E97 RID: 7831 RVA: 0x000879C2 File Offset: 0x00085BC2
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.Culture.StringId == "empire";
		}

		// Token: 0x040009B3 RID: 2483
		public const string Id = "EmpireTag";
	}
}
