using System;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View.Tableaus;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.TextureProviders
{
	public class BannerTableauTextureProvider : TextureProvider
	{
		public string BannerCodeText
		{
			set
			{
				this._bannerTableau.SetBannerCode(value);
			}
		}

		public bool IsNineGrid
		{
			set
			{
				this._bannerTableau.SetIsNineGrid(value);
			}
		}

		public float CustomRenderScale
		{
			set
			{
				this._bannerTableau.SetCustomRenderScale(value);
			}
		}

		public Vec2 UpdatePositionValueManual
		{
			set
			{
				this._bannerTableau.SetUpdatePositionValueManual(value);
			}
		}

		public Vec2 UpdateSizeValueManual
		{
			set
			{
				this._bannerTableau.SetUpdateSizeValueManual(value);
			}
		}

		public ValueTuple<float, bool> UpdateRotationValueManualWithMirror
		{
			set
			{
				this._bannerTableau.SetUpdateRotationValueManual(value);
			}
		}

		public int MeshIndexToUpdate
		{
			set
			{
				this._bannerTableau.SetMeshIndexToUpdate(value);
			}
		}

		public bool IsHidden
		{
			get
			{
				return this._isHidden;
			}
			set
			{
				if (this._isHidden != value)
				{
					this._isHidden = value;
				}
			}
		}

		public BannerTableauTextureProvider()
		{
			this._bannerTableau = new BannerTableau();
		}

		public override void Clear(bool clearNextFrame)
		{
			this._bannerTableau.OnFinalize();
			base.Clear(clearNextFrame);
		}

		private void CheckTexture()
		{
			if (this._texture != this._bannerTableau.Texture)
			{
				this._texture = this._bannerTableau.Texture;
				if (this._texture != null)
				{
					EngineTexture engineTexture = new EngineTexture(this._texture);
					this._providedTexture = new Texture(engineTexture);
					return;
				}
				this._providedTexture = null;
			}
		}

		public override Texture GetTexture(TwoDimensionContext twoDimensionContext, string name)
		{
			this.CheckTexture();
			return this._providedTexture;
		}

		public override void SetTargetSize(int width, int height)
		{
			base.SetTargetSize(width, height);
			this._bannerTableau.SetTargetSize(width, height);
		}

		public override void Tick(float dt)
		{
			base.Tick(dt);
			this.CheckTexture();
			this._bannerTableau.OnTick(dt);
		}

		private BannerTableau _bannerTableau;

		private Texture _texture;

		private Texture _providedTexture;

		private bool _isHidden;
	}
}
