using System;

namespace TaleWorlds.Library
{
	[Serializable]
	public struct Vec3i
	{
		public Vec3i(int x = 0, int y = 0, int z = 0)
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
		}

		public static bool operator ==(Vec3i v1, Vec3i v2)
		{
			return v1.X == v2.X && v1.Y == v2.Y && v1.Z == v2.Z;
		}

		public static bool operator !=(Vec3i v1, Vec3i v2)
		{
			return v1.X != v2.X || v1.Y != v2.Y || v1.Z != v2.Z;
		}

		public Vec3 ToVec3()
		{
			return new Vec3((float)this.X, (float)this.Y, (float)this.Z, -1f);
		}

		public int this[int index]
		{
			get
			{
				if (index == 0)
				{
					return this.X;
				}
				if (index != 1)
				{
					return this.Z;
				}
				return this.Y;
			}
			set
			{
				if (index == 0)
				{
					this.X = value;
					return;
				}
				if (index == 1)
				{
					this.Y = value;
					return;
				}
				this.Z = value;
			}
		}

		public static Vec3i operator *(Vec3i v, int mult)
		{
			return new Vec3i(v.X * mult, v.Y * mult, v.Z * mult);
		}

		public static Vec3i operator +(Vec3i v1, Vec3i v2)
		{
			return new Vec3i(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
		}

		public static Vec3i operator -(Vec3i v1, Vec3i v2)
		{
			return new Vec3i(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
		}

		public override bool Equals(object obj)
		{
			return obj != null && !(base.GetType() != obj.GetType()) && (((Vec3i)obj).X == this.X && ((Vec3i)obj).Y == this.Y) && ((Vec3i)obj).Z == this.Z;
		}

		public override int GetHashCode()
		{
			return (((this.X * 397) ^ this.Y) * 397) ^ this.Z;
		}

		public override string ToString()
		{
			return string.Format("{0}: {1}, {2}: {3}, {4}: {5}", new object[] { "X", this.X, "Y", this.Y, "Z", this.Z });
		}

		public int X;

		public int Y;

		public int Z;

		public static readonly Vec3i Zero = new Vec3i(0, 0, 0);
	}
}
