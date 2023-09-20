using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace SandBox.ViewModelCollection.Map
{
	// Token: 0x0200002B RID: 43
	public class MapEventVisualsVM : ViewModel
	{
		// Token: 0x06000335 RID: 821 RVA: 0x0000FADB File Offset: 0x0000DCDB
		public MapEventVisualsVM(Camera mapCamera, Func<Vec2, Vec3> getRealPositionOfEvent)
		{
			this._mapCamera = mapCamera;
			this._getRealPositionOfEvent = getRealPositionOfEvent;
			this.MapEvents = new MBBindingList<MapEventVisualItemVM>();
			this.UpdateMapEventsAuxPredicate = new TWParallel.ParallelForAuxPredicate(this.UpdateMapEventsAux);
		}

		// Token: 0x06000336 RID: 822 RVA: 0x0000FB1C File Offset: 0x0000DD1C
		private void UpdateMapEventsAux(int startInclusive, int endExclusive)
		{
			for (int i = startInclusive; i < endExclusive; i++)
			{
				this.MapEvents[i].ParallelUpdatePosition();
				this.MapEvents[i].DetermineIsVisibleOnMap();
			}
		}

		// Token: 0x06000337 RID: 823 RVA: 0x0000FB58 File Offset: 0x0000DD58
		public void Update(float dt)
		{
			TWParallel.For(0, this.MapEvents.Count, this.UpdateMapEventsAuxPredicate, 16);
			for (int i = 0; i < this.MapEvents.Count; i++)
			{
				this.MapEvents[i].UpdateBindingProperties();
			}
		}

		// Token: 0x06000338 RID: 824 RVA: 0x0000FBA5 File Offset: 0x0000DDA5
		public void OnMapEventVisibilityChanged(MapEvent mapEvent)
		{
			if (this._eventToVisualMap.ContainsKey(mapEvent))
			{
				this._eventToVisualMap[mapEvent].UpdateProperties();
			}
		}

		// Token: 0x06000339 RID: 825 RVA: 0x0000FBC8 File Offset: 0x0000DDC8
		public void OnMapEventStarted(MapEvent mapEvent)
		{
			if (!this._eventToVisualMap.ContainsKey(mapEvent))
			{
				if (!this.IsMapEventSettlementRelated(mapEvent))
				{
					MapEventVisualItemVM mapEventVisualItemVM = new MapEventVisualItemVM(this._mapCamera, mapEvent, this._getRealPositionOfEvent);
					this._eventToVisualMap.Add(mapEvent, mapEventVisualItemVM);
					this.MapEvents.Add(mapEventVisualItemVM);
					mapEventVisualItemVM.UpdateProperties();
				}
				return;
			}
			if (!this.IsMapEventSettlementRelated(mapEvent))
			{
				this._eventToVisualMap[mapEvent].UpdateProperties();
				return;
			}
			MapEventVisualItemVM mapEventVisualItemVM2 = this._eventToVisualMap[mapEvent];
			this.MapEvents.Remove(mapEventVisualItemVM2);
			this._eventToVisualMap.Remove(mapEvent);
		}

		// Token: 0x0600033A RID: 826 RVA: 0x0000FC64 File Offset: 0x0000DE64
		public void OnMapEventEnded(MapEvent mapEvent)
		{
			if (this._eventToVisualMap.ContainsKey(mapEvent))
			{
				MapEventVisualItemVM mapEventVisualItemVM = this._eventToVisualMap[mapEvent];
				this.MapEvents.Remove(mapEventVisualItemVM);
				this._eventToVisualMap.Remove(mapEvent);
			}
		}

		// Token: 0x0600033B RID: 827 RVA: 0x0000FCA6 File Offset: 0x0000DEA6
		private bool IsMapEventSettlementRelated(MapEvent mapEvent)
		{
			return mapEvent.MapEventSettlement != null;
		}

		// Token: 0x17000105 RID: 261
		// (get) Token: 0x0600033C RID: 828 RVA: 0x0000FCB1 File Offset: 0x0000DEB1
		// (set) Token: 0x0600033D RID: 829 RVA: 0x0000FCB9 File Offset: 0x0000DEB9
		public MBBindingList<MapEventVisualItemVM> MapEvents
		{
			get
			{
				return this._mapEvents;
			}
			set
			{
				if (this._mapEvents != value)
				{
					this._mapEvents = value;
					base.OnPropertyChangedWithValue<MBBindingList<MapEventVisualItemVM>>(value, "MapEvents");
				}
			}
		}

		// Token: 0x040001A8 RID: 424
		private readonly Camera _mapCamera;

		// Token: 0x040001A9 RID: 425
		private readonly Dictionary<MapEvent, MapEventVisualItemVM> _eventToVisualMap = new Dictionary<MapEvent, MapEventVisualItemVM>();

		// Token: 0x040001AA RID: 426
		private readonly Func<Vec2, Vec3> _getRealPositionOfEvent;

		// Token: 0x040001AB RID: 427
		private readonly TWParallel.ParallelForAuxPredicate UpdateMapEventsAuxPredicate;

		// Token: 0x040001AC RID: 428
		private MBBindingList<MapEventVisualItemVM> _mapEvents;
	}
}
