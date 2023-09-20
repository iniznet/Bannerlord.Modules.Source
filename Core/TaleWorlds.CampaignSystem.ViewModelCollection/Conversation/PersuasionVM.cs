using System;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Conversation.Persuasion;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Conversation
{
	// Token: 0x020000F6 RID: 246
	public class PersuasionVM : ViewModel
	{
		// Token: 0x06001738 RID: 5944 RVA: 0x0005603F File Offset: 0x0005423F
		public PersuasionVM(ConversationManager manager)
		{
			this.PersuasionProgress = new MBBindingList<BoolItemWithActionVM>();
			this._manager = manager;
		}

		// Token: 0x06001739 RID: 5945 RVA: 0x0005605C File Offset: 0x0005425C
		public void OnPersuasionProgress(Tuple<PersuasionOptionArgs, PersuasionOptionResult> selectedOption)
		{
			this.ProgressText = "";
			string text = null;
			string text2 = null;
			switch (selectedOption.Item2)
			{
			case PersuasionOptionResult.CriticalFailure:
				text = new TextObject("{=ocSW4WA2}Critical Fail!", null).ToString();
				text2 = "<a style=\"Conversation.Persuasion.Negative\"><b>{TEXT}</b></a>";
				break;
			case PersuasionOptionResult.Failure:
			case PersuasionOptionResult.Miss:
				text = new TextObject("{=JYOcl7Ox}Ineffective!", null).ToString();
				text2 = "<a style=\"Conversation.Persuasion.Neutral\"><b>{TEXT}</b></a>";
				break;
			case PersuasionOptionResult.Success:
				text = new TextObject("{=3F0y3ugx}Success!", null).ToString();
				text2 = "<a style=\"Conversation.Persuasion.Positive\"><b>{TEXT}</b></a>";
				break;
			case PersuasionOptionResult.CriticalSuccess:
				text = new TextObject("{=4U9EnZt5}Critical Success!", null).ToString();
				text2 = "<a style=\"Conversation.Persuasion.Positive\"><b>{TEXT}</b></a>";
				break;
			}
			this.ProgressText = text2.Replace("{TEXT}", text);
		}

		// Token: 0x0600173A RID: 5946 RVA: 0x0005610F File Offset: 0x0005430F
		public override void RefreshValues()
		{
			base.RefreshValues();
			PersuasionOptionVM currentPersuasionOption = this.CurrentPersuasionOption;
			if (currentPersuasionOption == null)
			{
				return;
			}
			currentPersuasionOption.RefreshValues();
		}

		// Token: 0x0600173B RID: 5947 RVA: 0x00056127 File Offset: 0x00054327
		public void SetCurrentOption(PersuasionOptionVM option)
		{
			if (this.CurrentPersuasionOption != option)
			{
				this.CurrentPersuasionOption = option;
			}
		}

		// Token: 0x0600173C RID: 5948 RVA: 0x0005613C File Offset: 0x0005433C
		public void RefreshPersusasion()
		{
			this.CurrentCritFailChance = 0;
			this.CurrentFailChance = 0;
			this.CurrentCritSuccessChance = 0;
			this.CurrentSuccessChance = 0;
			this.IsPersuasionActive = ConversationManager.GetPersuasionIsActive();
			this.PersuasionProgress.Clear();
			this.PersuasionHint = new BasicTooltipViewModel();
			if (this.IsPersuasionActive)
			{
				int num = (int)ConversationManager.GetPersuasionProgress();
				int num2 = (int)ConversationManager.GetPersuasionGoalValue();
				for (int i = 1; i <= num2; i++)
				{
					bool flag = i <= num;
					this.PersuasionProgress.Add(new BoolItemWithActionVM(null, flag, null));
				}
				if (this.CurrentPersuasionOption != null)
				{
					this.CurrentCritFailChance = this._currentPersuasionOption.CritFailChance;
					this.CurrentFailChance = this._currentPersuasionOption.FailChance;
					this.CurrentCritSuccessChance = this._currentPersuasionOption.CritSuccessChance;
					this.CurrentSuccessChance = this._currentPersuasionOption.SuccessChance;
				}
				this.PersuasionHint = new BasicTooltipViewModel(() => this.GetPersuasionTooltip());
			}
		}

		// Token: 0x0600173D RID: 5949 RVA: 0x00056229 File Offset: 0x00054429
		private string GetPersuasionTooltip()
		{
			if (ConversationManager.GetPersuasionIsActive())
			{
				GameTexts.SetVariable("CURRENT_PROGRESS", (int)ConversationManager.GetPersuasionProgress());
				GameTexts.SetVariable("TARGET_PROGRESS", (int)ConversationManager.GetPersuasionGoalValue());
				return GameTexts.FindText("str_persuasion_tooltip", null).ToString();
			}
			return "";
		}

		// Token: 0x0600173E RID: 5950 RVA: 0x00056268 File Offset: 0x00054468
		private void RefreshChangeValues()
		{
			float num;
			float num2;
			float num3;
			this._manager.GetPersuasionChanceValues(out num, out num2, out num3);
		}

		// Token: 0x170007DE RID: 2014
		// (get) Token: 0x0600173F RID: 5951 RVA: 0x00056286 File Offset: 0x00054486
		// (set) Token: 0x06001740 RID: 5952 RVA: 0x0005628E File Offset: 0x0005448E
		[DataSourceProperty]
		public BasicTooltipViewModel PersuasionHint
		{
			get
			{
				return this._persuasionHint;
			}
			set
			{
				if (this._persuasionHint != value)
				{
					this._persuasionHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "PersuasionHint");
				}
			}
		}

		// Token: 0x170007DF RID: 2015
		// (get) Token: 0x06001741 RID: 5953 RVA: 0x000562AC File Offset: 0x000544AC
		// (set) Token: 0x06001742 RID: 5954 RVA: 0x000562B4 File Offset: 0x000544B4
		[DataSourceProperty]
		public string ProgressText
		{
			get
			{
				return this._progressText;
			}
			set
			{
				if (this._progressText != value)
				{
					this._progressText = value;
					base.OnPropertyChangedWithValue<string>(value, "ProgressText");
				}
			}
		}

		// Token: 0x170007E0 RID: 2016
		// (get) Token: 0x06001743 RID: 5955 RVA: 0x000562D7 File Offset: 0x000544D7
		// (set) Token: 0x06001744 RID: 5956 RVA: 0x000562DF File Offset: 0x000544DF
		[DataSourceProperty]
		public MBBindingList<BoolItemWithActionVM> PersuasionProgress
		{
			get
			{
				return this._persuasionProgress;
			}
			set
			{
				if (value != this._persuasionProgress)
				{
					this._persuasionProgress = value;
					base.OnPropertyChangedWithValue<MBBindingList<BoolItemWithActionVM>>(value, "PersuasionProgress");
				}
			}
		}

		// Token: 0x170007E1 RID: 2017
		// (get) Token: 0x06001745 RID: 5957 RVA: 0x000562FD File Offset: 0x000544FD
		// (set) Token: 0x06001746 RID: 5958 RVA: 0x00056305 File Offset: 0x00054505
		[DataSourceProperty]
		public bool IsPersuasionActive
		{
			get
			{
				return this._isPersuasionActive;
			}
			set
			{
				if (value != this._isPersuasionActive)
				{
					if (value)
					{
						this.RefreshChangeValues();
					}
					this._isPersuasionActive = value;
					base.OnPropertyChangedWithValue(value, "IsPersuasionActive");
				}
			}
		}

		// Token: 0x170007E2 RID: 2018
		// (get) Token: 0x06001747 RID: 5959 RVA: 0x0005632C File Offset: 0x0005452C
		// (set) Token: 0x06001748 RID: 5960 RVA: 0x00056334 File Offset: 0x00054534
		[DataSourceProperty]
		public int CurrentSuccessChance
		{
			get
			{
				return this._currentSuccessChance;
			}
			set
			{
				if (this._currentSuccessChance != value)
				{
					this._currentSuccessChance = value;
					base.OnPropertyChangedWithValue(value, "CurrentSuccessChance");
				}
			}
		}

		// Token: 0x170007E3 RID: 2019
		// (get) Token: 0x06001749 RID: 5961 RVA: 0x00056352 File Offset: 0x00054552
		// (set) Token: 0x0600174A RID: 5962 RVA: 0x0005635A File Offset: 0x0005455A
		[DataSourceProperty]
		public PersuasionOptionVM CurrentPersuasionOption
		{
			get
			{
				return this._currentPersuasionOption;
			}
			set
			{
				if (this._currentPersuasionOption != value)
				{
					this._currentPersuasionOption = value;
					base.OnPropertyChangedWithValue<PersuasionOptionVM>(value, "CurrentPersuasionOption");
				}
			}
		}

		// Token: 0x170007E4 RID: 2020
		// (get) Token: 0x0600174B RID: 5963 RVA: 0x00056378 File Offset: 0x00054578
		// (set) Token: 0x0600174C RID: 5964 RVA: 0x00056380 File Offset: 0x00054580
		[DataSourceProperty]
		public int CurrentFailChance
		{
			get
			{
				return this._currentFailChance;
			}
			set
			{
				if (this._currentFailChance != value)
				{
					this._currentFailChance = value;
					base.OnPropertyChangedWithValue(value, "CurrentFailChance");
				}
			}
		}

		// Token: 0x170007E5 RID: 2021
		// (get) Token: 0x0600174D RID: 5965 RVA: 0x0005639E File Offset: 0x0005459E
		// (set) Token: 0x0600174E RID: 5966 RVA: 0x000563A6 File Offset: 0x000545A6
		[DataSourceProperty]
		public int CurrentCritSuccessChance
		{
			get
			{
				return this._currentCritSuccessChance;
			}
			set
			{
				if (this._currentCritSuccessChance != value)
				{
					this._currentCritSuccessChance = value;
					base.OnPropertyChangedWithValue(value, "CurrentCritSuccessChance");
				}
			}
		}

		// Token: 0x170007E6 RID: 2022
		// (get) Token: 0x0600174F RID: 5967 RVA: 0x000563C4 File Offset: 0x000545C4
		// (set) Token: 0x06001750 RID: 5968 RVA: 0x000563CC File Offset: 0x000545CC
		[DataSourceProperty]
		public int CurrentCritFailChance
		{
			get
			{
				return this._currentCritFailChance;
			}
			set
			{
				if (this._currentCritFailChance != value)
				{
					this._currentCritFailChance = value;
					base.OnPropertyChangedWithValue(value, "CurrentCritFailChance");
				}
			}
		}

		// Token: 0x04000AE2 RID: 2786
		internal const string PositiveText = "<a style=\"Conversation.Persuasion.Positive\"><b>{TEXT}</b></a>";

		// Token: 0x04000AE3 RID: 2787
		internal const string NegativeText = "<a style=\"Conversation.Persuasion.Negative\"><b>{TEXT}</b></a>";

		// Token: 0x04000AE4 RID: 2788
		internal const string NeutralText = "<a style=\"Conversation.Persuasion.Neutral\"><b>{TEXT}</b></a>";

		// Token: 0x04000AE5 RID: 2789
		private ConversationManager _manager;

		// Token: 0x04000AE6 RID: 2790
		private MBBindingList<BoolItemWithActionVM> _persuasionProgress;

		// Token: 0x04000AE7 RID: 2791
		private bool _isPersuasionActive;

		// Token: 0x04000AE8 RID: 2792
		private int _currentCritFailChance;

		// Token: 0x04000AE9 RID: 2793
		private int _currentFailChance;

		// Token: 0x04000AEA RID: 2794
		private int _currentSuccessChance;

		// Token: 0x04000AEB RID: 2795
		private int _currentCritSuccessChance;

		// Token: 0x04000AEC RID: 2796
		private string _progressText;

		// Token: 0x04000AED RID: 2797
		private PersuasionOptionVM _currentPersuasionOption;

		// Token: 0x04000AEE RID: 2798
		private BasicTooltipViewModel _persuasionHint;
	}
}
