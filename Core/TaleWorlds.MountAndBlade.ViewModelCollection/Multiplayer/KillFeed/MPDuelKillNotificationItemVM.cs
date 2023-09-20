using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.ClassLoadout;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.KillFeed
{
	public class MPDuelKillNotificationItemVM : ViewModel
	{
		public MPDuelKillNotificationItemVM(MissionPeer firstPlayerPeer, MissionPeer secondPlayerPeer, int firstPlayerScore, int secondPlayerScore, TroopType arenaTroopType, Action<MPDuelKillNotificationItemVM> onRemove)
		{
			this._onRemove = onRemove;
			this.ArenaType = (int)arenaTroopType;
			this.FirstPlayerScore = firstPlayerScore;
			this.SecondPlayerScore = secondPlayerScore;
			int intValue = MultiplayerOptions.OptionType.MinScoreToWinDuel.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
			this.IsEndOfDuel = this.FirstPlayerScore == intValue || this.SecondPlayerScore == intValue;
			this.InitProperties(firstPlayerPeer, secondPlayerPeer);
		}

		public void InitProperties(MissionPeer firstPlayerPeer, MissionPeer secondPlayerPeer)
		{
			TargetIconType peerIconType = this.GetPeerIconType(firstPlayerPeer);
			this.FirstPlayerName = firstPlayerPeer.DisplayedName;
			this.FirstPlayerCompassElement = new MPTeammateCompassTargetVM(peerIconType, Color.White.ToUnsignedInteger(), Color.White.ToUnsignedInteger(), BannerCode.CreateFrom(Banner.CreateOneColoredEmptyBanner(0)), false);
			TargetIconType peerIconType2 = this.GetPeerIconType(secondPlayerPeer);
			this.SecondPlayerName = secondPlayerPeer.DisplayedName;
			this.SecondPlayerCompassElement = new MPTeammateCompassTargetVM(peerIconType2, Color.White.ToUnsignedInteger(), Color.White.ToUnsignedInteger(), BannerCode.CreateFrom(Banner.CreateOneColoredEmptyBanner(0)), false);
		}

		private TargetIconType GetPeerIconType(MissionPeer peer)
		{
			return MultiplayerClassDivisions.GetMPHeroClassForPeer(peer, false).IconType;
		}

		public void ExecuteRemove()
		{
			this._onRemove(this);
		}

		[DataSourceProperty]
		public bool IsEndOfDuel
		{
			get
			{
				return this._isEndOfDuel;
			}
			set
			{
				if (value != this._isEndOfDuel)
				{
					this._isEndOfDuel = value;
					base.OnPropertyChangedWithValue(value, "IsEndOfDuel");
				}
			}
		}

		[DataSourceProperty]
		public int ArenaType
		{
			get
			{
				return this._arenaType;
			}
			set
			{
				if (value != this._arenaType)
				{
					this._arenaType = value;
					base.OnPropertyChangedWithValue(value, "ArenaType");
				}
			}
		}

		[DataSourceProperty]
		public int FirstPlayerScore
		{
			get
			{
				return this._firstPlayerScore;
			}
			set
			{
				if (value != this._firstPlayerScore)
				{
					this._firstPlayerScore = value;
					base.OnPropertyChangedWithValue(value, "FirstPlayerScore");
				}
			}
		}

		[DataSourceProperty]
		public int SecondPlayerScore
		{
			get
			{
				return this._secondPlayerScore;
			}
			set
			{
				if (value != this._secondPlayerScore)
				{
					this._secondPlayerScore = value;
					base.OnPropertyChangedWithValue(value, "SecondPlayerScore");
				}
			}
		}

		[DataSourceProperty]
		public string FirstPlayerName
		{
			get
			{
				return this._firstPlayerName;
			}
			set
			{
				if (value != this._firstPlayerName)
				{
					this._firstPlayerName = value;
					base.OnPropertyChangedWithValue<string>(value, "FirstPlayerName");
				}
			}
		}

		[DataSourceProperty]
		public string SecondPlayerName
		{
			get
			{
				return this._secondPlayerName;
			}
			set
			{
				if (value != this._secondPlayerName)
				{
					this._secondPlayerName = value;
					base.OnPropertyChangedWithValue<string>(value, "SecondPlayerName");
				}
			}
		}

		[DataSourceProperty]
		public MPTeammateCompassTargetVM FirstPlayerCompassElement
		{
			get
			{
				return this._firstPlayerCompassElement;
			}
			set
			{
				if (value != this._firstPlayerCompassElement)
				{
					this._firstPlayerCompassElement = value;
					base.OnPropertyChangedWithValue<MPTeammateCompassTargetVM>(value, "FirstPlayerCompassElement");
				}
			}
		}

		[DataSourceProperty]
		public MPTeammateCompassTargetVM SecondPlayerCompassElement
		{
			get
			{
				return this._secondPlayerCompassElement;
			}
			set
			{
				if (value != this._secondPlayerCompassElement)
				{
					this._secondPlayerCompassElement = value;
					base.OnPropertyChangedWithValue<MPTeammateCompassTargetVM>(value, "SecondPlayerCompassElement");
				}
			}
		}

		private Action<MPDuelKillNotificationItemVM> _onRemove;

		private bool _isEndOfDuel;

		private int _arenaType;

		private int _firstPlayerScore;

		private int _secondPlayerScore;

		private string _firstPlayerName;

		private string _secondPlayerName;

		private MPTeammateCompassTargetVM _firstPlayerCompassElement;

		private MPTeammateCompassTargetVM _secondPlayerCompassElement;
	}
}
