using System;
using System.Collections.Generic;

namespace TaleWorlds.Library.CodeGeneration
{
	// Token: 0x020000B8 RID: 184
	public class CodeBlock
	{
		// Token: 0x170000E5 RID: 229
		// (get) Token: 0x0600069F RID: 1695 RVA: 0x00014596 File Offset: 0x00012796
		public List<string> Lines
		{
			get
			{
				return this._lines;
			}
		}

		// Token: 0x060006A0 RID: 1696 RVA: 0x0001459E File Offset: 0x0001279E
		public CodeBlock()
		{
			this._lines = new List<string>();
		}

		// Token: 0x060006A1 RID: 1697 RVA: 0x000145B1 File Offset: 0x000127B1
		public void AddLine(string line)
		{
			this._lines.Add(line);
		}

		// Token: 0x060006A2 RID: 1698 RVA: 0x000145C0 File Offset: 0x000127C0
		public void AddLines(IEnumerable<string> lines)
		{
			foreach (string text in lines)
			{
				this._lines.Add(text);
			}
		}

		// Token: 0x04000208 RID: 520
		private List<string> _lines;
	}
}
