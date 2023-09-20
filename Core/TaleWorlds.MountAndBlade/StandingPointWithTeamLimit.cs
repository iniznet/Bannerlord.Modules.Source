using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000363 RID: 867
	public class StandingPointWithTeamLimit : StandingPoint
	{
		// Token: 0x17000883 RID: 2179
		// (get) Token: 0x06002F54 RID: 12116 RVA: 0x000C1066 File Offset: 0x000BF266
		// (set) Token: 0x06002F55 RID: 12117 RVA: 0x000C106E File Offset: 0x000BF26E
		public Team UsableTeam { get; set; }

		// Token: 0x06002F56 RID: 12118 RVA: 0x000C1077 File Offset: 0x000BF277
		public override bool IsDisabledForAgent(Agent agent)
		{
			return agent.Team != this.UsableTeam || base.IsDisabledForAgent(agent);
		}

		// Token: 0x06002F57 RID: 12119 RVA: 0x000C1090 File Offset: 0x000BF290
		protected internal override bool IsUsableBySide(BattleSideEnum side)
		{
			return side == this.UsableTeam.Side && base.IsUsableBySide(side);
		}
	}
}
