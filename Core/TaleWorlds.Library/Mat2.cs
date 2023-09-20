using System;

namespace TaleWorlds.Library
{
	public struct Mat2
	{
		public Mat2(float sx, float sy, float fx, float fy)
		{
			this.s.x = sx;
			this.s.y = sy;
			this.f.x = fx;
			this.f.y = fy;
		}

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

		public Vec2 s;

		public Vec2 f;

		public static Mat2 Identity = new Mat2(1f, 0f, 0f, 1f);
	}
}
