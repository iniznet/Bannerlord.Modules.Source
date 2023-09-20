using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Order
{
	// Token: 0x02000029 RID: 41
	public class OrderTroopItemVM : OrderSubjectVM
	{
		// Token: 0x170000E2 RID: 226
		// (get) Token: 0x060002E7 RID: 743 RVA: 0x0000D676 File Offset: 0x0000B876
		// (set) Token: 0x060002E8 RID: 744 RVA: 0x0000D67E File Offset: 0x0000B87E
		public bool ContainsDeadTroop { get; private set; }

		// Token: 0x060002E9 RID: 745 RVA: 0x0000D688 File Offset: 0x0000B888
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

		// Token: 0x060002EA RID: 746 RVA: 0x0000D744 File Offset: 0x0000B944
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

		// Token: 0x060002EB RID: 747 RVA: 0x0000D87C File Offset: 0x0000BA7C
		public override void OnFinalize()
		{
			this.Formation.OnUnitCountChanged -= this.FormationOnOnUnitCountChanged;
		}

		// Token: 0x060002EC RID: 748 RVA: 0x0000D895 File Offset: 0x0000BA95
		private void FormationOnOnUnitCountChanged(Formation formation)
		{
			this.CurrentMemberCount = (formation.IsPlayerTroopInFormation ? (formation.CountOfUnits - 1) : formation.CountOfUnits);
			this.UpdateCommanderInfo();
		}

		// Token: 0x060002ED RID: 749 RVA: 0x0000D8BB File Offset: 0x0000BABB
		public void OnFormationAgentRemoved(Agent agent)
		{
			if (!agent.IsActive())
			{
				this.ContainsDeadTroop = true;
			}
			this.UpdateCommanderInfo();
		}

		// Token: 0x060002EE RID: 750 RVA: 0x0000D8D4 File Offset: 0x0000BAD4
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

		// Token: 0x060002EF RID: 751 RVA: 0x0000D960 File Offset: 0x0000BB60
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

		// Token: 0x060002F0 RID: 752 RVA: 0x0000DA34 File Offset: 0x0000BC34
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

		// Token: 0x060002F1 RID: 753 RVA: 0x0000DBF4 File Offset: 0x0000BDF4
		public void UpdateFilterData(List<int> usedFilters)
		{
			this.ActiveFilters.Clear();
			foreach (int num in usedFilters)
			{
				this.ActiveFilters.Add(new OrderTroopItemFilterVM(num));
			}
		}

		// Token: 0x060002F2 RID: 754 RVA: 0x0000DC58 File Offset: 0x0000BE58
		public void ExecuteAction()
		{
			this.SetSelected(this);
		}

		// Token: 0x170000E3 RID: 227
		// (get) Token: 0x060002F3 RID: 755 RVA: 0x0000DC66 File Offset: 0x0000BE66
		// (set) Token: 0x060002F4 RID: 756 RVA: 0x0000DC6E File Offset: 0x0000BE6E
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

		// Token: 0x170000E4 RID: 228
		// (get) Token: 0x060002F5 RID: 757 RVA: 0x0000DC96 File Offset: 0x0000BE96
		// (set) Token: 0x060002F6 RID: 758 RVA: 0x0000DC9E File Offset: 0x0000BE9E
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

		// Token: 0x170000E5 RID: 229
		// (get) Token: 0x060002F7 RID: 759 RVA: 0x0000DCBC File Offset: 0x0000BEBC
		// (set) Token: 0x060002F8 RID: 760 RVA: 0x0000DCC4 File Offset: 0x0000BEC4
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

		// Token: 0x170000E6 RID: 230
		// (get) Token: 0x060002F9 RID: 761 RVA: 0x0000DCE2 File Offset: 0x0000BEE2
		// (set) Token: 0x060002FA RID: 762 RVA: 0x0000DCEA File Offset: 0x0000BEEA
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

		// Token: 0x170000E7 RID: 231
		// (get) Token: 0x060002FB RID: 763 RVA: 0x0000DD08 File Offset: 0x0000BF08
		// (set) Token: 0x060002FC RID: 764 RVA: 0x0000DD10 File Offset: 0x0000BF10
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

		// Token: 0x170000E8 RID: 232
		// (get) Token: 0x060002FD RID: 765 RVA: 0x0000DD2E File Offset: 0x0000BF2E
		// (set) Token: 0x060002FE RID: 766 RVA: 0x0000DD36 File Offset: 0x0000BF36
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

		// Token: 0x170000E9 RID: 233
		// (get) Token: 0x060002FF RID: 767 RVA: 0x0000DD54 File Offset: 0x0000BF54
		// (set) Token: 0x06000300 RID: 768 RVA: 0x0000DD5C File Offset: 0x0000BF5C
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

		// Token: 0x170000EA RID: 234
		// (get) Token: 0x06000301 RID: 769 RVA: 0x0000DD7A File Offset: 0x0000BF7A
		// (set) Token: 0x06000302 RID: 770 RVA: 0x0000DD82 File Offset: 0x0000BF82
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

		// Token: 0x0400016B RID: 363
		public FormationClass InitialFormationClass;

		// Token: 0x0400016C RID: 364
		public Formation Formation;

		// Token: 0x0400016D RID: 365
		public Type MachineType;

		// Token: 0x0400016E RID: 366
		public Action<OrderTroopItemVM> SetSelected;

		// Token: 0x04000170 RID: 368
		private OrderTroopItemFormationClassVM _cachedInfantryItem;

		// Token: 0x04000171 RID: 369
		private OrderTroopItemFormationClassVM _cachedRangedItem;

		// Token: 0x04000172 RID: 370
		private OrderTroopItemFormationClassVM _cachedCavalryItem;

		// Token: 0x04000173 RID: 371
		private OrderTroopItemFormationClassVM _cachedHorseArcherItem;

		// Token: 0x04000174 RID: 372
		private BasicCharacterObject _cachedCommander;

		// Token: 0x04000175 RID: 373
		private int _currentMemberCount;

		// Token: 0x04000176 RID: 374
		private int _morale;

		// Token: 0x04000177 RID: 375
		private float _ammoPercentage;

		// Token: 0x04000178 RID: 376
		private bool _isAmmoAvailable;

		// Token: 0x04000179 RID: 377
		private bool _haveTroops;

		// Token: 0x0400017A RID: 378
		private ImageIdentifierVM _commanderImageIdentifier;

		// Token: 0x0400017B RID: 379
		private MBBindingList<OrderTroopItemFormationClassVM> _activeFormationClasses;

		// Token: 0x0400017C RID: 380
		private MBBindingList<OrderTroopItemFilterVM> _activeFilters;
	}
}
