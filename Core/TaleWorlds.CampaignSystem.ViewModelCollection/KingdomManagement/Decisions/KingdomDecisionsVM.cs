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
	public class KingdomDecisionsVM : ViewModel
	{
		public bool IsCurrentDecisionActive
		{
			get
			{
				DecisionItemBaseVM currentDecision = this.CurrentDecision;
				return currentDecision != null && currentDecision.IsActive;
			}
		}

		private bool _shouldCheckForDecision { get; set; } = true;

		public KingdomDecisionsVM(Action refreshKingdomManagement)
		{
			this._refreshKingdomManagement = refreshKingdomManagement;
			this._examinedDecisionsSinceInit = new List<KingdomDecision>();
			this._examinedDecisionsSinceInit.AddRange(Clan.PlayerClan.Kingdom.UnresolvedDecisions.Where((KingdomDecision d) => d.ShouldBeCancelled()));
			this.IsRefreshed = true;
			this.RefreshValues();
		}

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

		private void OnSingleDecisionOver()
		{
			this._refreshKingdomManagement();
			this._shouldCheckForDecision = true;
		}

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

		public void SetDoneInputKey(HotKey hotKey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

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

		private List<KingdomDecision> _examinedDecisionsSinceInit;

		private readonly Action _refreshKingdomManagement;

		private InquiryData _queryData;

		private InputKeyItemVM _doneInputKey;

		private bool _isRefreshed;

		private bool _isActive;

		private int _notificationCount;

		private string _titleText;

		private DecisionItemBaseVM _currentDecision;
	}
}
