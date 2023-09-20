using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Order
{
	// Token: 0x02000028 RID: 40
	public class OrderTroopItemFormationClassVM : ViewModel
	{
		// Token: 0x060002E1 RID: 737 RVA: 0x0000D54C File Offset: 0x0000B74C
		public OrderTroopItemFormationClassVM(Formation formation, FormationClass formationClass)
		{
			this._formation = formation;
			this.FormationClass = formationClass;
			this.FormationClassValue = (int)formationClass;
		}

		// Token: 0x060002E2 RID: 738 RVA: 0x0000D56C File Offset: 0x0000B76C
		public void UpdateTroopCount()
		{
			switch (this.FormationClass)
			{
			case FormationClass.Infantry:
				this.TroopCount = (int)(this._formation.QuerySystem.InfantryUnitRatio * (float)this._formation.CountOfUnits);
				return;
			case FormationClass.Ranged:
				this.TroopCount = (int)(this._formation.QuerySystem.RangedUnitRatio * (float)this._formation.CountOfUnits);
				return;
			case FormationClass.Cavalry:
				this.TroopCount = (int)(this._formation.QuerySystem.CavalryUnitRatio * (float)this._formation.CountOfUnits);
				return;
			case FormationClass.HorseArcher:
				this.TroopCount = (int)(this._formation.QuerySystem.RangedCavalryUnitRatio * (float)this._formation.CountOfUnits);
				return;
			default:
				return;
			}
		}

		// Token: 0x170000E0 RID: 224
		// (get) Token: 0x060002E3 RID: 739 RVA: 0x0000D62A File Offset: 0x0000B82A
		// (set) Token: 0x060002E4 RID: 740 RVA: 0x0000D632 File Offset: 0x0000B832
		[DataSourceProperty]
		public int FormationClassValue
		{
			get
			{
				return this._formationClassValue;
			}
			set
			{
				if (value != this._formationClassValue)
				{
					this._formationClassValue = value;
					base.OnPropertyChangedWithValue(value, "FormationClassValue");
				}
			}
		}

		// Token: 0x170000E1 RID: 225
		// (get) Token: 0x060002E5 RID: 741 RVA: 0x0000D650 File Offset: 0x0000B850
		// (set) Token: 0x060002E6 RID: 742 RVA: 0x0000D658 File Offset: 0x0000B858
		[DataSourceProperty]
		public int TroopCount
		{
			get
			{
				return this._troopCount;
			}
			set
			{
				if (value != this._troopCount)
				{
					this._troopCount = value;
					base.OnPropertyChangedWithValue(value, "TroopCount");
				}
			}
		}

		// Token: 0x04000167 RID: 359
		public readonly FormationClass FormationClass;

		// Token: 0x04000168 RID: 360
		private readonly Formation _formation;

		// Token: 0x04000169 RID: 361
		private int _formationClassValue;

		// Token: 0x0400016A RID: 362
		private int _troopCount;
	}
}
