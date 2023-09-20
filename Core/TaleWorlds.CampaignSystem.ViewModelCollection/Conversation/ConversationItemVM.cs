using System;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Conversation.Persuasion;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Conversation
{
	// Token: 0x020000F3 RID: 243
	public class ConversationItemVM : ViewModel
	{
		// Token: 0x060016A6 RID: 5798 RVA: 0x00054334 File Offset: 0x00052534
		public ConversationItemVM(Action<int> action, Action onReadyToContinue, Action<ConversationItemVM> setCurrentAnswer, int index, ConversationSentenceOption option)
		{
			this.ActionWihIntIndex = action;
			this._option = option;
			this.Index = index;
			this._onReadyToContinue = onReadyToContinue;
			this.IsEnabled = this._option.IsClickable;
			this.HasPersuasion = this._option.HasPersuasion;
			this._setCurrentAnswer = setCurrentAnswer;
			this.PersuasionItem = new PersuasionOptionVM(Campaign.Current.ConversationManager, option, new Action(this.OnReadyToContinue));
			this.IsSpecial = this._option.IsSpecial;
			this.RefreshValues();
		}

		// Token: 0x060016A7 RID: 5799 RVA: 0x000543C8 File Offset: 0x000525C8
		private void OnReadyToContinue()
		{
			this._onReadyToContinue.DynamicInvokeWithLog(Array.Empty<object>());
		}

		// Token: 0x060016A8 RID: 5800 RVA: 0x000543DB File Offset: 0x000525DB
		public ConversationItemVM()
		{
			this.Index = 0;
			this.ItemText = "";
			this.IsEnabled = false;
			this.OptionHint = new HintViewModel();
			this.HasPersuasion = false;
			this._setCurrentAnswer = null;
		}

		// Token: 0x060016A9 RID: 5801 RVA: 0x00054418 File Offset: 0x00052618
		public override void RefreshValues()
		{
			base.RefreshValues();
			TextObject text = this._option.Text;
			string text2 = ((text != null) ? text.ToString() : null) ?? "";
			this.OptionHint = new HintViewModel((this._option.HintText != null) ? this._option.HintText : TextObject.Empty, null);
			PersuasionOptionVM persuasionItem = this.PersuasionItem;
			if (persuasionItem != null)
			{
				persuasionItem.RefreshValues();
			}
			if (this.PersuasionItem != null)
			{
				string persuasionAdditionalText = this.PersuasionItem.GetPersuasionAdditionalText();
				if (!string.IsNullOrEmpty(persuasionAdditionalText))
				{
					GameTexts.SetVariable("STR1", text2);
					GameTexts.SetVariable("STR2", persuasionAdditionalText);
					text2 = GameTexts.FindText("str_STR1_space_STR2", null).ToString();
				}
			}
			this.ItemText = text2;
		}

		// Token: 0x060016AA RID: 5802 RVA: 0x000544D2 File Offset: 0x000526D2
		public void ExecuteAction()
		{
			Action<int> actionWihIntIndex = this.ActionWihIntIndex;
			if (actionWihIntIndex == null)
			{
				return;
			}
			actionWihIntIndex(this.Index);
		}

		// Token: 0x060016AB RID: 5803 RVA: 0x000544EA File Offset: 0x000526EA
		public void SetCurrentAnswer()
		{
			Action<ConversationItemVM> setCurrentAnswer = this._setCurrentAnswer;
			if (setCurrentAnswer == null)
			{
				return;
			}
			setCurrentAnswer(this);
		}

		// Token: 0x060016AC RID: 5804 RVA: 0x000544FD File Offset: 0x000526FD
		public void ResetCurrentAnswer()
		{
			this._setCurrentAnswer(null);
		}

		// Token: 0x060016AD RID: 5805 RVA: 0x0005450B File Offset: 0x0005270B
		internal void OnPersuasionProgress(Tuple<PersuasionOptionArgs, PersuasionOptionResult> result)
		{
			PersuasionOptionVM persuasionItem = this.PersuasionItem;
			if (persuasionItem == null)
			{
				return;
			}
			persuasionItem.OnPersuasionProgress(result);
		}

		// Token: 0x170007A7 RID: 1959
		// (get) Token: 0x060016AE RID: 5806 RVA: 0x0005451E File Offset: 0x0005271E
		// (set) Token: 0x060016AF RID: 5807 RVA: 0x00054526 File Offset: 0x00052726
		[DataSourceProperty]
		public PersuasionOptionVM PersuasionItem
		{
			get
			{
				return this._persuasionItem;
			}
			set
			{
				if (this._persuasionItem != value)
				{
					this._persuasionItem = value;
					base.OnPropertyChangedWithValue<PersuasionOptionVM>(value, "PersuasionItem");
				}
			}
		}

		// Token: 0x170007A8 RID: 1960
		// (get) Token: 0x060016B0 RID: 5808 RVA: 0x00054544 File Offset: 0x00052744
		// (set) Token: 0x060016B1 RID: 5809 RVA: 0x0005454C File Offset: 0x0005274C
		[DataSourceProperty]
		public bool HasPersuasion
		{
			get
			{
				return this._hasPersuasion;
			}
			set
			{
				if (this._hasPersuasion != value)
				{
					this._hasPersuasion = value;
					base.OnPropertyChangedWithValue(value, "HasPersuasion");
				}
			}
		}

		// Token: 0x170007A9 RID: 1961
		// (get) Token: 0x060016B2 RID: 5810 RVA: 0x0005456A File Offset: 0x0005276A
		// (set) Token: 0x060016B3 RID: 5811 RVA: 0x00054572 File Offset: 0x00052772
		[DataSourceProperty]
		public int IconType
		{
			get
			{
				return this._iconType;
			}
			set
			{
				if (this._iconType != value)
				{
					this._iconType = value;
					base.OnPropertyChangedWithValue(value, "IconType");
				}
			}
		}

		// Token: 0x170007AA RID: 1962
		// (get) Token: 0x060016B4 RID: 5812 RVA: 0x00054590 File Offset: 0x00052790
		// (set) Token: 0x060016B5 RID: 5813 RVA: 0x00054598 File Offset: 0x00052798
		[DataSourceProperty]
		public HintViewModel OptionHint
		{
			get
			{
				return this._optionHint;
			}
			set
			{
				if (this._optionHint != value)
				{
					this._optionHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "OptionHint");
				}
			}
		}

		// Token: 0x170007AB RID: 1963
		// (get) Token: 0x060016B6 RID: 5814 RVA: 0x000545B6 File Offset: 0x000527B6
		// (set) Token: 0x060016B7 RID: 5815 RVA: 0x000545BE File Offset: 0x000527BE
		[DataSourceProperty]
		public string ItemText
		{
			get
			{
				return this._itemText;
			}
			set
			{
				if (this._itemText != value)
				{
					this._itemText = value;
					base.OnPropertyChangedWithValue<string>(value, "ItemText");
				}
			}
		}

		// Token: 0x170007AC RID: 1964
		// (get) Token: 0x060016B8 RID: 5816 RVA: 0x000545E1 File Offset: 0x000527E1
		// (set) Token: 0x060016B9 RID: 5817 RVA: 0x000545E9 File Offset: 0x000527E9
		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (this._isEnabled != value)
				{
					this._isEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEnabled");
				}
			}
		}

		// Token: 0x170007AD RID: 1965
		// (get) Token: 0x060016BA RID: 5818 RVA: 0x00054607 File Offset: 0x00052807
		// (set) Token: 0x060016BB RID: 5819 RVA: 0x0005460F File Offset: 0x0005280F
		[DataSourceProperty]
		public bool IsSpecial
		{
			get
			{
				return this._isSpecial;
			}
			set
			{
				if (this._isSpecial != value)
				{
					this._isSpecial = value;
					base.OnPropertyChangedWithValue(value, "IsSpecial");
				}
			}
		}

		// Token: 0x04000A9B RID: 2715
		public Action<int> ActionWihIntIndex;

		// Token: 0x04000A9C RID: 2716
		public Action<ConversationItemVM> _setCurrentAnswer;

		// Token: 0x04000A9D RID: 2717
		public int Index;

		// Token: 0x04000A9E RID: 2718
		private ConversationSentenceOption _option;

		// Token: 0x04000A9F RID: 2719
		private Action _onReadyToContinue;

		// Token: 0x04000AA0 RID: 2720
		private bool _hasPersuasion;

		// Token: 0x04000AA1 RID: 2721
		private bool _isSpecial;

		// Token: 0x04000AA2 RID: 2722
		private string _itemText;

		// Token: 0x04000AA3 RID: 2723
		private int _iconType;

		// Token: 0x04000AA4 RID: 2724
		private bool _isEnabled;

		// Token: 0x04000AA5 RID: 2725
		private PersuasionOptionVM _persuasionItem;

		// Token: 0x04000AA6 RID: 2726
		private HintViewModel _optionHint;
	}
}
