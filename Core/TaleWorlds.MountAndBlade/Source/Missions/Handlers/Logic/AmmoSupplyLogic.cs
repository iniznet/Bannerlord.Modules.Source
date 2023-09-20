using System;
using System.Collections.Generic;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.Source.Missions.Handlers.Logic
{
	public class AmmoSupplyLogic : MissionLogic
	{
		public AmmoSupplyLogic(List<BattleSideEnum> sideList)
		{
			this._sideList = sideList;
			this._checkTimer = new BasicMissionTimer();
		}

		public bool IsAgentEligibleForAmmoSupply(Agent agent)
		{
			if (agent.IsAIControlled && this._sideList.Contains(agent.Team.Side))
			{
				for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; equipmentIndex++)
				{
					if (!agent.Equipment[equipmentIndex].IsEmpty && agent.Equipment[equipmentIndex].IsAnyAmmo())
					{
						return true;
					}
				}
			}
			return false;
		}

		public override void OnMissionTick(float dt)
		{
			if (this._checkTimer.ElapsedTime > 3f)
			{
				this._checkTimer.Reset();
				foreach (Team team in base.Mission.Teams)
				{
					if (this._sideList.IndexOf(team.Side) >= 0)
					{
						foreach (Agent agent in team.ActiveAgents)
						{
							for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; equipmentIndex++)
							{
								if (agent.IsAIControlled && !agent.Equipment[equipmentIndex].IsEmpty && agent.Equipment[equipmentIndex].IsAnyAmmo())
								{
									short modifiedMaxAmount = agent.Equipment[equipmentIndex].ModifiedMaxAmount;
									short amount = agent.Equipment[equipmentIndex].Amount;
									short num = modifiedMaxAmount;
									if (modifiedMaxAmount > 1)
									{
										num = modifiedMaxAmount - 1;
									}
									if (amount < num)
									{
										agent.SetWeaponAmountInSlot(equipmentIndex, num, false);
									}
								}
							}
						}
					}
				}
			}
		}

		private const float CheckTimePeriod = 3f;

		private readonly List<BattleSideEnum> _sideList;

		private readonly BasicMissionTimer _checkTimer;
	}
}
