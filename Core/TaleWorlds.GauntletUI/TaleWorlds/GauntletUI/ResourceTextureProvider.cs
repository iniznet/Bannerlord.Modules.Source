using System;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI
{
	// Token: 0x02000027 RID: 39
	public class ResourceTextureProvider : TextureProvider
	{
		// Token: 0x060002DE RID: 734 RVA: 0x0000DDA1 File Offset: 0x0000BFA1
		public override Texture GetTexture(TwoDimensionContext twoDimensionContext, string name)
		{
			return twoDimensionContext.LoadTexture(name);
		}
	}
}
