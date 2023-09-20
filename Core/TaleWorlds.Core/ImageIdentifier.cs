using System;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.Core
{
	public class ImageIdentifier
	{
		public ImageIdentifierType ImageTypeCode { get; private set; }

		public string AdditionalArgs { get; private set; }

		public ImageIdentifier(ImageIdentifierType imageType = ImageIdentifierType.Null)
		{
			this.ImageTypeCode = imageType;
			this.Id = "";
			this.AdditionalArgs = "";
		}

		public ImageIdentifier(ItemObject itemObject, string bannerCode = "")
		{
			this.ImageTypeCode = ImageIdentifierType.Item;
			this.Id = itemObject.StringId;
			this.AdditionalArgs = bannerCode;
		}

		public ImageIdentifier(CharacterCode characterCode)
		{
			this.ImageTypeCode = ImageIdentifierType.Character;
			this.Id = characterCode.Code;
			this.AdditionalArgs = "";
		}

		public ImageIdentifier(CraftingPiece craftingPiece, string pieceUsageId)
		{
			this.ImageTypeCode = ImageIdentifierType.CraftingPiece;
			this.Id = ((craftingPiece != null) ? (craftingPiece.StringId + "$" + pieceUsageId) : "");
			this.AdditionalArgs = "";
		}

		public ImageIdentifier(BannerCode bannerCode, bool nineGrid = false)
		{
			this.ImageTypeCode = (nineGrid ? ImageIdentifierType.BannerCodeNineGrid : ImageIdentifierType.BannerCode);
			this.Id = ((bannerCode != null) ? bannerCode.Code : "");
			this.AdditionalArgs = "";
		}

		public ImageIdentifier(PlayerId playerId, int forcedAvatarIndex)
		{
			this.ImageTypeCode = ImageIdentifierType.MultiplayerAvatar;
			this.Id = playerId.ToString();
			this.AdditionalArgs = string.Format("{0}", forcedAvatarIndex);
		}

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

		public ImageIdentifier(ImageIdentifier code)
		{
			this.ImageTypeCode = code.ImageTypeCode;
			this.Id = code.Id;
			this.AdditionalArgs = code.AdditionalArgs;
		}

		public ImageIdentifier(string id, ImageIdentifierType type, string additionalArgs = "")
		{
			this.ImageTypeCode = type;
			this.Id = id;
			this.AdditionalArgs = additionalArgs;
		}

		public bool Equals(ImageIdentifier target)
		{
			return target != null && this.ImageTypeCode == target.ImageTypeCode && this.Id.Equals(target.Id) && this.AdditionalArgs.Equals(target.AdditionalArgs);
		}

		public string Id;
	}
}
