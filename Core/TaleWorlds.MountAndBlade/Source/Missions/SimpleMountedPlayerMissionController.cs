using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Source.Missions
{
	public class SimpleMountedPlayerMissionController : MissionLogic
	{
		public SimpleMountedPlayerMissionController()
		{
			this._game = Game.Current;
		}

		public override void AfterStart()
		{
			BasicCharacterObject @object = this._game.ObjectManager.GetObject<BasicCharacterObject>("aserai_tribal_horseman");
			GameEntity gameEntity = Mission.Current.Scene.FindEntityWithTag("sp_play");
			MatrixFrame matrixFrame = ((gameEntity != null) ? gameEntity.GetGlobalFrame() : MatrixFrame.Identity);
			AgentBuildData agentBuildData = new AgentBuildData(new BasicBattleAgentOrigin(@object));
			AgentBuildData agentBuildData2 = agentBuildData.InitialPosition(matrixFrame.origin);
			Vec2 vec = matrixFrame.rotation.f.AsVec2;
			vec = vec.Normalized();
			agentBuildData2.InitialDirection(vec).Controller(Agent.ControllerType.Player);
			base.Mission.SpawnAgent(agentBuildData, false).WieldInitialWeapons(Agent.WeaponWieldActionType.InstantAfterPickUp);
		}

		public override bool MissionEnded(ref MissionResult missionResult)
		{
			return base.Mission.InputManager.IsGameKeyPressed(4);
		}

		private Game _game;
	}
}
