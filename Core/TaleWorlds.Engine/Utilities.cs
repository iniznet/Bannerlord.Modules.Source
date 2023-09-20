﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Xml;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	public static class Utilities
	{
		public static void ConstructMainThreadJob(Delegate function, params object[] parameters)
		{
			Utilities.MainThreadJob mainThreadJob = new Utilities.MainThreadJob(function, parameters);
			Utilities.jobs.Enqueue(mainThreadJob);
		}

		public static void ConstructMainThreadJob(Semaphore semaphore, Delegate function, params object[] parameters)
		{
			Utilities.MainThreadJob mainThreadJob = new Utilities.MainThreadJob(semaphore, function, parameters);
			Utilities.jobs.Enqueue(mainThreadJob);
		}

		public static void RunJobs()
		{
			Utilities.MainThreadJob mainThreadJob;
			while (Utilities.jobs.TryDequeue(out mainThreadJob))
			{
				mainThreadJob.Invoke();
			}
		}

		public static void WaitJobs()
		{
			while (!Utilities.jobs.IsEmpty)
			{
			}
		}

		public static void OutputBenchmarkValuesToPerformanceReporter()
		{
			EngineApplicationInterface.IUtil.OutputBenchmarkValuesToPerformanceReporter();
		}

		public static void SetLoadingScreenPercentage(float value)
		{
			EngineApplicationInterface.IUtil.SetLoadingScreenPercentage(value);
		}

		public static void SetFixedDt(bool enabled, float dt)
		{
			EngineApplicationInterface.IUtil.SetFixedDt(enabled, dt);
		}

		public static void SetBenchmarkStatus(int status, string def)
		{
			EngineApplicationInterface.IUtil.SetBenchmarkStatus(status, def);
		}

		public static int GetBenchmarkStatus()
		{
			return EngineApplicationInterface.IUtil.GetBenchmarkStatus();
		}

		public static string GetApplicationMemoryStatistics()
		{
			return EngineApplicationInterface.IUtil.GetApplicationMemoryStatistics();
		}

		public static bool IsBenchmarkQuited()
		{
			return EngineApplicationInterface.IUtil.IsBenchmarkQuited();
		}

		public static string GetNativeMemoryStatistics()
		{
			return EngineApplicationInterface.IUtil.GetNativeMemoryStatistics();
		}

		public static bool CommandLineArgumentExists(string str)
		{
			return EngineApplicationInterface.IUtil.CommandLineArgumentExists(str);
		}

		public static string GetConsoleHostMachine()
		{
			return EngineApplicationInterface.IUtil.GetConsoleHostMachine();
		}

		public static string ExportNavMeshFaceMarks(string file_name)
		{
			return EngineApplicationInterface.IUtil.ExportNavMeshFaceMarks(file_name);
		}

		public static string TakeSSFromTop(string file_name)
		{
			return EngineApplicationInterface.IUtil.TakeSSFromTop(file_name);
		}

		public static void CheckIfAssetsAndSourcesAreSame()
		{
			EngineApplicationInterface.IUtil.CheckIfAssetsAndSourcesAreSame();
		}

		public static void DisableCoreGame()
		{
			EngineApplicationInterface.IUtil.DisableCoreGame();
		}

		public static float GetApplicationMemory()
		{
			return EngineApplicationInterface.IUtil.GetApplicationMemory();
		}

		public static void GatherCoreGameReferences(string scene_names)
		{
			EngineApplicationInterface.IUtil.GatherCoreGameReferences(scene_names);
		}

		public static bool IsOnlyCoreContentEnabled()
		{
			return EngineApplicationInterface.IUtil.GetCoreGameState() != 0;
		}

		public static void GetSnowAmountData(byte[] snowData)
		{
			EngineApplicationInterface.IUtil.GetSnowAmountData(snowData);
		}

		public static void FindMeshesWithoutLods(string module_name)
		{
			EngineApplicationInterface.IUtil.FindMeshesWithoutLods(module_name);
		}

		public static void SetDisableDumpGeneration(bool value)
		{
			EngineApplicationInterface.IUtil.SetDisableDumpGeneration(value);
		}

		public static void SetPrintCallstackAtCrahses(bool value)
		{
			EngineApplicationInterface.IUtil.SetPrintCallstackAtCrahses(value);
		}

		public static string[] GetModulesNames()
		{
			return EngineApplicationInterface.IUtil.GetModulesCode().Split(new char[] { '*' });
		}

		public static string GetFullModulePath(string moduleName)
		{
			return EngineApplicationInterface.IUtil.GetFullModulePath(moduleName);
		}

		public static string[] GetFullModulePaths()
		{
			return EngineApplicationInterface.IUtil.GetFullModulePaths().Split(new char[] { '*' });
		}

		public static string GetFullFilePathOfScene(string sceneName)
		{
			string fullFilePathOfScene = EngineApplicationInterface.IUtil.GetFullFilePathOfScene(sceneName);
			if (fullFilePathOfScene == "SCENE_NOT_FOUND")
			{
				throw new Exception("Scene '" + sceneName + "' was not found!");
			}
			return fullFilePathOfScene.Replace("$BASE/", Utilities.GetBasePath());
		}

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

		public static bool TryGetUniqueIdentifiersForScene(string sceneName, out UniqueSceneId identifiers)
		{
			identifiers = null;
			string text;
			return Utilities.TryGetFullFilePathOfScene(sceneName, out text) && Utilities.TryGetUniqueIdentifiersForSceneFile(text, out identifiers);
		}

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

		public static void PairSceneNameToModuleName(string sceneName, string moduleName)
		{
			EngineApplicationInterface.IUtil.PairSceneNameToModuleName(sceneName, moduleName);
		}

		public static string[] GetSingleModuleScenesOfModule(string moduleName)
		{
			return EngineApplicationInterface.IUtil.GetSingleModuleScenesOfModule(moduleName).Split(new char[] { '*' });
		}

		public static string GetFullCommandLineString()
		{
			return EngineApplicationInterface.IUtil.GetFullCommandLineString();
		}

		public static void SetScreenTextRenderingState(bool state)
		{
			EngineApplicationInterface.IUtil.SetScreenTextRenderingState(state);
		}

		public static void SetMessageLineRenderingState(bool state)
		{
			EngineApplicationInterface.IUtil.SetMessageLineRenderingState(state);
		}

		public static bool CheckIfTerrainShaderHeaderGenerationFinished()
		{
			return EngineApplicationInterface.IUtil.CheckIfTerrainShaderHeaderGenerationFinished();
		}

		public static void GenerateTerrainShaderHeaders(string targetPlatform, string targetConfig, string output_path)
		{
			EngineApplicationInterface.IUtil.GenerateTerrainShaderHeaders(targetPlatform, targetConfig, output_path);
		}

		public static void CompileTerrainShadersDist(string targetPlatform, string targetConfig, string output_path)
		{
			EngineApplicationInterface.IUtil.CompileTerrainShadersDist(targetPlatform, targetConfig, output_path);
		}

		public static void SetCrashOnAsserts(bool val)
		{
			EngineApplicationInterface.IUtil.SetCrashOnAsserts(val);
		}

		public static void SetCrashOnWarnings(bool val)
		{
			EngineApplicationInterface.IUtil.SetCrashOnWarnings(val);
		}

		public static void ToggleRender()
		{
			EngineApplicationInterface.IUtil.ToggleRender();
		}

		public static void SetRenderAgents(bool value)
		{
			EngineApplicationInterface.IUtil.SetRenderAgents(value);
		}

		public static bool CheckShaderCompilation()
		{
			return EngineApplicationInterface.IUtil.CheckShaderCompilation();
		}

		public static void CompileAllShaders(string targetPlatform)
		{
			EngineApplicationInterface.IUtil.CompileAllShaders(targetPlatform);
		}

		public static string GetExecutableWorkingDirectory()
		{
			return EngineApplicationInterface.IUtil.GetExecutableWorkingDirectory();
		}

		public static void SetDumpFolderPath(string path)
		{
			EngineApplicationInterface.IUtil.SetDumpFolderPath(path);
		}

		public static void CheckSceneForProblems(string sceneName)
		{
			EngineApplicationInterface.IUtil.CheckSceneForProblems(sceneName);
		}

		public static void SetCoreGameState(int state)
		{
			EngineApplicationInterface.IUtil.SetCoreGameState(state);
		}

		public static int GetCoreGameState()
		{
			return EngineApplicationInterface.IUtil.GetCoreGameState();
		}

		public static string ExecuteCommandLineCommand(string command)
		{
			return EngineApplicationInterface.IUtil.ExecuteCommandLineCommand(command);
		}

		public static void QuitGame()
		{
			EngineApplicationInterface.IUtil.QuitGame();
		}

		public static void ExitProcess(int exitCode)
		{
			EngineApplicationInterface.IUtil.ExitProcess(exitCode);
		}

		public static string GetBasePath()
		{
			return EngineApplicationInterface.IUtil.GetBaseDirectory();
		}

		public static string GetVisualTestsValidatePath()
		{
			return EngineApplicationInterface.IUtil.GetVisualTestsValidatePath();
		}

		public static string GetVisualTestsTestFilesPath()
		{
			return EngineApplicationInterface.IUtil.GetVisualTestsTestFilesPath();
		}

		public static string GetAttachmentsPath()
		{
			return EngineApplicationInterface.IUtil.GetAttachmentsPath();
		}

		public static void StartScenePerformanceReport(string folderPath)
		{
			EngineApplicationInterface.IUtil.StartScenePerformanceReport(folderPath);
		}

		public static bool IsSceneReportFinished()
		{
			return EngineApplicationInterface.IUtil.IsSceneReportFinished();
		}

		public static float GetFps()
		{
			return EngineApplicationInterface.IUtil.GetFps();
		}

		public static float GetMainFps()
		{
			return EngineApplicationInterface.IUtil.GetMainFps();
		}

		public static float GetRendererFps()
		{
			return EngineApplicationInterface.IUtil.GetRendererFps();
		}

		public static void EnableSingleGPUQueryPerFrame()
		{
			EngineApplicationInterface.IUtil.EnableSingleGPUQueryPerFrame();
		}

		public static void ClearDecalAtlas(DecalAtlasGroup atlasGroup)
		{
			EngineApplicationInterface.IUtil.clear_decal_atlas(atlasGroup);
		}

		public static void FlushManagedObjectsMemory()
		{
			Common.MemoryCleanupGC(false);
		}

		public static void OnLoadingWindowEnabled()
		{
			EngineApplicationInterface.IUtil.OnLoadingWindowEnabled();
		}

		public static void DebugSetGlobalLoadingWindowState(bool newState)
		{
			EngineApplicationInterface.IUtil.DebugSetGlobalLoadingWindowState(newState);
		}

		public static void OnLoadingWindowDisabled()
		{
			EngineApplicationInterface.IUtil.OnLoadingWindowDisabled();
		}

		public static void DisableGlobalLoadingWindow()
		{
			EngineApplicationInterface.IUtil.DisableGlobalLoadingWindow();
		}

		public static void EnableGlobalLoadingWindow()
		{
			EngineApplicationInterface.IUtil.EnableGlobalLoadingWindow();
		}

		public static void EnableGlobalEditDataCacher()
		{
			EngineApplicationInterface.IUtil.EnableGlobalEditDataCacher();
		}

		public static void DoFullBakeAllLevelsAutomated(string module, string scene)
		{
			EngineApplicationInterface.IUtil.DoFullBakeAllLevelsAutomated(module, scene);
		}

		public static int GetReturnCode()
		{
			return EngineApplicationInterface.IUtil.GetReturnCode();
		}

		public static void DisableGlobalEditDataCacher()
		{
			EngineApplicationInterface.IUtil.DisableGlobalEditDataCacher();
		}

		public static void DoFullBakeSingleLevelAutomated(string module, string scene)
		{
			EngineApplicationInterface.IUtil.DoFullBakeSingleLevelAutomated(module, scene);
		}

		public static void DoLightOnlyBakeSingleLevelAutomated(string module, string scene)
		{
			EngineApplicationInterface.IUtil.DoLightOnlyBakeSingleLevelAutomated(module, scene);
		}

		public static void DoLightOnlyBakeAllLevelsAutomated(string module, string scene)
		{
			EngineApplicationInterface.IUtil.DoLightOnlyBakeAllLevelsAutomated(module, scene);
		}

		public static bool DidAutomatedGIBakeFinished()
		{
			return EngineApplicationInterface.IUtil.DidAutomatedGIBakeFinished();
		}

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

		public static void AddCommandLineFunction(string concatName)
		{
			EngineApplicationInterface.IUtil.AddCommandLineFunction(concatName);
		}

		public static int GetNumberOfShaderCompilationsInProgress()
		{
			return EngineApplicationInterface.IUtil.GetNumberOfShaderCompilationsInProgress();
		}

		public static int IsDetailedSoundLogOn()
		{
			return EngineApplicationInterface.IUtil.IsDetailedSoundLogOn();
		}

		public static ulong GetCurrentCpuMemoryUsageMB()
		{
			return EngineApplicationInterface.IUtil.GetCurrentCpuMemoryUsage();
		}

		public static ulong GetGpuMemoryOfAllocationGroup(string name)
		{
			return EngineApplicationInterface.IUtil.GetGpuMemoryOfAllocationGroup(name);
		}

		public static void GetGPUMemoryStats(ref float totalMemory, ref float renderTargetMemory, ref float depthTargetMemory, ref float srvMemory, ref float bufferMemory)
		{
			EngineApplicationInterface.IUtil.GetGPUMemoryStats(ref totalMemory, ref renderTargetMemory, ref depthTargetMemory, ref srvMemory, ref bufferMemory);
		}

		public static void GetDetailedGPUMemoryData(ref int totalMemoryAllocated, ref int totalMemoryUsed, ref int emptyChunkTotalSize)
		{
			EngineApplicationInterface.IUtil.GetDetailedGPUBufferMemoryStats(ref totalMemoryAllocated, ref totalMemoryUsed, ref emptyChunkTotalSize);
		}

		public static void SetRenderMode(Utilities.EngineRenderDisplayMode mode)
		{
			EngineApplicationInterface.IUtil.SetRenderMode((int)mode);
		}

		public static void SetForceDrawEntityID(bool value)
		{
			EngineApplicationInterface.IUtil.SetForceDrawEntityID(value);
		}

		public static void AddPerformanceReportToken(string performance_type, string name, float loading_time)
		{
			EngineApplicationInterface.IUtil.AddPerformanceReportToken(performance_type, name, loading_time);
		}

		public static void AddSceneObjectReport(string scene_name, string report_name, float report_value)
		{
			EngineApplicationInterface.IUtil.AddSceneObjectReport(scene_name, report_name, report_value);
		}

		public static void OutputPerformanceReports()
		{
			EngineApplicationInterface.IUtil.OutputPerformanceReports();
		}

		public static int EngineFrameNo
		{
			get
			{
				return EngineApplicationInterface.IUtil.GetEngineFrameNo();
			}
		}

		public static bool EditModeEnabled
		{
			get
			{
				return EngineApplicationInterface.IUtil.IsEditModeEnabled();
			}
		}

		public static void TakeScreenshot(PlatformFilePath path)
		{
			EngineApplicationInterface.IUtil.TakeScreenshotFromPlatformPath(path);
		}

		public static void TakeScreenshot(string path)
		{
			EngineApplicationInterface.IUtil.TakeScreenshotFromStringPath(path);
		}

		public static void SetAllocationAlwaysValidScene(Scene scene)
		{
			EngineApplicationInterface.IUtil.SetAllocationAlwaysValidScene((scene != null) ? scene.Pointer : UIntPtr.Zero);
		}

		public static void CheckResourceModifications()
		{
			EngineApplicationInterface.IUtil.CheckResourceModifications();
		}

		public static void SetGraphicsPreset(int preset)
		{
			EngineApplicationInterface.IUtil.SetGraphicsPreset(preset);
		}

		public static string GetLocalOutputPath()
		{
			return EngineApplicationInterface.IUtil.GetLocalOutputPath();
		}

		public static string GetPCInfo()
		{
			return EngineApplicationInterface.IUtil.GetPCInfo();
		}

		public static int GetGPUMemoryMB()
		{
			return EngineApplicationInterface.IUtil.GetGPUMemoryMB();
		}

		public static int GetCurrentEstimatedGPUMemoryCostMB()
		{
			return EngineApplicationInterface.IUtil.GetCurrentEstimatedGPUMemoryCostMB();
		}

		public static void DumpGPUMemoryStatistics(string filePath)
		{
			EngineApplicationInterface.IUtil.DumpGPUMemoryStatistics(filePath);
		}

		public static int SaveDataAsTexture(string path, int width, int height, float[] data)
		{
			return EngineApplicationInterface.IUtil.SaveDataAsTexture(path, width, height, data);
		}

		public static void ClearOldResourcesAndObjects()
		{
			EngineApplicationInterface.IUtil.ClearOldResourcesAndObjects();
		}

		public static void LoadVirtualTextureTileset(string name)
		{
			EngineApplicationInterface.IUtil.LoadVirtualTextureTileset(name);
		}

		public static float GetDeltaTime(int timerId)
		{
			return EngineApplicationInterface.IUtil.GetDeltaTime(timerId);
		}

		public static void LoadSkyBoxes()
		{
			EngineApplicationInterface.IUtil.LoadSkyBoxes();
		}

		public static string GetApplicationName()
		{
			return EngineApplicationInterface.IUtil.GetApplicationName();
		}

		public static void SetWindowTitle(string title)
		{
			EngineApplicationInterface.IUtil.SetWindowTitle(title);
		}

		public static string ProcessWindowTitle(string title)
		{
			return EngineApplicationInterface.IUtil.ProcessWindowTitle(title);
		}

		public static uint GetCurrentProcessID()
		{
			return EngineApplicationInterface.IUtil.GetCurrentProcessID();
		}

		public static void DoDelayedexit(int returnCode)
		{
			EngineApplicationInterface.IUtil.DoDelayedexit(returnCode);
		}

		public static void SetAssertionsAndWarningsSetExitCode(bool value)
		{
			EngineApplicationInterface.IUtil.SetAssertionsAndWarningsSetExitCode(value);
		}

		public static void SetReportMode(bool reportMode)
		{
			EngineApplicationInterface.IUtil.SetReportMode(reportMode);
		}

		public static void SetAssertionAtShaderCompile(bool value)
		{
			EngineApplicationInterface.IUtil.SetAssertionAtShaderCompile(value);
		}

		public static void SetCrashReportCustomString(string customString)
		{
			EngineApplicationInterface.IUtil.SetCrashReportCustomString(customString);
		}

		public static void SetCrashReportCustomStack(string customStack)
		{
			EngineApplicationInterface.IUtil.SetCrashReportCustomStack(customStack);
		}

		public static int GetSteamAppId()
		{
			return EngineApplicationInterface.IUtil.GetSteamAppId();
		}

		public static void SetForceVsync(bool value)
		{
			Debug.Print("Force VSync State is now " + (value ? "ACTIVE" : "DEACTIVATED"), 0, Debug.DebugColor.DarkBlue, 17592186044416UL);
			EngineApplicationInterface.IUtil.SetForceVsync(value);
		}

		private static PlatformFilePath DefaultBannerlordConfigFullPath
		{
			get
			{
				return new PlatformFilePath(EngineFilePaths.ConfigsPath, "BannerlordConfig.txt");
			}
		}

		public static string LoadBannerlordConfigFile()
		{
			PlatformFilePath defaultBannerlordConfigFullPath = Utilities.DefaultBannerlordConfigFullPath;
			if (!FileHelper.FileExists(defaultBannerlordConfigFullPath))
			{
				return "";
			}
			return FileHelper.GetFileContentString(defaultBannerlordConfigFullPath);
		}

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

		public static void OpenOnscreenKeyboard(string initialText, string descriptionText, int maxLength, int keyboardTypeEnum)
		{
			EngineApplicationInterface.IUtil.OpenOnscreenKeyboard(initialText, descriptionText, maxLength, keyboardTypeEnum);
		}

		public static string GetSystemLanguage()
		{
			return EngineApplicationInterface.IUtil.GetSystemLanguage();
		}

		public static int RegisterGPUAllocationGroup(string name)
		{
			return EngineApplicationInterface.IUtil.RegisterGPUAllocationGroup(name);
		}

		public static int GetMemoryUsageOfCategory(int category)
		{
			return EngineApplicationInterface.IUtil.GetMemoryUsageOfCategory(category);
		}

		public static string GetDetailedXBOXMemoryInfo()
		{
			return EngineApplicationInterface.IUtil.GetDetailedXBOXMemoryInfo();
		}

		public static int GetVertexBufferChunkSystemMemoryUsage()
		{
			return EngineApplicationInterface.IUtil.GetVertexBufferChunkSystemMemoryUsage();
		}

		public static int GetBuildNumber()
		{
			return EngineApplicationInterface.IUtil.GetBuildNumber();
		}

		public static ApplicationVersion GetApplicationVersionWithBuildNumber()
		{
			return ApplicationVersion.FromParametersFile(null);
		}

		public static void ParallelFor(int startIndex, int endIndex, long curKey, int grainSize)
		{
			EngineApplicationInterface.IUtil.ManagedParallelFor(startIndex, endIndex, curKey, grainSize);
		}

		public static void ClearShaderMemory()
		{
			EngineApplicationInterface.IUtil.ClearShaderMemory();
		}

		public static void RegisterMeshForGPUMorph(string metaMeshName)
		{
			EngineApplicationInterface.IUtil.RegisterMeshForGPUMorph(metaMeshName);
		}

		public static void ParallelForWithDt(int startIndex, int endIndex, long curKey, int grainSize)
		{
			EngineApplicationInterface.IUtil.ManagedParallelForWithDt(startIndex, endIndex, curKey, grainSize);
		}

		public static ulong GetMainThreadId()
		{
			return EngineApplicationInterface.IUtil.GetMainThreadId();
		}

		public static ulong GetCurrentThreadId()
		{
			return EngineApplicationInterface.IUtil.GetCurrentThreadId();
		}

		private static ConcurrentQueue<Utilities.MainThreadJob> jobs = new ConcurrentQueue<Utilities.MainThreadJob>();

		public static bool renderingActive = true;

		public enum EngineRenderDisplayMode
		{
			ShowNone,
			ShowAlbedo,
			ShowNormals,
			ShowVertexNormals,
			ShowSpecular,
			ShowGloss,
			ShowOcclusion,
			ShowGbufferShadowMask,
			ShowTranslucency,
			ShowMotionVector,
			ShowVertexColor,
			ShowDepth,
			ShowTiledLightOverdraw,
			ShowTiledDecalOverdraw,
			ShowMeshId,
			ShowDisableSunLighting,
			ShowDebugTexture,
			ShowTextureDensity,
			ShowOverdraw,
			ShowVsComplexity,
			ShowPsComplexity,
			ShowDisableAmbientLighting,
			ShowEntityId,
			ShowPrtDiffuseAmbient,
			ShowLightDebugMode,
			ShowParticleShadingAtlas,
			ShowTerrainAngle,
			ShowParallaxDebug,
			ShowAlbedoValidation,
			NumDebugModes
		}

		private class MainThreadJob
		{
			internal MainThreadJob(Delegate function, object[] parameters)
			{
				this._function = function;
				this._parameters = parameters;
				this.wait_handle = null;
			}

			internal MainThreadJob(Semaphore sema, Delegate function, object[] parameters)
			{
				this._function = function;
				this._parameters = parameters;
				this.wait_handle = sema;
			}

			internal void Invoke()
			{
				this._function.DynamicInvoke(this._parameters);
				if (this.wait_handle != null)
				{
					this.wait_handle.Release();
				}
			}

			private Delegate _function;

			private object[] _parameters;

			private Semaphore wait_handle;
		}

		public class MainThreadPerformanceQuery : IDisposable
		{
			public MainThreadPerformanceQuery(string parent, string name)
			{
				this._name = name;
				this._parent = parent;
				this._stopWatch = new Stopwatch();
				this._stopWatch.Start();
			}

			public void Dispose()
			{
				this._stopWatch.Stop();
				float num = (float)this._stopWatch.Elapsed.TotalMilliseconds;
				num /= 1000f;
				EngineApplicationInterface.IUtil.AddMainThreadPerformanceQuery(this._parent, this._name, num);
			}

			private string _name;

			private string _parent;

			private Stopwatch _stopWatch;
		}
	}
}
