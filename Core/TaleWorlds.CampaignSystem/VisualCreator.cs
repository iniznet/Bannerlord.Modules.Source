using System;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;

namespace TaleWorlds.CampaignSystem
{
	public class VisualCreator
	{
		public IPartyVisualCreator PartyVisualCreator
		{
			get
			{
				return this._partyVisualCreator;
			}
			set
			{
				this._partyVisualCreator = value;
			}
		}

		public IPartyVisual CreatePartyVisual()
		{
			IPartyVisualCreator partyVisualCreator = this.PartyVisualCreator;
			if (partyVisualCreator == null)
			{
				return null;
			}
			return partyVisualCreator.CreatePartyVisual();
		}

		public IMapEventVisualCreator MapEventVisualCreator
		{
			get
			{
				return this._mapEventVisualCreator;
			}
			set
			{
				this._mapEventVisualCreator = value;
			}
		}

		public IMapEventVisual CreateMapEventVisual(MapEvent mapEvent)
		{
			IMapEventVisualCreator mapEventVisualCreator = this.MapEventVisualCreator;
			if (mapEventVisualCreator == null)
			{
				return null;
			}
			return mapEventVisualCreator.CreateMapEventVisual(mapEvent);
		}

		private IPartyVisualCreator _partyVisualCreator;

		private IMapEventVisualCreator _mapEventVisualCreator;
	}
}
