using System;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics
{
	public class HeroSkillHandler : MissionLogic
	{
		public override void AfterStart()
		{
			this._nextCaptainSkillMoraleBoostTime = MissionTime.SecondsFromNow(10f);
		}

		public override void OnMissionTick(float dt)
		{
			if (this._nextCaptainSkillMoraleBoostTime.IsPast)
			{
				this._boostMorale = true;
				this._nextMoraleTeam = 0;
				this._nextCaptainSkillMoraleBoostTime = MissionTime.SecondsFromNow(10f);
			}
			if (this._boostMorale)
			{
				if (this._nextMoraleTeam >= base.Mission.Teams.Count)
				{
					this._boostMorale = false;
					return;
				}
				Team team = base.Mission.Teams[this._nextMoraleTeam];
				this.BoostMoraleForTeam(team);
				this._nextMoraleTeam++;
			}
		}

		private void BoostMoraleForTeam(Team team)
		{
		}

		private MissionTime _nextCaptainSkillMoraleBoostTime;

		private bool _boostMorale;

		private int _nextMoraleTeam;
	}
}
