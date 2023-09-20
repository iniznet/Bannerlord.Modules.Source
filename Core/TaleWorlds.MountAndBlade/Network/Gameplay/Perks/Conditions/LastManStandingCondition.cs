using System;
using System.Xml;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Conditions
{
	// Token: 0x020003DB RID: 987
	public class LastManStandingCondition : MPPerkCondition
	{
		// Token: 0x17000926 RID: 2342
		// (get) Token: 0x06003433 RID: 13363 RVA: 0x000D84A9 File Offset: 0x000D66A9
		public override MPPerkCondition.PerkEventFlags EventFlags
		{
			get
			{
				return MPPerkCondition.PerkEventFlags.AliveBotCountChange | MPPerkCondition.PerkEventFlags.SpawnEnd;
			}
		}

		// Token: 0x06003434 RID: 13364 RVA: 0x000D84B0 File Offset: 0x000D66B0
		protected LastManStandingCondition()
		{
		}

		// Token: 0x06003435 RID: 13365 RVA: 0x000D84B8 File Offset: 0x000D66B8
		protected override void Deserialize(XmlNode node)
		{
		}

		// Token: 0x06003436 RID: 13366 RVA: 0x000D84BA File Offset: 0x000D66BA
		public override bool Check(MissionPeer peer)
		{
			return this.Check((peer != null) ? peer.ControlledAgent : null);
		}

		// Token: 0x06003437 RID: 13367 RVA: 0x000D84D0 File Offset: 0x000D66D0
		public override bool Check(Agent agent)
		{
			agent = ((agent != null && agent.IsMount) ? agent.RiderAgent : agent);
			MissionPeer missionPeer = ((agent != null) ? agent.MissionPeer : null) ?? ((agent != null) ? agent.OwningAgentMissionPeer : null);
			if (MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) <= 0 || ((missionPeer != null) ? missionPeer.ControlledFormation : null) == null || !agent.IsActive())
			{
				return false;
			}
			if (!agent.IsPlayerControlled)
			{
				return missionPeer.BotsUnderControlAlive == 1;
			}
			return missionPeer.BotsUnderControlAlive == 0;
		}

		// Token: 0x04001639 RID: 5689
		protected static string StringType = "LastManStanding";
	}
}
