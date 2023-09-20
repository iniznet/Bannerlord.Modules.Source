using System;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Scoreboard
{
	public class MissionScoreboardHeaderItemVM : BindingListStringItem
	{
		public MissionScoreboardHeaderItemVM(MissionScoreboardSideVM side, string headerID, string value, bool isAvatarStat, bool isIrregularStat)
			: base(value)
		{
			this._side = side;
			this.HeaderID = headerID;
			this.IsAvatarStat = isAvatarStat;
			this.IsIrregularStat = isIrregularStat;
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
		public bool IsIrregularStat
		{
			get
			{
				return this._isIrregularStat;
			}
			set
			{
				if (value != this._isIrregularStat)
				{
					this._isIrregularStat = value;
					base.OnPropertyChangedWithValue(value, "IsIrregularStat");
				}
			}
		}

		[DataSourceProperty]
		public bool IsAvatarStat
		{
			get
			{
				return this._isAvatarStat;
			}
			set
			{
				if (value != this._isAvatarStat)
				{
					this._isAvatarStat = value;
					base.OnPropertyChangedWithValue(value, "IsAvatarStat");
				}
			}
		}

		[DataSourceProperty]
		public MissionScoreboardPlayerSortControllerVM PlayerSortController
		{
			get
			{
				return this._side.PlayerSortController;
			}
		}

		private readonly MissionScoreboardSideVM _side;

		private string _headerID = "";

		private bool _isIrregularStat;

		private bool _isAvatarStat;
	}
}
