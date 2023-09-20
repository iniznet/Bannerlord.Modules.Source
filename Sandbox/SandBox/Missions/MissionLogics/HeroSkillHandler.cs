using System;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics
{
	// Token: 0x0200003A RID: 58
	public class HeroSkillHandler : MissionLogic
	{
		// Token: 0x060002C6 RID: 710 RVA: 0x000122CA File Offset: 0x000104CA
		public override void AfterStart()
		{
			this._nextCaptainSkillMoraleBoostTime = MissionTime.SecondsFromNow(10f);
		}

		// Token: 0x060002C7 RID: 711 RVA: 0x000122DC File Offset: 0x000104DC
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

		// Token: 0x060002C8 RID: 712 RVA: 0x00012368 File Offset: 0x00010568
		private void BoostMoraleForTeam(Team team)
		{
		}

		// Token: 0x0400015A RID: 346
		private MissionTime _nextCaptainSkillMoraleBoostTime;

		// Token: 0x0400015B RID: 347
		private bool _boostMorale;

		// Token: 0x0400015C RID: 348
		private int _nextMoraleTeam;
	}
}
