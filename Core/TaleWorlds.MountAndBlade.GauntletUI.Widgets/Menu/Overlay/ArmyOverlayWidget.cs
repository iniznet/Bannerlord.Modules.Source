using System;
using System.Linq;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Menu.Overlay
{
	// Token: 0x020000F1 RID: 241
	public class ArmyOverlayWidget : OverlayBaseWidget
	{
		// Token: 0x06000C7C RID: 3196 RVA: 0x00023027 File Offset: 0x00021227
		public ArmyOverlayWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000C7D RID: 3197 RVA: 0x00023038 File Offset: 0x00021238
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

		// Token: 0x06000C7E RID: 3198 RVA: 0x000230B8 File Offset: 0x000212B8
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

		// Token: 0x06000C7F RID: 3199 RVA: 0x00023214 File Offset: 0x00021414
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

		// Token: 0x06000C80 RID: 3200 RVA: 0x000232B0 File Offset: 0x000214B0
		private void OnExtendButtonClick(Widget button)
		{
			this._isOverlayExtended = !this._isOverlayExtended;
			this.UpdateExtendButtonVisual();
			this.RefreshOverlayExtendState(false);
		}

		// Token: 0x06000C81 RID: 3201 RVA: 0x000232D0 File Offset: 0x000214D0
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

		// Token: 0x17000477 RID: 1143
		// (get) Token: 0x06000C82 RID: 3202 RVA: 0x0002332C File Offset: 0x0002152C
		// (set) Token: 0x06000C83 RID: 3203 RVA: 0x00023334 File Offset: 0x00021534
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

		// Token: 0x17000478 RID: 1144
		// (get) Token: 0x06000C84 RID: 3204 RVA: 0x00023352 File Offset: 0x00021552
		// (set) Token: 0x06000C85 RID: 3205 RVA: 0x0002335A File Offset: 0x0002155A
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

		// Token: 0x17000479 RID: 1145
		// (get) Token: 0x06000C86 RID: 3206 RVA: 0x00023378 File Offset: 0x00021578
		// (set) Token: 0x06000C87 RID: 3207 RVA: 0x00023380 File Offset: 0x00021580
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

		// Token: 0x1700047A RID: 1146
		// (get) Token: 0x06000C88 RID: 3208 RVA: 0x000233CD File Offset: 0x000215CD
		// (set) Token: 0x06000C89 RID: 3209 RVA: 0x000233D5 File Offset: 0x000215D5
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

		// Token: 0x1700047B RID: 1147
		// (get) Token: 0x06000C8A RID: 3210 RVA: 0x000233F3 File Offset: 0x000215F3
		// (set) Token: 0x06000C8B RID: 3211 RVA: 0x000233FC File Offset: 0x000215FC
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

		// Token: 0x040005C1 RID: 1473
		private bool _isOverlayExtended = true;

		// Token: 0x040005C2 RID: 1474
		private int _armyItemCount;

		// Token: 0x040005C3 RID: 1475
		private bool _initialized;

		// Token: 0x040005C4 RID: 1476
		private Widget _overlay;

		// Token: 0x040005C5 RID: 1477
		private bool _isInfoBarExtended;

		// Token: 0x040005C6 RID: 1478
		private ButtonWidget _extendButton;

		// Token: 0x040005C7 RID: 1479
		private GridWidget _armyListGridWidget;

		// Token: 0x040005C8 RID: 1480
		private ContainerPageControlWidget _pageControlWidget;
	}
}
