using System;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.Engine.GauntletUI
{
	// Token: 0x02000003 RID: 3
	public class EngineTexture : ITexture
	{
		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000006 RID: 6 RVA: 0x00002086 File Offset: 0x00000286
		// (set) Token: 0x06000007 RID: 7 RVA: 0x0000208E File Offset: 0x0000028E
		public Texture Texture { get; private set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000008 RID: 8 RVA: 0x00002097 File Offset: 0x00000297
		int ITexture.Width
		{
			get
			{
				return this.Texture.Width;
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000009 RID: 9 RVA: 0x000020A4 File Offset: 0x000002A4
		int ITexture.Height
		{
			get
			{
				return this.Texture.Height;
			}
		}

		// Token: 0x0600000A RID: 10 RVA: 0x000020B1 File Offset: 0x000002B1
		public EngineTexture(Texture engineTexture)
		{
			this.Texture = engineTexture;
		}

		// Token: 0x0600000B RID: 11 RVA: 0x000020C0 File Offset: 0x000002C0
		bool ITexture.IsLoaded()
		{
			return this.Texture.IsLoaded();
		}

		// Token: 0x0600000C RID: 12 RVA: 0x000020CD File Offset: 0x000002CD
		void ITexture.Release()
		{
			this.Texture.Release();
		}
	}
}
