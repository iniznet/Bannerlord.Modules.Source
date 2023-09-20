using System;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.PlayerServices;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.Multiplayer.GauntletUI
{
	public class MultiplayerReportPlayerScreen : GlobalLayer
	{
		public static MultiplayerReportPlayerScreen Current { get; private set; }

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

		protected override void OnTick(float dt)
		{
			if (this._isActive)
			{
				if (base.Layer.Input.IsHotKeyReleased("Confirm"))
				{
					UISoundsHelper.PlayUISound("event:/ui/default");
					this._dataSource.ExecuteDone();
					return;
				}
				if (base.Layer.Input.IsHotKeyReleased("Exit"))
				{
					UISoundsHelper.PlayUISound("event:/ui/default");
					this._dataSource.ExecuteCancel();
				}
			}
		}

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

		private MultiplayerReportPlayerVM _dataSource;

		private IGauntletMovie _movie;

		private bool _isActive;
	}
}
