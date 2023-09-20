using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.GauntletUI.PrefabSystem
{
	public class WidgetFactory
	{
		public PrefabExtensionContext PrefabExtensionContext { get; private set; }

		public WidgetAttributeContext WidgetAttributeContext { get; private set; }

		public GeneratedPrefabContext GeneratedPrefabContext { get; private set; }

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

		private void OnResourceChange()
		{
			this.CheckForUpdates();
		}

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

		public void AddCustomType(string name, string path)
		{
			this._customTypePaths.Add(name, path);
		}

		public IEnumerable<string> GetPrefabNames()
		{
			return this._customTypePaths.Keys;
		}

		public IEnumerable<string> GetWidgetTypes()
		{
			return this._builtinTypes.Keys.Concat(this._customTypePaths.Keys);
		}

		public bool IsBuiltinType(string name)
		{
			return this._builtinTypes.ContainsKey(name);
		}

		public Type GetBuiltinType(string name)
		{
			return this._builtinTypes[name];
		}

		public bool IsCustomType(string typeName)
		{
			return this._customTypePaths.ContainsKey(typeName);
		}

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

		public event Action PrefabChange;

		private Dictionary<string, Type> _builtinTypes;

		private Dictionary<string, string> _customTypePaths;

		private Dictionary<string, CustomWidgetType> _liveCustomTypes;

		private Dictionary<string, int> _liveInstanceTracker;

		private ResourceDepot _resourceDepot;

		private readonly string _resourceFolder;
	}
}
