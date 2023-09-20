using System;
using System.Collections.Generic;
using System.Linq;
using NetworkMessages.FromClient;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class SiegeWeaponController
	{
		public MBReadOnlyList<SiegeWeapon> SelectedWeapons
		{
			get
			{
				return this._selectedWeapons;
			}
		}

		public event Action<SiegeWeaponOrderType, IEnumerable<SiegeWeapon>> OnOrderIssued;

		public event Action OnSelectedSiegeWeaponsChanged;

		public SiegeWeaponController(Mission mission, Team team)
		{
			this._mission = mission;
			this._team = team;
			this._selectedWeapons = new MBList<SiegeWeapon>();
			this.InitializeWeaponsForDeployment();
		}

		private void InitializeWeaponsForDeployment()
		{
			IEnumerable<SiegeWeapon> enumerable = from w in (from dp in this._mission.ActiveMissionObjects.FindAllWithType<DeploymentPoint>()
					where dp.Side == this._team.Side
					select dp).SelectMany((DeploymentPoint dp) => dp.DeployableWeapons)
				select w as SiegeWeapon;
			this._availableWeapons = enumerable.ToList<SiegeWeapon>();
		}

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

		public static bool IsWeaponSelectable(SiegeWeapon weapon)
		{
			return !weapon.IsDeactivated;
		}

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

		public static SiegeWeaponOrderType GetActiveMovementOrderOf(SiegeWeapon weapon)
		{
			if (!weapon.ForcedUse)
			{
				return SiegeWeaponOrderType.Stop;
			}
			return SiegeWeaponOrderType.Attack;
		}

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

		public static SiegeWeaponOrderType GetActiveFiringOrderOf(SiegeWeapon weapon)
		{
			if (!weapon.ForcedUse)
			{
				return SiegeWeaponOrderType.Stop;
			}
			return SiegeWeaponOrderType.Attack;
		}

		public static SiegeWeaponOrderType GetActiveAIControlOrderOf(SiegeWeapon weapon)
		{
			if (weapon.ForcedUse)
			{
				return SiegeWeaponOrderType.AIControlOn;
			}
			return SiegeWeaponOrderType.AIControlOff;
		}

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

		private readonly Mission _mission;

		private readonly Team _team;

		private List<SiegeWeapon> _availableWeapons;

		private MBList<SiegeWeapon> _selectedWeapons;
	}
}
