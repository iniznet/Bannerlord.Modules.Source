using System;
using TaleWorlds.Library;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.Core
{
	public class ImageIdentifierVM : ViewModel
	{
		[DataSourceProperty]
		public string Id
		{
			get
			{
				return this._imageIdentifierCode.Id;
			}
			private set
			{
				if (this._imageIdentifierCode.Id != value)
				{
					this._imageIdentifierCode.Id = value;
					base.OnPropertyChangedWithValue<string>(value, "Id");
				}
			}
		}

		[DataSourceProperty]
		public bool IsEmpty
		{
			get
			{
				return this._imageIdentifierCode.ImageTypeCode != ImageIdentifierType.Null && string.IsNullOrEmpty(this._imageIdentifierCode.Id);
			}
		}

		[DataSourceProperty]
		public bool IsValid
		{
			get
			{
				return !this.IsEmpty;
			}
		}

		[DataSourceProperty]
		public string AdditionalArgs
		{
			get
			{
				return this._imageIdentifierCode.AdditionalArgs;
			}
		}

		[DataSourceProperty]
		public int ImageTypeCode
		{
			get
			{
				return (int)this._imageIdentifierCode.ImageTypeCode;
			}
		}

		public ImageIdentifierVM(ImageIdentifierType imageType = ImageIdentifierType.Null)
		{
			this._imageIdentifierCode = new ImageIdentifier(imageType);
		}

		public ImageIdentifierVM(ItemObject itemObject, string bannerCode = "")
		{
			this._imageIdentifierCode = new ImageIdentifier(itemObject, bannerCode);
		}

		public ImageIdentifierVM(CharacterCode characterCode)
		{
			this._imageIdentifierCode = new ImageIdentifier(characterCode);
		}

		public ImageIdentifierVM(CraftingPiece craftingPiece, string pieceUsageID)
		{
			this._imageIdentifierCode = new ImageIdentifier(craftingPiece, pieceUsageID);
		}

		public ImageIdentifierVM(BannerCode bannerCode, bool nineGrid = false)
		{
			this._imageIdentifierCode = new ImageIdentifier(bannerCode, nineGrid);
		}

		public ImageIdentifierVM(Banner banner)
		{
			this._imageIdentifierCode = new ImageIdentifier(banner);
		}

		public ImageIdentifierVM(ImageIdentifier code)
		{
			this._imageIdentifierCode = new ImageIdentifier(code);
		}

		public ImageIdentifierVM(PlayerId id, int forcedAvatarIndex = -1)
		{
			this._imageIdentifierCode = new ImageIdentifier(id, forcedAvatarIndex);
		}

		public ImageIdentifierVM(string id, ImageIdentifierType type)
		{
			this._imageIdentifierCode = new ImageIdentifier(id, type, "");
		}

		public ImageIdentifierVM Clone()
		{
			return new ImageIdentifierVM(this.Id, (ImageIdentifierType)this.ImageTypeCode);
		}

		public bool Equals(ImageIdentifierVM target)
		{
			if (this._imageIdentifierCode != null || target._imageIdentifierCode != null)
			{
				ImageIdentifier imageIdentifierCode = this._imageIdentifierCode;
				return imageIdentifierCode != null && imageIdentifierCode.Equals(target._imageIdentifierCode);
			}
			return true;
		}

		private ImageIdentifier _imageIdentifierCode;
	}
}
