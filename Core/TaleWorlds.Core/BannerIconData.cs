﻿using System;

namespace TaleWorlds.Core
{
	public struct BannerIconData
	{
		public string MaterialName { get; private set; }

		public int TextureIndex { get; private set; }

		public BannerIconData(string materialName, int textureIndex)
		{
			this.MaterialName = materialName;
			this.TextureIndex = textureIndex;
		}
	}
}
