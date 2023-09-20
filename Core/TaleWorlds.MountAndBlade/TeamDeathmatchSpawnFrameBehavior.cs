using System;
using System.Linq;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002C8 RID: 712
	public class TeamDeathmatchSpawnFrameBehavior : SpawnFrameBehaviorBase
	{
		// Token: 0x06002708 RID: 9992 RVA: 0x00093DE0 File Offset: 0x00091FE0
		public override MatrixFrame GetSpawnFrame(Team team, bool hasMount, bool isInitialSpawn)
		{
			return base.GetSpawnFrameFromSpawnPoints(this.SpawnPoints.ToList<GameEntity>(), team, hasMount);
		}
	}
}
