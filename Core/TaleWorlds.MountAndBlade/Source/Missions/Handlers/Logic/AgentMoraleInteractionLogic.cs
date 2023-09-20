using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Source.Missions.Handlers.Logic
{
	// Token: 0x020003FC RID: 1020
	public class AgentMoraleInteractionLogic : MissionLogic
	{
		// Token: 0x0600350C RID: 13580 RVA: 0x000DCEC8 File Offset: 0x000DB0C8
		public AgentMoraleInteractionLogic()
		{
			this._nearbyAgentsCache = new MBList<Agent>();
			this._nearbyAllyAgentsCache = new MBList<Agent>();
		}

		// Token: 0x0600350D RID: 13581 RVA: 0x000DCF28 File Offset: 0x000DB128
		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
		{
			if (affectedAgent == null || !affectedAgent.IsHuman || (agentState != AgentState.Killed && agentState != AgentState.Unconscious))
			{
				return;
			}
			ValueTuple<float, float> valueTuple = MissionGameModels.Current.BattleMoraleModel.CalculateMaxMoraleChangeDueToAgentIncapacitated(affectedAgent, agentState, affectorAgent, killingBlow);
			float item = valueTuple.Item1;
			float item2 = valueTuple.Item2;
			if (item > 0f || item2 > 0f)
			{
				if (affectorAgent != null)
				{
					affectorAgent = (affectorAgent.IsHuman ? affectorAgent : (affectorAgent.IsMount ? affectorAgent.RiderAgent : null));
				}
				this.ApplyMoraleEffectOnAgentIncapacitated(affectedAgent, affectorAgent, item, item2, 4f);
			}
		}

		// Token: 0x0600350E RID: 13582 RVA: 0x000DCFAC File Offset: 0x000DB1AC
		public override void OnAgentFleeing(Agent affectedAgent)
		{
			if (affectedAgent == null || !affectedAgent.IsHuman)
			{
				return;
			}
			ValueTuple<float, float> valueTuple = MissionGameModels.Current.BattleMoraleModel.CalculateMaxMoraleChangeDueToAgentPanicked(affectedAgent);
			float item = valueTuple.Item1;
			float item2 = valueTuple.Item2;
			if (item > 0f || item2 > 0f)
			{
				this.ApplyMoraleEffectOnAgentIncapacitated(affectedAgent, null, item, item2, 4f);
			}
			if (MBRandom.RandomFloat < 0.7f)
			{
				affectedAgent.MakeVoice(SkinVoiceManager.VoiceType.Debacle, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
			}
		}

		// Token: 0x0600350F RID: 13583 RVA: 0x000DD01C File Offset: 0x000DB21C
		private void ApplyMoraleEffectOnAgentIncapacitated(Agent affectedAgent, Agent affectorAgent, float affectedSideMaxMoraleLoss, float affectorSideMoraleMaxGain, float effectRadius)
		{
			AgentMoraleInteractionLogic.<>c__DisplayClass15_0 CS$<>8__locals1 = new AgentMoraleInteractionLogic.<>c__DisplayClass15_0();
			CS$<>8__locals1.affectedAgent = affectedAgent;
			CS$<>8__locals1.affectorAgent = affectorAgent;
			this._agentsToReceiveMoraleLoss.Clear();
			this._agentsToReceiveMoraleGain.Clear();
			if (CS$<>8__locals1.affectedAgent != null && CS$<>8__locals1.affectedAgent.IsHuman)
			{
				Vec2 asVec = CS$<>8__locals1.affectedAgent.GetWorldPosition().AsVec2;
				base.Mission.GetNearbyAgents(asVec, effectRadius, this._nearbyAgentsCache);
				this.SelectRandomAgentsFromListToAgentSet(this._nearbyAgentsCache, this._agentsToReceiveMoraleLoss, 10, new Predicate<Agent>(CS$<>8__locals1.<ApplyMoraleEffectOnAgentIncapacitated>g__AffectedsAllyCondition|0));
				if (this._agentsToReceiveMoraleLoss.Count < 10 && CS$<>8__locals1.affectedAgent.Formation != null)
				{
					this.SelectRandomAgentsFromFormationToAgentSet(CS$<>8__locals1.affectedAgent.Formation, this._agentsToReceiveMoraleLoss, 10, new Predicate<IFormationUnit>(AgentMoraleInteractionLogic.<>c.<>9.<ApplyMoraleEffectOnAgentIncapacitated>g__FormationCondition|15_1));
				}
				if (CS$<>8__locals1.affectorAgent != null && CS$<>8__locals1.affectorAgent.IsActive() && CS$<>8__locals1.affectorAgent.IsHuman && CS$<>8__locals1.affectorAgent.IsAIControlled && CS$<>8__locals1.affectorAgent.IsEnemyOf(CS$<>8__locals1.affectedAgent))
				{
					this._agentsToReceiveMoraleGain.Add(CS$<>8__locals1.affectorAgent);
				}
				if (this._agentsToReceiveMoraleGain.Count < 10)
				{
					this.SelectRandomAgentsFromListToAgentSet(this._nearbyAgentsCache, this._agentsToReceiveMoraleGain, 10, new Predicate<Agent>(CS$<>8__locals1.<ApplyMoraleEffectOnAgentIncapacitated>g__AffectedsEnemyCondition|2));
				}
				if (this._agentsToReceiveMoraleGain.Count < 10)
				{
					Agent affectorAgent2 = CS$<>8__locals1.affectorAgent;
					if (((affectorAgent2 != null) ? affectorAgent2.Team : null) != null && CS$<>8__locals1.affectorAgent.IsEnemyOf(CS$<>8__locals1.affectedAgent))
					{
						Vec2 asVec2 = CS$<>8__locals1.affectorAgent.GetWorldPosition().AsVec2;
						if (asVec2.DistanceSquared(asVec) > 2.25f)
						{
							base.Mission.GetNearbyAllyAgents(asVec2, effectRadius, CS$<>8__locals1.affectedAgent.Team, this._nearbyAllyAgentsCache);
							this.SelectRandomAgentsFromListToAgentSet(this._nearbyAllyAgentsCache, this._agentsToReceiveMoraleGain, 10, new Predicate<Agent>(CS$<>8__locals1.<ApplyMoraleEffectOnAgentIncapacitated>g__AffectorsAllyCondition|4));
						}
					}
				}
				if (this._agentsToReceiveMoraleGain.Count < 10)
				{
					Agent affectorAgent3 = CS$<>8__locals1.affectorAgent;
					if (((affectorAgent3 != null) ? affectorAgent3.Formation : null) != null)
					{
						this.SelectRandomAgentsFromFormationToAgentSet(CS$<>8__locals1.affectorAgent.Formation, this._agentsToReceiveMoraleGain, 10, new Predicate<IFormationUnit>(AgentMoraleInteractionLogic.<>c.<>9.<ApplyMoraleEffectOnAgentIncapacitated>g__FormationCondition|15_3));
					}
				}
			}
			foreach (Agent agent in this._agentsToReceiveMoraleLoss)
			{
				float num = -MissionGameModels.Current.BattleMoraleModel.CalculateMoraleChangeToCharacter(agent, affectedSideMaxMoraleLoss);
				agent.ChangeMorale(num);
			}
			foreach (Agent agent2 in this._agentsToReceiveMoraleGain)
			{
				float num2 = MissionGameModels.Current.BattleMoraleModel.CalculateMoraleChangeToCharacter(agent2, affectorSideMoraleMaxGain);
				agent2.ChangeMorale(num2);
			}
		}

		// Token: 0x06003510 RID: 13584 RVA: 0x000DD324 File Offset: 0x000DB524
		private void SelectRandomAgentsFromListToAgentSet(MBReadOnlyList<Agent> agentsList, HashSet<Agent> outputAgentsSet, int maxCountInSet, Predicate<Agent> conditions = null)
		{
			if (outputAgentsSet != null && agentsList != null)
			{
				this._randomAgentSelector.Initialize(agentsList);
				Agent agent;
				while (outputAgentsSet.Count < maxCountInSet && this._randomAgentSelector.SelectRandom(out agent, conditions))
				{
					outputAgentsSet.Add(agent);
				}
			}
		}

		// Token: 0x06003511 RID: 13585 RVA: 0x000DD368 File Offset: 0x000DB568
		private void SelectRandomAgentsFromFormationToAgentSet(Formation formation, HashSet<Agent> outputAgentsSet, int maxCountInSet, Predicate<IFormationUnit> conditions = null)
		{
			if (outputAgentsSet != null && formation != null && formation.CountOfUnits > 0)
			{
				int num = Math.Max(0, maxCountInSet - outputAgentsSet.Count);
				if (num > 0)
				{
					int num2 = (int)((float)formation.CountOfDetachedUnits / (float)formation.CountOfUnits * (float)num);
					if (num2 > 0)
					{
						this._randomAgentSelector.Initialize(formation.DetachedUnits);
						int num3 = 0;
						Agent agent;
						while (num3 < num2 && outputAgentsSet.Count < maxCountInSet && this._randomAgentSelector.SelectRandom(out agent, conditions))
						{
							if (outputAgentsSet.Add(agent))
							{
								num3++;
							}
						}
					}
					if (outputAgentsSet.Count < maxCountInSet)
					{
						IFormationArrangement arrangement = formation.Arrangement;
						MBList<IFormationUnit> mblist;
						if ((mblist = ((arrangement != null) ? arrangement.GetAllUnits() : null)) != null && mblist.Count > 0)
						{
							this._randomFormationUnitSelector.Initialize(mblist);
							IFormationUnit formationUnit;
							while (outputAgentsSet.Count < maxCountInSet && this._randomFormationUnitSelector.SelectRandom(out formationUnit, conditions))
							{
								Agent agent2;
								if ((agent2 = formationUnit as Agent) != null)
								{
									outputAgentsSet.Add(agent2);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x040016B9 RID: 5817
		private const float DebacleVoiceChance = 0.7f;

		// Token: 0x040016BA RID: 5818
		private const float MoraleEffectRadius = 4f;

		// Token: 0x040016BB RID: 5819
		private const int MaxNumAgentsToGainMorale = 10;

		// Token: 0x040016BC RID: 5820
		private const int MaxNumAgentsToLoseMorale = 10;

		// Token: 0x040016BD RID: 5821
		private const float SquaredDistanceForSeparateAffectorQuery = 2.25f;

		// Token: 0x040016BE RID: 5822
		private const ushort RandomSelectorCapacity = 1024;

		// Token: 0x040016BF RID: 5823
		private readonly HashSet<Agent> _agentsToReceiveMoraleGain = new HashSet<Agent>();

		// Token: 0x040016C0 RID: 5824
		private readonly HashSet<Agent> _agentsToReceiveMoraleLoss = new HashSet<Agent>();

		// Token: 0x040016C1 RID: 5825
		private readonly MBFastRandomSelector<Agent> _randomAgentSelector = new MBFastRandomSelector<Agent>(1024);

		// Token: 0x040016C2 RID: 5826
		private readonly MBFastRandomSelector<IFormationUnit> _randomFormationUnitSelector = new MBFastRandomSelector<IFormationUnit>(1024);

		// Token: 0x040016C3 RID: 5827
		private readonly MBList<Agent> _nearbyAgentsCache;

		// Token: 0x040016C4 RID: 5828
		private readonly MBList<Agent> _nearbyAllyAgentsCache;
	}
}
