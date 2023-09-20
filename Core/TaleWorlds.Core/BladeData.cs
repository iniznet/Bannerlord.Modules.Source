using System;
using System.Xml;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.Core
{
	// Token: 0x02000044 RID: 68
	public sealed class BladeData : MBObjectBase
	{
		// Token: 0x170001B1 RID: 433
		// (get) Token: 0x06000544 RID: 1348 RVA: 0x0001390A File Offset: 0x00011B0A
		// (set) Token: 0x06000545 RID: 1349 RVA: 0x00013912 File Offset: 0x00011B12
		public DamageTypes ThrustDamageType { get; private set; }

		// Token: 0x170001B2 RID: 434
		// (get) Token: 0x06000546 RID: 1350 RVA: 0x0001391B File Offset: 0x00011B1B
		// (set) Token: 0x06000547 RID: 1351 RVA: 0x00013923 File Offset: 0x00011B23
		public float ThrustDamageFactor { get; private set; }

		// Token: 0x170001B3 RID: 435
		// (get) Token: 0x06000548 RID: 1352 RVA: 0x0001392C File Offset: 0x00011B2C
		// (set) Token: 0x06000549 RID: 1353 RVA: 0x00013934 File Offset: 0x00011B34
		public DamageTypes SwingDamageType { get; private set; }

		// Token: 0x170001B4 RID: 436
		// (get) Token: 0x0600054A RID: 1354 RVA: 0x0001393D File Offset: 0x00011B3D
		// (set) Token: 0x0600054B RID: 1355 RVA: 0x00013945 File Offset: 0x00011B45
		public float SwingDamageFactor { get; private set; }

		// Token: 0x170001B5 RID: 437
		// (get) Token: 0x0600054C RID: 1356 RVA: 0x0001394E File Offset: 0x00011B4E
		// (set) Token: 0x0600054D RID: 1357 RVA: 0x00013956 File Offset: 0x00011B56
		public float BladeLength { get; private set; }

		// Token: 0x170001B6 RID: 438
		// (get) Token: 0x0600054E RID: 1358 RVA: 0x0001395F File Offset: 0x00011B5F
		// (set) Token: 0x0600054F RID: 1359 RVA: 0x00013967 File Offset: 0x00011B67
		public float BladeWidth { get; private set; }

		// Token: 0x170001B7 RID: 439
		// (get) Token: 0x06000550 RID: 1360 RVA: 0x00013970 File Offset: 0x00011B70
		// (set) Token: 0x06000551 RID: 1361 RVA: 0x00013978 File Offset: 0x00011B78
		public short StackAmount { get; private set; }

		// Token: 0x170001B8 RID: 440
		// (get) Token: 0x06000552 RID: 1362 RVA: 0x00013981 File Offset: 0x00011B81
		// (set) Token: 0x06000553 RID: 1363 RVA: 0x00013989 File Offset: 0x00011B89
		public string PhysicsMaterial { get; private set; }

		// Token: 0x170001B9 RID: 441
		// (get) Token: 0x06000554 RID: 1364 RVA: 0x00013992 File Offset: 0x00011B92
		// (set) Token: 0x06000555 RID: 1365 RVA: 0x0001399A File Offset: 0x00011B9A
		public string BodyName { get; private set; }

		// Token: 0x170001BA RID: 442
		// (get) Token: 0x06000556 RID: 1366 RVA: 0x000139A3 File Offset: 0x00011BA3
		// (set) Token: 0x06000557 RID: 1367 RVA: 0x000139AB File Offset: 0x00011BAB
		public string HolsterMeshName { get; private set; }

		// Token: 0x170001BB RID: 443
		// (get) Token: 0x06000558 RID: 1368 RVA: 0x000139B4 File Offset: 0x00011BB4
		// (set) Token: 0x06000559 RID: 1369 RVA: 0x000139BC File Offset: 0x00011BBC
		public string HolsterBodyName { get; private set; }

		// Token: 0x170001BC RID: 444
		// (get) Token: 0x0600055A RID: 1370 RVA: 0x000139C5 File Offset: 0x00011BC5
		// (set) Token: 0x0600055B RID: 1371 RVA: 0x000139CD File Offset: 0x00011BCD
		public float HolsterMeshLength { get; private set; }

		// Token: 0x0600055C RID: 1372 RVA: 0x000139D6 File Offset: 0x00011BD6
		public BladeData(CraftingPiece.PieceTypes pieceType, float bladeLength)
		{
			this.PieceType = pieceType;
			this.BladeLength = bladeLength;
			this.ThrustDamageType = DamageTypes.Invalid;
			this.SwingDamageType = DamageTypes.Invalid;
		}

		// Token: 0x0600055D RID: 1373 RVA: 0x000139FC File Offset: 0x00011BFC
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

		// Token: 0x0400027C RID: 636
		public readonly CraftingPiece.PieceTypes PieceType;
	}
}
