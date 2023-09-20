using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x02000245 RID: 581
	public class VoiceGroupPersonaEarnestTribalTag : ConversationTag
	{
		// Token: 0x170007C9 RID: 1993
		// (get) Token: 0x06001ED5 RID: 7893 RVA: 0x00087D39 File Offset: 0x00085F39
		public override string StringId
		{
			get
			{
				return "VoiceGroupPersonaEarnestTribalTag";
			}
		}

		// Token: 0x06001ED6 RID: 7894 RVA: 0x00087D40 File Offset: 0x00085F40
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.GetPersona() == DefaultTraits.PersonaEarnest && ConversationTagHelper.TribalVoiceGroup(character);
		}

		// Token: 0x040009C8 RID: 2504
		public const string Id = "VoiceGroupPersonaEarnestTribalTag";
	}
}
