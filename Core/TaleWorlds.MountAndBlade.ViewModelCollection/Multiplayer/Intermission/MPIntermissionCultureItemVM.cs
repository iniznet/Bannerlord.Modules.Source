using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Intermission
{
	public class MPIntermissionCultureItemVM : MPCultureItemVM
	{
		public MPIntermissionCultureItemVM(string cultureCode, Action<MPIntermissionCultureItemVM> onPlayerVoted)
			: base(cultureCode, null)
		{
			this._onPlayerVoted = onPlayerVoted;
		}

		public void ExecuteVote()
		{
			this._onPlayerVoted(this);
		}

		[DataSourceProperty]
		public int Votes
		{
			get
			{
				return this._votes;
			}
			set
			{
				if (value != this._votes)
				{
					this._votes = value;
					base.OnPropertyChangedWithValue(value, "Votes");
				}
			}
		}

		private readonly Action<MPIntermissionCultureItemVM> _onPlayerVoted;

		private int _votes;
	}
}
