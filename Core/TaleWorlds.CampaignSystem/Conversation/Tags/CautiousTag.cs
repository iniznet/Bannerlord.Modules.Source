using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x0200023F RID: 575
	public class CautiousTag : ConversationTag
	{
		// Token: 0x170007C3 RID: 1987
		// (get) Token: 0x06001EC3 RID: 7875 RVA: 0x00087C3D File Offset: 0x00085E3D
		public override string StringId
		{
			get
			{
				return "CautiousTag";
			}
		}

		// Token: 0x06001EC4 RID: 7876 RVA: 0x00087C44 File Offset: 0x00085E44
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.HeroObject.GetTraitLevel(DefaultTraits.Valor) < 0;
		}

		// Token: 0x040009C2 RID: 2498
		public const string Id = "CautiousTag";
	}
}
