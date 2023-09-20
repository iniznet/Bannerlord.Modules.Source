using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	public class BattleObserverMissionLogic : MissionLogic
	{
		public IBattleObserver BattleObserver { get; private set; }

		public void SetObserver(IBattleObserver observer)
		{
			this.BattleObserver = observer;
		}

		public override void AfterStart()
		{
			this._builtAgentCountForSides = new int[2];
			this._removedAgentCountForSides = new int[2];
		}

		public override void OnAgentBuild(Agent agent, Banner banner)
		{
			if (agent.IsHuman)
			{
				BattleSideEnum side = agent.Team.Side;
				this.BattleObserver.TroopNumberChanged(side, agent.Origin.BattleCombatant, agent.Character, 1, 0, 0, 0, 0, 0);
				this._builtAgentCountForSides[(int)side]++;
			}
		}

		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
		{
			if (affectedAgent.IsHuman)
			{
				BattleSideEnum side = affectedAgent.Team.Side;
				switch (agentState)
				{
				case AgentState.Routed:
					this.BattleObserver.TroopNumberChanged(side, affectedAgent.Origin.BattleCombatant, affectedAgent.Character, -1, 0, 0, 1, 0, 0);
					break;
				case AgentState.Unconscious:
					this.BattleObserver.TroopNumberChanged(side, affectedAgent.Origin.BattleCombatant, affectedAgent.Character, -1, 0, 1, 0, 0, 0);
					break;
				case AgentState.Killed:
					this.BattleObserver.TroopNumberChanged(side, affectedAgent.Origin.BattleCombatant, affectedAgent.Character, -1, 1, 0, 0, 0, 0);
					break;
				default:
					throw new ArgumentOutOfRangeException("agentState", agentState, null);
				}
				this._removedAgentCountForSides[(int)side]++;
				if (affectorAgent != null && affectorAgent.IsHuman && (agentState == AgentState.Unconscious || agentState == AgentState.Killed))
				{
					this.BattleObserver.TroopNumberChanged(affectorAgent.Team.Side, affectorAgent.Origin.BattleCombatant, affectorAgent.Character, 0, 0, 0, 0, 1, 0);
				}
			}
		}

		public override void OnMissionResultReady(MissionResult missionResult)
		{
			if (missionResult.PlayerVictory)
			{
				this.BattleObserver.BattleResultsReady();
			}
		}

		public float GetDeathToBuiltAgentRatioForSide(BattleSideEnum side)
		{
			return (float)this._removedAgentCountForSides[(int)side] / (float)this._builtAgentCountForSides[(int)side];
		}

		private int[] _builtAgentCountForSides;

		private int[] _removedAgentCountForSides;
	}
}
