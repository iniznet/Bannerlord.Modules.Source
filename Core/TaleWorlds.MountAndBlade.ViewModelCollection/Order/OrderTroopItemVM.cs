using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Order
{
	public class OrderTroopItemVM : OrderSubjectVM
	{
		public bool ContainsDeadTroop { get; private set; }

		public OrderTroopItemVM(Formation formation, Action<OrderTroopItemVM> setSelected, Func<Formation, int> getMorale)
		{
			this.ActiveFormationClasses = new MBBindingList<OrderTroopItemFormationClassVM>();
			this.ActiveFilters = new MBBindingList<OrderTroopItemFilterVM>();
			this.InitialFormationClass = formation.FormationIndex;
			this.SetFormationClassFromFormation(formation);
			this.Formation = formation;
			this.SetSelected = setSelected;
			this.CurrentMemberCount = (formation.IsPlayerTroopInFormation ? (formation.CountOfUnits - 1) : formation.CountOfUnits);
			this.Morale = getMorale(formation);
			base.UnderAttackOfType = 0;
			base.BehaviorType = 0;
			if (Input.IsControllerConnected)
			{
				bool flag = !Input.IsMouseActive;
			}
			this.UpdateSelectionKeyInfo();
			this.UpdateCommanderInfo();
			this.Formation.OnUnitCountChanged += this.FormationOnOnUnitCountChanged;
		}

		public OrderTroopItemVM(OrderTroopItemVM troop, Action<OrderTroopItemVM> setSelected = null)
		{
			this.ActiveFormationClasses = new MBBindingList<OrderTroopItemFormationClassVM>();
			foreach (OrderTroopItemFormationClassVM orderTroopItemFormationClassVM in troop.ActiveFormationClasses)
			{
				this.ActiveFormationClasses.Add(new OrderTroopItemFormationClassVM(troop.Formation, orderTroopItemFormationClassVM.FormationClass));
			}
			this.ActiveFilters = new MBBindingList<OrderTroopItemFilterVM>();
			foreach (OrderTroopItemFilterVM orderTroopItemFilterVM in troop.ActiveFilters)
			{
				this.ActiveFilters.Add(new OrderTroopItemFilterVM(orderTroopItemFilterVM.FilterTypeValue));
			}
			this.InitialFormationClass = troop.InitialFormationClass;
			this.Formation = troop.Formation;
			this.SetSelected = setSelected ?? troop.SetSelected;
			this.CurrentMemberCount = (troop.Formation.IsPlayerTroopInFormation ? (troop.CurrentMemberCount - 1) : troop.CurrentMemberCount);
			this.Morale = troop.Morale;
			base.UnderAttackOfType = 0;
			base.BehaviorType = 0;
			this.UpdateCommanderInfo();
		}

		public override void OnFinalize()
		{
			this.Formation.OnUnitCountChanged -= this.FormationOnOnUnitCountChanged;
		}

		private void FormationOnOnUnitCountChanged(Formation formation)
		{
			this.CurrentMemberCount = (formation.IsPlayerTroopInFormation ? (formation.CountOfUnits - 1) : formation.CountOfUnits);
			this.UpdateCommanderInfo();
		}

		public void OnFormationAgentRemoved(Agent agent)
		{
			if (!agent.IsActive())
			{
				this.ContainsDeadTroop = true;
			}
			this.UpdateCommanderInfo();
		}

		private void UpdateCommanderInfo()
		{
			Formation formation = this.Formation;
			bool flag;
			if (formation == null)
			{
				flag = null != null;
			}
			else
			{
				Agent captain = formation.Captain;
				flag = ((captain != null) ? captain.Character : null) != null;
			}
			if (flag && (this.Formation.Captain.Character != this._cachedCommander || this.CommanderImageIdentifier == null))
			{
				this.CommanderImageIdentifier = new ImageIdentifierVM(CharacterCode.CreateFrom(this.Formation.Captain.Character));
				this._cachedCommander = this.Formation.Captain.Character;
				return;
			}
			this.CommanderImageIdentifier = null;
		}

		private void UpdateSelectionKeyInfo()
		{
			if (this.Formation == null)
			{
				return;
			}
			int num = -1;
			if (this.Formation.Index == 0)
			{
				num = 78;
			}
			else if (this.Formation.Index == 1)
			{
				num = 79;
			}
			else if (this.Formation.Index == 2)
			{
				num = 80;
			}
			else if (this.Formation.Index == 3)
			{
				num = 81;
			}
			else if (this.Formation.Index == 4)
			{
				num = 82;
			}
			else if (this.Formation.Index == 5)
			{
				num = 83;
			}
			else if (this.Formation.Index == 6)
			{
				num = 84;
			}
			else if (this.Formation.Index == 7)
			{
				num = 85;
			}
			if (num == -1)
			{
				return;
			}
			GameKey gameKey = HotKeyManager.GetCategory("MissionOrderHotkeyCategory").GetGameKey(num);
			base.SelectionKey = InputKeyItemVM.CreateFromGameKey(gameKey, false);
		}

		public bool SetFormationClassFromFormation(Formation formation)
		{
			bool flag = formation.QuerySystem.InfantryUnitRatio > 0f;
			bool flag2 = formation.QuerySystem.RangedUnitRatio > 0f;
			bool flag3 = formation.QuerySystem.CavalryUnitRatio > 0f;
			bool flag4 = formation.QuerySystem.RangedCavalryUnitRatio > 0f;
			if (flag && this._cachedInfantryItem == null)
			{
				this._cachedInfantryItem = new OrderTroopItemFormationClassVM(formation, FormationClass.Infantry);
				this.ActiveFormationClasses.Add(this._cachedInfantryItem);
			}
			else if (!flag)
			{
				this.ActiveFormationClasses.Remove(this._cachedInfantryItem);
				this._cachedInfantryItem = null;
			}
			if (flag2 && this._cachedRangedItem == null)
			{
				this._cachedRangedItem = new OrderTroopItemFormationClassVM(formation, FormationClass.Ranged);
				this.ActiveFormationClasses.Add(this._cachedRangedItem);
			}
			else if (!flag2)
			{
				this.ActiveFormationClasses.Remove(this._cachedRangedItem);
				this._cachedRangedItem = null;
			}
			if (flag3 && this._cachedCavalryItem == null)
			{
				this._cachedCavalryItem = new OrderTroopItemFormationClassVM(formation, FormationClass.Cavalry);
				this.ActiveFormationClasses.Add(this._cachedCavalryItem);
			}
			else if (!flag3)
			{
				this.ActiveFormationClasses.Remove(this._cachedCavalryItem);
				this._cachedCavalryItem = null;
			}
			if (flag4 && this._cachedHorseArcherItem == null)
			{
				this._cachedHorseArcherItem = new OrderTroopItemFormationClassVM(formation, FormationClass.HorseArcher);
				this.ActiveFormationClasses.Add(this._cachedHorseArcherItem);
			}
			else if (!flag4)
			{
				this.ActiveFormationClasses.Remove(this._cachedHorseArcherItem);
				this._cachedHorseArcherItem = null;
			}
			foreach (OrderTroopItemFormationClassVM orderTroopItemFormationClassVM in this.ActiveFormationClasses)
			{
				orderTroopItemFormationClassVM.UpdateTroopCount();
			}
			this.UpdateCommanderInfo();
			return false;
		}

		public void UpdateFilterData(List<int> usedFilters)
		{
			this.ActiveFilters.Clear();
			foreach (int num in usedFilters)
			{
				this.ActiveFilters.Add(new OrderTroopItemFilterVM(num));
			}
		}

		public void ExecuteAction()
		{
			this.SetSelected(this);
		}

		[DataSourceProperty]
		public int CurrentMemberCount
		{
			get
			{
				return this._currentMemberCount;
			}
			set
			{
				if (value != this._currentMemberCount)
				{
					this._currentMemberCount = value;
					base.OnPropertyChangedWithValue(value, "CurrentMemberCount");
					this.HaveTroops = value > 0;
				}
			}
		}

		[DataSourceProperty]
		public int Morale
		{
			get
			{
				return this._morale;
			}
			set
			{
				if (value != this._morale)
				{
					this._morale = value;
					base.OnPropertyChangedWithValue(value, "Morale");
				}
			}
		}

		[DataSourceProperty]
		public float AmmoPercentage
		{
			get
			{
				return this._ammoPercentage;
			}
			set
			{
				if (value != this._ammoPercentage)
				{
					this._ammoPercentage = value;
					base.OnPropertyChangedWithValue(value, "AmmoPercentage");
				}
			}
		}

		[DataSourceProperty]
		public bool IsAmmoAvailable
		{
			get
			{
				return this._isAmmoAvailable;
			}
			set
			{
				if (value != this._isAmmoAvailable)
				{
					this._isAmmoAvailable = value;
					base.OnPropertyChangedWithValue(value, "IsAmmoAvailable");
				}
			}
		}

		[DataSourceProperty]
		public bool HaveTroops
		{
			get
			{
				return this._haveTroops;
			}
			set
			{
				if (value != this._haveTroops)
				{
					this._haveTroops = value;
					base.OnPropertyChangedWithValue(value, "HaveTroops");
				}
			}
		}

		[DataSourceProperty]
		public ImageIdentifierVM CommanderImageIdentifier
		{
			get
			{
				return this._commanderImageIdentifier;
			}
			set
			{
				if (value != this._commanderImageIdentifier)
				{
					this._commanderImageIdentifier = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "CommanderImageIdentifier");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<OrderTroopItemFormationClassVM> ActiveFormationClasses
		{
			get
			{
				return this._activeFormationClasses;
			}
			set
			{
				if (value != this._activeFormationClasses)
				{
					this._activeFormationClasses = value;
					base.OnPropertyChangedWithValue<MBBindingList<OrderTroopItemFormationClassVM>>(value, "ActiveFormationClasses");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<OrderTroopItemFilterVM> ActiveFilters
		{
			get
			{
				return this._activeFilters;
			}
			set
			{
				if (value != this._activeFilters)
				{
					this._activeFilters = value;
					base.OnPropertyChangedWithValue<MBBindingList<OrderTroopItemFilterVM>>(value, "ActiveFilters");
				}
			}
		}

		public FormationClass InitialFormationClass;

		public Formation Formation;

		public Type MachineType;

		public Action<OrderTroopItemVM> SetSelected;

		private OrderTroopItemFormationClassVM _cachedInfantryItem;

		private OrderTroopItemFormationClassVM _cachedRangedItem;

		private OrderTroopItemFormationClassVM _cachedCavalryItem;

		private OrderTroopItemFormationClassVM _cachedHorseArcherItem;

		private BasicCharacterObject _cachedCommander;

		private int _currentMemberCount;

		private int _morale;

		private float _ammoPercentage;

		private bool _isAmmoAvailable;

		private bool _haveTroops;

		private ImageIdentifierVM _commanderImageIdentifier;

		private MBBindingList<OrderTroopItemFormationClassVM> _activeFormationClasses;

		private MBBindingList<OrderTroopItemFilterVM> _activeFilters;
	}
}
