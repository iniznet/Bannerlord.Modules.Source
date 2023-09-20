using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x0200022C RID: 556
	public class AnyNotableTypeTag : ConversationTag
	{
		// Token: 0x170007B0 RID: 1968
		// (get) Token: 0x06001E8A RID: 7818 RVA: 0x000878CB File Offset: 0x00085ACB
		public override string StringId
		{
			get
			{
				return "AnyNotableTypeTag";
			}
		}

		// Token: 0x06001E8B RID: 7819 RVA: 0x000878D2 File Offset: 0x00085AD2
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.HeroObject.IsNotable;
		}

		// Token: 0x040009AF RID: 2479
		public const string Id = "AnyNotableTypeTag";
	}
}
