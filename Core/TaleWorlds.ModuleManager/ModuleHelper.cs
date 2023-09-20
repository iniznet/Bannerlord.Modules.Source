using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TaleWorlds.Library;

namespace TaleWorlds.ModuleManager
{
	public static class ModuleHelper
	{
		private static string _pathPrefix
		{
			get
			{
				return BasePath.Name + "Modules/";
			}
		}

		public static string GetModuleFullPath(string moduleId)
		{
			ModuleHelper.EnsureModuleInfosAreLoaded();
			return ModuleHelper._allFoundModules[moduleId.ToLower()].FolderPath + "/";
		}

		public static ModuleInfo GetModuleInfo(string moduleId)
		{
			ModuleHelper.EnsureModuleInfosAreLoaded();
			string text = moduleId.ToLower();
			if (ModuleHelper._allFoundModules.ContainsKey(text))
			{
				return ModuleHelper._allFoundModules[text];
			}
			return null;
		}

		public static void InitializePlatformModuleExtension(IPlatformModuleExtension moduleExtension)
		{
			ModuleHelper._platformModuleExtension = moduleExtension;
			ModuleHelper._platformModuleExtension.Initialize();
		}

		public static void ClearPlatformModuleExtension()
		{
			if (ModuleHelper._platformModuleExtension != null)
			{
				ModuleHelper._platformModuleExtension.Destroy();
				ModuleHelper._platformModuleExtension = null;
			}
			ModuleHelper._allFoundModules = null;
		}

		private static void EnsureModuleInfosAreLoaded()
		{
			if (ModuleHelper._allFoundModules == null)
			{
				ModuleHelper.GetModules();
			}
		}

		private static IEnumerable<string> GetModulePaths(string directoryPath, int searchDepth)
		{
			string[] array;
			if (searchDepth > 0)
			{
				string[] directories = Directory.GetDirectories(directoryPath);
				foreach (string text in directories)
				{
					foreach (string text2 in ModuleHelper.GetModulePaths(text, searchDepth - 1).ToList<string>())
					{
						yield return text2;
					}
					List<string>.Enumerator enumerator = default(List<string>.Enumerator);
				}
				array = null;
			}
			string[] files = Directory.GetFiles(directoryPath, "SubModule.xml");
			foreach (string text3 in files)
			{
				yield return text3;
			}
			array = null;
			yield break;
			yield break;
		}

		private static List<ModuleInfo> GetPhysicalModules()
		{
			List<ModuleInfo> list = new List<ModuleInfo>();
			foreach (string text in ModuleHelper.GetModulePaths(ModuleHelper._pathPrefix, 1).ToArray<string>())
			{
				ModuleInfo moduleInfo = new ModuleInfo();
				try
				{
					string directoryName = Path.GetDirectoryName(text);
					moduleInfo.LoadWithFullPath(directoryName);
					list.Add(moduleInfo);
				}
				catch (Exception ex)
				{
					string text2 = string.Concat(new string[]
					{
						"Module ",
						text,
						" can't be loaded, there are some errors.",
						Environment.NewLine,
						Environment.NewLine,
						ex.Message
					});
					string text3 = "ERROR";
					Debug.ShowMessageBox(text2, text3, 4U);
				}
			}
			return list;
		}

		private static List<ModuleInfo> GetPlatformModules()
		{
			List<ModuleInfo> list = new List<ModuleInfo>();
			if (ModuleHelper._platformModuleExtension != null)
			{
				foreach (string text in ModuleHelper._platformModuleExtension.GetModulePaths())
				{
					ModuleInfo moduleInfo = new ModuleInfo();
					try
					{
						moduleInfo.LoadWithFullPath(text);
						list.Add(moduleInfo);
					}
					catch (Exception ex)
					{
						string text2 = string.Concat(new string[]
						{
							"Module ",
							text,
							" can't be loaded, there are some errors.",
							Environment.NewLine,
							Environment.NewLine,
							ex.Message
						});
						string text3 = "ERROR";
						Debug.ShowMessageBox(text2, text3, 4U);
					}
				}
			}
			return list;
		}

		public static List<ModuleInfo> GetModuleInfos(string[] moduleNames)
		{
			List<ModuleInfo> list = new List<ModuleInfo>();
			for (int i = 0; i < moduleNames.Length; i++)
			{
				ModuleInfo moduleInfo = ModuleHelper.GetModuleInfo(moduleNames[i]);
				list.Add(moduleInfo);
			}
			return list;
		}

