using System;
using System.Collections.Generic;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
	public static class ManagedOptions
	{
		public static float GetConfig(ManagedOptions.ManagedOptionsType type)
		{
			switch (type)
			{
			case ManagedOptions.ManagedOptionsType.Language:
				return (float)LocalizedTextManager.GetLanguageIds(NativeConfig.IsDevelopmentMode).IndexOf(BannerlordConfig.Language);
			case ManagedOptions.ManagedOptionsType.GyroOverrideForAttackDefend:
				return (float)(BannerlordConfig.GyroOverrideForAttackDefend ? 1 : 0);
			case ManagedOptions.ManagedOptionsType.ControlBlockDirection:
				return (float)BannerlordConfig.DefendDirectionControl;
			case ManagedOptions.ManagedOptionsType.ControlAttackDirection:
				return (float)BannerlordConfig.AttackDirectionControl;
			case ManagedOptions.ManagedOptionsType.NumberOfCorpses:
				return (float)BannerlordConfig.NumberOfCorpses;
			case ManagedOptions.ManagedOptionsType.BattleSize:
				return (float)BannerlordConfig.BattleSize;
			case ManagedOptions.ManagedOptionsType.ReinforcementWaveCount:
				return (float)BannerlordConfig.ReinforcementWaveCount;
			case ManagedOptions.ManagedOptionsType.TurnCameraWithHorseInFirstPerson:
				return (float)BannerlordConfig.TurnCameraWithHorseInFirstPerson;
			case ManagedOptions.ManagedOptionsType.ShowBlood:
				return (float)(BannerlordConfig.ShowBlood ? 1 : 0);
			case ManagedOptions.ManagedOptionsType.ShowAttackDirection:
				return (float)(BannerlordConfig.DisplayAttackDirection ? 1 : 0);
			case ManagedOptions.ManagedOptionsType.ShowTargetingReticle:
				return (float)(BannerlordConfig.DisplayTargetingReticule ? 1 : 0);
			case ManagedOptions.ManagedOptionsType.AutoSaveInterval:
				return (float)BannerlordConfig.AutoSaveInterval;
			case ManagedOptions.ManagedOptionsType.FriendlyTroopsBannerOpacity:
				return BannerlordConfig.FriendlyTroopsBannerOpacity;
			case ManagedOptions.ManagedOptionsType.ReportDamage:
				return (float)(BannerlordConfig.ReportDamage ? 1 : 0);
			case ManagedOptions.ManagedOptionsType.ReportBark:
				return (float)(BannerlordConfig.ReportBark ? 1 : 0);
			case ManagedOptions.ManagedOptionsType.LockTarget:
				return (float)(BannerlordConfig.LockTarget ? 1 : 0);
			case ManagedOptions.ManagedOptionsType.EnableTutorialHints:
				return (float)(BannerlordConfig.EnableTutorialHints ? 1 : 0);
			case ManagedOptions.ManagedOptionsType.ReportCasualtiesType:
				return (float)BannerlordConfig.ReportCasualtiesType;
			case ManagedOptions.ManagedOptionsType.ReportExperience:
				return (float)(BannerlordConfig.ReportExperience ? 1 : 0);
			case ManagedOptions.ManagedOptionsType.ReportPersonalDamage:
				return (float)(BannerlordConfig.ReportPersonalDamage ? 1 : 0);
			case ManagedOptions.ManagedOptionsType.FirstPersonFov:
				return BannerlordConfig.FirstPersonFov;
			case ManagedOptions.ManagedOptionsType.CombatCameraDistance:
				return BannerlordConfig.CombatCameraDistance;
			case ManagedOptions.ManagedOptionsType.EnableDamageTakenVisuals:
				return (float)(BannerlordConfig.EnableDamageTakenVisuals ? 1 : 0);
			case ManagedOptions.ManagedOptionsType.EnableVoiceChat:
				return (float)(BannerlordConfig.EnableVoiceChat ? 1 : 0);
			case ManagedOptions.ManagedOptionsType.EnableDeathIcon:
				return (float)(BannerlordConfig.EnableDeathIcon ? 1 : 0);
			case ManagedOptions.ManagedOptionsType.EnableNetworkAlertIcons:
				return (float)(BannerlordConfig.EnableNetworkAlertIcons ? 1 : 0);
			case ManagedOptions.ManagedOptionsType.ForceVSyncInMenus:
				return (float)(BannerlordConfig.ForceVSyncInMenus ? 1 : 0);
			case ManagedOptions.ManagedOptionsType.EnableVerticalAimCorrection:
				return (float)(BannerlordConfig.EnableVerticalAimCorrection ? 1 : 0);
			case ManagedOptions.ManagedOptionsType.ZoomSensitivityModifier:
				return BannerlordConfig.ZoomSensitivityModifier;
			case ManagedOptions.ManagedOptionsType.UIScale:
				return BannerlordConfig.UIScale;
			case ManagedOptions.ManagedOptionsType.CrosshairType:
				return (float)BannerlordConfig.CrosshairType;
			case ManagedOptions.ManagedOptionsType.EnableGenericAvatars:
				return (float)(BannerlordConfig.EnableGenericAvatars ? 1 : 0);
			case ManagedOptions.ManagedOptionsType.EnableGenericNames:
				return (float)(BannerlordConfig.EnableGenericNames ? 1 : 0);
			case ManagedOptions.ManagedOptionsType.OrderType:
				return (float)BannerlordConfig.OrderType;
			case ManagedOptions.ManagedOptionsType.OrderLayoutType:
				return (float)BannerlordConfig.OrderLayoutType;
			case ManagedOptions.ManagedOptionsType.AutoTrackAttackedSettlements:
				return (float)BannerlordConfig.AutoTrackAttackedSettlements;
			case ManagedOptions.ManagedOptionsType.StopGameOnFocusLost:
				return (float)(BannerlordConfig.StopGameOnFocusLost ? 1 : 0);
			case ManagedOptions.ManagedOptionsType.SlowDownOnOrder:
				return (float)(BannerlordConfig.SlowDownOnOrder ? 1 : 0);
			case ManagedOptions.ManagedOptionsType.HideFullServers:
				return (float)(BannerlordConfig.HideFullServers ? 1 : 0);
			case ManagedOptions.ManagedOptionsType.HideEmptyServers:
				return (float)(BannerlordConfig.HideEmptyServers ? 1 : 0);
			case ManagedOptions.ManagedOptionsType.HidePasswordProtectedServers:
				return (float)(BannerlordConfig.HidePasswordProtectedServers ? 1 : 0);
			case ManagedOptions.ManagedOptionsType.HideUnofficialServers:
				return (float)(BannerlordConfig.HideUnofficialServers ? 1 : 0);
			case ManagedOptions.ManagedOptionsType.HideModuleIncompatibleServers:
				return (float)(BannerlordConfig.HideModuleIncompatibleServers ? 1 : 0);
			case ManagedOptions.ManagedOptionsType.HideBattleUI:
				return (float)(BannerlordConfig.HideBattleUI ? 1 : 0);
			case ManagedOptions.ManagedOptionsType.UnitSpawnPrioritization:
				return (float)BannerlordConfig.UnitSpawnPrioritization;
			case ManagedOptions.ManagedOptionsType.EnableSingleplayerChatBox:
				return (float)(BannerlordConfig.EnableSingleplayerChatBox ? 1 : 0);
			case ManagedOptions.ManagedOptionsType.EnableMultiplayerChatBox:
				return (float)(BannerlordConfig.EnableMultiplayerChatBox ? 1 : 0);
			case ManagedOptions.ManagedOptionsType.VoiceLanguage:
				return (float)LocalizedVoiceManager.GetVoiceLanguageIds().IndexOf(BannerlordConfig.VoiceLanguage);
			default:
				Debug.FailedAssert("ManagedOptionsType not found", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Options\\ManagedOptions\\ManagedOptions.cs", "GetConfig", 168);
				return 0f;
			}
		}

		public static float GetDefaultConfig(ManagedOptions.ManagedOptionsType type)
		{
			switch (type)
			{
			case ManagedOptions.ManagedOptionsType.Language:
				return (float)LocalizedTextManager.GetLanguageIds(NativeConfig.IsDevelopmentMode).IndexOf(BannerlordConfig.DefaultLanguage);
			case ManagedOptions.ManagedOptionsType.GyroOverrideForAttackDefend:
				return 0f;
			case ManagedOptions.ManagedOptionsType.ControlBlockDirection:
				return 0f;
			case ManagedOptions.ManagedOptionsType.ControlAttackDirection:
				return 1f;
			case ManagedOptions.ManagedOptionsType.NumberOfCorpses:
				return 3f;
			case ManagedOptions.ManagedOptionsType.BattleSize:
				return 2f;
			case ManagedOptions.ManagedOptionsType.ReinforcementWaveCount:
				return 3f;
			case ManagedOptions.ManagedOptionsType.TurnCameraWithHorseInFirstPerson:
				return 2f;
			case ManagedOptions.ManagedOptionsType.ShowBlood:
				return 1f;
			case ManagedOptions.ManagedOptionsType.ShowAttackDirection:
				return 1f;
			case ManagedOptions.ManagedOptionsType.ShowTargetingReticle:
				return 1f;
			case ManagedOptions.ManagedOptionsType.AutoSaveInterval:
				return 30f;
			case ManagedOptions.ManagedOptionsType.FriendlyTroopsBannerOpacity:
				return 1f;
			case ManagedOptions.ManagedOptionsType.ReportDamage:
				return 1f;
			case ManagedOptions.ManagedOptionsType.ReportBark:
				return 1f;
			case ManagedOptions.ManagedOptionsType.LockTarget:
				return 0f;
			case ManagedOptions.ManagedOptionsType.EnableTutorialHints:
				return 1f;
			case ManagedOptions.ManagedOptionsType.ReportCasualtiesType:
				return 0f;
			case ManagedOptions.ManagedOptionsType.ReportExperience:
				return 1f;
			case ManagedOptions.ManagedOptionsType.ReportPersonalDamage:
				return 1f;
			case ManagedOptions.ManagedOptionsType.FirstPersonFov:
				return 65f;
			case ManagedOptions.ManagedOptionsType.CombatCameraDistance:
				return 1f;
			case ManagedOptions.ManagedOptionsType.EnableDamageTakenVisuals:
				return 1f;
			case ManagedOptions.ManagedOptionsType.EnableVoiceChat:
				return 1f;
			case ManagedOptions.ManagedOptionsType.EnableDeathIcon:
				return 1f;
			case ManagedOptions.ManagedOptionsType.EnableNetworkAlertIcons:
				return 1f;
			case ManagedOptions.ManagedOptionsType.ForceVSyncInMenus:
				return 1f;
			case ManagedOptions.ManagedOptionsType.EnableVerticalAimCorrection:
				return 1f;
			case ManagedOptions.ManagedOptionsType.ZoomSensitivityModifier:
				return 0.66666f;
			case ManagedOptions.ManagedOptionsType.UIScale:
				return 1f;
			case ManagedOptions.ManagedOptionsType.CrosshairType:
				return 0f;
			case ManagedOptions.ManagedOptionsType.EnableGenericAvatars:
				return 0f;
			case ManagedOptions.ManagedOptionsType.EnableGenericNames:
				return 0f;
			case ManagedOptions.ManagedOptionsType.OrderType:
				return 0f;
			case ManagedOptions.ManagedOptionsType.OrderLayoutType:
				return 0f;
			case ManagedOptions.ManagedOptionsType.AutoTrackAttackedSettlements:
				return 0f;
			case ManagedOptions.ManagedOptionsType.StopGameOnFocusLost:
				return 1f;
			case ManagedOptions.ManagedOptionsType.SlowDownOnOrder:
				return 1f;
			case ManagedOptions.ManagedOptionsType.HideFullServers:
				return 0f;
			case ManagedOptions.ManagedOptionsType.HideEmptyServers:
				return 0f;
			case ManagedOptions.ManagedOptionsType.HidePasswordProtectedServers:
				return 0f;
			case ManagedOptions.ManagedOptionsType.HideUnofficialServers:
				return 0f;
			case ManagedOptions.ManagedOptionsType.HideModuleIncompatibleServers:
				return 0f;
			case ManagedOptions.ManagedOptionsType.HideBattleUI:
				return 0f;
			case ManagedOptions.ManagedOptionsType.UnitSpawnPrioritization:
				return 0f;
			case ManagedOptions.ManagedOptionsType.EnableSingleplayerChatBox:
				return 1f;
			case ManagedOptions.ManagedOptionsType.EnableMultiplayerChatBox:
				return 1f;
			case ManagedOptions.ManagedOptionsType.VoiceLanguage:
				return (float)LocalizedVoiceManager.GetVoiceLanguageIds().IndexOf(BannerlordConfig.VoiceLanguage);
			default:
				Debug.FailedAssert("ManagedOptionsType not found", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Options\\ManagedOptions\\ManagedOptions.cs", "GetDefaultConfig", 273);
				return 0f;
			}
		}

		[MBCallback]
		internal static int GetConfigCount()
		{
			return 48;
		}

		[MBCallback]
		internal static float GetConfigValue(int type)
		{
			return ManagedOptions.GetConfig((ManagedOptions.ManagedOptionsType)type);
		}

		public static void SetConfig(ManagedOptions.ManagedOptionsType type, float value)
		{
			switch (type)
			{
			case ManagedOptions.ManagedOptionsType.Language:
			{
				List<string> list = LocalizedTextManager.GetLanguageIds(NativeConfig.IsDevelopmentMode);
				if (value >= 0f && value < (float)list.Count)
				{
					BannerlordConfig.Language = list[(int)value];
				}
				else
				{
					Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Options\\ManagedOptions\\ManagedOptions.cs", "SetConfig", 433);
					BannerlordConfig.Language = list[0];
				}
				break;
			}
			case ManagedOptions.ManagedOptionsType.GyroOverrideForAttackDefend:
				BannerlordConfig.GyroOverrideForAttackDefend = value != 0f;
				break;
			case ManagedOptions.ManagedOptionsType.ControlBlockDirection:
				BannerlordConfig.DefendDirectionControl = (int)value;
				break;
			case ManagedOptions.ManagedOptionsType.ControlAttackDirection:
				BannerlordConfig.AttackDirectionControl = (int)value;
				break;
			case ManagedOptions.ManagedOptionsType.NumberOfCorpses:
				BannerlordConfig.NumberOfCorpses = (int)value;
				break;
			case ManagedOptions.ManagedOptionsType.BattleSize:
				BannerlordConfig.BattleSize = (int)value;
				break;
			case ManagedOptions.ManagedOptionsType.ReinforcementWaveCount:
				BannerlordConfig.ReinforcementWaveCount = (int)value;
				break;
			case ManagedOptions.ManagedOptionsType.TurnCameraWithHorseInFirstPerson:
				BannerlordConfig.TurnCameraWithHorseInFirstPerson = (int)value;
				break;
			case ManagedOptions.ManagedOptionsType.ShowBlood:
				BannerlordConfig.ShowBlood = (double)value != 0.0;
				break;
			case ManagedOptions.ManagedOptionsType.ShowAttackDirection:
				BannerlordConfig.DisplayAttackDirection = (double)value != 0.0;
				break;
			case ManagedOptions.ManagedOptionsType.ShowTargetingReticle:
				BannerlordConfig.DisplayTargetingReticule = value != 0f;
				break;
			case ManagedOptions.ManagedOptionsType.AutoSaveInterval:
				BannerlordConfig.AutoSaveInterval = (int)value;
				break;
			case ManagedOptions.ManagedOptionsType.FriendlyTroopsBannerOpacity:
				BannerlordConfig.FriendlyTroopsBannerOpacity = value;
				break;
			case ManagedOptions.ManagedOptionsType.ReportDamage:
				BannerlordConfig.ReportDamage = value != 0f;
				break;
			case ManagedOptions.ManagedOptionsType.ReportBark:
				BannerlordConfig.ReportBark = value != 0f;
				break;
			case ManagedOptions.ManagedOptionsType.LockTarget:
				BannerlordConfig.LockTarget = value != 0f;
				break;
			case ManagedOptions.ManagedOptionsType.EnableTutorialHints:
				BannerlordConfig.EnableTutorialHints = value != 0f;
				break;
			case ManagedOptions.ManagedOptionsType.ReportCasualtiesType:
				BannerlordConfig.ReportCasualtiesType = (int)value;
				break;
			case ManagedOptions.ManagedOptionsType.ReportExperience:
				BannerlordConfig.ReportExperience = value != 0f;
				break;
			case ManagedOptions.ManagedOptionsType.ReportPersonalDamage:
				BannerlordConfig.ReportPersonalDamage = value != 0f;
				break;
			case ManagedOptions.ManagedOptionsType.FirstPersonFov:
				BannerlordConfig.FirstPersonFov = value;
				break;
			case ManagedOptions.ManagedOptionsType.CombatCameraDistance:
				BannerlordConfig.CombatCameraDistance = value;
				break;
			case ManagedOptions.ManagedOptionsType.EnableDamageTakenVisuals:
				BannerlordConfig.EnableDamageTakenVisuals = value != 0f;
				break;
			case ManagedOptions.ManagedOptionsType.EnableVoiceChat:
				BannerlordConfig.EnableVoiceChat = value != 0f;
				break;
			case ManagedOptions.ManagedOptionsType.EnableDeathIcon:
				BannerlordConfig.EnableDeathIcon = value != 0f;
				break;
			case ManagedOptions.ManagedOptionsType.EnableNetworkAlertIcons:
				BannerlordConfig.EnableNetworkAlertIcons = value != 0f;
				break;
			case ManagedOptions.ManagedOptionsType.ForceVSyncInMenus:
				BannerlordConfig.ForceVSyncInMenus = value != 0f;
				break;
			case ManagedOptions.ManagedOptionsType.EnableVerticalAimCorrection:
				BannerlordConfig.EnableVerticalAimCorrection = value != 0f;
				break;
			case ManagedOptions.ManagedOptionsType.ZoomSensitivityModifier:
				BannerlordConfig.ZoomSensitivityModifier = value;
				break;
			case ManagedOptions.ManagedOptionsType.UIScale:
				BannerlordConfig.UIScale = value;
				break;
			case ManagedOptions.ManagedOptionsType.CrosshairType:
				BannerlordConfig.CrosshairType = (int)value;
				break;
			case ManagedOptions.ManagedOptionsType.EnableGenericAvatars:
				BannerlordConfig.EnableGenericAvatars = value != 0f;
				break;
			case ManagedOptions.ManagedOptionsType.EnableGenericNames:
				BannerlordConfig.EnableGenericNames = value != 0f;
				break;
			case ManagedOptions.ManagedOptionsType.OrderType:
				BannerlordConfig.OrderType = (int)value;
				break;
			case ManagedOptions.ManagedOptionsType.OrderLayoutType:
				BannerlordConfig.OrderLayoutType = (int)value;
				break;
			case ManagedOptions.ManagedOptionsType.AutoTrackAttackedSettlements:
				BannerlordConfig.AutoTrackAttackedSettlements = (int)value;
				break;
			case ManagedOptions.ManagedOptionsType.StopGameOnFocusLost:
				BannerlordConfig.StopGameOnFocusLost = value != 0f;
				break;
			case ManagedOptions.ManagedOptionsType.SlowDownOnOrder:
				BannerlordConfig.SlowDownOnOrder = value != 0f;
				break;
			case ManagedOptions.ManagedOptionsType.HideFullServers:
				BannerlordConfig.HideFullServers = value != 0f;
				break;
			case ManagedOptions.ManagedOptionsType.HideEmptyServers:
				BannerlordConfig.HideEmptyServers = value != 0f;
				break;
			case ManagedOptions.ManagedOptionsType.HidePasswordProtectedServers:
				BannerlordConfig.HidePasswordProtectedServers = value != 0f;
				break;
			case ManagedOptions.ManagedOptionsType.HideUnofficialServers:
				BannerlordConfig.HideUnofficialServers = value != 0f;
				break;
			case ManagedOptions.ManagedOptionsType.HideModuleIncompatibleServers:
				BannerlordConfig.HideModuleIncompatibleServers = value != 0f;
				break;
			case ManagedOptions.ManagedOptionsType.HideBattleUI:
				BannerlordConfig.HideBattleUI = value != 0f;
				break;
			case ManagedOptions.ManagedOptionsType.UnitSpawnPrioritization:
				BannerlordConfig.UnitSpawnPrioritization = (int)value;
				break;
			case ManagedOptions.ManagedOptionsType.EnableSingleplayerChatBox:
				BannerlordConfig.EnableSingleplayerChatBox = value != 0f;
				break;
			case ManagedOptions.ManagedOptionsType.EnableMultiplayerChatBox:
				BannerlordConfig.EnableMultiplayerChatBox = value != 0f;
				break;
			case ManagedOptions.ManagedOptionsType.VoiceLanguage:
			{
				List<string> list = LocalizedVoiceManager.GetVoiceLanguageIds();
				if (value >= 0f && value < (float)list.Count)
				{
					BannerlordConfig.VoiceLanguage = list[(int)value];
				}
				else
				{
					Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Options\\ManagedOptions\\ManagedOptions.cs", "SetConfig", 451);
					BannerlordConfig.VoiceLanguage = list[0];
				}
				break;
			}
			}
			ManagedOptions.OnManagedOptionChangedDelegate onManagedOptionChanged = ManagedOptions.OnManagedOptionChanged;
			if (onManagedOptionChanged == null)
			{
				return;
			}
			onManagedOptionChanged(type);
		}

		public static SaveResult SaveConfig()
		{
			return BannerlordConfig.Save();
		}

		public static ManagedOptions.OnManagedOptionChangedDelegate OnManagedOptionChanged;

		public enum ManagedOptionsType
		{
			Language,
			GyroOverrideForAttackDefend,
			ControlBlockDirection,
			ControlAttackDirection,
			NumberOfCorpses,
			BattleSize,
			ReinforcementWaveCount,
			TurnCameraWithHorseInFirstPerson,
			ShowBlood,
			ShowAttackDirection,
			ShowTargetingReticle,
			AutoSaveInterval,
			FriendlyTroopsBannerOpacity,
			ReportDamage,
			ReportBark,
			LockTarget,
			EnableTutorialHints,
			ReportCasualtiesType,
			ReportExperience,
			ReportPersonalDamage,
			FirstPersonFov,
			CombatCameraDistance,
			EnableDamageTakenVisuals,
			EnableVoiceChat,
			EnableDeathIcon,
			EnableNetworkAlertIcons,
			ForceVSyncInMenus,
			EnableVerticalAimCorrection,
			ZoomSensitivityModifier,
			UIScale,
			CrosshairType,
			EnableGenericAvatars,
			EnableGenericNames,
			OrderType,
			OrderLayoutType,
			AutoTrackAttackedSettlements,
			StopGameOnFocusLost,
			SlowDownOnOrder,
			HideFullServers,
			HideEmptyServers,
			HidePasswordProtectedServers,
			HideUnofficialServers,
			HideModuleIncompatibleServers,
			HideBattleUI,
			UnitSpawnPrioritization,
			EnableSingleplayerChatBox,
			EnableMultiplayerChatBox,
			VoiceLanguage,
			ManagedOptionTypeCount
		}

		public delegate void OnManagedOptionChangedDelegate(ManagedOptions.ManagedOptionsType changedManagedOptionsType);
	}
}
