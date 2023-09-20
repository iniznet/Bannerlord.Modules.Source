using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.Engine.Options
{
	public class NativeOptions
	{
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

		public static bool IsGFXOptionChangeable(NativeOptions.ConfigQuality config)
		{
			return config < NativeOptions.ConfigQuality.GFXCustom;
		}

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

		public static event Action OnNativeOptionsApplied;

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
							if (EngineApplicationInterface.IConfig.CheckGFXSupportStatus(62))
							{
								NativeOptions._graphicsOptions.Add(new NativeBooleanOptionData(nativeOptionsType));
							}
							break;
						case NativeOptions.NativeOptionsType.PostFXStreaks:
							if (EngineApplicationInterface.IConfig.CheckGFXSupportStatus(63))
							{
								NativeOptions._graphicsOptions.Add(new NativeBooleanOptionData(nativeOptionsType));
							}
							break;
						case NativeOptions.NativeOptionsType.PostFXChromaticAberration:
							if (EngineApplicationInterface.IConfig.CheckGFXSupportStatus(64))
							{
								NativeOptions._graphicsOptions.Add(new NativeBooleanOptionData(nativeOptionsType));
							}
							break;
						case NativeOptions.NativeOptionsType.PostFXVignette:
							if (EngineApplicationInterface.IConfig.CheckGFXSupportStatus(65))
							{
								NativeOptions._graphicsOptions.Add(new NativeBooleanOptionData(nativeOptionsType));
							}
							break;
						case NativeOptions.NativeOptionsType.PostFXHexagonVignette:
							if (EngineApplicationInterface.IConfig.CheckGFXSupportStatus(66))
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

		public static void ReadRGLConfigFiles()
		{
			EngineApplicationInterface.IConfig.ReadRGLConfigFiles();
		}

		public static float GetConfig(NativeOptions.NativeOptionsType type)
		{
			return EngineApplicationInterface.IConfig.GetRGLConfig((int)type);
		}

		public static float GetDefaultConfig(NativeOptions.NativeOptionsType type)
		{
			return EngineApplicationInterface.IConfig.GetDefaultRGLConfig((int)type);
		}

		public static float GetDefaultConfigForOverallSettings(NativeOptions.NativeOptionsType type, int config)
		{
			return EngineApplicationInterface.IConfig.GetRGLConfigForDefaultSettings((int)type, config);
		}

		public static int GetGameKeys(int keyType, int i)
		{
			Debug.FailedAssert("This is not implemented. Changed from Exception to not cause crash.", "C:\\Develop\\MB3\\Source\\Engine\\TaleWorlds.Engine\\Options\\NativeOptions\\NativeOptions.cs", "GetGameKeys", 326);
			return 0;
		}

		public static string GetSoundDeviceName(int i)
		{
			return EngineApplicationInterface.IConfig.GetSoundDeviceName(i);
		}

		public static string GetMonitorDeviceName(int i)
		{
			return EngineApplicationInterface.IConfig.GetMonitorDeviceName(i);
		}

		public static string GetVideoDeviceName(int i)
		{
			return EngineApplicationInterface.IConfig.GetVideoDeviceName(i);
		}

		public static int GetSoundDeviceCount()
		{
			return EngineApplicationInterface.IConfig.GetSoundDeviceCount();
		}

		public static int GetMonitorDeviceCount()
		{
			return EngineApplicationInterface.IConfig.GetMonitorDeviceCount();
		}

		public static int GetVideoDeviceCount()
		{
			return EngineApplicationInterface.IConfig.GetVideoDeviceCount();
		}

		public static int GetResolutionCount()
		{
			return EngineApplicationInterface.IConfig.GetResolutionCount();
		}

		public static void RefreshOptionsData()
		{
			EngineApplicationInterface.IConfig.RefreshOptionsData();
		}

		public static int GetRefreshRateCount()
		{
			return EngineApplicationInterface.IConfig.GetRefreshRateCount();
		}

		public static int GetRefreshRateAtIndex(int index)
		{
			return EngineApplicationInterface.IConfig.GetRefreshRateAtIndex(index);
		}

		public static void SetCustomResolution(int width, int height)
		{
			EngineApplicationInterface.IConfig.SetCustomResolution(width, height);
		}

		public static void GetResolution(ref int width, ref int height)
		{
			EngineApplicationInterface.IConfig.GetDesktopResolution(ref width, ref height);
		}

		public static void GetDesktopResolution(ref int width, ref int height)
		{
			EngineApplicationInterface.IConfig.GetDesktopResolution(ref width, ref height);
		}

		public static Vec2 GetResolutionAtIndex(int index)
		{
			return EngineApplicationInterface.IConfig.GetResolutionAtIndex(index);
		}

		public static int GetDLSSTechnique()
		{
			return EngineApplicationInterface.IConfig.GetDlssTechnique();
		}

		public static bool Is120HzAvailable()
		{
			return EngineApplicationInterface.IConfig.Is120HzAvailable();
		}

		public static int GetDLSSOptionCount()
		{
			return EngineApplicationInterface.IConfig.GetDlssOptionCount();
		}

		public static bool GetIsDLSSAvailable()
		{
			return EngineApplicationInterface.IConfig.IsDlssAvailable();
		}

		public static bool CheckGFXSupportStatus(int enumType)
		{
			return EngineApplicationInterface.IConfig.CheckGFXSupportStatus(enumType);
		}

		public static void SetConfig(NativeOptions.NativeOptionsType type, float value)
		{
			EngineApplicationInterface.IConfig.SetRGLConfig((int)type, value);
			NativeOptions.OnNativeOptionChangedDelegate onNativeOptionChanged = NativeOptions.OnNativeOptionChanged;
			if (onNativeOptionChanged == null)
			{
				return;
			}
			onNativeOptionChanged(type);
		}

		public static void ApplyConfigChanges(bool resizeWindow)
		{
			EngineApplicationInterface.IConfig.ApplyConfigChanges(resizeWindow);
		}

		public static void SetGameKeys(int keyType, int index, int key)
		{
			Debug.FailedAssert("This is not implemented. Changed from Exception to not cause crash.", "C:\\Develop\\MB3\\Source\\Engine\\TaleWorlds.Engine\\Options\\NativeOptions\\NativeOptions.cs", "SetGameKeys", 439);
		}

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

		public static SaveResult SaveConfig()
		{
			return (SaveResult)EngineApplicationInterface.IConfig.SaveRGLConfig();
		}

		public static void SetBrightness(float gamma)
		{
			EngineApplicationInterface.IConfig.SetBrightness(gamma);
		}

		public static void SetDefaultGameKeys()
		{
			Debug.FailedAssert("This is not implemented. Changed from Exception to not cause crash.", "C:\\Develop\\MB3\\Source\\Engine\\TaleWorlds.Engine\\Options\\NativeOptions\\NativeOptions.cs", "SetDefaultGameKeys", 464);
		}

		public static void SetDefaultGameConfig()
		{
			EngineApplicationInterface.IConfig.SetDefaultGameConfig();
		}

		public static NativeOptions.OnNativeOptionChangedDelegate OnNativeOptionChanged;

		private static List<NativeOptionData> _videoOptions;

		private static List<NativeOptionData> _graphicsOptions;

		public enum ConfigQuality
		{
			GFXVeryLow,
			GFXLow,
			GFXMedium,
			GFXHigh,
			GFXVeryHigh,
			GFXCustom
		}

		public enum NativeOptionsType
		{
			None = -1,
			MasterVolume,
			SoundVolume,
			MusicVolume,
			VoiceChatVolume,
			VoiceOverVolume,
			SoundDevice,
			MaxSimultaneousSoundEventCount,
			SoundOutput,
			SoundPreset,
			KeepSoundInBackground,
			SoundOcclusion,
			MouseSensitivity,
			InvertMouseYAxis,
			MouseYMovementScale,
			TrailAmount,
			EnableVibration,
			EnableGyroAssistedAim,
			GyroAimSensitivity,
			EnableTouchpadMouse,
			EnableAlternateAiming,
			DisplayMode,
			SelectedMonitor,
			SelectedAdapter,
			ScreenResolution,
			RefreshRate,
			ResolutionScale,
			FrameLimiter,
			VSync,
			Brightness,
			OverAll,
			ShaderQuality,
			TextureBudget,
			TextureQuality,
			ShadowmapResolution,
			ShadowmapType,
			ShadowmapFiltering,
			ParticleDetail,
			ParticleQuality,
			FoliageQuality,
			CharacterDetail,
			EnvironmentDetail,
			TerrainQuality,
			NumberOfRagDolls,
			AnimationSamplingQuality,
			Occlusion,
			TextureFiltering,
			WaterQuality,
			Antialiasing,
			DLSS,
			LightingQuality,
			DecalQuality,
			DepthOfField,
			SSR,
			ClothSimulation,
			InteractiveGrass,
			SunShafts,
			SSSSS,
			Tesselation,
			Bloom,
			FilmGrain,
			MotionBlur,
			SharpenAmount,
			PostFXLensFlare,
			PostFXStreaks,
			PostFXChromaticAberration,
			PostFXVignette,
			PostFXHexagonVignette,
			BrightnessMin,
			BrightnessMax,
			BrightnessCalibrated,
			ExposureCompensation,
			DynamicResolution,
			DynamicResolutionTarget,
			NumOfOptionTypes,
			TotalOptions
		}

		public delegate void OnNativeOptionChangedDelegate(NativeOptions.NativeOptionsType changedNativeOptionsType);
	}
}
