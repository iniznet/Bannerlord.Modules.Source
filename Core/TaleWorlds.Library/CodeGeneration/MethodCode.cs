using System;
using System.Collections.Generic;

namespace TaleWorlds.Library.CodeGeneration
{
	// Token: 0x020000B7 RID: 183
	public class MethodCode
	{
		// Token: 0x170000DE RID: 222
		// (get) Token: 0x0600068C RID: 1676 RVA: 0x0001431A File Offset: 0x0001251A
		// (set) Token: 0x0600068D RID: 1677 RVA: 0x00014322 File Offset: 0x00012522
		public string Comment { get; set; }

		// Token: 0x170000DF RID: 223
		// (get) Token: 0x0600068E RID: 1678 RVA: 0x0001432B File Offset: 0x0001252B
		// (set) Token: 0x0600068F RID: 1679 RVA: 0x00014333 File Offset: 0x00012533
		public string Name { get; set; }

		// Token: 0x170000E0 RID: 224
		// (get) Token: 0x06000690 RID: 1680 RVA: 0x0001433C File Offset: 0x0001253C
		// (set) Token: 0x06000691 RID: 1681 RVA: 0x00014344 File Offset: 0x00012544
		public string MethodSignature { get; set; }

		// Token: 0x170000E1 RID: 225
		// (get) Token: 0x06000692 RID: 1682 RVA: 0x0001434D File Offset: 0x0001254D
		// (set) Token: 0x06000693 RID: 1683 RVA: 0x00014355 File Offset: 0x00012555
		public string ReturnParameter { get; set; }

		// Token: 0x170000E2 RID: 226
		// (get) Token: 0x06000694 RID: 1684 RVA: 0x0001435E File Offset: 0x0001255E
		// (set) Token: 0x06000695 RID: 1685 RVA: 0x00014366 File Offset: 0x00012566
		public bool IsStatic { get; set; }

		// Token: 0x170000E3 RID: 227
		// (get) Token: 0x06000696 RID: 1686 RVA: 0x0001436F File Offset: 0x0001256F
		// (set) Token: 0x06000697 RID: 1687 RVA: 0x00014377 File Offset: 0x00012577
		public MethodCodeAccessModifier AccessModifier { get; set; }

		// Token: 0x170000E4 RID: 228
		// (get) Token: 0x06000698 RID: 1688 RVA: 0x00014380 File Offset: 0x00012580
		// (set) Token: 0x06000699 RID: 1689 RVA: 0x00014388 File Offset: 0x00012588
		public MethodCodePolymorphismInfo PolymorphismInfo { get; set; }

		// Token: 0x0600069A RID: 1690 RVA: 0x00014391 File Offset: 0x00012591
		public MethodCode()
		{
			this.Name = "UnnamedMethod";
			this.MethodSignature = "()";
			this.PolymorphismInfo = MethodCodePolymorphismInfo.None;
			this.ReturnParameter = "void";
			this._lines = new List<string>();
		}

		// Token: 0x0600069B RID: 1691 RVA: 0x000143CC File Offset: 0x000125CC
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

		// Token: 0x0600069C RID: 1692 RVA: 0x00014528 File Offset: 0x00012728
		public void AddLine(string line)
		{
			this._lines.Add(line);
		}

		// Token: 0x0600069D RID: 1693 RVA: 0x00014538 File Offset: 0x00012738
		public void AddLines(IEnumerable<string> lines)
		{
			foreach (string text in lines)
			{
				this._lines.Add(text);
			}
		}

		// Token: 0x0600069E RID: 1694 RVA: 0x00014588 File Offset: 0x00012788
		public void AddCodeBlock(CodeBlock codeBlock)
		{
			this.AddLines(codeBlock.Lines);
		}

		// Token: 0x04000207 RID: 519
		private List<string> _lines;
	}
}
