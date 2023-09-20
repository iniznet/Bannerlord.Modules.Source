using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.CharacterCreation
{
	public class CharacterCreationReviewStageItemVM : ViewModel
	{
		public CharacterCreationReviewStageItemVM(ImageIdentifierVM imageIdentifier, string title, string text, string description)
			: this(title, text, description)
		{
			this.HasImage = true;
			this.ImageIdentifier = imageIdentifier;
		}

		public CharacterCreationReviewStageItemVM(string title, string text, string description)
		{
			this.Title = title;
			this.Text = text;
			this.Description = description;
		}

		[DataSourceProperty]
		public bool HasImage
		{
			get
			{
				return this._hasImage;
			}
			set
			{
				if (value != this._hasImage)
				{
					this._hasImage = value;
					base.OnPropertyChangedWithValue(value, "HasImage");
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
				if (value != this._imageIdentifier)
				{
					this._imageIdentifier = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "ImageIdentifier");
				}
			}
		}

		[DataSourceProperty]
		public string Title
		{
			get
			{
				return this._title;
			}
			set
			{
				if (value != this._title)
				{
					this._title = value;
					base.OnPropertyChangedWithValue<string>(value, "Title");
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
				if (value != this._text)
				{
					this._text = value;
					base.OnPropertyChangedWithValue<string>(value, "Text");
				}
			}
		}

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

		private bool _hasImage;

		private ImageIdentifierVM _imageIdentifier;

		private string _title;

		private string _text;

		private string _description;
	}
}
