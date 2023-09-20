using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem.Definition;
using TaleWorlds.SaveSystem.Load;
using TaleWorlds.SaveSystem.Save;

namespace TaleWorlds.SaveSystem
{
	// Token: 0x02000021 RID: 33
	public static class SaveManager
	{
		// Token: 0x060000B5 RID: 181 RVA: 0x00004230 File Offset: 0x00002430
		public static void InitializeGlobalDefinitionContext()
		{
			SaveManager._definitionContext = new DefinitionContext();
			SaveManager._definitionContext.FillWithCurrentTypes();
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x00004248 File Offset: 0x00002448
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

		// Token: 0x060000B7 RID: 183 RVA: 0x000043D8 File Offset: 0x000025D8
		public static MetaData LoadMetaData(string saveName, ISaveDriver driver)
		{
			return driver.LoadMetaData(saveName);
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x000043E1 File Offset: 0x000025E1
		public static LoadResult Load(string saveName, ISaveDriver driver)
		{
			return SaveManager.Load(saveName, driver, false);
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x000043EC File Offset: 0x000025EC
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

		// Token: 0x0400004F RID: 79
		public const string SaveFileExtension = "sav";

		// Token: 0x04000050 RID: 80
		private const int CurrentVersion = 1;

		// Token: 0x04000051 RID: 81
		private static DefinitionContext _definitionContext;
	}
}
