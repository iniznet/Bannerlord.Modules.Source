using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.GauntletUI.PrefabSystem;
using TaleWorlds.Library;
using TaleWorlds.Library.CodeGeneration;

namespace TaleWorlds.GauntletUI.CodeGenerator
{
	public class WidgetCodeGenerationInfoDatabindingExtension : WidgetCodeGenerationInfoExtension
	{
		public BindingPath FullBindingPath
		{
			get
			{
				if (this._usesParentsDatabinding)
				{
					return this.Parent.FullBindingPath;
				}
				if (this.IsRoot)
				{
					return this._bindingPath;
				}
				return this.Parent.FullBindingPath.Append(this._bindingPath).Simplify();
			}
		}

		private Type RootDataSourceType
		{
			get
			{
				if (!this.IsRoot)
				{
					return this.Parent.RootDataSourceType;
				}
				if (this._rootUsesSubPath)
				{
					return this._actualRootDataSourceType;
				}
				return this._dataSourceType;
			}
		}

		public bool IsBindingList
		{
			get
			{
				return typeof(IMBBindingList).IsAssignableFrom(this._dataSourceType);
			}
		}

		public WidgetCodeGenerationInfo WidgetCodeGenerationInfo { get; private set; }

		public WidgetCodeGenerationInfoDatabindingExtension Parent
		{
			get
			{
				if (this.WidgetCodeGenerationInfo.Parent != null)
				{
					return (WidgetCodeGenerationInfoDatabindingExtension)this.WidgetCodeGenerationInfo.Parent.Extension;
				}
				return null;
			}
		}

		public WidgetTemplateGenerateContext FirstItemTemplateCodeGenerationInfo { get; private set; }

		public WidgetTemplateGenerateContext LastItemTemplateCodeGenerationInfo { get; private set; }

		public WidgetTemplateGenerateContext ItemTemplateCodeGenerationInfo { get; private set; }

		public bool IsRoot
		{
			get
			{
				return this.Parent == null;
			}
		}

		public Dictionary<string, GeneratedBindDataInfo> BindDataInfos { get; private set; }

		public Dictionary<string, GeneratedBindCommandInfo> BindCommandInfos { get; private set; }

		public WidgetCodeGenerationInfoDatabindingExtension(WidgetCodeGenerationInfo widgetCodeGenerationInfo)
		{
			this.WidgetCodeGenerationInfo = widgetCodeGenerationInfo;
			this._widgetTemplateGenerateContext = this.WidgetCodeGenerationInfo.RootWidgetTemplateGenerateContext;
			this._widgetTemplate = this.WidgetCodeGenerationInfo.WidgetTemplate;
			this._codeGenerationContext = this._widgetTemplateGenerateContext.UICodeGenerationContext;
			this.BindDataInfos = new Dictionary<string, GeneratedBindDataInfo>();
			this.BindCommandInfos = new Dictionary<string, GeneratedBindCommandInfo>();
		}

		public override void Initialize()
		{
			if (this.IsRoot)
			{
				this._dataSourceType = this._widgetTemplateGenerateContext.Data["DataSourceType"] as Type;
				this._actualRootDataSourceType = this._dataSourceType;
				this._bindingPath = new BindingPath("Root");
				this._usesParentsDatabinding = false;
			}
			else
			{
				this._dataSourceType = this.Parent._dataSourceType;
				this._bindingPath = this.Parent._bindingPath;
				this._usesParentsDatabinding = true;
			}
			this.ReadAttributes();
			this.InitializeCommandBindings();
			this.InitializeDataBindings();
		}

		public override bool TryGetVariantPropertiesForNewDependency(out UICodeGenerationVariantExtension variantExtension, out Dictionary<string, object> data)
		{
			variantExtension = new UICodeGenerationDatabindingVariantExtension();
			data = new Dictionary<string, object>();
			data.Add("DataSourceType", this._dataSourceType);
			return true;
		}

