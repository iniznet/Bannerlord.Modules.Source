using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission
{
	public class CompassMarkerTextWidget : TextWidget
	{
		public CompassMarkerTextWidget(UIContext context)
			: base(context)
		{
		}

		private void UpdateBrush()
		{
			if (this.PrimaryBrush != null && this.SecondaryBrush != null)
			{
				base.Brush = (this.IsPrimary ? this.PrimaryBrush : this.SecondaryBrush);
			}
		}

		public bool IsPrimary
		{
			get
			{
				return this._isPrimary;
			}
			set
			{
				if (this._isPrimary != value)
				{
					this._isPrimary = value;
					base.OnPropertyChanged(value, "IsPrimary");
					this.UpdateBrush();
				}
			}
		}

		public float Position
		{
			get
			{
				return this._position;
			}
			set
			{
				if (Math.Abs(this._position - value) > 1E-45f)
				{
					this._position = value;
					base.OnPropertyChanged(value, "Position");
				}
			}
		}

		public Brush PrimaryBrush
		{
			get
			{
				return this._primaryBrush;
			}
			set
			{
				if (this._primaryBrush != value)
				{
					this._primaryBrush = value;
					base.OnPropertyChanged<Brush>(value, "PrimaryBrush");
					this.UpdateBrush();
				}
			}
		}

		public Brush SecondaryBrush
		{
			get
			{
				return this._secondaryBrush;
			}
			set
			{
				if (this._secondaryBrush != value)
				{
					this._secondaryBrush = value;
					base.OnPropertyChanged<Brush>(value, "SecondaryBrush");
					this.UpdateBrush();
				}
			}
		}

		private bool _isPrimary;

		private float _position;

		private Brush _primaryBrush;

		private Brush _secondaryBrush;
	}
}
