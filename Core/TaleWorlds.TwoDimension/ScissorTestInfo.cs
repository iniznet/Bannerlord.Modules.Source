using System;

namespace TaleWorlds.TwoDimension
{
	public struct ScissorTestInfo
	{
		public ScissorTestInfo(int x, int y, int width, int height)
		{
			this.X = x;
			this.Y = y;
			this.Width = width;
			this.Height = height;
		}

		public static explicit operator Quad(ScissorTestInfo scissor)
		{
			return new Quad
			{
				X = scissor.X,
				Y = scissor.Y,
				Width = scissor.Width,
				Height = scissor.Height
			};
		}

		public int X;

		public int Y;

		public int Width;

		public int Height;
	}
}
