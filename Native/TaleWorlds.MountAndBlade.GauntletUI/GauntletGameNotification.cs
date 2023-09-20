using System;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.MountAndBlade.GauntletUI.SceneNotification;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.GauntletUI
{
	public class GauntletGameNotification : GlobalLayer
	{
		public static GauntletGameNotification Current { get; private set; }

		private GauntletGameNotification()
		{
			this._dataSource = new GameNotificationVM();
			this._dataSource.ReceiveNewNotification += this.OnReceiveNewNotification;
			this._layer = new GauntletLayer(4007, "GauntletLayer", false);
			this.LoadMovie(false);
			base.Layer = this._layer;
			this._layer.InputRestrictions.SetInputRestrictions(false, 3);
		}

		private void OnReceiveNewNotification(GameNotificationItemVM notification)
		{
			if (!string.IsNullOrEmpty(notification.NotificationSoundId))
			{
				SoundEvent.PlaySound2D(notification.NotificationSoundId);
			}
		}

		public static void Initialize()
		{
			if (GauntletGameNotification.Current == null)
			{
				GauntletGameNotification.Current = new GauntletGameNotification();
				ScreenManager.AddGlobalLayer(GauntletGameNotification.Current, false);
			}
		}

		public static void OnFinalize()
		{
			GauntletGameNotification gauntletGameNotification = GauntletGameNotification.Current;
			if (gauntletGameNotification == null)
			{
				return;
			}
			GameNotificationVM dataSource = gauntletGameNotification._dataSource;
			if (dataSource == null)
			{
				return;
			}
			dataSource.ClearNotifications();
		}

		public void LoadMovie(bool forMultiplayer)
		{
			if (this._movie != null)
			{
				this._layer.ReleaseMovie(this._movie);
			}
			if (forMultiplayer)
			{
				this._movie = this._layer.LoadMovie("MultiplayerGameNotificationUI", this._dataSource);
				return;
			}
			this._movie = this._layer.LoadMovie("GameNotificationUI", this._dataSource);
		}

		protected override void OnTick(float dt)
		{
			base.OnTick(dt);
			bool isLoadingWindowActive = LoadingWindow.IsLoadingWindowActive;
			bool isActive = GauntletSceneNotification.Current.IsActive;
			if (isActive != this._isSuspended)
			{
				ScreenManager.SetSuspendLayer(GauntletGameNotification.Current._layer, isActive);
				this._isSuspended = isActive;
			}
			if (isLoadingWindowActive)
			{
				dt = 0f;
			}
			if (!this._isSuspended)
			{
				this._dataSource.Tick(dt);
			}
		}

		private GameNotificationVM _dataSource;

		private GauntletLayer _layer;

		private IGauntletMovie _movie;

		private bool _isSuspended;
	}
}
