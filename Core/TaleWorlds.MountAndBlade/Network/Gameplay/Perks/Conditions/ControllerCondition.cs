using System;
using System.Xml;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Conditions
{
	public class ControllerCondition : MPPerkCondition
	{
		public override MPPerkCondition.PerkEventFlags EventFlags
		{
			get
			{
				return MPPerkCondition.PerkEventFlags.PeerControlledAgentChange;
			}
		}

		protected ControllerCondition()
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
					XmlAttribute xmlAttribute = attributes["is_player_controlled"];
					text = ((xmlAttribute != null) ? xmlAttribute.Value : null);
				}
			}
			string text2 = text;
			this._isPlayerControlled = ((text2 != null) ? text2.ToLower() : null) == "true";
		}

		public override bool Check(MissionPeer peer)
		{
			return this.Check((peer != null) ? peer.ControlledAgent : null);
		}

		public override bool Check(Agent agent)
		{
			agent = ((agent != null && agent.IsMount) ? agent.RiderAgent : agent);
			return agent != null && agent.IsPlayerControlled == this._isPlayerControlled;
		}

		protected static string StringType = "Controller";

		private bool _isPlayerControlled;
	}
}
