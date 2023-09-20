using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.ComponentInterfaces
{
	// Token: 0x0200040E RID: 1038
	public abstract class AutoBlockModel : GameModel
	{
		// Token: 0x0600358F RID: 13711
		public abstract Agent.UsageDirection GetBlockDirection(Mission mission);
	}
}
