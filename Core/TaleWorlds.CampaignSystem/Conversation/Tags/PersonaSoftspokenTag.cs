using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x02000244 RID: 580
	public class PersonaSoftspokenTag : ConversationTag
	{
		// Token: 0x170007C8 RID: 1992
		// (get) Token: 0x06001ED2 RID: 7890 RVA: 0x00087D11 File Offset: 0x00085F11
		public override string StringId
		{
			get
			{
				return "PersonaSoftspokenTag";
			}
		}

		// Token: 0x06001ED3 RID: 7891 RVA: 0x00087D18 File Offset: 0x00085F18
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.GetPersona() == DefaultTraits.PersonaSoftspoken;
		}

		// Token: 0x040009C7 RID: 2503
		public const string Id = "PersonaSoftspokenTag";
	}
}
