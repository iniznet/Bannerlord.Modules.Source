using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics
{
	// Token: 0x02000051 RID: 81
	public class SandBoxMissionHandler : MissionLogic
	{
		// Token: 0x060003B3 RID: 947 RVA: 0x0001B475 File Offset: 0x00019675
		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
		{
			if (affectedAgent != null && affectedAgent.Character != CharacterObject.PlayerCharacter)
			{
				return;
			}
			if (affectedAgent == affectorAgent || affectorAgent == null)
			{
				Campaign.Current.GameMenuManager.SetNextMenu("settlement_player_unconscious");
			}
		}
	}
}
