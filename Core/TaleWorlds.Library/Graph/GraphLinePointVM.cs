using System;

namespace TaleWorlds.Library.Graph
{
	// Token: 0x020000AB RID: 171
	public class GraphLinePointVM : ViewModel
	{
		// Token: 0x06000629 RID: 1577 RVA: 0x00013345 File Offset: 0x00011545
		public GraphLinePointVM(float horizontalValue, float verticalValue)
		{
			this.HorizontalValue = horizontalValue;
			this.VerticalValue = verticalValue;
		}

		// Token: 0x170000C0 RID: 192
		// (get) Token: 0x0600062A RID: 1578 RVA: 0x0001335B File Offset: 0x0001155B
		// (set) Token: 0x0600062B RID: 1579 RVA: 0x00013363 File Offset: 0x00011563
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

		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x0600062C RID: 1580 RVA: 0x00013381 File Offset: 0x00011581
		// (set) Token: 0x0600062D RID: 1581 RVA: 0x00013389 File Offset: 0x00011589
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

		// Token: 0x040001D9 RID: 473
		private float _horizontalValue;

		// Token: 0x040001DA RID: 474
		private float _verticalValue;
	}
}
