using System;
using System.Xml;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace SandBox.Objects
{
	public class SettlementMusicData : MBObjectBase
	{
		public string MusicPath { get; private set; }

		public CultureObject Culture { get; private set; }

		public MBReadOnlyList<InstrumentData> Instruments
		{
			get
			{
				return this._instruments;
			}
		}

		public string LocationId { get; private set; }

		public int Tempo { get; private set; }

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

		private MBList<InstrumentData> _instruments;
	}
}
