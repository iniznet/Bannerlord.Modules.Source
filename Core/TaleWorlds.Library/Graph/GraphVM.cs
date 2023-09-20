using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace TaleWorlds.Library.Graph
{
	public class GraphVM : ViewModel
	{
		public GraphVM(string horizontalAxisLabel, string verticalAxisLabel)
		{
			this.Lines = new MBBindingList<GraphLineVM>();
			this.HorizontalAxisLabel = horizontalAxisLabel;
			this.VerticalAxisLabel = verticalAxisLabel;
		}

		public void Draw([TupleElementNames(new string[] { "line", "points" })] IEnumerable<ValueTuple<GraphLineVM, IEnumerable<GraphLinePointVM>>> linesWithPoints, in Vec2 horizontalRange, in Vec2 verticalRange, float autoRangeHorizontalCoefficient = 1f, float autoRangeVerticalCoefficient = 1f, bool useAutoHorizontalRange = false, bool useAutoVerticalRange = false)
		{
			this.Lines.Clear();
			float num = float.MaxValue;
			float num2 = float.MinValue;
			float num3 = float.MaxValue;
			float num4 = float.MinValue;
			foreach (ValueTuple<GraphLineVM, IEnumerable<GraphLinePointVM>> valueTuple in linesWithPoints)
			{
				GraphLineVM item = valueTuple.Item1;
				foreach (GraphLinePointVM graphLinePointVM in valueTuple.Item2)
				{
					if (useAutoHorizontalRange)
					{
						if (graphLinePointVM.HorizontalValue < num)
						{
							num = graphLinePointVM.HorizontalValue;
						}
						if (graphLinePointVM.HorizontalValue > num2)
						{
							num2 = graphLinePointVM.HorizontalValue;
						}
					}
					if (useAutoVerticalRange)
					{
						if (graphLinePointVM.VerticalValue < num3)
						{
							num3 = graphLinePointVM.VerticalValue;
						}
						if (graphLinePointVM.VerticalValue > num4)
						{
							num4 = graphLinePointVM.VerticalValue;
						}
					}
					item.Points.Add(graphLinePointVM);
				}
				this.Lines.Add(item);
			}
			Vec2 vec = horizontalRange;
			float x = vec.X;
			vec = horizontalRange;
			float y = vec.Y;
			vec = verticalRange;
			float x2 = vec.X;
			vec = verticalRange;
			float y2 = vec.Y;
			bool flag = num != float.MaxValue && num2 != float.MinValue;
			bool flag2 = num3 != float.MaxValue && num4 != float.MinValue;
			if (useAutoHorizontalRange && flag)
			{
				GraphVM.ExtendRangeToNearestMultipleOfCoefficient(num, num2, autoRangeHorizontalCoefficient, out x, out y);
			}
			if (useAutoVerticalRange && flag2)
			{
				GraphVM.ExtendRangeToNearestMultipleOfCoefficient(num3, num4, autoRangeVerticalCoefficient, out x2, out y2);
			}
			this.HorizontalMinValue = x;
			this.HorizontalMaxValue = y;
			this.VerticalMinValue = x2;
			this.VerticalMaxValue = y2;
		}

		private static void ExtendRangeToNearestMultipleOfCoefficient(float minValue, float maxValue, float coefficient, out float extendedMinValue, out float extendedMaxValue)
		{
			if (coefficient > 1E-05f)
			{
				extendedMinValue = (float)MathF.Floor(minValue / coefficient) * coefficient;
				extendedMaxValue = (float)MathF.Ceiling(maxValue / coefficient) * coefficient;
				if (extendedMinValue.ApproximatelyEqualsTo(extendedMaxValue, 1E-05f))
				{
					if (extendedMinValue - coefficient > 0f)
					{
						extendedMinValue -= coefficient;
						return;
					}
					extendedMaxValue += coefficient;
					return;
				}
			}
			else
			{
				extendedMinValue = minValue;
				extendedMaxValue = maxValue;
			}
		}

		[DataSourceProperty]
		public MBBindingList<GraphLineVM> Lines
		{
			get
			{
				return this._lines;
			}
			set
			{
				if (value != this._lines)
				{
					this._lines = value;
					base.OnPropertyChangedWithValue<MBBindingList<GraphLineVM>>(value, "Lines");
				}
			}
		}

		[DataSourceProperty]
		public string HorizontalAxisLabel
		{
			get
			{
				return this._horizontalAxisLabel;
			}
			set
			{
				if (value != this._horizontalAxisLabel)
				{
					this._horizontalAxisLabel = value;
					base.OnPropertyChangedWithValue<string>(value, "HorizontalAxisLabel");
				}
			}
		}

		[DataSourceProperty]
		public string VerticalAxisLabel
		{
			get
			{
				return this._verticalAxisLabel;
			}
			set
			{
				if (value != this._verticalAxisLabel)
				{
					this._verticalAxisLabel = value;
					base.OnPropertyChangedWithValue<string>(value, "VerticalAxisLabel");
				}
			}
		}

		[DataSourceProperty]
		public float HorizontalMinValue
		{
			get
			{
				return this._horizontalMinValue;
			}
			set
			{
				if (value != this._horizontalMinValue)
				{
					this._horizontalMinValue = value;
					base.OnPropertyChangedWithValue(value, "HorizontalMinValue");
				}
			}
		}

		[DataSourceProperty]
		public float HorizontalMaxValue
		{
			get
			{
				return this._horizontalMaxValue;
			}
			set
			{
				if (value != this._horizontalMaxValue)
				{
					this._horizontalMaxValue = value;
					base.OnPropertyChangedWithValue(value, "HorizontalMaxValue");
				}
			}
		}

		[DataSourceProperty]
		public float VerticalMinValue
		{
			get
			{
				return this._verticalMinValue;
			}
			set
			{
				if (value != this._verticalMinValue)
				{
					this._verticalMinValue = value;
					base.OnPropertyChangedWithValue(value, "VerticalMinValue");
				}
			}
		}

		[DataSourceProperty]
		public float VerticalMaxValue
		{
			get
			{
				return this._verticalMaxValue;
			}
			set
			{
				if (value != this._verticalMaxValue)
				{
					this._verticalMaxValue = value;
					base.OnPropertyChangedWithValue(value, "VerticalMaxValue");
				}
			}
		}

		private MBBindingList<GraphLineVM> _lines;

		private string _horizontalAxisLabel;

		private string _verticalAxisLabel;

		private float _horizontalMinValue;

		private float _horizontalMaxValue;

		private float _verticalMinValue;

		private float _verticalMaxValue;
	}
}
