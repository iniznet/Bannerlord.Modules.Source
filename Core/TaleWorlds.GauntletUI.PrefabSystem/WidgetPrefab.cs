using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using TaleWorlds.Library;

namespace TaleWorlds.GauntletUI.PrefabSystem
{
	public class WidgetPrefab
	{
		public Dictionary<string, VisualDefinitionTemplate> VisualDefinitionTemplates { get; set; }

		public Dictionary<string, ConstantDefinition> Constants { get; set; }

		public Dictionary<string, string> Parameters { get; set; }

		public Dictionary<string, XmlElement> CustomElements { get; set; }

		public WidgetTemplate RootTemplate { get; private set; }

		public WidgetPrefab()
		{
			this.VisualDefinitionTemplates = new Dictionary<string, VisualDefinitionTemplate>();
			this.Constants = new Dictionary<string, ConstantDefinition>();
			this.Parameters = new Dictionary<string, string>();
			this.CustomElements = new Dictionary<string, XmlElement>();
		}

		private static Dictionary<string, VisualDefinitionTemplate> LoadVisualDefinitions(XmlNode visualDefinitionsNode)
		{
			Dictionary<string, VisualDefinitionTemplate> dictionary = new Dictionary<string, VisualDefinitionTemplate>();
			foreach (object obj in visualDefinitionsNode.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				VisualDefinitionTemplate visualDefinitionTemplate = new VisualDefinitionTemplate();
				visualDefinitionTemplate.Name = xmlNode.Attributes["Name"].Value;
				XmlAttribute xmlAttribute = xmlNode.Attributes["TransitionDuration"];
				if (xmlAttribute != null)
				{
					visualDefinitionTemplate.TransitionDuration = Convert.ToSingle(xmlAttribute.Value, CultureInfo.InvariantCulture);
				}
				XmlAttribute xmlAttribute2 = xmlNode.Attributes["EaseIn"];
				bool flag;
				if (xmlAttribute2 != null && bool.TryParse(xmlAttribute2.Value, out flag))
				{
					visualDefinitionTemplate.EaseIn = flag;
				}
				XmlAttribute xmlAttribute3 = xmlNode.Attributes["DelayOnBegin"];
				if (xmlAttribute3 != null)
				{
					visualDefinitionTemplate.DelayOnBegin = Convert.ToSingle(xmlAttribute3.Value, CultureInfo.InvariantCulture);
				}
				foreach (object obj2 in xmlNode.ChildNodes)
				{
					XmlNode xmlNode2 = (XmlNode)obj2;
					VisualStateTemplate visualStateTemplate = new VisualStateTemplate();
					foreach (object obj3 in xmlNode2.Attributes)
					{
						XmlAttribute xmlAttribute4 = (XmlAttribute)obj3;
						string name = xmlAttribute4.Name;
						string value = xmlAttribute4.Value;
						if (name == "State")
						{
							visualStateTemplate.State = value;
						}
						else
						{
							visualStateTemplate.SetAttribute(name, value);
						}
					}
					visualDefinitionTemplate.AddVisualState(visualStateTemplate);
				}
				dictionary.Add(visualDefinitionTemplate.Name, visualDefinitionTemplate);
			}
			return dictionary;
		}

		private static void SaveVisualDefinitionsTo(XmlNode visualDefinitionsNode, Dictionary<string, VisualDefinitionTemplate> visualDefinitionTemplates)
		{
			foreach (VisualDefinitionTemplate visualDefinitionTemplate in visualDefinitionTemplates.Values)
			{
				visualDefinitionTemplate.Save(visualDefinitionsNode);
			}
		}

		private static Dictionary<string, string> LoadParameters(XmlNode constantsNode)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (object obj in constantsNode.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				string value = xmlNode.Attributes["Name"].Value;
				string value2 = xmlNode.Attributes["DefaultValue"].Value;
				dictionary.Add(value, value2);
			}
			return dictionary;
		}

