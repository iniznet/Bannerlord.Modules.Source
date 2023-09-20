using System;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.OfficialGame
{
	public class MPMatchmakingRegionSelectorItemVM : SelectorItemVM
	{
		public string RegionCode { get; private set; }

		public MPMatchmakingRegionSelectorItemVM(string regionCode, TextObject regionName)
			: base(regionName)
		{
			this.RegionCode = regionCode;
			this.IsRegionNone = regionCode == "None";
		}

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

		private bool _isRegionNone;
	}
}
