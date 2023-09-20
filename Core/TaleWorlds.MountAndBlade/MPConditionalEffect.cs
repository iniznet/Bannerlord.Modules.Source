using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class MPConditionalEffect
	{
		public MBReadOnlyList<MPPerkCondition> Conditions { get; }

		public MBReadOnlyList<MPPerkEffectBase> Effects { get; }

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

		public class ConditionalEffectContainer : List<MPConditionalEffect>
		{
			public ConditionalEffectContainer()
			{
			}

			public ConditionalEffectContainer(IEnumerable<MPConditionalEffect> conditionalEffects)
				: base(conditionalEffects)
			{
			}

			public bool GetState(MPConditionalEffect conditionalEffect, Agent agent)
			{
				ConditionalWeakTable<Agent, MPConditionalEffect.ConditionalEffectContainer.ConditionState> conditionalWeakTable;
				MPConditionalEffect.ConditionalEffectContainer.ConditionState conditionState;
				return this._states != null && this._states.TryGetValue(conditionalEffect, out conditionalWeakTable) && conditionalWeakTable.TryGetValue(agent, out conditionState) && conditionState.IsSatisfied;
			}

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

			public void ResetStates()
			{
				this._states = null;
			}

			private Dictionary<MPConditionalEffect, ConditionalWeakTable<Agent, MPConditionalEffect.ConditionalEffectContainer.ConditionState>> _states;

			private class ConditionState
			{
				public bool IsSatisfied { get; set; }
			}
		}
	}
}
