using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Intermission
{
	// Token: 0x020000B4 RID: 180
	public class MPIntermissionCultureItemVM : MPCultureItemVM
	{
		// Token: 0x060010F7 RID: 4343 RVA: 0x00037F74 File Offset: 0x00036174
		public MPIntermissionCultureItemVM(string cultureCode, Action<MPIntermissionCultureItemVM> onPlayerVoted)
			: base(cultureCode, null)
		{
			this._onPlayerVoted = onPlayerVoted;
		}

		// Token: 0x060010F8 RID: 4344 RVA: 0x00037F85 File Offset: 0x00036185
		public void ExecuteVote()
		{
			this._onPlayerVoted(this);
		}

		// Token: 0x17000577 RID: 1399
		// (get) Token: 0x060010F9 RID: 4345 RVA: 0x00037F93 File Offset: 0x00036193
		// (set) Token: 0x060010FA RID: 4346 RVA: 0x00037F9B File Offset: 0x0003619B
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

		// Token: 0x0400080C RID: 2060
		private readonly Action<MPIntermissionCultureItemVM> _onPlayerVoted;

		// Token: 0x0400080D RID: 2061
		private int _votes;
	}
}
