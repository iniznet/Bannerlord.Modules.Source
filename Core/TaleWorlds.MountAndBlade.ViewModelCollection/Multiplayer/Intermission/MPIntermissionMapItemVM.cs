using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Intermission
{
	// Token: 0x020000B5 RID: 181
	public class MPIntermissionMapItemVM : ViewModel
	{
		// Token: 0x060010FB RID: 4347 RVA: 0x00037FB9 File Offset: 0x000361B9
		public MPIntermissionMapItemVM(string mapID, Action<MPIntermissionMapItemVM> onPlayerVoted)
		{
			this.MapID = mapID;
			this._onPlayerVoted = onPlayerVoted;
			this.RefreshValues();
		}

		// Token: 0x060010FC RID: 4348 RVA: 0x00037FD5 File Offset: 0x000361D5
		public override void RefreshValues()
		{
			this.MapName = GameTexts.FindText("str_multiplayer_scene_name", this.MapID).ToString();
		}

		// Token: 0x060010FD RID: 4349 RVA: 0x00037FF2 File Offset: 0x000361F2
		public void ExecuteVote()
		{
			this._onPlayerVoted(this);
		}

		// Token: 0x17000578 RID: 1400
		// (get) Token: 0x060010FE RID: 4350 RVA: 0x00038000 File Offset: 0x00036200
		// (set) Token: 0x060010FF RID: 4351 RVA: 0x00038008 File Offset: 0x00036208
		[DataSourceProperty]
		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				if (value != this._isSelected)
				{
					this._isSelected = value;
					base.OnPropertyChangedWithValue(value, "IsSelected");
				}
			}
		}

		// Token: 0x17000579 RID: 1401
		// (get) Token: 0x06001100 RID: 4352 RVA: 0x00038026 File Offset: 0x00036226
		// (set) Token: 0x06001101 RID: 4353 RVA: 0x0003802E File Offset: 0x0003622E
		[DataSourceProperty]
		public string MapID
		{
			get
			{
				return this._mapID;
			}
			set
			{
				if (value != this._mapID)
				{
					this._mapID = value;
					base.OnPropertyChangedWithValue<string>(value, "MapID");
				}
			}
		}

		// Token: 0x1700057A RID: 1402
		// (get) Token: 0x06001102 RID: 4354 RVA: 0x00038051 File Offset: 0x00036251
		// (set) Token: 0x06001103 RID: 4355 RVA: 0x00038059 File Offset: 0x00036259
		[DataSourceProperty]
		public string MapName
		{
			get
			{
				return this._mapName;
			}
			set
			{
				if (value != this._mapName)
				{
					this._mapName = value;
					base.OnPropertyChangedWithValue<string>(value, "MapName");
				}
			}
		}

		// Token: 0x1700057B RID: 1403
		// (get) Token: 0x06001104 RID: 4356 RVA: 0x0003807C File Offset: 0x0003627C
		// (set) Token: 0x06001105 RID: 4357 RVA: 0x00038084 File Offset: 0x00036284
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

		// Token: 0x0400080E RID: 2062
		private readonly Action<MPIntermissionMapItemVM> _onPlayerVoted;

		// Token: 0x0400080F RID: 2063
		private bool _isSelected;

		// Token: 0x04000810 RID: 2064
		private string _mapID;

		// Token: 0x04000811 RID: 2065
		private string _mapName;

		// Token: 0x04000812 RID: 2066
		private int _votes;
	}
}
