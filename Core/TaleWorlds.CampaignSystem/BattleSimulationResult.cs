using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x02000022 RID: 34
	public class BattleSimulationResult
	{
		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600014D RID: 333 RVA: 0x0000EA13 File Offset: 0x0000CC13
		// (set) Token: 0x0600014E RID: 334 RVA: 0x0000EA1B File Offset: 0x0000CC1B
		public UniqueTroopDescriptor TroopDescriptor { get; private set; }

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600014F RID: 335 RVA: 0x0000EA24 File Offset: 0x0000CC24
		// (set) Token: 0x06000150 RID: 336 RVA: 0x0000EA2C File Offset: 0x0000CC2C
		public BattleSideEnum Side { get; private set; }

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000151 RID: 337 RVA: 0x0000EA35 File Offset: 0x0000CC35
		// (set) Token: 0x06000152 RID: 338 RVA: 0x0000EA3D File Offset: 0x0000CC3D
		public TroopProperty TroopProperty { get; private set; }

		// Token: 0x06000153 RID: 339 RVA: 0x0000EA46 File Offset: 0x0000CC46
		public BattleSimulationResult(UniqueTroopDescriptor troopDescriptor, BattleSideEnum side, TroopProperty troopProperty)
		{
			this.TroopDescriptor = troopDescriptor;
			this.Side = side;
			this.TroopProperty = troopProperty;
		}
	}
}
