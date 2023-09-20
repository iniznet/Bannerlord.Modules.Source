using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x02000225 RID: 549
	public class PlayerIsLiegeTag : ConversationTag
	{
		// Token: 0x170007A9 RID: 1961
		// (get) Token: 0x06001E75 RID: 7797 RVA: 0x0008778B File Offset: 0x0008598B
		public override string StringId
		{
			get
			{
				return "PlayerIsLiegeTag";
			}
		}

		// Token: 0x06001E76 RID: 7798 RVA: 0x00087794 File Offset: 0x00085994
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.HeroObject.MapFaction.IsKingdomFaction && character.HeroObject.MapFaction == Hero.MainHero.MapFaction && Hero.MainHero.MapFaction.Leader == Hero.MainHero;
		}

		// Token: 0x040009A8 RID: 2472
		public const string Id = "PlayerIsLiegeTag";
	}
}
