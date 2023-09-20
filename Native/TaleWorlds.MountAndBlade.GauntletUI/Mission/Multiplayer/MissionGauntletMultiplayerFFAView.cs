using System;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.Multiplayer;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission.Multiplayer
{
	// Token: 0x02000041 RID: 65
	[OverrideView(typeof(MissionMultiplayerFreeForAllUIHandler))]
	public class MissionGauntletMultiplayerFFAView : MissionView
	{
		// Token: 0x06000308 RID: 776 RVA: 0x00010F4C File Offset: 0x0000F14C
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

		// Token: 0x06000309 RID: 777 RVA: 0x00010F9C File Offset: 0x0000F19C
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

		// Token: 0x0400019C RID: 412
		private SpriteCategory _mpMissionCategory;
	}
}
