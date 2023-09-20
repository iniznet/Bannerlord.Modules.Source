using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Menu.TownManagement
{
	// Token: 0x020000EA RID: 234
	public class DevelopmentItemButtonWidget : ButtonWidget
	{
		// Token: 0x06000C15 RID: 3093 RVA: 0x00021C2D File Offset: 0x0001FE2D
		public DevelopmentItemButtonWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000C16 RID: 3094 RVA: 0x00021C38 File Offset: 0x0001FE38
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

		// Token: 0x06000C17 RID: 3095 RVA: 0x00021DD6 File Offset: 0x0001FFD6
		private void HandleFocus()
		{
			if (base.EventManager.LatestMouseUpWidget != base.ParentWidget && !base.ParentWidget.CheckIsMyChildRecursive(base.EventManager.LatestMouseUpWidget))
			{
				this.IsSelectedItem = false;
			}
		}

		// Token: 0x06000C18 RID: 3096 RVA: 0x00021E0C File Offset: 0x0002000C
		private void HandleChildrenAlphaValues()
		{
			this.SetAsActiveButtonWidget.Brush.AlphaFactor = (float)(this.IsSelectedItem ? 1 : 0);
			this.AddToQueueButtonWidget.Brush.AlphaFactor = (float)(this.IsSelectedItem ? 1 : 0);
			this.SelectedBlackOverlayWidget.AlphaFactor = (this.IsSelectedItem ? 0.7f : 0f);
		}

		// Token: 0x06000C19 RID: 3097 RVA: 0x00021E72 File Offset: 0x00020072
		private void HandleEnabledStates()
		{
			base.ParentWidget.DoNotPassEventsToChildren = !this.IsSelectedItem;
			base.DoNotPassEventsToChildren = !this.IsSelectedItem;
		}

		// Token: 0x06000C1A RID: 3098 RVA: 0x00021E97 File Offset: 0x00020097
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

		// Token: 0x06000C1B RID: 3099 RVA: 0x00021EC8 File Offset: 0x000200C8
		private void OnAddToQueueClick(Widget widget)
		{
			this.IsSelectedItem = false;
			base.EventFired("OnAddToQueue", Array.Empty<object>());
		}

		// Token: 0x06000C1C RID: 3100 RVA: 0x00021EE1 File Offset: 0x000200E1
		private void OnSetAsActiveDevelopmentClick(Widget widget)
		{
			this.IsSelectedItem = false;
			base.EventFired("SetAsActive", Array.Empty<object>());
		}

		// Token: 0x06000C1D RID: 3101 RVA: 0x00021EFA File Offset: 0x000200FA
		private void UpdateDevelopmentLevelVisual(int level)
		{
			if (!this.IsDaily)
			{
				this.DevelopmentLevelVisualWidget.SetState(level.ToString());
				this.DevelopmentLevelVisualWidget.IsVisible = level >= 0;
			}
		}

		// Token: 0x17000451 RID: 1105
		// (get) Token: 0x06000C1E RID: 3102 RVA: 0x00021F28 File Offset: 0x00020128
		// (set) Token: 0x06000C1F RID: 3103 RVA: 0x00021F30 File Offset: 0x00020130
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

		// Token: 0x17000452 RID: 1106
		// (get) Token: 0x06000C20 RID: 3104 RVA: 0x00021F81 File Offset: 0x00020181
		// (set) Token: 0x06000C21 RID: 3105 RVA: 0x00021F89 File Offset: 0x00020189
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

		// Token: 0x17000453 RID: 1107
		// (get) Token: 0x06000C22 RID: 3106 RVA: 0x00021FA7 File Offset: 0x000201A7
		// (set) Token: 0x06000C23 RID: 3107 RVA: 0x00021FAF File Offset: 0x000201AF
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

		// Token: 0x17000454 RID: 1108
		// (get) Token: 0x06000C24 RID: 3108 RVA: 0x00021FCD File Offset: 0x000201CD
		// (set) Token: 0x06000C25 RID: 3109 RVA: 0x00021FD5 File Offset: 0x000201D5
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

		// Token: 0x17000455 RID: 1109
		// (get) Token: 0x06000C26 RID: 3110 RVA: 0x00021FF3 File Offset: 0x000201F3
		// (set) Token: 0x06000C27 RID: 3111 RVA: 0x00021FFB File Offset: 0x000201FB
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

		// Token: 0x17000456 RID: 1110
		// (get) Token: 0x06000C28 RID: 3112 RVA: 0x00022019 File Offset: 0x00020219
		// (set) Token: 0x06000C29 RID: 3113 RVA: 0x00022021 File Offset: 0x00020221
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

		// Token: 0x17000457 RID: 1111
		// (get) Token: 0x06000C2A RID: 3114 RVA: 0x00022056 File Offset: 0x00020256
		// (set) Token: 0x06000C2B RID: 3115 RVA: 0x0002205E File Offset: 0x0002025E
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

		// Token: 0x17000458 RID: 1112
		// (get) Token: 0x06000C2C RID: 3116 RVA: 0x00022093 File Offset: 0x00020293
		// (set) Token: 0x06000C2D RID: 3117 RVA: 0x0002209B File Offset: 0x0002029B
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

		// Token: 0x17000459 RID: 1113
		// (get) Token: 0x06000C2E RID: 3118 RVA: 0x000220C5 File Offset: 0x000202C5
		// (set) Token: 0x06000C2F RID: 3119 RVA: 0x000220CD File Offset: 0x000202CD
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

		// Token: 0x1700045A RID: 1114
		// (get) Token: 0x06000C30 RID: 3120 RVA: 0x000220EB File Offset: 0x000202EB
		// (set) Token: 0x06000C31 RID: 3121 RVA: 0x000220F3 File Offset: 0x000202F3
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

		// Token: 0x1700045B RID: 1115
		// (get) Token: 0x06000C32 RID: 3122 RVA: 0x00022111 File Offset: 0x00020311
		// (set) Token: 0x06000C33 RID: 3123 RVA: 0x00022119 File Offset: 0x00020319
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

		// Token: 0x1700045C RID: 1116
		// (get) Token: 0x06000C34 RID: 3124 RVA: 0x00022137 File Offset: 0x00020337
		// (set) Token: 0x06000C35 RID: 3125 RVA: 0x0002213F File Offset: 0x0002033F
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

		// Token: 0x1700045D RID: 1117
		// (get) Token: 0x06000C36 RID: 3126 RVA: 0x0002215D File Offset: 0x0002035D
		// (set) Token: 0x06000C37 RID: 3127 RVA: 0x00022165 File Offset: 0x00020365
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

		// Token: 0x1700045E RID: 1118
		// (get) Token: 0x06000C38 RID: 3128 RVA: 0x00022183 File Offset: 0x00020383
		// (set) Token: 0x06000C39 RID: 3129 RVA: 0x0002218B File Offset: 0x0002038B
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

		// Token: 0x1700045F RID: 1119
		// (get) Token: 0x06000C3A RID: 3130 RVA: 0x000221A9 File Offset: 0x000203A9
		// (set) Token: 0x06000C3B RID: 3131 RVA: 0x000221B1 File Offset: 0x000203B1
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

		// Token: 0x17000460 RID: 1120
		// (get) Token: 0x06000C3C RID: 3132 RVA: 0x000221CF File Offset: 0x000203CF
		// (set) Token: 0x06000C3D RID: 3133 RVA: 0x000221D7 File Offset: 0x000203D7
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

		// Token: 0x17000461 RID: 1121
		// (get) Token: 0x06000C3E RID: 3134 RVA: 0x000221FC File Offset: 0x000203FC
		// (set) Token: 0x06000C3F RID: 3135 RVA: 0x00022204 File Offset: 0x00020404
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

		// Token: 0x04000593 RID: 1427
		private bool _isParentInitialized;

		// Token: 0x04000594 RID: 1428
		private bool _isSelectedItem;

		// Token: 0x04000595 RID: 1429
		private int _level;

		// Token: 0x04000596 RID: 1430
		private int _progress;

		// Token: 0x04000597 RID: 1431
		private bool _isDaily;

		// Token: 0x04000598 RID: 1432
		private bool _isProgressIndicatorsEnabled;

		// Token: 0x04000599 RID: 1433
		private Widget _developmentLevelVisualWidget;

		// Token: 0x0400059A RID: 1434
		private Widget _developmentBackVisualWidget;

		// Token: 0x0400059B RID: 1435
		private Widget _developmentFrontVisualWidget;

		// Token: 0x0400059C RID: 1436
		private Widget _selectedBlackOverlayWidget;

		// Token: 0x0400059D RID: 1437
		private NavigationScopeTargeter _actionButtonsNavigationScopeTargeter;

		// Token: 0x0400059E RID: 1438
		private NavigationForcedScopeCollectionTargeter _actionButtonsForcedScopeTargeter;

		// Token: 0x0400059F RID: 1439
		private ButtonWidget _addToQueueButtonWidget;

		// Token: 0x040005A0 RID: 1440
		private ButtonWidget _setAsActiveButtonWidget;

		// Token: 0x040005A1 RID: 1441
		private DevelopmentNameTextWidget _nameTextWidget;

		// Token: 0x040005A2 RID: 1442
		private Widget _progressClipWidget;

		// Token: 0x040005A3 RID: 1443
		private bool _isProgressShown;

		// Token: 0x040005A4 RID: 1444
		private bool _canBuild;
	}
}
