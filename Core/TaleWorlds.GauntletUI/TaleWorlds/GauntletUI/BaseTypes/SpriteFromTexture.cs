using System;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.BaseTypes
{
	// Token: 0x02000060 RID: 96
	internal class SpriteFromTexture : Sprite
	{
		// Token: 0x170001C2 RID: 450
		// (get) Token: 0x0600061D RID: 1565 RVA: 0x0001B242 File Offset: 0x00019442
		public override Texture Texture
		{
			get
			{
				return this._texture;
			}
		}

		// Token: 0x0600061E RID: 1566 RVA: 0x0001B24A File Offset: 0x0001944A
		public SpriteFromTexture(Texture texture, int width, int height)
			: base("Sprite", width, height)
		{
			this._texture = texture;
		}

		// Token: 0x0600061F RID: 1567 RVA: 0x0001B260 File Offset: 0x00019460
		public override float GetScaleToUse(float width, float height, float scale)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000620 RID: 1568 RVA: 0x0001B267 File Offset: 0x00019467
		protected override DrawObject2D GetArrays(SpriteDrawData spriteDrawData)
		{
			throw new NotImplementedException();
		}

		// Token: 0x040002E8 RID: 744
		private Texture _texture;
	}
}
