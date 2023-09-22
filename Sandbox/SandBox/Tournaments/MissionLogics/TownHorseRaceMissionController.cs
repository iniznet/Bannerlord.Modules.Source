using System;
using System.Collections.Generic;
using System.Linq;
using SandBox.Tournaments.AgentControllers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.Tournaments.MissionLogics
{
	public class TownHorseRaceMissionController : MissionLogic, ITournamentGameBehavior
	{
		public List<TownHorseRaceMissionController.CheckPoint> CheckPoints { get; private set; }

		public TownHorseRaceMissionController(CultureObject culture)
		{
			this._culture = culture;
			this._agents = new List<TownHorseRaceAgentController>();
		}

		public override void AfterStart()
		{
			base.AfterStart();
			this.CollectCheckPointsAndStartPoints();
			foreach (TownHorseRaceAgentController townHorseRaceAgentController in this._agents)
			{
				townHorseRaceAgentController.DisableMovement();
			}
			this._startTimer = new BasicMissionTimer();
		}

		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			if (this._startTimer != null && this._startTimer.ElapsedTime > 3f)
			{
				foreach (TownHorseRaceAgentController townHorseRaceAgentController in this._agents)
				{
					townHorseRaceAgentController.Start();
				}
			}
		}

		private void CollectCheckPointsAndStartPoints()
		{
			this.CheckPoints = new List<TownHorseRaceMissionController.CheckPoint>();
			foreach (GameEntity gameEntity in base.Mission.ActiveMissionObjects.Select((MissionObject amo) => amo.GameEntity))
			{
				VolumeBox firstScriptOfType = gameEntity.GetFirstScriptOfType<VolumeBox>();
				if (firstScriptOfType != null)
				{
					this.CheckPoints.Add(new TownHorseRaceMissionController.CheckPoint(firstScriptOfType));
				}
			}
			this.CheckPoints = this.CheckPoints.OrderBy((TownHorseRaceMissionController.CheckPoint x) => x.Name).ToList<TownHorseRaceMissionController.CheckPoint>();
			this._startPoints = base.Mission.Scene.FindEntitiesWithTag("sp_horse_race").ToList<GameEntity>();
		}

		private MatrixFrame GetStartFrame(int index)
		{
			MatrixFrame matrixFrame;
			if (index < this._startPoints.Count)
			{
				matrixFrame = this._startPoints[index].GetGlobalFrame();
			}
			else
			{
				matrixFrame = ((this._startPoints.Count > 0) ? this._startPoints[0].GetGlobalFrame() : MatrixFrame.Identity);
			}
			matrixFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
			return matrixFrame;
		}

		private void SetItemsAndSpawnCharacter(CharacterObject troop)
		{
			int count = this._agents.Count;
			Equipment equipment = new Equipment();
			equipment.AddEquipmentToSlotWithoutAgent(10, new EquipmentElement(Game.Current.ObjectManager.GetObject<ItemObject>("charger"), null, null, false));
			equipment.AddEquipmentToSlotWithoutAgent(11, new EquipmentElement(Game.Current.ObjectManager.GetObject<ItemObject>("horse_harness_e"), null, null, false));
			equipment.AddEquipmentToSlotWithoutAgent(0, new EquipmentElement(Game.Current.ObjectManager.GetObject<ItemObject>("horse_whip"), null, null, false));
			equipment.AddEquipmentToSlotWithoutAgent(6, new EquipmentElement(Game.Current.ObjectManager.GetObject<ItemObject>("short_padded_robe"), null, null, false));
			MatrixFrame startFrame = this.GetStartFrame(count);
			AgentBuildData agentBuildData = new AgentBuildData(troop).Team(this._teams[count]).InitialPosition(ref startFrame.origin);
			Vec2 vec = startFrame.rotation.f.AsVec2;
			vec = vec.Normalized();
			AgentBuildData agentBuildData2 = agentBuildData.InitialDirection(ref vec).Equipment(equipment).Controller((troop == CharacterObject.PlayerCharacter) ? 2 : 1);
			Agent agent = base.Mission.SpawnAgent(agentBuildData2, false);
			agent.Health = (float)agent.Monster.HitPoints;
			agent.WieldInitialWeapons(2, 0);
			this._agents.Add(this.AddHorseRaceAgentController(agent));
			if (troop == CharacterObject.PlayerCharacter)
			{
				base.Mission.PlayerTeam = this._teams[count];
			}
		}

		private TownHorseRaceAgentController AddHorseRaceAgentController(Agent agent)
		{
			return agent.AddController(typeof(TownHorseRaceAgentController)) as TownHorseRaceAgentController;
		}

		private void InitializeTeams(int count)
		{
			this._teams = new List<Team>();
			for (int i = 0; i < count; i++)
			{
				this._teams.Add(base.Mission.Teams.Add(-1, uint.MaxValue, uint.MaxValue, null, true, false, true));
			}
		}

		public void StartMatch(TournamentMatch match, bool isLastRound)
		{
			throw new NotImplementedException();
		}

		public void SkipMatch(TournamentMatch match)
		{
			throw new NotImplementedException();
		}

		public bool IsMatchEnded()
		{
			throw new NotImplementedException();
		}

		public void OnMatchEnded()
		{
			throw new NotImplementedException();
		}

		public const int TourCount = 2;

		private readonly List<TownHorseRaceAgentController> _agents;

		private List<Team> _teams;

		private List<GameEntity> _startPoints;

		private BasicMissionTimer _startTimer;

		private CultureObject _culture;

		public class CheckPoint
		{
			public string Name
			{
				get
				{
					return this._volumeBox.GameEntity.Name;
				}
			}

			public CheckPoint(VolumeBox volumeBox)
			{
				this._volumeBox = volumeBox;
				this._bestTargetList = MBExtensions.CollectChildrenEntitiesWithTag(this._volumeBox.GameEntity, "best_target_point");
				this._volumeBox.SetIsOccupiedDelegate(new VolumeBox.VolumeBoxDelegate(this.OnAgentsEnterCheckBox));
			}

			public Vec3 GetBestTargetPosition()
			{
				Vec3 vec;
				if (this._bestTargetList.Count > 0)
				{
					vec = this._bestTargetList[MBRandom.RandomInt(this._bestTargetList.Count)].GetGlobalFrame().origin;
				}
				else
				{
					vec = this._volumeBox.GameEntity.GetGlobalFrame().origin;
				}
				return vec;
			}

			public void AddToCheckList(Agent agent)
			{
				this._volumeBox.AddToCheckList(agent);
			}

			public void RemoveFromCheckList(Agent agent)
			{
				this._volumeBox.RemoveFromCheckList(agent);
			}

			private void OnAgentsEnterCheckBox(VolumeBox volumeBox, List<Agent> agentsInVolume)
			{
				foreach (Agent agent in agentsInVolume)
				{
					agent.GetController<TownHorseRaceAgentController>().OnEnterCheckPoint(volumeBox);
				}
			}

			private readonly VolumeBox _volumeBox;

			private readonly List<GameEntity> _bestTargetList;
		}
	}
}
