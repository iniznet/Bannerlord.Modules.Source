using System;
using TaleWorlds.Library;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Friends
{
	// Token: 0x02000085 RID: 133
	public class MPLobbyPartyPlayerVM : MPLobbyPlayerBaseVM
	{
		// Token: 0x06000BE9 RID: 3049 RVA: 0x0002A4D9 File Offset: 0x000286D9
		public MPLobbyPartyPlayerVM(PlayerId id, Action<MPLobbyPartyPlayerVM> onActivatePlayerActions)
			: base(id, "", null, null)
		{
			this._onActivatePlayerActions = onActivatePlayerActions;
		}

		// Token: 0x06000BEA RID: 3050 RVA: 0x0002A4F0 File Offset: 0x000286F0
		private void ExecuteActivatePlayerActions()
		{
			this._onActivatePlayerActions(this);
		}

		// Token: 0x170003AC RID: 940
		// (get) Token: 0x06000BEB RID: 3051 RVA: 0x0002A4FE File Offset: 0x000286FE
		// (set) Token: 0x06000BEC RID: 3052 RVA: 0x0002A506 File Offset: 0x00028706
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

		// Token: 0x170003AD RID: 941
		// (get) Token: 0x06000BED RID: 3053 RVA: 0x0002A524 File Offset: 0x00028724
		// (set) Token: 0x06000BEE RID: 3054 RVA: 0x0002A52C File Offset: 0x0002872C
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

		// Token: 0x040005A8 RID: 1448
		private Action<MPLobbyPartyPlayerVM> _onActivatePlayerActions;

		// Token: 0x040005A9 RID: 1449
		private bool _isWaitingConfirmation;

		// Token: 0x040005AA RID: 1450
		private bool _isPartyLeader;
	}
}
