using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x0200024F RID: 591
	public class VoiceGroupPersonaSoftspokenUpperTag : ConversationTag
	{
		// Token: 0x170007D3 RID: 2003
		// (get) Token: 0x06001EF3 RID: 7923 RVA: 0x00087EB5 File Offset: 0x000860B5
		public override string StringId
		{
			get
			{
				return "VoiceGroupPersonaSoftspokenUpperTag";
			}
		}

		// Token: 0x06001EF4 RID: 7924 RVA: 0x00087EBC File Offset: 0x000860BC
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.GetPersona() == DefaultTraits.PersonaSoftspoken && ConversationTagHelper.UsesHighRegister(character);
		}

		// Token: 0x040009D2 RID: 2514
		public const string Id = "VoiceGroupPersonaSoftspokenUpperTag";
	}
}
