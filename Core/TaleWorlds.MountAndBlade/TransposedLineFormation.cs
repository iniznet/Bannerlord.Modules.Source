using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class TransposedLineFormation : LineFormation
	{
		public TransposedLineFormation(IFormation owner)
			: base(owner, true)
		{
			base.IsStaggered = false;
			this.IsTransforming = true;
		}

		public override IFormationArrangement Clone(IFormation formation)
		{
			return new TransposedLineFormation(formation);
		}

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
