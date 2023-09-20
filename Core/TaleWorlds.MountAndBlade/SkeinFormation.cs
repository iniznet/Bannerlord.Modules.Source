using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000135 RID: 309
	public class SkeinFormation : LineFormation
	{
		// Token: 0x06000F79 RID: 3961 RVA: 0x0002E46A File Offset: 0x0002C66A
		public SkeinFormation(IFormation owner)
			: base(owner, true)
		{
		}

		// Token: 0x06000F7A RID: 3962 RVA: 0x0002E474 File Offset: 0x0002C674
		public override IFormationArrangement Clone(IFormation formation)
		{
			return new SkeinFormation(formation);
		}

		// Token: 0x06000F7B RID: 3963 RVA: 0x0002E47C File Offset: 0x0002C67C
		protected override Vec2 GetLocalPositionOfUnit(int fileIndex, int rankIndex)
		{
			float num = (float)(base.FileCount - 1) * (base.Interval + base.UnitDiameter);
			Vec2 vec = new Vec2((float)fileIndex * (base.Interval + base.UnitDiameter) - num / 2f, (float)(-(float)rankIndex) * (base.Distance + base.UnitDiameter));
			float offsetOfFile = this.GetOffsetOfFile(fileIndex);
			vec.y -= offsetOfFile;
			return vec;
		}

		// Token: 0x06000F7C RID: 3964 RVA: 0x0002E4E8 File Offset: 0x0002C6E8
		protected override Vec2 GetLocalPositionOfUnitWithAdjustment(int fileIndex, int rankIndex, float distanceBetweenAgentsAdjustment)
		{
			float num = base.Interval + distanceBetweenAgentsAdjustment;
			float num2 = (float)(base.FileCount - 1) * (num + base.UnitDiameter);
			Vec2 vec = new Vec2((float)fileIndex * (num + base.UnitDiameter) - num2 / 2f, (float)(-(float)rankIndex) * (base.Distance + base.UnitDiameter));
			float offsetOfFile = this.GetOffsetOfFile(fileIndex);
			vec.y -= offsetOfFile;
			return vec;
		}

		// Token: 0x06000F7D RID: 3965 RVA: 0x0002E554 File Offset: 0x0002C754
		private float GetOffsetOfFile(int fileIndex)
		{
			int num = base.FileCount / 2;
			return (float)MathF.Abs(fileIndex - num) * (base.Interval + base.UnitDiameter) / 2f;
		}

		// Token: 0x06000F7E RID: 3966 RVA: 0x0002E588 File Offset: 0x0002C788
		protected override bool TryGetUnitPositionIndexFromLocalPosition(Vec2 localPosition, out int fileIndex, out int rankIndex)
		{
			float num = (float)(base.FileCount - 1) * (base.Interval + base.UnitDiameter);
			fileIndex = MathF.Round((localPosition.x + num / 2f) / (base.Interval + base.UnitDiameter));
			if (fileIndex < 0 || fileIndex >= base.FileCount)
			{
				rankIndex = -1;
				return false;
			}
			float offsetOfFile = this.GetOffsetOfFile(fileIndex);
			localPosition.y += offsetOfFile;
			rankIndex = MathF.Round(-localPosition.y / (base.Distance + base.UnitDiameter));
			if (rankIndex < 0 || rankIndex >= base.RankCount)
			{
				fileIndex = -1;
				return false;
			}
			return true;
		}
	}
}
