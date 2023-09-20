using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace TaleWorlds.Library.Graph
{
	// Token: 0x020000AD RID: 173
	public class GraphVM : ViewModel
	{
		// Token: 0x06000635 RID: 1589 RVA: 0x00013444 File Offset: 0x00011644
		public GraphVM(string horizontalAxisLabel, string verticalAxisLabel)
		{
			this.Lines = new MBBindingList<GraphLineVM>();
			this.HorizontalAxisLabel = horizontalAxisLabel;
			this.VerticalAxisLabel = verticalAxisLabel;
		}

		// Token: 0x06000636 RID: 1590 RVA: 0x00013468 File Offset: 0x00011668
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

		// Token: 0x06000637 RID: 1591 RVA: 0x00013640 File Offset: 0x00011840
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

		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x06000638 RID: 1592 RVA: 0x000136A5 File Offset: 0x000118A5
		// (set) Token: 0x06000639 RID: 1593 RVA: 0x000136AD File Offset: 0x000118AD
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

		// Token: 0x170000C6 RID: 198
		// (get) Token: 0x0600063A RID: 1594 RVA: 0x000136CB File Offset: 0x000118CB
		// (set) Token: 0x0600063B RID: 1595 RVA: 0x000136D3 File Offset: 0x000118D3
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

		// Token: 0x170000C7 RID: 199
		// (get) Token: 0x0600063C RID: 1596 RVA: 0x000136F6 File Offset: 0x000118F6
		// (set) Token: 0x0600063D RID: 1597 RVA: 0x000136FE File Offset: 0x000118FE
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

		// Token: 0x170000C8 RID: 200
		// (get) Token: 0x0600063E RID: 1598 RVA: 0x00013721 File Offset: 0x00011921
		// (set) Token: 0x0600063F RID: 1599 RVA: 0x00013729 File Offset: 0x00011929
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

		// Token: 0x170000C9 RID: 201
		// (get) Token: 0x06000640 RID: 1600 RVA: 0x00013747 File Offset: 0x00011947
		// (set) Token: 0x06000641 RID: 1601 RVA: 0x0001374F File Offset: 0x0001194F
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

		// Token: 0x170000CA RID: 202
		// (get) Token: 0x06000642 RID: 1602 RVA: 0x0001376D File Offset: 0x0001196D
		// (set) Token: 0x06000643 RID: 1603 RVA: 0x00013775 File Offset: 0x00011975
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

		// Token: 0x170000CB RID: 203
		// (get) Token: 0x06000644 RID: 1604 RVA: 0x00013793 File Offset: 0x00011993
		// (set) Token: 0x06000645 RID: 1605 RVA: 0x0001379B File Offset: 0x0001199B
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

		// Token: 0x040001DE RID: 478
		private MBBindingList<GraphLineVM> _lines;

		// Token: 0x040001DF RID: 479
		private string _horizontalAxisLabel;

		// Token: 0x040001E0 RID: 480
		private string _verticalAxisLabel;

		// Token: 0x040001E1 RID: 481
		private float _horizontalMinValue;

		// Token: 0x040001E2 RID: 482
		private float _horizontalMaxValue;

		// Token: 0x040001E3 RID: 483
		private float _verticalMinValue;

		// Token: 0x040001E4 RID: 484
		private float _verticalMaxValue;
	}
}
