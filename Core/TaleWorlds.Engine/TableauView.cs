using System;
using TaleWorlds.DotNet;

namespace TaleWorlds.Engine
{
	// Token: 0x0200008A RID: 138
	[EngineClass("rglTableau_view")]
	public sealed class TableauView : SceneView
	{
		// Token: 0x06000A8A RID: 2698 RVA: 0x0000B8A0 File Offset: 0x00009AA0
		internal TableauView(UIntPtr meshPointer)
			: base(meshPointer)
		{
		}

		// Token: 0x06000A8B RID: 2699 RVA: 0x0000B8A9 File Offset: 0x00009AA9
		public static TableauView CreateTableauView()
		{
			return EngineApplicationInterface.ITableauView.CreateTableauView();
		}

		// Token: 0x06000A8C RID: 2700 RVA: 0x0000B8B5 File Offset: 0x00009AB5
		public void SetSortingEnabled(bool value)
		{
			EngineApplicationInterface.ITableauView.SetSortingEnabled(base.Pointer, value);
		}

		// Token: 0x06000A8D RID: 2701 RVA: 0x0000B8C8 File Offset: 0x00009AC8
		public void SetContinuousRendering(bool value)
		{
			EngineApplicationInterface.ITableauView.SetContinousRendering(base.Pointer, value);
		}

		// Token: 0x06000A8E RID: 2702 RVA: 0x0000B8DB File Offset: 0x00009ADB
		public void SetDoNotRenderThisFrame(bool value)
		{
			EngineApplicationInterface.ITableauView.SetDoNotRenderThisFrame(base.Pointer, value);
		}

		// Token: 0x06000A8F RID: 2703 RVA: 0x0000B8EE File Offset: 0x00009AEE
		public void SetDeleteAfterRendering(bool value)
		{
			EngineApplicationInterface.ITableauView.SetDeleteAfterRendering(base.Pointer, value);
		}

		// Token: 0x06000A90 RID: 2704 RVA: 0x0000B901 File Offset: 0x00009B01
		public static Texture AddTableau(string name, RenderTargetComponent.TextureUpdateEventHandler eventHandler, object objectRef, int tableauSizeX, int tableauSizeY)
		{
			Texture texture = Texture.CreateTableauTexture(name, eventHandler, objectRef, tableauSizeX, tableauSizeY);
			texture.TableauView.SetRenderOnDemand(false);
			return texture;
		}
	}
}
