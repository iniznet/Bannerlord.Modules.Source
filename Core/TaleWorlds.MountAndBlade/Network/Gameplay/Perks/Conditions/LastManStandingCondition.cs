using System;
using System.Xml;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Conditions
{
	public class LastManStandingCondition : MPPerkCondition
	{
		public override MPPerkCondition.PerkEventFlags EventFlags
		{
			get
			{
				return MPPerkCondition.PerkEventFlags.AliveBotCountChange | MPPerkCondition.PerkEventFlags.SpawnEnd;
			}
		}

		protected LastManStandingCondition()
		{
		}

		protected override void Deserialize(XmlNode node)
		{
		}

		public override bool Check(MissionPeer peer)
		{
			return this.Check((peer != null) ? peer.ControlledAgent : null);
		}

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

		protected static string StringType = "LastManStanding";
	}
}
