using System;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI;
using TaleWorlds.MountAndBlade.View.Tableaus;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.TextureProviders
{
	// Token: 0x02000015 RID: 21
	public class BrightnessDemoTextureProvider : TextureProvider
	{
		// Token: 0x17000012 RID: 18
		// (set) Token: 0x060000A0 RID: 160 RVA: 0x000054D2 File Offset: 0x000036D2
		public int DemoType
		{
			set
			{
				this._sceneTableau.SetDemoType(value);
			}
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x000054E0 File Offset: 0x000036E0
		public BrightnessDemoTextureProvider()
		{
			this._sceneTableau = new BrightnessDemoTableau();
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x000054F4 File Offset: 0x000036F4
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

		// Token: 0x060000A3 RID: 163 RVA: 0x00005572 File Offset: 0x00003772
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

		// Token: 0x060000A4 RID: 164 RVA: 0x00005592 File Offset: 0x00003792
		public override void Clear()
		{
			BrightnessDemoTableau sceneTableau = this._sceneTableau;
			if (sceneTableau != null)
			{
				sceneTableau.OnFinalize();
			}
			base.Clear();
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x000055AB File Offset: 0x000037AB
		public override void SetTargetSize(int width, int height)
		{
			base.SetTargetSize(width, height);
			this._sceneTableau.SetTargetSize(width, height);
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x000055C2 File Offset: 0x000037C2
		public override Texture GetTexture(TwoDimensionContext twoDimensionContext, string name)
		{
			this.CheckTexture();
			return this._providedTexture;
		}

		// Token: 0x04000067 RID: 103
		private BrightnessDemoTableau _sceneTableau;

		// Token: 0x04000068 RID: 104
		private Texture _texture;

		// Token: 0x04000069 RID: 105
		private Texture _providedTexture;

		// Token: 0x0400006A RID: 106
		private EngineTexture wrappedTexture;
	}
}
