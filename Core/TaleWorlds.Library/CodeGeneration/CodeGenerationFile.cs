using System;
using System.Collections.Generic;
using System.Text;

namespace TaleWorlds.Library.CodeGeneration
{
	// Token: 0x020000B4 RID: 180
	public class CodeGenerationFile
	{
		// Token: 0x06000679 RID: 1657 RVA: 0x00013F90 File Offset: 0x00012190
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

		// Token: 0x0600067A RID: 1658 RVA: 0x0001400C File Offset: 0x0001220C
		public void AddLine(string line)
		{
			this._lines.Add(line);
		}

		// Token: 0x0600067B RID: 1659 RVA: 0x0001401C File Offset: 0x0001221C
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

		// Token: 0x040001F8 RID: 504
		private List<string> _lines;
	}
}
