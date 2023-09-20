using System;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x020001F9 RID: 505
	public class WaryTag : ConversationTag
	{
		// Token: 0x1700077D RID: 1917
		// (get) Token: 0x06001DF1 RID: 7665 RVA: 0x00086BFB File Offset: 0x00084DFB
		public override string StringId
		{
			get
			{
				return "WaryTag";
			}
		}

		// Token: 0x06001DF2 RID: 7666 RVA: 0x00086C04 File Offset: 0x00084E04
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.HeroObject.MapFaction != Hero.MainHero.MapFaction && (Settlement.CurrentSettlement == null || Settlement.CurrentSettlement.SiegeEvent != null) && (Campaign.Current.ConversationManager.CurrentConversationIsFirst || FactionManager.IsAtWarAgainstFaction(character.HeroObject.MapFaction, Hero.MainHero.MapFaction));
		}

		// Token: 0x0400097B RID: 2427
		public const string Id = "WaryTag";
	}
}
