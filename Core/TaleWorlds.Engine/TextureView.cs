using System;
using TaleWorlds.DotNet;

namespace TaleWorlds.Engine
{
	// Token: 0x0200008C RID: 140
	[EngineClass("rglTexture_view")]
	public sealed class TextureView : View
	{
		// Token: 0x06000AAF RID: 2735 RVA: 0x0000BB96 File Offset: 0x00009D96
		internal TextureView(UIntPtr meshPointer)
			: base(meshPointer)
		{
		}

		// Token: 0x06000AB0 RID: 2736 RVA: 0x0000BB9F File Offset: 0x00009D9F
		public static TextureView CreateTextureView()
		{
			return EngineApplicationInterface.ITextureView.CreateTextureView();
		}

		// Token: 0x06000AB1 RID: 2737 RVA: 0x0000BBAB File Offset: 0x00009DAB
		public void SetTexture(Texture texture)
		{
			EngineApplicationInterface.ITextureView.SetTexture(base.Pointer, texture.Pointer);
		}
	}
}
