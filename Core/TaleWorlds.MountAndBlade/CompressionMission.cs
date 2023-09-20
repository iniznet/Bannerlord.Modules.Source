using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Diamond;

namespace TaleWorlds.MountAndBlade
{
	public static class CompressionMission
	{
		public static readonly CompressionInfo.Float DebugScaleValueCompressionInfo = new CompressionInfo.Float(0.5f, 1.5f, 13);

		public static readonly CompressionInfo.Integer AgentCompressionInfo = new CompressionInfo.Integer(-1, 11);

		public static readonly CompressionInfo.Integer WeaponAttachmentIndexCompressionInfo = new CompressionInfo.Integer(0, 8);

		public static readonly CompressionInfo.Integer AgentOffsetCompressionInfo = new CompressionInfo.Integer(0, 8);

		public static readonly CompressionInfo.Integer AgentHealthCompressionInfo = new CompressionInfo.Integer(-1, 11);

		public static readonly CompressionInfo.Integer AgentControllerCompressionInfo = new CompressionInfo.Integer(0, 2, true);

		public static readonly CompressionInfo.Integer TeamCompressionInfo = new CompressionInfo.Integer(-1, 10);

		public static readonly CompressionInfo.Integer TeamSideCompressionInfo = new CompressionInfo.Integer(-1, 4);

		public static readonly CompressionInfo.Integer RoundEndReasonCompressionInfo = new CompressionInfo.Integer(-1, 2, true);

		public static readonly CompressionInfo.Integer TeamScoreCompressionInfo = new CompressionInfo.Integer(-1023000, 1023000, true);

		public static readonly CompressionInfo.Integer FactionCompressionInfo = new CompressionInfo.Integer(0, 4);

		public static readonly CompressionInfo.Integer MissionOrderTypeCompressionInfo = new CompressionInfo.Integer(-1, 5);

		public static readonly CompressionInfo.Integer MissionRoundCountCompressionInfo = new CompressionInfo.Integer(-1, 7);

		public static readonly CompressionInfo.Integer MissionRoundStateCompressionInfo = new CompressionInfo.Integer(-1, 5, true);

		public static readonly CompressionInfo.Integer RoundTimeCompressionInfo = new CompressionInfo.Integer(0, MultiplayerOptions.OptionType.RoundTimeLimit.GetMaximumValue(), true);

		public static readonly CompressionInfo.Integer SelectedTroopIndexCompressionInfo = new CompressionInfo.Integer(-1, 15, true);

		public static readonly CompressionInfo.Integer MissileCompressionInfo = new CompressionInfo.Integer(0, 10);

		public static readonly CompressionInfo.Float MissileSpeedCompressionInfo = new CompressionInfo.Float(0f, 12, 0.05f);

		public static readonly CompressionInfo.Integer MissileCollisionReactionCompressionInfo = new CompressionInfo.Integer(0, 3, true);

		public static readonly CompressionInfo.Integer FlagCapturePointIndexCompressionInfo = new CompressionInfo.Integer(0, 3);

		public static readonly CompressionInfo.Integer FlagpoleIndexCompressionInfo = new CompressionInfo.Integer(0, 5, true);

		public static readonly CompressionInfo.Float FlagCapturePointDurationCompressionInfo = new CompressionInfo.Float(-1f, 14, 0.01f);

		public static readonly CompressionInfo.Float FlagProgressCompressionInfo = new CompressionInfo.Float(-1f, 1f, 12);

		public static readonly CompressionInfo.Float FlagClassicProgressCompressionInfo = new CompressionInfo.Float(0f, 1f, 11);

		public static readonly CompressionInfo.Integer FlagDirectionEnumCompressionInfo = new CompressionInfo.Integer(-1, 2, true);

		public static readonly CompressionInfo.Float FlagSpeedCompressionInfo = new CompressionInfo.Float(-1f, 14, 0.01f);

		public static readonly CompressionInfo.Integer FlagCaptureResultCompressionInfo = new CompressionInfo.Integer(0, 3, true);

		public static readonly CompressionInfo.Integer UsableGameObjectDestructionStateCompressionInfo = new CompressionInfo.Integer(0, 3);

		public static readonly CompressionInfo.Float UsableGameObjectHealthCompressionInfo = new CompressionInfo.Float(-1f, 18, 0.1f);

		public static readonly CompressionInfo.Float UsableGameObjectBlowMagnitude = new CompressionInfo.Float(0f, DestructableComponent.MaxBlowMagnitude, 8);

		public static readonly CompressionInfo.Float UsableGameObjectBlowDirection = new CompressionInfo.Float(-1f, 1f, 7);

		public static readonly CompressionInfo.Float CapturePointProgressCompressionInfo = new CompressionInfo.Float(0f, 1f, 10);

		public static readonly CompressionInfo.Integer ItemSlotCompressionInfo = new CompressionInfo.Integer(0, 4, true);

		public static readonly CompressionInfo.Integer WieldSlotCompressionInfo = new CompressionInfo.Integer(-1, 4, true);

		public static readonly CompressionInfo.Integer ItemDataCompressionInfo = new CompressionInfo.Integer(0, 10);

		public static readonly CompressionInfo.Integer WeaponReloadPhaseCompressionInfo = new CompressionInfo.Integer(0, 9, true);

		public static readonly CompressionInfo.Integer WeaponUsageIndexCompressionInfo = new CompressionInfo.Integer(0, 2);

