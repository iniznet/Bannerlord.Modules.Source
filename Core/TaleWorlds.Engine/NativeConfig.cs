using System;
using TaleWorlds.Engine.Options;

namespace TaleWorlds.Engine
{
	public static class NativeConfig
	{
		public static bool CheatMode { get; private set; }

		public static bool IsDevelopmentMode { get; private set; }

		public static bool LocalizationDebugMode { get; private set; }

		public static bool GetUIDebugMode { get; private set; }

		public static bool DisableSound { get; private set; }

		public static bool EnableEditMode { get; private set; }

		public static void OnConfigChanged()
		{
			NativeConfig.CheatMode = EngineApplicationInterface.IConfig.GetCheatMode();
			NativeConfig.IsDevelopmentMode = EngineApplicationInterface.IConfig.GetDevelopmentMode();
			NativeConfig.GetUIDebugMode = EngineApplicationInterface.IConfig.GetUIDebugMode();
			NativeConfig.LocalizationDebugMode = EngineApplicationInterface.IConfig.GetLocalizationDebugMode();
			NativeConfig.EnableEditMode = EngineApplicationInterface.IConfig.GetEnableEditMode();
			NativeConfig.DisableSound = EngineApplicationInterface.IConfig.GetDisableSound();
		}

		public static bool TableauCacheEnabled
		{
			get
			{
				return EngineApplicationInterface.IConfig.GetTableauCacheMode();
			}
		}

		public static bool DoLocalizationCheckAtStartup
		{
			get
			{
				return EngineApplicationInterface.IConfig.GetDoLocalizationCheckAtStartup();
			}
		}

		public static bool EnableClothSimulation
		{
			get
			{
				return EngineApplicationInterface.IConfig.GetEnableClothSimulation();
			}
		}

		public static int CharacterDetail
		{
			get
			{
				return EngineApplicationInterface.IConfig.GetCharacterDetail();
			}
		}

		public static bool InvertMouse
		{
			get
			{
				return EngineApplicationInterface.IConfig.GetInvertMouse();
			}
		}

		public static string LastOpenedScene
		{
			get
			{
				return EngineApplicationInterface.IConfig.GetLastOpenedScene();
			}
		}

		public static int AutoSaveInMinutes
		{
			get
			{
				return EngineApplicationInterface.IConfig.AutoSaveInMinutes();
			}
		}

		public static bool GetUIDoNotUseGeneratedPrefabs
		{
			get
			{
				return EngineApplicationInterface.IConfig.GetUIDoNotUseGeneratedPrefabs();
			}
		}

		public static string DebugLoginUsername
		{
			get
			{
				return EngineApplicationInterface.IConfig.GetDebugLoginUserName();
			}
		}

		public static string DebugLogicPassword
		{
			get
			{
				return EngineApplicationInterface.IConfig.GetDebugLoginPassword();
			}
		}

		public static bool DisableGuiMessages
		{
			get
			{
				return EngineApplicationInterface.IConfig.GetDisableGuiMessages();
			}
		}

		public static NativeOptions.ConfigQuality AutoGFXQuality
		{
			get
			{
				return (NativeOptions.ConfigQuality)EngineApplicationInterface.IConfig.GetAutoGFXQuality();
			}
		}

		public static void SetAutoConfigWrtHardware()
		{
			EngineApplicationInterface.IConfig.SetAutoConfigWrtHardware();
		}
	}
}
