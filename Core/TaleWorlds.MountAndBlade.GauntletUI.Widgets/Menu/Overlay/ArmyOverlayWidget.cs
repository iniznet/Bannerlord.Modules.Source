using System;
using System.Linq;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Menu.Overlay
{
	public class ArmyOverlayWidget : OverlayBaseWidget
	{
		public ArmyOverlayWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			int num = this._armyListGridWidget.Children.Count((Widget c) => c.IsVisible);
			if (num != this._armyItemCount)
			{
				this.Overlay.SetState("Reset");
				this._armyItemCount = num;
			}
			this.RefreshOverlayExtendState(!this._initialized);
			this.UpdateExtendButtonVisual();
			this._initialized = true;
		}

		private void RefreshOverlayExtendState(bool forceSetPosition)
		{
			string text = (this._isInfoBarExtended ? "MapExtended" : "MapNormal");
			string text2 = (this._isOverlayExtended ? "OverlayExtended" : "OverlayNormal");
			if (text + text2 != this.Overlay.CurrentState)
			{
				if (!this._isOverlayExtended)
				{
					if (forceSetPosition)
					{
						VisualState visualState;
						this.Overlay.VisualDefinition.VisualStates.TryGetValue(text + text2, out visualState);
						this.Overlay.PositionYOffset = visualState.PositionYOffset;
					}
				}
				else
				{
					float y = this.ArmyListGridWidget.Size.Y;
					float num;
					if (this._isInfoBarExtended)
					{
						VisualState visualState2;
						this.Overlay.VisualDefinition.VisualStates.TryGetValue("MapExtendedOverlayNormal", out visualState2);
						num = visualState2.PositionYOffset - y * base._inverseScaleToUse;
					}
					else
					{
						VisualState visualState3;
						this.Overlay.VisualDefinition.VisualStates.TryGetValue("MapNormalOverlayNormal", out visualState3);
						num = visualState3.PositionYOffset - y * base._inverseScaleToUse;
					}
					if (forceSetPosition)
					{
						this.Overlay.PositionYOffset = num;
					}
					VisualState visualState4;
					this.Overlay.VisualDefinition.VisualStates.TryGetValue(text + text2, out visualState4);
					visualState4.PositionYOffset = num;
				}
				this.Overlay.SetState(text + text2);
			}
		}

		private void UpdateExtendButtonVisual()
		{
			foreach (Style style in this.ExtendButton.Brush.Styles)
			{
				foreach (StyleLayer styleLayer in style.Layers)
				{
					styleLayer.VerticalFlip = this._isOverlayExtended;
				}
			}
		}

		private void OnExtendButtonClick(Widget button)
		{
			this._isOverlayExtended = !this._isOverlayExtended;
			this.UpdateExtendButtonVisual();
			this.RefreshOverlayExtendState(false);
		}

		private void OnArmyListPageCountChanged()
		{
			if (this.PageControlWidget.PageCount == 1)
			{
				this.Overlay.PositionXOffset = 40f;
				this.ExtendButton.PositionXOffset = -40f;
				return;
			}
			this.Overlay.PositionXOffset = 0f;
			this.ExtendButton.PositionXOffset = 0f;
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
				if (this._overlay != value)
				{
					this._overlay = value;
					base.OnPropertyChanged<Widget>(value, "Overlay");
				}
			}
		}

		[Editor(false)]
		public GridWidget ArmyListGridWidget
		{
			get
			{
				return this._armyListGridWidget;
			}
			set
			{
				if (this._armyListGridWidget != value)
				{
					this._armyListGridWidget = value;
					base.OnPropertyChanged<GridWidget>(value, "ArmyListGridWidget");
				}
			}
		}

		[Editor(false)]
		public ButtonWidget ExtendButton
		{
			get
			{
				return this._extendButton;
			}
			set
			{
				if (this._extendButton != value)
				{
					this._extendButton = value;
					base.OnPropertyChanged<ButtonWidget>(value, "ExtendButton");
					if (this._extendButton != null)
					{
						this._extendButton.ClickEventHandlers.Add(new Action<Widget>(this.OnExtendButtonClick));
					}
				}
			}
		}

		[Editor(false)]
		public bool IsInfoBarExtended
		{
			get
			{
				return this._isInfoBarExtended;
			}
			set
			{
				if (this._isInfoBarExtended != value)
				{
					this._isInfoBarExtended = value;
					base.OnPropertyChanged(value, "IsInfoBarExtended");
				}
			}
		}

		[Editor(false)]
		public ContainerPageControlWidget PageControlWidget
		{
			get
			{
				return this._pageControlWidget;
			}
			set
			{
				if (value != this._pageControlWidget)
				{
					if (this._pageControlWidget != null)
					{
						this._pageControlWidget.OnPageCountChanged -= this.OnArmyListPageCountChanged;
					}
					this._pageControlWidget = value;
					this._pageControlWidget.OnPageCountChanged += this.OnArmyListPageCountChanged;
					base.OnPropertyChanged<ContainerPageControlWidget>(value, "PageControlWidget");
				}
			}
		}

		private bool _isOverlayExtended = true;

		private int _armyItemCount;

		private bool _initialized;

		private Widget _overlay;

		private bool _isInfoBarExtended;

		private ButtonWidget _extendButton;

		private GridWidget _armyListGridWidget;

		private ContainerPageControlWidget _pageControlWidget;
	}
}
