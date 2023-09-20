using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Map.MapBar
{
	public class MapBarCustomValueTextWidget : TextWidget
	{
		public MapBarCustomValueTextWidget(UIContext context)
			: base(context)
		{
			base.OverrideDefaultStateSwitchingEnabled = true;
		}

		private void RefreshTextAnimation(int valueDifference)
		{
			if (valueDifference > 0)
			{
				if (base.CurrentState == "Positive")
				{
					base.BrushRenderer.RestartAnimation();
					return;
				}
				this.SetState("Positive");
				return;
			}
			else
			{
				if (valueDifference >= 0)
				{
					Debug.FailedAssert("Value change in party label cannot be 0", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.GauntletUI.Widgets\\Map\\MapBar\\MapBarCustomValueTextWidget.cs", "RefreshTextAnimation", 40);
					return;
				}
				if (base.CurrentState == "Negative")
				{
					base.BrushRenderer.RestartAnimation();
					return;
				}
				this.SetState("Negative");
				return;
			}
		}

		[Editor(false)]
		public int ValueAsInt
		{
			get
			{
				return this._totalTroops;
			}
			set
			{
				if (value != this._totalTroops)
				{
					this.RefreshTextAnimation(value - this._totalTroops);
					this._totalTroops = value;
					base.OnPropertyChanged(value, "ValueAsInt");
				}
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

		private bool _isWarning;

		private Color _normalColor;

		private Color _warningColor;

		private int _totalTroops;
	}
}
