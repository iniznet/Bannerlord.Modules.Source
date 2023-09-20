using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x02000228 RID: 552
	public class HeadmanNotableTypeTag : ConversationTag
	{
		// Token: 0x170007AC RID: 1964
		// (get) Token: 0x06001E7E RID: 7806 RVA: 0x00087837 File Offset: 0x00085A37
		public override string StringId
		{
			get
			{
				return "HeadmanNotableTypeTag";
			}
		}

		// Token: 0x06001E7F RID: 7807 RVA: 0x0008783E File Offset: 0x00085A3E
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.Occupation == Occupation.Headman;
		}

		// Token: 0x040009AB RID: 2475
		public const string Id = "HeadmanNotableTypeTag";
	}
}
