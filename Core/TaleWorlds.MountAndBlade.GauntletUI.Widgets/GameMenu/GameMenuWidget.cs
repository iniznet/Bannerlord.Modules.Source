using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.GameMenu
{
	// Token: 0x02000132 RID: 306
	public class GameMenuWidget : Widget
	{
		// Token: 0x170005B3 RID: 1459
		// (get) Token: 0x06001023 RID: 4131 RVA: 0x0002D9CE File Offset: 0x0002BBCE
		// (set) Token: 0x06001024 RID: 4132 RVA: 0x0002D9D6 File Offset: 0x0002BBD6
		public int EncounterModeMenuWidth { get; set; }

		// Token: 0x170005B4 RID: 1460
		// (get) Token: 0x06001025 RID: 4133 RVA: 0x0002D9DF File Offset: 0x0002BBDF
		// (set) Token: 0x06001026 RID: 4134 RVA: 0x0002D9E7 File Offset: 0x0002BBE7
		public int EncounterModeMenuHeight { get; set; }

		// Token: 0x170005B5 RID: 1461
		// (get) Token: 0x06001027 RID: 4135 RVA: 0x0002D9F0 File Offset: 0x0002BBF0
		// (set) Token: 0x06001028 RID: 4136 RVA: 0x0002D9F8 File Offset: 0x0002BBF8
		public int EncounterModeMenuMarginTop { get; set; }

		// Token: 0x170005B6 RID: 1462
		// (get) Token: 0x06001029 RID: 4137 RVA: 0x0002DA01 File Offset: 0x0002BC01
		// (set) Token: 0x0600102A RID: 4138 RVA: 0x0002DA09 File Offset: 0x0002BC09
		public int NormalModeMenuWidth { get; set; }

		// Token: 0x170005B7 RID: 1463
		// (get) Token: 0x0600102B RID: 4139 RVA: 0x0002DA12 File Offset: 0x0002BC12
		// (set) Token: 0x0600102C RID: 4140 RVA: 0x0002DA1A File Offset: 0x0002BC1A
		public int NormalModeMenuHeight { get; set; }

		// Token: 0x170005B8 RID: 1464
		// (get) Token: 0x0600102D RID: 4141 RVA: 0x0002DA23 File Offset: 0x0002BC23
		// (set) Token: 0x0600102E RID: 4142 RVA: 0x0002DA2B File Offset: 0x0002BC2B
		public int NormalModeMenuMarginTop { get; set; }

		// Token: 0x170005B9 RID: 1465
		// (get) Token: 0x0600102F RID: 4143 RVA: 0x0002DA34 File Offset: 0x0002BC34
		// (set) Token: 0x06001030 RID: 4144 RVA: 0x0002DA3C File Offset: 0x0002BC3C
		public bool IsOverlayExtended
		{
			get
			{
				return this._isOverlayExtended;
			}
			private set
			{
				if (value != this._isOverlayExtended)
				{
					this._isOverlayExtended = value;
					this.UpdateOverlayState();
				}
			}
		}

		// Token: 0x06001031 RID: 4145 RVA: 0x0002DA54 File Offset: 0x0002BC54
		public GameMenuWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06001032 RID: 4146 RVA: 0x0002DA6C File Offset: 0x0002BC6C
		protected override void OnLateUpdate(float dt)
		{
			if (!this._firstFrame)
			{
				if (this.IsNight)
				{
					base.Color = Color.Lerp(base.Color, new Color(0.23921569f, 0.4509804f, 0.8f, 1f), dt);
				}
				else
				{
					base.Color = Color.Lerp(base.Color, Color.White, dt);
				}
			}
			else
			{
				if (this.IsNight)
				{
					base.Color = new Color(0.23921569f, 0.4509804f, 0.8f, 1f);
				}
				else
				{
					base.Color = Color.White;
				}
				this._firstFrame = false;
				this.RefreshSize();
			}
			base.OnLateUpdate(dt);
		}

		// Token: 0x06001033 RID: 4147 RVA: 0x0002DB18 File Offset: 0x0002BD18
		private void RefreshSize()
		{
			base.SuggestedWidth = (float)(this.IsEncounterMenu ? this.EncounterModeMenuWidth : this.NormalModeMenuWidth);
			base.SuggestedHeight = (float)(this.IsEncounterMenu ? this.EncounterModeMenuHeight : this.NormalModeMenuHeight);
			base.ScaledSuggestedWidth = base.SuggestedWidth * base._scaleToUse;
			base.ScaledSuggestedHeight = base.SuggestedHeight * base._scaleToUse;
			base.MarginTop = (float)(this.IsEncounterMenu ? this.EncounterModeMenuMarginTop : this.NormalModeMenuMarginTop);
			this.ExtendButtonWidget.MarginTop = base.MarginTop;
		}

		// Token: 0x06001034 RID: 4148 RVA: 0x0002DBB3 File Offset: 0x0002BDB3
		private void OnExtendButtonClick(Widget button)
		{
			this.IsOverlayExtended = !this.IsOverlayExtended;
		}

		// Token: 0x06001035 RID: 4149 RVA: 0x0002DBC4 File Offset: 0x0002BDC4
		public void UpdateOverlayState()
		{
			this.ScopeTargeter.IsScopeEnabled = this._isOverlayExtended;
			string text = (this._isOverlayExtended ? "Default" : "Disabled");
			this.Overlay.SetState(text);
			foreach (Style style in this.ExtendButtonArrowWidget.Brush.Styles)
			{
				foreach (StyleLayer styleLayer in style.Layers)
				{
					styleLayer.HorizontalFlip = !this._isOverlayExtended;
				}
			}
		}

		// Token: 0x06001036 RID: 4150 RVA: 0x0002DC94 File Offset: 0x0002BE94
		private void TitleTextWidget_PropertyChanged(PropertyOwnerObject widget, string propertyName, object propertyValue)
		{
			if (propertyName == "Text")
			{
				this.TitleContainerWidget.IsVisible = !string.IsNullOrEmpty((string)propertyValue);
				this.IsOverlayExtended = true;
			}
		}

		// Token: 0x06001037 RID: 4151 RVA: 0x0002DCC3 File Offset: 0x0002BEC3
		private void OnOptionAdded(Widget parentWidget, Widget childWidget)
		{
			GameMenuItemWidget gameMenuItemWidget = childWidget as GameMenuItemWidget;
			gameMenuItemWidget.OnOptionStateChanged = (Action)Delegate.Combine(gameMenuItemWidget.OnOptionStateChanged, new Action(this.OnOptionStateChanged));
			this.IsOverlayExtended = true;
		}

		// Token: 0x06001038 RID: 4152 RVA: 0x0002DCF3 File Offset: 0x0002BEF3
		public void OnOptionStateChanged()
		{
			this.IsOverlayExtended = true;
		}

		// Token: 0x06001039 RID: 4153 RVA: 0x0002DCFC File Offset: 0x0002BEFC
		private void OnOptionRemoved(Widget widget, Widget child)
		{
			this.IsOverlayExtended = true;
		}

		// Token: 0x170005BA RID: 1466
		// (get) Token: 0x0600103A RID: 4154 RVA: 0x0002DD05 File Offset: 0x0002BF05
		// (set) Token: 0x0600103B RID: 4155 RVA: 0x0002DD0D File Offset: 0x0002BF0D
		[Editor(false)]
		public NavigationScopeTargeter ScopeTargeter
		{
			get
			{
				return this._scopeTargeter;
			}
			set
			{
				if (this._scopeTargeter != value)
				{
					this._scopeTargeter = value;
					base.OnPropertyChanged<NavigationScopeTargeter>(value, "ScopeTargeter");
				}
			}
		}

		// Token: 0x170005BB RID: 1467
		// (get) Token: 0x0600103C RID: 4156 RVA: 0x0002DD2B File Offset: 0x0002BF2B
		// (set) Token: 0x0600103D RID: 4157 RVA: 0x0002DD33 File Offset: 0x0002BF33
		[Editor(false)]
		public TextWidget TitleTextWidget
		{
			get
			{
				return this._titleTextWidget;
			}
			set
			{
				if (this._titleTextWidget != value)
				{
					this._titleTextWidget = value;
					base.OnPropertyChanged<TextWidget>(value, "TitleTextWidget");
					if (value != null)
					{
						value.PropertyChanged += this.TitleTextWidget_PropertyChanged;
					}
				}
			}
		}

		// Token: 0x170005BC RID: 1468
		// (get) Token: 0x0600103E RID: 4158 RVA: 0x0002DD66 File Offset: 0x0002BF66
		// (set) Token: 0x0600103F RID: 4159 RVA: 0x0002DD6E File Offset: 0x0002BF6E
		[Editor(false)]
		public Widget TitleContainerWidget
		{
			get
			{
				return this._titleContainerWidget;
			}
			set
			{
				if (this._titleContainerWidget != value)
				{
					this._titleContainerWidget = value;
					base.OnPropertyChanged<Widget>(value, "TitleContainerWidget");
				}
			}
		}

		// Token: 0x170005BD RID: 1469
		// (get) Token: 0x06001040 RID: 4160 RVA: 0x0002DD8C File Offset: 0x0002BF8C
		// (set) Token: 0x06001041 RID: 4161 RVA: 0x0002DD94 File Offset: 0x0002BF94
		[Editor(false)]
		public bool IsNight
		{
			get
			{
				return this._isNight;
			}
			set
			{
				if (this._isNight != value)
				{
					this._isNight = value;
					base.OnPropertyChanged(value, "IsNight");
				}
			}
		}

		// Token: 0x170005BE RID: 1470
		// (get) Token: 0x06001042 RID: 4162 RVA: 0x0002DDB2 File Offset: 0x0002BFB2
		// (set) Token: 0x06001043 RID: 4163 RVA: 0x0002DDBA File Offset: 0x0002BFBA
		[Editor(false)]
		public bool IsEncounterMenu
		{
			get
			{
				return this._isEncounterMenu;
			}
			set
			{
				if (this._isEncounterMenu != value)
				{
					this._isEncounterMenu = value;
					base.OnPropertyChanged(value, "IsEncounterMenu");
					this.RefreshSize();
				}
			}
		}

		// Token: 0x170005BF RID: 1471
		// (get) Token: 0x06001044 RID: 4164 RVA: 0x0002DDDE File Offset: 0x0002BFDE
		// (set) Token: 0x06001045 RID: 4165 RVA: 0x0002DDE6 File Offset: 0x0002BFE6
		[Editor(false)]
		public Widget Overlay
		{
			get
			{
				return this._overlay;
			}
			set
			{
				if (value != this._overlay)
				{
					this._overlay = value;
					base.OnPropertyChanged<Widget>(value, "Overlay");
				}
			}
		}

		// Token: 0x170005C0 RID: 1472
		// (get) Token: 0x06001046 RID: 4166 RVA: 0x0002DE04 File Offset: 0x0002C004
		// (set) Token: 0x06001047 RID: 4167 RVA: 0x0002DE0C File Offset: 0x0002C00C
		[Editor(false)]
		public ButtonWidget ExtendButtonWidget
		{
			get
			{
				return this._extendButtonWidget;
			}
			set
			{
				if (this._extendButtonWidget != value)
				{
					this._extendButtonWidget = value;
					base.OnPropertyChanged<ButtonWidget>(value, "ExtendButtonWidget");
					if (this._extendButtonWidget != null)
					{
						this._extendButtonWidget.ClickEventHandlers.Add(new Action<Widget>(this.OnExtendButtonClick));
					}
				}
			}
		}

		// Token: 0x170005C1 RID: 1473
		// (get) Token: 0x06001048 RID: 4168 RVA: 0x0002DE59 File Offset: 0x0002C059
		// (set) Token: 0x06001049 RID: 4169 RVA: 0x0002DE61 File Offset: 0x0002C061
		[Editor(false)]
		public BrushWidget ExtendButtonArrowWidget
		{
			get
			{
				return this._extendButtonArrowWidget;
			}
			set
			{
				if (value != this._extendButtonArrowWidget)
				{
					this._extendButtonArrowWidget = value;
					base.OnPropertyChanged<BrushWidget>(value, "ExtendButtonArrowWidget");
				}
			}
		}

		// Token: 0x170005C2 RID: 1474
		// (get) Token: 0x0600104A RID: 4170 RVA: 0x0002DE7F File Offset: 0x0002C07F
		// (set) Token: 0x0600104B RID: 4171 RVA: 0x0002DE88 File Offset: 0x0002C088
		[Editor(false)]
		public ListPanel OptionItemsList
		{
			get
			{
				return this._optionItemsList;
			}
			set
			{
				if (value != this._optionItemsList)
				{
					this._optionItemsList = value;
					this._optionItemsList.ItemAddEventHandlers.Add(new Action<Widget, Widget>(this.OnOptionAdded));
					this._optionItemsList.ItemRemoveEventHandlers.Add(new Action<Widget, Widget>(this.OnOptionRemoved));
					base.OnPropertyChanged<ListPanel>(value, "OptionItemsList");
				}
			}
		}

		// Token: 0x04000772 RID: 1906
		private bool _firstFrame = true;

		// Token: 0x04000779 RID: 1913
		private const string _extendedState = "Default";

		// Token: 0x0400077A RID: 1914
		private const string _hiddenState = "Disabled";

		// Token: 0x0400077B RID: 1915
		private bool _isOverlayExtended = true;

		// Token: 0x0400077C RID: 1916
		private NavigationScopeTargeter _scopeTargeter;

		// Token: 0x0400077D RID: 1917
		private TextWidget _titleTextWidget;

		// Token: 0x0400077E RID: 1918
		private Widget _titleContainerWidget;

		// Token: 0x0400077F RID: 1919
		private bool _isNight;

		// Token: 0x04000780 RID: 1920
		private bool _isEncounterMenu;

		// Token: 0x04000781 RID: 1921
		private Widget _overlay;

		// Token: 0x04000782 RID: 1922
		private ButtonWidget _extendButtonWidget;

		// Token: 0x04000783 RID: 1923
		private BrushWidget _extendButtonArrowWidget;

		// Token: 0x04000784 RID: 1924
		private ListPanel _optionItemsList;
	}
}
