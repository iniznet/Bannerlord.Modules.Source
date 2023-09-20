using System;

namespace TaleWorlds.TwoDimension
{
	public class Texture
	{
		public ITexture PlatformTexture { get; private set; }

		public int Width
		{
			get
			{
				return this.PlatformTexture.Width;
			}
		}

		public int Height
		{
			get
			{
				return this.PlatformTexture.Height;
			}
		}

		public Texture(ITexture platformTexture)
		{
			this.PlatformTexture = platformTexture;
		}

		public bool IsLoaded()
		{
			return this.PlatformTexture.IsLoaded();
		}
	}
}
