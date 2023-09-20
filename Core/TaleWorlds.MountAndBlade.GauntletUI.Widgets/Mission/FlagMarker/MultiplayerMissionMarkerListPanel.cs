using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.FlagMarker
{
	// Token: 0x020000E2 RID: 226
	public class MultiplayerMissionMarkerListPanel : ListPanel
	{
		// Token: 0x17000435 RID: 1077
		// (get) Token: 0x06000BC1 RID: 3009 RVA: 0x00020B4C File Offset: 0x0001ED4C
		// (set) Token: 0x06000BC2 RID: 3010 RVA: 0x00020B54 File Offset: 0x0001ED54
		public float FarAlphaTarget { get; set; } = 0.2f;

		// Token: 0x17000436 RID: 1078
		// (get) Token: 0x06000BC3 RID: 3011 RVA: 0x00020B5D File Offset: 0x0001ED5D
		// (set) Token: 0x06000BC4 RID: 3012 RVA: 0x00020B65 File Offset: 0x0001ED65
		public float FarDistanceCutoff { get; set; } = 50f;

		// Token: 0x17000437 RID: 1079
		// (get) Token: 0x06000BC5 RID: 3013 RVA: 0x00020B6E File Offset: 0x0001ED6E
		// (set) Token: 0x06000BC6 RID: 3014 RVA: 0x00020B76 File Offset: 0x0001ED76
		public float CloseDistanceCutoff { get; set; } = 25f;

		// Token: 0x06000BC7 RID: 3015 RVA: 0x00020B7F File Offset: 0x0001ED7F
		public MultiplayerMissionMarkerListPanel(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000BC8 RID: 3016 RVA: 0x00020BAC File Offset: 0x0001EDAC
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			float num = MathF.Clamp(dt * 12f, 0f, 1f);
			if (!this._initialized)
			{
				this.SetInitialAlphaValuesOnCreation();
				this._initialized = true;
			}
			if (this.IsMarkerEnabled)
			{
				using (IEnumerator<Widget> enumerator = base.AllChildrenAndThis.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Widget widget = enumerator.Current;
						bool flag;
						if (widget != this && widget != this._activeWidget)
						{
							Widget activeWidget = this._activeWidget;
							flag = activeWidget != null && activeWidget.CheckIsMyChildRecursive(widget);
						}
						else
						{
							flag = true;
						}
						if (flag)
						{
							float distanceRelatedAlphaTarget = this.GetDistanceRelatedAlphaTarget(this.Distance);
							if (widget == this.SpawnFlagIconWidget)
							{
								widget.SetAlpha(this.IsSpawnFlag ? this.LocalLerp(widget.AlphaFactor, distanceRelatedAlphaTarget, num) : 0f);
							}
							else
							{
								widget.SetAlpha(this.LocalLerp(widget.AlphaFactor, distanceRelatedAlphaTarget, num));
							}
							if (widget != this.RemovalTimeVisiblityWidget)
							{
								widget.IsVisible = (double)widget.AlphaFactor > 0.05;
							}
						}
						else if (widget != this.RemovalTimeVisiblityWidget)
						{
							widget.IsVisible = false;
						}
					}
					goto IL_1F6;
				}
			}
			foreach (Widget widget2 in base.AllChildrenAndThis)
			{
				bool flag2;
				if (widget2 != this && widget2 != this._activeWidget)
				{
					Widget activeWidget2 = this._activeWidget;
					flag2 = activeWidget2 != null && activeWidget2.CheckIsMyChildRecursive(widget2);
				}
				else
				{
					flag2 = true;
				}
				if (flag2)
				{
					if (widget2 == this.SpawnFlagIconWidget)
					{
						widget2.SetAlpha(this.IsSpawnFlag ? this.LocalLerp(widget2.AlphaFactor, 0f, num) : 0f);
					}
					else
					{
						widget2.SetAlpha(this.LocalLerp(widget2.AlphaFactor, 0f, num));
					}
					if (widget2 != this.RemovalTimeVisiblityWidget)
					{
						widget2.IsVisible = (double)widget2.AlphaFactor > 0.05;
					}
				}
				else if (widget2 != this.RemovalTimeVisiblityWidget)
				{
					widget2.IsVisible = false;
				}
			}
			IL_1F6:
			Widget activeWidget3 = this._activeWidget;
			if (activeWidget3 != null && activeWidget3.IsVisible)
			{
				if (this._activeMarkerType == MultiplayerMissionMarkerListPanel.MissionMarkerType.Flag)
				{
					float x = base.Context.EventManager.PageSize.X;
					float y = base.Context.EventManager.PageSize.Y;
					base.ScaledPositionXOffset = MathF.Clamp(this.Position.x - base.Size.X / 2f, 10f, x - base.Size.X - 10f);
					base.ScaledPositionYOffset = MathF.Clamp(this.Position.y - base.Size.Y / 2f, 10f, y - base.Size.Y - 10f);
					return;
				}
				base.ScaledPositionYOffset = this.Position.y - base.Size.Y / 2f;
				base.ScaledPositionXOffset = this.Position.x - base.Size.X / 2f;
			}
		}

		// Token: 0x06000BC9 RID: 3017 RVA: 0x00020EE4 File Offset: 0x0001F0E4
		private float GetDistanceRelatedAlphaTarget(int distance)
		{
			if ((float)distance > this.FarDistanceCutoff)
			{
				return this.FarAlphaTarget;
			}
			if ((float)distance <= this.FarDistanceCutoff && (float)distance >= this.CloseDistanceCutoff)
			{
				float num = (float)Math.Pow((double)(((float)distance - this.CloseDistanceCutoff) / (this.FarDistanceCutoff - this.CloseDistanceCutoff)), 0.3333333333333333);
				return MathF.Clamp(MathF.Lerp(1f, this.FarAlphaTarget, num, 1E-05f), this.FarAlphaTarget, 1f);
			}
			return 1f;
		}

		// Token: 0x06000BCA RID: 3018 RVA: 0x00020F6C File Offset: 0x0001F16C
		private void SetInitialAlphaValuesOnCreation()
		{
			if (this.IsMarkerEnabled)
			{
				using (IEnumerator<Widget> enumerator = base.AllChildrenAndThis.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Widget widget = enumerator.Current;
						bool flag;
						if (widget != this && widget != this._activeWidget)
						{
							Widget activeWidget = this._activeWidget;
							flag = activeWidget != null && activeWidget.CheckIsMyChildRecursive(widget);
						}
						else
						{
							flag = true;
						}
						if (flag)
						{
							if (widget == this.SpawnFlagIconWidget)
							{
								widget.SetAlpha((float)(this.IsSpawnFlag ? 1 : 0));
							}
							else
							{
								widget.SetAlpha(1f);
							}
							if (widget != this.RemovalTimeVisiblityWidget)
							{
								widget.IsVisible = (double)widget.AlphaFactor > 0.05;
							}
						}
						else if (widget != this.RemovalTimeVisiblityWidget)
						{
							widget.IsVisible = false;
						}
					}
					return;
				}
			}
			foreach (Widget widget2 in base.AllChildrenAndThis)
			{
				bool flag2;
				if (widget2 != this && widget2 != this._activeWidget)
				{
					Widget activeWidget2 = this._activeWidget;
					flag2 = activeWidget2 != null && activeWidget2.CheckIsMyChildRecursive(widget2);
				}
				else
				{
					flag2 = true;
				}
				if (flag2)
				{
					widget2.SetAlpha(0f);
					if (widget2 != this.RemovalTimeVisiblityWidget)
					{
						widget2.IsVisible = false;
					}
				}
				else if (widget2 != this.RemovalTimeVisiblityWidget)
				{
					widget2.IsVisible = false;
				}
			}
		}

		// Token: 0x06000BCB RID: 3019 RVA: 0x000210D0 File Offset: 0x0001F2D0
		private float LocalLerp(float start, float end, float delta)
		{
			if (Math.Abs(start - end) > 1E-45f)
			{
				return (end - start) * delta + start;
			}
			return end;
		}

		// Token: 0x06000BCC RID: 3020 RVA: 0x000210EC File Offset: 0x0001F2EC
		private void MarkerTypeUpdated()
		{
			this._activeMarkerType = (MultiplayerMissionMarkerListPanel.MissionMarkerType)this.MarkerType;
			switch (this._activeMarkerType)
			{
			case MultiplayerMissionMarkerListPanel.MissionMarkerType.Flag:
				this._activeWidget = this.FlagWidget;
				return;
			case MultiplayerMissionMarkerListPanel.MissionMarkerType.Peer:
				this._activeWidget = this.PeerWidget;
				return;
			case MultiplayerMissionMarkerListPanel.MissionMarkerType.SiegeEngine:
				this._activeWidget = this.SiegeEngineWidget;
				return;
			default:
				return;
			}
		}

		// Token: 0x17000438 RID: 1080
		// (get) Token: 0x06000BCD RID: 3021 RVA: 0x00021145 File Offset: 0x0001F345
		// (set) Token: 0x06000BCE RID: 3022 RVA: 0x0002114D File Offset: 0x0001F34D
		public Widget FlagWidget
		{
			get
			{
				return this._flagWidget;
			}
			set
			{
				if (this._flagWidget != value)
				{
					this._flagWidget = value;
					base.OnPropertyChanged<Widget>(value, "FlagWidget");
					this.MarkerTypeUpdated();
				}
			}
		}

		// Token: 0x17000439 RID: 1081
		// (get) Token: 0x06000BCF RID: 3023 RVA: 0x00021171 File Offset: 0x0001F371
		// (set) Token: 0x06000BD0 RID: 3024 RVA: 0x00021179 File Offset: 0x0001F379
		public Widget RemovalTimeVisiblityWidget
		{
			get
			{
				return this._removalTimeVisiblityWidget;
			}
			set
			{
				if (this._removalTimeVisiblityWidget != value)
				{
					this._removalTimeVisiblityWidget = value;
					base.OnPropertyChanged<Widget>(value, "RemovalTimeVisiblityWidget");
				}
			}
		}

		// Token: 0x1700043A RID: 1082
		// (get) Token: 0x06000BD1 RID: 3025 RVA: 0x00021197 File Offset: 0x0001F397
		// (set) Token: 0x06000BD2 RID: 3026 RVA: 0x0002119F File Offset: 0x0001F39F
		public Widget SpawnFlagIconWidget
		{
			get
			{
				return this._spawnFlagIconWidget;
			}
			set
			{
				if (this._spawnFlagIconWidget != value)
				{
					this._spawnFlagIconWidget = value;
					base.OnPropertyChanged<Widget>(value, "SpawnFlagIconWidget");
				}
			}
		}

		// Token: 0x1700043B RID: 1083
		// (get) Token: 0x06000BD3 RID: 3027 RVA: 0x000211BD File Offset: 0x0001F3BD
		// (set) Token: 0x06000BD4 RID: 3028 RVA: 0x000211C5 File Offset: 0x0001F3C5
		public Widget PeerWidget
		{
			get
			{
				return this._peerWidget;
			}
			set
			{
				if (this._peerWidget != value)
				{
					this._peerWidget = value;
					base.OnPropertyChanged<Widget>(value, "PeerWidget");
					this.MarkerTypeUpdated();
				}
			}
		}

		// Token: 0x1700043C RID: 1084
		// (get) Token: 0x06000BD5 RID: 3029 RVA: 0x000211E9 File Offset: 0x0001F3E9
		// (set) Token: 0x06000BD6 RID: 3030 RVA: 0x000211F1 File Offset: 0x0001F3F1
		public Widget SiegeEngineWidget
		{
			get
			{
				return this._siegeEngineWidget;
			}
			set
			{
				if (value != this._siegeEngineWidget)
				{
					this._siegeEngineWidget = value;
					base.OnPropertyChanged<Widget>(value, "SiegeEngineWidget");
					this.MarkerTypeUpdated();
				}
			}
		}

		// Token: 0x1700043D RID: 1085
		// (get) Token: 0x06000BD7 RID: 3031 RVA: 0x00021215 File Offset: 0x0001F415
		// (set) Token: 0x06000BD8 RID: 3032 RVA: 0x0002121D File Offset: 0x0001F41D
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
					base.OnPropertyChanged(value, "Position");
				}
			}
		}

		// Token: 0x1700043E RID: 1086
		// (get) Token: 0x06000BD9 RID: 3033 RVA: 0x00021240 File Offset: 0x0001F440
		// (set) Token: 0x06000BDA RID: 3034 RVA: 0x00021248 File Offset: 0x0001F448
		public int Distance
		{
			get
			{
				return this._distance;
			}
			set
			{
				if (this._distance != value)
				{
					this._distance = value;
					base.OnPropertyChanged(value, "Distance");
				}
			}
		}

		// Token: 0x1700043F RID: 1087
		// (get) Token: 0x06000BDB RID: 3035 RVA: 0x00021266 File Offset: 0x0001F466
		// (set) Token: 0x06000BDC RID: 3036 RVA: 0x0002126E File Offset: 0x0001F46E
		public bool IsMarkerEnabled
		{
			get
			{
				return this._isMarkerEnabled;
			}
			set
			{
				if (this._isMarkerEnabled != value)
				{
					this._isMarkerEnabled = value;
					base.OnPropertyChanged(value, "IsMarkerEnabled");
				}
			}
		}

		// Token: 0x17000440 RID: 1088
		// (get) Token: 0x06000BDD RID: 3037 RVA: 0x0002128C File Offset: 0x0001F48C
		// (set) Token: 0x06000BDE RID: 3038 RVA: 0x00021294 File Offset: 0x0001F494
		public bool IsSpawnFlag
		{
			get
			{
				return this._isSpawnFlag;
			}
			set
			{
				if (this._isSpawnFlag != value)
				{
					this._isSpawnFlag = value;
					base.OnPropertyChanged(value, "IsSpawnFlag");
				}
			}
		}

		// Token: 0x17000441 RID: 1089
		// (get) Token: 0x06000BDF RID: 3039 RVA: 0x000212B2 File Offset: 0x0001F4B2
		// (set) Token: 0x06000BE0 RID: 3040 RVA: 0x000212BA File Offset: 0x0001F4BA
		public int MarkerType
		{
			get
			{
				return this._markerType;
			}
			set
			{
				if (this._markerType != value)
				{
					this._markerType = value;
					base.OnPropertyChanged(value, "MarkerType");
					this.MarkerTypeUpdated();
				}
			}
		}

		// Token: 0x04000568 RID: 1384
		private const int FlagMarkerEdgeMargin = 10;

		// Token: 0x0400056C RID: 1388
		private MultiplayerMissionMarkerListPanel.MissionMarkerType _activeMarkerType;

		// Token: 0x0400056D RID: 1389
		private Widget _activeWidget;

		// Token: 0x0400056E RID: 1390
		private bool _initialized;

		// Token: 0x0400056F RID: 1391
		private int _distance;

		// Token: 0x04000570 RID: 1392
		private Widget _flagWidget;

		// Token: 0x04000571 RID: 1393
		private Widget _peerWidget;

		// Token: 0x04000572 RID: 1394
		private Widget _siegeEngineWidget;

		// Token: 0x04000573 RID: 1395
		private Widget _spawnFlagIconWidget;

		// Token: 0x04000574 RID: 1396
		private Vec2 _position;

		// Token: 0x04000575 RID: 1397
		private bool _isMarkerEnabled;

		// Token: 0x04000576 RID: 1398
		private bool _isSpawnFlag;

		// Token: 0x04000577 RID: 1399
		private int _markerType;

		// Token: 0x04000578 RID: 1400
		private Widget _removalTimeVisiblityWidget;

		// Token: 0x02000192 RID: 402
		public enum MissionMarkerType
		{
			// Token: 0x040008FE RID: 2302
			Flag,
			// Token: 0x040008FF RID: 2303
			Peer,
			// Token: 0x04000900 RID: 2304
			SiegeEngine
		}
	}
}
