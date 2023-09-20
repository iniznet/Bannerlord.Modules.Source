using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics
{
	// Token: 0x02000040 RID: 64
	public class LeaveMissionLogic : MissionLogic
	{
		// Token: 0x06000315 RID: 789 RVA: 0x000146B0 File Offset: 0x000128B0
		public override bool MissionEnded(ref MissionResult missionResult)
		{
			return base.Mission.MainAgent != null && !base.Mission.MainAgent.IsActive();
		}

		// Token: 0x06000316 RID: 790 RVA: 0x000146D4 File Offset: 0x000128D4
		public override void OnMissionTick(float dt)
		{
			if (Agent.Main == null || !Agent.Main.IsActive())
			{
				if (this._isAgentDeadTimer == null)
				{
					this._isAgentDeadTimer = new Timer(Mission.Current.CurrentTime, 5f, true);
				}
				if (this._isAgentDeadTimer.Check(Mission.Current.CurrentTime))
				{
					Mission.Current.NextCheckTimeEndMission = 0f;
					Mission.Current.EndMission();
					Campaign.Current.GameMenuManager.SetNextMenu(this.UnconsciousGameMenuID);
					return;
				}
			}
			else if (this._isAgentDeadTimer != null)
			{
				this._isAgentDeadTimer = null;
			}
		}

		// Token: 0x04000196 RID: 406
		public string UnconsciousGameMenuID = "settlement_player_unconscious";

		// Token: 0x04000197 RID: 407
		private Timer _isAgentDeadTimer;
	}
}
