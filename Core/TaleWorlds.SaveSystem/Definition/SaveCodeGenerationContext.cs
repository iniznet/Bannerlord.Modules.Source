using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace TaleWorlds.SaveSystem.Definition
{
	// Token: 0x02000065 RID: 101
	public class SaveCodeGenerationContext
	{
		// Token: 0x060002F9 RID: 761 RVA: 0x0000C1C1 File Offset: 0x0000A3C1
		public SaveCodeGenerationContext(DefinitionContext definitionContext)
		{
			this._definitionContext = definitionContext;
			this._assemblies = new Dictionary<Assembly, SaveCodeGenerationContextAssembly>();
		}

		// Token: 0x060002FA RID: 762 RVA: 0x0000C1DC File Offset: 0x0000A3DC
		public void AddAssembly(Assembly assembly, string defaultNamespace, string location, string fileName)
		{
			SaveCodeGenerationContextAssembly saveCodeGenerationContextAssembly = new SaveCodeGenerationContextAssembly(this._definitionContext, assembly, defaultNamespace, location, fileName);
			this._assemblies.Add(assembly, saveCodeGenerationContextAssembly);
		}

		// Token: 0x060002FB RID: 763 RVA: 0x0000C208 File Offset: 0x0000A408
		internal SaveCodeGenerationContextAssembly FindAssemblyInformation(Assembly assembly)
		{
			SaveCodeGenerationContextAssembly saveCodeGenerationContextAssembly;
			this._assemblies.TryGetValue(assembly, out saveCodeGenerationContextAssembly);
			return saveCodeGenerationContextAssembly;
		}

		// Token: 0x060002FC RID: 764 RVA: 0x0000C228 File Offset: 0x0000A428
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

		// Token: 0x040000EF RID: 239
		private Dictionary<Assembly, SaveCodeGenerationContextAssembly> _assemblies;

		// Token: 0x040000F0 RID: 240
		private DefinitionContext _definitionContext;
	}
}
