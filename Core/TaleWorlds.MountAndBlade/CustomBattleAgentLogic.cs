using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000267 RID: 615
	public class CustomBattleAgentLogic : MissionLogic
	{
		// Token: 0x060020E1 RID: 8417 RVA: 0x00075EA4 File Offset: 0x000740A4
		public override void OnAgentHit(Agent affectedAgent, Agent affectorAgent, in MissionWeapon affectorWeapon, in Blow blow, in AttackCollisionData attackCollisionData)
		{
			if (affectedAgent.Character != null && ((affectorAgent != null) ? affectorAgent.Character : null) != null && affectedAgent.State == AgentState.Active)
			{
				bool flag = affectedAgent.Health - (float)blow.InflictedDamage < 1f;
				bool flag2 = affectedAgent.Team.Side == affectorAgent.Team.Side;
				IAgentOriginBase origin = affectorAgent.Origin;
				BasicCharacterObject character = affectedAgent.Character;
				BasicCharacterObject basicCharacterObject;
				if (affectorAgent == null)
				{
					basicCharacterObject = null;
				}
				else
				{
					Formation formation = affectorAgent.Formation;
					if (formation == null)
					{
						basicCharacterObject = null;
					}
					else
					{
						Agent captain = formation.Captain;
						basicCharacterObject = ((captain != null) ? captain.Character : null);
					}
				}
				int inflictedDamage = blow.InflictedDamage;
				bool flag3 = flag;
				bool flag4 = flag2;
				MissionWeapon missionWeapon = affectorWeapon;
				origin.OnScoreHit(character, basicCharacterObject, inflictedDamage, flag3, flag4, missionWeapon.CurrentUsageItem);
			}
		}

		// Token: 0x060020E2 RID: 8418 RVA: 0x00075F54 File Offset: 0x00074154
		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
		{
			if (base.Mission.Mode != MissionMode.Deployment)
			{
				if (affectorAgent == null && affectedAgent.IsMount && agentState == AgentState.Routed)
				{
					return;
				}
				if (affectedAgent.Origin != null)
				{
					if (agentState == AgentState.Unconscious)
					{
						affectedAgent.Origin.SetWounded();
						if (affectedAgent == base.Mission.MainAgent)
						{
							this.BecomeGhost();
							return;
						}
					}
					else
					{
						if (agentState == AgentState.Killed)
						{
							affectedAgent.Origin.SetKilled();
							return;
						}
						affectedAgent.Origin.SetRouted();
					}
				}
			}
		}

		// Token: 0x060020E3 RID: 8419 RVA: 0x00075FC8 File Offset: 0x000741C8
		private void BecomeGhost()
		{
			Agent leader = base.Mission.PlayerEnemyTeam.Leader;
			if (leader != null)
			{
				leader.Controller = Agent.ControllerType.AI;
			}
			base.Mission.MainAgent.Controller = Agent.ControllerType.AI;
		}
	}
}
