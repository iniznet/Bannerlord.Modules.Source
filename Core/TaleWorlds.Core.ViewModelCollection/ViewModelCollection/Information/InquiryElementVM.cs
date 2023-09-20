using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.Core.ViewModelCollection.Information
{
	public class InquiryElementVM : ViewModel
	{
		public InquiryElementVM(InquiryElement elementData, TextObject hint)
		{
			this.Text = elementData.Title;
			this.ImageIdentifier = ((elementData.ImageIdentifier != null) ? new ImageIdentifierVM(elementData.ImageIdentifier) : new ImageIdentifierVM(ImageIdentifierType.Null));
			this.InquiryElement = elementData;
			this.IsEnabled = elementData.IsEnabled;
			this.HasVisuals = elementData.ImageIdentifier != null;
			this.Hint = new HintViewModel(hint, null);
		}

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

		public readonly InquiryElement InquiryElement;

		private bool _isSelected;

		private bool _isEnabled;

		private string _text;

		private bool _hasVisuals;

		private ImageIdentifierVM _imageIdentifier;

		private HintViewModel _hint;
	}
}
