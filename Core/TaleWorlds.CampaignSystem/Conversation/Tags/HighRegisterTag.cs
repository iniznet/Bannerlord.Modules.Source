using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x020001FC RID: 508
	public class HighRegisterTag : ConversationTag
	{
		// Token: 0x17000780 RID: 1920
		// (get) Token: 0x06001DFA RID: 7674 RVA: 0x00086D52 File Offset: 0x00084F52
		public override string StringId
		{
			get
			{
				return "HighRegisterTag";
			}
		}

		// Token: 0x06001DFB RID: 7675 RVA: 0x00086D59 File Offset: 0x00084F59
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && ConversationTagHelper.UsesHighRegister(character);
		}

		// Token: 0x0400097E RID: 2430
		public const string Id = "HighRegisterTag";
	}
}
