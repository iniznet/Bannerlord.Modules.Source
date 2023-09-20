using System;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.GauntletUI.Multiplayer
{
	// Token: 0x0200001E RID: 30
	public class MultiplayerAdminInformationScreen : GlobalLayer
	{
		// Token: 0x1700003F RID: 63
		// (get) Token: 0x06000118 RID: 280 RVA: 0x00006EE1 File Offset: 0x000050E1
		// (set) Token: 0x06000119 RID: 281 RVA: 0x00006EE8 File Offset: 0x000050E8
		public static MultiplayerAdminInformationScreen Current { get; private set; }

		// Token: 0x0600011A RID: 282 RVA: 0x00006EF0 File Offset: 0x000050F0
		public MultiplayerAdminInformationScreen()
		{
			this._dataSource = new MultiplayerAdminInformationVM();
			GauntletLayer gauntletLayer = new GauntletLayer(300, "GauntletLayer", false);
			this._movie = gauntletLayer.LoadMovie("MultiplayerAdminInformation", this._dataSource);
			base.Layer = gauntletLayer;
			InformationManager.OnAddSystemNotification += this.OnSystemNotificationReceived;
		}

		// Token: 0x0600011B RID: 283 RVA: 0x00006F4E File Offset: 0x0000514E
		public static void OnInitialize()
		{
			if (MultiplayerAdminInformationScreen.Current == null)
			{
				MultiplayerAdminInformationScreen.Current = new MultiplayerAdminInformationScreen();
				ScreenManager.AddGlobalLayer(MultiplayerAdminInformationScreen.Current, false);
			}
		}

		// Token: 0x0600011C RID: 284 RVA: 0x00006F6C File Offset: 0x0000516C
		public void OnFinalize()
		{
			InformationManager.OnAddSystemNotification -= this.OnSystemNotificationReceived;
		}

		// Token: 0x0600011D RID: 285 RVA: 0x00006F7F File Offset: 0x0000517F
		private void OnSystemNotificationReceived(string obj)
		{
			this._dataSource.OnNewMessageReceived(obj);
		}

		// Token: 0x0600011E RID: 286 RVA: 0x00006F8D File Offset: 0x0000518D
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

		// Token: 0x040000A2 RID: 162
		private MultiplayerAdminInformationVM _dataSource;

		// Token: 0x040000A3 RID: 163
		private IGauntletMovie _movie;
	}
}
