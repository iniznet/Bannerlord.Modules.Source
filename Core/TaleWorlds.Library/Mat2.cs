using System;

namespace TaleWorlds.Library
{
	// Token: 0x0200005C RID: 92
	public struct Mat2
	{
		// Token: 0x0600028B RID: 651 RVA: 0x000073E9 File Offset: 0x000055E9
		public Mat2(float sx, float sy, float fx, float fy)
		{
			this.s.x = sx;
			this.s.y = sy;
			this.f.x = fx;
			this.f.y = fy;
		}

		// Token: 0x0600028C RID: 652 RVA: 0x0000741C File Offset: 0x0000561C
		public void RotateCounterClockWise(float a)
		{
			float num;
			float num2;
			MathF.SinCos(a, out num, out num2);
			Vec2 vec = this.s * num2 + this.f * num;
			Vec2 vec2 = this.f * num2 - this.s * num;
			this.s = vec;
			this.f = vec2;
		}

		// Token: 0x040000F4 RID: 244
		public Vec2 s;

		// Token: 0x040000F5 RID: 245
		public Vec2 f;

		// Token: 0x040000F6 RID: 246
		public static Mat2 Identity = new Mat2(1f, 0f, 0f, 1f);
	}
}
