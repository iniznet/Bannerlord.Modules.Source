using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x020001C5 RID: 453
	public abstract class DifficultyModel : GameModel
	{
		// Token: 0x06001B5A RID: 7002
		public abstract float GetPlayerTroopsReceivedDamageMultiplier();

		// Token: 0x06001B5B RID: 7003
		public abstract float GetDamageToPlayerMultiplier();

		// Token: 0x06001B5C RID: 7004
		public abstract int GetPlayerRecruitSlotBonus();

		// Token: 0x06001B5D RID: 7005
		public abstract float GetPlayerMapMovementSpeedBonusMultiplier();

		// Token: 0x06001B5E RID: 7006
		public abstract float GetCombatAIDifficultyMultiplier();

		// Token: 0x06001B5F RID: 7007
		public abstract float GetPersuasionBonusChance();

		// Token: 0x06001B60 RID: 7008
		public abstract float GetClanMemberDeathChanceMultiplier();
	}
}
