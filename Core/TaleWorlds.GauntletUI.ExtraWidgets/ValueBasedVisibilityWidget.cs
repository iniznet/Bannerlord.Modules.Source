using System;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.GauntletUI.ExtraWidgets
{
	public class ValueBasedVisibilityWidget : Widget
	{
		public ValueBasedVisibilityWidget.WatchTypes WatchType { get; set; }

		public ValueBasedVisibilityWidget(UIContext context)
			: base(context)
		{
		}

		[Editor(false)]
		public int IndexToWatch
		{
			get
			{
				return this._indexToWatch;
			}
			set
			{
				if (this._indexToWatch != value)
				{
					this._indexToWatch = value;
					base.OnPropertyChanged(value, "IndexToWatch");
					switch (this.WatchType)
					{
					case ValueBasedVisibilityWidget.WatchTypes.Equal:
						base.IsVisible = value == this.IndexToBeVisible;
						return;
					case ValueBasedVisibilityWidget.WatchTypes.BiggerThan:
						base.IsVisible = value > this.IndexToBeVisible;
						return;
					case ValueBasedVisibilityWidget.WatchTypes.BiggerThanEqual:
						base.IsVisible = value >= this.IndexToBeVisible;
						return;
					case ValueBasedVisibilityWidget.WatchTypes.LessThan:
						base.IsVisible = value < this.IndexToBeVisible;
						return;
					case ValueBasedVisibilityWidget.WatchTypes.LessThanEqual:
						base.IsVisible = value <= this.IndexToBeVisible;
						return;
					case ValueBasedVisibilityWidget.WatchTypes.NotEqual:
						base.IsVisible = value != this.IndexToBeVisible;
						break;
					default:
						return;
					}
				}
			}
		}

		[Editor(false)]
		public float IndexToWatchFloat
		{
			get
			{
				return this._indexToWatchFloat;
			}
			set
			{
				if (this._indexToWatchFloat != value)
				{
					this._indexToWatchFloat = value;
					base.OnPropertyChanged(value, "IndexToWatchFloat");
					switch (this.WatchType)
					{
					case ValueBasedVisibilityWidget.WatchTypes.Equal:
						base.IsVisible = value == this.IndexToBeVisibleFloat;
						return;
					case ValueBasedVisibilityWidget.WatchTypes.BiggerThan:
						base.IsVisible = value > this.IndexToBeVisibleFloat;
						return;
					case ValueBasedVisibilityWidget.WatchTypes.BiggerThanEqual:
						base.IsVisible = value >= this.IndexToBeVisibleFloat;
						return;
					case ValueBasedVisibilityWidget.WatchTypes.LessThan:
						base.IsVisible = value < this.IndexToBeVisibleFloat;
						return;
					case ValueBasedVisibilityWidget.WatchTypes.LessThanEqual:
						base.IsVisible = value <= this.IndexToBeVisibleFloat;
						return;
					case ValueBasedVisibilityWidget.WatchTypes.NotEqual:
						base.IsVisible = value != this.IndexToBeVisibleFloat;
						break;
					default:
						return;
					}
				}
			}
		}

		[Editor(false)]
		public int IndexToBeVisible
		{
			get
			{
				return this._indexToBeVisible;
			}
			set
			{
				if (this._indexToBeVisible != value)
				{
					this._indexToBeVisible = value;
					base.OnPropertyChanged(value, "IndexToBeVisible");
				}
			}
		}

		[Editor(false)]
		public float IndexToBeVisibleFloat
		{
			get
			{
				return this._indexToBeVisibleFloat;
			}
			set
			{
				if (this._indexToBeVisibleFloat != value)
				{
					this._indexToBeVisibleFloat = value;
					base.OnPropertyChanged(value, "IndexToBeVisibleFloat");
				}
			}
		}

		private int _indexToBeVisible;

		private int _indexToWatch = -1;

		private float _indexToBeVisibleFloat;

		private float _indexToWatchFloat = -1f;

		public enum WatchTypes
		{
			Equal,
			BiggerThan,
			BiggerThanEqual,
			LessThan,
			LessThanEqual,
			NotEqual
		}
	}
}
