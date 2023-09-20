using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Inquiries
{
	// Token: 0x020000D2 RID: 210
	public class MultiSelectionQueryPopUpVM : PopUpBaseVM
	{
		// Token: 0x060013AE RID: 5038 RVA: 0x00040AB7 File Offset: 0x0003ECB7
		public MultiSelectionQueryPopUpVM(Action closeQuery)
			: base(closeQuery)
		{
			this.InquiryElements = new MBBindingList<InquiryElementVM>();
			this.MaxSelectableOptionCount = -1;
		}

		// Token: 0x060013AF RID: 5039 RVA: 0x00040AD4 File Offset: 0x0003ECD4
		public void SetData(MultiSelectionInquiryData data)
		{
			this._data = data;
			this.InquiryElements.Clear();
			foreach (InquiryElement inquiryElement in this._data.InquiryElements)
			{
				TextObject textObject = (string.IsNullOrEmpty(inquiryElement.Hint) ? TextObject.Empty : new TextObject("{=!}" + inquiryElement.Hint, null));
				this.InquiryElements.Add(new InquiryElementVM(inquiryElement, textObject));
			}
			base.TitleText = this._data.TitleText;
			base.PopUpLabel = this._data.DescriptionText;
			this.MaxSelectableOptionCount = this._data.MaxSelectableOptionCount;
			base.ButtonOkLabel = this._data.AffirmativeText;
			base.ButtonCancelLabel = this._data.NegativeText;
			base.IsButtonOkShown = true;
			base.IsButtonCancelShown = this._data.IsExitShown;
		}

		// Token: 0x060013B0 RID: 5040 RVA: 0x00040BE4 File Offset: 0x0003EDE4
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

		// Token: 0x060013B1 RID: 5041 RVA: 0x00040C68 File Offset: 0x0003EE68
		public override void ExecuteNegativeAction()
		{
			Action<List<InquiryElement>> negativeAction = this._data.NegativeAction;
			if (negativeAction != null)
			{
				negativeAction(new List<InquiryElement>());
			}
			base.CloseQuery();
		}

		// Token: 0x060013B2 RID: 5042 RVA: 0x00040C8B File Offset: 0x0003EE8B
		public override void OnClearData()
		{
			base.OnClearData();
			this._data = null;
			this.MaxSelectableOptionCount = -1;
		}

		// Token: 0x17000680 RID: 1664
		// (get) Token: 0x060013B4 RID: 5044 RVA: 0x00040CBF File Offset: 0x0003EEBF
		// (set) Token: 0x060013B3 RID: 5043 RVA: 0x00040CA1 File Offset: 0x0003EEA1
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

		// Token: 0x17000681 RID: 1665
		// (get) Token: 0x060013B5 RID: 5045 RVA: 0x00040CC7 File Offset: 0x0003EEC7
		// (set) Token: 0x060013B6 RID: 5046 RVA: 0x00040CCF File Offset: 0x0003EECF
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

		// Token: 0x04000975 RID: 2421
		private MultiSelectionInquiryData _data;

		// Token: 0x04000976 RID: 2422
		private MBBindingList<InquiryElementVM> _inquiryElements;

		// Token: 0x04000977 RID: 2423
		private int _maxSelectableOptionCount;
	}
}
