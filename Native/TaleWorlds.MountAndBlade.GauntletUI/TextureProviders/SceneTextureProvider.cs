using System;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI;
using TaleWorlds.MountAndBlade.View.Tableaus;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.TextureProviders
{
	public class SceneTextureProvider : TextureProvider
	{
		public Scene WantedScene { get; private set; }

		public bool? IsReady
		{
			get
			{
				SceneTableau sceneTableau = this._sceneTableau;
				if (sceneTableau == null)
				{
					return null;
				}
				return sceneTableau.IsReady;
			}
		}

		public object Scene
		{
			set
			{
				if (value != null)
				{
					this._sceneTableau = new SceneTableau();
					this._sceneTableau.SetScene(value);
					return;
				}
				this._sceneTableau.OnFinalize();
				this._sceneTableau = null;
			}
		}

		public SceneTextureProvider()
		{
			this._sceneTableau = new SceneTableau();
		}

		private void CheckTexture()
		{
			if (this._sceneTableau != null)
			{
				if (this._texture != this._sceneTableau._texture)
				{
					this._texture = this._sceneTableau._texture;
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
			SceneTableau sceneTableau = this._sceneTableau;
			if (sceneTableau == null)
			{
				return;
			}
			sceneTableau.OnTick(dt);
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

		private SceneTableau _sceneTableau;

		private Texture _texture;

		private Texture _providedTexture;

		private EngineTexture wrappedTexture;
	}
}
