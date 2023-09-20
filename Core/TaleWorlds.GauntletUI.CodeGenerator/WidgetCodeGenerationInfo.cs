using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.PrefabSystem;
using TaleWorlds.Library;
using TaleWorlds.Library.CodeGeneration;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.CodeGenerator
{
	public class WidgetCodeGenerationInfo
	{
		public WidgetTemplateGenerateContext RootWidgetTemplateGenerateContext { get; private set; }

		public WidgetTemplate WidgetTemplate { get; private set; }

		public WidgetFactory WidgetFactory { get; private set; }

		public string VariableName { get; private set; }

		public WidgetCodeGenerationInfo Parent { get; private set; }

		public WidgetCodeGenerationInfoExtension Extension { get; private set; }

		public WidgetTemplateGenerateContext ChildWidgetTemplateGenerateContext { get; private set; }

		public bool IsRoot
		{
			get
			{
				return this.Parent == null;
			}
		}

		public string Id
		{
			get
			{
				return this.WidgetTemplate.Id;
			}
		}

		public List<WidgetCodeGenerationInfo> Children { get; private set; }

		public WidgetCodeGenerationInfo(WidgetTemplateGenerateContext widgetTemplateGenerateContext, WidgetTemplate widgetTemplate, string variableName, WidgetCodeGenerationInfo parent)
		{
			this.RootWidgetTemplateGenerateContext = widgetTemplateGenerateContext;
			this.WidgetFactory = widgetTemplateGenerateContext.WidgetFactory;
			this.WidgetTemplate = widgetTemplate;
			this.VariableName = variableName;
			this.Parent = parent;
			this.Children = new List<WidgetCodeGenerationInfo>();
		}

		public WidgetCodeGenerationInfoChildSearchResult FindChild(BindingPath path)
		{
			string firstNode = path.FirstNode;
			BindingPath subPath = path.SubPath;
			if (firstNode == "..")
			{
				if (this.Parent != null)
				{
					return this.Parent.FindChild(subPath);
				}
				return new WidgetCodeGenerationInfoChildSearchResult
				{
					RemainingPath = path,
					ReachedWidget = this
				};
			}
			else
			{
				if (firstNode == ".")
				{
					return new WidgetCodeGenerationInfoChildSearchResult
					{
						FoundWidget = this
					};
				}
				foreach (WidgetCodeGenerationInfo widgetCodeGenerationInfo in this.Children)
				{
					if (!string.IsNullOrEmpty(widgetCodeGenerationInfo.Id) && widgetCodeGenerationInfo.Id == firstNode)
					{
						if (subPath == null)
						{
							return new WidgetCodeGenerationInfoChildSearchResult
							{
								FoundWidget = widgetCodeGenerationInfo
							};
						}
						return widgetCodeGenerationInfo.FindChild(subPath);
					}
				}
				return new WidgetCodeGenerationInfoChildSearchResult
				{
					RemainingPath = path,
					ReachedWidget = this
				};
			}
		}

		public Dictionary<string, WidgetAttributeTemplate> GetPassedParametersToChild()
		{
			Dictionary<string, WidgetAttributeTemplate> dictionary = new Dictionary<string, WidgetAttributeTemplate>();
			foreach (KeyValuePair<Type, Dictionary<string, WidgetAttributeTemplate>> keyValuePair in this.WidgetTemplate.Attributes)
			{
				foreach (KeyValuePair<string, WidgetAttributeTemplate> keyValuePair2 in keyValuePair.Value)
				{
					WidgetAttributeKeyType keyType = keyValuePair2.Value.KeyType;
					WidgetAttributeValueType valueType = keyValuePair2.Value.ValueType;
					string key = keyValuePair2.Value.Key;
					string value = keyValuePair2.Value.Value;
					if (keyType is WidgetAttributeKeyTypeParameter)
					{
						WidgetAttributeTemplate widgetAttributeTemplate = keyValuePair2.Value;
						WidgetAttributeTemplate widgetAttributeTemplate2;
						if (valueType is WidgetAttributeValueTypeParameter && this.RootWidgetTemplateGenerateContext.VariableCollection.GivenParameters.TryGetValue(key, out widgetAttributeTemplate2))
						{
							widgetAttributeTemplate = widgetAttributeTemplate2;
						}
						dictionary.Add(key, widgetAttributeTemplate);
					}
				}
			}
			return dictionary;
		}

		public void CheckDependendType()
		{
			if (!this.RootWidgetTemplateGenerateContext.IsBuiltinType(this.WidgetTemplate))
			{
				PrefabDependencyContext prefabDependencyContext = this.RootWidgetTemplateGenerateContext.PrefabDependencyContext;
				UICodeGenerationVariantExtension uicodeGenerationVariantExtension = null;
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				Dictionary<string, WidgetAttributeTemplate> passedParametersToChild = this.GetPassedParametersToChild();
				if (this.Extension != null)
				{
					this.Extension.TryGetVariantPropertiesForNewDependency(out uicodeGenerationVariantExtension, out dictionary);
				}
				if (!prefabDependencyContext.ContainsDependency(this.WidgetTemplate.Type, passedParametersToChild, dictionary, this.IsRoot))
				{
					if (this.IsRoot)
					{
						this.ChildWidgetTemplateGenerateContext = WidgetTemplateGenerateContext.CreateAsInheritedDependendPrefab(this.RootWidgetTemplateGenerateContext.UICodeGenerationContext, prefabDependencyContext, this.WidgetTemplate.Type, "InheritedPrefab", uicodeGenerationVariantExtension, dictionary, passedParametersToChild);
						return;
					}
					this.ChildWidgetTemplateGenerateContext = WidgetTemplateGenerateContext.CreateAsDependendPrefab(this.RootWidgetTemplateGenerateContext.UICodeGenerationContext, prefabDependencyContext, this.WidgetTemplate.Type, "DependendPrefab", uicodeGenerationVariantExtension, dictionary, passedParametersToChild);
					return;
				}
				else
				{
					PrefabDependency dependendPrefab = prefabDependencyContext.GetDependendPrefab(this.WidgetTemplate.Type, passedParametersToChild, dictionary, this.IsRoot);
					this.ChildWidgetTemplateGenerateContext = dependendPrefab.WidgetTemplateGenerateContext;
				}
			}
		}

		public string GetUseableTypeName()
		{
			string text;
			if (this.WidgetFactory.IsBuiltinType(this.WidgetTemplate.Type))
			{
				text = this.WidgetFactory.GetBuiltinType(this.WidgetTemplate.Type).FullName;
			}
			else
			{
				text = this.ChildWidgetTemplateGenerateContext.ClassName;
			}
			return text;
		}

		public VariableCode CreateVariableCode()
		{
			VariableCode variableCode = new VariableCode();
			variableCode.Name = this.VariableName;
			variableCode.AccessModifier = VariableCodeAccessModifier.Private;
			string useableTypeName = this.GetUseableTypeName();
			variableCode.Type = useableTypeName;
			return variableCode;
		}

		internal void AddChild(WidgetCodeGenerationInfo widgetCodeGenerationInfo)
		{
			this.Children.Add(widgetCodeGenerationInfo);
		}

		internal void FillCreateWidgetsMethod(MethodCode methodCode)
		{
			string text = ((this.Parent != null) ? this.Parent.VariableName : null);
			bool flag = this.WidgetFactory.IsBuiltinType(this.WidgetTemplate.Type);
			if (this.IsRoot)
			{
				methodCode.AddLine(this.VariableName + " = this;");
			}
			else
			{
				string useableTypeName = this.GetUseableTypeName();
				methodCode.AddLine(this.VariableName + " = new " + useableTypeName + "(this.Context);");
				if (this.Extension != null)
				{
					this.Extension.OnFillCreateWidgetMethod(methodCode);
				}
				if (this.Parent.ChildWidgetTemplateGenerateContext != null && this.Parent.ChildWidgetTemplateGenerateContext.GotLogicalChildrenLocation)
				{
					methodCode.AddLine(text + ".AddChildToLogicalLocation(" + this.VariableName + ");");
				}
				else
				{
					methodCode.AddLine(text + ".AddChild(" + this.VariableName + ");");
				}
			}
			if (!this.IsRoot && !flag)
			{
				methodCode.AddLine(this.VariableName + ".CreateWidgets();");
			}
		}

		internal void FillSetAttributesMethod(MethodCode methodCode)
		{
			VariableCollection variableCollection = this.RootWidgetTemplateGenerateContext.VariableCollection;
			string text = (this.IsRoot ? "this" : this.VariableName);
			if (!this.WidgetFactory.IsBuiltinType(this.WidgetTemplate.Type) && !this.IsRoot)
			{
				methodCode.AddLine(text + ".SetAttributes();");
			}
			foreach (KeyValuePair<Type, Dictionary<string, WidgetAttributeTemplate>> keyValuePair in this.WidgetTemplate.Attributes)
			{
				foreach (KeyValuePair<string, WidgetAttributeTemplate> keyValuePair2 in keyValuePair.Value)
				{
					WidgetAttributeKeyType keyType = keyValuePair2.Value.KeyType;
					WidgetAttributeValueType valueType = keyValuePair2.Value.ValueType;
					bool flag = false;
					string key = keyValuePair2.Value.Key;
					string value = keyValuePair2.Value.Value;
					if (keyType is WidgetAttributeKeyTypeId)
					{
						flag = true;
					}
					else if (keyType is WidgetAttributeKeyTypeAttribute)
					{
						if (valueType is WidgetAttributeValueTypeDefault)
						{
							this.AddDefaultAttributeSet(methodCode, text, key, value);
							flag = true;
						}
						else if (valueType is WidgetAttributeValueTypeConstant)
						{
							string constantValue = variableCollection.GetConstantValue(value);
							methodCode.AddLine("//From constant " + value + ":" + constantValue);
							this.AddDefaultAttributeSet(methodCode, text, key, constantValue);
							flag = true;
						}
						else if (valueType is WidgetAttributeValueTypeParameter)
						{
							string text2 = variableCollection.GetParameterDefaultValue(value);
							bool flag2 = true;
							WidgetAttributeTemplate widgetAttributeTemplate;
							if (variableCollection.GivenParameters.TryGetValue(value, out widgetAttributeTemplate))
							{
								if (widgetAttributeTemplate.ValueType is WidgetAttributeValueTypeDefault)
								{
									text2 = widgetAttributeTemplate.Value;
								}
								else if (widgetAttributeTemplate.ValueType is WidgetAttributeValueTypeConstant)
								{
									methodCode.AddLine("//parameter below has something different then default value type, " + widgetAttributeTemplate.ValueType);
								}
								else if (widgetAttributeTemplate.ValueType is WidgetAttributeValueTypeParameter)
								{
									methodCode.AddLine("//parameter below has something different then default value type, " + widgetAttributeTemplate.ValueType);
								}
								else
								{
									flag2 = false;
									methodCode.AddLine("//parameter below has something different then default value type, " + widgetAttributeTemplate.ValueType);
								}
							}
							if (flag2)
							{
								methodCode.AddLine("//From parameter " + value + ":" + text2);
								this.AddDefaultAttributeSet(methodCode, text, key, text2);
								flag = true;
							}
						}
					}
					if (!flag)
					{
						methodCode.AddLine("//");
						methodCode.AddLine("//" + keyValuePair2.Value.KeyType);
						methodCode.AddLine("//" + keyValuePair2.Value.ValueType);
						methodCode.AddLine("//" + keyValuePair2.Value.Key + " " + keyValuePair2.Value.Value);
						methodCode.AddLine("//");
					}
				}
			}
			methodCode.AddLine("");
			if (this.Extension != null)
			{
				this.Extension.OnFillSetAttributesMethod(methodCode);
			}
		}

		private Type GetAttributeType(string propertyName)
		{
			PropertyInfo propertyInfo = this.RootWidgetTemplateGenerateContext.VariableCollection.GetPropertyInfo(this.WidgetTemplate, propertyName);
			if (propertyInfo != null)
			{
				return propertyInfo.PropertyType;
			}
			return null;
		}

		private void AddDefaultAttributeSet(MethodCode methodCode, string targetWidgetName, string propertyName, string value)
		{
			Type attributeType = this.GetAttributeType(propertyName);
			if (attributeType != null)
			{
				if (attributeType == typeof(int))
				{
					methodCode.AddLine(string.Concat(new string[] { targetWidgetName, ".", propertyName, " = ", value, ";" }));
					return;
				}
				if (attributeType == typeof(float))
				{
					methodCode.AddLine(string.Concat(new string[] { targetWidgetName, ".", propertyName, " = ", value, "f;" }));
					return;
				}
				if (attributeType == typeof(bool))
				{
					methodCode.AddLine(string.Concat(new string[]
					{
						targetWidgetName,
						".",
						propertyName,
						" = ",
						value.ToLower(),
						";"
					}));
					return;
				}
				if (attributeType == typeof(string))
				{
					methodCode.AddLine(string.Concat(new string[] { targetWidgetName, ".", propertyName, " = @\"", value, "\";" }));
					return;
				}
				if (attributeType == typeof(Brush))
				{
					methodCode.AddLine(string.Concat(new string[] { targetWidgetName, ".", propertyName, " = this.Context.GetBrush(@\"", value, "\");" }));
					return;
				}
				if (attributeType == typeof(Sprite))
				{
					methodCode.AddLine(string.Concat(new string[] { targetWidgetName, ".", propertyName, " = this.Context.SpriteData.GetSprite(@\"", value, "\");" }));
					return;
				}
				if (attributeType.IsEnum)
				{
					methodCode.AddLine(string.Concat(new string[]
					{
						targetWidgetName,
						".",
						propertyName,
						" = ",
						WidgetTemplateGenerateContext.GetCodeFileType(attributeType),
						".",
						value,
						";"
					}));
					return;
				}
				if (attributeType == typeof(Color))
				{
					Color color = Color.ConvertStringToColor(value);
					methodCode.AddLine(string.Concat(new object[]
					{
						targetWidgetName, ".", propertyName, " = new TaleWorlds.Library.Color(", color.Red, "f, ", color.Green, "f, ", color.Blue, "f, ",
						color.Alpha, "f);"
					}));
					return;
				}
				if (attributeType == typeof(XmlElement))
				{
					methodCode.AddLine(string.Concat(new string[] { "//XmlElement - ", targetWidgetName, ".", propertyName, " = \"", value, "\";" }));
					return;
				}
				if (typeof(Widget).IsAssignableFrom(attributeType))
				{
					WidgetCodeGenerationInfoChildSearchResult widgetCodeGenerationInfoChildSearchResult = this.FindChild(new BindingPath(value));
					if (widgetCodeGenerationInfoChildSearchResult != null && widgetCodeGenerationInfoChildSearchResult.FoundWidget != null)
					{
						methodCode.AddLine("//Found widget during generation - " + value);
						methodCode.AddLine(string.Concat(new string[]
						{
							targetWidgetName,
							".",
							propertyName,
							" = ",
							widgetCodeGenerationInfoChildSearchResult.FoundWidget.VariableName,
							";"
						}));
						return;
					}
					if (widgetCodeGenerationInfoChildSearchResult == null || widgetCodeGenerationInfoChildSearchResult.ReachedWidget == null)
					{
						methodCode.AddLine(string.Concat(new string[] { targetWidgetName, ".", propertyName, " = ", targetWidgetName, ".FindChild(new TaleWorlds.Library.BindingPath(@\"", value, "\")) as ", attributeType.FullName, ";" }));
						return;
					}
					methodCode.AddLine("//Found widget partial path during generation - " + value);
					if (widgetCodeGenerationInfoChildSearchResult.RemainingPath.Nodes.Length == 1)
					{
						methodCode.AddLine(string.Concat(new string[]
						{
							targetWidgetName,
							".",
							propertyName,
							" = ",
							widgetCodeGenerationInfoChildSearchResult.ReachedWidget.VariableName,
							".FindChild(@\"",
							widgetCodeGenerationInfoChildSearchResult.RemainingPath.Path,
							"\") as ",
							attributeType.FullName,
							";"
						}));
						return;
					}
					methodCode.AddLine(string.Concat(new string[]
					{
						targetWidgetName,
						".",
						propertyName,
						" = ",
						widgetCodeGenerationInfoChildSearchResult.ReachedWidget.VariableName,
						".FindChild(new TaleWorlds.Library.BindingPath(@\"",
						widgetCodeGenerationInfoChildSearchResult.RemainingPath.Path,
						"\")) as ",
						attributeType.FullName,
						";"
					}));
					return;
				}
				else if (attributeType == typeof(VisualDefinition))
				{
					string text = "CreateVisualDefinition" + WidgetTemplateGenerateContext.GetUseableName(value);
					methodCode.AddLine(string.Concat(new string[] { targetWidgetName, ".", propertyName, " = ", text, "();" }));
				}
			}
		}

		internal void FillSetIdsMethod(MethodCode methodCode)
		{
			string text = (this.IsRoot ? "this" : this.VariableName);
			if (!this.WidgetFactory.IsBuiltinType(this.WidgetTemplate.Type) && !this.IsRoot)
			{
				methodCode.AddLine(text + ".SetIds();");
			}
			foreach (KeyValuePair<Type, Dictionary<string, WidgetAttributeTemplate>> keyValuePair in this.WidgetTemplate.Attributes)
			{
				foreach (KeyValuePair<string, WidgetAttributeTemplate> keyValuePair2 in keyValuePair.Value)
				{
					WidgetAttributeKeyType keyType = keyValuePair2.Value.KeyType;
					WidgetAttributeValueType valueType = keyValuePair2.Value.ValueType;
					string key = keyValuePair2.Value.Key;
					string value = keyValuePair2.Value.Value;
					if (keyType is WidgetAttributeKeyTypeId && valueType is WidgetAttributeValueTypeDefault)
					{
						methodCode.AddLine(text + ".Id = \"" + value + "\";");
					}
				}
			}
		}

		internal void AddExtension(WidgetCodeGenerationInfoExtension extension)
		{
			this.Extension = extension;
		}
	}
}
