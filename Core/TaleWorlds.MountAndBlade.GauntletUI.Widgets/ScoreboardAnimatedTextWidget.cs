using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class ScoreboardAnimatedTextWidget : TextWidget
	{
		public ScoreboardAnimatedTextWidget(UIContext context)
			: base(context)
		{
		}

		private void HandleValueChanged(int value)
		{
			base.Text = ((!this.ShowZero && value == 0) ? "" : value.ToString());
			base.BrushRenderer.RestartAnimation();
			base.RegisterUpdateBrushes();
		}

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

		private bool _showZero;

		private int _valueAsInt;
	}
}
