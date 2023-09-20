using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000258 RID: 600
	public class AgentVictoryLogic : MissionLogic
	{
		// Token: 0x17000646 RID: 1606
		// (get) Token: 0x06002060 RID: 8288 RVA: 0x00073084 File Offset: 0x00071284
		public AgentVictoryLogic.CheerActionGroupEnum CheerActionGroup
		{
			get
			{
				return this._cheerActionGroup;
			}
		}

		// Token: 0x17000647 RID: 1607
		// (get) Token: 0x06002061 RID: 8289 RVA: 0x0007308C File Offset: 0x0007128C
		public AgentVictoryLogic.CheerReactionTimeSettings CheerReactionTimerData
		{
			get
			{
				return this._cheerReactionTimerData;
			}
		}

		// Token: 0x06002062 RID: 8290 RVA: 0x00073094 File Offset: 0x00071294
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

		// Token: 0x06002063 RID: 8291 RVA: 0x00073110 File Offset: 0x00071310
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

		// Token: 0x06002064 RID: 8292 RVA: 0x00073170 File Offset: 0x00071370
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

		// Token: 0x06002065 RID: 8293 RVA: 0x000731CF File Offset: 0x000713CF
		public void SetCheerReactionTimerSettings(float minDuration = 1f, float maxDuration = 8f)
		{
			this._cheerReactionTimerData = new AgentVictoryLogic.CheerReactionTimeSettings(minDuration, maxDuration);
		}

		// Token: 0x06002066 RID: 8294 RVA: 0x000731DE File Offset: 0x000713DE
		public override void OnClearScene()
		{
			this._cheeringAgents.Clear();
		}

		// Token: 0x06002067 RID: 8295 RVA: 0x000731EC File Offset: 0x000713EC
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

		// Token: 0x06002068 RID: 8296 RVA: 0x00073242 File Offset: 0x00071442
		protected override void OnEndMission()
		{
			Mission.Current.IsBattleInRetreatEvent -= this.CheckIfIsInRetreat;
		}

		// Token: 0x06002069 RID: 8297 RVA: 0x0007325A File Offset: 0x0007145A
		public override void OnMissionTick(float dt)
		{
			if (this._cheeringAgents.Count > 0)
			{
				this.CheckAnimationAndVoice();
			}
		}

		// Token: 0x0600206A RID: 8298 RVA: 0x00073270 File Offset: 0x00071470
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

		// Token: 0x0600206B RID: 8299 RVA: 0x000733B8 File Offset: 0x000715B8
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

		// Token: 0x0600206C RID: 8300 RVA: 0x00073414 File Offset: 0x00071614
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

		// Token: 0x0600206D RID: 8301 RVA: 0x00073650 File Offset: 0x00071850
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

		// Token: 0x0600206E RID: 8302 RVA: 0x00073830 File Offset: 0x00071A30
		public void SetTimersOfVictoryReactionsOnTournamentVictoryForAgent(Agent agent, float minStartTime, float maxStartTime)
		{
			this._selectedCheerActions = this._midCheerActions;
			this.SetTimersOfVictoryReactionsForSingleAgent(agent, minStartTime, maxStartTime, false);
		}

		// Token: 0x0600206F RID: 8303 RVA: 0x00073848 File Offset: 0x00071A48
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

		// Token: 0x06002070 RID: 8304 RVA: 0x0007391C File Offset: 0x00071B1C
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

		// Token: 0x06002071 RID: 8305 RVA: 0x00073A4E File Offset: 0x00071C4E
		private bool CheckIfIsInRetreat()
		{
			return this._isInRetreat;
		}

		// Token: 0x04000BDE RID: 3038
		private const float HighCheerThreshold = 0.25f;

		// Token: 0x04000BDF RID: 3039
		private const float MidCheerThreshold = 0.75f;

		// Token: 0x04000BE0 RID: 3040
		private const float YellOnCheerCancelProbability = 0.25f;

		// Token: 0x04000BE1 RID: 3041
		private AgentVictoryLogic.CheerActionGroupEnum _cheerActionGroup;

		// Token: 0x04000BE2 RID: 3042
		private AgentVictoryLogic.CheerReactionTimeSettings _cheerReactionTimerData;

		// Token: 0x04000BE3 RID: 3043
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

		// Token: 0x04000BE4 RID: 3044
		private readonly ActionIndexCache[] _midCheerActions = new ActionIndexCache[]
		{
			ActionIndexCache.Create("act_cheer_1"),
			ActionIndexCache.Create("act_cheer_2"),
			ActionIndexCache.Create("act_cheer_3"),
			ActionIndexCache.Create("act_cheer_4")
		};

		// Token: 0x04000BE5 RID: 3045
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

		// Token: 0x04000BE6 RID: 3046
		private ActionIndexCache[] _selectedCheerActions;

		// Token: 0x04000BE7 RID: 3047
		private List<AgentVictoryLogic.CheeringAgent> _cheeringAgents;

		// Token: 0x04000BE8 RID: 3048
		private bool _isInRetreat;

		// Token: 0x0200055E RID: 1374
		public enum CheerActionGroupEnum
		{
			// Token: 0x04001CE8 RID: 7400
			None,
			// Token: 0x04001CE9 RID: 7401
			LowCheerActions,
			// Token: 0x04001CEA RID: 7402
			MidCheerActions,
			// Token: 0x04001CEB RID: 7403
			HighCheerActions
		}

		// Token: 0x0200055F RID: 1375
		public struct CheerReactionTimeSettings
		{
			// Token: 0x06003A4C RID: 14924 RVA: 0x000EB97F File Offset: 0x000E9B7F
			public CheerReactionTimeSettings(float minDuration, float maxDuration)
			{
				this.MinDuration = minDuration;
				this.MaxDuration = maxDuration;
			}

			// Token: 0x04001CEC RID: 7404
			public readonly float MinDuration;

			// Token: 0x04001CED RID: 7405
			public readonly float MaxDuration;
		}

		// Token: 0x02000560 RID: 1376
		private class CheeringAgent
		{
			// Token: 0x1700097D RID: 2429
			// (get) Token: 0x06003A4D RID: 14925 RVA: 0x000EB98F File Offset: 0x000E9B8F
			// (set) Token: 0x06003A4E RID: 14926 RVA: 0x000EB997 File Offset: 0x000E9B97
			public bool GotOrderRecently { get; private set; }

			// Token: 0x06003A4F RID: 14927 RVA: 0x000EB9A0 File Offset: 0x000E9BA0
			public CheeringAgent(Agent agent, bool isCheeringOnRetreat)
			{
				this.Agent = agent;
				this.IsCheeringOnRetreat = isCheeringOnRetreat;
			}

			// Token: 0x06003A50 RID: 14928 RVA: 0x000EB9B6 File Offset: 0x000E9BB6
			public void OrderReceived()
			{
				this.GotOrderRecently = true;
			}

			// Token: 0x04001CEE RID: 7406
			public readonly Agent Agent;

			// Token: 0x04001CEF RID: 7407
			public readonly bool IsCheeringOnRetreat;
		}
	}
}
