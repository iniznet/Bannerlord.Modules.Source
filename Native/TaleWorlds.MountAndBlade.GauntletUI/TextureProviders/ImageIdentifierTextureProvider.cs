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
	// Token: 0x02000017 RID: 23
	public class ImageIdentifierTextureProvider : TextureProvider
	{
		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060000C0 RID: 192 RVA: 0x000057A4 File Offset: 0x000039A4
		// (set) Token: 0x060000C1 RID: 193 RVA: 0x000057AC File Offset: 0x000039AC
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

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060000C2 RID: 194 RVA: 0x000057C5 File Offset: 0x000039C5
		// (set) Token: 0x060000C3 RID: 195 RVA: 0x000057CD File Offset: 0x000039CD
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

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060000C4 RID: 196 RVA: 0x000057ED File Offset: 0x000039ED
		// (set) Token: 0x060000C5 RID: 197 RVA: 0x000057F5 File Offset: 0x000039F5
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

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060000C6 RID: 198 RVA: 0x00005813 File Offset: 0x00003A13
		// (set) Token: 0x060000C7 RID: 199 RVA: 0x0000581B File Offset: 0x00003A1B
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

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060000C8 RID: 200 RVA: 0x00005839 File Offset: 0x00003A39
		// (set) Token: 0x060000C9 RID: 201 RVA: 0x00005841 File Offset: 0x00003A41
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

		// Token: 0x060000CA RID: 202 RVA: 0x0000585A File Offset: 0x00003A5A
		public ImageIdentifierTextureProvider()
		{
			this._textureRequiresRefreshing = true;
			this._receivedAvatarData = null;
			this._timeSinceAvatarFail = 6f;
		}

		// Token: 0x060000CB RID: 203 RVA: 0x0000587C File Offset: 0x00003A7C
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

		// Token: 0x060000CC RID: 204 RVA: 0x000059C1 File Offset: 0x00003BC1
		public override Texture GetTexture(TwoDimensionContext twoDimensionContext, string name)
		{
			this.CheckTexture();
			return this._providedTexture;
		}

		// Token: 0x060000CD RID: 205 RVA: 0x000059CF File Offset: 0x00003BCF
		public override void Tick(float dt)
		{
			base.Tick(dt);
			this._timeSinceAvatarFail += dt;
			this.CheckTexture();
		}

		// Token: 0x060000CE RID: 206 RVA: 0x000059EC File Offset: 0x00003BEC
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

		// Token: 0x060000CF RID: 207 RVA: 0x00005CB0 File Offset: 0x00003EB0
		private void OnAvatarLoaded(string avatarID, AvatarData avatarData)
		{
			if (avatarData != null)
			{
				this._creatingTexture = true;
				Texture texture = TableauCacheManager.Current.CreateAvatarTexture(avatarID, avatarData.Image, avatarData.Width, avatarData.Height, avatarData.Type);
				this.OnTextureCreated(texture);
			}
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x00005CF2 File Offset: 0x00003EF2
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

		// Token: 0x060000D1 RID: 209 RVA: 0x00005D2C File Offset: 0x00003F2C
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

		// Token: 0x060000D2 RID: 210 RVA: 0x00005E22 File Offset: 0x00004022
		private void OnTextureCreated(Texture texture)
		{
			this._texture = texture;
			this._textureRequiresRefreshing = false;
			this._handleNewlyCreatedTexture = true;
			this._creatingTexture = false;
		}

		// Token: 0x0400006F RID: 111
		private const float AvatarFailWaitTime = 5f;

		// Token: 0x04000070 RID: 112
		private ItemObject _itemObject;

		// Token: 0x04000071 RID: 113
		private CraftingPiece _craftingPiece;

		// Token: 0x04000072 RID: 114
		private BannerCode _bannerCode;

		// Token: 0x04000073 RID: 115
		private CharacterCode _characterCode;

		// Token: 0x04000074 RID: 116
		private Texture _texture;

		// Token: 0x04000075 RID: 117
		private Texture _providedTexture;

		// Token: 0x04000076 RID: 118
		private string _imageId;

		// Token: 0x04000077 RID: 119
		private string _additionalArgs;

		// Token: 0x04000078 RID: 120
		private int _imageTypeCode;

		// Token: 0x04000079 RID: 121
		private bool _isBig;

		// Token: 0x0400007A RID: 122
		private bool _isReleased;

		// Token: 0x0400007B RID: 123
		private AvatarData _receivedAvatarData;

		// Token: 0x0400007C RID: 124
		private float _timeSinceAvatarFail;

		// Token: 0x0400007D RID: 125
		private bool _textureRequiresRefreshing;

		// Token: 0x0400007E RID: 126
		private bool _handleNewlyCreatedTexture;

		// Token: 0x0400007F RID: 127
		private bool _creatingTexture;
	}
}
