﻿using System;
using TaleWorlds.Library.EventSystem;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.OrderOfBattle
{
	public class OrderOfBattleHeroAssignedToFormationEvent : EventBase
	{
		public Agent AssignedHero { get; private set; }

		public Formation AssignedFormation { get; private set; }

		public OrderOfBattleHeroAssignedToFormationEvent(Agent assignedHero, Formation assignedFormation)
		{
			this.AssignedHero = assignedHero;
			this.AssignedFormation = assignedFormation;
		}
	}
}
