using System;
using TaleWorlds.CampaignSystem.GameComponents;

namespace StoryMode.GameComponents
{
	// Token: 0x0200003A RID: 58
	public class StoryModeBanditDensityModel : DefaultBanditDensityModel
	{
		// Token: 0x170000C3 RID: 195
		// (get) Token: 0x060003A6 RID: 934 RVA: 0x000170C2 File Offset: 0x000152C2
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

		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x060003A7 RID: 935 RVA: 0x000170DD File Offset: 0x000152DD
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

		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x060003A8 RID: 936 RVA: 0x000170F8 File Offset: 0x000152F8
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

		// Token: 0x170000C6 RID: 198
		// (get) Token: 0x060003A9 RID: 937 RVA: 0x00017113 File Offset: 0x00015313
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
