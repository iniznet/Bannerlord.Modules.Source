using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;

namespace TaleWorlds.MountAndBlade
{
	public class AgentVictoryLogic : MissionLogic
	{
		public AgentVictoryLogic.CheerActionGroupEnum CheerActionGroup
		{
			get
			{
				return this._cheerActionGroup;
			}
		}

		public AgentVictoryLogic.CheerReactionTimeSettings CheerReactionTimerData
		{
			get
			{
				return this._cheerReactionTimerData;
			}
		}

		public override void AfterStart()
		{
			base.Mission.MissionCloseTimeAfterFinish = 60f;
			this._cheeringAgents = new List<AgentVictoryLogic.CheeringAgent>();
			this.SetCheerReactionTimerSettings(1f, 8f);
			if (base.Mission.PlayerTeam != null)
			{
				base.Mission.PlayerTeam.PlayerOrderController.OnOrderIssued += new OnOrderIssuedDelegate(this.MasterOrderControllerOnOrderIssued);
			}
			Mission.Current.IsBattleInRetreatEvent += this.CheckIfIsInRetreat;
		}

		private void MasterOrderControllerOnOrderIssued(OrderType orderType, IEnumerable<Formation> appliedFormations, object[] delegateparams)
		{
			MBList<Formation> mblist = appliedFormations.ToMBList<Formation>();
			for (int i = this._cheeringAgents.Count - 1; i >= 0; i--)
			{
				Agent agent = this._cheeringAgents[i].Agent;
				if (mblist.Contains(agent.Formation))
				{
					this._cheeringAgents[i].OrderReceived();
				}
			}
		}

		public void SetCheerActionGroup(AgentVictoryLogic.CheerActionGroupEnum cheerActionGroup = AgentVictoryLogic.CheerActionGroupEnum.None)
		{
			this._cheerActionGroup = cheerActionGroup;
			switch (this._cheerActionGroup)
			{
			case AgentVictoryLogic.CheerActionGroupEnum.LowCheerActions:
				this._selectedCheerActions = this._lowCheerActions;
				return;
			case AgentVictoryLogic.CheerActionGroupEnum.MidCheerActions:
				this._selectedCheerActions = this._midCheerActions;
				return;
			case AgentVictoryLogic.CheerActionGroupEnum.HighCheerActions:
				this._selectedCheerActions = this._highCheerActions;
				return;
			default:
				this._selectedCheerActions = null;
				return;
			}
		}

		public void SetCheerReactionTimerSettings(float minDuration = 1f, float maxDuration = 8f)
		{
			this._cheerReactionTimerData = new AgentVictoryLogic.CheerReactionTimeSettings(minDuration, maxDuration);
		}

		public override void OnClearScene()
		{
			this._cheeringAgents.Clear();
		}

		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
		{
			VictoryComponent component = affectedAgent.GetComponent<VictoryComponent>();
			if (component != null)
			{
				affectedAgent.RemoveComponent(component);
			}
			for (int i = 0; i < this._cheeringAgents.Count; i++)
			{
				if (this._cheeringAgents[i].Agent == affectedAgent)
				{
					this._cheeringAgents.RemoveAt(i);
					return;
				}
			}
		}

		protected override void OnEndMission()
		{
			Mission.Current.IsBattleInRetreatEvent -= this.CheckIfIsInRetreat;
		}

		public override void OnMissionTick(float dt)
		{
			if (this._cheeringAgents.Count > 0)
			{
				this.CheckAnimationAndVoice();
			}
		}

