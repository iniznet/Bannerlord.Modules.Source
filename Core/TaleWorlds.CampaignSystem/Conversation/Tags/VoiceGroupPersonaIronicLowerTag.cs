using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x0200024D RID: 589
	public class VoiceGroupPersonaIronicLowerTag : ConversationTag
	{
		// Token: 0x170007D1 RID: 2001
		// (get) Token: 0x06001EED RID: 7917 RVA: 0x00087E69 File Offset: 0x00086069
		public override string StringId
		{
			get
			{
				return "VoiceGroupPersonaIronicLowerTag";
			}
		}

		// Token: 0x06001EEE RID: 7918 RVA: 0x00087E70 File Offset: 0x00086070
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.GetPersona() == DefaultTraits.PersonaIronic && ConversationTagHelper.UsesLowRegister(character);
		}

		// Token: 0x040009D0 RID: 2512
		public const string Id = "VoiceGroupPersonaIronicLowerTag";
	}
}
