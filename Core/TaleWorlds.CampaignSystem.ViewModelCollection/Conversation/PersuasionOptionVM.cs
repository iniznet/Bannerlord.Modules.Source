using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Conversation.Persuasion;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Conversation
{
	// Token: 0x020000F5 RID: 245
	public class PersuasionOptionVM : ViewModel
	{
		// Token: 0x0600170B RID: 5899 RVA: 0x00055768 File Offset: 0x00053968
		public PersuasionOptionVM(ConversationManager manager, ConversationSentenceOption option, Action onReadyToContinue)
		{
			this._option = option;
			this._manager = manager;
			this._onReadyToContinue = onReadyToContinue;
			if (ConversationManager.GetPersuasionIsActive() && this._option.HasPersuasion)
			{
				float num;
				float num2;
				float num3;
				float num4;
				this._manager.GetPersuasionChances(this._option, out num, out num2, out num3, out num4);
				this.CritFailChance = (int)(num3 * 100f);
				this.FailChance = (int)(num4 * 100f);
				this.SuccessChance = (int)(num * 100f);
				this.CritSuccessChance = (int)(num2 * 100f);
				this._args = option.PersuationOptionArgs;
			}
			this.RefreshValues();
		}

		// Token: 0x0600170C RID: 5900 RVA: 0x00055810 File Offset: 0x00053A10
		public override void RefreshValues()
		{
			base.RefreshValues();
			if (ConversationManager.GetPersuasionIsActive() && this._option.HasPersuasion)
			{
				GameTexts.SetVariable("NUMBER", this.CritFailChance);
				this.CritFailChanceText = GameTexts.FindText("str_NUMBER_percent", null).ToString();
				GameTexts.SetVariable("NUMBER", this.FailChance);
				this.FailChanceText = GameTexts.FindText("str_NUMBER_percent", null).ToString();
				GameTexts.SetVariable("NUMBER", this.SuccessChance);
				this.SuccessChanceText = GameTexts.FindText("str_NUMBER_percent", null).ToString();
				GameTexts.SetVariable("NUMBER", this.CritSuccessChance);
				this.CritSuccessChanceText = GameTexts.FindText("str_NUMBER_percent", null).ToString();
				this.CritFailHint = new BasicTooltipViewModel(delegate
				{
					GameTexts.SetVariable("LEFT", GameTexts.FindText("str_persuasion_critical_fail", null));
					GameTexts.SetVariable("NUMBER", this.CritFailChance);
					GameTexts.SetVariable("RIGHT", GameTexts.FindText("str_NUMBER_percent", null));
					return GameTexts.FindText("str_LEFT_colon_RIGHT_wSpaceAfterColon", null).ToString();
				});
				this.FailHint = new BasicTooltipViewModel(delegate
				{
					GameTexts.SetVariable("LEFT", GameTexts.FindText("str_persuasion_fail", null));
					GameTexts.SetVariable("NUMBER", this.FailChance);
					GameTexts.SetVariable("RIGHT", GameTexts.FindText("str_NUMBER_percent", null));
					return GameTexts.FindText("str_LEFT_colon_RIGHT_wSpaceAfterColon", null).ToString();
				});
				this.SuccessHint = new BasicTooltipViewModel(delegate
				{
					GameTexts.SetVariable("LEFT", GameTexts.FindText("str_persuasion_success", null));
					GameTexts.SetVariable("NUMBER", this.SuccessChance);
					GameTexts.SetVariable("RIGHT", GameTexts.FindText("str_NUMBER_percent", null));
					return GameTexts.FindText("str_LEFT_colon_RIGHT_wSpaceAfterColon", null).ToString();
				});
				this.CritSuccessHint = new BasicTooltipViewModel(delegate
				{
					GameTexts.SetVariable("LEFT", GameTexts.FindText("str_persuasion_critical_success", null));
					GameTexts.SetVariable("NUMBER", this.CritSuccessChance);
					GameTexts.SetVariable("RIGHT", GameTexts.FindText("str_NUMBER_percent", null));
					return GameTexts.FindText("str_LEFT_colon_RIGHT_wSpaceAfterColon", null).ToString();
				});
				this.ProgressingOptionHint = new HintViewModel(GameTexts.FindText("str_persuasion_progressing_hint", null), null);
				this.BlockingOptionHint = new HintViewModel(GameTexts.FindText("str_persuasion_blocking_hint", null), null);
				this.IsABlockingOption = this._args.CanBlockOtherOption;
				this.IsAProgressingOption = this._args.CanMoveToTheNextReservation;
			}
		}

		// Token: 0x0600170D RID: 5901 RVA: 0x00055981 File Offset: 0x00053B81
		internal void OnPersuasionProgress(Tuple<PersuasionOptionArgs, PersuasionOptionResult> result)
		{
			this.IsPersuasionResultReady = true;
			if (result.Item1 == this._args)
			{
				this.PersuasionResultIndex = (int)result.Item2;
			}
		}

		// Token: 0x0600170E RID: 5902 RVA: 0x000559A4 File Offset: 0x00053BA4
		public string GetPersuasionAdditionalText()
		{
			string text = null;
			if (this._args != null)
			{
				if (this._args.SkillUsed != null)
				{
					text = ((Hero.MainHero.GetSkillValue(this._args.SkillUsed) <= 50) ? "<a style=\"Conversation.Persuasion.Neutral\"><b>{TEXT}</b></a>" : "<a style=\"Conversation.Persuasion.Positive\"><b>{TEXT}</b></a>").Replace("{TEXT}", this._args.SkillUsed.Name.ToString());
				}
				if (this._args.TraitUsed != null && !this._args.TraitUsed.IsHidden)
				{
					int traitLevel = Hero.MainHero.GetTraitLevel(this._args.TraitUsed);
					string text2;
					if (traitLevel == 0)
					{
						text2 = "<a style=\"Conversation.Persuasion.Neutral\"><b>{TEXT}</b></a>";
					}
					else
					{
						text2 = (((traitLevel > 0 && this._args.TraitEffect == TraitEffect.Positive) || (traitLevel < 0 && this._args.TraitEffect == TraitEffect.Negative)) ? "<a style=\"Conversation.Persuasion.Positive\"><b>{TEXT}</b></a>" : "<a style=\"Conversation.Persuasion.Negative\"><b>{TEXT}</b></a>");
					}
					text2 = text2.Replace("{TEXT}", this._args.TraitUsed.Name.ToString());
					if (text != null)
					{
						GameTexts.SetVariable("LEFT", text);
						GameTexts.SetVariable("RIGHT", text2);
						text = GameTexts.FindText("str_LEFT_comma_RIGHT", null).ToString();
					}
					else
					{
						text = text2;
					}
				}
				if (this._args.TraitCorrelation != null)
				{
					foreach (Tuple<TraitObject, int> tuple in this._args.TraitCorrelation)
					{
						if (tuple.Item2 != 0 && this._args.TraitUsed != tuple.Item1 && !tuple.Item1.IsHidden)
						{
							int traitLevel2 = Hero.MainHero.GetTraitLevel(tuple.Item1);
							string text3;
							if (traitLevel2 == 0)
							{
								text3 = "<a style=\"Conversation.Persuasion.Neutral\"><b>{TEXT}</b></a>";
							}
							else
							{
								text3 = ((traitLevel2 * tuple.Item2 > 0) ? "<a style=\"Conversation.Persuasion.Positive\"><b>{TEXT}</b></a>" : "<a style=\"Conversation.Persuasion.Negative\"><b>{TEXT}</b></a>");
							}
							text3 = text3.Replace("{TEXT}", tuple.Item1.Name.ToString());
							if (text != null)
							{
								GameTexts.SetVariable("LEFT", text);
								GameTexts.SetVariable("RIGHT", text3);
								text = GameTexts.FindText("str_LEFT_comma_RIGHT", null).ToString();
							}
							else
							{
								text = text3;
							}
						}
					}
				}
			}
			if (!string.IsNullOrEmpty(text))
			{
				GameTexts.SetVariable("STR", text);
				return GameTexts.FindText("str_STR_in_parentheses", null).ToString();
			}
			return string.Empty;
		}

		// Token: 0x0600170F RID: 5903 RVA: 0x00055C05 File Offset: 0x00053E05
		public void ExecuteReadyToContinue()
		{
			Action onReadyToContinue = this._onReadyToContinue;
			if (onReadyToContinue == null)
			{
				return;
			}
			onReadyToContinue.DynamicInvokeWithLog(Array.Empty<object>());
		}

		// Token: 0x170007CC RID: 1996
		// (get) Token: 0x06001710 RID: 5904 RVA: 0x00055C1D File Offset: 0x00053E1D
		// (set) Token: 0x06001711 RID: 5905 RVA: 0x00055C25 File Offset: 0x00053E25
		[DataSourceProperty]
		public bool IsPersuasionResultReady
		{
			get
			{
				return this._isPersuasionResultReady;
			}
			set
			{
				if (this._isPersuasionResultReady != value)
				{
					this._isPersuasionResultReady = value;
					base.OnPropertyChangedWithValue(value, "IsPersuasionResultReady");
				}
			}
		}

		// Token: 0x170007CD RID: 1997
		// (get) Token: 0x06001712 RID: 5906 RVA: 0x00055C43 File Offset: 0x00053E43
		// (set) Token: 0x06001713 RID: 5907 RVA: 0x00055C4B File Offset: 0x00053E4B
		[DataSourceProperty]
		public bool IsABlockingOption
		{
			get
			{
				return this._isABlockingOption;
			}
			set
			{
				if (this._isABlockingOption != value)
				{
					this._isABlockingOption = value;
					base.OnPropertyChangedWithValue(value, "IsABlockingOption");
				}
			}
		}

		// Token: 0x170007CE RID: 1998
		// (get) Token: 0x06001714 RID: 5908 RVA: 0x00055C69 File Offset: 0x00053E69
		// (set) Token: 0x06001715 RID: 5909 RVA: 0x00055C71 File Offset: 0x00053E71
		[DataSourceProperty]
		public bool IsAProgressingOption
		{
			get
			{
				return this._isAProgressingOption;
			}
			set
			{
				if (this._isAProgressingOption != value)
				{
					this._isAProgressingOption = value;
					base.OnPropertyChangedWithValue(value, "IsAProgressingOption");
				}
			}
		}

		// Token: 0x170007CF RID: 1999
		// (get) Token: 0x06001716 RID: 5910 RVA: 0x00055C8F File Offset: 0x00053E8F
		// (set) Token: 0x06001717 RID: 5911 RVA: 0x00055C97 File Offset: 0x00053E97
		[DataSourceProperty]
		public int SuccessChance
		{
			get
			{
				return this._successChance;
			}
			set
			{
				if (this._successChance != value)
				{
					this._successChance = value;
					base.OnPropertyChangedWithValue(value, "SuccessChance");
				}
			}
		}

		// Token: 0x170007D0 RID: 2000
		// (get) Token: 0x06001718 RID: 5912 RVA: 0x00055CB5 File Offset: 0x00053EB5
		// (set) Token: 0x06001719 RID: 5913 RVA: 0x00055CBD File Offset: 0x00053EBD
		[DataSourceProperty]
		public int PersuasionResultIndex
		{
			get
			{
				return this._persuasionResultIndex;
			}
			set
			{
				if (this._persuasionResultIndex != value)
				{
					this._persuasionResultIndex = value;
					base.OnPropertyChangedWithValue(value, "PersuasionResultIndex");
				}
			}
		}

		// Token: 0x170007D1 RID: 2001
		// (get) Token: 0x0600171A RID: 5914 RVA: 0x00055CDB File Offset: 0x00053EDB
		// (set) Token: 0x0600171B RID: 5915 RVA: 0x00055CE3 File Offset: 0x00053EE3
		[DataSourceProperty]
		public int FailChance
		{
			get
			{
				return this._failChance;
			}
			set
			{
				if (this._failChance != value)
				{
					this._failChance = value;
					base.OnPropertyChangedWithValue(value, "FailChance");
				}
			}
		}

		// Token: 0x170007D2 RID: 2002
		// (get) Token: 0x0600171C RID: 5916 RVA: 0x00055D01 File Offset: 0x00053F01
		// (set) Token: 0x0600171D RID: 5917 RVA: 0x00055D09 File Offset: 0x00053F09
		[DataSourceProperty]
		public int CritSuccessChance
		{
			get
			{
				return this._critSuccessChance;
			}
			set
			{
				if (this._critSuccessChance != value)
				{
					this._critSuccessChance = value;
					base.OnPropertyChangedWithValue(value, "CritSuccessChance");
				}
			}
		}

		// Token: 0x170007D3 RID: 2003
		// (get) Token: 0x0600171E RID: 5918 RVA: 0x00055D27 File Offset: 0x00053F27
		// (set) Token: 0x0600171F RID: 5919 RVA: 0x00055D2F File Offset: 0x00053F2F
		[DataSourceProperty]
		public int CritFailChance
		{
			get
			{
				return this._critFailChance;
			}
			set
			{
				if (this._critFailChance != value)
				{
					this._critFailChance = value;
					base.OnPropertyChangedWithValue(value, "CritFailChance");
				}
			}
		}

		// Token: 0x170007D4 RID: 2004
		// (get) Token: 0x06001720 RID: 5920 RVA: 0x00055D4D File Offset: 0x00053F4D
		// (set) Token: 0x06001721 RID: 5921 RVA: 0x00055D55 File Offset: 0x00053F55
		[DataSourceProperty]
		public string FailChanceText
		{
			get
			{
				return this._failChanceText;
			}
			set
			{
				if (this._failChanceText != value)
				{
					this._failChanceText = value;
					base.OnPropertyChangedWithValue<string>(value, "FailChanceText");
				}
			}
		}

		// Token: 0x170007D5 RID: 2005
		// (get) Token: 0x06001722 RID: 5922 RVA: 0x00055D78 File Offset: 0x00053F78
		// (set) Token: 0x06001723 RID: 5923 RVA: 0x00055D80 File Offset: 0x00053F80
		[DataSourceProperty]
		public string CritFailChanceText
		{
			get
			{
				return this._critFailChanceText;
			}
			set
			{
				if (this._critFailChanceText != value)
				{
					this._critFailChanceText = value;
					base.OnPropertyChangedWithValue<string>(value, "CritFailChanceText");
				}
			}
		}

		// Token: 0x170007D6 RID: 2006
		// (get) Token: 0x06001724 RID: 5924 RVA: 0x00055DA3 File Offset: 0x00053FA3
		// (set) Token: 0x06001725 RID: 5925 RVA: 0x00055DAB File Offset: 0x00053FAB
		[DataSourceProperty]
		public string SuccessChanceText
		{
			get
			{
				return this._successChanceText;
			}
			set
			{
				if (this._successChanceText != value)
				{
					this._successChanceText = value;
					base.OnPropertyChangedWithValue<string>(value, "SuccessChanceText");
				}
			}
		}

		// Token: 0x170007D7 RID: 2007
		// (get) Token: 0x06001726 RID: 5926 RVA: 0x00055DCE File Offset: 0x00053FCE
		// (set) Token: 0x06001727 RID: 5927 RVA: 0x00055DD6 File Offset: 0x00053FD6
		[DataSourceProperty]
		public string CritSuccessChanceText
		{
			get
			{
				return this._critSuccessChanceText;
			}
			set
			{
				if (this._critSuccessChanceText != value)
				{
					this._critSuccessChanceText = value;
					base.OnPropertyChangedWithValue<string>(value, "CritSuccessChanceText");
				}
			}
		}

		// Token: 0x170007D8 RID: 2008
		// (get) Token: 0x06001728 RID: 5928 RVA: 0x00055DF9 File Offset: 0x00053FF9
		// (set) Token: 0x06001729 RID: 5929 RVA: 0x00055E01 File Offset: 0x00054001
		[DataSourceProperty]
		public BasicTooltipViewModel CritFailHint
		{
			get
			{
				return this._critFailHint;
			}
			set
			{
				if (this._critFailHint != value)
				{
					this._critFailHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "CritFailHint");
				}
			}
		}

		// Token: 0x170007D9 RID: 2009
		// (get) Token: 0x0600172A RID: 5930 RVA: 0x00055E1F File Offset: 0x0005401F
		// (set) Token: 0x0600172B RID: 5931 RVA: 0x00055E27 File Offset: 0x00054027
		[DataSourceProperty]
		public BasicTooltipViewModel FailHint
		{
			get
			{
				return this._failHint;
			}
			set
			{
				if (this._failHint != value)
				{
					this._failHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "FailHint");
				}
			}
		}

		// Token: 0x170007DA RID: 2010
		// (get) Token: 0x0600172C RID: 5932 RVA: 0x00055E45 File Offset: 0x00054045
		// (set) Token: 0x0600172D RID: 5933 RVA: 0x00055E4D File Offset: 0x0005404D
		[DataSourceProperty]
		public BasicTooltipViewModel SuccessHint
		{
			get
			{
				return this._successHint;
			}
			set
			{
				if (this._successHint != value)
				{
					this._successHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "SuccessHint");
				}
			}
		}

		// Token: 0x170007DB RID: 2011
		// (get) Token: 0x0600172E RID: 5934 RVA: 0x00055E6B File Offset: 0x0005406B
		// (set) Token: 0x0600172F RID: 5935 RVA: 0x00055E73 File Offset: 0x00054073
		[DataSourceProperty]
		public BasicTooltipViewModel CritSuccessHint
		{
			get
			{
				return this._critSuccessHint;
			}
			set
			{
				if (this._critSuccessHint != value)
				{
					this._critSuccessHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "CritSuccessHint");
				}
			}
		}

		// Token: 0x170007DC RID: 2012
		// (get) Token: 0x06001730 RID: 5936 RVA: 0x00055E91 File Offset: 0x00054091
		// (set) Token: 0x06001731 RID: 5937 RVA: 0x00055E99 File Offset: 0x00054099
		[DataSourceProperty]
		public HintViewModel BlockingOptionHint
		{
			get
			{
				return this._blockingOptionHint;
			}
			set
			{
				if (this._blockingOptionHint != value)
				{
					this._blockingOptionHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "BlockingOptionHint");
				}
			}
		}

		// Token: 0x170007DD RID: 2013
		// (get) Token: 0x06001732 RID: 5938 RVA: 0x00055EB7 File Offset: 0x000540B7
		// (set) Token: 0x06001733 RID: 5939 RVA: 0x00055EBF File Offset: 0x000540BF
		[DataSourceProperty]
		public HintViewModel ProgressingOptionHint
		{
			get
			{
				return this._progressingOptionHint;
			}
			set
			{
				if (this._progressingOptionHint != value)
				{
					this._progressingOptionHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "ProgressingOptionHint");
				}
			}
		}

		// Token: 0x04000ACB RID: 2763
		private const int _minSkillValueForPositive = 50;

		// Token: 0x04000ACC RID: 2764
		private ConversationSentenceOption _option;

		// Token: 0x04000ACD RID: 2765
		private readonly ConversationManager _manager;

		// Token: 0x04000ACE RID: 2766
		private readonly PersuasionOptionArgs _args;

		// Token: 0x04000ACF RID: 2767
		private readonly Action _onReadyToContinue;

		// Token: 0x04000AD0 RID: 2768
		private int _critFailChance;

		// Token: 0x04000AD1 RID: 2769
		private int _failChance;

		// Token: 0x04000AD2 RID: 2770
		private int _successChance;

		// Token: 0x04000AD3 RID: 2771
		private int _critSuccessChance;

		// Token: 0x04000AD4 RID: 2772
		private bool _isPersuasionResultReady;

		// Token: 0x04000AD5 RID: 2773
		private int _persuasionResultIndex = -1;

		// Token: 0x04000AD6 RID: 2774
		private bool _isABlockingOption;

		// Token: 0x04000AD7 RID: 2775
		private bool _isAProgressingOption;

		// Token: 0x04000AD8 RID: 2776
		private string _critFailChanceText;

		// Token: 0x04000AD9 RID: 2777
		private string _failChanceText;

		// Token: 0x04000ADA RID: 2778
		private string _successChanceText;

		// Token: 0x04000ADB RID: 2779
		private string _critSuccessChanceText;

		// Token: 0x04000ADC RID: 2780
		private BasicTooltipViewModel _critFailHint;

		// Token: 0x04000ADD RID: 2781
		private BasicTooltipViewModel _failHint;

		// Token: 0x04000ADE RID: 2782
		private BasicTooltipViewModel _successHint;

		// Token: 0x04000ADF RID: 2783
		private BasicTooltipViewModel _critSuccessHint;

		// Token: 0x04000AE0 RID: 2784
		private HintViewModel _progressingOptionHint;

		// Token: 0x04000AE1 RID: 2785
		private HintViewModel _blockingOptionHint;
	}
}
