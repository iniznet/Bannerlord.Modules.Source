using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Armies;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Settlements
{
	// Token: 0x02000056 RID: 86
	public class KingdomSettlementItemVM : KingdomItemVM
	{
		// Token: 0x17000211 RID: 529
		// (get) Token: 0x06000705 RID: 1797 RVA: 0x0001EED5 File Offset: 0x0001D0D5
		// (set) Token: 0x06000706 RID: 1798 RVA: 0x0001EEDD File Offset: 0x0001D0DD
		public int Garrison { get; private set; }

		// Token: 0x17000212 RID: 530
		// (get) Token: 0x06000707 RID: 1799 RVA: 0x0001EEE6 File Offset: 0x0001D0E6
		// (set) Token: 0x06000708 RID: 1800 RVA: 0x0001EEEE File Offset: 0x0001D0EE
		public int Militia { get; private set; }

		// Token: 0x06000709 RID: 1801 RVA: 0x0001EEF8 File Offset: 0x0001D0F8
		public KingdomSettlementItemVM(Settlement settlement, Action<KingdomSettlementItemVM> onSelect)
		{
			this.Settlement = settlement;
			this._onSelect = onSelect;
			this.Name = settlement.Name.ToString();
			this.Villages = new MBBindingList<KingdomSettlementVillageItemVM>();
			SettlementComponent settlementComponent = settlement.SettlementComponent;
			this.SettlementImagePath = ((settlementComponent == null) ? "placeholder" : (settlementComponent.BackgroundMeshName + "_t"));
			this.ItemProperties = new MBBindingList<SelectableFiefItemPropertyVM>();
			this.ImageName = ((settlementComponent != null) ? settlementComponent.WaitMeshName : "");
			this.Owner = new HeroVM(settlement.OwnerClan.Leader, false);
			this.OwnerClanBanner = new ImageIdentifierVM(this.Settlement.OwnerClan.Banner);
			this.OwnerClanBanner_9 = new ImageIdentifierVM(BannerCode.CreateFrom(this.Settlement.OwnerClan.Banner), true);
			Town town = settlement.Town;
			this.WallLevel = ((town == null) ? (-1) : town.GetWallLevel());
			if (town != null)
			{
				this.Prosperity = MathF.Round(town.Prosperity);
				this.IconPath = town.BackgroundMeshName;
			}
			else if (settlement.IsCastle)
			{
				this.Prosperity = MathF.Round(settlement.Prosperity);
				this.IconPath = "";
			}
			foreach (Village village in this.Settlement.BoundVillages)
			{
				this.Villages.Add(new KingdomSettlementVillageItemVM(village));
			}
			int num;
			if (!this.Settlement.IsFortification)
			{
				num = (int)this.Settlement.Militia;
			}
			else
			{
				MobileParty garrisonParty = this.Settlement.Town.GarrisonParty;
				num = ((garrisonParty != null) ? garrisonParty.Party.NumberOfAllMembers : 0);
			}
			this.Defenders = num;
			this.RefreshValues();
		}

		// Token: 0x0600070A RID: 1802 RVA: 0x0001F0CC File Offset: 0x0001D2CC
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Villages.ApplyActionOnAllItems(delegate(KingdomSettlementVillageItemVM x)
			{
				x.RefreshValues();
			});
			this.UpdateProperties();
		}

		// Token: 0x0600070B RID: 1803 RVA: 0x0001F104 File Offset: 0x0001D304
		private void UpdateProperties()
		{
			this.ItemProperties.Clear();
			int num = (int)this.Settlement.Militia;
			List<TooltipProperty> militiaHint = (this.Settlement.IsVillage ? CampaignUIHelper.GetVillageMilitiaTooltip(this.Settlement.Village) : CampaignUIHelper.GetTownMilitiaTooltip(this.Settlement.Town));
			int num2 = ((this.Settlement.Town != null) ? ((int)this.Settlement.Town.MilitiaChange) : ((int)this.Settlement.Village.MilitiaChange));
			this.ItemProperties.Add(new SelectableFiefItemPropertyVM(GameTexts.FindText("str_militia", null).ToString(), num.ToString(), num2, SelectableItemPropertyVM.PropertyType.Militia, new BasicTooltipViewModel(() => militiaHint), false));
			BasicTooltipViewModel basicTooltipViewModel5;
			if (this.Settlement.Town != null)
			{
				BasicTooltipViewModel basicTooltipViewModel = new BasicTooltipViewModel(() => CampaignUIHelper.GetTownFoodTooltip(this.Settlement.Town));
				int num3 = (int)this.Settlement.Town.FoodChange;
				this.ItemProperties.Add(new SelectableFiefItemPropertyVM(GameTexts.FindText("str_food_stocks", null).ToString(), ((int)this.Settlement.Town.FoodStocks).ToString(), num3, SelectableItemPropertyVM.PropertyType.Food, basicTooltipViewModel, false));
				BasicTooltipViewModel basicTooltipViewModel2 = new BasicTooltipViewModel(() => CampaignUIHelper.GetTownGarrisonTooltip(this.Settlement.Town));
				int garrisonChange = this.Settlement.Town.GarrisonChange;
				Collection<SelectableFiefItemPropertyVM> itemProperties = this.ItemProperties;
				string text = GameTexts.FindText("str_garrison", null).ToString();
				MobileParty garrisonParty = this.Settlement.Town.GarrisonParty;
				itemProperties.Add(new SelectableFiefItemPropertyVM(text, ((garrisonParty != null) ? garrisonParty.Party.NumberOfAllMembers.ToString() : null) ?? "0", garrisonChange, SelectableItemPropertyVM.PropertyType.Garrison, basicTooltipViewModel2, false));
				BasicTooltipViewModel basicTooltipViewModel3 = new BasicTooltipViewModel(() => CampaignUIHelper.GetTownLoyaltyTooltip(this.Settlement.Town));
				int num4 = (int)this.Settlement.Town.LoyaltyChange;
				bool flag = this.Settlement.IsTown && this.Settlement.Town.Loyalty < (float)Campaign.Current.Models.SettlementLoyaltyModel.RebelliousStateStartLoyaltyThreshold;
				this.ItemProperties.Add(new SelectableFiefItemPropertyVM(GameTexts.FindText("str_loyalty", null).ToString(), string.Format("{0:0.#}", this.Settlement.Town.Loyalty), num4, SelectableItemPropertyVM.PropertyType.Loyalty, basicTooltipViewModel3, flag));
				BasicTooltipViewModel basicTooltipViewModel4 = new BasicTooltipViewModel(() => CampaignUIHelper.GetTownWallsTooltip(this.Settlement.Town));
				this.ItemProperties.Add(new SelectableFiefItemPropertyVM(GameTexts.FindText("str_walls", null).ToString(), this.Settlement.Town.GetWallLevel().ToString(), 0, SelectableItemPropertyVM.PropertyType.Wall, basicTooltipViewModel4, false));
				basicTooltipViewModel5 = new BasicTooltipViewModel(() => CampaignUIHelper.GetTownProsperityTooltip(this.Settlement.Town));
				BasicTooltipViewModel basicTooltipViewModel6 = new BasicTooltipViewModel(() => CampaignUIHelper.GetTownSecurityTooltip(this.Settlement.Town));
				int num5 = (int)this.Settlement.Town.SecurityChange;
				this.ItemProperties.Add(new SelectableFiefItemPropertyVM(GameTexts.FindText("str_security", null).ToString(), string.Format("{0:0.#}", this.Settlement.Town.Security), num5, SelectableItemPropertyVM.PropertyType.Security, basicTooltipViewModel6, false));
			}
			else
			{
				basicTooltipViewModel5 = new BasicTooltipViewModel(() => CampaignUIHelper.GetVillageProsperityTooltip(this.Settlement.Village));
			}
			int num6 = ((this.Settlement.Town != null) ? ((int)this.Settlement.Town.ProsperityChange) : ((int)this.Settlement.Village.HearthChange));
			if (this.Settlement.IsFortification)
			{
				this.ItemProperties.Add(new SelectableFiefItemPropertyVM(GameTexts.FindText("str_prosperity", null).ToString(), string.Format("{0:0.#}", this.Settlement.Town.Prosperity), num6, SelectableItemPropertyVM.PropertyType.Prosperity, basicTooltipViewModel5, false));
			}
		}

		// Token: 0x0600070C RID: 1804 RVA: 0x0001F4D7 File Offset: 0x0001D6D7
		protected override void OnSelect()
		{
			base.OnSelect();
			this._onSelect(this);
		}

		// Token: 0x0600070D RID: 1805 RVA: 0x0001F4EB File Offset: 0x0001D6EB
		private void ExecuteBeginHint()
		{
			InformationManager.ShowTooltip(typeof(Settlement), new object[] { this.Settlement, true });
		}

		// Token: 0x0600070E RID: 1806 RVA: 0x0001F514 File Offset: 0x0001D714
		private void ExecuteEndHint()
		{
			MBInformationManager.HideInformations();
		}

		// Token: 0x0600070F RID: 1807 RVA: 0x0001F51B File Offset: 0x0001D71B
		public void ExecuteLink()
		{
			if (this.Settlement != null)
			{
				Campaign.Current.EncyclopediaManager.GoToLink(this.Settlement.EncyclopediaLink);
			}
		}

		// Token: 0x17000213 RID: 531
		// (get) Token: 0x06000710 RID: 1808 RVA: 0x0001F53F File Offset: 0x0001D73F
		// (set) Token: 0x06000711 RID: 1809 RVA: 0x0001F547 File Offset: 0x0001D747
		[DataSourceProperty]
		public MBBindingList<SelectableFiefItemPropertyVM> ItemProperties
		{
			get
			{
				return this._itemProperties;
			}
			set
			{
				if (value != this._itemProperties)
				{
					this._itemProperties = value;
					base.OnPropertyChangedWithValue<MBBindingList<SelectableFiefItemPropertyVM>>(value, "ItemProperties");
				}
			}
		}

		// Token: 0x17000214 RID: 532
		// (get) Token: 0x06000712 RID: 1810 RVA: 0x0001F565 File Offset: 0x0001D765
		// (set) Token: 0x06000713 RID: 1811 RVA: 0x0001F56D File Offset: 0x0001D76D
		[DataSourceProperty]
		public MBBindingList<KingdomSettlementVillageItemVM> Villages
		{
			get
			{
				return this._villages;
			}
			set
			{
				if (value != this._villages)
				{
					this._villages = value;
					base.OnPropertyChangedWithValue<MBBindingList<KingdomSettlementVillageItemVM>>(value, "Villages");
				}
			}
		}

		// Token: 0x17000215 RID: 533
		// (get) Token: 0x06000714 RID: 1812 RVA: 0x0001F58B File Offset: 0x0001D78B
		// (set) Token: 0x06000715 RID: 1813 RVA: 0x0001F593 File Offset: 0x0001D793
		[DataSourceProperty]
		public string IconPath
		{
			get
			{
				return this._iconPath;
			}
			set
			{
				if (value != this._iconPath)
				{
					this._iconPath = value;
					base.OnPropertyChangedWithValue<string>(value, "IconPath");
				}
			}
		}

		// Token: 0x17000216 RID: 534
		// (get) Token: 0x06000716 RID: 1814 RVA: 0x0001F5B6 File Offset: 0x0001D7B6
		// (set) Token: 0x06000717 RID: 1815 RVA: 0x0001F5BE File Offset: 0x0001D7BE
		[DataSourceProperty]
		public int Defenders
		{
			get
			{
				return this._defenders;
			}
			set
			{
				if (value != this._defenders)
				{
					this._defenders = value;
					base.OnPropertyChangedWithValue(value, "Defenders");
				}
			}
		}

		// Token: 0x17000217 RID: 535
		// (get) Token: 0x06000718 RID: 1816 RVA: 0x0001F5DC File Offset: 0x0001D7DC
		// (set) Token: 0x06000719 RID: 1817 RVA: 0x0001F5E4 File Offset: 0x0001D7E4
		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					base.OnPropertyChangedWithValue<string>(value, "Name");
				}
			}
		}

		// Token: 0x17000218 RID: 536
		// (get) Token: 0x0600071A RID: 1818 RVA: 0x0001F607 File Offset: 0x0001D807
		// (set) Token: 0x0600071B RID: 1819 RVA: 0x0001F60F File Offset: 0x0001D80F
		[DataSourceProperty]
		public string ImageName
		{
			get
			{
				return this._imageName;
			}
			set
			{
				if (value != this._imageName)
				{
					this._imageName = value;
					base.OnPropertyChangedWithValue<string>(value, "ImageName");
				}
			}
		}

		// Token: 0x17000219 RID: 537
		// (get) Token: 0x0600071C RID: 1820 RVA: 0x0001F632 File Offset: 0x0001D832
		// (set) Token: 0x0600071D RID: 1821 RVA: 0x0001F63A File Offset: 0x0001D83A
		[DataSourceProperty]
		public string SettlementImagePath
		{
			get
			{
				return this._settlementImagePath;
			}
			set
			{
				if (value != this._settlementImagePath)
				{
					this._settlementImagePath = value;
					base.OnPropertyChangedWithValue<string>(value, "SettlementImagePath");
				}
			}
		}

		// Token: 0x1700021A RID: 538
		// (get) Token: 0x0600071E RID: 1822 RVA: 0x0001F65D File Offset: 0x0001D85D
		// (set) Token: 0x0600071F RID: 1823 RVA: 0x0001F665 File Offset: 0x0001D865
		[DataSourceProperty]
		public string GovernorName
		{
			get
			{
				return this._governorName;
			}
			set
			{
				if (value != this._governorName)
				{
					this._governorName = value;
					base.OnPropertyChangedWithValue<string>(value, "GovernorName");
				}
			}
		}

		// Token: 0x1700021B RID: 539
		// (get) Token: 0x06000720 RID: 1824 RVA: 0x0001F688 File Offset: 0x0001D888
		// (set) Token: 0x06000721 RID: 1825 RVA: 0x0001F690 File Offset: 0x0001D890
		[DataSourceProperty]
		public ImageIdentifierVM OwnerClanBanner
		{
			get
			{
				return this._ownerClanBanner;
			}
			set
			{
				if (value != this._ownerClanBanner)
				{
					this._ownerClanBanner = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "OwnerClanBanner");
				}
			}
		}

		// Token: 0x1700021C RID: 540
		// (get) Token: 0x06000722 RID: 1826 RVA: 0x0001F6AE File Offset: 0x0001D8AE
		// (set) Token: 0x06000723 RID: 1827 RVA: 0x0001F6B6 File Offset: 0x0001D8B6
		[DataSourceProperty]
		public ImageIdentifierVM OwnerClanBanner_9
		{
			get
			{
				return this._ownerClanBanner_9;
			}
			set
			{
				if (value != this._ownerClanBanner_9)
				{
					this._ownerClanBanner_9 = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "OwnerClanBanner_9");
				}
			}
		}

		// Token: 0x1700021D RID: 541
		// (get) Token: 0x06000724 RID: 1828 RVA: 0x0001F6D4 File Offset: 0x0001D8D4
		// (set) Token: 0x06000725 RID: 1829 RVA: 0x0001F6DC File Offset: 0x0001D8DC
		[DataSourceProperty]
		public HeroVM Owner
		{
			get
			{
				return this._owner;
			}
			set
			{
				if (value != this._owner)
				{
					this._owner = value;
					base.OnPropertyChangedWithValue<HeroVM>(value, "Owner");
				}
			}
		}

		// Token: 0x1700021E RID: 542
		// (get) Token: 0x06000726 RID: 1830 RVA: 0x0001F6FA File Offset: 0x0001D8FA
		// (set) Token: 0x06000727 RID: 1831 RVA: 0x0001F702 File Offset: 0x0001D902
		[DataSourceProperty]
		public int WallLevel
		{
			get
			{
				return this._wallLevel;
			}
			set
			{
				if (value != this._wallLevel)
				{
					this._wallLevel = value;
					base.OnPropertyChangedWithValue(value, "WallLevel");
				}
			}
		}

		// Token: 0x1700021F RID: 543
		// (get) Token: 0x06000728 RID: 1832 RVA: 0x0001F720 File Offset: 0x0001D920
		// (set) Token: 0x06000729 RID: 1833 RVA: 0x0001F728 File Offset: 0x0001D928
		[DataSourceProperty]
		public int Prosperity
		{
			get
			{
				return this._prosperity;
			}
			set
			{
				if (value != this._prosperity)
				{
					this._prosperity = value;
					base.OnPropertyChangedWithValue(value, "Prosperity");
				}
			}
		}

		// Token: 0x04000313 RID: 787
		private readonly Action<KingdomSettlementItemVM> _onSelect;

		// Token: 0x04000314 RID: 788
		public readonly Settlement Settlement;

		// Token: 0x04000317 RID: 791
		private string _iconPath;

		// Token: 0x04000318 RID: 792
		private string _name;

		// Token: 0x04000319 RID: 793
		private string _imageName;

		// Token: 0x0400031A RID: 794
		private string _settlementImagePath;

		// Token: 0x0400031B RID: 795
		private string _governorName;

		// Token: 0x0400031C RID: 796
		private ImageIdentifierVM _ownerClanBanner;

		// Token: 0x0400031D RID: 797
		private ImageIdentifierVM _ownerClanBanner_9;

		// Token: 0x0400031E RID: 798
		private HeroVM _owner;

		// Token: 0x0400031F RID: 799
		private MBBindingList<SelectableFiefItemPropertyVM> _itemProperties;

		// Token: 0x04000320 RID: 800
		private MBBindingList<KingdomSettlementVillageItemVM> _villages;

		// Token: 0x04000321 RID: 801
		private int _wallLevel;

		// Token: 0x04000322 RID: 802
		private int _prosperity;

		// Token: 0x04000323 RID: 803
		private int _defenders;
	}
}
