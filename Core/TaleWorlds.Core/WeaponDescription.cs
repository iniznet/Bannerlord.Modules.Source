using System;
using System.Collections;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.Core
{
	// Token: 0x02000046 RID: 70
	public class WeaponDescription : MBObjectBase
	{
		// Token: 0x170001BF RID: 447
		// (get) Token: 0x06000563 RID: 1379 RVA: 0x00013CE6 File Offset: 0x00011EE6
		// (set) Token: 0x06000564 RID: 1380 RVA: 0x00013CEE File Offset: 0x00011EEE
		public WeaponClass WeaponClass { get; private set; }

		// Token: 0x170001C0 RID: 448
		// (get) Token: 0x06000565 RID: 1381 RVA: 0x00013CF7 File Offset: 0x00011EF7
		// (set) Token: 0x06000566 RID: 1382 RVA: 0x00013CFF File Offset: 0x00011EFF
		public WeaponFlags WeaponFlags { get; private set; }

		// Token: 0x170001C1 RID: 449
		// (get) Token: 0x06000567 RID: 1383 RVA: 0x00013D08 File Offset: 0x00011F08
		// (set) Token: 0x06000568 RID: 1384 RVA: 0x00013D10 File Offset: 0x00011F10
		public string ItemUsageFeatures { get; private set; }

		// Token: 0x170001C2 RID: 450
		// (get) Token: 0x06000569 RID: 1385 RVA: 0x00013D19 File Offset: 0x00011F19
		// (set) Token: 0x0600056A RID: 1386 RVA: 0x00013D21 File Offset: 0x00011F21
		public bool RotatedInHand { get; private set; }

		// Token: 0x170001C3 RID: 451
		// (get) Token: 0x0600056B RID: 1387 RVA: 0x00013D2A File Offset: 0x00011F2A
		// (set) Token: 0x0600056C RID: 1388 RVA: 0x00013D32 File Offset: 0x00011F32
		public bool IsHiddenFromUI { get; set; }

		// Token: 0x170001C4 RID: 452
		// (get) Token: 0x0600056D RID: 1389 RVA: 0x00013D3B File Offset: 0x00011F3B
		public MBReadOnlyList<CraftingPiece> AvailablePieces
		{
			get
			{
				return this._availablePieces;
			}
		}

		// Token: 0x0600056E RID: 1390 RVA: 0x00013D44 File Offset: 0x00011F44
		public override void Deserialize(MBObjectManager objectManager, XmlNode node)
		{
			base.Deserialize(objectManager, node);
			this.WeaponClass = ((node.Attributes["weapon_class"] != null) ? ((WeaponClass)Enum.Parse(typeof(WeaponClass), node.Attributes["weapon_class"].Value)) : WeaponClass.Undefined);
			this.ItemUsageFeatures = ((node.Attributes["item_usage_features"] != null) ? node.Attributes["item_usage_features"].Value : "");
			this.RotatedInHand = XmlHelper.ReadBool(node, "rotated_in_hand");
			this.UseCenterOfMassAsHandBase = XmlHelper.ReadBool(node, "use_center_of_mass_as_hand_base");
			foreach (object obj in node.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (xmlNode.Name == "WeaponFlags")
				{
					using (IEnumerator enumerator2 = xmlNode.ChildNodes.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							object obj2 = enumerator2.Current;
							XmlNode xmlNode2 = (XmlNode)obj2;
							this.WeaponFlags |= (WeaponFlags)Enum.Parse(typeof(WeaponFlags), xmlNode2.Attributes["value"].Value);
						}
						continue;
					}
				}
				if (xmlNode.Name == "AvailablePieces")
				{
					this._availablePieces = new MBList<CraftingPiece>();
					foreach (object obj3 in xmlNode.ChildNodes)
					{
						XmlNode xmlNode3 = (XmlNode)obj3;
						if (xmlNode3.NodeType == XmlNodeType.Element)
						{
							string value = xmlNode3.Attributes["id"].Value;
							CraftingPiece @object = MBObjectManager.Instance.GetObject<CraftingPiece>(value);
							if (@object != null)
							{
								this._availablePieces.Add(@object);
							}
						}
					}
				}
			}
		}

		// Token: 0x0400028F RID: 655
		public bool UseCenterOfMassAsHandBase;

		// Token: 0x04000291 RID: 657
		private MBList<CraftingPiece> _availablePieces;
	}
}
