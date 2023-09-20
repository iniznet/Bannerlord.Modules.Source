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
	public class ClanSettlementItemVM : ViewModel
	{
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

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.VillagesText = GameTexts.FindText("str_villages", null).ToString();
			this.NotablesText = GameTexts.FindText("str_center_notables", null).ToString();
			this.MembersText = GameTexts.FindText("str_members", null).ToString();
			this.Name = this.Settlement.Name.ToString();
			this.UpdateProperties();
		}

		public void OnSettlementSelection()
		{
			this._onSelection(this);
		}

		public void ExecuteLink()
		{
			MBInformationManager.HideInformations();
			Campaign.Current.EncyclopediaManager.GoToLink(this.Settlement.EncyclopediaLink);
		}

		public void ExecuteCloseTooltip()
		{
			MBInformationManager.HideInformations();
		}

		public void ExecuteOpenTooltip()
		{
			InformationManager.ShowTooltip(typeof(Settlement), new object[] { this.Settlement, true });
		}

		public void ExecuteSendMembers()
		{
			Action onShowSendMembers = this._onShowSendMembers;
			if (onShowSendMembers == null)
			{
				return;
			}
			onShowSendMembers();
		}

		private void OnGovernorChanged(Hero oldHero, Hero newHero)
		{
			ChangeGovernorAction.Apply(this.Settlement.Town, newHero);
		}

		private bool IsGovernorAssignable(Hero oldHero, Hero newHero)
		{
			return newHero.IsActive && newHero.GovernorOf == null;
		}

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

		private bool IsSettlementSlotAssignable(Hero oldHero, Hero newHero)
		{
			return (oldHero == null || !oldHero.IsHumanPlayerCharacter) && !newHero.IsHumanPlayerCharacter && newHero.IsActive && (newHero.PartyBelongedTo == null || newHero.PartyBelongedTo.LeaderHero != newHero) && newHero.PartyBelongedToAsPrisoner == null;
		}

		private void ExecuteOpenSettlementPage()
		{
			Campaign.Current.EncyclopediaManager.GoToLink(this.Settlement.EncyclopediaLink);
		}

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

		private readonly Action<ClanSettlementItemVM> _onSelection;

		private readonly Action _onShowSendMembers;

		private readonly ITeleportationCampaignBehavior _teleportationBehavior;

		public readonly Settlement Settlement;

		private string _name;

		private HeroVM _governor;

		private string _fileName;

		private string _imageName;

		private string _villagesText;

		private string _notablesText;

		private string _membersText;

		private bool _isFortification;

		private bool _isSelected;

		private bool _hasGovernor;

		private bool _hasNotables;

		private bool _isSendMembersEnabled;

		private MBBindingList<SelectableFiefItemPropertyVM> _itemProperties;

		private MBBindingList<ClanSettlementItemVM> _villagesOwned;

		private MBBindingList<HeroVM> _notables;

		private MBBindingList<HeroVM> _members;

		private HintViewModel _sendMembersHint;
	}
}
