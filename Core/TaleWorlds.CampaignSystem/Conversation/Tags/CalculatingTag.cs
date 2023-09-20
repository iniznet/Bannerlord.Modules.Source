using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x0200023C RID: 572
	public class CalculatingTag : ConversationTag
	{
		// Token: 0x170007C0 RID: 1984
		// (get) Token: 0x06001EBA RID: 7866 RVA: 0x00087BB3 File Offset: 0x00085DB3
		public override string StringId
		{
			get
			{
				return "CalculatingTag";
			}
		}

		// Token: 0x06001EBB RID: 7867 RVA: 0x00087BBA File Offset: 0x00085DBA
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.HeroObject.GetTraitLevel(DefaultTraits.Calculating) > 0;
		}

		// Token: 0x040009BF RID: 2495
		public const string Id = "CalculatingTag";
	}
}
