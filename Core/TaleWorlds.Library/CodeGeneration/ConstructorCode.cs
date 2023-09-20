using System;
using System.Collections.Generic;

namespace TaleWorlds.Library.CodeGeneration
{
	// Token: 0x020000B6 RID: 182
	public class ConstructorCode
	{
		// Token: 0x170000D9 RID: 217
		// (get) Token: 0x0600067F RID: 1663 RVA: 0x00014168 File Offset: 0x00012368
		// (set) Token: 0x06000680 RID: 1664 RVA: 0x00014170 File Offset: 0x00012370
		public string Name { get; set; }

		// Token: 0x170000DA RID: 218
		// (get) Token: 0x06000681 RID: 1665 RVA: 0x00014179 File Offset: 0x00012379
		// (set) Token: 0x06000682 RID: 1666 RVA: 0x00014181 File Offset: 0x00012381
		public string MethodSignature { get; set; }

		// Token: 0x170000DB RID: 219
		// (get) Token: 0x06000683 RID: 1667 RVA: 0x0001418A File Offset: 0x0001238A
		// (set) Token: 0x06000684 RID: 1668 RVA: 0x00014192 File Offset: 0x00012392
		public string BaseCall { get; set; }

		// Token: 0x170000DC RID: 220
		// (get) Token: 0x06000685 RID: 1669 RVA: 0x0001419B File Offset: 0x0001239B
		// (set) Token: 0x06000686 RID: 1670 RVA: 0x000141A3 File Offset: 0x000123A3
		public bool IsStatic { get; set; }

		// Token: 0x170000DD RID: 221
		// (get) Token: 0x06000687 RID: 1671 RVA: 0x000141AC File Offset: 0x000123AC
		// (set) Token: 0x06000688 RID: 1672 RVA: 0x000141B4 File Offset: 0x000123B4
		public MethodCodeAccessModifier AccessModifier { get; set; }

		// Token: 0x06000689 RID: 1673 RVA: 0x000141BD File Offset: 0x000123BD
		public ConstructorCode()
		{
			this.Name = "UnassignedConstructorName";
			this.MethodSignature = "()";
			this.BaseCall = "";
			this._lines = new List<string>();
		}

		// Token: 0x0600068A RID: 1674 RVA: 0x000141F4 File Offset: 0x000123F4
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

		// Token: 0x0600068B RID: 1675 RVA: 0x0001430C File Offset: 0x0001250C
		public void AddLine(string line)
		{
			this._lines.Add(line);
		}

		// Token: 0x040001FF RID: 511
		private List<string> _lines;
	}
}
