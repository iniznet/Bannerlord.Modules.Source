using System;
using System.Collections.Generic;

namespace TaleWorlds.Library.CodeGeneration
{
	// Token: 0x020000B1 RID: 177
	public class ClassCode
	{
		// Token: 0x170000CC RID: 204
		// (get) Token: 0x06000655 RID: 1621 RVA: 0x00013A31 File Offset: 0x00011C31
		// (set) Token: 0x06000656 RID: 1622 RVA: 0x00013A39 File Offset: 0x00011C39
		public string Name { get; set; }

		// Token: 0x170000CD RID: 205
		// (get) Token: 0x06000657 RID: 1623 RVA: 0x00013A42 File Offset: 0x00011C42
		// (set) Token: 0x06000658 RID: 1624 RVA: 0x00013A4A File Offset: 0x00011C4A
		public bool IsGeneric { get; set; }

		// Token: 0x170000CE RID: 206
		// (get) Token: 0x06000659 RID: 1625 RVA: 0x00013A53 File Offset: 0x00011C53
		// (set) Token: 0x0600065A RID: 1626 RVA: 0x00013A5B File Offset: 0x00011C5B
		public int GenericTypeCount { get; set; }

		// Token: 0x170000CF RID: 207
		// (get) Token: 0x0600065B RID: 1627 RVA: 0x00013A64 File Offset: 0x00011C64
		// (set) Token: 0x0600065C RID: 1628 RVA: 0x00013A6C File Offset: 0x00011C6C
		public bool IsPartial { get; set; }

		// Token: 0x170000D0 RID: 208
		// (get) Token: 0x0600065D RID: 1629 RVA: 0x00013A75 File Offset: 0x00011C75
		// (set) Token: 0x0600065E RID: 1630 RVA: 0x00013A7D File Offset: 0x00011C7D
		public ClassCodeAccessModifier AccessModifier { get; set; }

		// Token: 0x170000D1 RID: 209
		// (get) Token: 0x0600065F RID: 1631 RVA: 0x00013A86 File Offset: 0x00011C86
		// (set) Token: 0x06000660 RID: 1632 RVA: 0x00013A8E File Offset: 0x00011C8E
		public bool IsClass { get; set; }

		// Token: 0x170000D2 RID: 210
		// (get) Token: 0x06000661 RID: 1633 RVA: 0x00013A97 File Offset: 0x00011C97
		// (set) Token: 0x06000662 RID: 1634 RVA: 0x00013A9F File Offset: 0x00011C9F
		public List<string> InheritedInterfaces { get; private set; }

		// Token: 0x170000D3 RID: 211
		// (get) Token: 0x06000663 RID: 1635 RVA: 0x00013AA8 File Offset: 0x00011CA8
		// (set) Token: 0x06000664 RID: 1636 RVA: 0x00013AB0 File Offset: 0x00011CB0
		public List<ClassCode> NestedClasses { get; private set; }

		// Token: 0x170000D4 RID: 212
		// (get) Token: 0x06000665 RID: 1637 RVA: 0x00013AB9 File Offset: 0x00011CB9
		// (set) Token: 0x06000666 RID: 1638 RVA: 0x00013AC1 File Offset: 0x00011CC1
		public List<MethodCode> Methods { get; private set; }

		// Token: 0x170000D5 RID: 213
		// (get) Token: 0x06000667 RID: 1639 RVA: 0x00013ACA File Offset: 0x00011CCA
		// (set) Token: 0x06000668 RID: 1640 RVA: 0x00013AD2 File Offset: 0x00011CD2
		public List<ConstructorCode> Constructors { get; private set; }

		// Token: 0x170000D6 RID: 214
		// (get) Token: 0x06000669 RID: 1641 RVA: 0x00013ADB File Offset: 0x00011CDB
		// (set) Token: 0x0600066A RID: 1642 RVA: 0x00013AE3 File Offset: 0x00011CE3
		public List<VariableCode> Variables { get; private set; }

		// Token: 0x170000D7 RID: 215
		// (get) Token: 0x0600066B RID: 1643 RVA: 0x00013AEC File Offset: 0x00011CEC
		// (set) Token: 0x0600066C RID: 1644 RVA: 0x00013AF4 File Offset: 0x00011CF4
		public CommentSection CommentSection { get; set; }

