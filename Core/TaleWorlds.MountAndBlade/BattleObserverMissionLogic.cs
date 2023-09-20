using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000263 RID: 611
	public class BattleObserverMissionLogic : MissionLogic
	{
		// Token: 0x17000650 RID: 1616
		// (get) Token: 0x060020C5 RID: 8389 RVA: 0x000755CF File Offset: 0x000737CF
		// (set) Token: 0x060020C6 RID: 8390 RVA: 0x000755D7 File Offset: 0x000737D7
		public IBattleObserver BattleObserver { get; private set; }

		// Token: 0x060020C7 RID: 8391 RVA: 0x000755E0 File Offset: 0x000737E0
		public void SetObserver(IBattleObserver observer)
		{
			this.BattleObserver = observer;
		}

		// Token: 0x060020C8 RID: 8392 RVA: 0x000755EC File Offset: 0x000737EC
		public override void AfterStart()
		{
			this._builtAgentCountForSides = new int[2];
			this._removedAgentCountForSides = new int[2];
			this._isSpawningInitialTroopsForSides = new bool[2];
			for (int i = 0; i < 2; i++)
			{
				this._isSpawningInitialTroopsForSides[i] = true;
			}
			this._missionSpawnLogic = base.Mission.GetMissionBehavior<MissionAgentSpawnLogic>();
			if (this._missionSpawnLogic != null)
			{
				this._missionSpawnLogic.OnInitialTroopsSpawned += this.OnInitialTroopsSpawned;
			}
		}

		// Token: 0x060020C9 RID: 8393 RVA: 0x00075662 File Offset: 0x00073862
		protected override void OnEndMission()
		{
			if (this._missionSpawnLogic != null)
			{
				this._missionSpawnLogic.OnInitialTroopsSpawned -= this.OnInitialTroopsSpawned;
			}
		}

		// Token: 0x060020CA RID: 8394 RVA: 0x00075684 File Offset: 0x00073884
		public override void OnAgentBuild(Agent agent, Banner banner)
		{
			if (agent.IsHuman)
			{
				BattleSideEnum side = agent.Team.Side;
				if (this._isSpawningInitialTroopsForSides[(int)side] || base.Mission.Mode != MissionMode.Deployment)
				{
					this.BattleObserver.TroopNumberChanged(side, agent.Origin.BattleCombatant, agent.Character, 1, 0, 0, 0, 0, 0);
					this._builtAgentCountForSides[(int)side]++;
				}
			}
		}

		// Token: 0x060020CB RID: 8395 RVA: 0x000756F4 File Offset: 0x000738F4
		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
		{
			if (affectedAgent.IsHuman)
			{
				BattleSideEnum side = affectedAgent.Team.Side;
				if (this._isSpawningInitialTroopsForSides[(int)side] || base.Mission.Mode != MissionMode.Deployment)
				{
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
		}

		// Token: 0x060020CC RID: 8396 RVA: 0x0007581B File Offset: 0x00073A1B
		public override void OnMissionResultReady(MissionResult missionResult)
		{
			if (missionResult.PlayerVictory)
			{
				this.BattleObserver.BattleResultsReady();
			}
		}

		// Token: 0x060020CD RID: 8397 RVA: 0x00075830 File Offset: 0x00073A30
		public float GetDeathToBuiltAgentRatioForSide(BattleSideEnum side)
		{
			return (float)this._removedAgentCountForSides[(int)side] / (float)this._builtAgentCountForSides[(int)side];
		}

		// Token: 0x060020CE RID: 8398 RVA: 0x00075845 File Offset: 0x00073A45
		private void OnInitialTroopsSpawned(BattleSideEnum battleSide, int spawnedUnitCount)
		{
			this._isSpawningInitialTroopsForSides[(int)battleSide] = false;
		}

		// Token: 0x04000C16 RID: 3094
		private int[] _builtAgentCountForSides;

		// Token: 0x04000C17 RID: 3095
		private int[] _removedAgentCountForSides;

		// Token: 0x04000C18 RID: 3096
		private bool[] _isSpawningInitialTroopsForSides;

		// Token: 0x04000C19 RID: 3097
		private MissionAgentSpawnLogic _missionSpawnLogic;
	}
}
