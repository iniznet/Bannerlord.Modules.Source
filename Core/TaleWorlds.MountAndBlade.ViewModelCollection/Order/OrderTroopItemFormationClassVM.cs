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
				this.TroopCount = this._formation.GetCountOfUnitsBelongingToLogicalClass(FormationClass.Infantry);
				return;
			case FormationClass.Ranged:
				this.TroopCount = this._formation.GetCountOfUnitsBelongingToLogicalClass(FormationClass.Ranged);
				return;
			case FormationClass.Cavalry:
				this.TroopCount = this._formation.GetCountOfUnitsBelongingToLogicalClass(FormationClass.Cavalry);
				return;
			case FormationClass.HorseArcher:
				this.TroopCount = this._formation.GetCountOfUnitsBelongingToLogicalClass(FormationClass.HorseArcher);
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
