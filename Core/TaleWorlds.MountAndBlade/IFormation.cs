using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200012F RID: 303
	public interface IFormation
	{
		// Token: 0x17000343 RID: 835
		// (get) Token: 0x06000E8A RID: 3722
		float Interval { get; }

		// Token: 0x17000344 RID: 836
		// (get) Token: 0x06000E8B RID: 3723
		float Distance { get; }

		// Token: 0x17000345 RID: 837
		// (get) Token: 0x06000E8C RID: 3724
		float UnitDiameter { get; }

		// Token: 0x17000346 RID: 838
		// (get) Token: 0x06000E8D RID: 3725
		float MinimumInterval { get; }

		// Token: 0x17000347 RID: 839
		// (get) Token: 0x06000E8E RID: 3726
		float MaximumInterval { get; }

		// Token: 0x17000348 RID: 840
		// (get) Token: 0x06000E8F RID: 3727
		float MinimumDistance { get; }

		// Token: 0x17000349 RID: 841
		// (get) Token: 0x06000E90 RID: 3728
		float MaximumDistance { get; }

		// Token: 0x1700034A RID: 842
		// (get) Token: 0x06000E91 RID: 3729
		int? OverridenUnitCount { get; }

		// Token: 0x06000E92 RID: 3730
		bool GetIsLocalPositionAvailable(Vec2 localPosition, Vec2? nearestAvailableUnitPositionLocal);

		// Token: 0x06000E93 RID: 3731
		bool BatchUnitPositions(MBArrayList<Vec2i> orderedPositionIndices, MBArrayList<Vec2> orderedLocalPositions, MBList2D<int> availabilityTable, MBList2D<WorldPosition> globalPositionTable, int fileCount, int rankCount);

		// Token: 0x06000E94 RID: 3732
		IFormationUnit GetClosestUnitTo(Vec2 localPosition, MBList<IFormationUnit> unitsWithSpaces = null, float? maxDistance = null);

		// Token: 0x06000E95 RID: 3733
		IFormationUnit GetClosestUnitTo(IFormationUnit targetUnit, MBList<IFormationUnit> unitsWithSpaces = null, float? maxDistance = null);

		// Token: 0x06000E96 RID: 3734
		void OnUnitAddedOrRemoved();

		// Token: 0x06000E97 RID: 3735
		void SetUnitToFollow(IFormationUnit unit, IFormationUnit toFollow, Vec2 vector);
	}
}
