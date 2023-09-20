using System;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI;
using TaleWorlds.MountAndBlade.View.Tableaus;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.TextureProviders
{
	// Token: 0x0200001A RID: 26
	public class SaveLoadHeroTableauTextureProvider : TextureProvider
	{
		// Token: 0x17000036 RID: 54
		// (set) Token: 0x060000EC RID: 236 RVA: 0x00006105 File Offset: 0x00004305
		public string HeroVisualCode
		{
			set
			{
				this._characterCode = value;
				this.DeserializeCharacterCode(this._characterCode);
			}
		}

		// Token: 0x17000037 RID: 55
		// (set) Token: 0x060000ED RID: 237 RVA: 0x0000611A File Offset: 0x0000431A
		public string BannerCode
		{
			set
			{
				this._tableau.SetBannerCode(value);
			}
		}

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x060000EE RID: 238 RVA: 0x00006128 File Offset: 0x00004328
		public bool IsVersionCompatible
		{
			get
			{
				return this._tableau.IsVersionCompatible;
			}
		}

		// Token: 0x17000039 RID: 57
		// (set) Token: 0x060000EF RID: 239 RVA: 0x00006135 File Offset: 0x00004335
		public bool CurrentlyRotating
		{
			set
			{
				this._tableau.RotateCharacter(value);
			}
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x00006143 File Offset: 0x00004343
		public SaveLoadHeroTableauTextureProvider()
		{
			this._tableau = new BasicCharacterTableau();
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x00006156 File Offset: 0x00004356
		public override void Tick(float dt)
		{
			this.CheckTexture();
			this._tableau.OnTick(dt);
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x0000616A File Offset: 0x0000436A
		public override void SetTargetSize(int width, int height)
		{
			base.SetTargetSize(width, height);
			this._tableau.SetTargetSize(width, height);
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x00006181 File Offset: 0x00004381
		private void DeserializeCharacterCode(string characterCode)
		{
			if (!string.IsNullOrEmpty(characterCode))
			{
				this._tableau.DeserializeCharacterCode(characterCode);
			}
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x00006198 File Offset: 0x00004398
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

		// Token: 0x060000F5 RID: 245 RVA: 0x000061FC File Offset: 0x000043FC
		public override void Clear()
		{
			this._tableau.OnFinalize();
			base.Clear();
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x0000620F File Offset: 0x0000440F
		public override Texture GetTexture(TwoDimensionContext twoDimensionContext, string name)
		{
			this.CheckTexture();
			return this._providedTexture;
		}

		// Token: 0x0400008B RID: 139
		private string _characterCode;

		// Token: 0x0400008C RID: 140
		private BasicCharacterTableau _tableau;

		// Token: 0x0400008D RID: 141
		private Texture _texture;

		// Token: 0x0400008E RID: 142
		private Texture _providedTexture;
	}
}
