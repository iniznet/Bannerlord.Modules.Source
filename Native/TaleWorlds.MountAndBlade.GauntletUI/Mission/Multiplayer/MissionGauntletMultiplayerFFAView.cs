using System;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.Multiplayer;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission.Multiplayer
{
	[OverrideView(typeof(MissionMultiplayerFreeForAllUIHandler))]
	public class MissionGauntletMultiplayerFFAView : MissionView
	{
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			this.ViewOrderPriority = 15;
			SpriteData spriteData = UIResourceManager.SpriteData;
			TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
			ResourceDepot uiresourceDepot = UIResourceManager.UIResourceDepot;
			this._mpMissionCategory = spriteData.SpriteCategories["ui_mpmission"];
			this._mpMissionCategory.Load(resourceContext, uiresourceDepot);
		}

		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			SpriteCategory mpMissionCategory = this._mpMissionCategory;
			if (mpMissionCategory == null)
			{
				return;
			}
			mpMissionCategory.Unload();
		}

		private SpriteCategory _mpMissionCategory;
	}
}
