using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade.Source.Missions
{
	// Token: 0x020003F3 RID: 1011
	public class EquipmentTestMissionController : MissionLogic
	{
		// Token: 0x060034CC RID: 13516 RVA: 0x000DBCA4 File Offset: 0x000D9EA4
		public override void AfterStart()
		{
			base.AfterStart();
			GameEntity gameEntity = base.Mission.Scene.FindEntityWithTag("spawnpoint_player");
			base.Mission.SpawnAgent(new AgentBuildData(Game.Current.PlayerTroop).Team(base.Mission.AttackerTeam).InitialFrameFromSpawnPointEntity(gameEntity).CivilianEquipment(false)
				.Controller(Agent.ControllerType.Player), false);
		}
	}
}
