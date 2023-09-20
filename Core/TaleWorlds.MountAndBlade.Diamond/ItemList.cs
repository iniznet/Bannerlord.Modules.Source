using System;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;

namespace TaleWorlds.MountAndBlade.Diamond
{
	internal class ItemList
	{
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

		internal static ItemType GetItemTypeOf(string typeId)
		{
			return ItemList._items[typeId].Type;
		}

		internal static bool IsItemValid(string itemId, string modifierId)
		{
			return ItemList._items.ContainsKey(itemId);
		}

		internal static int GetPriceOf(string itemId, string modifierId)
		{
			return ItemList._items[itemId].Price;
		}

		private static Dictionary<string, ItemInnerData> _items = new Dictionary<string, ItemInnerData>();
	}
}
