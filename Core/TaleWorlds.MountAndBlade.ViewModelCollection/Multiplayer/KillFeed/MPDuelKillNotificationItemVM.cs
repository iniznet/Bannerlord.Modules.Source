using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.ClassLoadout;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.KillFeed
{
	// Token: 0x020000AE RID: 174
	public class MPDuelKillNotificationItemVM : ViewModel
	{
		// Token: 0x06001099 RID: 4249 RVA: 0x00037070 File Offset: 0x00035270
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

		// Token: 0x0600109A RID: 4250 RVA: 0x000370D0 File Offset: 0x000352D0
		public void InitProperties(MissionPeer firstPlayerPeer, MissionPeer secondPlayerPeer)
		{
			TargetIconType peerIconType = this.GetPeerIconType(firstPlayerPeer);
			this.FirstPlayerName = firstPlayerPeer.DisplayedName;
			this.FirstPlayerCompassElement = new MPTeammateCompassTargetVM(peerIconType, Color.White.ToUnsignedInteger(), Color.White.ToUnsignedInteger(), BannerCode.CreateFrom(Banner.CreateOneColoredEmptyBanner(0)), false);
			TargetIconType peerIconType2 = this.GetPeerIconType(secondPlayerPeer);
			this.SecondPlayerName = secondPlayerPeer.DisplayedName;
			this.SecondPlayerCompassElement = new MPTeammateCompassTargetVM(peerIconType2, Color.White.ToUnsignedInteger(), Color.White.ToUnsignedInteger(), BannerCode.CreateFrom(Banner.CreateOneColoredEmptyBanner(0)), false);
		}

		// Token: 0x0600109B RID: 4251 RVA: 0x00037169 File Offset: 0x00035369
		private TargetIconType GetPeerIconType(MissionPeer peer)
		{
			return MultiplayerClassDivisions.GetMPHeroClassForPeer(peer, false).IconType;
		}

		// Token: 0x0600109C RID: 4252 RVA: 0x00037177 File Offset: 0x00035377
		public void ExecuteRemove()
		{
			this._onRemove(this);
		}

		// Token: 0x17000556 RID: 1366
		// (get) Token: 0x0600109D RID: 4253 RVA: 0x00037185 File Offset: 0x00035385
		// (set) Token: 0x0600109E RID: 4254 RVA: 0x0003718D File Offset: 0x0003538D
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

		// Token: 0x17000557 RID: 1367
		// (get) Token: 0x0600109F RID: 4255 RVA: 0x000371AB File Offset: 0x000353AB
		// (set) Token: 0x060010A0 RID: 4256 RVA: 0x000371B3 File Offset: 0x000353B3
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

		// Token: 0x17000558 RID: 1368
		// (get) Token: 0x060010A1 RID: 4257 RVA: 0x000371D1 File Offset: 0x000353D1
		// (set) Token: 0x060010A2 RID: 4258 RVA: 0x000371D9 File Offset: 0x000353D9
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

		// Token: 0x17000559 RID: 1369
		// (get) Token: 0x060010A3 RID: 4259 RVA: 0x000371F7 File Offset: 0x000353F7
		// (set) Token: 0x060010A4 RID: 4260 RVA: 0x000371FF File Offset: 0x000353FF
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

		// Token: 0x1700055A RID: 1370
		// (get) Token: 0x060010A5 RID: 4261 RVA: 0x0003721D File Offset: 0x0003541D
		// (set) Token: 0x060010A6 RID: 4262 RVA: 0x00037225 File Offset: 0x00035425
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

		// Token: 0x1700055B RID: 1371
		// (get) Token: 0x060010A7 RID: 4263 RVA: 0x00037248 File Offset: 0x00035448
		// (set) Token: 0x060010A8 RID: 4264 RVA: 0x00037250 File Offset: 0x00035450
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

		// Token: 0x1700055C RID: 1372
		// (get) Token: 0x060010A9 RID: 4265 RVA: 0x00037273 File Offset: 0x00035473
		// (set) Token: 0x060010AA RID: 4266 RVA: 0x0003727B File Offset: 0x0003547B
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

		// Token: 0x1700055D RID: 1373
		// (get) Token: 0x060010AB RID: 4267 RVA: 0x00037299 File Offset: 0x00035499
		// (set) Token: 0x060010AC RID: 4268 RVA: 0x000372A1 File Offset: 0x000354A1
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

		// Token: 0x040007E7 RID: 2023
		private Action<MPDuelKillNotificationItemVM> _onRemove;

		// Token: 0x040007E8 RID: 2024
		private bool _isEndOfDuel;

		// Token: 0x040007E9 RID: 2025
		private int _arenaType;

		// Token: 0x040007EA RID: 2026
		private int _firstPlayerScore;

		// Token: 0x040007EB RID: 2027
		private int _secondPlayerScore;

		// Token: 0x040007EC RID: 2028
		private string _firstPlayerName;

		// Token: 0x040007ED RID: 2029
		private string _secondPlayerName;

		// Token: 0x040007EE RID: 2030
		private MPTeammateCompassTargetVM _firstPlayerCompassElement;

		// Token: 0x040007EF RID: 2031
		private MPTeammateCompassTargetVM _secondPlayerCompassElement;
	}
}
