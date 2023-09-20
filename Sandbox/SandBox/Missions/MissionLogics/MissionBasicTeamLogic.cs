﻿using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics
{
	// Token: 0x02000045 RID: 69
	public class MissionBasicTeamLogic : MissionLogic
	{
		// Token: 0x06000370 RID: 880 RVA: 0x000190AD File Offset: 0x000172AD
		public override void AfterStart()
		{
			base.AfterStart();
			this.InitializeTeams(true);
		}

		// Token: 0x06000371 RID: 881 RVA: 0x000190BC File Offset: 0x000172BC
		private void GetTeamColor(BattleSideEnum side, bool isPlayerAttacker, out uint teamColor1, out uint teamColor2)
		{
			teamColor1 = uint.MaxValue;
			teamColor2 = uint.MaxValue;
			if (Campaign.Current.GameMode == 1)
			{
				if ((isPlayerAttacker && side == 1) || (!isPlayerAttacker && side == null))
				{
					teamColor1 = Hero.MainHero.MapFaction.Color;
					teamColor2 = Hero.MainHero.MapFaction.Color2;
					return;
				}
				if (MobileParty.MainParty.MapEvent != null)
				{
					if (MobileParty.MainParty.MapEvent.MapEventSettlement != null)
					{
						teamColor1 = MobileParty.MainParty.MapEvent.MapEventSettlement.MapFaction.Color;
						teamColor2 = MobileParty.MainParty.MapEvent.MapEventSettlement.MapFaction.Color2;
						return;
					}
					teamColor1 = MobileParty.MainParty.MapEvent.GetLeaderParty(side).MapFaction.Color;
					teamColor2 = MobileParty.MainParty.MapEvent.GetLeaderParty(side).MapFaction.Color2;
				}
			}
		}

		// Token: 0x06000372 RID: 882 RVA: 0x000191A4 File Offset: 0x000173A4
		private void InitializeTeams(bool isPlayerAttacker = true)
		{
			if (!Extensions.IsEmpty<Team>(base.Mission.Teams))
			{
				throw new MBIllegalValueException("Number of teams is not 0.");
			}
			uint num;
			uint num2;
			this.GetTeamColor(0, isPlayerAttacker, out num, out num2);
			uint num3;
			uint num4;
			this.GetTeamColor(1, isPlayerAttacker, out num3, out num4);
			base.Mission.Teams.Add(0, num, num2, null, true, false, true);
			base.Mission.Teams.Add(1, num3, num4, null, true, false, true);
			if (isPlayerAttacker)
			{
				base.Mission.Teams.Add(1, uint.MaxValue, uint.MaxValue, null, true, false, true);
				base.Mission.PlayerTeam = base.Mission.AttackerTeam;
				return;
			}
			base.Mission.Teams.Add(0, uint.MaxValue, uint.MaxValue, null, true, false, true);
			base.Mission.PlayerTeam = base.Mission.DefenderTeam;
		}
	}
}
