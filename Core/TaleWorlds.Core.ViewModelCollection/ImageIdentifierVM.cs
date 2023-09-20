using System;
using TaleWorlds.Library;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.Core
{
	// Token: 0x02000002 RID: 2
	public class ImageIdentifierVM : ViewModel
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000001 RID: 1 RVA: 0x00002048 File Offset: 0x00000248
		// (set) Token: 0x06000002 RID: 2 RVA: 0x00002055 File Offset: 0x00000255
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

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000003 RID: 3 RVA: 0x00002082 File Offset: 0x00000282
		[DataSourceProperty]
		public bool IsEmpty
		{
			get
			{
				return this._imageIdentifierCode.ImageTypeCode != ImageIdentifierType.Null && string.IsNullOrEmpty(this._imageIdentifierCode.Id);
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000004 RID: 4 RVA: 0x000020A3 File Offset: 0x000002A3
		[DataSourceProperty]
		public bool IsValid
		{
			get
			{
				return !this.IsEmpty;
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000005 RID: 5 RVA: 0x000020AE File Offset: 0x000002AE
		[DataSourceProperty]
		public string AdditionalArgs
		{
			get
			{
				return this._imageIdentifierCode.AdditionalArgs;
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000006 RID: 6 RVA: 0x000020BB File Offset: 0x000002BB
		[DataSourceProperty]
		public int ImageTypeCode
		{
			get
			{
				return (int)this._imageIdentifierCode.ImageTypeCode;
			}
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000020C8 File Offset: 0x000002C8
		public ImageIdentifierVM(ImageIdentifierType imageType = ImageIdentifierType.Null)
		{
			this._imageIdentifierCode = new ImageIdentifier(imageType);
		}

		// Token: 0x06000008 RID: 8 RVA: 0x000020DC File Offset: 0x000002DC
		public ImageIdentifierVM(ItemObject itemObject, string bannerCode = "")
		{
			this._imageIdentifierCode = new ImageIdentifier(itemObject, bannerCode);
		}

		// Token: 0x06000009 RID: 9 RVA: 0x000020F1 File Offset: 0x000002F1
		public ImageIdentifierVM(CharacterCode characterCode)
		{
			this._imageIdentifierCode = new ImageIdentifier(characterCode);
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00002105 File Offset: 0x00000305
		public ImageIdentifierVM(CraftingPiece craftingPiece, string pieceUsageID)
		{
			this._imageIdentifierCode = new ImageIdentifier(craftingPiece, pieceUsageID);
		}

		// Token: 0x0600000B RID: 11 RVA: 0x0000211A File Offset: 0x0000031A
		public ImageIdentifierVM(BannerCode bannerCode, bool nineGrid = false)
		{
			this._imageIdentifierCode = new ImageIdentifier(bannerCode, nineGrid);
		}

		// Token: 0x0600000C RID: 12 RVA: 0x0000212F File Offset: 0x0000032F
		public ImageIdentifierVM(Banner banner)
		{
			this._imageIdentifierCode = new ImageIdentifier(banner);
		}

		// Token: 0x0600000D RID: 13 RVA: 0x00002143 File Offset: 0x00000343
		public ImageIdentifierVM(ImageIdentifier code)
		{
			this._imageIdentifierCode = new ImageIdentifier(code);
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00002157 File Offset: 0x00000357
		public ImageIdentifierVM(PlayerId id, int forcedAvatarIndex = -1)
		{
			this._imageIdentifierCode = new ImageIdentifier(id, forcedAvatarIndex);
		}

		// Token: 0x0600000F RID: 15 RVA: 0x0000216C File Offset: 0x0000036C
		public ImageIdentifierVM(string id, ImageIdentifierType type)
		{
			this._imageIdentifierCode = new ImageIdentifier(id, type, "");
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002186 File Offset: 0x00000386
		public ImageIdentifierVM Clone()
		{
			return new ImageIdentifierVM(this.Id, (ImageIdentifierType)this.ImageTypeCode);
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002199 File Offset: 0x00000399
		public bool Equals(ImageIdentifierVM target)
		{
			if (this._imageIdentifierCode != null || target._imageIdentifierCode != null)
			{
				ImageIdentifier imageIdentifierCode = this._imageIdentifierCode;
				return imageIdentifierCode != null && imageIdentifierCode.Equals(target._imageIdentifierCode);
			}
			return true;
		}

		// Token: 0x04000001 RID: 1
		private ImageIdentifier _imageIdentifierCode;
	}
}
