using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.Core
{
	// Token: 0x02000097 RID: 151
	public class MBEquipmentRoster : MBObjectBase
	{
		// Token: 0x170002A2 RID: 674
		// (get) Token: 0x060007E8 RID: 2024 RVA: 0x0001B32E File Offset: 0x0001952E
		// (set) Token: 0x060007E9 RID: 2025 RVA: 0x0001B336 File Offset: 0x00019536
		public EquipmentFlags EquipmentFlags { get; private set; }

		// Token: 0x060007EA RID: 2026 RVA: 0x0001B33F File Offset: 0x0001953F
		public bool HasEquipmentFlags(EquipmentFlags flags)
		{
			return (this.EquipmentFlags & flags) == flags;
		}

		// Token: 0x060007EB RID: 2027 RVA: 0x0001B34C File Offset: 0x0001954C
		public bool IsEquipmentTemplate()
		{
			return this.EquipmentFlags > EquipmentFlags.None;
		}

		// Token: 0x170002A3 RID: 675
		// (get) Token: 0x060007EC RID: 2028 RVA: 0x0001B357 File Offset: 0x00019557
		public MBReadOnlyList<Equipment> AllEquipments
		{
			get
			{
				if (this._equipments.IsEmpty<Equipment>())
				{
					return new MBList<Equipment>(1) { MBEquipmentRoster.EmptyEquipment };
				}
				return this._equipments;
			}
		}

		// Token: 0x170002A4 RID: 676
		// (get) Token: 0x060007ED RID: 2029 RVA: 0x0001B37E File Offset: 0x0001957E
		public Equipment DefaultEquipment
		{
			get
			{
				if (this._equipments.IsEmpty<Equipment>())
				{
					return MBEquipmentRoster.EmptyEquipment;
				}
				return this._equipments.FirstOrDefault<Equipment>();
			}
		}

		// Token: 0x060007EE RID: 2030 RVA: 0x0001B39E File Offset: 0x0001959E
		public MBEquipmentRoster()
		{
			this._equipments = new MBList<Equipment>();
			this.EquipmentFlags = EquipmentFlags.None;
		}

		// Token: 0x060007EF RID: 2031 RVA: 0x0001B3B8 File Offset: 0x000195B8
		public void Init(MBObjectManager objectManager, XmlNode node)
		{
			if (node.Name == "EquipmentRoster")
			{
				this.InitEquipment(objectManager, node);
				return;
			}
			Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.Core\\MBEquipmentRoster.cs", "Init", 96);
		}

		// Token: 0x060007F0 RID: 2032 RVA: 0x0001B3EC File Offset: 0x000195EC
		public override void Deserialize(MBObjectManager objectManager, XmlNode node)
		{
			base.Deserialize(objectManager, node);
			if (node.Attributes["culture"] != null)
			{
				this.EquipmentCulture = MBObjectManager.Instance.ReadObjectReferenceFromXml<BasicCultureObject>("culture", node);
			}
			foreach (object obj in node.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (xmlNode.Name == "EquipmentSet")
				{
					this.InitEquipment(objectManager, xmlNode);
				}
				if (xmlNode.Name == "Flags")
				{
					foreach (object obj2 in xmlNode.Attributes)
					{
						XmlAttribute xmlAttribute = (XmlAttribute)obj2;
						EquipmentFlags equipmentFlags = (EquipmentFlags)Enum.Parse(typeof(EquipmentFlags), xmlAttribute.Name);
						if (bool.Parse(xmlAttribute.InnerText))
						{
							this.EquipmentFlags |= equipmentFlags;
						}
					}
				}
			}
		}

		// Token: 0x060007F1 RID: 2033 RVA: 0x0001B520 File Offset: 0x00019720
		private void InitEquipment(MBObjectManager objectManager, XmlNode node)
		{
			base.Initialize();
			Equipment equipment = new Equipment(node.Attributes["civilian"] != null && bool.Parse(node.Attributes["civilian"].Value));
			equipment.Deserialize(objectManager, node);
			this._equipments.Add(equipment);
			base.AfterInitialized();
		}

		// Token: 0x060007F2 RID: 2034 RVA: 0x0001B584 File Offset: 0x00019784
		public void AddEquipmentRoster(MBEquipmentRoster equipmentRoster, bool isCivilian)
		{
			foreach (Equipment equipment in equipmentRoster._equipments.ToList<Equipment>())
			{
				if (equipment.IsCivilian == isCivilian)
				{
					this._equipments.Add(equipment);
				}
			}
			this.EquipmentFlags = equipmentRoster.EquipmentFlags;
		}

		// Token: 0x060007F3 RID: 2035 RVA: 0x0001B5F8 File Offset: 0x000197F8
		public void AddOverridenEquipments(MBObjectManager objectManager, List<XmlNode> overridenEquipmentSlots)
		{
			List<Equipment> list = this._equipments.ToList<Equipment>();
			this._equipments.Clear();
			foreach (Equipment equipment in list)
			{
				this._equipments.Add(equipment.Clone(false));
			}
			foreach (XmlNode xmlNode in overridenEquipmentSlots)
			{
				foreach (Equipment equipment2 in this._equipments)
				{
					equipment2.DeserializeNode(objectManager, xmlNode);
				}
			}
		}

		// Token: 0x060007F4 RID: 2036 RVA: 0x0001B6E0 File Offset: 0x000198E0
		public void OrderEquipments()
		{
			this._equipments = new MBList<Equipment>(this._equipments.OrderByDescending((Equipment eq) => !eq.IsCivilian));
		}

		// Token: 0x060007F5 RID: 2037 RVA: 0x0001B717 File Offset: 0x00019917
		public void InitializeDefaultEquipment(string equipmentName)
		{
			if (this._equipments[0] == null)
			{
				this._equipments[0] = new Equipment(false);
			}
			this._equipments[0].FillFrom(Game.Current.GetDefaultEquipmentWithName(equipmentName), true);
		}

		// Token: 0x04000496 RID: 1174
		public static readonly Equipment EmptyEquipment = new Equipment(true);

		// Token: 0x04000497 RID: 1175
		private MBList<Equipment> _equipments;

		// Token: 0x04000498 RID: 1176
		public BasicCultureObject EquipmentCulture;
	}
}
