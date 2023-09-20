using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x0200022A RID: 554
	public class MerchantNotableTypeTag : ConversationTag
	{
		// Token: 0x170007AE RID: 1966
		// (get) Token: 0x06001E84 RID: 7812 RVA: 0x00087881 File Offset: 0x00085A81
		public override string StringId
		{
			get
			{
				return "MerchantNotableTypeTag";
			}
		}

		// Token: 0x06001E85 RID: 7813 RVA: 0x00087888 File Offset: 0x00085A88
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.Occupation == Occupation.Merchant;
		}

		// Token: 0x040009AD RID: 2477
		public const string Id = "MerchantNotableTypeTag";
	}
}
