using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade.View
{
	// Token: 0x02000007 RID: 7
	public static class BannerVisualExtensions
	{
		// Token: 0x0600003D RID: 61 RVA: 0x000038AC File Offset: 0x00001AAC
		public static Texture GetTableauTextureSmall(this Banner banner, Action<Texture> setAction)
		{
			return ((BannerVisual)banner.BannerVisual).GetTableauTextureSmall(setAction, true);
		}

		// Token: 0x0600003E RID: 62 RVA: 0x000038C0 File Offset: 0x00001AC0
		public static Texture GetTableauTextureLarge(this Banner banner, Action<Texture> setAction)
		{
			return ((BannerVisual)banner.BannerVisual).GetTableauTextureLarge(setAction, true);
		}

		// Token: 0x0600003F RID: 63 RVA: 0x000038D4 File Offset: 0x00001AD4
		public static MetaMesh ConvertToMultiMesh(this Banner banner)
		{
			return ((BannerVisual)banner.BannerVisual).ConvertToMultiMesh();
		}
	}
}
