using System;
using System.Diagnostics;
using System.Numerics;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Nameplate
{
	public class SettlementNameplateWidget : Widget, IComparable<SettlementNameplateWidget>
	{
		public SettlementNameplateWidget(UIContext context)
			: base(context)
		{
			this._positionTimer = new Stopwatch();
		}

		private float _screenEdgeAlphaTarget
		{
			get
			{
				return 1f;
			}
		}

		private float _normalNeutralAlphaTarget
		{
			get
			{
				return 0.35f;
			}
		}

		private float _normalAllyAlphaTarget
		{
			get
			{
				return 0.5f;
			}
		}

		private float _normalEnemyAlphaTarget
		{
			get
			{
				return 0.35f;
			}
		}

		private float _trackedAlphaTarget
		{
			get
			{
				return 0.8f;
			}
		}

		private float _trackedColorFactorTarget
		{
			get
			{
				return 1.3f;
			}
		}

		private float _normalColorFactorTarget
		{
			get
			{
				return 1f;
			}
		}

		protected override void OnParallelUpdate(float dt)
		{
			base.OnParallelUpdate(dt);
			SettlementNameplateItemWidget currentNameplate = this._currentNameplate;
			if (currentNameplate != null)
			{
				currentNameplate.ParallelUpdate(dt);
			}
			if (this._currentNameplate != null && this._cachedItemSize != this._currentNameplate.Size)
			{
				this._cachedItemSize = this._currentNameplate.Size;
				if (this._eventsListPanel != null)
				{
					this._eventsListPanel.ScaledPositionXOffset = this._cachedItemSize.X;
				}
				if (this._notificationListPanel != null)
				{
					this._notificationListPanel.ScaledPositionYOffset = -this._cachedItemSize.Y;
				}
				base.SuggestedWidth = this._cachedItemSize.X * base._inverseScaleToUse;
				base.SuggestedHeight = this._cachedItemSize.Y * base._inverseScaleToUse;
				base.ScaledSuggestedWidth = this._cachedItemSize.X;
				base.ScaledSuggestedHeight = this._cachedItemSize.Y;
			}
			base.IsEnabled = this.IsVisibleOnMap;
			this.UpdateNameplateTransparencyAndBrightness(dt);
			this.UpdatePosition();
			this.UpdateTutorialState();
		}

		private void UpdatePosition()
		{
			bool flag = false;
			if (this.IsVisibleOnMap || (this._positionTimer.IsRunning && this._positionTimer.Elapsed.Seconds < 2))
			{
				float x = base.Context.EventManager.PageSize.X;
				float y = base.Context.EventManager.PageSize.Y;
				Vec2 vec = this.Position;
				if (this.IsTracked)
				{
					if (this.WSign > 0 && vec.x - base.Size.X / 2f > 0f && vec.x + base.Size.X / 2f < x && vec.y > 0f && vec.y + base.Size.Y < y)
					{
						base.ScaledPositionXOffset = vec.x - base.Size.X / 2f;
						base.ScaledPositionYOffset = vec.y - base.Size.Y;
					}
					else
					{
						Vec2 vec2 = new Vec2(x / 2f, y / 2f);
						vec -= vec2;
						if (this.WSign < 0)
						{
							vec *= -1f;
						}
						float num = Mathf.Atan2(vec.y, vec.x) - 1.5707964f;
						float num2 = Mathf.Cos(num);
						float num3 = Mathf.Sin(num);
						float num4 = num2 / num3;
						Vec2 vec3 = vec2 * 1f;
						vec = ((num2 > 0f) ? new Vec2(-vec3.y / num4, vec2.y) : new Vec2(vec3.y / num4, -vec2.y));
						if (vec.x > vec3.x)
						{
							vec = new Vec2(vec3.x, -vec3.x * num4);
						}
						else if (vec.x < -vec3.x)
						{
							vec = new Vec2(-vec3.x, vec3.x * num4);
						}
						vec += vec2;
						flag = vec.y - base.Size.Y - this._currentNameplate.MapEventVisualWidget.Size.Y <= 0f;
						base.ScaledPositionXOffset = Mathf.Clamp(vec.x - base.Size.X / 2f, 0f, x - this._currentNameplate.Size.X);
						base.ScaledPositionYOffset = Mathf.Clamp(vec.y - base.Size.Y, 0f, y - (this._currentNameplate.Size.Y + 55f));
					}
				}
				else
				{
					base.ScaledPositionXOffset = vec.x - base.Size.X / 2f;
					base.ScaledPositionYOffset = vec.y - base.Size.Y;
				}
			}
			if (flag)
			{
				this._currentNameplate.MapEventVisualWidget.VerticalAlignment = VerticalAlignment.Bottom;
				this._currentNameplate.MapEventVisualWidget.ScaledPositionYOffset = this._currentNameplate.MapEventVisualWidget.Size.Y;
				return;
			}
			this._currentNameplate.MapEventVisualWidget.VerticalAlignment = VerticalAlignment.Top;
			this._currentNameplate.MapEventVisualWidget.ScaledPositionYOffset = -this._currentNameplate.MapEventVisualWidget.Size.Y;
		}

		private void OnNotificationListUpdated(Widget widget)
		{
			this._updatePositionNextFrame = true;
			this.AddLateUpdateAction();
		}

		private void OnNotificationListUpdated(Widget parentWidget, Widget addedWidget)
		{
			this._updatePositionNextFrame = true;
			this.AddLateUpdateAction();
		}

		private void AddLateUpdateAction()
		{
			if (!this._lateUpdateActionAdded)
			{
				base.EventManager.AddLateUpdateAction(this, new Action<float>(this.CustomLateUpdate), 1);
				this._lateUpdateActionAdded = true;
			}
		}

		private void CustomLateUpdate(float dt)
		{
			if (this._updatePositionNextFrame)
			{
				this.UpdatePosition();
				this._updatePositionNextFrame = false;
			}
			this._lateUpdateActionAdded = false;
		}

		private void UpdateTutorialState()
		{
			if (this._tutorialAnimState == SettlementNameplateWidget.TutorialAnimState.Start)
			{
				this._tutorialAnimState = SettlementNameplateWidget.TutorialAnimState.FirstFrame;
			}
			else
			{
				SettlementNameplateWidget.TutorialAnimState tutorialAnimState = this._tutorialAnimState;
			}
			if (this.IsTargetedByTutorial)
			{
				this.SetState("Default");
				return;
			}
			this.SetState("Disabled");
		}

		private void SetNameplateTypeVisual(int type)
		{
			if (this._currentNameplate == null)
			{
				this.SmallNameplateWidget.IsVisible = false;
				this.NormalNameplateWidget.IsVisible = false;
				this.BigNameplateWidget.IsVisible = false;
				switch (type)
				{
				case 0:
					this._currentNameplate = this.SmallNameplateWidget;
					this.SmallNameplateWidget.IsVisible = true;
					return;
				case 1:
					this._currentNameplate = this.NormalNameplateWidget;
					this.NormalNameplateWidget.IsVisible = true;
					return;
				case 2:
					this._currentNameplate = this.BigNameplateWidget;
					this.BigNameplateWidget.IsVisible = true;
					break;
				default:
					return;
				}
			}
		}

		private void SetNameplateRelationType(int type)
		{
			if (this._currentNameplate != null)
			{
				switch (type)
				{
				case 0:
					this._currentNameplate.Color = Color.Black;
					return;
				case 1:
					this._currentNameplate.Color = Color.ConvertStringToColor("#245E05FF");
					return;
				case 2:
					this._currentNameplate.Color = Color.ConvertStringToColor("#870707FF");
					break;
				default:
					return;
				}
			}
		}

		private void UpdateNameplateTransparencyAndBrightness(float dt)
		{
			float num = dt * this._lerpModifier;
			if (this.IsVisibleOnMap)
			{
				base.IsVisible = true;
				float num2 = this.DetermineTargetAlphaValue();
				float num3 = this.DetermineTargetColorFactor();
				float num4 = MathF.Lerp(this._currentNameplate.AlphaFactor, num2, num, 1E-05f);
				float num5 = MathF.Lerp(this._currentNameplate.ColorFactor, num3, num, 1E-05f);
				float num6 = MathF.Lerp(this._currentNameplate.SettlementNameTextWidget.ReadOnlyBrush.GlobalAlphaFactor, 1f, num, 1E-05f);
				this._currentNameplate.AlphaFactor = num4;
				this._currentNameplate.ColorFactor = num5;
				this._currentNameplate.SettlementNameTextWidget.Brush.GlobalAlphaFactor = num6;
				this._currentNameplate.SettlementBannerWidget.Brush.GlobalAlphaFactor = num6;
				this._currentNameplate.SettlementPartiesGridWidget.SetGlobalAlphaRecursively(num6);
				this._eventsListPanel.SetGlobalAlphaRecursively(num6);
			}
			else if (this._currentNameplate.AlphaFactor > this._lerpThreshold)
			{
				float num7 = MathF.Lerp(this._currentNameplate.AlphaFactor, 0f, num, 1E-05f);
				this._currentNameplate.AlphaFactor = num7;
				this._currentNameplate.SettlementNameTextWidget.Brush.GlobalAlphaFactor = num7;
				this._currentNameplate.SettlementBannerWidget.Brush.GlobalAlphaFactor = num7;
				this._currentNameplate.SettlementPartiesGridWidget.SetGlobalAlphaRecursively(num7);
				this._eventsListPanel.SetGlobalAlphaRecursively(num7);
			}
			else
			{
				base.IsVisible = false;
			}
			Widget settlementNameplateInspectedWidget = this._currentNameplate.SettlementNameplateInspectedWidget;
			if (this.IsInRange && this.IsVisibleOnMap)
			{
				if (Math.Abs(settlementNameplateInspectedWidget.AlphaFactor - 1f) > this._lerpThreshold)
				{
					settlementNameplateInspectedWidget.AlphaFactor = MathF.Lerp(settlementNameplateInspectedWidget.AlphaFactor, 1f, num, 1E-05f);
					return;
				}
			}
			else if (this._currentNameplate.AlphaFactor - 0f > this._lerpThreshold)
			{
				settlementNameplateInspectedWidget.AlphaFactor = MathF.Lerp(settlementNameplateInspectedWidget.AlphaFactor, 0f, num, 1E-05f);
			}
		}

		private float DetermineTargetAlphaValue()
		{
			if (this.IsInsideWindow)
			{
				if (this.IsTracked)
				{
					return this._trackedAlphaTarget;
				}
				if (this.RelationType == 0)
				{
					return this._normalNeutralAlphaTarget;
				}
				if (this.RelationType == 1)
				{
					return this._normalAllyAlphaTarget;
				}
				return this._normalEnemyAlphaTarget;
			}
			else
			{
				if (this.IsTracked)
				{
					return this._screenEdgeAlphaTarget;
				}
				return 0f;
			}
		}

		private float DetermineTargetColorFactor()
		{
			if (this.IsTracked)
			{
				return this._trackedColorFactorTarget;
			}
			return this._normalColorFactorTarget;
		}

		public int CompareTo(SettlementNameplateWidget other)
		{
			return other.DistanceToCamera.CompareTo(this.DistanceToCamera);
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
					if (this._isVisibleOnMap && !value)
					{
						this._positionTimer.Restart();
					}
					this._isVisibleOnMap = value;
					base.OnPropertyChanged(value, "IsVisibleOnMap");
				}
			}
		}

		public bool IsTracked
		{
			get
			{
				return this._isTracked;
			}
			set
			{
				if (this._isTracked != value)
				{
					this._isTracked = value;
					base.OnPropertyChanged(value, "IsTracked");
				}
			}
		}

		public bool IsTargetedByTutorial
		{
			get
			{
				return this._isTargetedByTutorial;
			}
			set
			{
				if (this._isTargetedByTutorial != value)
				{
					this._isTargetedByTutorial = value;
					base.OnPropertyChanged(value, "IsTargetedByTutorial");
					if (value)
					{
						this._tutorialAnimState = SettlementNameplateWidget.TutorialAnimState.Start;
					}
				}
			}
		}

		public bool IsInsideWindow
		{
			get
			{
				return this._isInsideWindow;
			}
			set
			{
				if (this._isInsideWindow != value)
				{
					this._isInsideWindow = value;
					base.OnPropertyChanged(value, "IsInsideWindow");
				}
			}
		}

		public bool IsInRange
		{
			get
			{
				return this._isInRange;
			}
			set
			{
				if (this._isInRange != value)
				{
					this._isInRange = value;
				}
			}
		}

		public int NameplateType
		{
			get
			{
				return this._nameplateType;
			}
			set
			{
				if (this._nameplateType != value)
				{
					this._nameplateType = value;
					base.OnPropertyChanged(value, "NameplateType");
					this.SetNameplateTypeVisual(value);
				}
			}
		}

		public int RelationType
		{
			get
			{
				return this._relationType;
			}
			set
			{
				if (this._relationType != value)
				{
					this._relationType = value;
					base.OnPropertyChanged(value, "RelationType");
					this.SetNameplateRelationType(value);
				}
			}
		}

		public int WSign
		{
			get
			{
				return this._wSign;
			}
			set
			{
				if (this._wSign != value)
				{
					this._wSign = value;
					base.OnPropertyChanged(value, "WSign");
				}
			}
		}

		public float WPos
		{
			get
			{
				return this._wPos;
			}
			set
			{
				if (this._wPos != value)
				{
					this._wPos = value;
					base.OnPropertyChanged(value, "WPos");
				}
			}
		}

		public float DistanceToCamera
		{
			get
			{
				return this._distanceToCamera;
			}
			set
			{
				if (this._distanceToCamera != value)
				{
					this._distanceToCamera = value;
					base.OnPropertyChanged(value, "DistanceToCamera");
				}
			}
		}

		public ListPanel NotificationListPanel
		{
			get
			{
				return this._notificationListPanel;
			}
			set
			{
				if (this._notificationListPanel != value)
				{
					this._notificationListPanel = value;
					base.OnPropertyChanged<ListPanel>(value, "NotificationListPanel");
					this._notificationListPanel.ItemAddEventHandlers.Add(new Action<Widget, Widget>(this.OnNotificationListUpdated));
					this._notificationListPanel.ItemAfterRemoveEventHandlers.Add(new Action<Widget>(this.OnNotificationListUpdated));
				}
			}
		}

		public ListPanel EventsListPanel
		{
			get
			{
				return this._eventsListPanel;
			}
			set
			{
				if (value != this._eventsListPanel)
				{
					this._eventsListPanel = value;
					base.OnPropertyChanged<ListPanel>(value, "EventsListPanel");
				}
			}
		}

		public SettlementNameplateItemWidget SmallNameplateWidget
		{
			get
			{
				return this._smallNameplateWidget;
			}
			set
			{
				if (this._smallNameplateWidget != value)
				{
					this._smallNameplateWidget = value;
					base.OnPropertyChanged<SettlementNameplateItemWidget>(value, "SmallNameplateWidget");
				}
			}
		}

		public SettlementNameplateItemWidget NormalNameplateWidget
		{
			get
			{
				return this._normalNameplateWidget;
			}
			set
			{
				if (this._normalNameplateWidget != value)
				{
					this._normalNameplateWidget = value;
					base.OnPropertyChanged<SettlementNameplateItemWidget>(value, "NormalNameplateWidget");
				}
			}
		}

		public SettlementNameplateItemWidget BigNameplateWidget
		{
			get
			{
				return this._bigNameplateWidget;
			}
			set
			{
				if (this._bigNameplateWidget != value)
				{
					this._bigNameplateWidget = value;
					base.OnPropertyChanged<SettlementNameplateItemWidget>(value, "BigNameplateWidget");
				}
			}
		}

		private readonly Stopwatch _positionTimer;

		private SettlementNameplateItemWidget _currentNameplate;

		private bool _updatePositionNextFrame;

		private SettlementNameplateWidget.TutorialAnimState _tutorialAnimState;

		private float _lerpThreshold = 5E-05f;

		private float _lerpModifier = 10f;

		private Vector2 _cachedItemSize;

		private bool _lateUpdateActionAdded;

		private Vec2 _position;

		private bool _isVisibleOnMap;

		private bool _isTracked;

		private bool _isInsideWindow;

		private bool _isTargetedByTutorial;

		private int _nameplateType = -1;

		private int _relationType = -1;

		private int _wSign;

		private float _wPos;

		private float _distanceToCamera;

		private bool _isInRange;

		private SettlementNameplateItemWidget _smallNameplateWidget;

		private SettlementNameplateItemWidget _normalNameplateWidget;

		private SettlementNameplateItemWidget _bigNameplateWidget;

		private ListPanel _notificationListPanel;

		private ListPanel _eventsListPanel;

		public enum TutorialAnimState
		{
			Idle,
			Start,
			FirstFrame,
			Playing
		}
	}
}
