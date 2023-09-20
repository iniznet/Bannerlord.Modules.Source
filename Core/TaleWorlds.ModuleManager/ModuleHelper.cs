using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TaleWorlds.Library;

namespace TaleWorlds.ModuleManager
{
	// Token: 0x02000005 RID: 5
	public static class ModuleHelper
	{
		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600000C RID: 12 RVA: 0x000020E9 File Offset: 0x000002E9
		private static string _pathPrefix
		{
			get
			{
				return BasePath.Name + "Modules/";
			}
		}

		// Token: 0x0600000D RID: 13 RVA: 0x000020FA File Offset: 0x000002FA
		public static string GetModuleFullPath(string moduleId)
		{
			ModuleHelper.EnsureModuleInfosAreLoaded();
			return ModuleHelper._allFoundModules[moduleId.ToLower()].FolderPath + "/";
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00002120 File Offset: 0x00000320
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

		// Token: 0x0600000F RID: 15 RVA: 0x00002153 File Offset: 0x00000353
		public static void InitializePlatformModuleExtension(IPlatformModuleExtension moduleExtension)
		{
			ModuleHelper._platformModuleExtension = moduleExtension;
			ModuleHelper._platformModuleExtension.Initialize();
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002165 File Offset: 0x00000365
		public static void ClearPlatformModuleExtension()
		{
			if (ModuleHelper._platformModuleExtension != null)
			{
				ModuleHelper._platformModuleExtension.Destroy();
				ModuleHelper._platformModuleExtension = null;
			}
			ModuleHelper._allFoundModules = null;
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002184 File Offset: 0x00000384
		private static void EnsureModuleInfosAreLoaded()
		{
			if (ModuleHelper._allFoundModules == null)
			{
				ModuleHelper.GetModules();
			}
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00002193 File Offset: 0x00000393
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

		// Token: 0x06000013 RID: 19 RVA: 0x000021AC File Offset: 0x000003AC
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

		// Token: 0x06000014 RID: 20 RVA: 0x00002264 File Offset: 0x00000464
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

		// Token: 0x06000015 RID: 21 RVA: 0x00002314 File Offset: 0x00000514
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

		// Token: 0x06000016 RID: 22 RVA: 0x00002348 File Offset: 0x00000548
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

		// Token: 0x06000017 RID: 23 RVA: 0x000024DC File Offset: 0x000006DC
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

		// Token: 0x06000018 RID: 24 RVA: 0x00002522 File Offset: 0x00000722
		public static string GetXmlPathForNative(string moduleId, string xmlName)
		{
			return ModuleHelper.GetModuleFullPath(moduleId) + xmlName;
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002530 File Offset: 0x00000730
		public static string GetXmlPathForNativeWBase(string moduleId, string xmlName)
		{
			return "$BASE/Modules/" + moduleId + "/" + xmlName;
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00002543 File Offset: 0x00000743
		public static string GetXsltPathForNative(string moduleId, string xsltName)
		{
			xsltName = xsltName.Remove(xsltName.Length - 4);
			return ModuleHelper.GetModuleFullPath(moduleId) + xsltName + ".xsl";
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00002566 File Offset: 0x00000766
		public static string GetPath(string id)
		{
			return ModuleHelper.GetModuleFullPath(id) + "SubModule.xml";
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00002578 File Offset: 0x00000778
		public static string GetXmlPath(string moduleId, string xmlName)
		{
			return ModuleHelper.GetModuleFullPath(moduleId) + "ModuleData/" + xmlName + ".xml";
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002590 File Offset: 0x00000790
		public static string GetXsltPath(string moduleId, string xmlName)
		{
			return ModuleHelper.GetModuleFullPath(moduleId) + "ModuleData/" + xmlName + ".xsl";
		}

		// Token: 0x0600001E RID: 30 RVA: 0x000025A8 File Offset: 0x000007A8
		public static string GetXsdPath(string xmlInfoId)
		{
			return BasePath.Name + "XmlSchemas/" + xmlInfoId + ".xsd";
		}

		// Token: 0x0600001F RID: 31 RVA: 0x000025BF File Offset: 0x000007BF
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

		// Token: 0x06000020 RID: 32 RVA: 0x000025D8 File Offset: 0x000007D8
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

		// Token: 0x04000009 RID: 9
		private static Dictionary<string, ModuleInfo> _allFoundModules;

		// Token: 0x0400000A RID: 10
		private static IPlatformModuleExtension _platformModuleExtension;
	}
}
