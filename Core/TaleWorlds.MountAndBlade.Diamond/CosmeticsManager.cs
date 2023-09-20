using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;

namespace TaleWorlds.MountAndBlade.Diamond
{
	public static class CosmeticsManager
	{
		public static MBReadOnlyList<CosmeticsManager.CosmeticElement> GetCosmeticElementList
		{
			get
			{
				if (!CosmeticsManager.HasXMLRead)
				{
					CosmeticsManager.LoadFromXml(ModuleHelper.GetModuleFullPath("Native") + "ModuleData/mpcosmetics.xml");
				}
				return CosmeticsManager.CosmeticElementList;
			}
		}

		public static CosmeticsManager.CosmeticElement GetCosmeticElement(string cosmeticId)
		{
			MBReadOnlyList<CosmeticsManager.CosmeticElement> getCosmeticElementList = CosmeticsManager.GetCosmeticElementList;
			if (getCosmeticElementList == null)
			{
				return null;
			}
			foreach (CosmeticsManager.CosmeticElement cosmeticElement in getCosmeticElementList)
			{
				if (cosmeticId == cosmeticElement.Id)
				{
					return cosmeticElement;
				}
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
			MBList<CosmeticsManager.CosmeticElement> mblist = new MBList<CosmeticsManager.CosmeticElement>();
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
							else
							{
								Debug.FailedAssert("Invalid cosmetic type: " + value2, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Diamond\\CosmeticsManager.cs", "LoadFromXml", 145);
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
								Debug.FailedAssert("Invalid cosmetic rarity: " + value3, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Diamond\\CosmeticsManager.cs", "LoadFromXml", 165);
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
								mblist.Add(new CosmeticsManager.ClothingCosmeticElement(value, cosmeticRarity, num, list, list2));
								break;
							}
							case CosmeticsManager.CosmeticType.Frame:
								mblist.Add(new CosmeticsManager.CosmeticElement(value, cosmeticRarity, num, cosmeticType));
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
								mblist.Add(new CosmeticsManager.SigilCosmeticElement(value, cosmeticRarity, num, text2));
								break;
							}
							}
						}
					}
				}
			}
			CosmeticsManager.CosmeticElementList = mblist;
			CosmeticsManager.HasXMLRead = true;
		}

		private static bool CheckForCosmeticsListDuplicatesDebug()
		{
			for (int i = 0; i < CosmeticsManager.CosmeticElementList.Count; i++)
			{
				for (int j = i + 1; j < CosmeticsManager.CosmeticElementList.Count; j++)
				{
					if (CosmeticsManager.CosmeticElementList[i].Id == CosmeticsManager.CosmeticElementList[j].Id)
					{
						Debug.FailedAssert(CosmeticsManager.CosmeticElementList[i].Id + " has more than one entry.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Diamond\\CosmeticsManager.cs", "CheckForCosmeticsListDuplicatesDebug", 228);
						return false;
					}
				}
			}
			return true;
		}

		private static MBReadOnlyList<CosmeticsManager.CosmeticElement> CosmeticElementList;

		public static bool HasXMLRead;

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
			Sigil
		}

		public class CosmeticElement
		{
			public bool IsFree
			{
				get
				{
					return this.Cost <= 0;
				}
			}

			public CosmeticElement(string id, CosmeticsManager.CosmeticRarity rarity, int cost, CosmeticsManager.CosmeticType type)
			{
				this.Id = id;
				this.Rarity = rarity;
				this.Cost = cost;
				this.Type = type;
			}

			public string Id;

			public CosmeticsManager.CosmeticRarity Rarity;

			public int Cost;

			public CosmeticsManager.CosmeticType Type;
		}

		public class ClothingCosmeticElement : CosmeticsManager.CosmeticElement
		{
			public ClothingCosmeticElement(string id, CosmeticsManager.CosmeticRarity rarity, int cost, List<string> replaceItemsId, List<Tuple<string, string>> replaceItemless)
				: base(id, rarity, cost, CosmeticsManager.CosmeticType.Clothing)
			{
				this.ReplaceItemsId = replaceItemsId;
				this.ReplaceItemless = replaceItemless;
			}

			public readonly List<string> ReplaceItemsId;

			public readonly List<Tuple<string, string>> ReplaceItemless;
		}

		public class SigilCosmeticElement : CosmeticsManager.CosmeticElement
		{
			public SigilCosmeticElement(string id, CosmeticsManager.CosmeticRarity rarity, int cost, string bannerCode)
				: base(id, rarity, cost, CosmeticsManager.CosmeticType.Sigil)
			{
				this.BannerCode = bannerCode;
			}

			public string BannerCode;
		}
	}
}
