using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x02000246 RID: 582
	public class VoiceGroupPersonaEarnestUpperTag : ConversationTag
	{
		// Token: 0x170007CA RID: 1994
		// (get) Token: 0x06001ED8 RID: 7896 RVA: 0x00087D5F File Offset: 0x00085F5F
		public override string StringId
		{
			get
			{
				return "VoiceGroupPersonaEarnestUpperTag";
			}
		}

		// Token: 0x06001ED9 RID: 7897 RVA: 0x00087D66 File Offset: 0x00085F66
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.GetPersona() == DefaultTraits.PersonaEarnest && ConversationTagHelper.UsesHighRegister(character);
		}

		// Token: 0x040009C9 RID: 2505
		public const string Id = "VoiceGroupPersonaEarnestUpperTag";
	}
}
