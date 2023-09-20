using System;
using SandBox.View.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;

namespace SandBox.View
{
	public class PartyVisualCreator : IPartyVisualCreator
	{
		IPartyVisual IPartyVisualCreator.CreatePartyVisual()
		{
			return new PartyVisual();
		}
	}
}
