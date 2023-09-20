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
	public class ArmyManagementItemVM : ViewModel
	{
		public float DistInTime { get; }

		public float _distance { get; }

		public Clan Clan { get; }

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

		public void ExecuteAction()
		{
			if (this.IsInCart)
			{
				this.OnRemove();
				return;
			}
			this.OnAddToCart();
		}

		private void OnRemove()
		{
			if (!this.IsMainHero)
			{
				this._onRemove(this);
				this.UpdateEligibility();
			}
		}

		private void OnAddToCart()
		{
			this.UpdateEligibility();
			if (this.IsEligible)
			{
				this._onAddToCart(this);
			}
			this.UpdateEligibility();
		}

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

		public void ExecuteBeginHint()
		{
			if (!this.IsEligible)
			{
				MBInformationManager.ShowHint(this._eligibilityReason.ToString());
				return;
			}
			InformationManager.ShowTooltip(typeof(MobileParty), new object[] { this.Party, true, true });
		}

		public void ExecuteEndHint()
		{
			MBInformationManager.HideInformations();
		}

		public void ExecuteOpenEncyclopedia()
		{
			MobileParty party = this.Party;
			if (((party != null) ? party.LeaderHero : null) != null)
			{
				Campaign.Current.EncyclopediaManager.GoToLink(this.Party.LeaderHero.EncyclopediaLink);
			}
		}

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

		private readonly Action<ArmyManagementItemVM> _onAddToCart;

		private readonly Action<ArmyManagementItemVM> _onRemove;

		private readonly Action<ArmyManagementItemVM> _onFocus;

		public readonly MobileParty Party;

		private const float _minimumPartySizeScoreNeeded = 0.4f;

		public bool CanJoinBackWithoutCost;

		private TextObject _eligibilityReason;

		private InputKeyItemVM _removeInputKey;

		private ImageIdentifierVM _clanBanner;

		private ImageIdentifierVM _lordFace;

		private string _nameText;

		private string _inArmyText;

		private string _leaderNameText;

		private int _relation = -102;

		private int _strength = -1;

		private string _distanceText;

		private int _cost = -1;

		private bool _isEligible;

		private bool _isMainHero;

		private bool _isInCart;

		private bool _isAlreadyWithPlayer;

		private bool _isTransferDisabled;

		private bool _isFocused;
	}
}
