using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Order
{
	public class OrderTroopItemFormationClassVM : ViewModel
	{
		public OrderTroopItemFormationClassVM(Formation formation, FormationClass formationClass)
		{
			this._formation = formation;
			this.FormationClass = formationClass;
			this.FormationClassValue = (int)formationClass;
		}

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

		public readonly FormationClass FormationClass;

		private readonly Formation _formation;

		private int _formationClassValue;

		private int _troopCount;
	}
}
