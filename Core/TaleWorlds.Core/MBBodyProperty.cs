using System;
using System.Xml;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.Core
{
	// Token: 0x02000094 RID: 148
	public class MBBodyProperty : MBObjectBase
	{
		// Token: 0x1700029F RID: 671
		// (get) Token: 0x060007DD RID: 2013 RVA: 0x0001B179 File Offset: 0x00019379
		public BodyProperties BodyPropertyMin
		{
			get
			{
				return this._bodyPropertyMin;
			}
		}

		// Token: 0x170002A0 RID: 672
		// (get) Token: 0x060007DE RID: 2014 RVA: 0x0001B181 File Offset: 0x00019381
		public BodyProperties BodyPropertyMax
		{
			get
			{
				return this._bodyPropertyMax;
			}
		}

		// Token: 0x060007DF RID: 2015 RVA: 0x0001B189 File Offset: 0x00019389
		public MBBodyProperty(string stringId)
			: base(stringId)
		{
		}

		// Token: 0x060007E0 RID: 2016 RVA: 0x0001B192 File Offset: 0x00019392
		public MBBodyProperty()
		{
		}

		// Token: 0x060007E1 RID: 2017 RVA: 0x0001B19C File Offset: 0x0001939C
		public void Init(BodyProperties bodyPropertyMin, BodyProperties bodyPropertyMax)
		{
			base.Initialize();
			this._bodyPropertyMin = bodyPropertyMin;
			this._bodyPropertyMax = bodyPropertyMax;
			if (this._bodyPropertyMax.Age <= 0f)
			{
				this._bodyPropertyMax = this._bodyPropertyMin;
			}
			if (this._bodyPropertyMin.Age <= 0f)
			{
				this._bodyPropertyMin = this._bodyPropertyMax;
			}
			base.AfterInitialized();
		}

		// Token: 0x060007E2 RID: 2018 RVA: 0x0001B200 File Offset: 0x00019400
		public override void Deserialize(MBObjectManager objectManager, XmlNode node)
		{
			base.Deserialize(objectManager, node);
			foreach (object obj in node.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (xmlNode.Name == "BodyPropertiesMin")
				{
					BodyProperties.FromXmlNode(xmlNode, out this._bodyPropertyMin);
				}
				else if (xmlNode.Name == "BodyPropertiesMax")
				{
					BodyProperties.FromXmlNode(xmlNode, out this._bodyPropertyMax);
				}
			}
			if (this._bodyPropertyMax.Age <= 0f)
			{
				this._bodyPropertyMax = this._bodyPropertyMin;
			}
			if (this._bodyPropertyMin.Age <= 0f)
			{
				this._bodyPropertyMin = this._bodyPropertyMax;
			}
		}

		// Token: 0x04000481 RID: 1153
		private BodyProperties _bodyPropertyMin;

		// Token: 0x04000482 RID: 1154
		private BodyProperties _bodyPropertyMax;
	}
}
