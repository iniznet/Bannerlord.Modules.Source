using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002C3 RID: 707
	public class DuelSpawnFrameBehavior : SpawnFrameBehaviorBase
	{
		// Token: 0x060026F5 RID: 9973 RVA: 0x00093374 File Offset: 0x00091574
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

		// Token: 0x060026F6 RID: 9974 RVA: 0x00093444 File Offset: 0x00091644
		public override MatrixFrame GetSpawnFrame(Team team, bool hasMount, bool isInitialSpawn)
		{
			int duelAreaIndexIfDuelTeam = Mission.Current.GetMissionBehavior<MissionMultiplayerDuel>().GetDuelAreaIndexIfDuelTeam(team);
			List<GameEntity> list = ((duelAreaIndexIfDuelTeam >= 0) ? this._duelAreaSpawnPoints[duelAreaIndexIfDuelTeam] : this.SpawnPoints.ToList<GameEntity>());
			return base.GetSpawnFrameFromSpawnPoints(list, team, hasMount);
		}

		// Token: 0x04000E6F RID: 3695
		private const string AreaSpawnPointTagExpression = "spawnpoint_area(_\\d+)*";

		// Token: 0x04000E70 RID: 3696
		private const string AreaSpawnPointTagPrefix = "spawnpoint_area_";

		// Token: 0x04000E71 RID: 3697
		private List<GameEntity>[] _duelAreaSpawnPoints;
	}
}
