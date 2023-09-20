using System;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.Refinement
{
	public class RefinementActionItemVM : ViewModel
	{
		public Crafting.RefiningFormula RefineFormula { get; }

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

		public void RefreshDynamicProperties()
		{
			this.IsEnabled = this.CheckInputsAvailable();
		}

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

		public void ExecuteSelectAction()
		{
			this._onSelect(this);
		}

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

		private readonly Action<RefinementActionItemVM> _onSelect;

		private MBBindingList<CraftingResourceItemVM> _inputMaterials;

		private MBBindingList<CraftingResourceItemVM> _outputMaterials;

		private bool _isSelected;

		private bool _isEnabled;
	}
}
