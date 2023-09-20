using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x02000241 RID: 577
	public class PersonaEarnestTag : ConversationTag
	{
		// Token: 0x170007C5 RID: 1989
		// (get) Token: 0x06001EC9 RID: 7881 RVA: 0x00087C99 File Offset: 0x00085E99
		public override string StringId
		{
			get
			{
				return "PersonaEarnestTag";
			}
		}

		// Token: 0x06001ECA RID: 7882 RVA: 0x00087CA0 File Offset: 0x00085EA0
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.GetPersona() == DefaultTraits.PersonaEarnest;
		}

		// Token: 0x040009C4 RID: 2500
		public const string Id = "PersonaEarnestTag";
	}
}
