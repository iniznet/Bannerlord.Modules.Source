using System;
using System.Linq;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics
{
	// Token: 0x02000046 RID: 70
	public class MissionCaravanOrVillagerTacticsHandler : MissionLogic
	{
		// Token: 0x06000374 RID: 884 RVA: 0x00019280 File Offset: 0x00017480
		public override void EarlyStart()
		{
			foreach (Team team in Mission.Current.Teams)
			{
				if (team.HasTeamAi)
				{
					if (!MapEvent.PlayerMapEvent.PartiesOnSide(team.Side).Any((MapEventParty p) => p.Party.IsMobile && p.Party.MobileParty.IsCaravan))
					{
						if (MapEvent.PlayerMapEvent.MapEventSettlement != null)
						{
							continue;
						}
						if (!MapEvent.PlayerMapEvent.PartiesOnSide(team.Side).Any((MapEventParty p) => p.Party.IsMobile && p.Party.MobileParty.IsVillager))
						{
							continue;
						}
					}
					team.AddTacticOption(new TacticDefensiveLine(team));
				}
			}
		}
	}
}
