using System;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x0200011C RID: 284
	internal class ItemList
	{
		// Token: 0x0600056D RID: 1389 RVA: 0x00008104 File Offset: 0x00006304
		static ItemList()
		{
			XmlDocument xmlDocument = new XmlDocument();
			string text = ModuleHelper.GetModuleFullPath("Native") + "ModuleData/mpitems.xml";
			if (ConfigurationManager.GetAppSettings("MultiplayerItemsFileName") != null)
			{
				text = ConfigurationManager.GetAppSettings("MultiplayerItemsFileName");
			}
			xmlDocument.Load(text);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("Items");
			if (xmlNode == null)
			{
				throw new Exception("'Items' node is not defined in mpitems.xml");
			}
			Debug.Print("---" + xmlNode.Name, 0, Debug.DebugColor.White, 17592186044416UL);
			foreach (object obj in xmlNode.ChildNodes)
			{
				XmlNode xmlNode2 = (XmlNode)obj;
				if (xmlNode2.NodeType == XmlNodeType.Element)
				{
					ItemInnerData itemInnerData = new ItemInnerData();
					itemInnerData.Deserialize(xmlNode2);
					Debug.Print(itemInnerData.TypeId, 0, Debug.DebugColor.White, 17592186044416UL);
					if (!ItemList._items.ContainsKey(itemInnerData.TypeId))
					{
						ItemList._items.Add(itemInnerData.TypeId, itemInnerData);
					}
					else
					{
						Debug.Print("--- Item type id already exists, check mpitems.xml for item type Id:" + itemInnerData.TypeId, 0, Debug.DebugColor.White, 17592186044416UL);
					}
				}
			}
		}

		// Token: 0x0600056E RID: 1390 RVA: 0x00008258 File Offset: 0x00006458
		internal static ItemType GetItemTypeOf(string typeId)
		{
			return ItemList._items[typeId].Type;
		}

		// Token: 0x0600056F RID: 1391 RVA: 0x0000826A File Offset: 0x0000646A
		internal static bool IsItemValid(string itemId, string modifierId)
		{
			return ItemList._items.ContainsKey(itemId);
		}

		// Token: 0x06000570 RID: 1392 RVA: 0x00008277 File Offset: 0x00006477
		internal static int GetPriceOf(string itemId, string modifierId)
		{
			return ItemList._items[itemId].Price;
		}

		// Token: 0x040002B6 RID: 694
		private static Dictionary<string, ItemInnerData> _items = new Dictionary<string, ItemInnerData>();
	}
}
