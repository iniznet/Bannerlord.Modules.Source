using System;
using TaleWorlds.Library;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Friends
{
	public class MPLobbyPartyPlayerVM : MPLobbyPlayerBaseVM
	{
		public MPLobbyPartyPlayerVM(PlayerId id, Action<MPLobbyPartyPlayerVM> onActivatePlayerActions)
			: base(id, "", null, null)
		{
			this._onActivatePlayerActions = onActivatePlayerActions;
		}

		private void ExecuteActivatePlayerActions()
		{
			this._onActivatePlayerActions(this);
		}

		[DataSourceProperty]
		public bool IsWaitingConfirmation
		{
			get
			{
				return this._isWaitingConfirmation;
			}
			set
			{
				if (value != this._isWaitingConfirmation)
				{
					this._isWaitingConfirmation = value;
					base.OnPropertyChangedWithValue(value, "IsWaitingConfirmation");
				}
			}
		}

		[DataSourceProperty]
		public bool IsPartyLeader
		{
			get
			{
				return this._isPartyLeader;
			}
			set
			{
				if (value != this._isPartyLeader)
				{
					this._isPartyLeader = value;
					base.OnPropertyChangedWithValue(value, "IsPartyLeader");
				}
			}
		}

		private Action<MPLobbyPartyPlayerVM> _onActivatePlayerActions;

		private bool _isWaitingConfirmation;

		private bool _isPartyLeader;
	}
}
