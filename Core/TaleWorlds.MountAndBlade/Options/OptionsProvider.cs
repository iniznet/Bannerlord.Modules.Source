using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Engine.Options;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Options.ManagedOptions;

namespace TaleWorlds.MountAndBlade.Options
{
	public static class OptionsProvider
	{
		public static OptionCategory GetVideoOptionCategory(bool isMainMenu, Action onBrightnessClick, Action onExposureClick, Action onBenchmarkClick)
		{
			return new OptionCategory(OptionsProvider.GetVideoGeneralOptions(isMainMenu, onBrightnessClick, onExposureClick, onBenchmarkClick), OptionsProvider.GetVideoOptionGroups());
		}

		private static IEnumerable<IOptionData> GetVideoGeneralOptions(bool isMainMenu, Action onBrightnessClick, Action onExposureClick, Action onBenchmarkClick)
		{
			if (isMainMenu)
			{
				yield return new ActionOptionData("Benchmark", onBenchmarkClick);
			}
			yield return new ActionOptionData(NativeOptions.NativeOptionsType.Brightness, onBrightnessClick);
			yield return new ActionOptionData(NativeOptions.NativeOptionsType.ExposureCompensation, onExposureClick);
			yield return new NativeSelectionOptionData(NativeOptions.NativeOptionsType.SelectedMonitor);
			yield return new NativeSelectionOptionData(NativeOptions.NativeOptionsType.SelectedAdapter);
			yield return new NativeSelectionOptionData(NativeOptions.NativeOptionsType.DisplayMode);
			yield return new NativeSelectionOptionData(NativeOptions.NativeOptionsType.ScreenResolution);
			yield return new NativeSelectionOptionData(NativeOptions.NativeOptionsType.RefreshRate);
			yield return new NativeSelectionOptionData(NativeOptions.NativeOptionsType.VSync);
			yield return new ManagedBooleanOptionData(ManagedOptions.ManagedOptionsType.ForceVSyncInMenus);
			yield return new NativeNumericOptionData(NativeOptions.NativeOptionsType.FrameLimiter);
			yield return new NativeNumericOptionData(NativeOptions.NativeOptionsType.SharpenAmount);
			yield break;
		}

		private static IEnumerable<IOptionData> GetPerformanceGeneralOptions(bool isMultiplayer)
		{
			yield return new NativeSelectionOptionData(NativeOptions.NativeOptionsType.OverAll);
			yield break;
		}

		private static IEnumerable<OptionGroup> GetVideoOptionGroups()
		{
			return null;
		}

		public static OptionCategory GetPerformanceOptionCategory(bool isMultiplayer)
		{
			return new OptionCategory(OptionsProvider.GetPerformanceGeneralOptions(isMultiplayer), OptionsProvider.GetPerformanceOptionGroups(isMultiplayer));
		}

		private static IEnumerable<OptionGroup> GetPerformanceOptionGroups(bool isMultiplayer)
		{
			yield return new OptionGroup(new TextObject("{=sRTd3RI5}Graphics", null), OptionsProvider.GetPerformanceGraphicsOptions(isMultiplayer));
			yield return new OptionGroup(new TextObject("{=vDMe8SCV}Resolution Scaling", null), OptionsProvider.GetPerformanceResolutionScalingOptions(isMultiplayer));
			yield return new OptionGroup(new TextObject("{=2zcrC0h1}Gameplay", null), OptionsProvider.GetPerformanceGameplayOptions(isMultiplayer));
			yield return new OptionGroup(new TextObject("{=xebFLnH2}Audio", null), OptionsProvider.GetPerformanceAudioOptions());
			yield break;
		}

