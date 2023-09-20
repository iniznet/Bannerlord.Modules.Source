using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	public abstract class SettlementAccessModel : GameModel
	{
		public abstract void CanMainHeroEnterSettlement(Settlement settlement, out SettlementAccessModel.AccessDetails accessDetails);

		public abstract void CanMainHeroEnterLordsHall(Settlement settlement, out SettlementAccessModel.AccessDetails accessDetails);

		public abstract void CanMainHeroEnterDungeon(Settlement settlement, out SettlementAccessModel.AccessDetails accessDetails);

		public abstract bool CanMainHeroAccessLocation(Settlement settlement, string locationId, out bool disableOption, out TextObject disabledText);

		public abstract bool CanMainHeroDoSettlementAction(Settlement settlement, SettlementAccessModel.SettlementAction settlementAction, out bool disableOption, out TextObject disabledText);

		public abstract bool IsRequestMeetingOptionAvailable(Settlement settlement, out bool disableOption, out TextObject disabledText);

		public enum AccessLevel
		{
			NoAccess,
			LimitedAccess,
			FullAccess
		}

		public enum AccessMethod
		{
			None,
			Direct,
			ByRequest
		}

		public enum AccessLimitationReason
		{
			None,
			HostileFaction,
			RelationshipWithOwner,
			CrimeRating,
			VillageIsLooted,
			Disguised,
			ClanTier,
			LocationEmpty
		}

		public enum LimitedAccessSolution
		{
			None,
			Bribe,
			Disguise
		}

		public enum PreliminaryActionObligation
		{
			None,
			Optional,
			Must
		}

		public enum PreliminaryActionType
		{
			None,
			SettlementIsTaken,
			FaceCharges
		}

		public enum SettlementAction
		{
			RecruitTroops,
			Craft,
			WalkAroundTheArena,
			JoinTournament,
			WatchTournament,
			Trade,
			WaitInSettlement,
			ManageTown
		}

		public struct AccessDetails
		{
			public SettlementAccessModel.AccessLevel AccessLevel;

			public SettlementAccessModel.AccessMethod AccessMethod;

			public SettlementAccessModel.AccessLimitationReason AccessLimitationReason;

			public SettlementAccessModel.LimitedAccessSolution LimitedAccessSolution;

			public SettlementAccessModel.PreliminaryActionObligation PreliminaryActionObligation;

			public SettlementAccessModel.PreliminaryActionType PreliminaryActionType;
		}
	}
}
