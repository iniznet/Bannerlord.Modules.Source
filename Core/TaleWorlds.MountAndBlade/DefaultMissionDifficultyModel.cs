using System;
using TaleWorlds.MountAndBlade.ComponentInterfaces;

namespace TaleWorlds.MountAndBlade
{
	public class DefaultMissionDifficultyModel : MissionDifficultyModel
	{
		public override float GetDamageMultiplierOfCombatDifficulty(Agent victimAgent, Agent attackerAgent = null)
		{
			float num = 1f;
			victimAgent = (victimAgent.IsMount ? victimAgent.RiderAgent : victimAgent);
			if (victimAgent != null)
			{
				if (victimAgent.IsMainAgent)
				{
					num = Mission.Current.DamageToPlayerMultiplier;
				}
				else
				{
					Mission mission = Mission.Current;
					Agent agent = ((mission != null) ? mission.MainAgent : null);
					if (agent != null && victimAgent.IsFriendOf(agent))
					{
						if (attackerAgent != null && attackerAgent == agent)
						{
							num = Mission.Current.DamageFromPlayerToFriendsMultiplier;
						}
						else
						{
							num = Mission.Current.DamageToFriendsMultiplier;
						}
					}
				}
			}
			return num;
		}
	}
}
