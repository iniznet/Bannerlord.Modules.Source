using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x0200024B RID: 587
	public class VoiceGroupPersonaIronicTribalTag : ConversationTag
	{
		// Token: 0x170007CF RID: 1999
		// (get) Token: 0x06001EE7 RID: 7911 RVA: 0x00087E1D File Offset: 0x0008601D
		public override string StringId
		{
			get
			{
				return "VoiceGroupPersonaIronicTribalTag";
			}
		}

		// Token: 0x06001EE8 RID: 7912 RVA: 0x00087E24 File Offset: 0x00086024
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.GetPersona() == DefaultTraits.PersonaIronic && ConversationTagHelper.TribalVoiceGroup(character);
		}

		// Token: 0x040009CE RID: 2510
		public const string Id = "VoiceGroupPersonaIronicTribalTag";
	}
}
