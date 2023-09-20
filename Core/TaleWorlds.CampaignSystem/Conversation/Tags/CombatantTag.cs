using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x020001F8 RID: 504
	public class CombatantTag : ConversationTag
	{
		// Token: 0x1700077C RID: 1916
		// (get) Token: 0x06001DEE RID: 7662 RVA: 0x00086BD2 File Offset: 0x00084DD2
		public override string StringId
		{
			get
			{
				return "CombatantTag";
			}
		}

		// Token: 0x06001DEF RID: 7663 RVA: 0x00086BD9 File Offset: 0x00084DD9
		public override bool IsApplicableTo(CharacterObject character)
		{
			return !character.IsHero || !character.HeroObject.IsNoncombatant;
		}

		// Token: 0x0400097A RID: 2426
		public const string Id = "CombatantTag";
	}
}
