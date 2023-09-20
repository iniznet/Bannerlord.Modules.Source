using System;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.Library;

namespace TaleWorlds.TwoDimension
{
	public class SpriteData
	{
		public Dictionary<string, SpritePart> SpritePartNames { get; private set; }

		public Dictionary<string, Sprite> SpriteNames { get; private set; }

		public Dictionary<string, SpriteCategory> SpriteCategories { get; private set; }

		public string Name { get; private set; }

		public SpriteData(string name)
		{
			this.Name = name;
			this.SpritePartNames = new Dictionary<string, SpritePart>();
			this.SpriteNames = new Dictionary<string, Sprite>();
			this.SpriteCategories = new Dictionary<string, SpriteCategory>();
		}

		public Sprite GetSprite(string name)
		{
			Sprite sprite;
			if (this.SpriteNames.TryGetValue(name, out sprite))
			{
				return sprite;
			}
			return null;
		}

		public bool SpriteExists(string spriteName)
		{
			return this.GetSprite(spriteName) != null;
		}

		private void LoadFromDepot(ResourceDepot resourceDepot)
		{
			XmlDocument xmlDocument = new XmlDocument();
			foreach (string text in resourceDepot.GetFilesEndingWith(this.Name + ".xml"))
			{
				xmlDocument.Load(text);
				XmlElement xmlElement = xmlDocument["SpriteData"];
				XmlNode xmlNode = xmlElement["SpriteCategories"];
				XmlNode xmlNode2 = xmlElement["SpriteParts"];
				XmlNode xmlNode3 = xmlElement["Sprites"];
				foreach (object obj in xmlNode)
				{
					XmlNode xmlNode4 = (XmlNode)obj;
					string innerText = xmlNode4["Name"].InnerText;
					int num = Convert.ToInt32(xmlNode4["SpriteSheetCount"].InnerText);
					bool flag = false;
					Vec2i[] array = new Vec2i[num];
					foreach (object obj2 in xmlNode4.ChildNodes)
					{
						XmlNode xmlNode5 = (XmlNode)obj2;
						if (xmlNode5.Name == "SpriteSheetSize")
						{
							int num2 = Convert.ToInt32(xmlNode5.Attributes["ID"].InnerText);
							int num3 = Convert.ToInt32(xmlNode5.Attributes["Width"].InnerText);
							int num4 = Convert.ToInt32(xmlNode5.Attributes["Height"].InnerText);
							array[num2 - 1] = new Vec2i(num3, num4);
						}
						else if (xmlNode5.Name == "AlwaysLoad")
						{
							flag = true;
						}
					}
					SpriteCategory spriteCategory = new SpriteCategory(innerText, this, num, flag)
					{
						SheetSizes = array
					};
					this.SpriteCategories[spriteCategory.Name] = spriteCategory;
				}
				foreach (object obj3 in xmlNode2)
				{
					XmlNode xmlNode6 = (XmlNode)obj3;
					string innerText2 = xmlNode6["Name"].InnerText;
					int num5 = Convert.ToInt32(xmlNode6["Width"].InnerText);
					int num6 = Convert.ToInt32(xmlNode6["Height"].InnerText);
					string innerText3 = xmlNode6["CategoryName"].InnerText;
					SpriteCategory spriteCategory2 = this.SpriteCategories[innerText3];
					SpritePart spritePart = new SpritePart(innerText2, spriteCategory2, num5, num6)
					{
						SheetID = Convert.ToInt32(xmlNode6["SheetID"].InnerText),
						SheetX = Convert.ToInt32(xmlNode6["SheetX"].InnerText),
						SheetY = Convert.ToInt32(xmlNode6["SheetY"].InnerText)
					};
					this.SpritePartNames[spritePart.Name] = spritePart;
					spritePart.UpdateInitValues();
				}
				foreach (object obj4 in xmlNode3)
				{
					XmlNode xmlNode7 = (XmlNode)obj4;
					Sprite sprite = null;
					if (xmlNode7.Name == "GenericSprite")
					{
						string innerText4 = xmlNode7["Name"].InnerText;
						string innerText5 = xmlNode7["SpritePartName"].InnerText;
						SpritePart spritePart2 = this.SpritePartNames[innerText5];
						sprite = new SpriteGeneric(innerText4, spritePart2);
					}
					else if (xmlNode7.Name == "NineRegionSprite")
					{
						string innerText6 = xmlNode7["Name"].InnerText;
						string innerText7 = xmlNode7["SpritePartName"].InnerText;
						int num7 = Convert.ToInt32(xmlNode7["LeftWidth"].InnerText);
						int num8 = Convert.ToInt32(xmlNode7["RightWidth"].InnerText);
						int num9 = Convert.ToInt32(xmlNode7["TopHeight"].InnerText);
						int num10 = Convert.ToInt32(xmlNode7["BottomHeight"].InnerText);
						sprite = new SpriteNineRegion(innerText6, this.SpritePartNames[innerText7], num7, num8, num9, num10);
					}
					this.SpriteNames[sprite.Name] = sprite;
				}
			}
		}

		public void Load(ResourceDepot resourceDepot)
		{
			this.LoadFromDepot(resourceDepot);
		}
	}
}
