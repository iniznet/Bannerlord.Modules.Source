using System;
using System.Collections.Generic;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer;
using TaleWorlds.MountAndBlade.ViewModelCollection.HUD.FormationMarker;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission.Singleplayer
{
	[OverrideView(typeof(MissionFormationMarkerUIHandler))]
	public class MissionGauntletFormationMarker : MissionGauntletBattleUIBase
	{
		protected override void OnCreateView()
		{
			this._formationTargets = new List<CompassItemUpdateParams>();
			this._dataSource = new MissionFormationMarkerVM(base.Mission, base.MissionScreen.CombatCamera);
			this._gauntletLayer = new GauntletLayer(this.ViewOrderPriority, "GauntletLayer", false);
			this._gauntletLayer.LoadMovie("FormationMarker", this._dataSource);
			base.MissionScreen.AddLayer(this._gauntletLayer);
			this._orderHandler = base.Mission.GetMissionBehavior<MissionGauntletSingleplayerOrderUIHandler>();
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

		private MissionFormationMarkerVM _dataSource;

		private GauntletLayer _gauntletLayer;

		private List<CompassItemUpdateParams> _formationTargets;

		private MissionGauntletSingleplayerOrderUIHandler _orderHandler;
	}
}
