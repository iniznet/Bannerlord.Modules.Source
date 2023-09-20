using System;

namespace TaleWorlds.Library.CodeGeneration
{
	// Token: 0x020000BC RID: 188
	public class VariableCode
	{
		// Token: 0x170000E8 RID: 232
		// (get) Token: 0x060006AA RID: 1706 RVA: 0x000146DE File Offset: 0x000128DE
		// (set) Token: 0x060006AB RID: 1707 RVA: 0x000146E6 File Offset: 0x000128E6
		public string Name { get; set; }

		// Token: 0x170000E9 RID: 233
		// (get) Token: 0x060006AC RID: 1708 RVA: 0x000146EF File Offset: 0x000128EF
		// (set) Token: 0x060006AD RID: 1709 RVA: 0x000146F7 File Offset: 0x000128F7
		public string Type { get; set; }

		// Token: 0x170000EA RID: 234
		// (get) Token: 0x060006AE RID: 1710 RVA: 0x00014700 File Offset: 0x00012900
		// (set) Token: 0x060006AF RID: 1711 RVA: 0x00014708 File Offset: 0x00012908
		public bool IsStatic { get; set; }

		// Token: 0x170000EB RID: 235
		// (get) Token: 0x060006B0 RID: 1712 RVA: 0x00014711 File Offset: 0x00012911
		// (set) Token: 0x060006B1 RID: 1713 RVA: 0x00014719 File Offset: 0x00012919
		public VariableCodeAccessModifier AccessModifier { get; set; }

		// Token: 0x060006B2 RID: 1714 RVA: 0x00014722 File Offset: 0x00012922
		public VariableCode()
		{
			this.Type = "System.Object";
			this.Name = "Unnamed variable";
			this.IsStatic = false;
			this.AccessModifier = VariableCodeAccessModifier.Private;
		}

		// Token: 0x060006B3 RID: 1715 RVA: 0x00014750 File Offset: 0x00012950
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
