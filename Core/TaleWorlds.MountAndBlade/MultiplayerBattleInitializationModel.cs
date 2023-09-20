using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.ComponentInterfaces;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001EA RID: 490
	public class MultiplayerBattleInitializationModel : BattleInitializationModel
	{
		// Token: 0x06001B88 RID: 7048 RVA: 0x00061BC8 File Offset: 0x0005FDC8
		public override List<FormationClass> GetAllAvailableTroopTypes()
		{
			return new List<FormationClass>();
		}

		// Token: 0x06001B89 RID: 7049 RVA: 0x00061BCF File Offset: 0x0005FDCF
		protected override bool CanPlayerSideDeployWithOrderOfBattleAux()
		{
			return false;
		}
	}
}
