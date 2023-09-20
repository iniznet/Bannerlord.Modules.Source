using System;

namespace TaleWorlds.MountAndBlade
{
	public static class CompressionBasic
	{
		public const float MaxPossibleAbsValueForSecondMaxQuaternionComponent = 0.7071068f;

		public const float MaxPositionZForCompression = 2521f;

		public const float MaxPositionForCompression = 10385f;

		public const float MinPositionForCompression = -100f;

		public const int MaxPeerCount = 511;

		public static readonly CompressionInfo.Integer PingValueCompressionInfo = new CompressionInfo.Integer(0, 1023, true);

		public static readonly CompressionInfo.Integer LossValueCompressionInfo = new CompressionInfo.Integer(0, 100, true);

		public static readonly CompressionInfo.Integer ServerPerformanceStateCompressionInfo = new CompressionInfo.Integer(0, 2, true);

		public static readonly CompressionInfo.UnsignedInteger ColorCompressionInfo = new CompressionInfo.UnsignedInteger(0U, 32);

		public static readonly CompressionInfo.Integer ItemDataValueCompressionInfo = new CompressionInfo.Integer(0, 16);

		public static readonly CompressionInfo.Integer RandomSeedCompressionInfo = new CompressionInfo.Integer(0, 2000, true);

		public static readonly CompressionInfo.Float PositionCompressionInfo = new CompressionInfo.Float(-100f, 10385f, 22);

		public static readonly CompressionInfo.Float LocalPositionCompressionInfo = new CompressionInfo.Float(-32f, 32f, 16);

		public static readonly CompressionInfo.Float LowResLocalPositionCompressionInfo = new CompressionInfo.Float(-32f, 32f, 12);

		public static readonly CompressionInfo.Float BigRangeLowResLocalPositionCompressionInfo = new CompressionInfo.Float(-1000f, 1000f, 16);

		public static readonly CompressionInfo.Integer PlayerCompressionInfo = new CompressionInfo.Integer(-1, 1022, true);

		public static readonly CompressionInfo.UnsignedInteger PeerComponentCompressionInfo = new CompressionInfo.UnsignedInteger(0U, 32);

		public static readonly CompressionInfo.UnsignedInteger GUIDCompressionInfo = new CompressionInfo.UnsignedInteger(0U, 32);

		public static readonly CompressionInfo.Integer FlagsCompressionInfo = new CompressionInfo.Integer(0, 30);

		public static readonly CompressionInfo.Integer GUIDIntCompressionInfo = new CompressionInfo.Integer(-1, 31);

		public static readonly CompressionInfo.Integer MissionObjectIDCompressionInfo = new CompressionInfo.Integer(-1, 4094, true);

		public static readonly CompressionInfo.Float UnitVectorCompressionInfo = new CompressionInfo.Float(-1.024f, 10, 0.002f);

		public static readonly CompressionInfo.Float LowResRadianCompressionInfo = new CompressionInfo.Float(-3.1515927f, 3.1515927f, 8);

		public static readonly CompressionInfo.Float RadianCompressionInfo = new CompressionInfo.Float(-3.1515927f, 3.1515927f, 10);

		public static readonly CompressionInfo.Float HighResRadianCompressionInfo = new CompressionInfo.Float(-3.1515927f, 3.1515927f, 13);

		public static readonly CompressionInfo.Float UltResRadianCompressionInfo = new CompressionInfo.Float(-3.1515927f, 3.1515927f, 30);

		public static readonly CompressionInfo.Float ScaleCompressionInfo = new CompressionInfo.Float(-0.001f, 10, 0.01f);

		public static readonly CompressionInfo.Float LowResQuaternionCompressionInfo = new CompressionInfo.Float(-0.7071068f, 0.7071068f, 6);

		public static readonly CompressionInfo.Integer OmittedQuaternionComponentIndexCompressionInfo = new CompressionInfo.Integer(0, 3, true);

		public static readonly CompressionInfo.Float ImpulseCompressionInfo = new CompressionInfo.Float(-500f, 16, 0.0153f);

		public static readonly CompressionInfo.Integer AnimationKeyCompressionInfo = new CompressionInfo.Integer(0, 8000, true);

		public static readonly CompressionInfo.Float AnimationSpeedCompressionInfo = new CompressionInfo.Float(0f, 9, 0.01f);

