using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x02000242 RID: 578
	public class PersonaCurtTag : ConversationTag
	{
		// Token: 0x170007C6 RID: 1990
		// (get) Token: 0x06001ECC RID: 7884 RVA: 0x00087CC1 File Offset: 0x00085EC1
		public override string StringId
		{
			get
			{
				return "PersonaCurtTag";
			}
		}

		// Token: 0x06001ECD RID: 7885 RVA: 0x00087CC8 File Offset: 0x00085EC8
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.GetPersona() == DefaultTraits.PersonaCurt;
		}

		// Token: 0x040009C5 RID: 2501
		public const string Id = "PersonaCurtTag";
	}
}
