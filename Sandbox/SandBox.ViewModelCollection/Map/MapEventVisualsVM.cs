using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace SandBox.ViewModelCollection.Map
{
	public class MapEventVisualsVM : ViewModel
	{
		public MapEventVisualsVM(Camera mapCamera, Func<Vec2, Vec3> getRealPositionOfEvent)
		{
			this._mapCamera = mapCamera;
			this._getRealPositionOfEvent = getRealPositionOfEvent;
			this.MapEvents = new MBBindingList<MapEventVisualItemVM>();
			this.UpdateMapEventsAuxPredicate = new TWParallel.ParallelForAuxPredicate(this.UpdateMapEventsAux);
		}

		private void UpdateMapEventsAux(int startInclusive, int endExclusive)
		{
			for (int i = startInclusive; i < endExclusive; i++)
			{
				this.MapEvents[i].ParallelUpdatePosition();
				this.MapEvents[i].DetermineIsVisibleOnMap();
			}
		}

		public void Update(float dt)
		{
			TWParallel.For(0, this.MapEvents.Count, this.UpdateMapEventsAuxPredicate, 16);
			for (int i = 0; i < this.MapEvents.Count; i++)
			{
				this.MapEvents[i].UpdateBindingProperties();
			}
		}

		public void OnMapEventVisibilityChanged(MapEvent mapEvent)
		{
			if (this._eventToVisualMap.ContainsKey(mapEvent))
			{
				this._eventToVisualMap[mapEvent].UpdateProperties();
			}
		}

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

		public void OnMapEventEnded(MapEvent mapEvent)
		{
			if (this._eventToVisualMap.ContainsKey(mapEvent))
			{
				MapEventVisualItemVM mapEventVisualItemVM = this._eventToVisualMap[mapEvent];
				this.MapEvents.Remove(mapEventVisualItemVM);
				this._eventToVisualMap.Remove(mapEvent);
			}
		}

		private bool IsMapEventSettlementRelated(MapEvent mapEvent)
		{
			return mapEvent.MapEventSettlement != null;
		}

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

		private readonly Camera _mapCamera;

		private readonly Dictionary<MapEvent, MapEventVisualItemVM> _eventToVisualMap = new Dictionary<MapEvent, MapEventVisualItemVM>();

		private readonly Func<Vec2, Vec3> _getRealPositionOfEvent;

		private readonly TWParallel.ParallelForAuxPredicate UpdateMapEventsAuxPredicate;

		private MBBindingList<MapEventVisualItemVM> _mapEvents;
	}
}
