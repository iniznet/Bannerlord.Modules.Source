using System;
using TaleWorlds.CampaignSystem.MapEvents;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x020000A7 RID: 167
	public interface IMapEventVisualCreator
	{
		// Token: 0x06001191 RID: 4497
		IMapEventVisual CreateMapEventVisual(MapEvent mapEvent);
	}
}
