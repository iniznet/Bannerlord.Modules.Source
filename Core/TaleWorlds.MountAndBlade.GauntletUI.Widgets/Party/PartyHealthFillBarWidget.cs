using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.ExtraWidgets;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Party
{
	public class PartyHealthFillBarWidget : FillBar
	{
		public PartyHealthFillBarWidget(UIContext context)
			: base(context)
		{
		}

		private void HealthUpdated()
		{
			if (this.brushLayer == null)
			{
				this.brushLayer = base.Brush.GetLayer("DefaultFill");
			}
			base.CurrentAmount = (base.InitialAmount = this.Health);
			if (this.IsWounded)
			{
				this.brushLayer.Color = this.WoundedColor;
			}
			else if (this.Health >= this.FullHealthyLimit)
			{
				this.brushLayer.Color = this.FullHealthyColor;
			}
			else
			{
				this.brushLayer.Color = this.HealthyColor;
			}
			if (this.HealthText != null)
			{
				this.HealthText.Text = this.Health + "%";
			}
		}

		[Editor(false)]
		public int Health
		{
			get
			{
				return this._health;
			}
			set
			{
				if (this._health != value)
				{
					this._health = value;
					base.OnPropertyChanged(value, "Health");
					this.HealthUpdated();
				}
			}
		}

		[Editor(false)]
		public bool IsWounded
		{
			get
			{
				return this._isWounded;
			}
			set
			{
				if (this._isWounded != value)
				{
					this._isWounded = value;
					base.OnPropertyChanged(value, "IsWounded");
					this.HealthUpdated();
				}
			}
		}

		[Editor(false)]
		public TextWidget HealthText
		{
			get
			{
				return this._healthText;
			}
			set
			{
				if (this._healthText != value)
				{
					this._healthText = value;
					base.OnPropertyChanged<TextWidget>(value, "HealthText");
					this.HealthUpdated();
				}
			}
		}

		private readonly int FullHealthyLimit = 90;

		private readonly Color WoundedColor = Color.FromUint(4290199102U);

		private readonly Color HealthyColor = Color.FromUint(4291732560U);

		private readonly Color FullHealthyColor = Color.FromUint(4284921662U);

		private BrushLayer brushLayer;

		private int _health;

		private bool _isWounded;

		private TextWidget _healthText;
	}
}
