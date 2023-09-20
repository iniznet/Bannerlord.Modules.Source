using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Inquiries
{
	public class MultiSelectionQueryPopUpVM : PopUpBaseVM
	{
		public MultiSelectionQueryPopUpVM(Action closeQuery)
			: base(closeQuery)
		{
			this.InquiryElements = new MBBindingList<InquiryElementVM>();
			this.MaxSelectableOptionCount = 0;
			this.MinSelectableOptionCount = 0;
			this._selectedOptionCount = 0;
		}

		public void SetData(MultiSelectionInquiryData data)
		{
			this._data = data;
			this.InquiryElements.Clear();
			foreach (InquiryElement inquiryElement in this._data.InquiryElements)
			{
				TextObject textObject = (string.IsNullOrEmpty(inquiryElement.Hint) ? TextObject.Empty : new TextObject("{=!}" + inquiryElement.Hint, null));
				this.InquiryElements.Add(new InquiryElementVM(inquiryElement, textObject, new Action<InquiryElementVM, bool>(this.OnInquiryElementSelected)));
			}
			base.TitleText = this._data.TitleText;
			base.PopUpLabel = this._data.DescriptionText;
			this.MaxSelectableOptionCount = this._data.MaxSelectableOptionCount;
			this.MinSelectableOptionCount = this._data.MinSelectableOptionCount;
			base.ButtonOkLabel = this._data.AffirmativeText;
			base.ButtonCancelLabel = this._data.NegativeText;
			base.IsButtonOkShown = true;
			base.IsButtonCancelShown = this._data.IsExitShown;
			this.RefreshIsButtonOkEnabled();
		}

		private void OnInquiryElementSelected(InquiryElementVM elementVM, bool isSelected)
		{
			if (isSelected)
			{
				this._selectedOptionCount++;
				if (this.MaxSelectableOptionCount != 1)
				{
					goto IL_5C;
				}
				using (IEnumerator<InquiryElementVM> enumerator = this.InquiryElements.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						InquiryElementVM inquiryElementVM = enumerator.Current;
						if (inquiryElementVM != elementVM)
						{
							inquiryElementVM.IsSelected = false;
						}
					}
					goto IL_5C;
				}
			}
			this._selectedOptionCount--;
			IL_5C:
			this.RefreshIsButtonOkEnabled();
		}

		public override void ExecuteAffirmativeAction()
		{
			if (this._data.AffirmativeAction != null)
			{
				List<InquiryElement> list = new List<InquiryElement>();
				foreach (InquiryElementVM inquiryElementVM in this.InquiryElements)
				{
					if (inquiryElementVM.IsSelected)
					{
						list.Add(inquiryElementVM.InquiryElement);
					}
				}
				this._data.AffirmativeAction(list);
			}
			base.CloseQuery();
		}

		public override void ExecuteNegativeAction()
		{
			Action<List<InquiryElement>> negativeAction = this._data.NegativeAction;
			if (negativeAction != null)
			{
				negativeAction(new List<InquiryElement>());
			}
			base.CloseQuery();
		}

		public override void OnClearData()
		{
			base.OnClearData();
			this._data = null;
			this.MaxSelectableOptionCount = 0;
			this.MinSelectableOptionCount = 0;
			this._selectedOptionCount = 0;
		}

		private void RefreshIsButtonOkEnabled()
		{
			base.IsButtonOkEnabled = (this.MaxSelectableOptionCount <= 0 || this._selectedOptionCount <= this.MaxSelectableOptionCount) && this._selectedOptionCount >= this.MinSelectableOptionCount;
		}

		[DataSourceProperty]
		public MBBindingList<InquiryElementVM> InquiryElements
		{
			get
			{
				return this._inquiryElements;
			}
			set
			{
				if (value != this._inquiryElements)
				{
					this._inquiryElements = value;
					base.OnPropertyChangedWithValue<MBBindingList<InquiryElementVM>>(value, "InquiryElements");
				}
			}
		}

		[DataSourceProperty]
		public int MaxSelectableOptionCount
		{
			get
			{
				return this._maxSelectableOptionCount;
			}
			set
			{
				if (value != this._maxSelectableOptionCount)
				{
					this._maxSelectableOptionCount = value;
					base.OnPropertyChangedWithValue(value, "MaxSelectableOptionCount");
				}
			}
		}

		[DataSourceProperty]
		public int MinSelectableOptionCount
		{
			get
			{
				return this._minSelectableOptionCount;
			}
			set
			{
				if (value != this._minSelectableOptionCount)
				{
					this._minSelectableOptionCount = value;
					base.OnPropertyChangedWithValue(value, "MinSelectableOptionCount");
				}
			}
		}

		private MultiSelectionInquiryData _data;

		private int _selectedOptionCount;

		private MBBindingList<InquiryElementVM> _inquiryElements;

		private int _maxSelectableOptionCount;

		private int _minSelectableOptionCount;
	}
}
