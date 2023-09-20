using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.GauntletUI.PrefabSystem;
using TaleWorlds.Library.CodeGeneration;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.CodeGenerator
{
	// Token: 0x02000006 RID: 6
	public class VariableCollection
	{
		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600002B RID: 43 RVA: 0x0000221F File Offset: 0x0000041F
		// (set) Token: 0x0600002C RID: 44 RVA: 0x00002227 File Offset: 0x00000427
		public BrushFactory BrushFactory { get; private set; }

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600002D RID: 45 RVA: 0x00002230 File Offset: 0x00000430
		// (set) Token: 0x0600002E RID: 46 RVA: 0x00002238 File Offset: 0x00000438
		public SpriteData SpriteData { get; private set; }

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600002F RID: 47 RVA: 0x00002241 File Offset: 0x00000441
		// (set) Token: 0x06000030 RID: 48 RVA: 0x00002249 File Offset: 0x00000449
		public WidgetFactory WidgetFactory { get; private set; }

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000031 RID: 49 RVA: 0x00002252 File Offset: 0x00000452
		// (set) Token: 0x06000032 RID: 50 RVA: 0x0000225A File Offset: 0x0000045A
		public Dictionary<string, ConstantGenerationContext> ConstantTypes { get; private set; }

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000033 RID: 51 RVA: 0x00002263 File Offset: 0x00000463
		// (set) Token: 0x06000034 RID: 52 RVA: 0x0000226B File Offset: 0x0000046B
		public Dictionary<string, ParameterGenerationContext> ParameterTypes { get; private set; }

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000035 RID: 53 RVA: 0x00002274 File Offset: 0x00000474
		// (set) Token: 0x06000036 RID: 54 RVA: 0x0000227C File Offset: 0x0000047C
		public Dictionary<string, WidgetAttributeTemplate> GivenParameters { get; private set; }

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000037 RID: 55 RVA: 0x00002285 File Offset: 0x00000485
		// (set) Token: 0x06000038 RID: 56 RVA: 0x0000228D File Offset: 0x0000048D
		public Dictionary<string, VisualDefinitionTemplate> VisualDefinitionTemplates { get; private set; }

		// Token: 0x06000039 RID: 57 RVA: 0x00002298 File Offset: 0x00000498
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

		// Token: 0x0600003A RID: 58 RVA: 0x000022EC File Offset: 0x000004EC
		public static string GetUseableName(string name)
		{
			return name.Replace(".", "_");
		}

		// Token: 0x0600003B RID: 59 RVA: 0x000022FE File Offset: 0x000004FE
		public void SetGivenParameters(Dictionary<string, WidgetAttributeTemplate> givenParameters)
		{
			this.GivenParameters = givenParameters;
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00002308 File Offset: 0x00000508
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

		// Token: 0x0600003D RID: 61 RVA: 0x000023E4 File Offset: 0x000005E4
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

		// Token: 0x0600003E RID: 62 RVA: 0x00002748 File Offset: 0x00000948
		private void FillVisualDefinitionsFromPrefab(WidgetPrefab prefab)
		{
			foreach (VisualDefinitionTemplate visualDefinitionTemplate in prefab.VisualDefinitionTemplates.Values)
			{
				this.VisualDefinitionTemplates.Add(visualDefinitionTemplate.Name, visualDefinitionTemplate);
			}
		}

		// Token: 0x0600003F RID: 63 RVA: 0x000027AC File Offset: 0x000009AC
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

		// Token: 0x06000040 RID: 64 RVA: 0x000028A0 File Offset: 0x00000AA0
		public string GetParameterDefaultValue(string parameterName)
		{
			if (this.ParameterTypes.ContainsKey(parameterName))
			{
				return this.ParameterTypes[parameterName].Value;
			}
			return "";
		}

		// Token: 0x06000041 RID: 65 RVA: 0x000028C8 File Offset: 0x00000AC8
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

		// Token: 0x06000042 RID: 66 RVA: 0x0000293C File Offset: 0x00000B3C
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

		// Token: 0x06000043 RID: 67 RVA: 0x00002970 File Offset: 0x00000B70
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

		// Token: 0x06000044 RID: 68 RVA: 0x000029DC File Offset: 0x00000BDC
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

		// Token: 0x06000045 RID: 69 RVA: 0x00002A40 File Offset: 0x00000C40
		public static PropertyInfo GetPropertyInfo(Type type, string name)
		{
			PropertyInfo propertyInfo;
			VariableCollection.GetPropertyInfo(type, name, 0, out propertyInfo);
			return propertyInfo;
		}
	}
}
