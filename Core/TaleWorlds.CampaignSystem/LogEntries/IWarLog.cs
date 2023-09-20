using System;

namespace TaleWorlds.CampaignSystem.LogEntries
{
	// Token: 0x020002F9 RID: 761
	public interface IWarLog
	{
		// Token: 0x06002B9D RID: 11165
		bool IsRelatedToWar(StanceLink stance, out IFaction effector, out IFaction effected);
	}
}
