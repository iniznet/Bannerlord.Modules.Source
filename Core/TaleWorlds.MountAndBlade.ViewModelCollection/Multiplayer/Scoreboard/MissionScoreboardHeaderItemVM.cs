using System;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Scoreboard
{
	// Token: 0x0200004D RID: 77
	public class MissionScoreboardHeaderItemVM : BindingListStringItem
	{
		// Token: 0x06000665 RID: 1637 RVA: 0x0001A387 File Offset: 0x00018587
		public MissionScoreboardHeaderItemVM(MissionScoreboardSideVM side, string headerID, string value, bool isAvatarStat, bool isIrregularStat)
			: base(value)
		{
			this._side = side;
			this.HeaderID = headerID;
			this.IsAvatarStat = isAvatarStat;
			this.IsIrregularStat = isIrregularStat;
		}

		// Token: 0x170001E9 RID: 489
		// (get) Token: 0x06000666 RID: 1638 RVA: 0x0001A3B9 File Offset: 0x000185B9
		// (set) Token: 0x06000667 RID: 1639 RVA: 0x0001A3C1 File Offset: 0x000185C1
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

		// Token: 0x170001EA RID: 490
		// (get) Token: 0x06000668 RID: 1640 RVA: 0x0001A3E4 File Offset: 0x000185E4
		// (set) Token: 0x06000669 RID: 1641 RVA: 0x0001A3EC File Offset: 0x000185EC
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

		// Token: 0x170001EB RID: 491
		// (get) Token: 0x0600066A RID: 1642 RVA: 0x0001A40A File Offset: 0x0001860A
		// (set) Token: 0x0600066B RID: 1643 RVA: 0x0001A412 File Offset: 0x00018612
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

		// Token: 0x170001EC RID: 492
		// (get) Token: 0x0600066C RID: 1644 RVA: 0x0001A430 File Offset: 0x00018630
		[DataSourceProperty]
		public MissionScoreboardPlayerSortControllerVM PlayerSortController
		{
			get
			{
				return this._side.PlayerSortController;
			}
		}

		// Token: 0x0400033F RID: 831
		private readonly MissionScoreboardSideVM _side;

		// Token: 0x04000340 RID: 832
		private string _headerID = "";

		// Token: 0x04000341 RID: 833
		private bool _isIrregularStat;

		// Token: 0x04000342 RID: 834
		private bool _isAvatarStat;
	}
}
