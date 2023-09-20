using System;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.Map
{
	// Token: 0x020000CD RID: 205
	public struct LocatableSearchData<T>
	{
		// Token: 0x0600129F RID: 4767 RVA: 0x00054420 File Offset: 0x00052620
		public LocatableSearchData(Vec2 position, float radius, int minX, int minY, int maxX, int maxY)
		{
			this.Position = position;
			this.RadiusSquared = radius * radius;
			this.MinY = minY;
			this.MaxXInclusive = maxX;
			this.MaxYInclusive = maxY;
			this.CurrentX = minX;
			this.CurrentY = minY - 1;
			this.CurrentLocatable = null;
		}

		// Token: 0x0400067C RID: 1660
		public readonly Vec2 Position;

		// Token: 0x0400067D RID: 1661
		public readonly float RadiusSquared;

		// Token: 0x0400067E RID: 1662
		public readonly int MinY;

		// Token: 0x0400067F RID: 1663
		public readonly int MaxXInclusive;

		// Token: 0x04000680 RID: 1664
		public readonly int MaxYInclusive;

		// Token: 0x04000681 RID: 1665
		public int CurrentX;

		// Token: 0x04000682 RID: 1666
		public int CurrentY;

		// Token: 0x04000683 RID: 1667
		internal ILocatable<T> CurrentLocatable;
	}
}
