using System;
using System.Numerics;

namespace TaleWorlds.TwoDimension
{
	public struct Rectangle
	{
		public float Width
		{
			get
			{
				return this.X2 - this.X;
			}
		}

		public float Height
		{
			get
			{
				return this.Y2 - this.Y;
			}
		}

		public Rectangle(float x, float y, float width, float height)
		{
			this.X = x;
			this.Y = y;
			this.X2 = x + width;
			this.Y2 = y + height;
		}

		public bool IsCollide(Rectangle other)
		{
			return other.X <= this.X2 && other.X2 >= this.X && other.Y <= this.Y2 && other.Y2 >= this.Y;
		}

		public bool IsSubRectOf(Rectangle other)
		{
			return other.X <= this.X && other.X2 >= this.X2 && other.Y <= this.Y && other.Y2 >= this.Y2;
		}

		public bool IsValid()
		{
			return this.Width > 0f && this.Height > 0f;
		}

		public bool IsPointInside(Vector2 point)
		{
			return point.X >= this.X && point.Y >= this.Y && point.X <= this.X2 && point.Y <= this.Y2;
		}

		public float X;

		public float Y;

		public float X2;

		public float Y2;
	}
}
