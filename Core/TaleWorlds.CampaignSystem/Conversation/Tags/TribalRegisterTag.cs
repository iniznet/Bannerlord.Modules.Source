using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x020001FE RID: 510
	public class TribalRegisterTag : ConversationTag
	{
		// Token: 0x17000782 RID: 1922
		// (get) Token: 0x06001E00 RID: 7680 RVA: 0x00086D9C File Offset: 0x00084F9C
		public override string StringId
		{
			get
			{
				return "TribalRegisterTag";
			}
		}

		// Token: 0x06001E01 RID: 7681 RVA: 0x00086DA3 File Offset: 0x00084FA3
		public override bool IsApplicableTo(CharacterObject character)
		{
			return !ConversationTagHelper.UsesHighRegister(character) && !ConversationTagHelper.UsesLowRegister(character);
		}

		// Token: 0x04000980 RID: 2432
		public const string Id = "TribalRegisterTag";
	}
}
