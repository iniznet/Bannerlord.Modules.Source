using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Source.Missions
{
	// Token: 0x020003F7 RID: 1015
	public class SimpleMountedPlayerMissionController : MissionLogic
	{
		// Token: 0x060034E1 RID: 13537 RVA: 0x000DC126 File Offset: 0x000DA326
		public SimpleMountedPlayerMissionController()
		{
			this._game = Game.Current;
		}

		// Token: 0x060034E2 RID: 13538 RVA: 0x000DC13C File Offset: 0x000DA33C
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

		// Token: 0x060034E3 RID: 13539 RVA: 0x000DC1D6 File Offset: 0x000DA3D6
		public override bool MissionEnded(ref MissionResult missionResult)
		{
			return base.Mission.InputManager.IsGameKeyPressed(4);
		}

		// Token: 0x040016A7 RID: 5799
		private Game _game;
	}
}
