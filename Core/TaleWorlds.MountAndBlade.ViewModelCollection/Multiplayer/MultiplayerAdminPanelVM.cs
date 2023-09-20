using System;
using JetBrains.Annotations;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.CustomGame;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.HostGame;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer
{
	// Token: 0x02000041 RID: 65
	public class MultiplayerAdminPanelVM : ViewModel
	{
		// Token: 0x06000576 RID: 1398 RVA: 0x00017640 File Offset: 0x00015840
		public MultiplayerAdminPanelVM(Action<bool> onEscapeMenuToggled, MultiplayerAdminComponent multiplayerAdminComponent)
		{
			this._onEscapeMenuToggled = onEscapeMenuToggled;
			this._multiplayerAdminComponent = multiplayerAdminComponent;
			this.HostGameOptions = new MPHostGameOptionsVM(true, MPCustomGameVM.CustomGameMode.CustomServer);
			this.RefreshValues();
		}

		// Token: 0x06000577 RID: 1399 RVA: 0x00017669 File Offset: 0x00015869
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.StartText = new TextObject("{=wkIVxzV6}Restart", null).ToString();
			this.ApplyText = new TextObject("{=BAaS5Dkc}Apply", null).ToString();
			this.HostGameOptions.RefreshValues();
		}

		// Token: 0x06000578 RID: 1400 RVA: 0x000176A8 File Offset: 0x000158A8
		[UsedImplicitly]
		private void ExecuteApply()
		{
			this._onEscapeMenuToggled(false);
			this._multiplayerAdminComponent.OnApplySettings();
		}

		// Token: 0x06000579 RID: 1401 RVA: 0x000176C1 File Offset: 0x000158C1
		[UsedImplicitly]
		private void ExecuteStart()
		{
			this._onEscapeMenuToggled(false);
			this._multiplayerAdminComponent.OnApplySettings();
			Mission.Current.GetMissionBehavior<MissionLobbyComponent>().SetStateEndingAsServer();
		}

		// Token: 0x0600057A RID: 1402 RVA: 0x000176E9 File Offset: 0x000158E9
		[UsedImplicitly]
		private void ExecuteExit()
		{
			this._onEscapeMenuToggled(false);
		}

		// Token: 0x1700019C RID: 412
		// (get) Token: 0x0600057B RID: 1403 RVA: 0x000176F7 File Offset: 0x000158F7
		// (set) Token: 0x0600057C RID: 1404 RVA: 0x000176FF File Offset: 0x000158FF
		[DataSourceProperty]
		public string ApplyText
		{
			get
			{
				return this._applyText;
			}
			set
			{
				if (value != this._applyText)
				{
					this._applyText = value;
					base.OnPropertyChangedWithValue<string>(value, "ApplyText");
				}
			}
		}

		// Token: 0x1700019D RID: 413
		// (get) Token: 0x0600057D RID: 1405 RVA: 0x00017722 File Offset: 0x00015922
		// (set) Token: 0x0600057E RID: 1406 RVA: 0x0001772A File Offset: 0x0001592A
		[DataSourceProperty]
		public string StartText
		{
			get
			{
				return this._startText;
			}
			set
			{
				if (value != this._startText)
				{
					this._startText = value;
					base.OnPropertyChangedWithValue<string>(value, "StartText");
				}
			}
		}

		// Token: 0x1700019E RID: 414
		// (get) Token: 0x0600057F RID: 1407 RVA: 0x0001774D File Offset: 0x0001594D
		// (set) Token: 0x06000580 RID: 1408 RVA: 0x00017755 File Offset: 0x00015955
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

		// Token: 0x040002C9 RID: 713
		private readonly Action<bool> _onEscapeMenuToggled;

		// Token: 0x040002CA RID: 714
		private readonly MultiplayerAdminComponent _multiplayerAdminComponent;

		// Token: 0x040002CB RID: 715
		private MPHostGameOptionsVM _hostGameOptions;

		// Token: 0x040002CC RID: 716
		private string _applyText;

		// Token: 0x040002CD RID: 717
		private string _startText;
	}
}
