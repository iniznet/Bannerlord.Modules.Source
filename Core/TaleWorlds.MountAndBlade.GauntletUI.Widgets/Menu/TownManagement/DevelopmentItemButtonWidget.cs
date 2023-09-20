using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Menu.TownManagement
{
	public class DevelopmentItemButtonWidget : ButtonWidget
	{
		public DevelopmentItemButtonWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			ButtonWidget buttonWidget;
			if (!this._isParentInitialized && (buttonWidget = base.ParentWidget as ButtonWidget) != null)
			{
				buttonWidget.ClickEventHandlers.Add(new Action<Widget>(this.OnParentClick));
				this._isParentInitialized = true;
			}
			if (!this.IsDaily)
			{
				this.HandleFocus();
				this.HandleEnabledStates();
				this.DevelopmentFrontVisualWidget.HeightSizePolicy = SizePolicy.Fixed;
				this.DevelopmentFrontVisualWidget.WidthSizePolicy = SizePolicy.Fixed;
				this.DevelopmentFrontVisualWidget.ScaledSuggestedHeight = this.DevelopmentBackVisualWidget.Size.Y;
				this.DevelopmentFrontVisualWidget.ScaledSuggestedWidth = this.DevelopmentBackVisualWidget.Size.X;
				if (this.IsProgressShown)
				{
					if (this.Progress > 0 || this.Level == 0)
					{
						this.ProgressClipWidget.HeightSizePolicy = SizePolicy.Fixed;
						this.ProgressClipWidget.ScaledSuggestedHeight = this.DevelopmentBackVisualWidget.Size.Y * ((float)this.Progress / 100f);
					}
					if (this.Level == 0)
					{
						this.DevelopmentBackVisualWidget.AlphaFactor = 0.8f;
						this.DevelopmentBackVisualWidget.SaturationFactor = -80f;
					}
					else
					{
						this.DevelopmentBackVisualWidget.AlphaFactor = 0.2f;
					}
				}
				else
				{
					this.ProgressClipWidget.HeightSizePolicy = SizePolicy.StretchToParent;
				}
				this.HandleChildrenAlphaValues();
			}
			this.DevelopmentBackVisualWidget.CircularClipEnabled = true;
			this.DevelopmentBackVisualWidget.CircularClipRadius = this.DevelopmentBackVisualWidget.Size.X / 2f * base._inverseScaleToUse - 10f * base._scaleToUse;
			this.DevelopmentBackVisualWidget.CircularClipSmoothingRadius = 3f;
		}

		private void HandleFocus()
		{
			if (base.EventManager.LatestMouseUpWidget != base.ParentWidget && !base.ParentWidget.CheckIsMyChildRecursive(base.EventManager.LatestMouseUpWidget))
			{
				this.IsSelectedItem = false;
			}
		}

		private void HandleChildrenAlphaValues()
		{
			this.SetAsActiveButtonWidget.Brush.AlphaFactor = (float)(this.IsSelectedItem ? 1 : 0);
			this.AddToQueueButtonWidget.Brush.AlphaFactor = (float)(this.IsSelectedItem ? 1 : 0);
			this.SelectedBlackOverlayWidget.AlphaFactor = (this.IsSelectedItem ? 0.7f : 0f);
		}

		private void HandleEnabledStates()
		{
			base.ParentWidget.DoNotPassEventsToChildren = !this.IsSelectedItem;
			base.DoNotPassEventsToChildren = !this.IsSelectedItem;
		}

		private void OnParentClick(Widget widget)
		{
			if (!this.IsSelectedItem && this.CanBuild)
			{
				this.IsSelectedItem = true;
			}
			if (!this.CanBuild)
			{
				DevelopmentNameTextWidget nameTextWidget = this.NameTextWidget;
				if (nameTextWidget == null)
				{
					return;
				}
				nameTextWidget.StartMaxTextAnimation();
			}
		}

		private void OnAddToQueueClick(Widget widget)
		{
			this.IsSelectedItem = false;
			base.EventFired("OnAddToQueue", Array.Empty<object>());
		}

		private void OnSetAsActiveDevelopmentClick(Widget widget)
		{
			this.IsSelectedItem = false;
			base.EventFired("SetAsActive", Array.Empty<object>());
		}

		private void UpdateDevelopmentLevelVisual(int level)
		{
			if (!this.IsDaily)
			{
				this.DevelopmentLevelVisualWidget.SetState(level.ToString());
				this.DevelopmentLevelVisualWidget.IsVisible = level >= 0;
			}
		}

		[Editor(false)]
		public bool IsSelectedItem
		{
			get
			{
				return this._isSelectedItem;
			}
			set
			{
				if (this._isSelectedItem != value)
				{
					this._isSelectedItem = value;
					base.OnPropertyChanged(value, "IsSelectedItem");
					if (this.ActionButtonsForcedScopeTargeter != null && this.ActionButtonsNavigationScopeTargeter != null)
					{
						this.ActionButtonsForcedScopeTargeter.IsCollectionEnabled = value;
						this.ActionButtonsNavigationScopeTargeter.IsScopeEnabled = value;
					}
				}
			}
		}

		[Editor(false)]
		public NavigationScopeTargeter ActionButtonsNavigationScopeTargeter
		{
			get
			{
				return this._actionButtonsNavigationScopeTargeter;
			}
			set
			{
				if (this._actionButtonsNavigationScopeTargeter != value)
				{
					this._actionButtonsNavigationScopeTargeter = value;
					base.OnPropertyChanged<NavigationScopeTargeter>(value, "ActionButtonsNavigationScopeTargeter");
				}
			}
		}

		[Editor(false)]
		public NavigationForcedScopeCollectionTargeter ActionButtonsForcedScopeTargeter
		{
			get
			{
				return this._actionButtonsForcedScopeTargeter;
			}
			set
			{
				if (this._actionButtonsForcedScopeTargeter != value)
				{
					this._actionButtonsForcedScopeTargeter = value;
					base.OnPropertyChanged<NavigationForcedScopeCollectionTargeter>(value, "ActionButtonsForcedScopeTargeter");
				}
			}
		}

		[Editor(false)]
		public Widget SelectedBlackOverlayWidget
		{
			get
			{
				return this._selectedBlackOverlayWidget;
			}
			set
			{
				if (this._selectedBlackOverlayWidget != value)
				{
					this._selectedBlackOverlayWidget = value;
					base.OnPropertyChanged<Widget>(value, "SelectedBlackOverlayWidget");
				}
			}
		}

		[Editor(false)]
		public DevelopmentNameTextWidget NameTextWidget
		{
			get
			{
				return this._nameTextWidget;
			}
			set
			{
				if (this._nameTextWidget != value)
				{
					this._nameTextWidget = value;
					base.OnPropertyChanged<DevelopmentNameTextWidget>(value, "NameTextWidget");
				}
			}
		}

		[Editor(false)]
		public ButtonWidget AddToQueueButtonWidget
		{
			get
			{
				return this._addToQueueButtonWidget;
			}
			set
			{
				if (this._addToQueueButtonWidget != value)
				{
					this._addToQueueButtonWidget = value;
					base.OnPropertyChanged<ButtonWidget>(value, "AddToQueueButtonWidget");
					value.ClickEventHandlers.Add(new Action<Widget>(this.OnAddToQueueClick));
				}
			}
		}

		[Editor(false)]
		public ButtonWidget SetAsActiveButtonWidget
		{
			get
			{
				return this._setAsActiveButtonWidget;
			}
			set
			{
				if (this._setAsActiveButtonWidget != value)
				{
					this._setAsActiveButtonWidget = value;
					base.OnPropertyChanged<ButtonWidget>(value, "SetAsActiveButtonWidget");
					value.ClickEventHandlers.Add(new Action<Widget>(this.OnSetAsActiveDevelopmentClick));
				}
			}
		}

		[Editor(false)]
		public Widget DevelopmentLevelVisualWidget
		{
			get
			{
				return this._developmentLevelVisualWidget;
			}
			set
			{
				if (this._developmentLevelVisualWidget != value)
				{
					this._developmentLevelVisualWidget = value;
					base.OnPropertyChanged<Widget>(value, "DevelopmentLevelVisualWidget");
					this.UpdateDevelopmentLevelVisual(this.Level);
				}
			}
		}

		[Editor(false)]
		public Widget ProgressClipWidget
		{
			get
			{
				return this._progressClipWidget;
			}
			set
			{
				if (this._progressClipWidget != value)
				{
					this._progressClipWidget = value;
					base.OnPropertyChanged<Widget>(value, "ProgressClipWidget");
				}
			}
		}

		[Editor(false)]
		public bool IsProgressShown
		{
			get
			{
				return this._isProgressShown;
			}
			set
			{
				if (this._isProgressShown != value)
				{
					this._isProgressShown = value;
					base.OnPropertyChanged(value, "IsProgressShown");
				}
			}
		}

		[Editor(false)]
		public bool CanBuild
		{
			get
			{
				return this._canBuild;
			}
			set
			{
				if (this._canBuild != value)
				{
					this._canBuild = value;
					base.OnPropertyChanged(value, "CanBuild");
				}
			}
		}

		[Editor(false)]
		public Widget DevelopmentBackVisualWidget
		{
			get
			{
				return this._developmentBackVisualWidget;
			}
			set
			{
				if (this._developmentBackVisualWidget != value)
				{
					this._developmentBackVisualWidget = value;
					base.OnPropertyChanged<Widget>(value, "DevelopmentBackVisualWidget");
				}
			}
		}

		[Editor(false)]
		public Widget DevelopmentFrontVisualWidget
		{
			get
			{
				return this._developmentFrontVisualWidget;
			}
			set
			{
				if (this._developmentFrontVisualWidget != value)
				{
					this._developmentFrontVisualWidget = value;
					base.OnPropertyChanged<Widget>(value, "DevelopmentFrontVisualWidget");
				}
			}
		}

		[Editor(false)]
		public bool IsProgressIndicatorsEnabled
		{
			get
			{
				return this._isProgressIndicatorsEnabled;
			}
			set
			{
				if (this._isProgressIndicatorsEnabled != value)
				{
					this._isProgressIndicatorsEnabled = value;
					base.OnPropertyChanged(value, "IsProgressIndicatorsEnabled");
				}
			}
		}

		[Editor(false)]
		public bool IsDaily
		{
			get
			{
				return this._isDaily;
			}
			set
			{
				if (this._isDaily != value)
				{
					this._isDaily = value;
					base.OnPropertyChanged(value, "IsDaily");
				}
			}
		}

		[Editor(false)]
		public int Level
		{
			get
			{
				return this._level;
			}
			set
			{
				if (this._level != value)
				{
					this._level = value;
					base.OnPropertyChanged(value, "Level");
					this.UpdateDevelopmentLevelVisual(value);
				}
			}
		}

		[Editor(false)]
		public int Progress
		{
			get
			{
				return this._progress;
			}
			set
			{
				if (this._progress != value)
				{
					this._progress = value;
					base.OnPropertyChanged(value, "Progress");
				}
			}
		}

		private bool _isParentInitialized;

		private bool _isSelectedItem;

		private int _level;

		private int _progress;

		private bool _isDaily;

		private bool _isProgressIndicatorsEnabled;

		private Widget _developmentLevelVisualWidget;

		private Widget _developmentBackVisualWidget;

		private Widget _developmentFrontVisualWidget;

		private Widget _selectedBlackOverlayWidget;

		private NavigationScopeTargeter _actionButtonsNavigationScopeTargeter;

		private NavigationForcedScopeCollectionTargeter _actionButtonsForcedScopeTargeter;

		private ButtonWidget _addToQueueButtonWidget;

		private ButtonWidget _setAsActiveButtonWidget;

		private DevelopmentNameTextWidget _nameTextWidget;

		private Widget _progressClipWidget;

		private bool _isProgressShown;

		private bool _canBuild;
	}
}
