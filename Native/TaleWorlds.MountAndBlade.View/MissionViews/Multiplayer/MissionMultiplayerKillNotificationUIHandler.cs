using System;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.View.MissionViews.Multiplayer
{
	public class MissionMultiplayerKillNotificationUIHandler : MissionView
	{
		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
		{
			base.OnAgentRemoved(affectedAgent, affectorAgent, agentState, killingBlow);
			if (GameNetwork.IsDedicatedServer || !affectedAgent.IsHuman)
			{
				return;
			}
			string text = ((affectorAgent == null) ? string.Empty : ((affectorAgent.MissionPeer != null) ? affectorAgent.MissionPeer.DisplayedName : affectorAgent.Name));
			string text2 = ((affectedAgent.MissionPeer != null) ? affectedAgent.MissionPeer.DisplayedName : affectedAgent.Name);
			uint num = 4291306250U;
			MissionPeer missionPeer = null;
			if (GameNetwork.MyPeer != null)
			{
				missionPeer = PeerExtensions.GetComponent<MissionPeer>(GameNetwork.MyPeer);
			}
			if (missionPeer != null && ((missionPeer.Team != base.Mission.SpectatorTeam && missionPeer.Team != affectedAgent.Team) || (affectorAgent != null && affectorAgent.MissionPeer == missionPeer)))
			{
				num = 4281589009U;
			}
			TextObject textObject;
			if (affectorAgent != null)
			{
				textObject = new TextObject("{=2ZarUUbw}{KILLERPLAYERNAME} has killed {KILLEDPLAYERNAME}!", null);
				textObject.SetTextVariable("KILLERPLAYERNAME", text);
			}
			else
			{
				textObject = new TextObject("{=9CnRKZOb}{KILLEDPLAYERNAME} has died!", null);
			}
			textObject.SetTextVariable("KILLEDPLAYERNAME", text2);
			MessageManager.DisplayMessage(textObject.ToString(), num);
		}
	}
}
