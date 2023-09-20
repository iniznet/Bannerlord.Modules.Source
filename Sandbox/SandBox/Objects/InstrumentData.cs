using System;
using System.Xml;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace SandBox.Objects
{
	// Token: 0x02000023 RID: 35
	public class InstrumentData : MBObjectBase
	{
		// Token: 0x17000024 RID: 36
		// (get) Token: 0x06000193 RID: 403 RVA: 0x0000BB80 File Offset: 0x00009D80
		public MBReadOnlyList<ValueTuple<HumanBone, string>> InstrumentEntities
		{
			get
			{
				return this._instrumentEntities;
			}
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x06000194 RID: 404 RVA: 0x0000BB88 File Offset: 0x00009D88
		// (set) Token: 0x06000195 RID: 405 RVA: 0x0000BB90 File Offset: 0x00009D90
		public string SittingAction { get; private set; }

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x06000196 RID: 406 RVA: 0x0000BB99 File Offset: 0x00009D99
		// (set) Token: 0x06000197 RID: 407 RVA: 0x0000BBA1 File Offset: 0x00009DA1
		public string StandingAction { get; private set; }

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x06000198 RID: 408 RVA: 0x0000BBAA File Offset: 0x00009DAA
		// (set) Token: 0x06000199 RID: 409 RVA: 0x0000BBB2 File Offset: 0x00009DB2
		public string Tag { get; private set; }

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x0600019A RID: 410 RVA: 0x0000BBBB File Offset: 0x00009DBB
		// (set) Token: 0x0600019B RID: 411 RVA: 0x0000BBC3 File Offset: 0x00009DC3
		public bool IsDataWithoutInstrument { get; private set; }

		// Token: 0x0600019C RID: 412 RVA: 0x0000BBCC File Offset: 0x00009DCC
		public InstrumentData()
		{
		}

		// Token: 0x0600019D RID: 413 RVA: 0x0000BBD4 File Offset: 0x00009DD4
		public InstrumentData(string stringId)
			: base(stringId)
		{
		}

		// Token: 0x0600019E RID: 414 RVA: 0x0000BBDD File Offset: 0x00009DDD
		public void InitializeInstrumentData(string sittingAction, string standingAction, bool isDataWithoutInstrument)
		{
			this.SittingAction = sittingAction;
			this.StandingAction = standingAction;
			this._instrumentEntities = new MBList<ValueTuple<HumanBone, string>>(0);
			this.IsDataWithoutInstrument = isDataWithoutInstrument;
			this.Tag = string.Empty;
		}

		// Token: 0x0600019F RID: 415 RVA: 0x0000BC0C File Offset: 0x00009E0C
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

		// Token: 0x040000A9 RID: 169
		private MBList<ValueTuple<HumanBone, string>> _instrumentEntities;
	}
}
