using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Library.EventSystem;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Decisions
{
	// Token: 0x02000063 RID: 99
	public class DecisionOptionVM : ViewModel
	{
		// Token: 0x17000298 RID: 664
		// (get) Token: 0x06000872 RID: 2162 RVA: 0x00023AB2 File Offset: 0x00021CB2
		// (set) Token: 0x06000873 RID: 2163 RVA: 0x00023ABA File Offset: 0x00021CBA
		public DecisionOutcome Option { get; private set; }

		// Token: 0x17000299 RID: 665
		// (get) Token: 0x06000874 RID: 2164 RVA: 0x00023AC3 File Offset: 0x00021CC3
		// (set) Token: 0x06000875 RID: 2165 RVA: 0x00023ACB File Offset: 0x00021CCB
		public KingdomDecision Decision { get; private set; }

		// Token: 0x06000876 RID: 2166 RVA: 0x00023AD4 File Offset: 0x00021CD4
		public DecisionOptionVM(DecisionOutcome option, KingdomDecision decision, KingdomElection kingdomDecisionMaker, Action<DecisionOptionVM> onSelect, Action<DecisionOptionVM> onSupportStrengthChange)
		{
			this._onSelect = onSelect;
			this._onSupportStrengthChange = onSupportStrengthChange;
			this._kingdomDecisionMaker = kingdomDecisionMaker;
			this.Decision = decision;
			this.CurrentSupportWeight = Supporter.SupportWeights.Choose;
			this.OptionHint = new HintViewModel();
			this.IsPlayerSupporter = !this._kingdomDecisionMaker.IsPlayerChooser;
			this.SupportersOfThisOption = new MBBindingList<DecisionSupporterVM>();
			this.Option = option;
			if (option != null)
			{
				Clan sponsorClan = option.SponsorClan;
				if (((sponsorClan != null) ? sponsorClan.Leader : null) != null)
				{
					this.Sponsor = new HeroVM(option.SponsorClan.Leader, false);
				}
				List<Supporter> supporterList = option.SupporterList;
				if (supporterList != null && supporterList.Count > 0)
				{
					foreach (Supporter supporter in option.SupporterList)
					{
						if (supporter.SupportWeight > Supporter.SupportWeights.StayNeutral)
						{
							if (supporter.Clan != option.SponsorClan)
							{
								this.SupportersOfThisOption.Add(new DecisionSupporterVM(supporter.Name, supporter.ImagePath, supporter.Clan, supporter.SupportWeight));
							}
							else
							{
								this.SponsorWeightImagePath = DecisionSupporterVM.GetSupporterWeightImagePath(supporter.SupportWeight);
							}
						}
					}
				}
				this.IsOptionForAbstain = false;
			}
			else
			{
				this.IsOptionForAbstain = true;
			}
			this.RefreshValues();
			this.RefreshSupportOptionEnabled();
			this.RefreshCanChooseOption();
		}

		// Token: 0x06000877 RID: 2167 RVA: 0x00023C4C File Offset: 0x00021E4C
		public override void RefreshValues()
		{
			base.RefreshValues();
			if (this.Option != null)
			{
				this.Name = this.Option.GetDecisionTitle().ToString();
				this.Description = this.Option.GetDecisionDescription().ToString();
			}
			else
			{
				this.Name = GameTexts.FindText("str_abstain", null).ToString();
				this.Description = GameTexts.FindText("str_kingdom_decision_abstain_desc", null).ToString();
			}
			MBBindingList<DecisionSupporterVM> supportersOfThisOption = this.SupportersOfThisOption;
			if (supportersOfThisOption == null)
			{
				return;
			}
			supportersOfThisOption.ApplyActionOnAllItems(delegate(DecisionSupporterVM x)
			{
				x.RefreshValues();
			});
		}

		// Token: 0x06000878 RID: 2168 RVA: 0x00023CF0 File Offset: 0x00021EF0
		private void ExecuteShowSupporterTooltip()
		{
			DecisionOutcome option = this.Option;
			if (option != null && option.SupporterList.Count > 0)
			{
				List<TooltipProperty> list = new List<TooltipProperty>();
				this._kingdomDecisionMaker.DetermineOfficialSupport();
				foreach (Supporter supporter in this.Option.SupporterList)
				{
					if (supporter.SupportWeight > Supporter.SupportWeights.StayNeutral && !supporter.IsPlayer)
					{
						int influenceCost = this.Decision.GetInfluenceCost(this.Option, supporter.Clan, supporter.SupportWeight);
						GameTexts.SetVariable("AMOUNT", influenceCost);
						GameTexts.SetVariable("INFLUENCE_ICON", "{=!}<img src=\"General\\Icons\\Influence@2x\" extend=\"7\">");
						list.Add(new TooltipProperty(supporter.Name.ToString(), GameTexts.FindText("str_amount_with_influence_icon", null).ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
					}
				}
				InformationManager.ShowTooltip(typeof(List<TooltipProperty>), new object[] { list });
			}
		}

		// Token: 0x06000879 RID: 2169 RVA: 0x00023E00 File Offset: 0x00022000
		private void ExecuteHideSupporterTooltip()
		{
			MBInformationManager.HideInformations();
		}

		// Token: 0x0600087A RID: 2170 RVA: 0x00023E08 File Offset: 0x00022008
		private void RefreshSupportOptionEnabled()
		{
			int influenceCostOfOutcome = this._kingdomDecisionMaker.GetInfluenceCostOfOutcome(this.Option, Clan.PlayerClan, Supporter.SupportWeights.SlightlyFavor);
			int influenceCostOfOutcome2 = this._kingdomDecisionMaker.GetInfluenceCostOfOutcome(this.Option, Clan.PlayerClan, Supporter.SupportWeights.StronglyFavor);
			int influenceCostOfOutcome3 = this._kingdomDecisionMaker.GetInfluenceCostOfOutcome(this.Option, Clan.PlayerClan, Supporter.SupportWeights.FullyPush);
			this.SupportOption1Text = influenceCostOfOutcome.ToString();
			this.SupportOption2Text = influenceCostOfOutcome2.ToString();
			this.SupportOption3Text = influenceCostOfOutcome3.ToString();
			this.IsSupportOption1Enabled = (float)influenceCostOfOutcome <= Clan.PlayerClan.Influence && !this.IsOptionForAbstain;
			this.IsSupportOption2Enabled = (float)influenceCostOfOutcome2 <= Clan.PlayerClan.Influence && !this.IsOptionForAbstain;
			this.IsSupportOption3Enabled = (float)influenceCostOfOutcome3 <= Clan.PlayerClan.Influence && !this.IsOptionForAbstain;
		}

		// Token: 0x0600087B RID: 2171 RVA: 0x00023EE4 File Offset: 0x000220E4
		private void OnSupportStrengthChange(int index)
		{
			if (!this.IsOptionForAbstain)
			{
				switch (index)
				{
				case 0:
					this.CurrentSupportWeight = Supporter.SupportWeights.SlightlyFavor;
					break;
				case 1:
					this.CurrentSupportWeight = Supporter.SupportWeights.StronglyFavor;
					break;
				case 2:
					this.CurrentSupportWeight = Supporter.SupportWeights.FullyPush;
					break;
				}
				this._kingdomDecisionMaker.OnPlayerSupport((!this.IsOptionForAbstain) ? this.Option : null, this.CurrentSupportWeight);
				this._onSupportStrengthChange(this);
			}
		}

		// Token: 0x0600087C RID: 2172 RVA: 0x00023F54 File Offset: 0x00022154
		public void AfterKingChooseOutcome()
		{
			this._hasKingChoosen = true;
			this.RefreshCanChooseOption();
		}

		// Token: 0x0600087D RID: 2173 RVA: 0x00023F64 File Offset: 0x00022164
		private void RefreshCanChooseOption()
		{
			if (this._hasKingChoosen)
			{
				this.CanBeChosen = false;
				return;
			}
			if (this.IsOptionForAbstain)
			{
				this.CanBeChosen = true;
				return;
			}
			if (this.IsPlayerSupporter)
			{
				this.CanBeChosen = (float)this._kingdomDecisionMaker.GetInfluenceCostOfOutcome(this.Option, Clan.PlayerClan, Supporter.SupportWeights.SlightlyFavor) <= Clan.PlayerClan.Influence;
			}
			else
			{
				int influenceCostOfOutcome = this._kingdomDecisionMaker.GetInfluenceCostOfOutcome(this.Option, Clan.PlayerClan, Supporter.SupportWeights.Choose);
				this.CanBeChosen = (float)influenceCostOfOutcome <= Clan.PlayerClan.Influence || influenceCostOfOutcome == 0;
			}
			this.OptionHint.HintText = (this.CanBeChosen ? TextObject.Empty : new TextObject("{=Xmw93W6a}Not Enough Influence", null));
		}

		// Token: 0x0600087E RID: 2174 RVA: 0x00024020 File Offset: 0x00022220
		private void ExecuteSelection()
		{
			this._onSelect(this);
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
			eventManager.TriggerEvent<PlayerSelectedAKingdomDecisionOptionEvent>(new PlayerSelectedAKingdomDecisionOptionEvent(this.Option));
		}

		// Token: 0x1700029A RID: 666
		// (get) Token: 0x0600087F RID: 2175 RVA: 0x00024052 File Offset: 0x00022252
		// (set) Token: 0x06000880 RID: 2176 RVA: 0x0002405A File Offset: 0x0002225A
		[DataSourceProperty]
		public HintViewModel OptionHint
		{
			get
			{
				return this._optionHint;
			}
			set
			{
				if (value != this._optionHint)
				{
					this._optionHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "OptionHint");
				}
			}
		}

		// Token: 0x1700029B RID: 667
		// (get) Token: 0x06000881 RID: 2177 RVA: 0x00024078 File Offset: 0x00022278
		// (set) Token: 0x06000882 RID: 2178 RVA: 0x00024080 File Offset: 0x00022280
		[DataSourceProperty]
		public MBBindingList<DecisionSupporterVM> SupportersOfThisOption
		{
			get
			{
				return this._supportersOfThisOption;
			}
			set
			{
				if (value != this._supportersOfThisOption)
				{
					this._supportersOfThisOption = value;
					base.OnPropertyChangedWithValue<MBBindingList<DecisionSupporterVM>>(value, "SupportersOfThisOption");
				}
			}
		}

		// Token: 0x1700029C RID: 668
		// (get) Token: 0x06000883 RID: 2179 RVA: 0x0002409E File Offset: 0x0002229E
		// (set) Token: 0x06000884 RID: 2180 RVA: 0x000240A6 File Offset: 0x000222A6
		[DataSourceProperty]
		public HeroVM Sponsor
		{
			get
			{
				return this._sponsor;
			}
			set
			{
				if (value != this._sponsor)
				{
					this._sponsor = value;
					base.OnPropertyChangedWithValue<HeroVM>(value, "Sponsor");
				}
			}
		}

		// Token: 0x1700029D RID: 669
		// (get) Token: 0x06000885 RID: 2181 RVA: 0x000240C4 File Offset: 0x000222C4
		// (set) Token: 0x06000886 RID: 2182 RVA: 0x000240CC File Offset: 0x000222CC
		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					base.OnPropertyChangedWithValue<string>(value, "Name");
				}
			}
		}

		// Token: 0x1700029E RID: 670
		// (get) Token: 0x06000887 RID: 2183 RVA: 0x000240EF File Offset: 0x000222EF
		// (set) Token: 0x06000888 RID: 2184 RVA: 0x000240F7 File Offset: 0x000222F7
		[DataSourceProperty]
		public string SponsorWeightImagePath
		{
			get
			{
				return this._sponsorWeightImagePath;
			}
			set
			{
				if (value != this._sponsorWeightImagePath)
				{
					this._sponsorWeightImagePath = value;
					base.OnPropertyChangedWithValue<string>(value, "SponsorWeightImagePath");
				}
			}
		}

		// Token: 0x1700029F RID: 671
		// (get) Token: 0x06000889 RID: 2185 RVA: 0x0002411A File Offset: 0x0002231A
		// (set) Token: 0x0600088A RID: 2186 RVA: 0x00024122 File Offset: 0x00022322
		[DataSourceProperty]
		public bool CanBeChosen
		{
			get
			{
				return this._canBeChosen;
			}
			set
			{
				if (value != this._canBeChosen)
				{
					this._canBeChosen = value;
					base.OnPropertyChangedWithValue(value, "CanBeChosen");
				}
			}
		}

		// Token: 0x170002A0 RID: 672
		// (get) Token: 0x0600088B RID: 2187 RVA: 0x00024140 File Offset: 0x00022340
		// (set) Token: 0x0600088C RID: 2188 RVA: 0x00024148 File Offset: 0x00022348
		[DataSourceProperty]
		public bool IsKingsOutcome
		{
			get
			{
				return this._isKingsOutcome;
			}
			set
			{
				if (value != this._isKingsOutcome)
				{
					this._isKingsOutcome = value;
					base.OnPropertyChangedWithValue(value, "IsKingsOutcome");
				}
			}
		}

		// Token: 0x170002A1 RID: 673
		// (get) Token: 0x0600088D RID: 2189 RVA: 0x00024166 File Offset: 0x00022366
		// (set) Token: 0x0600088E RID: 2190 RVA: 0x0002416E File Offset: 0x0002236E
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

		// Token: 0x170002A2 RID: 674
		// (get) Token: 0x0600088F RID: 2191 RVA: 0x0002418C File Offset: 0x0002238C
		// (set) Token: 0x06000890 RID: 2192 RVA: 0x00024194 File Offset: 0x00022394
		[DataSourceProperty]
		public bool IsHighlightEnabled
		{
			get
			{
				return this._isHighlightEnabled;
			}
			set
			{
				if (value != this._isHighlightEnabled)
				{
					this._isHighlightEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsHighlightEnabled");
				}
			}
		}

		// Token: 0x170002A3 RID: 675
		// (get) Token: 0x06000891 RID: 2193 RVA: 0x000241B2 File Offset: 0x000223B2
		// (set) Token: 0x06000892 RID: 2194 RVA: 0x000241BA File Offset: 0x000223BA
		[DataSourceProperty]
		public int WinPercentage
		{
			get
			{
				return this._winPercentage;
			}
			set
			{
				if (value != this._winPercentage)
				{
					this._winPercentage = value;
					base.OnPropertyChangedWithValue(value, "WinPercentage");
					GameTexts.SetVariable("NUMBER", value);
					this.WinPercentageStr = GameTexts.FindText("str_NUMBER_percent", null).ToString();
				}
			}
		}

		// Token: 0x170002A4 RID: 676
		// (get) Token: 0x06000893 RID: 2195 RVA: 0x000241F9 File Offset: 0x000223F9
		// (set) Token: 0x06000894 RID: 2196 RVA: 0x00024201 File Offset: 0x00022401
		[DataSourceProperty]
		public string WinPercentageStr
		{
			get
			{
				return this._winPercentageStr;
			}
			set
			{
				if (value != this._winPercentageStr)
				{
					this._winPercentageStr = value;
					base.OnPropertyChangedWithValue<string>(value, "WinPercentageStr");
				}
			}
		}

		// Token: 0x170002A5 RID: 677
		// (get) Token: 0x06000895 RID: 2197 RVA: 0x00024224 File Offset: 0x00022424
		// (set) Token: 0x06000896 RID: 2198 RVA: 0x0002422C File Offset: 0x0002242C
		[DataSourceProperty]
		public string Description
		{
			get
			{
				return this._description;
			}
			set
			{
				if (value != this._description)
				{
					this._description = value;
					base.OnPropertyChangedWithValue<string>(value, "Description");
				}
			}
		}

		// Token: 0x170002A6 RID: 678
		// (get) Token: 0x06000897 RID: 2199 RVA: 0x0002424F File Offset: 0x0002244F
		// (set) Token: 0x06000898 RID: 2200 RVA: 0x00024257 File Offset: 0x00022457
		[DataSourceProperty]
		public int InitialPercentage
		{
			get
			{
				return this._initialPercentage;
			}
			set
			{
				if (value != this._initialPercentage)
				{
					this._initialPercentage = value;
					base.OnPropertyChangedWithValue(value, "InitialPercentage");
				}
			}
		}

		// Token: 0x170002A7 RID: 679
		// (get) Token: 0x06000899 RID: 2201 RVA: 0x00024275 File Offset: 0x00022475
		// (set) Token: 0x0600089A RID: 2202 RVA: 0x0002427D File Offset: 0x0002247D
		[DataSourceProperty]
		public int InfluenceCost
		{
			get
			{
				return this._influenceCost;
			}
			set
			{
				if (value != this._influenceCost)
				{
					this._influenceCost = value;
					base.OnPropertyChangedWithValue(value, "InfluenceCost");
				}
			}
		}

		// Token: 0x170002A8 RID: 680
		// (get) Token: 0x0600089B RID: 2203 RVA: 0x0002429B File Offset: 0x0002249B
		// (set) Token: 0x0600089C RID: 2204 RVA: 0x000242A3 File Offset: 0x000224A3
		[DataSourceProperty]
		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				if (value != this._isSelected)
				{
					this._isSelected = value;
					base.OnPropertyChangedWithValue(value, "IsSelected");
				}
			}
		}

		// Token: 0x170002A9 RID: 681
		// (get) Token: 0x0600089D RID: 2205 RVA: 0x000242C1 File Offset: 0x000224C1
		// (set) Token: 0x0600089E RID: 2206 RVA: 0x000242C9 File Offset: 0x000224C9
		[DataSourceProperty]
		public bool IsOptionForAbstain
		{
			get
			{
				return this._isOptionForAbstain;
			}
			set
			{
				if (value != this._isOptionForAbstain)
				{
					this._isOptionForAbstain = value;
					base.OnPropertyChangedWithValue(value, "IsOptionForAbstain");
				}
			}
		}

		// Token: 0x170002AA RID: 682
		// (get) Token: 0x0600089F RID: 2207 RVA: 0x000242E7 File Offset: 0x000224E7
		// (set) Token: 0x060008A0 RID: 2208 RVA: 0x000242EF File Offset: 0x000224EF
		[DataSourceProperty]
		public Supporter.SupportWeights CurrentSupportWeight
		{
			get
			{
				return this._currentSupportWeight;
			}
			set
			{
				if (value != this._currentSupportWeight)
				{
					this._currentSupportWeight = value;
					base.OnPropertyChanged("CurrentSupportWeight");
					this.CurrentSupportWeightIndex = (int)value;
				}
			}
		}

		// Token: 0x170002AB RID: 683
		// (get) Token: 0x060008A1 RID: 2209 RVA: 0x00024313 File Offset: 0x00022513
		// (set) Token: 0x060008A2 RID: 2210 RVA: 0x0002431B File Offset: 0x0002251B
		[DataSourceProperty]
		public int CurrentSupportWeightIndex
		{
			get
			{
				return this._currentSupportWeightIndex;
			}
			set
			{
				if (value != this._currentSupportWeightIndex)
				{
					this._currentSupportWeightIndex = value;
					base.OnPropertyChangedWithValue(value, "CurrentSupportWeightIndex");
				}
			}
		}

		// Token: 0x170002AC RID: 684
		// (get) Token: 0x060008A3 RID: 2211 RVA: 0x00024339 File Offset: 0x00022539
		// (set) Token: 0x060008A4 RID: 2212 RVA: 0x00024341 File Offset: 0x00022541
		[DataSourceProperty]
		public string SupportOption1Text
		{
			get
			{
				return this._supportOption1Text;
			}
			set
			{
				if (value != this._supportOption1Text)
				{
					this._supportOption1Text = value;
					base.OnPropertyChangedWithValue<string>(value, "SupportOption1Text");
				}
			}
		}

		// Token: 0x170002AD RID: 685
		// (get) Token: 0x060008A5 RID: 2213 RVA: 0x00024364 File Offset: 0x00022564
		// (set) Token: 0x060008A6 RID: 2214 RVA: 0x0002436C File Offset: 0x0002256C
		[DataSourceProperty]
		public string SupportOption2Text
		{
			get
			{
				return this._supportOption2Text;
			}
			set
			{
				if (value != this._supportOption2Text)
				{
					this._supportOption2Text = value;
					base.OnPropertyChangedWithValue<string>(value, "SupportOption2Text");
				}
			}
		}

		// Token: 0x170002AE RID: 686
		// (get) Token: 0x060008A7 RID: 2215 RVA: 0x0002438F File Offset: 0x0002258F
		// (set) Token: 0x060008A8 RID: 2216 RVA: 0x00024397 File Offset: 0x00022597
		[DataSourceProperty]
		public string SupportOption3Text
		{
			get
			{
				return this._supportOption3Text;
			}
			set
			{
				if (value != this._supportOption3Text)
				{
					this._supportOption3Text = value;
					base.OnPropertyChangedWithValue<string>(value, "SupportOption3Text");
				}
			}
		}

		// Token: 0x170002AF RID: 687
		// (get) Token: 0x060008A9 RID: 2217 RVA: 0x000243BA File Offset: 0x000225BA
		// (set) Token: 0x060008AA RID: 2218 RVA: 0x000243C2 File Offset: 0x000225C2
		[DataSourceProperty]
		public bool IsSupportOption1Enabled
		{
			get
			{
				return this._isSupportOption1Enabled;
			}
			set
			{
				if (value != this._isSupportOption1Enabled)
				{
					this._isSupportOption1Enabled = value;
					base.OnPropertyChangedWithValue(value, "IsSupportOption1Enabled");
				}
			}
		}

		// Token: 0x170002B0 RID: 688
		// (get) Token: 0x060008AB RID: 2219 RVA: 0x000243E0 File Offset: 0x000225E0
		// (set) Token: 0x060008AC RID: 2220 RVA: 0x000243E8 File Offset: 0x000225E8
		[DataSourceProperty]
		public bool IsSupportOption2Enabled
		{
			get
			{
				return this._isSupportOption2Enabled;
			}
			set
			{
				if (value != this._isSupportOption2Enabled)
				{
					this._isSupportOption2Enabled = value;
					base.OnPropertyChangedWithValue(value, "IsSupportOption2Enabled");
				}
			}
		}

		// Token: 0x170002B1 RID: 689
		// (get) Token: 0x060008AD RID: 2221 RVA: 0x00024406 File Offset: 0x00022606
		// (set) Token: 0x060008AE RID: 2222 RVA: 0x0002440E File Offset: 0x0002260E
		[DataSourceProperty]
		public bool IsSupportOption3Enabled
		{
			get
			{
				return this._isSupportOption3Enabled;
			}
			set
			{
				if (value != this._isSupportOption3Enabled)
				{
					this._isSupportOption3Enabled = value;
					base.OnPropertyChangedWithValue(value, "IsSupportOption3Enabled");
				}
			}
		}

		// Token: 0x040003CE RID: 974
		private readonly Action<DecisionOptionVM> _onSelect;

		// Token: 0x040003CF RID: 975
		private readonly Action<DecisionOptionVM> _onSupportStrengthChange;

		// Token: 0x040003D0 RID: 976
		private readonly KingdomElection _kingdomDecisionMaker;

		// Token: 0x040003D1 RID: 977
		private bool _hasKingChoosen;

		// Token: 0x040003D2 RID: 978
		private MBBindingList<DecisionSupporterVM> _supportersOfThisOption;

		// Token: 0x040003D3 RID: 979
		private HeroVM _sponsor;

		// Token: 0x040003D4 RID: 980
		private bool _isOptionForAbstain;

		// Token: 0x040003D5 RID: 981
		private bool _isPlayerSupporter;

		// Token: 0x040003D6 RID: 982
		private bool _isSelected;

		// Token: 0x040003D7 RID: 983
		private bool _canBeChosen;

		// Token: 0x040003D8 RID: 984
		private bool _isKingsOutcome;

		// Token: 0x040003D9 RID: 985
		private bool _isHighlightEnabled;

		// Token: 0x040003DA RID: 986
		private int _winPercentage = -1;

		// Token: 0x040003DB RID: 987
		private int _influenceCost;

		// Token: 0x040003DC RID: 988
		private int _initialPercentage = -99;

		// Token: 0x040003DD RID: 989
		private int _currentSupportWeightIndex;

		// Token: 0x040003DE RID: 990
		private string _name;

		// Token: 0x040003DF RID: 991
		private string _description;

		// Token: 0x040003E0 RID: 992
		private string _winPercentageStr;

		// Token: 0x040003E1 RID: 993
		private string _sponsorWeightImagePath;

		// Token: 0x040003E2 RID: 994
		private Supporter.SupportWeights _currentSupportWeight;

		// Token: 0x040003E3 RID: 995
		private string _supportOption1Text;

		// Token: 0x040003E4 RID: 996
		private bool _isSupportOption1Enabled;

		// Token: 0x040003E5 RID: 997
		private string _supportOption2Text;

		// Token: 0x040003E6 RID: 998
		private bool _isSupportOption2Enabled;

		// Token: 0x040003E7 RID: 999
		private string _supportOption3Text;

		// Token: 0x040003E8 RID: 1000
		private bool _isSupportOption3Enabled;

		// Token: 0x040003E9 RID: 1001
		private HintViewModel _optionHint;
	}
}
