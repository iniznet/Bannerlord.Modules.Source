using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.View.Map
{
	// Token: 0x02000039 RID: 57
	public class CampaignCompass : CampaignEntityVisualComponent
	{
		// Token: 0x060001CB RID: 459 RVA: 0x00011FD0 File Offset: 0x000101D0
		protected override void OnInitialize()
		{
			base.OnInitialize();
			this.CreateCompassEntity();
		}

		// Token: 0x060001CC RID: 460 RVA: 0x00011FE0 File Offset: 0x000101E0
		public override void OnVisualTick(MapScreen screen, float realDt, float dt)
		{
			base.OnVisualTick(screen, realDt, dt);
			MatrixFrame cameraFrame = screen._mapCameraView.CameraFrame;
			MatrixFrame identity = MatrixFrame.Identity;
			float num = 10.555058f * (-0.17f / (Screen.RealScreenResolutionWidth / Screen.RealScreenResolutionHeight));
			identity.origin = cameraFrame.TransformToParent(new Vec3(num * 1.045f, -1.080246f, -4.198421f, -1f));
			identity.rotation.ApplyScaleLocal(new Vec3(8.5f, 8.5f, 8.5f, -1f));
			this._compassEntity.SetFrame(ref identity);
			this._compassEntity.GetFirstMesh().SetMeshRenderOrder(255);
			this._compassEntity.SetVisibilityExcludeParents(!MBEditor.IsEditModeOn);
		}

		// Token: 0x060001CD RID: 461 RVA: 0x000120A3 File Offset: 0x000102A3
		public override void OnLoadSavedGame()
		{
			base.OnLoadSavedGame();
			this.CreateCompassEntity();
		}

		// Token: 0x060001CE RID: 462 RVA: 0x000120B4 File Offset: 0x000102B4
		private void CreateCompassEntity()
		{
			this._compassEntity = GameEntity.CreateEmpty(((MapScene)Campaign.Current.MapSceneWrapper).Scene, true);
			MetaMesh copy = MetaMesh.GetCopy("compass", true, false);
			MatrixFrame identity = MatrixFrame.Identity;
			identity.Scale(new Vec3(0.28f, 0.28f, 0.28f, -1f));
			copy.Frame = identity;
			int meshCount = copy.MeshCount;
			for (int i = 0; i < meshCount; i++)
			{
				Mesh meshAtIndex = copy.GetMeshAtIndex(i);
				meshAtIndex.SetMeshRenderOrder(255);
				meshAtIndex.VisibilityMask = 1;
				Material material = meshAtIndex.GetMaterial().CreateCopy();
				material.Flags |= 256;
				meshAtIndex.SetMaterial(material);
			}
			this._compassEntity.AddComponent(copy);
		}

		// Token: 0x040000FB RID: 251
		private GameEntity _compassEntity;
	}
}
