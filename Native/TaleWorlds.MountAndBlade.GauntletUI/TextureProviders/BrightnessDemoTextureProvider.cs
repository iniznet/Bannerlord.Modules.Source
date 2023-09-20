using System;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI;
using TaleWorlds.MountAndBlade.View.Tableaus;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.TextureProviders
{
	public class BrightnessDemoTextureProvider : TextureProvider
	{
		public int DemoType
		{
			set
			{
				this._sceneTableau.SetDemoType(value);
			}
		}

		public BrightnessDemoTextureProvider()
		{
			this._sceneTableau = new BrightnessDemoTableau();
		}

		private void CheckTexture()
		{
			if (this._sceneTableau != null)
			{
				if (this._texture != this._sceneTableau.Texture)
				{
					this._texture = this._sceneTableau.Texture;
					if (this._texture != null)
					{
						this.wrappedTexture = new EngineTexture(this._texture);
						this._providedTexture = new Texture(this.wrappedTexture);
						return;
					}
					this._providedTexture = null;
					return;
				}
			}
			else
			{
				this._providedTexture = null;
			}
		}

		public override void Tick(float dt)
		{
			base.Tick(dt);
			this.CheckTexture();
			BrightnessDemoTableau sceneTableau = this._sceneTableau;
			if (sceneTableau == null)
			{
				return;
			}
			sceneTableau.OnTick(dt);
		}

		public override void Clear(bool clearNextFrame)
		{
			BrightnessDemoTableau sceneTableau = this._sceneTableau;
			if (sceneTableau != null)
			{
				sceneTableau.OnFinalize();
			}
			base.Clear(clearNextFrame);
		}

		public override void SetTargetSize(int width, int height)
		{
			base.SetTargetSize(width, height);
			this._sceneTableau.SetTargetSize(width, height);
		}

		public override Texture GetTexture(TwoDimensionContext twoDimensionContext, string name)
		{
			this.CheckTexture();
			return this._providedTexture;
		}

		private BrightnessDemoTableau _sceneTableau;

		private Texture _texture;

		private Texture _providedTexture;

		private EngineTexture wrappedTexture;
	}
}
