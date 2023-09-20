using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class TabControlWidget : Widget
	{
		public TabControlWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (!this.FirstButton.ClickEventHandlers.Contains(new Action<Widget>(this.OnFirstButtonClick)))
			{
				this.FirstButton.ClickEventHandlers.Add(new Action<Widget>(this.OnFirstButtonClick));
			}
			if (!this.SecondButton.ClickEventHandlers.Contains(new Action<Widget>(this.OnSecondButtonClick)))
			{
				this.SecondButton.ClickEventHandlers.Add(new Action<Widget>(this.OnSecondButtonClick));
			}
			this.FirstButton.IsSelected = this.FirstItem.IsVisible;
			this.SecondButton.IsSelected = this.SecondItem.IsVisible;
		}

		public void OnFirstButtonClick(Widget widget)
		{
			if (!this._firstItem.IsVisible && this._secondItem.IsVisible)
			{
				this._secondItem.IsVisible = false;
				this._firstItem.IsVisible = true;
			}
		}

		public void OnSecondButtonClick(Widget widget)
		{
			if (this._firstItem.IsVisible && !this._secondItem.IsVisible)
			{
				this._secondItem.IsVisible = true;
				this._firstItem.IsVisible = false;
			}
		}

		[Editor(false)]
		public ButtonWidget FirstButton
		{
			get
			{
				return this._firstButton;
			}
			set
			{
				if (this._firstButton != value)
				{
					this._firstButton = value;
					base.OnPropertyChanged<ButtonWidget>(value, "FirstButton");
				}
			}
		}

		[Editor(false)]
		public ButtonWidget SecondButton
		{
			get
			{
				return this._secondButton;
			}
			set
			{
				if (this._secondButton != value)
				{
					this._secondButton = value;
					base.OnPropertyChanged<ButtonWidget>(value, "SecondButton");
				}
			}
		}

		[Editor(false)]
		public Widget SecondItem
		{
			get
			{
				return this._secondItem;
			}
			set
			{
				if (this._secondItem != value)
				{
					this._secondItem = value;
					base.OnPropertyChanged<Widget>(value, "SecondItem");
				}
			}
		}

		[Editor(false)]
		public Widget FirstItem
		{
			get
			{
				return this._firstItem;
			}
			set
			{
				if (this._firstItem != value)
				{
					this._firstItem = value;
					base.OnPropertyChanged<Widget>(value, "FirstItem");
				}
			}
		}

		private ButtonWidget _firstButton;

		private ButtonWidget _secondButton;

		private Widget _firstItem;

		private Widget _secondItem;
	}
}
