using System;
using TaleWorlds.CampaignSystem.ComponentInterfaces;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultDifficultyModel : DifficultyModel
	{
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
