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
	// Token: 0x02000067 RID: 103
	public class DecisionItemBaseVM : ViewModel
	{
		// Token: 0x170002BF RID: 703
		// (get) Token: 0x060008D9 RID: 2265 RVA: 0x00024C58 File Offset: 0x00022E58
		// (set) Token: 0x060008DA RID: 2266 RVA: 0x00024C60 File Offset: 0x00022E60
		public KingdomElection KingdomDecisionMaker { get; private set; }

		// Token: 0x170002C0 RID: 704
		// (get) Token: 0x060008DB RID: 2267 RVA: 0x00024C6C File Offset: 0x00022E6C
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

		// Token: 0x060008DC RID: 2268 RVA: 0x00024D2C File Offset: 0x00022F2C
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

		// Token: 0x060008DD RID: 2269 RVA: 0x00024DC4 File Offset: 0x00022FC4
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

		// Token: 0x060008DE RID: 2270 RVA: 0x00024E38 File Offset: 0x00023038
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

		// Token: 0x060008DF RID: 2271 RVA: 0x00024ECC File Offset: 0x000230CC
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

		// Token: 0x060008E0 RID: 2272 RVA: 0x00025060 File Offset: 0x00023260
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

		// Token: 0x060008E1 RID: 2273 RVA: 0x000250E8 File Offset: 0x000232E8
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

		// Token: 0x060008E2 RID: 2274 RVA: 0x0002516E File Offset: 0x0002336E
		private void OnSupportStrengthChange(DecisionOptionVM option)
		{
			this.RefreshWinPercentages();
			this.RefreshCanEndDecision();
			this.RefreshRelationChangeText();
			this.RefreshInfluenceCost();
		}

		// Token: 0x060008E3 RID: 2275 RVA: 0x00025188 File Offset: 0x00023388
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

		// Token: 0x060008E4 RID: 2276 RVA: 0x00025424 File Offset: 0x00023624
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

		// Token: 0x060008E5 RID: 2277 RVA: 0x00025480 File Offset: 0x00023680
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

		// Token: 0x060008E6 RID: 2278 RVA: 0x000255C0 File Offset: 0x000237C0
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

		// Token: 0x060008E7 RID: 2279 RVA: 0x000256AD File Offset: 0x000238AD
		protected void ExecuteLink(string link)
		{
			Campaign.Current.EncyclopediaManager.GoToLink(link);
		}

		// Token: 0x060008E8 RID: 2280 RVA: 0x000256C0 File Offset: 0x000238C0
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

		// Token: 0x060008E9 RID: 2281 RVA: 0x0002571E File Offset: 0x0002391E
		protected void ExecuteHideStageTooltip()
		{
			MBInformationManager.HideInformations();
		}

		// Token: 0x060008EA RID: 2282 RVA: 0x00025725 File Offset: 0x00023925
		public void ExecuteFinalSelection()
		{
			if (this.CanEndDecision)
			{
				this.KingdomDecisionMaker.ApplySelection();
				this._finalSelectionDone = true;
				this.RefreshCanEndDecision();
			}
		}

		// Token: 0x060008EB RID: 2283 RVA: 0x00025748 File Offset: 0x00023948
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

		// Token: 0x060008EC RID: 2284 RVA: 0x000257CD File Offset: 0x000239CD
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

		// Token: 0x060008ED RID: 2285 RVA: 0x000257FC File Offset: 0x000239FC
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

		// Token: 0x060008EE RID: 2286 RVA: 0x0002587C File Offset: 0x00023A7C
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

		// Token: 0x060008EF RID: 2287 RVA: 0x000258BB File Offset: 0x00023ABB
		public void SetDoneInputKey(InputKeyItemVM inputKeyItemVM)
		{
			this.DoneInputKey = inputKeyItemVM;
		}

		// Token: 0x170002C1 RID: 705
		// (get) Token: 0x060008F0 RID: 2288 RVA: 0x000258C4 File Offset: 0x00023AC4
		// (set) Token: 0x060008F1 RID: 2289 RVA: 0x000258CC File Offset: 0x00023ACC
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

		// Token: 0x170002C2 RID: 706
		// (get) Token: 0x060008F2 RID: 2290 RVA: 0x000258EA File Offset: 0x00023AEA
		// (set) Token: 0x060008F3 RID: 2291 RVA: 0x000258F2 File Offset: 0x00023AF2
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

		// Token: 0x170002C3 RID: 707
		// (get) Token: 0x060008F4 RID: 2292 RVA: 0x00025910 File Offset: 0x00023B10
		// (set) Token: 0x060008F5 RID: 2293 RVA: 0x00025918 File Offset: 0x00023B18
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

		// Token: 0x170002C4 RID: 708
		// (get) Token: 0x060008F6 RID: 2294 RVA: 0x00025936 File Offset: 0x00023B36
		// (set) Token: 0x060008F7 RID: 2295 RVA: 0x0002593E File Offset: 0x00023B3E
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

		// Token: 0x170002C5 RID: 709
		// (get) Token: 0x060008F8 RID: 2296 RVA: 0x00025961 File Offset: 0x00023B61
		// (set) Token: 0x060008F9 RID: 2297 RVA: 0x00025969 File Offset: 0x00023B69
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

		// Token: 0x170002C6 RID: 710
		// (get) Token: 0x060008FA RID: 2298 RVA: 0x00025987 File Offset: 0x00023B87
		// (set) Token: 0x060008FB RID: 2299 RVA: 0x0002598F File Offset: 0x00023B8F
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

		// Token: 0x170002C7 RID: 711
		// (get) Token: 0x060008FC RID: 2300 RVA: 0x000259AD File Offset: 0x00023BAD
		// (set) Token: 0x060008FD RID: 2301 RVA: 0x000259B5 File Offset: 0x00023BB5
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

		// Token: 0x170002C8 RID: 712
		// (get) Token: 0x060008FE RID: 2302 RVA: 0x000259D3 File Offset: 0x00023BD3
		// (set) Token: 0x060008FF RID: 2303 RVA: 0x000259DB File Offset: 0x00023BDB
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

		// Token: 0x170002C9 RID: 713
		// (get) Token: 0x06000900 RID: 2304 RVA: 0x000259F9 File Offset: 0x00023BF9
		// (set) Token: 0x06000901 RID: 2305 RVA: 0x00025A01 File Offset: 0x00023C01
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

		// Token: 0x170002CA RID: 714
		// (get) Token: 0x06000902 RID: 2306 RVA: 0x00025A1F File Offset: 0x00023C1F
		// (set) Token: 0x06000903 RID: 2307 RVA: 0x00025A27 File Offset: 0x00023C27
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

		// Token: 0x170002CB RID: 715
		// (get) Token: 0x06000904 RID: 2308 RVA: 0x00025A4A File Offset: 0x00023C4A
		// (set) Token: 0x06000905 RID: 2309 RVA: 0x00025A52 File Offset: 0x00023C52
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

		// Token: 0x170002CC RID: 716
		// (get) Token: 0x06000906 RID: 2310 RVA: 0x00025A75 File Offset: 0x00023C75
		// (set) Token: 0x06000907 RID: 2311 RVA: 0x00025A7D File Offset: 0x00023C7D
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

		// Token: 0x170002CD RID: 717
		// (get) Token: 0x06000908 RID: 2312 RVA: 0x00025AA0 File Offset: 0x00023CA0
		// (set) Token: 0x06000909 RID: 2313 RVA: 0x00025AA8 File Offset: 0x00023CA8
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

		// Token: 0x170002CE RID: 718
		// (get) Token: 0x0600090A RID: 2314 RVA: 0x00025ACB File Offset: 0x00023CCB
		// (set) Token: 0x0600090B RID: 2315 RVA: 0x00025AD3 File Offset: 0x00023CD3
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

		// Token: 0x170002CF RID: 719
		// (get) Token: 0x0600090C RID: 2316 RVA: 0x00025AF6 File Offset: 0x00023CF6
		// (set) Token: 0x0600090D RID: 2317 RVA: 0x00025AFE File Offset: 0x00023CFE
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

		// Token: 0x040003FE RID: 1022
		protected readonly KingdomDecision _decision;

		// Token: 0x040003FF RID: 1023
		private readonly Action _onDecisionOver;

		// Token: 0x04000400 RID: 1024
		private DecisionOptionVM _currentSelectedOption;

		// Token: 0x04000401 RID: 1025
		private bool _finalSelectionDone;

		// Token: 0x04000402 RID: 1026
		private bool _isDecisionOptionsHighlightEnabled;

		// Token: 0x04000403 RID: 1027
		private string _decisionOptionsHighlightID = "DecisionOptions";

		// Token: 0x04000404 RID: 1028
		private string _latestTutorialElementID;

		// Token: 0x04000405 RID: 1029
		private InputKeyItemVM _doneInputKey;

		// Token: 0x04000406 RID: 1030
		private int _decisionType;

		// Token: 0x04000407 RID: 1031
		private bool _isActive;

		// Token: 0x04000408 RID: 1032
		private bool _isPlayerSupporter;

		// Token: 0x04000409 RID: 1033
		private bool _canEndDecision;

		// Token: 0x0400040A RID: 1034
		private bool _isKingsDecisionOver;

		// Token: 0x0400040B RID: 1035
		private int _currentStageIndex = -1;

		// Token: 0x0400040C RID: 1036
		private string _titleText;

		// Token: 0x0400040D RID: 1037
		private string _doneText;

		// Token: 0x0400040E RID: 1038
		private string _descriptionText;

		// Token: 0x0400040F RID: 1039
		private string _influenceCostText;

		// Token: 0x04000410 RID: 1040
		private string _totalInfluenceText;

		// Token: 0x04000411 RID: 1041
		private string _increaseRelationText;

		// Token: 0x04000412 RID: 1042
		private HintViewModel _endDecisionHint;

		// Token: 0x04000413 RID: 1043
		private MBBindingList<DecisionOptionVM> _decisionOptionsList;

		// Token: 0x0200019D RID: 413
		protected enum DecisionTypes
		{
			// Token: 0x04000F42 RID: 3906
			Default,
			// Token: 0x04000F43 RID: 3907
			Settlement,
			// Token: 0x04000F44 RID: 3908
			ExpelClan,
			// Token: 0x04000F45 RID: 3909
			Policy,
			// Token: 0x04000F46 RID: 3910
			DeclareWar,
			// Token: 0x04000F47 RID: 3911
			MakePeace,
			// Token: 0x04000F48 RID: 3912
			KingSelection
		}
	}
}