		public static IEnumerable<IOptionData> GetPerformanceGraphicsOptions(bool isMultiplayer)
		{
			yield return new NativeSelectionOptionData(NativeOptions.NativeOptionsType.Antialiasing);
			yield return new NativeSelectionOptionData(NativeOptions.NativeOptionsType.ShaderQuality);
			yield return new NativeSelectionOptionData(NativeOptions.NativeOptionsType.TextureBudget);
			yield return new NativeSelectionOptionData(NativeOptions.NativeOptionsType.TextureQuality);
			yield return new NativeSelectionOptionData(NativeOptions.NativeOptionsType.TextureFiltering);
			yield return new NativeSelectionOptionData(NativeOptions.NativeOptionsType.CharacterDetail);
			yield return new NativeSelectionOptionData(NativeOptions.NativeOptionsType.ShadowmapResolution);
			yield return new NativeSelectionOptionData(NativeOptions.NativeOptionsType.ShadowmapType);
			yield return new NativeSelectionOptionData(NativeOptions.NativeOptionsType.ShadowmapFiltering);
			yield return new NativeSelectionOptionData(NativeOptions.NativeOptionsType.ParticleDetail);
			yield return new NativeSelectionOptionData(NativeOptions.NativeOptionsType.ParticleQuality);
			yield return new NativeSelectionOptionData(NativeOptions.NativeOptionsType.FoliageQuality);
			yield return new NativeSelectionOptionData(NativeOptions.NativeOptionsType.TerrainQuality);
			yield return new NativeSelectionOptionData(NativeOptions.NativeOptionsType.EnvironmentDetail);
			yield return new NativeSelectionOptionData(NativeOptions.NativeOptionsType.Occlusion);
			yield return new NativeSelectionOptionData(NativeOptions.NativeOptionsType.DecalQuality);
			yield return new NativeSelectionOptionData(NativeOptions.NativeOptionsType.WaterQuality);
			if (!isMultiplayer)
			{
				yield return new ManagedSelectionOptionData(ManagedOptions.ManagedOptionsType.NumberOfCorpses);
			}
			yield return new NativeSelectionOptionData(NativeOptions.NativeOptionsType.NumberOfRagDolls);
			yield return new NativeSelectionOptionData(NativeOptions.NativeOptionsType.LightingQuality);
			yield return new NativeBooleanOptionData(NativeOptions.NativeOptionsType.ClothSimulation);
			yield return new NativeBooleanOptionData(NativeOptions.NativeOptionsType.SunShafts);
			yield return new NativeBooleanOptionData(NativeOptions.NativeOptionsType.Tesselation);
			yield return new NativeBooleanOptionData(NativeOptions.NativeOptionsType.InteractiveGrass);
			yield return new NativeBooleanOptionData(NativeOptions.NativeOptionsType.SSR);
			yield return new NativeBooleanOptionData(NativeOptions.NativeOptionsType.SSSSS);
			yield return new NativeBooleanOptionData(NativeOptions.NativeOptionsType.MotionBlur);
			yield return new NativeBooleanOptionData(NativeOptions.NativeOptionsType.DepthOfField);
			yield return new NativeBooleanOptionData(NativeOptions.NativeOptionsType.Bloom);
			yield return new NativeBooleanOptionData(NativeOptions.NativeOptionsType.FilmGrain);
			if (NativeOptions.CheckGFXSupportStatus(61))
			{
				yield return new NativeBooleanOptionData(NativeOptions.NativeOptionsType.PostFXVignette);
			}
			if (NativeOptions.CheckGFXSupportStatus(60))
			{
				yield return new NativeBooleanOptionData(NativeOptions.NativeOptionsType.PostFXChromaticAberration);
			}
			if (NativeOptions.CheckGFXSupportStatus(58))
			{
				yield return new NativeBooleanOptionData(NativeOptions.NativeOptionsType.PostFXLensFlare);
			}
			if (NativeOptions.CheckGFXSupportStatus(62))
			{
				yield return new NativeBooleanOptionData(NativeOptions.NativeOptionsType.PostFXHexagonVignette);
			}
			if (NativeOptions.CheckGFXSupportStatus(59))
			{
				yield return new NativeBooleanOptionData(NativeOptions.NativeOptionsType.PostFXStreaks);
			}
			yield break;
		}

		public static IEnumerable<IOptionData> GetPerformanceResolutionScalingOptions(bool isMultiplayer)
		{
			yield return new NativeSelectionOptionData(NativeOptions.NativeOptionsType.DLSS);
			yield return new NativeNumericOptionData(NativeOptions.NativeOptionsType.ResolutionScale);
			yield return new NativeBooleanOptionData(NativeOptions.NativeOptionsType.DynamicResolution);
			yield return new NativeNumericOptionData(NativeOptions.NativeOptionsType.DynamicResolutionTarget);
			yield break;
		}

