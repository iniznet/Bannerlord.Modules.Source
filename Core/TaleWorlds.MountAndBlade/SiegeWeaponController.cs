using System;
using System.Collections.Generic;
using System.Linq;
using NetworkMessages.FromClient;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000153 RID: 339
	public class SiegeWeaponController
	{
		// Token: 0x170003A2 RID: 930
		// (get) Token: 0x060010F7 RID: 4343 RVA: 0x00037D3C File Offset: 0x00035F3C
		public MBReadOnlyList<SiegeWeapon> SelectedWeapons
		{
			get
			{
				return this._selectedWeapons;
			}
		}

		// Token: 0x1400000C RID: 12
		// (add) Token: 0x060010F8 RID: 4344 RVA: 0x00037D44 File Offset: 0x00035F44
		// (remove) Token: 0x060010F9 RID: 4345 RVA: 0x00037D7C File Offset: 0x00035F7C
		public event Action<SiegeWeaponOrderType, IEnumerable<SiegeWeapon>> OnOrderIssued;

		// Token: 0x1400000D RID: 13
		// (add) Token: 0x060010FA RID: 4346 RVA: 0x00037DB4 File Offset: 0x00035FB4
		// (remove) Token: 0x060010FB RID: 4347 RVA: 0x00037DEC File Offset: 0x00035FEC
		public event Action OnSelectedSiegeWeaponsChanged;

		// Token: 0x060010FC RID: 4348 RVA: 0x00037E21 File Offset: 0x00036021
		public SiegeWeaponController(Mission mission, Team team)
		{
			this._mission = mission;
			this._team = team;
			this._selectedWeapons = new MBList<SiegeWeapon>();
			this.InitializeWeaponsForDeployment();
		}

		// Token: 0x060010FD RID: 4349 RVA: 0x00037E48 File Offset: 0x00036048
		private void InitializeWeaponsForDeployment()
		{
			IEnumerable<SiegeWeapon> enumerable = from w in (from dp in this._mission.ActiveMissionObjects.FindAllWithType<DeploymentPoint>()
					where dp.Side == this._team.Side
					select dp).SelectMany((DeploymentPoint dp) => dp.DeployableWeapons)
				select w as SiegeWeapon;
			this._availableWeapons = enumerable.ToList<SiegeWeapon>();
		}

		// Token: 0x060010FE RID: 4350 RVA: 0x00037ECC File Offset: 0x000360CC
		private void InitializeWeapons()
		{
			this._availableWeapons = new List<SiegeWeapon>();
			this._availableWeapons.AddRange(from w in this._mission.ActiveMissionObjects.FindAllWithType<RangedSiegeWeapon>()
				where w.Side == this._team.Side
				select w);
			if (this._team.Side == BattleSideEnum.Attacker)
			{
				this._availableWeapons.AddRange(from w in this._mission.ActiveMissionObjects.FindAllWithType<SiegeWeapon>()
					where w is IPrimarySiegeWeapon && !(w is RangedSiegeWeapon)
					select w);
			}
			this._availableWeapons.Sort((SiegeWeapon w1, SiegeWeapon w2) => this.GetShortcutIndexOf(w1).CompareTo(this.GetShortcutIndexOf(w2)));
		}

		// Token: 0x060010FF RID: 4351 RVA: 0x00037F74 File Offset: 0x00036174
		public void Select(SiegeWeapon weapon)
		{
			if (this.SelectedWeapons.Contains(weapon) || !SiegeWeaponController.IsWeaponSelectable(weapon))
			{
				Debug.FailedAssert("Weapon already selected or is not selectable", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\SiegeWeaponController.cs", "Select", 82);
				return;
			}
			if (GameNetwork.IsClient)
			{
				GameNetwork.BeginModuleEventAsClient();
				GameNetwork.WriteMessage(new SelectSiegeWeapon(weapon));
				GameNetwork.EndModuleEventAsClient();
			}
			this._selectedWeapons.Add(weapon);
			Action onSelectedSiegeWeaponsChanged = this.OnSelectedSiegeWeaponsChanged;
			if (onSelectedSiegeWeaponsChanged == null)
			{
				return;
			}
			onSelectedSiegeWeaponsChanged();
		}

		// Token: 0x06001100 RID: 4352 RVA: 0x00037FE6 File Offset: 0x000361E6
		public void ClearSelectedWeapons()
		{
			bool isClient = GameNetwork.IsClient;
			this._selectedWeapons.Clear();
			Action onSelectedSiegeWeaponsChanged = this.OnSelectedSiegeWeaponsChanged;
			if (onSelectedSiegeWeaponsChanged == null)
			{
				return;
			}
			onSelectedSiegeWeaponsChanged();
		}

		// Token: 0x06001101 RID: 4353 RVA: 0x0003800C File Offset: 0x0003620C
		public void Deselect(SiegeWeapon weapon)
		{
			if (!this.SelectedWeapons.Contains(weapon))
			{
				Debug.FailedAssert("Trying to deselect an unselected weapon", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\SiegeWeaponController.cs", "Deselect", 113);
				return;
			}
			if (GameNetwork.IsClient)
			{
				GameNetwork.BeginModuleEventAsClient();
				GameNetwork.WriteMessage(new UnselectSiegeWeapon(weapon));
				GameNetwork.EndModuleEventAsClient();
			}
			this._selectedWeapons.Remove(weapon);
			Action onSelectedSiegeWeaponsChanged = this.OnSelectedSiegeWeaponsChanged;
			if (onSelectedSiegeWeaponsChanged == null)
			{
				return;
			}
			onSelectedSiegeWeaponsChanged();
		}

		// Token: 0x06001102 RID: 4354 RVA: 0x00038078 File Offset: 0x00036278
		public void SelectAll()
		{
			if (GameNetwork.IsClient)
			{
				GameNetwork.BeginModuleEventAsClient();
				GameNetwork.WriteMessage(new SelectAllSiegeWeapons());
				GameNetwork.EndModuleEventAsClient();
			}
			this._selectedWeapons.Clear();
			foreach (SiegeWeapon siegeWeapon in this._availableWeapons)
			{
				this._selectedWeapons.Add(siegeWeapon);
			}
			Action onSelectedSiegeWeaponsChanged = this.OnSelectedSiegeWeaponsChanged;
			if (onSelectedSiegeWeaponsChanged == null)
			{
				return;
			}
			onSelectedSiegeWeaponsChanged();
		}

		// Token: 0x06001103 RID: 4355 RVA: 0x00038108 File Offset: 0x00036308
		public static bool IsWeaponSelectable(SiegeWeapon weapon)
		{
			return !weapon.IsDeactivated;
		}

		// Token: 0x06001104 RID: 4356 RVA: 0x00038114 File Offset: 0x00036314
		public static SiegeWeaponOrderType GetActiveOrderOf(SiegeWeapon weapon)
		{
			if (!weapon.ForcedUse)
			{
				return SiegeWeaponOrderType.Stop;
			}
			if (!(weapon is RangedSiegeWeapon))
			{
				return SiegeWeaponOrderType.Attack;
			}
			switch (((RangedSiegeWeapon)weapon).Focus)
			{
			case RangedSiegeWeapon.FiringFocus.Troops:
				return SiegeWeaponOrderType.FireAtTroops;
			case RangedSiegeWeapon.FiringFocus.Walls:
				return SiegeWeaponOrderType.FireAtWalls;
			case RangedSiegeWeapon.FiringFocus.RangedSiegeWeapons:
				return SiegeWeaponOrderType.FireAtRangedSiegeWeapons;
			case RangedSiegeWeapon.FiringFocus.PrimarySiegeWeapons:
				return SiegeWeaponOrderType.FireAtPrimarySiegeWeapons;
			default:
				Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\SiegeWeaponController.cs", "GetActiveOrderOf", 166);
				return SiegeWeaponOrderType.FireAtTroops;
			}
		}

		// Token: 0x06001105 RID: 4357 RVA: 0x0003817B File Offset: 0x0003637B
		public static SiegeWeaponOrderType GetActiveMovementOrderOf(SiegeWeapon weapon)
		{
			if (!weapon.ForcedUse)
			{
				return SiegeWeaponOrderType.Stop;
			}
			return SiegeWeaponOrderType.Attack;
		}

		// Token: 0x06001106 RID: 4358 RVA: 0x00038188 File Offset: 0x00036388
		public static SiegeWeaponOrderType GetActiveFacingOrderOf(SiegeWeapon weapon)
		{
			if (!(weapon is RangedSiegeWeapon))
			{
				return SiegeWeaponOrderType.FireAtWalls;
			}
			switch (((RangedSiegeWeapon)weapon).Focus)
			{
			case RangedSiegeWeapon.FiringFocus.Troops:
				return SiegeWeaponOrderType.FireAtTroops;
			case RangedSiegeWeapon.FiringFocus.Walls:
				return SiegeWeaponOrderType.FireAtWalls;
			case RangedSiegeWeapon.FiringFocus.RangedSiegeWeapons:
				return SiegeWeaponOrderType.FireAtRangedSiegeWeapons;
			case RangedSiegeWeapon.FiringFocus.PrimarySiegeWeapons:
				return SiegeWeaponOrderType.FireAtPrimarySiegeWeapons;
			default:
				Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\SiegeWeaponController.cs", "GetActiveFacingOrderOf", 204);
				return SiegeWeaponOrderType.FireAtTroops;
			}
		}

		// Token: 0x06001107 RID: 4359 RVA: 0x000381E5 File Offset: 0x000363E5
		public static SiegeWeaponOrderType GetActiveFiringOrderOf(SiegeWeapon weapon)
		{
			if (!weapon.ForcedUse)
			{
				return SiegeWeaponOrderType.Stop;
			}
			return SiegeWeaponOrderType.Attack;
		}

		// Token: 0x06001108 RID: 4360 RVA: 0x000381F2 File Offset: 0x000363F2
		public static SiegeWeaponOrderType GetActiveAIControlOrderOf(SiegeWeapon weapon)
		{
			if (weapon.ForcedUse)
			{
				return SiegeWeaponOrderType.AIControlOn;
			}
			return SiegeWeaponOrderType.AIControlOff;
		}

		// Token: 0x06001109 RID: 4361 RVA: 0x00038200 File Offset: 0x00036400
		private void SetOrderAux(SiegeWeaponOrderType order, SiegeWeapon weapon)
		{
			switch (order)
			{
			case SiegeWeaponOrderType.Stop:
			case SiegeWeaponOrderType.AIControlOff:
				weapon.SetForcedUse(false);
				return;
			case SiegeWeaponOrderType.Attack:
			case SiegeWeaponOrderType.AIControlOn:
				weapon.SetForcedUse(true);
				return;
			case SiegeWeaponOrderType.FireAtWalls:
			{
				weapon.SetForcedUse(true);
				RangedSiegeWeapon rangedSiegeWeapon = weapon as RangedSiegeWeapon;
				if (rangedSiegeWeapon != null)
				{
					rangedSiegeWeapon.Focus = RangedSiegeWeapon.FiringFocus.Walls;
					return;
				}
				break;
			}
			case SiegeWeaponOrderType.FireAtTroops:
			{
				weapon.SetForcedUse(true);
				RangedSiegeWeapon rangedSiegeWeapon2 = weapon as RangedSiegeWeapon;
				if (rangedSiegeWeapon2 != null)
				{
					rangedSiegeWeapon2.Focus = RangedSiegeWeapon.FiringFocus.Troops;
					return;
				}
				break;
			}
			case SiegeWeaponOrderType.FireAtRangedSiegeWeapons:
			{
				weapon.SetForcedUse(true);
				RangedSiegeWeapon rangedSiegeWeapon3 = weapon as RangedSiegeWeapon;
				if (rangedSiegeWeapon3 != null)
				{
					rangedSiegeWeapon3.Focus = RangedSiegeWeapon.FiringFocus.RangedSiegeWeapons;
					return;
				}
				break;
			}
			case SiegeWeaponOrderType.FireAtPrimarySiegeWeapons:
			{
				weapon.SetForcedUse(true);
				RangedSiegeWeapon rangedSiegeWeapon4 = weapon as RangedSiegeWeapon;
				if (rangedSiegeWeapon4 != null)
				{
					rangedSiegeWeapon4.Focus = RangedSiegeWeapon.FiringFocus.PrimarySiegeWeapons;
					return;
				}
				break;
			}
			default:
				Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\SiegeWeaponController.cs", "SetOrderAux", 294);
				break;
			}
		}

		// Token: 0x0600110A RID: 4362 RVA: 0x000382C4 File Offset: 0x000364C4
		public void SetOrder(SiegeWeaponOrderType order)
		{
			if (GameNetwork.IsClient)
			{
				GameNetwork.BeginModuleEventAsClient();
				GameNetwork.WriteMessage(new ApplySiegeWeaponOrder(order));
				GameNetwork.EndModuleEventAsClient();
			}
			foreach (SiegeWeapon siegeWeapon in this.SelectedWeapons)
			{
				this.SetOrderAux(order, siegeWeapon);
			}
			Action<SiegeWeaponOrderType, IEnumerable<SiegeWeapon>> onOrderIssued = this.OnOrderIssued;
			if (onOrderIssued == null)
			{
				return;
			}
			onOrderIssued(order, this.SelectedWeapons);
		}

		// Token: 0x0600110B RID: 4363 RVA: 0x0003834C File Offset: 0x0003654C
		public int GetShortcutIndexOf(SiegeWeapon weapon)
		{
			FormationAI.BehaviorSide sideOf = SiegeWeaponController.GetSideOf(weapon);
			int num = ((sideOf == FormationAI.BehaviorSide.Left) ? 1 : ((sideOf == FormationAI.BehaviorSide.Right) ? 2 : 0));
			if (!(weapon is IPrimarySiegeWeapon))
			{
				num += 3;
			}
			return num;
		}

		// Token: 0x0600110C RID: 4364 RVA: 0x0003837C File Offset: 0x0003657C
		private static FormationAI.BehaviorSide GetSideOf(SiegeWeapon weapon)
		{
			IPrimarySiegeWeapon primarySiegeWeapon = weapon as IPrimarySiegeWeapon;
			if (primarySiegeWeapon != null)
			{
				return primarySiegeWeapon.WeaponSide;
			}
			if (weapon is RangedSiegeWeapon)
			{
				return FormationAI.BehaviorSide.Middle;
			}
			Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\SiegeWeaponController.cs", "GetSideOf", 346);
			return FormationAI.BehaviorSide.Middle;
		}

		// Token: 0x04000458 RID: 1112
		private readonly Mission _mission;

		// Token: 0x04000459 RID: 1113
		private readonly Team _team;

		// Token: 0x0400045A RID: 1114
		private List<SiegeWeapon> _availableWeapons;

		// Token: 0x0400045B RID: 1115
		private MBList<SiegeWeapon> _selectedWeapons;
	}
}
