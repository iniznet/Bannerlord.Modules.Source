using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x02000203 RID: 515
	public class PlayerIsDaughterTag : ConversationTag
	{
		// Token: 0x17000787 RID: 1927
		// (get) Token: 0x06001E0F RID: 7695 RVA: 0x00086E77 File Offset: 0x00085077
		public override string StringId
		{
			get
			{
				return "PlayerIsDaughterTag";
			}
		}

		// Token: 0x06001E10 RID: 7696 RVA: 0x00086E7E File Offset: 0x0008507E
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && Hero.MainHero.IsFemale && (Hero.MainHero.Father == character.HeroObject || Hero.MainHero.Mother == character.HeroObject);
		}

		// Token: 0x04000985 RID: 2437
		public const string Id = "PlayerIsDaughterTag";
	}
}