		public static IEnumerable<IOptionData> GetPerformanceGameplayOptions(bool isMultiplayer)
		{
			if (!isMultiplayer)
			{
				yield return new ManagedSelectionOptionData(ManagedOptions.ManagedOptionsType.BattleSize);
			}
			yield return new NativeSelectionOptionData(NativeOptions.NativeOptionsType.AnimationSamplingQuality);
			yield break;
		}

		public static IEnumerable<IOptionData> GetPerformanceAudioOptions()
		{
			yield return new NativeSelectionOptionData(NativeOptions.NativeOptionsType.MaxSimultaneousSoundEventCount);
			yield break;
		}

		private static IEnumerable<IOptionData> GetAudioGeneralOptions(bool isMultiplayer)
		{
			yield return new NativeNumericOptionData(NativeOptions.NativeOptionsType.MasterVolume);
			yield return new NativeNumericOptionData(NativeOptions.NativeOptionsType.SoundVolume);
			yield return new NativeNumericOptionData(NativeOptions.NativeOptionsType.MusicVolume);
			yield return new NativeNumericOptionData(NativeOptions.NativeOptionsType.VoiceOverVolume);
			yield return new NativeSelectionOptionData(NativeOptions.NativeOptionsType.SoundDevice);
			yield return new NativeSelectionOptionData(NativeOptions.NativeOptionsType.SoundOutput);
			yield return new NativeBooleanOptionData(NativeOptions.NativeOptionsType.KeepSoundInBackground);
			if (isMultiplayer)
			{
				yield return new ManagedBooleanOptionData(ManagedOptions.ManagedOptionsType.EnableVoiceChat);
			}
			else
			{
				yield return new NativeBooleanOptionData(NativeOptions.NativeOptionsType.SoundOcclusion);
			}
			yield break;
		}

		public static OptionCategory GetAudioOptionCategory(bool isMultiplayer)
		{
			return new OptionCategory(OptionsProvider.GetAudioGeneralOptions(isMultiplayer), OptionsProvider.GetAudioOptionGroups(isMultiplayer));
		}

		private static IEnumerable<OptionGroup> GetAudioOptionGroups(bool isMultiplayer)
		{
			return null;
		}

		public static OptionCategory GetGameplayOptionCategory(bool isMultiplayer)
		{
			return new OptionCategory(OptionsProvider.GetGameplayGeneralOptions(isMultiplayer), OptionsProvider.GetGameplayOptionGroups(isMultiplayer));
		}

		private static IEnumerable<IOptionData> GetGameplayGeneralOptions(bool isMultiplayer)
		{
			yield return new ManagedSelectionOptionData(ManagedOptions.ManagedOptionsType.Language);
			if (!isMultiplayer)
			{
				yield return new ManagedSelectionOptionData(ManagedOptions.ManagedOptionsType.VoiceLanguage);
				yield return new ManagedSelectionOptionData(ManagedOptions.ManagedOptionsType.UnitSpawnPrioritization);
				yield return new ManagedSelectionOptionData(ManagedOptions.ManagedOptionsType.ReinforcementWaveCount);
			}
			yield break;
		}

		private static IEnumerable<OptionGroup> GetGameplayOptionGroups(bool isMultiplayer)
		{
			yield return new OptionGroup(new TextObject("{=m9KoYCv5}Controls", null), OptionsProvider.GetGameplayControlsOptions(isMultiplayer));
			yield return new OptionGroup(new TextObject("{=uZ6q4Qs2}Visuals", null), OptionsProvider.GetGameplayVisualOptions(isMultiplayer));
			yield return new OptionGroup(new TextObject("{=gAfbULHM}Camera", null), OptionsProvider.GetGameplayCameraOptions(isMultiplayer));
			yield return new OptionGroup(new TextObject("{=WRMyiiYJ}User Interface", null), OptionsProvider.GetGameplayUIOptions(isMultiplayer));
			if (!isMultiplayer)
			{
				yield return new OptionGroup(new TextObject("{=ys9baYiQ}Campaign", null), OptionsProvider.GetGameplayCampaignOptions());
			}
			yield break;
		}

