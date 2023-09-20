using System;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.CraftingSystem;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting
{
	public class CraftingAvailableHeroItemVM : ViewModel
	{
		public Hero Hero { get; }

		public CraftingAvailableHeroItemVM(Hero hero, Action<CraftingAvailableHeroItemVM> onSelection)
		{
			this._onSelection = onSelection;
			this._craftingBehavior = Campaign.Current.GetCampaignBehavior<ICraftingCampaignBehavior>();
			this.Hero = hero;
			this.HeroData = new HeroVM(this.Hero, false);
			this.Hint = new BasicTooltipViewModel(() => CampaignUIHelper.GetCraftingHeroTooltip(this.Hero, this._craftingOrder));
			this.CraftingPerks = new MBBindingList<CraftingPerkVM>();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.HeroData.RefreshValues();
		}

		public void RefreshStamina()
		{
			this.CurrentStamina = (float)this._craftingBehavior.GetHeroCraftingStamina(this.Hero);
			this.MaxStamina = this._craftingBehavior.GetMaxHeroCraftingStamina(this.Hero);
			int num = (int)(this.CurrentStamina / (float)this.MaxStamina * 100f);
			GameTexts.SetVariable("NUMBER", num);
			this.StaminaPercentage = GameTexts.FindText("str_NUMBER_percent", null).ToString();
		}

		public void RefreshOrderAvailability(CraftingOrder order)
		{
			this._craftingOrder = order;
			if (order != null)
			{
				this.IsDisabled = !order.IsOrderAvailableForHero(this.Hero);
				return;
			}
			this.IsDisabled = false;
		}

		public void RefreshSkills()
		{
			this.SmithySkillLevel = this.Hero.GetSkillValue(DefaultSkills.Crafting);
		}

		public void RefreshPerks()
		{
			this.CraftingPerks.Clear();
			foreach (PerkObject perkObject in PerkObject.All)
			{
				if (perkObject.Skill == DefaultSkills.Crafting && this.Hero.GetPerkValue(perkObject))
				{
					this.CraftingPerks.Add(new CraftingPerkVM(perkObject));
				}
			}
			this.PerksText = ((this.CraftingPerks.Count > 0) ? new TextObject("{=8lCWWK9G}Smithing Perks", null).ToString() : new TextObject("{=WHRq5Dp0}No Smithing Perks", null).ToString());
		}

		public void ExecuteSelection()
		{
			Action<CraftingAvailableHeroItemVM> onSelection = this._onSelection;
			if (onSelection == null)
			{
				return;
			}
			onSelection(this);
		}

		[DataSourceProperty]
		public bool IsDisabled
		{
			get
			{
				return this._isDisabled;
			}
			set
			{
				if (value != this._isDisabled)
				{
					this._isDisabled = value;
					base.OnPropertyChangedWithValue(value, "IsDisabled");
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
		public HeroVM HeroData
		{
			get
			{
				return this._heroData;
			}
			set
			{
				if (value != this._heroData)
				{
					this._heroData = value;
					base.OnPropertyChangedWithValue<HeroVM>(value, "HeroData");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel Hint
		{
			get
			{
				return this._hint;
			}
			set
			{
				if (value != this._hint)
				{
					this._hint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "Hint");
				}
			}
		}

		[DataSourceProperty]
		public float CurrentStamina
		{
			get
			{
				return this._currentStamina;
			}
			set
			{
				if (value != this._currentStamina)
				{
					this._currentStamina = value;
					base.OnPropertyChangedWithValue(value, "CurrentStamina");
				}
			}
		}

		[DataSourceProperty]
		public int MaxStamina
		{
			get
			{
				return this._maxStamina;
			}
			set
			{
				if (value != this._maxStamina)
				{
					this._maxStamina = value;
					base.OnPropertyChangedWithValue(value, "MaxStamina");
				}
			}
		}

		[DataSourceProperty]
		public string StaminaPercentage
		{
			get
			{
				return this._staminaPercentage;
			}
			set
			{
				if (value != this._staminaPercentage)
				{
					this._staminaPercentage = value;
					base.OnPropertyChangedWithValue<string>(value, "StaminaPercentage");
				}
			}
		}

		[DataSourceProperty]
		public int SmithySkillLevel
		{
			get
			{
				return this._smithySkillLevel;
			}
			set
			{
				if (value != this._smithySkillLevel)
				{
					this._smithySkillLevel = value;
					base.OnPropertyChangedWithValue(value, "SmithySkillLevel");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<CraftingPerkVM> CraftingPerks
		{
			get
			{
				return this._craftingPerks;
			}
			set
			{
				if (value != this._craftingPerks)
				{
					this._craftingPerks = value;
					base.OnPropertyChangedWithValue<MBBindingList<CraftingPerkVM>>(value, "CraftingPerks");
				}
			}
		}

		[DataSourceProperty]
		public string PerksText
		{
			get
			{
				return this._perksText;
			}
			set
			{
				if (value != this._perksText)
				{
					this._perksText = value;
					base.OnPropertyChangedWithValue<string>(value, "PerksText");
				}
			}
		}

		private readonly Action<CraftingAvailableHeroItemVM> _onSelection;

		private readonly ICraftingCampaignBehavior _craftingBehavior;

		private CraftingOrder _craftingOrder;

		private HeroVM _heroData;

		private BasicTooltipViewModel _hint;

		private float _currentStamina;

		private int _maxStamina;

		private string _staminaPercentage;

		private bool _isDisabled;

		private bool _isSelected;

		private int _smithySkillLevel;

		private MBBindingList<CraftingPerkVM> _craftingPerks;

		private string _perksText;
	}
}
