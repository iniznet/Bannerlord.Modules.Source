using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.FlagMarker
{
	public class MultiplayerMissionMarkerListPanel : ListPanel
	{
		public float FarAlphaTarget { get; set; } = 0.2f;

		public float FarDistanceCutoff { get; set; } = 50f;

		public float CloseDistanceCutoff { get; set; } = 25f;

		public MultiplayerMissionMarkerListPanel(UIContext context)
			: base(context)
		{
		}

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

		private float LocalLerp(float start, float end, float delta)
		{
			if (Math.Abs(start - end) > 1E-45f)
			{
				return (end - start) * delta + start;
			}
			return end;
		}

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

		private const int FlagMarkerEdgeMargin = 10;

		private MultiplayerMissionMarkerListPanel.MissionMarkerType _activeMarkerType;

		private Widget _activeWidget;

		private bool _initialized;

		private int _distance;

		private Widget _flagWidget;

		private Widget _peerWidget;

		private Widget _siegeEngineWidget;

		private Widget _spawnFlagIconWidget;

		private Vec2 _position;

		private bool _isMarkerEnabled;

		private bool _isSpawnFlag;

		private int _markerType;

		private Widget _removalTimeVisiblityWidget;

		public enum MissionMarkerType
		{
			Flag,
			Peer,
			SiegeEngine
		}
	}
}
