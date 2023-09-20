using System;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000274 RID: 628
	public class MissionHardBorderPlacer : MissionLogic
	{
		// Token: 0x06002192 RID: 8594 RVA: 0x0007AD5C File Offset: 0x00078F5C
		public override void EarlyStart()
		{
			base.EarlyStart();
			Scene scene = base.Mission.Scene;
			GameEntity gameEntity = GameEntity.CreateEmpty(scene, true);
			scene.FillEntityWithHardBorderPhysicsBarrier(gameEntity);
		}
	}
}
