using System;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.Map
{
	public struct LocatableSearchData<T>
	{
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

		public readonly Vec2 Position;

		public readonly float RadiusSquared;

		public readonly int MinY;

		public readonly int MaxXInclusive;

		public readonly int MaxYInclusive;

		public int CurrentX;

		public int CurrentY;

		internal ILocatable<T> CurrentLocatable;
	}
}
