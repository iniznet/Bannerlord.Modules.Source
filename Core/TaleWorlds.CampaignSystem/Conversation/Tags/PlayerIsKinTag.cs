using System;
using System.Linq;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x02000209 RID: 521
	public class PlayerIsKinTag : ConversationTag
	{
		// Token: 0x1700078D RID: 1933
		// (get) Token: 0x06001E21 RID: 7713 RVA: 0x00086FE3 File Offset: 0x000851E3
		public override string StringId
		{
			get
			{
				return "PlayerIsKinTag";
			}
		}

		// Token: 0x06001E22 RID: 7714 RVA: 0x00086FEC File Offset: 0x000851EC
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && (character.HeroObject.Siblings.Contains(Hero.MainHero) || character.HeroObject.Mother == Hero.MainHero || character.HeroObject.Father == Hero.MainHero || character.HeroObject.Spouse == Hero.MainHero);
		}

		// Token: 0x0400098B RID: 2443
		public const string Id = "PlayerIsKinTag";
	}
}
