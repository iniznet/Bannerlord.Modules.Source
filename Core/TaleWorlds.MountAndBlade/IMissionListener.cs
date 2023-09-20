using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200024D RID: 589
	public interface IMissionListener
	{
		// Token: 0x06001FF0 RID: 8176
		void OnEquipItemsFromSpawnEquipmentBegin(Agent agent, Agent.CreationType creationType);

		// Token: 0x06001FF1 RID: 8177
		void OnEquipItemsFromSpawnEquipment(Agent agent, Agent.CreationType creationType);

		// Token: 0x06001FF2 RID: 8178
		void OnEndMission();

		// Token: 0x06001FF3 RID: 8179
		void OnMissionModeChange(MissionMode oldMissionMode, bool atStart);

		// Token: 0x06001FF4 RID: 8180
		void OnConversationCharacterChanged();

		// Token: 0x06001FF5 RID: 8181
		void OnResetMission();

		// Token: 0x06001FF6 RID: 8182
		void OnInitialDeploymentPlanMade(BattleSideEnum battleSide, bool isFirstPlan);
	}
}
