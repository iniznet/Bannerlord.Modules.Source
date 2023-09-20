using System;
using System.Xml;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Conditions
{
	// Token: 0x020003D5 RID: 981
	public class AgentStatusCondition : MPPerkCondition
	{
		// Token: 0x1700091D RID: 2333
		// (get) Token: 0x0600340C RID: 13324 RVA: 0x000D7D77 File Offset: 0x000D5F77
		public override MPPerkCondition.PerkEventFlags EventFlags
		{
			get
			{
				return MPPerkCondition.PerkEventFlags.MountChange;
			}
		}

		// Token: 0x0600340D RID: 13325 RVA: 0x000D7D7E File Offset: 0x000D5F7E
		protected AgentStatusCondition()
		{
		}

		// Token: 0x0600340E RID: 13326 RVA: 0x000D7D88 File Offset: 0x000D5F88
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
				Debug.FailedAssert("provided 'agent_status' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\Conditions\\AgentStatusCondition.cs", "Deserialize", 31);
			}
		}

		// Token: 0x0600340F RID: 13327 RVA: 0x000D7DE2 File Offset: 0x000D5FE2
		public override bool Check(MissionPeer peer)
		{
			return this.Check((peer != null) ? peer.ControlledAgent : null);
		}

		// Token: 0x06003410 RID: 13328 RVA: 0x000D7DF6 File Offset: 0x000D5FF6
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

		// Token: 0x0400162C RID: 5676
		protected static string StringType = "AgentStatus";

		// Token: 0x0400162D RID: 5677
		private AgentStatusCondition.AgentStatus _status;

		// Token: 0x020006C7 RID: 1735
		private enum AgentStatus
		{
			// Token: 0x040022AE RID: 8878
			OnFoot,
			// Token: 0x040022AF RID: 8879
			OnMount
		}
	}
}
