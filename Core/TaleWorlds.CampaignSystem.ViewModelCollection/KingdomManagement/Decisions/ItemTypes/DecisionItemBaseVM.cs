using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.ViewModelCollection.Input;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Core.ViewModelCollection.Tutorial;
using TaleWorlds.Library;
using TaleWorlds.Library.EventSystem;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Decisions.ItemTypes
{
	public class DecisionItemBaseVM : ViewModel
	{
		public KingdomElection KingdomDecisionMaker { get; private set; }

		private float _currentInfluenceCost
		{
			get
			{
				if (this._currentSelectedOption != null && !this._currentSelectedOption.IsOptionForAbstain)
				{
					if (!this.IsPlayerSupporter)
					{
						return (float)Campaign.Current.Models.ClanPoliticsModel.GetInfluenceRequiredToOverrideKingdomDecision(this.KingdomDecisionMaker.PossibleOutcomes.MaxBy((DecisionOutcome o) => o.WinChance), this._currentSelectedOption.Option, this._decision);
					}
					if (this._currentSelectedOption.CurrentSupportWeight != Supporter.SupportWeights.Choose)
					{
						return (float)this.KingdomDecisionMaker.GetInfluenceCostOfOutcome(this._currentSelectedOption.Option, Clan.PlayerClan, this._currentSelectedOption.CurrentSupportWeight);
					}
				}
				return 0f;
			}
		}

		public DecisionItemBaseVM(KingdomDecision decision, Action onDecisionOver)
		{
			this._decision = decision;
			this._onDecisionOver = onDecisionOver;
			this.DecisionType = 0;
			this.DecisionOptionsList = new MBBindingList<DecisionOptionVM>();
			this.EndDecisionHint = new HintViewModel();
			CampaignEvents.KingdomDecisionConcluded.AddNonSerializedListener(this, new Action<KingdomDecision, DecisionOutcome, bool>(this.OnKingdomDecisionConcluded));
			this.RefreshValues();
			this.InitValues();
			Game game = Game.Current;
			if (game == null)
			{
				return;
			}
			EventManager eventManager = game.EventManager;
			if (eventManager == null)
			{
				return;
			}
			eventManager.RegisterEvent<TutorialNotificationElementChangeEvent>(new Action<TutorialNotificationElementChangeEvent>(this.OnTutorialNotificationElementIDChange));
		}

		private void OnKingdomDecisionConcluded(KingdomDecision decision, DecisionOutcome outcome, bool isPlayerInvolved)
		{
			if (decision == this._decision)
			{
				this.IsKingsDecisionOver = true;
				this.CurrentStageIndex = 1;
				foreach (DecisionOptionVM decisionOptionVM in this.DecisionOptionsList)
				{
					if (decisionOptionVM.Option == outcome)
					{
						decisionOptionVM.IsKingsOutcome = true;
					}
					decisionOptionVM.AfterKingChooseOutcome();
				}
			}
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.DoneText = GameTexts.FindText("str_done", null).ToString();
			GameTexts.SetVariable("TOTAL_INFLUENCE", MathF.Round(Hero.MainHero.Clan.Influence));
			this.TotalInfluenceText = GameTexts.FindText("str_total_influence", null).ToString();
			this.RefreshInfluenceCost();
			MBBindingList<DecisionOptionVM> decisionOptionsList = this.DecisionOptionsList;
			if (decisionOptionsList == null)
			{
				return;
			}
			decisionOptionsList.ApplyActionOnAllItems(delegate(DecisionOptionVM x)
			{
				x.RefreshValues();
			});
		}

		protected virtual void InitValues()
		{
			this.DecisionOptionsList.Clear();
			this.KingdomDecisionMaker = new KingdomElection(this._decision);
			this.KingdomDecisionMaker.StartElection();
			this.CurrentStageIndex = ((!this.KingdomDecisionMaker.IsPlayerChooser) ? 0 : 1);
			this.IsPlayerSupporter = !this.KingdomDecisionMaker.IsPlayerChooser;
			this.KingdomDecisionMaker.DetermineOfficialSupport();
			foreach (DecisionOutcome decisionOutcome in this.KingdomDecisionMaker.PossibleOutcomes)
			{
				DecisionOptionVM decisionOptionVM = new DecisionOptionVM(decisionOutcome, this._decision, this.KingdomDecisionMaker, new Action<DecisionOptionVM>(this.OnChangeVote), new Action<DecisionOptionVM>(this.OnSupportStrengthChange))
				{
					WinPercentage = MathF.Round(decisionOutcome.WinChance * 100f),
					InitialPercentage = MathF.Round(decisionOutcome.WinChance * 100f)
				};
				this.DecisionOptionsList.Add(decisionOptionVM);
			}
			if (this.IsPlayerSupporter)
			{
				DecisionOptionVM decisionOptionVM2 = new DecisionOptionVM(null, null, this.KingdomDecisionMaker, new Action<DecisionOptionVM>(this.OnAbstain), new Action<DecisionOptionVM>(this.OnSupportStrengthChange));
				this.DecisionOptionsList.Add(decisionOptionVM2);
			}
			this.TitleText = this.KingdomDecisionMaker.GetTitle().ToString();
			this.DescriptionText = this.KingdomDecisionMaker.GetDescription().ToString();
			this.RefreshInfluenceCost();
			this.RefreshCanEndDecision();
			this.RefreshRelationChangeText();
			this.IsActive = true;
		}

		private void OnChangeVote(DecisionOptionVM target)
		{
			if (this._currentSelectedOption != target)
			{
				if (this._currentSelectedOption != null)
				{
					this._currentSelectedOption.IsSelected = false;
				}
				this._currentSelectedOption = target;
				this._currentSelectedOption.IsSelected = true;
				this.KingdomDecisionMaker.OnPlayerSupport((!this._currentSelectedOption.IsOptionForAbstain) ? this._currentSelectedOption.Option : null, this._currentSelectedOption.CurrentSupportWeight);
				this.RefreshWinPercentages();
				this.RefreshInfluenceCost();
				this.RefreshCanEndDecision();
				this.RefreshRelationChangeText();
			}
		}

		private void OnAbstain(DecisionOptionVM target)
		{
			if (this._currentSelectedOption != target)
			{
				if (this._currentSelectedOption != null)
				{
					this._currentSelectedOption.IsSelected = false;
				}
				this._currentSelectedOption = target;
				this._currentSelectedOption.IsSelected = true;
				this.KingdomDecisionMaker.OnPlayerSupport((!this._currentSelectedOption.IsOptionForAbstain) ? this._currentSelectedOption.Option : null, this._currentSelectedOption.CurrentSupportWeight);
				this.RefreshWinPercentages();
				this.RefreshInfluenceCost();
				this.RefreshCanEndDecision();
				this.RefreshRelationChangeText();
			}
		}

		private void OnSupportStrengthChange(DecisionOptionVM option)
		{
			this.RefreshWinPercentages();
			this.RefreshCanEndDecision();
			this.RefreshRelationChangeText();
			this.RefreshInfluenceCost();
		}

		private void RefreshWinPercentages()
		{
			this.KingdomDecisionMaker.DetermineOfficialSupport();
			using (List<DecisionOutcome>.Enumerator enumerator = this.KingdomDecisionMaker.PossibleOutcomes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					DecisionOutcome option = enumerator.Current;
					DecisionOptionVM decisionOptionVM = this.DecisionOptionsList.FirstOrDefault((DecisionOptionVM c) => c.Option == option);
					if (decisionOptionVM == null)
					{
						Debug.FailedAssert("Couldn't find option to update win chance for!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem.ViewModelCollection\\KingdomManagement\\Decisions\\ItemTypes\\DecisionItemBaseVM.cs", "RefreshWinPercentages", 210);
					}
					else
					{
						decisionOptionVM.WinPercentage = (int)MathF.Round(option.WinChance * 100f, 2);
					}
				}
			}
			int num = this.DecisionOptionsList.Where((DecisionOptionVM d) => !d.IsOptionForAbstain).Sum((DecisionOptionVM d) => d.WinPercentage);
			if (num != 100)
			{
				int num2 = 100 - num;
				List<DecisionOptionVM> list = this.DecisionOptionsList.Where((DecisionOptionVM opt) => opt.Sponsor != null).ToList<DecisionOptionVM>();
				int num3 = list.Select((DecisionOptionVM opt) => opt.WinPercentage).Sum();
				if (num3 == 0)
				{
					int num4 = num2 / list.Count;
					foreach (DecisionOptionVM decisionOptionVM2 in list)
					{
						decisionOptionVM2.WinPercentage += num4;
					}
					list[0].WinPercentage += num2 - num4 * list.Count;
					return;
				}
				int num5 = 0;
				foreach (DecisionOptionVM decisionOptionVM3 in list.Where((DecisionOptionVM opt) => opt.WinPercentage > 0).ToList<DecisionOptionVM>())
				{
					int num6 = MathF.Floor((float)num2 * ((float)decisionOptionVM3.WinPercentage / (float)num3));
					decisionOptionVM3.WinPercentage += num6;
					num5 += num6;
				}
				list[0].WinPercentage += num2 - num5;
			}
		}

		private void RefreshInfluenceCost()
		{
			if (this._currentInfluenceCost > 0f)
			{
				GameTexts.SetVariable("AMOUNT", this._currentInfluenceCost);
				GameTexts.SetVariable("INFLUENCE_ICON", "{=!}<img src=\"General\\Icons\\Influence@2x\" extend=\"7\">");
				this.InfluenceCostText = GameTexts.FindText("str_decision_influence_cost", null).ToString();
				return;
			}
			this.InfluenceCostText = "";
		}

		private void RefreshRelationChangeText()
		{
			this.RelationChangeText = "";
			DecisionOptionVM currentSelectedOption = this._currentSelectedOption;
			if (currentSelectedOption != null && !currentSelectedOption.IsOptionForAbstain)
			{
				foreach (DecisionOptionVM decisionOptionVM in this.DecisionOptionsList)
				{
					DecisionOutcome option = decisionOptionVM.Option;
					if (((option != null) ? option.SponsorClan : null) != null && decisionOptionVM.Option.SponsorClan != Clan.PlayerClan)
					{
						bool flag = this._currentSelectedOption == decisionOptionVM;
						GameTexts.SetVariable("HERO_NAME", decisionOptionVM.Option.SponsorClan.Leader.EncyclopediaLinkWithName);
						string text = (flag ? GameTexts.FindText("str_decision_relation_increase", null).ToString() : GameTexts.FindText("str_decision_relation_decrease", null).ToString());
						if (string.IsNullOrEmpty(this.RelationChangeText))
						{
							this.RelationChangeText = text;
						}
						else
						{
							GameTexts.SetVariable("newline", "\n");
							GameTexts.SetVariable("STR1", this.RelationChangeText);
							GameTexts.SetVariable("STR2", text);
							this.RelationChangeText = GameTexts.FindText("str_string_newline_string", null).ToString();
						}
					}
				}
			}
		}

		private void RefreshCanEndDecision()
		{
			bool flag = this._currentSelectedOption != null && (!this.IsPlayerSupporter || this._currentSelectedOption.CurrentSupportWeight > Supporter.SupportWeights.Choose);
			bool flag2 = this._currentInfluenceCost <= Clan.PlayerClan.Influence || this._currentInfluenceCost == 0f;
			DecisionOptionVM currentSelectedOption = this._currentSelectedOption;
			bool flag3 = currentSelectedOption != null && currentSelectedOption.IsOptionForAbstain;
			this.CanEndDecision = !this._finalSelectionDone && (flag3 || (flag && flag2));
			if (this.CanEndDecision)
			{
				this.EndDecisionHint.HintText = TextObject.Empty;
				return;
			}
			if (flag)
			{
				if (!flag2)
				{
					this.EndDecisionHint.HintText = GameTexts.FindText("str_decision_not_enough_influence", null);
				}
				return;
			}
			if (this.IsPlayerSupporter)
			{
				this.EndDecisionHint.HintText = GameTexts.FindText("str_decision_need_to_select_an_option_and_support", null);
				return;
			}
			this.EndDecisionHint.HintText = GameTexts.FindText("str_decision_need_to_select_an_outcome", null);
		}

		protected void ExecuteLink(string link)
		{
			Campaign.Current.EncyclopediaManager.GoToLink(link);
		}

		protected void ExecuteShowStageTooltip()
		{
			if (!this.IsPlayerSupporter)
			{
				MBInformationManager.ShowHint(GameTexts.FindText("str_decision_second_stage_player_decider", null).ToString());
				return;
			}
			if (this.CurrentStageIndex == 0)
			{
				MBInformationManager.ShowHint(GameTexts.FindText("str_decision_first_stage_player_supporter", null).ToString());
				return;
			}
			MBInformationManager.ShowHint(GameTexts.FindText("str_decision_second_stage_player_supporter", null).ToString());
		}

		protected void ExecuteHideStageTooltip()
		{
			MBInformationManager.HideInformations();
		}

		public void ExecuteFinalSelection()
		{
			if (this.CanEndDecision)
			{
				this.KingdomDecisionMaker.ApplySelection();
				this._finalSelectionDone = true;
				this.RefreshCanEndDecision();
			}
		}

		protected void ExecuteDone()
		{
			TextObject chosenOutcomeText = this.KingdomDecisionMaker.GetChosenOutcomeText();
			this.IsActive = false;
			InformationManager.ShowInquiry(new InquiryData(GameTexts.FindText("str_decision_outcome", null).ToString(), chosenOutcomeText.ToString(), true, false, GameTexts.FindText("str_ok", null).ToString(), "", delegate
			{
				this._onDecisionOver();
			}, null, "", 0f, null, null, null), false, false);
			CampaignEvents.KingdomDecisionConcluded.ClearListeners(this);
			this._currentSelectedOption = null;
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			Game game = Game.Current;
			if (game == null)
			{
				return;
			}
			EventManager eventManager = game.EventManager;
			if (eventManager == null)
			{
				return;
			}
			eventManager.UnregisterEvent<TutorialNotificationElementChangeEvent>(new Action<TutorialNotificationElementChangeEvent>(this.OnTutorialNotificationElementIDChange));
		}

		private void OnTutorialNotificationElementIDChange(TutorialNotificationElementChangeEvent obj)
		{
			if (this._latestTutorialElementID != obj.NewNotificationElementID)
			{
				this._latestTutorialElementID = obj.NewNotificationElementID;
				if (this._isDecisionOptionsHighlightEnabled && this._latestTutorialElementID != this._decisionOptionsHighlightID)
				{
					this.SetOptionsHighlight(false);
					this._isDecisionOptionsHighlightEnabled = false;
					return;
				}
				if (!this._isDecisionOptionsHighlightEnabled && this._latestTutorialElementID == this._decisionOptionsHighlightID)
				{
					this.SetOptionsHighlight(true);
					this._isDecisionOptionsHighlightEnabled = true;
				}
			}
		}

		private void SetOptionsHighlight(bool state)
		{
			for (int i = 0; i < this.DecisionOptionsList.Count; i++)
			{
				DecisionOptionVM decisionOptionVM = this.DecisionOptionsList[i];
				if (decisionOptionVM.CanBeChosen)
				{
					decisionOptionVM.IsHighlightEnabled = state;
				}
			}
		}

		public void SetDoneInputKey(InputKeyItemVM inputKeyItemVM)
		{
			this.DoneInputKey = inputKeyItemVM;
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
		public HintViewModel EndDecisionHint
		{
			get
			{
				return this._endDecisionHint;
			}
			set
			{
				if (value != this._endDecisionHint)
				{
					this._endDecisionHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "EndDecisionHint");
				}
			}
		}

		[DataSourceProperty]
		public int DecisionType
		{
			get
			{
				return this._decisionType;
			}
			set
			{
				if (value != this._decisionType)
				{
					this._decisionType = value;
					base.OnPropertyChangedWithValue(value, "DecisionType");
				}
			}
		}

		[DataSourceProperty]
		public string TotalInfluenceText
		{
			get
			{
				return this._totalInfluenceText;
			}
			set
			{
				if (value != this._totalInfluenceText)
				{
					this._totalInfluenceText = value;
					base.OnPropertyChangedWithValue<string>(value, "TotalInfluenceText");
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
		public int CurrentStageIndex
		{
			get
			{
				return this._currentStageIndex;
			}
			set
			{
				if (value != this._currentStageIndex)
				{
					this._currentStageIndex = value;
					base.OnPropertyChangedWithValue(value, "CurrentStageIndex");
				}
			}
		}

		[DataSourceProperty]
		public bool IsPlayerSupporter
		{
			get
			{
				return this._isPlayerSupporter;
			}
			set
			{
				if (value != this._isPlayerSupporter)
				{
					this._isPlayerSupporter = value;
					base.OnPropertyChangedWithValue(value, "IsPlayerSupporter");
				}
			}
		}

		[DataSourceProperty]
		public bool CanEndDecision
		{
			get
			{
				return this._canEndDecision;
			}
			set
			{
				if (value != this._canEndDecision)
				{
					this._canEndDecision = value;
					base.OnPropertyChangedWithValue(value, "CanEndDecision");
				}
			}
		}

		[DataSourceProperty]
		public bool IsKingsDecisionOver
		{
			get
			{
				return this._isKingsDecisionOver;
			}
			set
			{
				if (value != this._isKingsDecisionOver)
				{
					this._isKingsDecisionOver = value;
					base.OnPropertyChangedWithValue(value, "IsKingsDecisionOver");
				}
			}
		}

		[DataSourceProperty]
		public string RelationChangeText
		{
			get
			{
				return this._increaseRelationText;
			}
			set
			{
				if (value != this._increaseRelationText)
				{
					this._increaseRelationText = value;
					base.OnPropertyChangedWithValue<string>(value, "RelationChangeText");
				}
			}
		}

		[DataSourceProperty]
		public string DescriptionText
		{
			get
			{
				return this._descriptionText;
			}
			set
			{
				if (value != this._descriptionText)
				{
					this._descriptionText = value;
					base.OnPropertyChangedWithValue<string>(value, "DescriptionText");
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
		public string DoneText
		{
			get
			{
				return this._doneText;
			}
			set
			{
				if (value != this._doneText)
				{
					this._doneText = value;
					base.OnPropertyChangedWithValue<string>(value, "DoneText");
				}
			}
		}

		[DataSourceProperty]
		public string InfluenceCostText
		{
			get
			{
				return this._influenceCostText;
			}
			set
			{
				if (value != this._influenceCostText)
				{
					this._influenceCostText = value;
					base.OnPropertyChangedWithValue<string>(value, "InfluenceCostText");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<DecisionOptionVM> DecisionOptionsList
		{
			get
			{
				return this._decisionOptionsList;
			}
			set
			{
				if (value != this._decisionOptionsList)
				{
					this._decisionOptionsList = value;
					base.OnPropertyChangedWithValue<MBBindingList<DecisionOptionVM>>(value, "DecisionOptionsList");
				}
			}
		}

		protected readonly KingdomDecision _decision;

		private readonly Action _onDecisionOver;

		private DecisionOptionVM _currentSelectedOption;

		private bool _finalSelectionDone;

		private bool _isDecisionOptionsHighlightEnabled;

		private string _decisionOptionsHighlightID = "DecisionOptions";

		private string _latestTutorialElementID;

		private InputKeyItemVM _doneInputKey;

		private int _decisionType;

		private bool _isActive;

		private bool _isPlayerSupporter;

		private bool _canEndDecision;

		private bool _isKingsDecisionOver;

		private int _currentStageIndex = -1;

		private string _titleText;

		private string _doneText;

		private string _descriptionText;

		private string _influenceCostText;

		private string _totalInfluenceText;

		private string _increaseRelationText;

		private HintViewModel _endDecisionHint;

		private MBBindingList<DecisionOptionVM> _decisionOptionsList;

		protected enum DecisionTypes
		{
			Default,
			Settlement,
			ExpelClan,
			Policy,
			DeclareWar,
			MakePeace,
			KingSelection
		}
	}
}
