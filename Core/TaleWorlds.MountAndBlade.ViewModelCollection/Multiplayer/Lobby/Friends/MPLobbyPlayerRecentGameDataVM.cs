using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Friends
{
	// Token: 0x02000087 RID: 135
	public class MPLobbyPlayerRecentGameDataVM : ViewModel
	{
		// Token: 0x06000C9D RID: 3229 RVA: 0x0002C728 File Offset: 0x0002A928
		public MPLobbyPlayerRecentGameDataVM(int result, string gameType, string map, string date)
		{
			this.Result = result;
			this.GameType = gameType;
			this.Map = map;
			this.Date = date;
		}

		// Token: 0x170003F1 RID: 1009
		// (get) Token: 0x06000C9E RID: 3230 RVA: 0x0002C74D File Offset: 0x0002A94D
		// (set) Token: 0x06000C9F RID: 3231 RVA: 0x0002C755 File Offset: 0x0002A955
		[DataSourceProperty]
		public int Result
		{
			get
			{
				return this._result;
			}
			set
			{
				if (value != this._result)
				{
					this._result = value;
					base.OnPropertyChangedWithValue(value, "Result");
				}
			}
		}

		// Token: 0x170003F2 RID: 1010
		// (get) Token: 0x06000CA0 RID: 3232 RVA: 0x0002C773 File Offset: 0x0002A973
		// (set) Token: 0x06000CA1 RID: 3233 RVA: 0x0002C77B File Offset: 0x0002A97B
		[DataSourceProperty]
		public string GameType
		{
			get
			{
				return this._gameType;
			}
			set
			{
				if (value != this._gameType)
				{
					this._gameType = value;
					base.OnPropertyChangedWithValue<string>(value, "GameType");
				}
			}
		}

		// Token: 0x170003F3 RID: 1011
		// (get) Token: 0x06000CA2 RID: 3234 RVA: 0x0002C79E File Offset: 0x0002A99E
		// (set) Token: 0x06000CA3 RID: 3235 RVA: 0x0002C7A6 File Offset: 0x0002A9A6
		[DataSourceProperty]
		public string Map
		{
			get
			{
				return this._map;
			}
			set
			{
				if (value != this._map)
				{
					this._map = value;
					base.OnPropertyChangedWithValue<string>(value, "Map");
				}
			}
		}

		// Token: 0x170003F4 RID: 1012
		// (get) Token: 0x06000CA4 RID: 3236 RVA: 0x0002C7C9 File Offset: 0x0002A9C9
		// (set) Token: 0x06000CA5 RID: 3237 RVA: 0x0002C7D1 File Offset: 0x0002A9D1
		[DataSourceProperty]
		public string Date
		{
			get
			{
				return this._date;
			}
			set
			{
				if (value != this._date)
				{
					this._date = value;
					base.OnPropertyChangedWithValue<string>(value, "Date");
				}
			}
		}

		// Token: 0x04000608 RID: 1544
		private int _result;

		// Token: 0x04000609 RID: 1545
		private string _gameType;

		// Token: 0x0400060A RID: 1546
		private string _map;

		// Token: 0x0400060B RID: 1547
		private string _date;
	}
}
