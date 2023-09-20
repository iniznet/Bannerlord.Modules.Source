using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Xml;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x02000092 RID: 146
	public static class Utilities
	{
		// Token: 0x06000AC6 RID: 2758 RVA: 0x0000BD9C File Offset: 0x00009F9C
		public static void ConstructMainThreadJob(Delegate function, params object[] parameters)
		{
			Utilities.MainThreadJob mainThreadJob = new Utilities.MainThreadJob(function, parameters);
			Utilities.jobs.Enqueue(mainThreadJob);
		}

		// Token: 0x06000AC7 RID: 2759 RVA: 0x0000BDBC File Offset: 0x00009FBC
		public static void ConstructMainThreadJob(Semaphore semaphore, Delegate function, params object[] parameters)
		{
			Utilities.MainThreadJob mainThreadJob = new Utilities.MainThreadJob(semaphore, function, parameters);
			Utilities.jobs.Enqueue(mainThreadJob);
		}

		// Token: 0x06000AC8 RID: 2760 RVA: 0x0000BDE0 File Offset: 0x00009FE0
		public static void RunJobs()
		{
			Utilities.MainThreadJob mainThreadJob;
			while (Utilities.jobs.TryDequeue(out mainThreadJob))
			{
				mainThreadJob.Invoke();
			}
		}

		// Token: 0x06000AC9 RID: 2761 RVA: 0x0000BE03 File Offset: 0x0000A003
		public static void WaitJobs()
		{
			while (!Utilities.jobs.IsEmpty)
			{
			}
		}

		// Token: 0x06000ACA RID: 2762 RVA: 0x0000BE11 File Offset: 0x0000A011
		public static void OutputBenchmarkValuesToPerformanceReporter()
		{
			EngineApplicationInterface.IUtil.OutputBenchmarkValuesToPerformanceReporter();
		}

		// Token: 0x06000ACB RID: 2763 RVA: 0x0000BE1D File Offset: 0x0000A01D
		public static void SetLoadingScreenPercentage(float value)
		{
			EngineApplicationInterface.IUtil.SetLoadingScreenPercentage(value);
		}

		// Token: 0x06000ACC RID: 2764 RVA: 0x0000BE2A File Offset: 0x0000A02A
		public static void SetFixedDt(bool enabled, float dt)
		{
			EngineApplicationInterface.IUtil.SetFixedDt(enabled, dt);
		}

		// Token: 0x06000ACD RID: 2765 RVA: 0x0000BE38 File Offset: 0x0000A038
		public static void SetBenchmarkStatus(int status, string def)
		{
			EngineApplicationInterface.IUtil.SetBenchmarkStatus(status, def);
		}

		// Token: 0x06000ACE RID: 2766 RVA: 0x0000BE46 File Offset: 0x0000A046
		public static int GetBenchmarkStatus()
		{
			return EngineApplicationInterface.IUtil.GetBenchmarkStatus();
		}

		// Token: 0x06000ACF RID: 2767 RVA: 0x0000BE52 File Offset: 0x0000A052
		public static string GetApplicationMemoryStatistics()
		{
			return EngineApplicationInterface.IUtil.GetApplicationMemoryStatistics();
		}

		// Token: 0x06000AD0 RID: 2768 RVA: 0x0000BE5E File Offset: 0x0000A05E
		public static bool IsBenchmarkQuited()
		{
			return EngineApplicationInterface.IUtil.IsBenchmarkQuited();
		}

		// Token: 0x06000AD1 RID: 2769 RVA: 0x0000BE6A File Offset: 0x0000A06A
		public static string GetNativeMemoryStatistics()
		{
			return EngineApplicationInterface.IUtil.GetNativeMemoryStatistics();
		}

		// Token: 0x06000AD2 RID: 2770 RVA: 0x0000BE76 File Offset: 0x0000A076
		public static bool CommandLineArgumentExists(string str)
		{
			return EngineApplicationInterface.IUtil.CommandLineArgumentExists(str);
		}

		// Token: 0x06000AD3 RID: 2771 RVA: 0x0000BE83 File Offset: 0x0000A083
		public static string GetConsoleHostMachine()
		{
			return EngineApplicationInterface.IUtil.GetConsoleHostMachine();
		}

		// Token: 0x06000AD4 RID: 2772 RVA: 0x0000BE8F File Offset: 0x0000A08F
		public static string ExportNavMeshFaceMarks(string file_name)
		{
			return EngineApplicationInterface.IUtil.ExportNavMeshFaceMarks(file_name);
		}

		// Token: 0x06000AD5 RID: 2773 RVA: 0x0000BE9C File Offset: 0x0000A09C
		public static string TakeSSFromTop(string file_name)
		{
			return EngineApplicationInterface.IUtil.TakeSSFromTop(file_name);
		}

		// Token: 0x06000AD6 RID: 2774 RVA: 0x0000BEA9 File Offset: 0x0000A0A9
		public static void CheckIfAssetsAndSourcesAreSame()
		{
			EngineApplicationInterface.IUtil.CheckIfAssetsAndSourcesAreSame();
		}

		// Token: 0x06000AD7 RID: 2775 RVA: 0x0000BEB5 File Offset: 0x0000A0B5
		public static void DisableCoreGame()
		{
			EngineApplicationInterface.IUtil.DisableCoreGame();
		}

		// Token: 0x06000AD8 RID: 2776 RVA: 0x0000BEC1 File Offset: 0x0000A0C1
		public static float GetApplicationMemory()
		{
			return EngineApplicationInterface.IUtil.GetApplicationMemory();
		}

		// Token: 0x06000AD9 RID: 2777 RVA: 0x0000BECD File Offset: 0x0000A0CD
		public static void GatherCoreGameReferences(string scene_names)
		{
			EngineApplicationInterface.IUtil.GatherCoreGameReferences(scene_names);
		}

		// Token: 0x06000ADA RID: 2778 RVA: 0x0000BEDA File Offset: 0x0000A0DA
		public static bool IsOnlyCoreContentEnabled()
		{
			return EngineApplicationInterface.IUtil.GetCoreGameState() != 0;
		}

		// Token: 0x06000ADB RID: 2779 RVA: 0x0000BEE9 File Offset: 0x0000A0E9
		public static void GetSnowAmountData(byte[] snowData)
		{
			EngineApplicationInterface.IUtil.GetSnowAmountData(snowData);
		}

		// Token: 0x06000ADC RID: 2780 RVA: 0x0000BEF6 File Offset: 0x0000A0F6
		public static void FindMeshesWithoutLods(string module_name)
		{
			EngineApplicationInterface.IUtil.FindMeshesWithoutLods(module_name);
		}

		// Token: 0x06000ADD RID: 2781 RVA: 0x0000BF03 File Offset: 0x0000A103
		public static void SetDisableDumpGeneration(bool value)
		{
			EngineApplicationInterface.IUtil.SetDisableDumpGeneration(value);
		}

		// Token: 0x06000ADE RID: 2782 RVA: 0x0000BF10 File Offset: 0x0000A110
		public static void SetPrintCallstackAtCrahses(bool value)
		{
			EngineApplicationInterface.IUtil.SetPrintCallstackAtCrahses(value);
		}

		// Token: 0x06000ADF RID: 2783 RVA: 0x0000BF1D File Offset: 0x0000A11D
		public static string[] GetModulesNames()
		{
			return EngineApplicationInterface.IUtil.GetModulesCode().Split(new char[] { '*' });
		}

		// Token: 0x06000AE0 RID: 2784 RVA: 0x0000BF39 File Offset: 0x0000A139
		public static string GetFullModulePath(string moduleName)
		{
			return EngineApplicationInterface.IUtil.GetFullModulePath(moduleName);
		}

		// Token: 0x06000AE1 RID: 2785 RVA: 0x0000BF46 File Offset: 0x0000A146
		public static string[] GetFullModulePaths()
		{
			return EngineApplicationInterface.IUtil.GetFullModulePaths().Split(new char[] { '*' });
		}

		// Token: 0x06000AE2 RID: 2786 RVA: 0x0000BF62 File Offset: 0x0000A162
		public static string GetFullFilePathOfScene(string sceneName)
		{
			string fullFilePathOfScene = EngineApplicationInterface.IUtil.GetFullFilePathOfScene(sceneName);
			if (fullFilePathOfScene == "SCENE_NOT_FOUND")
			{
				throw new Exception("Scene '" + sceneName + "' was not found!");
			}
			return fullFilePathOfScene.Replace("$BASE/", Utilities.GetBasePath());
		}

		// Token: 0x06000AE3 RID: 2787 RVA: 0x0000BFA4 File Offset: 0x0000A1A4
		public static bool TryGetFullFilePathOfScene(string sceneName, out string fullPath)
		{
			bool flag;
			try
			{
				fullPath = Utilities.GetFullFilePathOfScene(sceneName);
				flag = true;
			}
			catch (Exception)
			{
				fullPath = null;
				flag = false;
			}
			return flag;
		}

		// Token: 0x06000AE4 RID: 2788 RVA: 0x0000BFD8 File Offset: 0x0000A1D8
		public static bool TryGetUniqueIdentifiersForScene(string sceneName, out UniqueSceneId identifiers)
		{
			identifiers = null;
			string text;
			return Utilities.TryGetFullFilePathOfScene(sceneName, out text) && Utilities.TryGetUniqueIdentifiersForSceneFile(text, out identifiers);
		}

		// Token: 0x06000AE5 RID: 2789 RVA: 0x0000BFFC File Offset: 0x0000A1FC
		public static bool TryGetUniqueIdentifiersForSceneFile(string xsceneFilePath, out UniqueSceneId identifiers)
		{
			identifiers = null;
			using (XmlReader xmlReader = XmlReader.Create(xsceneFilePath))
			{
				string attribute;
				string attribute2;
				if (xmlReader.MoveToContent() == XmlNodeType.Element && xmlReader.Name == "scene" && (attribute = xmlReader.GetAttribute("unique_token")) != null && (attribute2 = xmlReader.GetAttribute("revision")) != null)
				{
					identifiers = new UniqueSceneId(attribute, attribute2);
				}
			}
			return identifiers != null;
		}

		// Token: 0x06000AE6 RID: 2790 RVA: 0x0000C078 File Offset: 0x0000A278
		public static void PairSceneNameToModuleName(string sceneName, string moduleName)
		{
			EngineApplicationInterface.IUtil.PairSceneNameToModuleName(sceneName, moduleName);
		}

		// Token: 0x06000AE7 RID: 2791 RVA: 0x0000C086 File Offset: 0x0000A286
		public static string[] GetSingleModuleScenesOfModule(string moduleName)
		{
			return EngineApplicationInterface.IUtil.GetSingleModuleScenesOfModule(moduleName).Split(new char[] { '*' });
		}

		// Token: 0x06000AE8 RID: 2792 RVA: 0x0000C0A3 File Offset: 0x0000A2A3
		public static string GetFullCommandLineString()
		{
			return EngineApplicationInterface.IUtil.GetFullCommandLineString();
		}

		// Token: 0x06000AE9 RID: 2793 RVA: 0x0000C0AF File Offset: 0x0000A2AF
		public static void SetScreenTextRenderingState(bool state)
		{
			EngineApplicationInterface.IUtil.SetScreenTextRenderingState(state);
		}

		// Token: 0x06000AEA RID: 2794 RVA: 0x0000C0BC File Offset: 0x0000A2BC
		public static void SetMessageLineRenderingState(bool state)
		{
			EngineApplicationInterface.IUtil.SetMessageLineRenderingState(state);
		}

		// Token: 0x06000AEB RID: 2795 RVA: 0x0000C0C9 File Offset: 0x0000A2C9
		public static bool CheckIfTerrainShaderHeaderGenerationFinished()
		{
			return EngineApplicationInterface.IUtil.CheckIfTerrainShaderHeaderGenerationFinished();
		}

		// Token: 0x06000AEC RID: 2796 RVA: 0x0000C0D5 File Offset: 0x0000A2D5
		public static void GenerateTerrainShaderHeaders(string targetPlatform, string targetConfig, string output_path)
		{
			EngineApplicationInterface.IUtil.GenerateTerrainShaderHeaders(targetPlatform, targetConfig, output_path);
		}

		// Token: 0x06000AED RID: 2797 RVA: 0x0000C0E4 File Offset: 0x0000A2E4
		public static void CompileTerrainShadersDist(string targetPlatform, string targetConfig, string output_path)
		{
			EngineApplicationInterface.IUtil.CompileTerrainShadersDist(targetPlatform, targetConfig, output_path);
		}

		// Token: 0x06000AEE RID: 2798 RVA: 0x0000C0F3 File Offset: 0x0000A2F3
		public static void SetCrashOnAsserts(bool val)
		{
			EngineApplicationInterface.IUtil.SetCrashOnAsserts(val);
		}

		// Token: 0x06000AEF RID: 2799 RVA: 0x0000C100 File Offset: 0x0000A300
		public static void SetCrashOnWarnings(bool val)
		{
			EngineApplicationInterface.IUtil.SetCrashOnWarnings(val);
		}

		// Token: 0x06000AF0 RID: 2800 RVA: 0x0000C10D File Offset: 0x0000A30D
		public static void ToggleRender()
		{
			EngineApplicationInterface.IUtil.ToggleRender();
		}

		// Token: 0x06000AF1 RID: 2801 RVA: 0x0000C119 File Offset: 0x0000A319
		public static void SetRenderAgents(bool value)
		{
			EngineApplicationInterface.IUtil.SetRenderAgents(value);
		}

		// Token: 0x06000AF2 RID: 2802 RVA: 0x0000C126 File Offset: 0x0000A326
		public static bool CheckShaderCompilation()
		{
			return EngineApplicationInterface.IUtil.CheckShaderCompilation();
		}

		// Token: 0x06000AF3 RID: 2803 RVA: 0x0000C132 File Offset: 0x0000A332
		public static void CompileAllShaders(string targetPlatform)
		{
			EngineApplicationInterface.IUtil.CompileAllShaders(targetPlatform);
		}

		// Token: 0x06000AF4 RID: 2804 RVA: 0x0000C13F File Offset: 0x0000A33F
		public static string GetExecutableWorkingDirectory()
		{
			return EngineApplicationInterface.IUtil.GetExecutableWorkingDirectory();
		}

		// Token: 0x06000AF5 RID: 2805 RVA: 0x0000C14B File Offset: 0x0000A34B
		public static void SetDumpFolderPath(string path)
		{
			EngineApplicationInterface.IUtil.SetDumpFolderPath(path);
		}

		// Token: 0x06000AF6 RID: 2806 RVA: 0x0000C158 File Offset: 0x0000A358
		public static void CheckSceneForProblems(string sceneName)
		{
			EngineApplicationInterface.IUtil.CheckSceneForProblems(sceneName);
		}

		// Token: 0x06000AF7 RID: 2807 RVA: 0x0000C165 File Offset: 0x0000A365
		public static void SetCoreGameState(int state)
		{
			EngineApplicationInterface.IUtil.SetCoreGameState(state);
		}

		// Token: 0x06000AF8 RID: 2808 RVA: 0x0000C172 File Offset: 0x0000A372
		public static int GetCoreGameState()
		{
			return EngineApplicationInterface.IUtil.GetCoreGameState();
		}

		// Token: 0x06000AF9 RID: 2809 RVA: 0x0000C17E File Offset: 0x0000A37E
		public static string ExecuteCommandLineCommand(string command)
		{
			return EngineApplicationInterface.IUtil.ExecuteCommandLineCommand(command);
		}

		// Token: 0x06000AFA RID: 2810 RVA: 0x0000C18B File Offset: 0x0000A38B
		public static void QuitGame()
		{
			EngineApplicationInterface.IUtil.QuitGame();
		}

		// Token: 0x06000AFB RID: 2811 RVA: 0x0000C197 File Offset: 0x0000A397
		public static void ExitProcess(int exitCode)
		{
			EngineApplicationInterface.IUtil.ExitProcess(exitCode);
		}

		// Token: 0x06000AFC RID: 2812 RVA: 0x0000C1A4 File Offset: 0x0000A3A4
		public static string GetBasePath()
		{
			return EngineApplicationInterface.IUtil.GetBaseDirectory();
		}

		// Token: 0x06000AFD RID: 2813 RVA: 0x0000C1B0 File Offset: 0x0000A3B0
		public static string GetVisualTestsValidatePath()
		{
			return EngineApplicationInterface.IUtil.GetVisualTestsValidatePath();
		}

		// Token: 0x06000AFE RID: 2814 RVA: 0x0000C1BC File Offset: 0x0000A3BC
		public static string GetVisualTestsTestFilesPath()
		{
			return EngineApplicationInterface.IUtil.GetVisualTestsTestFilesPath();
		}

		// Token: 0x06000AFF RID: 2815 RVA: 0x0000C1C8 File Offset: 0x0000A3C8
		public static string GetAttachmentsPath()
		{
			return EngineApplicationInterface.IUtil.GetAttachmentsPath();
		}

		// Token: 0x06000B00 RID: 2816 RVA: 0x0000C1D4 File Offset: 0x0000A3D4
		public static void StartScenePerformanceReport(string folderPath)
		{
			EngineApplicationInterface.IUtil.StartScenePerformanceReport(folderPath);
		}

		// Token: 0x06000B01 RID: 2817 RVA: 0x0000C1E1 File Offset: 0x0000A3E1
		public static bool IsSceneReportFinished()
		{
			return EngineApplicationInterface.IUtil.IsSceneReportFinished();
		}

		// Token: 0x06000B02 RID: 2818 RVA: 0x0000C1ED File Offset: 0x0000A3ED
		public static float GetFps()
		{
			return EngineApplicationInterface.IUtil.GetFps();
		}

		// Token: 0x06000B03 RID: 2819 RVA: 0x0000C1F9 File Offset: 0x0000A3F9
		public static float GetMainFps()
		{
			return EngineApplicationInterface.IUtil.GetMainFps();
		}

		// Token: 0x06000B04 RID: 2820 RVA: 0x0000C205 File Offset: 0x0000A405
		public static float GetRendererFps()
		{
			return EngineApplicationInterface.IUtil.GetRendererFps();
		}

		// Token: 0x06000B05 RID: 2821 RVA: 0x0000C211 File Offset: 0x0000A411
		public static void EnableSingleGPUQueryPerFrame()
		{
			EngineApplicationInterface.IUtil.EnableSingleGPUQueryPerFrame();
		}

		// Token: 0x06000B06 RID: 2822 RVA: 0x0000C21D File Offset: 0x0000A41D
		public static void ClearDecalAtlas(DecalAtlasGroup atlasGroup)
		{
			EngineApplicationInterface.IUtil.clear_decal_atlas(atlasGroup);
		}

		// Token: 0x06000B07 RID: 2823 RVA: 0x0000C22A File Offset: 0x0000A42A
		public static void FlushManagedObjectsMemory()
		{
			Common.MemoryCleanupGC(false);
		}

		// Token: 0x06000B08 RID: 2824 RVA: 0x0000C232 File Offset: 0x0000A432
		public static void OnLoadingWindowEnabled()
		{
			EngineApplicationInterface.IUtil.OnLoadingWindowEnabled();
		}

		// Token: 0x06000B09 RID: 2825 RVA: 0x0000C23E File Offset: 0x0000A43E
		public static void DebugSetGlobalLoadingWindowState(bool newState)
		{
			EngineApplicationInterface.IUtil.DebugSetGlobalLoadingWindowState(newState);
		}

		// Token: 0x06000B0A RID: 2826 RVA: 0x0000C24B File Offset: 0x0000A44B
		public static void OnLoadingWindowDisabled()
		{
			EngineApplicationInterface.IUtil.OnLoadingWindowDisabled();
		}

		// Token: 0x06000B0B RID: 2827 RVA: 0x0000C257 File Offset: 0x0000A457
		public static void DisableGlobalLoadingWindow()
		{
			EngineApplicationInterface.IUtil.DisableGlobalLoadingWindow();
		}

		// Token: 0x06000B0C RID: 2828 RVA: 0x0000C263 File Offset: 0x0000A463
		public static void EnableGlobalLoadingWindow()
		{
			EngineApplicationInterface.IUtil.EnableGlobalLoadingWindow();
		}

		// Token: 0x06000B0D RID: 2829 RVA: 0x0000C26F File Offset: 0x0000A46F
		public static void EnableGlobalEditDataCacher()
		{
			EngineApplicationInterface.IUtil.EnableGlobalEditDataCacher();
		}

		// Token: 0x06000B0E RID: 2830 RVA: 0x0000C27B File Offset: 0x0000A47B
		public static void DoFullBakeAllLevelsAutomated(string module, string scene)
		{
			EngineApplicationInterface.IUtil.DoFullBakeAllLevelsAutomated(module, scene);
		}

		// Token: 0x06000B0F RID: 2831 RVA: 0x0000C289 File Offset: 0x0000A489
		public static int GetReturnCode()
		{
			return EngineApplicationInterface.IUtil.GetReturnCode();
		}

		// Token: 0x06000B10 RID: 2832 RVA: 0x0000C295 File Offset: 0x0000A495
		public static void DisableGlobalEditDataCacher()
		{
			EngineApplicationInterface.IUtil.DisableGlobalEditDataCacher();
		}

		// Token: 0x06000B11 RID: 2833 RVA: 0x0000C2A1 File Offset: 0x0000A4A1
		public static void DoFullBakeSingleLevelAutomated(string module, string scene)
		{
			EngineApplicationInterface.IUtil.DoFullBakeSingleLevelAutomated(module, scene);
		}

		// Token: 0x06000B12 RID: 2834 RVA: 0x0000C2AF File Offset: 0x0000A4AF
		public static void DoLightOnlyBakeSingleLevelAutomated(string module, string scene)
		{
			EngineApplicationInterface.IUtil.DoLightOnlyBakeSingleLevelAutomated(module, scene);
		}

		// Token: 0x06000B13 RID: 2835 RVA: 0x0000C2BD File Offset: 0x0000A4BD
		public static void DoLightOnlyBakeAllLevelsAutomated(string module, string scene)
		{
			EngineApplicationInterface.IUtil.DoLightOnlyBakeAllLevelsAutomated(module, scene);
		}

		// Token: 0x06000B14 RID: 2836 RVA: 0x0000C2CB File Offset: 0x0000A4CB
		public static bool DidAutomatedGIBakeFinished()
		{
			return EngineApplicationInterface.IUtil.DidAutomatedGIBakeFinished();
		}

		// Token: 0x06000B15 RID: 2837 RVA: 0x0000C2D8 File Offset: 0x0000A4D8
		public static void GetSelectedEntities(ref List<GameEntity> gameEntities)
		{
			int editorSelectedEntityCount = EngineApplicationInterface.IUtil.GetEditorSelectedEntityCount();
			UIntPtr[] array = new UIntPtr[editorSelectedEntityCount];
			EngineApplicationInterface.IUtil.GetEditorSelectedEntities(array);
			for (int i = 0; i < editorSelectedEntityCount; i++)
			{
				gameEntities.Add(new GameEntity(array[i]));
			}
		}

		// Token: 0x06000B16 RID: 2838 RVA: 0x0000C320 File Offset: 0x0000A520
		public static void DeleteEntitiesInEditorScene(List<GameEntity> gameEntities)
		{
			int count = gameEntities.Count;
			UIntPtr[] array = new UIntPtr[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = gameEntities[i].Pointer;
			}
			EngineApplicationInterface.IUtil.DeleteEntitiesInEditorScene(array, count);
		}

		// Token: 0x06000B17 RID: 2839 RVA: 0x0000C364 File Offset: 0x0000A564
		public static void CreateSelectionInEditor(List<GameEntity> gameEntities, string name)
		{
			int count = gameEntities.Count;
			UIntPtr[] array = new UIntPtr[gameEntities.Count];
			for (int i = 0; i < count; i++)
			{
				array[i] = gameEntities[i].Pointer;
			}
			EngineApplicationInterface.IUtil.CreateSelectionInEditor(array, count, name);
		}

		// Token: 0x06000B18 RID: 2840 RVA: 0x0000C3AC File Offset: 0x0000A5AC
		public static void SelectEntities(List<GameEntity> gameEntities)
		{
			int count = gameEntities.Count;
			UIntPtr[] array = new UIntPtr[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = gameEntities[i].Pointer;
			}
			EngineApplicationInterface.IUtil.SelectEntities(array, count);
		}

		// Token: 0x06000B19 RID: 2841 RVA: 0x0000C3F0 File Offset: 0x0000A5F0
		public static void GetEntitiesOfSelectionSet(string selectionSetName, ref List<GameEntity> gameEntities)
		{
			int entityCountOfSelectionSet = EngineApplicationInterface.IUtil.GetEntityCountOfSelectionSet(selectionSetName);
			UIntPtr[] array = new UIntPtr[entityCountOfSelectionSet];
			EngineApplicationInterface.IUtil.GetEntitiesOfSelectionSet(selectionSetName, array);
			for (int i = 0; i < entityCountOfSelectionSet; i++)
			{
				gameEntities.Add(new GameEntity(array[i]));
			}
		}

		// Token: 0x06000B1A RID: 2842 RVA: 0x0000C437 File Offset: 0x0000A637
		public static void AddCommandLineFunction(string concatName)
		{
			EngineApplicationInterface.IUtil.AddCommandLineFunction(concatName);
		}

		// Token: 0x06000B1B RID: 2843 RVA: 0x0000C444 File Offset: 0x0000A644
		public static int GetNumberOfShaderCompilationsInProgress()
		{
			return EngineApplicationInterface.IUtil.GetNumberOfShaderCompilationsInProgress();
		}

		// Token: 0x06000B1C RID: 2844 RVA: 0x0000C450 File Offset: 0x0000A650
		public static int IsDetailedSoundLogOn()
		{
			return EngineApplicationInterface.IUtil.IsDetailedSoundLogOn();
		}

		// Token: 0x06000B1D RID: 2845 RVA: 0x0000C45C File Offset: 0x0000A65C
		public static ulong GetCurrentCpuMemoryUsageMB()
		{
			return EngineApplicationInterface.IUtil.GetCurrentCpuMemoryUsage();
		}

		// Token: 0x06000B1E RID: 2846 RVA: 0x0000C468 File Offset: 0x0000A668
		public static ulong GetGpuMemoryOfAllocationGroup(string name)
		{
			return EngineApplicationInterface.IUtil.GetGpuMemoryOfAllocationGroup(name);
		}

		// Token: 0x06000B1F RID: 2847 RVA: 0x0000C475 File Offset: 0x0000A675
		public static void GetGPUMemoryStats(ref float totalMemory, ref float renderTargetMemory, ref float depthTargetMemory, ref float srvMemory, ref float bufferMemory)
		{
			EngineApplicationInterface.IUtil.GetGPUMemoryStats(ref totalMemory, ref renderTargetMemory, ref depthTargetMemory, ref srvMemory, ref bufferMemory);
		}

		// Token: 0x06000B20 RID: 2848 RVA: 0x0000C487 File Offset: 0x0000A687
		public static void GetDetailedGPUMemoryData(ref int totalMemoryAllocated, ref int totalMemoryUsed, ref int emptyChunkTotalSize)
		{
			EngineApplicationInterface.IUtil.GetDetailedGPUBufferMemoryStats(ref totalMemoryAllocated, ref totalMemoryUsed, ref emptyChunkTotalSize);
		}

		// Token: 0x06000B21 RID: 2849 RVA: 0x0000C496 File Offset: 0x0000A696
		public static void SetRenderMode(Utilities.EngineRenderDisplayMode mode)
		{
			EngineApplicationInterface.IUtil.SetRenderMode((int)mode);
		}

		// Token: 0x06000B22 RID: 2850 RVA: 0x0000C4A3 File Offset: 0x0000A6A3
		public static void SetForceDrawEntityID(bool value)
		{
			EngineApplicationInterface.IUtil.SetForceDrawEntityID(value);
		}

		// Token: 0x06000B23 RID: 2851 RVA: 0x0000C4B0 File Offset: 0x0000A6B0
		public static void AddPerformanceReportToken(string performance_type, string name, float loading_time)
		{
			EngineApplicationInterface.IUtil.AddPerformanceReportToken(performance_type, name, loading_time);
		}

		// Token: 0x06000B24 RID: 2852 RVA: 0x0000C4BF File Offset: 0x0000A6BF
		public static void AddSceneObjectReport(string scene_name, string report_name, float report_value)
		{
			EngineApplicationInterface.IUtil.AddSceneObjectReport(scene_name, report_name, report_value);
		}

		// Token: 0x06000B25 RID: 2853 RVA: 0x0000C4CE File Offset: 0x0000A6CE
		public static void OutputPerformanceReports()
		{
			EngineApplicationInterface.IUtil.OutputPerformanceReports();
		}

		// Token: 0x1700008A RID: 138
		// (get) Token: 0x06000B26 RID: 2854 RVA: 0x0000C4DA File Offset: 0x0000A6DA
		public static int EngineFrameNo
		{
			get
			{
				return EngineApplicationInterface.IUtil.GetEngineFrameNo();
			}
		}

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x06000B27 RID: 2855 RVA: 0x0000C4E6 File Offset: 0x0000A6E6
		public static bool EditModeEnabled
		{
			get
			{
				return EngineApplicationInterface.IUtil.IsEditModeEnabled();
			}
		}

		// Token: 0x06000B28 RID: 2856 RVA: 0x0000C4F2 File Offset: 0x0000A6F2
		public static void TakeScreenshot(PlatformFilePath path)
		{
			EngineApplicationInterface.IUtil.TakeScreenshotFromPlatformPath(path);
		}

		// Token: 0x06000B29 RID: 2857 RVA: 0x0000C4FF File Offset: 0x0000A6FF
		public static void TakeScreenshot(string path)
		{
			EngineApplicationInterface.IUtil.TakeScreenshotFromStringPath(path);
		}

		// Token: 0x06000B2A RID: 2858 RVA: 0x0000C50C File Offset: 0x0000A70C
		public static void SetAllocationAlwaysValidScene(Scene scene)
		{
			EngineApplicationInterface.IUtil.SetAllocationAlwaysValidScene((scene != null) ? scene.Pointer : UIntPtr.Zero);
		}

		// Token: 0x06000B2B RID: 2859 RVA: 0x0000C52E File Offset: 0x0000A72E
		public static void CheckResourceModifications()
		{
			EngineApplicationInterface.IUtil.CheckResourceModifications();
		}

		// Token: 0x06000B2C RID: 2860 RVA: 0x0000C53A File Offset: 0x0000A73A
		public static void SetGraphicsPreset(int preset)
		{
			EngineApplicationInterface.IUtil.SetGraphicsPreset(preset);
		}

		// Token: 0x06000B2D RID: 2861 RVA: 0x0000C547 File Offset: 0x0000A747
		public static string GetLocalOutputPath()
		{
			return EngineApplicationInterface.IUtil.GetLocalOutputPath();
		}

		// Token: 0x06000B2E RID: 2862 RVA: 0x0000C553 File Offset: 0x0000A753
		public static string GetPCInfo()
		{
			return EngineApplicationInterface.IUtil.GetPCInfo();
		}

		// Token: 0x06000B2F RID: 2863 RVA: 0x0000C55F File Offset: 0x0000A75F
		public static int GetGPUMemoryMB()
		{
			return EngineApplicationInterface.IUtil.GetGPUMemoryMB();
		}

		// Token: 0x06000B30 RID: 2864 RVA: 0x0000C56B File Offset: 0x0000A76B
		public static int GetCurrentEstimatedGPUMemoryCostMB()
		{
			return EngineApplicationInterface.IUtil.GetCurrentEstimatedGPUMemoryCostMB();
		}

		// Token: 0x06000B31 RID: 2865 RVA: 0x0000C577 File Offset: 0x0000A777
		public static void DumpGPUMemoryStatistics(string filePath)
		{
			EngineApplicationInterface.IUtil.DumpGPUMemoryStatistics(filePath);
		}

		// Token: 0x06000B32 RID: 2866 RVA: 0x0000C584 File Offset: 0x0000A784
		public static int SaveDataAsTexture(string path, int width, int height, float[] data)
		{
			return EngineApplicationInterface.IUtil.SaveDataAsTexture(path, width, height, data);
		}

		// Token: 0x06000B33 RID: 2867 RVA: 0x0000C594 File Offset: 0x0000A794
		public static void ClearOldResourcesAndObjects()
		{
			EngineApplicationInterface.IUtil.ClearOldResourcesAndObjects();
		}

		// Token: 0x06000B34 RID: 2868 RVA: 0x0000C5A0 File Offset: 0x0000A7A0
		public static void LoadVirtualTextureTileset(string name)
		{
			EngineApplicationInterface.IUtil.LoadVirtualTextureTileset(name);
		}

		// Token: 0x06000B35 RID: 2869 RVA: 0x0000C5AD File Offset: 0x0000A7AD
		public static float GetDeltaTime(int timerId)
		{
			return EngineApplicationInterface.IUtil.GetDeltaTime(timerId);
		}

		// Token: 0x06000B36 RID: 2870 RVA: 0x0000C5BA File Offset: 0x0000A7BA
		public static void LoadSkyBoxes()
		{
			EngineApplicationInterface.IUtil.LoadSkyBoxes();
		}

		// Token: 0x06000B37 RID: 2871 RVA: 0x0000C5C6 File Offset: 0x0000A7C6
		public static string GetApplicationName()
		{
			return EngineApplicationInterface.IUtil.GetApplicationName();
		}

		// Token: 0x06000B38 RID: 2872 RVA: 0x0000C5D2 File Offset: 0x0000A7D2
		public static void SetWindowTitle(string title)
		{
			EngineApplicationInterface.IUtil.SetWindowTitle(title);
		}

		// Token: 0x06000B39 RID: 2873 RVA: 0x0000C5DF File Offset: 0x0000A7DF
		public static string ProcessWindowTitle(string title)
		{
			return EngineApplicationInterface.IUtil.ProcessWindowTitle(title);
		}

		// Token: 0x06000B3A RID: 2874 RVA: 0x0000C5EC File Offset: 0x0000A7EC
		public static uint GetCurrentProcessID()
		{
			return EngineApplicationInterface.IUtil.GetCurrentProcessID();
		}

		// Token: 0x06000B3B RID: 2875 RVA: 0x0000C5F8 File Offset: 0x0000A7F8
		public static void DoDelayedexit(int returnCode)
		{
			EngineApplicationInterface.IUtil.DoDelayedexit(returnCode);
		}

		// Token: 0x06000B3C RID: 2876 RVA: 0x0000C605 File Offset: 0x0000A805
		public static void SetAssertionsAndWarningsSetExitCode(bool value)
		{
			EngineApplicationInterface.IUtil.SetAssertionsAndWarningsSetExitCode(value);
		}

		// Token: 0x06000B3D RID: 2877 RVA: 0x0000C612 File Offset: 0x0000A812
		public static void SetReportMode(bool reportMode)
		{
			EngineApplicationInterface.IUtil.SetReportMode(reportMode);
		}

		// Token: 0x06000B3E RID: 2878 RVA: 0x0000C61F File Offset: 0x0000A81F
		public static void SetAssertionAtShaderCompile(bool value)
		{
			EngineApplicationInterface.IUtil.SetAssertionAtShaderCompile(value);
		}

		// Token: 0x06000B3F RID: 2879 RVA: 0x0000C62C File Offset: 0x0000A82C
		public static void SetCrashReportCustomString(string customString)
		{
			EngineApplicationInterface.IUtil.SetCrashReportCustomString(customString);
		}

		// Token: 0x06000B40 RID: 2880 RVA: 0x0000C639 File Offset: 0x0000A839
		public static void SetCrashReportCustomStack(string customStack)
		{
			EngineApplicationInterface.IUtil.SetCrashReportCustomStack(customStack);
		}

		// Token: 0x06000B41 RID: 2881 RVA: 0x0000C646 File Offset: 0x0000A846
		public static int GetSteamAppId()
		{
			return EngineApplicationInterface.IUtil.GetSteamAppId();
		}

		// Token: 0x06000B42 RID: 2882 RVA: 0x0000C652 File Offset: 0x0000A852
		public static void SetForceVsync(bool value)
		{
			Debug.Print("Force VSync State is now " + (value ? "ACTIVE" : "DEACTIVATED"), 0, Debug.DebugColor.DarkBlue, 17592186044416UL);
			EngineApplicationInterface.IUtil.SetForceVsync(value);
		}

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x06000B43 RID: 2883 RVA: 0x0000C688 File Offset: 0x0000A888
		private static PlatformFilePath DefaultBannerlordConfigFullPath
		{
			get
			{
				return new PlatformFilePath(EngineFilePaths.ConfigsPath, "BannerlordConfig.txt");
			}
		}

		// Token: 0x06000B44 RID: 2884 RVA: 0x0000C69C File Offset: 0x0000A89C
		public static string LoadBannerlordConfigFile()
		{
			PlatformFilePath defaultBannerlordConfigFullPath = Utilities.DefaultBannerlordConfigFullPath;
			if (!FileHelper.FileExists(defaultBannerlordConfigFullPath))
			{
				return "";
			}
			return FileHelper.GetFileContentString(defaultBannerlordConfigFullPath);
		}

		// Token: 0x06000B45 RID: 2885 RVA: 0x0000C6C4 File Offset: 0x0000A8C4
		public static SaveResult SaveConfigFile(string configProperties)
		{
			PlatformFilePath defaultBannerlordConfigFullPath = Utilities.DefaultBannerlordConfigFullPath;
			SaveResult saveResult;
			try
			{
				string text = configProperties.Substring(0, configProperties.Length - 1);
				FileHelper.SaveFileString(defaultBannerlordConfigFullPath, text);
				saveResult = SaveResult.Success;
			}
			catch
			{
				Debug.Print("Could not create Bannerlord Config file", 0, Debug.DebugColor.White, 17592186044416UL);
				saveResult = SaveResult.ConfigFileFailure;
			}
			return saveResult;
		}

		// Token: 0x06000B46 RID: 2886 RVA: 0x0000C720 File Offset: 0x0000A920
		public static void OpenOnscreenKeyboard(string initialText, string descriptionText, int maxLength, int keyboardTypeEnum)
		{
			EngineApplicationInterface.IUtil.OpenOnscreenKeyboard(initialText, descriptionText, maxLength, keyboardTypeEnum);
		}

		// Token: 0x06000B47 RID: 2887 RVA: 0x0000C730 File Offset: 0x0000A930
		public static string GetSystemLanguage()
		{
			return EngineApplicationInterface.IUtil.GetSystemLanguage();
		}

		// Token: 0x06000B48 RID: 2888 RVA: 0x0000C73C File Offset: 0x0000A93C
		public static int RegisterGPUAllocationGroup(string name)
		{
			return EngineApplicationInterface.IUtil.RegisterGPUAllocationGroup(name);
		}

		// Token: 0x06000B49 RID: 2889 RVA: 0x0000C749 File Offset: 0x0000A949
		public static int GetMemoryUsageOfCategory(int category)
		{
			return EngineApplicationInterface.IUtil.GetMemoryUsageOfCategory(category);
		}

		// Token: 0x06000B4A RID: 2890 RVA: 0x0000C756 File Offset: 0x0000A956
		public static string GetDetailedXBOXMemoryInfo()
		{
			return EngineApplicationInterface.IUtil.GetDetailedXBOXMemoryInfo();
		}

		// Token: 0x06000B4B RID: 2891 RVA: 0x0000C762 File Offset: 0x0000A962
		public static int GetVertexBufferChunkSystemMemoryUsage()
		{
			return EngineApplicationInterface.IUtil.GetVertexBufferChunkSystemMemoryUsage();
		}

		// Token: 0x06000B4C RID: 2892 RVA: 0x0000C76E File Offset: 0x0000A96E
		public static int GetBuildNumber()
		{
			return EngineApplicationInterface.IUtil.GetBuildNumber();
		}

		// Token: 0x06000B4D RID: 2893 RVA: 0x0000C77A File Offset: 0x0000A97A
		public static ApplicationVersion GetApplicationVersionWithBuildNumber()
		{
			return ApplicationVersion.FromParametersFile(null);
		}

		// Token: 0x06000B4E RID: 2894 RVA: 0x0000C782 File Offset: 0x0000A982
		public static void ParallelFor(int startIndex, int endIndex, long curKey, int grainSize)
		{
			EngineApplicationInterface.IUtil.ManagedParallelFor(startIndex, endIndex, curKey, grainSize);
		}

		// Token: 0x06000B4F RID: 2895 RVA: 0x0000C792 File Offset: 0x0000A992
		public static void ClearShaderMemory()
		{
			EngineApplicationInterface.IUtil.ClearShaderMemory();
		}

		// Token: 0x06000B50 RID: 2896 RVA: 0x0000C79E File Offset: 0x0000A99E
		public static void RegisterMeshForGPUMorph(string metaMeshName)
		{
			EngineApplicationInterface.IUtil.RegisterMeshForGPUMorph(metaMeshName);
		}

		// Token: 0x06000B51 RID: 2897 RVA: 0x0000C7AB File Offset: 0x0000A9AB
		public static void ParallelForWithDt(int startIndex, int endIndex, long curKey, int grainSize)
		{
			EngineApplicationInterface.IUtil.ManagedParallelForWithDt(startIndex, endIndex, curKey, grainSize);
		}

		// Token: 0x06000B52 RID: 2898 RVA: 0x0000C7BB File Offset: 0x0000A9BB
		public static ulong GetMainThreadId()
		{
			return EngineApplicationInterface.IUtil.GetMainThreadId();
		}

		// Token: 0x06000B53 RID: 2899 RVA: 0x0000C7C7 File Offset: 0x0000A9C7
		public static ulong GetCurrentThreadId()
		{
			return EngineApplicationInterface.IUtil.GetCurrentThreadId();
		}

		// Token: 0x040001E0 RID: 480
		private static ConcurrentQueue<Utilities.MainThreadJob> jobs = new ConcurrentQueue<Utilities.MainThreadJob>();

		// Token: 0x040001E1 RID: 481
		public static bool renderingActive = true;

		// Token: 0x020000C6 RID: 198
		public enum EngineRenderDisplayMode
		{
			// Token: 0x0400040D RID: 1037
			ShowNone,
			// Token: 0x0400040E RID: 1038
			ShowAlbedo,
			// Token: 0x0400040F RID: 1039
			ShowNormals,
			// Token: 0x04000410 RID: 1040
			ShowVertexNormals,
			// Token: 0x04000411 RID: 1041
			ShowSpecular,
			// Token: 0x04000412 RID: 1042
			ShowGloss,
			// Token: 0x04000413 RID: 1043
			ShowOcclusion,
			// Token: 0x04000414 RID: 1044
			ShowGbufferShadowMask,
			// Token: 0x04000415 RID: 1045
			ShowTranslucency,
			// Token: 0x04000416 RID: 1046
			ShowMotionVector,
			// Token: 0x04000417 RID: 1047
			ShowVertexColor,
			// Token: 0x04000418 RID: 1048
			ShowDepth,
			// Token: 0x04000419 RID: 1049
			ShowTiledLightOverdraw,
			// Token: 0x0400041A RID: 1050
			ShowTiledDecalOverdraw,
			// Token: 0x0400041B RID: 1051
			ShowMeshId,
			// Token: 0x0400041C RID: 1052
			ShowDisableSunLighting,
			// Token: 0x0400041D RID: 1053
			ShowDebugTexture,
			// Token: 0x0400041E RID: 1054
			ShowTextureDensity,
			// Token: 0x0400041F RID: 1055
			ShowOverdraw,
			// Token: 0x04000420 RID: 1056
			ShowVsComplexity,
			// Token: 0x04000421 RID: 1057
			ShowPsComplexity,
			// Token: 0x04000422 RID: 1058
			ShowDisableAmbientLighting,
			// Token: 0x04000423 RID: 1059
			ShowEntityId,
			// Token: 0x04000424 RID: 1060
			ShowPrtDiffuseAmbient,
			// Token: 0x04000425 RID: 1061
			ShowLightDebugMode,
			// Token: 0x04000426 RID: 1062
			ShowParticleShadingAtlas,
			// Token: 0x04000427 RID: 1063
			ShowTerrainAngle,
			// Token: 0x04000428 RID: 1064
			ShowParallaxDebug,
			// Token: 0x04000429 RID: 1065
			ShowAlbedoValidation,
			// Token: 0x0400042A RID: 1066
			NumDebugModes
		}

		// Token: 0x020000C7 RID: 199
		private class MainThreadJob
		{
			// Token: 0x06000C74 RID: 3188 RVA: 0x0000F6E7 File Offset: 0x0000D8E7
			internal MainThreadJob(Delegate function, object[] parameters)
			{
				this._function = function;
				this._parameters = parameters;
				this.wait_handle = null;
			}

			// Token: 0x06000C75 RID: 3189 RVA: 0x0000F704 File Offset: 0x0000D904
			internal MainThreadJob(Semaphore sema, Delegate function, object[] parameters)
			{
				this._function = function;
				this._parameters = parameters;
				this.wait_handle = sema;
			}

			// Token: 0x06000C76 RID: 3190 RVA: 0x0000F721 File Offset: 0x0000D921
			internal void Invoke()
			{
				this._function.DynamicInvoke(this._parameters);
				if (this.wait_handle != null)
				{
					this.wait_handle.Release();
				}
			}

			// Token: 0x0400042B RID: 1067
			private Delegate _function;

			// Token: 0x0400042C RID: 1068
			private object[] _parameters;

			// Token: 0x0400042D RID: 1069
			private Semaphore wait_handle;
		}

		// Token: 0x020000C8 RID: 200
		public class MainThreadPerformanceQuery : IDisposable
		{
			// Token: 0x06000C77 RID: 3191 RVA: 0x0000F749 File Offset: 0x0000D949
			public MainThreadPerformanceQuery(string parent, string name)
			{
				this._name = name;
				this._parent = parent;
				this._stopWatch = new Stopwatch();
				this._stopWatch.Start();
			}

			// Token: 0x06000C78 RID: 3192 RVA: 0x0000F778 File Offset: 0x0000D978
			public void Dispose()
			{
				this._stopWatch.Stop();
				float num = (float)this._stopWatch.Elapsed.TotalMilliseconds;
				num /= 1000f;
				EngineApplicationInterface.IUtil.AddMainThreadPerformanceQuery(this._parent, this._name, num);
			}

			// Token: 0x0400042E RID: 1070
			private string _name;

			// Token: 0x0400042F RID: 1071
			private string _parent;

			// Token: 0x04000430 RID: 1072
			private Stopwatch _stopWatch;
		}
	}
}
