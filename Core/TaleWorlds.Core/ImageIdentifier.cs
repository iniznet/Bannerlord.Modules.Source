using System;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.Core
{
	// Token: 0x02000086 RID: 134
	public class ImageIdentifier
	{
		// Token: 0x17000291 RID: 657
		// (get) Token: 0x060007B4 RID: 1972 RVA: 0x0001AD10 File Offset: 0x00018F10
		// (set) Token: 0x060007B5 RID: 1973 RVA: 0x0001AD18 File Offset: 0x00018F18
		public ImageIdentifierType ImageTypeCode { get; private set; }

		// Token: 0x17000292 RID: 658
		// (get) Token: 0x060007B6 RID: 1974 RVA: 0x0001AD21 File Offset: 0x00018F21
		// (set) Token: 0x060007B7 RID: 1975 RVA: 0x0001AD29 File Offset: 0x00018F29
		public string AdditionalArgs { get; private set; }

		// Token: 0x060007B8 RID: 1976 RVA: 0x0001AD32 File Offset: 0x00018F32
		public ImageIdentifier(ImageIdentifierType imageType = ImageIdentifierType.Null)
		{
			this.ImageTypeCode = imageType;
			this.Id = "";
			this.AdditionalArgs = "";
		}

		// Token: 0x060007B9 RID: 1977 RVA: 0x0001AD57 File Offset: 0x00018F57
		public ImageIdentifier(ItemObject itemObject, string bannerCode = "")
		{
			this.ImageTypeCode = ImageIdentifierType.Item;
			this.Id = itemObject.StringId;
			this.AdditionalArgs = bannerCode;
		}

		// Token: 0x060007BA RID: 1978 RVA: 0x0001AD79 File Offset: 0x00018F79
		public ImageIdentifier(CharacterCode characterCode)
		{
			this.ImageTypeCode = ImageIdentifierType.Character;
			this.Id = characterCode.Code;
			this.AdditionalArgs = "";
		}

		// Token: 0x060007BB RID: 1979 RVA: 0x0001AD9F File Offset: 0x00018F9F
		public ImageIdentifier(CraftingPiece craftingPiece, string pieceUsageId)
		{
			this.ImageTypeCode = ImageIdentifierType.CraftingPiece;
			this.Id = ((craftingPiece != null) ? (craftingPiece.StringId + "$" + pieceUsageId) : "");
			this.AdditionalArgs = "";
		}

		// Token: 0x060007BC RID: 1980 RVA: 0x0001ADDA File Offset: 0x00018FDA
		public ImageIdentifier(BannerCode bannerCode, bool nineGrid = false)
		{
			this.ImageTypeCode = (nineGrid ? ImageIdentifierType.BannerCodeNineGrid : ImageIdentifierType.BannerCode);
			this.Id = ((bannerCode != null) ? bannerCode.Code : "");
			this.AdditionalArgs = "";
		}

		// Token: 0x060007BD RID: 1981 RVA: 0x0001AE16 File Offset: 0x00019016
		public ImageIdentifier(PlayerId playerId, int forcedAvatarIndex)
		{
			this.ImageTypeCode = ImageIdentifierType.MultiplayerAvatar;
			this.Id = playerId.ToString();
			this.AdditionalArgs = string.Format("{0}", forcedAvatarIndex);
		}

		// Token: 0x060007BE RID: 1982 RVA: 0x0001AE50 File Offset: 0x00019050
		public ImageIdentifier(Banner banner)
		{
			this.ImageTypeCode = ImageIdentifierType.BannerCode;
			this.AdditionalArgs = "";
			if (banner != null)
			{
				BannerCode bannerCode = BannerCode.CreateFrom(banner);
				this.Id = bannerCode.Code;
				return;
			}
			this.Id = "";
		}

		// Token: 0x060007BF RID: 1983 RVA: 0x0001AE97 File Offset: 0x00019097
		public ImageIdentifier(ImageIdentifier code)
		{
			this.ImageTypeCode = code.ImageTypeCode;
			this.Id = code.Id;
			this.AdditionalArgs = code.AdditionalArgs;
		}

		// Token: 0x060007C0 RID: 1984 RVA: 0x0001AEC3 File Offset: 0x000190C3
		public ImageIdentifier(string id, ImageIdentifierType type, string additionalArgs = "")
		{
			this.ImageTypeCode = type;
			this.Id = id;
			this.AdditionalArgs = additionalArgs;
		}

		// Token: 0x060007C1 RID: 1985 RVA: 0x0001AEE0 File Offset: 0x000190E0
		public bool Equals(ImageIdentifier target)
		{
			return target != null && this.ImageTypeCode == target.ImageTypeCode && this.Id.Equals(target.Id) && this.AdditionalArgs.Equals(target.AdditionalArgs);
		}

		// Token: 0x040003E2 RID: 994
		public string Id;
	}
}
