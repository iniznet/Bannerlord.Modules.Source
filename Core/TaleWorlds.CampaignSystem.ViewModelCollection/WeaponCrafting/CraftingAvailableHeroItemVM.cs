using System;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.CraftingSystem;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting
{
	// Token: 0x020000D6 RID: 214
	public class CraftingAvailableHeroItemVM : ViewModel
	{
		// Token: 0x170006A0 RID: 1696
		// (get) Token: 0x060013CB RID: 5067 RVA: 0x0004BA35 File Offset: 0x00049C35
		public Hero Hero { get; }

		// Token: 0x060013CC RID: 5068 RVA: 0x0004BA40 File Offset: 0x00049C40
		public CraftingAvailableHeroItemVM(Hero hero, Action<CraftingAvailableHeroItemVM> onSelection)
		{
			this._onSelection = onSelection;
			this._craftingBehavior = Campaign.Current.GetCampaignBehavior<ICraftingCampaignBehavior>();
			this.Hero = hero;
			this.HeroData = new HeroVM(this.Hero, false);
			this.Hint = new BasicTooltipViewModel(() => CampaignUIHelper.GetCraftingHeroTooltip(this.Hero, this._craftingOrder));
		}

		// Token: 0x060013CD RID: 5069 RVA: 0x0004BA9A File Offset: 0x00049C9A
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.HeroData.RefreshValues();
		}

		// Token: 0x060013CE RID: 5070 RVA: 0x0004BAB0 File Offset: 0x00049CB0
		public void RefreshStamina()
		{
			this.CurrentStamina = (float)this._craftingBehavior.GetHeroCraftingStamina(this.Hero);
			this.MaxStamina = this._craftingBehavior.GetMaxHeroCraftingStamina(this.Hero);
			int num = (int)(this.CurrentStamina / (float)this.MaxStamina * 100f);
			GameTexts.SetVariable("NUMBER", num);
			this.StaminaPercentage = GameTexts.FindText("str_NUMBER_percent", null).ToString();
		}

		// Token: 0x060013CF RID: 5071 RVA: 0x0004BB23 File Offset: 0x00049D23
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

		// Token: 0x060013D0 RID: 5072 RVA: 0x0004BB4C File Offset: 0x00049D4C
		public void RefreshSkills()
		{
			this.SmithySkillLevel = this.Hero.GetSkillValue(DefaultSkills.Crafting);
		}

		// Token: 0x060013D1 RID: 5073 RVA: 0x0004BB64 File Offset: 0x00049D64
		public void ExecuteSelection()
		{
			Action<CraftingAvailableHeroItemVM> onSelection = this._onSelection;
			if (onSelection == null)
			{
				return;
			}
			onSelection(this);
		}

		// Token: 0x170006A1 RID: 1697
		// (get) Token: 0x060013D2 RID: 5074 RVA: 0x0004BB77 File Offset: 0x00049D77
		// (set) Token: 0x060013D3 RID: 5075 RVA: 0x0004BB7F File Offset: 0x00049D7F
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

		// Token: 0x170006A2 RID: 1698
		// (get) Token: 0x060013D4 RID: 5076 RVA: 0x0004BB9D File Offset: 0x00049D9D
		// (set) Token: 0x060013D5 RID: 5077 RVA: 0x0004BBA5 File Offset: 0x00049DA5
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

		// Token: 0x170006A3 RID: 1699
		// (get) Token: 0x060013D6 RID: 5078 RVA: 0x0004BBC3 File Offset: 0x00049DC3
		// (set) Token: 0x060013D7 RID: 5079 RVA: 0x0004BBCB File Offset: 0x00049DCB
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

		// Token: 0x170006A4 RID: 1700
		// (get) Token: 0x060013D8 RID: 5080 RVA: 0x0004BBE9 File Offset: 0x00049DE9
		// (set) Token: 0x060013D9 RID: 5081 RVA: 0x0004BBF1 File Offset: 0x00049DF1
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

		// Token: 0x170006A5 RID: 1701
		// (get) Token: 0x060013DA RID: 5082 RVA: 0x0004BC0F File Offset: 0x00049E0F
		// (set) Token: 0x060013DB RID: 5083 RVA: 0x0004BC17 File Offset: 0x00049E17
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

		// Token: 0x170006A6 RID: 1702
		// (get) Token: 0x060013DC RID: 5084 RVA: 0x0004BC35 File Offset: 0x00049E35
		// (set) Token: 0x060013DD RID: 5085 RVA: 0x0004BC3D File Offset: 0x00049E3D
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

		// Token: 0x170006A7 RID: 1703
		// (get) Token: 0x060013DE RID: 5086 RVA: 0x0004BC60 File Offset: 0x00049E60
		// (set) Token: 0x060013DF RID: 5087 RVA: 0x0004BC68 File Offset: 0x00049E68
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

		// Token: 0x04000939 RID: 2361
		private readonly Action<CraftingAvailableHeroItemVM> _onSelection;

		// Token: 0x0400093A RID: 2362
		private readonly ICraftingCampaignBehavior _craftingBehavior;

		// Token: 0x0400093B RID: 2363
		private CraftingOrder _craftingOrder;

		// Token: 0x0400093C RID: 2364
		private HeroVM _heroData;

		// Token: 0x0400093D RID: 2365
		private BasicTooltipViewModel _hint;

		// Token: 0x0400093E RID: 2366
		private float _currentStamina;

		// Token: 0x0400093F RID: 2367
		private int _maxStamina;

		// Token: 0x04000940 RID: 2368
		private string _staminaPercentage;

		// Token: 0x04000941 RID: 2369
		private bool _isDisabled;

		// Token: 0x04000942 RID: 2370
		private int _smithySkillLevel;
	}
}
