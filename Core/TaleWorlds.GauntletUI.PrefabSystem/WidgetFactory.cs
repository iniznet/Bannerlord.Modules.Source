using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.GauntletUI.PrefabSystem
{
	// Token: 0x02000019 RID: 25
	public class WidgetFactory
	{
		// Token: 0x17000026 RID: 38
		// (get) Token: 0x0600009C RID: 156 RVA: 0x00003642 File Offset: 0x00001842
		// (set) Token: 0x0600009D RID: 157 RVA: 0x0000364A File Offset: 0x0000184A
		public PrefabExtensionContext PrefabExtensionContext { get; private set; }

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x0600009E RID: 158 RVA: 0x00003653 File Offset: 0x00001853
		// (set) Token: 0x0600009F RID: 159 RVA: 0x0000365B File Offset: 0x0000185B
		public WidgetAttributeContext WidgetAttributeContext { get; private set; }

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060000A0 RID: 160 RVA: 0x00003664 File Offset: 0x00001864
		// (set) Token: 0x060000A1 RID: 161 RVA: 0x0000366C File Offset: 0x0000186C
		public GeneratedPrefabContext GeneratedPrefabContext { get; private set; }

		// Token: 0x060000A2 RID: 162 RVA: 0x00003678 File Offset: 0x00001878
		public WidgetFactory(ResourceDepot resourceDepot, string resourceFolder)
		{
			this._resourceDepot = resourceDepot;
			this._resourceDepot.OnResourceChange += this.OnResourceChange;
			this._resourceFolder = resourceFolder;
			this._builtinTypes = new Dictionary<string, Type>();
			this._liveCustomTypes = new Dictionary<string, CustomWidgetType>();
			this._customTypePaths = new Dictionary<string, string>();
			this._liveInstanceTracker = new Dictionary<string, int>();
			this.PrefabExtensionContext = new PrefabExtensionContext();
			this.WidgetAttributeContext = new WidgetAttributeContext();
			this.GeneratedPrefabContext = new GeneratedPrefabContext();
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x000036FD File Offset: 0x000018FD
		private void OnResourceChange()
		{
			this.CheckForUpdates();
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00003708 File Offset: 0x00001908
		public void Initialize(List<string> assemblyOrder = null)
		{
			foreach (PrefabExtension prefabExtension in this.PrefabExtensionContext.PrefabExtensions)
			{
				prefabExtension.RegisterAttributeTypes(this.WidgetAttributeContext);
			}
			foreach (Type type in WidgetInfo.CollectWidgetTypes())
			{
				bool flag = true;
				if (this._builtinTypes.ContainsKey(type.Name) && assemblyOrder != null)
				{
					flag = assemblyOrder.IndexOf(type.Assembly.GetName().Name + ".dll") > assemblyOrder.IndexOf(this._builtinTypes[type.Name].Assembly.GetName().Name + ".dll");
				}
				if (flag)
				{
					this._builtinTypes[type.Name] = type;
				}
			}
			foreach (KeyValuePair<string, string> keyValuePair in this.GetPrefabNamesAndPathsFromCurrentPath())
			{
				this.AddCustomType(keyValuePair.Key, keyValuePair.Value);
			}
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x00003870 File Offset: 0x00001A70
		private Dictionary<string, string> GetPrefabNamesAndPathsFromCurrentPath()
		{
			string[] files = this._resourceDepot.GetFiles(this._resourceFolder, ".xml", false);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (string text in files)
			{
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(text);
				string directoryName = Path.GetDirectoryName(text);
				if (!dictionary.ContainsKey(fileNameWithoutExtension))
				{
					dictionary.Add(fileNameWithoutExtension, directoryName + "\\");
				}
				else
				{
					Debug.FailedAssert(string.Concat(new string[]
					{
						"This prefab has already been added: ",
						fileNameWithoutExtension,
						". Previous Directory: ",
						dictionary[fileNameWithoutExtension],
						" | New Directory: ",
						directoryName
					}), "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI.PrefabSystem\\WidgetFactory.cs", "GetPrefabNamesAndPathsFromCurrentPath", 94);
					dictionary[fileNameWithoutExtension] = directoryName + "\\";
				}
			}
			return dictionary;
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00003939 File Offset: 0x00001B39
		public void AddCustomType(string name, string path)
		{
			this._customTypePaths.Add(name, path);
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x00003948 File Offset: 0x00001B48
		public IEnumerable<string> GetPrefabNames()
		{
			return this._customTypePaths.Keys;
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x00003955 File Offset: 0x00001B55
		public IEnumerable<string> GetWidgetTypes()
		{
			return this._builtinTypes.Keys.Concat(this._customTypePaths.Keys);
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x00003972 File Offset: 0x00001B72
		public bool IsBuiltinType(string name)
		{
			return this._builtinTypes.ContainsKey(name);
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00003980 File Offset: 0x00001B80
		public Type GetBuiltinType(string name)
		{
			return this._builtinTypes[name];
		}

		// Token: 0x060000AB RID: 171 RVA: 0x0000398E File Offset: 0x00001B8E
		public bool IsCustomType(string typeName)
		{
			return this._customTypePaths.ContainsKey(typeName);
		}

		// Token: 0x060000AC RID: 172 RVA: 0x0000399C File Offset: 0x00001B9C
		public string GetCustomTypePath(string name)
		{
			string text;
			if (this._customTypePaths.TryGetValue(name, out text))
			{
				return text;
			}
			Debug.FailedAssert("false", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI.PrefabSystem\\WidgetFactory.cs", "GetCustomTypePath", 139);
			return "";
		}

		// Token: 0x060000AD RID: 173 RVA: 0x000039DC File Offset: 0x00001BDC
		public Widget CreateBuiltinWidget(UIContext context, string typeName)
		{
			Type type;
			Widget widget;
			if (this._builtinTypes.TryGetValue(typeName, out type))
			{
				widget = (Widget)type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance, null, new Type[] { typeof(UIContext) }, null).InvokeWithLog(new object[] { context });
			}
			else
			{
				widget = new Widget(context);
				Debug.FailedAssert("builtin widget type not found in CreateBuiltinWidget(" + typeName + ")", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI.PrefabSystem\\WidgetFactory.cs", "CreateBuiltinWidget", 160);
			}
			return widget;
		}

		// Token: 0x060000AE RID: 174 RVA: 0x00003A60 File Offset: 0x00001C60
		public WidgetPrefab GetCustomType(string typeName)
		{
			CustomWidgetType customWidgetType;
			if (this._liveCustomTypes.TryGetValue(typeName, out customWidgetType))
			{
				Dictionary<string, int> liveInstanceTracker = this._liveInstanceTracker;
				int num = liveInstanceTracker[typeName];
				liveInstanceTracker[typeName] = num + 1;
				return customWidgetType.WidgetPrefab;
			}
			string text;
			if (this._customTypePaths.TryGetValue(typeName, out text))
			{
				CustomWidgetType customWidgetType2 = new CustomWidgetType(this, text, typeName);
				this._liveCustomTypes[typeName] = customWidgetType2;
				this._liveInstanceTracker[typeName] = 1;
				return customWidgetType2.WidgetPrefab;
			}
			Debug.FailedAssert("Couldn't find Custom Widget type: " + typeName, "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI.PrefabSystem\\WidgetFactory.cs", "GetCustomType", 183);
			return null;
		}

		// Token: 0x060000AF RID: 175 RVA: 0x00003AFC File Offset: 0x00001CFC
		public void OnUnload(string typeName)
		{
			if (this._liveCustomTypes.ContainsKey(typeName))
			{
				Dictionary<string, int> liveInstanceTracker = this._liveInstanceTracker;
				int num = liveInstanceTracker[typeName];
				liveInstanceTracker[typeName] = num - 1;
				if (this._liveInstanceTracker[typeName] == 0)
				{
					this._liveCustomTypes.Remove(typeName);
					this._liveInstanceTracker.Remove(typeName);
				}
			}
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x00003B58 File Offset: 0x00001D58
		public void CheckForUpdates()
		{
			bool flag = false;
			Dictionary<string, string> prefabNamesAndPathsFromCurrentPath = this.GetPrefabNamesAndPathsFromCurrentPath();
			foreach (KeyValuePair<string, string> keyValuePair in prefabNamesAndPathsFromCurrentPath)
			{
				if (!this._customTypePaths.ContainsKey(keyValuePair.Key))
				{
					this.AddCustomType(keyValuePair.Key, keyValuePair.Value);
				}
			}
			List<string> list = null;
			foreach (string text in this._customTypePaths.Keys)
			{
				if (!prefabNamesAndPathsFromCurrentPath.ContainsKey(text))
				{
					if (list == null)
					{
						list = new List<string>();
					}
					list.Add(text);
					flag = true;
				}
			}
			if (list != null)
			{
				foreach (string text2 in list)
				{
					this._customTypePaths.Remove(text2);
				}
			}
			foreach (CustomWidgetType customWidgetType in this._liveCustomTypes.Values)
			{
				flag = flag || customWidgetType.CheckForUpdate();
			}
			if (flag)
			{
				Action prefabChange = this.PrefabChange;
				if (prefabChange == null)
				{
					return;
				}
				prefabChange();
			}
		}

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x060000B1 RID: 177 RVA: 0x00003CE0 File Offset: 0x00001EE0
		// (remove) Token: 0x060000B2 RID: 178 RVA: 0x00003D18 File Offset: 0x00001F18
		public event Action PrefabChange;

		// Token: 0x04000032 RID: 50
		private Dictionary<string, Type> _builtinTypes;

		// Token: 0x04000033 RID: 51
		private Dictionary<string, string> _customTypePaths;

		// Token: 0x04000034 RID: 52
		private Dictionary<string, CustomWidgetType> _liveCustomTypes;

		// Token: 0x04000035 RID: 53
		private Dictionary<string, int> _liveInstanceTracker;

		// Token: 0x04000036 RID: 54
		private ResourceDepot _resourceDepot;

		// Token: 0x04000037 RID: 55
		private readonly string _resourceFolder;
	}
}
