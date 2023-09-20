using System;

namespace TaleWorlds.Library.CodeGeneration
{
	public class VariableCode
	{
		public string Name { get; set; }

		public string Type { get; set; }

		public bool IsStatic { get; set; }

		public VariableCodeAccessModifier AccessModifier { get; set; }

		public VariableCode()
		{
			this.Type = "System.Object";
			this.Name = "Unnamed variable";
			this.IsStatic = false;
			this.AccessModifier = VariableCodeAccessModifier.Private;
		}

		public string GenerateLine()
		{
			string text = "";
			if (this.AccessModifier == VariableCodeAccessModifier.Public)
			{
				text += "public ";
			}
			else if (this.AccessModifier == VariableCodeAccessModifier.Protected)
			{
				text += "protected ";
			}
			else if (this.AccessModifier == VariableCodeAccessModifier.Private)
			{
				text += "private ";
			}
			else if (this.AccessModifier == VariableCodeAccessModifier.Internal)
			{
				text += "internal ";
			}
			if (this.IsStatic)
			{
				text += "static ";
			}
			return string.Concat(new string[] { text, this.Type, " ", this.Name, ";" });
		}
	}
}
