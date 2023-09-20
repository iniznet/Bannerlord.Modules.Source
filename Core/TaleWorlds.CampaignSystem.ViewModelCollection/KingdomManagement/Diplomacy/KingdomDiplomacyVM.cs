using System;
using System.Linq;
using TaleWorlds.CampaignSystem.BarterSystem.Barterables;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Diplomacy
{
	public class KingdomDiplomacyVM : KingdomCategoryVM
	{
		public KingdomDiplomacyVM(Action<KingdomDecision> forceDecision)
		{
			this._forceDecision = forceDecision;
			this._playerKingdom = Hero.MainHero.MapFaction as Kingdom;
			this.PlayerWars = new MBBindingList<KingdomWarItemVM>();
			this.PlayerTruces = new MBBindingList<KingdomTruceItemVM>();
			this.WarsSortController = new KingdomWarSortControllerVM(ref this._playerWars);
			this.ActionHint = new HintViewModel();
			this.ExecuteShowStatComparisons();
			this.RefreshValues();
			this.SetDefaultSelectedItem();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.BehaviorSelection = new SelectorVM<SelectorItemVM>(0, new Action<SelectorVM<SelectorItemVM>>(this.OnBehaviorSelectionChanged));
			this.BehaviorSelection.AddItem(new SelectorItemVM(GameTexts.FindText("str_kingdom_war_strategy_balanced", null), GameTexts.FindText("str_kingdom_war_strategy_balanced_desc", null)));
			this.BehaviorSelection.AddItem(new SelectorItemVM(GameTexts.FindText("str_kingdom_war_strategy_defensive", null), GameTexts.FindText("str_kingdom_war_strategy_defensive_desc", null)));
			this.BehaviorSelection.AddItem(new SelectorItemVM(GameTexts.FindText("str_kingdom_war_strategy_offensive", null), GameTexts.FindText("str_kingdom_war_strategy_offensive_desc", null)));
			this.RefreshDiplomacyList();
			Kingdom kingdom = Clan.PlayerClan.Kingdom;
			int num;
			if (kingdom == null)
			{
				num = 0;
			}
			else
			{
				num = kingdom.UnresolvedDecisions.Count((KingdomDecision d) => !d.ShouldBeCancelled());
			}
			base.NotificationCount = num;
			this.BehaviorSelectionTitle = GameTexts.FindText("str_kingdom_war_strategy", null).ToString();
			base.NoItemSelectedText = GameTexts.FindText("str_kingdom_no_war_selected", null).ToString();
			this.PlayerWarsText = GameTexts.FindText("str_kingdom_at_war", null).ToString();
			this.PlayerTrucesText = GameTexts.FindText("str_kingdom_at_peace", null).ToString();
			this.WarsText = GameTexts.FindText("str_diplomatic_group", null).ToString();
			this.ShowStatBarsHint = new HintViewModel(GameTexts.FindText("str_kingdom_war_show_comparison_bars", null), null);
			this.ShowWarLogsHint = new HintViewModel(GameTexts.FindText("str_kingdom_war_show_war_logs", null), null);
			this.PlayerWars.ApplyActionOnAllItems(delegate(KingdomWarItemVM x)
			{
				x.RefreshValues();
			});
			this.PlayerTruces.ApplyActionOnAllItems(delegate(KingdomTruceItemVM x)
			{
				x.RefreshValues();
			});
			KingdomDiplomacyItemVM currentSelectedDiplomacyItem = this.CurrentSelectedDiplomacyItem;
			if (currentSelectedDiplomacyItem == null)
			{
				return;
			}
			currentSelectedDiplomacyItem.RefreshValues();
		}

		public void RefreshDiplomacyList()
		{
			this.PlayerWars.Clear();
			this.PlayerTruces.Clear();
			foreach (StanceLink stanceLink in from x in this._playerKingdom.Stances
				where x.IsAtWar
				select x into w
				orderby w.Faction1.Name.ToString() + w.Faction2.Name.ToString()
				select w)
			{
				if (stanceLink.Faction1.IsKingdomFaction && stanceLink.Faction2.IsKingdomFaction)
				{
					this.PlayerWars.Add(new KingdomWarItemVM(stanceLink, new Action<KingdomWarItemVM>(this.OnDiplomacyItemSelection), new Action<KingdomWarItemVM>(this.OnDeclarePeace)));
				}
			}
			foreach (Kingdom kingdom in Kingdom.All)
			{
				if (kingdom != this._playerKingdom && !kingdom.IsEliminated && (FactionManager.IsAlliedWithFaction(kingdom, this._playerKingdom) || FactionManager.IsNeutralWithFaction(kingdom, this._playerKingdom)))
				{
					this.PlayerTruces.Add(new KingdomTruceItemVM(this._playerKingdom, kingdom, new Action<KingdomDiplomacyItemVM>(this.OnDiplomacyItemSelection), new Action<KingdomTruceItemVM>(this.OnDeclareWar)));
				}
			}
			GameTexts.SetVariable("STR", this.PlayerWars.Count);
			this.NumOfPlayerWarsText = GameTexts.FindText("str_STR_in_parentheses", null).ToString();
			GameTexts.SetVariable("STR", this.PlayerTruces.Count);
			this.NumOfPlayerTrucesText = GameTexts.FindText("str_STR_in_parentheses", null).ToString();
			this.SetDefaultSelectedItem();
		}

		public void SelectKingdom(Kingdom kingdom)
		{
			bool flag = false;
			foreach (KingdomWarItemVM kingdomWarItemVM in this.PlayerWars)
			{
				if (kingdomWarItemVM.Faction1 == kingdom || kingdomWarItemVM.Faction2 == kingdom)
				{
					this.OnSetCurrentDiplomacyItem(kingdomWarItemVM);
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				foreach (KingdomTruceItemVM kingdomTruceItemVM in this.PlayerTruces)
				{
					if (kingdomTruceItemVM.Faction1 == kingdom || kingdomTruceItemVM.Faction2 == kingdom)
					{
						this.OnSetCurrentDiplomacyItem(kingdomTruceItemVM);
						flag = true;
						break;
					}
				}
			}
		}

		private void OnSetCurrentDiplomacyItem(KingdomDiplomacyItemVM item)
		{
			if (item is KingdomWarItemVM)
			{
				this.OnSetWarItem(item as KingdomWarItemVM);
			}
			else if (item is KingdomTruceItemVM)
			{
				this.OnSetPeaceItem(item as KingdomTruceItemVM);
			}
			this.RefreshCurrentWarVisuals(item);
			this.UpdateBehaviorSelection();
		}

		private void OnSetWarItem(KingdomWarItemVM item)
		{
			this._currentItemsUnresolvedDecision = Clan.PlayerClan.Kingdom.UnresolvedDecisions.FirstOrDefault(delegate(KingdomDecision d)
			{
				MakePeaceKingdomDecision makePeaceKingdomDecision2;
				return (makePeaceKingdomDecision2 = d as MakePeaceKingdomDecision) != null && makePeaceKingdomDecision2.FactionToMakePeaceWith == item.Faction2 && !d.ShouldBeCancelled();
			});
			if (this._currentItemsUnresolvedDecision != null)
			{
				MakePeaceKingdomDecision makePeaceKingdomDecision = this._currentItemsUnresolvedDecision as MakePeaceKingdomDecision;
				this._dailyPeaceTributeToPay = ((makePeaceKingdomDecision != null) ? makePeaceKingdomDecision.DailyTributeToBePaid : 0);
				this.ActionName = GameTexts.FindText("str_resolve", null).ToString();
				this.ActionInfluenceCost = 0;
				TextObject textObject;
				this.IsActionEnabled = this.GetActionStatusForDiplomacyItemWithReason(item, true, out textObject);
				this.ActionHint.HintText = textObject;
				this.ProposeActionExplanationText = GameTexts.FindText("str_resolve_explanation", null).ToString();
				return;
			}
			this.ActionName = ((this._playerKingdom.Clans.Count > 1) ? GameTexts.FindText("str_policy_propose", null).ToString() : GameTexts.FindText("str_policy_enact", null).ToString());
			this.ActionInfluenceCost = Campaign.Current.Models.DiplomacyModel.GetInfluenceCostOfProposingPeace();
			PeaceBarterable peaceBarterable = new PeaceBarterable(this._playerKingdom.Leader, this._playerKingdom, item.Faction2, CampaignTime.Years(1f));
			int num = -peaceBarterable.GetValueForFaction(item.Faction2);
			if (item.Faction2 is Kingdom)
			{
				foreach (Clan clan in ((Kingdom)item.Faction2).Clans)
				{
					int num2 = -peaceBarterable.GetValueForFaction(clan);
					if (num2 > num)
					{
						num = num2;
					}
				}
			}
			int num3 = num;
			if (num3 > -5000 && num3 < 5000)
			{
				num3 = 0;
			}
			this._dailyPeaceTributeToPay = Campaign.Current.Models.DiplomacyModel.GetDailyTributeForValue(num3);
			this._dailyPeaceTributeToPay = 10 * (this._dailyPeaceTributeToPay / 10);
			TextObject textObject2;
			this.IsActionEnabled = this.GetActionStatusForDiplomacyItemWithReason(item, false, out textObject2);
			this.ActionHint.HintText = textObject2;
			TextObject textObject3 = ((this._dailyPeaceTributeToPay == 0) ? GameTexts.FindText("str_propose_peace_explanation", null) : ((this._dailyPeaceTributeToPay > 0) ? GameTexts.FindText("str_propose_peace_explanation_pay_tribute", null) : GameTexts.FindText("str_propose_peace_explanation_get_tribute", null)));
			this.ProposeActionExplanationText = textObject3.SetTextVariable("SUPPORT", this.CalculatePeaceSupport(item, this._dailyPeaceTributeToPay)).SetTextVariable("TRIBUTE", MathF.Abs(this._dailyPeaceTributeToPay)).ToString();
			Kingdom kingdom = Clan.PlayerClan.Kingdom;
			base.NotificationCount = ((kingdom != null) ? kingdom.UnresolvedDecisions.Count : 0);
		}

		private void OnSetPeaceItem(KingdomTruceItemVM item)
		{
			this._currentItemsUnresolvedDecision = Clan.PlayerClan.Kingdom.UnresolvedDecisions.FirstOrDefault(delegate(KingdomDecision d)
			{
				DeclareWarDecision declareWarDecision;
				return (declareWarDecision = d as DeclareWarDecision) != null && declareWarDecision.FactionToDeclareWarOn == item.Faction2 && !d.ShouldBeCancelled();
			});
			if (this._currentItemsUnresolvedDecision != null)
			{
				this.ActionName = GameTexts.FindText("str_resolve", null).ToString();
				this.ActionInfluenceCost = 0;
				TextObject textObject;
				this.IsActionEnabled = this.GetActionStatusForDiplomacyItemWithReason(item, true, out textObject);
				this.ActionHint.HintText = textObject;
				this.ProposeActionExplanationText = GameTexts.FindText("str_resolve_explanation", null).ToString();
				return;
			}
			this.ActionName = ((this._playerKingdom.Clans.Count > 1) ? GameTexts.FindText("str_policy_propose", null).ToString() : GameTexts.FindText("str_policy_enact", null).ToString());
			this.ActionInfluenceCost = Campaign.Current.Models.DiplomacyModel.GetInfluenceCostOfProposingWar(Clan.PlayerClan.Kingdom);
			TextObject textObject2;
			this.IsActionEnabled = this.GetActionStatusForDiplomacyItemWithReason(item, false, out textObject2);
			this.ActionHint.HintText = textObject2;
			this.ProposeActionExplanationText = GameTexts.FindText("str_propose_war_explanation", null).SetTextVariable("SUPPORT", KingdomDiplomacyVM.CalculateWarSupport(item.Faction2)).ToString();
		}

		private bool GetActionStatusForDiplomacyItemWithReason(KingdomDiplomacyItemVM item, bool isResolve, out TextObject disabledReason)
		{
			TextObject textObject;
			if (!CampaignUIHelper.GetMapScreenActionIsEnabledWithReason(out textObject))
			{
				disabledReason = textObject;
				return false;
			}
			if (!isResolve && Clan.PlayerClan.Influence < (float)this.ActionInfluenceCost)
			{
				disabledReason = GameTexts.FindText("str_warning_you_dont_have_enough_influence", null);
				return false;
			}
			if (Clan.PlayerClan.IsUnderMercenaryService)
			{
				disabledReason = GameTexts.FindText("str_cannot_propose_war_truce_while_mercenary", null);
				return false;
			}
			KingdomTruceItemVM kingdomTruceItemVM;
			TextObject textObject3;
			if ((kingdomTruceItemVM = item as KingdomTruceItemVM) != null)
			{
				TextObject textObject2;
				if (!Campaign.Current.Models.KingdomDecisionPermissionModel.IsWarDecisionAllowedBetweenKingdoms(kingdomTruceItemVM.Faction1 as Kingdom, kingdomTruceItemVM.Faction2 as Kingdom, out textObject2))
				{
					disabledReason = textObject2;
					return false;
				}
			}
			else if (item is KingdomWarItemVM && !Campaign.Current.Models.KingdomDecisionPermissionModel.IsPeaceDecisionAllowedBetweenKingdoms(item.Faction1 as Kingdom, item.Faction2 as Kingdom, out textObject3))
			{
				disabledReason = textObject3;
				return false;
			}
			disabledReason = TextObject.Empty;
			return true;
		}

		private void RefreshCurrentWarVisuals(KingdomDiplomacyItemVM item)
		{
			if (item != null)
			{
				if (this.CurrentSelectedDiplomacyItem != null)
				{
					this.CurrentSelectedDiplomacyItem.IsSelected = false;
				}
				this.CurrentSelectedDiplomacyItem = item;
				if (this.CurrentSelectedDiplomacyItem != null)
				{
					this.CurrentSelectedDiplomacyItem.IsSelected = true;
				}
			}
		}

		private void OnDiplomacyItemSelection(KingdomDiplomacyItemVM item)
		{
			if (this.CurrentSelectedDiplomacyItem != item)
			{
				if (this.CurrentSelectedDiplomacyItem != null)
				{
					this.CurrentSelectedDiplomacyItem.IsSelected = false;
				}
				this.CurrentSelectedDiplomacyItem = item;
				base.IsAcceptableItemSelected = item != null;
				this.OnSetCurrentDiplomacyItem(item);
			}
		}

		private void OnDeclareWar(KingdomTruceItemVM item)
		{
			if (this._currentItemsUnresolvedDecision != null)
			{
				this._forceDecision(this._currentItemsUnresolvedDecision);
				return;
			}
			DeclareWarDecision declareWarDecision = new DeclareWarDecision(Clan.PlayerClan, item.Faction2);
			Clan.PlayerClan.Kingdom.AddDecision(declareWarDecision, false);
			this._forceDecision(declareWarDecision);
		}

		private void OnDeclarePeace(KingdomWarItemVM item)
		{
			if (this._currentItemsUnresolvedDecision != null)
			{
				this._forceDecision(this._currentItemsUnresolvedDecision);
				return;
			}
			MakePeaceKingdomDecision makePeaceKingdomDecision = new MakePeaceKingdomDecision(Clan.PlayerClan, item.Faction2 as Kingdom, this._dailyPeaceTributeToPay, true);
			Clan.PlayerClan.Kingdom.AddDecision(makePeaceKingdomDecision, false);
			this._forceDecision(makePeaceKingdomDecision);
		}

		private void ExecuteAction()
		{
			if (this.CurrentSelectedDiplomacyItem != null)
			{
				if (this.CurrentSelectedDiplomacyItem is KingdomWarItemVM)
				{
					this.OnDeclarePeace(this.CurrentSelectedDiplomacyItem as KingdomWarItemVM);
					return;
				}
				if (this.CurrentSelectedDiplomacyItem is KingdomTruceItemVM)
				{
					this.OnDeclareWar(this.CurrentSelectedDiplomacyItem as KingdomTruceItemVM);
				}
			}
		}

		private void ExecuteShowWarLogs()
		{
			this.IsDisplayingWarLogs = true;
			this.IsDisplayingStatComparisons = false;
		}

		private void ExecuteShowStatComparisons()
		{
			this.IsDisplayingWarLogs = false;
			this.IsDisplayingStatComparisons = true;
		}

		private void SetDefaultSelectedItem()
		{
			KingdomDiplomacyItemVM kingdomDiplomacyItemVM = this.PlayerWars.FirstOrDefault<KingdomWarItemVM>();
			KingdomDiplomacyItemVM kingdomDiplomacyItemVM2 = this.PlayerTruces.FirstOrDefault<KingdomTruceItemVM>();
			this.OnDiplomacyItemSelection(kingdomDiplomacyItemVM ?? kingdomDiplomacyItemVM2);
		}

		private void UpdateBehaviorSelection()
		{
			if (Hero.MainHero.MapFaction.IsKingdomFaction && Hero.MainHero.MapFaction.Leader == Hero.MainHero && this.CurrentSelectedDiplomacyItem != null)
			{
				StanceLink stanceWith = Hero.MainHero.MapFaction.GetStanceWith(this.CurrentSelectedDiplomacyItem.Faction2);
				this.BehaviorSelection.SelectedIndex = stanceWith.BehaviorPriority;
			}
		}

		private void OnBehaviorSelectionChanged(SelectorVM<SelectorItemVM> s)
		{
			if (!this._isChangingDiplomacyItem && Hero.MainHero.MapFaction.IsKingdomFaction && Hero.MainHero.MapFaction.Leader == Hero.MainHero && this.CurrentSelectedDiplomacyItem != null)
			{
				Hero.MainHero.MapFaction.GetStanceWith(this.CurrentSelectedDiplomacyItem.Faction2).BehaviorPriority = s.SelectedIndex;
			}
		}

		[DataSourceProperty]
		public MBBindingList<KingdomWarItemVM> PlayerWars
		{
			get
			{
				return this._playerWars;
			}
			set
			{
				if (value != this._playerWars)
				{
					this._playerWars = value;
					base.OnPropertyChangedWithValue<MBBindingList<KingdomWarItemVM>>(value, "PlayerWars");
				}
			}
		}

		[DataSourceProperty]
		public int ActionInfluenceCost
		{
			get
			{
				return this._actionInfluenceCost;
			}
			set
			{
				if (value != this._actionInfluenceCost)
				{
					this._actionInfluenceCost = value;
					base.OnPropertyChangedWithValue(value, "ActionInfluenceCost");
				}
			}
		}

		[DataSourceProperty]
		public bool IsActionEnabled
		{
			get
			{
				return this._isActionEnabled;
			}
			set
			{
				if (value != this._isActionEnabled)
				{
					this._isActionEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsActionEnabled");
				}
			}
		}

		[DataSourceProperty]
		public bool IsDisplayingWarLogs
		{
			get
			{
				return this._isDisplayingWarLogs;
			}
			set
			{
				if (value != this._isDisplayingWarLogs)
				{
					this._isDisplayingWarLogs = value;
					base.OnPropertyChangedWithValue(value, "IsDisplayingWarLogs");
				}
			}
		}

		[DataSourceProperty]
		public bool IsDisplayingStatComparisons
		{
			get
			{
				return this._isDisplayingStatComparisons;
			}
			set
			{
				if (value != this._isDisplayingStatComparisons)
				{
					this._isDisplayingStatComparisons = value;
					base.OnPropertyChangedWithValue(value, "IsDisplayingStatComparisons");
				}
			}
		}

		[DataSourceProperty]
		public bool IsWar
		{
			get
			{
				return this._isWar;
			}
			set
			{
				if (value != this._isWar)
				{
					this._isWar = value;
					if (!value)
					{
						this.ExecuteShowStatComparisons();
					}
					base.OnPropertyChangedWithValue(value, "IsWar");
				}
			}
		}

		[DataSourceProperty]
		public string ActionName
		{
			get
			{
				return this._actionName;
			}
			set
			{
				if (value != this._actionName)
				{
					this._actionName = value;
					base.OnPropertyChangedWithValue<string>(value, "ActionName");
				}
			}
		}

		[DataSourceProperty]
		public string ProposeActionExplanationText
		{
			get
			{
				return this._proposeActionExplanationText;
			}
			set
			{
				if (value != this._proposeActionExplanationText)
				{
					this._proposeActionExplanationText = value;
					base.OnPropertyChangedWithValue<string>(value, "ProposeActionExplanationText");
				}
			}
		}

		[DataSourceProperty]
		public string BehaviorSelectionTitle
		{
			get
			{
				return this._behaviorSelectionTitle;
			}
			set
			{
				if (value != this._behaviorSelectionTitle)
				{
					this._behaviorSelectionTitle = value;
					base.OnPropertyChangedWithValue<string>(value, "BehaviorSelectionTitle");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<KingdomTruceItemVM> PlayerTruces
		{
			get
			{
				return this._playerTruces;
			}
			set
			{
				if (value != this._playerTruces)
				{
					this._playerTruces = value;
					base.OnPropertyChangedWithValue<MBBindingList<KingdomTruceItemVM>>(value, "PlayerTruces");
				}
			}
		}

		[DataSourceProperty]
		public KingdomDiplomacyItemVM CurrentSelectedDiplomacyItem
		{
			get
			{
				return this._currentSelectedItem;
			}
			set
			{
				if (value != this._currentSelectedItem)
				{
					this._isChangingDiplomacyItem = true;
					this._currentSelectedItem = value;
					this.IsWar = value is KingdomWarItemVM;
					base.OnPropertyChangedWithValue<KingdomDiplomacyItemVM>(value, "CurrentSelectedDiplomacyItem");
					this._isChangingDiplomacyItem = false;
				}
			}
		}

		[DataSourceProperty]
		public KingdomWarSortControllerVM WarsSortController
		{
			get
			{
				return this._warsSortController;
			}
			set
			{
				if (value != this._warsSortController)
				{
					this._warsSortController = value;
					base.OnPropertyChangedWithValue<KingdomWarSortControllerVM>(value, "WarsSortController");
				}
			}
		}

		[DataSourceProperty]
		public string PlayerWarsText
		{
			get
			{
				return this._playerWarsText;
			}
			set
			{
				if (value != this._playerWarsText)
				{
					this._playerWarsText = value;
					base.OnPropertyChangedWithValue<string>(value, "PlayerWarsText");
				}
			}
		}

		[DataSourceProperty]
		public string WarsText
		{
			get
			{
				return this._warsText;
			}
			set
			{
				if (value != this._warsText)
				{
					this._warsText = value;
					base.OnPropertyChangedWithValue<string>(value, "WarsText");
				}
			}
		}

		[DataSourceProperty]
		public string NumOfPlayerWarsText
		{
			get
			{
				return this._numOfPlayerWarsText;
			}
			set
			{
				if (value != this._numOfPlayerWarsText)
				{
					this._numOfPlayerWarsText = value;
					base.OnPropertyChangedWithValue<string>(value, "NumOfPlayerWarsText");
				}
			}
		}

		[DataSourceProperty]
		public string PlayerTrucesText
		{
			get
			{
				return this._otherWarsText;
			}
			set
			{
				if (value != this._otherWarsText)
				{
					this._otherWarsText = value;
					base.OnPropertyChangedWithValue<string>(value, "PlayerTrucesText");
				}
			}
		}

		[DataSourceProperty]
		public string NumOfPlayerTrucesText
		{
			get
			{
				return this._numOfOtherWarsText;
			}
			set
			{
				if (value != this._numOfOtherWarsText)
				{
					this._numOfOtherWarsText = value;
					base.OnPropertyChangedWithValue<string>(value, "NumOfPlayerTrucesText");
				}
			}
		}

		[DataSourceProperty]
		public SelectorVM<SelectorItemVM> BehaviorSelection
		{
			get
			{
				return this._behaviorSelection;
			}
			set
			{
				if (value != this._behaviorSelection)
				{
					this._behaviorSelection = value;
					base.OnPropertyChangedWithValue<SelectorVM<SelectorItemVM>>(value, "BehaviorSelection");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel ShowStatBarsHint
		{
			get
			{
				return this._showStatBarsHint;
			}
			set
			{
				if (value != this._showStatBarsHint)
				{
					this._showStatBarsHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "ShowStatBarsHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel ShowWarLogsHint
		{
			get
			{
				return this._showWarLogsHint;
			}
			set
			{
				if (value != this._showWarLogsHint)
				{
					this._showWarLogsHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "ShowWarLogsHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel ActionHint
		{
			get
			{
				return this._actionHint;
			}
			set
			{
				if (value != this._actionHint)
				{
					this._actionHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "ActionHint");
				}
			}
		}

		private static int CalculateWarSupport(IFaction faction)
		{
			return MathF.Round(new KingdomElection(new DeclareWarDecision(Clan.PlayerClan, faction)).GetLikelihoodForSponsor(Clan.PlayerClan) * 100f);
		}

		private int CalculatePeaceSupport(KingdomWarItemVM policy, int dailyTributeToBePaid)
		{
			return MathF.Round(new KingdomElection(new MakePeaceKingdomDecision(Clan.PlayerClan, policy.Faction2, dailyTributeToBePaid, true)).GetLikelihoodForSponsor(Clan.PlayerClan) * 100f);
		}

		private KingdomDecision _currentItemsUnresolvedDecision;

		private readonly Action<KingdomDecision> _forceDecision;

		private readonly Kingdom _playerKingdom;

		private int _dailyPeaceTributeToPay;

		private bool _isChangingDiplomacyItem;

		private MBBindingList<KingdomWarItemVM> _playerWars;

		private MBBindingList<KingdomTruceItemVM> _playerTruces;

		private KingdomWarSortControllerVM _warsSortController;

		private KingdomDiplomacyItemVM _currentSelectedItem;

		private SelectorVM<SelectorItemVM> _behaviorSelection;

		private HintViewModel _showStatBarsHint;

		private HintViewModel _showWarLogsHint;

		private HintViewModel _actionHint;

		private string _playerWarsText;

		private string _numOfPlayerWarsText;

		private string _otherWarsText;

		private string _numOfOtherWarsText;

		private string _warsText;

		private string _actionName;

		private string _proposeActionExplanationText;

		private string _behaviorSelectionTitle;

		private int _actionInfluenceCost;

		private bool _isActionEnabled;

		private bool _isDisplayingWarLogs;

		private bool _isDisplayingStatComparisons;

		private bool _isWar;
	}
}
