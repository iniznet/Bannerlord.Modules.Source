using System;

namespace TaleWorlds.TwoDimension.Standalone.Native.Windows
{
	public struct Point
	{
		public Point(int x, int y)
		{
			this.X = x;
			this.Y = y;
		}

		public int X;

		public int Y;
	}
}
