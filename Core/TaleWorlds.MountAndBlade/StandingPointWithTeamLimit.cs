using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	public class StandingPointWithTeamLimit : StandingPoint
	{
		public Team UsableTeam { get; set; }

		public override bool IsDisabledForAgent(Agent agent)
		{
			return agent.Team != this.UsableTeam || base.IsDisabledForAgent(agent);
		}

		protected internal override bool IsUsableBySide(BattleSideEnum side)
		{
			return side == this.UsableTeam.Side && base.IsUsableBySide(side);
		}
	}
}