		private static Dictionary<string, XmlElement> LoadCustomElements(XmlNode customElementsNode)
		{
			Dictionary<string, XmlElement> dictionary = new Dictionary<string, XmlElement>();
			foreach (object obj in customElementsNode.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				string value = xmlNode.Attributes["Name"].Value;
				XmlElement xmlElement = xmlNode.FirstChild as XmlElement;
				dictionary.Add(value, xmlElement);
			}
			return dictionary;
		}

		private static void SaveParametersTo(XmlNode parametersNode, Dictionary<string, string> parameters)
		{
			foreach (KeyValuePair<string, string> keyValuePair in parameters)
			{
				XmlNode xmlNode = parametersNode.OwnerDocument.CreateElement("Parameter");
				XmlAttribute xmlAttribute = parametersNode.OwnerDocument.CreateAttribute(keyValuePair.Key);
				xmlAttribute.InnerText = keyValuePair.Value;
				xmlNode.Attributes.Append(xmlAttribute);
				parametersNode.AppendChild(xmlNode);
			}
		}

		private static Dictionary<string, ConstantDefinition> LoadConstants(XmlNode constantsNode)
		{
			Dictionary<string, ConstantDefinition> dictionary = new Dictionary<string, ConstantDefinition>();
			foreach (object obj in constantsNode.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				XmlAttribute xmlAttribute = xmlNode.Attributes["Name"];
				XmlAttribute xmlAttribute2 = xmlNode.Attributes["Value"];
				XmlAttribute xmlAttribute3 = xmlNode.Attributes["Prefix"];
				XmlAttribute xmlAttribute4 = xmlNode.Attributes["Suffix"];
				XmlAttribute xmlAttribute5 = xmlNode.Attributes["BrushName"];
				XmlAttribute xmlAttribute6 = xmlNode.Attributes["BrushLayer"];
				XmlAttribute xmlAttribute7 = xmlNode.Attributes["BrushValueType"];
				XmlAttribute xmlAttribute8 = xmlNode.Attributes["SpriteName"];
				XmlAttribute xmlAttribute9 = xmlNode.Attributes["SpriteValueType"];
				XmlAttribute xmlAttribute10 = xmlNode.Attributes["Additive"];
				XmlAttribute xmlAttribute11 = xmlNode.Attributes["MultiplyResult"];
				XmlAttribute xmlAttribute12 = xmlNode.Attributes["BooleanCheck"];
				XmlAttribute xmlAttribute13 = xmlNode.Attributes["OnTrue"];
				XmlAttribute xmlAttribute14 = xmlNode.Attributes["OnFalse"];
				ConstantDefinition constantDefinition = null;
				if (xmlAttribute != null)
				{
					string value = xmlAttribute.Value;
					if (xmlAttribute2 != null)
					{
						string value2 = xmlAttribute2.Value;
						constantDefinition = new ConstantDefinition(value);
						constantDefinition.Type = ConstantDefinitionType.Constant;
						constantDefinition.Value = value2;
					}
					else if (xmlAttribute12 != null && xmlAttribute13 != null && xmlAttribute14 != null)
					{
						string value3 = xmlAttribute12.Value;
						string value4 = xmlAttribute13.Value;
						string value5 = xmlAttribute14.Value;
						constantDefinition = new ConstantDefinition(value);
						constantDefinition.Value = value3;
						constantDefinition.OnTrueValue = value4;
						constantDefinition.OnFalseValue = value5;
						constantDefinition.Type = ConstantDefinitionType.BooleanCheck;
					}
					else if (xmlAttribute5 != null && xmlAttribute6 != null && xmlAttribute7 != null)
					{
						string value6 = xmlAttribute5.Value;
						string value7 = xmlAttribute6.Value;
						string value8 = xmlAttribute7.Value;
						if (value8 == "Width" || value8 == "Height")
						{
							constantDefinition = new ConstantDefinition(value);
							constantDefinition.BrushName = value6;
							constantDefinition.LayerName = value7;
							constantDefinition.Type = ((value8 == "Width") ? ConstantDefinitionType.BrushLayerWidth : ConstantDefinitionType.BrushLayerHeight);
						}
					}
					else if (xmlAttribute8 != null && xmlAttribute9 != null)
					{
						string value9 = xmlAttribute8.Value;
						string value10 = xmlAttribute9.Value;
						if (value10 == "Width" || value10 == "Height")
						{
							constantDefinition = new ConstantDefinition(value);
							constantDefinition.SpriteName = value9;
							constantDefinition.Type = ((value10 == "Width") ? ConstantDefinitionType.SpriteWidth : ConstantDefinitionType.SpriteHeight);
						}
					}
					if (constantDefinition != null && xmlAttribute3 != null)
					{
						string value11 = xmlAttribute3.Value;
						constantDefinition.Prefix = value11;
					}
					if (constantDefinition != null && xmlAttribute4 != null)
					{
						string value12 = xmlAttribute4.Value;
						constantDefinition.Suffix = value12;
					}
					if (constantDefinition != null && xmlAttribute10 != null)
					{
						string value13 = xmlAttribute10.Value;
						constantDefinition.Additive = value13;
					}
					if (constantDefinition != null && xmlAttribute11 != null)
					{
						string value14 = xmlAttribute11.Value;
						constantDefinition.MultiplyResult = Convert.ToSingle(value14, CultureInfo.InvariantCulture);
					}
				}
				if (constantDefinition != null)
				{
					dictionary.Add(constantDefinition.Name, constantDefinition);
				}
			}
			return dictionary;
		}

