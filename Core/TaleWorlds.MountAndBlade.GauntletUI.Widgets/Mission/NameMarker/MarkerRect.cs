using System;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.NameMarker
{
	// Token: 0x020000DC RID: 220
	public class MarkerRect
	{
		// Token: 0x1700040E RID: 1038
		// (get) Token: 0x06000B5B RID: 2907 RVA: 0x0001F8C3 File Offset: 0x0001DAC3
		// (set) Token: 0x06000B5C RID: 2908 RVA: 0x0001F8CB File Offset: 0x0001DACB
		public float Left { get; private set; }

		// Token: 0x1700040F RID: 1039
		// (get) Token: 0x06000B5D RID: 2909 RVA: 0x0001F8D4 File Offset: 0x0001DAD4
		// (set) Token: 0x06000B5E RID: 2910 RVA: 0x0001F8DC File Offset: 0x0001DADC
		public float Right { get; private set; }

		// Token: 0x17000410 RID: 1040
		// (get) Token: 0x06000B5F RID: 2911 RVA: 0x0001F8E5 File Offset: 0x0001DAE5
		// (set) Token: 0x06000B60 RID: 2912 RVA: 0x0001F8ED File Offset: 0x0001DAED
		public float Top { get; private set; }

		// Token: 0x17000411 RID: 1041
		// (get) Token: 0x06000B61 RID: 2913 RVA: 0x0001F8F6 File Offset: 0x0001DAF6
		// (set) Token: 0x06000B62 RID: 2914 RVA: 0x0001F8FE File Offset: 0x0001DAFE
		public float Bottom { get; private set; }

		// Token: 0x17000412 RID: 1042
		// (get) Token: 0x06000B63 RID: 2915 RVA: 0x0001F907 File Offset: 0x0001DB07
		public float CenterX
		{
			get
			{
				return this.Left + (this.Right - this.Left) / 2f;
			}
		}

		// Token: 0x17000413 RID: 1043
		// (get) Token: 0x06000B64 RID: 2916 RVA: 0x0001F923 File Offset: 0x0001DB23
		public float CenterY
		{
			get
			{
				return this.Top + (this.Bottom - this.Top) / 2f;
			}
		}

		// Token: 0x17000414 RID: 1044
		// (get) Token: 0x06000B65 RID: 2917 RVA: 0x0001F93F File Offset: 0x0001DB3F
		public float Width
		{
			get
			{
				return this.Right - this.Left;
			}
		}

		// Token: 0x17000415 RID: 1045
		// (get) Token: 0x06000B66 RID: 2918 RVA: 0x0001F94E File Offset: 0x0001DB4E
		public float Height
		{
			get
			{
				return this.Bottom - this.Top;
			}
		}

		// Token: 0x06000B67 RID: 2919 RVA: 0x0001F95D File Offset: 0x0001DB5D
		public MarkerRect()
		{
			this.Reset();
		}

		// Token: 0x06000B68 RID: 2920 RVA: 0x0001F96B File Offset: 0x0001DB6B
		public void Reset()
		{
			this.Left = 0f;
			this.Right = 0f;
			this.Top = 0f;
			this.Bottom = 0f;
		}

		// Token: 0x06000B69 RID: 2921 RVA: 0x0001F999 File Offset: 0x0001DB99
		public void UpdatePoints(float left, float right, float top, float bottom)
		{
			this.Left = left;
			this.Right = right;
			this.Top = top;
			this.Bottom = bottom;
		}

		// Token: 0x06000B6A RID: 2922 RVA: 0x0001F9B8 File Offset: 0x0001DBB8
		public bool IsOverlapping(MarkerRect other)
		{
			return other.Left <= this.Right && other.Right >= this.Left && other.Top <= this.Bottom && other.Bottom >= this.Top;
		}
	}
}
