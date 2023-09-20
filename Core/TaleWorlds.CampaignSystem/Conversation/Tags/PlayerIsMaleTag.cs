using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x02000211 RID: 529
	public class PlayerIsMaleTag : ConversationTag
	{
		// Token: 0x17000795 RID: 1941
		// (get) Token: 0x06001E39 RID: 7737 RVA: 0x000871C6 File Offset: 0x000853C6
		public override string StringId
		{
			get
			{
				return "PlayerIsMaleTag";
			}
		}

		// Token: 0x06001E3A RID: 7738 RVA: 0x000871CD File Offset: 0x000853CD
		public override bool IsApplicableTo(CharacterObject character)
		{
			return !Hero.MainHero.IsFemale;
		}

		// Token: 0x04000993 RID: 2451
		public const string Id = "PlayerIsMaleTag";
	}
}
