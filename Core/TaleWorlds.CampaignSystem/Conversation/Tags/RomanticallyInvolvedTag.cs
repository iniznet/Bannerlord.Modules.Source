using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x0200021E RID: 542
	public class RomanticallyInvolvedTag : ConversationTag
	{
		// Token: 0x170007A2 RID: 1954
		// (get) Token: 0x06001E60 RID: 7776 RVA: 0x000875B7 File Offset: 0x000857B7
		public override string StringId
		{
			get
			{
				return "RomanticallyInvolvedTag";
			}
		}

		// Token: 0x06001E61 RID: 7777 RVA: 0x000875BE File Offset: 0x000857BE
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && Romance.GetRomanticLevel(character.HeroObject, CharacterObject.PlayerCharacter.HeroObject) >= Romance.RomanceLevelEnum.CourtshipStarted;
		}

		// Token: 0x040009A0 RID: 2464
		public const string Id = "RomanticallyInvolvedTag";
	}
}
