﻿using System;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.Core
{
	public class PropertyOwnerF<T> : MBObjectBase where T : MBObjectBase
	{
		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this._attributes);
		}

		public PropertyOwnerF()
		{
			this._attributes = new Dictionary<T, float>();
		}

		public PropertyOwnerF(PropertyOwnerF<T> propertyOwner)
		{
			this._attributes = new Dictionary<T, float>(propertyOwner._attributes);
		}

		public void SetPropertyValue(T attribute, float value)
		{
			if (!value.ApproximatelyEqualsTo(0f, 1E-05f))
			{
				this._attributes[attribute] = value;
				return;
			}
			if (this.HasProperty(attribute))
			{
				this._attributes.Remove(attribute);
			}
		}

		public float GetPropertyValue(T attribute)
		{
			if (attribute == null)
			{
				Debug.FailedAssert("attribute in GetPropertyValue can not be null!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.Core\\PropertyOwner.cs", "GetPropertyValue", 146);
				return 0f;
			}
			float num;
			if (!this._attributes.TryGetValue(attribute, out num))
			{
				return 0f;
			}
			return num;
		}

		public bool HasProperty(T attribute)
		{
			return this._attributes.ContainsKey(attribute);
		}

		public void ClearAllProperty()
		{
			this._attributes.Clear();
		}

		public void Serialize(XmlWriter writer)
		{
			writer.WriteStartElement("attributes");
			foreach (KeyValuePair<T, float> keyValuePair in this._attributes)
			{
				writer.WriteStartElement("attribute");
				writer.WriteAttributeString("id", keyValuePair.Key.StringId);
				writer.WriteAttributeString("value", keyValuePair.Value.ToString());
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}

		public override void Deserialize(MBObjectManager objectManager, XmlNode node)
		{
			this.Initialize();
			foreach (object obj in node.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (xmlNode.Name == "attributes")
				{
					foreach (object obj2 in xmlNode.ChildNodes)
					{
						XmlNode xmlNode2 = (XmlNode)obj2;
						if (xmlNode2.Name == "attribute")
						{
							string innerText = xmlNode2.Attributes["id"].InnerText;
							if (innerText.Substring(0, innerText.IndexOf('.')).Equals("Attrib"))
							{
								T t = objectManager.ReadObjectReferenceFromXml("id", typeof(T), xmlNode2) as T;
								int num = Convert.ToInt32(xmlNode2.Attributes["value"].Value);
								this.SetPropertyValue(t, (float)num);
							}
						}
					}
				}
			}
		}

		[SaveableField(10)]
		protected Dictionary<T, float> _attributes;
	}
}
