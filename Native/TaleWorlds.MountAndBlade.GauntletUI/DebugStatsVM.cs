using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI
{
	// Token: 0x02000006 RID: 6
	internal class DebugStatsVM : ViewModel
	{
		// Token: 0x06000020 RID: 32 RVA: 0x00002C54 File Offset: 0x00000E54
		public DebugStatsVM()
		{
			this.GameVersion = ApplicationVersion.FromParametersFile(null).ToString();
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000021 RID: 33 RVA: 0x00002C81 File Offset: 0x00000E81
		// (set) Token: 0x06000022 RID: 34 RVA: 0x00002C89 File Offset: 0x00000E89
		[DataSourceProperty]
		public string GameVersion
		{
			get
			{
				return this._gameVersion;
			}
			set
			{
				if (value != this._gameVersion)
				{
					this._gameVersion = value;
					base.OnPropertyChangedWithValue<string>(value, "GameVersion");
				}
			}
		}

		// Token: 0x04000013 RID: 19
		private string _gameVersion;
	}
}
