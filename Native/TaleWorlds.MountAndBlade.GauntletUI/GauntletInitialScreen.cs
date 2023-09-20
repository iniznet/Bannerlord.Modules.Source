using System;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Options;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions;
using TaleWorlds.MountAndBlade.ViewModelCollection.InitialMenu;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.GauntletUI
{
	// Token: 0x0200000B RID: 11
	[GameStateScreen(typeof(InitialState))]
	public class GauntletInitialScreen : MBInitialScreenBase
	{
		// Token: 0x0600003E RID: 62 RVA: 0x00003280 File Offset: 0x00001480
		public GauntletInitialScreen(InitialState initialState)
			: base(initialState)
		{
		}

		// Token: 0x0600003F RID: 63 RVA: 0x0000328C File Offset: 0x0000148C
		protected override void OnInitialize()
		{
			base.OnInitialize();
			this._dataSource = new InitialMenuVM(base._state);
			this._gauntletLayer = new GauntletLayer(1, "GauntletLayer", false);
			this._gauntletLayer.LoadMovie("InitialScreen", this._dataSource);
			this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, 3);
			this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			base.AddLayer(this._gauntletLayer);
			this._gauntletLayer.IsFocusLayer = true;
			ScreenManager.TrySetFocus(this._gauntletLayer);
			if (NativeOptions.GetConfig(65) < 2f)
			{
				this._brightnessOptionDataSource = new BrightnessOptionVM(new Action<bool>(this.OnCloseBrightness))
				{
					Visible = true
				};
				this._gauntletBrightnessLayer = new GauntletLayer(2, "GauntletLayer", false);
				this._gauntletBrightnessLayer.InputRestrictions.SetInputRestrictions(true, 3);
				this._brightnessOptionMovie = this._gauntletBrightnessLayer.LoadMovie("BrightnessOption", this._brightnessOptionDataSource);
				base.AddLayer(this._gauntletBrightnessLayer);
			}
			MouseManager.ShowCursor(false);
			MouseManager.ShowCursor(true);
			GauntletGameNotification gauntletGameNotification = GauntletGameNotification.Current;
			if (gauntletGameNotification != null)
			{
				gauntletGameNotification.LoadMovie(false);
			}
			GauntletChatLogView gauntletChatLogView = GauntletChatLogView.Current;
			if (gauntletChatLogView != null)
			{
				gauntletChatLogView.LoadMovie(false);
			}
			InformationManager.ClearAllMessages();
			base._state.OnGameContentUpdated += new OnGameContentUpdatedDelegate(this.OnGameContentUpdated);
			this.SetGainNavigationAfterFrames(3);
		}

		// Token: 0x06000040 RID: 64 RVA: 0x000033F4 File Offset: 0x000015F4
		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			if (this._gauntletLayer.Input.IsHotKeyReleased("Exit"))
			{
				BrightnessOptionVM brightnessOptionDataSource = this._brightnessOptionDataSource;
				if (brightnessOptionDataSource != null && brightnessOptionDataSource.Visible)
				{
					this._brightnessOptionDataSource.ExecuteCancel();
					return;
				}
				ExposureOptionVM exposureOptionDataSource = this._exposureOptionDataSource;
				if (exposureOptionDataSource != null && exposureOptionDataSource.Visible)
				{
					this._exposureOptionDataSource.ExecuteCancel();
					return;
				}
			}
			else if (this._gauntletLayer.Input.IsHotKeyReleased("Confirm"))
			{
				BrightnessOptionVM brightnessOptionDataSource2 = this._brightnessOptionDataSource;
				if (brightnessOptionDataSource2 != null && brightnessOptionDataSource2.Visible)
				{
					this._brightnessOptionDataSource.ExecuteConfirm();
					return;
				}
				ExposureOptionVM exposureOptionDataSource2 = this._exposureOptionDataSource;
				if (exposureOptionDataSource2 != null && exposureOptionDataSource2.Visible)
				{
					this._exposureOptionDataSource.ExecuteConfirm();
				}
			}
		}

		// Token: 0x06000041 RID: 65 RVA: 0x000034B5 File Offset: 0x000016B5
		protected override void OnActivate()
		{
			base.OnActivate();
			InitialMenuVM dataSource = this._dataSource;
			if (dataSource != null)
			{
				dataSource.RefreshMenuOptions();
			}
			this.SetGainNavigationAfterFrames(3);
		}

		// Token: 0x06000042 RID: 66 RVA: 0x000034D5 File Offset: 0x000016D5
		private void SetGainNavigationAfterFrames(int frameCount)
		{
			this._gauntletLayer._gauntletUIContext.EventManager.GainNavigationAfterFrames(frameCount, delegate
			{
				BrightnessOptionVM brightnessOptionDataSource = this._brightnessOptionDataSource;
				if (brightnessOptionDataSource == null || !brightnessOptionDataSource.Visible)
				{
					ExposureOptionVM exposureOptionDataSource = this._exposureOptionDataSource;
					return exposureOptionDataSource == null || !exposureOptionDataSource.Visible;
				}
				return false;
			});
		}

		// Token: 0x06000043 RID: 67 RVA: 0x000034F9 File Offset: 0x000016F9
		private void OnGameContentUpdated()
		{
			InitialMenuVM dataSource = this._dataSource;
			if (dataSource == null)
			{
				return;
			}
			dataSource.RefreshMenuOptions();
		}

		// Token: 0x06000044 RID: 68 RVA: 0x0000350B File Offset: 0x0000170B
		private void OnCloseBrightness(bool isConfirm)
		{
			this._gauntletBrightnessLayer.ReleaseMovie(this._brightnessOptionMovie);
			base.RemoveLayer(this._gauntletBrightnessLayer);
			this._brightnessOptionDataSource = null;
			NativeOptions.SaveConfig();
			this.OpenExposureControl();
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00003540 File Offset: 0x00001740
		private void OpenExposureControl()
		{
			this._exposureOptionDataSource = new ExposureOptionVM(new Action<bool>(this.OnCloseExposureControl))
			{
				Visible = true
			};
			this._gauntletExposureLayer = new GauntletLayer(2, "GauntletLayer", false);
			this._gauntletExposureLayer.InputRestrictions.SetInputRestrictions(true, 3);
			this._exposureOptionMovie = this._gauntletExposureLayer.LoadMovie("ExposureOption", this._exposureOptionDataSource);
			base.AddLayer(this._gauntletExposureLayer);
		}

		// Token: 0x06000046 RID: 70 RVA: 0x000035B7 File Offset: 0x000017B7
		private void OnCloseExposureControl(bool isConfirm)
		{
			this._gauntletExposureLayer.ReleaseMovie(this._exposureOptionMovie);
			base.RemoveLayer(this._gauntletExposureLayer);
			this._exposureOptionDataSource = null;
			NativeOptions.SaveConfig();
		}

		// Token: 0x06000047 RID: 71 RVA: 0x000035E4 File Offset: 0x000017E4
		protected override void OnFinalize()
		{
			base.OnFinalize();
			if (base._state != null)
			{
				base._state.OnGameContentUpdated -= new OnGameContentUpdatedDelegate(this.OnGameContentUpdated);
			}
			if (this._gauntletLayer != null)
			{
				base.RemoveLayer(this._gauntletLayer);
			}
			InitialMenuVM dataSource = this._dataSource;
			if (dataSource != null)
			{
				dataSource.OnFinalize();
			}
			this._dataSource = null;
		}

		// Token: 0x04000026 RID: 38
		private GauntletLayer _gauntletLayer;

		// Token: 0x04000027 RID: 39
		private GauntletLayer _gauntletBrightnessLayer;

		// Token: 0x04000028 RID: 40
		private GauntletLayer _gauntletExposureLayer;

		// Token: 0x04000029 RID: 41
		private InitialMenuVM _dataSource;

		// Token: 0x0400002A RID: 42
		private BrightnessOptionVM _brightnessOptionDataSource;

		// Token: 0x0400002B RID: 43
		private ExposureOptionVM _exposureOptionDataSource;

		// Token: 0x0400002C RID: 44
		private IGauntletMovie _brightnessOptionMovie;

		// Token: 0x0400002D RID: 45
		private IGauntletMovie _exposureOptionMovie;
	}
}
