using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000262 RID: 610
	public class BattleMissionStarterLogic : MissionLogic
	{
		// Token: 0x060020C2 RID: 8386 RVA: 0x000755B0 File Offset: 0x000737B0
		public BattleMissionStarterLogic()
		{
		}

		// Token: 0x060020C3 RID: 8387 RVA: 0x000755B8 File Offset: 0x000737B8
		public BattleMissionStarterLogic(IMissionTroopSupplier defenderTroopSupplier = null, IMissionTroopSupplier attackerTroopSupplier = null)
		{
		}

		// Token: 0x060020C4 RID: 8388 RVA: 0x000755C0 File Offset: 0x000737C0
		public override void AfterStart()
		{
			base.Mission.SetMissionMode(MissionMode.Battle, true);
		}
	}
}
