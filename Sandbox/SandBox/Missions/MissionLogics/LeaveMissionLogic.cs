using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics
{
	public class LeaveMissionLogic : MissionLogic
	{
		public override bool MissionEnded(ref MissionResult missionResult)
		{
			return base.Mission.MainAgent != null && !base.Mission.MainAgent.IsActive();
		}

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

		public string UnconsciousGameMenuID = "settlement_player_unconscious";

		private Timer _isAgentDeadTimer;
	}
}
