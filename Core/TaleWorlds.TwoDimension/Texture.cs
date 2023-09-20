using System;

namespace TaleWorlds.TwoDimension
{
	// Token: 0x02000034 RID: 52
	public class Texture
	{
		// Token: 0x170000BF RID: 191
		// (get) Token: 0x06000241 RID: 577 RVA: 0x0000938A File Offset: 0x0000758A
		// (set) Token: 0x06000242 RID: 578 RVA: 0x00009392 File Offset: 0x00007592
		public ITexture PlatformTexture { get; private set; }

		// Token: 0x170000C0 RID: 192
		// (get) Token: 0x06000243 RID: 579 RVA: 0x0000939B File Offset: 0x0000759B
		public int Width
		{
			get
			{
				return this.PlatformTexture.Width;
			}
		}

		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x06000244 RID: 580 RVA: 0x000093A8 File Offset: 0x000075A8
		public int Height
		{
			get
			{
				return this.PlatformTexture.Height;
			}
		}

		// Token: 0x06000245 RID: 581 RVA: 0x000093B5 File Offset: 0x000075B5
		public Texture(ITexture platformTexture)
		{
			this.PlatformTexture = platformTexture;
		}

		// Token: 0x06000246 RID: 582 RVA: 0x000093C4 File Offset: 0x000075C4
		public bool IsLoaded()
		{
			return this.PlatformTexture.IsLoaded();
		}
	}
}
