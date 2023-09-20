using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.HUD.Compass
{
	// Token: 0x020000F0 RID: 240
	public class CompassMarkerVM : ViewModel
	{
		// Token: 0x170006F9 RID: 1785
		// (get) Token: 0x0600153D RID: 5437 RVA: 0x0004529A File Offset: 0x0004349A
		// (set) Token: 0x0600153E RID: 5438 RVA: 0x000452A2 File Offset: 0x000434A2
		public float Angle { get; private set; }

		// Token: 0x0600153F RID: 5439 RVA: 0x000452AB File Offset: 0x000434AB
		public CompassMarkerVM(bool isPrimary, float angle, string text)
		{
			this.IsPrimary = isPrimary;
			this.Angle = angle;
			this.Text = (this.IsPrimary ? text : ("-" + text + "-"));
		}

		// Token: 0x06001540 RID: 5440 RVA: 0x000452E2 File Offset: 0x000434E2
		public void Refresh(float circleX, float x, float distance)
		{
			this.FullPosition = circleX;
			this.Position = x;
			this.Distance = MathF.Round(distance);
		}

		// Token: 0x170006FA RID: 1786
		// (get) Token: 0x06001541 RID: 5441 RVA: 0x000452FE File Offset: 0x000434FE
		// (set) Token: 0x06001542 RID: 5442 RVA: 0x00045306 File Offset: 0x00043506
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

		// Token: 0x170006FB RID: 1787
		// (get) Token: 0x06001543 RID: 5443 RVA: 0x00045324 File Offset: 0x00043524
		// (set) Token: 0x06001544 RID: 5444 RVA: 0x0004532C File Offset: 0x0004352C
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

		// Token: 0x170006FC RID: 1788
		// (get) Token: 0x06001545 RID: 5445 RVA: 0x0004534F File Offset: 0x0004354F
		// (set) Token: 0x06001546 RID: 5446 RVA: 0x00045357 File Offset: 0x00043557
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

		// Token: 0x170006FD RID: 1789
		// (get) Token: 0x06001547 RID: 5447 RVA: 0x00045375 File Offset: 0x00043575
		// (set) Token: 0x06001548 RID: 5448 RVA: 0x0004537D File Offset: 0x0004357D
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

		// Token: 0x170006FE RID: 1790
		// (get) Token: 0x06001549 RID: 5449 RVA: 0x000453A6 File Offset: 0x000435A6
		// (set) Token: 0x0600154A RID: 5450 RVA: 0x000453AE File Offset: 0x000435AE
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

		// Token: 0x04000A2C RID: 2604
		private bool _isPrimary;

		// Token: 0x04000A2D RID: 2605
		private string _text;

		// Token: 0x04000A2E RID: 2606
		private int _distance;

		// Token: 0x04000A2F RID: 2607
		private float _position;

		// Token: 0x04000A30 RID: 2608
		private float _fullPosition;
	}
}
