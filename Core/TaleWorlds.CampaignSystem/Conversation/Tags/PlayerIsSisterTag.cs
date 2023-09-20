using System;
using System.Linq;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x02000208 RID: 520
	public class PlayerIsSisterTag : ConversationTag
	{
		// Token: 0x1700078C RID: 1932
		// (get) Token: 0x06001E1E RID: 7710 RVA: 0x00086FA7 File Offset: 0x000851A7
		public override string StringId
		{
			get
			{
				return "PlayerIsSisterTag";
			}
		}

		// Token: 0x06001E1F RID: 7711 RVA: 0x00086FAE File Offset: 0x000851AE
		public override bool IsApplicableTo(CharacterObject character)
		{
			return Hero.MainHero.IsFemale && character.IsHero && character.HeroObject.Siblings.Contains(Hero.MainHero);
		}

		// Token: 0x0400098A RID: 2442
		public const string Id = "PlayerIsSisterTag";
	}
}
