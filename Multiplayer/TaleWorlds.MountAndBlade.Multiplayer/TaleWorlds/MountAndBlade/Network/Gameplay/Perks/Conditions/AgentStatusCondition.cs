using System;
using System.Xml;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Conditions
{
	public class AgentStatusCondition : MPPerkCondition
	{
		public override MPPerkCondition.PerkEventFlags EventFlags
		{
			get
			{
				return 1024;
			}
		}

		protected AgentStatusCondition()
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
					XmlAttribute xmlAttribute = attributes["agent_status"];
					text = ((xmlAttribute != null) ? xmlAttribute.Value : null);
				}
			}
			if (!Enum.TryParse<AgentStatusCondition.AgentStatus>(text, true, out this._status))
			{
				Debug.FailedAssert("provided 'agent_status' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Multiplayer\\Perks\\Conditions\\AgentStatusCondition.cs", "Deserialize", 31);
			}
		}

		public override bool Check(MissionPeer peer)
		{
			return this.Check((peer != null) ? peer.ControlledAgent : null);
		}

		public override bool Check(Agent agent)
		{
			if (agent == null)
			{
				return false;
			}
			if (agent.MountAgent == null)
			{
				return this._status == AgentStatusCondition.AgentStatus.OnFoot;
			}
			return this._status == AgentStatusCondition.AgentStatus.OnMount;
		}

		protected static string StringType = "AgentStatus";

		private AgentStatusCondition.AgentStatus _status;

		private enum AgentStatus
		{
			OnFoot,
			OnMount
		}
	}
}
