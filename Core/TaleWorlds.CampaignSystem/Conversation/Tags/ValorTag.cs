using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x0200023E RID: 574
	public class ValorTag : ConversationTag
	{
		// Token: 0x170007C2 RID: 1986
		// (get) Token: 0x06001EC0 RID: 7872 RVA: 0x00087C0F File Offset: 0x00085E0F
		public override string StringId
		{
			get
			{
				return "ValorTag";
			}
		}

		// Token: 0x06001EC1 RID: 7873 RVA: 0x00087C16 File Offset: 0x00085E16
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.HeroObject.GetTraitLevel(DefaultTraits.Valor) > 0;
		}

		// Token: 0x040009C1 RID: 2497
		public const string Id = "ValorTag";
	}
}