		private void CheckAnimationAndVoice()
		{
			for (int i = this._cheeringAgents.Count - 1; i >= 0; i--)
			{
				Agent agent = this._cheeringAgents[i].Agent;
				bool gotOrderRecently = this._cheeringAgents[i].GotOrderRecently;
				bool isCheeringOnRetreat = this._cheeringAgents[i].IsCheeringOnRetreat;
				VictoryComponent component = agent.GetComponent<VictoryComponent>();
				if (component != null)
				{
					if (this.CheckIfIsInRetreat() && gotOrderRecently)
					{
						agent.RemoveComponent(component);
						agent.SetActionChannel(1, ActionIndexCache.act_none, false, (ulong)agent.GetCurrentActionPriority(1), 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
						if (MBRandom.RandomFloat > 0.25f)
						{
							agent.MakeVoice(SkinVoiceManager.VoiceType.Yell, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
						}
						if (isCheeringOnRetreat)
						{
							agent.ClearTargetFrame();
						}
						this._cheeringAgents.RemoveAt(i);
					}
					else if (component.CheckTimer())
					{
						if (!agent.IsActive())
						{
							Debug.FailedAssert("Agent trying to cheer without being active", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\MissionLogics\\AgentVictoryLogic.cs", "CheckAnimationAndVoice", 230);
							Debug.Print("Agent trying to cheer without being active", 0, Debug.DebugColor.White, 17592186044416UL);
						}
						bool flag;
						this.ChooseWeaponToCheerWithCheerAndUpdateTimer(agent, out flag);
						if (flag)
						{
							component.ChangeTimerDuration(6f, 12f);
						}
					}
				}
			}
		}

		private void SelectVictoryCondition(BattleSideEnum side)
		{
			if (this._cheerActionGroup == AgentVictoryLogic.CheerActionGroupEnum.None)
			{
				BattleObserverMissionLogic missionBehavior = Mission.Current.GetMissionBehavior<BattleObserverMissionLogic>();
				if (missionBehavior != null)
				{
					float deathToBuiltAgentRatioForSide = missionBehavior.GetDeathToBuiltAgentRatioForSide(side);
					if (deathToBuiltAgentRatioForSide < 0.25f)
					{
						this.SetCheerActionGroup(AgentVictoryLogic.CheerActionGroupEnum.HighCheerActions);
						return;
					}
					if (deathToBuiltAgentRatioForSide < 0.75f)
					{
						this.SetCheerActionGroup(AgentVictoryLogic.CheerActionGroupEnum.MidCheerActions);
						return;
					}
					this.SetCheerActionGroup(AgentVictoryLogic.CheerActionGroupEnum.LowCheerActions);
					return;
				}
				else
				{
					this.SetCheerActionGroup(AgentVictoryLogic.CheerActionGroupEnum.MidCheerActions);
				}
			}
		}

		public void SetTimersOfVictoryReactionsOnBattleEnd(BattleSideEnum side)
		{
			this._isInRetreat = false;
			this.SelectVictoryCondition(side);
			foreach (Team team in base.Mission.Teams)
			{
				if (team.Side == side)
				{
					foreach (Formation formation in team.FormationsIncludingSpecialAndEmpty)
					{
						if (formation.CountOfUnits > 0)
						{
							formation.SetMovementOrder(MovementOrder.MovementOrderStop);
						}
					}
				}
			}
			using (List<Agent>.Enumerator enumerator3 = base.Mission.Agents.GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					Agent agent = enumerator3.Current;
					if (agent.IsHuman && agent.IsAIControlled && agent.Team != null && side == agent.Team.Side && agent.CurrentWatchState == Agent.WatchState.Alarmed && agent.GetComponent<VictoryComponent>() == null)
					{
						if (this._cheeringAgents.AnyQ((AgentVictoryLogic.CheeringAgent a) => a.Agent == agent))
						{
							Debug.FailedAssert("Adding a duplicate agent in _cheeringAgents", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\MissionLogics\\AgentVictoryLogic.cs", "SetTimersOfVictoryReactionsOnBattleEnd", 304);
							Debug.Print("Adding a duplicate agent in _cheeringAgents", 0, Debug.DebugColor.White, 17592186044416UL);
						}
						agent.AddComponent(new VictoryComponent(agent, new RandomTimer(base.Mission.CurrentTime, this._cheerReactionTimerData.MinDuration, this._cheerReactionTimerData.MaxDuration)));
						this._cheeringAgents.Add(new AgentVictoryLogic.CheeringAgent(agent, false));
					}
				}
			}
		}

		public void SetTimersOfVictoryReactionsOnRetreat(BattleSideEnum side)
		{
			this._isInRetreat = true;
			this.SelectVictoryCondition(side);
			List<Agent> list = base.Mission.Agents.Where((Agent agent) => agent.IsHuman && agent.IsAIControlled && agent.Team.Side == side).ToList<Agent>();
			int num = (int)((float)list.Count * 0.5f);
			List<Agent> list2 = new List<Agent>();
			int num2 = 0;
			while (num2 < list.Count && list2.Count != num)
			{
				Agent agent3 = list[num2];
				int num3 = list.Count - num2;
				int num4 = num - list2.Count;
				int num5 = num3 - num4;
				float num6 = MBMath.ClampFloat((float)(num - num5) / (float)num, 0f, 1f);
				float num7;
				Vec3 vec;
				if (num6 < 1f && agent3.TryGetImmediateEnemyAgentMovementData(out num7, out vec))
				{
					float maximumForwardUnlimitedSpeed = agent3.MaximumForwardUnlimitedSpeed;
					float num8 = num7;
					if (maximumForwardUnlimitedSpeed > num8)
					{
						float num9 = (agent3.Position - vec).LengthSquared / (maximumForwardUnlimitedSpeed - num8);
						if (num9 < 900f)
						{
							float num10 = num6 - -1f;
							float num11 = num9 / 900f;
							num6 = -1f + num10 * num11;
						}
					}
				}
				if (MBRandom.RandomFloat <= 0.5f + 0.5f * num6)
				{
					list2.Add(agent3);
				}
				num2++;
			}
			foreach (Agent agent2 in list2)
			{
				MatrixFrame frame = agent2.Frame;
				Vec2 asVec = frame.origin.AsVec2;
				Vec3 f = frame.rotation.f;
				agent2.SetTargetPositionAndDirectionSynched(ref asVec, ref f);
				this.SetTimersOfVictoryReactionsForSingleAgent(agent2, this._cheerReactionTimerData.MinDuration, this._cheerReactionTimerData.MaxDuration, true);
			}
		}

		public void SetTimersOfVictoryReactionsOnTournamentVictoryForAgent(Agent agent, float minStartTime, float maxStartTime)
		{
			this._selectedCheerActions = this._midCheerActions;
			this.SetTimersOfVictoryReactionsForSingleAgent(agent, minStartTime, maxStartTime, false);
		}

		private void SetTimersOfVictoryReactionsForSingleAgent(Agent agent, float minStartTime, float maxStartTime, bool isCheeringOnRetreat)
		{
			if (agent.IsActive() && agent.IsHuman && agent.IsAIControlled)
			{
				if (this._cheeringAgents.AnyQ((AgentVictoryLogic.CheeringAgent a) => a.Agent == agent))
				{
					Debug.FailedAssert("Adding a duplicate agent in _cheeringAgents", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\MissionLogics\\AgentVictoryLogic.cs", "SetTimersOfVictoryReactionsForSingleAgent", 393);
					Debug.Print("Adding a duplicate agent in _cheeringAgents", 0, Debug.DebugColor.White, 17592186044416UL);
				}
				agent.AddComponent(new VictoryComponent(agent, new RandomTimer(base.Mission.CurrentTime, minStartTime, maxStartTime)));
				this._cheeringAgents.Add(new AgentVictoryLogic.CheeringAgent(agent, isCheeringOnRetreat));
			}
		}

		private void ChooseWeaponToCheerWithCheerAndUpdateTimer(Agent cheerAgent, out bool resetTimer)
		{
			resetTimer = false;
			if (cheerAgent.GetCurrentActionType(1) != Agent.ActionCodeType.EquipUnequip)
			{
				EquipmentIndex wieldedItemIndex = cheerAgent.GetWieldedItemIndex(Agent.HandIndex.MainHand);
				bool flag = wieldedItemIndex != EquipmentIndex.None && !cheerAgent.Equipment[wieldedItemIndex].Item.ItemFlags.HasAnyFlag(ItemFlags.DropOnAnyAction);
				if (!flag)
				{
					EquipmentIndex equipmentIndex = EquipmentIndex.None;
					for (EquipmentIndex equipmentIndex2 = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex2 < EquipmentIndex.ExtraWeaponSlot; equipmentIndex2++)
					{
						if (!cheerAgent.Equipment[equipmentIndex2].IsEmpty && !cheerAgent.Equipment[equipmentIndex2].Item.ItemFlags.HasAnyFlag(ItemFlags.DropOnAnyAction))
						{
							equipmentIndex = equipmentIndex2;
							break;
						}
					}
					if (equipmentIndex == EquipmentIndex.None)
					{
						if (wieldedItemIndex != EquipmentIndex.None)
						{
							cheerAgent.TryToSheathWeaponInHand(Agent.HandIndex.MainHand, Agent.WeaponWieldActionType.WithAnimation);
						}
						else
						{
							flag = true;
						}
					}
					else
					{
						cheerAgent.TryToWieldWeaponInSlot(equipmentIndex, Agent.WeaponWieldActionType.WithAnimation, false);
					}
				}
				if (flag)
				{
					ActionIndexCache[] array = this._selectedCheerActions;
					if (cheerAgent.HasMount)
					{
						array = this._midCheerActions;
					}
					cheerAgent.SetActionChannel(1, array[MBRandom.RandomInt(array.Length)], false, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
					cheerAgent.MakeVoice(SkinVoiceManager.VoiceType.Victory, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
					resetTimer = true;
				}
			}
		}

		private bool CheckIfIsInRetreat()
		{
			return this._isInRetreat;
		}

		private const float HighCheerThreshold = 0.25f;

		private const float MidCheerThreshold = 0.75f;

		private const float YellOnCheerCancelProbability = 0.25f;

		private AgentVictoryLogic.CheerActionGroupEnum _cheerActionGroup;

		private AgentVictoryLogic.CheerReactionTimeSettings _cheerReactionTimerData;

		private readonly ActionIndexCache[] _lowCheerActions = new ActionIndexCache[]
		{
			ActionIndexCache.Create("act_cheering_low_01"),
			ActionIndexCache.Create("act_cheering_low_02"),
			ActionIndexCache.Create("act_cheering_low_03"),
			ActionIndexCache.Create("act_cheering_low_04"),
			ActionIndexCache.Create("act_cheering_low_05"),
			ActionIndexCache.Create("act_cheering_low_06"),
			ActionIndexCache.Create("act_cheering_low_07"),
			ActionIndexCache.Create("act_cheering_low_08"),
			ActionIndexCache.Create("act_cheering_low_09"),
			ActionIndexCache.Create("act_cheering_low_10")
		};

		private readonly ActionIndexCache[] _midCheerActions = new ActionIndexCache[]
		{
			ActionIndexCache.Create("act_cheer_1"),
			ActionIndexCache.Create("act_cheer_2"),
			ActionIndexCache.Create("act_cheer_3"),
			ActionIndexCache.Create("act_cheer_4")
		};

		private readonly ActionIndexCache[] _highCheerActions = new ActionIndexCache[]
		{
			ActionIndexCache.Create("act_cheering_high_01"),
			ActionIndexCache.Create("act_cheering_high_02"),
			ActionIndexCache.Create("act_cheering_high_03"),
			ActionIndexCache.Create("act_cheering_high_04"),
			ActionIndexCache.Create("act_cheering_high_05"),
			ActionIndexCache.Create("act_cheering_high_06"),
			ActionIndexCache.Create("act_cheering_high_07"),
			ActionIndexCache.Create("act_cheering_high_08")
		};

		private ActionIndexCache[] _selectedCheerActions;

		private List<AgentVictoryLogic.CheeringAgent> _cheeringAgents;

		private bool _isInRetreat;

		public enum CheerActionGroupEnum
		{
			None,
			LowCheerActions,
			MidCheerActions,
			HighCheerActions
		}

		public struct CheerReactionTimeSettings
		{
			public CheerReactionTimeSettings(float minDuration, float maxDuration)
			{
				this.MinDuration = minDuration;
				this.MaxDuration = maxDuration;
			}

			public readonly float MinDuration;

			public readonly float MaxDuration;
		}

		private class CheeringAgent
		{
			public bool GotOrderRecently { get; private set; }

			public CheeringAgent(Agent agent, bool isCheeringOnRetreat)
			{
				this.Agent = agent;
				this.IsCheeringOnRetreat = isCheeringOnRetreat;
			}

			public void OrderReceived()
			{
				this.GotOrderRecently = true;
			}

			public readonly Agent Agent;

			public readonly bool IsCheeringOnRetreat;
		}
	}
}
