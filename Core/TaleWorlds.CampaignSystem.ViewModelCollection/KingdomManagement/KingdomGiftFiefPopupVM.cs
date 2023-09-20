using System;
using Helpers;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Clans;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement
{
	// Token: 0x02000053 RID: 83
	public class KingdomGiftFiefPopupVM : ViewModel
	{
		// Token: 0x06000680 RID: 1664 RVA: 0x0001DAB9 File Offset: 0x0001BCB9
		public KingdomGiftFiefPopupVM(Action onSettlementGranted)
		{
			this._clans = new MBBindingList<KingdomClanItemVM>();
			this._onSettlementGranted = onSettlementGranted;
			this.ClanSortController = new KingdomClanSortControllerVM(ref this._clans);
			this.RefreshValues();
		}

		// Token: 0x06000681 RID: 1665 RVA: 0x0001DAEC File Offset: 0x0001BCEC
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

		// Token: 0x06000682 RID: 1666 RVA: 0x0001DBC9 File Offset: 0x0001BDC9
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

		// Token: 0x06000683 RID: 1667 RVA: 0x0001DC04 File Offset: 0x0001BE04
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

		// Token: 0x06000684 RID: 1668 RVA: 0x0001DCBC File Offset: 0x0001BEBC
		public void OpenWith(Settlement settlement)
		{
			this._settlementToGive = settlement;
			this.RefreshClanList();
			this.IsOpen = true;
		}

		// Token: 0x06000685 RID: 1669 RVA: 0x0001DCD2 File Offset: 0x0001BED2
		public void Close()
		{
			this._settlementToGive = null;
			this.IsOpen = false;
		}

		// Token: 0x06000686 RID: 1670 RVA: 0x0001DCE4 File Offset: 0x0001BEE4
		private void ExecuteGiftSettlement()
		{
			if (this._settlementToGive != null && this.CurrentSelectedClan != null)
			{
				Campaign.Current.KingdomManager.GiftSettlementOwnership(this._settlementToGive, this.CurrentSelectedClan.Clan);
				this.Close();
				this._onSettlementGranted();
			}
		}

		// Token: 0x06000687 RID: 1671 RVA: 0x0001DD32 File Offset: 0x0001BF32
		private void ExecuteClose()
		{
			this.Close();
		}

		// Token: 0x170001E5 RID: 485
		// (get) Token: 0x06000688 RID: 1672 RVA: 0x0001DD3A File Offset: 0x0001BF3A
		// (set) Token: 0x06000689 RID: 1673 RVA: 0x0001DD42 File Offset: 0x0001BF42
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

		// Token: 0x170001E6 RID: 486
		// (get) Token: 0x0600068A RID: 1674 RVA: 0x0001DD60 File Offset: 0x0001BF60
		// (set) Token: 0x0600068B RID: 1675 RVA: 0x0001DD68 File Offset: 0x0001BF68
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

		// Token: 0x170001E7 RID: 487
		// (get) Token: 0x0600068C RID: 1676 RVA: 0x0001DD86 File Offset: 0x0001BF86
		// (set) Token: 0x0600068D RID: 1677 RVA: 0x0001DD8E File Offset: 0x0001BF8E
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

		// Token: 0x170001E8 RID: 488
		// (get) Token: 0x0600068E RID: 1678 RVA: 0x0001DDAC File Offset: 0x0001BFAC
		// (set) Token: 0x0600068F RID: 1679 RVA: 0x0001DDB4 File Offset: 0x0001BFB4
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

		// Token: 0x170001E9 RID: 489
		// (get) Token: 0x06000690 RID: 1680 RVA: 0x0001DDD2 File Offset: 0x0001BFD2
		// (set) Token: 0x06000691 RID: 1681 RVA: 0x0001DDDA File Offset: 0x0001BFDA
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

		// Token: 0x170001EA RID: 490
		// (get) Token: 0x06000692 RID: 1682 RVA: 0x0001DDF8 File Offset: 0x0001BFF8
		// (set) Token: 0x06000693 RID: 1683 RVA: 0x0001DE00 File Offset: 0x0001C000
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

		// Token: 0x170001EB RID: 491
		// (get) Token: 0x06000694 RID: 1684 RVA: 0x0001DE23 File Offset: 0x0001C023
		// (set) Token: 0x06000695 RID: 1685 RVA: 0x0001DE2B File Offset: 0x0001C02B
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

		// Token: 0x170001EC RID: 492
		// (get) Token: 0x06000696 RID: 1686 RVA: 0x0001DE4E File Offset: 0x0001C04E
		// (set) Token: 0x06000697 RID: 1687 RVA: 0x0001DE56 File Offset: 0x0001C056
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

		// Token: 0x170001ED RID: 493
		// (get) Token: 0x06000698 RID: 1688 RVA: 0x0001DE79 File Offset: 0x0001C079
		// (set) Token: 0x06000699 RID: 1689 RVA: 0x0001DE81 File Offset: 0x0001C081
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

		// Token: 0x170001EE RID: 494
		// (get) Token: 0x0600069A RID: 1690 RVA: 0x0001DEA4 File Offset: 0x0001C0A4
		// (set) Token: 0x0600069B RID: 1691 RVA: 0x0001DEAC File Offset: 0x0001C0AC
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

		// Token: 0x170001EF RID: 495
		// (get) Token: 0x0600069C RID: 1692 RVA: 0x0001DECF File Offset: 0x0001C0CF
		// (set) Token: 0x0600069D RID: 1693 RVA: 0x0001DED7 File Offset: 0x0001C0D7
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

		// Token: 0x170001F0 RID: 496
		// (get) Token: 0x0600069E RID: 1694 RVA: 0x0001DEFA File Offset: 0x0001C0FA
		// (set) Token: 0x0600069F RID: 1695 RVA: 0x0001DF02 File Offset: 0x0001C102
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

		// Token: 0x170001F1 RID: 497
		// (get) Token: 0x060006A0 RID: 1696 RVA: 0x0001DF25 File Offset: 0x0001C125
		// (set) Token: 0x060006A1 RID: 1697 RVA: 0x0001DF2D File Offset: 0x0001C12D
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

		// Token: 0x170001F2 RID: 498
		// (get) Token: 0x060006A2 RID: 1698 RVA: 0x0001DF50 File Offset: 0x0001C150
		// (set) Token: 0x060006A3 RID: 1699 RVA: 0x0001DF58 File Offset: 0x0001C158
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

		// Token: 0x040002E0 RID: 736
		private Settlement _settlementToGive;

		// Token: 0x040002E1 RID: 737
		private Action _onSettlementGranted;

		// Token: 0x040002E2 RID: 738
		private bool _isAnyClanSelected;

		// Token: 0x040002E3 RID: 739
		private MBBindingList<KingdomClanItemVM> _clans;

		// Token: 0x040002E4 RID: 740
		private KingdomClanItemVM _currentSelectedClan;

		// Token: 0x040002E5 RID: 741
		private KingdomClanSortControllerVM _clanSortController;

		// Token: 0x040002E6 RID: 742
		private bool _isOpen;

		// Token: 0x040002E7 RID: 743
		private string _titleText;

		// Token: 0x040002E8 RID: 744
		private string _giftText;

		// Token: 0x040002E9 RID: 745
		private string _cancelText;

		// Token: 0x040002EA RID: 746
		private string _bannerText;

		// Token: 0x040002EB RID: 747
		private string _nameText;

		// Token: 0x040002EC RID: 748
		private string _influenceText;

		// Token: 0x040002ED RID: 749
		private string _membersText;

		// Token: 0x040002EE RID: 750
		private string _fiefsText;

		// Token: 0x040002EF RID: 751
		private string _typeText;
	}
}
