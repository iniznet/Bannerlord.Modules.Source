using System;
using TaleWorlds.Library;

namespace SandBox.ViewModelCollection.SaveLoad
{
	// Token: 0x0200000E RID: 14
	public class SavedGameGroupVM : ViewModel
	{
		// Token: 0x0600012C RID: 300 RVA: 0x0000773C File Offset: 0x0000593C
		public SavedGameGroupVM()
		{
			this.SavedGamesList = new MBBindingList<SavedGameVM>();
		}

		// Token: 0x0600012D RID: 301 RVA: 0x0000774F File Offset: 0x0000594F
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.SavedGamesList.ApplyActionOnAllItems(delegate(SavedGameVM s)
			{
				s.RefreshValues();
			});
		}

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x0600012E RID: 302 RVA: 0x00007781 File Offset: 0x00005981
		// (set) Token: 0x0600012F RID: 303 RVA: 0x00007789 File Offset: 0x00005989
		[DataSourceProperty]
		public bool IsFilteredOut
		{
			get
			{
				return this._isFilteredOut;
			}
			set
			{
				if (value != this._isFilteredOut)
				{
					this._isFilteredOut = value;
					base.OnPropertyChangedWithValue(value, "IsFilteredOut");
				}
			}
		}

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x06000130 RID: 304 RVA: 0x000077A7 File Offset: 0x000059A7
		// (set) Token: 0x06000131 RID: 305 RVA: 0x000077AF File Offset: 0x000059AF
		[DataSourceProperty]
		public MBBindingList<SavedGameVM> SavedGamesList
		{
			get
			{
				return this._savedGamesList;
			}
			set
			{
				if (value != this._savedGamesList)
				{
					this._savedGamesList = value;
					base.OnPropertyChangedWithValue<MBBindingList<SavedGameVM>>(value, "SavedGamesList");
				}
			}
		}

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x06000132 RID: 306 RVA: 0x000077CD File Offset: 0x000059CD
		// (set) Token: 0x06000133 RID: 307 RVA: 0x000077D5 File Offset: 0x000059D5
		[DataSourceProperty]
		public string IdentifierID
		{
			get
			{
				return this._identifierID;
			}
			set
			{
				if (value != this._identifierID)
				{
					this._identifierID = value;
					base.OnPropertyChangedWithValue<string>(value, "IdentifierID");
				}
			}
		}

		// Token: 0x04000074 RID: 116
		private bool _isFilteredOut;

		// Token: 0x04000075 RID: 117
		private MBBindingList<SavedGameVM> _savedGamesList;

		// Token: 0x04000076 RID: 118
		private string _identifierID;
	}
}
