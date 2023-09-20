using System;
using SandBox.View.Map;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI;
using TaleWorlds.TwoDimension;

namespace SandBox.GauntletUI
{
	public class MapConversationTextureProvider : TextureProvider
	{
		public object Data
		{
			set
			{
				this._mapConversationTableau.SetData(value);
			}
		}

		public bool IsEnabled
		{
			set
			{
				this._mapConversationTableau.SetEnabled(value);
			}
		}

		public MapConversationTextureProvider()
		{
			this._mapConversationTableau = new MapConversationTableau();
		}

		public override void Clear()
		{
			this._mapConversationTableau.OnFinalize();
			base.Clear();
		}

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

		public override Texture GetTexture(TwoDimensionContext twoDimensionContext, string name)
		{
			this.CheckTexture();
			return this._providedTexture;
		}

		public override void SetTargetSize(int width, int height)
		{
			base.SetTargetSize(width, height);
			this._mapConversationTableau.SetTargetSize(width, height);
		}

		public override void Tick(float dt)
		{
			base.Tick(dt);
			this.CheckTexture();
			this._mapConversationTableau.OnTick(dt);
		}

		private MapConversationTableau _mapConversationTableau;

		private Texture _texture;

		private Texture _providedTexture;
	}
}
