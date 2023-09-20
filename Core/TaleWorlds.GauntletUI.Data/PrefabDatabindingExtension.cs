using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.PrefabSystem;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.Data
{
	// Token: 0x0200000A RID: 10
	public class PrefabDatabindingExtension : PrefabExtension
	{
		// Token: 0x0600007F RID: 127 RVA: 0x00003A3E File Offset: 0x00001C3E
		private static WidgetAttributeTemplate GetDataSource(WidgetTemplate widgetTemplate)
		{
			return widgetTemplate.GetFirstAttributeIfExist<WidgetAttributeKeyTypeDataSource>();
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00003A48 File Offset: 0x00001C48
		protected override void RegisterAttributeTypes(WidgetAttributeContext widgetAttributeContext)
		{
			WidgetAttributeKeyTypeDataSource widgetAttributeKeyTypeDataSource = new WidgetAttributeKeyTypeDataSource();
			WidgetAttributeKeyTypeCommand widgetAttributeKeyTypeCommand = new WidgetAttributeKeyTypeCommand();
			WidgetAttributeKeyTypeCommandParameter widgetAttributeKeyTypeCommandParameter = new WidgetAttributeKeyTypeCommandParameter();
			widgetAttributeContext.RegisterKeyType(widgetAttributeKeyTypeDataSource);
			widgetAttributeContext.RegisterKeyType(widgetAttributeKeyTypeCommand);
			widgetAttributeContext.RegisterKeyType(widgetAttributeKeyTypeCommandParameter);
			WidgetAttributeValueTypeBindingPath widgetAttributeValueTypeBindingPath = new WidgetAttributeValueTypeBindingPath();
			WidgetAttributeValueTypeBinding widgetAttributeValueTypeBinding = new WidgetAttributeValueTypeBinding();
			widgetAttributeContext.RegisterValueType(widgetAttributeValueTypeBindingPath);
			widgetAttributeContext.RegisterValueType(widgetAttributeValueTypeBinding);
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00003A98 File Offset: 0x00001C98
		protected override void OnWidgetCreated(WidgetCreationData widgetCreationData, WidgetInstantiationResult widgetInstantiationResult, int childCount)
		{
			GauntletMovie extensionData = widgetCreationData.GetExtensionData<GauntletMovie>();
			if (extensionData != null)
			{
				WidgetTemplate template = widgetInstantiationResult.Template;
				Widget widget = widgetInstantiationResult.Widget;
				Widget parent = widgetCreationData.Parent;
				GauntletView gauntletView = ((parent != null) ? parent.GetComponent<GauntletView>() : null);
				GauntletView gauntletView2 = new GauntletView(extensionData, gauntletView, widget, childCount);
				widget.AddComponent(gauntletView2);
				widgetInstantiationResult.AddExtensionData(gauntletView2, true);
				ItemTemplateUsage extensionData2 = template.GetExtensionData<ItemTemplateUsage>();
				if (extensionData2 != null)
				{
					gauntletView2.ItemTemplateUsageWithData = new ItemTemplateUsageWithData(extensionData2);
				}
				if (gauntletView != null)
				{
					gauntletView.AddChild(gauntletView2);
				}
			}
		}

		// Token: 0x06000082 RID: 130 RVA: 0x00003B0C File Offset: 0x00001D0C
		protected override void OnSave(PrefabExtensionContext prefabExtensionContext, XmlNode node, WidgetTemplate widgetTemplate)
		{
			ItemTemplateUsage extensionData = widgetTemplate.GetExtensionData<ItemTemplateUsage>();
			if (extensionData != null)
			{
				XmlDocument ownerDocument = node.OwnerDocument;
				XmlNode xmlNode = ownerDocument.CreateElement("ItemTemplate");
				extensionData.DefaultItemTemplate.Save(prefabExtensionContext, xmlNode);
				node.AppendChild(xmlNode);
				if (extensionData.FirstItemTemplate != null)
				{
					XmlNode xmlNode2 = ownerDocument.CreateElement("ItemTemplate");
					XmlAttribute xmlAttribute = ownerDocument.CreateAttribute("Type");
					xmlAttribute.InnerText = "First";
					xmlNode2.Attributes.Append(xmlAttribute);
					extensionData.DefaultItemTemplate.Save(prefabExtensionContext, xmlNode2);
					node.AppendChild(xmlNode2);
				}
				if (extensionData.LastItemTemplate != null)
				{
					XmlNode xmlNode3 = ownerDocument.CreateElement("ItemTemplate");
					XmlAttribute xmlAttribute2 = ownerDocument.CreateAttribute("Type");
					xmlAttribute2.InnerText = "Last";
					xmlNode3.Attributes.Append(xmlAttribute2);
					extensionData.DefaultItemTemplate.Save(prefabExtensionContext, xmlNode3);
					node.AppendChild(xmlNode3);
				}
			}
		}

		// Token: 0x06000083 RID: 131 RVA: 0x00003BF4 File Offset: 0x00001DF4
		protected override void OnAttributesSet(WidgetCreationData widgetCreationData, WidgetInstantiationResult widgetInstantiationResult, Dictionary<string, WidgetAttributeTemplate> parameters)
		{
			if (widgetInstantiationResult.Template.GetExtensionData<ItemTemplateUsage>() != null)
			{
				ItemTemplateUsageWithData itemTemplateUsageWithData = widgetInstantiationResult.GetGauntletView().ItemTemplateUsageWithData;
				foreach (KeyValuePair<string, WidgetAttributeTemplate> keyValuePair in parameters)
				{
					string key = keyValuePair.Key;
					WidgetAttributeTemplate value = keyValuePair.Value;
					itemTemplateUsageWithData.GivenParameters.Add(key, value);
				}
			}
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00003C74 File Offset: 0x00001E74
		protected override void DoLoading(PrefabExtensionContext prefabExtensionContext, WidgetAttributeContext widgetAttributeContext, WidgetTemplate template, XmlNode node)
		{
			XmlNodeList xmlNodeList = node.SelectNodes("ItemTemplate");
			ItemTemplateUsage itemTemplateUsage = null;
			foreach (object obj in xmlNodeList)
			{
				XmlNode xmlNode = (XmlNode)obj;
				XmlAttribute xmlAttribute = xmlNode.Attributes["Type"];
				if (xmlAttribute == null || xmlAttribute.Value == "Default")
				{
					XmlNode firstChild = xmlNode.FirstChild;
					itemTemplateUsage = new ItemTemplateUsage(WidgetTemplate.LoadFrom(prefabExtensionContext, widgetAttributeContext, firstChild));
					template.AddExtensionData(itemTemplateUsage);
				}
			}
			if (itemTemplateUsage != null)
			{
				foreach (object obj2 in xmlNodeList)
				{
					XmlNode xmlNode2 = (XmlNode)obj2;
					XmlAttribute xmlAttribute2 = xmlNode2.Attributes["Type"];
					if (xmlAttribute2 != null && !(xmlAttribute2.Value == "Default"))
					{
						XmlNode firstChild2 = xmlNode2.FirstChild;
						WidgetTemplate widgetTemplate = WidgetTemplate.LoadFrom(prefabExtensionContext, widgetAttributeContext, firstChild2);
						if (xmlAttribute2.Value == "First")
						{
							itemTemplateUsage.FirstItemTemplate = widgetTemplate;
						}
						else if (xmlAttribute2.Value == "Last")
						{
							itemTemplateUsage.LastItemTemplate = widgetTemplate;
						}
					}
				}
			}
		}

		// Token: 0x06000085 RID: 133 RVA: 0x00003DDC File Offset: 0x00001FDC
		protected override void OnLoadingFinished(WidgetPrefab widgetPrefab)
		{
			this.SetRootTemplate(widgetPrefab.RootTemplate, widgetPrefab);
		}

		// Token: 0x06000086 RID: 134 RVA: 0x00003DEC File Offset: 0x00001FEC
		private void SetRootTemplate(WidgetTemplate widgetTemplate, WidgetPrefab prefab)
		{
			ItemTemplateUsage extensionData = widgetTemplate.GetExtensionData<ItemTemplateUsage>();
			if (extensionData != null)
			{
				if (extensionData.FirstItemTemplate != null)
				{
					extensionData.FirstItemTemplate.SetRootTemplate(prefab);
					this.SetRootTemplate(extensionData.FirstItemTemplate, prefab);
				}
				if (extensionData.LastItemTemplate != null)
				{
					extensionData.LastItemTemplate.SetRootTemplate(prefab);
					this.SetRootTemplate(extensionData.LastItemTemplate, prefab);
				}
				if (extensionData.DefaultItemTemplate != null)
				{
					extensionData.DefaultItemTemplate.SetRootTemplate(prefab);
					this.SetRootTemplate(extensionData.DefaultItemTemplate, prefab);
				}
			}
			for (int i = 0; i < widgetTemplate.ChildCount; i++)
			{
				WidgetTemplate childAt = widgetTemplate.GetChildAt(i);
				this.SetRootTemplate(childAt, prefab);
			}
		}

		// Token: 0x06000087 RID: 135 RVA: 0x00003E88 File Offset: 0x00002088
		protected override void AfterAttributesSet(WidgetCreationData widgetCreationData, WidgetInstantiationResult widgetInstantiationResult, Dictionary<string, WidgetAttributeTemplate> parameters)
		{
			WidgetTemplate template = widgetInstantiationResult.Template;
			GauntletView gauntletView = widgetInstantiationResult.GetGauntletView();
			bool flag = gauntletView.Parent == null;
			WidgetAttributeTemplate dataSource = PrefabDatabindingExtension.GetDataSource(template);
			WidgetPrefab prefab = template.Prefab;
			BrushFactory brushFactory = widgetCreationData.BrushFactory;
			SpriteData spriteData = widgetCreationData.SpriteData;
			BindingPath bindingPath = null;
			if (flag)
			{
				bindingPath = new BindingPath("Root");
			}
			else if (dataSource != null)
			{
				if (dataSource.ValueType is WidgetAttributeValueTypeBindingPath)
				{
					bindingPath = new BindingPath(dataSource.Value);
				}
				else if (dataSource.ValueType is WidgetAttributeValueTypeParameter)
				{
					string value = dataSource.Value;
					string text = prefab.GetParameterDefaultValue(value);
					if (parameters.ContainsKey(value) && parameters[value].ValueType is WidgetAttributeValueTypeBindingPath)
					{
						text = parameters[value].Value;
					}
					if (!string.IsNullOrEmpty(value))
					{
						bindingPath = new BindingPath(text);
					}
				}
				else if (dataSource.ValueType is WidgetAttributeValueTypeConstant)
				{
					string value2 = dataSource.Value;
					string value3 = prefab.GetConstantValue(value2).GetValue(brushFactory, spriteData, prefab.Constants, parameters, prefab.Parameters);
					if (!string.IsNullOrEmpty(value3))
					{
						bindingPath = new BindingPath(value3);
					}
				}
			}
			Dictionary<string, WidgetAttributeTemplate> dictionary = template.GetAttributesOf<WidgetAttributeKeyTypeParameter>().ToDictionary((WidgetAttributeTemplate key) => key.Key);
			foreach (KeyValuePair<string, WidgetAttributeTemplate> keyValuePair in parameters)
			{
				if (!dictionary.ContainsKey(keyValuePair.Key))
				{
					dictionary.Add(keyValuePair.Key, keyValuePair.Value);
				}
			}
			if (widgetInstantiationResult.CustomWidgetInstantiationData != null)
			{
				WidgetInstantiationResult customWidgetInstantiationData = widgetInstantiationResult.CustomWidgetInstantiationData;
				this.AfterAttributesSet(widgetCreationData, customWidgetInstantiationData, dictionary);
			}
			if (bindingPath != null)
			{
				gauntletView.InitializeViewModelPath(bindingPath);
			}
			IEnumerable<WidgetAttributeTemplate> attributesOf = template.GetAttributesOf<WidgetAttributeKeyTypeCommand>();
			Dictionary<string, WidgetAttributeTemplate> dictionary2 = template.GetAttributesOf<WidgetAttributeKeyTypeCommandParameter>().ToDictionary((WidgetAttributeTemplate key) => key.Key);
			foreach (WidgetAttributeTemplate widgetAttributeTemplate in attributesOf)
			{
				string key3 = widgetAttributeTemplate.Key;
				string value4 = widgetAttributeTemplate.Value;
				if (dictionary2.ContainsKey(key3))
				{
					WidgetAttributeTemplate widgetAttributeTemplate2 = dictionary2[key3];
					string text2 = null;
					if (widgetAttributeTemplate2.ValueType is WidgetAttributeValueTypeDefault)
					{
						text2 = widgetAttributeTemplate2.Value;
					}
					else if (parameters.ContainsKey(widgetAttributeTemplate2.Value))
					{
						text2 = parameters[widgetAttributeTemplate2.Value].Value;
					}
					if (!string.IsNullOrEmpty(text2))
					{
						gauntletView.BindCommand(key3, new BindingPath(value4), text2);
					}
					else
					{
						gauntletView.BindCommand(key3, new BindingPath(value4), null);
					}
				}
				else
				{
					string text3 = value4;
					if (widgetAttributeTemplate.ValueType is WidgetAttributeValueTypeParameter && parameters.ContainsKey(value4))
					{
						text3 = parameters[value4].Value;
					}
					gauntletView.BindCommand(key3, new BindingPath(text3), null);
				}
			}
			foreach (WidgetAttributeTemplate widgetAttributeTemplate3 in template.AllAttributes)
			{
				WidgetAttributeKeyType keyType = widgetAttributeTemplate3.KeyType;
				WidgetAttributeValueType valueType = widgetAttributeTemplate3.ValueType;
				string key2 = widgetAttributeTemplate3.Key;
				string value5 = widgetAttributeTemplate3.Value;
				if (keyType is WidgetAttributeKeyTypeAttribute || keyType is WidgetAttributeKeyTypeId)
				{
					if (valueType is WidgetAttributeValueTypeBinding)
					{
						gauntletView.BindData(key2, new BindingPath(value5));
					}
					else if (valueType is WidgetAttributeValueTypeParameter)
					{
						string text4 = value5;
						if (parameters.ContainsKey(text4))
						{
							WidgetAttributeTemplate widgetAttributeTemplate4 = parameters[text4];
							if (widgetAttributeTemplate4.ValueType is WidgetAttributeValueTypeBinding)
							{
								string value6 = widgetAttributeTemplate4.Value;
								if (!string.IsNullOrEmpty(value6))
								{
									gauntletView.BindData(key2, new BindingPath(value6));
								}
							}
						}
					}
				}
			}
			foreach (WidgetInstantiationResult widgetInstantiationResult2 in widgetInstantiationResult.Children)
			{
				this.AfterAttributesSet(widgetCreationData, widgetInstantiationResult2, parameters);
			}
		}
	}
}
