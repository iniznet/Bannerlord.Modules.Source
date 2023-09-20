using System;
using System.Xml;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Conditions
{
	// Token: 0x020003DE RID: 990
	public class MountHealthCondition : MPPerkCondition
	{
		// Token: 0x1700092B RID: 2347
		// (get) Token: 0x06003447 RID: 13383 RVA: 0x000D881C File Offset: 0x000D6A1C
		public override MPPerkCondition.PerkEventFlags EventFlags
		{
			get
			{
				return MPPerkCondition.PerkEventFlags.MountHealthChange | MPPerkCondition.PerkEventFlags.MountChange;
			}
		}

		// Token: 0x06003448 RID: 13384 RVA: 0x000D8823 File Offset: 0x000D6A23
		protected MountHealthCondition()
		{
		}

		// Token: 0x06003449 RID: 13385 RVA: 0x000D882C File Offset: 0x000D6A2C
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
				Debug.FailedAssert("provided 'min' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\Conditions\\MountHealthCondition.cs", "Deserialize", 34);
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
				Debug.FailedAssert("provided 'max' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\Conditions\\MountHealthCondition.cs", "Deserialize", 44);
			}
		}

		// Token: 0x0600344A RID: 13386 RVA: 0x000D8949 File Offset: 0x000D6B49
		public override bool Check(MissionPeer peer)
		{
			return this.Check((peer != null) ? peer.ControlledAgent : null);
		}

		// Token: 0x0600344B RID: 13387 RVA: 0x000D8960 File Offset: 0x000D6B60
		public override bool Check(Agent agent)
		{
			agent = ((agent != null && !agent.IsMount) ? agent.MountAgent : agent);
			if (agent != null)
			{
				float num = (this._isRatio ? (agent.Health / agent.HealthLimit) : agent.Health);
				return num >= this._min && num <= this._max;
			}
			return false;
		}

		// Token: 0x0400163F RID: 5695
		protected static string StringType = "MountHealth";

		// Token: 0x04001640 RID: 5696
		private bool _isRatio;

		// Token: 0x04001641 RID: 5697
		private float _min;

		// Token: 0x04001642 RID: 5698
		private float _max;
	}
}
