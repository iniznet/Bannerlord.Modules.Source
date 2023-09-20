using System;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Inquiries
{
	public class TextQueryPopUpVM : PopUpBaseVM
	{
		public TextQueryPopUpVM(Action closeQuery)
			: base(closeQuery)
		{
			this.DoneButtonDisabledReasonHint = new HintViewModel();
		}

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

		public override void ExecuteAffirmativeAction()
		{
			Action<string> affirmativeAction = this._data.AffirmativeAction;
			if (affirmativeAction != null)
			{
				affirmativeAction(this.InputText);
			}
			base.CloseQuery();
		}

		public override void ExecuteNegativeAction()
		{
			Action negativeAction = this._data.NegativeAction;
			if (negativeAction != null)
			{
				negativeAction();
			}
			base.CloseQuery();
		}

		public override void OnClearData()
		{
			base.OnClearData();
			this._data = null;
		}

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

		private TextInquiryData _data;

		[DataSourceProperty]
		private string _inputText;

		private bool _isInputObfuscated;

		private HintViewModel _doneButtonDisabledReasonHint;
	}
}
