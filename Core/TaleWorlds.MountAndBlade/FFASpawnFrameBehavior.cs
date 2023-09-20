using System;
using System.Linq;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002C4 RID: 708
	public class FFASpawnFrameBehavior : SpawnFrameBehaviorBase
	{
		// Token: 0x060026F8 RID: 9976 RVA: 0x0009348D File Offset: 0x0009168D
		public override MatrixFrame GetSpawnFrame(Team team, bool hasMount, bool isInitialSpawn)
		{
			return base.GetSpawnFrameFromSpawnPoints(this.SpawnPoints.ToList<GameEntity>(), null, hasMount);
		}
	}
}
