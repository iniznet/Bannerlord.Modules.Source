using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.Engine.Options
{
	// Token: 0x020000A2 RID: 162
	public class NativeOptions
	{
		// Token: 0x06000BCA RID: 3018 RVA: 0x0000D328 File Offset: 0x0000B528
		public static string GetGFXPresetName(NativeOptions.ConfigQuality presetIndex)
		{
			switch (presetIndex)
			{
			case NativeOptions.ConfigQuality.GFXVeryLow:
				return "1";
			case NativeOptions.ConfigQuality.GFXLow:
				return "2";
			case NativeOptions.ConfigQuality.GFXMedium:
				return "3";
			case NativeOptions.ConfigQuality.GFXHigh:
				return "4";
			case NativeOptions.ConfigQuality.GFXVeryHigh:
				return "5";
			case NativeOptions.ConfigQuality.GFXCustom:
				return "Custom";
			default:
				return "Unknown";
			}
		}

		// Token: 0x06000BCB RID: 3019 RVA: 0x0000D37E File Offset: 0x0000B57E
		public static bool IsGFXOptionChangeable(NativeOptions.ConfigQuality config)
		{
			return config < NativeOptions.ConfigQuality.GFXCustom;
		}

		// Token: 0x06000BCC RID: 3020 RVA: 0x0000D384 File Offset: 0x0000B584
		private static void CorrectSelection(List<NativeOptionData> audioOptions)
		{
			foreach (NativeOptionData nativeOptionData in audioOptions)
			{
				if (nativeOptionData.Type == NativeOptions.NativeOptionsType.SoundDevice)
				{
					int num = 0;
					for (int i = 0; i < NativeOptions.GetSoundDeviceCount(); i++)
					{
						if (NativeOptions.GetSoundDeviceName(i) != "")
						{
							num = i;
						}
					}
					if (nativeOptionData.GetValue(false) > (float)num)
					{
						NativeOptions.SetConfig(NativeOptions.NativeOptionsType.SoundDevice, 0f);
						nativeOptionData.SetValue(0f);
					}
				}
			}
		}

		// Token: 0x14000004 RID: 4
		// (add) Token: 0x06000BCD RID: 3021 RVA: 0x0000D41C File Offset: 0x0000B61C
		// (remove) Token: 0x06000BCE RID: 3022 RVA: 0x0000D450 File Offset: 0x0000B650
		public static event Action OnNativeOptionsApplied;

		// Token: 0x17000095 RID: 149
		// (get) Token: 0x06000BCF RID: 3023 RVA: 0x0000D484 File Offset: 0x0000B684
		public static List<NativeOptionData> VideoOptions
		{
			get
			{
				if (NativeOptions._videoOptions == null)
				{
					NativeOptions._videoOptions = new List<NativeOptionData>();
					for (NativeOptions.NativeOptionsType nativeOptionsType = NativeOptions.NativeOptionsType.None; nativeOptionsType < NativeOptions.NativeOptionsType.TotalOptions; nativeOptionsType++)
					{
						if (nativeOptionsType - NativeOptions.NativeOptionsType.DisplayMode <= 7 || nativeOptionsType == NativeOptions.NativeOptionsType.SharpenAmount)
						{
							NativeOptions._videoOptions.Add(new NativeNumericOptionData(nativeOptionsType));
						}
					}
				}
				return NativeOptions._videoOptions;
			}
		}

		// Token: 0x17000096 RID: 150
		// (get) Token: 0x06000BD0 RID: 3024 RVA: 0x0000D4D0 File Offset: 0x0000B6D0
		public static List<NativeOptionData> GraphicsOptions
		{
			get
			{
				if (NativeOptions._graphicsOptions == null)
				{
					NativeOptions._graphicsOptions = new List<NativeOptionData>();
					for (NativeOptions.NativeOptionsType nativeOptionsType = NativeOptions.NativeOptionsType.None; nativeOptionsType < NativeOptions.NativeOptionsType.TotalOptions; nativeOptionsType++)
					{
						switch (nativeOptionsType)
						{
						case NativeOptions.NativeOptionsType.MaxSimultaneousSoundEventCount:
						case NativeOptions.NativeOptionsType.OverAll:
						case NativeOptions.NativeOptionsType.ShaderQuality:
						case NativeOptions.NativeOptionsType.TextureBudget:
						case NativeOptions.NativeOptionsType.TextureQuality:
						case NativeOptions.NativeOptionsType.ShadowmapResolution:
						case NativeOptions.NativeOptionsType.ShadowmapType:
						case NativeOptions.NativeOptionsType.ShadowmapFiltering:
						case NativeOptions.NativeOptionsType.ParticleDetail:
						case NativeOptions.NativeOptionsType.ParticleQuality:
						case NativeOptions.NativeOptionsType.FoliageQuality:
						case NativeOptions.NativeOptionsType.CharacterDetail:
						case NativeOptions.NativeOptionsType.EnvironmentDetail:
						case NativeOptions.NativeOptionsType.TerrainQuality:
						case NativeOptions.NativeOptionsType.NumberOfRagDolls:
						case NativeOptions.NativeOptionsType.AnimationSamplingQuality:
						case NativeOptions.NativeOptionsType.Occlusion:
						case NativeOptions.NativeOptionsType.TextureFiltering:
						case NativeOptions.NativeOptionsType.WaterQuality:
						case NativeOptions.NativeOptionsType.Antialiasing:
						case NativeOptions.NativeOptionsType.LightingQuality:
						case NativeOptions.NativeOptionsType.DecalQuality:
							NativeOptions._graphicsOptions.Add(new NativeSelectionOptionData(nativeOptionsType));
							break;
						case NativeOptions.NativeOptionsType.DLSS:
							if (NativeOptions.GetIsDLSSAvailable())
							{
								NativeOptions._graphicsOptions.Add(new NativeSelectionOptionData(nativeOptionsType));
							}
							break;
						case NativeOptions.NativeOptionsType.DepthOfField:
						case NativeOptions.NativeOptionsType.SSR:
						case NativeOptions.NativeOptionsType.ClothSimulation:
						case NativeOptions.NativeOptionsType.InteractiveGrass:
						case NativeOptions.NativeOptionsType.SunShafts:
						case NativeOptions.NativeOptionsType.SSSSS:
						case NativeOptions.NativeOptionsType.Tesselation:
						case NativeOptions.NativeOptionsType.Bloom:
						case NativeOptions.NativeOptionsType.FilmGrain:
						case NativeOptions.NativeOptionsType.MotionBlur:
						case NativeOptions.NativeOptionsType.DynamicResolution:
							NativeOptions._graphicsOptions.Add(new NativeBooleanOptionData(nativeOptionsType));
							break;
						case NativeOptions.NativeOptionsType.PostFXLensFlare:
							if (EngineApplicationInterface.IConfig.CheckGFXSupportStatus(58))
							{
								NativeOptions._graphicsOptions.Add(new NativeBooleanOptionData(nativeOptionsType));
							}
							break;
						case NativeOptions.NativeOptionsType.PostFXStreaks:
							if (EngineApplicationInterface.IConfig.CheckGFXSupportStatus(59))
							{
								NativeOptions._graphicsOptions.Add(new NativeBooleanOptionData(nativeOptionsType));
							}
							break;
						case NativeOptions.NativeOptionsType.PostFXChromaticAberration:
							if (EngineApplicationInterface.IConfig.CheckGFXSupportStatus(60))
							{
								NativeOptions._graphicsOptions.Add(new NativeBooleanOptionData(nativeOptionsType));
							}
							break;
						case NativeOptions.NativeOptionsType.PostFXVignette:
							if (EngineApplicationInterface.IConfig.CheckGFXSupportStatus(61))
							{
								NativeOptions._graphicsOptions.Add(new NativeBooleanOptionData(nativeOptionsType));
							}
							break;
						case NativeOptions.NativeOptionsType.PostFXHexagonVignette:
							if (EngineApplicationInterface.IConfig.CheckGFXSupportStatus(62))
							{
								NativeOptions._graphicsOptions.Add(new NativeBooleanOptionData(nativeOptionsType));
							}
							break;
						case NativeOptions.NativeOptionsType.DynamicResolutionTarget:
							NativeOptions._graphicsOptions.Add(new NativeNumericOptionData(nativeOptionsType));
							break;
						}
					}
				}
				return NativeOptions._graphicsOptions;
			}
		}

		// Token: 0x06000BD1 RID: 3025 RVA: 0x0000D714 File Offset: 0x0000B914
		public static void ReadRGLConfigFiles()
		{
			EngineApplicationInterface.IConfig.ReadRGLConfigFiles();
		}

		// Token: 0x06000BD2 RID: 3026 RVA: 0x0000D720 File Offset: 0x0000B920
		public static float GetConfig(NativeOptions.NativeOptionsType type)
		{
			return EngineApplicationInterface.IConfig.GetRGLConfig((int)type);
		}

		// Token: 0x06000BD3 RID: 3027 RVA: 0x0000D72D File Offset: 0x0000B92D
		public static float GetDefaultConfig(NativeOptions.NativeOptionsType type)
		{
			return EngineApplicationInterface.IConfig.GetDefaultRGLConfig((int)type);
		}

		// Token: 0x06000BD4 RID: 3028 RVA: 0x0000D73A File Offset: 0x0000B93A
		public static float GetDefaultConfigForOverallSettings(NativeOptions.NativeOptionsType type, int config)
		{
			return EngineApplicationInterface.IConfig.GetRGLConfigForDefaultSettings((int)type, config);
		}

		// Token: 0x06000BD5 RID: 3029 RVA: 0x0000D748 File Offset: 0x0000B948
		public static int GetGameKeys(int keyType, int i)
		{
			Debug.FailedAssert("This is not implemented. Changed from Exception to not cause crash.", "C:\\Develop\\MB3\\Source\\Engine\\TaleWorlds.Engine\\Options\\NativeOptions\\NativeOptions.cs", "GetGameKeys", 322);
			return 0;
		}

		// Token: 0x06000BD6 RID: 3030 RVA: 0x0000D764 File Offset: 0x0000B964
		public static string GetSoundDeviceName(int i)
		{
			return EngineApplicationInterface.IConfig.GetSoundDeviceName(i);
		}

		// Token: 0x06000BD7 RID: 3031 RVA: 0x0000D771 File Offset: 0x0000B971
		public static string GetMonitorDeviceName(int i)
		{
			return EngineApplicationInterface.IConfig.GetMonitorDeviceName(i);
		}

		// Token: 0x06000BD8 RID: 3032 RVA: 0x0000D77E File Offset: 0x0000B97E
		public static string GetVideoDeviceName(int i)
		{
			return EngineApplicationInterface.IConfig.GetVideoDeviceName(i);
		}

		// Token: 0x06000BD9 RID: 3033 RVA: 0x0000D78B File Offset: 0x0000B98B
		public static int GetSoundDeviceCount()
		{
			return EngineApplicationInterface.IConfig.GetSoundDeviceCount();
		}

		// Token: 0x06000BDA RID: 3034 RVA: 0x0000D797 File Offset: 0x0000B997
		public static int GetMonitorDeviceCount()
		{
			return EngineApplicationInterface.IConfig.GetMonitorDeviceCount();
		}

		// Token: 0x06000BDB RID: 3035 RVA: 0x0000D7A3 File Offset: 0x0000B9A3
		public static int GetVideoDeviceCount()
		{
			return EngineApplicationInterface.IConfig.GetVideoDeviceCount();
		}

		// Token: 0x06000BDC RID: 3036 RVA: 0x0000D7AF File Offset: 0x0000B9AF
		public static int GetResolutionCount()
		{
			return EngineApplicationInterface.IConfig.GetResolutionCount();
		}

		// Token: 0x06000BDD RID: 3037 RVA: 0x0000D7BB File Offset: 0x0000B9BB
		public static void RefreshOptionsData()
		{
			EngineApplicationInterface.IConfig.RefreshOptionsData();
		}

		// Token: 0x06000BDE RID: 3038 RVA: 0x0000D7C7 File Offset: 0x0000B9C7
		public static int GetRefreshRateCount()
		{
			return EngineApplicationInterface.IConfig.GetRefreshRateCount();
		}

		// Token: 0x06000BDF RID: 3039 RVA: 0x0000D7D3 File Offset: 0x0000B9D3
		public static int GetRefreshRateAtIndex(int index)
		{
			return EngineApplicationInterface.IConfig.GetRefreshRateAtIndex(index);
		}

		// Token: 0x06000BE0 RID: 3040 RVA: 0x0000D7E0 File Offset: 0x0000B9E0
		public static void SetCustomResolution(int width, int height)
		{
			EngineApplicationInterface.IConfig.SetCustomResolution(width, height);
		}

		// Token: 0x06000BE1 RID: 3041 RVA: 0x0000D7EE File Offset: 0x0000B9EE
		public static void GetResolution(ref int width, ref int height)
		{
			EngineApplicationInterface.IConfig.GetDesktopResolution(ref width, ref height);
		}

		// Token: 0x06000BE2 RID: 3042 RVA: 0x0000D7FC File Offset: 0x0000B9FC
		public static void GetDesktopResolution(ref int width, ref int height)
		{
			EngineApplicationInterface.IConfig.GetDesktopResolution(ref width, ref height);
		}

		// Token: 0x06000BE3 RID: 3043 RVA: 0x0000D80A File Offset: 0x0000BA0A
		public static Vec2 GetResolutionAtIndex(int index)
		{
			return EngineApplicationInterface.IConfig.GetResolutionAtIndex(index);
		}

		// Token: 0x06000BE4 RID: 3044 RVA: 0x0000D817 File Offset: 0x0000BA17
		public static int GetDLSSTechnique()
		{
			return EngineApplicationInterface.IConfig.GetDlssTechnique();
		}

		// Token: 0x06000BE5 RID: 3045 RVA: 0x0000D823 File Offset: 0x0000BA23
		public static bool Is120HzAvailable()
		{
			return EngineApplicationInterface.IConfig.Is120HzAvailable();
		}

		// Token: 0x06000BE6 RID: 3046 RVA: 0x0000D82F File Offset: 0x0000BA2F
		public static int GetDLSSOptionCount()
		{
			return EngineApplicationInterface.IConfig.GetDlssOptionCount();
		}

		// Token: 0x06000BE7 RID: 3047 RVA: 0x0000D83B File Offset: 0x0000BA3B
		public static bool GetIsDLSSAvailable()
		{
			return EngineApplicationInterface.IConfig.IsDlssAvailable();
		}

		// Token: 0x06000BE8 RID: 3048 RVA: 0x0000D847 File Offset: 0x0000BA47
		public static bool CheckGFXSupportStatus(int enumType)
		{
			return EngineApplicationInterface.IConfig.CheckGFXSupportStatus(enumType);
		}

		// Token: 0x06000BE9 RID: 3049 RVA: 0x0000D854 File Offset: 0x0000BA54
		public static void SetConfig(NativeOptions.NativeOptionsType type, float value)
		{
			EngineApplicationInterface.IConfig.SetRGLConfig((int)type, value);
			if (NativeOptions.OnNativeOptionChanged != null)
			{
				NativeOptions.OnNativeOptionChanged(type);
			}
		}

		// Token: 0x06000BEA RID: 3050 RVA: 0x0000D874 File Offset: 0x0000BA74
		public static void ApplyConfigChanges(bool resizeWindow)
		{
			EngineApplicationInterface.IConfig.ApplyConfigChanges(resizeWindow);
		}

		// Token: 0x06000BEB RID: 3051 RVA: 0x0000D881 File Offset: 0x0000BA81
		public static void SetGameKeys(int keyType, int index, int key)
		{
			Debug.FailedAssert("This is not implemented. Changed from Exception to not cause crash.", "C:\\Develop\\MB3\\Source\\Engine\\TaleWorlds.Engine\\Options\\NativeOptions\\NativeOptions.cs", "SetGameKeys", 438);
		}

		// Token: 0x06000BEC RID: 3052 RVA: 0x0000D89C File Offset: 0x0000BA9C
		public static void Apply(int texture_budget, int sharpen_amount, int hdr, int dof_mode, int motion_blur, int ssr, int size, int texture_filtering, int trail_amount, int dynamic_resolution_target)
		{
			EngineApplicationInterface.IConfig.Apply(texture_budget, sharpen_amount, hdr, dof_mode, motion_blur, ssr, size, texture_filtering, trail_amount, dynamic_resolution_target);
			Action onNativeOptionsApplied = NativeOptions.OnNativeOptionsApplied;
			if (onNativeOptionsApplied == null)
			{
				return;
			}
			onNativeOptionsApplied();
		}

		// Token: 0x06000BED RID: 3053 RVA: 0x0000D8D2 File Offset: 0x0000BAD2
		public static SaveResult SaveConfig()
		{
			return (SaveResult)EngineApplicationInterface.IConfig.SaveRGLConfig();
		}

		// Token: 0x06000BEE RID: 3054 RVA: 0x0000D8DE File Offset: 0x0000BADE
		public static void SetBrightness(float gamma)
		{
			EngineApplicationInterface.IConfig.SetBrightness(gamma);
		}

		// Token: 0x06000BEF RID: 3055 RVA: 0x0000D8EB File Offset: 0x0000BAEB
		public static void SetDefaultGameKeys()
		{
			Debug.FailedAssert("This is not implemented. Changed from Exception to not cause crash.", "C:\\Develop\\MB3\\Source\\Engine\\TaleWorlds.Engine\\Options\\NativeOptions\\NativeOptions.cs", "SetDefaultGameKeys", 463);
		}

		// Token: 0x06000BF0 RID: 3056 RVA: 0x0000D906 File Offset: 0x0000BB06
		public static void SetDefaultGameConfig()
		{
			EngineApplicationInterface.IConfig.SetDefaultGameConfig();
		}

		// Token: 0x040001FB RID: 507
		public static NativeOptions.OnNativeOptionChangedDelegate OnNativeOptionChanged;

		// Token: 0x040001FD RID: 509
		private static List<NativeOptionData> _videoOptions;

		// Token: 0x040001FE RID: 510
		private static List<NativeOptionData> _graphicsOptions;

		// Token: 0x020000CD RID: 205
		public enum ConfigQuality
		{
			// Token: 0x04000461 RID: 1121
			GFXVeryLow,
			// Token: 0x04000462 RID: 1122
			GFXLow,
			// Token: 0x04000463 RID: 1123
			GFXMedium,
			// Token: 0x04000464 RID: 1124
			GFXHigh,
			// Token: 0x04000465 RID: 1125
			GFXVeryHigh,
			// Token: 0x04000466 RID: 1126
			GFXCustom
		}

		// Token: 0x020000CE RID: 206
		public enum NativeOptionsType
		{
			// Token: 0x04000468 RID: 1128
			None = -1,
			// Token: 0x04000469 RID: 1129
			MasterVolume,
			// Token: 0x0400046A RID: 1130
			SoundVolume,
			// Token: 0x0400046B RID: 1131
			MusicVolume,
			// Token: 0x0400046C RID: 1132
			VoiceChatVolume,
			// Token: 0x0400046D RID: 1133
			VoiceOverVolume,
			// Token: 0x0400046E RID: 1134
			SoundDevice,
			// Token: 0x0400046F RID: 1135
			MaxSimultaneousSoundEventCount,
			// Token: 0x04000470 RID: 1136
			SoundOutput,
			// Token: 0x04000471 RID: 1137
			SoundPreset,
			// Token: 0x04000472 RID: 1138
			KeepSoundInBackground,
			// Token: 0x04000473 RID: 1139
			SoundOcclusion,
			// Token: 0x04000474 RID: 1140
			MouseSensitivity,
			// Token: 0x04000475 RID: 1141
			InvertMouseYAxis,
			// Token: 0x04000476 RID: 1142
			MouseYMovementScale,
			// Token: 0x04000477 RID: 1143
			TrailAmount,
			// Token: 0x04000478 RID: 1144
			EnableVibration,
			// Token: 0x04000479 RID: 1145
			DisplayMode,
			// Token: 0x0400047A RID: 1146
			SelectedMonitor,
			// Token: 0x0400047B RID: 1147
			SelectedAdapter,
			// Token: 0x0400047C RID: 1148
			ScreenResolution,
			// Token: 0x0400047D RID: 1149
			RefreshRate,
			// Token: 0x0400047E RID: 1150
			ResolutionScale,
			// Token: 0x0400047F RID: 1151
			FrameLimiter,
			// Token: 0x04000480 RID: 1152
			VSync,
			// Token: 0x04000481 RID: 1153
			Brightness,
			// Token: 0x04000482 RID: 1154
			OverAll,
			// Token: 0x04000483 RID: 1155
			ShaderQuality,
			// Token: 0x04000484 RID: 1156
			TextureBudget,
			// Token: 0x04000485 RID: 1157
			TextureQuality,
			// Token: 0x04000486 RID: 1158
			ShadowmapResolution,
			// Token: 0x04000487 RID: 1159
			ShadowmapType,
			// Token: 0x04000488 RID: 1160
			ShadowmapFiltering,
			// Token: 0x04000489 RID: 1161
			ParticleDetail,
			// Token: 0x0400048A RID: 1162
			ParticleQuality,
			// Token: 0x0400048B RID: 1163
			FoliageQuality,
			// Token: 0x0400048C RID: 1164
			CharacterDetail,
			// Token: 0x0400048D RID: 1165
			EnvironmentDetail,
			// Token: 0x0400048E RID: 1166
			TerrainQuality,
			// Token: 0x0400048F RID: 1167
			NumberOfRagDolls,
			// Token: 0x04000490 RID: 1168
			AnimationSamplingQuality,
			// Token: 0x04000491 RID: 1169
			Occlusion,
			// Token: 0x04000492 RID: 1170
			TextureFiltering,
			// Token: 0x04000493 RID: 1171
			WaterQuality,
			// Token: 0x04000494 RID: 1172
			Antialiasing,
			// Token: 0x04000495 RID: 1173
			DLSS,
			// Token: 0x04000496 RID: 1174
			LightingQuality,
			// Token: 0x04000497 RID: 1175
			DecalQuality,
			// Token: 0x04000498 RID: 1176
			DepthOfField,
			// Token: 0x04000499 RID: 1177
			SSR,
			// Token: 0x0400049A RID: 1178
			ClothSimulation,
			// Token: 0x0400049B RID: 1179
			InteractiveGrass,
			// Token: 0x0400049C RID: 1180
			SunShafts,
			// Token: 0x0400049D RID: 1181
			SSSSS,
			// Token: 0x0400049E RID: 1182
			Tesselation,
			// Token: 0x0400049F RID: 1183
			Bloom,
			// Token: 0x040004A0 RID: 1184
			FilmGrain,
			// Token: 0x040004A1 RID: 1185
			MotionBlur,
			// Token: 0x040004A2 RID: 1186
			SharpenAmount,
			// Token: 0x040004A3 RID: 1187
			PostFXLensFlare,
			// Token: 0x040004A4 RID: 1188
			PostFXStreaks,
			// Token: 0x040004A5 RID: 1189
			PostFXChromaticAberration,
			// Token: 0x040004A6 RID: 1190
			PostFXVignette,
			// Token: 0x040004A7 RID: 1191
			PostFXHexagonVignette,
			// Token: 0x040004A8 RID: 1192
			BrightnessMin,
			// Token: 0x040004A9 RID: 1193
			BrightnessMax,
			// Token: 0x040004AA RID: 1194
			BrightnessCalibrated,
			// Token: 0x040004AB RID: 1195
			ExposureCompensation,
			// Token: 0x040004AC RID: 1196
			DynamicResolution,
			// Token: 0x040004AD RID: 1197
			DynamicResolutionTarget,
			// Token: 0x040004AE RID: 1198
			NumOfOptionTypes,
			// Token: 0x040004AF RID: 1199
			TotalOptions
		}

		// Token: 0x020000CF RID: 207
		// (Invoke) Token: 0x06000C7A RID: 3194
		public delegate void OnNativeOptionChangedDelegate(NativeOptions.NativeOptionsType changedNativeOptionsType);
	}
}
