using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x0200024E RID: 590
	public class VoiceGroupPersonaSoftspokenTribalTag : ConversationTag
	{
		// Token: 0x170007D2 RID: 2002
		// (get) Token: 0x06001EF0 RID: 7920 RVA: 0x00087E8F File Offset: 0x0008608F
		public override string StringId
		{
			get
			{
				return "VoiceGroupPersonaSoftspokenTribalTag";
			}
		}

		// Token: 0x06001EF1 RID: 7921 RVA: 0x00087E96 File Offset: 0x00086096
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.GetPersona() == DefaultTraits.PersonaSoftspoken && ConversationTagHelper.TribalVoiceGroup(character);
		}

		// Token: 0x040009D1 RID: 2513
		public const string Id = "VoiceGroupPersonaSoftspokenTribalTag";
	}
}
