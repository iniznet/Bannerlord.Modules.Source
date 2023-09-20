using System;
using System.Collections.Generic;

namespace TaleWorlds.Library.CodeGeneration
{
	// Token: 0x020000BB RID: 187
	public class NamespaceCode
	{
		// Token: 0x170000E6 RID: 230
		// (get) Token: 0x060006A3 RID: 1699 RVA: 0x00014610 File Offset: 0x00012810
		// (set) Token: 0x060006A4 RID: 1700 RVA: 0x00014618 File Offset: 0x00012818
		public string Name { get; set; }

		// Token: 0x170000E7 RID: 231
		// (get) Token: 0x060006A5 RID: 1701 RVA: 0x00014621 File Offset: 0x00012821
		// (set) Token: 0x060006A6 RID: 1702 RVA: 0x00014629 File Offset: 0x00012829
		public List<ClassCode> Classes { get; private set; }

		// Token: 0x060006A7 RID: 1703 RVA: 0x00014632 File Offset: 0x00012832
		public NamespaceCode()
		{
			this.Classes = new List<ClassCode>();
		}

		// Token: 0x060006A8 RID: 1704 RVA: 0x00014648 File Offset: 0x00012848
		public void GenerateInto(CodeGenerationFile codeGenerationFile)
		{
			codeGenerationFile.AddLine("namespace " + this.Name);
			codeGenerationFile.AddLine("{");
			foreach (ClassCode classCode in this.Classes)
			{
				classCode.GenerateInto(codeGenerationFile);
				codeGenerationFile.AddLine("");
			}
			codeGenerationFile.AddLine("}");
		}

		// Token: 0x060006A9 RID: 1705 RVA: 0x000146D0 File Offset: 0x000128D0
		public void AddClass(ClassCode clasCode)
		{
			this.Classes.Add(clasCode);
		}
	}
}
