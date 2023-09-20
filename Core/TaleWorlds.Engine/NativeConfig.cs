using System;
using TaleWorlds.Engine.Options;

namespace TaleWorlds.Engine
{
	// Token: 0x0200006C RID: 108
	public static class NativeConfig
	{
		// Token: 0x17000051 RID: 81
		// (get) Token: 0x06000866 RID: 2150 RVA: 0x0000860E File Offset: 0x0000680E
		// (set) Token: 0x06000867 RID: 2151 RVA: 0x00008615 File Offset: 0x00006815
		public static bool CheatMode { get; private set; }

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x06000868 RID: 2152 RVA: 0x0000861D File Offset: 0x0000681D
		// (set) Token: 0x06000869 RID: 2153 RVA: 0x00008624 File Offset: 0x00006824
		public static bool IsDevelopmentMode { get; private set; }

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x0600086A RID: 2154 RVA: 0x0000862C File Offset: 0x0000682C
		// (set) Token: 0x0600086B RID: 2155 RVA: 0x00008633 File Offset: 0x00006833
		public static bool LocalizationDebugMode { get; private set; }

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x0600086C RID: 2156 RVA: 0x0000863B File Offset: 0x0000683B
		// (set) Token: 0x0600086D RID: 2157 RVA: 0x00008642 File Offset: 0x00006842
		public static bool GetUIDebugMode { get; private set; }

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x0600086E RID: 2158 RVA: 0x0000864A File Offset: 0x0000684A
		// (set) Token: 0x0600086F RID: 2159 RVA: 0x00008651 File Offset: 0x00006851
		public static bool DisableSound { get; private set; }

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x06000870 RID: 2160 RVA: 0x00008659 File Offset: 0x00006859
		// (set) Token: 0x06000871 RID: 2161 RVA: 0x00008660 File Offset: 0x00006860
		public static bool EnableEditMode { get; private set; }

		// Token: 0x06000872 RID: 2162 RVA: 0x00008668 File Offset: 0x00006868
		public static void OnConfigChanged()
		{
			NativeConfig.CheatMode = EngineApplicationInterface.IConfig.GetCheatMode();
			NativeConfig.IsDevelopmentMode = EngineApplicationInterface.IConfig.GetDevelopmentMode();
			NativeConfig.GetUIDebugMode = EngineApplicationInterface.IConfig.GetUIDebugMode();
			NativeConfig.LocalizationDebugMode = EngineApplicationInterface.IConfig.GetLocalizationDebugMode();
			NativeConfig.EnableEditMode = EngineApplicationInterface.IConfig.GetEnableEditMode();
			NativeConfig.DisableSound = EngineApplicationInterface.IConfig.GetDisableSound();
		}

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x06000873 RID: 2163 RVA: 0x000086CF File Offset: 0x000068CF
		public static bool TableauCacheEnabled
		{
			get
			{
				return EngineApplicationInterface.IConfig.GetTableauCacheMode();
			}
		}

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x06000874 RID: 2164 RVA: 0x000086DB File Offset: 0x000068DB
		public static bool DoLocalizationCheckAtStartup
		{
			get
			{
				return EngineApplicationInterface.IConfig.GetDoLocalizationCheckAtStartup();
			}
		}

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x06000875 RID: 2165 RVA: 0x000086E7 File Offset: 0x000068E7
		public static bool EnableClothSimulation
		{
			get
			{
				return EngineApplicationInterface.IConfig.GetEnableClothSimulation();
			}
		}

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x06000876 RID: 2166 RVA: 0x000086F3 File Offset: 0x000068F3
		public static int CharacterDetail
		{
			get
			{
				return EngineApplicationInterface.IConfig.GetCharacterDetail();
			}
		}

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x06000877 RID: 2167 RVA: 0x000086FF File Offset: 0x000068FF
		public static bool InvertMouse
		{
			get
			{
				return EngineApplicationInterface.IConfig.GetInvertMouse();
			}
		}

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x06000878 RID: 2168 RVA: 0x0000870B File Offset: 0x0000690B
		public static string LastOpenedScene
		{
			get
			{
				return EngineApplicationInterface.IConfig.GetLastOpenedScene();
			}
		}

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x06000879 RID: 2169 RVA: 0x00008717 File Offset: 0x00006917
		public static int AutoSaveInMinutes
		{
			get
			{
				return EngineApplicationInterface.IConfig.AutoSaveInMinutes();
			}
		}

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x0600087A RID: 2170 RVA: 0x00008723 File Offset: 0x00006923
		public static bool GetUIDoNotUseGeneratedPrefabs
		{
			get
			{
				return EngineApplicationInterface.IConfig.GetUIDoNotUseGeneratedPrefabs();
			}
		}

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x0600087B RID: 2171 RVA: 0x0000872F File Offset: 0x0000692F
		public static string DebugLoginUsername
		{
			get
			{
				return EngineApplicationInterface.IConfig.GetDebugLoginUserName();
			}
		}

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x0600087C RID: 2172 RVA: 0x0000873B File Offset: 0x0000693B
		public static string DebugLogicPassword
		{
			get
			{
				return EngineApplicationInterface.IConfig.GetDebugLoginPassword();
			}
		}

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x0600087D RID: 2173 RVA: 0x00008747 File Offset: 0x00006947
		public static bool DisableGuiMessages
		{
			get
			{
				return EngineApplicationInterface.IConfig.GetDisableGuiMessages();
			}
		}

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x0600087E RID: 2174 RVA: 0x00008753 File Offset: 0x00006953
		public static NativeOptions.ConfigQuality AutoGFXQuality
		{
			get
			{
				return (NativeOptions.ConfigQuality)EngineApplicationInterface.IConfig.GetAutoGFXQuality();
			}
		}

		// Token: 0x0600087F RID: 2175 RVA: 0x0000875F File Offset: 0x0000695F
		public static void SetAutoConfigWrtHardware()
		{
			EngineApplicationInterface.IConfig.SetAutoConfigWrtHardware();
		}
	}
}
