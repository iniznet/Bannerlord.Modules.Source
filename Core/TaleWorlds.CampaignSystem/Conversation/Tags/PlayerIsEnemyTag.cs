using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x020001FF RID: 511
	public class PlayerIsEnemyTag : ConversationTag
	{
		// Token: 0x17000783 RID: 1923
		// (get) Token: 0x06001E03 RID: 7683 RVA: 0x00086DC0 File Offset: 0x00084FC0
		public override string StringId
		{
			get
			{
				return "PlayerIsEnemyTag";
			}
		}

		// Token: 0x06001E04 RID: 7684 RVA: 0x00086DC7 File Offset: 0x00084FC7
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && FactionManager.IsAtWarAgainstFaction(character.HeroObject.MapFaction, Hero.MainHero.MapFaction);
		}

		// Token: 0x04000981 RID: 2433
		public const string Id = "PlayerIsEnemyTag";
	}
}
