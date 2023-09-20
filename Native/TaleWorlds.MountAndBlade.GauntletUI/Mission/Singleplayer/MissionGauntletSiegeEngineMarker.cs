using System;
using System.Collections.Generic;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer;
using TaleWorlds.MountAndBlade.ViewModelCollection.HUD.FormationMarker;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission.Singleplayer
{
	[OverrideView(typeof(MissionSiegeEngineMarkerView))]
	public class MissionGauntletSiegeEngineMarker : MissionGauntletBattleUIBase
	{
		protected override void OnCreateView()
		{
			this._dataSource = new MissionSiegeEngineMarkerVM(base.Mission, base.MissionScreen.CombatCamera);
			this._gauntletLayer = new GauntletLayer(this.ViewOrderPriority, "GauntletLayer", false);
			this._gauntletLayer.LoadMovie("SiegeEngineMarker", this._dataSource);
			base.MissionScreen.AddLayer(this._gauntletLayer);
			this._orderHandler = base.Mission.GetMissionBehavior<MissionGauntletSingleplayerOrderUIHandler>();
		}

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

		protected override void OnDestroyView()
		{
			base.MissionScreen.RemoveLayer(this._gauntletLayer);
			this._gauntletLayer = null;
			this._dataSource.OnFinalize();
			this._dataSource = null;
		}

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

		public override void OnPhotoModeActivated()
		{
			base.OnPhotoModeActivated();
			if (base.IsViewActive)
			{
				this._gauntletLayer._gauntletUIContext.ContextAlpha = 0f;
			}
		}

		public override void OnPhotoModeDeactivated()
		{
			base.OnPhotoModeDeactivated();
			if (base.IsViewActive)
			{
				this._gauntletLayer._gauntletUIContext.ContextAlpha = 1f;
			}
		}

		private List<SiegeWeapon> siegeEngines = new List<SiegeWeapon>();

		private MissionSiegeEngineMarkerVM _dataSource;

		private GauntletLayer _gauntletLayer;

		private MissionGauntletSingleplayerOrderUIHandler _orderHandler;
	}
}