		private static void SaveConstantsTo(XmlNode constantsNode, Dictionary<string, ConstantDefinition> constants)
		{
			foreach (ConstantDefinition constantDefinition in constants.Values)
			{
				XmlNode xmlNode = constantsNode.OwnerDocument.CreateElement("Constant");
				XmlAttribute xmlAttribute = constantsNode.OwnerDocument.CreateAttribute("Name");
				xmlAttribute.InnerText = constantDefinition.Name;
				xmlNode.Attributes.Append(xmlAttribute);
				switch (constantDefinition.Type)
				{
				case ConstantDefinitionType.Constant:
				{
					XmlAttribute xmlAttribute2 = constantsNode.OwnerDocument.CreateAttribute("Value");
					xmlAttribute2.InnerText = constantDefinition.Value;
					xmlNode.Attributes.Append(xmlAttribute2);
					break;
				}
				case ConstantDefinitionType.BooleanCheck:
				{
					XmlAttribute xmlAttribute3 = constantsNode.OwnerDocument.CreateAttribute("BooleanCheck");
					xmlAttribute3.InnerText = constantDefinition.Value;
					xmlNode.Attributes.Append(xmlAttribute3);
					XmlAttribute xmlAttribute4 = constantsNode.OwnerDocument.CreateAttribute("OnTrue");
					xmlAttribute4.InnerText = constantDefinition.OnTrueValue;
					xmlNode.Attributes.Append(xmlAttribute4);
					XmlAttribute xmlAttribute5 = constantsNode.OwnerDocument.CreateAttribute("OnFalse");
					xmlAttribute5.InnerText = constantDefinition.OnFalseValue;
					xmlNode.Attributes.Append(xmlAttribute5);
					break;
				}
				case ConstantDefinitionType.BrushLayerWidth:
				{
					XmlAttribute xmlAttribute6 = constantsNode.OwnerDocument.CreateAttribute("BrushName");
					xmlAttribute6.InnerText = constantDefinition.BrushName;
					xmlNode.Attributes.Append(xmlAttribute6);
					XmlAttribute xmlAttribute7 = constantsNode.OwnerDocument.CreateAttribute("BrushLayer");
					xmlAttribute7.InnerText = constantDefinition.LayerName;
					xmlNode.Attributes.Append(xmlAttribute7);
					XmlAttribute xmlAttribute8 = constantsNode.OwnerDocument.CreateAttribute("BrushValueType");
					xmlAttribute8.InnerText = "Width";
					xmlNode.Attributes.Append(xmlAttribute8);
					break;
				}
				case ConstantDefinitionType.BrushLayerHeight:
				{
					XmlAttribute xmlAttribute9 = constantsNode.OwnerDocument.CreateAttribute("BrushName");
					xmlAttribute9.InnerText = constantDefinition.BrushName;
					xmlNode.Attributes.Append(xmlAttribute9);
					XmlAttribute xmlAttribute10 = constantsNode.OwnerDocument.CreateAttribute("BrushLayer");
					xmlAttribute10.InnerText = constantDefinition.LayerName;
					xmlNode.Attributes.Append(xmlAttribute10);
					XmlAttribute xmlAttribute11 = constantsNode.OwnerDocument.CreateAttribute("BrushValueType");
					xmlAttribute11.InnerText = "Height";
					xmlNode.Attributes.Append(xmlAttribute11);
					break;
				}
				case ConstantDefinitionType.SpriteWidth:
				{
					XmlAttribute xmlAttribute12 = constantsNode.OwnerDocument.CreateAttribute("SpriteName");
					xmlAttribute12.InnerText = constantDefinition.SpriteName;
					xmlNode.Attributes.Append(xmlAttribute12);
					XmlAttribute xmlAttribute13 = constantsNode.OwnerDocument.CreateAttribute("SpriteValueType");
					xmlAttribute13.InnerText = "Width";
					xmlNode.Attributes.Append(xmlAttribute13);
					break;
				}
				case ConstantDefinitionType.SpriteHeight:
				{
					XmlAttribute xmlAttribute14 = constantsNode.OwnerDocument.CreateAttribute("SpriteName");
					xmlAttribute14.InnerText = constantDefinition.SpriteName;
					xmlNode.Attributes.Append(xmlAttribute14);
					XmlAttribute xmlAttribute15 = constantsNode.OwnerDocument.CreateAttribute("SpriteValueType");
					xmlAttribute15.InnerText = "Height";
					xmlNode.Attributes.Append(xmlAttribute15);
					break;
				}
				default:
					Debug.FailedAssert("false", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI.PrefabSystem\\WidgetPrefab.cs", "SaveConstantsTo", 355);
					break;
				}
				if (!string.IsNullOrEmpty(constantDefinition.Additive))
				{
					XmlAttribute xmlAttribute16 = constantsNode.OwnerDocument.CreateAttribute("Additive");
					xmlAttribute16.InnerText = constantDefinition.Additive;
					xmlNode.Attributes.Append(xmlAttribute16);
				}
				if (constantDefinition.MultiplyResult != 1f)
				{
					XmlAttribute xmlAttribute17 = constantsNode.OwnerDocument.CreateAttribute("MultiplyResult");
					xmlAttribute17.InnerText = constantDefinition.MultiplyResult.ToString();
					xmlNode.Attributes.Append(xmlAttribute17);
				}
				constantsNode.AppendChild(xmlNode);
			}
		}

		public static WidgetPrefab LoadFrom(PrefabExtensionContext prefabExtensionContext, WidgetAttributeContext widgetAttributeContext, string path)
		{
			path = Path.GetFullPath(path);
			XmlDocument xmlDocument = new XmlDocument();
			using (XmlReader xmlReader = XmlReader.Create(path, new XmlReaderSettings
			{
				IgnoreComments = true
			}))
			{
				xmlDocument.Load(xmlReader);
			}
			WidgetPrefab widgetPrefab = new WidgetPrefab();
			XmlNode xmlNode = xmlDocument.SelectSingleNode("Prefab");
			WidgetTemplate widgetTemplate;
			if (xmlNode != null)
			{
				XmlNode xmlNode2 = xmlNode.SelectSingleNode("Parameters");
				XmlNode xmlNode3 = xmlNode.SelectSingleNode("Constants");
				XmlNode xmlNode4 = xmlNode.SelectSingleNode("Variables");
				XmlNode xmlNode5 = xmlNode.SelectSingleNode("VisualDefinitions");
				XmlNode xmlNode6 = xmlNode.SelectSingleNode("CustomElements");
				XmlNode firstChild = xmlNode.SelectSingleNode("Window").FirstChild;
				widgetTemplate = WidgetTemplate.LoadFrom(prefabExtensionContext, widgetAttributeContext, firstChild);
				if (xmlNode2 != null)
				{
					widgetPrefab.Parameters = WidgetPrefab.LoadParameters(xmlNode2);
				}
				if (xmlNode3 != null)
				{
					widgetPrefab.Constants = WidgetPrefab.LoadConstants(xmlNode3);
				}
				if (xmlNode6 != null)
				{
					widgetPrefab.CustomElements = WidgetPrefab.LoadCustomElements(xmlNode6);
				}
				if (xmlNode5 != null)
				{
					widgetPrefab.VisualDefinitionTemplates = WidgetPrefab.LoadVisualDefinitions(xmlNode5);
				}
			}
			else
			{
				XmlNode firstChild2 = xmlDocument.SelectSingleNode("Window").FirstChild;
				widgetTemplate = WidgetTemplate.LoadFrom(prefabExtensionContext, widgetAttributeContext, firstChild2);
			}
			widgetTemplate.SetRootTemplate(widgetPrefab);
			widgetPrefab.RootTemplate = widgetTemplate;
			foreach (PrefabExtension prefabExtension in prefabExtensionContext.PrefabExtensions)
			{
				prefabExtension.OnLoadingFinished(widgetPrefab);
			}
			return widgetPrefab;
		}

		public XmlDocument Save(PrefabExtensionContext prefabExtensionContext)
		{
			XmlDocument xmlDocument = new XmlDocument();
			XmlNode xmlNode = xmlDocument.CreateElement("Prefab");
			XmlNode xmlNode2 = xmlDocument.CreateElement("Parameters");
			WidgetPrefab.SaveParametersTo(xmlNode2, this.Parameters);
			xmlNode.AppendChild(xmlNode2);
			XmlNode xmlNode3 = xmlDocument.CreateElement("Constants");
			WidgetPrefab.SaveConstantsTo(xmlNode3, this.Constants);
			xmlNode.AppendChild(xmlNode3);
			XmlNode xmlNode4 = xmlDocument.CreateElement("VisualDefinitions");
			WidgetPrefab.SaveVisualDefinitionsTo(xmlNode4, this.VisualDefinitionTemplates);
			xmlNode.AppendChild(xmlNode4);
			XmlNode xmlNode5 = xmlDocument.CreateElement("Window");
			this.RootTemplate.Save(prefabExtensionContext, xmlNode5);
			xmlNode.AppendChild(xmlNode5);
			xmlDocument.AppendChild(xmlNode);
			return xmlDocument;
		}

		public WidgetInstantiationResult Instantiate(WidgetCreationData widgetCreationData)
		{
			return this.RootTemplate.Instantiate(widgetCreationData, new Dictionary<string, WidgetAttributeTemplate>());
		}

		public WidgetInstantiationResult Instantiate(WidgetCreationData widgetCreationData, Dictionary<string, WidgetAttributeTemplate> parameters)
		{
			return this.RootTemplate.Instantiate(widgetCreationData, parameters);
		}

		public void OnRelease()
		{
			this.RootTemplate.OnRelease();
		}

		public ConstantDefinition GetConstantValue(string name)
		{
			ConstantDefinition constantDefinition;
			this.Constants.TryGetValue(name, out constantDefinition);
			return constantDefinition;
		}

		public string GetParameterDefaultValue(string name)
		{
			string text;
			this.Parameters.TryGetValue(name, out text);
			return text;
		}
	}
}
