using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x02000036 RID: 54
	[ApplicationInterfaceBase]
	internal interface IUtil
	{
		// Token: 0x0600047B RID: 1147
		[EngineMethod("output_benchmark_values_to_performance_reporter", false)]
		void OutputBenchmarkValuesToPerformanceReporter();

		// Token: 0x0600047C RID: 1148
		[EngineMethod("set_loading_screen_percentage", false)]
		void SetLoadingScreenPercentage(float value);

		// Token: 0x0600047D RID: 1149
		[EngineMethod("set_fixed_dt", false)]
		void SetFixedDt(bool enabled, float dt);

		// Token: 0x0600047E RID: 1150
		[EngineMethod("set_benchmark_status", false)]
		void SetBenchmarkStatus(int status, string def);

		// Token: 0x0600047F RID: 1151
		[EngineMethod("get_benchmark_status", false)]
		int GetBenchmarkStatus();

		// Token: 0x06000480 RID: 1152
		[EngineMethod("is_benchmark_quited", false)]
		bool IsBenchmarkQuited();

		// Token: 0x06000481 RID: 1153
		[EngineMethod("get_application_memory_statistics", false)]
		string GetApplicationMemoryStatistics();

		// Token: 0x06000482 RID: 1154
		[EngineMethod("get_native_memory_statistics", false)]
		string GetNativeMemoryStatistics();

		// Token: 0x06000483 RID: 1155
		[EngineMethod("command_line_argument_exits", false)]
		bool CommandLineArgumentExists(string str);

		// Token: 0x06000484 RID: 1156
		[EngineMethod("export_nav_mesh_face_marks", false)]
		string ExportNavMeshFaceMarks(string file_name);

		// Token: 0x06000485 RID: 1157
		[EngineMethod("take_ss_from_top", false)]
		string TakeSSFromTop(string file_name);

		// Token: 0x06000486 RID: 1158
		[EngineMethod("check_if_assets_and_sources_are_same", false)]
		void CheckIfAssetsAndSourcesAreSame();

		// Token: 0x06000487 RID: 1159
		[EngineMethod("get_snow_amount_data", false)]
		void GetSnowAmountData(byte[] snowData);

		// Token: 0x06000488 RID: 1160
		[EngineMethod("gather_core_game_references", false)]
		void GatherCoreGameReferences(string scene_names);

		// Token: 0x06000489 RID: 1161
		[EngineMethod("get_application_memory", false)]
		float GetApplicationMemory();

		// Token: 0x0600048A RID: 1162
		[EngineMethod("disable_core_game", false)]
		void DisableCoreGame();

		// Token: 0x0600048B RID: 1163
		[EngineMethod("find_meshes_without_lods", false)]
		void FindMeshesWithoutLods(string module_name);

		// Token: 0x0600048C RID: 1164
		[EngineMethod("set_print_callstack_at_crashes", false)]
		void SetPrintCallstackAtCrahses(bool value);

		// Token: 0x0600048D RID: 1165
		[EngineMethod("set_disable_dump_generation", false)]
		void SetDisableDumpGeneration(bool value);

		// Token: 0x0600048E RID: 1166
		[EngineMethod("get_modules_code", false)]
		string GetModulesCode();

		// Token: 0x0600048F RID: 1167
		[EngineMethod("get_full_module_path", false)]
		string GetFullModulePath(string moduleName);

		// Token: 0x06000490 RID: 1168
		[EngineMethod("get_full_module_paths", false)]
		string GetFullModulePaths();

		// Token: 0x06000491 RID: 1169
		[EngineMethod("get_full_file_path_of_scene", false)]
		string GetFullFilePathOfScene(string sceneName);

		// Token: 0x06000492 RID: 1170
		[EngineMethod("pair_scene_name_to_module_name", false)]
		void PairSceneNameToModuleName(string sceneName, string moduleName);

		// Token: 0x06000493 RID: 1171
		[EngineMethod("get_single_module_scenes_of_module", false)]
		string GetSingleModuleScenesOfModule(string moduleName);

		// Token: 0x06000494 RID: 1172
		[EngineMethod("get_executable_working_directory", false)]
		string GetExecutableWorkingDirectory();

		// Token: 0x06000495 RID: 1173
		[EngineMethod("add_main_thread_performance_query", false)]
		void AddMainThreadPerformanceQuery(string parent, string name, float seconds);

		// Token: 0x06000496 RID: 1174
		[EngineMethod("set_dump_folder_path", false)]
		void SetDumpFolderPath(string path);

		// Token: 0x06000497 RID: 1175
		[EngineMethod("check_scene_for_problems", false)]
		void CheckSceneForProblems(string path);

		// Token: 0x06000498 RID: 1176
		[EngineMethod("set_screen_text_rendering_state", false)]
		void SetScreenTextRenderingState(bool value);

		// Token: 0x06000499 RID: 1177
		[EngineMethod("set_message_line_rendering_state", false)]
		void SetMessageLineRenderingState(bool value);

		// Token: 0x0600049A RID: 1178
		[EngineMethod("check_shader_compilation", false)]
		bool CheckShaderCompilation();

		// Token: 0x0600049B RID: 1179
		[EngineMethod("set_crash_on_asserts", false)]
		void SetCrashOnAsserts(bool val);

		// Token: 0x0600049C RID: 1180
		[EngineMethod("check_if_terrain_shader_header_generation_finished", false)]
		bool CheckIfTerrainShaderHeaderGenerationFinished();

		// Token: 0x0600049D RID: 1181
		[EngineMethod("set_crash_on_warnings", false)]
		void SetCrashOnWarnings(bool val);

		// Token: 0x0600049E RID: 1182
		[EngineMethod("generate_terrain_shader_headers", false)]
		void GenerateTerrainShaderHeaders(string targetPlatform, string targetConfig, string output_path);

		// Token: 0x0600049F RID: 1183
		[EngineMethod("compile_terrain_shaders_dist", false)]
		void CompileTerrainShadersDist(string targetPlatform, string targetConfig, string output_path);

		// Token: 0x060004A0 RID: 1184
		[EngineMethod("compile_all_shaders", false)]
		void CompileAllShaders(string targetPlatform);

		// Token: 0x060004A1 RID: 1185
		[EngineMethod("toggle_render", false)]
		void ToggleRender();

		// Token: 0x060004A2 RID: 1186
		[EngineMethod("set_force_draw_entity_id", false)]
		void SetForceDrawEntityID(bool value);

		// Token: 0x060004A3 RID: 1187
		[EngineMethod("set_render_agents", false)]
		void SetRenderAgents(bool value);

		// Token: 0x060004A4 RID: 1188
		[EngineMethod("get_core_game_state", false)]
		int GetCoreGameState();

		// Token: 0x060004A5 RID: 1189
		[EngineMethod("set_core_game_state", false)]
		void SetCoreGameState(int state);

		// Token: 0x060004A6 RID: 1190
		[EngineMethod("execute_command_line_command", false)]
		string ExecuteCommandLineCommand(string command);

		// Token: 0x060004A7 RID: 1191
		[EngineMethod("quit_game", false)]
		void QuitGame();

		// Token: 0x060004A8 RID: 1192
		[EngineMethod("exit_process", false)]
		void ExitProcess(int exitCode);

		// Token: 0x060004A9 RID: 1193
		[EngineMethod("start_scene_performance_report", false)]
		void StartScenePerformanceReport(string folderPath);

		// Token: 0x060004AA RID: 1194
		[EngineMethod("get_base_directory", false)]
		string GetBaseDirectory();

		// Token: 0x060004AB RID: 1195
		[EngineMethod("get_visual_tests_test_files_path", false)]
		string GetVisualTestsTestFilesPath();

		// Token: 0x060004AC RID: 1196
		[EngineMethod("get_visual_tests_validate_path", false)]
		string GetVisualTestsValidatePath();

		// Token: 0x060004AD RID: 1197
		[EngineMethod("get_attachments_path", false)]
		string GetAttachmentsPath();

		// Token: 0x060004AE RID: 1198
		[EngineMethod("is_scene_performance_report_finished", false)]
		bool IsSceneReportFinished();

		// Token: 0x060004AF RID: 1199
		[EngineMethod("flush_managed_objects_memory", false)]
		void FlushManagedObjectsMemory();

		// Token: 0x060004B0 RID: 1200
		[EngineMethod("set_render_mode", false)]
		void SetRenderMode(int mode);

		// Token: 0x060004B1 RID: 1201
		[EngineMethod("add_performance_report_token", false)]
		void AddPerformanceReportToken(string performance_type, string name, float loading_time);

		// Token: 0x060004B2 RID: 1202
		[EngineMethod("add_scene_object_report", false)]
		void AddSceneObjectReport(string scene_name, string report_name, float report_value);

		// Token: 0x060004B3 RID: 1203
		[EngineMethod("output_performance_reports", false)]
		void OutputPerformanceReports();

		// Token: 0x060004B4 RID: 1204
		[EngineMethod("add_command_line_function", false)]
		void AddCommandLineFunction(string concatName);

		// Token: 0x060004B5 RID: 1205
		[EngineMethod("get_number_of_shader_compilations_in_progress", false)]
		int GetNumberOfShaderCompilationsInProgress();

		// Token: 0x060004B6 RID: 1206
		[EngineMethod("get_editor_selected_entity_count", false)]
		int GetEditorSelectedEntityCount();

		// Token: 0x060004B7 RID: 1207
		[EngineMethod("get_entity_count_of_selection_set", false)]
		int GetEntityCountOfSelectionSet(string name);

		// Token: 0x060004B8 RID: 1208
		[EngineMethod("get_build_number", false)]
		int GetBuildNumber();

		// Token: 0x060004B9 RID: 1209
		[EngineMethod("get_entities_of_selection_set", false)]
		void GetEntitiesOfSelectionSet(string name, UIntPtr[] gameEntitiesTemp);

		// Token: 0x060004BA RID: 1210
		[EngineMethod("get_editor_selected_entities", false)]
		void GetEditorSelectedEntities(UIntPtr[] gameEntitiesTemp);

		// Token: 0x060004BB RID: 1211
		[EngineMethod("delete_entities_in_editor_scene", false)]
		void DeleteEntitiesInEditorScene(UIntPtr[] gameEntities, int entityCount);

		// Token: 0x060004BC RID: 1212
		[EngineMethod("create_selection_set_in_editor", false)]
		void CreateSelectionInEditor(UIntPtr[] gameEntities, int entityCount, string name);

		// Token: 0x060004BD RID: 1213
		[EngineMethod("select_entities_in_editor", false)]
		void SelectEntities(UIntPtr[] gameEntities, int entityCount);

		// Token: 0x060004BE RID: 1214
		[EngineMethod("get_current_cpu_memory_usage", false)]
		ulong GetCurrentCpuMemoryUsage();

		// Token: 0x060004BF RID: 1215
		[EngineMethod("get_gpu_memory_stats", false)]
		void GetGPUMemoryStats(ref float totalMemory, ref float renderTargetMemory, ref float depthTargetMemory, ref float srvMemory, ref float bufferMemory);

		// Token: 0x060004C0 RID: 1216
		[EngineMethod("get_gpu_memory_of_allocation_group", false)]
		ulong GetGpuMemoryOfAllocationGroup(string allocationName);

		// Token: 0x060004C1 RID: 1217
		[EngineMethod("get_detailed_gpu_buffer_memory_stats", false)]
		void GetDetailedGPUBufferMemoryStats(ref int totalMemoryAllocated, ref int totalMemoryUsed, ref int emptyChunkCount);

		// Token: 0x060004C2 RID: 1218
		[EngineMethod("is_detailed_soung_log_on", false)]
		int IsDetailedSoundLogOn();

		// Token: 0x060004C3 RID: 1219
		[EngineMethod("get_main_fps", false)]
		float GetMainFps();

		// Token: 0x060004C4 RID: 1220
		[EngineMethod("on_loading_window_enabled", false)]
		void OnLoadingWindowEnabled();

		// Token: 0x060004C5 RID: 1221
		[EngineMethod("debug_set_global_loading_window_state", false)]
		void DebugSetGlobalLoadingWindowState(bool s);

		// Token: 0x060004C6 RID: 1222
		[EngineMethod("on_loading_window_disabled", false)]
		void OnLoadingWindowDisabled();

		// Token: 0x060004C7 RID: 1223
		[EngineMethod("disable_global_loading_window", false)]
		void DisableGlobalLoadingWindow();

		// Token: 0x060004C8 RID: 1224
		[EngineMethod("enable_global_loading_window", false)]
		void EnableGlobalLoadingWindow();

		// Token: 0x060004C9 RID: 1225
		[EngineMethod("enable_global_edit_data_cacher", false)]
		void EnableGlobalEditDataCacher();

		// Token: 0x060004CA RID: 1226
		[EngineMethod("get_renderer_fps", false)]
		float GetRendererFps();

		// Token: 0x060004CB RID: 1227
		[EngineMethod("disable_global_edit_data_cacher", false)]
		void DisableGlobalEditDataCacher();

		// Token: 0x060004CC RID: 1228
		[EngineMethod("enable_single_gpu_query_per_frame", false)]
		void EnableSingleGPUQueryPerFrame();

		// Token: 0x060004CD RID: 1229
		[EngineMethod("clear_decal_atlas", false)]
		void clear_decal_atlas(DecalAtlasGroup atlasGroup);

		// Token: 0x060004CE RID: 1230
		[EngineMethod("get_fps", false)]
		float GetFps();

		// Token: 0x060004CF RID: 1231
		[EngineMethod("get_full_command_line_string", false)]
		string GetFullCommandLineString();

		// Token: 0x060004D0 RID: 1232
		[EngineMethod("take_screenshot_from_string_path", false)]
		void TakeScreenshotFromStringPath(string path);

		// Token: 0x060004D1 RID: 1233
		[EngineMethod("take_screenshot_from_platform_path", false)]
		void TakeScreenshotFromPlatformPath(PlatformFilePath path);

		// Token: 0x060004D2 RID: 1234
		[EngineMethod("check_resource_modifications", false)]
		void CheckResourceModifications();

		// Token: 0x060004D3 RID: 1235
		[EngineMethod("set_graphics_preset", false)]
		void SetGraphicsPreset(int preset);

		// Token: 0x060004D4 RID: 1236
		[EngineMethod("clear_old_resources_and_objects", false)]
		void ClearOldResourcesAndObjects();

		// Token: 0x060004D5 RID: 1237
		[EngineMethod("load_virtual_texture_tileset", false)]
		void LoadVirtualTextureTileset(string name);

		// Token: 0x060004D6 RID: 1238
		[EngineMethod("get_delta_time", false)]
		float GetDeltaTime(int timerId);

		// Token: 0x060004D7 RID: 1239
		[EngineMethod("load_sky_boxes", false)]
		void LoadSkyBoxes();

		// Token: 0x060004D8 RID: 1240
		[EngineMethod("get_engine_frame_no", false)]
		int GetEngineFrameNo();

		// Token: 0x060004D9 RID: 1241
		[EngineMethod("set_allocation_always_valid_scene", false)]
		void SetAllocationAlwaysValidScene(UIntPtr scene);

		// Token: 0x060004DA RID: 1242
		[EngineMethod("get_console_host_machine", false)]
		string GetConsoleHostMachine();

		// Token: 0x060004DB RID: 1243
		[EngineMethod("is_edit_mode_enabled", false)]
		bool IsEditModeEnabled();

		// Token: 0x060004DC RID: 1244
		[EngineMethod("get_pc_info", false)]
		string GetPCInfo();

		// Token: 0x060004DD RID: 1245
		[EngineMethod("get_gpu_memory_mb", false)]
		int GetGPUMemoryMB();

		// Token: 0x060004DE RID: 1246
		[EngineMethod("get_current_estimated_gpu_memory_cost_mb", false)]
		int GetCurrentEstimatedGPUMemoryCostMB();

		// Token: 0x060004DF RID: 1247
		[EngineMethod("dump_gpu_memory_statistics", false)]
		void DumpGPUMemoryStatistics(string filePath);

		// Token: 0x060004E0 RID: 1248
		[EngineMethod("save_data_as_texture", false)]
		int SaveDataAsTexture(string path, int width, int height, float[] data);

		// Token: 0x060004E1 RID: 1249
		[EngineMethod("get_application_name", false)]
		string GetApplicationName();

		// Token: 0x060004E2 RID: 1250
		[EngineMethod("set_window_title", false)]
		void SetWindowTitle(string title);

		// Token: 0x060004E3 RID: 1251
		[EngineMethod("process_window_title", false)]
		string ProcessWindowTitle(string title);

		// Token: 0x060004E4 RID: 1252
		[EngineMethod("get_current_process_id", false)]
		uint GetCurrentProcessID();

		// Token: 0x060004E5 RID: 1253
		[EngineMethod("do_delayed_exit", false)]
		void DoDelayedexit(int returnCode);

		// Token: 0x060004E6 RID: 1254
		[EngineMethod("set_report_mode", false)]
		void SetReportMode(bool reportMode);

		// Token: 0x060004E7 RID: 1255
		[EngineMethod("set_assertions_and_warnings_set_exit_code", false)]
		void SetAssertionsAndWarningsSetExitCode(bool value);

		// Token: 0x060004E8 RID: 1256
		[EngineMethod("set_assertion_at_shader_compile", false)]
		void SetAssertionAtShaderCompile(bool value);

		// Token: 0x060004E9 RID: 1257
		[EngineMethod("did_automated_gi_bake_finished", false)]
		bool DidAutomatedGIBakeFinished();

		// Token: 0x060004EA RID: 1258
		[EngineMethod("do_full_bake_all_levels_automated", false)]
		void DoFullBakeAllLevelsAutomated(string module, string sceneName);

		// Token: 0x060004EB RID: 1259
		[EngineMethod("do_full_bake_single_level_automated", false)]
		void DoFullBakeSingleLevelAutomated(string module, string sceneName);

		// Token: 0x060004EC RID: 1260
		[EngineMethod("get_return_code", false)]
		int GetReturnCode();

		// Token: 0x060004ED RID: 1261
		[EngineMethod("do_light_only_bake_single_level_automated", false)]
		void DoLightOnlyBakeSingleLevelAutomated(string module, string sceneName);

		// Token: 0x060004EE RID: 1262
		[EngineMethod("do_light_only_bake_all_levels_automated", false)]
		void DoLightOnlyBakeAllLevelsAutomated(string module, string sceneName);

		// Token: 0x060004EF RID: 1263
		[EngineMethod("get_local_output_dir", false)]
		string GetLocalOutputPath();

		// Token: 0x060004F0 RID: 1264
		[EngineMethod("set_crash_report_custom_string", false)]
		void SetCrashReportCustomString(string customString);

		// Token: 0x060004F1 RID: 1265
		[EngineMethod("set_crash_report_custom_managed_stack", false)]
		void SetCrashReportCustomStack(string customStack);

		// Token: 0x060004F2 RID: 1266
		[EngineMethod("get_steam_appid", false)]
		int GetSteamAppId();

		// Token: 0x060004F3 RID: 1267
		[EngineMethod("set_force_vsync", false)]
		void SetForceVsync(bool value);

		// Token: 0x060004F4 RID: 1268
		[EngineMethod("get_system_language", false)]
		string GetSystemLanguage();

		// Token: 0x060004F5 RID: 1269
		[EngineMethod("managed_parallel_for", false)]
		void ManagedParallelFor(int fromInclusive, int toExclusive, long curKey, int grainSize);

		// Token: 0x060004F6 RID: 1270
		[EngineMethod("get_main_thread_id", false)]
		ulong GetMainThreadId();

		// Token: 0x060004F7 RID: 1271
		[EngineMethod("get_current_thread_id", false)]
		ulong GetCurrentThreadId();

		// Token: 0x060004F8 RID: 1272
		[EngineMethod("register_mesh_for_gpu_morph", false)]
		void RegisterMeshForGPUMorph(string metaMeshName);

		// Token: 0x060004F9 RID: 1273
		[EngineMethod("managed_parallel_for_with_dt", false)]
		void ManagedParallelForWithDt(int fromInclusive, int toExclusive, long curKey, int grainSize);

		// Token: 0x060004FA RID: 1274
		[EngineMethod("clear_shader_memory", false)]
		void ClearShaderMemory();

		// Token: 0x060004FB RID: 1275
		[EngineMethod("open_onscreen_keyboard", false)]
		void OpenOnscreenKeyboard(string initialText, string descriptionText, int maxLength, int keyboardTypeEnum);

		// Token: 0x060004FC RID: 1276
		[EngineMethod("register_gpu_allocation_group", false)]
		int RegisterGPUAllocationGroup(string name);

		// Token: 0x060004FD RID: 1277
		[EngineMethod("get_memory_usage_of_category", false)]
		int GetMemoryUsageOfCategory(int index);

		// Token: 0x060004FE RID: 1278
		[EngineMethod("get_vertex_buffer_chunk_system_memory_usage", false)]
		int GetVertexBufferChunkSystemMemoryUsage();

		// Token: 0x060004FF RID: 1279
		[EngineMethod("get_detailed_xbox_memory_info", false)]
		string GetDetailedXBOXMemoryInfo();
	}
}
