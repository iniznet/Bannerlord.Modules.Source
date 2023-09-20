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
	// Token: 0x0200005D RID: 93
	public class KingdomDiplomacyVM : KingdomCategoryVM
	{
		// Token: 0x060007EB RID: 2027 RVA: 0x00021CA8 File Offset: 0x0001FEA8
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

		// Token: 0x060007EC RID: 2028 RVA: 0x00021D1C File Offset: 0x0001FF1C
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

		// Token: 0x060007ED RID: 2029 RVA: 0x00021F00 File Offset: 0x00020100
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

		// Token: 0x060007EE RID: 2030 RVA: 0x000220E0 File Offset: 0x000202E0
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

		// Token: 0x060007EF RID: 2031 RVA: 0x000221A0 File Offset: 0x000203A0
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

		// Token: 0x060007F0 RID: 2032 RVA: 0x000221DC File Offset: 0x000203DC
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

		// Token: 0x060007F1 RID: 2033 RVA: 0x000224A0 File Offset: 0x000206A0
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

		// Token: 0x060007F2 RID: 2034 RVA: 0x000225F0 File Offset: 0x000207F0
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

		// Token: 0x060007F3 RID: 2035 RVA: 0x000226CB File Offset: 0x000208CB
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

		// Token: 0x060007F4 RID: 2036 RVA: 0x000226FF File Offset: 0x000208FF
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

		// Token: 0x060007F5 RID: 2037 RVA: 0x00022738 File Offset: 0x00020938
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

		// Token: 0x060007F6 RID: 2038 RVA: 0x00022790 File Offset: 0x00020990
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

		// Token: 0x060007F7 RID: 2039 RVA: 0x000227F4 File Offset: 0x000209F4
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

		// Token: 0x060007F8 RID: 2040 RVA: 0x00022846 File Offset: 0x00020A46
		private void ExecuteShowWarLogs()
		{
			this.IsDisplayingWarLogs = true;
			this.IsDisplayingStatComparisons = false;
		}

		// Token: 0x060007F9 RID: 2041 RVA: 0x00022856 File Offset: 0x00020A56
		private void ExecuteShowStatComparisons()
		{
			this.IsDisplayingWarLogs = false;
			this.IsDisplayingStatComparisons = true;
		}

		// Token: 0x060007FA RID: 2042 RVA: 0x00022868 File Offset: 0x00020A68
		private void SetDefaultSelectedItem()
		{
			KingdomDiplomacyItemVM kingdomDiplomacyItemVM = this.PlayerWars.FirstOrDefault<KingdomWarItemVM>();
			KingdomDiplomacyItemVM kingdomDiplomacyItemVM2 = this.PlayerTruces.FirstOrDefault<KingdomTruceItemVM>();
			this.OnDiplomacyItemSelection(kingdomDiplomacyItemVM ?? kingdomDiplomacyItemVM2);
		}

		// Token: 0x060007FB RID: 2043 RVA: 0x0002289C File Offset: 0x00020A9C
		private void UpdateBehaviorSelection()
		{
			if (Hero.MainHero.MapFaction.IsKingdomFaction && Hero.MainHero.MapFaction.Leader == Hero.MainHero && this.CurrentSelectedDiplomacyItem != null)
			{
				StanceLink stanceWith = Hero.MainHero.MapFaction.GetStanceWith(this.CurrentSelectedDiplomacyItem.Faction2);
				this.BehaviorSelection.SelectedIndex = stanceWith.BehaviorPriority;
			}
		}

		// Token: 0x060007FC RID: 2044 RVA: 0x00022904 File Offset: 0x00020B04
		private void OnBehaviorSelectionChanged(SelectorVM<SelectorItemVM> s)
		{
			if (!this._isChangingDiplomacyItem && Hero.MainHero.MapFaction.IsKingdomFaction && Hero.MainHero.MapFaction.Leader == Hero.MainHero && this.CurrentSelectedDiplomacyItem != null)
			{
				Hero.MainHero.MapFaction.GetStanceWith(this.CurrentSelectedDiplomacyItem.Faction2).BehaviorPriority = s.SelectedIndex;
			}
		}

		// Token: 0x1700026D RID: 621
		// (get) Token: 0x060007FD RID: 2045 RVA: 0x0002296D File Offset: 0x00020B6D
		// (set) Token: 0x060007FE RID: 2046 RVA: 0x00022975 File Offset: 0x00020B75
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

		// Token: 0x1700026E RID: 622
		// (get) Token: 0x060007FF RID: 2047 RVA: 0x00022993 File Offset: 0x00020B93
		// (set) Token: 0x06000800 RID: 2048 RVA: 0x0002299B File Offset: 0x00020B9B
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

		// Token: 0x1700026F RID: 623
		// (get) Token: 0x06000801 RID: 2049 RVA: 0x000229B9 File Offset: 0x00020BB9
		// (set) Token: 0x06000802 RID: 2050 RVA: 0x000229C1 File Offset: 0x00020BC1
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

		// Token: 0x17000270 RID: 624
		// (get) Token: 0x06000803 RID: 2051 RVA: 0x000229DF File Offset: 0x00020BDF
		// (set) Token: 0x06000804 RID: 2052 RVA: 0x000229E7 File Offset: 0x00020BE7
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

		// Token: 0x17000271 RID: 625
		// (get) Token: 0x06000805 RID: 2053 RVA: 0x00022A05 File Offset: 0x00020C05
		// (set) Token: 0x06000806 RID: 2054 RVA: 0x00022A0D File Offset: 0x00020C0D
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

		// Token: 0x17000272 RID: 626
		// (get) Token: 0x06000807 RID: 2055 RVA: 0x00022A2B File Offset: 0x00020C2B
		// (set) Token: 0x06000808 RID: 2056 RVA: 0x00022A33 File Offset: 0x00020C33
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

		// Token: 0x17000273 RID: 627
		// (get) Token: 0x06000809 RID: 2057 RVA: 0x00022A5A File Offset: 0x00020C5A
		// (set) Token: 0x0600080A RID: 2058 RVA: 0x00022A62 File Offset: 0x00020C62
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

		// Token: 0x17000274 RID: 628
		// (get) Token: 0x0600080B RID: 2059 RVA: 0x00022A85 File Offset: 0x00020C85
		// (set) Token: 0x0600080C RID: 2060 RVA: 0x00022A8D File Offset: 0x00020C8D
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

		// Token: 0x17000275 RID: 629
		// (get) Token: 0x0600080D RID: 2061 RVA: 0x00022AB0 File Offset: 0x00020CB0
		// (set) Token: 0x0600080E RID: 2062 RVA: 0x00022AB8 File Offset: 0x00020CB8
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

		// Token: 0x17000276 RID: 630
		// (get) Token: 0x0600080F RID: 2063 RVA: 0x00022ADB File Offset: 0x00020CDB
		// (set) Token: 0x06000810 RID: 2064 RVA: 0x00022AE3 File Offset: 0x00020CE3
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

		// Token: 0x17000277 RID: 631
		// (get) Token: 0x06000811 RID: 2065 RVA: 0x00022B01 File Offset: 0x00020D01
		// (set) Token: 0x06000812 RID: 2066 RVA: 0x00022B09 File Offset: 0x00020D09
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

		// Token: 0x17000278 RID: 632
		// (get) Token: 0x06000813 RID: 2067 RVA: 0x00022B44 File Offset: 0x00020D44
		// (set) Token: 0x06000814 RID: 2068 RVA: 0x00022B4C File Offset: 0x00020D4C
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

		// Token: 0x17000279 RID: 633
		// (get) Token: 0x06000815 RID: 2069 RVA: 0x00022B6A File Offset: 0x00020D6A
		// (set) Token: 0x06000816 RID: 2070 RVA: 0x00022B72 File Offset: 0x00020D72
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

		// Token: 0x1700027A RID: 634
		// (get) Token: 0x06000817 RID: 2071 RVA: 0x00022B95 File Offset: 0x00020D95
		// (set) Token: 0x06000818 RID: 2072 RVA: 0x00022B9D File Offset: 0x00020D9D
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

		// Token: 0x1700027B RID: 635
		// (get) Token: 0x06000819 RID: 2073 RVA: 0x00022BC0 File Offset: 0x00020DC0
		// (set) Token: 0x0600081A RID: 2074 RVA: 0x00022BC8 File Offset: 0x00020DC8
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

		// Token: 0x1700027C RID: 636
		// (get) Token: 0x0600081B RID: 2075 RVA: 0x00022BEB File Offset: 0x00020DEB
		// (set) Token: 0x0600081C RID: 2076 RVA: 0x00022BF3 File Offset: 0x00020DF3
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

		// Token: 0x1700027D RID: 637
		// (get) Token: 0x0600081D RID: 2077 RVA: 0x00022C16 File Offset: 0x00020E16
		// (set) Token: 0x0600081E RID: 2078 RVA: 0x00022C1E File Offset: 0x00020E1E
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

		// Token: 0x1700027E RID: 638
		// (get) Token: 0x0600081F RID: 2079 RVA: 0x00022C41 File Offset: 0x00020E41
		// (set) Token: 0x06000820 RID: 2080 RVA: 0x00022C49 File Offset: 0x00020E49
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

		// Token: 0x1700027F RID: 639
		// (get) Token: 0x06000821 RID: 2081 RVA: 0x00022C67 File Offset: 0x00020E67
		// (set) Token: 0x06000822 RID: 2082 RVA: 0x00022C6F File Offset: 0x00020E6F
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

		// Token: 0x17000280 RID: 640
		// (get) Token: 0x06000823 RID: 2083 RVA: 0x00022C8D File Offset: 0x00020E8D
		// (set) Token: 0x06000824 RID: 2084 RVA: 0x00022C95 File Offset: 0x00020E95
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

		// Token: 0x17000281 RID: 641
		// (get) Token: 0x06000825 RID: 2085 RVA: 0x00022CB3 File Offset: 0x00020EB3
		// (set) Token: 0x06000826 RID: 2086 RVA: 0x00022CBB File Offset: 0x00020EBB
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

		// Token: 0x06000827 RID: 2087 RVA: 0x00022CD9 File Offset: 0x00020ED9
		private static int CalculateWarSupport(IFaction faction)
		{
			return MathF.Round(new KingdomElection(new DeclareWarDecision(Clan.PlayerClan, faction)).GetLikelihoodForSponsor(Clan.PlayerClan) * 100f);
		}

		// Token: 0x06000828 RID: 2088 RVA: 0x00022D00 File Offset: 0x00020F00
		private int CalculatePeaceSupport(KingdomWarItemVM policy, int dailyTributeToBePaid)
		{
			return MathF.Round(new KingdomElection(new MakePeaceKingdomDecision(Clan.PlayerClan, policy.Faction2, dailyTributeToBePaid, true)).GetLikelihoodForSponsor(Clan.PlayerClan) * 100f);
		}

		// Token: 0x0400038A RID: 906
		private KingdomDecision _currentItemsUnresolvedDecision;

		// Token: 0x0400038B RID: 907
		private readonly Action<KingdomDecision> _forceDecision;

		// Token: 0x0400038C RID: 908
		private readonly Kingdom _playerKingdom;

		// Token: 0x0400038D RID: 909
		private int _dailyPeaceTributeToPay;

		// Token: 0x0400038E RID: 910
		private bool _isChangingDiplomacyItem;

		// Token: 0x0400038F RID: 911
		private MBBindingList<KingdomWarItemVM> _playerWars;

		// Token: 0x04000390 RID: 912
		private MBBindingList<KingdomTruceItemVM> _playerTruces;

		// Token: 0x04000391 RID: 913
		private KingdomWarSortControllerVM _warsSortController;

		// Token: 0x04000392 RID: 914
		private KingdomDiplomacyItemVM _currentSelectedItem;

		// Token: 0x04000393 RID: 915
		private SelectorVM<SelectorItemVM> _behaviorSelection;

		// Token: 0x04000394 RID: 916
		private HintViewModel _showStatBarsHint;

		// Token: 0x04000395 RID: 917
		private HintViewModel _showWarLogsHint;

		// Token: 0x04000396 RID: 918
		private HintViewModel _actionHint;

		// Token: 0x04000397 RID: 919
		private string _playerWarsText;

		// Token: 0x04000398 RID: 920
		private string _numOfPlayerWarsText;

		// Token: 0x04000399 RID: 921
		private string _otherWarsText;

		// Token: 0x0400039A RID: 922
		private string _numOfOtherWarsText;

		// Token: 0x0400039B RID: 923
		private string _warsText;

		// Token: 0x0400039C RID: 924
		private string _actionName;

		// Token: 0x0400039D RID: 925
		private string _proposeActionExplanationText;

		// Token: 0x0400039E RID: 926
		private string _behaviorSelectionTitle;

		// Token: 0x0400039F RID: 927
		private int _actionInfluenceCost;

		// Token: 0x040003A0 RID: 928
		private bool _isActionEnabled;

		// Token: 0x040003A1 RID: 929
		private bool _isDisplayingWarLogs;

		// Token: 0x040003A2 RID: 930
		private bool _isDisplayingStatComparisons;

		// Token: 0x040003A3 RID: 931
		private bool _isWar;
	}
}
