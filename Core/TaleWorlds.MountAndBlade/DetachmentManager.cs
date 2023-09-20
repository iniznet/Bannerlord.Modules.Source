﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200013A RID: 314
	public class DetachmentManager
	{
		// Token: 0x1700037C RID: 892
		// (get) Token: 0x06000FC8 RID: 4040 RVA: 0x0002EFE2 File Offset: 0x0002D1E2
		public MBReadOnlyList<ValueTuple<IDetachment, DetachmentData>> Detachments
		{
			get
			{
				return this._detachments;
			}
		}

		// Token: 0x06000FC9 RID: 4041 RVA: 0x0002EFEC File Offset: 0x0002D1EC
		public DetachmentManager(Team team)
		{
			this._detachments = new MBList<ValueTuple<IDetachment, DetachmentData>>();
			this._detachmentDataDictionary = new Dictionary<IDetachment, DetachmentData>();
			this._team = team;
			team.OnFormationsChanged += this.Team_OnFormationsChanged;
			this._slotIndexWeightTuplesCache = new List<ValueTuple<int, float>>();
			this._templateCostCache = new List<float>();
		}

		// Token: 0x06000FCA RID: 4042 RVA: 0x0002F044 File Offset: 0x0002D244
		private void Team_OnFormationsChanged(Team team, Formation formation)
		{
			float currentTime = Mission.Current.CurrentTime;
			foreach (IDetachment detachment in formation.Detachments)
			{
				DetachmentData detachmentData = this._detachmentDataDictionary[detachment];
				detachmentData.agentScores.Clear();
				detachmentData.firstTime = currentTime;
			}
		}

		// Token: 0x06000FCB RID: 4043 RVA: 0x0002F0B8 File Offset: 0x0002D2B8
		public void Clear()
		{
			this._team.OnFormationsChanged -= this.Team_OnFormationsChanged;
			this._team.OnFormationsChanged += this.Team_OnFormationsChanged;
			foreach (ValueTuple<IDetachment, DetachmentData> valueTuple in this.Detachments.ToList<ValueTuple<IDetachment, DetachmentData>>())
			{
				this.DestroyDetachment(valueTuple.Item1);
			}
		}

		// Token: 0x06000FCC RID: 4044 RVA: 0x0002F144 File Offset: 0x0002D344
		public bool ContainsDetachment(IDetachment detachment)
		{
			return this._detachmentDataDictionary.ContainsKey(detachment);
		}

		// Token: 0x06000FCD RID: 4045 RVA: 0x0002F154 File Offset: 0x0002D354
		public void MakeDetachment(IDetachment detachment)
		{
			DetachmentData detachmentData = new DetachmentData();
			this._detachments.Add(new ValueTuple<IDetachment, DetachmentData>(detachment, detachmentData));
			this._detachmentDataDictionary[detachment] = detachmentData;
		}

		// Token: 0x06000FCE RID: 4046 RVA: 0x0002F188 File Offset: 0x0002D388
		public void DestroyDetachment(IDetachment destroyedDetachment)
		{
			for (int i = 0; i < this._detachments.Count; i++)
			{
				ValueTuple<IDetachment, DetachmentData> valueTuple = this._detachments[i];
				if (valueTuple.Item1 == destroyedDetachment)
				{
					for (int j = valueTuple.Item2.joinedFormations.Count - 1; j >= 0; j--)
					{
						valueTuple.Item2.joinedFormations[j].LeaveDetachment(destroyedDetachment);
					}
					this._detachments.RemoveAt(i);
					break;
				}
			}
			this._detachmentDataDictionary.Remove(destroyedDetachment);
		}

		// Token: 0x06000FCF RID: 4047 RVA: 0x0002F210 File Offset: 0x0002D410
		public void OnFormationJoinDetachment(Formation formation, IDetachment joinedDetachment)
		{
			DetachmentData detachmentData = this._detachmentDataDictionary[joinedDetachment];
			detachmentData.joinedFormations.Add(formation);
			detachmentData.firstTime = MBCommon.GetTotalMissionTime();
			joinedDetachment.FormationStartUsing(formation);
		}

		// Token: 0x06000FD0 RID: 4048 RVA: 0x0002F23C File Offset: 0x0002D43C
		public void OnFormationLeaveDetachment(Formation formation, IDetachment leftDetachment)
		{
			DetachmentData detachmentData = this._detachmentDataDictionary[leftDetachment];
			detachmentData.joinedFormations.Remove(formation);
			detachmentData.agentScores.RemoveAll((ValueTuple<Agent, List<float>> ags) => ags.Item1.Formation == formation);
			detachmentData.firstTime = MBCommon.GetTotalMissionTime();
			leftDetachment.FormationStopUsing(formation);
		}

		// Token: 0x06000FD1 RID: 4049 RVA: 0x0002F2A4 File Offset: 0x0002D4A4
		public void TickDetachments()
		{
			float totalMissionTime = MBCommon.GetTotalMissionTime();
			if (!Mission.Current.IsLoadingFinished || !Mission.Current.AllowAiTicking)
			{
				using (List<ValueTuple<IDetachment, DetachmentData>>.Enumerator enumerator = this._detachments.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ValueTuple<IDetachment, DetachmentData> valueTuple = enumerator.Current;
						valueTuple.Item2.firstTime = totalMissionTime;
					}
					return;
				}
			}
			foreach (ValueTuple<IDetachment, DetachmentData> valueTuple2 in this._detachments)
			{
				if (valueTuple2.Item1.ComputeAndCacheDetachmentWeight(this._team.Side) > -3.4028235E+38f && valueTuple2.Item2.IsPrecalculated())
				{
					valueTuple2.Item1.ResetEvaluation();
				}
			}
			bool flag = this._detachments.Count == 0;
			while (!flag)
			{
				ValueTuple<IDetachment, DetachmentData> valueTuple3 = new ValueTuple<IDetachment, DetachmentData>(null, null);
				float num = float.MinValue;
				foreach (ValueTuple<IDetachment, DetachmentData> valueTuple4 in this._detachments)
				{
					float detachmentWeightFromCache = valueTuple4.Item1.GetDetachmentWeightFromCache();
					if (num < detachmentWeightFromCache && valueTuple4.Item2.IsPrecalculated() && !valueTuple4.Item1.IsEvaluated())
					{
						num = detachmentWeightFromCache;
						valueTuple3 = valueTuple4;
					}
				}
				if (valueTuple3.Item1 != null)
				{
					IDetachment item = valueTuple3.Item1;
					DetachmentData item2 = valueTuple3.Item2;
					if (valueTuple3.Item1.IsDetachmentRecentlyEvaluated())
					{
						foreach (ValueTuple<IDetachment, DetachmentData> valueTuple5 in this._detachments)
						{
							if (!valueTuple5.Item1.IsEvaluated())
							{
								valueTuple5.Item1.UnmarkDetachment();
							}
						}
					}
					item.GetSlotIndexWeightTuples(this._slotIndexWeightTuplesCache);
					while (!flag && this._slotIndexWeightTuplesCache.Count > 0)
					{
						ValueTuple<int, float> valueTuple6 = new ValueTuple<int, float>(0, 0f);
						float num2 = float.MinValue;
						foreach (ValueTuple<int, float> valueTuple7 in this._slotIndexWeightTuplesCache)
						{
							if (num2 < valueTuple7.Item2)
							{
								num2 = valueTuple7.Item2;
								valueTuple6 = valueTuple7;
							}
						}
						float num3 = float.MaxValue;
						int num4 = -1;
						for (int i = 0; i < item2.agentScores.Count; i++)
						{
							ValueTuple<Agent, List<float>> valueTuple8 = item2.agentScores[i];
							if (item.IsSlotAtIndexAvailableForAgent(valueTuple6.Item1, valueTuple8.Item1))
							{
								float num5 = valueTuple8.Item2[valueTuple6.Item1];
								if (num3 > num5)
								{
									num3 = num5;
									num4 = i;
								}
							}
						}
						if (num4 != -1)
						{
							Agent movingAgentAtSlotIndex = item.GetMovingAgentAtSlotIndex(valueTuple6.Item1);
							float num6 = float.MaxValue;
							float num7 = float.MaxValue;
							if (movingAgentAtSlotIndex != null)
							{
								int num8 = -1;
								for (int j = 0; j < item2.agentScores.Count; j++)
								{
									if (item2.agentScores[j].Item1 == movingAgentAtSlotIndex)
									{
										num8 = j;
										break;
									}
								}
								float exactCostOfAgentAtSlot = item.GetExactCostOfAgentAtSlot(movingAgentAtSlotIndex, valueTuple6.Item1);
								if (num8 == -1)
								{
									this._templateCostCache = item.GetTemplateCostsOfAgent(movingAgentAtSlotIndex, this._templateCostCache);
									num7 = this._templateCostCache[valueTuple6.Item1];
								}
								else
								{
									ValueTuple<Agent, List<float>> valueTuple9 = item2.agentScores[num8];
									num7 = valueTuple9.Item2[valueTuple6.Item1];
									valueTuple9.Item2[valueTuple6.Item1] = exactCostOfAgentAtSlot;
								}
								num6 = exactCostOfAgentAtSlot;
							}
							if (movingAgentAtSlotIndex != null)
							{
								item.MarkSlotAtIndex(valueTuple6.Item1);
							}
							ValueTuple<Agent, List<float>> valueTuple10 = item2.agentScores[num4];
							float num9 = valueTuple10.Item2[valueTuple6.Item1];
							if (movingAgentAtSlotIndex != null && num6 <= num9)
							{
								flag = true;
							}
							else if (movingAgentAtSlotIndex != null)
							{
								float exactCostOfAgentAtSlot2 = item.GetExactCostOfAgentAtSlot(valueTuple10.Item1, valueTuple6.Item1);
								valueTuple10.Item2[valueTuple6.Item1] = exactCostOfAgentAtSlot2;
								if (num6 < exactCostOfAgentAtSlot2)
								{
									flag = true;
								}
								else if (num6 == exactCostOfAgentAtSlot2 && num7 < num9)
								{
									flag = true;
								}
							}
							if (!flag)
							{
								item.AddAgentAtSlotIndex(valueTuple10.Item1, valueTuple6.Item1);
								flag = true;
							}
						}
						else
						{
							this._slotIndexWeightTuplesCache.Remove(valueTuple6);
						}
					}
					this._slotIndexWeightTuplesCache.Clear();
					if (!flag)
					{
						valueTuple3.Item1.SetAsEvaluated();
					}
				}
				else
				{
					flag = true;
				}
			}
		}

		// Token: 0x06000FD2 RID: 4050 RVA: 0x0002F764 File Offset: 0x0002D964
		public void TickAgent(Agent agent)
		{
			bool isDetachedFromFormation = agent.IsDetachedFromFormation;
			if (agent.IsDetachableFromFormation)
			{
				if (isDetachedFromFormation && !agent.Detachment.IsAgentEligible(agent))
				{
					agent.Detachment.RemoveAgent(agent);
					Formation formation = agent.Formation;
					if (formation != null)
					{
						formation.AttachUnit(agent);
					}
				}
				if (!agent.IsDetachedFromFormation)
				{
					float totalMissionTime = MBCommon.GetTotalMissionTime();
					bool flag = totalMissionTime - agent.LastDetachmentTickAgentTime > 1.5f;
					using (List<IDetachment>.Enumerator enumerator = agent.Formation.Detachments.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							IDetachment detachment = enumerator.Current;
							float detachmentWeight = detachment.GetDetachmentWeight(agent.Formation.Team.Side);
							DetachmentData detachmentData = this._detachmentDataDictionary[detachment];
							if (detachmentWeight > -3.4028235E+38f)
							{
								int num = -1;
								for (int i = 0; i < detachmentData.agentScores.Count; i++)
								{
									if (detachmentData.agentScores[i].Item1 == agent)
									{
										num = i;
										break;
									}
								}
								if (num == -1)
								{
									if (detachmentData.agentScores.Count == 0)
									{
										detachmentData.firstTime = totalMissionTime;
									}
									List<float> templateCostsOfAgent = detachment.GetTemplateCostsOfAgent(agent, null);
									detachmentData.agentScores.Add(new ValueTuple<Agent, List<float>>(agent, templateCostsOfAgent));
									agent.LastDetachmentTickAgentTime = totalMissionTime;
								}
								else if (flag)
								{
									ValueTuple<Agent, List<float>> valueTuple = detachmentData.agentScores[num];
									detachmentData.agentScores[num] = new ValueTuple<Agent, List<float>>(valueTuple.Item1, detachment.GetTemplateCostsOfAgent(agent, valueTuple.Item2));
									agent.LastDetachmentTickAgentTime = totalMissionTime;
								}
							}
							else
							{
								detachmentData.firstTime = totalMissionTime;
							}
						}
						return;
					}
				}
				if (agent.AIMoveToGameObjectIsEnabled())
				{
					float totalMissionTime2 = MBCommon.GetTotalMissionTime();
					DetachmentData detachmentData2 = this._detachmentDataDictionary[agent.Detachment];
					int num2 = -1;
					for (int j = 0; j < detachmentData2.agentScores.Count; j++)
					{
						if (detachmentData2.agentScores[j].Item1 == agent)
						{
							num2 = j;
							break;
						}
					}
					if (num2 == -1)
					{
						if (detachmentData2.agentScores.Count == 0)
						{
							detachmentData2.firstTime = totalMissionTime2;
						}
						List<float> templateCostsOfAgent2 = agent.Detachment.GetTemplateCostsOfAgent(agent, null);
						detachmentData2.agentScores.Add(new ValueTuple<Agent, List<float>>(agent, templateCostsOfAgent2));
						agent.LastDetachmentTickAgentTime = totalMissionTime2;
						return;
					}
					if (totalMissionTime2 - agent.LastDetachmentTickAgentTime > 1.5f)
					{
						ValueTuple<Agent, List<float>> valueTuple2 = detachmentData2.agentScores[num2];
						detachmentData2.agentScores[num2] = new ValueTuple<Agent, List<float>>(valueTuple2.Item1, agent.Detachment.GetTemplateCostsOfAgent(agent, valueTuple2.Item2));
						agent.LastDetachmentTickAgentTime = totalMissionTime2;
					}
				}
			}
		}

		// Token: 0x06000FD3 RID: 4051 RVA: 0x0002FA24 File Offset: 0x0002DC24
		public void OnAgentRemoved(Agent agent)
		{
			Predicate<ValueTuple<Agent, List<float>>> <>9__0;
			foreach (IDetachment detachment in agent.Formation.Detachments)
			{
				List<ValueTuple<Agent, List<float>>> agentScores = this._detachmentDataDictionary[detachment].agentScores;
				Predicate<ValueTuple<Agent, List<float>>> predicate;
				if ((predicate = <>9__0) == null)
				{
					predicate = (<>9__0 = (ValueTuple<Agent, List<float>> ags) => ags.Item1 == agent);
				}
				agentScores.RemoveAll(predicate);
			}
		}

		// Token: 0x06000FD4 RID: 4052 RVA: 0x0002FAC0 File Offset: 0x0002DCC0
		public void RemoveScoresOfAgentFromDetachments(Agent agent)
		{
			foreach (ValueTuple<IDetachment, DetachmentData> valueTuple in this._detachments)
			{
				if (!agent.AIMoveToGameObjectIsEnabled() || agent.Detachment != valueTuple.Item1)
				{
					valueTuple.Item2.RemoveScoreOfAgent(agent);
				}
			}
		}

		// Token: 0x06000FD5 RID: 4053 RVA: 0x0002FB30 File Offset: 0x0002DD30
		public void AddAgentAsMovingToDetachment(Agent agent, IDetachment detachment)
		{
			if (detachment != null)
			{
				this._detachmentDataDictionary[detachment].MovingAgentCount++;
			}
		}

		// Token: 0x06000FD6 RID: 4054 RVA: 0x0002FB50 File Offset: 0x0002DD50
		public void RemoveAgentAsMovingToDetachment(Agent agent)
		{
			if (agent.Detachment != null)
			{
				if (!this._detachmentDataDictionary.ContainsKey(agent.Detachment))
				{
					Debug.Print("DUMP-1671 | " + agent.Detachment.ToString(), 0, Debug.DebugColor.White, 17592186044416UL);
				}
				this._detachmentDataDictionary[agent.Detachment].MovingAgentCount--;
			}
		}

		// Token: 0x06000FD7 RID: 4055 RVA: 0x0002FBBC File Offset: 0x0002DDBC
		public void AddAgentAsDefendingToDetachment(Agent agent, IDetachment detachment)
		{
			this._detachmentDataDictionary[detachment].DefendingAgentCount++;
		}

		// Token: 0x06000FD8 RID: 4056 RVA: 0x0002FBD8 File Offset: 0x0002DDD8
		public void RemoveAgentAsDefendingToDetachment(Agent agent)
		{
			if (!this._detachmentDataDictionary.ContainsKey(agent.Detachment))
			{
				Debug.Print("DUMP-1671 | " + ((agent.Detachment != null) ? agent.Detachment.ToString() : "null"), 0, Debug.DebugColor.White, 17592186044416UL);
			}
			this._detachmentDataDictionary[agent.Detachment].DefendingAgentCount--;
		}

		// Token: 0x06000FD9 RID: 4057 RVA: 0x0002FC4B File Offset: 0x0002DE4B
		[Conditional("DEBUG")]
		private void AssertDetachments()
		{
		}

		// Token: 0x06000FDA RID: 4058 RVA: 0x0002FC50 File Offset: 0x0002DE50
		[Conditional("DEBUG")]
		public void AssertDetachment(Team team, IDetachment detachment)
		{
			this._detachmentDataDictionary[detachment].joinedFormations.Where((Formation f) => f.CountOfUnits > 0);
			team.FormationsIncludingSpecialAndEmpty.Where((Formation f) => f.CountOfUnits > 0 && f.Detachments.Contains(detachment));
		}

		// Token: 0x040003B0 RID: 944
		private readonly MBList<ValueTuple<IDetachment, DetachmentData>> _detachments;

		// Token: 0x040003B1 RID: 945
		private readonly Dictionary<IDetachment, DetachmentData> _detachmentDataDictionary;

		// Token: 0x040003B2 RID: 946
		private readonly List<ValueTuple<int, float>> _slotIndexWeightTuplesCache;

		// Token: 0x040003B3 RID: 947
		private List<float> _templateCostCache;

		// Token: 0x040003B4 RID: 948
		private readonly Team _team;
	}
}
