using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000373 RID: 883
	public class ClearHandInverseKinematicsOnStopUsageComponent : UsableMissionObjectComponent
	{
		// Token: 0x0600304D RID: 12365 RVA: 0x000C6AD7 File Offset: 0x000C4CD7
		protected internal override void OnUseStopped(Agent userAgent, bool isSuccessful = true)
		{
			userAgent.ClearHandInverseKinematics();
		}
	}
}
