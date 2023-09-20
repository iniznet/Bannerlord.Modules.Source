using System;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Library;

namespace SandBox.ViewModelCollection.Map
{
	// Token: 0x02000029 RID: 41
	public class MapCheatsVM : ViewModel
	{
		// Token: 0x0600032B RID: 811 RVA: 0x0000F927 File Offset: 0x0000DB27
		public MapCheatsVM()
		{
			this.Cheats = new MBBindingList<StringItemWithActionVM>();
		}

		// Token: 0x0600032C RID: 812 RVA: 0x0000F93A File Offset: 0x0000DB3A
		public void AddCheat(string name, string cheatCode, bool closeOnExecute)
		{
			this.Cheats.Add(new CheatItemVM(name, cheatCode, closeOnExecute, new Action(this.ExecuteDisable)));
		}

		// Token: 0x0600032D RID: 813 RVA: 0x0000F95B File Offset: 0x0000DB5B
		public void ExecuteEnable()
		{
			this.IsEnabled = true;
		}

		// Token: 0x0600032E RID: 814 RVA: 0x0000F964 File Offset: 0x0000DB64
		public void ExecuteDisable()
		{
			this.IsEnabled = false;
		}

		// Token: 0x17000103 RID: 259
		// (get) Token: 0x0600032F RID: 815 RVA: 0x0000F96D File Offset: 0x0000DB6D
		// (set) Token: 0x06000330 RID: 816 RVA: 0x0000F975 File Offset: 0x0000DB75
		[DataSourceProperty]
		public MBBindingList<StringItemWithActionVM> Cheats
		{
			get
			{
				return this._cheats;
			}
			set
			{
				if (value != this._cheats)
				{
					this._cheats = value;
					base.OnPropertyChangedWithValue<MBBindingList<StringItemWithActionVM>>(value, "Cheats");
				}
			}
		}

		// Token: 0x17000104 RID: 260
		// (get) Token: 0x06000331 RID: 817 RVA: 0x0000F993 File Offset: 0x0000DB93
		// (set) Token: 0x06000332 RID: 818 RVA: 0x0000F99B File Offset: 0x0000DB9B
		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (value != this._isEnabled)
				{
					this._isEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEnabled");
				}
			}
		}

		// Token: 0x040001A3 RID: 419
		private bool _isEnabled;

		// Token: 0x040001A4 RID: 420
		private MBBindingList<StringItemWithActionVM> _cheats;
	}
}
