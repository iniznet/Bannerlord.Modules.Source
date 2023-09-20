using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Diamond;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Scoreboard
{
	public class MissionScoreboardPlayerVM : MPPlayerVM
	{
		public int Score { get; private set; }

		public bool IsBot { get; private set; }

		public bool IsMine
		{
			get
			{
				return this._lobbyPeer != null && this._lobbyPeer.IsMine;
			}
		}

		public bool IsTeammate
		{
			get
			{
				return this._lobbyPeer != null && this._lobbyPeer.Team.IsPlayerTeam;
			}
		}

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

		public void Tick(float dt)
		{
			if (!this.IsBot)
			{
				base.IsDead = this._lobbyPeer == null || !this._lobbyPeer.IsControlledAgentActive;
			}
		}

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

		public void ExecuteSelection()
		{
			Action<MissionScoreboardPlayerVM> executeActivate = this._executeActivate;
			if (executeActivate == null)
			{
				return;
			}
			executeActivate(this);
		}

		public void UpdateIsMuted()
		{
			bool flag = PermaMuteList.IsPlayerMuted(this._lobbyPeer.Peer.Id);
			this.IsTextMuted = flag || this._chatBox.IsPlayerMuted(this._lobbyPeer.Peer.Id);
			this.IsVoiceMuted = flag || base.Peer.IsMutedFromGameOrPlatform;
		}

		public void SetMVPBadgeCount(int badgeCount)
		{
			this.MVPBadges.Clear();
			for (int i = 0; i < badgeCount; i++)
			{
				this.MVPBadges.Add(new MissionScoreboardMVPItemVM());
			}
		}

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

		private const string BadgeHeaderID = "badge";

		private readonly MissionPeer _lobbyPeer;

		private readonly Action<MissionScoreboardPlayerVM> _executeActivate;

		private readonly ChatBox _chatBox;

		private int _ping;

		private bool _isPlayer;

		private bool _isVoiceMuted;

		private bool _isTextMuted;

		private MBBindingList<MissionScoreboardStatItemVM> _stats;

		private MBBindingList<MissionScoreboardMVPItemVM> _mvpBadges;
	}
}
