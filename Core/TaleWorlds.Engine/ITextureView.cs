using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x0200002F RID: 47
	[ApplicationInterfaceBase]
	internal interface ITextureView
	{
		// Token: 0x06000437 RID: 1079
		[EngineMethod("create_texture_view", false)]
		TextureView CreateTextureView();

		// Token: 0x06000438 RID: 1080
		[EngineMethod("set_texture", false)]
		void SetTexture(UIntPtr pointer, UIntPtr texture_ptr);
	}
}
