using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.HUD.Compass
{
	public class CompassMarkerVM : ViewModel
	{
		public float Angle { get; private set; }

		public CompassMarkerVM(bool isPrimary, float angle, string text)
		{
			this.IsPrimary = isPrimary;
			this.Angle = angle;
			this.Text = (this.IsPrimary ? text : ("-" + text + "-"));
		}

		public void Refresh(float circleX, float x, float distance)
		{
			this.FullPosition = circleX;
			this.Position = x;
			this.Distance = MathF.Round(distance);
		}

		[DataSourceProperty]
		public bool IsPrimary
		{
			get
			{
				return this._isPrimary;
			}
			set
			{
				if (value != this._isPrimary)
				{
					this._isPrimary = value;
					base.OnPropertyChangedWithValue(value, "IsPrimary");
				}
			}
		}

		[DataSourceProperty]
		public string Text
		{
			get
			{
				return this._text;
			}
			set
			{
				if (value != this._text)
				{
					this._text = value;
					base.OnPropertyChangedWithValue<string>(value, "Text");
				}
			}
		}

		[DataSourceProperty]
		public int Distance
		{
			get
			{
				return this._distance;
			}
			set
			{
				if (value != this._distance)
				{
					this._distance = value;
					base.OnPropertyChangedWithValue(value, "Distance");
				}
			}
		}

		[DataSourceProperty]
		public float Position
		{
			get
			{
				return this._position;
			}
			set
			{
				if (MathF.Abs(value - this._position) > 1E-45f)
				{
					this._position = value;
					base.OnPropertyChangedWithValue(value, "Position");
				}
			}
		}

		[DataSourceProperty]
		public float FullPosition
		{
			get
			{
				return this._fullPosition;
			}
			set
			{
				if (MathF.Abs(value - this._fullPosition) > 1E-45f)
				{
					this._fullPosition = value;
					base.OnPropertyChangedWithValue(value, "FullPosition");
				}
			}
		}

		private bool _isPrimary;

		private string _text;

		private int _distance;

		private float _position;

		private float _fullPosition;
	}
}
