using System;
using System.Numerics;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Map;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Nameplate
{
	// Token: 0x02000073 RID: 115
	public class SettlementNameplateItemWidget : Widget
	{
		// Token: 0x06000657 RID: 1623 RVA: 0x00012F3B File Offset: 0x0001113B
		public SettlementNameplateItemWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x1700023C RID: 572
		// (get) Token: 0x06000658 RID: 1624 RVA: 0x00012F44 File Offset: 0x00011144
		// (set) Token: 0x06000659 RID: 1625 RVA: 0x00012F4C File Offset: 0x0001114C
		public bool IsOverWidget { get; private set; }

		// Token: 0x1700023D RID: 573
		// (get) Token: 0x0600065A RID: 1626 RVA: 0x00012F55 File Offset: 0x00011155
		// (set) Token: 0x0600065B RID: 1627 RVA: 0x00012F5D File Offset: 0x0001115D
		public int QuestType { get; set; }

		// Token: 0x1700023E RID: 574
		// (get) Token: 0x0600065C RID: 1628 RVA: 0x00012F66 File Offset: 0x00011166
		// (set) Token: 0x0600065D RID: 1629 RVA: 0x00012F6E File Offset: 0x0001116E
		public int IssueType { get; set; }

		// Token: 0x0600065E RID: 1630 RVA: 0x00012F78 File Offset: 0x00011178
		public void ParallelUpdate(float dt)
		{
			if (base.ParentWidget.IsEnabled)
			{
				this.IsOverWidget = this.IsMouseOverWidget();
				if (this.IsOverWidget && !this._hoverBegan)
				{
					this._hoverBegan = true;
					this._widgetToShow.IsVisible = true;
				}
				else if (!this.IsOverWidget && this._hoverBegan)
				{
					this._hoverBegan = false;
					this._widgetToShow.IsVisible = false;
				}
				if (!this.IsOverWidget && this._widgetToShow.IsVisible)
				{
					this._widgetToShow.IsVisible = false;
					return;
				}
			}
			else
			{
				this._widgetToShow.IsVisible = false;
			}
		}

		// Token: 0x0600065F RID: 1631 RVA: 0x00013014 File Offset: 0x00011214
		private bool IsMouseOverWidget()
		{
			Vector2 globalPosition = base.GlobalPosition;
			return this.IsBetween(base.EventManager.MousePosition.X, globalPosition.X, globalPosition.X + base.Size.X) && this.IsBetween(base.EventManager.MousePosition.Y, globalPosition.Y, globalPosition.Y + base.Size.Y);
		}

		// Token: 0x06000660 RID: 1632 RVA: 0x00013088 File Offset: 0x00011288
		private bool IsBetween(float number, float min, float max)
		{
			return number >= min && number <= max;
		}

		// Token: 0x1700023F RID: 575
		// (get) Token: 0x06000661 RID: 1633 RVA: 0x00013097 File Offset: 0x00011297
		// (set) Token: 0x06000662 RID: 1634 RVA: 0x0001309F File Offset: 0x0001129F
		public Widget SettlementNameplateCapsuleWidget
		{
			get
			{
				return this._settlementNameplateCapsuleWidget;
			}
			set
			{
				if (this._settlementNameplateCapsuleWidget != value)
				{
					this._settlementNameplateCapsuleWidget = value;
					base.OnPropertyChanged<Widget>(value, "SettlementNameplateCapsuleWidget");
				}
			}
		}

		// Token: 0x17000240 RID: 576
		// (get) Token: 0x06000663 RID: 1635 RVA: 0x000130BD File Offset: 0x000112BD
		// (set) Token: 0x06000664 RID: 1636 RVA: 0x000130C5 File Offset: 0x000112C5
		public GridWidget SettlementPartiesGridWidget
		{
			get
			{
				return this._settlementPartiesGridWidget;
			}
			set
			{
				if (this._settlementPartiesGridWidget != value)
				{
					this._settlementPartiesGridWidget = value;
					base.OnPropertyChanged<GridWidget>(value, "SettlementPartiesGridWidget");
				}
			}
		}

		// Token: 0x17000241 RID: 577
		// (get) Token: 0x06000665 RID: 1637 RVA: 0x000130E3 File Offset: 0x000112E3
		// (set) Token: 0x06000666 RID: 1638 RVA: 0x000130EB File Offset: 0x000112EB
		public MapEventVisualBrushWidget MapEventVisualWidget
		{
			get
			{
				return this._mapEventVisualWidget;
			}
			set
			{
				if (this._mapEventVisualWidget != value)
				{
					this._mapEventVisualWidget = value;
					base.OnPropertyChanged<MapEventVisualBrushWidget>(value, "MapEventVisualWidget");
				}
			}
		}

		// Token: 0x17000242 RID: 578
		// (get) Token: 0x06000667 RID: 1639 RVA: 0x00013109 File Offset: 0x00011309
		// (set) Token: 0x06000668 RID: 1640 RVA: 0x00013111 File Offset: 0x00011311
		[Editor(false)]
		public Widget WidgetToShow
		{
			get
			{
				return this._widgetToShow;
			}
			set
			{
				if (this._widgetToShow != value)
				{
					this._widgetToShow = value;
					base.OnPropertyChanged<Widget>(value, "WidgetToShow");
				}
			}
		}

		// Token: 0x17000243 RID: 579
		// (get) Token: 0x06000669 RID: 1641 RVA: 0x0001312F File Offset: 0x0001132F
		// (set) Token: 0x0600066A RID: 1642 RVA: 0x00013137 File Offset: 0x00011337
		public Widget SettlementNameplateInspectedWidget
		{
			get
			{
				return this._settlementNameplateInspectedWidget;
			}
			set
			{
				if (this._settlementNameplateInspectedWidget != value)
				{
					this._settlementNameplateInspectedWidget = value;
					base.OnPropertyChanged<Widget>(value, "SettlementNameplateInspectedWidget");
				}
			}
		}

		// Token: 0x17000244 RID: 580
		// (get) Token: 0x0600066B RID: 1643 RVA: 0x00013155 File Offset: 0x00011355
		// (set) Token: 0x0600066C RID: 1644 RVA: 0x0001315D File Offset: 0x0001135D
		public MaskedTextureWidget SettlementBannerWidget
		{
			get
			{
				return this._settlementBannerWidget;
			}
			set
			{
				if (this._settlementBannerWidget != value)
				{
					this._settlementBannerWidget = value;
					base.OnPropertyChanged<MaskedTextureWidget>(value, "SettlementBannerWidget");
				}
			}
		}

		// Token: 0x17000245 RID: 581
		// (get) Token: 0x0600066D RID: 1645 RVA: 0x0001317B File Offset: 0x0001137B
		// (set) Token: 0x0600066E RID: 1646 RVA: 0x00013183 File Offset: 0x00011383
		public TextWidget SettlementNameTextWidget
		{
			get
			{
				return this._settlementNameTextWidget;
			}
			set
			{
				if (this._settlementNameTextWidget != value)
				{
					this._settlementNameTextWidget = value;
					base.OnPropertyChanged<TextWidget>(value, "SettlementNameTextWidget");
				}
			}
		}

		// Token: 0x040002C7 RID: 711
		private bool _hoverBegan;

		// Token: 0x040002CB RID: 715
		private Widget _settlementNameplateCapsuleWidget;

		// Token: 0x040002CC RID: 716
		private Widget _settlementNameplateInspectedWidget;

		// Token: 0x040002CD RID: 717
		private MapEventVisualBrushWidget _mapEventVisualWidget;

		// Token: 0x040002CE RID: 718
		private MaskedTextureWidget _settlementBannerWidget;

		// Token: 0x040002CF RID: 719
		private TextWidget _settlementNameTextWidget;

		// Token: 0x040002D0 RID: 720
		private GridWidget _settlementPartiesGridWidget;

		// Token: 0x040002D1 RID: 721
		private Widget _widgetToShow;
	}
}
