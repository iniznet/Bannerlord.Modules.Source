using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.CraftingSystem;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign.Order
{
	public class CraftingOrderItemVM : ViewModel
	{
		public CraftingOrder CraftingOrder { get; }

		public CraftingOrderItemVM(CraftingOrder order, Action<CraftingOrderItemVM> onSelection, Func<CraftingAvailableHeroItemVM> getCurrentCraftingHero, List<CraftingStatData> orderStatDatas)
		{
			this.CraftingOrder = order;
			this._orderOwner = order.OrderOwner;
			this._getCurrentCraftingHero = getCurrentCraftingHero;
			this._orderStatDatas = orderStatDatas;
			this._onSelection = onSelection;
			this.WeaponAttributes = new MBBindingList<WeaponAttributeVM>();
			this.OrderOwnerData = new HeroVM(this._orderOwner, false);
			this._weaponTemplate = order.PreCraftedWeaponDesignItem.WeaponDesign.Template;
			this.OrderWeaponTypeCode = this._weaponTemplate.StringId;
			this.RefreshValues();
			this.RefreshStats();
		}

		public void RefreshStats()
		{
			this.WeaponAttributes.Clear();
			ItemObject preCraftedWeaponDesignItem = this.CraftingOrder.PreCraftedWeaponDesignItem;
			if (((preCraftedWeaponDesignItem != null) ? preCraftedWeaponDesignItem.Weapons : null) == null)
			{
				Debug.FailedAssert("Crafting order does not contain any valid weapons", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem.ViewModelCollection\\Crafting\\WeaponDesign\\Order\\CraftingOrderItemVM.cs", "RefreshStats", 50);
				return;
			}
			this.CraftingOrder.GetStatWeapon();
			foreach (CraftingStatData craftingStatData in this._orderStatDatas)
			{
				if (craftingStatData.IsValid)
				{
					this.WeaponAttributes.Add(new WeaponAttributeVM(craftingStatData.Type, craftingStatData.DamageType, craftingStatData.DescriptionText.ToString(), craftingStatData.CurValue));
				}
			}
			IEnumerable<Hero> enumerable = from x in CraftingHelper.GetAvailableHeroesForCrafting()
				where this.CraftingOrder.IsOrderAvailableForHero(x)
				select x;
			this.HasAvailableHeroes = enumerable.Any<Hero>();
			this.OrderPrice = this.CraftingOrder.BaseGoldReward;
			this.RefreshDifficulty();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.OrderNumberText = GameTexts.FindText("str_crafting_order_header", null).ToString();
			this.OrderWeaponType = this._weaponTemplate.TemplateName.ToString();
			this.OrderDifficultyLabelText = this._difficultyText.ToString();
			this.OrderDifficultyValueText = MathF.Round(this.CraftingOrder.OrderDifficulty).ToString();
			this.DisabledReasonHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetCraftingOrderDisabledReasonTooltip(this._getCurrentCraftingHero().Hero, this.CraftingOrder));
		}

		private void RefreshDifficulty()
		{
			Hero hero = this._getCurrentCraftingHero().Hero;
			int skillValue = hero.GetSkillValue(DefaultSkills.Crafting);
			this.IsEnabled = this.CraftingOrder.IsOrderAvailableForHero(hero);
			this.IsDifficultySuitableForHero = this.CraftingOrder.OrderDifficulty < (float)skillValue;
		}

		public void ExecuteSelectOrder()
		{
			Action<CraftingOrderItemVM> onSelection = this._onSelection;
			if (onSelection == null)
			{
				return;
			}
			onSelection(this);
		}

		[DataSourceProperty]
		public bool HasAvailableHeroes
		{
			get
			{
				return this._hasAvailableHeroes;
			}
			set
			{
				if (value != this._hasAvailableHeroes)
				{
					this._hasAvailableHeroes = value;
					base.OnPropertyChangedWithValue(value, "HasAvailableHeroes");
				}
			}
		}

		[DataSourceProperty]
		public bool IsDifficultySuitableForHero
		{
			get
			{
				return this._isDifficultySuitableForHero;
			}
			set
			{
				if (value != this._isDifficultySuitableForHero)
				{
					this._isDifficultySuitableForHero = value;
					base.OnPropertyChangedWithValue(value, "IsDifficultySuitableForHero");
				}
			}
		}

		[DataSourceProperty]
		public string OrderDifficultyLabelText
		{
			get
			{
				return this._orderDifficultyLabelText;
			}
			set
			{
				if (value != this._orderDifficultyLabelText)
				{
					this._orderDifficultyLabelText = value;
					base.OnPropertyChangedWithValue<string>(value, "OrderDifficultyLabelText");
				}
			}
		}

		[DataSourceProperty]
		public string OrderDifficultyValueText
		{
			get
			{
				return this._orderDifficultyValueText;
			}
			set
			{
				if (value != this._orderDifficultyValueText)
				{
					this._orderDifficultyValueText = value;
					base.OnPropertyChangedWithValue<string>(value, "OrderDifficultyValueText");
				}
			}
		}

		[DataSourceProperty]
		public string OrderNumberText
		{
			get
			{
				return this._orderNumberText;
			}
			set
			{
				if (value != this._orderNumberText)
				{
					this._orderNumberText = value;
					base.OnPropertyChangedWithValue<string>(value, "OrderNumberText");
				}
			}
		}

		[DataSourceProperty]
		public string OrderWeaponType
		{
			get
			{
				return this._orderWeaponType;
			}
			set
			{
				if (value != this._orderWeaponType)
				{
					this._orderWeaponType = value;
					base.OnPropertyChangedWithValue<string>(value, "OrderWeaponType");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<WeaponAttributeVM> WeaponAttributes
		{
			get
			{
				return this._weaponAttributes;
			}
			set
			{
				if (value != this._weaponAttributes)
				{
					this._weaponAttributes = value;
					base.OnPropertyChangedWithValue<MBBindingList<WeaponAttributeVM>>(value, "WeaponAttributes");
				}
			}
		}

		[DataSourceProperty]
		public HeroVM OrderOwnerData
		{
			get
			{
				return this._orderOwnerData;
			}
			set
			{
				if (value != this._orderOwnerData)
				{
					this._orderOwnerData = value;
					base.OnPropertyChangedWithValue<HeroVM>(value, "OrderOwnerData");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel DisabledReasonHint
		{
			get
			{
				return this._disabledReasonHint;
			}
			set
			{
				if (value != this._disabledReasonHint)
				{
					this._disabledReasonHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "DisabledReasonHint");
				}
			}
		}

		[DataSourceProperty]
		public int OrderPrice
		{
			get
			{
				return this._orderPrice;
			}
			set
			{
				if (value != this._orderPrice)
				{
					this._orderPrice = value;
					base.OnPropertyChangedWithValue(value, "OrderPrice");
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
		public string OrderWeaponTypeCode
		{
			get
			{
				return this._orderWeaponTypeCode;
			}
			set
			{
				if (value != this._orderWeaponTypeCode)
				{
					this._orderWeaponTypeCode = value;
					base.OnPropertyChangedWithValue<string>(value, "OrderWeaponTypeCode");
				}
			}
		}

		private Hero _orderOwner;

		private Action<CraftingOrderItemVM> _onSelection;

		private Func<CraftingAvailableHeroItemVM> _getCurrentCraftingHero;

		private CraftingTemplate _weaponTemplate;

		private TextObject _difficultyText = new TextObject("{=udPWHmOm}Difficulty:", null);

		private List<CraftingStatData> _orderStatDatas;

		private bool _hasAvailableHeroes;

		private bool _isDifficultySuitableForHero;

		private string _orderDifficultyLabelText;

		private string _orderDifficultyValueText;

		private string _orderNumberText;

		private string _orderWeaponType;

		private MBBindingList<WeaponAttributeVM> _weaponAttributes;

		private HeroVM _orderOwnerData;

		private int _orderPrice;

		private bool _isEnabled;

		private bool _isSelected;

		private BasicTooltipViewModel _disabledReasonHint;

		private string _orderWeaponTypeCode;
	}
}
