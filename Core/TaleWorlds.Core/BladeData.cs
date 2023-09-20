using System;
using System.Xml;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.Core
{
	public sealed class BladeData : MBObjectBase
	{
		public DamageTypes ThrustDamageType { get; private set; }

		public float ThrustDamageFactor { get; private set; }

		public DamageTypes SwingDamageType { get; private set; }

		public float SwingDamageFactor { get; private set; }

		public float BladeLength { get; private set; }

		public float BladeWidth { get; private set; }

		public short StackAmount { get; private set; }

		public string PhysicsMaterial { get; private set; }

		public string BodyName { get; private set; }

		public string HolsterMeshName { get; private set; }

		public string HolsterBodyName { get; private set; }

		public float HolsterMeshLength { get; private set; }

		public BladeData(CraftingPiece.PieceTypes pieceType, float bladeLength)
		{
			this.PieceType = pieceType;
			this.BladeLength = bladeLength;
			this.ThrustDamageType = DamageTypes.Invalid;
			this.SwingDamageType = DamageTypes.Invalid;
		}

		public override void Deserialize(MBObjectManager objectManager, XmlNode childNode)
		{
			this.Initialize();
			XmlAttribute xmlAttribute = childNode.Attributes["stack_amount"];
			XmlAttribute xmlAttribute2 = childNode.Attributes["blade_length"];
			XmlAttribute xmlAttribute3 = childNode.Attributes["blade_width"];
			XmlAttribute xmlAttribute4 = childNode.Attributes["physics_material"];
			XmlAttribute xmlAttribute5 = childNode.Attributes["body_name"];
			XmlAttribute xmlAttribute6 = childNode.Attributes["holster_mesh"];
			XmlAttribute xmlAttribute7 = childNode.Attributes["holster_body_name"];
			XmlAttribute xmlAttribute8 = childNode.Attributes["holster_mesh_length"];
			this.StackAmount = ((xmlAttribute != null) ? short.Parse(xmlAttribute.Value) : 1);
			this.BladeLength = ((xmlAttribute2 != null) ? (0.01f * float.Parse(xmlAttribute2.Value)) : this.BladeLength);
			this.BladeWidth = ((xmlAttribute3 != null) ? (0.01f * float.Parse(xmlAttribute3.Value)) : (0.15f + this.BladeLength * 0.3f));
			this.PhysicsMaterial = ((xmlAttribute4 != null) ? xmlAttribute4.InnerText : null);
			this.BodyName = ((xmlAttribute5 != null) ? xmlAttribute5.InnerText : null);
			this.HolsterMeshName = ((xmlAttribute6 != null) ? xmlAttribute6.InnerText : null);
			this.HolsterBodyName = ((xmlAttribute7 != null) ? xmlAttribute7.InnerText : null);
			this.HolsterMeshLength = 0.01f * ((xmlAttribute8 != null) ? float.Parse(xmlAttribute8.Value) : 0f);
			foreach (object obj in childNode.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				string name = xmlNode.Name;
				if (!(name == "Thrust"))
				{
					if (name == "Swing")
					{
						XmlAttribute xmlAttribute9 = xmlNode.Attributes["damage_type"];
						XmlAttribute xmlAttribute10 = xmlNode.Attributes["damage_factor"];
						this.SwingDamageType = (DamageTypes)Enum.Parse(typeof(DamageTypes), xmlAttribute9.Value, true);
						this.SwingDamageFactor = float.Parse(xmlAttribute10.Value);
					}
				}
				else
				{
					XmlAttribute xmlAttribute11 = xmlNode.Attributes["damage_type"];
					XmlAttribute xmlAttribute12 = xmlNode.Attributes["damage_factor"];
					this.ThrustDamageType = (DamageTypes)Enum.Parse(typeof(DamageTypes), xmlAttribute11.Value, true);
					this.ThrustDamageFactor = float.Parse(xmlAttribute12.Value);
				}
			}
		}

		public readonly CraftingPiece.PieceTypes PieceType;
	}
}
