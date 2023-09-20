using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x02000250 RID: 592
	public class VoiceGroupPersonaSoftspokenLowerTag : ConversationTag
	{
		// Token: 0x170007D4 RID: 2004
		// (get) Token: 0x06001EF6 RID: 7926 RVA: 0x00087EDB File Offset: 0x000860DB
		public override string StringId
		{
			get
			{
				return "VoiceGroupPersonaSoftspokenLowerTag";
			}
		}

		// Token: 0x06001EF7 RID: 7927 RVA: 0x00087EE2 File Offset: 0x000860E2
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.GetPersona() == DefaultTraits.PersonaSoftspoken && ConversationTagHelper.UsesLowRegister(character);
		}

		// Token: 0x040009D3 RID: 2515
		public const string Id = "VoiceGroupPersonaSoftspokenLowerTag";
	}
}
