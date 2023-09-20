using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x02000205 RID: 517
	public class PlayerIsFatherTag : ConversationTag
	{
		// Token: 0x17000789 RID: 1929
		// (get) Token: 0x06001E15 RID: 7701 RVA: 0x00086F11 File Offset: 0x00085111
		public override string StringId
		{
			get
			{
				return "PlayerIsFatherTag";
			}
		}

		// Token: 0x06001E16 RID: 7702 RVA: 0x00086F18 File Offset: 0x00085118
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.HeroObject.Father == Hero.MainHero;
		}

		// Token: 0x04000987 RID: 2439
		public const string Id = "PlayerIsFatherTag";
	}
}
