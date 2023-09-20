using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x02000237 RID: 567
	public class CruelTag : ConversationTag
	{
		// Token: 0x170007BB RID: 1979
		// (get) Token: 0x06001EAB RID: 7851 RVA: 0x00087ACD File Offset: 0x00085CCD
		public override string StringId
		{
			get
			{
				return "CruelTag";
			}
		}

		// Token: 0x06001EAC RID: 7852 RVA: 0x00087AD4 File Offset: 0x00085CD4
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.HeroObject.GetTraitLevel(DefaultTraits.Mercy) < 0;
		}

		// Token: 0x040009BA RID: 2490
		public const string Id = "CruelTag";
	}
}
