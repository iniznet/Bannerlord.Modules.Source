using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.GameMenu
{
	public class GameMenuWidget : Widget
	{
		public int EncounterModeMenuWidth { get; set; }

		public int EncounterModeMenuHeight { get; set; }

		public int EncounterModeMenuMarginTop { get; set; }

		public int NormalModeMenuWidth { get; set; }

		public int NormalModeMenuHeight { get; set; }

		public int NormalModeMenuMarginTop { get; set; }

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

		public GameMenuWidget(UIContext context)
			: base(context)
		{
		}

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

		private void RefreshSize()
		{
			base.SuggestedWidth = (float)(this.IsEncounterMenu ? this.EncounterModeMenuWidth : this.NormalModeMenuWidth);
			base.SuggestedHeight = (float)(this.IsEncounterMenu ? this.EncounterModeMenuHeight : this.NormalModeMenuHeight);
			base.ScaledSuggestedWidth = base.SuggestedWidth * base._scaleToUse;
			base.ScaledSuggestedHeight = base.SuggestedHeight * base._scaleToUse;
			base.MarginTop = (float)(this.IsEncounterMenu ? this.EncounterModeMenuMarginTop : this.NormalModeMenuMarginTop);
			this.ExtendButtonWidget.MarginTop = base.MarginTop;
		}

		private void OnExtendButtonClick(Widget button)
		{
			this.IsOverlayExtended = !this.IsOverlayExtended;
		}

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

		private void TitleTextWidget_PropertyChanged(PropertyOwnerObject widget, string propertyName, object propertyValue)
		{
			if (propertyName == "Text")
			{
				this.TitleContainerWidget.IsVisible = !string.IsNullOrEmpty((string)propertyValue);
				this.IsOverlayExtended = true;
			}
		}

		private void OnOptionAdded(Widget parentWidget, Widget childWidget)
		{
			GameMenuItemWidget gameMenuItemWidget = childWidget as GameMenuItemWidget;
			gameMenuItemWidget.OnOptionStateChanged = (Action)Delegate.Combine(gameMenuItemWidget.OnOptionStateChanged, new Action(this.OnOptionStateChanged));
			this.IsOverlayExtended = true;
		}

		public void OnOptionStateChanged()
		{
			this.IsOverlayExtended = true;
		}

		private void OnOptionRemoved(Widget widget, Widget child)
		{
			this.IsOverlayExtended = true;
		}

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

		private bool _firstFrame = true;

		private const string _extendedState = "Default";

		private const string _hiddenState = "Disabled";

		private bool _isOverlayExtended = true;

		private NavigationScopeTargeter _scopeTargeter;

		private TextWidget _titleTextWidget;

		private Widget _titleContainerWidget;

		private bool _isNight;

		private bool _isEncounterMenu;

		private Widget _overlay;

		private ButtonWidget _extendButtonWidget;

		private BrushWidget _extendButtonArrowWidget;

		private ListPanel _optionItemsList;
	}
}
