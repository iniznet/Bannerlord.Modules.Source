using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.ViewModelCollection.Input;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Decisions.ItemTypes;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Decisions
{
	// Token: 0x02000066 RID: 102
	public class KingdomDecisionsVM : ViewModel
	{
		// Token: 0x170002B7 RID: 695
		// (get) Token: 0x060008BF RID: 2239 RVA: 0x00024609 File Offset: 0x00022809
		public bool IsCurrentDecisionActive
		{
			get
			{
				DecisionItemBaseVM currentDecision = this.CurrentDecision;
				return currentDecision != null && currentDecision.IsActive;
			}
		}

		// Token: 0x170002B8 RID: 696
		// (get) Token: 0x060008C0 RID: 2240 RVA: 0x0002461C File Offset: 0x0002281C
		// (set) Token: 0x060008C1 RID: 2241 RVA: 0x00024624 File Offset: 0x00022824
		private bool _shouldCheckForDecision { get; set; } = true;

		// Token: 0x060008C2 RID: 2242 RVA: 0x00024630 File Offset: 0x00022830
		public KingdomDecisionsVM(Action refreshKingdomManagement)
		{
			this._refreshKingdomManagement = refreshKingdomManagement;
			this._examinedDecisionsSinceInit = new List<KingdomDecision>();
			this._examinedDecisionsSinceInit.AddRange(Clan.PlayerClan.Kingdom.UnresolvedDecisions.Where((KingdomDecision d) => d.ShouldBeCancelled()));
			this.IsRefreshed = true;
			this.RefreshValues();
		}

		// Token: 0x060008C3 RID: 2243 RVA: 0x000246A7 File Offset: 0x000228A7
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.TitleText = GameTexts.FindText("str_kingdom_decisions", null).ToString();
			DecisionItemBaseVM currentDecision = this.CurrentDecision;
			if (currentDecision == null)
			{
				return;
			}
			currentDecision.RefreshValues();
		}

		// Token: 0x060008C4 RID: 2244 RVA: 0x000246D8 File Offset: 0x000228D8
		public void OnFrameTick()
		{
			this.IsActive = this.IsCurrentDecisionActive;
			if (this._shouldCheckForDecision)
			{
				if (this.CurrentDecision != null)
				{
					DecisionItemBaseVM currentDecision = this.CurrentDecision;
					if (currentDecision == null || currentDecision.IsActive)
					{
						return;
					}
				}
				if (Clan.PlayerClan.Kingdom.UnresolvedDecisions.Except(this._examinedDecisionsSinceInit).Any<KingdomDecision>())
				{
					this.QueryForNextDecision();
				}
			}
		}

		// Token: 0x060008C5 RID: 2245 RVA: 0x00024740 File Offset: 0x00022940
		public void QueryForNextDecision()
		{
			KingdomDecision curDecision = Clan.PlayerClan.Kingdom.UnresolvedDecisions.Except(this._examinedDecisionsSinceInit).FirstOrDefault<KingdomDecision>();
			KingdomDecision curDecision2 = curDecision;
			if (curDecision2 != null && !curDecision2.ShouldBeCancelled())
			{
				this._shouldCheckForDecision = false;
				this._examinedDecisionsSinceInit.Add(curDecision);
				if (curDecision.IsPlayerParticipant)
				{
					TextObject generalTitle = new KingdomElection(curDecision).GetGeneralTitle();
					GameTexts.SetVariable("DECISION_NAME", generalTitle.ToString());
					string text = (curDecision.NeedsPlayerResolution ? GameTexts.FindText("str_you_need_to_resolve_decision", null).ToString() : GameTexts.FindText("str_do_you_want_to_resolve_decision", null).ToString());
					if (!curDecision.NeedsPlayerResolution && curDecision.TriggerTime.IsFuture)
					{
						GameTexts.SetVariable("HOUR", ((int)curDecision.TriggerTime.RemainingHoursFromNow).ToString());
						GameTexts.SetVariable("newline", "\n");
						GameTexts.SetVariable("STR1", text);
						GameTexts.SetVariable("STR2", GameTexts.FindText("str_decision_will_be_resolved_in_hours", null));
						text = GameTexts.FindText("str_string_newline_string", null).ToString();
					}
					this._queryData = new InquiryData(GameTexts.FindText("str_decision", null).ToString(), text, true, !curDecision.NeedsPlayerResolution, GameTexts.FindText("str_ok", null).ToString(), GameTexts.FindText("str_cancel", null).ToString(), delegate
					{
						this.RefreshWith(curDecision);
					}, delegate
					{
						this._shouldCheckForDecision = true;
					}, "", 0f, null, null, null);
					this._shouldCheckForDecision = false;
					InformationManager.ShowInquiry(this._queryData, false, false);
					return;
				}
			}
			else
			{
				this._shouldCheckForDecision = false;
				this._queryData = null;
			}
		}

		// Token: 0x060008C6 RID: 2246 RVA: 0x00024930 File Offset: 0x00022B30
		public void RefreshWith(KingdomDecision decision)
		{
			if (decision.IsSingleClanDecision())
			{
				KingdomElection kingdomElection = new KingdomElection(decision);
				kingdomElection.StartElection();
				kingdomElection.ApplySelection();
				InformationManager.ShowInquiry(new InquiryData(GameTexts.FindText("str_decision_outcome", null).ToString(), kingdomElection.GetChosenOutcomeText().ToString(), true, false, GameTexts.FindText("str_ok", null).ToString(), "", delegate
				{
					this.OnSingleDecisionOver();
				}, null, "", 0f, null, null, null), false, false);
				return;
			}
			this._shouldCheckForDecision = false;
			this.CurrentDecision = this.GetDecisionItemBasedOnType(decision);
			this.CurrentDecision.SetDoneInputKey(this.DoneInputKey);
		}

		// Token: 0x060008C7 RID: 2247 RVA: 0x000249D6 File Offset: 0x00022BD6
		private void OnSingleDecisionOver()
		{
			this._refreshKingdomManagement();
			this._shouldCheckForDecision = true;
		}

		// Token: 0x060008C8 RID: 2248 RVA: 0x000249EA File Offset: 0x00022BEA
		private void OnDecisionOver()
		{
			this._refreshKingdomManagement();
			DecisionItemBaseVM currentDecision = this.CurrentDecision;
			if (currentDecision != null)
			{
				currentDecision.OnFinalize();
			}
			this.CurrentDecision = null;
			this._shouldCheckForDecision = true;
		}

		// Token: 0x060008C9 RID: 2249 RVA: 0x00024A18 File Offset: 0x00022C18
		private DecisionItemBaseVM GetDecisionItemBasedOnType(KingdomDecision decision)
		{
			SettlementClaimantDecision settlementClaimantDecision;
			if ((settlementClaimantDecision = decision as SettlementClaimantDecision) != null)
			{
				return new SettlementDecisionItemVM(settlementClaimantDecision.Settlement, decision, new Action(this.OnDecisionOver));
			}
			SettlementClaimantPreliminaryDecision settlementClaimantPreliminaryDecision;
			if ((settlementClaimantPreliminaryDecision = decision as SettlementClaimantPreliminaryDecision) != null)
			{
				return new SettlementDecisionItemVM(settlementClaimantPreliminaryDecision.Settlement, decision, new Action(this.OnDecisionOver));
			}
			ExpelClanFromKingdomDecision expelClanFromKingdomDecision;
			if ((expelClanFromKingdomDecision = decision as ExpelClanFromKingdomDecision) != null)
			{
				return new ExpelClanDecisionItemVM(expelClanFromKingdomDecision, new Action(this.OnDecisionOver));
			}
			KingdomPolicyDecision kingdomPolicyDecision;
			if ((kingdomPolicyDecision = decision as KingdomPolicyDecision) != null)
			{
				return new PolicyDecisionItemVM(kingdomPolicyDecision, new Action(this.OnDecisionOver));
			}
			DeclareWarDecision declareWarDecision;
			if ((declareWarDecision = decision as DeclareWarDecision) != null)
			{
				return new DeclareWarDecisionItemVM(declareWarDecision, new Action(this.OnDecisionOver));
			}
			MakePeaceKingdomDecision makePeaceKingdomDecision;
			if ((makePeaceKingdomDecision = decision as MakePeaceKingdomDecision) != null)
			{
				return new MakePeaceDecisionItemVM(makePeaceKingdomDecision, new Action(this.OnDecisionOver));
			}
			KingSelectionKingdomDecision kingSelectionKingdomDecision;
			if ((kingSelectionKingdomDecision = decision as KingSelectionKingdomDecision) != null)
			{
				return new KingSelectionDecisionItemVM(kingSelectionKingdomDecision, new Action(this.OnDecisionOver));
			}
			Debug.FailedAssert("No defined decision type for this decision! This shouldn't happen", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem.ViewModelCollection\\KingdomManagement\\Decisions\\KingdomDecisionsVM.cs", "GetDecisionItemBasedOnType", 169);
			return new DecisionItemBaseVM(decision, new Action(this.OnDecisionOver));
		}

		// Token: 0x060008CA RID: 2250 RVA: 0x00024B2D File Offset: 0x00022D2D
		public override void OnFinalize()
		{
			base.OnFinalize();
			this.DoneInputKey.OnFinalize();
			DecisionItemBaseVM currentDecision = this.CurrentDecision;
			if (currentDecision != null)
			{
				currentDecision.OnFinalize();
			}
			this.CurrentDecision = null;
		}

		// Token: 0x060008CB RID: 2251 RVA: 0x00024B58 File Offset: 0x00022D58
		public void SetDoneInputKey(HotKey hotKey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x170002B9 RID: 697
		// (get) Token: 0x060008CC RID: 2252 RVA: 0x00024B67 File Offset: 0x00022D67
		// (set) Token: 0x060008CD RID: 2253 RVA: 0x00024B6F File Offset: 0x00022D6F
		[DataSourceProperty]
		public InputKeyItemVM DoneInputKey
		{
			get
			{
				return this._doneInputKey;
			}
			set
			{
				if (value != this._doneInputKey)
				{
					this._doneInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "DoneInputKey");
				}
			}
		}

		// Token: 0x170002BA RID: 698
		// (get) Token: 0x060008CE RID: 2254 RVA: 0x00024B8D File Offset: 0x00022D8D
		// (set) Token: 0x060008CF RID: 2255 RVA: 0x00024B95 File Offset: 0x00022D95
		[DataSourceProperty]
		public DecisionItemBaseVM CurrentDecision
		{
			get
			{
				return this._currentDecision;
			}
			set
			{
				if (value != this._currentDecision)
				{
					this._currentDecision = value;
					base.OnPropertyChangedWithValue<DecisionItemBaseVM>(value, "CurrentDecision");
				}
			}
		}

		// Token: 0x170002BB RID: 699
		// (get) Token: 0x060008D0 RID: 2256 RVA: 0x00024BB3 File Offset: 0x00022DB3
		// (set) Token: 0x060008D1 RID: 2257 RVA: 0x00024BBB File Offset: 0x00022DBB
		[DataSourceProperty]
		public int NotificationCount
		{
			get
			{
				return this._notificationCount;
			}
			set
			{
				if (value != this._notificationCount)
				{
					this._notificationCount = value;
					base.OnPropertyChangedWithValue(value, "NotificationCount");
				}
			}
		}

		// Token: 0x170002BC RID: 700
		// (get) Token: 0x060008D2 RID: 2258 RVA: 0x00024BD9 File Offset: 0x00022DD9
		// (set) Token: 0x060008D3 RID: 2259 RVA: 0x00024BE1 File Offset: 0x00022DE1
		[DataSourceProperty]
		public bool IsRefreshed
		{
			get
			{
				return this._isRefreshed;
			}
			set
			{
				if (value != this._isRefreshed)
				{
					this._isRefreshed = value;
					base.OnPropertyChangedWithValue(value, "IsRefreshed");
				}
			}
		}

		// Token: 0x170002BD RID: 701
		// (get) Token: 0x060008D4 RID: 2260 RVA: 0x00024BFF File Offset: 0x00022DFF
		// (set) Token: 0x060008D5 RID: 2261 RVA: 0x00024C07 File Offset: 0x00022E07
		[DataSourceProperty]
		public bool IsActive
		{
			get
			{
				return this._isActive;
			}
			set
			{
				if (value != this._isActive)
				{
					this._isActive = value;
					base.OnPropertyChangedWithValue(value, "IsActive");
				}
			}
		}

		// Token: 0x170002BE RID: 702
		// (get) Token: 0x060008D6 RID: 2262 RVA: 0x00024C25 File Offset: 0x00022E25
		// (set) Token: 0x060008D7 RID: 2263 RVA: 0x00024C2D File Offset: 0x00022E2D
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

		// Token: 0x040003F3 RID: 1011
		private List<KingdomDecision> _examinedDecisionsSinceInit;

		// Token: 0x040003F4 RID: 1012
		private readonly Action _refreshKingdomManagement;

		// Token: 0x040003F6 RID: 1014
		private InquiryData _queryData;

		// Token: 0x040003F7 RID: 1015
		private InputKeyItemVM _doneInputKey;

		// Token: 0x040003F8 RID: 1016
		private bool _isRefreshed;

		// Token: 0x040003F9 RID: 1017
		private bool _isActive;

		// Token: 0x040003FA RID: 1018
		private int _notificationCount;

		// Token: 0x040003FB RID: 1019
		private string _titleText;

		// Token: 0x040003FC RID: 1020
		private DecisionItemBaseVM _currentDecision;
	}
}