		private static IEnumerable<IOptionData> GetGameplayControlsOptions(bool isMultiplayer)
		{
			yield return new ManagedSelectionOptionData(ManagedOptions.ManagedOptionsType.ControlBlockDirection);
			yield return new ManagedSelectionOptionData(ManagedOptions.ManagedOptionsType.ControlAttackDirection);
			yield return new NativeNumericOptionData(NativeOptions.NativeOptionsType.MouseYMovementScale);
			yield return new NativeNumericOptionData(NativeOptions.NativeOptionsType.MouseSensitivity);
			yield return new NativeBooleanOptionData(NativeOptions.NativeOptionsType.InvertMouseYAxis);
			yield return new NativeBooleanOptionData(NativeOptions.NativeOptionsType.EnableVibration);
			if (!isMultiplayer)
			{
				yield return new ManagedBooleanOptionData(ManagedOptions.ManagedOptionsType.LockTarget);
				yield return new ManagedBooleanOptionData(ManagedOptions.ManagedOptionsType.SlowDownOnOrder);
				yield return new ManagedBooleanOptionData(ManagedOptions.ManagedOptionsType.StopGameOnFocusLost);
			}
			yield break;
		}

		private static IEnumerable<IOptionData> GetGameplayVisualOptions(bool isMultiplayer)
		{
			yield return new NativeNumericOptionData(NativeOptions.NativeOptionsType.TrailAmount);
			yield return new ManagedNumericOptionData(ManagedOptions.ManagedOptionsType.FriendlyTroopsBannerOpacity);
			yield return new ManagedBooleanOptionData(ManagedOptions.ManagedOptionsType.ShowBlood);
			yield break;
		}

		private static IEnumerable<IOptionData> GetGameplayCameraOptions(bool isMultiplayer)
		{
			yield return new ManagedSelectionOptionData(ManagedOptions.ManagedOptionsType.TurnCameraWithHorseInFirstPerson);
			yield return new ManagedNumericOptionData(ManagedOptions.ManagedOptionsType.FirstPersonFov);
			yield return new ManagedNumericOptionData(ManagedOptions.ManagedOptionsType.CombatCameraDistance);
			yield return new ManagedBooleanOptionData(ManagedOptions.ManagedOptionsType.EnableVerticalAimCorrection);
			yield break;
		}

		private static IEnumerable<IOptionData> GetGameplayUIOptions(bool isMultiplayer)
		{
			yield return new ManagedSelectionOptionData(ManagedOptions.ManagedOptionsType.CrosshairType);
			yield return new ManagedSelectionOptionData(ManagedOptions.ManagedOptionsType.OrderType);
			yield return new ManagedSelectionOptionData(ManagedOptions.ManagedOptionsType.OrderLayoutType);
			yield return new ManagedSelectionOptionData(ManagedOptions.ManagedOptionsType.ReportCasualtiesType);
			yield return new ManagedNumericOptionData(ManagedOptions.ManagedOptionsType.UIScale);
			yield return new ManagedBooleanOptionData(ManagedOptions.ManagedOptionsType.ShowAttackDirection);
			yield return new ManagedBooleanOptionData(ManagedOptions.ManagedOptionsType.ShowTargetingReticle);
			yield return new ManagedBooleanOptionData(ManagedOptions.ManagedOptionsType.ReportDamage);
			yield return new ManagedBooleanOptionData(ManagedOptions.ManagedOptionsType.ReportBark);
			if (!isMultiplayer)
			{
				yield return new ManagedBooleanOptionData(ManagedOptions.ManagedOptionsType.ReportExperience);
			}
			yield return new ManagedBooleanOptionData(ManagedOptions.ManagedOptionsType.ReportPersonalDamage);
			yield return new ManagedBooleanOptionData(ManagedOptions.ManagedOptionsType.EnableDamageTakenVisuals);
			if (isMultiplayer)
			{
				yield return new ManagedBooleanOptionData(ManagedOptions.ManagedOptionsType.EnableMultiplayerChatBox);
				yield return new ManagedBooleanOptionData(ManagedOptions.ManagedOptionsType.EnableDeathIcon);
				yield return new ManagedBooleanOptionData(ManagedOptions.ManagedOptionsType.EnableNetworkAlertIcons);
				yield return new ManagedBooleanOptionData(ManagedOptions.ManagedOptionsType.EnableGenericAvatars);
				yield return new ManagedBooleanOptionData(ManagedOptions.ManagedOptionsType.EnableGenericNames);
			}
			else
			{
				yield return new ManagedBooleanOptionData(ManagedOptions.ManagedOptionsType.EnableSingleplayerChatBox);
				yield return new ManagedBooleanOptionData(ManagedOptions.ManagedOptionsType.HideBattleUI);
				yield return new ManagedBooleanOptionData(ManagedOptions.ManagedOptionsType.EnableTutorialHints);
			}
			yield break;
		}

