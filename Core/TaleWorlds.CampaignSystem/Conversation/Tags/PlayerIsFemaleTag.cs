using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x02000210 RID: 528
	public class PlayerIsFemaleTag : ConversationTag
	{
		// Token: 0x17000794 RID: 1940
		// (get) Token: 0x06001E36 RID: 7734 RVA: 0x000871AB File Offset: 0x000853AB
		public override string StringId
		{
			get
			{
				return "PlayerIsFemaleTag";
			}
		}

		// Token: 0x06001E37 RID: 7735 RVA: 0x000871B2 File Offset: 0x000853B2
		public override bool IsApplicableTo(CharacterObject character)
		{
			return Hero.MainHero.IsFemale;
		}

		// Token: 0x04000992 RID: 2450
		public const string Id = "PlayerIsFemaleTag";
	}
}
