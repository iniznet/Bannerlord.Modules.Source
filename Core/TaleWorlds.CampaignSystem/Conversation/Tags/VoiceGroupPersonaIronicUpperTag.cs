using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x0200024C RID: 588
	public class VoiceGroupPersonaIronicUpperTag : ConversationTag
	{
		// Token: 0x170007D0 RID: 2000
		// (get) Token: 0x06001EEA RID: 7914 RVA: 0x00087E43 File Offset: 0x00086043
		public override string StringId
		{
			get
			{
				return "VoiceGroupPersonaIronicUpperTag";
			}
		}

		// Token: 0x06001EEB RID: 7915 RVA: 0x00087E4A File Offset: 0x0008604A
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.GetPersona() == DefaultTraits.PersonaIronic && ConversationTagHelper.UsesHighRegister(character);
		}

		// Token: 0x040009CF RID: 2511
		public const string Id = "VoiceGroupPersonaIronicUpperTag";
	}
}
