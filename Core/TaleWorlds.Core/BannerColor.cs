using System;

namespace TaleWorlds.Core
{
	public struct BannerColor
	{
		public uint Color { get; private set; }

		public bool PlayerCanChooseForSigil { get; private set; }

		public bool PlayerCanChooseForBackground { get; private set; }

		public BannerColor(uint color, bool playerCanChooseForSigil, bool playerCanChooseForBackground)
		{
			this.Color = color;
			this.PlayerCanChooseForSigil = playerCanChooseForSigil;
			this.PlayerCanChooseForBackground = playerCanChooseForBackground;
		}
	}
}