		public static readonly CompressionInfo.Float AnimationProgressCompressionInfo = new CompressionInfo.Float(0f, 1f, 9);

		public static readonly CompressionInfo.Float VertexAnimationSpeedCompressionInfo = new CompressionInfo.Float(0f, 9, 0.1f);

		public static readonly CompressionInfo.Integer PercentageCompressionInfo = new CompressionInfo.Integer(0, 100, true);

		public static readonly CompressionInfo.Integer EntityChildCountCompressionInfo = new CompressionInfo.Integer(0, 8);

		public static readonly CompressionInfo.Integer AgentHitDamageCompressionInfo = new CompressionInfo.Integer(0, 2000, true);

		public static readonly CompressionInfo.Integer AgentHitModifiedDamageCompressionInfo = new CompressionInfo.Integer(-2000, 2000, true);

		public static readonly CompressionInfo.Float AgentHitRelativeSpeedCompressionInfo = new CompressionInfo.Float(0f, 17, 0.01f);

		public static readonly CompressionInfo.Integer AgentHitArmorCompressionInfo = new CompressionInfo.Integer(0, 200, true);

		public static readonly CompressionInfo.Integer AgentHitBoneIndexCompressionInfo = new CompressionInfo.Integer(-1, 63, true);

		public static readonly CompressionInfo.Integer AgentHitBodyPartCompressionInfo = new CompressionInfo.Integer(-1, 8, true);

		public static readonly CompressionInfo.Integer AgentHitDamageTypeCompressionInfo = new CompressionInfo.Integer(-1, 2, true);

		public static readonly CompressionInfo.Integer RoundGoldAmountCompressionInfo = new CompressionInfo.Integer(-1, 2000, true);

		public static readonly CompressionInfo.Integer DebugIntNonCompressionInfo = new CompressionInfo.Integer(int.MinValue, 32);

		public static readonly CompressionInfo.UnsignedLongInteger DebugULongNonCompressionInfo = new CompressionInfo.UnsignedLongInteger(0UL, 64);

		public static readonly CompressionInfo.Float AgentAgeCompressionInfo = new CompressionInfo.Float(0f, 128f, 10);

		public static readonly CompressionInfo.Float FaceKeyDataCompressionInfo = new CompressionInfo.Float(0f, 1f, 10);

		public static readonly CompressionInfo.Integer PlayerChosenBadgeCompressionInfo = new CompressionInfo.Integer(-1, 8);

		public static readonly CompressionInfo.Integer MaxNumberOfPlayersCompressionInfo = new CompressionInfo.Integer(MultiplayerOptions.OptionType.MaxNumberOfPlayers.GetMinimumValue(), MultiplayerOptions.OptionType.MaxNumberOfPlayers.GetMaximumValue(), true);

		public static readonly CompressionInfo.Integer MinNumberOfPlayersForMatchStartCompressionInfo = new CompressionInfo.Integer(MultiplayerOptions.OptionType.MinNumberOfPlayersForMatchStart.GetMinimumValue(), MultiplayerOptions.OptionType.MinNumberOfPlayersForMatchStart.GetMaximumValue(), true);

		public static readonly CompressionInfo.Integer MapTimeLimitCompressionInfo = new CompressionInfo.Integer(MultiplayerOptions.OptionType.MapTimeLimit.GetMinimumValue(), MultiplayerOptions.OptionType.MapTimeLimit.GetMaximumValue(), true);

		public static readonly CompressionInfo.Integer RoundTotalCompressionInfo = new CompressionInfo.Integer(MultiplayerOptions.OptionType.RoundTotal.GetMinimumValue(), MultiplayerOptions.OptionType.RoundTotal.GetMaximumValue(), true);

		public static readonly CompressionInfo.Integer RoundTimeLimitCompressionInfo = new CompressionInfo.Integer(MultiplayerOptions.OptionType.RoundTimeLimit.GetMinimumValue(), MultiplayerOptions.OptionType.RoundTimeLimit.GetMaximumValue(), true);

		public static readonly CompressionInfo.Integer RoundPreparationTimeLimitCompressionInfo = new CompressionInfo.Integer(MultiplayerOptions.OptionType.RoundPreparationTimeLimit.GetMinimumValue(), MultiplayerOptions.OptionType.RoundPreparationTimeLimit.GetMaximumValue(), true);

