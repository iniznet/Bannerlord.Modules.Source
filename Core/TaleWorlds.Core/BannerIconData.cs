using System;

namespace TaleWorlds.Core
{
	// Token: 0x02000014 RID: 20
	public struct BannerIconData
	{
		// Token: 0x1700003E RID: 62
		// (get) Token: 0x060000E1 RID: 225 RVA: 0x00004868 File Offset: 0x00002A68
		// (set) Token: 0x060000E2 RID: 226 RVA: 0x00004870 File Offset: 0x00002A70
		public string MaterialName { get; private set; }

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x060000E3 RID: 227 RVA: 0x00004879 File Offset: 0x00002A79
		// (set) Token: 0x060000E4 RID: 228 RVA: 0x00004881 File Offset: 0x00002A81
		public int TextureIndex { get; private set; }

		// Token: 0x060000E5 RID: 229 RVA: 0x0000488A File Offset: 0x00002A8A
		public BannerIconData(string materialName, int textureIndex)
		{
			this.MaterialName = materialName;
			this.TextureIndex = textureIndex;
		}
	}
}
