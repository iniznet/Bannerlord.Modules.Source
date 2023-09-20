using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace TaleWorlds.SaveSystem.Definition
{
	public class SaveCodeGenerationContext
	{
		public SaveCodeGenerationContext(DefinitionContext definitionContext)
		{
			this._definitionContext = definitionContext;
			this._assemblies = new Dictionary<Assembly, SaveCodeGenerationContextAssembly>();
		}

		public void AddAssembly(Assembly assembly, string defaultNamespace, string location, string fileName)
		{
			SaveCodeGenerationContextAssembly saveCodeGenerationContextAssembly = new SaveCodeGenerationContextAssembly(this._definitionContext, assembly, defaultNamespace, location, fileName);
			this._assemblies.Add(assembly, saveCodeGenerationContextAssembly);
		}

		internal SaveCodeGenerationContextAssembly FindAssemblyInformation(Assembly assembly)
		{
			SaveCodeGenerationContextAssembly saveCodeGenerationContextAssembly;
			this._assemblies.TryGetValue(assembly, out saveCodeGenerationContextAssembly);
			return saveCodeGenerationContextAssembly;
		}

		internal void FillFiles()
		{
			List<Tuple<string, string>> list = new List<Tuple<string, string>>();
			foreach (SaveCodeGenerationContextAssembly saveCodeGenerationContextAssembly in this._assemblies.Values)
			{
				saveCodeGenerationContextAssembly.Generate();
				string text = saveCodeGenerationContextAssembly.GenerateText();
				list.Add(new Tuple<string, string>(saveCodeGenerationContextAssembly.Location + saveCodeGenerationContextAssembly.FileName, text));
			}
			foreach (Tuple<string, string> tuple in list)
			{
				File.WriteAllText(tuple.Item1, tuple.Item2, Encoding.UTF8);
			}
		}

		private Dictionary<Assembly, SaveCodeGenerationContextAssembly> _assemblies;

		private DefinitionContext _definitionContext;
	}
}
