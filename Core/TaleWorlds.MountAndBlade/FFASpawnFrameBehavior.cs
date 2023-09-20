using System;
using System.Linq;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class FFASpawnFrameBehavior : SpawnFrameBehaviorBase
	{
		public override MatrixFrame GetSpawnFrame(Team team, bool hasMount, bool isInitialSpawn)
		{
			return base.GetSpawnFrameFromSpawnPoints(this.SpawnPoints.ToList<GameEntity>(), null, hasMount);
		}
	}
}
