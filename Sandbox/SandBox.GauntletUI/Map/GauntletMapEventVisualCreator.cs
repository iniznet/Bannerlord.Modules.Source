using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.MapEvents;

namespace SandBox.GauntletUI.Map
{
	// Token: 0x02000028 RID: 40
	public class GauntletMapEventVisualCreator : IMapEventVisualCreator
	{
		// Token: 0x0600017A RID: 378 RVA: 0x0000B710 File Offset: 0x00009910
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

		// Token: 0x0600017B RID: 379 RVA: 0x0000B788 File Offset: 0x00009988
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

		// Token: 0x0600017C RID: 380 RVA: 0x0000B7D0 File Offset: 0x000099D0
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

		// Token: 0x0600017D RID: 381 RVA: 0x0000B808 File Offset: 0x00009A08
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

		// Token: 0x0600017E RID: 382 RVA: 0x0000B83E File Offset: 0x00009A3E
		public IEnumerable<GauntletMapEventVisual> GetCurrentEvents()
		{
			return this._listOfEvents.AsEnumerable<GauntletMapEventVisual>();
		}

		// Token: 0x040000C3 RID: 195
		public List<IGauntletMapEventVisualHandler> Handlers = new List<IGauntletMapEventVisualHandler>();

		// Token: 0x040000C4 RID: 196
		private readonly List<GauntletMapEventVisual> _listOfEvents = new List<GauntletMapEventVisual>();
	}
}
