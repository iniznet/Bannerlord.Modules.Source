using System;
using System.Collections.Generic;

namespace TaleWorlds.Library.CodeGeneration
{
	public class ClassCode
	{
		public string Name { get; set; }

		public bool IsGeneric { get; set; }

		public int GenericTypeCount { get; set; }

		public bool IsPartial { get; set; }

		public ClassCodeAccessModifier AccessModifier { get; set; }

		public bool IsClass { get; set; }

		public List<string> InheritedInterfaces { get; private set; }

		public List<ClassCode> NestedClasses { get; private set; }

		public List<MethodCode> Methods { get; private set; }

		public List<ConstructorCode> Constructors { get; private set; }

		public List<VariableCode> Variables { get; private set; }

		public CommentSection CommentSection { get; set; }

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

		public void AddVariable(VariableCode variableCode)
		{
			this.Variables.Add(variableCode);
		}

		public void AddNestedClass(ClassCode clasCode)
		{
			this.NestedClasses.Add(clasCode);
		}

		public void AddMethod(MethodCode methodCode)
		{
			this.Methods.Add(methodCode);
		}

		public void AddConsturctor(ConstructorCode constructorCode)
		{
			constructorCode.Name = this.Name;
			this.Constructors.Add(constructorCode);
		}

		public void AddInterface(string interfaceName)
		{
			this.InheritedInterfaces.Add(interfaceName);
		}
	}
}
