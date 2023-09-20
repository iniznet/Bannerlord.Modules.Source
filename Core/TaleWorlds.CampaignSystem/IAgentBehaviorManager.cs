﻿using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem
{
	public interface IAgentBehaviorManager
	{
		void AddQuestCharacterBehaviors(IAgent agent);

		void AddWandererBehaviors(IAgent agent);

		void AddOutdoorWandererBehaviors(IAgent agent);

		void AddIndoorWandererBehaviors(IAgent agent);

		void AddFixedCharacterBehaviors(IAgent agent);

		void AddAmbushPlayerBehaviors(IAgent agent);

		void AddStandGuardBehaviors(IAgent agent);

		void AddPatrollingGuardBehaviors(IAgent agent);

		void AddCompanionBehaviors(IAgent agent);

		void AddBodyguardBehaviors(IAgent agent);

		void AddFirstCompanionBehavior(IAgent agent);
	}
}
