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
	// Token: 0x020000EA RID: 234
	public class CraftingOrderItemVM : ViewModel
	{
		// Token: 0x1700076E RID: 1902
		// (get) Token: 0x060015FF RID: 5631 RVA: 0x0005271D File Offset: 0x0005091D
		public CraftingOrder CraftingOrder { get; }

		// Token: 0x06001600 RID: 5632 RVA: 0x00052728 File Offset: 0x00050928
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

		// Token: 0x06001601 RID: 5633 RVA: 0x000527C8 File Offset: 0x000509C8
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

		// Token: 0x06001602 RID: 5634 RVA: 0x000528CC File Offset: 0x00050ACC
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.OrderNumberText = GameTexts.FindText("str_crafting_order_header", null).ToString();
			this.OrderWeaponType = this._weaponTemplate.TemplateName.ToString();
			this.OrderDifficultyLabelText = this._difficultyText.ToString();
			this.OrderDifficultyValueText = MathF.Round(this.CraftingOrder.OrderDifficulty).ToString();
			this.DisabledReasonHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetCraftingOrderDisabledReasonTooltip(this._getCurrentCraftingHero().Hero, this.CraftingOrder));
		}

		// Token: 0x06001603 RID: 5635 RVA: 0x00052954 File Offset: 0x00050B54
		private void RefreshDifficulty()
		{
			Hero hero = this._getCurrentCraftingHero().Hero;
			int skillValue = hero.GetSkillValue(DefaultSkills.Crafting);
			this.IsEnabled = this.CraftingOrder.IsOrderAvailableForHero(hero);
			this.IsDifficultySuitableForHero = this.CraftingOrder.OrderDifficulty < (float)skillValue;
		}

		// Token: 0x06001604 RID: 5636 RVA: 0x000529A5 File Offset: 0x00050BA5
		public void ExecuteSelectOrder()
		{
			Action<CraftingOrderItemVM> onSelection = this._onSelection;
			if (onSelection == null)
			{
				return;
			}
			onSelection(this);
		}

		// Token: 0x1700076F RID: 1903
		// (get) Token: 0x06001605 RID: 5637 RVA: 0x000529B8 File Offset: 0x00050BB8
		// (set) Token: 0x06001606 RID: 5638 RVA: 0x000529C0 File Offset: 0x00050BC0
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

		// Token: 0x17000770 RID: 1904
		// (get) Token: 0x06001607 RID: 5639 RVA: 0x000529DE File Offset: 0x00050BDE
		// (set) Token: 0x06001608 RID: 5640 RVA: 0x000529E6 File Offset: 0x00050BE6
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

		// Token: 0x17000771 RID: 1905
		// (get) Token: 0x06001609 RID: 5641 RVA: 0x00052A04 File Offset: 0x00050C04
		// (set) Token: 0x0600160A RID: 5642 RVA: 0x00052A0C File Offset: 0x00050C0C
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

		// Token: 0x17000772 RID: 1906
		// (get) Token: 0x0600160B RID: 5643 RVA: 0x00052A2F File Offset: 0x00050C2F
		// (set) Token: 0x0600160C RID: 5644 RVA: 0x00052A37 File Offset: 0x00050C37
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

		// Token: 0x17000773 RID: 1907
		// (get) Token: 0x0600160D RID: 5645 RVA: 0x00052A5A File Offset: 0x00050C5A
		// (set) Token: 0x0600160E RID: 5646 RVA: 0x00052A62 File Offset: 0x00050C62
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

		// Token: 0x17000774 RID: 1908
		// (get) Token: 0x0600160F RID: 5647 RVA: 0x00052A85 File Offset: 0x00050C85
		// (set) Token: 0x06001610 RID: 5648 RVA: 0x00052A8D File Offset: 0x00050C8D
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

		// Token: 0x17000775 RID: 1909
		// (get) Token: 0x06001611 RID: 5649 RVA: 0x00052AB0 File Offset: 0x00050CB0
		// (set) Token: 0x06001612 RID: 5650 RVA: 0x00052AB8 File Offset: 0x00050CB8
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

		// Token: 0x17000776 RID: 1910
		// (get) Token: 0x06001613 RID: 5651 RVA: 0x00052AD6 File Offset: 0x00050CD6
		// (set) Token: 0x06001614 RID: 5652 RVA: 0x00052ADE File Offset: 0x00050CDE
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

		// Token: 0x17000777 RID: 1911
		// (get) Token: 0x06001615 RID: 5653 RVA: 0x00052AFC File Offset: 0x00050CFC
		// (set) Token: 0x06001616 RID: 5654 RVA: 0x00052B04 File Offset: 0x00050D04
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

		// Token: 0x17000778 RID: 1912
		// (get) Token: 0x06001617 RID: 5655 RVA: 0x00052B22 File Offset: 0x00050D22
		// (set) Token: 0x06001618 RID: 5656 RVA: 0x00052B2A File Offset: 0x00050D2A
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

		// Token: 0x17000779 RID: 1913
		// (get) Token: 0x06001619 RID: 5657 RVA: 0x00052B48 File Offset: 0x00050D48
		// (set) Token: 0x0600161A RID: 5658 RVA: 0x00052B50 File Offset: 0x00050D50
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

		// Token: 0x1700077A RID: 1914
		// (get) Token: 0x0600161B RID: 5659 RVA: 0x00052B6E File Offset: 0x00050D6E
		// (set) Token: 0x0600161C RID: 5660 RVA: 0x00052B76 File Offset: 0x00050D76
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

		// Token: 0x1700077B RID: 1915
		// (get) Token: 0x0600161D RID: 5661 RVA: 0x00052B94 File Offset: 0x00050D94
		// (set) Token: 0x0600161E RID: 5662 RVA: 0x00052B9C File Offset: 0x00050D9C
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

		// Token: 0x04000A4A RID: 2634
		private Hero _orderOwner;

		// Token: 0x04000A4B RID: 2635
		private Action<CraftingOrderItemVM> _onSelection;

		// Token: 0x04000A4C RID: 2636
		private Func<CraftingAvailableHeroItemVM> _getCurrentCraftingHero;

		// Token: 0x04000A4D RID: 2637
		private CraftingTemplate _weaponTemplate;

		// Token: 0x04000A4E RID: 2638
		private TextObject _difficultyText = new TextObject("{=udPWHmOm}Difficulty:", null);

		// Token: 0x04000A4F RID: 2639
		private List<CraftingStatData> _orderStatDatas;

		// Token: 0x04000A50 RID: 2640
		private bool _hasAvailableHeroes;

		// Token: 0x04000A51 RID: 2641
		private bool _isDifficultySuitableForHero;

		// Token: 0x04000A52 RID: 2642
		private string _orderDifficultyLabelText;

		// Token: 0x04000A53 RID: 2643
		private string _orderDifficultyValueText;

		// Token: 0x04000A54 RID: 2644
		private string _orderNumberText;

		// Token: 0x04000A55 RID: 2645
		private string _orderWeaponType;

		// Token: 0x04000A56 RID: 2646
		private MBBindingList<WeaponAttributeVM> _weaponAttributes;

		// Token: 0x04000A57 RID: 2647
		private HeroVM _orderOwnerData;

		// Token: 0x04000A58 RID: 2648
		private int _orderPrice;

		// Token: 0x04000A59 RID: 2649
		private bool _isEnabled;

		// Token: 0x04000A5A RID: 2650
		private bool _isSelected;

		// Token: 0x04000A5B RID: 2651
		private BasicTooltipViewModel _disabledReasonHint;

		// Token: 0x04000A5C RID: 2652
		private string _orderWeaponTypeCode;
	}
}
