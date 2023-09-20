using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x02000033 RID: 51
	public class ScoreboardAnimatedTextWidget : TextWidget
	{
		// Token: 0x060002DE RID: 734 RVA: 0x000097F2 File Offset: 0x000079F2
		public ScoreboardAnimatedTextWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060002DF RID: 735 RVA: 0x000097FB File Offset: 0x000079FB
		private void HandleValueChanged(int value)
		{
			base.Text = ((!this.ShowZero && value == 0) ? "" : value.ToString());
			base.BrushRenderer.RestartAnimation();
			base.RegisterUpdateBrushes();
		}

		// Token: 0x17000101 RID: 257
		// (get) Token: 0x060002E0 RID: 736 RVA: 0x0000982D File Offset: 0x00007A2D
		// (set) Token: 0x060002E1 RID: 737 RVA: 0x00009835 File Offset: 0x00007A35
		[Editor(false)]
		public int ValueAsInt
		{
			get
			{
				return this._valueAsInt;
			}
			set
			{
				if (value != this._valueAsInt)
				{
					this._valueAsInt = value;
					base.OnPropertyChanged(value, "ValueAsInt");
					this.HandleValueChanged(value);
				}
			}
		}

		// Token: 0x17000102 RID: 258
		// (get) Token: 0x060002E2 RID: 738 RVA: 0x0000985A File Offset: 0x00007A5A
		// (set) Token: 0x060002E3 RID: 739 RVA: 0x00009862 File Offset: 0x00007A62
		[Editor(false)]
		public bool ShowZero
		{
			get
			{
				return this._showZero;
			}
			set
			{
				if (this._showZero != value)
				{
					this._showZero = value;
					base.OnPropertyChanged(value, "ShowZero");
					this.HandleValueChanged(this._valueAsInt);
				}
			}
		}

		// Token: 0x0400012C RID: 300
		private bool _showZero;

		// Token: 0x0400012D RID: 301
		private int _valueAsInt;
	}
}
