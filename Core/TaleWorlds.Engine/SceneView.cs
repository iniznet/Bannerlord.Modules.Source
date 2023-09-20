using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x0200007E RID: 126
	[EngineClass("rglScene_view")]
	public class SceneView : View
	{
		// Token: 0x060009AC RID: 2476 RVA: 0x0000A5E1 File Offset: 0x000087E1
		internal SceneView(UIntPtr meshPointer)
			: base(meshPointer)
		{
		}

		// Token: 0x060009AD RID: 2477 RVA: 0x0000A5EA File Offset: 0x000087EA
		public static SceneView CreateSceneView()
		{
			return EngineApplicationInterface.ISceneView.CreateSceneView();
		}

		// Token: 0x060009AE RID: 2478 RVA: 0x0000A5F6 File Offset: 0x000087F6
		public void SetScene(Scene scene)
		{
			EngineApplicationInterface.ISceneView.SetScene(base.Pointer, scene.Pointer);
		}

		// Token: 0x060009AF RID: 2479 RVA: 0x0000A60E File Offset: 0x0000880E
		public void SetAcceptGlobalDebugRenderObjects(bool value)
		{
			EngineApplicationInterface.ISceneView.SetAcceptGlobalDebugRenderObjects(base.Pointer, value);
		}

		// Token: 0x060009B0 RID: 2480 RVA: 0x0000A621 File Offset: 0x00008821
		public void SetRenderWithPostfx(bool value)
		{
			EngineApplicationInterface.ISceneView.SetRenderWithPostfx(base.Pointer, value);
		}

		// Token: 0x060009B1 RID: 2481 RVA: 0x0000A634 File Offset: 0x00008834
		public void SetPostfxConfigParams(int value)
		{
			EngineApplicationInterface.ISceneView.SetPostfxConfigParams(base.Pointer, value);
		}

		// Token: 0x060009B2 RID: 2482 RVA: 0x0000A647 File Offset: 0x00008847
		public void SetForceShaderCompilation(bool value)
		{
			EngineApplicationInterface.ISceneView.SetForceShaderCompilation(base.Pointer, value);
		}

		// Token: 0x060009B3 RID: 2483 RVA: 0x0000A65A File Offset: 0x0000885A
		public bool CheckSceneReadyToRender()
		{
			return EngineApplicationInterface.ISceneView.CheckSceneReadyToRender(base.Pointer);
		}

		// Token: 0x060009B4 RID: 2484 RVA: 0x0000A66C File Offset: 0x0000886C
		public void SetCamera(Camera camera)
		{
			EngineApplicationInterface.ISceneView.SetCamera(base.Pointer, camera.Pointer);
		}

		// Token: 0x060009B5 RID: 2485 RVA: 0x0000A684 File Offset: 0x00008884
		public void SetResolutionScaling(bool value)
		{
			EngineApplicationInterface.ISceneView.SetResolutionScaling(base.Pointer, value);
		}

		// Token: 0x060009B6 RID: 2486 RVA: 0x0000A697 File Offset: 0x00008897
		public void SetPostfxFromConfig()
		{
			EngineApplicationInterface.ISceneView.SetPostfxFromConfig(base.Pointer);
		}

		// Token: 0x060009B7 RID: 2487 RVA: 0x0000A6A9 File Offset: 0x000088A9
		public Vec2 WorldPointToScreenPoint(Vec3 position)
		{
			return EngineApplicationInterface.ISceneView.WorldPointToScreenPoint(base.Pointer, position);
		}

		// Token: 0x060009B8 RID: 2488 RVA: 0x0000A6BC File Offset: 0x000088BC
		public Vec2 ScreenPointToViewportPoint(Vec2 position)
		{
			return EngineApplicationInterface.ISceneView.ScreenPointToViewportPoint(base.Pointer, position.x, position.y);
		}

		// Token: 0x060009B9 RID: 2489 RVA: 0x0000A6DA File Offset: 0x000088DA
		public bool ProjectedMousePositionOnGround(out Vec3 groundPosition, out Vec3 groundNormal, bool mouseVisible, BodyFlags excludeBodyOwnerFlags, bool checkOccludedSurface)
		{
			return EngineApplicationInterface.ISceneView.ProjectedMousePositionOnGround(base.Pointer, out groundPosition, out groundNormal, mouseVisible, excludeBodyOwnerFlags, checkOccludedSurface);
		}

		// Token: 0x060009BA RID: 2490 RVA: 0x0000A6F3 File Offset: 0x000088F3
		public void TranslateMouse(ref Vec3 worldMouseNear, ref Vec3 worldMouseFar, float maxDistance = -1f)
		{
			EngineApplicationInterface.ISceneView.TranslateMouse(base.Pointer, ref worldMouseNear, ref worldMouseFar, maxDistance);
		}

		// Token: 0x060009BB RID: 2491 RVA: 0x0000A708 File Offset: 0x00008908
		public void SetSceneUsesSkybox(bool value)
		{
			EngineApplicationInterface.ISceneView.SetSceneUsesSkybox(base.Pointer, value);
		}

		// Token: 0x060009BC RID: 2492 RVA: 0x0000A71B File Offset: 0x0000891B
		public void SetSceneUsesShadows(bool value)
		{
			EngineApplicationInterface.ISceneView.SetSceneUsesShadows(base.Pointer, value);
		}

		// Token: 0x060009BD RID: 2493 RVA: 0x0000A72E File Offset: 0x0000892E
		public void SetSceneUsesContour(bool value)
		{
			EngineApplicationInterface.ISceneView.SetSceneUsesContour(base.Pointer, value);
		}

		// Token: 0x060009BE RID: 2494 RVA: 0x0000A741 File Offset: 0x00008941
		public void DoNotClear(bool value)
		{
			EngineApplicationInterface.ISceneView.DoNotClear(base.Pointer, value);
		}

		// Token: 0x060009BF RID: 2495 RVA: 0x0000A754 File Offset: 0x00008954
		public void AddClearTask(bool clearOnlySceneview = false)
		{
			EngineApplicationInterface.ISceneView.AddClearTask(base.Pointer, clearOnlySceneview);
		}

		// Token: 0x060009C0 RID: 2496 RVA: 0x0000A767 File Offset: 0x00008967
		public bool ReadyToRender()
		{
			return EngineApplicationInterface.ISceneView.ReadyToRender(base.Pointer);
		}

		// Token: 0x060009C1 RID: 2497 RVA: 0x0000A779 File Offset: 0x00008979
		public void SetClearAndDisableAfterSucessfullRender(bool value)
		{
			EngineApplicationInterface.ISceneView.SetClearAndDisableAfterSucessfullRender(base.Pointer, value);
		}

		// Token: 0x060009C2 RID: 2498 RVA: 0x0000A78C File Offset: 0x0000898C
		public void SetClearGbuffer(bool value)
		{
			EngineApplicationInterface.ISceneView.SetClearGbuffer(base.Pointer, value);
		}

		// Token: 0x060009C3 RID: 2499 RVA: 0x0000A79F File Offset: 0x0000899F
		public void SetShadowmapResolutionMultiplier(float value)
		{
			EngineApplicationInterface.ISceneView.SetShadowmapResolutionMultiplier(base.Pointer, value);
		}

		// Token: 0x060009C4 RID: 2500 RVA: 0x0000A7B2 File Offset: 0x000089B2
		public void SetPointlightResolutionMultiplier(float value)
		{
			EngineApplicationInterface.ISceneView.SetPointlightResolutionMultiplier(base.Pointer, value);
		}

		// Token: 0x060009C5 RID: 2501 RVA: 0x0000A7C5 File Offset: 0x000089C5
		public void SetCleanScreenUntilLoadingDone(bool value)
		{
			EngineApplicationInterface.ISceneView.SetCleanScreenUntilLoadingDone(base.Pointer, value);
		}

		// Token: 0x060009C6 RID: 2502 RVA: 0x0000A7D8 File Offset: 0x000089D8
		public void ClearAll(bool clearScene, bool removeTerrain)
		{
			EngineApplicationInterface.ISceneView.ClearAll(base.Pointer, clearScene, removeTerrain);
		}

		// Token: 0x060009C7 RID: 2503 RVA: 0x0000A7EC File Offset: 0x000089EC
		public void SetFocusedShadowmap(bool enable, ref Vec3 center, float radius)
		{
			EngineApplicationInterface.ISceneView.SetFocusedShadowmap(base.Pointer, enable, ref center, radius);
		}

		// Token: 0x060009C8 RID: 2504 RVA: 0x0000A801 File Offset: 0x00008A01
		public Scene GetScene()
		{
			return EngineApplicationInterface.ISceneView.GetScene(base.Pointer);
		}

		// Token: 0x060009C9 RID: 2505 RVA: 0x0000A814 File Offset: 0x00008A14
		public bool RayCastForClosestEntityOrTerrain(Vec3 sourcePoint, Vec3 targetPoint, out float collisionDistance, out Vec3 closestPoint, float rayThickness = 0.01f, BodyFlags excludeBodyFlags = BodyFlags.CommonFocusRayCastExcludeFlags)
		{
			collisionDistance = float.NaN;
			closestPoint = Vec3.Invalid;
			UIntPtr zero = UIntPtr.Zero;
			return EngineApplicationInterface.ISceneView.RayCastForClosestEntityOrTerrain(base.Pointer, ref sourcePoint, ref targetPoint, rayThickness, ref collisionDistance, ref closestPoint, ref zero, excludeBodyFlags);
		}
	}
}
