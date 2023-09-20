using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class SortButtonWidget : ButtonWidget
	{
		public SortButtonWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this.SortVisualWidget != null)
			{
				switch (this.SortState)
				{
				case 0:
					this.SortVisualWidget.SetState("Default");
					return;
				case 1:
					this.SortVisualWidget.SetState("Ascending");
					return;
				case 2:
					this.SortVisualWidget.SetState("Descending");
					break;
				default:
					return;
				}
			}
		}

		[Editor(false)]
		public int SortState
		{
			get
			{
				return this._sortState;
			}
			set
			{
				if (this._sortState != value)
				{
					this._sortState = value;
					base.OnPropertyChanged(value, "SortState");
				}
			}
		}

		[Editor(false)]
		public BrushWidget SortVisualWidget
		{
			get
			{
				return this._sortVisualWidget;
			}
			set
			{
				if (this._sortVisualWidget != value)
				{
					this._sortVisualWidget = value;
					base.OnPropertyChanged<BrushWidget>(value, "SortVisualWidget");
				}
			}
		}

		private int _sortState;

		private BrushWidget _sortVisualWidget;
	}
}
