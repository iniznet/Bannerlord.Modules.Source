using System;
using SandBox.Missions.MissionLogics;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace SandBox.View.Missions
{
	// Token: 0x02000019 RID: 25
	public class MissionItemCalatogView : MissionView
	{
		// Token: 0x0600009F RID: 159 RVA: 0x00008FC0 File Offset: 0x000071C0
		public override void AfterStart()
		{
			base.AfterStart();
			this._itemCatalogController = base.Mission.GetMissionBehavior<ItemCatalogController>();
			this._itemCatalogController.BeforeCatalogTick += this.OnBeforeCatalogTick;
			this._itemCatalogController.AfterCatalogTick += this.OnAfterCatalogTick;
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00009012 File Offset: 0x00007212
		private void OnBeforeCatalogTick(int currentItemIndex)
		{
			Utilities.TakeScreenshot("ItemCatalog/" + this._itemCatalogController.AllItems[currentItemIndex - 1].Name + ".bmp");
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x00009040 File Offset: 0x00007240
		private void OnAfterCatalogTick()
		{
			MatrixFrame matrixFrame = default(MatrixFrame);
			Vec3 lookDirection = base.Mission.MainAgent.LookDirection;
			matrixFrame.origin = base.Mission.MainAgent.Position + lookDirection * 2f + new Vec3(0f, 0f, 1.273f, -1f);
			matrixFrame.rotation.u = lookDirection;
			matrixFrame.rotation.s = new Vec3(1f, 0f, 0f, -1f);
			matrixFrame.rotation.f = new Vec3(0f, 0f, 1f, -1f);
			matrixFrame.rotation.Orthonormalize();
			base.Mission.SetCameraFrame(ref matrixFrame, 1f);
			Camera camera = Camera.CreateCamera();
			camera.Frame = matrixFrame;
			base.MissionScreen.CustomCamera = camera;
		}

		// Token: 0x04000074 RID: 116
		private ItemCatalogController _itemCatalogController;
	}
}
