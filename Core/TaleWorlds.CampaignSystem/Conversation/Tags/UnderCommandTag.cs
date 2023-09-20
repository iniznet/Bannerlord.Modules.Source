using System;
using Helpers;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x0200021A RID: 538
	public class UnderCommandTag : ConversationTag
	{
		// Token: 0x1700079E RID: 1950
		// (get) Token: 0x06001E54 RID: 7764 RVA: 0x000874AA File Offset: 0x000856AA
		public override string StringId
		{
			get
			{
				return "UnderCommandTag";
			}
		}

		// Token: 0x06001E55 RID: 7765 RVA: 0x000874B1 File Offset: 0x000856B1
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.HeroObject.Spouse != Hero.MainHero && HeroHelper.UnderPlayerCommand(character.HeroObject);
		}

		// Token: 0x0400099C RID: 2460
		public const string Id = "UnderCommandTag";
	}
}
