using System;
using System.Collections.Generic;
using System.Linq;
using SandBox.Missions.MissionLogics;
using SandBox.Tournaments.AgentControllers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.Tournaments.MissionLogics
{
	// Token: 0x0200001A RID: 26
	public class TournamentArcheryMissionController : MissionLogic, ITournamentGameBehavior
	{
		// Token: 0x1700000E RID: 14
		// (get) Token: 0x060000F2 RID: 242 RVA: 0x00007698 File Offset: 0x00005898
		public IEnumerable<ArcheryTournamentAgentController> AgentControllers
		{
			get
			{
				return this._agentControllers;
			}
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x000076A0 File Offset: 0x000058A0
		public TournamentArcheryMissionController(CultureObject culture)
		{
			this._culture = culture;
			this.ShootingPositions = new List<GameEntity>();
			this._agentControllers = new List<ArcheryTournamentAgentController>();
			this._archeryEquipment = new Equipment();
			this._archeryEquipment.AddEquipmentToSlotWithoutAgent(0, new EquipmentElement(Game.Current.ObjectManager.GetObject<ItemObject>("nordic_shortbow"), null, null, false));
			this._archeryEquipment.AddEquipmentToSlotWithoutAgent(1, new EquipmentElement(Game.Current.ObjectManager.GetObject<ItemObject>("blunt_arrows"), null, null, false));
			this._archeryEquipment.AddEquipmentToSlotWithoutAgent(6, new EquipmentElement(Game.Current.ObjectManager.GetObject<ItemObject>("desert_lamellar"), null, null, false));
			this._archeryEquipment.AddEquipmentToSlotWithoutAgent(8, new EquipmentElement(Game.Current.ObjectManager.GetObject<ItemObject>("reinforced_mail_mitten"), null, null, false));
			this._archeryEquipment.AddEquipmentToSlotWithoutAgent(7, new EquipmentElement(Game.Current.ObjectManager.GetObject<ItemObject>("leather_cavalier_boots"), null, null, false));
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x000077A4 File Offset: 0x000059A4
		public override void AfterStart()
		{
			TournamentBehavior.DeleteTournamentSetsExcept(base.Mission.Scene.FindEntityWithTag("tournament_archery"));
			this._spawnPoints = base.Mission.Scene.FindEntitiesWithTag("sp_arena").ToList<GameEntity>();
			base.Mission.SetMissionMode(2, true);
			this._targets = (from x in MBExtensions.FindAllWithType<DestructableComponent>(base.Mission.ActiveMissionObjects)
				where x.GameEntity.HasTag("archery_target")
				select x).ToList<DestructableComponent>();
			foreach (DestructableComponent destructableComponent in this._targets)
			{
				destructableComponent.OnDestroyed += new DestructableComponent.OnHitTakenAndDestroyedDelegate(this.OnTargetDestroyed);
			}
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x00007888 File Offset: 0x00005A88
		public void StartMatch(TournamentMatch match, bool isLastRound)
		{
			this._match = match;
			this.ResetTargets();
			int count = this._spawnPoints.Count;
			int num = 0;
			int num2 = 0;
			foreach (TournamentTeam tournamentTeam in this._match.Teams)
			{
				Team team = base.Mission.Teams.Add(-1, MissionAgentHandler.GetRandomTournamentTeamColor(num2), uint.MaxValue, null, true, false, true);
				foreach (TournamentParticipant tournamentParticipant in tournamentTeam.Participants)
				{
					tournamentParticipant.MatchEquipment = this._archeryEquipment.Clone(false);
					MatrixFrame globalFrame = this._spawnPoints[num % count].GetGlobalFrame();
					globalFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
					this.SetItemsAndSpawnCharacter(tournamentParticipant, team, globalFrame);
					num++;
				}
				num2++;
			}
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x00007998 File Offset: 0x00005B98
		public void SkipMatch(TournamentMatch match)
		{
			this._match = match;
			this.Simulate();
		}

		// Token: 0x060000F7 RID: 247 RVA: 0x000079A8 File Offset: 0x00005BA8
		private void Simulate()
		{
			this._isSimulated = false;
			List<TournamentParticipant> list = this._match.Participants.ToList<TournamentParticipant>();
			int i = this._targets.Count;
			while (i > 0)
			{
				foreach (TournamentParticipant tournamentParticipant in list)
				{
					if (i == 0)
					{
						break;
					}
					if (MBRandom.RandomFloat < this.GetDeadliness(tournamentParticipant))
					{
						tournamentParticipant.AddScore(1);
						i--;
					}
				}
			}
			this._isSimulated = true;
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x00007A40 File Offset: 0x00005C40
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
			if (this._endTimer == null && (!this.IsThereAnyTargetLeft() || !this.IsThereAnyArrowLeft()))
			{
				this._endTimer = new BasicMissionTimer();
			}
			return false;
		}

		// Token: 0x060000F9 RID: 249 RVA: 0x00007AA8 File Offset: 0x00005CA8
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

		// Token: 0x060000FA RID: 250 RVA: 0x00007B04 File Offset: 0x00005D04
		private void ResetTargets()
		{
			foreach (DestructableComponent destructableComponent in this._targets)
			{
				destructableComponent.Reset();
			}
		}

		// Token: 0x060000FB RID: 251 RVA: 0x00007B54 File Offset: 0x00005D54
		private void SetItemsAndSpawnCharacter(TournamentParticipant participant, Team team, MatrixFrame frame)
		{
			AgentBuildData agentBuildData = new AgentBuildData(new SimpleAgentOrigin(participant.Character, -1, null, participant.Descriptor)).Team(team).Equipment(participant.MatchEquipment).InitialPosition(ref frame.origin);
			Vec2 vec = frame.rotation.f.AsVec2;
			vec = vec.Normalized();
			AgentBuildData agentBuildData2 = agentBuildData.InitialDirection(ref vec).Controller(participant.Character.IsPlayerCharacter ? 2 : 1);
			Agent agent = base.Mission.SpawnAgent(agentBuildData2, false);
			agent.Health = agent.HealthLimit;
			ArcheryTournamentAgentController archeryTournamentAgentController = agent.AddController(typeof(ArcheryTournamentAgentController)) as ArcheryTournamentAgentController;
			archeryTournamentAgentController.SetTargets(this._targets);
			this._agentControllers.Add(archeryTournamentAgentController);
			if (participant.Character.IsPlayerCharacter)
			{
				agent.WieldInitialWeapons(2);
				base.Mission.PlayerTeam = team;
				return;
			}
			agent.SetWatchState(2);
		}

		// Token: 0x060000FC RID: 252 RVA: 0x00007C40 File Offset: 0x00005E40
		public void OnTargetDestroyed(DestructableComponent destroyedComponent, Agent destroyerAgent, in MissionWeapon attackerWeapon, ScriptComponentBehavior attackerScriptComponentBehavior, int inflictedDamage)
		{
			foreach (ArcheryTournamentAgentController archeryTournamentAgentController in this.AgentControllers)
			{
				archeryTournamentAgentController.OnTargetHit(destroyerAgent, destroyedComponent);
				this._match.GetParticipant(destroyerAgent.Origin.UniqueSeed).AddScore(1);
			}
		}

		// Token: 0x060000FD RID: 253 RVA: 0x00007CAC File Offset: 0x00005EAC
		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			if (!this.IsMatchEnded())
			{
				foreach (Agent agent in base.Mission.Agents)
				{
					ArcheryTournamentAgentController controller = agent.GetController<ArcheryTournamentAgentController>();
					if (controller != null)
					{
						controller.OnTick();
					}
				}
			}
		}

		// Token: 0x060000FE RID: 254 RVA: 0x00007D1C File Offset: 0x00005F1C
		public override void OnAgentHit(Agent affectedAgent, Agent affectorAgent, in MissionWeapon attackerWeapon, in Blow blow, in AttackCollisionData attackCollisionData)
		{
			base.Mission.EndMission();
		}

		// Token: 0x060000FF RID: 255 RVA: 0x00007D29 File Offset: 0x00005F29
		private bool IsThereAnyTargetLeft()
		{
			return this._targets.Any((DestructableComponent e) => !e.IsDestroyed);
		}

		// Token: 0x06000100 RID: 256 RVA: 0x00007D55 File Offset: 0x00005F55
		private bool IsThereAnyArrowLeft()
		{
			return base.Mission.Agents.Any((Agent agent) => agent.Equipment.GetAmmoAmount(agent.Equipment[0].CurrentUsageItem.AmmoClass) > 0);
		}

		// Token: 0x06000101 RID: 257 RVA: 0x00007D86 File Offset: 0x00005F86
		private float GetDeadliness(TournamentParticipant participant)
		{
			return 0.01f + (float)participant.Character.GetSkillValue(DefaultSkills.Bow) / 300f * 0.19f;
		}

		// Token: 0x04000054 RID: 84
		private readonly List<ArcheryTournamentAgentController> _agentControllers;

		// Token: 0x04000055 RID: 85
		private TournamentMatch _match;

		// Token: 0x04000056 RID: 86
		private BasicMissionTimer _endTimer;

		// Token: 0x04000057 RID: 87
		private List<GameEntity> _spawnPoints;

		// Token: 0x04000058 RID: 88
		private bool _isSimulated;

		// Token: 0x04000059 RID: 89
		private CultureObject _culture;

		// Token: 0x0400005A RID: 90
		private List<DestructableComponent> _targets;

		// Token: 0x0400005B RID: 91
		public List<GameEntity> ShootingPositions;

		// Token: 0x0400005C RID: 92
		private readonly Equipment _archeryEquipment;
	}
}
