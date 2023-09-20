using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.Screens;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.View.Map
{
	// Token: 0x02000045 RID: 69
	public class MapCursor
	{
		// Token: 0x06000247 RID: 583 RVA: 0x000152A8 File Offset: 0x000134A8
		public void Initialize(MapScreen parentMapScreen)
		{
			this._targetCircleRotationStartTime = 0f;
			this._smallAtlasTextureIndex = 0;
			this._mapScreen = parentMapScreen;
			Scene scene = (Campaign.Current.MapSceneWrapper as MapScene).Scene;
			this._gameCursorValidDecalMaterial = Material.GetFromResource("map_cursor_valid_decal");
			this._gameCursorInvalidDecalMaterial = Material.GetFromResource("map_cursor_invalid_decal");
			this._mapCursorDecalEntity = GameEntity.CreateEmpty(scene, true);
			this._mapCursorDecalEntity.Name = "tCursor";
			this._mapCursorDecal = Decal.CreateDecal(null);
			this._mapCursorDecal.SetMaterial(this._gameCursorValidDecalMaterial);
			this._mapCursorDecalEntity.AddComponent(this._mapCursorDecal);
			scene.AddDecalInstance(this._mapCursorDecal, "editor_set", true);
			MatrixFrame frame = this._mapCursorDecalEntity.GetFrame();
			frame.Scale(new Vec3(0.38f, 0.38f, 0.38f, -1f));
			this._mapCursorDecal.SetFrame(frame);
		}

		// Token: 0x06000248 RID: 584 RVA: 0x00015398 File Offset: 0x00013598
		public void BeforeTick(float dt)
		{
			SceneLayer sceneLayer = this._mapScreen.SceneLayer;
			Camera camera = this._mapScreen._mapCameraView.Camera;
			float cameraDistance = this._mapScreen._mapCameraView.CameraDistance;
			Vec3 zero = Vec3.Zero;
			Vec3 zero2 = Vec3.Zero;
			Vec2 vec = sceneLayer.SceneView.ScreenPointToViewportPoint(new Vec2(0.5f, 0.5f));
			camera.ViewportPointToWorldRay(ref zero, ref zero2, vec);
			PathFaceRecord pathFaceRecord = default(PathFaceRecord);
			float num;
			Vec3 vec2;
			this._mapScreen.GetCursorIntersectionPoint(ref zero, ref zero2, out num, out vec2, ref pathFaceRecord, 79617);
			Vec3 vec3;
			sceneLayer.SceneView.ProjectedMousePositionOnGround(ref vec2, ref vec3, false, 0, false);
			if (this._mapCursorDecalEntity != null)
			{
				this._smallAtlasTextureIndex = this.GetCircleIndex();
				bool flag = Campaign.Current.MapSceneWrapper.AreFacesOnSameIsland(pathFaceRecord, MobileParty.MainParty.CurrentNavigationFace, false);
				this._mapCursorDecal.SetMaterial((flag || this._anotherEntityHiglighted) ? this._gameCursorValidDecalMaterial : this._gameCursorInvalidDecalMaterial);
				this._mapCursorDecal.SetVectorArgument(0.166f, 1f, 0.166f * (float)this._smallAtlasTextureIndex, 0f);
				this.SetAlpha(this._anotherEntityHiglighted ? 0.2f : 1f);
				MatrixFrame frame = this._mapCursorDecalEntity.GetFrame();
				frame.origin = vec2;
				bool flag2 = !this._smoothRotationNormalStart.IsNonZero;
				Vec3 vec4 = ((cameraDistance > 160f) ? Vec3.Up : vec3);
				if (!this._smoothRotationNormalEnd.NearlyEquals(vec4, 1E-05f))
				{
					this._smoothRotationNormalStart = (flag2 ? vec4 : this._smoothRotationNormalCurrent);
					this._smoothRotationNormalEnd = vec4;
					this._smoothRotationNormalStart.Normalize();
					this._smoothRotationNormalEnd.Normalize();
					this._smoothRotationAlpha = 0f;
				}
				this._smoothRotationNormalCurrent = Vec3.Lerp(this._smoothRotationNormalStart, this._smoothRotationNormalEnd, this._smoothRotationAlpha);
				this._smoothRotationAlpha += 12f * dt;
				this._smoothRotationAlpha = MathF.Clamp(this._smoothRotationAlpha, 0f, 1f);
				this._smoothRotationNormalCurrent.Normalize();
				frame.rotation.f = camera.Frame.rotation.f;
				frame.rotation.f.z = 0f;
				frame.rotation.f.Normalize();
				frame.rotation.u = this._smoothRotationNormalCurrent;
				frame.rotation.u.Normalize();
				frame.rotation.s = Vec3.CrossProduct(frame.rotation.u, frame.rotation.f);
				float num2 = (cameraDistance + 80f) * (cameraDistance + 80f) / 10000f;
				num2 = MathF.Clamp(num2, 0.2f, 38f);
				frame.Scale(Vec3.One * num2);
				this._mapCursorDecalEntity.SetGlobalFrame(ref frame);
				this._anotherEntityHiglighted = false;
			}
		}

		// Token: 0x06000249 RID: 585 RVA: 0x000156A4 File Offset: 0x000138A4
		public void SetVisible(bool value)
		{
			if (value)
			{
				if (this._gameCursorActive && !this._mapScreen.GetMouseVisible())
				{
					return;
				}
				this._mapScreen.SetMouseVisible(false);
				this._mapCursorDecalEntity.SetVisibilityExcludeParents(true);
				if (this._mapScreen.CurrentVisualOfTooltip != null)
				{
					this._mapScreen.RemoveMapTooltip();
				}
				Vec2 resolution = Input.Resolution;
				Input.SetMousePosition((int)(resolution.X / 2f), (int)(resolution.Y / 2f));
				this._gameCursorActive = true;
				return;
			}
			else
			{
				bool flag = !(GameStateManager.Current.ActiveState is MapState) || (!this._mapScreen.SceneLayer.Input.IsKeyDown(225) && !this._mapScreen.SceneLayer.Input.IsKeyDown(226));
				if (!this._gameCursorActive && this._mapScreen.GetMouseVisible() == flag)
				{
					return;
				}
				this._mapScreen.SetMouseVisible(flag);
				this._mapCursorDecalEntity.SetVisibilityExcludeParents(false);
				this._gameCursorActive = false;
				return;
			}
		}

		// Token: 0x0600024A RID: 586 RVA: 0x000157B3 File Offset: 0x000139B3
		protected internal void OnMapTerrainClick()
		{
			this._targetCircleRotationStartTime = MBCommon.GetApplicationTime();
		}

		// Token: 0x0600024B RID: 587 RVA: 0x000157C0 File Offset: 0x000139C0
		protected internal void OnAnotherEntityHighlighted()
		{
			this._anotherEntityHiglighted = true;
		}

		// Token: 0x0600024C RID: 588 RVA: 0x000157CC File Offset: 0x000139CC
		protected internal void SetAlpha(float alpha)
		{
			Color color = Color.FromUint(this._mapCursorDecal.GetFactor1());
			Color color2;
			color2..ctor(color.Red, color.Green, color.Blue, alpha);
			this._mapCursorDecal.SetFactor1(color2.ToUnsignedInteger());
		}

		// Token: 0x0600024D RID: 589 RVA: 0x00015818 File Offset: 0x00013A18
		private int GetCircleIndex()
		{
			int num = (int)((MBCommon.GetApplicationTime() - this._targetCircleRotationStartTime) / 0.033f);
			if (num >= 10)
			{
				return 0;
			}
			int num2 = num % 10;
			if (num2 >= 5)
			{
				num2 = 10 - num2 - 1;
			}
			return num2;
		}

		// Token: 0x04000138 RID: 312
		private const string GameCursorValidDecalMaterialName = "map_cursor_valid_decal";

		// Token: 0x04000139 RID: 313
		private const string GameCursorInvalidDecalMaterialName = "map_cursor_invalid_decal";

		// Token: 0x0400013A RID: 314
		private const float CursorDecalBaseScale = 0.38f;

		// Token: 0x0400013B RID: 315
		private GameEntity _mapCursorDecalEntity;

		// Token: 0x0400013C RID: 316
		private Decal _mapCursorDecal;

		// Token: 0x0400013D RID: 317
		private MapScreen _mapScreen;

		// Token: 0x0400013E RID: 318
		private Material _gameCursorValidDecalMaterial;

		// Token: 0x0400013F RID: 319
		private Material _gameCursorInvalidDecalMaterial;

		// Token: 0x04000140 RID: 320
		private Vec3 _smoothRotationNormalStart;

		// Token: 0x04000141 RID: 321
		private Vec3 _smoothRotationNormalEnd;

		// Token: 0x04000142 RID: 322
		private Vec3 _smoothRotationNormalCurrent;

		// Token: 0x04000143 RID: 323
		private float _smoothRotationAlpha;

		// Token: 0x04000144 RID: 324
		private int _smallAtlasTextureIndex;

		// Token: 0x04000145 RID: 325
		private float _targetCircleRotationStartTime;

		// Token: 0x04000146 RID: 326
		private bool _gameCursorActive;

		// Token: 0x04000147 RID: 327
		private bool _anotherEntityHiglighted;
	}
}
