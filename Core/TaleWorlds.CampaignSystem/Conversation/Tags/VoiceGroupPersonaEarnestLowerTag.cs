using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x02000247 RID: 583
	public class VoiceGroupPersonaEarnestLowerTag : ConversationTag
	{
		// Token: 0x170007CB RID: 1995
		// (get) Token: 0x06001EDB RID: 7899 RVA: 0x00087D85 File Offset: 0x00085F85
		public override string StringId
		{
			get
			{
				return "VoiceGroupPersonaEarnestLowerTag";
			}
		}

		// Token: 0x06001EDC RID: 7900 RVA: 0x00087D8C File Offset: 0x00085F8C
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.GetPersona() == DefaultTraits.PersonaEarnest && ConversationTagHelper.UsesLowRegister(character);
		}

		// Token: 0x040009CA RID: 2506
		public const string Id = "VoiceGroupPersonaEarnestLowerTag";
	}
}
