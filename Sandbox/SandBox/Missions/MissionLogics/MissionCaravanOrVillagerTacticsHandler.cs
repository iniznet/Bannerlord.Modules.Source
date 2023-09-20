using System;
using System.Linq;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics
{
	public class MissionCaravanOrVillagerTacticsHandler : MissionLogic
	{
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
