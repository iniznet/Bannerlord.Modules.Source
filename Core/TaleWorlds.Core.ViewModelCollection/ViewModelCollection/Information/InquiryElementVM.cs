using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.Core.ViewModelCollection.Information
{
	// Token: 0x02000019 RID: 25
	public class InquiryElementVM : ViewModel
	{
		// Token: 0x06000111 RID: 273 RVA: 0x000040B0 File Offset: 0x000022B0
		public InquiryElementVM(InquiryElement elementData, TextObject hint)
		{
			this.Text = elementData.Title;
			this.ImageIdentifier = ((elementData.ImageIdentifier != null) ? new ImageIdentifierVM(elementData.ImageIdentifier) : new ImageIdentifierVM(ImageIdentifierType.Null));
			this.InquiryElement = elementData;
			this.IsEnabled = elementData.IsEnabled;
			this.HasVisuals = elementData.ImageIdentifier != null;
			this.Hint = new HintViewModel(hint, null);
		}

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x06000112 RID: 274 RVA: 0x0000411F File Offset: 0x0000231F
		// (set) Token: 0x06000113 RID: 275 RVA: 0x00004127 File Offset: 0x00002327
		[DataSourceProperty]
		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				if (this._isSelected != value)
				{
					this._isSelected = value;
					base.OnPropertyChangedWithValue(value, "IsSelected");
				}
			}
		}

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x06000114 RID: 276 RVA: 0x00004145 File Offset: 0x00002345
		// (set) Token: 0x06000115 RID: 277 RVA: 0x0000414D File Offset: 0x0000234D
		[DataSourceProperty]
		public bool HasVisuals
		{
			get
			{
				return this._hasVisuals;
			}
			set
			{
				if (this._hasVisuals != value)
				{
					this._hasVisuals = value;
					base.OnPropertyChangedWithValue(value, "HasVisuals");
				}
			}
		}

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x06000116 RID: 278 RVA: 0x0000416B File Offset: 0x0000236B
		// (set) Token: 0x06000117 RID: 279 RVA: 0x00004173 File Offset: 0x00002373
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

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x06000118 RID: 280 RVA: 0x00004191 File Offset: 0x00002391
		// (set) Token: 0x06000119 RID: 281 RVA: 0x00004199 File Offset: 0x00002399
		[DataSourceProperty]
		public string Text
		{
			get
			{
				return this._text;
			}
			set
			{
				if (this._text != value)
				{
					this._text = value;
					base.OnPropertyChangedWithValue<string>(value, "Text");
				}
			}
		}

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x0600011A RID: 282 RVA: 0x000041BC File Offset: 0x000023BC
		// (set) Token: 0x0600011B RID: 283 RVA: 0x000041C4 File Offset: 0x000023C4
		[DataSourceProperty]
		public ImageIdentifierVM ImageIdentifier
		{
			get
			{
				return this._imageIdentifier;
			}
			set
			{
				if (this._imageIdentifier != value)
				{
					this._imageIdentifier = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "ImageIdentifier");
				}
			}
		}

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x0600011C RID: 284 RVA: 0x000041E2 File Offset: 0x000023E2
		// (set) Token: 0x0600011D RID: 285 RVA: 0x000041EA File Offset: 0x000023EA
		[DataSourceProperty]
		public HintViewModel Hint
		{
			get
			{
				return this._hint;
			}
			set
			{
				if (this._hint != value)
				{
					this._hint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "Hint");
				}
			}
		}

		// Token: 0x0400006E RID: 110
		public readonly InquiryElement InquiryElement;

		// Token: 0x0400006F RID: 111
		private bool _isSelected;

		// Token: 0x04000070 RID: 112
		private bool _isEnabled;

		// Token: 0x04000071 RID: 113
		private string _text;

		// Token: 0x04000072 RID: 114
		private bool _hasVisuals;

		// Token: 0x04000073 RID: 115
		private ImageIdentifierVM _imageIdentifier;

		// Token: 0x04000074 RID: 116
		private HintViewModel _hint;
	}
}
