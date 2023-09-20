using System;

namespace TaleWorlds.Library
{
	[Serializable]
	public struct Vec2i : IEquatable<Vec2i>
	{
		public int Item1
		{
			get
			{
				return this.X;
			}
		}

		public int Item2
		{
			get
			{
				return this.Y;
			}
		}

		public Vec2i(int x = 0, int y = 0)
		{
			this.X = x;
			this.Y = y;
		}

		public static bool operator ==(Vec2i a, Vec2i b)
		{
			return a.X == b.X && a.Y == b.Y;
		}

		public static bool operator !=(Vec2i a, Vec2i b)
		{
			return a.X != b.X || a.Y != b.Y;
		}

		public override bool Equals(object obj)
		{
			return obj != null && !(base.GetType() != obj.GetType()) && ((Vec2i)obj).X == this.X && ((Vec2i)obj).Y == this.Y;
		}

		public bool Equals(Vec2i value)
		{
			return value.X == this.X && value.Y == this.Y;
		}

		public override int GetHashCode()
		{
			return (23 * 31 + this.X.GetHashCode()) * 31 + this.Y.GetHashCode();
		}

		public int X;

		public int Y;

		public static readonly Vec2i Side = new Vec2i(1, 0);

		public static readonly Vec2i Forward = new Vec2i(0, 1);

		public static readonly Vec2i One = new Vec2i(1, 1);

		public static readonly Vec2i Zero = new Vec2i(0, 0);
	}
}
