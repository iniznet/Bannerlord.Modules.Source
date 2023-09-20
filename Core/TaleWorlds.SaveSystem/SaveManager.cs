using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem.Definition;
using TaleWorlds.SaveSystem.Load;
using TaleWorlds.SaveSystem.Save;

namespace TaleWorlds.SaveSystem
{
	public static class SaveManager
	{
		public static void InitializeGlobalDefinitionContext()
		{
			SaveManager._definitionContext = new DefinitionContext();
			SaveManager._definitionContext.FillWithCurrentTypes();
		}

		public static SaveOutput Save(object target, MetaData metaData, string saveName, ISaveDriver driver)
		{
			if (SaveManager._definitionContext == null)
			{
				SaveManager.InitializeGlobalDefinitionContext();
			}
			SaveOutput saveOutput = null;
			if (SaveManager._definitionContext.GotError)
			{
				List<SaveError> list = new List<SaveError>();
				foreach (string text in SaveManager._definitionContext.Errors)
				{
					list.Add(new SaveError(text));
				}
				saveOutput = SaveOutput.CreateFailed(list, SaveResult.GeneralFailure);
			}
			else
			{
				using (new PerformanceTestBlock("Save Context"))
				{
					Debug.Print("Saving with new context", 0, Debug.DebugColor.White, 17592186044416UL);
					SaveContext saveContext = new SaveContext(SaveManager._definitionContext);
					string text2;
					if (saveContext.Save(target, metaData, out text2))
					{
						try
						{
							Task<SaveResultWithMessage> task = driver.Save(saveName, 1, metaData, saveContext.SaveData);
							if (task.IsCompleted)
							{
								if (task.Result.SaveResult == SaveResult.Success)
								{
									saveOutput = SaveOutput.CreateSuccessful(saveContext.SaveData);
								}
								else
								{
									saveOutput = SaveOutput.CreateFailed(new SaveError[]
									{
										new SaveError(task.Result.Message)
									}, task.Result.SaveResult);
								}
							}
							else
							{
								saveOutput = SaveOutput.CreateContinuing(task);
							}
							return saveOutput;
						}
						catch (Exception ex)
						{
							return SaveOutput.CreateFailed(new SaveError[]
							{
								new SaveError(ex.Message)
							}, SaveResult.GeneralFailure);
						}
					}
					saveOutput = SaveOutput.CreateFailed(new SaveError[]
					{
						new SaveError(text2)
					}, SaveResult.GeneralFailure);
				}
			}
			return saveOutput;
		}

		public static MetaData LoadMetaData(string saveName, ISaveDriver driver)
		{
			return driver.LoadMetaData(saveName);
		}

		public static LoadResult Load(string saveName, ISaveDriver driver)
		{
			return SaveManager.Load(saveName, driver, false);
		}

		public static LoadResult Load(string saveName, ISaveDriver driver, bool loadAsLateInitialize)
		{
			DefinitionContext definitionContext = new DefinitionContext();
			definitionContext.FillWithCurrentTypes();
			LoadContext loadContext = new LoadContext(definitionContext, driver);
			LoadData loadData = driver.Load(saveName);
			LoadResult loadResult;
			if (loadContext.Load(loadData, loadAsLateInitialize))
			{
				LoadCallbackInitializator loadCallbackInitializator = null;
				if (loadAsLateInitialize)
				{
					loadCallbackInitializator = loadContext.CreateLoadCallbackInitializator(loadData);
				}
				loadResult = LoadResult.CreateSuccessful(loadContext.RootObject, loadData.MetaData, loadCallbackInitializator);
			}
			else
			{
				loadResult = LoadResult.CreateFailed(new LoadError[]
				{
					new LoadError("Not implemented")
				});
			}
			return loadResult;
		}

		public const string SaveFileExtension = "sav";

		private const int CurrentVersion = 1;

		private static DefinitionContext _definitionContext;
	}
}
