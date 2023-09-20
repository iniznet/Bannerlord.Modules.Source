using System;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Multiplayer.View.MissionViews;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.TeamSelection;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.Multiplayer.GauntletUI.Mission
{
	[OverrideView(typeof(MultiplayerCultureSelectUIHandler))]
	public class MissionGauntletCultureSelection : MissionView
	{
		public MissionGauntletCultureSelection()
		{
			this.ViewOrderPriority = 22;
		}

		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			this._missionLobbyComponent = base.Mission.GetMissionBehavior<MissionLobbyComponent>();
			this._missionLobbyComponent.OnCultureSelectionRequested += this.OnCultureSelectionRequested;
		}

		public override void OnMissionScreenFinalize()
		{
			this._missionLobbyComponent.OnCultureSelectionRequested -= this.OnCultureSelectionRequested;
			base.OnMissionScreenFinalize();
		}

		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			if (this._toOpen && base.MissionScreen.SetDisplayDialog(true))
			{
				this._toOpen = false;
				this.OnOpen();
			}
		}

		private void OnOpen()
		{
			this._dataSource = new MultiplayerCultureSelectVM(new Action<BasicCultureObject>(this.OnCultureSelected), new Action(this.OnClose));
			this._gauntletLayer = new GauntletLayer(this.ViewOrderPriority, "GauntletLayer", false);
			this._gauntletLayer.LoadMovie("MultiplayerCultureSelection", this._dataSource);
			this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, 7);
			SpriteData spriteData = UIResourceManager.SpriteData;
			TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
			ResourceDepot uiresourceDepot = UIResourceManager.UIResourceDepot;
			base.MissionScreen.AddLayer(this._gauntletLayer);
		}

		private void OnClose()
		{
			base.MissionScreen.RemoveLayer(this._gauntletLayer);
			base.MissionScreen.SetDisplayDialog(false);
			this._gauntletLayer.InputRestrictions.ResetInputRestrictions();
			this._gauntletLayer = null;
			this._dataSource.OnFinalize();
			this._dataSource = null;
		}

		private void OnCultureSelectionRequested()
		{
			this._toOpen = true;
		}

		private void OnCultureSelected(BasicCultureObject culture)
		{
			this._missionLobbyComponent.OnCultureSelected(culture);
			this.OnClose();
		}

		private GauntletLayer _gauntletLayer;

		private MultiplayerCultureSelectVM _dataSource;

		private MissionLobbyComponent _missionLobbyComponent;

		private bool _toOpen;
	}
}
