using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x02000202 RID: 514
	public class PlayerIsSpouseTag : ConversationTag
	{
		// Token: 0x17000786 RID: 1926
		// (get) Token: 0x06001E0C RID: 7692 RVA: 0x00086E4A File Offset: 0x0008504A
		public override string StringId
		{
			get
			{
				return "PlayerIsSpouseTag";
			}
		}

		// Token: 0x06001E0D RID: 7693 RVA: 0x00086E51 File Offset: 0x00085051
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && Hero.MainHero.Spouse == character.HeroObject;
		}

		// Token: 0x04000984 RID: 2436
		public const string Id = "PlayerIsSpouseTag";
	}
}
