using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x02000200 RID: 512
	public class AlliedLordTag : ConversationTag
	{
		// Token: 0x17000784 RID: 1924
		// (get) Token: 0x06001E06 RID: 7686 RVA: 0x00086DF5 File Offset: 0x00084FF5
		public override string StringId
		{
			get
			{
				return "PlayerIsAlliedTag";
			}
		}

		// Token: 0x06001E07 RID: 7687 RVA: 0x00086DFC File Offset: 0x00084FFC
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && FactionManager.IsAlliedWithFaction(character.HeroObject.MapFaction, Hero.MainHero.MapFaction);
		}

		// Token: 0x04000982 RID: 2434
		public const string Id = "PlayerIsAlliedTag";
	}
}
