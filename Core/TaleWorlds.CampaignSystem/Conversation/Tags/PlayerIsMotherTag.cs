using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x02000206 RID: 518
	public class PlayerIsMotherTag : ConversationTag
	{
		// Token: 0x1700078A RID: 1930
		// (get) Token: 0x06001E18 RID: 7704 RVA: 0x00086F3E File Offset: 0x0008513E
		public override string StringId
		{
			get
			{
				return "PlayerIsMotherTag";
			}
		}

		// Token: 0x06001E19 RID: 7705 RVA: 0x00086F45 File Offset: 0x00085145
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.HeroObject.Mother == Hero.MainHero;
		}

		// Token: 0x04000988 RID: 2440
		public const string Id = "PlayerIsMotherTag";
	}
}
