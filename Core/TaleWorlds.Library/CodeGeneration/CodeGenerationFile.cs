using System;
using System.Collections.Generic;
using System.Text;

namespace TaleWorlds.Library.CodeGeneration
{
	public class CodeGenerationFile
	{
		public CodeGenerationFile(List<string> usingDefinitions = null)
		{
			this._lines = new List<string>();
			if (usingDefinitions != null && usingDefinitions.Count > 0)
			{
				foreach (string text in usingDefinitions)
				{
					this.AddLine("using " + text + ";");
				}
			}
		}

		public void AddLine(string line)
		{
			this._lines.Add(line);
		}

		public string GenerateText()
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			foreach (string text in this._lines)
			{
				if (text == "}" || text == "};")
				{
					num--;
				}
				string text2 = "";
				for (int i = 0; i < num; i++)
				{
					text2 += "\t";
				}
				text2 = text2 + text + "\n";
				if (text == "{")
				{
					num++;
				}
				stringBuilder.Append(text2);
			}
			return stringBuilder.ToString();
		}

		private List<string> _lines;
	}
}
