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
	// Token: 0x0200000D RID: 13
	[GameStateScreen(typeof(ProfileSelectionState))]
	public class GauntletProfileSelectionScreen : MBProfileSelectionScreenBase
	{
		// Token: 0x06000050 RID: 80 RVA: 0x00003C8B File Offset: 0x00001E8B
		public GauntletProfileSelectionScreen(ProfileSelectionState state)
			: base(state)
		{
			this._state = state;
			this._state.OnProfileSelection += new ProfileSelectionState.OnProfileSelectionEvent(this.OnProfileSelection);
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00003CB2 File Offset: 0x00001EB2
		private void OnProfileSelection()
		{
			ProfileSelectionVM dataSource = this._dataSource;
			if (dataSource == null)
			{
				return;
			}
			dataSource.OnActivate(this._state.IsDirectPlayPossible);
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00003CD0 File Offset: 0x00001ED0
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

		// Token: 0x06000053 RID: 83 RVA: 0x00003D99 File Offset: 0x00001F99
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

		// Token: 0x06000054 RID: 84 RVA: 0x00003DBC File Offset: 0x00001FBC
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

		// Token: 0x06000055 RID: 85 RVA: 0x00003E24 File Offset: 0x00002024
		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
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

		// Token: 0x04000035 RID: 53
		private GauntletLayer _gauntletLayer;

		// Token: 0x04000036 RID: 54
		private ProfileSelectionVM _dataSource;

		// Token: 0x04000037 RID: 55
		private ProfileSelectionState _state;
	}
}
