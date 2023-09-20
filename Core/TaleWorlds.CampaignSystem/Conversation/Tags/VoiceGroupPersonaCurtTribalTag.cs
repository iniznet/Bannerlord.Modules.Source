using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x02000248 RID: 584
	public class VoiceGroupPersonaCurtTribalTag : ConversationTag
	{
		// Token: 0x170007CC RID: 1996
		// (get) Token: 0x06001EDE RID: 7902 RVA: 0x00087DAB File Offset: 0x00085FAB
		public override string StringId
		{
			get
			{
				return "VoiceGroupPersonaCurtTribalTag";
			}
		}

		// Token: 0x06001EDF RID: 7903 RVA: 0x00087DB2 File Offset: 0x00085FB2
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.GetPersona() == DefaultTraits.PersonaCurt && ConversationTagHelper.TribalVoiceGroup(character);
		}

		// Token: 0x040009CB RID: 2507
		public const string Id = "VoiceGroupPersonaCurtTribalTag";
	}
}
