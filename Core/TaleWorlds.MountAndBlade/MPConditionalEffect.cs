using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000311 RID: 785
	public class MPConditionalEffect
	{
		// Token: 0x1700078D RID: 1933
		// (get) Token: 0x06002A6C RID: 10860 RVA: 0x000A4E51 File Offset: 0x000A3051
		public MBReadOnlyList<MPPerkCondition> Conditions { get; }

		// Token: 0x1700078E RID: 1934
		// (get) Token: 0x06002A6D RID: 10861 RVA: 0x000A4E59 File Offset: 0x000A3059
		public MBReadOnlyList<MPPerkEffectBase> Effects { get; }

		// Token: 0x1700078F RID: 1935
		// (get) Token: 0x06002A6E RID: 10862 RVA: 0x000A4E64 File Offset: 0x000A3064
		public MPPerkCondition.PerkEventFlags EventFlags
		{
			get
			{
				MPPerkCondition.PerkEventFlags perkEventFlags = MPPerkCondition.PerkEventFlags.None;
				foreach (MPPerkCondition mpperkCondition in this.Conditions)
				{
					perkEventFlags |= mpperkCondition.EventFlags;
				}
				return perkEventFlags;
			}
		}

		// Token: 0x17000790 RID: 1936
		// (get) Token: 0x06002A6F RID: 10863 RVA: 0x000A4EBC File Offset: 0x000A30BC
		public bool IsTickRequired
		{
			get
			{
				using (List<MPPerkEffectBase>.Enumerator enumerator = this.Effects.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.IsTickRequired)
						{
							return true;
						}
					}
				}
				return false;
			}
		}

		// Token: 0x06002A70 RID: 10864 RVA: 0x000A4F18 File Offset: 0x000A3118
		public MPConditionalEffect(List<string> gameModes, XmlNode node)
		{
			MBList<MPPerkCondition> mblist = new MBList<MPPerkCondition>();
			MBList<MPPerkEffectBase> mblist2 = new MBList<MPPerkEffectBase>();
			foreach (object obj in node.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (xmlNode.Name == "Conditions")
				{
					using (IEnumerator enumerator2 = xmlNode.ChildNodes.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							object obj2 = enumerator2.Current;
							XmlNode xmlNode2 = (XmlNode)obj2;
							if (xmlNode2.NodeType == XmlNodeType.Element)
							{
								mblist.Add(MPPerkCondition.CreateFrom(gameModes, xmlNode2));
							}
						}
						continue;
					}
				}
				if (xmlNode.Name == "Effects")
				{
					foreach (object obj3 in xmlNode.ChildNodes)
					{
						XmlNode xmlNode3 = (XmlNode)obj3;
						if (xmlNode3.NodeType == XmlNodeType.Element)
						{
							MPPerkEffect mpperkEffect = MPPerkEffect.CreateFrom(xmlNode3);
							mblist2.Add(mpperkEffect);
						}
					}
				}
			}
			this.Conditions = mblist;
			this.Effects = mblist2;
		}

		// Token: 0x06002A71 RID: 10865 RVA: 0x000A507C File Offset: 0x000A327C
		public bool Check(MissionPeer peer)
		{
			using (List<MPPerkCondition>.Enumerator enumerator = this.Conditions.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.Check(peer))
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x06002A72 RID: 10866 RVA: 0x000A50D8 File Offset: 0x000A32D8
		public bool Check(Agent agent)
		{
			using (List<MPPerkCondition>.Enumerator enumerator = this.Conditions.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.Check(agent))
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x06002A73 RID: 10867 RVA: 0x000A5134 File Offset: 0x000A3334
		public void OnEvent(bool isWarmup, MissionPeer peer, MPConditionalEffect.ConditionalEffectContainer container)
		{
			if (MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) <= 0)
			{
				if (peer == null)
				{
					return;
				}
				Agent controlledAgent = peer.ControlledAgent;
				bool? flag = ((controlledAgent != null) ? new bool?(controlledAgent.IsActive()) : null);
				bool flag2 = true;
				if (!((flag.GetValueOrDefault() == flag2) & (flag != null)))
				{
					return;
				}
			}
			bool flag3 = true;
			foreach (MPPerkCondition mpperkCondition in this.Conditions)
			{
				if (mpperkCondition.IsPeerCondition && !mpperkCondition.Check(peer))
				{
					flag3 = false;
					break;
				}
			}
			if (!flag3)
			{
				if (MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) > 0)
				{
					MBList<IFormationUnit> mblist;
					if (peer == null)
					{
						mblist = null;
					}
					else
					{
						Formation controlledFormation = peer.ControlledFormation;
						mblist = ((controlledFormation != null) ? controlledFormation.Arrangement.GetAllUnits() : null);
					}
					MBList<IFormationUnit> mblist2 = mblist;
					if (mblist2 == null)
					{
						return;
					}
					using (List<IFormationUnit>.Enumerator enumerator2 = mblist2.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							Agent agent;
							if ((agent = enumerator2.Current as Agent) != null && agent.IsActive())
							{
								this.UpdateAgentState(isWarmup, container, agent, false);
							}
						}
						return;
					}
				}
				this.UpdateAgentState(isWarmup, container, peer.ControlledAgent, false);
				return;
			}
			if (MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) > 0)
			{
				MBList<IFormationUnit> mblist3;
				if (peer == null)
				{
					mblist3 = null;
				}
				else
				{
					Formation controlledFormation2 = peer.ControlledFormation;
					mblist3 = ((controlledFormation2 != null) ? controlledFormation2.Arrangement.GetAllUnits() : null);
				}
				MBList<IFormationUnit> mblist4 = mblist3;
				if (mblist4 == null)
				{
					return;
				}
				using (List<IFormationUnit>.Enumerator enumerator2 = mblist4.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						Agent agent2;
						if ((agent2 = enumerator2.Current as Agent) != null && agent2.IsActive())
						{
							bool flag4 = true;
							foreach (MPPerkCondition mpperkCondition2 in this.Conditions)
							{
								if (!mpperkCondition2.IsPeerCondition && !mpperkCondition2.Check(agent2))
								{
									flag4 = false;
									break;
								}
							}
							this.UpdateAgentState(isWarmup, container, agent2, flag4);
						}
					}
					return;
				}
			}
			bool flag5 = true;
			foreach (MPPerkCondition mpperkCondition3 in this.Conditions)
			{
				if (!mpperkCondition3.IsPeerCondition && !mpperkCondition3.Check(peer.ControlledAgent))
				{
					flag5 = false;
					break;
				}
			}
			this.UpdateAgentState(isWarmup, container, peer.ControlledAgent, flag5);
		}

		// Token: 0x06002A74 RID: 10868 RVA: 0x000A53D4 File Offset: 0x000A35D4
		public void OnEvent(bool isWarmup, Agent agent, MPConditionalEffect.ConditionalEffectContainer container)
		{
			if (agent != null)
			{
				bool flag = true;
				using (List<MPPerkCondition>.Enumerator enumerator = this.Conditions.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (!enumerator.Current.Check(agent))
						{
							flag = false;
							break;
						}
					}
				}
				this.UpdateAgentState(isWarmup, container, agent, flag);
			}
		}

		// Token: 0x06002A75 RID: 10869 RVA: 0x000A543C File Offset: 0x000A363C
		public void OnTick(bool isWarmup, MissionPeer peer, int tickCount)
		{
			if (MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) <= 0)
			{
				if (peer == null)
				{
					return;
				}
				Agent controlledAgent = peer.ControlledAgent;
				bool? flag = ((controlledAgent != null) ? new bool?(controlledAgent.IsActive()) : null);
				bool flag2 = true;
				if (!((flag.GetValueOrDefault() == flag2) & (flag != null)))
				{
					return;
				}
			}
			bool flag3 = true;
			foreach (MPPerkCondition mpperkCondition in this.Conditions)
			{
				if (mpperkCondition.IsPeerCondition && !mpperkCondition.Check(peer))
				{
					flag3 = false;
					break;
				}
			}
			if (flag3)
			{
				if (MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) > 0)
				{
					MBList<IFormationUnit> mblist;
					if (peer == null)
					{
						mblist = null;
					}
					else
					{
						Formation controlledFormation = peer.ControlledFormation;
						mblist = ((controlledFormation != null) ? controlledFormation.Arrangement.GetAllUnits() : null);
					}
					MBList<IFormationUnit> mblist2 = mblist;
					if (mblist2 == null)
					{
						return;
					}
					using (List<IFormationUnit>.Enumerator enumerator2 = mblist2.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							Agent agent;
							if ((agent = enumerator2.Current as Agent) != null && agent.IsActive())
							{
								bool flag4 = true;
								foreach (MPPerkCondition mpperkCondition2 in this.Conditions)
								{
									if (!mpperkCondition2.IsPeerCondition && !mpperkCondition2.Check(agent))
									{
										flag4 = false;
										break;
									}
								}
								if (flag4)
								{
									foreach (MPPerkEffectBase mpperkEffectBase in this.Effects)
									{
										if ((!isWarmup || !mpperkEffectBase.IsDisabledInWarmup) && mpperkEffectBase.IsTickRequired)
										{
											mpperkEffectBase.OnTick(agent, tickCount);
										}
									}
								}
							}
						}
						return;
					}
				}
				bool flag5 = true;
				foreach (MPPerkCondition mpperkCondition3 in this.Conditions)
				{
					if (!mpperkCondition3.IsPeerCondition && !mpperkCondition3.Check(peer.ControlledAgent))
					{
						flag5 = false;
						break;
					}
				}
				if (flag5)
				{
					foreach (MPPerkEffectBase mpperkEffectBase2 in this.Effects)
					{
						if ((!isWarmup || !mpperkEffectBase2.IsDisabledInWarmup) && mpperkEffectBase2.IsTickRequired)
						{
							mpperkEffectBase2.OnTick(peer.ControlledAgent, tickCount);
						}
					}
				}
			}
		}

		// Token: 0x06002A76 RID: 10870 RVA: 0x000A56FC File Offset: 0x000A38FC
		private void UpdateAgentState(bool isWarmup, MPConditionalEffect.ConditionalEffectContainer container, Agent agent, bool state)
		{
			if (container.GetState(this, agent) != state)
			{
				container.SetState(this, agent, state);
				foreach (MPPerkEffectBase mpperkEffectBase in this.Effects)
				{
					if (!isWarmup || !mpperkEffectBase.IsDisabledInWarmup)
					{
						mpperkEffectBase.OnUpdate(agent, state);
					}
				}
			}
		}

		// Token: 0x0200062E RID: 1582
		public class ConditionalEffectContainer : List<MPConditionalEffect>
		{
			// Token: 0x06003DDD RID: 15837 RVA: 0x000F38BA File Offset: 0x000F1ABA
			public ConditionalEffectContainer()
			{
			}

			// Token: 0x06003DDE RID: 15838 RVA: 0x000F38C2 File Offset: 0x000F1AC2
			public ConditionalEffectContainer(IEnumerable<MPConditionalEffect> conditionalEffects)
				: base(conditionalEffects)
			{
			}

			// Token: 0x06003DDF RID: 15839 RVA: 0x000F38CC File Offset: 0x000F1ACC
			public bool GetState(MPConditionalEffect conditionalEffect, Agent agent)
			{
				ConditionalWeakTable<Agent, MPConditionalEffect.ConditionalEffectContainer.ConditionState> conditionalWeakTable;
				MPConditionalEffect.ConditionalEffectContainer.ConditionState conditionState;
				return this._states != null && this._states.TryGetValue(conditionalEffect, out conditionalWeakTable) && conditionalWeakTable.TryGetValue(agent, out conditionState) && conditionState.IsSatisfied;
			}

			// Token: 0x06003DE0 RID: 15840 RVA: 0x000F3904 File Offset: 0x000F1B04
			public void SetState(MPConditionalEffect conditionalEffect, Agent agent, bool state)
			{
				if (this._states == null)
				{
					this._states = new Dictionary<MPConditionalEffect, ConditionalWeakTable<Agent, MPConditionalEffect.ConditionalEffectContainer.ConditionState>>();
					ConditionalWeakTable<Agent, MPConditionalEffect.ConditionalEffectContainer.ConditionState> conditionalWeakTable = new ConditionalWeakTable<Agent, MPConditionalEffect.ConditionalEffectContainer.ConditionState>();
					conditionalWeakTable.Add(agent, new MPConditionalEffect.ConditionalEffectContainer.ConditionState
					{
						IsSatisfied = state
					});
					this._states.Add(conditionalEffect, conditionalWeakTable);
					return;
				}
				ConditionalWeakTable<Agent, MPConditionalEffect.ConditionalEffectContainer.ConditionState> conditionalWeakTable2;
				if (!this._states.TryGetValue(conditionalEffect, out conditionalWeakTable2))
				{
					conditionalWeakTable2 = new ConditionalWeakTable<Agent, MPConditionalEffect.ConditionalEffectContainer.ConditionState>();
					conditionalWeakTable2.Add(agent, new MPConditionalEffect.ConditionalEffectContainer.ConditionState
					{
						IsSatisfied = state
					});
					this._states.Add(conditionalEffect, conditionalWeakTable2);
					return;
				}
				MPConditionalEffect.ConditionalEffectContainer.ConditionState conditionState;
				if (!conditionalWeakTable2.TryGetValue(agent, out conditionState))
				{
					conditionalWeakTable2.Add(agent, new MPConditionalEffect.ConditionalEffectContainer.ConditionState
					{
						IsSatisfied = state
					});
					return;
				}
				conditionState.IsSatisfied = state;
			}

			// Token: 0x06003DE1 RID: 15841 RVA: 0x000F39A8 File Offset: 0x000F1BA8
			public void ResetStates()
			{
				this._states = null;
			}

			// Token: 0x04002011 RID: 8209
			private Dictionary<MPConditionalEffect, ConditionalWeakTable<Agent, MPConditionalEffect.ConditionalEffectContainer.ConditionState>> _states;

			// Token: 0x02000705 RID: 1797
			private class ConditionState
			{
				// Token: 0x17000A2C RID: 2604
				// (get) Token: 0x06004092 RID: 16530 RVA: 0x000FA1C3 File Offset: 0x000F83C3
				// (set) Token: 0x06004093 RID: 16531 RVA: 0x000FA1CB File Offset: 0x000F83CB
				public bool IsSatisfied { get; set; }
			}
		}
	}
}
