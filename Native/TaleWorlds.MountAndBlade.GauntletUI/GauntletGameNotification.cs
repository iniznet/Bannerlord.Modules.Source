using System;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.MountAndBlade.GauntletUI.SceneNotification;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.GauntletUI
{
	// Token: 0x02000007 RID: 7
	public class GauntletGameNotification : GlobalLayer
	{
		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000023 RID: 35 RVA: 0x00002CAC File Offset: 0x00000EAC
		// (set) Token: 0x06000024 RID: 36 RVA: 0x00002CB3 File Offset: 0x00000EB3
		public static GauntletGameNotification Current { get; private set; }

		// Token: 0x06000025 RID: 37 RVA: 0x00002CBC File Offset: 0x00000EBC
		private GauntletGameNotification()
		{
			this._dataSource = new GameNotificationVM();
			this._dataSource.ReceiveNewNotification += this.OnReceiveNewNotification;
			this._layer = new GauntletLayer(4007, "GauntletLayer", false);
			this.LoadMovie(false);
			base.Layer = this._layer;
			this._layer.InputRestrictions.SetInputRestrictions(false, 3);
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00002D2C File Offset: 0x00000F2C
		private void OnReceiveNewNotification(GameNotificationItemVM notification)
		{
			if (!string.IsNullOrEmpty(notification.NotificationSoundId))
			{
				SoundEvent.PlaySound2D(notification.NotificationSoundId);
			}
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00002D47 File Offset: 0x00000F47
		public static void Initialize()
		{
			if (GauntletGameNotification.Current == null)
			{
				GauntletGameNotification.Current = new GauntletGameNotification();
				ScreenManager.AddGlobalLayer(GauntletGameNotification.Current, false);
			}
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002D65 File Offset: 0x00000F65
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

		// Token: 0x06000029 RID: 41 RVA: 0x00002D80 File Offset: 0x00000F80
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

		// Token: 0x0600002A RID: 42 RVA: 0x00002DE4 File Offset: 0x00000FE4
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

		// Token: 0x04000014 RID: 20
		private GameNotificationVM _dataSource;

		// Token: 0x04000015 RID: 21
		private GauntletLayer _layer;

		// Token: 0x04000016 RID: 22
		private IGauntletMovie _movie;

		// Token: 0x04000018 RID: 24
		private bool _isSuspended;
	}
}
