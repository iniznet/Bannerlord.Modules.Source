using System;
using System.Collections.Generic;

namespace TaleWorlds.Library.CodeGeneration
{
	public class MethodCode
	{
		public string Comment { get; set; }

		public string Name { get; set; }

		public string MethodSignature { get; set; }

		public string ReturnParameter { get; set; }

		public bool IsStatic { get; set; }

		public MethodCodeAccessModifier AccessModifier { get; set; }

		public MethodCodePolymorphismInfo PolymorphismInfo { get; set; }

		public MethodCode()
		{
			this.Name = "UnnamedMethod";
			this.MethodSignature = "()";
			this.PolymorphismInfo = MethodCodePolymorphismInfo.None;
			this.ReturnParameter = "void";
			this._lines = new List<string>();
		}

		public void GenerateInto(CodeGenerationFile codeGenerationFile)
		{
			string text = "";
			if (this.AccessModifier == MethodCodeAccessModifier.Public)
			{
				text += "public ";
			}
			else if (this.AccessModifier == MethodCodeAccessModifier.Protected)
			{
				text += "protected ";
			}
			else if (this.AccessModifier == MethodCodeAccessModifier.Private)
			{
				text += "private ";
			}
			else if (this.AccessModifier == MethodCodeAccessModifier.Internal)
			{
				text += "internal ";
			}
			if (this.IsStatic)
			{
				text += "static ";
			}
			if (this.PolymorphismInfo == MethodCodePolymorphismInfo.Virtual)
			{
				text += "virtual ";
			}
			else if (this.PolymorphismInfo == MethodCodePolymorphismInfo.Override)
			{
				text += "override ";
			}
			text = string.Concat(new string[] { text, this.ReturnParameter, " ", this.Name, this.MethodSignature });
			if (!string.IsNullOrEmpty(this.Comment))
			{
				codeGenerationFile.AddLine(this.Comment);
			}
			codeGenerationFile.AddLine(text);
			codeGenerationFile.AddLine("{");
			foreach (string text2 in this._lines)
			{
				codeGenerationFile.AddLine(text2);
			}
			codeGenerationFile.AddLine("}");
		}

		public void AddLine(string line)
		{
			this._lines.Add(line);
		}

		public void AddLines(IEnumerable<string> lines)
		{
			foreach (string text in lines)
			{
				this._lines.Add(text);
			}
		}

		public void AddCodeBlock(CodeBlock codeBlock)
		{
			this.AddLines(codeBlock.Lines);
		}

		private List<string> _lines;
	}
}
