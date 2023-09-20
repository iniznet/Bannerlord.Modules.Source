using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement
{
	// Token: 0x0200010A RID: 266
	public class ClanSettlementItemVM : ViewModel
	{
		// Token: 0x06001963 RID: 6499 RVA: 0x0005BCE8 File Offset: 0x00059EE8
		public ClanSettlementItemVM(Settlement settlement, Action<ClanSettlementItemVM> onSelection, Action onShowSendMembers, ITeleportationCampaignBehavior teleportationBehavior)
		{
			this.Settlement = settlement;
			this._onSelection = onSelection;
			this._onShowSendMembers = onShowSendMembers;
			this._teleportationBehavior = teleportationBehavior;
			this.IsFortification = settlement.IsFortification;
			SettlementComponent settlementComponent = settlement.SettlementComponent;
			this.FileName = ((settlementComponent == null) ? "placeholder" : (settlementComponent.BackgroundMeshName + "_t"));
			this.ItemProperties = new MBBindingList<SelectableFiefItemPropertyVM>();
			this.ImageName = ((settlementComponent != null) ? settlementComponent.WaitMeshName : "");
			this.VillagesOwned = new MBBindingList<ClanSettlementItemVM>();
			this.Notables = new MBBindingList<HeroVM>();
			this.Members = new MBBindingList<HeroVM>();
			this.RefreshValues();
		}

		// Token: 0x06001964 RID: 6500 RVA: 0x0005BD94 File Offset: 0x00059F94
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.VillagesText = GameTexts.FindText("str_villages", null).ToString();
			this.NotablesText = GameTexts.FindText("str_center_notables", null).ToString();
			this.MembersText = GameTexts.FindText("str_members", null).ToString();
			this.Name = this.Settlement.Name.ToString();
			this.UpdateProperties();
		}

		// Token: 0x06001965 RID: 6501 RVA: 0x0005BE05 File Offset: 0x0005A005
		public void OnSettlementSelection()
		{
			this._onSelection(this);
		}

		// Token: 0x06001966 RID: 6502 RVA: 0x0005BE13 File Offset: 0x0005A013
		public void ExecuteLink()
		{
			MBInformationManager.HideInformations();
			Campaign.Current.EncyclopediaManager.GoToLink(this.Settlement.EncyclopediaLink);
		}

		// Token: 0x06001967 RID: 6503 RVA: 0x0005BE34 File Offset: 0x0005A034
		public void ExecuteCloseTooltip()
		{
			MBInformationManager.HideInformations();
		}

		// Token: 0x06001968 RID: 6504 RVA: 0x0005BE3B File Offset: 0x0005A03B
		public void ExecuteOpenTooltip()
		{
			InformationManager.ShowTooltip(typeof(Settlement), new object[] { this.Settlement, true });
		}

		// Token: 0x06001969 RID: 6505 RVA: 0x0005BE64 File Offset: 0x0005A064
		public void ExecuteSendMembers()
		{
			Action onShowSendMembers = this._onShowSendMembers;
			if (onShowSendMembers == null)
			{
				return;
			}
			onShowSendMembers();
		}

		// Token: 0x0600196A RID: 6506 RVA: 0x0005BE76 File Offset: 0x0005A076
		private void OnGovernorChanged(Hero oldHero, Hero newHero)
		{
			ChangeGovernorAction.Apply(this.Settlement.Town, newHero);
		}

		// Token: 0x0600196B RID: 6507 RVA: 0x0005BE89 File Offset: 0x0005A089
		private bool IsGovernorAssignable(Hero oldHero, Hero newHero)
		{
			return newHero.IsActive && newHero.GovernorOf == null;
		}

		// Token: 0x0600196C RID: 6508 RVA: 0x0005BEA0 File Offset: 0x0005A0A0
		private void UpdateProperties()
		{
			this.ItemProperties.Clear();
			this.VillagesOwned.Clear();
			this.Notables.Clear();
			this.Members.Clear();
			foreach (Village village in this.Settlement.BoundVillages)
			{
				this.VillagesOwned.Add(new ClanSettlementItemVM(village.Settlement, null, null, null));
			}
			this.HasNotables = !this.Settlement.Notables.IsEmpty<Hero>();
			foreach (Hero hero in this.Settlement.Notables)
			{
				this.Notables.Add(new HeroVM(hero, false));
			}
			foreach (Hero hero2 in this.Settlement.HeroesWithoutParty.Where((Hero h) => h.Clan == Clan.PlayerClan))
			{
				this.Members.Add(new HeroVM(hero2, false));
			}
			this.HasGovernor = false;
			if (!this.Settlement.IsVillage)
			{
				Town town = this.Settlement.Town;
				Hero hero3 = ((((town != null) ? town.Governor : null) != null) ? this.Settlement.Town.Governor : CampaignUIHelper.GetTeleportingGovernor(this.Settlement, this._teleportationBehavior));
				this.HasGovernor = hero3 != null;
				this.Governor = (this.HasGovernor ? new HeroVM(hero3, false) : null);
			}
			this.IsFortification = this.Settlement.IsFortification;
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
				this.ItemProperties.Add(new SelectableFiefItemPropertyVM(GameTexts.FindText("str_prosperity", null).ToString(), this.Settlement.Prosperity.ToString(), num6, SelectableItemPropertyVM.PropertyType.Prosperity, basicTooltipViewModel5, false));
			}
			TextObject textObject;
			this.IsSendMembersEnabled = CampaignUIHelper.GetMapScreenActionIsEnabledWithReason(out textObject);
			TextObject textObject2 = new TextObject("{=uGMGjUZy}Send your clan members to {SETTLEMENT_NAME}", null);
			textObject2.SetTextVariable("SETTLEMENT_NAME", this.Settlement.Name.ToString());
			this.SendMembersHint = new HintViewModel(this.IsSendMembersEnabled ? textObject2 : textObject, null);
		}

		// Token: 0x0600196D RID: 6509 RVA: 0x0005C4A8 File Offset: 0x0005A6A8
		private bool IsSettlementSlotAssignable(Hero oldHero, Hero newHero)
		{
			return (oldHero == null || !oldHero.IsHumanPlayerCharacter) && !newHero.IsHumanPlayerCharacter && newHero.IsActive && (newHero.PartyBelongedTo == null || newHero.PartyBelongedTo.LeaderHero != newHero) && newHero.PartyBelongedToAsPrisoner == null;
		}

		// Token: 0x0600196E RID: 6510 RVA: 0x0005C4F7 File Offset: 0x0005A6F7
		private void ExecuteOpenSettlementPage()
		{
			Campaign.Current.EncyclopediaManager.GoToLink(this.Settlement.EncyclopediaLink);
		}

		// Token: 0x170008B5 RID: 2229
		// (get) Token: 0x0600196F RID: 6511 RVA: 0x0005C513 File Offset: 0x0005A713
		// (set) Token: 0x06001970 RID: 6512 RVA: 0x0005C51B File Offset: 0x0005A71B
		[DataSourceProperty]
		public HeroVM Governor
		{
			get
			{
				return this._governor;
			}
			set
			{
				if (value != this._governor)
				{
					this._governor = value;
					base.OnPropertyChangedWithValue<HeroVM>(value, "Governor");
				}
			}
		}

		// Token: 0x170008B6 RID: 2230
		// (get) Token: 0x06001971 RID: 6513 RVA: 0x0005C539 File Offset: 0x0005A739
		// (set) Token: 0x06001972 RID: 6514 RVA: 0x0005C541 File Offset: 0x0005A741
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

		// Token: 0x170008B7 RID: 2231
		// (get) Token: 0x06001973 RID: 6515 RVA: 0x0005C55F File Offset: 0x0005A75F
		// (set) Token: 0x06001974 RID: 6516 RVA: 0x0005C567 File Offset: 0x0005A767
		[DataSourceProperty]
		public string FileName
		{
			get
			{
				return this._fileName;
			}
			set
			{
				if (value != this._fileName)
				{
					this._fileName = value;
					base.OnPropertyChangedWithValue<string>(value, "FileName");
				}
			}
		}

		// Token: 0x170008B8 RID: 2232
		// (get) Token: 0x06001975 RID: 6517 RVA: 0x0005C58A File Offset: 0x0005A78A
		// (set) Token: 0x06001976 RID: 6518 RVA: 0x0005C592 File Offset: 0x0005A792
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

		// Token: 0x170008B9 RID: 2233
		// (get) Token: 0x06001977 RID: 6519 RVA: 0x0005C5B5 File Offset: 0x0005A7B5
		// (set) Token: 0x06001978 RID: 6520 RVA: 0x0005C5BD File Offset: 0x0005A7BD
		[DataSourceProperty]
		public string VillagesText
		{
			get
			{
				return this._villagesText;
			}
			set
			{
				if (value != this._villagesText)
				{
					this._villagesText = value;
					base.OnPropertyChangedWithValue<string>(value, "VillagesText");
				}
			}
		}

		// Token: 0x170008BA RID: 2234
		// (get) Token: 0x06001979 RID: 6521 RVA: 0x0005C5E0 File Offset: 0x0005A7E0
		// (set) Token: 0x0600197A RID: 6522 RVA: 0x0005C5E8 File Offset: 0x0005A7E8
		[DataSourceProperty]
		public string NotablesText
		{
			get
			{
				return this._notablesText;
			}
			set
			{
				if (value != this._notablesText)
				{
					this._notablesText = value;
					base.OnPropertyChangedWithValue<string>(value, "NotablesText");
				}
			}
		}

		// Token: 0x170008BB RID: 2235
		// (get) Token: 0x0600197B RID: 6523 RVA: 0x0005C60B File Offset: 0x0005A80B
		// (set) Token: 0x0600197C RID: 6524 RVA: 0x0005C613 File Offset: 0x0005A813
		[DataSourceProperty]
		public string MembersText
		{
			get
			{
				return this._membersText;
			}
			set
			{
				if (value != this._membersText)
				{
					this._membersText = value;
					base.OnPropertyChangedWithValue<string>(value, "MembersText");
				}
			}
		}

		// Token: 0x170008BC RID: 2236
		// (get) Token: 0x0600197D RID: 6525 RVA: 0x0005C636 File Offset: 0x0005A836
		// (set) Token: 0x0600197E RID: 6526 RVA: 0x0005C63E File Offset: 0x0005A83E
		[DataSourceProperty]
		public bool IsFortification
		{
			get
			{
				return this._isFortification;
			}
			set
			{
				if (value != this._isFortification)
				{
					this._isFortification = value;
					base.OnPropertyChangedWithValue(value, "IsFortification");
				}
			}
		}

		// Token: 0x170008BD RID: 2237
		// (get) Token: 0x0600197F RID: 6527 RVA: 0x0005C65C File Offset: 0x0005A85C
		// (set) Token: 0x06001980 RID: 6528 RVA: 0x0005C664 File Offset: 0x0005A864
		[DataSourceProperty]
		public bool HasGovernor
		{
			get
			{
				return this._hasGovernor;
			}
			set
			{
				if (value != this._hasGovernor)
				{
					this._hasGovernor = value;
					base.OnPropertyChangedWithValue(value, "HasGovernor");
				}
			}
		}

		// Token: 0x170008BE RID: 2238
		// (get) Token: 0x06001981 RID: 6529 RVA: 0x0005C682 File Offset: 0x0005A882
		// (set) Token: 0x06001982 RID: 6530 RVA: 0x0005C68A File Offset: 0x0005A88A
		[DataSourceProperty]
		public bool HasNotables
		{
			get
			{
				return this._hasNotables;
			}
			set
			{
				if (value != this._hasNotables)
				{
					this._hasNotables = value;
					base.OnPropertyChangedWithValue(value, "HasNotables");
				}
			}
		}

		// Token: 0x170008BF RID: 2239
		// (get) Token: 0x06001983 RID: 6531 RVA: 0x0005C6A8 File Offset: 0x0005A8A8
		// (set) Token: 0x06001984 RID: 6532 RVA: 0x0005C6B0 File Offset: 0x0005A8B0
		[DataSourceProperty]
		public bool IsSendMembersEnabled
		{
			get
			{
				return this._isSendMembersEnabled;
			}
			set
			{
				if (value != this._isSendMembersEnabled)
				{
					this._isSendMembersEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsSendMembersEnabled");
				}
			}
		}

		// Token: 0x170008C0 RID: 2240
		// (get) Token: 0x06001985 RID: 6533 RVA: 0x0005C6CE File Offset: 0x0005A8CE
		// (set) Token: 0x06001986 RID: 6534 RVA: 0x0005C6D6 File Offset: 0x0005A8D6
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

		// Token: 0x170008C1 RID: 2241
		// (get) Token: 0x06001987 RID: 6535 RVA: 0x0005C6F4 File Offset: 0x0005A8F4
		// (set) Token: 0x06001988 RID: 6536 RVA: 0x0005C6FC File Offset: 0x0005A8FC
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

		// Token: 0x170008C2 RID: 2242
		// (get) Token: 0x06001989 RID: 6537 RVA: 0x0005C71F File Offset: 0x0005A91F
		// (set) Token: 0x0600198A RID: 6538 RVA: 0x0005C727 File Offset: 0x0005A927
		[DataSourceProperty]
		public MBBindingList<ClanSettlementItemVM> VillagesOwned
		{
			get
			{
				return this._villagesOwned;
			}
			set
			{
				if (value != this._villagesOwned)
				{
					this._villagesOwned = value;
					base.OnPropertyChangedWithValue<MBBindingList<ClanSettlementItemVM>>(value, "VillagesOwned");
				}
			}
		}

		// Token: 0x170008C3 RID: 2243
		// (get) Token: 0x0600198B RID: 6539 RVA: 0x0005C745 File Offset: 0x0005A945
		// (set) Token: 0x0600198C RID: 6540 RVA: 0x0005C74D File Offset: 0x0005A94D
		[DataSourceProperty]
		public MBBindingList<HeroVM> Notables
		{
			get
			{
				return this._notables;
			}
			set
			{
				if (value != this._notables)
				{
					this._notables = value;
					base.OnPropertyChangedWithValue<MBBindingList<HeroVM>>(value, "Notables");
				}
			}
		}

		// Token: 0x170008C4 RID: 2244
		// (get) Token: 0x0600198D RID: 6541 RVA: 0x0005C76B File Offset: 0x0005A96B
		// (set) Token: 0x0600198E RID: 6542 RVA: 0x0005C773 File Offset: 0x0005A973
		[DataSourceProperty]
		public MBBindingList<HeroVM> Members
		{
			get
			{
				return this._members;
			}
			set
			{
				if (value != this._members)
				{
					this._members = value;
					base.OnPropertyChangedWithValue<MBBindingList<HeroVM>>(value, "Members");
				}
			}
		}

		// Token: 0x170008C5 RID: 2245
		// (get) Token: 0x0600198F RID: 6543 RVA: 0x0005C791 File Offset: 0x0005A991
		// (set) Token: 0x06001990 RID: 6544 RVA: 0x0005C799 File Offset: 0x0005A999
		[DataSourceProperty]
		public HintViewModel SendMembersHint
		{
			get
			{
				return this._sendMembersHint;
			}
			set
			{
				if (value != this._sendMembersHint)
				{
					this._sendMembersHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "SendMembersHint");
				}
			}
		}

		// Token: 0x04000C0F RID: 3087
		private readonly Action<ClanSettlementItemVM> _onSelection;

		// Token: 0x04000C10 RID: 3088
		private readonly Action _onShowSendMembers;

		// Token: 0x04000C11 RID: 3089
		private readonly ITeleportationCampaignBehavior _teleportationBehavior;

		// Token: 0x04000C12 RID: 3090
		public readonly Settlement Settlement;

		// Token: 0x04000C13 RID: 3091
		private string _name;

		// Token: 0x04000C14 RID: 3092
		private HeroVM _governor;

		// Token: 0x04000C15 RID: 3093
		private string _fileName;

		// Token: 0x04000C16 RID: 3094
		private string _imageName;

		// Token: 0x04000C17 RID: 3095
		private string _villagesText;

		// Token: 0x04000C18 RID: 3096
		private string _notablesText;

		// Token: 0x04000C19 RID: 3097
		private string _membersText;

		// Token: 0x04000C1A RID: 3098
		private bool _isFortification;

		// Token: 0x04000C1B RID: 3099
		private bool _isSelected;

		// Token: 0x04000C1C RID: 3100
		private bool _hasGovernor;

		// Token: 0x04000C1D RID: 3101
		private bool _hasNotables;

		// Token: 0x04000C1E RID: 3102
		private bool _isSendMembersEnabled;

		// Token: 0x04000C1F RID: 3103
		private MBBindingList<SelectableFiefItemPropertyVM> _itemProperties;

		// Token: 0x04000C20 RID: 3104
		private MBBindingList<ClanSettlementItemVM> _villagesOwned;

		// Token: 0x04000C21 RID: 3105
		private MBBindingList<HeroVM> _notables;

		// Token: 0x04000C22 RID: 3106
		private MBBindingList<HeroVM> _members;

		// Token: 0x04000C23 RID: 3107
		private HintViewModel _sendMembersHint;
	}
}
