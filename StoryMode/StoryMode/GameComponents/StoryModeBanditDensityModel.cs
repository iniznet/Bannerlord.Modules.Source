using System;
using TaleWorlds.CampaignSystem.GameComponents;

namespace StoryMode.GameComponents
{
	public class StoryModeBanditDensityModel : DefaultBanditDensityModel
	{
		public override int NumberOfMaximumLooterParties
		{
			get
			{
				if (StoryModeManager.Current.MainStoryLine.IsPlayerInteractionRestricted)
				{
					return 0;
				}
				return base.NumberOfMaximumLooterParties;
			}
		}

		public override int NumberOfMaximumBanditPartiesAroundEachHideout
		{
			get
			{
				if (StoryModeManager.Current.MainStoryLine.IsPlayerInteractionRestricted)
				{
					return 0;
				}
				return base.NumberOfMaximumBanditPartiesAroundEachHideout;
			}
		}

		public override int NumberOfMaximumBanditPartiesInEachHideout
		{
			get
			{
				if (StoryModeManager.Current.MainStoryLine.IsPlayerInteractionRestricted)
				{
					return 0;
				}
				return base.NumberOfMaximumBanditPartiesInEachHideout;
			}
		}

		public override int NumberOfMaximumHideoutsAtEachBanditFaction
		{
			get
			{
				if (StoryModeManager.Current.MainStoryLine.IsPlayerInteractionRestricted)
				{
					return 0;
				}
				return base.NumberOfMaximumHideoutsAtEachBanditFaction;
			}
		}
	}
}
