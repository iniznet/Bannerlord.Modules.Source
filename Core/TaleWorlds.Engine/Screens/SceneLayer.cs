using System;
using System.Numerics;
using TaleWorlds.Library;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.Engine.Screens
{
	public class SceneLayer : ScreenLayer
	{
		public bool ClearSceneOnFinalize { get; private set; }

		public bool AutoToggleSceneView { get; private set; }

		public SceneView SceneView
		{
			get
			{
				return this._sceneView;
			}
		}

		public SceneLayer(string categoryId = "SceneLayer", bool clearSceneOnFinalize = true, bool autoToggleSceneView = true)
			: base(-100, categoryId)
		{
			base.Name = "SceneLayer";
			this.ClearSceneOnFinalize = clearSceneOnFinalize;
			base.InputRestrictions.SetInputRestrictions(false, InputUsageMask.All);
			this._sceneView = SceneView.CreateSceneView();
			this.AutoToggleSceneView = autoToggleSceneView;
			base.IsFocusLayer = true;
		}

		protected override void OnActivate()
		{
			base.OnActivate();
			if (this.AutoToggleSceneView)
			{
				this._sceneView.SetEnable(true);
			}
			ScreenManager.TrySetFocus(this);
		}

		protected override void OnDeactivate()
		{
			base.OnDeactivate();
			if (this.AutoToggleSceneView)
			{
				this._sceneView.SetEnable(false);
			}
		}

		protected override void OnFinalize()
		{
			if (this.ClearSceneOnFinalize)
			{
				this._sceneView.ClearAll(true, true);
			}
			base.OnFinalize();
		}

		public void SetScene(Scene scene)
		{
			this._sceneView.SetScene(scene);
		}

		public void SetRenderWithPostfx(bool value)
		{
			this._sceneView.SetRenderWithPostfx(value);
		}

		public void SetPostfxConfigParams(int value)
		{
			this._sceneView.SetPostfxConfigParams(value);
		}

		public void SetCamera(Camera camera)
		{
			this._sceneView.SetCamera(camera);
		}

		public void SetPostfxFromConfig()
		{
			this._sceneView.SetPostfxFromConfig();
		}

		public Vec2 WorldPointToScreenPoint(Vec3 position)
		{
			return this._sceneView.WorldPointToScreenPoint(position);
		}

		public Vec2 ScreenPointToViewportPoint(Vec2 position)
		{
			return this._sceneView.ScreenPointToViewportPoint(position);
		}

		public bool ProjectedMousePositionOnGround(out Vec3 groundPosition, out Vec3 groundNormal, bool mouseVisible, BodyFlags excludeBodyOwnerFlags, bool checkOccludedSurface)
		{
			return this._sceneView.ProjectedMousePositionOnGround(out groundPosition, out groundNormal, mouseVisible, excludeBodyOwnerFlags, checkOccludedSurface);
		}

		public void TranslateMouse(ref Vec3 worldMouseNear, ref Vec3 worldMouseFar, float maxDistance = -1f)
		{
			this._sceneView.TranslateMouse(ref worldMouseNear, ref worldMouseFar, maxDistance);
		}

		public void SetSceneUsesSkybox(bool value)
		{
			this._sceneView.SetSceneUsesSkybox(value);
		}

		public void SetSceneUsesShadows(bool value)
		{
			this._sceneView.SetSceneUsesShadows(value);
		}

		public void SetSceneUsesContour(bool value)
		{
			this._sceneView.SetSceneUsesContour(value);
		}

		public void SetShadowmapResolutionMultiplier(float value)
		{
			this._sceneView.SetShadowmapResolutionMultiplier(value);
		}

		public void SetFocusedShadowmap(bool enable, ref Vec3 center, float radius)
		{
			this._sceneView.SetFocusedShadowmap(enable, ref center, radius);
		}

		public void DoNotClear(bool value)
		{
			this._sceneView.DoNotClear(value);
		}

		public bool ReadyToRender()
		{
			return this._sceneView.ReadyToRender();
		}

		public void SetCleanScreenUntilLoadingDone(bool value)
		{
			this._sceneView.SetCleanScreenUntilLoadingDone(value);
		}

		public void ClearAll()
		{
			this._sceneView.ClearAll(true, true);
		}

		public void ClearRuntimeGPUMemory(bool remove_terrain)
		{
			this._sceneView.ClearAll(false, remove_terrain);
		}

		protected override void RefreshGlobalOrder(ref int currentOrder)
		{
			this._sceneView.SetRenderOrder(currentOrder);
			currentOrder++;
		}

		public override bool HitTest(Vector2 position)
		{
			return true;
		}

		public override bool HitTest()
		{
			return true;
		}

		public override bool FocusTest()
		{
			return true;
		}

		private SceneView _sceneView;
	}
}
