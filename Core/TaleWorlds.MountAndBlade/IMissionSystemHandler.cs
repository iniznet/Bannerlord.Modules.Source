using System;
using System.Collections.Generic;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000232 RID: 562
	public interface IMissionSystemHandler
	{
		// Token: 0x06001F20 RID: 7968
		void OnMissionAfterStarting(Mission mission);

		// Token: 0x06001F21 RID: 7969
		void OnMissionLoadingFinished(Mission mission);

		// Token: 0x06001F22 RID: 7970
		void BeforeMissionTick(Mission mission, float realDt);

		// Token: 0x06001F23 RID: 7971
		void AfterMissionTick(Mission mission, float realDt);

		// Token: 0x06001F24 RID: 7972
		void UpdateCamera(Mission mission, float realDt);

		// Token: 0x06001F25 RID: 7973
		bool RenderIsReady();

		// Token: 0x06001F26 RID: 7974
		IEnumerable<MissionBehavior> OnAddBehaviors(IEnumerable<MissionBehavior> behaviors, Mission mission, string missionName, bool addDefaultMissionBehaviors);
	}
}
