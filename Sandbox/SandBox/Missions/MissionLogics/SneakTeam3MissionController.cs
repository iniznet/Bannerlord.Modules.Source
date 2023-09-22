using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics
{
	public class SneakTeam3MissionController : MissionLogic
	{
		public SneakTeam3MissionController()
		{
			this._game = Game.Current;
			this._townRegionProps = new List<List<GameEntity>>();
			this._isScrollObtained = false;
		}

		public override void AfterStart()
		{
			base.AfterStart();
			base.Mission.SetMissionMode(4, true);
			base.Mission.Scene.TimeOfDay = 20.5f;
			this.GetAllProps();
			this.RandomizeScrollPosition();
			GameEntity gameEntity = base.Mission.Scene.FindEntityWithTag("spawnpoint_player");
			MatrixFrame matrixFrame = ((gameEntity != null) ? gameEntity.GetGlobalFrame() : MatrixFrame.Identity);
			if (gameEntity != null)
			{
				matrixFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
			}
			Mission mission = base.Mission;
			AgentBuildData agentBuildData = new AgentBuildData(this._game.PlayerTroop).Team(base.Mission.PlayerTeam).InitialPosition(ref matrixFrame.origin);
			Vec2 vec = matrixFrame.rotation.f.AsVec2;
			vec = vec.Normalized();
			this._playerAgent = mission.SpawnAgent(agentBuildData.InitialDirection(ref vec).NoHorses(true).Controller(2), false);
			this._playerAgent.WieldInitialWeapons(2, 0);
		}

		private void GetAllProps()
		{
			for (int i = 0; i < 5; i++)
			{
				List<GameEntity> list = new List<GameEntity>();
				IEnumerable<GameEntity> enumerable = base.Mission.Scene.FindEntitiesWithTag("patrol_region_" + i);
				list.AddRange(enumerable);
				this._townRegionProps.Add(list);
			}
		}

		private void RandomizeScrollPosition()
		{
			int num = MBRandom.RandomInt(3);
			GameEntity gameEntity = base.Mission.Scene.FindEntityWithTag("scroll_" + num);
			if (gameEntity != null)
			{
				GameEntity gameEntity2 = base.Mission.Scene.FindEntityWithTag("khuzait_scroll");
				if (gameEntity2 != null)
				{
					MatrixFrame frame = gameEntity.GetFrame();
					frame.origin.z = frame.origin.z + 0.9f;
					gameEntity2.SetFrame(ref frame);
				}
			}
		}

		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
		}

		private bool IsPlayerDead()
		{
			return base.Mission.MainAgent == null || !base.Mission.MainAgent.IsActive();
		}

		public override void OnObjectUsed(Agent userAgent, UsableMissionObject usedObject)
		{
			if (usedObject.GameEntity.HasTag("khuzait_scroll"))
			{
				this._isScrollObtained = true;
			}
		}

		public override bool MissionEnded(ref MissionResult missionResult)
		{
			return this._isScrollObtained || this.IsPlayerDead();
		}

		private Game _game;

		private List<List<GameEntity>> _townRegionProps;

		private Agent _playerAgent;

		private const string _targetEntityTag = "khuzait_scroll";

		private bool _isScrollObtained;
	}
}
