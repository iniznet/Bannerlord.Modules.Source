using System;
using SandBox.Missions.MissionLogics;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace SandBox.View.Missions
{
	public class MissionItemCalatogView : MissionView
	{
		public override void AfterStart()
		{
			base.AfterStart();
			this._itemCatalogController = base.Mission.GetMissionBehavior<ItemCatalogController>();
			this._itemCatalogController.BeforeCatalogTick += this.OnBeforeCatalogTick;
			this._itemCatalogController.AfterCatalogTick += this.OnAfterCatalogTick;
		}

		private void OnBeforeCatalogTick(int currentItemIndex)
		{
			Utilities.TakeScreenshot("ItemCatalog/" + this._itemCatalogController.AllItems[currentItemIndex - 1].Name + ".bmp");
		}

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

		private ItemCatalogController _itemCatalogController;
	}
}
