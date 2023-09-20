using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

		public static List<Type> CheckSaveableTypes()
		{
			List<Type> list = new List<Type>();
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				foreach (Type type in assemblies[i].GetTypesSafe(null))
				{
					PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					foreach (FieldInfo fieldInfo in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
					{
						Attribute[] array = fieldInfo.GetCustomAttributes(typeof(SaveableFieldAttribute)).ToArray<Attribute>();
						if (array.Length != 0)
						{
							SaveableFieldAttribute saveableFieldAttribute = (SaveableFieldAttribute)array[0];
							Type fieldType = fieldInfo.FieldType;
							if (!SaveManager._definitionContext.HasDefinition(fieldType) && !list.Contains(fieldType) && !fieldType.IsInterface && fieldType.FullName != null)
							{
								list.Add(fieldType);
							}
						}
					}
					foreach (PropertyInfo propertyInfo in properties)
					{
						Attribute[] array3 = propertyInfo.GetCustomAttributes(typeof(SaveablePropertyAttribute)).ToArray<Attribute>();
						if (array3.Length != 0)
						{
							SaveablePropertyAttribute saveablePropertyAttribute = (SaveablePropertyAttribute)array3[0];
							Type propertyType = propertyInfo.PropertyType;
							if (!SaveManager._definitionContext.HasDefinition(propertyType) && !list.Contains(propertyType) && !propertyType.IsInterface && propertyType.FullName != null)
							{
								list.Add(propertyType);
							}
						}
					}
				}
			}
			return list;
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
				Debug.Print("------Saving with new context. Save name: " + saveName + "------", 0, Debug.DebugColor.White, 17592186044416UL);
				SaveContext saveContext;
				string text2;
				if ((saveContext = new SaveContext(SaveManager._definitionContext)).Save(target, metaData, out text2))
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
