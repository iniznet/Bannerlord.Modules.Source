using System;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.BaseTypes
{
	internal class SpriteFromTexture : Sprite
	{
		public override Texture Texture
		{
			get
			{
				return this._texture;
			}
		}

		public SpriteFromTexture(Texture texture, int width, int height)
			: base("Sprite", width, height)
		{
			this._texture = texture;
		}

		public override float GetScaleToUse(float width, float height, float scale)
		{
			throw new NotImplementedException();
		}

		protected override DrawObject2D GetArrays(SpriteDrawData spriteDrawData)
		{
			throw new NotImplementedException();
		}

		private Texture _texture;
	}
}
