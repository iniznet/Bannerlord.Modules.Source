using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Objects;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000252 RID: 594
	public interface IAnalyticsFlagInfo : IMissionBehavior
	{
		// Token: 0x17000642 RID: 1602
		// (get) Token: 0x06002002 RID: 8194
		MBReadOnlyList<FlagCapturePoint> AllCapturePoints { get; }

		// Token: 0x06002003 RID: 8195
		Team GetFlagOwnerTeam(FlagCapturePoint flag);
	}
}
