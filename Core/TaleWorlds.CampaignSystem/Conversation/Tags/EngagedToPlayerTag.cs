using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x02000220 RID: 544
	public class EngagedToPlayerTag : ConversationTag
	{
		// Token: 0x170007A4 RID: 1956
		// (get) Token: 0x06001E66 RID: 7782 RVA: 0x00087671 File Offset: 0x00085871
		public override string StringId
		{
			get
			{
				return "EngagedToPlayerTag";
			}
		}

		// Token: 0x06001E67 RID: 7783 RVA: 0x00087678 File Offset: 0x00085878
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && Romance.GetRomanticLevel(character.HeroObject, Hero.MainHero) == Romance.RomanceLevelEnum.CoupleAgreedOnMarriage;
		}

		// Token: 0x040009A3 RID: 2467
		public const string Id = "EngagedToPlayerTag";
	}
}
