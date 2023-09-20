﻿using System;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.Core
{
	public class CharacterSkills : PropertyOwner<SkillObject>
	{
		public CharacterSkills(CharacterSkills propertyOwner)
			: base(propertyOwner)
		{
		}

		public CharacterSkills()
		{
		}

		public override void Deserialize(MBObjectManager objectManager, XmlNode node)
		{
			this.Initialize();
			foreach (object obj in node.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (xmlNode.NodeType != XmlNodeType.Comment)
				{
					XmlAttributeCollection attributes = xmlNode.Attributes;
					string value = attributes["id"].Value;
					string value2 = attributes["value"].Value;
					SkillObject @object = Game.Current.ObjectManager.GetObject<SkillObject>(value);
					if (@object != null)
					{
						int num = ((value2 != null) ? Convert.ToInt32(value2) : 1);
						base.SetPropertyValue(@object, num);
					}
				}
			}
			foreach (object obj2 in node.ChildNodes)
			{
				XmlNode xmlNode2 = (XmlNode)obj2;
				if (xmlNode2.NodeType != XmlNodeType.Comment)
				{
					string text = xmlNode2.Name;
					int num2 = 1;
					if (text == "skill")
					{
						text = null;
						if (xmlNode2.Attributes != null)
						{
							XmlAttribute xmlAttribute = xmlNode2.Attributes["id"];
							if (xmlAttribute != null)
							{
								text = xmlAttribute.InnerText;
							}
						}
					}
					if (text != null)
					{
						SkillObject object2 = Game.Current.ObjectManager.GetObject<SkillObject>(text);
						if (object2 != null)
						{
							XmlAttribute xmlAttribute2 = xmlNode2.Attributes["value"];
							if (xmlAttribute2 != null)
							{
								string value3 = xmlAttribute2.Value;
								num2 = ((value3 != null) ? Convert.ToInt32(value3) : 1);
							}
							base.SetPropertyValue(object2, num2);
						}
					}
				}
			}
		}

		internal static void AutoGeneratedStaticCollectObjectsCharacterSkills(object o, List<object> collectedObjects)
		{
			((CharacterSkills)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
		}
	}
}
