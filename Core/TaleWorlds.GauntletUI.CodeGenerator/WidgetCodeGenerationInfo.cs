﻿using System;
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
	// Token: 0x02000013 RID: 19
	public class WidgetCodeGenerationInfo
	{
		// Token: 0x17000041 RID: 65
		// (get) Token: 0x060000F8 RID: 248 RVA: 0x00007E6D File Offset: 0x0000606D
		// (set) Token: 0x060000F9 RID: 249 RVA: 0x00007E75 File Offset: 0x00006075
		public WidgetTemplateGenerateContext RootWidgetTemplateGenerateContext { get; private set; }

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x060000FA RID: 250 RVA: 0x00007E7E File Offset: 0x0000607E
		// (set) Token: 0x060000FB RID: 251 RVA: 0x00007E86 File Offset: 0x00006086
		public WidgetTemplate WidgetTemplate { get; private set; }

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x060000FC RID: 252 RVA: 0x00007E8F File Offset: 0x0000608F
		// (set) Token: 0x060000FD RID: 253 RVA: 0x00007E97 File Offset: 0x00006097
		public WidgetFactory WidgetFactory { get; private set; }

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x060000FE RID: 254 RVA: 0x00007EA0 File Offset: 0x000060A0
		// (set) Token: 0x060000FF RID: 255 RVA: 0x00007EA8 File Offset: 0x000060A8
		public string VariableName { get; private set; }

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x06000100 RID: 256 RVA: 0x00007EB1 File Offset: 0x000060B1
		// (set) Token: 0x06000101 RID: 257 RVA: 0x00007EB9 File Offset: 0x000060B9
		public WidgetCodeGenerationInfo Parent { get; private set; }

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x06000102 RID: 258 RVA: 0x00007EC2 File Offset: 0x000060C2
		// (set) Token: 0x06000103 RID: 259 RVA: 0x00007ECA File Offset: 0x000060CA
		public WidgetCodeGenerationInfoExtension Extension { get; private set; }

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x06000104 RID: 260 RVA: 0x00007ED3 File Offset: 0x000060D3
		// (set) Token: 0x06000105 RID: 261 RVA: 0x00007EDB File Offset: 0x000060DB
		public WidgetTemplateGenerateContext ChildWidgetTemplateGenerateContext { get; private set; }

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x06000106 RID: 262 RVA: 0x00007EE4 File Offset: 0x000060E4
		public bool IsRoot
		{
			get
			{
				return this.Parent == null;
			}
		}

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x06000107 RID: 263 RVA: 0x00007EEF File Offset: 0x000060EF
		public string Id
		{
			get
			{
				return this.WidgetTemplate.Id;
			}
		}

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x06000108 RID: 264 RVA: 0x00007EFC File Offset: 0x000060FC
		// (set) Token: 0x06000109 RID: 265 RVA: 0x00007F04 File Offset: 0x00006104
		public List<WidgetCodeGenerationInfo> Children { get; private set; }

		// Token: 0x0600010A RID: 266 RVA: 0x00007F0D File Offset: 0x0000610D
		public WidgetCodeGenerationInfo(WidgetTemplateGenerateContext widgetTemplateGenerateContext, WidgetTemplate widgetTemplate, string variableName, WidgetCodeGenerationInfo parent)
		{
			this.RootWidgetTemplateGenerateContext = widgetTemplateGenerateContext;
			this.WidgetFactory = widgetTemplateGenerateContext.WidgetFactory;
			this.WidgetTemplate = widgetTemplate;
			this.VariableName = variableName;
			this.Parent = parent;
			this.Children = new List<WidgetCodeGenerationInfo>();
		}

		// Token: 0x0600010B RID: 267 RVA: 0x00007F4C File Offset: 0x0000614C
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

		// Token: 0x0600010C RID: 268 RVA: 0x0000804C File Offset: 0x0000624C
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

		// Token: 0x0600010D RID: 269 RVA: 0x00008164 File Offset: 0x00006364
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

		// Token: 0x0600010E RID: 270 RVA: 0x0000825C File Offset: 0x0000645C
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

		// Token: 0x0600010F RID: 271 RVA: 0x000082AC File Offset: 0x000064AC
		public VariableCode CreateVariableCode()
		{
			VariableCode variableCode = new VariableCode();
			variableCode.Name = this.VariableName;
			variableCode.AccessModifier = VariableCodeAccessModifier.Private;
			string useableTypeName = this.GetUseableTypeName();
			variableCode.Type = useableTypeName;
			return variableCode;
		}

		// Token: 0x06000110 RID: 272 RVA: 0x000082DF File Offset: 0x000064DF
		internal void AddChild(WidgetCodeGenerationInfo widgetCodeGenerationInfo)
		{
			this.Children.Add(widgetCodeGenerationInfo);
		}

		// Token: 0x06000111 RID: 273 RVA: 0x000082F0 File Offset: 0x000064F0
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

		// Token: 0x06000112 RID: 274 RVA: 0x00008400 File Offset: 0x00006600
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

		// Token: 0x06000113 RID: 275 RVA: 0x00008734 File Offset: 0x00006934
		private Type GetAttributeType(string propertyName)
		{
			PropertyInfo propertyInfo = this.RootWidgetTemplateGenerateContext.VariableCollection.GetPropertyInfo(this.WidgetTemplate, propertyName);
			if (propertyInfo != null)
			{
				return propertyInfo.PropertyType;
			}
			return null;
		}

		// Token: 0x06000114 RID: 276 RVA: 0x0000876C File Offset: 0x0000696C
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

		// Token: 0x06000115 RID: 277 RVA: 0x00008CC0 File Offset: 0x00006EC0
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

		// Token: 0x06000116 RID: 278 RVA: 0x00008DF8 File Offset: 0x00006FF8
		internal void AddExtension(WidgetCodeGenerationInfoExtension extension)
		{
			this.Extension = extension;
		}
	}
}
