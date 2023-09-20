using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Scoreboard
{
	public class MissionScoreboardStatItemVM : ViewModel
	{
		public MissionScoreboardStatItemVM(MissionScoreboardPlayerVM belongedPlayer, string headerID, string item)
		{
			this.Item = item;
			this.HeaderID = headerID;
			this.BelongedPlayer = belongedPlayer;
		}

		[DataSourceProperty]
		public string Item
		{
			get
			{
				return this._item;
			}
			set
			{
				if (value != this._item)
				{
					this._item = value;
					base.OnPropertyChangedWithValue<string>(value, "Item");
				}
			}
		}

		[DataSourceProperty]
		public string HeaderID
		{
			get
			{
				return this._headerID;
			}
			set
			{
				if (value != this._headerID)
				{
					this._headerID = value;
					base.OnPropertyChangedWithValue<string>(value, "HeaderID");
				}
			}
		}

		[DataSourceProperty]
		public MissionScoreboardPlayerVM BelongedPlayer
		{
			get
			{
				return this._belongedPlayer;
			}
			set
			{
				if (value != this._belongedPlayer)
				{
					this._belongedPlayer = value;
					base.OnPropertyChangedWithValue<MissionScoreboardPlayerVM>(value, "BelongedPlayer");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<MissionScoreboardMVPItemVM> MVPBadges
		{
			get
			{
				return this.BelongedPlayer.MVPBadges;
			}
		}

		private string _item;

		private string _headerID = "";

		private MissionScoreboardPlayerVM _belongedPlayer;
	}
}
