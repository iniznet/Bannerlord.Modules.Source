using System;
using System.Collections.Generic;

namespace TaleWorlds.Library.CodeGeneration
{
	// Token: 0x020000B5 RID: 181
	public class CommentSection
	{
		// Token: 0x0600067C RID: 1660 RVA: 0x000140E4 File Offset: 0x000122E4
		public CommentSection()
		{
			this._lines = new List<string>();
		}

		// Token: 0x0600067D RID: 1661 RVA: 0x000140F7 File Offset: 0x000122F7
		public void AddCommentLine(string line)
		{
			this._lines.Add(line);
		}

		// Token: 0x0600067E RID: 1662 RVA: 0x00014108 File Offset: 0x00012308
		public void GenerateInto(CodeGenerationFile codeGenerationFile)
		{
			foreach (string text in this._lines)
			{
				codeGenerationFile.AddLine("//" + text);
			}
		}

		// Token: 0x040001F9 RID: 505
		private List<string> _lines;
	}
}
