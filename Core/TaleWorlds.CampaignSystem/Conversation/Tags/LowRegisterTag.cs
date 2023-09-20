using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x020001FD RID: 509
	public class LowRegisterTag : ConversationTag
	{
		// Token: 0x17000781 RID: 1921
		// (get) Token: 0x06001DFD RID: 7677 RVA: 0x00086D73 File Offset: 0x00084F73
		public override string StringId
		{
			get
			{
				return "LowRegisterTag";
			}
		}

		// Token: 0x06001DFE RID: 7678 RVA: 0x00086D7A File Offset: 0x00084F7A
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && !ConversationTagHelper.UsesHighRegister(character) && ConversationTagHelper.UsesLowRegister(character);
		}

		// Token: 0x0400097F RID: 2431
		public const string Id = "LowRegisterTag";
	}
}
