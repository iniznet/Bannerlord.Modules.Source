using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class DuelSpawnFrameBehavior : SpawnFrameBehaviorBase
	{
		public override void Initialize()
		{
			base.Initialize();
			this._duelAreaSpawnPoints = new List<GameEntity>[16];
			foreach (GameEntity gameEntity in Mission.Current.Scene.FindEntitiesWithTagExpression("spawnpoint_area(_\\d+)*"))
			{
				int num = int.Parse(gameEntity.Tags.Single((string tag) => tag.StartsWith("spawnpoint_area_")).Replace("spawnpoint_area_", "")) - 1;
				if (this._duelAreaSpawnPoints[num] == null)
				{
					this._duelAreaSpawnPoints[num] = new List<GameEntity>();
				}
				this._duelAreaSpawnPoints[num].Add(gameEntity);
			}
		}

		public override MatrixFrame GetSpawnFrame(Team team, bool hasMount, bool isInitialSpawn)
		{
			int duelAreaIndexIfDuelTeam = Mission.Current.GetMissionBehavior<MissionMultiplayerDuel>().GetDuelAreaIndexIfDuelTeam(team);
			List<GameEntity> list = ((duelAreaIndexIfDuelTeam >= 0) ? this._duelAreaSpawnPoints[duelAreaIndexIfDuelTeam] : this.SpawnPoints.ToList<GameEntity>());
			return base.GetSpawnFrameFromSpawnPoints(list, team, hasMount);
		}

		private const string AreaSpawnPointTagExpression = "spawnpoint_area(_\\d+)*";

		private const string AreaSpawnPointTagPrefix = "spawnpoint_area_";

		private List<GameEntity>[] _duelAreaSpawnPoints;
	}
}
