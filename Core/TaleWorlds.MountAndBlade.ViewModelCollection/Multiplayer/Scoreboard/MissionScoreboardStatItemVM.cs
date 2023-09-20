using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Scoreboard
{
	// Token: 0x02000052 RID: 82
	public class MissionScoreboardStatItemVM : ViewModel
	{
		// Token: 0x060006CE RID: 1742 RVA: 0x0001B4BA File Offset: 0x000196BA
		public MissionScoreboardStatItemVM(MissionScoreboardPlayerVM belongedPlayer, string headerID, string item)
		{
			this.Item = item;
			this.HeaderID = headerID;
			this.BelongedPlayer = belongedPlayer;
		}

		// Token: 0x17000210 RID: 528
		// (get) Token: 0x060006CF RID: 1743 RVA: 0x0001B4E2 File Offset: 0x000196E2
		// (set) Token: 0x060006D0 RID: 1744 RVA: 0x0001B4EA File Offset: 0x000196EA
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

		// Token: 0x17000211 RID: 529
		// (get) Token: 0x060006D1 RID: 1745 RVA: 0x0001B50D File Offset: 0x0001970D
		// (set) Token: 0x060006D2 RID: 1746 RVA: 0x0001B515 File Offset: 0x00019715
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

		// Token: 0x17000212 RID: 530
		// (get) Token: 0x060006D3 RID: 1747 RVA: 0x0001B538 File Offset: 0x00019738
		// (set) Token: 0x060006D4 RID: 1748 RVA: 0x0001B540 File Offset: 0x00019740
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

		// Token: 0x17000213 RID: 531
		// (get) Token: 0x060006D5 RID: 1749 RVA: 0x0001B55E File Offset: 0x0001975E
		[DataSourceProperty]
		public MBBindingList<MissionScoreboardMVPItemVM> MVPBadges
		{
			get
			{
				return this.BelongedPlayer.MVPBadges;
			}
		}

		// Token: 0x04000378 RID: 888
		private string _item;

		// Token: 0x04000379 RID: 889
		private string _headerID = "";

		// Token: 0x0400037A RID: 890
		private MissionScoreboardPlayerVM _belongedPlayer;
	}
}
