using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Friends
{
	public class MPLobbyPlayerRecentGameDataVM : ViewModel
	{
		public MPLobbyPlayerRecentGameDataVM(int result, string gameType, string map, string date)
		{
			this.Result = result;
			this.GameType = gameType;
			this.Map = map;
			this.Date = date;
		}

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

		private int _result;

		private string _gameType;

		private string _map;

		private string _date;
	}
}
