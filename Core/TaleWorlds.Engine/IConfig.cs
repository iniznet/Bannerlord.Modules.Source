using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x02000015 RID: 21
	[ApplicationInterfaceBase]
	internal interface IConfig
	{
		// Token: 0x060000B4 RID: 180
		[EngineMethod("check_gfx_support_status", false)]
		bool CheckGFXSupportStatus(int enum_id);

		// Token: 0x060000B5 RID: 181
		[EngineMethod("is_dlss_available", false)]
		bool IsDlssAvailable();

		// Token: 0x060000B6 RID: 182
		[EngineMethod("is_120hz_available", false)]
		bool Is120HzAvailable();

		// Token: 0x060000B7 RID: 183
		[EngineMethod("get_dlss_technique", false)]
		int GetDlssTechnique();

		// Token: 0x060000B8 RID: 184
		[EngineMethod("get_dlss_option_count", false)]
		int GetDlssOptionCount();

		// Token: 0x060000B9 RID: 185
		[EngineMethod("get_disable_sound", false)]
		bool GetDisableSound();

		// Token: 0x060000BA RID: 186
		[EngineMethod("get_cheat_mode", false)]
		bool GetCheatMode();

		// Token: 0x060000BB RID: 187
		[EngineMethod("get_development_mode", false)]
		bool GetDevelopmentMode();

		// Token: 0x060000BC RID: 188
		[EngineMethod("get_localization_debug_mode", false)]
		bool GetLocalizationDebugMode();

		// Token: 0x060000BD RID: 189
		[EngineMethod("get_do_localization_check_at_startup", false)]
		bool GetDoLocalizationCheckAtStartup();

		// Token: 0x060000BE RID: 190
		[EngineMethod("get_tableau_cache_mode", false)]
		bool GetTableauCacheMode();

		// Token: 0x060000BF RID: 191
		[EngineMethod("get_enable_edit_mode", false)]
		bool GetEnableEditMode();

		// Token: 0x060000C0 RID: 192
		[EngineMethod("get_enable_cloth_simulation", false)]
		bool GetEnableClothSimulation();

		// Token: 0x060000C1 RID: 193
		[EngineMethod("get_character_detail", false)]
		int GetCharacterDetail();

		// Token: 0x060000C2 RID: 194
		[EngineMethod("get_invert_mouse", false)]
		bool GetInvertMouse();

		// Token: 0x060000C3 RID: 195
		[EngineMethod("get_last_opened_scene", false)]
		string GetLastOpenedScene();

		// Token: 0x060000C4 RID: 196
		[EngineMethod("read_rgl_config_files", false)]
		void ReadRGLConfigFiles();

		// Token: 0x060000C5 RID: 197
		[EngineMethod("set_rgl_config", false)]
		void SetRGLConfig(int type, float value);

		// Token: 0x060000C6 RID: 198
		[EngineMethod("apply_config_changes", false)]
		void ApplyConfigChanges(bool resizeWindow);

		// Token: 0x060000C7 RID: 199
		[EngineMethod("get_rgl_config_for_default_settings", false)]
		float GetRGLConfigForDefaultSettings(int type, int defaultSettings);

		// Token: 0x060000C8 RID: 200
		[EngineMethod("get_rgl_config", false)]
		float GetRGLConfig(int type);

		// Token: 0x060000C9 RID: 201
		[EngineMethod("get_default_rgl_config", false)]
		float GetDefaultRGLConfig(int type);

		// Token: 0x060000CA RID: 202
		[EngineMethod("save_rgl_config", false)]
		int SaveRGLConfig();

		// Token: 0x060000CB RID: 203
		[EngineMethod("set_brightness", false)]
		void SetBrightness(float brightness);

		// Token: 0x060000CC RID: 204
		[EngineMethod("set_sharpen_amount", false)]
		void SetSharpenAmount(float sharpen_amount);

		// Token: 0x060000CD RID: 205
		[EngineMethod("get_sound_device_name", false)]
		string GetSoundDeviceName(int i);

		// Token: 0x060000CE RID: 206
		[EngineMethod("get_current_sound_device_index", false)]
		int GetCurrentSoundDeviceIndex();

		// Token: 0x060000CF RID: 207
		[EngineMethod("get_sound_device_count", false)]
		int GetSoundDeviceCount();

		// Token: 0x060000D0 RID: 208
		[EngineMethod("get_resolution_count", false)]
		int GetResolutionCount();

		// Token: 0x060000D1 RID: 209
		[EngineMethod("get_refresh_rate_count", false)]
		int GetRefreshRateCount();

		// Token: 0x060000D2 RID: 210
		[EngineMethod("get_refresh_rate_at_index", false)]
		int GetRefreshRateAtIndex(int index);

		// Token: 0x060000D3 RID: 211
		[EngineMethod("get_resolution", false)]
		void GetResolution(ref int width, ref int height);

		// Token: 0x060000D4 RID: 212
		[EngineMethod("get_desktop_resolution", false)]
		void GetDesktopResolution(ref int width, ref int height);

		// Token: 0x060000D5 RID: 213
		[EngineMethod("get_resolution_at_index", false)]
		Vec2 GetResolutionAtIndex(int index);

		// Token: 0x060000D6 RID: 214
		[EngineMethod("set_custom_resolution", false)]
		void SetCustomResolution(int width, int height);

		// Token: 0x060000D7 RID: 215
		[EngineMethod("refresh_options_data ", false)]
		void RefreshOptionsData();

		// Token: 0x060000D8 RID: 216
		[EngineMethod("set_sound_device", false)]
		void SetSoundDevice(int i);

		// Token: 0x060000D9 RID: 217
		[EngineMethod("apply", false)]
		void Apply(int texture_budget, int sharpen_amount, int hdr, int dof_mode, int motion_blur, int ssr, int size, int texture_filtering, int trail_amount, int dynamic_resolution_target);

		// Token: 0x060000DA RID: 218
		[EngineMethod("set_default_game_config", false)]
		void SetDefaultGameConfig();

		// Token: 0x060000DB RID: 219
		[EngineMethod("auto_save_in_minutes", false)]
		int AutoSaveInMinutes();

		// Token: 0x060000DC RID: 220
		[EngineMethod("get_ui_debug_mode", false)]
		bool GetUIDebugMode();

		// Token: 0x060000DD RID: 221
		[EngineMethod("get_ui_do_not_use_generated_prefabs", false)]
		bool GetUIDoNotUseGeneratedPrefabs();

		// Token: 0x060000DE RID: 222
		[EngineMethod("get_debug_login_username", false)]
		string GetDebugLoginUserName();

		// Token: 0x060000DF RID: 223
		[EngineMethod("get_debug_login_password", false)]
		string GetDebugLoginPassword();

		// Token: 0x060000E0 RID: 224
		[EngineMethod("get_disable_gui_messages", false)]
		bool GetDisableGuiMessages();

		// Token: 0x060000E1 RID: 225
		[EngineMethod("get_auto_gfx_quality", false)]
		int GetAutoGFXQuality();

		// Token: 0x060000E2 RID: 226
		[EngineMethod("set_auto_config_wrt_hardware", false)]
		void SetAutoConfigWrtHardware();

		// Token: 0x060000E3 RID: 227
		[EngineMethod("get_monitor_device_name", false)]
		string GetMonitorDeviceName(int i);

		// Token: 0x060000E4 RID: 228
		[EngineMethod("get_video_device_name", false)]
		string GetVideoDeviceName(int i);

		// Token: 0x060000E5 RID: 229
		[EngineMethod("get_monitor_device_count", false)]
		int GetMonitorDeviceCount();

		// Token: 0x060000E6 RID: 230
		[EngineMethod("get_video_device_count", false)]
		int GetVideoDeviceCount();
	}
}
