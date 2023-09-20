using System;
using System.Collections.Generic;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
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
			this._formationTargetHandler = base.Mission.GetMissionBehavior<MissionFormationTargetSelectionHandler>();
			if (this._formationTargetHandler != null)
			{
				this._formationTargetHandler.OnFormationFocused += this.OnFormationFocusedFromHandler;
			}
		}

		protected override void OnDestroyView()
		{
			if (this._formationTargetHandler != null)
			{
				this._formationTargetHandler.OnFormationFocused -= this.OnFormationFocusedFromHandler;
			}
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
					MissionFormationMarkerVM dataSource = this._dataSource;
					bool flag;
					if (!base.Input.IsGameKeyDown(5))
					{
						MissionGauntletSingleplayerOrderUIHandler orderHandler = this._orderHandler;
						flag = orderHandler != null && orderHandler.IsOrderMenuActive;
					}
					else
					{
						flag = true;
					}
					dataSource.IsEnabled = flag;
					if (this._formationTargetHandler != null)
					{
						this._dataSource.SetFocusedFormations(this._focusedFormationsCache);
					}
				}
				MissionFormationMarkerVM dataSource2 = this._dataSource;
				bool flag2;
				if (this._formationTargetHandler != null)
				{
					MissionGauntletSingleplayerOrderUIHandler orderHandler2 = this._orderHandler;
					flag2 = orderHandler2 != null && orderHandler2.IsOrderMenuActive;
				}
				else
				{
					flag2 = false;
				}
				dataSource2.IsFormationTargetRelevant = flag2;
				this._dataSource.Tick(dt);
			}
		}

		private void OnFormationFocusedFromHandler(MBReadOnlyList<Formation> obj)
		{
			this._focusedFormationsCache = obj;
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

		private MBReadOnlyList<Formation> _focusedFormationsCache;

		private MissionGauntletSingleplayerOrderUIHandler _orderHandler;

		private MissionFormationTargetSelectionHandler _formationTargetHandler;
	}
}
