using System;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.Engine.GauntletUI
{
	public class EngineTexture : ITexture
	{
		public Texture Texture { get; private set; }

		int ITexture.Width
		{
			get
			{
				return this.Texture.Width;
			}
		}

		int ITexture.Height
		{
			get
			{
				return this.Texture.Height;
			}
		}

		public EngineTexture(Texture engineTexture)
		{
			this.Texture = engineTexture;
		}

		bool ITexture.IsLoaded()
		{
			return this.Texture.IsLoaded();
		}

		void ITexture.Release()
		{
			this.Texture.Release();
		}
	}
}