		public static readonly CompressionInfo.Integer TauntIndexCompressionInfo = new CompressionInfo.Integer(0, TauntUsageManager.GetTauntItemCount() - 1, true);

		public static readonly CompressionInfo.Integer BarkIndexCompressionInfo = new CompressionInfo.Integer(0, SkinVoiceManager.VoiceType.MpBarks.Length - 1, true);

		public static readonly CompressionInfo.Integer UsageDirectionCompressionInfo = new CompressionInfo.Integer(-1, 9, true);

		public static readonly CompressionInfo.Float SpawnedItemVelocityCompressionInfo = new CompressionInfo.Float(-100f, 100f, 12);

		public static readonly CompressionInfo.Float SpawnedItemAngularVelocityCompressionInfo = new CompressionInfo.Float(-100f, 100f, 12);

		public static readonly CompressionInfo.UnsignedInteger SpawnedItemWeaponSpawnFlagCompressionInfo = new CompressionInfo.UnsignedInteger(0U, EnumHelper.GetCombinedUIntEnumFlagsValue(typeof(Mission.WeaponSpawnFlags)), true);

		public static readonly CompressionInfo.Integer RangedSiegeWeaponAmmoCompressionInfo = new CompressionInfo.Integer(0, 7);

		public static readonly CompressionInfo.Integer RangedSiegeWeaponAmmoIndexCompressionInfo = new CompressionInfo.Integer(0, 3);

		public static readonly CompressionInfo.Integer RangedSiegeWeaponStateCompressionInfo = new CompressionInfo.Integer(0, 8, true);

		public static readonly CompressionInfo.Integer SiegeLadderStateCompressionInfo = new CompressionInfo.Integer(0, 9, true);

		public static readonly CompressionInfo.Integer BatteringRamStateCompressionInfo = new CompressionInfo.Integer(0, 2, true);

		public static readonly CompressionInfo.Integer SiegeLadderAnimationStateCompressionInfo = new CompressionInfo.Integer(0, 2, true);

		public static readonly CompressionInfo.Float SiegeMachineComponentAngularSpeedCompressionInfo = new CompressionInfo.Float(-20f, 20f, 12);

		public static readonly CompressionInfo.Integer SiegeTowerGateStateCompressionInfo = new CompressionInfo.Integer(0, 3, true);

		public static readonly CompressionInfo.Integer NumberOfPacesCompressionInfo = new CompressionInfo.Integer(0, 3);

		public static readonly CompressionInfo.Float WalkingSpeedLimitCompressionInfo = new CompressionInfo.Float(-0.01f, 9, 0.01f);

		public static readonly CompressionInfo.Float StepSizeCompressionInfo = new CompressionInfo.Float(-0.01f, 7, 0.01f);

		public static readonly CompressionInfo.Integer BoneIndexCompressionInfo = new CompressionInfo.Integer(0, 63, true);

		public static readonly CompressionInfo.Integer AgentPrefabComponentIndexCompressionInfo = new CompressionInfo.Integer(0, 4);

		public static readonly CompressionInfo.Integer MultiplayerPollRejectReasonCompressionInfo = new CompressionInfo.Integer(0, 3, true);

		public static readonly CompressionInfo.Integer MultiplayerNotificationCompressionInfo = new CompressionInfo.Integer(0, MultiplayerGameNotificationsComponent.NotificationCount, true);

		public static readonly CompressionInfo.Integer MultiplayerNotificationParameterCompressionInfo = new CompressionInfo.Integer(-1, 8);

		public static readonly CompressionInfo.Integer PerkListIndexCompressionInfo = new CompressionInfo.Integer(0, 2);

		public static readonly CompressionInfo.Integer PerkIndexCompressionInfo = new CompressionInfo.Integer(0, 4);

		public static readonly CompressionInfo.Float FlagDominationMoraleCompressionInfo = new CompressionInfo.Float(-1f, 8, 0.01f);

		public static readonly CompressionInfo.Integer TdmGoldChangeCompressionInfo = new CompressionInfo.Integer(0, 2000, true);

		public static readonly CompressionInfo.Integer TdmGoldGainTypeCompressionInfo = new CompressionInfo.Integer(0, 12);

		public static readonly CompressionInfo.Integer DuelAreaIndexCompressionInfo = new CompressionInfo.Integer(0, 4);

		public static readonly CompressionInfo.Integer AutomatedBattleIndexCompressionInfo = new CompressionInfo.Integer(0, 10, true);

		public static readonly CompressionInfo.Integer SiegeMoraleCompressionInfo = new CompressionInfo.Integer(0, 1440, true);

		public static readonly CompressionInfo.Integer SiegeMoralePerFlagCompressionInfo = new CompressionInfo.Integer(0, 90, true);

		public static CompressionInfo.Integer ActionSetCompressionInfo;

		public static CompressionInfo.Integer MonsterUsageSetCompressionInfo;

		public static readonly CompressionInfo.Integer OrderTypeCompressionInfo = new CompressionInfo.Integer(0, 42, true);

		public static readonly CompressionInfo.Integer FormationClassCompressionInfo = new CompressionInfo.Integer(-1, 10, true);

		public static readonly CompressionInfo.Float OrderPositionCompressionInfo = new CompressionInfo.Float(-100000f, 100000f, 24);

		public static readonly CompressionInfo.Integer SynchedMissionObjectReadableRecordTypeIndex = new CompressionInfo.Integer(-1, 8);
	}
}
