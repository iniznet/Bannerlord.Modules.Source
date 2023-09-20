using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.CustomGame;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.HostGame
{
	// Token: 0x02000076 RID: 118
	public class MPHostGameVM : ViewModel
	{
		// Token: 0x06000ABF RID: 2751 RVA: 0x00026898 File Offset: 0x00024A98
		public MPHostGameVM(LobbyState lobbyState, MPCustomGameVM.CustomGameMode customGameMode)
		{
			this._lobbyState = lobbyState;
			this._customGameMode = customGameMode;
			this.HostGameOptions = new MPHostGameOptionsVM(false, this._customGameMode);
			this.RefreshValues();
		}

		// Token: 0x06000AC0 RID: 2752 RVA: 0x000268C6 File Offset: 0x00024AC6
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.CreateText = new TextObject("{=aRzlp5XH}CREATE", null).ToString();
			this.HostGameOptions.RefreshValues();
		}

		// Token: 0x06000AC1 RID: 2753 RVA: 0x000268EF File Offset: 0x00024AEF
		public void ExecuteStart()
		{
			if (this._customGameMode == MPCustomGameVM.CustomGameMode.CustomServer)
			{
				this._lobbyState.HostGame();
				return;
			}
			if (this._customGameMode == MPCustomGameVM.CustomGameMode.PremadeGame)
			{
				this._lobbyState.CreatePremadeGame();
			}
		}

		// Token: 0x17000356 RID: 854
		// (get) Token: 0x06000AC2 RID: 2754 RVA: 0x00026919 File Offset: 0x00024B19
		// (set) Token: 0x06000AC3 RID: 2755 RVA: 0x00026921 File Offset: 0x00024B21
		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (value != this._isEnabled)
				{
					this._isEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEnabled");
				}
			}
		}

		// Token: 0x17000357 RID: 855
		// (get) Token: 0x06000AC4 RID: 2756 RVA: 0x0002693F File Offset: 0x00024B3F
		// (set) Token: 0x06000AC5 RID: 2757 RVA: 0x00026947 File Offset: 0x00024B47
		[DataSourceProperty]
		public MPHostGameOptionsVM HostGameOptions
		{
			get
			{
				return this._hostGameOptions;
			}
			set
			{
				if (value != this._hostGameOptions)
				{
					this._hostGameOptions = value;
					base.OnPropertyChangedWithValue<MPHostGameOptionsVM>(value, "HostGameOptions");
				}
			}
		}

		// Token: 0x17000358 RID: 856
		// (get) Token: 0x06000AC6 RID: 2758 RVA: 0x00026965 File Offset: 0x00024B65
		// (set) Token: 0x06000AC7 RID: 2759 RVA: 0x0002696D File Offset: 0x00024B6D
		[DataSourceProperty]
		public string CreateText
		{
			get
			{
				return this._createText;
			}
			set
			{
				if (value != this._createText)
				{
					this._createText = value;
					base.OnPropertyChangedWithValue<string>(value, "CreateText");
				}
			}
		}

		// Token: 0x04000536 RID: 1334
		private LobbyState _lobbyState;

		// Token: 0x04000537 RID: 1335
		private MPCustomGameVM.CustomGameMode _customGameMode;

		// Token: 0x04000538 RID: 1336
		private bool _isEnabled;

		// Token: 0x04000539 RID: 1337
		private MPHostGameOptionsVM _hostGameOptions;

		// Token: 0x0400053A RID: 1338
		private string _createText;
	}
}
