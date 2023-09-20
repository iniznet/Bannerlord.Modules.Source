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
	public class TournamentArcheryMissionController : MissionLogic, ITournamentGameBehavior
	{
		public IEnumerable<ArcheryTournamentAgentController> AgentControllers
		{
			get
			{
				return this._agentControllers;
			}
		}

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

		public void SkipMatch(TournamentMatch match)
		{
			this._match = match;
			this.Simulate();
		}

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

		private void ResetTargets()
		{
			foreach (DestructableComponent destructableComponent in this._targets)
			{
				destructableComponent.Reset();
			}
		}

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

		public void OnTargetDestroyed(DestructableComponent destroyedComponent, Agent destroyerAgent, in MissionWeapon attackerWeapon, ScriptComponentBehavior attackerScriptComponentBehavior, int inflictedDamage)
		{
			foreach (ArcheryTournamentAgentController archeryTournamentAgentController in this.AgentControllers)
			{
				archeryTournamentAgentController.OnTargetHit(destroyerAgent, destroyedComponent);
				this._match.GetParticipant(destroyerAgent.Origin.UniqueSeed).AddScore(1);
			}
		}

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

		public override void OnAgentHit(Agent affectedAgent, Agent affectorAgent, in MissionWeapon attackerWeapon, in Blow blow, in AttackCollisionData attackCollisionData)
		{
			base.Mission.EndMission();
		}

		private bool IsThereAnyTargetLeft()
		{
			return this._targets.Any((DestructableComponent e) => !e.IsDestroyed);
		}

		private bool IsThereAnyArrowLeft()
		{
			return base.Mission.Agents.Any((Agent agent) => agent.Equipment.GetAmmoAmount(agent.Equipment[0].CurrentUsageItem.AmmoClass) > 0);
		}

		private float GetDeadliness(TournamentParticipant participant)
		{
			return 0.01f + (float)participant.Character.GetSkillValue(DefaultSkills.Bow) / 300f * 0.19f;
		}

		private readonly List<ArcheryTournamentAgentController> _agentControllers;

		private TournamentMatch _match;

		private BasicMissionTimer _endTimer;

		private List<GameEntity> _spawnPoints;

		private bool _isSimulated;

		private CultureObject _culture;

		private List<DestructableComponent> _targets;

		public List<GameEntity> ShootingPositions;

		private readonly Equipment _archeryEquipment;
	}
}
