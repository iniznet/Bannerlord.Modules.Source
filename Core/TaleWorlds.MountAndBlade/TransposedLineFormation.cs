using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000133 RID: 307
	public class TransposedLineFormation : LineFormation
	{
		// Token: 0x06000F72 RID: 3954 RVA: 0x0002E35B File Offset: 0x0002C55B
		public TransposedLineFormation(IFormation owner)
			: base(owner, true)
		{
			base.IsStaggered = false;
			this.IsTransforming = true;
		}

		// Token: 0x06000F73 RID: 3955 RVA: 0x0002E373 File Offset: 0x0002C573
		public override IFormationArrangement Clone(IFormation formation)
		{
			return new TransposedLineFormation(formation);
		}

		// Token: 0x06000F74 RID: 3956 RVA: 0x0002E37C File Offset: 0x0002C57C
		public override void RearrangeFrom(IFormationArrangement arrangement)
		{
			if (arrangement is ColumnFormation)
			{
				int unitCount = arrangement.UnitCount;
				if (unitCount > 0)
				{
					int? fileCountStatic = FormOrder.GetFileCountStatic(((Formation)this.owner).FormOrder.OrderEnum, unitCount);
					if (fileCountStatic != null)
					{
						int num = MathF.Ceiling((float)unitCount * 1f / (float)fileCountStatic.Value);
						base.FormFromFlankWidth(num, false);
					}
				}
			}
			base.RearrangeFrom(arrangement);
		}
	}
}
