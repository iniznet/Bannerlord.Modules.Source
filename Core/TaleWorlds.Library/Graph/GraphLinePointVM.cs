using System;

namespace TaleWorlds.Library.Graph
{
	public class GraphLinePointVM : ViewModel
	{
		public GraphLinePointVM(float horizontalValue, float verticalValue)
		{
			this.HorizontalValue = horizontalValue;
			this.VerticalValue = verticalValue;
		}

		[DataSourceProperty]
		public float HorizontalValue
		{
			get
			{
				return this._horizontalValue;
			}
			set
			{
				if (value != this._horizontalValue)
				{
					this._horizontalValue = value;
					base.OnPropertyChangedWithValue(value, "HorizontalValue");
				}
			}
		}

		[DataSourceProperty]
		public float VerticalValue
		{
			get
			{
				return this._verticalValue;
			}
			set
			{
				if (value != this._verticalValue)
				{
					this._verticalValue = value;
					base.OnPropertyChangedWithValue(value, "VerticalValue");
				}
			}
		}

		private float _horizontalValue;

		private float _verticalValue;
	}
}
