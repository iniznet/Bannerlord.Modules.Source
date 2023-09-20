using System;
using System.Xml;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Conditions
{
	// Token: 0x020003D8 RID: 984
	public class ControllerCondition : MPPerkCondition
	{
		// Token: 0x17000922 RID: 2338
		// (get) Token: 0x06003420 RID: 13344 RVA: 0x000D80F8 File Offset: 0x000D62F8
		public override MPPerkCondition.PerkEventFlags EventFlags
		{
			get
			{
				return MPPerkCondition.PerkEventFlags.PeerControlledAgentChange;
			}
		}

		// Token: 0x06003421 RID: 13345 RVA: 0x000D80FC File Offset: 0x000D62FC
		protected ControllerCondition()
		{
		}

		// Token: 0x06003422 RID: 13346 RVA: 0x000D8104 File Offset: 0x000D6304
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

		// Token: 0x06003423 RID: 13347 RVA: 0x000D8157 File Offset: 0x000D6357
		public override bool Check(MissionPeer peer)
		{
			return this.Check((peer != null) ? peer.ControlledAgent : null);
		}

		// Token: 0x06003424 RID: 13348 RVA: 0x000D816B File Offset: 0x000D636B
		public override bool Check(Agent agent)
		{
			agent = ((agent != null && agent.IsMount) ? agent.RiderAgent : agent);
			return agent != null && agent.IsPlayerControlled == this._isPlayerControlled;
		}

		// Token: 0x04001631 RID: 5681
		protected static string StringType = "Controller";

		// Token: 0x04001632 RID: 5682
		private bool _isPlayerControlled;
	}
}