		public bool CheckIfRequiresDataComponentForWidget()
		{
			foreach (KeyValuePair<Type, Dictionary<string, WidgetAttributeTemplate>> keyValuePair in this._widgetTemplate.Attributes)
			{
				foreach (KeyValuePair<string, WidgetAttributeTemplate> keyValuePair2 in keyValuePair.Value)
				{
					WidgetAttributeKeyType keyType = keyValuePair2.Value.KeyType;
					WidgetAttributeValueType valueType = keyValuePair2.Value.ValueType;
					string key = keyValuePair2.Value.Key;
					string value = keyValuePair2.Value.Value;
					if (keyType is WidgetAttributeKeyTypeAttribute && key == "AcceptDrag")
					{
						return true;
					}
				}
			}
			return false;
		}

		public override void OnFillCreateWidgetMethod(MethodCode methodCode)
		{
			if (this.CheckIfRequiresDataComponentForWidget())
			{
				methodCode.AddLine("//Requires data component");
				methodCode.AddLine("{");
				methodCode.AddLine("var widgetComponent = new TaleWorlds.GauntletUI.Data.GeneratedWidgetData(" + this.WidgetCodeGenerationInfo.VariableName + ");");
				methodCode.AddLine(this.WidgetCodeGenerationInfo.VariableName + ".AddComponent(widgetComponent);");
				methodCode.AddLine("}");
			}
		}

		private void CheckDependency()
		{
			ItemTemplateUsage extensionData = this._widgetTemplate.GetExtensionData<ItemTemplateUsage>();
			if (extensionData != null)
			{
				Type type = this._dataSourceType.GetGenericArguments()[0];
				WidgetTemplate defaultItemTemplate = extensionData.DefaultItemTemplate;
				PrefabDependencyContext prefabDependencyContext = this.WidgetCodeGenerationInfo.RootWidgetTemplateGenerateContext.PrefabDependencyContext;
				VariableCollection variableCollection = this.WidgetCodeGenerationInfo.RootWidgetTemplateGenerateContext.VariableCollection;
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("DataSourceType", type);
				this.ItemTemplateCodeGenerationInfo = WidgetTemplateGenerateContext.CreateAsCustomWidgetTemplate(this._codeGenerationContext, prefabDependencyContext, defaultItemTemplate, "ItemTemplate", variableCollection, new UICodeGenerationDatabindingVariantExtension(), dictionary);
				if (extensionData.FirstItemTemplate != null)
				{
					Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
					dictionary2.Add("DataSourceType", type);
					this.FirstItemTemplateCodeGenerationInfo = WidgetTemplateGenerateContext.CreateAsCustomWidgetTemplate(this._codeGenerationContext, prefabDependencyContext, extensionData.FirstItemTemplate, "FirstItemTemplate", variableCollection, new UICodeGenerationDatabindingVariantExtension(), dictionary2);
				}
				if (extensionData.LastItemTemplate != null)
				{
					Dictionary<string, object> dictionary3 = new Dictionary<string, object>();
					dictionary3.Add("DataSourceType", type);
					this.LastItemTemplateCodeGenerationInfo = WidgetTemplateGenerateContext.CreateAsCustomWidgetTemplate(this._codeGenerationContext, prefabDependencyContext, extensionData.LastItemTemplate, "LastItemTemplate", variableCollection, new UICodeGenerationDatabindingVariantExtension(), dictionary3);
				}
			}
		}

