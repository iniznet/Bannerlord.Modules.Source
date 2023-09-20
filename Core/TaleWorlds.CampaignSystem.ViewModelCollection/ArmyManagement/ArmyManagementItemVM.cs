using System;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.CampaignSystem.ViewModelCollection.Input;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ArmyManagement
{
	// Token: 0x02000132 RID: 306
	public class ArmyManagementItemVM : ViewModel
	{
		// Token: 0x17000A15 RID: 2581
		// (get) Token: 0x06001D6C RID: 7532 RVA: 0x0006912B File Offset: 0x0006732B
		public float DistInTime { get; }

		// Token: 0x17000A16 RID: 2582
		// (get) Token: 0x06001D6D RID: 7533 RVA: 0x00069133 File Offset: 0x00067333
		public float _distance { get; }

		// Token: 0x17000A17 RID: 2583
		// (get) Token: 0x06001D6E RID: 7534 RVA: 0x0006913B File Offset: 0x0006733B
		public Clan Clan { get; }

		// Token: 0x06001D6F RID: 7535 RVA: 0x00069144 File Offset: 0x00067344
		public ArmyManagementItemVM(Action<ArmyManagementItemVM> onAddToCart, Action<ArmyManagementItemVM> onRemove, Action<ArmyManagementItemVM> onFocus, MobileParty mobileParty)
		{
			ArmyManagementCalculationModel armyManagementCalculationModel = Campaign.Current.Models.ArmyManagementCalculationModel;
			this._onAddToCart = onAddToCart;
			this._onRemove = onRemove;
			this._onFocus = onFocus;
			this.Party = mobileParty;
			this._eligibilityReason = TextObject.Empty;
			this.ClanBanner = new ImageIdentifierVM(BannerCode.CreateFrom(mobileParty.LeaderHero.ClanBanner), true);
			CharacterCode characterCode = CampaignUIHelper.GetCharacterCode(mobileParty.LeaderHero.CharacterObject, false);
			this.LordFace = new ImageIdentifierVM(characterCode);
			this.Relation = armyManagementCalculationModel.GetPartyRelation(mobileParty.LeaderHero);
			this.Strength = this.Party.MemberRoster.TotalManCount;
			this._distance = Campaign.Current.Models.MapDistanceModel.GetDistance(this.Party, MobileParty.MainParty);
			this.DistInTime = (float)MathF.Ceiling(this._distance / this.Party.Speed);
			this.Clan = mobileParty.LeaderHero.Clan;
			this.IsMainHero = mobileParty.IsMainParty;
			this.UpdateEligibility();
			this.Cost = armyManagementCalculationModel.CalculatePartyInfluenceCost(MobileParty.MainParty, mobileParty);
			this.IsTransferDisabled = this.IsMainHero || PlayerSiege.PlayerSiegeEvent != null;
			this.RefreshValues();
		}

		// Token: 0x06001D70 RID: 7536 RVA: 0x000692A4 File Offset: 0x000674A4
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.InArmyText = GameTexts.FindText("str_in_army", null).ToString();
			this.LeaderNameText = this.Party.LeaderHero.Name.ToString();
			this.NameText = this.Party.Name.ToString();
			if (!this.Party.IsMainParty)
			{
				this.DistanceText = (((int)this._distance < 5) ? GameTexts.FindText("str_nearby", null).ToString() : CampaignUIHelper.GetPartyDistanceByTimeText((float)((int)this._distance), this.Party.Speed));
			}
		}

		// Token: 0x06001D71 RID: 7537 RVA: 0x00069345 File Offset: 0x00067545
		public void ExecuteAction()
		{
			if (this.IsInCart)
			{
				this.OnRemove();
				return;
			}
			this.OnAddToCart();
		}

		// Token: 0x06001D72 RID: 7538 RVA: 0x0006935C File Offset: 0x0006755C
		private void OnRemove()
		{
			if (!this.IsMainHero)
			{
				this._onRemove(this);
				this.UpdateEligibility();
			}
		}

		// Token: 0x06001D73 RID: 7539 RVA: 0x00069378 File Offset: 0x00067578
		private void OnAddToCart()
		{
			this.UpdateEligibility();
			if (this.IsEligible)
			{
				this._onAddToCart(this);
			}
			this.UpdateEligibility();
		}

		// Token: 0x06001D74 RID: 7540 RVA: 0x0006939A File Offset: 0x0006759A
		public void ExecuteSetFocused()
		{
			this.IsFocused = true;
			Action<ArmyManagementItemVM> onFocus = this._onFocus;
			if (onFocus == null)
			{
				return;
			}
			onFocus(this);
		}

		// Token: 0x06001D75 RID: 7541 RVA: 0x000693B4 File Offset: 0x000675B4
		public void ExecuteSetUnfocused()
		{
			this.IsFocused = false;
			Action<ArmyManagementItemVM> onFocus = this._onFocus;
			if (onFocus == null)
			{
				return;
			}
			onFocus(null);
		}

		// Token: 0x06001D76 RID: 7542 RVA: 0x000693D0 File Offset: 0x000675D0
		public void UpdateEligibility()
		{
			GameModels models = Campaign.Current.Models;
			ArmyManagementCalculationModel armyManagementCalculationModel = ((models != null) ? models.ArmyManagementCalculationModel : null);
			float num = ((armyManagementCalculationModel != null) ? armyManagementCalculationModel.GetPartySizeScore(this.Party) : 0f);
			IDisbandPartyCampaignBehavior behavior = Campaign.Current.CampaignBehaviorManager.GetBehavior<IDisbandPartyCampaignBehavior>();
			bool flag = false;
			this._eligibilityReason = TextObject.Empty;
			if (!this.CanJoinBackWithoutCost)
			{
				if (PlayerSiege.PlayerSiegeEvent != null)
				{
					this._eligibilityReason = GameTexts.FindText("str_action_disabled_reason_siege", null);
				}
				else if (this.Party == null)
				{
					this._eligibilityReason = new TextObject("{=f6vTzVar}Does not have a mobile party.", null);
				}
				else
				{
					Hero leaderHero = this.Party.LeaderHero;
					IFaction mapFaction = Hero.MainHero.MapFaction;
					if (leaderHero == ((mapFaction != null) ? mapFaction.Leader : null))
					{
						this._eligibilityReason = new TextObject("{=ipLqVv1f}You cannot invite the ruler's party to your army.", null);
					}
					else
					{
						if (this.Party.Army != null)
						{
							Army army = this.Party.Army;
							MobileParty partyBelongedTo = Hero.MainHero.PartyBelongedTo;
							if (army != ((partyBelongedTo != null) ? partyBelongedTo.Army : null))
							{
								this._eligibilityReason = new TextObject("{=aROohsat}Already in another army.", null);
								goto IL_217;
							}
						}
						if (this.Party.Army != null)
						{
							Army army2 = this.Party.Army;
							MobileParty partyBelongedTo2 = Hero.MainHero.PartyBelongedTo;
							if (army2 == ((partyBelongedTo2 != null) ? partyBelongedTo2.Army : null))
							{
								this._eligibilityReason = new TextObject("{=Vq8yavES}Already in army.", null);
								goto IL_217;
							}
						}
						if (this.Party.MapEvent != null || this.Party.SiegeEvent != null)
						{
							this._eligibilityReason = new TextObject("{=pkbUiKFJ}Currently fighting an enemy.", null);
						}
						else if (num <= 0.4f)
						{
							this._eligibilityReason = new TextObject("{=SVJlOYCB}Party has less men than 40% of it's party size limit.", null);
						}
						else if (this.IsInCart)
						{
							this._eligibilityReason = new TextObject("{=idRXFzQ6}Already added to the army.", null);
						}
						else if (this.Party.IsDisbanding || (behavior != null && behavior.IsPartyWaitingForDisband(this.Party)))
						{
							this._eligibilityReason = new TextObject("{=tFGM0yav}This party is disbanding.", null);
						}
						else if (armyManagementCalculationModel != null && !armyManagementCalculationModel.CheckPartyEligibility(this.Party))
						{
							this._eligibilityReason = new TextObject("{=nuK4Afnr}Party is not eligible to join the army.", null);
						}
						else
						{
							flag = true;
						}
					}
				}
			}
			else
			{
				flag = true;
			}
			IL_217:
			this.IsEligible = flag;
		}

		// Token: 0x06001D77 RID: 7543 RVA: 0x000695FC File Offset: 0x000677FC
		public void ExecuteBeginHint()
		{
			if (!this.IsEligible)
			{
				MBInformationManager.ShowHint(this._eligibilityReason.ToString());
				return;
			}
			InformationManager.ShowTooltip(typeof(MobileParty), new object[] { this.Party, true, true });
		}

		// Token: 0x06001D78 RID: 7544 RVA: 0x00069652 File Offset: 0x00067852
		public void ExecuteEndHint()
		{
			MBInformationManager.HideInformations();
		}

		// Token: 0x06001D79 RID: 7545 RVA: 0x00069659 File Offset: 0x00067859
		public void ExecuteOpenEncyclopedia()
		{
			MobileParty party = this.Party;
			if (((party != null) ? party.LeaderHero : null) != null)
			{
				Campaign.Current.EncyclopediaManager.GoToLink(this.Party.LeaderHero.EncyclopediaLink);
			}
		}

		// Token: 0x17000A18 RID: 2584
		// (get) Token: 0x06001D7A RID: 7546 RVA: 0x0006968E File Offset: 0x0006788E
		// (set) Token: 0x06001D7B RID: 7547 RVA: 0x00069696 File Offset: 0x00067896
		[DataSourceProperty]
		public InputKeyItemVM RemoveInputKey
		{
			get
			{
				return this._removeInputKey;
			}
			set
			{
				if (value != this._removeInputKey)
				{
					this._removeInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "RemoveInputKey");
				}
			}
		}

		// Token: 0x17000A19 RID: 2585
		// (get) Token: 0x06001D7C RID: 7548 RVA: 0x000696B4 File Offset: 0x000678B4
		// (set) Token: 0x06001D7D RID: 7549 RVA: 0x000696BC File Offset: 0x000678BC
		[DataSourceProperty]
		public bool IsEligible
		{
			get
			{
				return this._isEligible;
			}
			set
			{
				if (value != this._isEligible)
				{
					this._isEligible = value;
					base.OnPropertyChangedWithValue(value, "IsEligible");
				}
			}
		}

		// Token: 0x17000A1A RID: 2586
		// (get) Token: 0x06001D7E RID: 7550 RVA: 0x000696DA File Offset: 0x000678DA
		// (set) Token: 0x06001D7F RID: 7551 RVA: 0x000696E2 File Offset: 0x000678E2
		[DataSourceProperty]
		public bool IsInCart
		{
			get
			{
				return this._isInCart;
			}
			set
			{
				if (value != this._isInCart)
				{
					this._isInCart = value;
					base.OnPropertyChangedWithValue(value, "IsInCart");
				}
			}
		}

		// Token: 0x17000A1B RID: 2587
		// (get) Token: 0x06001D80 RID: 7552 RVA: 0x00069700 File Offset: 0x00067900
		// (set) Token: 0x06001D81 RID: 7553 RVA: 0x00069708 File Offset: 0x00067908
		[DataSourceProperty]
		public bool IsMainHero
		{
			get
			{
				return this._isMainHero;
			}
			set
			{
				if (value != this._isMainHero)
				{
					this._isMainHero = value;
					base.OnPropertyChangedWithValue(value, "IsMainHero");
				}
			}
		}

		// Token: 0x17000A1C RID: 2588
		// (get) Token: 0x06001D82 RID: 7554 RVA: 0x00069726 File Offset: 0x00067926
		// (set) Token: 0x06001D83 RID: 7555 RVA: 0x0006972E File Offset: 0x0006792E
		[DataSourceProperty]
		public int Strength
		{
			get
			{
				return this._strength;
			}
			set
			{
				if (value != this._strength)
				{
					this._strength = value;
					base.OnPropertyChangedWithValue(value, "Strength");
				}
			}
		}

		// Token: 0x17000A1D RID: 2589
		// (get) Token: 0x06001D84 RID: 7556 RVA: 0x0006974C File Offset: 0x0006794C
		// (set) Token: 0x06001D85 RID: 7557 RVA: 0x00069754 File Offset: 0x00067954
		[DataSourceProperty]
		public string DistanceText
		{
			get
			{
				return this._distanceText;
			}
			set
			{
				if (value != this._distanceText)
				{
					this._distanceText = value;
					base.OnPropertyChangedWithValue<string>(value, "DistanceText");
				}
			}
		}

		// Token: 0x17000A1E RID: 2590
		// (get) Token: 0x06001D86 RID: 7558 RVA: 0x00069777 File Offset: 0x00067977
		// (set) Token: 0x06001D87 RID: 7559 RVA: 0x0006977F File Offset: 0x0006797F
		[DataSourceProperty]
		public string InArmyText
		{
			get
			{
				return this._inArmyText;
			}
			set
			{
				if (value != this._inArmyText)
				{
					this._inArmyText = value;
					base.OnPropertyChangedWithValue<string>(value, "InArmyText");
				}
			}
		}

		// Token: 0x17000A1F RID: 2591
		// (get) Token: 0x06001D88 RID: 7560 RVA: 0x000697A2 File Offset: 0x000679A2
		// (set) Token: 0x06001D89 RID: 7561 RVA: 0x000697AA File Offset: 0x000679AA
		[DataSourceProperty]
		public int Cost
		{
			get
			{
				return this._cost;
			}
			set
			{
				if (value != this._cost)
				{
					this._cost = value;
					base.OnPropertyChangedWithValue(value, "Cost");
				}
			}
		}

		// Token: 0x17000A20 RID: 2592
		// (get) Token: 0x06001D8A RID: 7562 RVA: 0x000697C8 File Offset: 0x000679C8
		// (set) Token: 0x06001D8B RID: 7563 RVA: 0x000697D0 File Offset: 0x000679D0
		[DataSourceProperty]
		public int Relation
		{
			get
			{
				return this._relation;
			}
			set
			{
				if (value != this._relation)
				{
					this._relation = value;
					base.OnPropertyChangedWithValue(value, "Relation");
				}
			}
		}

		// Token: 0x17000A21 RID: 2593
		// (get) Token: 0x06001D8C RID: 7564 RVA: 0x000697EE File Offset: 0x000679EE
		// (set) Token: 0x06001D8D RID: 7565 RVA: 0x000697F6 File Offset: 0x000679F6
		[DataSourceProperty]
		public ImageIdentifierVM ClanBanner
		{
			get
			{
				return this._clanBanner;
			}
			set
			{
				if (value != this._clanBanner)
				{
					this._clanBanner = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "ClanBanner");
				}
			}
		}

		// Token: 0x17000A22 RID: 2594
		// (get) Token: 0x06001D8E RID: 7566 RVA: 0x00069814 File Offset: 0x00067A14
		// (set) Token: 0x06001D8F RID: 7567 RVA: 0x0006981C File Offset: 0x00067A1C
		[DataSourceProperty]
		public ImageIdentifierVM LordFace
		{
			get
			{
				return this._lordFace;
			}
			set
			{
				if (value != this._lordFace)
				{
					this._lordFace = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "LordFace");
				}
			}
		}

		// Token: 0x17000A23 RID: 2595
		// (get) Token: 0x06001D90 RID: 7568 RVA: 0x0006983A File Offset: 0x00067A3A
		// (set) Token: 0x06001D91 RID: 7569 RVA: 0x00069842 File Offset: 0x00067A42
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

		// Token: 0x17000A24 RID: 2596
		// (get) Token: 0x06001D92 RID: 7570 RVA: 0x00069865 File Offset: 0x00067A65
		// (set) Token: 0x06001D93 RID: 7571 RVA: 0x0006986D File Offset: 0x00067A6D
		[DataSourceProperty]
		public bool IsAlreadyWithPlayer
		{
			get
			{
				return this._isAlreadyWithPlayer;
			}
			set
			{
				if (value != this._isAlreadyWithPlayer)
				{
					this._isAlreadyWithPlayer = value;
					base.OnPropertyChangedWithValue(value, "IsAlreadyWithPlayer");
				}
			}
		}

		// Token: 0x17000A25 RID: 2597
		// (get) Token: 0x06001D94 RID: 7572 RVA: 0x0006988B File Offset: 0x00067A8B
		// (set) Token: 0x06001D95 RID: 7573 RVA: 0x00069893 File Offset: 0x00067A93
		[DataSourceProperty]
		public bool IsTransferDisabled
		{
			get
			{
				return this._isTransferDisabled;
			}
			set
			{
				if (value != this._isTransferDisabled)
				{
					this._isTransferDisabled = value;
					base.OnPropertyChangedWithValue(value, "IsTransferDisabled");
				}
			}
		}

		// Token: 0x17000A26 RID: 2598
		// (get) Token: 0x06001D96 RID: 7574 RVA: 0x000698B1 File Offset: 0x00067AB1
		// (set) Token: 0x06001D97 RID: 7575 RVA: 0x000698B9 File Offset: 0x00067AB9
		[DataSourceProperty]
		public string LeaderNameText
		{
			get
			{
				return this._leaderNameText;
			}
			set
			{
				if (value != this._leaderNameText)
				{
					this._leaderNameText = value;
					base.OnPropertyChangedWithValue<string>(value, "LeaderNameText");
				}
			}
		}

		// Token: 0x17000A27 RID: 2599
		// (get) Token: 0x06001D98 RID: 7576 RVA: 0x000698DC File Offset: 0x00067ADC
		// (set) Token: 0x06001D99 RID: 7577 RVA: 0x000698E4 File Offset: 0x00067AE4
		[DataSourceProperty]
		public bool IsFocused
		{
			get
			{
				return this._isFocused;
			}
			set
			{
				if (value != this._isFocused)
				{
					this._isFocused = value;
					base.OnPropertyChangedWithValue(value, "IsFocused");
				}
			}
		}

		// Token: 0x04000DDB RID: 3547
		private readonly Action<ArmyManagementItemVM> _onAddToCart;

		// Token: 0x04000DDC RID: 3548
		private readonly Action<ArmyManagementItemVM> _onRemove;

		// Token: 0x04000DDD RID: 3549
		private readonly Action<ArmyManagementItemVM> _onFocus;

		// Token: 0x04000DDE RID: 3550
		public readonly MobileParty Party;

		// Token: 0x04000DDF RID: 3551
		private const float _minimumPartySizeScoreNeeded = 0.4f;

		// Token: 0x04000DE0 RID: 3552
		public bool CanJoinBackWithoutCost;

		// Token: 0x04000DE1 RID: 3553
		private TextObject _eligibilityReason;

		// Token: 0x04000DE2 RID: 3554
		private InputKeyItemVM _removeInputKey;

		// Token: 0x04000DE3 RID: 3555
		private ImageIdentifierVM _clanBanner;

		// Token: 0x04000DE4 RID: 3556
		private ImageIdentifierVM _lordFace;

		// Token: 0x04000DE5 RID: 3557
		private string _nameText;

		// Token: 0x04000DE6 RID: 3558
		private string _inArmyText;

		// Token: 0x04000DE7 RID: 3559
		private string _leaderNameText;

		// Token: 0x04000DE8 RID: 3560
		private int _relation = -102;

		// Token: 0x04000DE9 RID: 3561
		private int _strength = -1;

		// Token: 0x04000DEA RID: 3562
		private string _distanceText;

		// Token: 0x04000DEB RID: 3563
		private int _cost = -1;

		// Token: 0x04000DEC RID: 3564
		private bool _isEligible;

		// Token: 0x04000DED RID: 3565
		private bool _isMainHero;

		// Token: 0x04000DEE RID: 3566
		private bool _isInCart;

		// Token: 0x04000DEF RID: 3567
		private bool _isAlreadyWithPlayer;

		// Token: 0x04000DF0 RID: 3568
		private bool _isTransferDisabled;

		// Token: 0x04000DF1 RID: 3569
		private bool _isFocused;
	}
}
