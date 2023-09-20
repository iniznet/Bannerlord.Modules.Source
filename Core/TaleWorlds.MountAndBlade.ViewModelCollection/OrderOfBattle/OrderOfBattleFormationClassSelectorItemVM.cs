using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.OrderOfBattle
{
	// Token: 0x0200002A RID: 42
	public class OrderOfBattleFormationClassSelectorItemVM : SelectorItemVM
	{
		// Token: 0x06000303 RID: 771 RVA: 0x0000DDA0 File Offset: 0x0000BFA0
		public OrderOfBattleFormationClassSelectorItemVM(DeploymentFormationClass formationClass)
			: base(formationClass.ToString())
		{
			this.FormationClass = formationClass;
			this.FormationClassInt = (int)formationClass;
			this.RefreshValues();
		}

		// Token: 0x06000304 RID: 772 RVA: 0x0000DDC9 File Offset: 0x0000BFC9
		public override void RefreshValues()
		{
			base.Hint = new HintViewModel(this.FormationClass.GetClassName(), null);
		}

		// Token: 0x170000EB RID: 235
		// (get) Token: 0x06000305 RID: 773 RVA: 0x0000DDE2 File Offset: 0x0000BFE2
		// (set) Token: 0x06000306 RID: 774 RVA: 0x0000DDEA File Offset: 0x0000BFEA
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

		// Token: 0x0400017D RID: 381
		public readonly DeploymentFormationClass FormationClass;

		// Token: 0x0400017E RID: 382
		private int _formationClassInt;
	}
}
