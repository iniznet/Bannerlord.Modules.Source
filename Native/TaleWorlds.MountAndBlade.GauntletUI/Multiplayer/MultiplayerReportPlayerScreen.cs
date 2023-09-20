using System;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer;
using TaleWorlds.PlayerServices;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.GauntletUI.Multiplayer
{
	// Token: 0x02000021 RID: 33
	public class MultiplayerReportPlayerScreen : GlobalLayer
	{
		// Token: 0x17000044 RID: 68
		// (get) Token: 0x06000180 RID: 384 RVA: 0x0000881E File Offset: 0x00006A1E
		// (set) Token: 0x06000181 RID: 385 RVA: 0x00008825 File Offset: 0x00006A25
		public static MultiplayerReportPlayerScreen Current { get; private set; }

		// Token: 0x06000182 RID: 386 RVA: 0x00008830 File Offset: 0x00006A30
		public MultiplayerReportPlayerScreen()
		{
			this._dataSource = new MultiplayerReportPlayerVM(new Action<string, PlayerId, string, PlayerReportType, string>(this.OnReportDone), new Action(this.OnClose));
			this._dataSource.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
			this._dataSource.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
			GauntletLayer gauntletLayer = new GauntletLayer(350, "GauntletLayer", false);
			this._movie = gauntletLayer.LoadMovie("MultiplayerReportPlayer", this._dataSource);
			gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			base.Layer = gauntletLayer;
		}

		// Token: 0x06000183 RID: 387 RVA: 0x000088E8 File Offset: 0x00006AE8
		protected override void OnTick(float dt)
		{
			if (this._isActive)
			{
				if (base.Layer.Input.IsHotKeyReleased("Confirm"))
				{
					this._dataSource.ExecuteDone();
					return;
				}
				if (base.Layer.Input.IsHotKeyReleased("Exit"))
				{
					this._dataSource.ExecuteCancel();
				}
			}
		}

		// Token: 0x06000184 RID: 388 RVA: 0x00008944 File Offset: 0x00006B44
		private void OnClose()
		{
			if (!this._isActive)
			{
				return;
			}
			this._isActive = false;
			base.Layer.InputRestrictions.ResetInputRestrictions();
			ScreenManager.SetSuspendLayer(base.Layer, true);
			base.Layer.IsFocusLayer = false;
			ScreenManager.TryLoseFocus(base.Layer);
		}

		// Token: 0x06000185 RID: 389 RVA: 0x00008994 File Offset: 0x00006B94
		public static void OnInitialize()
		{
			if (MultiplayerReportPlayerScreen.Current == null)
			{
				MultiplayerReportPlayerScreen.Current = new MultiplayerReportPlayerScreen();
				ScreenManager.AddGlobalLayer(MultiplayerReportPlayerScreen.Current, false);
				MultiplayerReportPlayerManager.ReportHandlers += MultiplayerReportPlayerScreen.Current.OnReportRequest;
				MultiplayerReportPlayerScreen.Current._isActive = false;
				ScreenManager.SetSuspendLayer(MultiplayerReportPlayerScreen.Current.Layer, true);
			}
		}

		// Token: 0x06000186 RID: 390 RVA: 0x000089F0 File Offset: 0x00006BF0
		public static void OnFinalize()
		{
			if (MultiplayerReportPlayerScreen.Current != null)
			{
				ScreenManager.RemoveGlobalLayer(MultiplayerReportPlayerScreen.Current);
				MultiplayerReportPlayerScreen.Current._movie.Release();
				MultiplayerReportPlayerManager.ReportHandlers -= MultiplayerReportPlayerScreen.Current.OnReportRequest;
				MultiplayerReportPlayerScreen.Current._dataSource.OnFinalize();
				MultiplayerReportPlayerScreen.Current._dataSource = null;
				MultiplayerReportPlayerScreen.Current = null;
			}
		}

		// Token: 0x06000187 RID: 391 RVA: 0x00008A54 File Offset: 0x00006C54
		private void OnReportRequest(string gameId, PlayerId playerId, string playerName, bool isRequestedFromMission)
		{
			if (this._isActive)
			{
				return;
			}
			this._isActive = true;
			ScreenManager.SetSuspendLayer(base.Layer, false);
			base.Layer.IsFocusLayer = true;
			ScreenManager.TrySetFocus(base.Layer);
			base.Layer.InputRestrictions.SetInputRestrictions(true, 7);
			this._dataSource.OpenNewReportWithGamePlayerId(gameId, playerId, playerName, isRequestedFromMission);
		}

		// Token: 0x06000188 RID: 392 RVA: 0x00008AB6 File Offset: 0x00006CB6
		private void OnReportDone(string gameId, PlayerId playerId, string playerName, PlayerReportType reportReason, string reasonText)
		{
			if (!this._isActive)
			{
				return;
			}
			this.OnClose();
			NetworkMain.GameClient.ReportPlayer(gameId, playerId, playerName, reportReason, reasonText);
			Game.Current.GetGameHandler<ChatBox>().SetPlayerMuted(playerId, true);
			MultiplayerReportPlayerManager.OnPlayerReported(playerId);
		}

		// Token: 0x040000BD RID: 189
		private MultiplayerReportPlayerVM _dataSource;

		// Token: 0x040000BE RID: 190
		private IGauntletMovie _movie;

		// Token: 0x040000BF RID: 191
		private bool _isActive;
	}
}