		// Token: 0x0600066D RID: 1645 RVA: 0x00013B00 File Offset: 0x00011D00
		public ClassCode()
		{
			this.IsClass = true;
			this.IsGeneric = false;
			this.GenericTypeCount = 0;
			this.InheritedInterfaces = new List<string>();
			this.NestedClasses = new List<ClassCode>();
			this.Methods = new List<MethodCode>();
			this.Constructors = new List<ConstructorCode>();
			this.Variables = new List<VariableCode>();
			this.AccessModifier = ClassCodeAccessModifier.DoNotMention;
			this.Name = "UnnamedClass";
			this.CommentSection = null;
		}

		// Token: 0x0600066E RID: 1646 RVA: 0x00013B78 File Offset: 0x00011D78
		public void GenerateInto(CodeGenerationFile codeGenerationFile)
		{
			if (this.CommentSection != null)
			{
				this.CommentSection.GenerateInto(codeGenerationFile);
			}
			string text = "";
			if (this.AccessModifier == ClassCodeAccessModifier.Public)
			{
				text += "public ";
			}
			else if (this.AccessModifier == ClassCodeAccessModifier.Internal)
			{
				text += "internal ";
			}
			if (this.IsPartial)
			{
				text += "partial ";
			}
			string text2 = "class";
			if (!this.IsClass)
			{
				text2 = "struct";
			}
			text = text + text2 + " " + this.Name;
			if (this.InheritedInterfaces.Count > 0)
			{
				text += " : ";
				for (int i = 0; i < this.InheritedInterfaces.Count; i++)
				{
					string text3 = this.InheritedInterfaces[i];
					text = text + " " + text3;
					if (i + 1 != this.InheritedInterfaces.Count)
					{
						text += ", ";
					}
				}
			}
			if (this.IsGeneric)
			{
				text += "<";
				for (int j = 0; j < this.GenericTypeCount; j++)
				{
					if (this.GenericTypeCount == 1)
					{
						text += "T";
					}
					else
					{
						text = text + "T" + j;
					}
					if (j + 1 != this.GenericTypeCount)
					{
						text += ", ";
					}
				}
				text += ">";
			}
			codeGenerationFile.AddLine(text);
			codeGenerationFile.AddLine("{");
			foreach (ClassCode classCode in this.NestedClasses)
			{
				classCode.GenerateInto(codeGenerationFile);
			}
			foreach (VariableCode variableCode in this.Variables)
			{
				string text4 = variableCode.GenerateLine();
				codeGenerationFile.AddLine(text4);
			}
			if (this.Variables.Count > 0)
			{
				codeGenerationFile.AddLine("");
			}
			foreach (ConstructorCode constructorCode in this.Constructors)
			{
				constructorCode.GenerateInto(codeGenerationFile);
				codeGenerationFile.AddLine("");
			}
			foreach (MethodCode methodCode in this.Methods)
			{
				methodCode.GenerateInto(codeGenerationFile);
				codeGenerationFile.AddLine("");
			}
			codeGenerationFile.AddLine("}");
		}

		// Token: 0x0600066F RID: 1647 RVA: 0x00013E40 File Offset: 0x00012040
		public void AddVariable(VariableCode variableCode)
		{
			this.Variables.Add(variableCode);
		}

		// Token: 0x06000670 RID: 1648 RVA: 0x00013E4E File Offset: 0x0001204E
		public void AddNestedClass(ClassCode clasCode)
		{
			this.NestedClasses.Add(clasCode);
		}

		// Token: 0x06000671 RID: 1649 RVA: 0x00013E5C File Offset: 0x0001205C
		public void AddMethod(MethodCode methodCode)
		{
			this.Methods.Add(methodCode);
		}

		// Token: 0x06000672 RID: 1650 RVA: 0x00013E6A File Offset: 0x0001206A
		public void AddConsturctor(ConstructorCode constructorCode)
		{
			constructorCode.Name = this.Name;
			this.Constructors.Add(constructorCode);
		}

		// Token: 0x06000673 RID: 1651 RVA: 0x00013E84 File Offset: 0x00012084
		public void AddInterface(string interfaceName)
		{
			this.InheritedInterfaces.Add(interfaceName);
		}
	}
}
