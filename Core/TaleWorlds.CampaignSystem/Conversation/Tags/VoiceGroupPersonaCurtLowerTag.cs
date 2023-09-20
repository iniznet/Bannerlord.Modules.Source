using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x0200024A RID: 586
	public class VoiceGroupPersonaCurtLowerTag : ConversationTag
	{
		// Token: 0x170007CE RID: 1998
		// (get) Token: 0x06001EE4 RID: 7908 RVA: 0x00087DF7 File Offset: 0x00085FF7
		public override string StringId
		{
			get
			{
				return "VoiceGroupPersonaCurtLowerTag";
			}
		}

		// Token: 0x06001EE5 RID: 7909 RVA: 0x00087DFE File Offset: 0x00085FFE
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.GetPersona() == DefaultTraits.PersonaCurt && ConversationTagHelper.UsesLowRegister(character);
		}

		// Token: 0x040009CD RID: 2509
		public const string Id = "VoiceGroupPersonaCurtLowerTag";
	}
}
