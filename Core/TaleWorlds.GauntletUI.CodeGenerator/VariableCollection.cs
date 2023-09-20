using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.GauntletUI.PrefabSystem;
using TaleWorlds.Library.CodeGeneration;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.CodeGenerator
{
	public class VariableCollection
	{
		public BrushFactory BrushFactory { get; private set; }

		public SpriteData SpriteData { get; private set; }

		public WidgetFactory WidgetFactory { get; private set; }

		public Dictionary<string, ConstantGenerationContext> ConstantTypes { get; private set; }

		public Dictionary<string, ParameterGenerationContext> ParameterTypes { get; private set; }

		public Dictionary<string, WidgetAttributeTemplate> GivenParameters { get; private set; }

		public Dictionary<string, VisualDefinitionTemplate> VisualDefinitionTemplates { get; private set; }

		public VariableCollection(WidgetFactory widgetFactory, BrushFactory brushFactory, SpriteData spriteData)
		{
			this.WidgetFactory = widgetFactory;
			this.BrushFactory = brushFactory;
			this.SpriteData = spriteData;
			this.GivenParameters = new Dictionary<string, WidgetAttributeTemplate>();
			this.ConstantTypes = new Dictionary<string, ConstantGenerationContext>();
			this.ParameterTypes = new Dictionary<string, ParameterGenerationContext>();
			this.VisualDefinitionTemplates = new Dictionary<string, VisualDefinitionTemplate>();
		}

		public static string GetUseableName(string name)
		{
			return name.Replace(".", "_");
		}

		public void SetGivenParameters(Dictionary<string, WidgetAttributeTemplate> givenParameters)
		{
			this.GivenParameters = givenParameters;
		}

		public void FillFromPrefab(WidgetPrefab prefab)
		{
			foreach (ConstantDefinition constantDefinition in prefab.Constants.Values)
			{
				ConstantGenerationContext constantGenerationContext = new ConstantGenerationContext(constantDefinition);
				this.ConstantTypes.Add(constantDefinition.Name, constantGenerationContext);
			}
			foreach (KeyValuePair<string, string> keyValuePair in prefab.Parameters)
			{
				string key = keyValuePair.Key;
				string value = keyValuePair.Value;
				ParameterGenerationContext parameterGenerationContext = new ParameterGenerationContext(key, value);
				this.ParameterTypes.Add(key, parameterGenerationContext);
			}
			this.FillVisualDefinitionsFromPrefab(prefab);
		}

		public void FillVisualDefinitionCreators(ClassCode classCode)
		{
			Dictionary<string, ConstantDefinition> dictionary = new Dictionary<string, ConstantDefinition>();
			Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
			foreach (KeyValuePair<string, ConstantGenerationContext> keyValuePair in this.ConstantTypes)
			{
				dictionary.Add(keyValuePair.Key, keyValuePair.Value.ConstantDefinition);
			}
			foreach (KeyValuePair<string, ParameterGenerationContext> keyValuePair2 in this.ParameterTypes)
			{
				dictionary2.Add(keyValuePair2.Key, keyValuePair2.Value.Value);
			}
			foreach (VisualDefinitionTemplate visualDefinitionTemplate in this.VisualDefinitionTemplates.Values)
			{
				MethodCode methodCode = new MethodCode();
				methodCode.Name = "CreateVisualDefinition" + VariableCollection.GetUseableName(visualDefinitionTemplate.Name);
				methodCode.AccessModifier = MethodCodeAccessModifier.Private;
				methodCode.ReturnParameter = "TaleWorlds.GauntletUI.VisualDefinition";
				string name = visualDefinitionTemplate.Name;
				float transitionDuration = visualDefinitionTemplate.TransitionDuration;
				float delayOnBegin = visualDefinitionTemplate.DelayOnBegin;
				string text = visualDefinitionTemplate.EaseIn.ToString().ToLower();
				methodCode.AddLine(string.Concat(new object[] { "var visualDefinition = new TaleWorlds.GauntletUI.VisualDefinition(\"", name, "\", ", transitionDuration, "f, ", delayOnBegin, "f, ", text, ");" }));
				foreach (VisualStateTemplate visualStateTemplate in visualDefinitionTemplate.VisualStates.Values)
				{
					methodCode.AddLine("");
					methodCode.AddLine("{");
					methodCode.AddLine("var visualState = new TaleWorlds.GauntletUI.VisualState(\"" + visualStateTemplate.State + "\");");
					foreach (KeyValuePair<string, string> keyValuePair3 in visualStateTemplate.GetAttributes())
					{
						string key = keyValuePair3.Key;
						string actualValueOf = ConstantDefinition.GetActualValueOf(keyValuePair3.Value, this.BrushFactory, this.SpriteData, dictionary, this.GivenParameters, dictionary2);
						methodCode.AddLine(string.Concat(new string[] { "visualState.", key, " = ", actualValueOf, "f;" }));
					}
					methodCode.AddLine("visualDefinition.AddVisualState(visualState);");
					methodCode.AddLine("}");
				}
				methodCode.AddLine("");
				methodCode.AddLine("return visualDefinition;");
				classCode.AddMethod(methodCode);
			}
		}

		private void FillVisualDefinitionsFromPrefab(WidgetPrefab prefab)
		{
			foreach (VisualDefinitionTemplate visualDefinitionTemplate in prefab.VisualDefinitionTemplates.Values)
			{
				this.VisualDefinitionTemplates.Add(visualDefinitionTemplate.Name, visualDefinitionTemplate);
			}
		}

		public string GetConstantValue(string constantName)
		{
			ConstantDefinition constantDefinition = this.ConstantTypes[constantName].ConstantDefinition;
			Dictionary<string, ConstantDefinition> dictionary = new Dictionary<string, ConstantDefinition>();
			Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
			foreach (KeyValuePair<string, ConstantGenerationContext> keyValuePair in this.ConstantTypes)
			{
				dictionary.Add(keyValuePair.Key, keyValuePair.Value.ConstantDefinition);
			}
			foreach (KeyValuePair<string, ParameterGenerationContext> keyValuePair2 in this.ParameterTypes)
			{
				dictionary2.Add(keyValuePair2.Key, keyValuePair2.Value.Value);
			}
			return constantDefinition.GetValue(this.BrushFactory, this.SpriteData, dictionary, this.GivenParameters, dictionary2);
		}

		public string GetParameterDefaultValue(string parameterName)
		{
			if (this.ParameterTypes.ContainsKey(parameterName))
			{
				return this.ParameterTypes[parameterName].Value;
			}
			return "";
		}

		public string GetParameterValue(string parameterName)
		{
			if (this.GivenParameters.ContainsKey(parameterName))
			{
				WidgetAttributeTemplate widgetAttributeTemplate = this.GivenParameters[parameterName];
				if (widgetAttributeTemplate.ValueType is WidgetAttributeValueTypeDefault)
				{
					return widgetAttributeTemplate.Value;
				}
				WidgetAttributeValueTypeParameter widgetAttributeValueTypeParameter = widgetAttributeTemplate.ValueType as WidgetAttributeValueTypeParameter;
				return widgetAttributeTemplate.Value;
			}
			else
			{
				if (this.ParameterTypes.ContainsKey(parameterName))
				{
					return this.ParameterTypes[parameterName].Value;
				}
				return "";
			}
		}

		private static bool IsDigitsOnly(string str)
		{
			for (int i = 0; i < str.Length; i++)
			{
				if (!char.IsDigit(str[i]))
				{
					return false;
				}
			}
			return true;
		}

		public PropertyInfo GetPropertyInfo(WidgetTemplate widgetTemplate, string propertyName)
		{
			Type type;
			if (this.WidgetFactory.IsBuiltinType(widgetTemplate.Type))
			{
				type = this.WidgetFactory.GetBuiltinType(widgetTemplate.Type);
			}
			else
			{
				WidgetPrefab customType = this.WidgetFactory.GetCustomType(widgetTemplate.Type);
				type = this.WidgetFactory.GetBuiltinType(customType.RootTemplate.Type);
			}
			PropertyInfo propertyInfo;
			VariableCollection.GetPropertyInfo(type, propertyName, 0, out propertyInfo);
			return propertyInfo;
		}

		private static void GetPropertyInfo(Type type, string name, int nameStartIndex, out PropertyInfo targetPropertyInfo)
		{
			int num = name.IndexOf('.', nameStartIndex);
			string text = ((num >= 0) ? name.Substring(nameStartIndex, num) : ((nameStartIndex > 0) ? name.Substring(nameStartIndex) : name));
			PropertyInfo property = type.GetProperty(text, BindingFlags.Instance | BindingFlags.Public);
			if (!(property != null))
			{
				targetPropertyInfo = null;
				return;
			}
			if (num < 0)
			{
				targetPropertyInfo = property;
				return;
			}
			VariableCollection.GetPropertyInfo(property.PropertyType, name, num + 1, out targetPropertyInfo);
		}

		public static PropertyInfo GetPropertyInfo(Type type, string name)
		{
			PropertyInfo propertyInfo;
			VariableCollection.GetPropertyInfo(type, name, 0, out propertyInfo);
			return propertyInfo;
		}
	}
}
