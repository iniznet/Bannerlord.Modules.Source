﻿using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	public interface IMissionListener
	{
		void OnEquipItemsFromSpawnEquipmentBegin(Agent agent, Agent.CreationType creationType);

		void OnEquipItemsFromSpawnEquipment(Agent agent, Agent.CreationType creationType);

		void OnEndMission();

		void OnMissionModeChange(MissionMode oldMissionMode, bool atStart);

		void OnConversationCharacterChanged();

		void OnResetMission();

		void OnInitialDeploymentPlanMade(BattleSideEnum battleSide, bool isFirstPlan);
	}
}
