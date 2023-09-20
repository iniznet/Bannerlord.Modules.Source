using System;
using SandBox.Missions.AgentControllers;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics
{
	// Token: 0x02000034 RID: 52
	public class AmbushIntroLogic : MissionLogic
	{
		// Token: 0x0600026E RID: 622 RVA: 0x00010875 File Offset: 0x0000EA75
		public override void OnCreated()
		{
			this._ambushMission = base.Mission.GetMissionBehavior<AmbushMissionController>();
		}

		// Token: 0x0600026F RID: 623 RVA: 0x00010888 File Offset: 0x0000EA88
		public void StartIntro()
		{
			Action startIntroAction = this.StartIntroAction;
			if (startIntroAction == null)
			{
				return;
			}
			startIntroAction();
		}

		// Token: 0x06000270 RID: 624 RVA: 0x0001089A File Offset: 0x0000EA9A
		public void OnIntroEnded()
		{
			this._ambushMission.OnIntroductionFinish();
			base.Mission.RemoveMissionBehavior(this);
		}

		// Token: 0x0400013E RID: 318
		private AmbushMissionController _ambushMission;

		// Token: 0x0400013F RID: 319
		public Action StartIntroAction;
	}
}
