using System;
using System.Collections.Generic;

namespace TaleWorlds.Library.CodeGeneration
{
	// Token: 0x020000B3 RID: 179
	public class CodeGenerationContext
	{
		// Token: 0x170000D8 RID: 216
		// (get) Token: 0x06000674 RID: 1652 RVA: 0x00013E92 File Offset: 0x00012092
		// (set) Token: 0x06000675 RID: 1653 RVA: 0x00013E9A File Offset: 0x0001209A
		public List<NamespaceCode> Namespaces { get; private set; }

		// Token: 0x06000676 RID: 1654 RVA: 0x00013EA3 File Offset: 0x000120A3
		public CodeGenerationContext()
		{
			this.Namespaces = new List<NamespaceCode>();
		}

		// Token: 0x06000677 RID: 1655 RVA: 0x00013EB8 File Offset: 0x000120B8
		public NamespaceCode FindOrCreateNamespace(string name)
		{
			foreach (NamespaceCode namespaceCode in this.Namespaces)
			{
				if (namespaceCode.Name == name)
				{
					return namespaceCode;
				}
			}
			NamespaceCode namespaceCode2 = new NamespaceCode();
			namespaceCode2.Name = name;
			this.Namespaces.Add(namespaceCode2);
			return namespaceCode2;
		}

		// Token: 0x06000678 RID: 1656 RVA: 0x00013F34 File Offset: 0x00012134
		public void GenerateInto(CodeGenerationFile codeGenerationFile)
		{
			foreach (NamespaceCode namespaceCode in this.Namespaces)
			{
				namespaceCode.GenerateInto(codeGenerationFile);
				codeGenerationFile.AddLine("");
			}
		}
	}
}
