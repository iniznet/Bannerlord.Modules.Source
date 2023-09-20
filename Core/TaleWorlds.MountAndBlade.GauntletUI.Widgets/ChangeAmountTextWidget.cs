using System;
using System.Linq;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x0200000A RID: 10
	public class ChangeAmountTextWidget : TextWidget
	{
		// Token: 0x0600002D RID: 45 RVA: 0x00002562 File Offset: 0x00000762
		public ChangeAmountTextWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00002574 File Offset: 0x00000774
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

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600002F RID: 47 RVA: 0x0000264E File Offset: 0x0000084E
		// (set) Token: 0x06000030 RID: 48 RVA: 0x00002656 File Offset: 0x00000856
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

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000031 RID: 49 RVA: 0x0000267B File Offset: 0x0000087B
		// (set) Token: 0x06000032 RID: 50 RVA: 0x00002683 File Offset: 0x00000883
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

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000033 RID: 51 RVA: 0x000026A1 File Offset: 0x000008A1
		// (set) Token: 0x06000034 RID: 52 RVA: 0x000026A9 File Offset: 0x000008A9
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

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000035 RID: 53 RVA: 0x000026C7 File Offset: 0x000008C7
		// (set) Token: 0x06000036 RID: 54 RVA: 0x000026D0 File Offset: 0x000008D0
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

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000037 RID: 55 RVA: 0x00002741 File Offset: 0x00000941
		// (set) Token: 0x06000038 RID: 56 RVA: 0x0000274C File Offset: 0x0000094C
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

		// Token: 0x04000010 RID: 16
		private bool _isVisualsDirty;

		// Token: 0x04000011 RID: 17
		private Brush _negativeBrush;

		// Token: 0x04000012 RID: 18
		private Brush _positiveBrush;

		// Token: 0x04000013 RID: 19
		private bool _useParentheses;

		// Token: 0x04000014 RID: 20
		private int _amount;

		// Token: 0x04000015 RID: 21
		private string _negativeBrushName;

		// Token: 0x04000016 RID: 22
		private string _positiveBrushName;

		// Token: 0x04000017 RID: 23
		private bool _shouldBeVisible = true;
	}
}
