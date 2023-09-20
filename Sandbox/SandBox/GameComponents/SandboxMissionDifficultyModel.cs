using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ComponentInterfaces;

namespace SandBox.GameComponents
{
	// Token: 0x0200008D RID: 141
	public class SandboxMissionDifficultyModel : MissionDifficultyModel
	{
		// Token: 0x060005DE RID: 1502 RVA: 0x0002C9C8 File Offset: 0x0002ABC8
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
					IAgentOriginBase origin = victimAgent.Origin;
					PartyBase partyBase;
					if ((partyBase = ((origin != null) ? origin.BattleCombatant : null) as PartyBase) != null)
					{
						Mission mission = Mission.Current;
						object obj;
						if (mission == null)
						{
							obj = null;
						}
						else
						{
							Agent mainAgent = mission.MainAgent;
							if (mainAgent == null)
							{
								obj = null;
							}
							else
							{
								IAgentOriginBase origin2 = mainAgent.Origin;
								obj = ((origin2 != null) ? origin2.BattleCombatant : null);
							}
						}
						PartyBase partyBase2;
						if ((partyBase2 = obj as PartyBase) != null && partyBase == partyBase2)
						{
							if (attackerAgent != null)
							{
								Mission mission2 = Mission.Current;
								if (attackerAgent == ((mission2 != null) ? mission2.MainAgent : null))
								{
									return Mission.Current.DamageFromPlayerToFriendsMultiplier;
								}
							}
							num = Mission.Current.DamageToFriendsMultiplier;
						}
					}
				}
			}
			return num;
		}
	}
}
