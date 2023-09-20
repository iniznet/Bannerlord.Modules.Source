using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics
{
	public class IndoorMissionController : MissionLogic
	{
		public override void OnCreated()
		{
			base.OnCreated();
			base.Mission.DoesMissionRequireCivilianEquipment = true;
		}

		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._missionAgentHandler = base.Mission.GetMissionBehavior<MissionAgentHandler>();
		}

		public override void AfterStart()
		{
			base.AfterStart();
			base.Mission.SetMissionMode(0, true);
			base.Mission.IsInventoryAccessible = !Campaign.Current.IsMainHeroDisguised;
			base.Mission.IsQuestScreenAccessible = true;
			this._missionAgentHandler.SpawnPlayer(base.Mission.DoesMissionRequireCivilianEquipment, true, false, false, false, "");
			this._missionAgentHandler.SpawnLocationCharacters(null);
		}

		private MissionAgentHandler _missionAgentHandler;
	}
}