		private void ReadAttributes()
		{
			VariableCollection variableCollection = this._widgetTemplateGenerateContext.VariableCollection;
			foreach (KeyValuePair<Type, Dictionary<string, WidgetAttributeTemplate>> keyValuePair in this._widgetTemplate.Attributes)
			{
				foreach (KeyValuePair<string, WidgetAttributeTemplate> keyValuePair2 in keyValuePair.Value)
				{
					WidgetAttributeKeyType keyType = keyValuePair2.Value.KeyType;
					WidgetAttributeValueType valueType = keyValuePair2.Value.ValueType;
					string key = keyValuePair2.Value.Key;
					string value = keyValuePair2.Value.Value;
					if (keyType is WidgetAttributeKeyTypeDataSource)
					{
						if (valueType is WidgetAttributeValueTypeBindingPath)
						{
							this.AssignBindingPathFromValue(value);
						}
						else if (valueType is WidgetAttributeValueTypeParameter)
						{
							string parameterValue = variableCollection.GetParameterValue(value);
							if (!string.IsNullOrEmpty(parameterValue))
							{
								this.AssignBindingPathFromValue(parameterValue);
							}
						}
					}
					else if (keyType is WidgetAttributeKeyTypeAttribute || keyType is WidgetAttributeKeyTypeId)
					{
						if (valueType is WidgetAttributeValueTypeBinding)
						{
							GeneratedBindDataInfo generatedBindDataInfo = new GeneratedBindDataInfo(key, value);
							this.BindDataInfos.Add(generatedBindDataInfo.Property, generatedBindDataInfo);
						}
						else if (valueType is WidgetAttributeValueTypeParameter)
						{
							string text = value;
							if (variableCollection.GivenParameters.ContainsKey(text) && variableCollection.GivenParameters[text].ValueType is WidgetAttributeValueTypeBinding)
							{
								string value2 = variableCollection.GivenParameters[text].Value;
								GeneratedBindDataInfo generatedBindDataInfo2 = new GeneratedBindDataInfo(key, value2);
								this.BindDataInfos.Add(generatedBindDataInfo2.Property, generatedBindDataInfo2);
							}
						}
					}
					else if (keyType is WidgetAttributeKeyTypeCommand)
					{
						string text2 = value;
						if (valueType is WidgetAttributeValueTypeParameter && variableCollection.GivenParameters.ContainsKey(value))
						{
							text2 = variableCollection.GivenParameters[value].Value;
						}
						GeneratedBindCommandInfo generatedBindCommandInfo = new GeneratedBindCommandInfo(key, text2);
						this.BindCommandInfos.Add(generatedBindCommandInfo.Command, generatedBindCommandInfo);
					}
					else if (keyType is WidgetAttributeKeyTypeCommandParameter)
					{
						GeneratedBindCommandInfo generatedBindCommandInfo2 = this.BindCommandInfos[key];
						generatedBindCommandInfo2.GotParameter = true;
						if (valueType is WidgetAttributeValueTypeDefault)
						{
							generatedBindCommandInfo2.Parameter = value;
						}
						else if (valueType is WidgetAttributeValueTypeParameter)
						{
							string parameterValue2 = this.WidgetCodeGenerationInfo.RootWidgetTemplateGenerateContext.VariableCollection.GetParameterValue(value);
							generatedBindCommandInfo2.Parameter = parameterValue2;
						}
					}
				}
			}
			this.CheckDependency();
		}

