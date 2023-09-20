using System;
using TaleWorlds.CampaignSystem.Siege;

namespace TaleWorlds.CampaignSystem
{
	public interface ISiegeEventVisualCreator
	{
		ISiegeEventVisual CreateSiegeEventVisual(SiegeEvent siegeEvent);
	}
}
