﻿using System;

namespace TaleWorlds.MountAndBlade.Source.Missions.Handlers.Logic
{
	public class BattleMissionAgentInteractionLogic : MissionLogic
	{
		public override bool IsThereAgentAction(Agent userAgent, Agent otherAgent)
		{
			return !otherAgent.IsEnemyOf(userAgent);
		}
	}
}
