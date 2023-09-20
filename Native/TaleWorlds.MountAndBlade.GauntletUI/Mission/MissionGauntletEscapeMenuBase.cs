using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.ViewModelCollection.EscapeMenu;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission
{
	public abstract class MissionGauntletEscapeMenuBase : MissionEscapeMenuView
	{
		protected MissionGauntletEscapeMenuBase(string viewFile)
		{
			base.OnMissionScreenInitialize();
			this._viewFile = viewFile;
			this.ViewOrderPriority = 50;
		}

		protected virtual List<EscapeMenuItemVM> GetEscapeMenuItems()
		{
			return null;
		}

		public override void OnMissionScreenFinalize()
		{
			this.DataSource.OnFinalize();
			this.DataSource = null;
			this._gauntletLayer = null;
			this._movie = null;
			base.OnMissionScreenFinalize();
		}

		public override bool OnEscape()
		{
			if (!this._isRenderingStarted)
			{
				return false;
			}
			if (!base.IsActive)
			{
				this.DataSource.RefreshItems(this.GetEscapeMenuItems());
			}
			return this.OnEscapeMenuToggled(!base.IsActive);
		}

		protected bool OnEscapeMenuToggled(bool isOpened)
		{
			if (base.IsActive == isOpened)
			{
				return false;
			}
			base.IsActive = isOpened;
			if (isOpened)
			{
				this.DataSource.RefreshValues();
				if (!GameNetwork.IsMultiplayer)
				{
					MBCommon.PauseGameEngine();
					Game.Current.GameStateManager.RegisterActiveStateDisableRequest(this);
				}
				this._gauntletLayer = new GauntletLayer(this.ViewOrderPriority, "GauntletLayer", false);
				this._gauntletLayer.IsFocusLayer = true;
				this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, 7);
				this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
				this._movie = this._gauntletLayer.LoadMovie(this._viewFile, this.DataSource);
				base.MissionScreen.AddLayer(this._gauntletLayer);
				ScreenManager.TrySetFocus(this._gauntletLayer);
			}
			else
			{
				if (!GameNetwork.IsMultiplayer)
				{
					MBCommon.UnPauseGameEngine();
					Game.Current.GameStateManager.UnregisterActiveStateDisableRequest(this);
				}
				this._gauntletLayer.InputRestrictions.ResetInputRestrictions();
				base.MissionScreen.RemoveLayer(this._gauntletLayer);
				this._movie = null;
				this._gauntletLayer = null;
			}
			return true;
		}

		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			if (base.IsActive && (this._gauntletLayer.Input.IsHotKeyReleased("ToggleEscapeMenu") || this._gauntletLayer.Input.IsHotKeyReleased("Exit")))
			{
				this.OnEscapeMenuToggled(false);
			}
		}

		public override void OnSceneRenderingStarted()
		{
			base.OnSceneRenderingStarted();
			this._isRenderingStarted = true;
		}

		protected EscapeMenuVM DataSource;

		private GauntletLayer _gauntletLayer;

		private IGauntletMovie _movie;

		private string _viewFile;

		private bool _isRenderingStarted;
	}
}
