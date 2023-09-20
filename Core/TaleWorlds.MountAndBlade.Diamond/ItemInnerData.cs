using System;
using System.Xml;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x0200011D RID: 285
	internal class ItemInnerData
	{
		// Token: 0x1700020C RID: 524
		// (get) Token: 0x06000572 RID: 1394 RVA: 0x00008291 File Offset: 0x00006491
		// (set) Token: 0x06000573 RID: 1395 RVA: 0x00008299 File Offset: 0x00006499
		internal string TypeId { get; private set; }

		// Token: 0x1700020D RID: 525
		// (get) Token: 0x06000574 RID: 1396 RVA: 0x000082A2 File Offset: 0x000064A2
		// (set) Token: 0x06000575 RID: 1397 RVA: 0x000082AA File Offset: 0x000064AA
		internal ItemType Type { get; private set; }

		// Token: 0x1700020E RID: 526
		// (get) Token: 0x06000576 RID: 1398 RVA: 0x000082B3 File Offset: 0x000064B3
		// (set) Token: 0x06000577 RID: 1399 RVA: 0x000082BB File Offset: 0x000064BB
		internal int Price { get; private set; }

		// Token: 0x06000578 RID: 1400 RVA: 0x000082C4 File Offset: 0x000064C4
		internal void Deserialize(XmlNode node)
		{
			this.TypeId = node.Attributes["id"].Value;
			this.Price = ((node.Attributes["value"] != null) ? int.Parse(node.Attributes["value"].Value) : 0);
			foreach (object obj in node.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (xmlNode.Name == "flags")
				{
					foreach (object obj2 in xmlNode.ChildNodes)
					{
						XmlNode xmlNode2 = (XmlNode)obj2;
						if (xmlNode2.Name == "flag" && xmlNode2.Attributes["name"].Value == "type")
						{
							string value = xmlNode2.Attributes["value"].Value;
							this.Type = (ItemType)Enum.Parse(typeof(ItemType), value, true);
						}
					}
				}
			}
		}
	}
}
