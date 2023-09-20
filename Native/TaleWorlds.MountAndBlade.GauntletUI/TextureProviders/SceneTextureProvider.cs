using System;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI;
using TaleWorlds.MountAndBlade.View.Tableaus;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.TextureProviders
{
	// Token: 0x0200001B RID: 27
	public class SceneTextureProvider : TextureProvider
	{
		// Token: 0x1700003A RID: 58
		// (get) Token: 0x060000F7 RID: 247 RVA: 0x0000621D File Offset: 0x0000441D
		// (set) Token: 0x060000F8 RID: 248 RVA: 0x00006225 File Offset: 0x00004425
		public Scene WantedScene { get; private set; }

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x060000F9 RID: 249 RVA: 0x00006230 File Offset: 0x00004430
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

		// Token: 0x1700003C RID: 60
		// (set) Token: 0x060000FA RID: 250 RVA: 0x00006256 File Offset: 0x00004456
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

		// Token: 0x060000FB RID: 251 RVA: 0x00006285 File Offset: 0x00004485
		public SceneTextureProvider()
		{
			this._sceneTableau = new SceneTableau();
		}

		// Token: 0x060000FC RID: 252 RVA: 0x00006298 File Offset: 0x00004498
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

		// Token: 0x060000FD RID: 253 RVA: 0x00006316 File Offset: 0x00004516
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

		// Token: 0x060000FE RID: 254 RVA: 0x00006336 File Offset: 0x00004536
		public override void SetTargetSize(int width, int height)
		{
			base.SetTargetSize(width, height);
			this._sceneTableau.SetTargetSize(width, height);
		}

		// Token: 0x060000FF RID: 255 RVA: 0x0000634D File Offset: 0x0000454D
		public override Texture GetTexture(TwoDimensionContext twoDimensionContext, string name)
		{
			this.CheckTexture();
			return this._providedTexture;
		}

		// Token: 0x0400008F RID: 143
		private SceneTableau _sceneTableau;

		// Token: 0x04000090 RID: 144
		private Texture _texture;

		// Token: 0x04000091 RID: 145
		private Texture _providedTexture;

		// Token: 0x04000092 RID: 146
		private EngineTexture wrappedTexture;
	}
}
