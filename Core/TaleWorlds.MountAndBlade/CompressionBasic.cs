using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002E5 RID: 741
	public static class CompressionBasic
	{
		// Token: 0x04000ED5 RID: 3797
		public const float MaxPossibleAbsValueForSecondMaxQuaternionComponent = 0.7071068f;

		// Token: 0x04000ED6 RID: 3798
		public const float MaxPositionZForCompression = 2521f;

		// Token: 0x04000ED7 RID: 3799
		public const float MaxPositionForCompression = 10385f;

		// Token: 0x04000ED8 RID: 3800
		public const float MinPositionForCompression = -100f;

		// Token: 0x04000ED9 RID: 3801
		public const int MaxPeerCount = 511;

		// Token: 0x04000EDA RID: 3802
		public static readonly CompressionInfo.Integer PingValueCompressionInfo = new CompressionInfo.Integer(0, 1023, true);

		// Token: 0x04000EDB RID: 3803
		public static readonly CompressionInfo.Integer LossValueCompressionInfo = new CompressionInfo.Integer(0, 100, true);

		// Token: 0x04000EDC RID: 3804
		public static readonly CompressionInfo.Integer ServerPerformanceStateCompressionInfo = new CompressionInfo.Integer(0, 2, true);

		// Token: 0x04000EDD RID: 3805
		public static CompressionInfo.Float PositionCompressionInfo = new CompressionInfo.Float(-100f, 10385f, 22);

		// Token: 0x04000EDE RID: 3806
		public static CompressionInfo.Float LocalPositionCompressionInfo = new CompressionInfo.Float(-32f, 32f, 16);

		// Token: 0x04000EDF RID: 3807
		public static CompressionInfo.Float LowResLocalPositionCompressionInfo = new CompressionInfo.Float(-32f, 32f, 12);

		// Token: 0x04000EE0 RID: 3808
		public static CompressionInfo.Float BigRangeLowResLocalPositionCompressionInfo = new CompressionInfo.Float(-1000f, 1000f, 16);

		// Token: 0x04000EE1 RID: 3809
		public static CompressionInfo.Integer PlayerCompressionInfo = new CompressionInfo.Integer(-1, 1022, true);

		// Token: 0x04000EE2 RID: 3810
		public static CompressionInfo.UnsignedInteger PeerComponentCompressionInfo = new CompressionInfo.UnsignedInteger(0U, 32);

		// Token: 0x04000EE3 RID: 3811
		public static CompressionInfo.UnsignedInteger GUIDCompressionInfo = new CompressionInfo.UnsignedInteger(0U, 32);

		// Token: 0x04000EE4 RID: 3812
		public static CompressionInfo.Integer FlagsCompressionInfo = new CompressionInfo.Integer(0, 30);

		// Token: 0x04000EE5 RID: 3813
		public static CompressionInfo.Integer GUIDIntCompressionInfo = new CompressionInfo.Integer(-1, 31);

		// Token: 0x04000EE6 RID: 3814
		public static CompressionInfo.Integer MissionObjectIDCompressionInfo = new CompressionInfo.Integer(-1, 4094, true);

		// Token: 0x04000EE7 RID: 3815
		public static CompressionInfo.Float UnitVectorCompressionInfo = new CompressionInfo.Float(-1.024f, 10, 0.002f);

		// Token: 0x04000EE8 RID: 3816
		public static CompressionInfo.Float LowResRadianCompressionInfo = new CompressionInfo.Float(-3.1515927f, 3.1515927f, 8);

		// Token: 0x04000EE9 RID: 3817
		public static CompressionInfo.Float RadianCompressionInfo = new CompressionInfo.Float(-3.1515927f, 3.1515927f, 10);

		// Token: 0x04000EEA RID: 3818
		public static CompressionInfo.Float HighResRadianCompressionInfo = new CompressionInfo.Float(-3.1515927f, 3.1515927f, 13);

		// Token: 0x04000EEB RID: 3819
		public static CompressionInfo.Float UltResRadianCompressionInfo = new CompressionInfo.Float(-3.1515927f, 3.1515927f, 30);

		// Token: 0x04000EEC RID: 3820
		public static CompressionInfo.Float ScaleCompressionInfo = new CompressionInfo.Float(-0.001f, 10, 0.01f);

		// Token: 0x04000EED RID: 3821
		public static CompressionInfo.Float LowResQuaternionCompressionInfo = new CompressionInfo.Float(-0.7071068f, 0.7071068f, 6);

		// Token: 0x04000EEE RID: 3822
		public static CompressionInfo.Integer OmittedQuaternionComponentIndexCompressionInfo = new CompressionInfo.Integer(0, 3, true);

		// Token: 0x04000EEF RID: 3823
		public static CompressionInfo.Float ImpulseCompressionInfo = new CompressionInfo.Float(-500f, 16, 0.0153f);

		// Token: 0x04000EF0 RID: 3824
		public static CompressionInfo.Integer AnimationKeyCompressionInfo = new CompressionInfo.Integer(0, 8000, true);

		// Token: 0x04000EF1 RID: 3825
		public static CompressionInfo.Float AnimationSpeedCompressionInfo = new CompressionInfo.Float(0f, 9, 0.01f);

		// Token: 0x04000EF2 RID: 3826
		public static CompressionInfo.Float AnimationProgressCompressionInfo = new CompressionInfo.Float(0f, 1f, 9);

		// Token: 0x04000EF3 RID: 3827
		public static CompressionInfo.Float VertexAnimationSpeedCompressionInfo = new CompressionInfo.Float(0f, 9, 0.1f);

		// Token: 0x04000EF4 RID: 3828
		public static CompressionInfo.Integer PercentageCompressionInfo = new CompressionInfo.Integer(0, 100, true);

		// Token: 0x04000EF5 RID: 3829
		public static CompressionInfo.Integer EntityChildCountCompressionInfo = new CompressionInfo.Integer(0, 255, true);

		// Token: 0x04000EF6 RID: 3830
		public static CompressionInfo.Integer AgentHitDamageCompressionInfo = new CompressionInfo.Integer(0, 2000, true);

		// Token: 0x04000EF7 RID: 3831
		public static CompressionInfo.Integer AgentHitModifiedDamageCompressionInfo = new CompressionInfo.Integer(-2000, 2000, true);

		// Token: 0x04000EF8 RID: 3832
		public static CompressionInfo.Float AgentHitRelativeSpeedCompressionInfo = new CompressionInfo.Float(0f, 17, 0.01f);

		// Token: 0x04000EF9 RID: 3833
		public static CompressionInfo.Integer AgentHitArmorCompressionInfo = new CompressionInfo.Integer(0, 200, true);

		// Token: 0x04000EFA RID: 3834
		public static CompressionInfo.Integer AgentHitBoneIndexCompressionInfo = new CompressionInfo.Integer(-1, 63, true);

		// Token: 0x04000EFB RID: 3835
		public static CompressionInfo.Integer AgentHitBodyPartCompressionInfo = new CompressionInfo.Integer(-1, 8, true);

		// Token: 0x04000EFC RID: 3836
		public static CompressionInfo.Integer AgentHitDamageTypeCompressionInfo = new CompressionInfo.Integer(-1, 2, true);

		// Token: 0x04000EFD RID: 3837
		public static CompressionInfo.Integer RoundGoldAmountCompressionInfo = new CompressionInfo.Integer(-1, 2000, true);

		// Token: 0x04000EFE RID: 3838
		public static CompressionInfo.Integer DebugIntNonCompressionInfo = new CompressionInfo.Integer(int.MinValue, 32);

		// Token: 0x04000EFF RID: 3839
		public static CompressionInfo.UnsignedLongInteger DebugULongNonCompressionInfo = new CompressionInfo.UnsignedLongInteger(0UL, 64);

		// Token: 0x04000F00 RID: 3840
		public static CompressionInfo.Float AgentAgeCompressionInfo = new CompressionInfo.Float(0f, 128f, 10);

		// Token: 0x04000F01 RID: 3841
		public static CompressionInfo.Float FaceKeyDataCompressionInfo = new CompressionInfo.Float(0f, 1f, 10);

		// Token: 0x04000F02 RID: 3842
		public static CompressionInfo.Integer PlayerChosenBadgeCompressionInfo = new CompressionInfo.Integer(-1, 8);

		// Token: 0x04000F03 RID: 3843
		public static CompressionInfo.Integer MaxNumberOfPlayersCompressionInfo = new CompressionInfo.Integer(MultiplayerOptions.OptionType.MaxNumberOfPlayers.GetMinimumValue(), MultiplayerOptions.OptionType.MaxNumberOfPlayers.GetMaximumValue(), true);

		// Token: 0x04000F04 RID: 3844
		public static CompressionInfo.Integer PlayerCountLimitForWarmupCompressionInfo;

		// Token: 0x04000F05 RID: 3845
		public static CompressionInfo.Integer MinNumberOfPlayersForMatchStartCompressionInfo = new CompressionInfo.Integer(MultiplayerOptions.OptionType.MinNumberOfPlayersForMatchStart.GetMinimumValue(), MultiplayerOptions.OptionType.MinNumberOfPlayersForMatchStart.GetMaximumValue(), true);

		// Token: 0x04000F06 RID: 3846
		public static CompressionInfo.Integer MapTimeLimitCompressionInfo = new CompressionInfo.Integer(MultiplayerOptions.OptionType.MapTimeLimit.GetMinimumValue(), MultiplayerOptions.OptionType.MapTimeLimit.GetMaximumValue(), true);

		// Token: 0x04000F07 RID: 3847
		public static CompressionInfo.Integer RoundTotalCompressionInfo = new CompressionInfo.Integer(MultiplayerOptions.OptionType.RoundTotal.GetMinimumValue(), MultiplayerOptions.OptionType.RoundTotal.GetMaximumValue(), true);

		// Token: 0x04000F08 RID: 3848
		public static CompressionInfo.Integer RoundTimeLimitCompressionInfo = new CompressionInfo.Integer(MultiplayerOptions.OptionType.RoundTimeLimit.GetMinimumValue(), MultiplayerOptions.OptionType.RoundTimeLimit.GetMaximumValue(), true);

		// Token: 0x04000F09 RID: 3849
		public static CompressionInfo.Integer RoundPreparationTimeLimitCompressionInfo = new CompressionInfo.Integer(MultiplayerOptions.OptionType.RoundPreparationTimeLimit.GetMinimumValue(), MultiplayerOptions.OptionType.RoundPreparationTimeLimit.GetMaximumValue(), true);

		// Token: 0x04000F0A RID: 3850
		public static CompressionInfo.Integer FlagRemovalTimeCompressionInfo;

		// Token: 0x04000F0B RID: 3851
		public static CompressionInfo.Integer RespawnPeriodCompressionInfo = new CompressionInfo.Integer(MultiplayerOptions.OptionType.RespawnPeriodTeam1.GetMinimumValue(), MultiplayerOptions.OptionType.RespawnPeriodTeam1.GetMaximumValue(), true);

		// Token: 0x04000F0C RID: 3852
		public static CompressionInfo.Integer GoldGainChangePercentageCompressionInfo = new CompressionInfo.Integer(MultiplayerOptions.OptionType.GoldGainChangePercentageTeam1.GetMinimumValue(), MultiplayerOptions.OptionType.GoldGainChangePercentageTeam1.GetMaximumValue(), true);

		// Token: 0x04000F0D RID: 3853
		public static CompressionInfo.Integer SpectatorCameraTypeCompressionInfo = new CompressionInfo.Integer(-1, 7, true);

		// Token: 0x04000F0E RID: 3854
		public static CompressionInfo.Integer PollAcceptThresholdCompressionInfo = new CompressionInfo.Integer(MultiplayerOptions.OptionType.PollAcceptThreshold.GetMinimumValue(), MultiplayerOptions.OptionType.PollAcceptThreshold.GetMaximumValue(), true);

		// Token: 0x04000F0F RID: 3855
		public static CompressionInfo.Integer NumberOfBotsTeamCompressionInfo = new CompressionInfo.Integer(MultiplayerOptions.OptionType.NumberOfBotsTeam1.GetMinimumValue(), MultiplayerOptions.OptionType.NumberOfBotsTeam1.GetMaximumValue(), true);

		// Token: 0x04000F10 RID: 3856
		public static CompressionInfo.Integer NumberOfBotsPerFormationCompressionInfo = new CompressionInfo.Integer(MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetMinimumValue(), MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetMaximumValue(), true);

		// Token: 0x04000F11 RID: 3857
		public static CompressionInfo.Integer AutoTeamBalanceLimitCompressionInfo = new CompressionInfo.Integer(0, 5, true);

		// Token: 0x04000F12 RID: 3858
		public static CompressionInfo.Integer FriendlyFireDamageCompressionInfo = new CompressionInfo.Integer(MultiplayerOptions.OptionType.FriendlyFireDamageMeleeFriendPercent.GetMinimumValue(), MultiplayerOptions.OptionType.FriendlyFireDamageMeleeFriendPercent.GetMaximumValue(), true);

		// Token: 0x04000F13 RID: 3859
		public static CompressionInfo.Integer SiegeEngineSpeedPercentCompressionInfo;

		// Token: 0x04000F14 RID: 3860
		public static CompressionInfo.Integer ForcedAvatarIndexCompressionInfo = new CompressionInfo.Integer(-1, 100, true);

		// Token: 0x04000F15 RID: 3861
		public static CompressionInfo.Integer IntermissionStateCompressionInfo = new CompressionInfo.Integer(0, Enum.GetNames(typeof(MultiplayerIntermissionState)).Length - 1, false);

		// Token: 0x04000F16 RID: 3862
		public static CompressionInfo.Float IntermissionTimerCompressionInfo = new CompressionInfo.Float(0f, 240f, 14);

		// Token: 0x04000F17 RID: 3863
		public static CompressionInfo.Integer IntermissionMapVoteItemCountCompressionInfo = new CompressionInfo.Integer(0, 99, true);

		// Token: 0x04000F18 RID: 3864
		public static CompressionInfo.Integer IntermissionVoterCountCompressionInfo = new CompressionInfo.Integer(0, 1023, true);

		// Token: 0x04000F19 RID: 3865
		public static CompressionInfo.Integer ActionCodeCompressionInfo;

		// Token: 0x04000F1A RID: 3866
		public static CompressionInfo.Integer AnimationIndexCompressionInfo;

		// Token: 0x04000F1B RID: 3867
		public static CompressionInfo.Integer CultureIndexCompressionInfo;

		// Token: 0x04000F1C RID: 3868
		public static CompressionInfo.Integer SoundEventsCompressionInfo;

		// Token: 0x04000F1D RID: 3869
		public static CompressionInfo.Integer NetworkComponentEventTypeFromServerCompressionInfo;

		// Token: 0x04000F1E RID: 3870
		public static CompressionInfo.Integer NetworkComponentEventTypeFromClientCompressionInfo;

		// Token: 0x04000F1F RID: 3871
		public static CompressionInfo.Integer TroopTypeCompressionInfo = new CompressionInfo.Integer(-1, 3, true);

		// Token: 0x04000F20 RID: 3872
		public static CompressionInfo.Integer BannerDataCountCompressionInfo = new CompressionInfo.Integer(0, 31, true);

		// Token: 0x04000F21 RID: 3873
		public static CompressionInfo.Integer BannerDataMeshIdCompressionInfo = new CompressionInfo.Integer(0, 13);

		// Token: 0x04000F22 RID: 3874
		public static CompressionInfo.Integer BannerDataColorIndexCompressionInfo = new CompressionInfo.Integer(0, 10);

		// Token: 0x04000F23 RID: 3875
		public static CompressionInfo.Integer BannerDataSizeCompressionInfo = new CompressionInfo.Integer(-8000, 8000, true);

		// Token: 0x04000F24 RID: 3876
		public static CompressionInfo.Integer BannerDataRotationCompressionInfo = new CompressionInfo.Integer(0, 360, true);
	}
}
