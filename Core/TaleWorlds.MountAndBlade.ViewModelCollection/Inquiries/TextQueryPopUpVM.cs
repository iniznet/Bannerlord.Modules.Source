using System;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Inquiries
{
	// Token: 0x020000D4 RID: 212
	public class TextQueryPopUpVM : PopUpBaseVM
	{
		// Token: 0x060013C4 RID: 5060 RVA: 0x00041052 File Offset: 0x0003F252
		public TextQueryPopUpVM(Action closeQuery)
			: base(closeQuery)
		{
			this.DoneButtonDisabledReasonHint = new HintViewModel();
		}

		// Token: 0x060013C5 RID: 5061 RVA: 0x00041068 File Offset: 0x0003F268
		public void SetData(TextInquiryData data)
		{
			this._data = data;
			base.TitleText = this._data.TitleText;
			base.PopUpLabel = this._data.Text;
			base.ButtonOkLabel = this._data.AffirmativeText;
			base.ButtonCancelLabel = this._data.NegativeText;
			base.IsButtonOkShown = this._data.IsAffirmativeOptionShown;
			base.IsButtonCancelShown = this._data.IsNegativeOptionShown;
			this.IsInputObfuscated = this._data.IsInputObfuscated;
			this.InputText = this._data.DefaultInputText;
			Func<string, Tuple<bool, string>> textCondition = this._data.TextCondition;
			base.IsButtonOkEnabled = textCondition == null || textCondition(this.InputText).Item1;
		}

		// Token: 0x060013C6 RID: 5062 RVA: 0x0004112C File Offset: 0x0003F32C
		public override void ExecuteAffirmativeAction()
		{
			Action<string> affirmativeAction = this._data.AffirmativeAction;
			if (affirmativeAction != null)
			{
				affirmativeAction(this.InputText);
			}
			base.CloseQuery();
		}

		// Token: 0x060013C7 RID: 5063 RVA: 0x00041150 File Offset: 0x0003F350
		public override void ExecuteNegativeAction()
		{
			Action negativeAction = this._data.NegativeAction;
			if (negativeAction != null)
			{
				negativeAction();
			}
			base.CloseQuery();
		}

		// Token: 0x060013C8 RID: 5064 RVA: 0x0004116E File Offset: 0x0003F36E
		public override void OnClearData()
		{
			base.OnClearData();
			this._data = null;
		}

		// Token: 0x17000685 RID: 1669
		// (get) Token: 0x060013CA RID: 5066 RVA: 0x00041214 File Offset: 0x0003F414
		// (set) Token: 0x060013C9 RID: 5065 RVA: 0x00041180 File Offset: 0x0003F380
		[DataSourceProperty]
		public string InputText
		{
			get
			{
				return this._inputText;
			}
			set
			{
				if (value != this._inputText)
				{
					this._inputText = value;
					base.OnPropertyChangedWithValue<string>(value, "InputText");
					Func<string, Tuple<bool, string>> textCondition = this._data.TextCondition;
					Tuple<bool, string> tuple = ((textCondition != null) ? textCondition(value) : null);
					base.IsButtonOkEnabled = tuple == null || tuple.Item1;
					this.DoneButtonDisabledReasonHint.HintText = (string.IsNullOrEmpty((tuple != null) ? tuple.Item2 : null) ? TextObject.Empty : new TextObject("{=!}" + tuple.Item2, null));
				}
			}
		}

		// Token: 0x17000686 RID: 1670
		// (get) Token: 0x060013CB RID: 5067 RVA: 0x0004121C File Offset: 0x0003F41C
		// (set) Token: 0x060013CC RID: 5068 RVA: 0x00041224 File Offset: 0x0003F424
		public bool IsInputObfuscated
		{
			get
			{
				return this._isInputObfuscated;
			}
			set
			{
				if (value != this._isInputObfuscated)
				{
					this._isInputObfuscated = value;
					base.OnPropertyChangedWithValue(value, "IsInputObfuscated");
				}
			}
		}

		// Token: 0x17000687 RID: 1671
		// (get) Token: 0x060013CD RID: 5069 RVA: 0x00041242 File Offset: 0x0003F442
		// (set) Token: 0x060013CE RID: 5070 RVA: 0x0004124A File Offset: 0x0003F44A
		[DataSourceProperty]
		public HintViewModel DoneButtonDisabledReasonHint
		{
			get
			{
				return this._doneButtonDisabledReasonHint;
			}
			set
			{
				if (value != this._doneButtonDisabledReasonHint)
				{
					this._doneButtonDisabledReasonHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "DoneButtonDisabledReasonHint");
				}
			}
		}

		// Token: 0x0400097F RID: 2431
		private TextInquiryData _data;

		// Token: 0x04000980 RID: 2432
		[DataSourceProperty]
		private string _inputText;

		// Token: 0x04000981 RID: 2433
		private bool _isInputObfuscated;

		// Token: 0x04000982 RID: 2434
		private HintViewModel _doneButtonDisabledReasonHint;
	}
}
