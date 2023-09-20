using System;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.MountAndBlade.ViewModelCollection.ProfileSelection;
using TaleWorlds.PlatformService;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.GauntletUI
{
	[GameStateScreen(typeof(ProfileSelectionState))]
	public class GauntletProfileSelectionScreen : MBProfileSelectionScreenBase
	{
		public GauntletProfileSelectionScreen(ProfileSelectionState state)
			: base(state)
		{
			this._state = state;
			this._state.OnProfileSelection += new ProfileSelectionState.OnProfileSelectionEvent(this.OnProfileSelection);
		}

		private void OnProfileSelection()
		{
			ProfileSelectionVM dataSource = this._dataSource;
			if (dataSource == null)
			{
				return;
			}
			dataSource.OnActivate(this._state.IsDirectPlayPossible);
		}

		protected override void OnInitialize()
		{
			base.OnInitialize();
			this._gauntletLayer = new GauntletLayer(1, "GauntletLayer", false);
			this._dataSource = new ProfileSelectionVM(this._state.IsDirectPlayPossible);
			ProfileSelectionVM dataSource = this._dataSource;
			if (dataSource != null)
			{
				dataSource.OnActivate(this._state.IsDirectPlayPossible);
			}
			this._gauntletLayer.LoadMovie("ProfileSelectionScreen", this._dataSource);
			this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, 7);
			this._gauntletLayer.IsFocusLayer = true;
			this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			ScreenManager.TrySetFocus(this._gauntletLayer);
			base.AddLayer(this._gauntletLayer);
			MouseManager.ShowCursor(false);
			MouseManager.ShowCursor(true);
		}

		protected override void OnActivate()
		{
			base.OnActivate();
			ProfileSelectionVM dataSource = this._dataSource;
			if (dataSource == null)
			{
				return;
			}
			dataSource.OnActivate(this._state.IsDirectPlayPossible);
		}

		protected override void OnFinalize()
		{
			base.OnFinalize();
			this._state.OnProfileSelection -= new ProfileSelectionState.OnProfileSelectionEvent(this.OnProfileSelection);
			this._gauntletLayer.IsFocusLayer = false;
			ScreenManager.TryLoseFocus(this._gauntletLayer);
			base.RemoveLayer(this._gauntletLayer);
			this._gauntletLayer = null;
			this._dataSource.OnFinalize();
			this._dataSource = null;
		}

		protected override void OnProfileSelectionTick(float dt)
		{
			base.OnProfileSelectionTick(dt);
			if (!this._state.IsDirectPlayPossible || !this._gauntletLayer.Input.IsHotKeyReleased("Play"))
			{
				if (this._gauntletLayer.Input.IsHotKeyReleased("SelectProfile"))
				{
					base.OnActivateProfileSelection();
				}
				return;
			}
			if (PlatformServices.Instance.UserLoggedIn)
			{
				this._state.StartGame();
				return;
			}
			base.OnActivateProfileSelection();
		}

		private GauntletLayer _gauntletLayer;

		private ProfileSelectionVM _dataSource;

		private ProfileSelectionState _state;
	}
}
