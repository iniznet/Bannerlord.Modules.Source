using System;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.MountAndBlade.Multiplayer.View.MissionViews;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace TaleWorlds.MountAndBlade.Multiplayer.GauntletUI.Mission
{
	[OverrideView(typeof(MultiplayerAdminPanelUIHandler))]
	public class MissionGauntletAdminPanel : MissionView
	{
		public MissionGauntletAdminPanel()
		{
			this.ViewOrderPriority = 45;
		}

		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			this._multiplayerAdminComponent = base.Mission.GetMissionBehavior<MultiplayerAdminComponent>();
			this._dataSource = new MultiplayerAdminPanelVM(new Action<bool>(this.OnEscapeMenuToggled), this._multiplayerAdminComponent);
			this._multiplayerAdminComponent.OnShowAdminMenu += this.OnShowAdminPanel;
		}

		public override void OnMissionScreenFinalize()
		{
			this._multiplayerAdminComponent.OnShowAdminMenu -= this.OnShowAdminPanel;
			this._dataSource.OnFinalize();
			this._dataSource = null;
			this.OnEscapeMenuToggled(false);
			base.OnMissionScreenFinalize();
		}

		public override bool OnEscape()
		{
			if (this._isActive)
			{
				this.OnExitAdminPanel();
				return true;
			}
			return base.OnEscape();
		}

		private void OnExitAdminPanel()
		{
			this.OnEscapeMenuToggled(false);
		}

		private void OnShowAdminPanel()
		{
			this.OnEscapeMenuToggled(true);
		}

		private void OnEscapeMenuToggled(bool isOpened)
		{
			if (isOpened == this._isActive || !base.MissionScreen.SetDisplayDialog(isOpened))
			{
				return;
			}
			this._isActive = isOpened;
			if (isOpened)
			{
				this._gauntletLayer = new GauntletLayer(this.ViewOrderPriority, "GauntletLayer", false);
				this._movie = this._gauntletLayer.LoadMovie("Lobby.HostGame", this._dataSource);
				this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, 1);
				base.MissionScreen.AddLayer(this._gauntletLayer);
				return;
			}
			this._gauntletLayer.InputRestrictions.ResetInputRestrictions();
			base.MissionScreen.RemoveLayer(this._gauntletLayer);
			this._gauntletLayer = null;
		}

		private GauntletLayer _gauntletLayer;

		private MultiplayerAdminPanelVM _dataSource;

		private IGauntletMovie _movie;

		private bool _isActive;

		private MultiplayerAdminComponent _multiplayerAdminComponent;
	}
}
