using System;
using System.Linq;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.Refinement
{
	public class RefinementVM : ViewModel
	{
		public RefinementVM(Action onRefinementSelectionChange, Func<CraftingAvailableHeroItemVM> getCurrentHero)
		{
			this._onRefinementSelectionChange = onRefinementSelectionChange;
			this._craftingBehavior = Campaign.Current.GetCampaignBehavior<ICraftingCampaignBehavior>();
			this._getCurrentHero = getCurrentHero;
			this.AvailableRefinementActions = new MBBindingList<RefinementActionItemVM>();
			this.SetupRefinementActionsList(this._getCurrentHero().Hero);
		}

		private void SetupRefinementActionsList(Hero craftingHero)
		{
			this.UpdateRefinementFormulas(craftingHero);
			this.RefreshRefinementActionsList(craftingHero);
		}

		internal void OnCraftingHeroChanged(CraftingAvailableHeroItemVM newHero)
		{
			this.SetupRefinementActionsList(this._getCurrentHero().Hero);
			this.SelectDefaultAction();
		}

		private void UpdateRefinementFormulas(Hero hero)
		{
			this.AvailableRefinementActions.Clear();
			foreach (Crafting.RefiningFormula refiningFormula in Campaign.Current.Models.SmithingModel.GetRefiningFormulas(hero))
			{
				this.AvailableRefinementActions.Add(new RefinementActionItemVM(refiningFormula, new Action<RefinementActionItemVM>(this.OnSelectAction)));
			}
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.RefinementText = new TextObject("{=p7raHA9x}Refinement", null).ToString();
			this.AvailableRefinementActions.ApplyActionOnAllItems(delegate(RefinementActionItemVM x)
			{
				x.RefreshValues();
			});
			RefinementActionItemVM currentSelectedAction = this.CurrentSelectedAction;
			if (currentSelectedAction == null)
			{
				return;
			}
			currentSelectedAction.RefreshValues();
		}

		public void ExecuteSelectedRefinement(Hero currentCraftingHero)
		{
			if (this.CurrentSelectedAction != null)
			{
				ICraftingCampaignBehavior craftingBehavior = this._craftingBehavior;
				if (craftingBehavior != null)
				{
					craftingBehavior.DoRefinement(currentCraftingHero, this.CurrentSelectedAction.RefineFormula);
				}
				this.RefreshRefinementActionsList(currentCraftingHero);
				if (!this.CurrentSelectedAction.IsEnabled)
				{
					this.OnSelectAction(null);
				}
			}
		}

		public void RefreshRefinementActionsList(Hero craftingHero)
		{
			foreach (RefinementActionItemVM refinementActionItemVM in this.AvailableRefinementActions)
			{
				refinementActionItemVM.RefreshDynamicProperties();
			}
			if (this.CurrentSelectedAction == null)
			{
				this.SelectDefaultAction();
			}
		}

		private void SelectDefaultAction()
		{
			RefinementActionItemVM refinementActionItemVM = this.AvailableRefinementActions.FirstOrDefault((RefinementActionItemVM a) => a.IsEnabled);
			if (refinementActionItemVM != null)
			{
				this.OnSelectAction(refinementActionItemVM);
			}
		}

		private void OnSelectAction(RefinementActionItemVM selectedAction)
		{
			if (this.CurrentSelectedAction != null)
			{
				this.CurrentSelectedAction.IsSelected = false;
			}
			this.CurrentSelectedAction = selectedAction;
			this._onRefinementSelectionChange();
			if (this.CurrentSelectedAction != null)
			{
				this.CurrentSelectedAction.IsSelected = true;
			}
		}

		[DataSourceProperty]
		public RefinementActionItemVM CurrentSelectedAction
		{
			get
			{
				return this._currentSelectedAction;
			}
			set
			{
				if (value != this._currentSelectedAction)
				{
					this._currentSelectedAction = value;
					base.OnPropertyChangedWithValue<RefinementActionItemVM>(value, "CurrentSelectedAction");
					this.IsValidRefinementActionSelected = value != null;
				}
			}
		}

		[DataSourceProperty]
		public bool IsValidRefinementActionSelected
		{
			get
			{
				return this._isValidRefinementActionSelected;
			}
			set
			{
				if (value != this._isValidRefinementActionSelected)
				{
					this._isValidRefinementActionSelected = value;
					base.OnPropertyChangedWithValue(value, "IsValidRefinementActionSelected");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<RefinementActionItemVM> AvailableRefinementActions
		{
			get
			{
				return this._availableRefinementActions;
			}
			set
			{
				if (value != this._availableRefinementActions)
				{
					this._availableRefinementActions = value;
					base.OnPropertyChangedWithValue<MBBindingList<RefinementActionItemVM>>(value, "AvailableRefinementActions");
				}
			}
		}

		[DataSourceProperty]
		public string RefinementText
		{
			get
			{
				return this._refinementText;
			}
			set
			{
				if (value != this._refinementText)
				{
					this._refinementText = value;
					base.OnPropertyChangedWithValue<string>(value, "RefinementText");
				}
			}
		}

		private readonly Action _onRefinementSelectionChange;

		private readonly ICraftingCampaignBehavior _craftingBehavior;

		private readonly Func<CraftingAvailableHeroItemVM> _getCurrentHero;

		private RefinementActionItemVM _currentSelectedAction;

		private bool _isValidRefinementActionSelected;

		private MBBindingList<RefinementActionItemVM> _availableRefinementActions;

		private string _refinementText;
	}
}
