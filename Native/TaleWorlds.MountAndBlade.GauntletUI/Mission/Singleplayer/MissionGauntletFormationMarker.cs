using System;
using System.Collections.Generic;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer;
using TaleWorlds.MountAndBlade.ViewModelCollection.HUD.FormationMarker;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission.Singleplayer
{
	// Token: 0x0200002F RID: 47
	[OverrideView(typeof(MissionFormationMarkerUIHandler))]
	public class MissionGauntletFormationMarker : MissionGauntletBattleUIBase
	{
		// Token: 0x06000241 RID: 577 RVA: 0x0000C92C File Offset: 0x0000AB2C
		protected override void OnCreateView()
		{
			this._formationTargets = new List<CompassItemUpdateParams>();
			this._dataSource = new MissionFormationMarkerVM(base.Mission, base.MissionScreen.CombatCamera);
			this._gauntletLayer = new GauntletLayer(this.ViewOrderPriority, "GauntletLayer", false);
			this._gauntletLayer.LoadMovie("FormationMarker", this._dataSource);
			base.MissionScreen.AddLayer(this._gauntletLayer);
			this._orderHandler = base.Mission.GetMissionBehavior<MissionGauntletSingleplayerOrderUIHandler>();
		}

		// Token: 0x06000242 RID: 578 RVA: 0x0000C9B0 File Offset: 0x0000ABB0
		protected override void OnDestroyView()
		{
			base.MissionScreen.RemoveLayer(this._gauntletLayer);
			this._gauntletLayer = null;
			this._dataSource.OnFinalize();
			this._dataSource = null;
		}

		// Token: 0x06000243 RID: 579 RVA: 0x0000C9DC File Offset: 0x0000ABDC
		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			if (base.IsViewActive)
			{
				if (!this._orderHandler.IsBattleDeployment)
				{
					this._dataSource.IsEnabled = base.Input.IsGameKeyDown(5);
				}
				this._dataSource.Tick(dt);
			}
		}

		// Token: 0x06000244 RID: 580 RVA: 0x0000CA28 File Offset: 0x0000AC28
		public override void OnPhotoModeActivated()
		{
			base.OnPhotoModeActivated();
			if (base.IsViewActive)
			{
				this._gauntletLayer._gauntletUIContext.ContextAlpha = 0f;
			}
		}

		// Token: 0x06000245 RID: 581 RVA: 0x0000CA4D File Offset: 0x0000AC4D
		public override void OnPhotoModeDeactivated()
		{
			base.OnPhotoModeDeactivated();
			if (base.IsViewActive)
			{
				this._gauntletLayer._gauntletUIContext.ContextAlpha = 1f;
			}
		}

		// Token: 0x0400011E RID: 286
		private MissionFormationMarkerVM _dataSource;

		// Token: 0x0400011F RID: 287
		private GauntletLayer _gauntletLayer;

		// Token: 0x04000120 RID: 288
		private List<CompassItemUpdateParams> _formationTargets;

		// Token: 0x04000121 RID: 289
		private MissionGauntletSingleplayerOrderUIHandler _orderHandler;
	}
}
