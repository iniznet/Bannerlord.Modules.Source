using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI
{
	internal class DebugStatsVM : ViewModel
	{
		public DebugStatsVM()
		{
			this.GameVersion = ApplicationVersion.FromParametersFile(null).ToString();
		}

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

		private string _gameVersion;
	}
}
