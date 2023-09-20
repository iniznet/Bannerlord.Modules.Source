using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x0200016C RID: 364
	public abstract class SettlementAccessModel : GameModel
	{
		// Token: 0x060018C6 RID: 6342
		public abstract void CanMainHeroEnterSettlement(Settlement settlement, out SettlementAccessModel.AccessDetails accessDetails);

		// Token: 0x060018C7 RID: 6343
		public abstract void CanMainHeroEnterLordsHall(Settlement settlement, out SettlementAccessModel.AccessDetails accessDetails);

		// Token: 0x060018C8 RID: 6344
		public abstract void CanMainHeroEnterDungeon(Settlement settlement, out SettlementAccessModel.AccessDetails accessDetails);

		// Token: 0x060018C9 RID: 6345
		public abstract bool CanMainHeroAccessLocation(Settlement settlement, string locationId, out bool disableOption, out TextObject disabledText);

		// Token: 0x060018CA RID: 6346
		public abstract bool CanMainHeroDoSettlementAction(Settlement settlement, SettlementAccessModel.SettlementAction settlementAction, out bool disableOption, out TextObject disabledText);

		// Token: 0x060018CB RID: 6347
		public abstract bool IsRequestMeetingOptionAvailable(Settlement settlement, out bool disableOption, out TextObject disabledText);

		// Token: 0x02000552 RID: 1362
		public enum AccessLevel
		{
			// Token: 0x04001665 RID: 5733
			NoAccess,
			// Token: 0x04001666 RID: 5734
			LimitedAccess,
			// Token: 0x04001667 RID: 5735
			FullAccess
		}

		// Token: 0x02000553 RID: 1363
		public enum AccessMethod
		{
			// Token: 0x04001669 RID: 5737
			None,
			// Token: 0x0400166A RID: 5738
			Direct,
			// Token: 0x0400166B RID: 5739
			ByRequest
		}

		// Token: 0x02000554 RID: 1364
		public enum AccessLimitationReason
		{
			// Token: 0x0400166D RID: 5741
			None,
			// Token: 0x0400166E RID: 5742
			HostileFaction,
			// Token: 0x0400166F RID: 5743
			RelationshipWithOwner,
			// Token: 0x04001670 RID: 5744
			CrimeRating,
			// Token: 0x04001671 RID: 5745
			VillageIsLooted,
			// Token: 0x04001672 RID: 5746
			Disguised,
			// Token: 0x04001673 RID: 5747
			ClanTier,
			// Token: 0x04001674 RID: 5748
			LocationEmpty
		}

		// Token: 0x02000555 RID: 1365
		public enum LimitedAccessSolution
		{
			// Token: 0x04001676 RID: 5750
			None,
			// Token: 0x04001677 RID: 5751
			Bribe,
			// Token: 0x04001678 RID: 5752
			Disguise
		}

		// Token: 0x02000556 RID: 1366
		public enum PreliminaryActionObligation
		{
			// Token: 0x0400167A RID: 5754
			None,
			// Token: 0x0400167B RID: 5755
			Optional,
			// Token: 0x0400167C RID: 5756
			Must
		}

		// Token: 0x02000557 RID: 1367
		public enum PreliminaryActionType
		{
			// Token: 0x0400167E RID: 5758
			None,
			// Token: 0x0400167F RID: 5759
			SettlementIsTaken,
			// Token: 0x04001680 RID: 5760
			FaceCharges
		}

		// Token: 0x02000558 RID: 1368
		public enum SettlementAction
		{
			// Token: 0x04001682 RID: 5762
			RecruitTroops,
			// Token: 0x04001683 RID: 5763
			Craft,
			// Token: 0x04001684 RID: 5764
			WalkAroundTheArena,
			// Token: 0x04001685 RID: 5765
			JoinTournament,
			// Token: 0x04001686 RID: 5766
			WatchTournament,
			// Token: 0x04001687 RID: 5767
			Trade,
			// Token: 0x04001688 RID: 5768
			WaitInSettlement,
			// Token: 0x04001689 RID: 5769
			ManageTown
		}

		// Token: 0x02000559 RID: 1369
		public struct AccessDetails
		{
			// Token: 0x0400168A RID: 5770
			public SettlementAccessModel.AccessLevel AccessLevel;

			// Token: 0x0400168B RID: 5771
			public SettlementAccessModel.AccessMethod AccessMethod;

			// Token: 0x0400168C RID: 5772
			public SettlementAccessModel.AccessLimitationReason AccessLimitationReason;

			// Token: 0x0400168D RID: 5773
			public SettlementAccessModel.LimitedAccessSolution LimitedAccessSolution;

			// Token: 0x0400168E RID: 5774
			public SettlementAccessModel.PreliminaryActionObligation PreliminaryActionObligation;

			// Token: 0x0400168F RID: 5775
			public SettlementAccessModel.PreliminaryActionType PreliminaryActionType;
		}
	}
}
