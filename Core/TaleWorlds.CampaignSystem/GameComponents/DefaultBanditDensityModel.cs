using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x020000EF RID: 239
	public class DefaultBanditDensityModel : BanditDensityModel
	{
		// Token: 0x170005BA RID: 1466
		// (get) Token: 0x0600146A RID: 5226 RVA: 0x0005B085 File Offset: 0x00059285
		public override int NumberOfMaximumLooterParties
		{
			get
			{
				return 150;
			}
		}

		// Token: 0x170005BB RID: 1467
		// (get) Token: 0x0600146B RID: 5227 RVA: 0x0005B08C File Offset: 0x0005928C
		public override int NumberOfMinimumBanditPartiesInAHideoutToInfestIt
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x170005BC RID: 1468
		// (get) Token: 0x0600146C RID: 5228 RVA: 0x0005B08F File Offset: 0x0005928F
		public override int NumberOfMaximumBanditPartiesInEachHideout
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x170005BD RID: 1469
		// (get) Token: 0x0600146D RID: 5229 RVA: 0x0005B092 File Offset: 0x00059292
		public override int NumberOfMaximumBanditPartiesAroundEachHideout
		{
			get
			{
				return 8;
			}
		}

		// Token: 0x170005BE RID: 1470
		// (get) Token: 0x0600146E RID: 5230 RVA: 0x0005B095 File Offset: 0x00059295
		public override int NumberOfMaximumHideoutsAtEachBanditFaction
		{
			get
			{
				return 10;
			}
		}

		// Token: 0x170005BF RID: 1471
		// (get) Token: 0x0600146F RID: 5231 RVA: 0x0005B099 File Offset: 0x00059299
		public override int NumberOfInitialHideoutsAtEachBanditFaction
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x170005C0 RID: 1472
		// (get) Token: 0x06001470 RID: 5232 RVA: 0x0005B09C File Offset: 0x0005929C
		public override int NumberOfMinimumBanditTroopsInHideoutMission
		{
			get
			{
				return 10;
			}
		}

		// Token: 0x170005C1 RID: 1473
		// (get) Token: 0x06001471 RID: 5233 RVA: 0x0005B0A0 File Offset: 0x000592A0
		public override int NumberOfMaximumTroopCountForFirstFightInHideout
		{
			get
			{
				return MathF.Floor(6f * (2f + Campaign.Current.PlayerProgress));
			}
		}

		// Token: 0x170005C2 RID: 1474
		// (get) Token: 0x06001472 RID: 5234 RVA: 0x0005B0BD File Offset: 0x000592BD
		public override int NumberOfMaximumTroopCountForBossFightInHideout
		{
			get
			{
				return MathF.Floor(1f + 5f * (1f + Campaign.Current.PlayerProgress));
			}
		}

		// Token: 0x170005C3 RID: 1475
		// (get) Token: 0x06001473 RID: 5235 RVA: 0x0005B0E0 File Offset: 0x000592E0
		public override float SpawnPercentageForFirstFightInHideoutMission
		{
			get
			{
				return 0.75f;
			}
		}

		// Token: 0x06001474 RID: 5236 RVA: 0x0005B0E8 File Offset: 0x000592E8
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
