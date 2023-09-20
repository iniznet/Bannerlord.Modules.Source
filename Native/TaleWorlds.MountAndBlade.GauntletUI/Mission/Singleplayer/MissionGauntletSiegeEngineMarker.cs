using System;
using System.Collections.Generic;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer;
using TaleWorlds.MountAndBlade.ViewModelCollection.HUD.FormationMarker;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission.Singleplayer
{
	// Token: 0x02000034 RID: 52
	[OverrideView(typeof(MissionSiegeEngineMarkerView))]
	public class MissionGauntletSiegeEngineMarker : MissionGauntletBattleUIBase
	{
		// Token: 0x06000279 RID: 633 RVA: 0x0000DD18 File Offset: 0x0000BF18
		protected override void OnCreateView()
		{
			this._dataSource = new MissionSiegeEngineMarkerVM(base.Mission, base.MissionScreen.CombatCamera);
			this._gauntletLayer = new GauntletLayer(this.ViewOrderPriority, "GauntletLayer", false);
			this._gauntletLayer.LoadMovie("SiegeEngineMarker", this._dataSource);
			base.MissionScreen.AddLayer(this._gauntletLayer);
			this._orderHandler = base.Mission.GetMissionBehavior<MissionGauntletSingleplayerOrderUIHandler>();
		}

		// Token: 0x0600027A RID: 634 RVA: 0x0000DD94 File Offset: 0x0000BF94
		public override void OnDeploymentFinished()
		{
			base.OnDeploymentFinished();
			List<SiegeWeapon> list = new List<SiegeWeapon>();
			using (List<MissionObject>.Enumerator enumerator = base.Mission.ActiveMissionObjects.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SiegeWeapon siegeWeapon;
					if ((siegeWeapon = enumerator.Current as SiegeWeapon) != null && siegeWeapon.DestructionComponent != null && siegeWeapon.Side != -1)
					{
						list.Add(siegeWeapon);
					}
				}
			}
			this._dataSource.InitializeWith(list);
		}

		// Token: 0x0600027B RID: 635 RVA: 0x0000DE20 File Offset: 0x0000C020
		protected override void OnDestroyView()
		{
			base.MissionScreen.RemoveLayer(this._gauntletLayer);
			this._gauntletLayer = null;
			this._dataSource.OnFinalize();
			this._dataSource = null;
		}

		// Token: 0x0600027C RID: 636 RVA: 0x0000DE4C File Offset: 0x0000C04C
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

		// Token: 0x0600027D RID: 637 RVA: 0x0000DE98 File Offset: 0x0000C098
		public override void OnPhotoModeActivated()
		{
			base.OnPhotoModeActivated();
			if (base.IsViewActive)
			{
				this._gauntletLayer._gauntletUIContext.ContextAlpha = 0f;
			}
		}

		// Token: 0x0600027E RID: 638 RVA: 0x0000DEBD File Offset: 0x0000C0BD
		public override void OnPhotoModeDeactivated()
		{
			base.OnPhotoModeDeactivated();
			if (base.IsViewActive)
			{
				this._gauntletLayer._gauntletUIContext.ContextAlpha = 1f;
			}
		}

		// Token: 0x04000145 RID: 325
		private List<SiegeWeapon> siegeEngines = new List<SiegeWeapon>();

		// Token: 0x04000146 RID: 326
		private MissionSiegeEngineMarkerVM _dataSource;

		// Token: 0x04000147 RID: 327
		private GauntletLayer _gauntletLayer;

		// Token: 0x04000148 RID: 328
		private MissionGauntletSingleplayerOrderUIHandler _orderHandler;
	}
}
