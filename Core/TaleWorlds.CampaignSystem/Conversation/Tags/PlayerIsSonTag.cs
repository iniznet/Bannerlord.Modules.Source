using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x02000204 RID: 516
	public class PlayerIsSonTag : ConversationTag
	{
		// Token: 0x17000788 RID: 1928
		// (get) Token: 0x06001E12 RID: 7698 RVA: 0x00086EC4 File Offset: 0x000850C4
		public override string StringId
		{
			get
			{
				return "PlayerIsSonTag";
			}
		}

		// Token: 0x06001E13 RID: 7699 RVA: 0x00086ECB File Offset: 0x000850CB
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && !Hero.MainHero.IsFemale && (Hero.MainHero.Father == character.HeroObject || Hero.MainHero.Mother == character.HeroObject);
		}

		// Token: 0x04000986 RID: 2438
		public const string Id = "PlayerIsSonTag";
	}
}
