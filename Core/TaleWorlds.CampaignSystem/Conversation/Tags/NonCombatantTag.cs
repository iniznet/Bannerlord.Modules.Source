using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x020001F7 RID: 503
	public class NonCombatantTag : ConversationTag
	{
		// Token: 0x1700077B RID: 1915
		// (get) Token: 0x06001DEB RID: 7659 RVA: 0x00086BAC File Offset: 0x00084DAC
		public override string StringId
		{
			get
			{
				return "NonCombatantTag";
			}
		}

		// Token: 0x06001DEC RID: 7660 RVA: 0x00086BB3 File Offset: 0x00084DB3
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.HeroObject.IsNoncombatant;
		}

		// Token: 0x04000979 RID: 2425
		public const string Id = "NonCombatantTag";
	}
}
