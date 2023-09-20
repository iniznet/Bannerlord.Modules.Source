using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x02000249 RID: 585
	public class VoiceGroupPersonaCurtUpperTag : ConversationTag
	{
		// Token: 0x170007CD RID: 1997
		// (get) Token: 0x06001EE1 RID: 7905 RVA: 0x00087DD1 File Offset: 0x00085FD1
		public override string StringId
		{
			get
			{
				return "VoiceGroupPersonaCurtUpperTag";
			}
		}

		// Token: 0x06001EE2 RID: 7906 RVA: 0x00087DD8 File Offset: 0x00085FD8
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.GetPersona() == DefaultTraits.PersonaCurt && ConversationTagHelper.UsesHighRegister(character);
		}

		// Token: 0x040009CC RID: 2508
		public const string Id = "VoiceGroupPersonaCurtUpperTag";
	}
}
