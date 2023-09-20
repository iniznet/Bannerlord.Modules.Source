using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics
{
	public class RetirementMissionLogic : MissionLogic
	{
		public override void AfterStart()
		{
			base.AfterStart();
			this.SpawnHermit();
			((LeaveMissionLogic)base.Mission.MissionLogics.FirstOrDefault((MissionLogic x) => x is LeaveMissionLogic)).UnconsciousGameMenuID = "retirement_after_player_knockedout";
		}

		private void SpawnHermit()
		{
			List<GameEntity> list = base.Mission.Scene.FindEntitiesWithTag("sp_hermit").ToList<GameEntity>();
			MatrixFrame globalFrame = list[MBRandom.RandomInt(list.Count<GameEntity>())].GetGlobalFrame();
			CharacterObject @object = Campaign.Current.ObjectManager.GetObject<CharacterObject>("sp_hermit");
			AgentBuildData agentBuildData = new AgentBuildData(@object).TroopOrigin(new SimpleAgentOrigin(@object, -1, null, default(UniqueTroopDescriptor))).Team(base.Mission.SpectatorTeam).InitialPosition(ref globalFrame.origin);
			Vec2 vec = globalFrame.rotation.f.AsVec2;
			vec = vec.Normalized();
			AgentBuildData agentBuildData2 = agentBuildData.InitialDirection(ref vec).CivilianEquipment(true).NoHorses(true)
				.NoWeapons(true)
				.ClothingColor1(base.Mission.PlayerTeam.Color)
				.ClothingColor2(base.Mission.PlayerTeam.Color2);
			base.Mission.SpawnAgent(agentBuildData2, false).SetMortalityState(1);
		}
	}
}
