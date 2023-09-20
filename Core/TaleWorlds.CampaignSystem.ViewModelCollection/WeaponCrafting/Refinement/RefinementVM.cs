using System;
using System.Linq;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.Refinement
{
	// Token: 0x020000F1 RID: 241
	public class RefinementVM : ViewModel
	{
		// Token: 0x0600168B RID: 5771 RVA: 0x00053DCC File Offset: 0x00051FCC
		public RefinementVM(Action onRefinementSelectionChange, Func<CraftingAvailableHeroItemVM> getCurrentHero)
		{
			this._onRefinementSelectionChange = onRefinementSelectionChange;
			this._craftingBehavior = Campaign.Current.GetCampaignBehavior<ICraftingCampaignBehavior>();
			this._getCurrentHero = getCurrentHero;
			this.AvailableRefinementActions = new MBBindingList<RefinementActionItemVM>();
			this.SetupRefinementActionsList(this._getCurrentHero().Hero);
		}

		// Token: 0x0600168C RID: 5772 RVA: 0x00053E1E File Offset: 0x0005201E
		private void SetupRefinementActionsList(Hero craftingHero)
		{
			this.UpdateRefinementFormulas(craftingHero);
			this.RefreshRefinementActionsList(craftingHero);
		}

		// Token: 0x0600168D RID: 5773 RVA: 0x00053E2E File Offset: 0x0005202E
		internal void OnCraftingHeroChanged(CraftingAvailableHeroItemVM newHero)
		{
			this.SetupRefinementActionsList(this._getCurrentHero().Hero);
			this.SelectDefaultAction();
		}

		// Token: 0x0600168E RID: 5774 RVA: 0x00053E4C File Offset: 0x0005204C
		private void UpdateRefinementFormulas(Hero hero)
		{
			this.AvailableRefinementActions.Clear();
			foreach (Crafting.RefiningFormula refiningFormula in Campaign.Current.Models.SmithingModel.GetRefiningFormulas(hero))
			{
				this.AvailableRefinementActions.Add(new RefinementActionItemVM(refiningFormula, new Action<RefinementActionItemVM>(this.OnSelectAction)));
			}
		}

		// Token: 0x0600168F RID: 5775 RVA: 0x00053ECC File Offset: 0x000520CC
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

		// Token: 0x06001690 RID: 5776 RVA: 0x00053F30 File Offset: 0x00052130
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

		// Token: 0x06001691 RID: 5777 RVA: 0x00053F80 File Offset: 0x00052180
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

		// Token: 0x06001692 RID: 5778 RVA: 0x00053FD8 File Offset: 0x000521D8
		private void SelectDefaultAction()
		{
			RefinementActionItemVM refinementActionItemVM = this.AvailableRefinementActions.FirstOrDefault((RefinementActionItemVM a) => a.IsEnabled);
			if (refinementActionItemVM != null)
			{
				this.OnSelectAction(refinementActionItemVM);
			}
		}

		// Token: 0x06001693 RID: 5779 RVA: 0x0005401A File Offset: 0x0005221A
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

		// Token: 0x170007A0 RID: 1952
		// (get) Token: 0x06001694 RID: 5780 RVA: 0x00054056 File Offset: 0x00052256
		// (set) Token: 0x06001695 RID: 5781 RVA: 0x0005405E File Offset: 0x0005225E
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

		// Token: 0x170007A1 RID: 1953
		// (get) Token: 0x06001696 RID: 5782 RVA: 0x00054086 File Offset: 0x00052286
		// (set) Token: 0x06001697 RID: 5783 RVA: 0x0005408E File Offset: 0x0005228E
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

		// Token: 0x170007A2 RID: 1954
		// (get) Token: 0x06001698 RID: 5784 RVA: 0x000540AC File Offset: 0x000522AC
		// (set) Token: 0x06001699 RID: 5785 RVA: 0x000540B4 File Offset: 0x000522B4
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

		// Token: 0x170007A3 RID: 1955
		// (get) Token: 0x0600169A RID: 5786 RVA: 0x000540D2 File Offset: 0x000522D2
		// (set) Token: 0x0600169B RID: 5787 RVA: 0x000540DA File Offset: 0x000522DA
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

		// Token: 0x04000A90 RID: 2704
		private readonly Action _onRefinementSelectionChange;

		// Token: 0x04000A91 RID: 2705
		private readonly ICraftingCampaignBehavior _craftingBehavior;

		// Token: 0x04000A92 RID: 2706
		private readonly Func<CraftingAvailableHeroItemVM> _getCurrentHero;

		// Token: 0x04000A93 RID: 2707
		private RefinementActionItemVM _currentSelectedAction;

		// Token: 0x04000A94 RID: 2708
		private bool _isValidRefinementActionSelected;

		// Token: 0x04000A95 RID: 2709
		private MBBindingList<RefinementActionItemVM> _availableRefinementActions;

		// Token: 0x04000A96 RID: 2710
		private string _refinementText;
	}
}
