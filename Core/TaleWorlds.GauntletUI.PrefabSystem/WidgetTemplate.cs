using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.PrefabSystem
{
	public class WidgetTemplate
	{
		public bool LogicalChildrenLocation { get; private set; }

		public string Id
		{
			get
			{
				WidgetAttributeTemplate firstAttributeIfExist = this.GetFirstAttributeIfExist<WidgetAttributeKeyTypeId>();
				if (firstAttributeIfExist != null)
				{
					return firstAttributeIfExist.Value;
				}
				return "";
			}
		}

		public string Type
		{
			get
			{
				return this._type;
			}
		}

		public int ChildCount
		{
			get
			{
				return this._children.Count;
			}
		}

		public Dictionary<string, WidgetAttributeTemplate> GivenParameters
		{
			get
			{
				return this.GetAttributesOf<WidgetAttributeKeyTypeParameter>().ToDictionary((WidgetAttributeTemplate key) => key.Key);
			}
		}

		public WidgetPrefab Prefab { get; private set; }

		public WidgetTemplate RootTemplate
		{
			get
			{
				return this.Prefab.RootTemplate;
			}
		}

		public Dictionary<Type, Dictionary<string, WidgetAttributeTemplate>> Attributes
		{
			get
			{
				return this._attributes;
			}
		}

		public object Tag { get; set; }

		public WidgetTemplate(string type)
		{
			this._type = type;
			this._extensionData = new Dictionary<string, object>();
			this.Tag = Guid.NewGuid();
			this._attributes = new Dictionary<Type, Dictionary<string, WidgetAttributeTemplate>>();
			this._children = new List<WidgetTemplate>();
			this._customTypeChildren = new List<WidgetTemplate>();
		}

		internal void LoadAttributeCollection(WidgetAttributeContext widgetAttributeContext, XmlAttributeCollection attributes)
		{
			foreach (WidgetAttributeKeyType widgetAttributeKeyType in widgetAttributeContext.RegisteredKeyTypes)
			{
				this._attributes.Add(widgetAttributeKeyType.GetType(), new Dictionary<string, WidgetAttributeTemplate>());
			}
			foreach (object obj in attributes)
			{
				XmlAttribute xmlAttribute = (XmlAttribute)obj;
				string name = xmlAttribute.Name;
				string value = xmlAttribute.Value;
				this.AddAttributeTo(widgetAttributeContext, name, value);
			}
		}

		public void AddExtensionData(string name, object data)
		{
			if (this._extensionData.ContainsKey(name))
			{
				this._extensionData[name] = data;
				return;
			}
			this._extensionData.Add(name, data);
		}

		public T GetExtensionData<T>(string name) where T : class
		{
			object obj;
			this._extensionData.TryGetValue(name, out obj);
			return obj as T;
		}

		public void RemoveExtensionData(string name)
		{
			this._extensionData.Remove(name);
		}

		public void AddExtensionData(object data)
		{
			this.AddExtensionData(data.GetType().Name, data);
		}

		public T GetExtensionData<T>() where T : class
		{
			return this.GetExtensionData<T>(typeof(T).Name);
		}

		public void RemoveExtensionData<T>() where T : class
		{
			this.RemoveExtensionData(typeof(T).Name);
		}

		public IEnumerable<WidgetAttributeTemplate> GetAttributesOf<T>() where T : WidgetAttributeKeyType
		{
			return this._attributes[typeof(T)].Values;
		}

		public IEnumerable<WidgetAttributeTemplate> GetAttributesOf<TKey, TValue>() where TKey : WidgetAttributeKeyType where TValue : WidgetAttributeValueType
		{
			IEnumerable<WidgetAttributeTemplate> attributesOf = this.GetAttributesOf<TKey>();
			foreach (WidgetAttributeTemplate widgetAttributeTemplate in attributesOf)
			{
				if (widgetAttributeTemplate.ValueType is TValue)
				{
					yield return widgetAttributeTemplate;
				}
			}
			IEnumerator<WidgetAttributeTemplate> enumerator = null;
			yield break;
			yield break;
		}

		public IEnumerable<WidgetAttributeTemplate> AllAttributes
		{
			get
			{
				foreach (Dictionary<string, WidgetAttributeTemplate> dictionary in this._attributes.Values)
				{
					foreach (WidgetAttributeTemplate widgetAttributeTemplate in dictionary.Values)
					{
						yield return widgetAttributeTemplate;
					}
					Dictionary<string, WidgetAttributeTemplate>.ValueCollection.Enumerator enumerator2 = default(Dictionary<string, WidgetAttributeTemplate>.ValueCollection.Enumerator);
				}
				Dictionary<Type, Dictionary<string, WidgetAttributeTemplate>>.ValueCollection.Enumerator enumerator = default(Dictionary<Type, Dictionary<string, WidgetAttributeTemplate>>.ValueCollection.Enumerator);
				yield break;
				yield break;
			}
		}

		public WidgetAttributeTemplate GetFirstAttributeIfExist<T>() where T : WidgetAttributeKeyType
		{
			using (IEnumerator<WidgetAttributeTemplate> enumerator = this.GetAttributesOf<T>().GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					return enumerator.Current;
				}
			}
			return null;
		}

		public void SetAttribute(WidgetAttributeTemplate attribute)
		{
			Dictionary<string, WidgetAttributeTemplate> dictionary = this._attributes[attribute.KeyType.GetType()];
			if (dictionary.ContainsKey(attribute.Key))
			{
				dictionary[attribute.Key] = attribute;
				return;
			}
			dictionary.Add(attribute.Key, attribute);
		}

		public WidgetTemplate GetChildAt(int i)
		{
			return this._children[i];
		}

		public void AddChild(WidgetTemplate child)
		{
			this._children.Add(child);
		}

		public void RemoveChild(WidgetTemplate child)
		{
			this._children.Remove(child);
		}

		public void SwapChildren(WidgetTemplate child1, WidgetTemplate child2)
		{
			int num = this._children.IndexOf(child1);
			int num2 = this._children.IndexOf(child2);
			WidgetTemplate widgetTemplate = this._children[num];
			this._children[num] = this._children[num2];
			this._children[num2] = widgetTemplate;
		}

		public WidgetInstantiationResult Instantiate(WidgetCreationData widgetCreationData, Dictionary<string, WidgetAttributeTemplate> parameters)
		{
			PrefabExtensionContext prefabExtensionContext = widgetCreationData.PrefabExtensionContext;
			WidgetInstantiationResult widgetInstantiationResult = this.CreateWidgets(widgetCreationData);
			this.SetAttributes(widgetCreationData, widgetInstantiationResult, parameters);
			foreach (PrefabExtension prefabExtension in prefabExtensionContext.PrefabExtensions)
			{
				prefabExtension.AfterAttributesSet(widgetCreationData, widgetInstantiationResult, parameters);
			}
			return widgetInstantiationResult;
		}

		private WidgetInstantiationResult CreateWidgets(WidgetCreationData widgetCreationData)
		{
			this._usedFactory = widgetCreationData.WidgetFactory;
			PrefabExtensionContext prefabExtensionContext = this._usedFactory.PrefabExtensionContext;
			UIContext context = widgetCreationData.Context;
			Widget widget = null;
			Widget parent = widgetCreationData.Parent;
			WidgetInstantiationResult widgetInstantiationResult = null;
			WidgetInstantiationResult widgetInstantiationResult3;
			if (this._usedFactory.IsCustomType(this._type))
			{
				WidgetInstantiationResult widgetInstantiationResult2 = this._usedFactory.GetCustomType(this._type).RootTemplate.CreateWidgets(widgetCreationData);
				this._customTypeChildren.AddRange(widgetInstantiationResult2.Children.Select((WidgetInstantiationResult c) => c.Template));
				widget = widgetInstantiationResult2.Widget;
				widgetInstantiationResult = new WidgetInstantiationResult(widget, this, widgetInstantiationResult2);
				widgetInstantiationResult3 = widgetInstantiationResult.GetLogicalOrDefaultChildrenLocation();
			}
			else
			{
				widget = this._usedFactory.CreateBuiltinWidget(context, this._type);
				widgetInstantiationResult = new WidgetInstantiationResult(widget, this);
				if (parent != null)
				{
					parent.AddChild(widget);
				}
				foreach (PrefabExtension prefabExtension in prefabExtensionContext.PrefabExtensions)
				{
					prefabExtension.OnWidgetCreated(widgetCreationData, widgetInstantiationResult, this.ChildCount);
				}
				widgetInstantiationResult3 = widgetInstantiationResult;
			}
			widget.Tag = this.Tag;
			widget.Id = this.Id;
			foreach (WidgetTemplate widgetTemplate in this._children)
			{
				WidgetCreationData widgetCreationData2 = new WidgetCreationData(widgetCreationData, widgetInstantiationResult3);
				WidgetInstantiationResult widgetInstantiationResult4 = widgetTemplate.CreateWidgets(widgetCreationData2);
				widgetInstantiationResult3.Children.Add(widgetInstantiationResult4);
			}
			return widgetInstantiationResult;
		}

		public void OnRelease()
		{
			if (this._usedFactory.IsCustomType(this._type))
			{
				this._usedFactory.OnUnload(this._type);
			}
			foreach (WidgetTemplate widgetTemplate in this._children)
			{
				widgetTemplate.OnRelease();
			}
			foreach (WidgetTemplate widgetTemplate2 in this._customTypeChildren)
			{
				widgetTemplate2.OnRelease();
			}
		}

		private void SetAttributes(WidgetCreationData widgetCreationData, WidgetInstantiationResult widgetInstantiationResult, Dictionary<string, WidgetAttributeTemplate> parameters)
		{
			BrushFactory brushFactory = widgetCreationData.BrushFactory;
			SpriteData spriteData = widgetCreationData.SpriteData;
			PrefabExtensionContext prefabExtensionContext = widgetCreationData.PrefabExtensionContext;
			Widget widget = widgetInstantiationResult.Widget;
			WidgetPrefab prefab = widgetInstantiationResult.Template.Prefab;
			foreach (PrefabExtension prefabExtension in prefabExtensionContext.PrefabExtensions)
			{
				prefabExtension.OnAttributesSet(widgetCreationData, widgetInstantiationResult, parameters);
			}
			if (widgetInstantiationResult.CustomWidgetInstantiationData != null)
			{
				WidgetInstantiationResult customWidgetInstantiationData = widgetInstantiationResult.CustomWidgetInstantiationData;
				WidgetTemplate template = customWidgetInstantiationData.Template;
				Dictionary<string, WidgetAttributeTemplate> dictionary = new Dictionary<string, WidgetAttributeTemplate>();
				foreach (KeyValuePair<string, WidgetAttributeTemplate> keyValuePair in this.GivenParameters)
				{
					string key = keyValuePair.Key;
					WidgetAttributeTemplate widgetAttributeTemplate = keyValuePair.Value;
					WidgetAttributeTemplate widgetAttributeTemplate2;
					if (widgetAttributeTemplate.KeyType is WidgetAttributeKeyTypeParameter && widgetAttributeTemplate.ValueType is WidgetAttributeValueTypeParameter && parameters.TryGetValue(key, out widgetAttributeTemplate2))
					{
						widgetAttributeTemplate = widgetAttributeTemplate2;
					}
					dictionary.Add(key, widgetAttributeTemplate);
				}
				template.SetAttributes(widgetCreationData, customWidgetInstantiationData, dictionary);
			}
			foreach (WidgetAttributeTemplate widgetAttributeTemplate3 in this.AllAttributes)
			{
				WidgetAttributeKeyType keyType = widgetAttributeTemplate3.KeyType;
				WidgetAttributeValueType valueType = widgetAttributeTemplate3.ValueType;
				string key2 = widgetAttributeTemplate3.Key;
				string value = widgetAttributeTemplate3.Value;
				if (keyType is WidgetAttributeKeyTypeAttribute)
				{
					if (valueType is WidgetAttributeValueTypeDefault)
					{
						WidgetExtensions.SetWidgetAttributeFromString(widget, key2, value, brushFactory, spriteData, prefab.VisualDefinitionTemplates, prefab.Constants, parameters, prefab.CustomElements, this.Prefab.Parameters);
					}
					else if (valueType is WidgetAttributeValueTypeConstant)
					{
						string value2 = prefab.GetConstantValue(value).GetValue(brushFactory, spriteData, prefab.Constants, parameters, this.Prefab.Parameters);
						if (!string.IsNullOrEmpty(value2))
						{
							WidgetExtensions.SetWidgetAttributeFromString(widget, key2, value2, brushFactory, spriteData, prefab.VisualDefinitionTemplates, prefab.Constants, parameters, prefab.CustomElements, this.Prefab.Parameters);
						}
					}
					else if (valueType is WidgetAttributeValueTypeParameter)
					{
						string text = value;
						string text2 = this.Prefab.GetParameterDefaultValue(text);
						WidgetAttributeTemplate widgetAttributeTemplate4;
						if (parameters.TryGetValue(text, out widgetAttributeTemplate4) && widgetAttributeTemplate4.ValueType is WidgetAttributeValueTypeDefault)
						{
							text2 = widgetAttributeTemplate4.Value;
						}
						if (!string.IsNullOrEmpty(text2))
						{
							WidgetExtensions.SetWidgetAttributeFromString(widget, key2, text2, brushFactory, spriteData, prefab.VisualDefinitionTemplates, prefab.Constants, parameters, prefab.CustomElements, this.Prefab.Parameters);
						}
					}
				}
			}
			foreach (WidgetInstantiationResult widgetInstantiationResult2 in widgetInstantiationResult.Children)
			{
				widgetInstantiationResult2.Template.SetAttributes(widgetCreationData, widgetInstantiationResult2, parameters);
			}
		}

		public static WidgetTemplate LoadFrom(PrefabExtensionContext prefabExtensionContext, WidgetAttributeContext widgetAttributeContext, XmlNode node)
		{
			WidgetTemplate widgetTemplate = new WidgetTemplate(node.Name);
			widgetTemplate.LoadAttributeCollection(widgetAttributeContext, node.Attributes);
			if (node.SelectSingleNode("LogicalChildrenLocation") != null)
			{
				widgetTemplate.LogicalChildrenLocation = true;
			}
			foreach (PrefabExtension prefabExtension in prefabExtensionContext.PrefabExtensions)
			{
				prefabExtension.DoLoading(prefabExtensionContext, widgetAttributeContext, widgetTemplate, node);
			}
			XmlNode xmlNode = node.SelectSingleNode("Children");
			if (xmlNode != null)
			{
				foreach (object obj in xmlNode.ChildNodes)
				{
					XmlNode xmlNode2 = (XmlNode)obj;
					WidgetTemplate widgetTemplate2 = WidgetTemplate.LoadFrom(prefabExtensionContext, widgetAttributeContext, xmlNode2);
					widgetTemplate.AddChild(widgetTemplate2);
				}
			}
			return widgetTemplate;
		}

		public void SetRootTemplate(WidgetPrefab prefab)
		{
			this.Prefab = prefab;
			foreach (WidgetTemplate widgetTemplate in this._children)
			{
				widgetTemplate.SetRootTemplate(prefab);
			}
		}

		public void AddAttributeTo(WidgetAttributeContext widgetAttributeContext, string name, string value)
		{
			WidgetAttributeKeyType keyType = widgetAttributeContext.GetKeyType(name);
			string keyName = keyType.GetKeyName(name);
			WidgetAttributeValueType valueType = widgetAttributeContext.GetValueType(value);
			string attributeValue = valueType.GetAttributeValue(value);
			this.SetAttribute(new WidgetAttributeTemplate
			{
				KeyType = keyType,
				ValueType = valueType,
				Key = keyName,
				Value = attributeValue
			});
		}

		public void RemoveAttributeFrom(WidgetAttributeContext widgetAttributeContext, string fullName)
		{
			WidgetAttributeKeyType keyType = widgetAttributeContext.GetKeyType(fullName);
			string keyName = keyType.GetKeyName(fullName);
			this.RemoveAttributeFrom(keyType, keyName);
		}

		public void RemoveAttributeFrom<T>(string name) where T : WidgetAttributeKeyType
		{
			Dictionary<string, WidgetAttributeTemplate> dictionary = this._attributes[typeof(T)];
			if (dictionary.ContainsKey(name))
			{
				dictionary.Remove(name);
			}
		}

		public void RemoveAttributeFrom(WidgetAttributeKeyType keyType, string name)
		{
			Dictionary<string, WidgetAttributeTemplate> dictionary = this._attributes[keyType.GetType()];
			if (dictionary.ContainsKey(name))
			{
				dictionary.Remove(name);
			}
		}

		private static void AddAttributeTo(XmlNode node, string name, string value)
		{
			XmlAttribute xmlAttribute = node.OwnerDocument.CreateAttribute(name);
			xmlAttribute.InnerText = value.ToString();
			node.Attributes.Append(xmlAttribute);
		}

		public void Save(PrefabExtensionContext prefabExtensionContext, XmlNode parentNode)
		{
			XmlDocument ownerDocument = parentNode.OwnerDocument;
			XmlNode xmlNode = ownerDocument.CreateElement(this._type);
			foreach (WidgetAttributeTemplate widgetAttributeTemplate in this.AllAttributes)
			{
				WidgetAttributeKeyType keyType = widgetAttributeTemplate.KeyType;
				WidgetAttributeValueType valueType = widgetAttributeTemplate.ValueType;
				string key = widgetAttributeTemplate.Key;
				string value = widgetAttributeTemplate.Value;
				string serializedKey = keyType.GetSerializedKey(key);
				string serializedValue = valueType.GetSerializedValue(value);
				WidgetTemplate.AddAttributeTo(xmlNode, serializedKey, serializedValue);
			}
			foreach (PrefabExtension prefabExtension in prefabExtensionContext.PrefabExtensions)
			{
				prefabExtension.OnSave(prefabExtensionContext, xmlNode, this);
			}
			if (this._children.Count > 0)
			{
				XmlNode xmlNode2 = ownerDocument.CreateElement("Children");
				foreach (WidgetTemplate widgetTemplate in this._children)
				{
					widgetTemplate.Save(prefabExtensionContext, xmlNode2);
				}
				xmlNode.AppendChild(xmlNode2);
			}
			parentNode.AppendChild(xmlNode);
		}

		private string _type;

		private WidgetFactory _usedFactory;

		private List<WidgetTemplate> _children;

		private List<WidgetTemplate> _customTypeChildren;

		private Dictionary<Type, Dictionary<string, WidgetAttributeTemplate>> _attributes;

		private Dictionary<string, object> _extensionData;
	}
}
