using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade.Source.Missions
{
	public class EquipmentTestMissionController : MissionLogic
	{
		public override void AfterStart()
		{
			base.AfterStart();
			GameEntity gameEntity = base.Mission.Scene.FindEntityWithTag("spawnpoint_player");
			base.Mission.SpawnAgent(new AgentBuildData(Game.Current.PlayerTroop).Team(base.Mission.AttackerTeam).InitialFrameFromSpawnPointEntity(gameEntity).CivilianEquipment(false)
				.Controller(Agent.ControllerType.Player), false);
		}
	}
}
