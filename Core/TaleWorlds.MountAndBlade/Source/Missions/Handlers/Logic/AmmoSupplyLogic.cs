using System;
using System.Collections.Generic;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.Source.Missions.Handlers.Logic
{
	// Token: 0x020003FD RID: 1021
	public class AmmoSupplyLogic : MissionLogic
	{
		// Token: 0x06003512 RID: 13586 RVA: 0x000DD461 File Offset: 0x000DB661
		public AmmoSupplyLogic(List<BattleSideEnum> sideList)
		{
			this._sideList = sideList;
			this._checkTimer = new BasicMissionTimer();
		}

		// Token: 0x06003513 RID: 13587 RVA: 0x000DD47C File Offset: 0x000DB67C
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

		// Token: 0x06003514 RID: 13588 RVA: 0x000DD4E4 File Offset: 0x000DB6E4
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

		// Token: 0x040016C5 RID: 5829
		private const float CheckTimePeriod = 3f;

		// Token: 0x040016C6 RID: 5830
		private readonly List<BattleSideEnum> _sideList;

		// Token: 0x040016C7 RID: 5831
		private readonly BasicMissionTimer _checkTimer;
	}
}
