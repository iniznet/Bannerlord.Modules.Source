using System;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000184 RID: 388
	public class MBAgentRendererSceneController
	{
		// Token: 0x06001441 RID: 5185 RVA: 0x0004E880 File Offset: 0x0004CA80
		internal MBAgentRendererSceneController(UIntPtr pointer)
		{
			this._pointer = pointer;
		}

		// Token: 0x06001442 RID: 5186 RVA: 0x0004E890 File Offset: 0x0004CA90
		~MBAgentRendererSceneController()
		{
		}

		// Token: 0x06001443 RID: 5187 RVA: 0x0004E8B8 File Offset: 0x0004CAB8
		public void SetEnforcedVisibilityForAllAgents(Scene scene)
		{
			MBAPI.IMBAgentVisuals.SetEnforcedVisibilityForAllAgents(scene.Pointer, this._pointer);
		}

		// Token: 0x06001444 RID: 5188 RVA: 0x0004E8D0 File Offset: 0x0004CAD0
		public static MBAgentRendererSceneController CreateNewAgentRendererSceneController(Scene scene, int maxRenderCount)
		{
			return new MBAgentRendererSceneController(MBAPI.IMBAgentVisuals.CreateAgentRendererSceneController(scene.Pointer, maxRenderCount));
		}

		// Token: 0x06001445 RID: 5189 RVA: 0x0004E8E8 File Offset: 0x0004CAE8
		public void SetDoTimerBasedForcedSkeletonUpdates(bool value)
		{
			MBAPI.IMBAgentVisuals.SetDoTimerBasedForcedSkeletonUpdates(this._pointer, value);
		}

		// Token: 0x06001446 RID: 5190 RVA: 0x0004E8FB File Offset: 0x0004CAFB
		public static void DestructAgentRendererSceneController(Scene scene, MBAgentRendererSceneController rendererSceneController, bool deleteThisFrame)
		{
			MBAPI.IMBAgentVisuals.DestructAgentRendererSceneController(scene.Pointer, rendererSceneController._pointer, deleteThisFrame);
			rendererSceneController._pointer = UIntPtr.Zero;
		}

		// Token: 0x06001447 RID: 5191 RVA: 0x0004E91F File Offset: 0x0004CB1F
		public static void ValidateAgentVisualsReseted(Scene scene, MBAgentRendererSceneController rendererSceneController)
		{
			MBAPI.IMBAgentVisuals.ValidateAgentVisualsReseted(scene.Pointer, rendererSceneController._pointer);
		}

		// Token: 0x040006C7 RID: 1735
		private UIntPtr _pointer;
	}
}
