using System;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.Multiplayer.GauntletUI
{
	public class MultiplayerAdminInformationScreen : GlobalLayer
	{
		public static MultiplayerAdminInformationScreen Current { get; private set; }

		public MultiplayerAdminInformationScreen()
		{
			this._dataSource = new MultiplayerAdminInformationVM();
			GauntletLayer gauntletLayer = new GauntletLayer(300, "GauntletLayer", false);
			this._movie = gauntletLayer.LoadMovie("MultiplayerAdminInformation", this._dataSource);
			base.Layer = gauntletLayer;
			InformationManager.OnAddSystemNotification += this.OnSystemNotificationReceived;
		}

		public static void OnInitialize()
		{
			if (MultiplayerAdminInformationScreen.Current == null)
			{
				MultiplayerAdminInformationScreen.Current = new MultiplayerAdminInformationScreen();
				ScreenManager.AddGlobalLayer(MultiplayerAdminInformationScreen.Current, false);
			}
		}

		public void OnFinalize()
		{
			InformationManager.OnAddSystemNotification -= this.OnSystemNotificationReceived;
		}

		private void OnSystemNotificationReceived(string obj)
		{
			this._dataSource.OnNewMessageReceived(obj);
		}

		public static void OnRemove()
		{
			if (MultiplayerAdminInformationScreen.Current != null)
			{
				MultiplayerAdminInformationScreen.Current.OnFinalize();
				ScreenManager.RemoveGlobalLayer(MultiplayerAdminInformationScreen.Current);
				MultiplayerAdminInformationScreen.Current._movie.Release();
				MultiplayerAdminInformationScreen.Current._dataSource = null;
				MultiplayerAdminInformationScreen.Current = null;
			}
		}

		private MultiplayerAdminInformationVM _dataSource;

		private IGauntletMovie _movie;
	}
}
