using System;
using SandBox.Missions.AgentControllers;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics
{
	public class AmbushIntroLogic : MissionLogic
	{
		public override void OnCreated()
		{
			this._ambushMission = base.Mission.GetMissionBehavior<AmbushMissionController>();
		}

		public void StartIntro()
		{
			Action startIntroAction = this.StartIntroAction;
			if (startIntroAction == null)
			{
				return;
			}
			startIntroAction();
		}

		public void OnIntroEnded()
		{
			this._ambushMission.OnIntroductionFinish();
			base.Mission.RemoveMissionBehavior(this);
		}

		private AmbushMissionController _ambushMission;

		public Action StartIntroAction;
	}
}