		private static IEnumerable<IOptionData> GetGameplayCampaignOptions()
		{
			yield return new ManagedSelectionOptionData(ManagedOptions.ManagedOptionsType.AutoTrackAttackedSettlements);
			yield return new ManagedNumericOptionData(ManagedOptions.ManagedOptionsType.AutoSaveInterval);
			yield break;
		}

		public static IEnumerable<string> GetGameKeyCategoriesList(bool isMultiplayer)
		{
			yield return GameKeyMainCategories.ActionCategory;
			yield return GameKeyMainCategories.OrderMenuCategory;
			if (!isMultiplayer)
			{
				yield return GameKeyMainCategories.CampaignMapCategory;
				yield return GameKeyMainCategories.MenuShortcutCategory;
				yield return GameKeyMainCategories.PhotoModeCategory;
			}
			else
			{
				yield return GameKeyMainCategories.PollCategory;
			}
			yield return GameKeyMainCategories.ChatCategory;
			yield break;
		}

		public static OptionCategory GetControllerOptionCategory()
		{
			return new OptionCategory(OptionsProvider.GetControllerBaseOptions(), OptionsProvider.GetControllerOptionGroups());
		}

		private static IEnumerable<IOptionData> GetControllerBaseOptions()
		{
			return null;
		}

		private static IEnumerable<OptionGroup> GetControllerOptionGroups()
		{
			return null;
		}

		public static Dictionary<NativeOptions.NativeOptionsType, float[]> GetDefaultNativeOptions()
		{
			if (OptionsProvider._defaultNativeOptions == null)
			{
				OptionsProvider._defaultNativeOptions = new Dictionary<NativeOptions.NativeOptionsType, float[]>();
				foreach (NativeOptionData nativeOptionData in NativeOptions.VideoOptions.Union(NativeOptions.GraphicsOptions))
				{
					float[] array = new float[OptionsProvider._overallConfigCount];
					bool flag = false;
					for (int i = 0; i < OptionsProvider._overallConfigCount; i++)
					{
						array[i] = NativeOptions.GetDefaultConfigForOverallSettings(nativeOptionData.Type, i);
						if (array[i] < 0f)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						OptionsProvider._defaultNativeOptions[nativeOptionData.Type] = array;
					}
				}
			}
			return OptionsProvider._defaultNativeOptions;
		}

		public static Dictionary<ManagedOptions.ManagedOptionsType, float[]> GetDefaultManagedOptions()
		{
			if (OptionsProvider._defaultManagedOptions == null)
			{
				OptionsProvider._defaultManagedOptions = new Dictionary<ManagedOptions.ManagedOptionsType, float[]>();
				float[] array = new float[OptionsProvider._overallConfigCount];
				for (int i = 0; i < OptionsProvider._overallConfigCount; i++)
				{
					array[i] = (float)i;
				}
				OptionsProvider._defaultManagedOptions.Add(ManagedOptions.ManagedOptionsType.BattleSize, array);
				array = new float[OptionsProvider._overallConfigCount];
				for (int j = 0; j < OptionsProvider._overallConfigCount; j++)
				{
					array[j] = (float)j;
				}
				OptionsProvider._defaultManagedOptions.Add(ManagedOptions.ManagedOptionsType.NumberOfCorpses, array);
			}
			return OptionsProvider._defaultManagedOptions;
		}

		private static readonly int _overallConfigCount = NativeSelectionOptionData.GetOptionsLimit(NativeOptions.NativeOptionsType.OverAll) - 1;

		private static Dictionary<NativeOptions.NativeOptionsType, float[]> _defaultNativeOptions;

		private static Dictionary<ManagedOptions.ManagedOptionsType, float[]> _defaultManagedOptions;
	}
}
