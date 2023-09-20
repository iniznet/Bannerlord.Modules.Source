using System;

namespace TaleWorlds.Library
{
	// Token: 0x02000098 RID: 152
	[Serializable]
	public struct Vec2i : IEquatable<Vec2i>
	{
		// Token: 0x17000095 RID: 149
		// (get) Token: 0x0600054C RID: 1356 RVA: 0x00010B5D File Offset: 0x0000ED5D
		public int Item1
		{
			get
			{
				return this.X;
			}
		}

		// Token: 0x17000096 RID: 150
		// (get) Token: 0x0600054D RID: 1357 RVA: 0x00010B65 File Offset: 0x0000ED65
		public int Item2
		{
			get
			{
				return this.Y;
			}
		}

		// Token: 0x0600054E RID: 1358 RVA: 0x00010B6D File Offset: 0x0000ED6D
		public Vec2i(int x = 0, int y = 0)
		{
			this.X = x;
			this.Y = y;
		}

		// Token: 0x0600054F RID: 1359 RVA: 0x00010B7D File Offset: 0x0000ED7D
		public static bool operator ==(Vec2i a, Vec2i b)
		{
			return a.X == b.X && a.Y == b.Y;
		}

		// Token: 0x06000550 RID: 1360 RVA: 0x00010B9D File Offset: 0x0000ED9D
		public static bool operator !=(Vec2i a, Vec2i b)
		{
			return a.X != b.X || a.Y != b.Y;
		}

		// Token: 0x06000551 RID: 1361 RVA: 0x00010BC0 File Offset: 0x0000EDC0
		public override bool Equals(object obj)
		{
			return obj != null && !(base.GetType() != obj.GetType()) && ((Vec2i)obj).X == this.X && ((Vec2i)obj).Y == this.Y;
		}

		// Token: 0x06000552 RID: 1362 RVA: 0x00010C17 File Offset: 0x0000EE17
		public bool Equals(Vec2i value)
		{
			return value.X == this.X && value.Y == this.Y;
		}

		// Token: 0x06000553 RID: 1363 RVA: 0x00010C37 File Offset: 0x0000EE37
		public override int GetHashCode()
		{
			return (23 * 31 + this.X.GetHashCode()) * 31 + this.Y.GetHashCode();
		}

		// Token: 0x0400018B RID: 395
		public int X;

		// Token: 0x0400018C RID: 396
		public int Y;

		// Token: 0x0400018D RID: 397
		public static readonly Vec2i Side = new Vec2i(1, 0);

		// Token: 0x0400018E RID: 398
		public static readonly Vec2i Forward = new Vec2i(0, 1);

		// Token: 0x0400018F RID: 399
		public static readonly Vec2i One = new Vec2i(1, 1);

		// Token: 0x04000190 RID: 400
		public static readonly Vec2i Zero = new Vec2i(0, 0);
	}
}
