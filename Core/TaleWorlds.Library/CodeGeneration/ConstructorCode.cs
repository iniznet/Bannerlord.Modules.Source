using System;
using System.Collections.Generic;

namespace TaleWorlds.Library.CodeGeneration
{
	public class ConstructorCode
	{
		public string Name { get; set; }

		public string MethodSignature { get; set; }

		public string BaseCall { get; set; }

		public bool IsStatic { get; set; }

		public MethodCodeAccessModifier AccessModifier { get; set; }

		public ConstructorCode()
		{
			this.Name = "UnassignedConstructorName";
			this.MethodSignature = "()";
			this.BaseCall = "";
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
			text = text + this.Name + this.MethodSignature;
			if (!string.IsNullOrEmpty(this.BaseCall))
			{
				text = text + " : base" + this.BaseCall;
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

		private List<string> _lines;
	}
}
