using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;
using TaleWorlds.MountAndBlade.Diamond.Cosmetics.CosmeticTypes;

namespace TaleWorlds.MountAndBlade.Diamond.Cosmetics
{
	public static class CosmeticsManager
	{
		static CosmeticsManager()
		{
			CosmeticsManager.LoadFromXml(ModuleHelper.GetModuleFullPath("Native") + "ModuleData/mpcosmetics.xml");
		}

		public static MBReadOnlyList<CosmeticElement> CosmeticElementsList
		{
			get
			{
				return CosmeticsManager._cosmeticElementList;
			}
		}

		public static CosmeticElement GetCosmeticElement(string cosmeticId)
		{
			CosmeticElement cosmeticElement;
			if (CosmeticsManager._cosmeticElementsLookup.TryGetValue(cosmeticId, out cosmeticElement))
			{
				return cosmeticElement;
			}
			return null;
		}

		public static void LoadFromXml(string path)
		{
			XmlDocument xmlDocument = new XmlDocument();
			StreamReader streamReader = new StreamReader(path);
			streamReader.ReadToEnd();
			xmlDocument.Load(path);
			streamReader.Close();
			CosmeticsManager._cosmeticElementsLookup.Clear();
			MBList<CosmeticElement> mblist = new MBList<CosmeticElement>();
			foreach (object obj in xmlDocument.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (xmlNode.Name == "Cosmetics")
				{
					foreach (object obj2 in xmlNode.ChildNodes)
					{
						XmlNode xmlNode2 = (XmlNode)obj2;
						if (xmlNode2.Name == "Cosmetic")
						{
							string value = xmlNode2.Attributes["id"].Value;
							CosmeticsManager.CosmeticType cosmeticType = CosmeticsManager.CosmeticType.Clothing;
							string value2 = xmlNode2.Attributes["type"].Value;
							if (value2 == "Clothing")
							{
								cosmeticType = CosmeticsManager.CosmeticType.Clothing;
							}
							else if (value2 == "Frame")
							{
								cosmeticType = CosmeticsManager.CosmeticType.Frame;
							}
							else if (value2 == "Sigil")
							{
								cosmeticType = CosmeticsManager.CosmeticType.Sigil;
							}
							else if (value2 == "Taunt")
							{
								cosmeticType = CosmeticsManager.CosmeticType.Taunt;
							}
							else
							{
								Debug.FailedAssert("Invalid cosmetic type: " + value2, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Diamond\\Cosmetics\\CosmeticsManager.cs", "LoadFromXml", 103);
							}
							CosmeticsManager.CosmeticRarity cosmeticRarity = CosmeticsManager.CosmeticRarity.Common;
							string value3 = xmlNode2.Attributes["rarity"].Value;
							if (value3 == "Common")
							{
								cosmeticRarity = CosmeticsManager.CosmeticRarity.Common;
							}
							else if (value3 == "Rare")
							{
								cosmeticRarity = CosmeticsManager.CosmeticRarity.Rare;
							}
							else if (value3 == "Unique")
							{
								cosmeticRarity = CosmeticsManager.CosmeticRarity.Unique;
							}
							else
							{
								Debug.FailedAssert("Invalid cosmetic rarity: " + value3, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Diamond\\Cosmetics\\CosmeticsManager.cs", "LoadFromXml", 123);
							}
							int num = int.Parse(xmlNode2.Attributes["cost"].Value);
							switch (cosmeticType)
							{
							case CosmeticsManager.CosmeticType.Clothing:
							{
								List<string> list = new List<string>();
								List<Tuple<string, string>> list2 = new List<Tuple<string, string>>();
								foreach (object obj3 in xmlNode2.ChildNodes)
								{
									XmlNode xmlNode3 = (XmlNode)obj3;
									if (xmlNode3.Name == "Replace")
									{
										foreach (object obj4 in xmlNode3.ChildNodes)
										{
											XmlNode xmlNode4 = (XmlNode)obj4;
											if (xmlNode4.Name == "Item")
											{
												list.Add(xmlNode4.Attributes.Item(0).Value);
											}
											else if (xmlNode4.Name == "Itemless")
											{
												list2.Add(Tuple.Create<string, string>(xmlNode4.Attributes.Item(0).Value, xmlNode4.Attributes.Item(1).Value));
											}
										}
									}
								}
								mblist.Add(new ClothingCosmeticElement(value, cosmeticRarity, num, list, list2));
								break;
							}
							case CosmeticsManager.CosmeticType.Frame:
								mblist.Add(new CosmeticElement(value, cosmeticRarity, num, cosmeticType));
								break;
							case CosmeticsManager.CosmeticType.Sigil:
							{
								XmlAttributeCollection attributes = xmlNode2.Attributes;
								string text;
								if (attributes == null)
								{
									text = null;
								}
								else
								{
									XmlAttribute xmlAttribute = attributes["banner_code"];
									text = ((xmlAttribute != null) ? xmlAttribute.Value : null);
								}
								string text2 = text;
								mblist.Add(new SigilCosmeticElement(value, cosmeticRarity, num, text2));
								break;
							}
							case CosmeticsManager.CosmeticType.Taunt:
							{
								XmlAttributeCollection attributes2 = xmlNode2.Attributes;
								string text3;
								if (attributes2 == null)
								{
									text3 = null;
								}
								else
								{
									XmlAttribute xmlAttribute2 = attributes2["name"];
									text3 = ((xmlAttribute2 != null) ? xmlAttribute2.Value : null);
								}
								string text4 = text3;
								TauntCosmeticElement tauntCosmeticElement = new TauntCosmeticElement(-1, value, cosmeticRarity, num, text4);
								mblist.Add(tauntCosmeticElement);
								break;
							}
							}
						}
					}
				}
			}
			CosmeticsManager._cosmeticElementsLookup = new Dictionary<string, CosmeticElement>();
			foreach (CosmeticElement cosmeticElement in mblist)
			{
				CosmeticsManager._cosmeticElementsLookup[cosmeticElement.Id] = cosmeticElement;
			}
			CosmeticsManager._cosmeticElementList = mblist;
		}

		private static bool CheckForCosmeticsListDuplicatesDebug()
		{
			for (int i = 0; i < CosmeticsManager._cosmeticElementList.Count; i++)
			{
				for (int j = i + 1; j < CosmeticsManager._cosmeticElementList.Count; j++)
				{
					if (CosmeticsManager._cosmeticElementList[i].Id == CosmeticsManager._cosmeticElementList[j].Id)
					{
						Debug.FailedAssert(CosmeticsManager._cosmeticElementList[i].Id + " has more than one entry.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Diamond\\Cosmetics\\CosmeticsManager.cs", "CheckForCosmeticsListDuplicatesDebug", 200);
						return false;
					}
				}
			}
			return true;
		}

		private static MBReadOnlyList<CosmeticElement> _cosmeticElementList = new MBReadOnlyList<CosmeticElement>();

		private static Dictionary<string, CosmeticElement> _cosmeticElementsLookup = new Dictionary<string, CosmeticElement>();

		public enum CosmeticRarity
		{
			Default,
			Common,
			Rare,
			Unique
		}

		public enum CosmeticType
		{
			Clothing,
			Frame,
			Sigil,
			Taunt
		}
	}
}
