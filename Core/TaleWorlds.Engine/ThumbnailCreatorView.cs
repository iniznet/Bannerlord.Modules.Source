using System;
using TaleWorlds.DotNet;

namespace TaleWorlds.Engine
{
	// Token: 0x0200008D RID: 141
	[EngineClass("rglThumbnail_creator_view")]
	public sealed class ThumbnailCreatorView : View
	{
		// Token: 0x06000AB2 RID: 2738 RVA: 0x0000BBC3 File Offset: 0x00009DC3
		internal ThumbnailCreatorView(UIntPtr pointer)
			: base(pointer)
		{
		}

		// Token: 0x06000AB3 RID: 2739 RVA: 0x0000BBCC File Offset: 0x00009DCC
		[EngineCallback]
		internal static void OnThumbnailRenderComplete(string renderId, Texture renderTarget)
		{
			ThumbnailCreatorView.renderCallback(renderId, renderTarget);
		}

		// Token: 0x06000AB4 RID: 2740 RVA: 0x0000BBDA File Offset: 0x00009DDA
		public static ThumbnailCreatorView CreateThumbnailCreatorView()
		{
			return EngineApplicationInterface.IThumbnailCreatorView.CreateThumbnailCreatorView();
		}

		// Token: 0x06000AB5 RID: 2741 RVA: 0x0000BBE6 File Offset: 0x00009DE6
		public void RegisterScene(Scene scene, bool usePostFx = true)
		{
			EngineApplicationInterface.IThumbnailCreatorView.RegisterScene(base.Pointer, scene.Pointer, usePostFx);
		}

		// Token: 0x06000AB6 RID: 2742 RVA: 0x0000BBFF File Offset: 0x00009DFF
		public void RegisterEntity(Scene scene, Camera cam, Texture texture, GameEntity itemEntity, int allocationGroupIndex, string renderId = "")
		{
			EngineApplicationInterface.IThumbnailCreatorView.RegisterEntity(base.Pointer, scene.Pointer, cam.Pointer, texture.Pointer, itemEntity.Pointer, renderId, allocationGroupIndex);
		}

		// Token: 0x06000AB7 RID: 2743 RVA: 0x0000BC2E File Offset: 0x00009E2E
		public void ClearRequests()
		{
			EngineApplicationInterface.IThumbnailCreatorView.ClearRequests(base.Pointer);
		}

		// Token: 0x06000AB8 RID: 2744 RVA: 0x0000BC40 File Offset: 0x00009E40
		public void CancelRequest(string renderID)
		{
			EngineApplicationInterface.IThumbnailCreatorView.CancelRequest(base.Pointer, renderID);
		}

		// Token: 0x06000AB9 RID: 2745 RVA: 0x0000BC54 File Offset: 0x00009E54
		public void RegisterEntityWithoutTexture(Scene scene, Camera camera, GameEntity entity, int width, int height, int allocationGroupIndex, string renderId = "", string debugName = "")
		{
			EngineApplicationInterface.IThumbnailCreatorView.RegisterEntityWithoutTexture(base.Pointer, scene.Pointer, camera.Pointer, entity.Pointer, width, height, renderId, debugName, allocationGroupIndex);
		}

		// Token: 0x06000ABA RID: 2746 RVA: 0x0000BC8D File Offset: 0x00009E8D
		public int GetNumberOfPendingRequests()
		{
			return EngineApplicationInterface.IThumbnailCreatorView.GetNumberOfPendingRequests(base.Pointer);
		}

		// Token: 0x06000ABB RID: 2747 RVA: 0x0000BC9F File Offset: 0x00009E9F
		public bool IsMemoryCleared()
		{
			return EngineApplicationInterface.IThumbnailCreatorView.IsMemoryCleared(base.Pointer);
		}

		// Token: 0x040001AA RID: 426
		public static ThumbnailCreatorView.OnThumbnailRenderCompleteDelegate renderCallback;

		// Token: 0x020000C5 RID: 197
		// (Invoke) Token: 0x06000C71 RID: 3185
		public delegate void OnThumbnailRenderCompleteDelegate(string renderId, Texture renderTarget);
	}
}
