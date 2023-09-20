using System;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.ViewModelCollection.Map
{
	// Token: 0x0200002C RID: 44
	public class MapEventVisualItemVM : ViewModel
	{
		// Token: 0x17000106 RID: 262
		// (get) Token: 0x0600033E RID: 830 RVA: 0x0000FCD7 File Offset: 0x0000DED7
		// (set) Token: 0x0600033F RID: 831 RVA: 0x0000FCDF File Offset: 0x0000DEDF
		public MapEvent MapEvent { get; private set; }

		// Token: 0x06000340 RID: 832 RVA: 0x0000FCE8 File Offset: 0x0000DEE8
		public MapEventVisualItemVM(Camera mapCamera, MapEvent mapEvent, Func<Vec2, Vec3> getRealPositionOfEvent)
		{
			this._mapCamera = mapCamera;
			this._getRealPositionOfEvent = getRealPositionOfEvent;
			this.MapEvent = mapEvent;
			this._mapEventPositionCache = mapEvent.Position;
			this._mapEventRealPosition = this._getRealPositionOfEvent(this._mapEventPositionCache);
		}

		// Token: 0x06000341 RID: 833 RVA: 0x0000FD28 File Offset: 0x0000DF28
		public void UpdateProperties()
		{
			this.EventType = (int)SandBoxUIHelper.GetMapEventVisualTypeFromMapEvent(this.MapEvent);
			this._isAVisibleEvent = this.MapEvent.IsVisible;
		}

		// Token: 0x06000342 RID: 834 RVA: 0x0000FD4C File Offset: 0x0000DF4C
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

		// Token: 0x06000343 RID: 835 RVA: 0x0000FE13 File Offset: 0x0000E013
		public void DetermineIsVisibleOnMap()
		{
			this._bindIsVisibleOnMap = this._latestW > 0f && this._mapCamera.Position.z < 200f && this._isAVisibleEvent;
		}

		// Token: 0x06000344 RID: 836 RVA: 0x0000FE48 File Offset: 0x0000E048
		public void UpdateBindingProperties()
		{
			this.Position = this._bindPosition;
			this.IsVisibleOnMap = this._bindIsVisibleOnMap;
		}

		// Token: 0x17000107 RID: 263
		// (get) Token: 0x06000345 RID: 837 RVA: 0x0000FE62 File Offset: 0x0000E062
		// (set) Token: 0x06000346 RID: 838 RVA: 0x0000FE6A File Offset: 0x0000E06A
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

		// Token: 0x17000108 RID: 264
		// (get) Token: 0x06000347 RID: 839 RVA: 0x0000FE8D File Offset: 0x0000E08D
		// (set) Token: 0x06000348 RID: 840 RVA: 0x0000FE95 File Offset: 0x0000E095
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

		// Token: 0x17000109 RID: 265
		// (get) Token: 0x06000349 RID: 841 RVA: 0x0000FEB3 File Offset: 0x0000E0B3
		// (set) Token: 0x0600034A RID: 842 RVA: 0x0000FEBB File Offset: 0x0000E0BB
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

		// Token: 0x040001AE RID: 430
		private Camera _mapCamera;

		// Token: 0x040001AF RID: 431
		private bool _isAVisibleEvent;

		// Token: 0x040001B0 RID: 432
		private Func<Vec2, Vec3> _getRealPositionOfEvent;

		// Token: 0x040001B1 RID: 433
		private Vec2 _mapEventPositionCache;

		// Token: 0x040001B2 RID: 434
		private Vec3 _mapEventRealPosition;

		// Token: 0x040001B3 RID: 435
		private const float CameraDistanceCutoff = 200f;

		// Token: 0x040001B4 RID: 436
		private Vec2 _bindPosition;

		// Token: 0x040001B5 RID: 437
		private bool _bindIsVisibleOnMap;

		// Token: 0x040001B6 RID: 438
		private float _latestX;

		// Token: 0x040001B7 RID: 439
		private float _latestY;

		// Token: 0x040001B8 RID: 440
		private float _latestW;

		// Token: 0x040001B9 RID: 441
		private Vec2 _position;

		// Token: 0x040001BA RID: 442
		private int _eventType;

		// Token: 0x040001BB RID: 443
		private bool _isVisibleOnMap;
	}
}
