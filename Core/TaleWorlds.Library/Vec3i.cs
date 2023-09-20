using System;

namespace TaleWorlds.Library
{
	// Token: 0x0200009A RID: 154
	[Serializable]
	public struct Vec3i
	{
		// Token: 0x0600058E RID: 1422 RVA: 0x00011C91 File Offset: 0x0000FE91
		public Vec3i(int x = 0, int y = 0, int z = 0)
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
		}

		// Token: 0x0600058F RID: 1423 RVA: 0x00011CA8 File Offset: 0x0000FEA8
		public static bool operator ==(Vec3i v1, Vec3i v2)
		{
			return v1.X == v2.X && v1.Y == v2.Y && v1.Z == v2.Z;
		}

		// Token: 0x06000590 RID: 1424 RVA: 0x00011CD6 File Offset: 0x0000FED6
		public static bool operator !=(Vec3i v1, Vec3i v2)
		{
			return v1.X != v2.X || v1.Y != v2.Y || v1.Z != v2.Z;
		}

		// Token: 0x06000591 RID: 1425 RVA: 0x00011D07 File Offset: 0x0000FF07
		public Vec3 ToVec3()
		{
			return new Vec3((float)this.X, (float)this.Y, (float)this.Z, -1f);
		}

		// Token: 0x170000A5 RID: 165
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

		// Token: 0x06000594 RID: 1428 RVA: 0x00011D65 File Offset: 0x0000FF65
		public static Vec3i operator *(Vec3i v, int mult)
		{
			return new Vec3i(v.X * mult, v.Y * mult, v.Z * mult);
		}

		// Token: 0x06000595 RID: 1429 RVA: 0x00011D84 File Offset: 0x0000FF84
		public static Vec3i operator +(Vec3i v1, Vec3i v2)
		{
			return new Vec3i(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
		}

		// Token: 0x06000596 RID: 1430 RVA: 0x00011DB2 File Offset: 0x0000FFB2
		public static Vec3i operator -(Vec3i v1, Vec3i v2)
		{
			return new Vec3i(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
		}

		// Token: 0x06000597 RID: 1431 RVA: 0x00011DE0 File Offset: 0x0000FFE0
		public override bool Equals(object obj)
		{
			return obj != null && !(base.GetType() != obj.GetType()) && (((Vec3i)obj).X == this.X && ((Vec3i)obj).Y == this.Y) && ((Vec3i)obj).Z == this.Z;
		}

		// Token: 0x06000598 RID: 1432 RVA: 0x00011E4A File Offset: 0x0001004A
		public override int GetHashCode()
		{
			return (((this.X * 397) ^ this.Y) * 397) ^ this.Z;
		}

		// Token: 0x06000599 RID: 1433 RVA: 0x00011E6C File Offset: 0x0001006C
		public override string ToString()
		{
			return string.Format("{0}: {1}, {2}: {3}, {4}: {5}", new object[] { "X", this.X, "Y", this.Y, "Z", this.Z });
		}

		// Token: 0x0400019B RID: 411
		public int X;

		// Token: 0x0400019C RID: 412
		public int Y;

		// Token: 0x0400019D RID: 413
		public int Z;

		// Token: 0x0400019E RID: 414
		public static readonly Vec3i Zero = new Vec3i(0, 0, 0);
	}
}
