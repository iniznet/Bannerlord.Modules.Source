using System;
using System.Numerics;

namespace TaleWorlds.TwoDimension
{
	// Token: 0x0200002B RID: 43
	public struct Rectangle
	{
		// Token: 0x17000097 RID: 151
		// (get) Token: 0x060001C7 RID: 455 RVA: 0x0000771C File Offset: 0x0000591C
		public float Width
		{
			get
			{
				return this.X2 - this.X;
			}
		}

		// Token: 0x17000098 RID: 152
		// (get) Token: 0x060001C8 RID: 456 RVA: 0x0000772B File Offset: 0x0000592B
		public float Height
		{
			get
			{
				return this.Y2 - this.Y;
			}
		}

		// Token: 0x060001C9 RID: 457 RVA: 0x0000773A File Offset: 0x0000593A
		public Rectangle(float x, float y, float width, float height)
		{
			this.X = x;
			this.Y = y;
			this.X2 = x + width;
			this.Y2 = y + height;
		}

		// Token: 0x060001CA RID: 458 RVA: 0x0000775D File Offset: 0x0000595D
		public bool IsCollide(Rectangle other)
		{
			return other.X <= this.X2 && other.X2 >= this.X && other.Y <= this.Y2 && other.Y2 >= this.Y;
		}

		// Token: 0x060001CB RID: 459 RVA: 0x0000779C File Offset: 0x0000599C
		public bool IsSubRectOf(Rectangle other)
		{
			return other.X <= this.X && other.X2 >= this.X2 && other.Y <= this.Y && other.Y2 >= this.Y2;
		}

		// Token: 0x060001CC RID: 460 RVA: 0x000077DB File Offset: 0x000059DB
		public bool IsValid()
		{
			return this.Width > 0f && this.Height > 0f;
		}

		// Token: 0x060001CD RID: 461 RVA: 0x000077F9 File Offset: 0x000059F9
		public bool IsPointInside(Vector2 point)
		{
			return point.X >= this.X && point.Y >= this.Y && point.X <= this.X2 && point.Y <= this.Y2;
		}

		// Token: 0x040000EC RID: 236
		public float X;

		// Token: 0x040000ED RID: 237
		public float Y;

		// Token: 0x040000EE RID: 238
		public float X2;

		// Token: 0x040000EF RID: 239
		public float Y2;
	}
}
