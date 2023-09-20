using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.Library;

namespace TaleWorlds.GauntletUI.PrefabSystem
{
	// Token: 0x02000005 RID: 5
	public class GeneratedPrefabContext
	{
		// Token: 0x06000027 RID: 39 RVA: 0x00002591 File Offset: 0x00000791
		public GeneratedPrefabContext()
		{
			this._generatedPrefabs = new Dictionary<string, Dictionary<string, CreateGeneratedWidget>>();
			this._prefabCreators = new List<IGeneratedUIPrefabCreator>();
		}

		// Token: 0x06000028 RID: 40 RVA: 0x000025B0 File Offset: 0x000007B0
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

		// Token: 0x06000029 RID: 41 RVA: 0x00002620 File Offset: 0x00000820
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

		// Token: 0x0600002A RID: 42 RVA: 0x00002688 File Offset: 0x00000888
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

		// Token: 0x0600002B RID: 43 RVA: 0x00002724 File Offset: 0x00000924
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

		// Token: 0x0600002C RID: 44 RVA: 0x00002804 File Offset: 0x00000A04
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

		// Token: 0x04000019 RID: 25
		private Assembly[] _assemblies;

		// Token: 0x0400001A RID: 26
		private List<IGeneratedUIPrefabCreator> _prefabCreators;

		// Token: 0x0400001B RID: 27
		private Dictionary<string, Dictionary<string, CreateGeneratedWidget>> _generatedPrefabs;
	}
}
