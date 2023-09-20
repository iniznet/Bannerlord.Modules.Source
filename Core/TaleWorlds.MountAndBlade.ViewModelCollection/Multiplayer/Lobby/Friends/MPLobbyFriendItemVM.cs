using System;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Friends
{
	// Token: 0x02000081 RID: 129
	public class MPLobbyFriendItemVM : MPLobbyPlayerBaseVM
	{
		// Token: 0x06000B50 RID: 2896 RVA: 0x00027EA6 File Offset: 0x000260A6
		public MPLobbyFriendItemVM(PlayerId ID, Action<MPLobbyPlayerBaseVM> onActivatePlayerActions, Action<PlayerId> onInviteToClan = null, Action<PlayerId> onFriendRequestAnswered = null)
			: base(ID, string.Empty, onInviteToClan, onFriendRequestAnswered)
		{
			this._onActivatePlayerActions = onActivatePlayerActions;
		}

		// Token: 0x06000B51 RID: 2897 RVA: 0x00027EBE File Offset: 0x000260BE
		private void ExecuteActivatePlayerActions()
		{
			this._onActivatePlayerActions(this);
		}

		// Token: 0x0400056E RID: 1390
		private Action<MPLobbyPlayerBaseVM> _onActivatePlayerActions;
	}
}
