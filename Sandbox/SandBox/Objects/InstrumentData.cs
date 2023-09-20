using System;
using System.Xml;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace SandBox.Objects
{
	public class InstrumentData : MBObjectBase
	{
		public MBReadOnlyList<ValueTuple<HumanBone, string>> InstrumentEntities
		{
			get
			{
				return this._instrumentEntities;
			}
		}

		public string SittingAction { get; private set; }

		public string StandingAction { get; private set; }

		public string Tag { get; private set; }

		public bool IsDataWithoutInstrument { get; private set; }

		public InstrumentData()
		{
		}

		public InstrumentData(string stringId)
			: base(stringId)
		{
		}

		public void InitializeInstrumentData(string sittingAction, string standingAction, bool isDataWithoutInstrument)
		{
			this.SittingAction = sittingAction;
			this.StandingAction = standingAction;
			this._instrumentEntities = new MBList<ValueTuple<HumanBone, string>>(0);
			this.IsDataWithoutInstrument = isDataWithoutInstrument;
			this.Tag = string.Empty;
		}

		public override void Deserialize(MBObjectManager objectManager, XmlNode node)
		{
			base.Deserialize(objectManager, node);
			this.SittingAction = Convert.ToString(node.Attributes["sittingAction"].Value);
			this.StandingAction = Convert.ToString(node.Attributes["standingAction"].Value);
			XmlAttribute xmlAttribute = node.Attributes["tag"];
			this.Tag = Convert.ToString((xmlAttribute != null) ? xmlAttribute.Value : null);
			this._instrumentEntities = new MBList<ValueTuple<HumanBone, string>>();
			if (node.HasChildNodes)
			{
				foreach (object obj in node.ChildNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					if (xmlNode.Name == "Entities")
					{
						foreach (object obj2 in xmlNode.ChildNodes)
						{
							XmlNode xmlNode2 = (XmlNode)obj2;
							if (xmlNode2.Name == "Entity")
							{
								XmlAttributeCollection attributes = xmlNode2.Attributes;
								if (((attributes != null) ? attributes["name"] : null) != null && xmlNode2.Attributes["bone"] != null)
								{
									string text = Convert.ToString(xmlNode2.Attributes["name"].Value);
									HumanBone humanBone;
									if (Enum.TryParse<HumanBone>(xmlNode2.Attributes["bone"].Value, out humanBone))
									{
										this._instrumentEntities.Add(new ValueTuple<HumanBone, string>(humanBone, text));
									}
									else
									{
										Debug.FailedAssert("Couldn't parse bone xml node for instrument.", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox\\Objects\\InstrumentData.cs", "Deserialize", 62);
									}
								}
								else
								{
									Debug.FailedAssert("Couldn't find required attributes of entity xml node in Instrument", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox\\Objects\\InstrumentData.cs", "Deserialize", 67);
								}
							}
						}
					}
				}
			}
			this._instrumentEntities.Capacity = this._instrumentEntities.Count;
		}

		private MBList<ValueTuple<HumanBone, string>> _instrumentEntities;
	}
}
