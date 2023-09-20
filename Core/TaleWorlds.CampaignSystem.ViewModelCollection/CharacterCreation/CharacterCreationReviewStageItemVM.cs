using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.CharacterCreation
{
	// Token: 0x0200012B RID: 299
	public class CharacterCreationReviewStageItemVM : ViewModel
	{
		// Token: 0x06001C80 RID: 7296 RVA: 0x000665E0 File Offset: 0x000647E0
		public CharacterCreationReviewStageItemVM(ImageIdentifierVM imageIdentifier, string title, string text, string description)
			: this(title, text, description)
		{
			this.HasImage = true;
			this.ImageIdentifier = imageIdentifier;
		}

		// Token: 0x06001C81 RID: 7297 RVA: 0x000665FA File Offset: 0x000647FA
		public CharacterCreationReviewStageItemVM(string title, string text, string description)
		{
			this.Title = title;
			this.Text = text;
			this.Description = description;
		}

		// Token: 0x170009C2 RID: 2498
		// (get) Token: 0x06001C82 RID: 7298 RVA: 0x00066617 File Offset: 0x00064817
		// (set) Token: 0x06001C83 RID: 7299 RVA: 0x0006661F File Offset: 0x0006481F
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

		// Token: 0x170009C3 RID: 2499
		// (get) Token: 0x06001C84 RID: 7300 RVA: 0x0006663D File Offset: 0x0006483D
		// (set) Token: 0x06001C85 RID: 7301 RVA: 0x00066645 File Offset: 0x00064845
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

		// Token: 0x170009C4 RID: 2500
		// (get) Token: 0x06001C86 RID: 7302 RVA: 0x00066663 File Offset: 0x00064863
		// (set) Token: 0x06001C87 RID: 7303 RVA: 0x0006666B File Offset: 0x0006486B
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

		// Token: 0x170009C5 RID: 2501
		// (get) Token: 0x06001C88 RID: 7304 RVA: 0x0006668E File Offset: 0x0006488E
		// (set) Token: 0x06001C89 RID: 7305 RVA: 0x00066696 File Offset: 0x00064896
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

		// Token: 0x170009C6 RID: 2502
		// (get) Token: 0x06001C8A RID: 7306 RVA: 0x000666B9 File Offset: 0x000648B9
		// (set) Token: 0x06001C8B RID: 7307 RVA: 0x000666C1 File Offset: 0x000648C1
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

		// Token: 0x04000D70 RID: 3440
		private bool _hasImage;

		// Token: 0x04000D71 RID: 3441
		private ImageIdentifierVM _imageIdentifier;

		// Token: 0x04000D72 RID: 3442
		private string _title;

		// Token: 0x04000D73 RID: 3443
		private string _text;

		// Token: 0x04000D74 RID: 3444
		private string _description;
	}
}
