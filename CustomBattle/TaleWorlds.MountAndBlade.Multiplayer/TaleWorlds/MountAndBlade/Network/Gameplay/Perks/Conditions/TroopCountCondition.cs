using System;
using System.Xml;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Conditions
{
	public class TroopCountCondition : MPPerkCondition
	{
		public override MPPerkCondition.PerkEventFlags EventFlags
		{
			get
			{
				return 272;
			}
		}

		public override bool IsPeerCondition
		{
			get
			{
				return true;
			}
		}

		protected TroopCountCondition()
		{
		}

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
				Debug.FailedAssert("provided 'min' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Multiplayer\\Perks\\Conditions\\TroopCountCondition.cs", "Deserialize", 39);
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
				Debug.FailedAssert("provided 'max' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Multiplayer\\Perks\\Conditions\\TroopCountCondition.cs", "Deserialize", 49);
			}
		}

		public override bool Check(MissionPeer peer)
		{
			if (peer == null || MultiplayerOptionsExtensions.GetIntValue(20, 0) <= 0 || peer.ControlledFormation == null)
			{
				return false;
			}
			int num = (peer.IsControlledAgentActive ? (peer.BotsUnderControlAlive + 1) : peer.BotsUnderControlAlive);
			if (this._isRatio)
			{
				float num2 = (float)num / (float)(peer.BotsUnderControlTotal + 1);
				return num2 >= this._min && num2 <= this._max;
			}
			return (float)num >= this._min && (float)num <= this._max;
		}

		public override bool Check(Agent agent)
		{
			agent = ((agent != null && agent.IsMount) ? agent.RiderAgent : agent);
			return this.Check(((agent != null) ? agent.MissionPeer : null) ?? ((agent != null) ? agent.OwningAgentMissionPeer : null));
		}

		protected static string StringType = "TroopCount";

		private bool _isRatio;

		private float _min;

		private float _max;
	}
}
