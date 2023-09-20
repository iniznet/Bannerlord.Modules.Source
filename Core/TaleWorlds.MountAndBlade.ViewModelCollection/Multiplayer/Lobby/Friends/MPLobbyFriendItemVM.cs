using System;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Friends
{
	public class MPLobbyFriendItemVM : MPLobbyPlayerBaseVM
	{
		public MPLobbyFriendItemVM(PlayerId ID, Action<MPLobbyPlayerBaseVM> onActivatePlayerActions, Action<PlayerId> onInviteToClan = null, Action<PlayerId> onFriendRequestAnswered = null)
			: base(ID, string.Empty, onInviteToClan, onFriendRequestAnswered)
		{
			this._onActivatePlayerActions = onActivatePlayerActions;
		}

		private void ExecuteActivatePlayerActions()
		{
			this._onActivatePlayerActions(this);
		}

		private Action<MPLobbyPlayerBaseVM> _onActivatePlayerActions;
	}
}
