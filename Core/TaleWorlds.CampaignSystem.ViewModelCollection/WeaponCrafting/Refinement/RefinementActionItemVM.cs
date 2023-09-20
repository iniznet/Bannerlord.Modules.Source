using System;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.Refinement
{
	// Token: 0x020000F0 RID: 240
	public class RefinementActionItemVM : ViewModel
	{
		// Token: 0x1700079B RID: 1947
		// (get) Token: 0x0600167D RID: 5757 RVA: 0x00053B10 File Offset: 0x00051D10
		public Crafting.RefiningFormula RefineFormula { get; }

		// Token: 0x0600167E RID: 5758 RVA: 0x00053B18 File Offset: 0x00051D18
		public RefinementActionItemVM(Crafting.RefiningFormula refineFormula, Action<RefinementActionItemVM> onSelect)
		{
			this._onSelect = onSelect;
			this.RefineFormula = refineFormula;
			this.InputMaterials = new MBBindingList<CraftingResourceItemVM>();
			this.OutputMaterials = new MBBindingList<CraftingResourceItemVM>();
			SmithingModel smithingModel = Campaign.Current.Models.SmithingModel;
			if (this.RefineFormula.Input1Count > 0)
			{
				this.InputMaterials.Add(new CraftingResourceItemVM(this.RefineFormula.Input1, this.RefineFormula.Input1Count, 0));
			}
			if (this.RefineFormula.Input2Count > 0)
			{
				this.InputMaterials.Add(new CraftingResourceItemVM(this.RefineFormula.Input2, this.RefineFormula.Input2Count, 0));
			}
			if (this.RefineFormula.OutputCount > 0)
			{
				this.OutputMaterials.Add(new CraftingResourceItemVM(this.RefineFormula.Output, this.RefineFormula.OutputCount, 0));
			}
			if (this.RefineFormula.Output2Count > 0)
			{
				this.OutputMaterials.Add(new CraftingResourceItemVM(this.RefineFormula.Output2, this.RefineFormula.Output2Count, 0));
			}
			this.RefreshDynamicProperties();
			this.RefreshValues();
		}

		// Token: 0x0600167F RID: 5759 RVA: 0x00053C40 File Offset: 0x00051E40
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.InputMaterials.ApplyActionOnAllItems(delegate(CraftingResourceItemVM m)
			{
				m.RefreshValues();
			});
			this.OutputMaterials.ApplyActionOnAllItems(delegate(CraftingResourceItemVM m)
			{
				m.RefreshValues();
			});
		}

		// Token: 0x06001680 RID: 5760 RVA: 0x00053CA7 File Offset: 0x00051EA7
		public void RefreshDynamicProperties()
		{
			this.IsEnabled = this.CheckInputsAvailable();
		}

		// Token: 0x06001681 RID: 5761 RVA: 0x00053CB8 File Offset: 0x00051EB8
		private bool CheckInputsAvailable()
		{
			ItemRoster itemRoster = MobileParty.MainParty.ItemRoster;
			foreach (CraftingResourceItemVM craftingResourceItemVM in this.InputMaterials)
			{
				if (itemRoster.GetItemNumber(craftingResourceItemVM.ResourceItem) < craftingResourceItemVM.ResourceAmount)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06001682 RID: 5762 RVA: 0x00053D24 File Offset: 0x00051F24
		public void ExecuteSelectAction()
		{
			this._onSelect(this);
		}

		// Token: 0x1700079C RID: 1948
		// (get) Token: 0x06001683 RID: 5763 RVA: 0x00053D32 File Offset: 0x00051F32
		// (set) Token: 0x06001684 RID: 5764 RVA: 0x00053D3A File Offset: 0x00051F3A
		[DataSourceProperty]
		public MBBindingList<CraftingResourceItemVM> InputMaterials
		{
			get
			{
				return this._inputMaterials;
			}
			set
			{
				if (value != this._inputMaterials)
				{
					this._inputMaterials = value;
					base.OnPropertyChangedWithValue<MBBindingList<CraftingResourceItemVM>>(value, "InputMaterials");
				}
			}
		}

		// Token: 0x1700079D RID: 1949
		// (get) Token: 0x06001685 RID: 5765 RVA: 0x00053D58 File Offset: 0x00051F58
		// (set) Token: 0x06001686 RID: 5766 RVA: 0x00053D60 File Offset: 0x00051F60
		[DataSourceProperty]
		public MBBindingList<CraftingResourceItemVM> OutputMaterials
		{
			get
			{
				return this._outputMaterials;
			}
			set
			{
				if (value != this._outputMaterials)
				{
					this._outputMaterials = value;
					base.OnPropertyChangedWithValue<MBBindingList<CraftingResourceItemVM>>(value, "OutputMaterials");
				}
			}
		}

		// Token: 0x1700079E RID: 1950
		// (get) Token: 0x06001687 RID: 5767 RVA: 0x00053D7E File Offset: 0x00051F7E
		// (set) Token: 0x06001688 RID: 5768 RVA: 0x00053D86 File Offset: 0x00051F86
		[DataSourceProperty]
		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				if (value != this._isSelected)
				{
					this._isSelected = value;
					base.OnPropertyChangedWithValue(value, "IsSelected");
				}
			}
		}

		// Token: 0x1700079F RID: 1951
		// (get) Token: 0x06001689 RID: 5769 RVA: 0x00053DA4 File Offset: 0x00051FA4
		// (set) Token: 0x0600168A RID: 5770 RVA: 0x00053DAC File Offset: 0x00051FAC
		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (value != this._isEnabled)
				{
					this._isEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEnabled");
				}
			}
		}

		// Token: 0x04000A8A RID: 2698
		private readonly Action<RefinementActionItemVM> _onSelect;

		// Token: 0x04000A8C RID: 2700
		private MBBindingList<CraftingResourceItemVM> _inputMaterials;

		// Token: 0x04000A8D RID: 2701
		private MBBindingList<CraftingResourceItemVM> _outputMaterials;

		// Token: 0x04000A8E RID: 2702
		private bool _isSelected;

		// Token: 0x04000A8F RID: 2703
		private bool _isEnabled;
	}
}
