using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Scoreboard
{
	public class MultiplayerScoreboardScreenWidget : Widget
	{
		public MultiplayerScoreboardScreenWidget(UIContext context)
			: base(context)
		{
		}

		private void UpdateSidesList()
		{
			if (this.SidesList == null)
			{
				return;
			}
			base.SuggestedWidth = (float)(this.IsSingleSide ? this.SingleColumnedWidth : this.DoubleColumnedWidth);
		}

		[DataSourceProperty]
		public bool IsSingleSide
		{
			get
			{
				return this._isSingleSide;
			}
			set
			{
				if (value != this._isSingleSide)
				{
					this._isSingleSide = value;
					base.OnPropertyChanged(value, "IsSingleSide");
					this.UpdateSidesList();
				}
			}
		}

		[DataSourceProperty]
		public int SingleColumnedWidth
		{
			get
			{
				return this._singleColumnedWidth;
			}
			set
			{
				if (value != this._singleColumnedWidth)
				{
					this._singleColumnedWidth = value;
					base.OnPropertyChanged(value, "SingleColumnedWidth");
				}
			}
		}

		[DataSourceProperty]
		public int DoubleColumnedWidth
		{
			get
			{
				return this._doubleColumnedWidth;
			}
			set
			{
				if (value != this._doubleColumnedWidth)
				{
					this._doubleColumnedWidth = value;
					base.OnPropertyChanged(value, "DoubleColumnedWidth");
				}
			}
		}

		[DataSourceProperty]
		public ListPanel SidesList
		{
			get
			{
				return this._sidesList;
			}
			set
			{
				if (value != this._sidesList)
				{
					this._sidesList = value;
					base.OnPropertyChanged<ListPanel>(value, "SidesList");
					this.UpdateSidesList();
				}
			}
		}

		private bool _isSingleSide;

		private int _singleColumnedWidth;

		private int _doubleColumnedWidth;

		private ListPanel _sidesList;
	}
}
