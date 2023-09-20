using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Map.MapBar
{
	public class MapBarTextWidget : TextWidget
	{
		public MapBarTextWidget(UIContext context)
			: base(context)
		{
			base.intPropertyChanged += this.TextPropertyChanged;
			base.OverrideDefaultStateSwitchingEnabled = true;
		}

		private void TextPropertyChanged(PropertyOwnerObject widget, string propertyName, int propertyValue)
		{
			if (propertyName == "IntText")
			{
				if (this._prevValue != -99)
				{
					if (propertyValue - this._prevValue > 0)
					{
						if (base.CurrentState == "Positive")
						{
							base.BrushRenderer.RestartAnimation();
						}
						else
						{
							this.SetState("Positive");
						}
					}
					else if (propertyValue - this._prevValue < 0)
					{
						if (base.CurrentState == "Negative")
						{
							base.BrushRenderer.RestartAnimation();
						}
						else
						{
							this.SetState("Negative");
						}
					}
				}
				this._prevValue = propertyValue;
			}
		}

		[Editor(false)]
		public bool IsWarning
		{
			get
			{
				return this._isWarning;
			}
			set
			{
				if (value != this._isWarning)
				{
					this._isWarning = value;
					base.OnPropertyChanged(value, "IsWarning");
					base.ReadOnlyBrush.GetStyleOrDefault(base.CurrentState);
					Color color = Color.Black;
					if (value)
					{
						color = this.WarningColor;
					}
					else
					{
						color = this.NormalColor;
					}
					foreach (Style style in base.Brush.Styles)
					{
						style.FontColor = color;
					}
				}
			}
		}

		[Editor(false)]
		public Color NormalColor
		{
			get
			{
				return this._normalColor;
			}
			set
			{
				if (value.Alpha != this._normalColor.Alpha || value.Blue != this._normalColor.Blue || value.Red != this._normalColor.Red || value.Green != this._normalColor.Green)
				{
					this._normalColor = value;
					base.OnPropertyChanged(value, "NormalColor");
				}
			}
		}

		[Editor(false)]
		public Color WarningColor
		{
			get
			{
				return this._warningColor;
			}
			set
			{
				if (value.Alpha != this._warningColor.Alpha || value.Blue != this._warningColor.Blue || value.Red != this._warningColor.Red || value.Green != this._warningColor.Green)
				{
					this._warningColor = value;
					base.OnPropertyChanged(value, "WarningColor");
				}
			}
		}

		private int _prevValue = -99;

		private bool _isWarning;

		private Color _normalColor;

		private Color _warningColor;
	}
}
