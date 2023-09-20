﻿using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade.View
{
	public static class BannerVisualExtensions
	{
		public static Texture GetTableauTextureSmall(this Banner banner, Action<Texture> setAction)
		{
			return ((BannerVisual)banner.BannerVisual).GetTableauTextureSmall(setAction, true);
		}

		public static Texture GetTableauTextureLarge(this Banner banner, Action<Texture> setAction)
		{
			return ((BannerVisual)banner.BannerVisual).GetTableauTextureLarge(setAction, true);
		}

		public static MetaMesh ConvertToMultiMesh(this Banner banner)
		{
			return ((BannerVisual)banner.BannerVisual).ConvertToMultiMesh();
		}
	}
}
