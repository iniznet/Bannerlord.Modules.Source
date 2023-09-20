using System;
using System.Linq;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x02000207 RID: 519
	public class PlayerIsBrotherTag : ConversationTag
	{
		// Token: 0x1700078B RID: 1931
		// (get) Token: 0x06001E1B RID: 7707 RVA: 0x00086F6B File Offset: 0x0008516B
		public override string StringId
		{
			get
			{
				return "PlayerIsBrotherTag";
			}
		}

		// Token: 0x06001E1C RID: 7708 RVA: 0x00086F72 File Offset: 0x00085172
		public override bool IsApplicableTo(CharacterObject character)
		{
			return !Hero.MainHero.IsFemale && character.IsHero && character.HeroObject.Siblings.Contains(Hero.MainHero);
		}

		// Token: 0x04000989 RID: 2441
		public const string Id = "PlayerIsBrotherTag";
	}
}
