using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Kingdom
{
	public class DecisionSupportStrengthListPanel : ListPanel
	{
		public bool IsAbstain { get; set; }

		public bool IsPlayerSupporter { get; set; }

		public bool IsOptionSelected { get; set; }

		public bool IsKingsOutcome { get; set; }

		public DecisionSupportStrengthListPanel(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			switch (this.CurrentIndex)
			{
			case 2:
				this.StrengthButton0.IsSelected = true;
				this.StrengthButton1.IsSelected = false;
				this.StrengthButton2.IsSelected = false;
				break;
			case 3:
				this.StrengthButton0.IsSelected = false;
				this.StrengthButton1.IsSelected = true;
				this.StrengthButton2.IsSelected = false;
				break;
			case 4:
				this.StrengthButton0.IsSelected = false;
				this.StrengthButton1.IsSelected = false;
				this.StrengthButton2.IsSelected = true;
				break;
			}
			base.GamepadNavigationIndex = (this.IsOptionSelected ? (-1) : 0);
		}

		private void SetButtonsEnabled(bool isEnabled)
		{
			this.StrengthButton0.IsEnabled = isEnabled;
			this.StrengthButton1.IsEnabled = isEnabled;
			this.StrengthButton2.IsEnabled = isEnabled;
		}

		[Editor(false)]
		public int CurrentIndex
		{
			get
			{
				return this._currentIndex;
			}
			set
			{
				if (this._currentIndex != value)
				{
					this._currentIndex = value;
					base.OnPropertyChanged(value, "CurrentIndex");
				}
			}
		}

		[Editor(false)]
		public ButtonWidget StrengthButton0
		{
			get
			{
				return this._strengthButton0;
			}
			set
			{
				if (this._strengthButton0 != value)
				{
					this._strengthButton0 = value;
					base.OnPropertyChanged<ButtonWidget>(value, "StrengthButton0");
				}
			}
		}

		[Editor(false)]
		public ButtonWidget StrengthButton1
		{
			get
			{
				return this._strengthButton1;
			}
			set
			{
				if (this._strengthButton1 != value)
				{
					this._strengthButton1 = value;
					base.OnPropertyChanged<ButtonWidget>(value, "StrengthButton1");
				}
			}
		}

		[Editor(false)]
		public ButtonWidget StrengthButton2
		{
			get
			{
				return this._strengthButton2;
			}
			set
			{
				if (this._strengthButton2 != value)
				{
					this._strengthButton2 = value;
					base.OnPropertyChanged<ButtonWidget>(value, "StrengthButton2");
				}
			}
		}

		[Editor(false)]
		public RichTextWidget StrengthButton0Text
		{
			get
			{
				return this._strengthButton0Text;
			}
			set
			{
				if (this._strengthButton0Text != value)
				{
					this._strengthButton0Text = value;
					base.OnPropertyChanged<RichTextWidget>(value, "StrengthButton0Text");
				}
			}
		}

		[Editor(false)]
		public RichTextWidget StrengthButton1Text
		{
			get
			{
				return this._strengthButton1Text;
			}
			set
			{
				if (this._strengthButton1Text != value)
				{
					this._strengthButton1Text = value;
					base.OnPropertyChanged<RichTextWidget>(value, "StrengthButton1Text");
				}
			}
		}

		[Editor(false)]
		public RichTextWidget StrengthButton2Text
		{
			get
			{
				return this._strengthButton2Text;
			}
			set
			{
				if (this._strengthButton2Text != value)
				{
					this._strengthButton2Text = value;
					base.OnPropertyChanged<RichTextWidget>(value, "StrengthButton2Text");
				}
			}
		}

		private ButtonWidget _strengthButton0;

		private RichTextWidget _strengthButton0Text;

		private ButtonWidget _strengthButton1;

		private RichTextWidget _strengthButton1Text;

		private ButtonWidget _strengthButton2;

		private RichTextWidget _strengthButton2Text;

		private int _currentIndex;
	}
}
