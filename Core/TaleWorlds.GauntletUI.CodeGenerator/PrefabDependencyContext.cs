using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI.PrefabSystem;
using TaleWorlds.Library.CodeGeneration;

namespace TaleWorlds.GauntletUI.CodeGenerator
{
	// Token: 0x0200000A RID: 10
	public class PrefabDependencyContext
	{
		// Token: 0x17000029 RID: 41
		// (get) Token: 0x0600006D RID: 109 RVA: 0x000035E5 File Offset: 0x000017E5
		// (set) Token: 0x0600006E RID: 110 RVA: 0x000035ED File Offset: 0x000017ED
		public string RootClassName { get; private set; }

		// Token: 0x0600006F RID: 111 RVA: 0x000035F6 File Offset: 0x000017F6
		public PrefabDependencyContext(string rootClassName)
		{
			this.RootClassName = rootClassName;
			this._prefabDependencies = new List<PrefabDependency>();
		}

		// Token: 0x06000070 RID: 112 RVA: 0x00003610 File Offset: 0x00001810
		public string GenerateDependencyName()
		{
			this._dependencyIndex++;
			return this.RootClassName + "_Dependency_" + this._dependencyIndex;
		}

		// Token: 0x06000071 RID: 113 RVA: 0x0000363C File Offset: 0x0000183C
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

		// Token: 0x06000072 RID: 114 RVA: 0x000036C8 File Offset: 0x000018C8
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

		// Token: 0x06000073 RID: 115 RVA: 0x00003764 File Offset: 0x00001964
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

		// Token: 0x06000074 RID: 116 RVA: 0x00003844 File Offset: 0x00001A44
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

		// Token: 0x06000075 RID: 117 RVA: 0x000038D4 File Offset: 0x00001AD4
		public void GenerateInto(NamespaceCode namespaceCode)
		{
			for (int i = 0; i < this._prefabDependencies.Count; i++)
			{
				this._prefabDependencies[i].WidgetTemplateGenerateContext.GenerateInto(namespaceCode);
			}
		}

		// Token: 0x06000076 RID: 118 RVA: 0x0000390E File Offset: 0x00001B0E
		public bool ContainsDependency(string type, Dictionary<string, WidgetAttributeTemplate> givenParameters, Dictionary<string, object> data, bool isRoot)
		{
			return this.GetDependendPrefab(type, givenParameters, data, isRoot) != null;
		}

		// Token: 0x0400002C RID: 44
		private List<PrefabDependency> _prefabDependencies;

		// Token: 0x0400002D RID: 45
		private int _dependencyIndex;
	}
}
