using System;
using System.Xml;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Conditions
{
	// Token: 0x020003DA RID: 986
	public class HealthCondition : MPPerkCondition
	{
		// Token: 0x17000925 RID: 2341
		// (get) Token: 0x0600342D RID: 13357 RVA: 0x000D8300 File Offset: 0x000D6500
		public override MPPerkCondition.PerkEventFlags EventFlags
		{
			get
			{
				return MPPerkCondition.PerkEventFlags.HealthChange;
			}
		}

		// Token: 0x0600342E RID: 13358 RVA: 0x000D8303 File Offset: 0x000D6503
		protected HealthCondition()
		{
		}

		// Token: 0x0600342F RID: 13359 RVA: 0x000D830C File Offset: 0x000D650C
		protected override void Deserialize(XmlNode node)
		{
			string text;
			if (node == null)
			{
				text = null;
			}
			else
			{
				XmlAttributeCollection attributes = node.Attributes;
				if (attributes == null)
				{
					text = null;
				}
				else
				{
					XmlAttribute xmlAttribute = attributes["is_ratio"];
					text = ((xmlAttribute != null) ? xmlAttribute.Value : null);
				}
			}
			string text2 = text;
			this._isRatio = ((text2 != null) ? text2.ToLower() : null) == "true";
			string text3;
			if (node == null)
			{
				text3 = null;
			}
			else
			{
				XmlAttributeCollection attributes2 = node.Attributes;
				if (attributes2 == null)
				{
					text3 = null;
				}
				else
				{
					XmlAttribute xmlAttribute2 = attributes2["min"];
					text3 = ((xmlAttribute2 != null) ? xmlAttribute2.Value : null);
				}
			}
			string text4 = text3;
			if (text4 == null)
			{
				this._min = 0f;
			}
			else if (!float.TryParse(text4, out this._min))
			{
				Debug.FailedAssert("provided 'min' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\Conditions\\HealthCondition.cs", "Deserialize", 34);
			}
			string text5;
			if (node == null)
			{
				text5 = null;
			}
			else
			{
				XmlAttributeCollection attributes3 = node.Attributes;
				if (attributes3 == null)
				{
					text5 = null;
				}
				else
				{
					XmlAttribute xmlAttribute3 = attributes3["max"];
					text5 = ((xmlAttribute3 != null) ? xmlAttribute3.Value : null);
				}
			}
			string text6 = text5;
			if (text6 == null)
			{
				this._max = (this._isRatio ? 1f : float.MaxValue);
				return;
			}
			if (!float.TryParse(text6, out this._max))
			{
				Debug.FailedAssert("provided 'max' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\Conditions\\HealthCondition.cs", "Deserialize", 44);
			}
		}

		// Token: 0x06003430 RID: 13360 RVA: 0x000D8429 File Offset: 0x000D6629
		public override bool Check(MissionPeer peer)
		{
			return this.Check((peer != null) ? peer.ControlledAgent : null);
		}

		// Token: 0x06003431 RID: 13361 RVA: 0x000D8440 File Offset: 0x000D6640
		public override bool Check(Agent agent)
		{
			agent = ((agent != null && agent.IsMount) ? agent.RiderAgent : agent);
			if (agent != null)
			{
				float num = (this._isRatio ? (agent.Health / agent.HealthLimit) : agent.Health);
				return num >= this._min && num <= this._max;
			}
			return false;
		}

		// Token: 0x04001635 RID: 5685
		protected static string StringType = "Health";

		// Token: 0x04001636 RID: 5686
		private bool _isRatio;

		// Token: 0x04001637 RID: 5687
		private float _min;

		// Token: 0x04001638 RID: 5688
		private float _max;
	}
}
