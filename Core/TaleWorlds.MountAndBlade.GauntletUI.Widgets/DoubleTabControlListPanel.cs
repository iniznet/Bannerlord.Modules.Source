using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class DoubleTabControlListPanel : ListPanel
	{
		public DoubleTabControlListPanel(UIContext context)
			: base(context)
		{
		}

		public void OnFirstTabClick(Widget widget)
		{
			if (!this._firstList.IsVisible && this._secondList.IsVisible)
			{
				this._secondList.IsVisible = false;
				this._firstList.IsVisible = true;
			}
		}

		public void OnSecondTabClick(Widget widget)
		{
			if (this._firstList.IsVisible && !this._secondList.IsVisible)
			{
				this._secondList.IsVisible = true;
				this._firstList.IsVisible = false;
			}
		}

		[Editor(false)]
		public ButtonWidget FirstListButton
		{
			get
			{
				return this._firstListButton;
			}
			set
			{
				if (this._firstListButton != value)
				{
					this._firstListButton = value;
					base.OnPropertyChanged<ButtonWidget>(value, "FirstListButton");
					if (this.FirstListButton != null && !this.FirstListButton.ClickEventHandlers.Contains(new Action<Widget>(this.OnFirstTabClick)))
					{
						this.FirstListButton.ClickEventHandlers.Add(new Action<Widget>(this.OnFirstTabClick));
					}
				}
			}
		}

		[Editor(false)]
		public ButtonWidget SecondListButton
		{
			get
			{
				return this._secondListButton;
			}
			set
			{
				if (this._secondListButton != value)
				{
					this._secondListButton = value;
					base.OnPropertyChanged<ButtonWidget>(value, "SecondListButton");
					if (this.SecondListButton != null && !this.SecondListButton.ClickEventHandlers.Contains(new Action<Widget>(this.OnSecondTabClick)))
					{
						this.SecondListButton.ClickEventHandlers.Add(new Action<Widget>(this.OnSecondTabClick));
					}
				}
			}
		}

		[Editor(false)]
		public Widget FirstList
		{
			get
			{
				return this._firstList;
			}
			set
			{
				if (this._firstList != value)
				{
					this._firstList = value;
					base.OnPropertyChanged<Widget>(value, "FirstList");
				}
			}
		}

		[Editor(false)]
		public Widget SecondList
		{
			get
			{
				return this._secondList;
			}
			set
			{
				if (this._secondList != value)
				{
					this._secondList = value;
					base.OnPropertyChanged<Widget>(value, "SecondList");
				}
			}
		}

		private ButtonWidget _firstListButton;

		private ButtonWidget _secondListButton;

		private Widget _firstList;

		private Widget _secondList;
	}
}
