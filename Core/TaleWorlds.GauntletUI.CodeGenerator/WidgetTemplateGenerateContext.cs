using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI.PrefabSystem;
using TaleWorlds.Library.CodeGeneration;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.CodeGenerator
{
	public class WidgetTemplateGenerateContext
	{
		public WidgetFactory WidgetFactory { get; private set; }

		public UICodeGenerationContext UICodeGenerationContext { get; private set; }

		public VariableCollection VariableCollection { get; private set; }

		public string PrefabName { get; private set; }

		public string VariantName { get; private set; }

		public string ClassName { get; private set; }

		public Dictionary<string, object> Data { get; private set; }

		public List<WidgetCodeGenerationInfo> WidgetCodeGenerationInformations { get; private set; }

		public PrefabDependencyContext PrefabDependencyContext { get; private set; }

		public WidgetTemplateGenerateContextType ContextType { get; private set; }

		public bool GotLogicalChildrenLocation { get; private set; }

		private WidgetTemplateGenerateContext(UICodeGenerationContext uiCodeGenerationContext, PrefabDependencyContext prefabDependencyContext, WidgetTemplateGenerateContextType contextType, string className)
		{
			this.UICodeGenerationContext = uiCodeGenerationContext;
			this.WidgetFactory = this.UICodeGenerationContext.WidgetFactory;
			BrushFactory brushFactory = this.UICodeGenerationContext.BrushFactory;
			SpriteData spriteData = this.UICodeGenerationContext.SpriteData;
			this.VariableCollection = new VariableCollection(this.WidgetFactory, brushFactory, spriteData);
			this.WidgetCodeGenerationInformations = new List<WidgetCodeGenerationInfo>();
			this.ClassName = className;
			this.PrefabDependencyContext = prefabDependencyContext;
			this.ContextType = contextType;
		}

		public static WidgetTemplateGenerateContext CreateAsRoot(UICodeGenerationContext uiCodeGenerationContext, string prefabName, string variantName, UICodeGenerationVariantExtension variantExtension, Dictionary<string, object> data)
		{
			string useablePrefabClassName = WidgetTemplateGenerateContext.GetUseablePrefabClassName(prefabName, variantName);
			PrefabDependencyContext prefabDependencyContext = new PrefabDependencyContext(useablePrefabClassName);
			WidgetTemplateGenerateContext widgetTemplateGenerateContext = new WidgetTemplateGenerateContext(uiCodeGenerationContext, prefabDependencyContext, WidgetTemplateGenerateContextType.RootPrefab, useablePrefabClassName);
			widgetTemplateGenerateContext.PrepareAsPrefab(prefabName, variantName, variantExtension, data, new Dictionary<string, WidgetAttributeTemplate>());
			return widgetTemplateGenerateContext;
		}

		public static WidgetTemplateGenerateContext CreateAsDependendPrefab(UICodeGenerationContext uiCodeGenerationContext, PrefabDependencyContext prefabDependencyContext, string prefabName, string variantName, UICodeGenerationVariantExtension variantExtension, Dictionary<string, object> data, Dictionary<string, WidgetAttributeTemplate> givenParameters)
		{
			string text = prefabDependencyContext.GenerateDependencyName() + "_" + WidgetTemplateGenerateContext.GetUseablePrefabClassName(prefabName, variantName);
			WidgetTemplateGenerateContext widgetTemplateGenerateContext = new WidgetTemplateGenerateContext(uiCodeGenerationContext, prefabDependencyContext, WidgetTemplateGenerateContextType.DependendPrefab, text);
			widgetTemplateGenerateContext.PrepareAsPrefab(prefabName, variantName, variantExtension, data, givenParameters);
			prefabDependencyContext.AddDependentWidgetTemplateGenerateContext(widgetTemplateGenerateContext);
			return widgetTemplateGenerateContext;
		}

		public static WidgetTemplateGenerateContext CreateAsInheritedDependendPrefab(UICodeGenerationContext uiCodeGenerationContext, PrefabDependencyContext prefabDependencyContext, string prefabName, string variantName, UICodeGenerationVariantExtension variantExtension, Dictionary<string, object> data, Dictionary<string, WidgetAttributeTemplate> givenParameters)
		{
			string text = prefabDependencyContext.GenerateDependencyName() + "_" + WidgetTemplateGenerateContext.GetUseablePrefabClassName(prefabName, variantName);
			WidgetTemplateGenerateContext widgetTemplateGenerateContext = new WidgetTemplateGenerateContext(uiCodeGenerationContext, prefabDependencyContext, WidgetTemplateGenerateContextType.InheritedDependendPrefab, text);
			widgetTemplateGenerateContext.PrepareAsPrefab(prefabName, variantName, variantExtension, data, givenParameters);
			prefabDependencyContext.AddDependentWidgetTemplateGenerateContext(widgetTemplateGenerateContext);
			return widgetTemplateGenerateContext;
		}

		public static WidgetTemplateGenerateContext CreateAsCustomWidgetTemplate(UICodeGenerationContext uiCodeGenerationContext, PrefabDependencyContext prefabDependencyContext, WidgetTemplate widgetTemplate, string identifierName, VariableCollection variableCollection, UICodeGenerationVariantExtension variantExtension, Dictionary<string, object> data)
		{
			string text = prefabDependencyContext.GenerateDependencyName() + "_" + identifierName;
			WidgetTemplateGenerateContext widgetTemplateGenerateContext = new WidgetTemplateGenerateContext(uiCodeGenerationContext, prefabDependencyContext, WidgetTemplateGenerateContextType.CustomWidgetTemplate, text);
			widgetTemplateGenerateContext.PrepareAsWidgetTemplate(widgetTemplate, variableCollection, variantExtension, data);
			prefabDependencyContext.AddDependentWidgetTemplateGenerateContext(widgetTemplateGenerateContext);
			return widgetTemplateGenerateContext;
		}

		private void PrepareAsPrefab(string prefabName, string variantName, UICodeGenerationVariantExtension variantExtension, Dictionary<string, object> data, Dictionary<string, WidgetAttributeTemplate> givenParameters)
		{
			this.PrefabName = prefabName;
			this.VariantName = variantName;
			this._variantExtension = variantExtension;
			this.Data = data;
			this.VariableCollection.SetGivenParameters(givenParameters);
			this._prefab = this.WidgetFactory.GetCustomType(this.PrefabName);
			this._rootWidgetTemplate = this._prefab.RootTemplate;
			this.GotLogicalChildrenLocation = this.FindLogicalChildrenLocation(this._rootWidgetTemplate) != null;
			if (this._variantExtension != null)
			{
				this._variantExtension.Initialize(this);
			}
			this.VariableCollection.FillFromPrefab(this._prefab);
		}

		private void PrepareAsWidgetTemplate(WidgetTemplate rootWidgetTemplate, VariableCollection variableCollection, UICodeGenerationVariantExtension variantExtension, Dictionary<string, object> data)
		{
			this._rootWidgetTemplate = rootWidgetTemplate;
			this.Data = data;
			this._variantExtension = variantExtension;
			this.VariableCollection = variableCollection;
			if (this._variantExtension != null)
			{
				this._variantExtension.Initialize(this);
			}
		}

		private WidgetTemplate FindLogicalChildrenLocation(WidgetTemplate template)
		{
			if (template.LogicalChildrenLocation)
			{
				return template;
			}
			for (int i = 0; i < template.ChildCount; i++)
			{
				WidgetTemplate childAt = template.GetChildAt(i);
				WidgetTemplate widgetTemplate = this.FindLogicalChildrenLocation(childAt);
				if (widgetTemplate != null)
				{
					return widgetTemplate;
				}
			}
			return null;
		}

		private CommentSection CreateCommentSection()
		{
			CommentSection commentSection = new CommentSection();
			foreach (KeyValuePair<string, object> keyValuePair in this.Data)
			{
				commentSection.AddCommentLine(string.Concat(new object[] { "Data: ", keyValuePair.Key, " - ", keyValuePair.Value }));
			}
			foreach (KeyValuePair<string, WidgetAttributeTemplate> keyValuePair2 in this.VariableCollection.GivenParameters)
			{
				WidgetAttributeTemplate value = keyValuePair2.Value;
				commentSection.AddCommentLine(string.Concat(new object[] { "Given Parameter: ", keyValuePair2.Key, " - ", value.Value, " ", value.KeyType, " ", value.ValueType }));
			}
			return commentSection;
		}

		public Type GetWidgetTypeWithinPrefabRoots(string typeName)
		{
			if (this.WidgetFactory.IsCustomType(typeName))
			{
				WidgetPrefab customType = this.WidgetFactory.GetCustomType(typeName);
				return this.GetWidgetTypeWithinPrefabRoots(customType.RootTemplate.Type);
			}
			return this.WidgetFactory.GetBuiltinType(typeName);
		}

		public bool CheckIfInheritsAnotherPrefab()
		{
			string type = this._rootWidgetTemplate.Type;
			return this.WidgetFactory.IsCustomType(type);
		}

		private void AddInheritedType(ClassCode classCode)
		{
			string type = this._rootWidgetTemplate.Type;
			string text;
			if (this.WidgetFactory.IsCustomType(type))
			{
				text = this._rootWidgetCodeGenerationInfo.ChildWidgetTemplateGenerateContext.ClassName;
			}
			else
			{
				text = this.WidgetFactory.GetBuiltinType(type).FullName;
			}
			classCode.InheritedInterfaces.Add(text);
		}

		public void GenerateInto(NamespaceCode namespaceCode)
		{
			ClassCode classCode = new ClassCode();
			classCode.Name = this.ClassName;
			classCode.AccessModifier = ClassCodeAccessModifier.Public;
			if (this._variantExtension != null)
			{
				this._variantExtension.AddExtensionVariables(classCode);
			}
			this.CreateWidgetInformations(this._rootWidgetTemplate, "_widget", null);
			if (this.GotLogicalChildrenLocation)
			{
				this.GenerateAddChildToLogicalLocationMethod(classCode);
			}
			this.CheckDependencies();
			this.VariableCollection.FillVisualDefinitionCreators(classCode);
			this.FillWidgetVariables(classCode);
			this.GenerateCreateWidgetsMethod(classCode);
			this.GenerateSetIdsMethod(classCode);
			this.GenerateSetAttributesMethod(classCode);
			this.AddInheritedType(classCode);
			if (this._variantExtension != null)
			{
				this._variantExtension.DoExtraCodeGeneration(classCode);
			}
			classCode.AddConsturctor(new ConstructorCode
			{
				MethodSignature = "(TaleWorlds.GauntletUI.UIContext context)",
				BaseCall = "(context)"
			});
			classCode.CommentSection = this.CreateCommentSection();
			namespaceCode.AddClass(classCode);
			if (this.ContextType == WidgetTemplateGenerateContextType.RootPrefab)
			{
				this.PrefabDependencyContext.GenerateInto(namespaceCode);
			}
		}

		private void GenerateAddChildToLogicalLocationMethod(ClassCode classCode)
		{
			WidgetTemplate widgetTemplate = this.FindLogicalChildrenLocation(this._rootWidgetTemplate);
			WidgetCodeGenerationInfo widgetCodeGenerationInfo = this.FindWidgetCodeGenerationInformation(widgetTemplate);
			MethodCode methodCode = new MethodCode();
			methodCode.Name = "AddChildToLogicalLocation";
			methodCode.AccessModifier = MethodCodeAccessModifier.Public;
			methodCode.MethodSignature = "(TaleWorlds.GauntletUI.BaseTypes.Widget widget)";
			methodCode.AddLine(widgetCodeGenerationInfo.VariableName + ".AddChild(widget);");
			classCode.AddMethod(methodCode);
		}

		public MethodCode GenerateCreatorMethod()
		{
			MethodCode methodCode = new MethodCode();
			methodCode.Name = "Create" + this.ClassName;
			methodCode.ReturnParameter = "TaleWorlds.GauntletUI.PrefabSystem.GeneratedPrefabInstantiationResult";
			methodCode.MethodSignature = "(TaleWorlds.GauntletUI.UIContext context, System.Collections.Generic.Dictionary<string, object> data)";
			methodCode.AddLine("var widget = new " + this.ClassName + "(context);");
			methodCode.AddLine("widget.CreateWidgets();");
			methodCode.AddLine("widget.SetIds();");
			methodCode.AddLine("widget.SetAttributes();");
			methodCode.AddLine("var result = new TaleWorlds.GauntletUI.PrefabSystem.GeneratedPrefabInstantiationResult(widget);");
			if (this._variantExtension != null)
			{
				this._variantExtension.AddExtrasToCreatorMethod(methodCode);
			}
			methodCode.AddLine("return result;");
			return methodCode;
		}

		public static string GetUseableName(string name)
		{
			return name.Replace(".", "_");
		}

		private static string GetUseablePrefabClassName(string name, string variantName)
		{
			string text = name.Replace(".", "_");
			string text2 = variantName.Replace(".", "_");
			return text + "__" + text2;
		}

		private WidgetCodeGenerationInfo FindWidgetCodeGenerationInformation(WidgetTemplate widgetTemplate)
		{
			foreach (WidgetCodeGenerationInfo widgetCodeGenerationInfo in this.WidgetCodeGenerationInformations)
			{
				if (widgetCodeGenerationInfo.WidgetTemplate == widgetTemplate)
				{
					return widgetCodeGenerationInfo;
				}
			}
			return null;
		}

		private void CreateWidgetInformations(WidgetTemplate widgetTemplate, string variableName, WidgetCodeGenerationInfo parent)
		{
			WidgetCodeGenerationInfo widgetCodeGenerationInfo = new WidgetCodeGenerationInfo(this, widgetTemplate, variableName, parent);
			this.WidgetCodeGenerationInformations.Add(widgetCodeGenerationInfo);
			if (parent != null)
			{
				parent.AddChild(widgetCodeGenerationInfo);
			}
			else
			{
				this._rootWidgetCodeGenerationInfo = widgetCodeGenerationInfo;
			}
			if (this._variantExtension != null)
			{
				WidgetCodeGenerationInfoExtension widgetCodeGenerationInfoExtension = this._variantExtension.CreateWidgetCodeGenerationInfoExtension(widgetCodeGenerationInfo);
				widgetCodeGenerationInfo.AddExtension(widgetCodeGenerationInfoExtension);
				widgetCodeGenerationInfoExtension.Initialize();
			}
			for (int i = 0; i < widgetTemplate.ChildCount; i++)
			{
				WidgetTemplate childAt = widgetTemplate.GetChildAt(i);
				string text = variableName + "_" + i;
				this.CreateWidgetInformations(childAt, text, widgetCodeGenerationInfo);
			}
		}

		private void CheckDependencies()
		{
			foreach (WidgetCodeGenerationInfo widgetCodeGenerationInfo in this.WidgetCodeGenerationInformations)
			{
				widgetCodeGenerationInfo.CheckDependendType();
			}
		}

		private void FillWidgetVariables(ClassCode classCode)
		{
			foreach (WidgetCodeGenerationInfo widgetCodeGenerationInfo in this.WidgetCodeGenerationInformations)
			{
				VariableCode variableCode = widgetCodeGenerationInfo.CreateVariableCode();
				classCode.AddVariable(variableCode);
			}
		}

		public bool IsBuiltinType(WidgetTemplate widgetTemplate)
		{
			return this.WidgetFactory.IsBuiltinType(widgetTemplate.Type);
		}

		private void GenerateCreateWidgetsMethod(ClassCode classCode)
		{
			MethodCode methodCode = new MethodCode();
			methodCode.Name = "CreateWidgets";
			classCode.AddMethod(methodCode);
			if (this.CheckIfInheritsAnotherPrefab())
			{
				methodCode.PolymorphismInfo = MethodCodePolymorphismInfo.Override;
				methodCode.AddLine("base.CreateWidgets();");
			}
			else if (this.ContextType == WidgetTemplateGenerateContextType.InheritedDependendPrefab)
			{
				methodCode.PolymorphismInfo = MethodCodePolymorphismInfo.Virtual;
			}
			foreach (WidgetCodeGenerationInfo widgetCodeGenerationInfo in this.WidgetCodeGenerationInformations)
			{
				widgetCodeGenerationInfo.FillCreateWidgetsMethod(methodCode);
			}
		}

		private void GenerateSetIdsMethod(ClassCode classCode)
		{
			MethodCode methodCode = new MethodCode();
			methodCode.Name = "SetIds";
			classCode.AddMethod(methodCode);
			if (this.CheckIfInheritsAnotherPrefab())
			{
				methodCode.PolymorphismInfo = MethodCodePolymorphismInfo.Override;
				methodCode.AddLine("base.SetIds();");
			}
			else if (this.ContextType == WidgetTemplateGenerateContextType.InheritedDependendPrefab)
			{
				methodCode.PolymorphismInfo = MethodCodePolymorphismInfo.Virtual;
			}
			foreach (WidgetCodeGenerationInfo widgetCodeGenerationInfo in this.WidgetCodeGenerationInformations)
			{
				widgetCodeGenerationInfo.FillSetIdsMethod(methodCode);
			}
		}

		private void GenerateSetAttributesMethod(ClassCode classCode)
		{
			MethodCode methodCode = new MethodCode();
			methodCode.Name = "SetAttributes";
			classCode.AddMethod(methodCode);
			if (this.CheckIfInheritsAnotherPrefab())
			{
				methodCode.PolymorphismInfo = MethodCodePolymorphismInfo.Override;
				methodCode.AddLine("base.SetAttributes();");
			}
			else if (this.ContextType == WidgetTemplateGenerateContextType.InheritedDependendPrefab)
			{
				methodCode.PolymorphismInfo = MethodCodePolymorphismInfo.Virtual;
			}
			foreach (WidgetCodeGenerationInfo widgetCodeGenerationInfo in this.WidgetCodeGenerationInformations)
			{
				widgetCodeGenerationInfo.FillSetAttributesMethod(methodCode);
			}
		}

		public static string GetCodeFileType(Type type)
		{
			return type.FullName.Replace('+', '.');
		}

		private WidgetPrefab _prefab;

		private WidgetTemplate _rootWidgetTemplate;

		private UICodeGenerationVariantExtension _variantExtension;

		private WidgetCodeGenerationInfo _rootWidgetCodeGenerationInfo;
	}
}