		public static readonly CompressionInfo.Integer RespawnPeriodCompressionInfo = new CompressionInfo.Integer(MultiplayerOptions.OptionType.RespawnPeriodTeam1.GetMinimumValue(), MultiplayerOptions.OptionType.RespawnPeriodTeam1.GetMaximumValue(), true);

		public static readonly CompressionInfo.Integer GoldGainChangePercentageCompressionInfo = new CompressionInfo.Integer(MultiplayerOptions.OptionType.GoldGainChangePercentageTeam1.GetMinimumValue(), MultiplayerOptions.OptionType.GoldGainChangePercentageTeam1.GetMaximumValue(), true);

		public static readonly CompressionInfo.Integer SpectatorCameraTypeCompressionInfo = new CompressionInfo.Integer(-1, 7, true);

		public static readonly CompressionInfo.Integer PollAcceptThresholdCompressionInfo = new CompressionInfo.Integer(MultiplayerOptions.OptionType.PollAcceptThreshold.GetMinimumValue(), MultiplayerOptions.OptionType.PollAcceptThreshold.GetMaximumValue(), true);

		public static readonly CompressionInfo.Integer NumberOfBotsTeamCompressionInfo = new CompressionInfo.Integer(MultiplayerOptions.OptionType.NumberOfBotsTeam1.GetMinimumValue(), MultiplayerOptions.OptionType.NumberOfBotsTeam1.GetMaximumValue(), true);

		public static readonly CompressionInfo.Integer NumberOfBotsPerFormationCompressionInfo = new CompressionInfo.Integer(MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetMinimumValue(), MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetMaximumValue(), true);

		public static readonly CompressionInfo.Integer AutoTeamBalanceLimitCompressionInfo = new CompressionInfo.Integer(MultiplayerOptions.OptionType.AutoTeamBalanceThreshold.GetMinimumValue(), MultiplayerOptions.OptionType.AutoTeamBalanceThreshold.GetMaximumValue(), true);

		public static readonly CompressionInfo.Integer FriendlyFireDamageCompressionInfo = new CompressionInfo.Integer(MultiplayerOptions.OptionType.FriendlyFireDamageMeleeFriendPercent.GetMinimumValue(), MultiplayerOptions.OptionType.FriendlyFireDamageMeleeFriendPercent.GetMaximumValue(), true);

		public static readonly CompressionInfo.Integer ForcedAvatarIndexCompressionInfo = new CompressionInfo.Integer(-1, 99, true);

		public static readonly CompressionInfo.Integer IntermissionStateCompressionInfo = new CompressionInfo.Integer(0, Enum.GetNames(typeof(MultiplayerIntermissionState)).Length - 1, false);

		public static readonly CompressionInfo.Float IntermissionTimerCompressionInfo = new CompressionInfo.Float(0f, 240f, 14);

		public static readonly CompressionInfo.Integer IntermissionMapVoteItemCountCompressionInfo = new CompressionInfo.Integer(0, 99, true);

		public static readonly CompressionInfo.Integer IntermissionVoterCountCompressionInfo = new CompressionInfo.Integer(0, 1022, true);

		public static CompressionInfo.Integer ActionCodeCompressionInfo;

		public static CompressionInfo.Integer AnimationIndexCompressionInfo;

		public static CompressionInfo.Integer CultureIndexCompressionInfo;

		public static CompressionInfo.Integer SoundEventsCompressionInfo;

		public static CompressionInfo.Integer NetworkComponentEventTypeFromServerCompressionInfo;

		public static CompressionInfo.Integer NetworkComponentEventTypeFromClientCompressionInfo;

		public static readonly CompressionInfo.Integer TroopTypeCompressionInfo = new CompressionInfo.Integer(-1, 2, true);

		public static readonly CompressionInfo.Integer BannerDataCountCompressionInfo = new CompressionInfo.Integer(0, 31, true);

		public static readonly CompressionInfo.Integer BannerDataMeshIdCompressionInfo = new CompressionInfo.Integer(0, 13);

		public static readonly CompressionInfo.Integer BannerDataColorIndexCompressionInfo = new CompressionInfo.Integer(0, 10);

		public static readonly CompressionInfo.Integer BannerDataSizeCompressionInfo = new CompressionInfo.Integer(-8000, 8000, true);

		public static readonly CompressionInfo.Integer BannerDataRotationCompressionInfo = new CompressionInfo.Integer(0, 360, true);
	}
}
