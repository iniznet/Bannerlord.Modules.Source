using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Diamond;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Scoreboard
{
	// Token: 0x02000050 RID: 80
	public class MissionScoreboardPlayerVM : MPPlayerVM
	{
		// Token: 0x170001F9 RID: 505
		// (get) Token: 0x0600068E RID: 1678 RVA: 0x0001A968 File Offset: 0x00018B68
		// (set) Token: 0x0600068F RID: 1679 RVA: 0x0001A970 File Offset: 0x00018B70
		public int Score { get; private set; }

		// Token: 0x170001FA RID: 506
		// (get) Token: 0x06000690 RID: 1680 RVA: 0x0001A979 File Offset: 0x00018B79
		// (set) Token: 0x06000691 RID: 1681 RVA: 0x0001A981 File Offset: 0x00018B81
		public bool IsBot { get; private set; }

		// Token: 0x170001FB RID: 507
		// (get) Token: 0x06000692 RID: 1682 RVA: 0x0001A98A File Offset: 0x00018B8A
		public bool IsMine
		{
			get
			{
				return this._lobbyPeer != null && this._lobbyPeer.IsMine;
			}
		}

		// Token: 0x170001FC RID: 508
		// (get) Token: 0x06000693 RID: 1683 RVA: 0x0001A9A1 File Offset: 0x00018BA1
		public bool IsTeammate
		{
			get
			{
				return this._lobbyPeer != null && this._lobbyPeer.Team.IsPlayerTeam;
			}
		}

		// Token: 0x06000694 RID: 1684 RVA: 0x0001A9C0 File Offset: 0x00018BC0
		public MissionScoreboardPlayerVM(MissionPeer peer, string[] attributes, string[] headerIDs, int score, Action<MissionScoreboardPlayerVM> executeActivate)
			: base(peer)
		{
			this._chatBox = Game.Current.GetGameHandler<ChatBox>();
			this._executeActivate = executeActivate;
			this._lobbyPeer = peer;
			this.Stats = new MBBindingList<MissionScoreboardStatItemVM>();
			for (int i = 0; i < attributes.Length; i++)
			{
				this.Stats.Add(new MissionScoreboardStatItemVM(this, headerIDs[i], ""));
			}
			this.UpdateAttributes(attributes, score);
			this.IsPlayer = this.IsMine;
			this.MVPBadges = new MBBindingList<MissionScoreboardMVPItemVM>();
			base.Peer.SetMuted(PermaMuteList.IsPlayerMuted(peer.Peer.Id));
			this.UpdateIsMuted();
		}

		// Token: 0x06000695 RID: 1685 RVA: 0x0001AA68 File Offset: 0x00018C68
		public MissionScoreboardPlayerVM(string[] attributes, string[] headerIDs, int score, Action<MissionScoreboardPlayerVM> executeActivate)
			: base(null)
		{
			this._executeActivate = executeActivate;
			this.Stats = new MBBindingList<MissionScoreboardStatItemVM>();
			for (int i = 0; i < attributes.Length; i++)
			{
				this.Stats.Add(new MissionScoreboardStatItemVM(this, headerIDs[i], ""));
			}
			this.UpdateAttributes(attributes, score);
			this.IsBot = true;
			this.IsPlayer = false;
			base.IsDead = false;
		}

		// Token: 0x06000696 RID: 1686 RVA: 0x0001AAD3 File Offset: 0x00018CD3
		public void Tick(float dt)
		{
			if (!this.IsBot)
			{
				base.IsDead = this._lobbyPeer == null || !this._lobbyPeer.IsControlledAgentActive;
			}
		}

		// Token: 0x06000697 RID: 1687 RVA: 0x0001AAFC File Offset: 0x00018CFC
		public void UpdateAttributes(string[] attributes, int score)
		{
			if (this.Stats.Count == attributes.Length)
			{
				for (int i = 0; i < attributes.Length; i++)
				{
					this.Stats[i].Item = attributes[i] ?? string.Empty;
				}
			}
			this.Score = score;
		}

		// Token: 0x06000698 RID: 1688 RVA: 0x0001AB4B File Offset: 0x00018D4B
		public void ExecuteSelection()
		{
			Action<MissionScoreboardPlayerVM> executeActivate = this._executeActivate;
			if (executeActivate == null)
			{
				return;
			}
			executeActivate(this);
		}

		// Token: 0x06000699 RID: 1689 RVA: 0x0001AB60 File Offset: 0x00018D60
		public void UpdateIsMuted()
		{
			bool flag = PermaMuteList.IsPlayerMuted(this._lobbyPeer.Peer.Id);
			this.IsTextMuted = flag || this._chatBox.IsPlayerMuted(this._lobbyPeer.Peer.Id);
			this.IsVoiceMuted = flag || base.Peer.IsMutedFromGameOrPlatform;
		}

		// Token: 0x0600069A RID: 1690 RVA: 0x0001ABC4 File Offset: 0x00018DC4
		public void SetMVPBadgeCount(int badgeCount)
		{
			this.MVPBadges.Clear();
			for (int i = 0; i < badgeCount; i++)
			{
				this.MVPBadges.Add(new MissionScoreboardMVPItemVM());
			}
		}

		// Token: 0x170001FD RID: 509
		// (get) Token: 0x0600069B RID: 1691 RVA: 0x0001ABF8 File Offset: 0x00018DF8
		// (set) Token: 0x0600069C RID: 1692 RVA: 0x0001AC00 File Offset: 0x00018E00
		[DataSourceProperty]
		public int Ping
		{
			get
			{
				return this._ping;
			}
			set
			{
				if (value != this._ping)
				{
					this._ping = value;
					base.OnPropertyChangedWithValue(value, "Ping");
				}
			}
		}

		// Token: 0x170001FE RID: 510
		// (get) Token: 0x0600069D RID: 1693 RVA: 0x0001AC1E File Offset: 0x00018E1E
		// (set) Token: 0x0600069E RID: 1694 RVA: 0x0001AC26 File Offset: 0x00018E26
		[DataSourceProperty]
		public bool IsPlayer
		{
			get
			{
				return this._isPlayer;
			}
			set
			{
				if (value != this._isPlayer)
				{
					this._isPlayer = value;
					base.OnPropertyChangedWithValue(value, "IsPlayer");
				}
			}
		}

		// Token: 0x170001FF RID: 511
		// (get) Token: 0x0600069F RID: 1695 RVA: 0x0001AC44 File Offset: 0x00018E44
		// (set) Token: 0x060006A0 RID: 1696 RVA: 0x0001AC4C File Offset: 0x00018E4C
		[DataSourceProperty]
		public bool IsVoiceMuted
		{
			get
			{
				return this._isVoiceMuted;
			}
			set
			{
				if (value != this._isVoiceMuted)
				{
					this._isVoiceMuted = value;
					base.OnPropertyChangedWithValue(value, "IsVoiceMuted");
				}
			}
		}

		// Token: 0x17000200 RID: 512
		// (get) Token: 0x060006A1 RID: 1697 RVA: 0x0001AC6A File Offset: 0x00018E6A
		// (set) Token: 0x060006A2 RID: 1698 RVA: 0x0001AC72 File Offset: 0x00018E72
		[DataSourceProperty]
		public bool IsTextMuted
		{
			get
			{
				return this._isTextMuted;
			}
			set
			{
				if (value != this._isTextMuted)
				{
					this._isTextMuted = value;
					base.OnPropertyChangedWithValue(value, "IsTextMuted");
				}
			}
		}

		// Token: 0x17000201 RID: 513
		// (get) Token: 0x060006A3 RID: 1699 RVA: 0x0001AC90 File Offset: 0x00018E90
		// (set) Token: 0x060006A4 RID: 1700 RVA: 0x0001AC98 File Offset: 0x00018E98
		[DataSourceProperty]
		public MBBindingList<MissionScoreboardStatItemVM> Stats
		{
			get
			{
				return this._stats;
			}
			set
			{
				if (value != this._stats)
				{
					this._stats = value;
					base.OnPropertyChangedWithValue<MBBindingList<MissionScoreboardStatItemVM>>(value, "Stats");
				}
			}
		}

		// Token: 0x17000202 RID: 514
		// (get) Token: 0x060006A5 RID: 1701 RVA: 0x0001ACB6 File Offset: 0x00018EB6
		// (set) Token: 0x060006A6 RID: 1702 RVA: 0x0001ACBE File Offset: 0x00018EBE
		[DataSourceProperty]
		public MBBindingList<MissionScoreboardMVPItemVM> MVPBadges
		{
			get
			{
				return this._mvpBadges;
			}
			set
			{
				if (value != this._mvpBadges)
				{
					this._mvpBadges = value;
					base.OnPropertyChangedWithValue<MBBindingList<MissionScoreboardMVPItemVM>>(value, "MVPBadges");
				}
			}
		}

		// Token: 0x04000358 RID: 856
		private const string BadgeHeaderID = "badge";

		// Token: 0x0400035B RID: 859
		private readonly MissionPeer _lobbyPeer;

		// Token: 0x0400035C RID: 860
		private readonly Action<MissionScoreboardPlayerVM> _executeActivate;

		// Token: 0x0400035D RID: 861
		private readonly ChatBox _chatBox;

		// Token: 0x0400035E RID: 862
		private int _ping;

		// Token: 0x0400035F RID: 863
		private bool _isPlayer;

		// Token: 0x04000360 RID: 864
		private bool _isVoiceMuted;

		// Token: 0x04000361 RID: 865
		private bool _isTextMuted;

		// Token: 0x04000362 RID: 866
		private MBBindingList<MissionScoreboardStatItemVM> _stats;

		// Token: 0x04000363 RID: 867
		private MBBindingList<MissionScoreboardMVPItemVM> _mvpBadges;
	}
}