		private void InitializeCommandBindings()
		{
			foreach (GeneratedBindCommandInfo generatedBindCommandInfo in this.BindCommandInfos.Values)
			{
				BindingPath bindingPath = new BindingPath(generatedBindCommandInfo.Path);
				BindingPath bindingPath2 = this.FullBindingPath;
				if (bindingPath.Nodes.Length > 1)
				{
					bindingPath2 = this.FullBindingPath.Append(bindingPath.ParentPath).Simplify();
				}
				MethodInfo method = WidgetCodeGenerationInfoDatabindingExtension.GetViewModelTypeAtPath(this.RootDataSourceType, bindingPath2).GetMethod(bindingPath.LastNode, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				int num = 0;
				if (method != null)
				{
					ParameterInfo[] parameters = method.GetParameters();
					num = parameters.Length;
					for (int i = 0; i < parameters.Length; i++)
					{
						GeneratedBindCommandParameterInfo generatedBindCommandParameterInfo = new GeneratedBindCommandParameterInfo();
						Type parameterType = parameters[i].ParameterType;
						generatedBindCommandParameterInfo.Type = parameterType;
						if (typeof(ViewModel).IsAssignableFrom(parameterType) || typeof(IMBBindingList).IsAssignableFrom(parameterType))
						{
							generatedBindCommandParameterInfo.IsViewModel = true;
						}
						generatedBindCommandInfo.MethodParameters.Add(generatedBindCommandParameterInfo);
					}
				}
				generatedBindCommandInfo.Method = method;
				generatedBindCommandInfo.ParameterCount = num;
			}
		}

		private void InitializeDataBindings()
		{
			foreach (GeneratedBindDataInfo generatedBindDataInfo in this.BindDataInfos.Values)
			{
				PropertyInfo property = WidgetCodeGenerationInfoDatabindingExtension.GetViewModelTypeAtPath(this.RootDataSourceType, this.FullBindingPath).GetProperty(generatedBindDataInfo.Path, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (property != null)
				{
					Type propertyType = property.PropertyType;
					generatedBindDataInfo.ViewModelPropertType = propertyType;
				}
				string type = this.WidgetCodeGenerationInfo.WidgetTemplate.Type;
				PropertyInfo propertyInfo = VariableCollection.GetPropertyInfo(this.WidgetCodeGenerationInfo.RootWidgetTemplateGenerateContext.GetWidgetTypeWithinPrefabRoots(type), generatedBindDataInfo.Property);
				if (propertyInfo != null)
				{
					Type propertyType2 = propertyInfo.PropertyType;
					generatedBindDataInfo.WidgetPropertyType = propertyType2;
				}
				if (property != null && propertyInfo != null && !generatedBindDataInfo.WidgetPropertyType.IsAssignableFrom(generatedBindDataInfo.ViewModelPropertType))
				{
					generatedBindDataInfo.RequiresConversion = true;
				}
			}
		}

		private void AssignBindingPathFromValue(string value)
		{
			BindingPath bindingPath = new BindingPath(value);
			BindingPath bindingPath2 = this.FullBindingPath.Append(bindingPath).Simplify();
			Type viewModelTypeAtPath = WidgetCodeGenerationInfoDatabindingExtension.GetViewModelTypeAtPath(this.RootDataSourceType, bindingPath2);
			this._usesParentsDatabinding = false;
			this._dataSourceType = viewModelTypeAtPath;
			if (this.IsRoot)
			{
				this._bindingPath = new BindingPath("Root").Append(bindingPath);
				this._rootUsesSubPath = true;
				return;
			}
			this._bindingPath = bindingPath;
		}

		public override void OnFillSetAttributesMethod(MethodCode methodCode)
		{
		}

		public static Type GetViewModelTypeAtPath(Type type, BindingPath path)
		{
			BindingPath subPath = path.SubPath;
			if (subPath != null)
			{
				PropertyInfo property = WidgetCodeGenerationInfoDatabindingExtension.GetProperty(type, subPath.FirstNode);
				if (property != null)
				{
					Type returnType = property.GetGetMethod().ReturnType;
					if (typeof(ViewModel).IsAssignableFrom(returnType))
					{
						return WidgetCodeGenerationInfoDatabindingExtension.GetViewModelTypeAtPath(returnType, subPath);
					}
					if (typeof(IMBBindingList).IsAssignableFrom(returnType))
					{
						return WidgetCodeGenerationInfoDatabindingExtension.GetChildTypeAtPath(returnType, subPath);
					}
				}
				return null;
			}
			return type;
		}

		private static Type GetChildTypeAtPath(Type bindingListType, BindingPath path)
		{
			BindingPath subPath = path.SubPath;
			if (subPath == null)
			{
				return bindingListType;
			}
			Type type = bindingListType.GetGenericArguments()[0];
			if (typeof(ViewModel).IsAssignableFrom(type))
			{
				return WidgetCodeGenerationInfoDatabindingExtension.GetViewModelTypeAtPath(type, subPath);
			}
			if (typeof(IMBBindingList).IsAssignableFrom(type))
			{
				return WidgetCodeGenerationInfoDatabindingExtension.GetChildTypeAtPath(type, subPath);
			}
			return null;
		}

		private static PropertyInfo GetProperty(Type type, string name)
		{
			foreach (PropertyInfo propertyInfo in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
			{
				if (propertyInfo.Name == name)
				{
					return propertyInfo;
				}
			}
			return null;
		}

		private Type _actualRootDataSourceType;

		private Type _dataSourceType;

		private BindingPath _bindingPath;

		private bool _usesParentsDatabinding;

		private bool _rootUsesSubPath;

		private WidgetTemplateGenerateContext _widgetTemplateGenerateContext;

		private UICodeGenerationContext _codeGenerationContext;

		private WidgetTemplate _widgetTemplate;
	}
}
