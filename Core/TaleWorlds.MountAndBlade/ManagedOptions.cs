using System;
using System.Collections.Generic;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000389 RID: 905
	public static class ManagedOptions
	{
		// Token: 0x060031B1 RID: 12721 RVA: 0x000CE404 File Offset: 0x000CC604
		public static float GetConfig(ManagedOptions.ManagedOptionsType type)
		{
			switch (type)
			{
			case ManagedOptions.ManagedOptionsType.Language:
				return (float)LocalizedTextManager.GetLanguageIds(NativeConfig.IsDevelopmentMode).IndexOf(BannerlordConfig.Language);
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
				Debug.FailedAssert("ManagedOptionsType not found", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Options\\ManagedOptions\\ManagedOptions.cs", "GetConfig", 162);
				return 0f;
			}
		}

		// Token: 0x060031B2 RID: 12722 RVA: 0x000CE6EC File Offset: 0x000CC8EC
		public static float GetDefaultConfig(ManagedOptions.ManagedOptionsType type)
		{
			switch (type)
			{
			case ManagedOptions.ManagedOptionsType.Language:
				return (float)LocalizedTextManager.GetLanguageIds(NativeConfig.IsDevelopmentMode).IndexOf(BannerlordConfig.DefaultLanguage);
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
				Debug.FailedAssert("ManagedOptionsType not found", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Options\\ManagedOptions\\ManagedOptions.cs", "GetDefaultConfig", 263);
				return 0f;
			}
		}

		// Token: 0x060031B3 RID: 12723 RVA: 0x000CE909 File Offset: 0x000CCB09
		[MBCallback]
		internal static int GetConfigCount()
		{
			return 46;
		}

		// Token: 0x060031B4 RID: 12724 RVA: 0x000CE90D File Offset: 0x000CCB0D
		[MBCallback]
		internal static float GetConfigValue(int type)
		{
			return ManagedOptions.GetConfig((ManagedOptions.ManagedOptionsType)type);
		}

		// Token: 0x060031B5 RID: 12725 RVA: 0x000CE918 File Offset: 0x000CCB18
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
					Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Options\\ManagedOptions\\ManagedOptions.cs", "SetConfig", 417);
					BannerlordConfig.Language = list[0];
				}
				break;
			}
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
					Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Options\\ManagedOptions\\ManagedOptions.cs", "SetConfig", 435);
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

		// Token: 0x060031B6 RID: 12726 RVA: 0x000CED9D File Offset: 0x000CCF9D
		public static SaveResult SaveConfig()
		{
			return BannerlordConfig.Save();
		}

		// Token: 0x040014E1 RID: 5345
		public static ManagedOptions.OnManagedOptionChangedDelegate OnManagedOptionChanged;

		// Token: 0x02000694 RID: 1684
		public enum ManagedOptionsType
		{
			// Token: 0x04002167 RID: 8551
			Language,
			// Token: 0x04002168 RID: 8552
			ControlBlockDirection,
			// Token: 0x04002169 RID: 8553
			ControlAttackDirection,
			// Token: 0x0400216A RID: 8554
			NumberOfCorpses,
			// Token: 0x0400216B RID: 8555
			BattleSize,
			// Token: 0x0400216C RID: 8556
			ReinforcementWaveCount,
			// Token: 0x0400216D RID: 8557
			TurnCameraWithHorseInFirstPerson,
			// Token: 0x0400216E RID: 8558
			ShowBlood,
			// Token: 0x0400216F RID: 8559
			ShowAttackDirection,
			// Token: 0x04002170 RID: 8560
			ShowTargetingReticle,
			// Token: 0x04002171 RID: 8561
			AutoSaveInterval,
			// Token: 0x04002172 RID: 8562
			FriendlyTroopsBannerOpacity,
			// Token: 0x04002173 RID: 8563
			ReportDamage,
			// Token: 0x04002174 RID: 8564
			ReportBark,
			// Token: 0x04002175 RID: 8565
			LockTarget,
			// Token: 0x04002176 RID: 8566
			EnableTutorialHints,
			// Token: 0x04002177 RID: 8567
			ReportCasualtiesType,
			// Token: 0x04002178 RID: 8568
			ReportExperience,
			// Token: 0x04002179 RID: 8569
			ReportPersonalDamage,
			// Token: 0x0400217A RID: 8570
			FirstPersonFov,
			// Token: 0x0400217B RID: 8571
			CombatCameraDistance,
			// Token: 0x0400217C RID: 8572
			EnableDamageTakenVisuals,
			// Token: 0x0400217D RID: 8573
			EnableVoiceChat,
			// Token: 0x0400217E RID: 8574
			EnableDeathIcon,
			// Token: 0x0400217F RID: 8575
			EnableNetworkAlertIcons,
			// Token: 0x04002180 RID: 8576
			ForceVSyncInMenus,
			// Token: 0x04002181 RID: 8577
			EnableVerticalAimCorrection,
			// Token: 0x04002182 RID: 8578
			UIScale,
			// Token: 0x04002183 RID: 8579
			CrosshairType,
			// Token: 0x04002184 RID: 8580
			EnableGenericAvatars,
			// Token: 0x04002185 RID: 8581
			EnableGenericNames,
			// Token: 0x04002186 RID: 8582
			OrderType,
			// Token: 0x04002187 RID: 8583
			OrderLayoutType,
			// Token: 0x04002188 RID: 8584
			AutoTrackAttackedSettlements,
			// Token: 0x04002189 RID: 8585
			StopGameOnFocusLost,
			// Token: 0x0400218A RID: 8586
			SlowDownOnOrder,
			// Token: 0x0400218B RID: 8587
			HideFullServers,
			// Token: 0x0400218C RID: 8588
			HideEmptyServers,
			// Token: 0x0400218D RID: 8589
			HidePasswordProtectedServers,
			// Token: 0x0400218E RID: 8590
			HideUnofficialServers,
			// Token: 0x0400218F RID: 8591
			HideModuleIncompatibleServers,
			// Token: 0x04002190 RID: 8592
			HideBattleUI,
			// Token: 0x04002191 RID: 8593
			UnitSpawnPrioritization,
			// Token: 0x04002192 RID: 8594
			EnableSingleplayerChatBox,
			// Token: 0x04002193 RID: 8595
			EnableMultiplayerChatBox,
			// Token: 0x04002194 RID: 8596
			VoiceLanguage,
			// Token: 0x04002195 RID: 8597
			ManagedOptionTypeCount
		}

		// Token: 0x02000695 RID: 1685
		// (Invoke) Token: 0x06003EDB RID: 16091
		public delegate void OnManagedOptionChangedDelegate(ManagedOptions.ManagedOptionsType changedManagedOptionsType);
	}
}
