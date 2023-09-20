using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.OrderOfBattle
{
	public class OrderOfBattleFormationClassSelectorItemVM : SelectorItemVM
	{
		public OrderOfBattleFormationClassSelectorItemVM(DeploymentFormationClass formationClass)
			: base(formationClass.ToString())
		{
			this.FormationClass = formationClass;
			this.FormationClassInt = (int)formationClass;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.Hint = new HintViewModel(this.FormationClass.GetClassName(), null);
		}

		[DataSourceProperty]
		public int FormationClassInt
		{
			get
			{
				return this._formationClassInt;
			}
			set
			{
				if (value != this._formationClassInt)
				{
					this._formationClassInt = value;
					base.OnPropertyChangedWithValue(value, "FormationClassInt");
				}
			}
		}

		public readonly DeploymentFormationClass FormationClass;

		private int _formationClassInt;
	}
}
