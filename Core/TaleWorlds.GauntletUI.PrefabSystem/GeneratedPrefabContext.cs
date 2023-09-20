using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.Library;

namespace TaleWorlds.GauntletUI.PrefabSystem
{
	public class GeneratedPrefabContext
	{
		public GeneratedPrefabContext()
		{
			this._generatedPrefabs = new Dictionary<string, Dictionary<string, CreateGeneratedWidget>>();
			this._prefabCreators = new List<IGeneratedUIPrefabCreator>();
		}

		public void CollectPrefabs()
		{
			this._generatedPrefabs.Clear();
			this._assemblies = GeneratedPrefabContext.GetPrefabAssemblies();
			this.FindGeneratedPrefabCreators();
			foreach (IGeneratedUIPrefabCreator generatedUIPrefabCreator in this._prefabCreators)
			{
				generatedUIPrefabCreator.CollectGeneratedPrefabDefinitions(this);
			}
		}

		public void AddGeneratedPrefab(string prefabName, string variantName, CreateGeneratedWidget creator)
		{
			if (!this._generatedPrefabs.ContainsKey(prefabName))
			{
				this._generatedPrefabs.Add(prefabName, new Dictionary<string, CreateGeneratedWidget>());
			}
			if (!this._generatedPrefabs[prefabName].ContainsKey(variantName))
			{
				this._generatedPrefabs[prefabName].Add(variantName, creator);
				return;
			}
			this._generatedPrefabs[prefabName][variantName] = creator;
		}

		private static Assembly[] GetPrefabAssemblies()
		{
			List<Assembly> list = new List<Assembly>();
			Assembly assembly = typeof(WidgetPrefab).Assembly;
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			list.Add(assembly);
			foreach (Assembly assembly2 in assemblies)
			{
				if (assembly2 != assembly)
				{
					AssemblyName[] referencedAssemblies = assembly2.GetReferencedAssemblies();
					for (int j = 0; j < referencedAssemblies.Length; j++)
					{
						if (referencedAssemblies[j].ToString() == assembly.GetName().ToString())
						{
							list.Add(assembly2);
							break;
						}
					}
				}
			}
			return list.ToArray();
		}

		private void FindGeneratedPrefabCreators()
		{
			this._prefabCreators.Clear();
			foreach (Assembly assembly in this._assemblies)
			{
				Type[] array = new Type[0];
				try
				{
					array = assembly.GetTypes();
				}
				catch (Exception ex)
				{
					Debug.Print(ex.Message, 0, Debug.DebugColor.White, 17592186044416UL);
					Debug.Print(ex.StackTrace, 0, Debug.DebugColor.White, 17592186044416UL);
				}
				foreach (Type type in array)
				{
					if (typeof(IGeneratedUIPrefabCreator).IsAssignableFrom(type) && typeof(IGeneratedUIPrefabCreator) != type)
					{
						IGeneratedUIPrefabCreator generatedUIPrefabCreator = (IGeneratedUIPrefabCreator)Activator.CreateInstance(type);
						this._prefabCreators.Add(generatedUIPrefabCreator);
					}
				}
			}
		}

		public GeneratedPrefabInstantiationResult InstantiatePrefab(UIContext conext, string prefabName, string variantName, Dictionary<string, object> data)
		{
			Dictionary<string, CreateGeneratedWidget> dictionary;
			CreateGeneratedWidget createGeneratedWidget;
			if (this._generatedPrefabs.TryGetValue(prefabName, out dictionary) && dictionary.TryGetValue(variantName, out createGeneratedWidget))
			{
				return createGeneratedWidget(conext, data);
			}
			return null;
		}

		private Assembly[] _assemblies;

		private List<IGeneratedUIPrefabCreator> _prefabCreators;

		private Dictionary<string, Dictionary<string, CreateGeneratedWidget>> _generatedPrefabs;
	}
}
