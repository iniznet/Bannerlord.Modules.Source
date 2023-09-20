using System;
using System.Collections.Generic;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000259 RID: 601
	// (Invoke) Token: 0x06002074 RID: 8308
	public delegate void PlayerTurnToChooseFormationToLeadEvent(Dictionary<int, Agent> lockedFormationIndicesAndSergeants, List<int> remainingFormationIndices);
}
