using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultBanditDensityModel : BanditDensityModel
	{
		public override int NumberOfMaximumLooterParties
		{
			get
			{
				return 150;
			}
		}

		public override int NumberOfMinimumBanditPartiesInAHideoutToInfestIt
		{
			get
			{
				return 2;
			}
		}

		public override int NumberOfMaximumBanditPartiesInEachHideout
		{
			get
			{
				return 4;
			}
		}

		public override int NumberOfMaximumBanditPartiesAroundEachHideout
		{
			get
			{
				return 8;
			}
		}

		public override int NumberOfMaximumHideoutsAtEachBanditFaction
		{
			get
			{
				return 10;
			}
		}

		public override int NumberOfInitialHideoutsAtEachBanditFaction
		{
			get
			{
				return 3;
			}
		}

		public override int NumberOfMinimumBanditTroopsInHideoutMission
		{
			get
			{
				return 10;
			}
		}

		public override int NumberOfMaximumTroopCountForFirstFightInHideout
		{
			get
			{
				return MathF.Floor(6f * (2f + Campaign.Current.PlayerProgress));
			}
		}

		public override int NumberOfMaximumTroopCountForBossFightInHideout
		{
			get
			{
				return MathF.Floor(1f + 5f * (1f + Campaign.Current.PlayerProgress));
			}
		}

		public override float SpawnPercentageForFirstFightInHideoutMission
		{
			get
			{
				return 0.75f;
			}
		}

		public override int GetPlayerMaximumTroopCountForHideoutMission(MobileParty party)
		{
			float num = 10f;
			if (party.HasPerk(DefaultPerks.Tactics.SmallUnitTactics, false))
			{
				num += DefaultPerks.Tactics.SmallUnitTactics.PrimaryBonus;
			}
			return MathF.Round(num);
		}
	}
}
