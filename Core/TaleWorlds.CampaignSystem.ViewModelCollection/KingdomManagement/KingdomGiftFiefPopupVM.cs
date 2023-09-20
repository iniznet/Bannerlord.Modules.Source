using System;
using Helpers;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Clans;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement
{
	public class KingdomGiftFiefPopupVM : ViewModel
	{
		public KingdomGiftFiefPopupVM(Action onSettlementGranted)
		{
			this._clans = new MBBindingList<KingdomClanItemVM>();
			this._onSettlementGranted = onSettlementGranted;
			this.ClanSortController = new KingdomClanSortControllerVM(ref this._clans);
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.TitleText = new TextObject("{=rOKAvjtT}Gift Settlement", null).ToString();
			this.GiftText = GameTexts.FindText("str_gift", null).ToString();
			this.CancelText = GameTexts.FindText("str_cancel", null).ToString();
			this.NameText = GameTexts.FindText("str_scoreboard_header", "name").ToString();
			this.InfluenceText = GameTexts.FindText("str_influence", null).ToString();
			this.FiefsText = GameTexts.FindText("str_fiefs", null).ToString();
			this.MembersText = GameTexts.FindText("str_members", null).ToString();
			this.BannerText = GameTexts.FindText("str_banner", null).ToString();
			this.TypeText = GameTexts.FindText("str_sort_by_type_label", null).ToString();
		}

		private void SetCurrentSelectedClan(KingdomClanItemVM clan)
		{
			if (clan != this.CurrentSelectedClan)
			{
				if (this.CurrentSelectedClan != null)
				{
					this.CurrentSelectedClan.IsSelected = false;
				}
				this.CurrentSelectedClan = clan;
				this.CurrentSelectedClan.IsSelected = true;
				this.IsAnyClanSelected = true;
			}
		}

		private void RefreshClanList()
		{
			this.Clans.Clear();
			foreach (Clan clan in Clan.PlayerClan.Kingdom.Clans)
			{
				if (FactionHelper.CanClanBeGrantedFief(clan))
				{
					this.Clans.Add(new KingdomClanItemVM(clan, new Action<KingdomClanItemVM>(this.SetCurrentSelectedClan)));
				}
			}
			if (this.Clans.Count > 0)
			{
				this.SetCurrentSelectedClan(this.Clans[0]);
			}
			if (this.ClanSortController != null)
			{
				this.ClanSortController.SortByCurrentState();
			}
		}

		public void OpenWith(Settlement settlement)
		{
			this._settlementToGive = settlement;
			this.RefreshClanList();
			this.IsOpen = true;
		}

		public void Close()
		{
			this._settlementToGive = null;
			this.IsOpen = false;
		}

		private void ExecuteGiftSettlement()
		{
			if (this._settlementToGive != null && this.CurrentSelectedClan != null)
			{
				Campaign.Current.KingdomManager.GiftSettlementOwnership(this._settlementToGive, this.CurrentSelectedClan.Clan);
				this.Close();
				this._onSettlementGranted();
			}
		}

		private void ExecuteClose()
		{
			this.Close();
		}

		[DataSourceProperty]
		public bool IsAnyClanSelected
		{
			get
			{
				return this._isAnyClanSelected;
			}
			set
			{
				if (value != this._isAnyClanSelected)
				{
					this._isAnyClanSelected = value;
					base.OnPropertyChangedWithValue(value, "IsAnyClanSelected");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<KingdomClanItemVM> Clans
		{
			get
			{
				return this._clans;
			}
			set
			{
				if (value != this._clans)
				{
					this._clans = value;
					base.OnPropertyChangedWithValue<MBBindingList<KingdomClanItemVM>>(value, "Clans");
				}
			}
		}

		[DataSourceProperty]
		public KingdomClanItemVM CurrentSelectedClan
		{
			get
			{
				return this._currentSelectedClan;
			}
			set
			{
				if (value != this._currentSelectedClan)
				{
					this._currentSelectedClan = value;
					base.OnPropertyChangedWithValue<KingdomClanItemVM>(value, "CurrentSelectedClan");
				}
			}
		}

		[DataSourceProperty]
		public KingdomClanSortControllerVM ClanSortController
		{
			get
			{
				return this._clanSortController;
			}
			set
			{
				if (value != this._clanSortController)
				{
					this._clanSortController = value;
					base.OnPropertyChangedWithValue<KingdomClanSortControllerVM>(value, "ClanSortController");
				}
			}
		}

		[DataSourceProperty]
		public bool IsOpen
		{
			get
			{
				return this._isOpen;
			}
			set
			{
				if (value != this._isOpen)
				{
					this._isOpen = value;
					base.OnPropertyChangedWithValue(value, "IsOpen");
				}
			}
		}

		[DataSourceProperty]
		public string TitleText
		{
			get
			{
				return this._titleText;
			}
			set
			{
				if (value != this._titleText)
				{
					this._titleText = value;
					base.OnPropertyChangedWithValue<string>(value, "TitleText");
				}
			}
		}

		[DataSourceProperty]
		public string GiftText
		{
			get
			{
				return this._giftText;
			}
			set
			{
				if (value != this._giftText)
				{
					this._giftText = value;
					base.OnPropertyChangedWithValue<string>(value, "GiftText");
				}
			}
		}

		[DataSourceProperty]
		public string CancelText
		{
			get
			{
				return this._cancelText;
			}
			set
			{
				if (value != this._cancelText)
				{
					this._cancelText = value;
					base.OnPropertyChangedWithValue<string>(value, "CancelText");
				}
			}
		}

		[DataSourceProperty]
		public string BannerText
		{
			get
			{
				return this._bannerText;
			}
			set
			{
				if (value != this._bannerText)
				{
					this._bannerText = value;
					base.OnPropertyChangedWithValue<string>(value, "BannerText");
				}
			}
		}

		[DataSourceProperty]
		public string TypeText
		{
			get
			{
				return this._typeText;
			}
			set
			{
				if (value != this._typeText)
				{
					this._typeText = value;
					base.OnPropertyChangedWithValue<string>(value, "TypeText");
				}
			}
		}

		[DataSourceProperty]
		public string NameText
		{
			get
			{
				return this._nameText;
			}
			set
			{
				if (value != this._nameText)
				{
					this._nameText = value;
					base.OnPropertyChangedWithValue<string>(value, "NameText");
				}
			}
		}

		[DataSourceProperty]
		public string InfluenceText
		{
			get
			{
				return this._influenceText;
			}
			set
			{
				if (value != this._influenceText)
				{
					this._influenceText = value;
					base.OnPropertyChangedWithValue<string>(value, "InfluenceText");
				}
			}
		}

		[DataSourceProperty]
		public string FiefsText
		{
			get
			{
				return this._fiefsText;
			}
			set
			{
				if (value != this._fiefsText)
				{
					this._fiefsText = value;
					base.OnPropertyChangedWithValue<string>(value, "FiefsText");
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

		private Settlement _settlementToGive;

		private Action _onSettlementGranted;

		private bool _isAnyClanSelected;

		private MBBindingList<KingdomClanItemVM> _clans;

		private KingdomClanItemVM _currentSelectedClan;

		private KingdomClanSortControllerVM _clanSortController;

		private bool _isOpen;

		private string _titleText;

		private string _giftText;

		private string _cancelText;

		private string _bannerText;

		private string _nameText;

		private string _influenceText;

		private string _membersText;

		private string _fiefsText;

		private string _typeText;
	}
}
