using System;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI;
using TaleWorlds.MountAndBlade.View.Tableaus;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.TextureProviders
{
	public class SaveLoadHeroTableauTextureProvider : TextureProvider
	{
		public string HeroVisualCode
		{
			set
			{
				this._characterCode = value;
				this.DeserializeCharacterCode(this._characterCode);
			}
		}

		public string BannerCode
		{
			set
			{
				this._tableau.SetBannerCode(value);
			}
		}

		public bool IsVersionCompatible
		{
			get
			{
				return this._tableau.IsVersionCompatible;
			}
		}

		public bool CurrentlyRotating
		{
			set
			{
				this._tableau.RotateCharacter(value);
			}
		}

		public SaveLoadHeroTableauTextureProvider()
		{
			this._tableau = new BasicCharacterTableau();
		}

		public override void Tick(float dt)
		{
			this.CheckTexture();
			this._tableau.OnTick(dt);
		}

		public override void SetTargetSize(int width, int height)
		{
			base.SetTargetSize(width, height);
			this._tableau.SetTargetSize(width, height);
		}

		private void DeserializeCharacterCode(string characterCode)
		{
			if (!string.IsNullOrEmpty(characterCode))
			{
				this._tableau.DeserializeCharacterCode(characterCode);
			}
		}

		private void CheckTexture()
		{
			if (this._texture != this._tableau.Texture)
			{
				this._texture = this._tableau.Texture;
				if (this._texture != null)
				{
					EngineTexture engineTexture = new EngineTexture(this._texture);
					this._providedTexture = new Texture(engineTexture);
					return;
				}
				this._providedTexture = null;
			}
		}

		public override void Clear(bool clearNextFrame)
		{
			this._tableau.OnFinalize();
			base.Clear(clearNextFrame);
		}

		public override Texture GetTexture(TwoDimensionContext twoDimensionContext, string name)
		{
			this.CheckTexture();
			return this._providedTexture;
		}

		private string _characterCode;

		private BasicCharacterTableau _tableau;

		private Texture _texture;

		private Texture _providedTexture;
	}
}
