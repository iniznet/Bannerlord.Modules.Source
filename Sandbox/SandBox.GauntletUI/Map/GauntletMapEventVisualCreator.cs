using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.MapEvents;

namespace SandBox.GauntletUI.Map
{
	public class GauntletMapEventVisualCreator : IMapEventVisualCreator
	{
		public IMapEventVisual CreateMapEventVisual(MapEvent mapEvent)
		{
			GauntletMapEventVisual newEventVisual = new GauntletMapEventVisual(mapEvent, new Action<GauntletMapEventVisual>(this.OnMapEventInitialized), new Action<GauntletMapEventVisual>(this.OnMapEventVisibilityChanged), new Action<GauntletMapEventVisual>(this.OnMapEventOver));
			List<IGauntletMapEventVisualHandler> handlers = this.Handlers;
			if (handlers != null)
			{
				handlers.ForEach(delegate(IGauntletMapEventVisualHandler h)
				{
					h.OnNewEventStarted(newEventVisual);
				});
			}
			this._listOfEvents.Add(newEventVisual);
			return newEventVisual;
		}

		private void OnMapEventOver(GauntletMapEventVisual overEvent)
		{
			this._listOfEvents.Remove(overEvent);
			List<IGauntletMapEventVisualHandler> handlers = this.Handlers;
			if (handlers == null)
			{
				return;
			}
			handlers.ForEach(delegate(IGauntletMapEventVisualHandler h)
			{
				h.OnEventEnded(overEvent);
			});
		}

		private void OnMapEventInitialized(GauntletMapEventVisual initializedEvent)
		{
			List<IGauntletMapEventVisualHandler> handlers = this.Handlers;
			if (handlers == null)
			{
				return;
			}
			handlers.ForEach(delegate(IGauntletMapEventVisualHandler h)
			{
				h.OnInitialized(initializedEvent);
			});
		}

		private void OnMapEventVisibilityChanged(GauntletMapEventVisual visibilityChangedEvent)
		{
			List<IGauntletMapEventVisualHandler> handlers = this.Handlers;
			if (handlers == null)
			{
				return;
			}
			handlers.ForEach(delegate(IGauntletMapEventVisualHandler h)
			{
				h.OnEventVisibilityChanged(visibilityChangedEvent);
			});
		}

		public IEnumerable<GauntletMapEventVisual> GetCurrentEvents()
		{
			return this._listOfEvents.AsEnumerable<GauntletMapEventVisual>();
		}

		public List<IGauntletMapEventVisualHandler> Handlers = new List<IGauntletMapEventVisualHandler>();

		private readonly List<GauntletMapEventVisual> _listOfEvents = new List<GauntletMapEventVisual>();
	}
}