		public static IEnumerable<ModuleInfo> GetModules()
		{
			if (ModuleHelper._allFoundModules == null)
			{
				List<ModuleInfo> list = new List<ModuleInfo>();
				List<ModuleInfo> physicalModules = ModuleHelper.GetPhysicalModules();
				List<ModuleInfo> platformModules = ModuleHelper.GetPlatformModules();
				list.AddRange(physicalModules);
				list.AddRange(platformModules);
				List<ModuleInfo> list2 = new List<ModuleInfo>();
				ModuleHelper._allFoundModules = new Dictionary<string, ModuleInfo>();
				foreach (ModuleInfo moduleInfo in list)
				{
					if (moduleInfo.IsOfficial)
					{
						list2.Add(moduleInfo);
						moduleInfo.UpdateVersionChangeSet();
					}
					if (!ModuleHelper._allFoundModules.ContainsKey(moduleInfo.Id.ToLower()))
					{
						ModuleHelper._allFoundModules.Add(moduleInfo.Id.ToLower(), moduleInfo);
					}
				}
				foreach (ModuleInfo moduleInfo2 in list)
				{
					using (List<DependedModule>.Enumerator enumerator2 = moduleInfo2.DependedModules.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							DependedModule dependedModule = enumerator2.Current;
							if (list2.Any((ModuleInfo m) => m.Id == dependedModule.ModuleId))
							{
								dependedModule.UpdateVersionChangeSet();
							}
						}
					}
				}
			}
			return ModuleHelper._allFoundModules.Select((KeyValuePair<string, ModuleInfo> m) => m.Value);
		}

		public static string GetMbprojPath(string id)
		{
			ModuleHelper.EnsureModuleInfosAreLoaded();
			string text = id.ToLower();
			if (ModuleHelper._allFoundModules.ContainsKey(text))
			{
				return ModuleHelper._allFoundModules[text].FolderPath + "/ModuleData/project.mbproj";
			}
			return "";
		}

		public static string GetXmlPathForNative(string moduleId, string xmlName)
		{
			return ModuleHelper.GetModuleFullPath(moduleId) + xmlName;
		}

		public static string GetXmlPathForNativeWBase(string moduleId, string xmlName)
		{
			return "$BASE/Modules/" + moduleId + "/" + xmlName;
		}

		public static string GetXsltPathForNative(string moduleId, string xsltName)
		{
			xsltName = xsltName.Remove(xsltName.Length - 4);
			return ModuleHelper.GetModuleFullPath(moduleId) + xsltName + ".xsl";
		}

		public static string GetPath(string id)
		{
			return ModuleHelper.GetModuleFullPath(id) + "SubModule.xml";
		}

		public static string GetXmlPath(string moduleId, string xmlName)
		{
			return ModuleHelper.GetModuleFullPath(moduleId) + "ModuleData/" + xmlName + ".xml";
		}

		public static string GetXsltPath(string moduleId, string xmlName)
		{
			return ModuleHelper.GetModuleFullPath(moduleId) + "ModuleData/" + xmlName + ".xsl";
		}

		public static string GetXsdPath(string xmlInfoId)
		{
			return BasePath.Name + "XmlSchemas/" + xmlInfoId + ".xsd";
		}

		public static IEnumerable<ModuleInfo> GetDependentModulesOf(IEnumerable<ModuleInfo> source, ModuleInfo module)
		{
			using (List<DependedModule>.Enumerator enumerator = module.DependedModules.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					DependedModule item = enumerator.Current;
					ModuleInfo moduleInfo = source.FirstOrDefault((ModuleInfo i) => i.Id == item.ModuleId);
					if (moduleInfo != null)
					{
						yield return moduleInfo;
					}
				}
			}
			List<DependedModule>.Enumerator enumerator = default(List<DependedModule>.Enumerator);
			Func<DependedModule, bool> <>9__1;
			foreach (ModuleInfo moduleInfo2 in source)
			{
				IEnumerable<DependedModule> modulesToLoadAfterThis = moduleInfo2.ModulesToLoadAfterThis;
				Func<DependedModule, bool> func;
				if ((func = <>9__1) == null)
				{
					func = (<>9__1 = (DependedModule m) => m.ModuleId == module.Id);
				}
				if (modulesToLoadAfterThis.Any(func))
				{
					yield return moduleInfo2;
				}
			}
			IEnumerator<ModuleInfo> enumerator2 = null;
			yield break;
			yield break;
		}

		public static List<ModuleInfo> GetSortedModules(string[] moduleIDs)
		{
			List<ModuleInfo> modules = ModuleHelper.GetModuleInfos(moduleIDs);
			IList<ModuleInfo> list = MBMath.TopologySort<ModuleInfo>(modules, (ModuleInfo module) => ModuleHelper.GetDependentModulesOf(modules, module));
			List<ModuleInfo> list2;
			if ((list2 = list as List<ModuleInfo>) == null)
			{
				return list.ToList<ModuleInfo>();
			}
			return list2;
		}

		private static Dictionary<string, ModuleInfo> _allFoundModules;

		private static IPlatformModuleExtension _platformModuleExtension;
	}
}
