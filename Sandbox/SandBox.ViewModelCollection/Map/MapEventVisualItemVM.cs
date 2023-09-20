using System;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.ViewModelCollection.Map
{
	public class MapEventVisualItemVM : ViewModel
	{
		public MapEvent MapEvent { get; private set; }

		public MapEventVisualItemVM(Camera mapCamera, MapEvent mapEvent, Func<Vec2, Vec3> getRealPositionOfEvent)
		{
			this._mapCamera = mapCamera;
			this._getRealPositionOfEvent = getRealPositionOfEvent;
			this.MapEvent = mapEvent;
			this._mapEventPositionCache = mapEvent.Position;
			this._mapEventRealPosition = this._getRealPositionOfEvent(this._mapEventPositionCache);
		}

		public void UpdateProperties()
		{
			this.EventType = (int)SandBoxUIHelper.GetMapEventVisualTypeFromMapEvent(this.MapEvent);
			this._isAVisibleEvent = this.MapEvent.IsVisible;
		}

		public void ParallelUpdatePosition()
		{
			this._latestX = 0f;
			this._latestY = 0f;
			this._latestW = 0f;
			if (this._mapEventPositionCache != this.MapEvent.Position)
			{
				this._mapEventPositionCache = this.MapEvent.Position;
				this._mapEventRealPosition = this._getRealPositionOfEvent(this._mapEventPositionCache);
			}
			MBWindowManager.WorldToScreenInsideUsableArea(this._mapCamera, this._mapEventRealPosition + new Vec3(0f, 0f, 1.5f, -1f), ref this._latestX, ref this._latestY, ref this._latestW);
			this._bindPosition = new Vec2(this._latestX, this._latestY);
		}

		public void DetermineIsVisibleOnMap()
		{
			this._bindIsVisibleOnMap = this._latestW > 0f && this._mapCamera.Position.z < 200f && this._isAVisibleEvent;
		}

		public void UpdateBindingProperties()
		{
			this.Position = this._bindPosition;
			this.IsVisibleOnMap = this._bindIsVisibleOnMap;
		}

		public Vec2 Position
		{
			get
			{
				return this._position;
			}
			set
			{
				if (this._position != value)
				{
					this._position = value;
					base.OnPropertyChangedWithValue(value, "Position");
				}
			}
		}

		public int EventType
		{
			get
			{
				return this._eventType;
			}
			set
			{
				if (this._eventType != value)
				{
					this._eventType = value;
					base.OnPropertyChangedWithValue(value, "EventType");
				}
			}
		}

		public bool IsVisibleOnMap
		{
			get
			{
				return this._isVisibleOnMap;
			}
			set
			{
				if (this._isVisibleOnMap != value)
				{
					this._isVisibleOnMap = value;
					base.OnPropertyChangedWithValue(value, "IsVisibleOnMap");
				}
			}
		}

		private Camera _mapCamera;

		private bool _isAVisibleEvent;

		private Func<Vec2, Vec3> _getRealPositionOfEvent;

		private Vec2 _mapEventPositionCache;

		private Vec3 _mapEventRealPosition;

		private const float CameraDistanceCutoff = 200f;

		private Vec2 _bindPosition;

		private bool _bindIsVisibleOnMap;

		private float _latestX;

		private float _latestY;

		private float _latestW;

		private Vec2 _position;

		private int _eventType;

		private bool _isVisibleOnMap;
	}
}
