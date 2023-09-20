using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI.PrefabSystem;
using TaleWorlds.Library.CodeGeneration;

namespace TaleWorlds.GauntletUI.CodeGenerator
{
	public class PrefabDependencyContext
	{
		public string RootClassName { get; private set; }

		public PrefabDependencyContext(string rootClassName)
		{
			this.RootClassName = rootClassName;
			this._prefabDependencies = new List<PrefabDependency>();
		}

		public string GenerateDependencyName()
		{
			this._dependencyIndex++;
			return this.RootClassName + "_Dependency_" + this._dependencyIndex;
		}

		public void AddDependentWidgetTemplateGenerateContext(WidgetTemplateGenerateContext widgetTemplateGenerateContext)
		{
			if (widgetTemplateGenerateContext.ContextType == WidgetTemplateGenerateContextType.DependendPrefab)
			{
				PrefabDependency prefabDependency = new PrefabDependency(widgetTemplateGenerateContext.PrefabName, widgetTemplateGenerateContext.VariantName, false, widgetTemplateGenerateContext);
				this._prefabDependencies.Add(prefabDependency);
			}
			if (widgetTemplateGenerateContext.ContextType == WidgetTemplateGenerateContextType.InheritedDependendPrefab)
			{
				PrefabDependency prefabDependency2 = new PrefabDependency(widgetTemplateGenerateContext.PrefabName, widgetTemplateGenerateContext.VariantName, true, widgetTemplateGenerateContext);
				this._prefabDependencies.Add(prefabDependency2);
				return;
			}
			if (widgetTemplateGenerateContext.ContextType == WidgetTemplateGenerateContextType.CustomWidgetTemplate)
			{
				PrefabDependency prefabDependency3 = new PrefabDependency(widgetTemplateGenerateContext.ClassName, widgetTemplateGenerateContext.VariantName, false, widgetTemplateGenerateContext);
				this._prefabDependencies.Add(prefabDependency3);
			}
		}

		public PrefabDependency GetDependendPrefab(string type, Dictionary<string, WidgetAttributeTemplate> givenParameters, Dictionary<string, object> data, bool isRoot)
		{
			foreach (PrefabDependency prefabDependency in this._prefabDependencies)
			{
				if (prefabDependency.Type == type && prefabDependency.IsRoot == isRoot)
				{
					Dictionary<string, WidgetAttributeTemplate> givenParameters2 = prefabDependency.WidgetTemplateGenerateContext.VariableCollection.GivenParameters;
					Dictionary<string, object> data2 = prefabDependency.WidgetTemplateGenerateContext.Data;
					if (PrefabDependencyContext.CompareGivenParameters(givenParameters, givenParameters2) && PrefabDependencyContext.CompareData(data, data2))
					{
						return prefabDependency;
					}
				}
			}
			return null;
		}

		private static bool CompareGivenParameters(Dictionary<string, WidgetAttributeTemplate> a, Dictionary<string, WidgetAttributeTemplate> b)
		{
			if (a.Count != b.Count)
			{
				return false;
			}
			foreach (KeyValuePair<string, WidgetAttributeTemplate> keyValuePair in a)
			{
				WidgetAttributeTemplate value = keyValuePair.Value;
				if (!b.ContainsKey(keyValuePair.Key))
				{
					return false;
				}
				WidgetAttributeTemplate widgetAttributeTemplate = b[keyValuePair.Key];
				if (value.Value != widgetAttributeTemplate.Value || value.KeyType.GetType() != widgetAttributeTemplate.KeyType.GetType() || value.ValueType.GetType() != widgetAttributeTemplate.ValueType.GetType())
				{
					return false;
				}
			}
			return true;
		}

		private static bool CompareData(Dictionary<string, object> a, Dictionary<string, object> b)
		{
			if (a.Count != b.Count)
			{
				return false;
			}
			foreach (KeyValuePair<string, object> keyValuePair in a)
			{
				object value = keyValuePair.Value;
				if (!b.ContainsKey(keyValuePair.Key))
				{
					return false;
				}
				object obj = b[keyValuePair.Key];
				if (value != obj)
				{
					return false;
				}
			}
			return true;
		}

		public void GenerateInto(NamespaceCode namespaceCode)
		{
			for (int i = 0; i < this._prefabDependencies.Count; i++)
			{
				this._prefabDependencies[i].WidgetTemplateGenerateContext.GenerateInto(namespaceCode);
			}
		}

		public bool ContainsDependency(string type, Dictionary<string, WidgetAttributeTemplate> givenParameters, Dictionary<string, object> data, bool isRoot)
		{
			return this.GetDependendPrefab(type, givenParameters, data, isRoot) != null;
		}

		private List<PrefabDependency> _prefabDependencies;

		private int _dependencyIndex;
	}
}
