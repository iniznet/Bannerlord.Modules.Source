using System;
using System.Linq;
using TaleWorlds.Avatar.PlayerServices;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View.Tableaus;
using TaleWorlds.ObjectSystem;
using TaleWorlds.PlayerServices;
using TaleWorlds.PlayerServices.Avatar;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.TextureProviders
{
	public class ImageIdentifierTextureProvider : TextureProvider
	{
		public bool IsBig
		{
			get
			{
				return this._isBig;
			}
			set
			{
				if (this._isBig != value)
				{
					this._isBig = value;
					this._textureRequiresRefreshing = true;
				}
			}
		}

		public bool IsReleased
		{
			get
			{
				return this._isReleased;
			}
			set
			{
				if (this._isReleased != value)
				{
					this._isReleased = value;
					if (this._isReleased)
					{
						this.ReleaseCache();
					}
				}
			}
		}

		public string ImageId
		{
			get
			{
				return this._imageId;
			}
			set
			{
				if (this._imageId != value)
				{
					this._imageId = value;
					this._textureRequiresRefreshing = true;
				}
			}
		}

		public string AdditionalArgs
		{
			get
			{
				return this._additionalArgs;
			}
			set
			{
				if (this._additionalArgs != value)
				{
					this._additionalArgs = value;
					this._textureRequiresRefreshing = true;
				}
			}
		}

		public int ImageTypeCode
		{
			get
			{
				return this._imageTypeCode;
			}
			set
			{
				if (this._imageTypeCode != value)
				{
					this._imageTypeCode = value;
					this._textureRequiresRefreshing = true;
				}
			}
		}

		public ImageIdentifierTextureProvider()
		{
			this._textureRequiresRefreshing = true;
			this._receivedAvatarData = null;
			this._timeSinceAvatarFail = 6f;
		}

		private void CheckTexture()
		{
			if (this._textureRequiresRefreshing && (!this._creatingTexture || this.ImageTypeCode == 4))
			{
				if (this._receivedAvatarData != null)
				{
					if (this._receivedAvatarData.Status == 1)
					{
						AvatarData receivedAvatarData = this._receivedAvatarData;
						this._receivedAvatarData = null;
						this.OnAvatarLoaded(this.ImageId + "." + this.AdditionalArgs, receivedAvatarData);
					}
					else if (this._receivedAvatarData.Status == 2)
					{
						this._receivedAvatarData = null;
						this.OnTextureCreated(null);
						this._textureRequiresRefreshing = true;
						this._timeSinceAvatarFail = 0f;
					}
				}
				else if (this.ImageTypeCode != 0)
				{
					if (this._timeSinceAvatarFail > 5f)
					{
						this.CreateImageWithId(this.ImageId, this.ImageTypeCode, this.AdditionalArgs);
					}
				}
				else
				{
					this.OnTextureCreated(null);
				}
			}
			if (this._handleNewlyCreatedTexture)
			{
				Texture texture = null;
				Texture providedTexture = this._providedTexture;
				EngineTexture engineTexture;
				if ((engineTexture = ((providedTexture != null) ? providedTexture.PlatformTexture : null) as EngineTexture) != null)
				{
					texture = engineTexture.Texture;
				}
				if (this._texture != texture)
				{
					if (this._texture != null)
					{
						EngineTexture engineTexture2 = new EngineTexture(this._texture);
						this._providedTexture = new Texture(engineTexture2);
					}
					else
					{
						this._providedTexture = null;
					}
				}
				this._handleNewlyCreatedTexture = false;
			}
		}

		public override Texture GetTexture(TwoDimensionContext twoDimensionContext, string name)
		{
			this.CheckTexture();
			return this._providedTexture;
		}

		public override void Tick(float dt)
		{
			base.Tick(dt);
			this._timeSinceAvatarFail += dt;
			this.CheckTexture();
		}

		public void CreateImageWithId(string id, int typeAsInt, string additionalArgs)
		{
			if (string.IsNullOrEmpty(id))
			{
				if (typeAsInt == 5)
				{
					CharacterCode characterCode = this._characterCode;
					if (characterCode == null || characterCode.IsEmpty)
					{
						this.OnTextureCreated(TableauCacheManager.Current.GetCachedHeroSilhouetteTexture());
						return;
					}
				}
				this.OnTextureCreated(null);
				return;
			}
			switch (typeAsInt)
			{
			case 0:
				this.OnTextureCreated(null);
				return;
			case 1:
				this._itemObject = MBObjectManager.Instance.GetObject<ItemObject>(id);
				Debug.Print("Render Requested: " + id, 0, 12, 17592186044416UL);
				if (this._itemObject == null)
				{
					Debug.FailedAssert("WRONG Item IMAGE IDENTIFIER ID", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.GauntletUI\\TextureProviders\\ImageIdentifierTextureProvider.cs", "CreateImageWithId", 214);
					this.OnTextureCreated(null);
					return;
				}
				this._creatingTexture = true;
				TableauCacheManager.Current.BeginCreateItemTexture(this._itemObject, additionalArgs, new Action<Texture>(this.OnTextureCreated));
				return;
			case 2:
				this._craftingPiece = MBObjectManager.Instance.GetObject<CraftingPiece>(id.Split(new char[] { '$' })[0]);
				if (this._craftingPiece == null)
				{
					Debug.FailedAssert("WRONG CraftingPiece IMAGE IDENTIFIER ID", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.GauntletUI\\TextureProviders\\ImageIdentifierTextureProvider.cs", "CreateImageWithId", 225);
					this.OnTextureCreated(null);
					return;
				}
				this._creatingTexture = true;
				TableauCacheManager.Current.BeginCreateCraftingPieceTexture(this._craftingPiece, id.Split(new char[] { '$' })[1], new Action<Texture>(this.OnTextureCreated));
				return;
			case 3:
				this._bannerCode = BannerCode.CreateFrom(id);
				this._creatingTexture = true;
				TableauCacheManager.Current.BeginCreateBannerTexture(this._bannerCode, new Action<Texture>(this.OnTextureCreated), false, false);
				return;
			case 4:
			{
				this._creatingTexture = true;
				PlayerId playerId = PlayerId.FromString(id);
				int num;
				if (!Extensions.IsEmpty<char>(additionalArgs))
				{
					num = int.Parse(additionalArgs);
				}
				else
				{
					NetworkCommunicator networkCommunicator = GameNetwork.NetworkPeers.FirstOrDefault((NetworkCommunicator np) => np.VirtualPlayer.Id == playerId);
					num = ((networkCommunicator != null) ? networkCommunicator.ForcedAvatarIndex : (-1));
				}
				AvatarData playerAvatar = AvatarServices.GetPlayerAvatar(playerId, num);
				if (playerAvatar != null)
				{
					this._receivedAvatarData = playerAvatar;
					return;
				}
				this._timeSinceAvatarFail = 0f;
				return;
			}
			case 5:
				this._characterCode = CharacterCode.CreateFrom(id);
				if (FaceGen.GetMaturityTypeWithAge(this._characterCode.BodyProperties.Age) <= 1)
				{
					this.OnTextureCreated(null);
					return;
				}
				this._creatingTexture = true;
				TableauCacheManager.Current.BeginCreateCharacterTexture(this._characterCode, new Action<Texture>(this.OnTextureCreated), this.IsBig);
				return;
			case 6:
				this._bannerCode = BannerCode.CreateFrom(id);
				this._creatingTexture = true;
				TableauCacheManager.Current.BeginCreateBannerTexture(this._bannerCode, new Action<Texture>(this.OnTextureCreated), true, this.IsBig);
				return;
			default:
				Debug.FailedAssert("WRONG IMAGE IDENTIFIER ID", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.GauntletUI\\TextureProviders\\ImageIdentifierTextureProvider.cs", "CreateImageWithId", 284);
				return;
			}
		}

		private void OnAvatarLoaded(string avatarID, AvatarData avatarData)
		{
			if (avatarData != null)
			{
				this._creatingTexture = true;
				Texture texture = TableauCacheManager.Current.CreateAvatarTexture(avatarID, avatarData.Image, avatarData.Width, avatarData.Height, avatarData.Type);
				this.OnTextureCreated(texture);
			}
		}

		public override void Clear()
		{
			base.Clear();
			this._providedTexture = null;
			this._textureRequiresRefreshing = true;
			this._itemObject = null;
			this._craftingPiece = null;
			this._bannerCode = null;
			this._characterCode = null;
			this._creatingTexture = false;
		}

		public void ReleaseCache()
		{
			switch (this.ImageTypeCode)
			{
			case 0:
			case 4:
				break;
			case 1:
				if (this._itemObject != null)
				{
					TableauCacheManager.Current.ReleaseTextureWithId(this._itemObject);
					return;
				}
				break;
			case 2:
				if (this._craftingPiece != null)
				{
					TableauCacheManager.Current.ReleaseTextureWithId(this._craftingPiece, this.ImageId.Split(new char[] { '$' })[1]);
					return;
				}
				break;
			case 3:
				if (this._bannerCode != null)
				{
					TableauCacheManager.Current.ReleaseTextureWithId(this._bannerCode, false, false);
					return;
				}
				break;
			case 5:
				if (this._characterCode != null && FaceGen.GetMaturityTypeWithAge(this._characterCode.BodyProperties.Age) > 1)
				{
					TableauCacheManager.Current.ReleaseTextureWithId(this._characterCode, this.IsBig);
					return;
				}
				break;
			case 6:
				TableauCacheManager.Current.ReleaseTextureWithId(this._bannerCode, true, this.IsBig);
				break;
			default:
				return;
			}
		}

		private void OnTextureCreated(Texture texture)
		{
			this._texture = texture;
			this._textureRequiresRefreshing = false;
			this._handleNewlyCreatedTexture = true;
			this._creatingTexture = false;
		}

		private const float AvatarFailWaitTime = 5f;

		private ItemObject _itemObject;

		private CraftingPiece _craftingPiece;

		private BannerCode _bannerCode;

		private CharacterCode _characterCode;

		private Texture _texture;

		private Texture _providedTexture;

		private string _imageId;

		private string _additionalArgs;

		private int _imageTypeCode;

		private bool _isBig;

		private bool _isReleased;

		private AvatarData _receivedAvatarData;

		private float _timeSinceAvatarFail;

		private bool _textureRequiresRefreshing;

		private bool _handleNewlyCreatedTexture;

		private bool _creatingTexture;
	}
}
