using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.Core
{
	public class MBEquipmentRoster : MBObjectBase
	{
		public EquipmentFlags EquipmentFlags { get; private set; }

		public bool HasEquipmentFlags(EquipmentFlags flags)
		{
			return (this.EquipmentFlags & flags) == flags;
		}

		public bool IsEquipmentTemplate()
		{
			return this.EquipmentFlags > EquipmentFlags.None;
		}

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

		public MBEquipmentRoster()
		{
			this._equipments = new MBList<Equipment>();
			this.EquipmentFlags = EquipmentFlags.None;
		}

		public void Init(MBObjectManager objectManager, XmlNode node)
		{
			if (node.Name == "EquipmentRoster")
			{
				this.InitEquipment(objectManager, node);
				return;
			}
			Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.Core\\MBEquipmentRoster.cs", "Init", 96);
		}

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

		private void InitEquipment(MBObjectManager objectManager, XmlNode node)
		{
			base.Initialize();
			Equipment equipment = new Equipment(node.Attributes["civilian"] != null && bool.Parse(node.Attributes["civilian"].Value));
			equipment.Deserialize(objectManager, node);
			this._equipments.Add(equipment);
			base.AfterInitialized();
		}

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

		public void OrderEquipments()
		{
			this._equipments = new MBList<Equipment>(this._equipments.OrderByDescending((Equipment eq) => !eq.IsCivilian));
		}

		public void InitializeDefaultEquipment(string equipmentName)
		{
			if (this._equipments[0] == null)
			{
				this._equipments[0] = new Equipment(false);
			}
			this._equipments[0].FillFrom(Game.Current.GetDefaultEquipmentWithName(equipmentName), true);
		}

		public static readonly Equipment EmptyEquipment = new Equipment(true);

		private MBList<Equipment> _equipments;

		public BasicCultureObject EquipmentCulture;
	}
}
