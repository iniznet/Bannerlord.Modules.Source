using System;
using SandBox.View.Map;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI;
using TaleWorlds.TwoDimension;

namespace SandBox.GauntletUI
{
	// Token: 0x0200000F RID: 15
	public class MapConversationTextureProvider : TextureProvider
	{
		// Token: 0x17000007 RID: 7
		// (set) Token: 0x060000A6 RID: 166 RVA: 0x00006D6B File Offset: 0x00004F6B
		public object Data
		{
			set
			{
				this._mapConversationTableau.SetData(value);
			}
		}

		// Token: 0x17000008 RID: 8
		// (set) Token: 0x060000A7 RID: 167 RVA: 0x00006D79 File Offset: 0x00004F79
		public bool IsEnabled
		{
			set
			{
				this._mapConversationTableau.SetEnabled(value);
			}
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x00006D87 File Offset: 0x00004F87
		public MapConversationTextureProvider()
		{
			this._mapConversationTableau = new MapConversationTableau();
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x00006D9A File Offset: 0x00004F9A
		public override void Clear()
		{
			this._mapConversationTableau.OnFinalize();
			base.Clear();
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00006DB0 File Offset: 0x00004FB0
		private void CheckTexture()
		{
			if (this._texture != this._mapConversationTableau.Texture)
			{
				this._texture = this._mapConversationTableau.Texture;
				if (this._texture != null)
				{
					EngineTexture engineTexture = new EngineTexture(this._texture);
					this._providedTexture = new Texture(engineTexture);
					return;
				}
				this._providedTexture = null;
			}
		}

		// Token: 0x060000AB RID: 171 RVA: 0x00006E14 File Offset: 0x00005014
		public override Texture GetTexture(TwoDimensionContext twoDimensionContext, string name)
		{
			this.CheckTexture();
			return this._providedTexture;
		}

		// Token: 0x060000AC RID: 172 RVA: 0x00006E22 File Offset: 0x00005022
		public override void SetTargetSize(int width, int height)
		{
			base.SetTargetSize(width, height);
			this._mapConversationTableau.SetTargetSize(width, height);
		}

		// Token: 0x060000AD RID: 173 RVA: 0x00006E39 File Offset: 0x00005039
		public override void Tick(float dt)
		{
			base.Tick(dt);
			this.CheckTexture();
			this._mapConversationTableau.OnTick(dt);
		}

		// Token: 0x04000053 RID: 83
		private MapConversationTableau _mapConversationTableau;

		// Token: 0x04000054 RID: 84
		private Texture _texture;

		// Token: 0x04000055 RID: 85
		private Texture _providedTexture;
	}
}
