using System;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer
{
	// Token: 0x02000047 RID: 71
	public class MultiplayerPollProgressVM : ViewModel
	{
		// Token: 0x060005D8 RID: 1496 RVA: 0x00018B6A File Offset: 0x00016D6A
		public MultiplayerPollProgressVM()
		{
			this.Keys = new MBBindingList<InputKeyItemVM>();
		}

		// Token: 0x060005D9 RID: 1497 RVA: 0x00018B80 File Offset: 0x00016D80
		public void OnKickPollOpened(MissionPeer initiatorPeer, MissionPeer targetPeer, bool isBanRequested)
		{
			this.TargetPlayer = new MPPlayerVM(targetPeer);
			this.PollInitiatorName = initiatorPeer.DisplayedName;
			GameTexts.SetVariable("ACTION", isBanRequested ? MultiplayerPollProgressVM._banText : MultiplayerPollProgressVM._kickText);
			this.PollDescription = new TextObject("{=qyuhC21P}wants to {ACTION}", null).ToString();
			this.VotesAccepted = 0;
			this.VotesRejected = 0;
			this.AreKeysEnabled = NetworkMain.GameClient.PlayerID != targetPeer.Peer.Id;
			this.HasOngoingPoll = true;
		}

		// Token: 0x060005DA RID: 1498 RVA: 0x00018C09 File Offset: 0x00016E09
		public void OnPollUpdated(int votesAccepted, int votesRejected)
		{
			this.VotesAccepted = votesAccepted;
			this.VotesRejected = votesRejected;
		}

		// Token: 0x060005DB RID: 1499 RVA: 0x00018C19 File Offset: 0x00016E19
		public void OnPollClosed()
		{
			this.HasOngoingPoll = false;
		}

		// Token: 0x060005DC RID: 1500 RVA: 0x00018C22 File Offset: 0x00016E22
		public void OnPollOptionPicked()
		{
			this.AreKeysEnabled = false;
		}

		// Token: 0x060005DD RID: 1501 RVA: 0x00018C2B File Offset: 0x00016E2B
		public void AddKey(GameKey key)
		{
			this.Keys.Add(InputKeyItemVM.CreateFromGameKey(key, false));
		}

		// Token: 0x170001B7 RID: 439
		// (get) Token: 0x060005DE RID: 1502 RVA: 0x00018C3F File Offset: 0x00016E3F
		// (set) Token: 0x060005DF RID: 1503 RVA: 0x00018C47 File Offset: 0x00016E47
		[DataSourceProperty]
		public bool HasOngoingPoll
		{
			get
			{
				return this._hasOngoingPoll;
			}
			set
			{
				if (value != this._hasOngoingPoll)
				{
					this._hasOngoingPoll = value;
					base.OnPropertyChangedWithValue(value, "HasOngoingPoll");
				}
			}
		}

		// Token: 0x170001B8 RID: 440
		// (get) Token: 0x060005E0 RID: 1504 RVA: 0x00018C65 File Offset: 0x00016E65
		// (set) Token: 0x060005E1 RID: 1505 RVA: 0x00018C6D File Offset: 0x00016E6D
		[DataSourceProperty]
		public bool AreKeysEnabled
		{
			get
			{
				return this._areKeysEnabled;
			}
			set
			{
				if (value != this._areKeysEnabled)
				{
					this._areKeysEnabled = value;
					base.OnPropertyChangedWithValue(value, "AreKeysEnabled");
				}
			}
		}

		// Token: 0x170001B9 RID: 441
		// (get) Token: 0x060005E2 RID: 1506 RVA: 0x00018C8B File Offset: 0x00016E8B
		// (set) Token: 0x060005E3 RID: 1507 RVA: 0x00018C93 File Offset: 0x00016E93
		[DataSourceProperty]
		public int VotesAccepted
		{
			get
			{
				return this._votesAccepted;
			}
			set
			{
				if (this._votesAccepted != value)
				{
					this._votesAccepted = value;
					base.OnPropertyChangedWithValue(value, "VotesAccepted");
				}
			}
		}

		// Token: 0x170001BA RID: 442
		// (get) Token: 0x060005E4 RID: 1508 RVA: 0x00018CB1 File Offset: 0x00016EB1
		// (set) Token: 0x060005E5 RID: 1509 RVA: 0x00018CB9 File Offset: 0x00016EB9
		[DataSourceProperty]
		public int VotesRejected
		{
			get
			{
				return this._votesRejected;
			}
			set
			{
				if (this._votesRejected != value)
				{
					this._votesRejected = value;
					base.OnPropertyChangedWithValue(value, "VotesRejected");
				}
			}
		}

		// Token: 0x170001BB RID: 443
		// (get) Token: 0x060005E6 RID: 1510 RVA: 0x00018CD7 File Offset: 0x00016ED7
		// (set) Token: 0x060005E7 RID: 1511 RVA: 0x00018CDF File Offset: 0x00016EDF
		[DataSourceProperty]
		public string PollInitiatorName
		{
			get
			{
				return this._pollInitiatorName;
			}
			set
			{
				if (this._pollInitiatorName != value)
				{
					this._pollInitiatorName = value;
					base.OnPropertyChangedWithValue<string>(value, "PollInitiatorName");
				}
			}
		}

		// Token: 0x170001BC RID: 444
		// (get) Token: 0x060005E8 RID: 1512 RVA: 0x00018D02 File Offset: 0x00016F02
		// (set) Token: 0x060005E9 RID: 1513 RVA: 0x00018D0A File Offset: 0x00016F0A
		[DataSourceProperty]
		public string PollDescription
		{
			get
			{
				return this._pollDescription;
			}
			set
			{
				if (this._pollDescription != value)
				{
					this._pollDescription = value;
					base.OnPropertyChangedWithValue<string>(value, "PollDescription");
				}
			}
		}

		// Token: 0x170001BD RID: 445
		// (get) Token: 0x060005EA RID: 1514 RVA: 0x00018D2D File Offset: 0x00016F2D
		// (set) Token: 0x060005EB RID: 1515 RVA: 0x00018D35 File Offset: 0x00016F35
		[DataSourceProperty]
		public MPPlayerVM TargetPlayer
		{
			get
			{
				return this._targetPlayer;
			}
			set
			{
				if (value != this._targetPlayer)
				{
					this._targetPlayer = value;
					base.OnPropertyChangedWithValue<MPPlayerVM>(value, "TargetPlayer");
				}
			}
		}

		// Token: 0x170001BE RID: 446
		// (get) Token: 0x060005EC RID: 1516 RVA: 0x00018D53 File Offset: 0x00016F53
		// (set) Token: 0x060005ED RID: 1517 RVA: 0x00018D5B File Offset: 0x00016F5B
		[DataSourceProperty]
		public MBBindingList<InputKeyItemVM> Keys
		{
			get
			{
				return this._keys;
			}
			set
			{
				if (this._keys != value)
				{
					this._keys = value;
					base.OnPropertyChangedWithValue<MBBindingList<InputKeyItemVM>>(value, "Keys");
				}
			}
		}

		// Token: 0x040002F5 RID: 757
		private static readonly TextObject _kickText = new TextObject("{=gk5dCG1j}kick", null);

		// Token: 0x040002F6 RID: 758
		private static readonly TextObject _banText = new TextObject("{=sFDrUfNR}ban", null);

		// Token: 0x040002F7 RID: 759
		private bool _hasOngoingPoll;

		// Token: 0x040002F8 RID: 760
		private bool _areKeysEnabled;

		// Token: 0x040002F9 RID: 761
		private int _votesAccepted;

		// Token: 0x040002FA RID: 762
		private int _votesRejected;

		// Token: 0x040002FB RID: 763
		private string _pollInitiatorName;

		// Token: 0x040002FC RID: 764
		private string _pollDescription;

		// Token: 0x040002FD RID: 765
		private MPPlayerVM _targetPlayer;

		// Token: 0x040002FE RID: 766
		private MBBindingList<InputKeyItemVM> _keys;
	}
}
