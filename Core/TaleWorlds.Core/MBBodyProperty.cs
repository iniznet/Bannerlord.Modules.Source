using System;
using System.Xml;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.Core
{
	public class MBBodyProperty : MBObjectBase
	{
		public BodyProperties BodyPropertyMin
		{
			get
			{
				return this._bodyPropertyMin;
			}
		}

		public BodyProperties BodyPropertyMax
		{
			get
			{
				return this._bodyPropertyMax;
			}
		}

		public MBBodyProperty(string stringId)
			: base(stringId)
		{
		}

		public MBBodyProperty()
		{
		}

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

		private BodyProperties _bodyPropertyMin;

		private BodyProperties _bodyPropertyMax;
	}
}
