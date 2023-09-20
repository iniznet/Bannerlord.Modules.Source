using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x02000243 RID: 579
	public class PersonaIronicTag : ConversationTag
	{
		// Token: 0x170007C7 RID: 1991
		// (get) Token: 0x06001ECF RID: 7887 RVA: 0x00087CE9 File Offset: 0x00085EE9
		public override string StringId
		{
			get
			{
				return "PersonaIronicTag";
			}
		}

		// Token: 0x06001ED0 RID: 7888 RVA: 0x00087CF0 File Offset: 0x00085EF0
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.GetPersona() == DefaultTraits.PersonaIronic;
		}

		// Token: 0x040009C6 RID: 2502
		public const string Id = "PersonaIronicTag";
	}
}
