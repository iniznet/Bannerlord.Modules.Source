using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics
{
	// Token: 0x02000056 RID: 86
	public class SneakTeam3MissionController : MissionLogic
	{
		// Token: 0x060003C2 RID: 962 RVA: 0x0001B7B4 File Offset: 0x000199B4
		public SneakTeam3MissionController()
		{
			this._game = Game.Current;
			this._townRegionProps = new List<List<GameEntity>>();
			this._isScrollObtained = false;
		}

		// Token: 0x060003C3 RID: 963 RVA: 0x0001B7DC File Offset: 0x000199DC
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
			this._playerAgent.WieldInitialWeapons(2);
		}

		// Token: 0x060003C4 RID: 964 RVA: 0x0001B8D8 File Offset: 0x00019AD8
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

		// Token: 0x060003C5 RID: 965 RVA: 0x0001B92C File Offset: 0x00019B2C
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

		// Token: 0x060003C6 RID: 966 RVA: 0x0001B9AD File Offset: 0x00019BAD
		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
		}

		// Token: 0x060003C7 RID: 967 RVA: 0x0001B9B6 File Offset: 0x00019BB6
		private bool IsPlayerDead()
		{
			return base.Mission.MainAgent == null || !base.Mission.MainAgent.IsActive();
		}

		// Token: 0x060003C8 RID: 968 RVA: 0x0001B9DA File Offset: 0x00019BDA
		public override void OnObjectUsed(Agent userAgent, UsableMissionObject usedObject)
		{
			if (usedObject.GameEntity.HasTag("khuzait_scroll"))
			{
				this._isScrollObtained = true;
			}
		}

		// Token: 0x060003C9 RID: 969 RVA: 0x0001B9F5 File Offset: 0x00019BF5
		public override bool MissionEnded(ref MissionResult missionResult)
		{
			return this._isScrollObtained || this.IsPlayerDead();
		}

		// Token: 0x040001C3 RID: 451
		private Game _game;

		// Token: 0x040001C4 RID: 452
		private List<List<GameEntity>> _townRegionProps;

		// Token: 0x040001C5 RID: 453
		private Agent _playerAgent;

		// Token: 0x040001C6 RID: 454
		private const string _targetEntityTag = "khuzait_scroll";

		// Token: 0x040001C7 RID: 455
		private bool _isScrollObtained;
	}
}
