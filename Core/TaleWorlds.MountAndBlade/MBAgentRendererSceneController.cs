using System;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	public class MBAgentRendererSceneController
	{
		internal MBAgentRendererSceneController(UIntPtr pointer)
		{
			this._pointer = pointer;
		}

		~MBAgentRendererSceneController()
		{
		}

		public void SetEnforcedVisibilityForAllAgents(Scene scene)
		{
			MBAPI.IMBAgentVisuals.SetEnforcedVisibilityForAllAgents(scene.Pointer, this._pointer);
		}

		public static MBAgentRendererSceneController CreateNewAgentRendererSceneController(Scene scene, int maxRenderCount)
		{
			return new MBAgentRendererSceneController(MBAPI.IMBAgentVisuals.CreateAgentRendererSceneController(scene.Pointer, maxRenderCount));
		}

		public void SetDoTimerBasedForcedSkeletonUpdates(bool value)
		{
			MBAPI.IMBAgentVisuals.SetDoTimerBasedForcedSkeletonUpdates(this._pointer, value);
		}

		public static void DestructAgentRendererSceneController(Scene scene, MBAgentRendererSceneController rendererSceneController, bool deleteThisFrame)
		{
			MBAPI.IMBAgentVisuals.DestructAgentRendererSceneController(scene.Pointer, rendererSceneController._pointer, deleteThisFrame);
			rendererSceneController._pointer = UIntPtr.Zero;
		}

		public static void ValidateAgentVisualsReseted(Scene scene, MBAgentRendererSceneController rendererSceneController)
		{
			MBAPI.IMBAgentVisuals.ValidateAgentVisualsReseted(scene.Pointer, rendererSceneController._pointer);
		}

		private UIntPtr _pointer;
	}
}
