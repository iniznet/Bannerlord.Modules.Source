using System;
using System.Linq;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class ChangeAmountTextWidget : TextWidget
	{
		public ChangeAmountTextWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._isVisualsDirty)
			{
				if (!this.ShouldBeVisible)
				{
					base.IsVisible = false;
				}
				else
				{
					base.IsVisible = this.Amount != 0;
					if (base.IsVisible)
					{
						base.Text = ((this.Amount > 0) ? ("+" + this.Amount.ToString()) : this.Amount.ToString());
						if (this.UseParentheses)
						{
							base.Text = "(" + base.Text + ")";
						}
						if (this.Amount > 0)
						{
							base.Brush = this._positiveBrush;
						}
						else if (this.Amount < 0)
						{
							base.Brush = this._negativeBrush;
						}
					}
				}
				this._isVisualsDirty = true;
			}
		}

		[Editor(false)]
		public int Amount
		{
			get
			{
				return this._amount;
			}
			set
			{
				if (this._amount != value)
				{
					this._amount = value;
					base.OnPropertyChanged(value, "Amount");
					this._isVisualsDirty = false;
				}
			}
		}

		[Editor(false)]
		public bool UseParentheses
		{
			get
			{
				return this._useParentheses;
			}
			set
			{
				if (this._useParentheses != value)
				{
					this._useParentheses = value;
					base.OnPropertyChanged(value, "UseParentheses");
				}
			}
		}

		[Editor(false)]
		public bool ShouldBeVisible
		{
			get
			{
				return this._shouldBeVisible;
			}
			set
			{
				if (this._shouldBeVisible != value)
				{
					this._shouldBeVisible = value;
					base.OnPropertyChanged(value, "ShouldBeVisible");
				}
			}
		}

		[Editor(false)]
		public string NegativeBrushName
		{
			get
			{
				return this._negativeBrushName;
			}
			set
			{
				if (this._negativeBrushName != value)
				{
					this._negativeBrushName = value;
					base.OnPropertyChanged<string>(value, "NegativeBrushName");
					this._negativeBrush = base.EventManager.Context.Brushes.First((Brush b) => b.Name == value);
				}
			}
		}

		[Editor(false)]
		public string PositiveBrushName
		{
			get
			{
				return this._positiveBrushName;
			}
			set
			{
				if (this._positiveBrushName != value)
				{
					this._positiveBrushName = value;
					base.OnPropertyChanged<string>(value, "PositiveBrushName");
					this._positiveBrush = base.EventManager.Context.Brushes.First((Brush b) => b.Name == value);
				}
			}
		}

		private bool _isVisualsDirty;

		private Brush _negativeBrush;

		private Brush _positiveBrush;

		private bool _useParentheses;

		private int _amount;

		private string _negativeBrushName;

		private string _positiveBrushName;

		private bool _shouldBeVisible = true;
	}
}
