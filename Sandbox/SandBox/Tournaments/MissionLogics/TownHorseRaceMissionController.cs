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
	// Token: 0x0200001E RID: 30
	public class TownHorseRaceMissionController : MissionLogic, ITournamentGameBehavior
	{
		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000167 RID: 359 RVA: 0x0000AAF6 File Offset: 0x00008CF6
		// (set) Token: 0x06000168 RID: 360 RVA: 0x0000AAFE File Offset: 0x00008CFE
		public List<TownHorseRaceMissionController.CheckPoint> CheckPoints { get; private set; }

		// Token: 0x06000169 RID: 361 RVA: 0x0000AB07 File Offset: 0x00008D07
		public TownHorseRaceMissionController(CultureObject culture)
		{
			this._culture = culture;
			this._agents = new List<TownHorseRaceAgentController>();
		}

		// Token: 0x0600016A RID: 362 RVA: 0x0000AB24 File Offset: 0x00008D24
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

		// Token: 0x0600016B RID: 363 RVA: 0x0000AB8C File Offset: 0x00008D8C
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

		// Token: 0x0600016C RID: 364 RVA: 0x0000AC00 File Offset: 0x00008E00
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

		// Token: 0x0600016D RID: 365 RVA: 0x0000ACE8 File Offset: 0x00008EE8
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

		// Token: 0x0600016E RID: 366 RVA: 0x0000AD4C File Offset: 0x00008F4C
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
			agent.WieldInitialWeapons(2);
			this._agents.Add(this.AddHorseRaceAgentController(agent));
			if (troop == CharacterObject.PlayerCharacter)
			{
				base.Mission.PlayerTeam = this._teams[count];
			}
		}

		// Token: 0x0600016F RID: 367 RVA: 0x0000AEBC File Offset: 0x000090BC
		private TownHorseRaceAgentController AddHorseRaceAgentController(Agent agent)
		{
			return agent.AddController(typeof(TownHorseRaceAgentController)) as TownHorseRaceAgentController;
		}

		// Token: 0x06000170 RID: 368 RVA: 0x0000AED4 File Offset: 0x000090D4
		private void InitializeTeams(int count)
		{
			this._teams = new List<Team>();
			for (int i = 0; i < count; i++)
			{
				this._teams.Add(base.Mission.Teams.Add(-1, uint.MaxValue, uint.MaxValue, null, true, false, true));
			}
		}

		// Token: 0x06000171 RID: 369 RVA: 0x0000AF1A File Offset: 0x0000911A
		public void StartMatch(TournamentMatch match, bool isLastRound)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000172 RID: 370 RVA: 0x0000AF21 File Offset: 0x00009121
		public void SkipMatch(TournamentMatch match)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000173 RID: 371 RVA: 0x0000AF28 File Offset: 0x00009128
		public bool IsMatchEnded()
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000174 RID: 372 RVA: 0x0000AF2F File Offset: 0x0000912F
		public void OnMatchEnded()
		{
			throw new NotImplementedException();
		}

		// Token: 0x04000095 RID: 149
		public const int TourCount = 2;

		// Token: 0x04000097 RID: 151
		private readonly List<TownHorseRaceAgentController> _agents;

		// Token: 0x04000098 RID: 152
		private List<Team> _teams;

		// Token: 0x04000099 RID: 153
		private List<GameEntity> _startPoints;

		// Token: 0x0400009A RID: 154
		private BasicMissionTimer _startTimer;

		// Token: 0x0400009B RID: 155
		private CultureObject _culture;

		// Token: 0x020000FE RID: 254
		public class CheckPoint
		{
			// Token: 0x170000E2 RID: 226
			// (get) Token: 0x06000C9A RID: 3226 RVA: 0x00061A5F File Offset: 0x0005FC5F
			public string Name
			{
				get
				{
					return this._volumeBox.GameEntity.Name;
				}
			}

			// Token: 0x06000C9B RID: 3227 RVA: 0x00061A74 File Offset: 0x0005FC74
			public CheckPoint(VolumeBox volumeBox)
			{
				this._volumeBox = volumeBox;
				this._bestTargetList = MBExtensions.CollectChildrenEntitiesWithTag(this._volumeBox.GameEntity, "best_target_point");
				this._volumeBox.SetIsOccupiedDelegate(new VolumeBox.VolumeBoxDelegate(this.OnAgentsEnterCheckBox));
			}

			// Token: 0x06000C9C RID: 3228 RVA: 0x00061AC0 File Offset: 0x0005FCC0
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

			// Token: 0x06000C9D RID: 3229 RVA: 0x00061B1A File Offset: 0x0005FD1A
			public void AddToCheckList(Agent agent)
			{
				this._volumeBox.AddToCheckList(agent);
			}

			// Token: 0x06000C9E RID: 3230 RVA: 0x00061B28 File Offset: 0x0005FD28
			public void RemoveFromCheckList(Agent agent)
			{
				this._volumeBox.RemoveFromCheckList(agent);
			}

			// Token: 0x06000C9F RID: 3231 RVA: 0x00061B38 File Offset: 0x0005FD38
			private void OnAgentsEnterCheckBox(VolumeBox volumeBox, List<Agent> agentsInVolume)
			{
				foreach (Agent agent in agentsInVolume)
				{
					Debug.Print(string.Concat(new object[] { agent.Name, "(", agent.Index, ") entered the checkpoint ", this.Name }), 0, 12, 17592186044416UL);
					agent.GetController<TownHorseRaceAgentController>().OnEnterCheckPoint(volumeBox);
				}
			}

			// Token: 0x040004F5 RID: 1269
			private readonly VolumeBox _volumeBox;

			// Token: 0x040004F6 RID: 1270
			private readonly List<GameEntity> _bestTargetList;
		}
	}
}
