using System;
using System.Xml;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace SandBox.Objects
{
	// Token: 0x02000025 RID: 37
	public class SettlementMusicData : MBObjectBase
	{
		// Token: 0x1700002F RID: 47
		// (get) Token: 0x060001B3 RID: 435 RVA: 0x0000C222 File Offset: 0x0000A422
		// (set) Token: 0x060001B4 RID: 436 RVA: 0x0000C22A File Offset: 0x0000A42A
		public string MusicPath { get; private set; }

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x060001B5 RID: 437 RVA: 0x0000C233 File Offset: 0x0000A433
		// (set) Token: 0x060001B6 RID: 438 RVA: 0x0000C23B File Offset: 0x0000A43B
		public CultureObject Culture { get; private set; }

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x060001B7 RID: 439 RVA: 0x0000C244 File Offset: 0x0000A444
		public MBReadOnlyList<InstrumentData> Instruments
		{
			get
			{
				return this._instruments;
			}
		}

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x060001B8 RID: 440 RVA: 0x0000C24C File Offset: 0x0000A44C
		// (set) Token: 0x060001B9 RID: 441 RVA: 0x0000C254 File Offset: 0x0000A454
		public string LocationId { get; private set; }

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x060001BA RID: 442 RVA: 0x0000C25D File Offset: 0x0000A45D
		// (set) Token: 0x060001BB RID: 443 RVA: 0x0000C265 File Offset: 0x0000A465
		public int Tempo { get; private set; }

		// Token: 0x060001BC RID: 444 RVA: 0x0000C270 File Offset: 0x0000A470
		public override void Deserialize(MBObjectManager objectManager, XmlNode node)
		{
			base.Deserialize(objectManager, node);
			this.MusicPath = Convert.ToString(node.Attributes["event_id"].Value);
			this.Culture = Game.Current.ObjectManager.ReadObjectReferenceFromXml<CultureObject>("culture", node);
			this.LocationId = Convert.ToString(node.Attributes["location"].Value);
			this.Tempo = Convert.ToInt32(node.Attributes["tempo"].Value);
			this._instruments = new MBList<InstrumentData>();
			if (node.HasChildNodes)
			{
				foreach (object obj in node.ChildNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					if (xmlNode.Name == "Instruments")
					{
						foreach (object obj2 in xmlNode.ChildNodes)
						{
							XmlNode xmlNode2 = (XmlNode)obj2;
							if (xmlNode2.Name == "Instrument")
							{
								XmlAttributeCollection attributes = xmlNode2.Attributes;
								if (((attributes != null) ? attributes["id"] : null) != null)
								{
									string text = Convert.ToString(xmlNode2.Attributes["id"].Value);
									InstrumentData @object = MBObjectManager.Instance.GetObject<InstrumentData>(text);
									if (@object != null)
									{
										this._instruments.Add(@object);
									}
								}
								else
								{
									Debug.FailedAssert("Couldn't find required attributes of instrument xml node in Track", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox\\Objects\\SettlementMusicData.cs", "Deserialize", 57);
								}
							}
						}
					}
				}
			}
			this._instruments.Capacity = this._instruments.Count;
		}

		// Token: 0x040000B5 RID: 181
		private MBList<InstrumentData> _instruments;
	}
}
