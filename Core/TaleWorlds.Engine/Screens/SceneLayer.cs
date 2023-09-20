using System;
using System.Numerics;
using TaleWorlds.Library;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.Engine.Screens
{
	// Token: 0x02000099 RID: 153
	public class SceneLayer : ScreenLayer
	{
		// Token: 0x17000092 RID: 146
		// (get) Token: 0x06000B89 RID: 2953 RVA: 0x0000CDDC File Offset: 0x0000AFDC
		// (set) Token: 0x06000B8A RID: 2954 RVA: 0x0000CDE4 File Offset: 0x0000AFE4
		public bool ClearSceneOnFinalize { get; private set; }

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x06000B8B RID: 2955 RVA: 0x0000CDED File Offset: 0x0000AFED
		// (set) Token: 0x06000B8C RID: 2956 RVA: 0x0000CDF5 File Offset: 0x0000AFF5
		public bool AutoToggleSceneView { get; private set; }

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x06000B8D RID: 2957 RVA: 0x0000CDFE File Offset: 0x0000AFFE
		public SceneView SceneView
		{
			get
			{
				return this._sceneView;
			}
		}

		// Token: 0x06000B8E RID: 2958 RVA: 0x0000CE08 File Offset: 0x0000B008
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

		// Token: 0x06000B8F RID: 2959 RVA: 0x0000CE56 File Offset: 0x0000B056
		protected override void OnActivate()
		{
			base.OnActivate();
			if (this.AutoToggleSceneView)
			{
				this._sceneView.SetEnable(true);
			}
			ScreenManager.TrySetFocus(this);
		}

		// Token: 0x06000B90 RID: 2960 RVA: 0x0000CE78 File Offset: 0x0000B078
		protected override void OnDeactivate()
		{
			base.OnDeactivate();
			if (this.AutoToggleSceneView)
			{
				this._sceneView.SetEnable(false);
			}
		}

		// Token: 0x06000B91 RID: 2961 RVA: 0x0000CE94 File Offset: 0x0000B094
		protected override void OnFinalize()
		{
			if (this.ClearSceneOnFinalize)
			{
				this._sceneView.ClearAll(true, true);
			}
			base.OnFinalize();
		}

		// Token: 0x06000B92 RID: 2962 RVA: 0x0000CEB1 File Offset: 0x0000B0B1
		public void SetScene(Scene scene)
		{
			this._sceneView.SetScene(scene);
		}

		// Token: 0x06000B93 RID: 2963 RVA: 0x0000CEBF File Offset: 0x0000B0BF
		public void SetRenderWithPostfx(bool value)
		{
			this._sceneView.SetRenderWithPostfx(value);
		}

		// Token: 0x06000B94 RID: 2964 RVA: 0x0000CECD File Offset: 0x0000B0CD
		public void SetPostfxConfigParams(int value)
		{
			this._sceneView.SetPostfxConfigParams(value);
		}

		// Token: 0x06000B95 RID: 2965 RVA: 0x0000CEDB File Offset: 0x0000B0DB
		public void SetCamera(Camera camera)
		{
			this._sceneView.SetCamera(camera);
		}

		// Token: 0x06000B96 RID: 2966 RVA: 0x0000CEE9 File Offset: 0x0000B0E9
		public void SetPostfxFromConfig()
		{
			this._sceneView.SetPostfxFromConfig();
		}

		// Token: 0x06000B97 RID: 2967 RVA: 0x0000CEF6 File Offset: 0x0000B0F6
		public Vec2 WorldPointToScreenPoint(Vec3 position)
		{
			return this._sceneView.WorldPointToScreenPoint(position);
		}

		// Token: 0x06000B98 RID: 2968 RVA: 0x0000CF04 File Offset: 0x0000B104
		public Vec2 ScreenPointToViewportPoint(Vec2 position)
		{
			return this._sceneView.ScreenPointToViewportPoint(position);
		}

		// Token: 0x06000B99 RID: 2969 RVA: 0x0000CF12 File Offset: 0x0000B112
		public bool ProjectedMousePositionOnGround(out Vec3 groundPosition, out Vec3 groundNormal, bool mouseVisible, BodyFlags excludeBodyOwnerFlags, bool checkOccludedSurface)
		{
			return this._sceneView.ProjectedMousePositionOnGround(out groundPosition, out groundNormal, mouseVisible, excludeBodyOwnerFlags, checkOccludedSurface);
		}

		// Token: 0x06000B9A RID: 2970 RVA: 0x0000CF26 File Offset: 0x0000B126
		public void TranslateMouse(ref Vec3 worldMouseNear, ref Vec3 worldMouseFar, float maxDistance = -1f)
		{
			this._sceneView.TranslateMouse(ref worldMouseNear, ref worldMouseFar, maxDistance);
		}

		// Token: 0x06000B9B RID: 2971 RVA: 0x0000CF36 File Offset: 0x0000B136
		public void SetSceneUsesSkybox(bool value)
		{
			this._sceneView.SetSceneUsesSkybox(value);
		}

		// Token: 0x06000B9C RID: 2972 RVA: 0x0000CF44 File Offset: 0x0000B144
		public void SetSceneUsesShadows(bool value)
		{
			this._sceneView.SetSceneUsesShadows(value);
		}

		// Token: 0x06000B9D RID: 2973 RVA: 0x0000CF52 File Offset: 0x0000B152
		public void SetSceneUsesContour(bool value)
		{
			this._sceneView.SetSceneUsesContour(value);
		}

		// Token: 0x06000B9E RID: 2974 RVA: 0x0000CF60 File Offset: 0x0000B160
		public void SetShadowmapResolutionMultiplier(float value)
		{
			this._sceneView.SetShadowmapResolutionMultiplier(value);
		}

		// Token: 0x06000B9F RID: 2975 RVA: 0x0000CF6E File Offset: 0x0000B16E
		public void SetFocusedShadowmap(bool enable, ref Vec3 center, float radius)
		{
			this._sceneView.SetFocusedShadowmap(enable, ref center, radius);
		}

		// Token: 0x06000BA0 RID: 2976 RVA: 0x0000CF7E File Offset: 0x0000B17E
		public void DoNotClear(bool value)
		{
			this._sceneView.DoNotClear(value);
		}

		// Token: 0x06000BA1 RID: 2977 RVA: 0x0000CF8C File Offset: 0x0000B18C
		public bool ReadyToRender()
		{
			return this._sceneView.ReadyToRender();
		}

		// Token: 0x06000BA2 RID: 2978 RVA: 0x0000CF99 File Offset: 0x0000B199
		public void SetCleanScreenUntilLoadingDone(bool value)
		{
			this._sceneView.SetCleanScreenUntilLoadingDone(value);
		}

		// Token: 0x06000BA3 RID: 2979 RVA: 0x0000CFA7 File Offset: 0x0000B1A7
		public void ClearAll()
		{
			this._sceneView.ClearAll(true, true);
		}

		// Token: 0x06000BA4 RID: 2980 RVA: 0x0000CFB6 File Offset: 0x0000B1B6
		public void ClearRuntimeGPUMemory(bool remove_terrain)
		{
			this._sceneView.ClearAll(false, remove_terrain);
		}

		// Token: 0x06000BA5 RID: 2981 RVA: 0x0000CFC5 File Offset: 0x0000B1C5
		protected override void RefreshGlobalOrder(ref int currentOrder)
		{
			this._sceneView.SetRenderOrder(currentOrder);
			currentOrder++;
		}

		// Token: 0x06000BA6 RID: 2982 RVA: 0x0000CFDA File Offset: 0x0000B1DA
		public override bool HitTest(Vector2 position)
		{
			return true;
		}

		// Token: 0x06000BA7 RID: 2983 RVA: 0x0000CFDD File Offset: 0x0000B1DD
		public override bool HitTest()
		{
			return true;
		}

		// Token: 0x06000BA8 RID: 2984 RVA: 0x0000CFE0 File Offset: 0x0000B1E0
		public override bool FocusTest()
		{
			return true;
		}

		// Token: 0x040001F4 RID: 500
		private SceneView _sceneView;
	}
}
