using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.PlatformService;

namespace TaleWorlds.MountAndBlade.Multiplayer
{
	public class MultiplayerSubModule : MBSubModuleBase
	{
		protected internal override void OnSubModuleLoad()
		{
			base.OnSubModuleLoad();
			Module.CurrentModule.AddMultiplayerGameMode(new MissionBasedMultiplayerGameMode("FreeForAll"));
			Module.CurrentModule.AddMultiplayerGameMode(new MissionBasedMultiplayerGameMode("TeamDeathmatch"));
			Module.CurrentModule.AddMultiplayerGameMode(new MissionBasedMultiplayerGameMode("Duel"));
			Module.CurrentModule.AddMultiplayerGameMode(new MissionBasedMultiplayerGameMode("Siege"));
			Module.CurrentModule.AddMultiplayerGameMode(new MissionBasedMultiplayerGameMode("Captain"));
			Module.CurrentModule.AddMultiplayerGameMode(new MissionBasedMultiplayerGameMode("Skirmish"));
			Module.CurrentModule.AddMultiplayerGameMode(new MissionBasedMultiplayerGameMode("Battle"));
			TextObject coreContentDisabledReason = new TextObject("{=V8BXjyYq}Disabled during installation.", null);
			if (Module.CurrentModule.StartupInfo.StartupType != 3)
			{
				Module.CurrentModule.AddInitialStateOption(new InitialStateOption("Multiplayer", new TextObject("{=YDYnuBmC}Multiplayer", null), 9997, new Action(this.StartMultiplayer), () => new ValueTuple<bool, TextObject>(Module.CurrentModule.IsOnlyCoreContentEnabled, coreContentDisabledReason), null));
			}
		}

		public override void OnGameLoaded(Game game, object initializerObject)
		{
			base.OnGameLoaded(game, initializerObject);
			MultiplayerMain.Initialize(new GameNetworkHandler());
		}

		protected internal override void OnApplicationTick(float dt)
		{
			base.OnApplicationTick(dt);
		}

		protected internal override void OnBeforeInitialModuleScreenSetAsRoot()
		{
			base.OnBeforeInitialModuleScreenSetAsRoot();
			if (GameNetwork.IsDedicatedServer)
			{
				MBGameManager.StartNewGame(new MultiplayerGameManager());
			}
		}

		public override void OnInitialState()
		{
			base.OnInitialState();
			if (Utilities.CommandLineArgumentExists("+connect_lobby"))
			{
				MBGameManager.StartNewGame(new MultiplayerGameManager());
				return;
			}
			if (!Module.CurrentModule.IsOnlyCoreContentEnabled && Module.CurrentModule.MultiplayerRequested)
			{
				MBGameManager.StartNewGame(new MultiplayerGameManager());
			}
		}

		private async void StartMultiplayer()
		{
			if (!this._isConnectingToMultiplayer)
			{
				this._isConnectingToMultiplayer = true;
				bool flag = NetworkMain.GameClient != null && await NetworkMain.GameClient.CheckConnection();
				bool isConnected = flag;
				PlatformServices.Instance.CheckPrivilege(0, true, delegate(bool result)
				{
					if (!isConnected || !result)
					{
						string text = new TextObject("{=ksq1IBh3}No connection", null).ToString();
						string text2 = new TextObject("{=5VIbo2Cb}No connection could be established to the lobby server. Check your internet connection and try again.", null).ToString();
						InformationManager.ShowInquiry(new InquiryData(text, text2, false, true, "", new TextObject("{=dismissnotification}Dismiss", null).ToString(), null, delegate
						{
							InformationManager.HideInquiry();
						}, "", 0f, null, null, null), false, false);
						return;
					}
					MBGameManager.StartNewGame(new MultiplayerGameManager());
				});
				this._isConnectingToMultiplayer = false;
			}
		}

		protected internal override void OnNetworkTick(float dt)
		{
			base.OnNetworkTick(dt);
			MultiplayerMain.Tick(dt);
			InternetAvailabilityChecker.Tick(dt);
		}

		private bool _isConnectingToMultiplayer;
	}
}
