using System;
using System.Collections.Generic;
using System.Linq;
using SandBox.Tournaments.AgentControllers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace SandBox.Tournaments.MissionLogics
{
	// Token: 0x0200001D RID: 29
	public class TournamentJoustingMissionController : MissionLogic, ITournamentGameBehavior
	{
		// Token: 0x14000003 RID: 3
		// (add) Token: 0x0600014D RID: 333 RVA: 0x00009CAC File Offset: 0x00007EAC
		// (remove) Token: 0x0600014E RID: 334 RVA: 0x00009CE4 File Offset: 0x00007EE4
		public event TournamentJoustingMissionController.JoustingEventDelegate VictoryAchieved;

		// Token: 0x14000004 RID: 4
		// (add) Token: 0x0600014F RID: 335 RVA: 0x00009D1C File Offset: 0x00007F1C
		// (remove) Token: 0x06000150 RID: 336 RVA: 0x00009D54 File Offset: 0x00007F54
		public event TournamentJoustingMissionController.JoustingEventDelegate PointGanied;

		// Token: 0x14000005 RID: 5
		// (add) Token: 0x06000151 RID: 337 RVA: 0x00009D8C File Offset: 0x00007F8C
		// (remove) Token: 0x06000152 RID: 338 RVA: 0x00009DC4 File Offset: 0x00007FC4
		public event TournamentJoustingMissionController.JoustingEventDelegate Disqualified;

		// Token: 0x14000006 RID: 6
		// (add) Token: 0x06000153 RID: 339 RVA: 0x00009DFC File Offset: 0x00007FFC
		// (remove) Token: 0x06000154 RID: 340 RVA: 0x00009E34 File Offset: 0x00008034
		public event TournamentJoustingMissionController.JoustingEventDelegate Unconscious;

		// Token: 0x14000007 RID: 7
		// (add) Token: 0x06000155 RID: 341 RVA: 0x00009E6C File Offset: 0x0000806C
		// (remove) Token: 0x06000156 RID: 342 RVA: 0x00009EA4 File Offset: 0x000080A4
		public event TournamentJoustingMissionController.JoustingAgentStateChangedEventDelegate AgentStateChanged;

		// Token: 0x06000157 RID: 343 RVA: 0x00009EDC File Offset: 0x000080DC
		public TournamentJoustingMissionController(CultureObject culture)
		{
			this._culture = culture;
			this._match = null;
			this.RegionBoxList = new List<GameEntity>(2);
			this.RegionExitBoxList = new List<GameEntity>(2);
			this.CornerBackStartList = new List<MatrixFrame>();
			this.CornerStartList = new List<GameEntity>(2);
			this.CornerMiddleList = new List<MatrixFrame>();
			this.CornerFinishList = new List<MatrixFrame>();
			this.IsSwordDuelStarted = false;
			this._joustingEquipment = new Equipment();
			this._joustingEquipment.AddEquipmentToSlotWithoutAgent(10, new EquipmentElement(Game.Current.ObjectManager.GetObject<ItemObject>("charger"), null, null, false));
			this._joustingEquipment.AddEquipmentToSlotWithoutAgent(11, new EquipmentElement(Game.Current.ObjectManager.GetObject<ItemObject>("horse_harness_e"), null, null, false));
			this._joustingEquipment.AddEquipmentToSlotWithoutAgent(0, new EquipmentElement(Game.Current.ObjectManager.GetObject<ItemObject>("vlandia_lance_2_t4"), null, null, false));
			this._joustingEquipment.AddEquipmentToSlotWithoutAgent(1, new EquipmentElement(Game.Current.ObjectManager.GetObject<ItemObject>("leather_round_shield"), null, null, false));
			this._joustingEquipment.AddEquipmentToSlotWithoutAgent(6, new EquipmentElement(Game.Current.ObjectManager.GetObject<ItemObject>("desert_lamellar"), null, null, false));
			this._joustingEquipment.AddEquipmentToSlotWithoutAgent(5, new EquipmentElement(Game.Current.ObjectManager.GetObject<ItemObject>("nasal_helmet_with_mail"), null, null, false));
			this._joustingEquipment.AddEquipmentToSlotWithoutAgent(8, new EquipmentElement(Game.Current.ObjectManager.GetObject<ItemObject>("reinforced_mail_mitten"), null, null, false));
			this._joustingEquipment.AddEquipmentToSlotWithoutAgent(7, new EquipmentElement(Game.Current.ObjectManager.GetObject<ItemObject>("leather_cavalier_boots"), null, null, false));
		}

		// Token: 0x06000158 RID: 344 RVA: 0x0000A098 File Offset: 0x00008298
		public override void AfterStart()
		{
			TournamentBehavior.DeleteTournamentSetsExcept(base.Mission.Scene.FindEntityWithTag("tournament_jousting"));
			for (int i = 0; i < 2; i++)
			{
				GameEntity gameEntity = base.Mission.Scene.FindEntityWithTag("sp_jousting_back_" + i);
				GameEntity gameEntity2 = base.Mission.Scene.FindEntityWithTag("sp_jousting_start_" + i);
				GameEntity gameEntity3 = base.Mission.Scene.FindEntityWithTag("sp_jousting_middle_" + i);
				GameEntity gameEntity4 = base.Mission.Scene.FindEntityWithTag("sp_jousting_finish_" + i);
				this.CornerBackStartList.Add(gameEntity.GetGlobalFrame());
				this.CornerStartList.Add(gameEntity2);
				this.CornerMiddleList.Add(gameEntity3.GetGlobalFrame());
				this.CornerFinishList.Add(gameEntity4.GetGlobalFrame());
			}
			GameEntity gameEntity5 = base.Mission.Scene.FindEntityWithName("region_box_0");
			this.RegionBoxList.Add(gameEntity5);
			GameEntity gameEntity6 = base.Mission.Scene.FindEntityWithName("region_box_1");
			this.RegionBoxList.Add(gameEntity6);
			GameEntity gameEntity7 = base.Mission.Scene.FindEntityWithName("region_end_box_0");
			this.RegionExitBoxList.Add(gameEntity7);
			GameEntity gameEntity8 = base.Mission.Scene.FindEntityWithName("region_end_box_1");
			this.RegionExitBoxList.Add(gameEntity8);
			base.Mission.SetMissionMode(2, true);
		}

		// Token: 0x06000159 RID: 345 RVA: 0x0000A23C File Offset: 0x0000843C
		public void StartMatch(TournamentMatch match, bool isLastRound)
		{
			this._match = match;
			int num = 0;
			foreach (TournamentTeam tournamentTeam in this._match.Teams)
			{
				Team team = base.Mission.Teams.Add(-1, uint.MaxValue, uint.MaxValue, null, true, false, true);
				foreach (TournamentParticipant tournamentParticipant in tournamentTeam.Participants)
				{
					tournamentParticipant.MatchEquipment = this._joustingEquipment.Clone(false);
					this.SetItemsAndSpawnCharacter(tournamentParticipant, team, num);
				}
				num++;
			}
			List<Team> list = base.Mission.Teams.ToList<Team>();
			for (int i = 0; i < list.Count; i++)
			{
				for (int j = i + 1; j < list.Count; j++)
				{
					list[i].SetIsEnemyOf(list[j], true);
				}
			}
			base.Mission.Scene.SetAbilityOfFacesWithId(1, false);
			base.Mission.Scene.SetAbilityOfFacesWithId(2, false);
		}

		// Token: 0x0600015A RID: 346 RVA: 0x0000A37C File Offset: 0x0000857C
		public void SkipMatch(TournamentMatch match)
		{
			this._match = match;
			this.Simulate();
		}

		// Token: 0x0600015B RID: 347 RVA: 0x0000A38C File Offset: 0x0000858C
		public bool IsMatchEnded()
		{
			if (this._isSimulated || this._match == null)
			{
				return true;
			}
			if (this._endTimer != null && this._endTimer.ElapsedTime > 6f)
			{
				this._endTimer = null;
				return true;
			}
			if (this._endTimer == null && this._winnerTeam != null)
			{
				this._endTimer = new BasicMissionTimer();
			}
			return false;
		}

		// Token: 0x0600015C RID: 348 RVA: 0x0000A3EC File Offset: 0x000085EC
		public void OnMatchEnded()
		{
			SandBoxHelpers.MissionHelper.FadeOutAgents(base.Mission.Agents, true, false);
			base.Mission.ClearCorpses(false);
			base.Mission.Teams.Clear();
			base.Mission.RemoveSpawnedItemsAndMissiles();
			this._match = null;
			this._endTimer = null;
			this._isSimulated = false;
		}

		// Token: 0x0600015D RID: 349 RVA: 0x0000A448 File Offset: 0x00008648
		private void Simulate()
		{
			this._isSimulated = false;
			List<TournamentParticipant> participants = this._match.Participants.ToList<TournamentParticipant>();
			while (participants.Count > 1 && participants.Any((TournamentParticipant x) => x.Team != participants[0].Team))
			{
				if (participants.Any((TournamentParticipant x) => x.Score >= 3))
				{
					break;
				}
				TournamentParticipant tournamentParticipant = participants[MBRandom.RandomInt(participants.Count)];
				TournamentParticipant tournamentParticipant2 = participants[MBRandom.RandomInt(participants.Count)];
				while (tournamentParticipant == tournamentParticipant2 || tournamentParticipant.Team == tournamentParticipant2.Team)
				{
					tournamentParticipant2 = participants[MBRandom.RandomInt(participants.Count)];
				}
				tournamentParticipant.AddScore(1);
			}
			this._isSimulated = true;
		}

		// Token: 0x0600015E RID: 350 RVA: 0x0000A548 File Offset: 0x00008748
		private void SetItemsAndSpawnCharacter(TournamentParticipant participant, Team team, int cornerIndex)
		{
			AgentBuildData agentBuildData = new AgentBuildData(new SimpleAgentOrigin(participant.Character, -1, null, participant.Descriptor)).Team(team).InitialFrameFromSpawnPointEntity(this.CornerStartList[cornerIndex]).Equipment(participant.MatchEquipment)
				.Controller(participant.Character.IsPlayerCharacter ? 2 : 1);
			Agent agent = base.Mission.SpawnAgent(agentBuildData, false);
			agent.Health = agent.HealthLimit;
			this.AddJoustingAgentController(agent);
			agent.GetController<JoustingAgentController>().CurrentCornerIndex = cornerIndex;
			if (participant.Character.IsPlayerCharacter)
			{
				agent.WieldInitialWeapons(2);
				base.Mission.PlayerTeam = team;
				return;
			}
			agent.SetWatchState(2);
		}

		// Token: 0x0600015F RID: 351 RVA: 0x0000A5FB File Offset: 0x000087FB
		private void AddJoustingAgentController(Agent agent)
		{
			agent.AddController(typeof(JoustingAgentController));
		}

		// Token: 0x06000160 RID: 352 RVA: 0x0000A610 File Offset: 0x00008810
		public bool IsAgentInTheTrack(Agent agent, bool inCurrentTrack = true)
		{
			bool flag = false;
			if (agent != null)
			{
				JoustingAgentController controller = agent.GetController<JoustingAgentController>();
				int num = (inCurrentTrack ? controller.CurrentCornerIndex : (1 - controller.CurrentCornerIndex));
				flag = this.RegionBoxList[num].CheckPointWithOrientedBoundingBox(agent.Position);
			}
			return flag;
		}

		// Token: 0x06000161 RID: 353 RVA: 0x0000A658 File Offset: 0x00008858
		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			if (!base.Mission.IsMissionEnding)
			{
				foreach (Agent agent in base.Mission.Agents)
				{
					JoustingAgentController controller = agent.GetController<JoustingAgentController>();
					if (controller != null)
					{
						controller.UpdateState();
					}
				}
				this.CheckStartOfSwordDuel();
			}
		}

		// Token: 0x06000162 RID: 354 RVA: 0x0000A6D4 File Offset: 0x000088D4
		private void CheckStartOfSwordDuel()
		{
			if (!base.Mission.IsMissionEnding)
			{
				if (!this.IsSwordDuelStarted)
				{
					if (base.Mission.Agents.Count <= 0)
					{
						return;
					}
					if (base.Mission.Agents.Count((Agent a) => a.IsMount) >= 2)
					{
						return;
					}
					this.IsSwordDuelStarted = true;
					this.RemoveBarriers();
					base.Mission.Scene.SetAbilityOfFacesWithId(2, true);
					using (List<Agent>.Enumerator enumerator = base.Mission.Agents.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							Agent agent = enumerator.Current;
							if (agent.IsHuman)
							{
								JoustingAgentController controller = agent.GetController<JoustingAgentController>();
								controller.State = JoustingAgentController.JoustingAgentState.SwordDuel;
								controller.PrepareAgentToSwordDuel();
							}
						}
						return;
					}
				}
				foreach (Agent agent2 in base.Mission.Agents)
				{
					if (agent2.IsHuman)
					{
						JoustingAgentController controller2 = agent2.GetController<JoustingAgentController>();
						controller2.State = JoustingAgentController.JoustingAgentState.SwordDuel;
						if (controller2.PrepareEquipmentsAfterDismount && agent2.MountAgent == null)
						{
							CharacterObject characterObject = (CharacterObject)agent2.Character;
							controller2.PrepareEquipmentsForSwordDuel();
							agent2.DisableScriptedMovement();
							if (characterObject == CharacterObject.PlayerCharacter)
							{
								agent2.Controller = 2;
							}
						}
					}
				}
			}
		}

		// Token: 0x06000163 RID: 355 RVA: 0x0000A854 File Offset: 0x00008A54
		private void RemoveBarriers()
		{
			foreach (GameEntity gameEntity in base.Mission.Scene.FindEntitiesWithTag("jousting_barrier").ToList<GameEntity>())
			{
				gameEntity.Remove(95);
			}
		}

		// Token: 0x06000164 RID: 356 RVA: 0x0000A8BC File Offset: 0x00008ABC
		public override void OnAgentHit(Agent affectedAgent, Agent affectorAgent, in MissionWeapon attackerWeapon, in Blow blow, in AttackCollisionData attackCollisionData)
		{
			if (!base.Mission.IsMissionEnding && !this.IsSwordDuelStarted && affectedAgent.IsHuman && affectorAgent.IsHuman && affectedAgent != affectorAgent)
			{
				JoustingAgentController controller = affectorAgent.GetController<JoustingAgentController>();
				JoustingAgentController controller2 = affectedAgent.GetController<JoustingAgentController>();
				if (this.IsAgentInTheTrack(affectorAgent, true) && controller2.IsRiding() && controller.IsRiding())
				{
					this._match.GetParticipant(affectorAgent.Origin.UniqueSeed).AddScore(1);
					controller.Score++;
					if (controller.Score >= 3)
					{
						this._winnerTeam = affectorAgent.Team;
						TournamentJoustingMissionController.JoustingEventDelegate victoryAchieved = this.VictoryAchieved;
						if (victoryAchieved == null)
						{
							return;
						}
						victoryAchieved(affectorAgent, affectedAgent);
						return;
					}
					else
					{
						TournamentJoustingMissionController.JoustingEventDelegate pointGanied = this.PointGanied;
						if (pointGanied == null)
						{
							return;
						}
						pointGanied(affectorAgent, affectedAgent);
						return;
					}
				}
				else
				{
					this._match.GetParticipant(affectorAgent.Origin.UniqueSeed).AddScore(-100);
					this._winnerTeam = affectedAgent.Team;
					MBTextManager.SetTextVariable("OPPONENT_GENDER", affectorAgent.Character.IsFemale ? "0" : "1", false);
					TournamentJoustingMissionController.JoustingEventDelegate disqualified = this.Disqualified;
					if (disqualified == null)
					{
						return;
					}
					disqualified(affectorAgent, affectedAgent);
				}
			}
		}

		// Token: 0x06000165 RID: 357 RVA: 0x0000A9F0 File Offset: 0x00008BF0
		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
		{
			if (!base.Mission.IsMissionEnding && affectedAgent.IsHuman && affectorAgent.IsHuman && affectedAgent != affectorAgent)
			{
				if (this.IsAgentInTheTrack(affectorAgent, true) || this.IsSwordDuelStarted)
				{
					this._match.GetParticipant(affectorAgent.Origin.UniqueSeed).AddScore(100);
					this._winnerTeam = affectorAgent.Team;
					if (this.Unconscious != null)
					{
						this.Unconscious(affectorAgent, affectedAgent);
						return;
					}
				}
				else
				{
					this._match.GetParticipant(affectorAgent.Origin.UniqueSeed).AddScore(-100);
					this._winnerTeam = affectedAgent.Team;
					MBTextManager.SetTextVariable("OPPONENT_GENDER", affectorAgent.Character.IsFemale ? "0" : "1", false);
					if (this.Disqualified != null)
					{
						this.Disqualified(affectorAgent, affectedAgent);
					}
				}
			}
		}

		// Token: 0x06000166 RID: 358 RVA: 0x0000AADF File Offset: 0x00008CDF
		public void OnJoustingAgentStateChanged(Agent agent, JoustingAgentController.JoustingAgentState state)
		{
			if (this.AgentStateChanged != null)
			{
				this.AgentStateChanged(agent, state);
			}
		}

		// Token: 0x04000088 RID: 136
		private Team _winnerTeam;

		// Token: 0x04000089 RID: 137
		public List<GameEntity> RegionBoxList;

		// Token: 0x0400008A RID: 138
		public List<GameEntity> RegionExitBoxList;

		// Token: 0x0400008B RID: 139
		public List<MatrixFrame> CornerBackStartList;

		// Token: 0x0400008C RID: 140
		public List<GameEntity> CornerStartList;

		// Token: 0x0400008D RID: 141
		public List<MatrixFrame> CornerMiddleList;

		// Token: 0x0400008E RID: 142
		public List<MatrixFrame> CornerFinishList;

		// Token: 0x0400008F RID: 143
		public bool IsSwordDuelStarted;

		// Token: 0x04000090 RID: 144
		private TournamentMatch _match;

		// Token: 0x04000091 RID: 145
		private BasicMissionTimer _endTimer;

		// Token: 0x04000092 RID: 146
		private bool _isSimulated;

		// Token: 0x04000093 RID: 147
		private CultureObject _culture;

		// Token: 0x04000094 RID: 148
		private readonly Equipment _joustingEquipment;

		// Token: 0x020000FA RID: 250
		// (Invoke) Token: 0x06000C8D RID: 3213
		public delegate void JoustingEventDelegate(Agent affectedAgent, Agent affectorAgent);

		// Token: 0x020000FB RID: 251
		// (Invoke) Token: 0x06000C91 RID: 3217
		public delegate void JoustingAgentStateChangedEventDelegate(Agent agent, JoustingAgentController.JoustingAgentState state);
	}
}
