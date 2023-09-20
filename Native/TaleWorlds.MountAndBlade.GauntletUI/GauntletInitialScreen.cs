using System;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Options;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions;
using TaleWorlds.MountAndBlade.ViewModelCollection.InitialMenu;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.GauntletUI
{
	[GameStateScreen(typeof(InitialState))]
	public class GauntletInitialScreen : MBInitialScreenBase
	{
		public GauntletInitialScreen(InitialState initialState)
			: base(initialState)
		{
		}

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
			if (NativeOptions.GetConfig(69) < 2f)
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

		protected override void OnInitialScreenTick(float dt)
		{
			base.OnInitialScreenTick(dt);
			if (this._gauntletLayer.Input.IsHotKeyReleased("Exit"))
			{
				BrightnessOptionVM brightnessOptionDataSource = this._brightnessOptionDataSource;
				if (brightnessOptionDataSource != null && brightnessOptionDataSource.Visible)
				{
					UISoundsHelper.PlayUISound("event:/ui/default");
					this._brightnessOptionDataSource.ExecuteCancel();
					return;
				}
				ExposureOptionVM exposureOptionDataSource = this._exposureOptionDataSource;
				if (exposureOptionDataSource != null && exposureOptionDataSource.Visible)
				{
					UISoundsHelper.PlayUISound("event:/ui/default");
					this._exposureOptionDataSource.ExecuteCancel();
					return;
				}
			}
			else if (this._gauntletLayer.Input.IsHotKeyReleased("Confirm"))
			{
				BrightnessOptionVM brightnessOptionDataSource2 = this._brightnessOptionDataSource;
				if (brightnessOptionDataSource2 != null && brightnessOptionDataSource2.Visible)
				{
					UISoundsHelper.PlayUISound("event:/ui/default");
					this._brightnessOptionDataSource.ExecuteConfirm();
					return;
				}
				ExposureOptionVM exposureOptionDataSource2 = this._exposureOptionDataSource;
				if (exposureOptionDataSource2 != null && exposureOptionDataSource2.Visible)
				{
					UISoundsHelper.PlayUISound("event:/ui/default");
					this._exposureOptionDataSource.ExecuteConfirm();
				}
			}
		}

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

		private void OnGameContentUpdated()
		{
			InitialMenuVM dataSource = this._dataSource;
			if (dataSource == null)
			{
				return;
			}
			dataSource.RefreshMenuOptions();
		}

		private void OnCloseBrightness(bool isConfirm)
		{
			this._gauntletBrightnessLayer.ReleaseMovie(this._brightnessOptionMovie);
			base.RemoveLayer(this._gauntletBrightnessLayer);
			this._brightnessOptionDataSource = null;
			this._gauntletBrightnessLayer = null;
			NativeOptions.SaveConfig();
			this.OpenExposureControl();
		}

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

		private void OnCloseExposureControl(bool isConfirm)
		{
			this._gauntletExposureLayer.ReleaseMovie(this._exposureOptionMovie);
			base.RemoveLayer(this._gauntletExposureLayer);
			this._exposureOptionDataSource = null;
			this._gauntletExposureLayer = null;
			NativeOptions.SaveConfig();
		}

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
			this._gauntletLayer = null;
		}

		private GauntletLayer _gauntletLayer;

		private GauntletLayer _gauntletBrightnessLayer;

		private GauntletLayer _gauntletExposureLayer;

		private InitialMenuVM _dataSource;

		private BrightnessOptionVM _brightnessOptionDataSource;

		private ExposureOptionVM _exposureOptionDataSource;

		private IGauntletMovie _brightnessOptionMovie;

		private IGauntletMovie _exposureOptionMovie;
	}
}
