﻿using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.ComponentInterfaces;

namespace TaleWorlds.MountAndBlade
{
	public class CustomBattleInitializationModel : BattleInitializationModel
	{
		public override List<FormationClass> GetAllAvailableTroopTypes()
		{
			List<FormationClass> list = new List<FormationClass>();
			foreach (Agent agent in Mission.Current.PlayerTeam.ActiveAgents)
			{
				BasicCharacterObject character = agent.Character;
				if (character.IsInfantry && !character.IsMounted && !list.Contains(FormationClass.Infantry))
				{
					list.Add(FormationClass.Infantry);
				}
				if (character.IsRanged && !character.IsMounted && !list.Contains(FormationClass.Ranged))
				{
					list.Add(FormationClass.Ranged);
				}
				if (character.IsMounted && !character.IsRanged && !list.Contains(FormationClass.Cavalry))
				{
					list.Add(FormationClass.Cavalry);
				}
				if (character.IsMounted && character.IsRanged && !list.Contains(FormationClass.HorseArcher))
				{
					list.Add(FormationClass.HorseArcher);
				}
			}
			return list;
		}

		protected override bool CanPlayerSideDeployWithOrderOfBattleAux()
		{
			if (Mission.Current.IsSallyOutBattle)
			{
				return false;
			}
			Team playerTeam = Mission.Current.PlayerTeam;
			return Mission.Current.GetMissionBehavior<MissionAgentSpawnLogic>().GetNumberOfPlayerControllableTroops() >= 20;
		}
	}
}
