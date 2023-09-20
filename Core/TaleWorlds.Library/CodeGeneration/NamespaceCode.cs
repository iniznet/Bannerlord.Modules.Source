using System;
using System.Collections.Generic;

namespace TaleWorlds.Library.CodeGeneration
{
	public class NamespaceCode
	{
		public string Name { get; set; }

		public List<ClassCode> Classes { get; private set; }

		public NamespaceCode()
		{
			this.Classes = new List<ClassCode>();
		}

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

		public void AddClass(ClassCode clasCode)
		{
			this.Classes.Add(clasCode);
		}
	}
}
