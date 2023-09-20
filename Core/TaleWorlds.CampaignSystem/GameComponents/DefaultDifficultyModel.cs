using System;
using TaleWorlds.CampaignSystem.ComponentInterfaces;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x02000104 RID: 260
	public class DefaultDifficultyModel : DifficultyModel
	{
		// Token: 0x0600152A RID: 5418 RVA: 0x00061160 File Offset: 0x0005F360
		public override float GetPlayerTroopsReceivedDamageMultiplier()
		{
			switch (CampaignOptions.PlayerTroopsReceivedDamage)
			{
			case CampaignOptions.Difficulty.VeryEasy:
				return 0.5f;
			case CampaignOptions.Difficulty.Easy:
				return 0.75f;
			case CampaignOptions.Difficulty.Realistic:
				return 1f;
			default:
				return 1f;
			}
		}

		// Token: 0x0600152B RID: 5419 RVA: 0x000611A0 File Offset: 0x0005F3A0
		public override float GetDamageToPlayerMultiplier()
		{
			switch (CampaignOptions.PlayerReceivedDamage)
			{
			case CampaignOptions.Difficulty.VeryEasy:
				return 0.25f;
			case CampaignOptions.Difficulty.Easy:
				return 0.5f;
			case CampaignOptions.Difficulty.Realistic:
				return 1f;
			default:
				return 1f;
			}
		}

		// Token: 0x0600152C RID: 5420 RVA: 0x000611E0 File Offset: 0x0005F3E0
		public override int GetPlayerRecruitSlotBonus()
		{
			switch (CampaignOptions.RecruitmentDifficulty)
			{
			case CampaignOptions.Difficulty.VeryEasy:
				return 2;
			case CampaignOptions.Difficulty.Easy:
				return 1;
			case CampaignOptions.Difficulty.Realistic:
				return 0;
			default:
				return 0;
			}
		}

		// Token: 0x0600152D RID: 5421 RVA: 0x00061210 File Offset: 0x0005F410
		public override float GetPlayerMapMovementSpeedBonusMultiplier()
		{
			switch (CampaignOptions.PlayerMapMovementSpeed)
			{
			case CampaignOptions.Difficulty.VeryEasy:
				return 0.1f;
			case CampaignOptions.Difficulty.Easy:
				return 0.05f;
			case CampaignOptions.Difficulty.Realistic:
				return 0f;
			default:
				return 0f;
			}
		}

		// Token: 0x0600152E RID: 5422 RVA: 0x00061250 File Offset: 0x0005F450
		public override float GetCombatAIDifficultyMultiplier()
		{
			switch (CampaignOptions.CombatAIDifficulty)
			{
			case CampaignOptions.Difficulty.VeryEasy:
				return 0.1f;
			case CampaignOptions.Difficulty.Easy:
				return 0.32f;
			case CampaignOptions.Difficulty.Realistic:
				return 0.96f;
			default:
				return 0.5f;
			}
		}

		// Token: 0x0600152F RID: 5423 RVA: 0x00061290 File Offset: 0x0005F490
		public override float GetPersuasionBonusChance()
		{
			switch (CampaignOptions.PersuasionSuccessChance)
			{
			case CampaignOptions.Difficulty.VeryEasy:
				return 0.1f;
			case CampaignOptions.Difficulty.Easy:
				return 0.05f;
			case CampaignOptions.Difficulty.Realistic:
				return 0f;
			default:
				return 0f;
			}
		}

		// Token: 0x06001530 RID: 5424 RVA: 0x000612D0 File Offset: 0x0005F4D0
		public override float GetClanMemberDeathChanceMultiplier()
		{
			switch (CampaignOptions.ClanMemberDeathChance)
			{
			case CampaignOptions.Difficulty.VeryEasy:
				return -1f;
			case CampaignOptions.Difficulty.Easy:
				return -0.5f;
			case CampaignOptions.Difficulty.Realistic:
				return 0f;
			default:
				return 0f;
			}
		}
	}
}
