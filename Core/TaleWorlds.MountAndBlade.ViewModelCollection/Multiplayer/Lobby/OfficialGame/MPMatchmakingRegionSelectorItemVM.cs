using System;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.OfficialGame
{
	// Token: 0x02000073 RID: 115
	public class MPMatchmakingRegionSelectorItemVM : SelectorItemVM
	{
		// Token: 0x1700034D RID: 845
		// (get) Token: 0x06000A9F RID: 2719 RVA: 0x00026056 File Offset: 0x00024256
		// (set) Token: 0x06000AA0 RID: 2720 RVA: 0x0002605E File Offset: 0x0002425E
		public string RegionCode { get; private set; }

		// Token: 0x06000AA1 RID: 2721 RVA: 0x00026067 File Offset: 0x00024267
		public MPMatchmakingRegionSelectorItemVM(string regionCode, TextObject regionName)
			: base(regionName)
		{
			this.RegionCode = regionCode;
			this.IsRegionNone = regionCode == "None";
		}

		// Token: 0x1700034E RID: 846
		// (get) Token: 0x06000AA2 RID: 2722 RVA: 0x00026088 File Offset: 0x00024288
		// (set) Token: 0x06000AA3 RID: 2723 RVA: 0x00026090 File Offset: 0x00024290
		[DataSourceProperty]
		public bool IsRegionNone
		{
			get
			{
				return this._isRegionNone;
			}
			set
			{
				if (value != this._isRegionNone)
				{
					this._isRegionNone = value;
					base.OnPropertyChangedWithValue(value, "IsRegionNone");
				}
			}
		}

		// Token: 0x04000525 RID: 1317
		private bool _isRegionNone;
	}
}
