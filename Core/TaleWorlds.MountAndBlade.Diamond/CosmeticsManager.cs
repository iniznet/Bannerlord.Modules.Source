using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x02000106 RID: 262
	public static class CosmeticsManager
	{
		// Token: 0x170001D6 RID: 470
		// (get) Token: 0x060004BB RID: 1211 RVA: 0x00006AC2 File Offset: 0x00004CC2
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

		// Token: 0x060004BC RID: 1212 RVA: 0x00006AEC File Offset: 0x00004CEC
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

		// Token: 0x060004BD RID: 1213 RVA: 0x00006B54 File Offset: 0x00004D54
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

		// Token: 0x060004BE RID: 1214 RVA: 0x00006F58 File Offset: 0x00005158
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

		// Token: 0x0400021C RID: 540
		private static MBReadOnlyList<CosmeticsManager.CosmeticElement> CosmeticElementList;

		// Token: 0x0400021D RID: 541
		public static bool HasXMLRead;

		// Token: 0x02000175 RID: 373
		public enum CosmeticRarity
		{
			// Token: 0x040004EF RID: 1263
			Default,
			// Token: 0x040004F0 RID: 1264
			Common,
			// Token: 0x040004F1 RID: 1265
			Rare,
			// Token: 0x040004F2 RID: 1266
			Unique
		}

		// Token: 0x02000176 RID: 374
		public enum CosmeticType
		{
			// Token: 0x040004F4 RID: 1268
			Clothing,
			// Token: 0x040004F5 RID: 1269
			Frame,
			// Token: 0x040004F6 RID: 1270
			Sigil
		}

		// Token: 0x02000177 RID: 375
		public class CosmeticElement
		{
			// Token: 0x1700031F RID: 799
			// (get) Token: 0x06000900 RID: 2304 RVA: 0x0000FF26 File Offset: 0x0000E126
			public bool IsFree
			{
				get
				{
					return this.Cost <= 0;
				}
			}

			// Token: 0x06000901 RID: 2305 RVA: 0x0000FF34 File Offset: 0x0000E134
			public CosmeticElement(string id, CosmeticsManager.CosmeticRarity rarity, int cost, CosmeticsManager.CosmeticType type)
			{
				this.Id = id;
				this.Rarity = rarity;
				this.Cost = cost;
				this.Type = type;
			}

			// Token: 0x040004F7 RID: 1271
			public string Id;

			// Token: 0x040004F8 RID: 1272
			public CosmeticsManager.CosmeticRarity Rarity;

			// Token: 0x040004F9 RID: 1273
			public int Cost;

			// Token: 0x040004FA RID: 1274
			public CosmeticsManager.CosmeticType Type;
		}

		// Token: 0x02000178 RID: 376
		public class ClothingCosmeticElement : CosmeticsManager.CosmeticElement
		{
			// Token: 0x06000902 RID: 2306 RVA: 0x0000FF59 File Offset: 0x0000E159
			public ClothingCosmeticElement(string id, CosmeticsManager.CosmeticRarity rarity, int cost, List<string> replaceItemsId, List<Tuple<string, string>> replaceItemless)
				: base(id, rarity, cost, CosmeticsManager.CosmeticType.Clothing)
			{
				this.ReplaceItemsId = replaceItemsId;
				this.ReplaceItemless = replaceItemless;
			}

			// Token: 0x040004FB RID: 1275
			public readonly List<string> ReplaceItemsId;

			// Token: 0x040004FC RID: 1276
			public readonly List<Tuple<string, string>> ReplaceItemless;
		}

		// Token: 0x02000179 RID: 377
		public class SigilCosmeticElement : CosmeticsManager.CosmeticElement
		{
			// Token: 0x06000903 RID: 2307 RVA: 0x0000FF75 File Offset: 0x0000E175
			public SigilCosmeticElement(string id, CosmeticsManager.CosmeticRarity rarity, int cost, string bannerCode)
				: base(id, rarity, cost, CosmeticsManager.CosmeticType.Sigil)
			{
				this.BannerCode = bannerCode;
			}

			// Token: 0x040004FD RID: 1277
			public string BannerCode;
		}
	}
}
