using System;
using System.Numerics;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Map;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Nameplate
{
	public class SettlementNameplateItemWidget : Widget
	{
		public SettlementNameplateItemWidget(UIContext context)
			: base(context)
		{
		}

		public bool IsOverWidget { get; private set; }

		public int QuestType { get; set; }

		public int IssueType { get; set; }

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

		private bool IsMouseOverWidget()
		{
			Vector2 globalPosition = base.GlobalPosition;
			return this.IsBetween(base.EventManager.MousePosition.X, globalPosition.X, globalPosition.X + base.Size.X) && this.IsBetween(base.EventManager.MousePosition.Y, globalPosition.Y, globalPosition.Y + base.Size.Y);
		}

		private bool IsBetween(float number, float min, float max)
		{
			return number >= min && number <= max;
		}

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

		private bool _hoverBegan;

		private Widget _settlementNameplateCapsuleWidget;

		private Widget _settlementNameplateInspectedWidget;

		private MapEventVisualBrushWidget _mapEventVisualWidget;

		private MaskedTextureWidget _settlementBannerWidget;

		private TextWidget _settlementNameTextWidget;

		private GridWidget _settlementPartiesGridWidget;

		private Widget _widgetToShow;
	}
}
