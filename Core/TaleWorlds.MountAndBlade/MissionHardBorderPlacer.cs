using System;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	public class MissionHardBorderPlacer : MissionLogic
	{
		public override void EarlyStart()
		{
			base.EarlyStart();
			Scene scene = base.Mission.Scene;
			GameEntity gameEntity = GameEntity.CreateEmpty(scene, true);
			scene.FillEntityWithHardBorderPhysicsBarrier(gameEntity);
		}
	}
}
