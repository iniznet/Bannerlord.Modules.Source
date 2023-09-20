using System;

namespace TaleWorlds.TwoDimension
{
	// Token: 0x0200002C RID: 44
	public struct ScissorTestInfo
	{
		// Token: 0x060001CE RID: 462 RVA: 0x00007838 File Offset: 0x00005A38
		public ScissorTestInfo(int x, int y, int width, int height)
		{
			this.X = x;
			this.Y = y;
			this.Width = width;
			this.Height = height;
		}

		// Token: 0x060001CF RID: 463 RVA: 0x00007858 File Offset: 0x00005A58
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

		// Token: 0x040000F0 RID: 240
		public int X;

		// Token: 0x040000F1 RID: 241
		public int Y;

		// Token: 0x040000F2 RID: 242
		public int Width;

		// Token: 0x040000F3 RID: 243
		public int Height;
	}
}
