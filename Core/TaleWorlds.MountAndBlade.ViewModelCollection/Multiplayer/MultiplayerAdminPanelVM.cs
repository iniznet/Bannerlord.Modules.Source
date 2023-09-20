using System;
using JetBrains.Annotations;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.CustomGame;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.HostGame;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer
{
	public class MultiplayerAdminPanelVM : ViewModel
	{
		public MultiplayerAdminPanelVM(Action<bool> onEscapeMenuToggled, MultiplayerAdminComponent multiplayerAdminComponent)
		{
			this._onEscapeMenuToggled = onEscapeMenuToggled;
			this._multiplayerAdminComponent = multiplayerAdminComponent;
			this.HostGameOptions = new MPHostGameOptionsVM(true, MPCustomGameVM.CustomGameMode.CustomServer);
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.StartText = new TextObject("{=wkIVxzV6}Restart", null).ToString();
			this.ApplyText = new TextObject("{=BAaS5Dkc}Apply", null).ToString();
			this.HostGameOptions.RefreshValues();
		}

		[UsedImplicitly]
		private void ExecuteApply()
		{
			this._onEscapeMenuToggled(false);
			this._multiplayerAdminComponent.OnApplySettings();
		}

		[UsedImplicitly]
		private void ExecuteStart()
		{
			this._onEscapeMenuToggled(false);
			this._multiplayerAdminComponent.OnApplySettings();
			Mission.Current.GetMissionBehavior<MissionLobbyComponent>().SetStateEndingAsServer();
		}

		[UsedImplicitly]
		private void ExecuteExit()
		{
			this._onEscapeMenuToggled(false);
		}

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

		private readonly Action<bool> _onEscapeMenuToggled;

		private readonly MultiplayerAdminComponent _multiplayerAdminComponent;

		private MPHostGameOptionsVM _hostGameOptions;

		private string _applyText;

		private string _startText;
	}
}
